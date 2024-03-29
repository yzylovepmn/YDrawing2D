﻿using System;
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
using static YRenderingSystem.GLFunc;
using static YRenderingSystem.GLConst;
using static YRenderingSystem.GL;
using System.Threading.Tasks;

namespace YRenderingSystem
{
    public enum RenderMode
    {
        Sync, // 同步
        Async // 异步
    }

    public enum ResourceMode
    {
        Normal,
        Global
    }

    /// <summary>
    /// The positive direction of the X axis is right(→) and the positive direction of the Y axis is up(↑);
    /// </summary>
    public class GLPanel : HwndHost, IDisposable
    {
        private const string ShaderSourcePrefix = "YRenderingSystem._2D.Resources.";
        private static readonly string[] _shaders_line = new string[] { "line.vert", "line.geom", "line.frag" };
        private static readonly string[] _shaders_arc = new string[] { "arc.vert", "arc.geom", "arc.frag" };
        private static readonly string[] _shaders_fill = new string[] { "fill.vert", "fill.frag" };
        private static readonly string[] _shaders_arrow = new string[] { "arrow.vert", "arrow.geom", "fill.frag" };

        public GLPanel(PointF origin, Color color, float frameRate = 60, RenderMode renderMode = RenderMode.Sync, ResourceMode resourceMode = ResourceMode.Global)
        {
            Focusable = true;
            Origin = origin;
            Color = color;
            _frameSpan = Math.Max(1, (int)(1000 / frameRate));
            _renderMode = renderMode;
            _resourceMode = resourceMode;
            _isInit = false;
            _isDisposed = false;
            _view = new MatrixF();
            _viewResverse = new MatrixF();
            _visuals = new List<GLVisual>();
            _fillModels = new Dictionary<Color, List<MeshModel>>();
            _arrowModels = new Dictionary<Color, List<MeshModel>>();
            _streamModels = new List<MeshModel>();
            _lineModels = new SortedDictionary<PenF, List<MeshModel>>();
            _arcModels = new SortedDictionary<PenF, List<MeshModel>>();
            _pointModels = new SortedDictionary<PointPair, List<MeshModel>>();
            _shaders = new List<Shader>();

            _timer = new Timer(_AfterPainted);
            _watch = new Stopwatch();
            _preference = new Preference(this);
        }

        #region Field & Property
        internal ContextHandle Context { get { return _context; } }
        private ContextHandle _context;

        private bool _isDisposed;
        private int _disposeFlag;
        private bool _isInit;
        private int _signal;
        private int _frameSpan;
        private Timer _timer;
        private Stopwatch _watch;

        #region Matrix
        internal MatrixF TransformToDevice { get { return _transformToDevice; } }
        private MatrixF _transformToDevice;
        private MatrixF _worldToNDC;
        private MatrixF _view;
        private MatrixF _viewResverse;
        #endregion

        #region Shader
        internal Shader Lineshader { get { return _lineshader; } }
        private Shader _lineshader;

        internal Shader Arcshader { get { return _arcshader; } }
        private Shader _arcshader;

        internal Shader Fillshader { get { return _fillshader; } }
        private Shader _fillshader;

        internal Shader ArrowShader { get { return _arrowShader; } }
        private Shader _arrowShader;

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
        internal uint[] Texture_Dash { get { return _texture_dash; } }
        private uint[] _texture_dash;
        #endregion

        #region Uniform
        private uint[] _matrix;
        #endregion

        public IEnumerable<GLVisual> Visuals { get { return _visuals; } }
        protected List<GLVisual> _visuals;

        public Preference Preference { get { return _preference; } }
        private Preference _preference;

        private Dictionary<Color, List<MeshModel>> _fillModels;
        private Dictionary<Color, List<MeshModel>> _arrowModels;
        private List<MeshModel> _streamModels;
        private SortedDictionary<PenF, List<MeshModel>> _lineModels;
        private SortedDictionary<PenF, List<MeshModel>> _arcModels;
        private SortedDictionary<PointPair, List<MeshModel>> _pointModels;

        public RenderMode RenderMode { get { return _renderMode; } }
        private RenderMode _renderMode;

        public ResourceMode ResourceMode { get { return _resourceMode; } }
        private ResourceMode _resourceMode;

        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                //_brush = new SolidColorBrush(_color);
                _red = (float)Math.Pow(_color.R / (float)byte.MaxValue, 2.2);
                _green = (float)Math.Pow(_color.G / (float)byte.MaxValue, 2.2);
                _blue = (float)Math.Pow(_color.B / (float)byte.MaxValue, 2.2);
                _Refresh();
            }
        }
        private Color _color;
        //private SolidColorBrush _brush;
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
        private PointF _FlipY(PointF point)
        {
            point.Y = (float)ActualHeight - point.Y;
            return point;
        }

        public PointF ToWpf(PointF pointInView)
        {
            return _FlipY(_ViewToWorld(pointInView));
        }

        public PointF ToView(PointF pointInWpf)
        {
            return _WorldToView(_FlipY(pointInWpf));
        }

        private RectF _FlipY(RectF rect)
        {
            rect.Y = (float)ActualHeight - rect.Bottom;
            return rect;
        }

        public RectF ToWpf(RectF rectInView)
        {
            return _FlipY(_ViewToWorld(rectInView));
        }

        public RectF ToView(RectF rectInWpf)
        {
            return _WorldToView(_FlipY(rectInWpf));
        }

        private PointF _WorldToView(PointF p)
        {
            p = new PointF(p.X - _origin.X, p.Y - _origin.Y);
            p = _viewResverse.Transform(p);
            return p;
        }

        private PointF _ViewToWorld(PointF p)
        {
            p = _view.Transform(p);
            p = new PointF(p.X + _origin.X, p.Y + _origin.Y);
            return p;
        }

        private RectF _WorldToView(RectF rect)
        {
            var _rect = new RectF(rect.X - _origin.X, rect.Y - _origin.Y, rect.Width, rect.Height);
            _rect.Transform(_viewResverse);
            return _rect;
        }

        private RectF _ViewToWorld(RectF rect)
        {
            rect.Transform(_view);
            var _rect = new RectF(rect.X + _origin.X, rect.Y + _origin.Y, rect.Width, rect.Height);
            return _rect;
        }

        public void ResetView(bool isRefresh = true)
        {
            _view = MatrixF.Identity;
            _AfterTransform(isRefresh);
        }

        public void Scale(float scaleX, float scaleY, bool isRefresh = true)
        {
            ScaleAt(scaleX, scaleY, 0, 0, isRefresh);
        }

        public void ScaleAt(float scaleX, float scaleY, float centerX, float centerY, bool isRefresh = true)
        {
            if (scaleX == 0 || scaleY == 0) return;

            _view.ScaleAtPrepend(scaleX, scaleY, centerX, centerY);
            _AfterTransform(isRefresh);
        }

        public void ScaleAt(PointF center, float scaleX, float scaleY, bool isRefresh = true)
        {
            ScaleAt(scaleX, scaleY, center.X, center.Y, isRefresh);
        }

        public void Translate(VectorF vector, bool isRefresh = true)
        {
            Translate(vector.X, vector.Y, isRefresh);
        }

        public void Translate(float offsetX, float offsetY, bool isRefresh = true)
        {
            _view.TranslatePrepend(offsetX, offsetY);
            _AfterTransform(isRefresh);
        }

        private void _AfterTransform(bool isRefresh = true)
        {
            var view = _view;
            _view.Invert();
            _viewResverse = _view;
            _view = view;

            if (isRefresh)
                _Refresh(false);
        }
        #endregion

        #region Visual
        public void MoveToTop(GLVisual visual, bool refresh = false)
        {
            if (_resourceMode == ResourceMode.Normal)
            {
                _visuals.Remove(visual);
                _visuals.Add(visual);

                if (refresh)
                    _Refresh();
            }
        }

        public void MoveToBottom(GLVisual visual, bool refresh = false)
        {
            if (_resourceMode == ResourceMode.Normal)
            {
                _visuals.Remove(visual);
                _visuals.Insert(0, visual);

                if (refresh)
                    _Refresh();
            }
        }

        public void MoveOnTop(GLVisual source, GLVisual target, bool refresh = false)
        {
            if (_resourceMode == ResourceMode.Normal)
            {
                _visuals.Remove(source);
                var index = _visuals.IndexOf(target);
                _visuals.Insert(index + 1, source);

                if (refresh)
                    _Refresh();
            }
        }

        public void MoveOnBottom(GLVisual source, GLVisual target, bool refresh = false)
        {
            if (_resourceMode == ResourceMode.Normal)
            {
                _visuals.Remove(source);
                var index = _visuals.IndexOf(target);
                _visuals.Insert(index, source);

                if (refresh)
                    _Refresh();
            }
        }

        public GLVisual HitTest(PointF pointInView, float sensitive = 6)
        {
            foreach (var visual in _visuals.Where(v => v.HitTestVisible))
                if (visual.HitTest(pointInView, sensitive * _viewResverse.M11))
                    return visual;
            return null;
        }

        public IEnumerable<GLVisual> HitTest(RectF rectInView, bool isFullContain = false)
        {
            foreach (var visual in _visuals.Where(v => v.HitTestVisible))
                if (visual.HitTest(rectInView, isFullContain))
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
            MakeSureCurrentContext(_context);

            if (!visual.IsDeleted)
            {
                if (visual.Panel != this)
                    throw new InvalidOperationException("Visual has already a logical parent!");
                else throw new InvalidOperationException("Visual has already been added");
            }

            _AddVisual(visual);
            _Update(visual);
            if (refresh)
                _Refresh();
        }

        public void InsertVisuals(int index, IEnumerable<GLVisual> visuals, bool refresh = false)
        {
            MakeSureCurrentContext(_context);

            foreach (var visual in visuals)
            {
                if (!visual.IsDeleted)
                {
                    if (visual.Panel != this)
                        throw new InvalidOperationException("Visual has already a logical parent!");
                    else throw new InvalidOperationException("Visual has already been added");
                }
            }

            _InsertVisuals(index, visuals);
            foreach (var visual in visuals)
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
            MakeSureCurrentContext(_context);

            if (visual.Panel != this)
                throw new InvalidOperationException("Logical parent error!");
            _DetachVisual(visual);
            _RemoveVisual(visual);
            if (!visual.IsUpdating)
            {
                visual.Reset();
                visual.Panel = null;
            }
            if (refresh)
                _Refresh();
        }

        public void RemoveAll()
        {
            MakeSureCurrentContext(_context);

            _DisposeModels();
            foreach (var visual in _visuals)
            {
                if (visual.IsUpdating) continue;
                visual.Reset();
                visual.Panel = null;
            }
            _RemoveAll();
            _Refresh();
        }

        private void _AddVisual(GLVisual visual)
        {
            visual.IsDeleted = false;
            visual.Panel = this;
            _visuals.Add(visual);
        }

        private void _InsertVisuals(int index, IEnumerable<GLVisual> visuals)
        {
            foreach (var visual in visuals)
            {
                visual.IsDeleted = false;
                visual.Panel = this;
            }
            _visuals.InsertRange(index, visuals);
        }

        private void _RemoveVisual(GLVisual visual)
        {
            visual.IsDeleted = true;
            _visuals.Remove(visual);
        }

        private void _RemoveAll()
        {
            _visuals.ForEach(visual => visual.IsDeleted = true);
            _visuals.Clear();
        }

        /// <summary>
        /// Update viausl
        /// </summary>
        /// <param name="visual">The visual to update</param>
        /// <param name="refresh">Whether to refresh the frame buffer immediately</param>
        public async void Update(GLVisual visual, bool refresh = false)
        {
            MakeSureCurrentContext(_context);

            if (visual.IsDeleted) return;
            if (_renderMode == RenderMode.Sync)
                _Update(visual, true);
            else await _UpdateAsync(visual, true);
            if (refresh || _renderMode == RenderMode.Async)
                _Refresh();
        }

        /// <summary>
        /// Update viausl
        /// </summary>
        /// <param name="visual">The visual to update</param>
        /// <param name="refresh">Whether to refresh the frame buffer immediately</param>
        public async Task UpdateAsync(GLVisual visual, bool refresh = false)
        {
            MakeSureCurrentContext(_context);

            if (visual.IsDeleted) return;
            if (_renderMode == RenderMode.Sync)
                _Update(visual, true);
            else await _UpdateAsync(visual, true);
            if (refresh || _renderMode == RenderMode.Async)
                _Refresh();
        }

        /// <param name="needDetach">whether remove current context from render buffer</param>
        private void _Update(GLVisual visual, bool needDetach = false)
        {
            if (needDetach)
                _DetachVisual(visual);
            visual.Update();            // context has been swapped
            visual.BackContext.Clear(); // so we clear back context
            _AttachVisual(visual);
        }

        /// <param name="needDetach">whether remove current context from render buffer</param>
        private async Task _UpdateAsync(GLVisual visual, bool needDetach = false)
        {
            if (visual.TryEnterUpdate())
            {
                if (needDetach)
                    _DetachVisual(visual); // remove current context from render buffer but not clear itself (for hittest)
                await visual.UpdateAsync(); // context has been swapped
                visual.BackContext?.Clear(); // so we clear back context

                // has been deleted when updating ? 
                if (!visual.IsDeleted) // not deleted
                {
                    _AttachVisual(visual); // add current context to render buffer
                    if (!visual.TryExitUpdate())
                        await _UpdateAsync(visual, true);
                }
                else // has been  deleted
                {
                    visual.Reset(); // reset visual
                    visual.Panel = null; // disconnect from panel
                    visual.ExitUpdate(); // endup updating !!!!!
                    if (visual.NeedDispose)
                        visual.Dispose();
                }
            }
        }

        /// <summary>
        /// Update all visuals and refresh frame buffer
        /// </summary>
        public async void UpdateAll()
        {
            MakeSureCurrentContext(_context);

            _DisposeModels(); /// clear render buffer, so we do not need remove context from render buffer.<see cref="_Update(GLVisual, bool)"/> and <see cref="_UpdateAsync(GLVisual, bool)"/>
            if (_renderMode == RenderMode.Sync)
            {
                foreach (var visual in _visuals)
                    _Update(visual);
            }
            else
            {
                foreach (var visual in _visuals.ToList())
                    await _UpdateAsync(visual);
            }
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

        #region Aliased
        internal void EnableAliased()
        {
            MakeSureCurrentContext(_context);
            Disable(GL_LINE_SMOOTH);
            //_DeleteFrameBuffer();
            _Refresh(false);
        }

        internal void DisableAliased()
        {
            MakeSureCurrentContext(_context);
            Enable(GL_LINE_SMOOTH);
            //_CreateFrameBuffer();
            _Refresh(false);
        }

        private void _CreateFrameBuffer()
        {
            if (_fbo != null) return;

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
        }

        private void _DeleteFrameBuffer()
        {
            if (_fbo == null) return;

            DeleteFramebuffers(1, _fbo);
            DeleteTextures(1, _texture_msaa);
            DeleteRenderbuffers(1, _rbo);

            _fbo = null;
            _texture_msaa = null;
            _rbo = null;
        }
        #endregion

        #region RenderFrame
        private void _DispatchFrame()
        {
            if (_isDisposed || !IsLoaded) return;
            MakeSureCurrentContext(_context);

            //if (!_preference.Aliased)
            //    BindFramebuffer(GL_FRAMEBUFFER, _fbo[0]);

            ClearColor(_red, _green, _blue, 1.0f);
            ClearStencil(0);
            Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

            BindBuffer(GL_UNIFORM_BUFFER, _matrix[0]);
            BufferSubData(GL_UNIFORM_BUFFER, 0, 12 * sizeof(float), _worldToNDC.GetData(true));
            BufferSubData(GL_UNIFORM_BUFFER, 12 * sizeof(float), 12 * sizeof(float), _view.GetData(true));
            _DrawModels();

            //if (!_preference.Aliased)
            //{
            //    BindFramebuffer(GL_READ_FRAMEBUFFER, _fbo[0]);
            //    BindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
            //    BlitFramebuffer(0, 0, _viewWidth, _viewHeight, 0, 0, _viewWidth, _viewHeight, GL_COLOR_BUFFER_BIT, GL_NEAREST);
            //}

            SwapBuffers(_context.HDC);
        }
        #endregion

        #region EventHook
        /// <summary>
        /// Make sure to capture mouse events
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Transparent, null, new System.Windows.Rect(RenderSize));
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
            //Enable(GL_LINE_SMOOTH);
            Enable(GL_FRAMEBUFFER_SRGB); // Gamma Correction
            BlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            StencilMask(1);
            _CreateResource();

            //_timer.Start(Timeout.Infinite, Timeout.Infinite);
        }

        private void _CreateResource()
        {
            MakeSureCurrentContext(_context);

            _lineshader = GenShader(_shaders_line);
            _arcshader = GenShader(_shaders_arc);
            _fillshader = GenShader(_shaders_fill);
            _arrowShader = GenShader(_shaders_arrow);

            _shaders.Add(_lineshader);
            _shaders.Add(_arcshader);
            _shaders.Add(_fillshader);
            _shaders.Add(_arrowShader);

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

            if (!_preference.Aliased)
            {
                //_CreateFrameBuffer();
                Enable(GL_LINE_SMOOTH);
            }

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
            _shaders.DisposeInner();
            _shaders.Clear();
            _lineshader = null;
            _arcshader = null;
            _fillshader = null;
            _arrowShader = null;

            //_DeleteFrameBuffer();
            DeleteTextures(1, _texture_dash);
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

            if (needUpdate && _resourceMode == ResourceMode.Global)
                _EndInitModels();

            if (Interlocked.Increment(ref _signal) == 1)
                _timer.Change(0, Timeout.Infinite);
        }

        private void _AfterPainted()
        {
            //_EnterDispose();

            if (!_isDisposed)
            {
                _watch.Restart();
                var old = Volatile.Read(ref _signal);

                //_ExitDispose();
                Dispatcher.Invoke(() => { _DispatchFrame(); }, DispatcherPriority.Render);
                //_EnterDispose();

                if (!_isDisposed)
                {
                    _watch.Stop();
                    var span = (int)_watch.ElapsedMilliseconds;

                    if (_frameSpan > span)
                        Thread.Sleep(_frameSpan - span);

                    if (Interlocked.CompareExchange(ref _signal, 0, old) != old)
                        _timer.Change(0, Timeout.Infinite);
                }
            }

            //_ExitDispose();
        }

        private void _AttachVisual(GLVisual visual)
        {
            if (_resourceMode == ResourceMode.Normal)
            {
                visual.AttachData();
                visual.EndInitModels();
            }
            else
            {
                var drawContext = visual.CurrentContext;
                MakeSureCurrentContext(_context);

                foreach (var primitive in drawContext.Primitives)
                {
                    if (!primitive.Pen.IsNULL || primitive.Type == PrimitiveType.ComplexGeometry)
                        _AttachModel(primitive, primitive.Pen);
                    if (primitive.Filled)
                    {
                        if (primitive.Type == PrimitiveType.ComplexGeometry)
                            _AttachStreamModel(primitive);
                        else if (primitive.Type == PrimitiveType.Arrow)
                            _AttachArrowModels(primitive);
                        else if (primitive.Type == PrimitiveType.Point)
                            _AttachPointModels(primitive);
                        else _AttachFillModels(primitive);
                    }
                }
            }
        }

        private void _DetachVisual(GLVisual visual)
        {
            if (_resourceMode == ResourceMode.Normal)
                visual.DetachData();
            else
            {
                var drawContext = visual.CurrentContext;
                visual.Detach(drawContext);
            }
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
            var models = default(SortedDictionary<PenF, List<MeshModel>>);

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

        private void _AttachPointModels(IPrimitive primitive)
        {
            var point = primitive as _Point;
            var model = default(MeshModel);
            var models = _pointModels;
            var pair = new PointPair(point.FillColor.Value, point.PointSize);

            if (models.ContainsKey(pair))
                model = models[pair].Last();
            else
            {
                model = _GreateModel(point, true);
                models.Add(pair, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(point, true);
                models[pair].Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private void _AttachArrowModels(IPrimitive primitive)
        {
            var model = default(MeshModel);
            var models = _arrowModels;

            if (models.ContainsKey(primitive.FillColor.Value))
                model = models[primitive.FillColor.Value].Last();
            else
            {
                model = _GreateModel(primitive, true);
                models.Add(primitive.FillColor.Value, new List<MeshModel>() { model });
            }
            if (!model.TryAttachPrimitive(primitive, false))
            {
                model = _GreateModel(primitive, true);
                models[primitive.FillColor.Value].Add(model);
                model.TryAttachPrimitive(primitive, false);
            }
            primitive.FillModel = model;
        }

        private MeshModel _GreateModel(IPrimitive primitive, bool isFilled = false)
        {
            var model = default(MeshModel);
            if (isFilled)
            {
                if (primitive == null)
                    model = new FillModel();
                else if (primitive.Type == PrimitiveType.Point)
                    model = new PointsModel();
                else model = new ArrowModel();
            }
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
            MakeSureCurrentContext(_context);

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

            foreach (var pair in _arrowModels.ToList())
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
                    _arrowModels.Remove(pair.Key);
            }

            foreach (var pair in _pointModels.ToList())
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
                    _pointModels.Remove(pair.Key);
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
            if (_resourceMode == ResourceMode.Normal)
            {
                foreach (var visual in _visuals)
                    visual.DrawModels();
            }
            else
            {
                Enable(GL_CULL_FACE);
                foreach (var pair in _fillModels)
                    _DrawFilledModelHandle(pair, _fillshader);

                foreach (var pair in _arcModels)
                    _DrawModelHandle(pair, _arcshader);

                foreach (var pair in _lineModels)
                    _DrawModelHandle(pair, _lineshader);

                foreach (var pair in _arrowModels)
                    _DrawFilledModelHandle(pair, _arrowShader);

                foreach (var pair in _pointModels)
                    _DrawPointModelsHandle(pair, _fillshader);
                Disable(GL_CULL_FACE);

                Enable(GL_STENCIL_TEST);
                _DrawSteramModelHandle(_streamModels, _fillshader);
                Disable(GL_STENCIL_TEST);
            }
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
                shader.SetFloat("dashedFactor", pair.Key.Data.Length * 8);
                BindTexture(GL_TEXTURE_1D, _texture_dash[0]);
                TexImage1D(GL_TEXTURE_1D, 0, GL_RED, pair.Key.Data.Length, 0, GL_RED, GL_UNSIGNED_BYTE, pair.Key.Data);
            }
            foreach (var model in pair.Value)
                model.Draw(shader);
        }

        private void _DrawFilledModelHandle(KeyValuePair<Color, List<MeshModel>> pair, Shader shader)
        {
            shader.Use();
            shader.SetVec4("color", 1, pair.Key.GetData());
            foreach (var model in pair.Value)
                model.Draw(shader);
        }

        private void _DrawPointModelsHandle(KeyValuePair<PointPair, List<MeshModel>> pair, Shader shader)
        {
            shader.Use();
            shader.SetVec4("color", 1, pair.Key.Color.GetData());
            PointSize(pair.Key.PointSize);
            foreach (var model in pair.Value)
                model.Draw(shader);
        }

        private void _DrawSteramModelHandle(List<MeshModel> models, Shader shader)
        {
            shader.Use();
            foreach (var model in models)
                model.Draw(shader);
        }

        private Shader GenShader(string[] shaders)
        {
            var header = CreateGLSLHeader();
            var source = new List<ShaderSource>();
            string code;
            foreach (var shader in shaders)
            {
                using (var stream = new StreamReader(YRenderingSystem.Resources.OpenStream(ShaderSourcePrefix, shader)))
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
            return Shader.GenShader(source, _context);
        }
        #endregion

        #region Override
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

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
        protected virtual void DisposeCore()
        {
            _DisposeModels();
            _DisposeVisuals();
            _visuals = null;

            _fillModels = null;
            _arrowModels = null;
            _streamModels = null;
            _lineModels = null;
            _arcModels = null;
            _pointModels = null;
        }

        private void _DisposeVisuals()
        {
            //_visuals.DisposeInner();
            _visuals.ForEach(visual => visual.Dispose());
            _visuals.Clear();
        }

        private void _DisposeModels()
        {
            if (_resourceMode == ResourceMode.Normal)
                _visuals.ForEach(visual => visual.DetachData());
            else
            {
                MakeSureCurrentContext(_context);

                foreach (var item in _fillModels?.Values)
                    item?.DisposeInner();
                foreach (var item in _arrowModels?.Values)
                    item?.DisposeInner();
                _streamModels?.DisposeInner();
                foreach (var item in _lineModels?.Values)
                    item?.DisposeInner();
                foreach (var item in _arcModels?.Values)
                    item?.DisposeInner();
                foreach (var item in _pointModels?.Values)
                    item?.DisposeInner();

                _fillModels?.Clear();
                _arrowModels?.Clear();
                _streamModels?.Clear();
                _lineModels?.Clear();
                _arcModels?.Clear();
                _pointModels?.Clear();
            }
        }

        protected sealed override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_isDisposed) return;

            _EnterDispose();

            _isDisposed = true;
            Dispatcher.Invoke(() => DisposeCore());
            _Destroy();

            _timer = null;
            _watch = null;

            _preference.Dispose();
            _preference = null;

            _ExitDispose();
        }
        #endregion
    }
}