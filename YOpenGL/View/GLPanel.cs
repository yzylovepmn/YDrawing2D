using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;
using static YOpenGL.GL;

namespace YOpenGL
{
    /// <summary>
    /// The positive direction of the X axis is right(→) and the positive direction of the Y axis is up(↑);
    /// </summary>
    public class GLPanel : HwndHost, IDisposable
    {
        private static readonly string[] _shaders_line = new string[] { "line.vert", "line.geom", "line.frag" };
        private static readonly string[] _shaders_arc = new string[] { "arc.vert", "arc.geom", "arc.frag" };
        private static readonly string[] _shaders_fill = new string[] { "fill.vert", "fill.frag" };

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
            _streamModels = new List<MeshModel>();
            _lineModels = new Dictionary<PenF, List<MeshModel>>();
            _arcModels = new Dictionary<PenF, List<MeshModel>>();
            _shaders = new List<Shader>();

            _timer = new Timer(_AfterPainted);
            _watch = new Stopwatch();
        }

        #region Private Field
        private ContextHandle _context;

        private bool _isDisposed;
        private int _disposeFlag;
        private bool _isInit;
        private int _signal;
        private int _frameSpan;
        private Timer _timer;
        private Stopwatch _watch;

        #region Matrix
        private MatrixF _transformToDevice;
        private MatrixF _worldToNDC;
        private MatrixF _view;
        private MatrixF _viewResverse;
        #endregion

        #region Shader
        private Shader _lineshader;
        private Shader _arcshader;
        private Shader _fillshader;
        private List<Shader> _shaders;
        #endregion

        #region MSAA
        private uint[] _fbo;
        private uint[] _texture_msaa;
        private uint[] _rbo;

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
        private List<MeshModel> _streamModels;
        private Dictionary<PenF, List<MeshModel>> _lineModels;
        private Dictionary<PenF, List<MeshModel>> _arcModels;

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
                _Refresh();
            }
        }
        private Color _color;
        private SolidColorBrush _brush;
        private float _red;
        private float _green;
        private float _blue;

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

        public float ScaleX { get { return Math.Abs(_view.M11); } }

        public float ScaleY { get { return Math.Abs(_view.M22); } }

        public float OffsetX { get { return _view.OffsetX; } }

        public float OffsetY { get { return _view.OffsetY; } }
        #endregion

        #region Sync
        private void _EnterDispose()
        {
            while (Interlocked.Exchange(ref _disposeFlag, 1) == 1)
                Thread.Sleep(1);
        }

        private void _ExitDispose()
        {
            Interlocked.Exchange(ref _disposeFlag, 0);
        }
        #endregion

        #region Transform
        public PointF WorldToView(PointF p)
        {
            p = new PointF(p.X - _origin.X, p.Y - _origin.Y);
            p = _viewResverse.Transform(p);
            return p;
        }

        public PointF ViewToWorld(PointF p)
        {
            p = _view.Transform(p);
            p = new PointF(p.X + _origin.X, p.Y + _origin.Y);
            return p;
        }

        public RectF WorldToView(RectF rect)
        {
            var _rect = new RectF(rect.X - _origin.X, rect.Y - _origin.Y, rect.Width, rect.Height);
            _rect.Transform(_viewResverse);
            return _rect;
        }

        public RectF ViewToWorld(RectF rect)
        {
            rect.Transform(_view);
            var _rect = new RectF(rect.X + _origin.X, rect.Y + _origin.Y, rect.Width, rect.Height);
            return _rect;
        }

        public void ResetView()
        {
            _view = MatrixF.Identity;
            _AfterTransform();
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

            _Refresh(false);
        }
        #endregion

        #region Visual
        public PointF GetMousePosition(MouseEventArgs e)
        {
            var p = e.GetPosition(this);
            return new PointF((float)p.X, (float)(RenderSize.Height - p.Y));
        }

        public GLVisual HitTest(PointF point, float sensitive = 6)
        {
            point = WorldToView(point);
            foreach (var visual in _visuals.Where(v => v.HitTestVisible))
                if (visual.HitTest(point, sensitive * _viewResverse.M11))
                    return visual;
            return null;
        }

        public IEnumerable<GLVisual> HitTest(RectF rect, bool isFullContain = false)
        {
            rect = WorldToView(rect);
            foreach (var visual in _visuals.Where(v => v.HitTestVisible))
                if (visual.HitTest(rect, isFullContain))
                    yield return visual;
            yield break;
        }

        /// <summary>
        /// Add a visual
        /// </summary>
        /// <param name="visual">The visual to add</param>
        /// <param name="refresh">Whether to refresh the frame buffer immediately</param>
        public void AddVisual(GLVisual visual, bool refresh = false)
        {
            if (visual.Panel != null)
                throw new InvalidOperationException("Visual already has a logical parent!");

            _visuals.Add(visual);
            visual.Panel = this;
            _Update(visual);
            if (refresh)
                _Refresh();
        }

        /// <summary>
        /// Remove a visual
        /// </summary>
        /// <param name="visual">The visual to remove</param>
        /// <param name="refresh">Whether to refresh the frame buffer immediately</param>
        public void RemoveVisual(GLVisual visual, bool refresh = true)
        {
            _DetachVisual(visual);
            _visuals.Remove(visual);
            visual.Panel = null;
            if (refresh)
                _Refresh();
        }

        public void RemoveAll()
        {
            _DisposeModels();
            foreach (var visual in _visuals)
            {
                visual.Reset();
                visual.Panel = null;
            }
            _visuals.Clear();
            _Refresh();
        }

        /// <summary>
        /// Update viausl
        /// </summary>
        /// <param name="visual">The visual to update</param>
        /// <param name="refresh">Whether to refresh the frame buffer immediately</param>
        public void Update(GLVisual visual, bool refresh = false)
        {
            _Update(visual, true);
            if (refresh)
                _Refresh();
        }

        private void _Update(GLVisual visual, bool needDetach = false)
        {
            if (needDetach)
                _DetachVisual(visual);
            visual.Update();
            _AttachVisual(visual);
        }

        /// <summary>
        /// Update all visuals and refresh frame buffer
        /// </summary>
        public void UpdateAll()
        {
            _DisposeModels();
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
            MakeSureCurrentContext(_context);

            BindFramebuffer(GL_FRAMEBUFFER, _fbo[0]);

            ClearColor(_red, _green, _blue, 1.0f);
            Clear(GL_COLOR_BUFFER_BIT);

            BindBuffer(GL_UNIFORM_BUFFER, _matrix[0]);
            BufferSubData(GL_UNIFORM_BUFFER, 0, 12 * sizeof(float), _worldToNDC.GetData(true));
            BufferSubData(GL_UNIFORM_BUFFER, 12 * sizeof(float), 12 * sizeof(float), _view.GetData(true));
            _DrawModels();

            BindFramebuffer(GL_READ_FRAMEBUFFER, _fbo[0]);
            BindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
            BlitFramebuffer(0, 0, _viewWidth, _viewHeight, 0, 0, _viewWidth, _viewHeight, GL_COLOR_BUFFER_BIT, GL_NEAREST);

            SwapBuffers(_context.HDC);
        }
        #endregion

        #region EventHook
        /// <summary>
        /// Make sure to capture mouse events
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(_brush, null, new System.Windows.Rect(RenderSize));
        }
        #endregion

        #region Init & Destroy
        private void _Init()
        {
            if (_isInit) return;
            _isInit = true;
            _transformToDevice = (MatrixF)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;

            _context = CreateContextCurrent(Handle);
            Init();
            Enable(GL_BLEND);
            Enable(GL_LINE_WIDTH);
            Enable(GL_LINE_SMOOTH);
            Enable(GL_FRAMEBUFFER_SRGB); // Gamma Correction
            BlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            StencilMask(1);
            _CreateResource();

            _timer.Start(Timeout.Infinite, Timeout.Infinite);
        }

        private void _CreateResource()
        {
            _lineshader = GenShader(_shaders_line);
            _arcshader = GenShader(_shaders_arc);
            _fillshader = GenShader(_shaders_fill);

            _shaders.Add(_lineshader);
            _shaders.Add(_arcshader);
            _shaders.Add(_fillshader);

            // for draw cicle and arc
            _arcshader.Use();
            _arcshader.SetVec2("samples", 65, GeometryHelper.UnitCicle.GetData());

            // for dash line texture
            _texture_dash = new uint[1];
            GenTextures(1, _texture_dash);
            BindTexture(GL_TEXTURE_1D, _texture_dash[0]);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_WRAP_S, GL_REPEAT);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            BindTexture(GL_TEXTURE_1D, 0);

            // for anti-aliasing
            _fbo = new uint[1];
            _texture_msaa = new uint[1];
            _rbo = new uint[1];
            GenFramebuffers(1, _fbo);
            BindFramebuffer(GL_FRAMEBUFFER, _fbo[0]);
            GenTextures(1, _texture_msaa);
            BindTexture(GL_TEXTURE_2D_MULTISAMPLE, _texture_msaa[0]);
            FramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D_MULTISAMPLE, _texture_msaa[0], 0);
            TexImage2DMultisample(GL_TEXTURE_2D_MULTISAMPLE, 4, GL_RGB, (int)(SystemParameters.PrimaryScreenWidth * _transformToDevice.M11), (int)(SystemParameters.PrimaryScreenHeight * _transformToDevice.M11), GL_TRUE);
            GenRenderbuffers(1, _rbo);
            BindRenderbuffer(GL_RENDERBUFFER, _rbo[0]);
            RenderbufferStorageMultisample(GL_RENDERBUFFER, 4, GL_STENCIL_INDEX1, (int)(SystemParameters.PrimaryScreenWidth * _transformToDevice.M11), (int)(SystemParameters.PrimaryScreenHeight * _transformToDevice.M11));
            FramebufferRenderbuffer(GL_FRAMEBUFFER, GL_STENCIL_ATTACHMENT, GL_RENDERBUFFER, _rbo[0]);

            // for transform
            _matrix = new uint[1];
            GenBuffers(1, _matrix);
            BindBuffer(GL_UNIFORM_BUFFER, _matrix[0]);
            BufferData(GL_UNIFORM_BUFFER, 24 * sizeof(float), default(byte[]), GL_STATIC_DRAW);
            BindBufferBase(GL_UNIFORM_BUFFER, 0, _matrix[0]);
            BindBuffer(GL_UNIFORM_BUFFER, 0);

            //Set binding point
            foreach (var shader in _shaders)
                UniformBlockBinding(shader.ID, GetUniformBlockIndex(shader.ID, "Matrices"), 0);
        }

        private void _DeleteResource()
        {
            _shaders.Dispose();
            _shaders.Clear();
            _lineshader = null;
            _arcshader = null;
            _fillshader = null;

            DeleteTextures(1, _texture_dash);

            DeleteFramebuffers(1, _fbo);
            DeleteTextures(1, _texture_msaa);
            DeleteRenderbuffers(1, _rbo);

            DeleteBuffers(1, _matrix);
        }

        private void _Destroy()
        {
            _timer.Stop();
            _timer.Dispose();

            MakeSureCurrentContext(_context);
            _DeleteResource();
            DeleteContext(_context);
            _isInit = false;
        }
        #endregion

        #region Private
        private void _Refresh(bool needUpdate = true)
        {
            if (!_isInit) return;

            if (needUpdate)
                _EndInitModels();

            if (Interlocked.Increment(ref _signal) == 1)
                _timer.Change(0, Timeout.Infinite);
        }

        private void _AfterPainted()
        {
            _EnterDispose();

            if (!_isDisposed)
            {
                var old = Interlocked.Decrement(ref _signal);

                _watch.Restart();
                var before = _watch.ElapsedMilliseconds;
                Dispatcher.Invoke(() => { _DispatchFrame(); });
                var span = (int)(_watch.ElapsedMilliseconds - before);
                _watch.Stop();

                if (_frameSpan > span)
                    Thread.Sleep(_frameSpan - span);

                if (Interlocked.CompareExchange(ref _signal, 0, old) != old)
                    _timer.Change(0, Timeout.Infinite);
            }

            _ExitDispose();
        }

        private void _AttachVisual(GLVisual visual)
        {
            foreach (var primitive in visual.Context.Primitives)
            {
                if (!primitive.Pen.IsNULL || primitive.Type == PrimitiveType.ComplexGeometry)
                    _AttachModel(primitive, primitive.Pen);
                if (primitive.Filled)
                {
                    if (primitive.Type == PrimitiveType.ComplexGeometry)
                        _AttachStreamModel(primitive);
                    else _AttachFillModels(primitive);
                }
            }
        }

        private void _DetachVisual(GLVisual visual)
        {
            visual.Detach();
        }

        private void _AttachModel(IPrimitive primitive, PenF pen)
        {
            if (primitive.Type == PrimitiveType.ComplexGeometry)
            {
                foreach (var child in ((_ComplexGeometry)primitive).Children.Where(c => !c.Pen.IsNULL))
                    _AttachModel(child, child.Pen);
                return;
            }

            if (primitive.Type == PrimitiveType.SimpleGeometry)
            {
                foreach (var child in ((_SimpleGeometry)primitive).Stream.Where(c => !c.Pen.IsNULL))
                    _AttachModel(child, pen);
                return;
            }

            var model = default(MeshModel);
            var models = default(Dictionary<PenF, List<MeshModel>>);

            if (primitive.Type == PrimitiveType.Arc)
                models = _arcModels;
            else models = _lineModels;

            if (models.ContainsKey(pen))
                model = models[pen].Last();
            else
            {
                model = _GreateModel(primitive);
                models.Add(pen, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive))
            {
                model = _GreateModel(primitive);
                models[pen].Add(model);
                model.TryAttachPrimitive(primitive);
            }
            primitive.Model = model;
        }

        private void _AttachStreamModel(IPrimitive primitive)
        {
            var model = default(MeshModel);
            if (_streamModels.Count == 0)
            {
                model = _GreateModel(primitive);
                _streamModels.Add(model);
            }
            else model = _streamModels.Last();
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(primitive);
                _streamModels.Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private void _AttachFillModels(IPrimitive primitive)
        {
            var model = default(MeshModel);
            var models = _fillModels;

            if (models.ContainsKey(primitive.FillColor.Value))
                model = models[primitive.FillColor.Value].Last();
            else
            {
                model = _GreateModel(null, true);
                models.Add(primitive.FillColor.Value, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(null, true);
                models[primitive.FillColor.Value].Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private MeshModel _GreateModel(IPrimitive primitive, bool isFilled = false)
        {
            var model = default(MeshModel);
            if (isFilled)
                model = new FillModel();
            else if (primitive.Type == PrimitiveType.ComplexGeometry)
                model = new StreamModel();
            else if (primitive.Type == PrimitiveType.Arc)
                model = new ArcModel();
            else model = new LinesModel();
            model.BeginInit();
            return model;
        }

        private void _EndInitModels()
        {
            foreach (var pair in _lineModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _lineModels.Remove(pair.Key);
            }

            foreach (var pair in _arcModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _arcModels.Remove(pair.Key);
            }

            foreach (var pair in _fillModels.ToList())
            {
                foreach (var model in pair.Value.ToList())
                {
                    if (model.NeedDisposed)
                    {
                        model.Dispose();
                        pair.Value.Remove(model);
                    }
                    else model.EndInit();
                }
                if (pair.Value.Count == 0)
                    _fillModels.Remove(pair.Key);
            }

            foreach (var model in _streamModels.ToList())
            {
                if (model.NeedDisposed)
                {
                    model.Dispose();
                    _streamModels.Remove(model);
                }
                else model.EndInit();
            }
        }

        private void _DrawModels()
        {
            Enable(GL_STENCIL_TEST);
            _DrawSteramModelHandle(_streamModels, _fillshader);
            Disable(GL_STENCIL_TEST);

            Enable(GL_CULL_FACE);
            foreach (var pair in _fillModels)
                _DrawFilledModelHandle(pair, _fillshader);

            foreach (var pair in _lineModels)
                _DrawModelHandle(pair, _lineshader);

            foreach (var pair in _arcModels)
                _DrawModelHandle(pair, _arcshader);
            Disable(GL_CULL_FACE);
        }

        private void _DrawModelHandle(KeyValuePair<PenF, List<MeshModel>> pair, Shader shader)
        {
            //Set line width
            LineWidth(pair.Key.Thickness / _transformToDevice.M11);

            shader.Use();
            shader.SetBool("dashed", pair.Key.Data != null);
            shader.SetVec4("color", 1, pair.Key.Color.GetData());

            if (pair.Key.Data != null)
            {
                // Set line pattern
                BindTexture(GL_TEXTURE_1D, _texture_dash[0]);
                TexImage1D(GL_TEXTURE_1D, 0, GL_RED, pair.Key.Data.Length, 0, GL_RED, GL_UNSIGNED_BYTE, pair.Key.Data);

                foreach (var model in pair.Value)
                    model.Draw(shader);
            }
            else foreach (var model in pair.Value)
                    model.Draw(shader);
        }

        private void _DrawFilledModelHandle(KeyValuePair<Color, List<MeshModel>> pair, Shader shader)
        {
            shader.Use();
            shader.SetVec4("color", 1, pair.Key.GetData());
            foreach (var model in pair.Value)
                model.Draw(shader);
        }

        private void _DrawSteramModelHandle(List<MeshModel> models, Shader shader)
        {
            shader.Use();
            foreach (var model in models)
                model.Draw(shader);
        }

        private static Shader GenShader(string[] shaders)
        {
            var header = CreateGLSLHeader();
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
                    if (shader.EndsWith("geom"))
                        type = ShaderType.Geom;
                    source.Add(new ShaderSource(type, code.Replace("#version 330 core", header)));
                }
            }
            return Shader.GenShader(source);
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
                                      Win32Helper.WS_CHILD | Win32Helper.WS_VISIBLE | Win32Helper.WS_CLIPCHILDREN | Win32Helper.WS_CLIPSIBLINGS,
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
            MakeSureCurrentContext(_context);

            _worldToNDC = new MatrixF();
            _worldToNDC.Translate(_origin.X, _origin.Y);
            _worldToNDC.Scale(2 / width, 2 / height);
            _worldToNDC.Translate(-1, -1);
            _viewWidth = (int)(width * _transformToDevice.M11);
            _viewHeight = (int)(height * _transformToDevice.M22);

            Viewport(0, 0, _viewWidth, _viewHeight);

            // for dashed line
            _lineshader.Use();
            _lineshader.SetVec2("screenSize", 1, new float[] { width, height });

            // for dashed cicle and arc
            _arcshader.Use();
            _arcshader.SetVec2("screenSize", 1, new float[] { width, height });
        }
        #endregion

        #region Dispose
        private void _Dispose()
        {
            _DisposeVisuals();
            _visuals = null;

            _DisposeModels();
            _fillModels = null;
            _streamModels = null;
            _lineModels = null;
            _arcModels = null;
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
            _streamModels?.Dispose();
            foreach (var item in _lineModels.Values)
                item?.Dispose();
            foreach (var item in _arcModels.Values)
                item?.Dispose();

            _fillModels.Clear();
            _streamModels.Clear();
            _lineModels.Clear();
            _arcModels.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_isDisposed) return;

            _EnterDispose();

            _isDisposed = true;
            _Dispose();
            _Destroy();

            _timer = null;
            _watch = null;

            _ExitDispose();
        }
        #endregion
    }
}