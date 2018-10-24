using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace YOpenGL
{
    /// <summary>
    /// The positive direction of the X axis is right(→) and the positive direction of the Y axis is up(↑);
    /// </summary>
    public class GLPanel : HwndHost, IDisposable
    {
        private static readonly string[] _shaders_normal = new string[] { "Internel.vert", "Internel.frag" };
        private static readonly string[] _shaders_dashed = new string[] { "Internel.vert", "Internel_Dash.geo", "Internel_Dash.frag" };

        public GLPanel(PointF origin, Color color, float frameRate = 60)
        {
            Origin = origin;
            Color = color;
            _frameSpan = Math.Max(1, (int)(1000 / frameRate));
            _isInit = false;
            _isDisposed = false;
            _view = new MatrixF();
            _viewResverse = new MatrixF();
            _visuals = new List<GLVisual>();
            _fillModels = new Dictionary<Color, List<MeshModel>>();
            _lineModels = new Dictionary<PenF, List<MeshModel>>();
            _shaders = new List<Shader>();

            _timer = new Timer(_AfterPainted);
        }

        #region Private Field
        private bool _isDisposed;
        private bool _isInit;
        private bool _needUpdate;
        private int _signal;
        private int _frameSpan;
        private Shader _shader_normal;
        private Shader _shader_dashed;
        private List<Shader> _shaders;

        private MatrixF _transformToDevice;
        private MatrixF _worldToNDC;
        private MatrixF _view;
        private MatrixF _viewResverse;

        private float _red;
        private float _green;
        private float _blue;

        private Timer _timer;

        #region MSAA
        private uint[] _fbo;
        private uint[] _texture_msaa;

        private int _viewWidth;
        private int _viewHeight;
        #endregion

        #region Dash
        private uint[] _texture_dash;
        #endregion

        #region Uniform
        private uint[] _matrix;
        #endregion

        #endregion

        #region Property
        public IEnumerable<GLVisual> Visuals { get { return _visuals; } }
        protected List<GLVisual> _visuals;

        private Dictionary<Color, List<MeshModel>> _fillModels;
        private Dictionary<PenF, List<MeshModel>> _lineModels;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                _brush = new SolidColorBrush(_color);
                _red = _color.R / (float)byte.MaxValue;
                _green = _color.G / (float)byte.MaxValue;
                _blue = _color.B / (float)byte.MaxValue;
            }
        }
        private Color _color;
        private SolidColorBrush _brush;

        public PointF Origin
        {
            get { return _origin; }
            set
            {
                if (_origin != value)
                    _origin = value;
            }
        }
        private PointF _origin;
        #endregion

        #region Transform
        public PointF ViewToWorld(PointF p)
        {
            p = new PointF(p.X - _origin.X, p.Y - _origin.Y);
            p = _viewResverse.Transform(p);
            return p;
        }

        public void Scale(float scaleX, float scaleY)
        {
            _view.Scale(scaleX, scaleY);
            _AfterTransform();
        }

        public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY)
        {
            centerX -= _origin.X;
            centerY -= _origin.Y;
            _view.ScaleAt(scaleX, scaleY, centerX, centerY);
            _AfterTransform();
        }

        public void ScaleAt(PointF p, float scaleX, float scaleY)
        {
            ScaleAt(scaleX, scaleY, p.X, p.Y);
        }

        public void Translate(VectorF vector)
        {
            Translate(vector.X, vector.Y);
        }

        public void Translate(float offsetX, float offsetY)
        {
            _view.Translate(offsetX, offsetY);
            _AfterTransform();
        }

        private void _AfterTransform()
        {
            var view = _view;
            _view.Invert();
            _viewResverse = _view;
            _view = view;

            _Refresh();
        }
        #endregion

        #region Visual
        public GLVisual HitTest(PointF point, float sensitive = 5)
        {
            point = ViewToWorld(point);
            foreach (var visual in _visuals)
                if (visual.HitTest(point, sensitive * _viewResverse.M11 * _transformToDevice.M11))
                    return visual;
            return null;
        }

        /// <summary>
        /// Add a visual
        /// </summary>
        /// <param name="visual">The visual to add</param>
        /// <param name="refresh">Whether to refresh the frame buffer immediately</param>
        public void AddVisual(GLVisual visual, bool refresh = false)
        {
            _visuals.Add(visual);
            _Update(visual);
            if (refresh)
                _Refresh();
        }

        public void RemoveVisual(GLVisual visual)
        {
            _visuals.Remove(visual);
            _needUpdate = true;
            _Refresh();
        }

        /// <summary>
        /// Update viausl
        /// </summary>
        /// <param name="visual">The visual to update</param>
        /// <param name="refresh">Whether to refresh the frame buffer immediately</param>
        public void Update(GLVisual visual, bool refresh = false)
        {
            _Update(visual);
            if (refresh)
                _Refresh();
        }

        private void _Update(GLVisual visual)
        {
            visual.Update();
            _needUpdate = true;
        }

        /// <summary>
        /// Update all visuals and refresh frame buffer
        /// </summary>
        public void UpdateAll()
        {
            foreach (var visual in _visuals)
                _Update(visual);
            _Refresh();
        }

        /// <summary>
        /// Force refresh frame buffer
        /// </summary>
        public void Refresh()
        {
            _Refresh();
        }
        #endregion

        #region RenderFrame
        private void _DispatchFrame()
        {
            GLFunc.BindFramebuffer(GLConst.GL_FRAMEBUFFER, _fbo[0]);
            GLFunc.TexImage2DMultisample(GLConst.GL_TEXTURE_2D_MULTISAMPLE, 4, GLConst.GL_RGB, _viewWidth, _viewHeight, GLConst.GL_TRUE);
            GLFunc.ClearColor(_red, _green, _blue, 1.0f);
            GLFunc.Clear(GLConst.GL_COLOR_BUFFER_BIT | GLConst.GL_DEPTH_BUFFER_BIT | GLConst.GL_STENCIL_BUFFER_BIT);

            GLFunc.BindBuffer(GLConst.GL_UNIFORM_BUFFER, _matrix[0]);
            GLFunc.BufferSubData(GLConst.GL_UNIFORM_BUFFER, 0, 12 * sizeof(float), _worldToNDC.GetData(true));
            GLFunc.BufferSubData(GLConst.GL_UNIFORM_BUFFER, 12 * sizeof(float), 12 * sizeof(float), _view.GetData(true));
            _InvalidateModel();
            _DrawModels();

            GLFunc.BindFramebuffer(GLConst.GL_READ_FRAMEBUFFER, _fbo[0]);
            GLFunc.BindFramebuffer(GLConst.GL_DRAW_FRAMEBUFFER, 0);
            GLFunc.BlitFramebuffer(0, 0, _viewWidth, _viewHeight, 0, 0, _viewWidth, _viewHeight, GLConst.GL_COLOR_BUFFER_BIT, GLConst.GL_NEAREST);

            GLFunc.SwapBuffers();
        }
        #endregion

        #region EventHook
        /// <summary>
        /// Make sure to capture mouse events
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(_brush, null, new Rect(RenderSize));
        }
        #endregion

        #region Init & Destroy
        private void _Init()
        {
            if (_isInit) return;
            _isInit = true;
            _transformToDevice = (MatrixF)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;

            GL.MakeContextCurrent(Handle);
            GLFunc.Init();
            GLFunc.Enable(GLConst.GL_BLEND);
            GLFunc.Enable(GLConst.GL_CULL_FACE);
            GLFunc.Enable(GLConst.GL_LINE_WIDTH);
            GLFunc.Enable(GLConst.GL_LINE_SMOOTH);
            GLFunc.BlendFunc(GLConst.GL_SRC_ALPHA, GLConst.GL_ONE_MINUS_SRC_ALPHA);
            _CreateResource();

            _shader_dashed.Use();
            _shader_dashed.SetFloat("patternSize", 16.0f);

            _timer.Start(Timeout.Infinite, Timeout.Infinite);
        }

        private void _CreateResource()
        {
            _shader_normal = GenShader(_shaders_normal);
            _shader_dashed = GenShader(_shaders_dashed);
            _shaders.Add(_shader_normal);
            _shaders.Add(_shader_dashed);

            // for dash line texture
            _texture_dash = new uint[1];
            GLFunc.GenTextures(1, _texture_dash);
            GLFunc.BindTexture(GLConst.GL_TEXTURE_1D, _texture_dash[0]);
            GLFunc.TexParameteri(GLConst.GL_TEXTURE_1D, GLConst.GL_TEXTURE_WRAP_S, GLConst.GL_REPEAT);
            GLFunc.TexParameteri(GLConst.GL_TEXTURE_1D, GLConst.GL_TEXTURE_WRAP_T, GLConst.GL_REPEAT);
            GLFunc.TexParameteri(GLConst.GL_TEXTURE_1D, GLConst.GL_TEXTURE_MIN_FILTER, GLConst.GL_NEAREST);
            GLFunc.TexParameteri(GLConst.GL_TEXTURE_1D, GLConst.GL_TEXTURE_MAG_FILTER, GLConst.GL_NEAREST);
            GLFunc.BindTexture(GLConst.GL_TEXTURE_1D, 0);

            // for anti-aliasing
            _fbo = new uint[1];
            _texture_msaa = new uint[1];
            GLFunc.GenFramebuffers(1, _fbo);
            GLFunc.BindFramebuffer(GLConst.GL_FRAMEBUFFER, _fbo[0]);
            GLFunc.GenTextures(1, _texture_msaa);
            GLFunc.BindTexture(GLConst.GL_TEXTURE_2D_MULTISAMPLE, _texture_msaa[0]);
            GLFunc.FramebufferTexture2D(GLConst.GL_FRAMEBUFFER, GLConst.GL_COLOR_ATTACHMENT0, GLConst.GL_TEXTURE_2D_MULTISAMPLE, _texture_msaa[0], 0);

            // for transform
            _matrix = new uint[1];
            GLFunc.GenBuffers(1, _matrix);
            GLFunc.BindBuffer(GLConst.GL_UNIFORM_BUFFER, _matrix[0]);
            GLFunc.BufferData(GLConst.GL_UNIFORM_BUFFER, 24 * sizeof(float), default(byte[]), GLConst.GL_STATIC_DRAW);
            GLFunc.BindBufferRange(GLConst.GL_UNIFORM_BUFFER, 0, _matrix[0], 0, 24 * sizeof(float));
            GLFunc.BindBuffer(GLConst.GL_UNIFORM_BUFFER, 0);
            foreach (var shader in _shaders)
                GLFunc.UniformBlockBinding(shader.ID, GLFunc.GetUniformBlockIndex(shader.ID, "Matrices"), 0);
        }

        private void _DeleteResource()
        {
            _shaders.Dispose();
            _shaders.Clear();
            _shader_normal = null;
            _shader_dashed = null;

            GLFunc.DeleteTextures(1, _texture_dash);

            GLFunc.DeleteFramebuffers(1, _fbo);
            GLFunc.DeleteTextures(1, _texture_msaa);

            GLFunc.DeleteBuffers(1, _matrix);
        }

        private void _Destroy()
        {
            if (_isDisposed) return;

            _timer.Stop();
            _timer.Dispose();

            _DeleteResource();
            GL.DeleteContext();
            GLFunc.Dispose();
            _isInit = false;
        }
        #endregion

        #region Private
        private void _Refresh()
        {
            if (Interlocked.Increment(ref _signal) == 1)
                _timer.Change(0, Timeout.Infinite);
        }

        private void _AfterPainted()
        {
            var old = Interlocked.Decrement(ref _signal);
            Dispatcher.Invoke(() => { _DispatchFrame(); });
            Thread.Sleep(_frameSpan);
            if (Interlocked.CompareExchange(ref _signal, 0, old) != old)
                _timer.Change(0, Timeout.Infinite);
        }

        private void _InvalidateModel()
        {
            if (_needUpdate)
            {
                _needUpdate = false;
                _DisposeModels();
                foreach (var visual in _visuals)
                {
                    foreach (var primitive in visual.Context.Primitives)
                    {
                        _AttachLinesModel(primitive);
                        if (primitive.Filled)
                            _AttachFillModels(primitive);
                    }
                }
                _EndInitModels();
            }
        }

        private void _AttachLinesModel(IPrimitive primitive)
        {
            var model = default(MeshModel);
            if (_lineModels.ContainsKey(primitive.Pen))
                model = _lineModels[primitive.Pen].Last();
            else
            {
                model = new LinesModel();
                model.BeginInit();
                _lineModels.Add(primitive.Pen, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive))
            {
                model = new LinesModel();
                model.BeginInit();
                _lineModels[primitive.Pen].Add(model);
                model.TryAttachPrimitive(primitive);
            }
        }

        private void _AttachFillModels(IPrimitive primitive)
        {

        }

        private void _EndInitModels()
        {
            foreach (var value in _lineModels.Values)
                foreach (var model in value)
                    model.EndInit();

            foreach (var value in _fillModels.Values)
                foreach (var model in value)
                    model.EndInit();
        }

        private void _DrawModels()
        {
            foreach (var pair in _lineModels)
            {
                if(pair.Key.Data != null)
                {
                    _shader_dashed.Use();

                    // Set line pattern
                    GLFunc.BindTexture(GLConst.GL_TEXTURE_1D, _texture_dash[0]);
                    GLFunc.TexImage1D(GLConst.GL_TEXTURE_1D, 0, GLConst.GL_RED, pair.Key.Data.Length, 0, GLConst.GL_RED, GLConst.GL_UNSIGNED_BYTE, pair.Key.Data);

                    foreach (var model in pair.Value)
                        model.Draw(_shader_dashed, pair.Key, _transformToDevice.M11);
                }
                else
                {
                    _shader_normal.Use();
                    foreach (var model in pair.Value)
                        model.Draw(_shader_normal, pair.Key, _transformToDevice.M11);
                }
            }

            foreach (var pair in _fillModels)
            {
                _shader_normal.Use();
                foreach (var model in pair.Value)
                    model.Draw(_shader_normal, pair.Key);
            }
        }

        private static Shader GenShader(string[] shaders)
        {
            var header = GL.CreateGLSLHeader();
            var source = new List<ShaderSource>();
            string code;
            foreach (var shader in shaders)
            {
                using (var stream = new StreamReader(YOpenGL.Resources.OpenStream(shader)))
                {
                    code = stream.ReadToEnd();
                    var type = default(ShaderType);
                    if (shader.EndsWith("vert"))
                        type = ShaderType.Vert;
                    if (shader.EndsWith("frag"))
                        type = ShaderType.Frag;
                    if (shader.EndsWith("geo"))
                        type = ShaderType.Geo;
                    source.Add(new ShaderSource(type, string.Concat(header, code)));
                }
            }
            return Shader.CreateShader(source);
        }
        #endregion

        #region Override
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (!_isDisposed)
            {
                switch ((WindowMessage)msg)
                {
                    case WindowMessage.PAINT:
                        // Block background color updates
                        var paint = new PAINTSTRUCT(32);
                        Win32Helper.BeginPaint(Handle, ref paint);
                        Win32Helper.EndPaint(Handle, ref paint);

                        _Refresh();
                        break;
                }
            }
            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var _hwndHost = Win32Helper.CreateWindowEx(0, "static", "",
                                      Win32Helper.WS_CHILD | Win32Helper.WS_VISIBLE,
                                      0, 0,
                                      0, 0,
                                      hwndParent.Handle,
                                      IntPtr.Zero,
                                      IntPtr.Zero,
                                      0);

            return new HandleRef(this, _hwndHost);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _Destroy();
            Win32Helper.DestroyWindow(hwnd.Handle);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _Init();
            _OnRenderSizeChanged((float)sizeInfo.NewSize.Width, (float)sizeInfo.NewSize.Height);
        }

        private void _OnRenderSizeChanged(float width, float height)
        {
            width = Math.Max(1, width);
            height = Math.Max(1, height);
            _worldToNDC = new MatrixF();
            _worldToNDC.Translate(_origin.X, _origin.Y);
            _worldToNDC.Scale(2 / width, 2 / height);
            _worldToNDC.Translate(-1, -1);
            _viewWidth = (int)(width * _transformToDevice.M11);
            _viewHeight = (int)(height * _transformToDevice.M22);
            GLFunc.Viewport(0, 0, _viewWidth, _viewHeight);

            _shader_dashed.Use();
            _shader_dashed.SetVec2("screenSize", new float[] { _viewWidth, _viewHeight });
        }
        #endregion

        #region Dispose
        private void _Dispose()
        {
            _DisposeVisuals();
            _visuals = null;

            _DisposeModels();
            _fillModels = null;
            _lineModels = null;
        }

        private void _DisposeVisuals()
        {
            _visuals.Dispose();
            _visuals.Clear();
        }

        private void _DisposeModels()
        {
            foreach (var item in _fillModels.Values)
                item?.Dispose();
            foreach (var item in _lineModels.Values)
                item?.Dispose();

            _fillModels.Clear();
            _lineModels.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_isDisposed) return;

            _Dispose();
            _Destroy();

            _timer = null;
            _isDisposed = true;
        }
        #endregion
    }
}