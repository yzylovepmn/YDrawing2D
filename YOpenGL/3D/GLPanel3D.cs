using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using static YOpenGL.GLFunc;
using static YOpenGL.GLConst;
using static YOpenGL.GL;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Input;
using System.ComponentModel;

namespace YOpenGL._3D
{
    public class GLPanel3D : HwndHost, IGLContext, IDisposable
    {
        public const string TransformUniformBlockName = "Matrices";
        public const string LightsUniformBlockName = "Lights";
        private const string ShaderSourcePrefix = "YOpenGL._3D.Shaders.";
        private static readonly string[] _defaultShadeSource = new string[] { "default.vert", "default.frag" };

        private const int LightsCount = 10;
        private const int AmbientLightSize = 4 * LightsCount * sizeof(float);
        private const int DirLightSize = 12 * LightsCount * sizeof(float);
        private const int PointLightSize = 16 * LightsCount * sizeof(float);
        private const int SpotLightSize = 24 * LightsCount * sizeof(float);
        private const int AmbientLightOffset = 0;
        private const int DirLightOffset = AmbientLightOffset + AmbientLightSize;
        private const int PointLightOffset = DirLightOffset + DirLightSize;
        private const int SpotLightOffset = PointLightOffset + PointLightSize;

        public GLPanel3D(Color backgroundColor, float frameRate = 60)
        {
            _isInit = false;
            _timer = new Timer(_OnRender);
            _watch = new Stopwatch();
            _frameSpan = Math.Max(1, (int)(1000 / frameRate));
            _camera = new Camera(this, CameraType.Perspective, 1000, 800, 1, float.PositiveInfinity, new Point3F(0, 0, 1), new Vector3F(0, 0, -1), new Vector3F(0, 1, 0));
            _selector = new RectSelector(this);
            _models = new List<GLModel3D>();
            _lights = new List<Light>();
            _mouseEventHandler = new MouseEventHandler(this);
            _zoomExtentWhenLoaded = true;
            _zoomSensitivity = 1;
            _rotationSensitivity = 1;
            _modelUpDirection = new Vector3F(0, 0, 1);

            BackgroundColor = backgroundColor;
            Focusable = true;

            _camera.PropertyChanged += _OnCameraPropertyChanged;
            Loaded += _OnLoaded;
        }

        #region Field & Property
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                _red = (float)Math.Pow(_backgroundColor.R / (double)byte.MaxValue, 2.1);
                _green = (float)Math.Pow(_backgroundColor.G / (double)byte.MaxValue, 2.1);
                _blue = (float)Math.Pow(_backgroundColor.B / (double)byte.MaxValue, 2.1);
                _Refresh();
            }
        }
        private Color _backgroundColor;
        private float _red;
        private float _green;
        private float _blue;

        public IEnumerable<GLModel3D> Models { get { return _models; } }
        private List<GLModel3D> _models;

        public IEnumerable<Light> Lights { get { return _lights; } }
        private List<Light> _lights;

        public ContextHandle Context { get { return _context; } }
        private ContextHandle _context;

        public Camera Camera { get { return _camera; } }
        private Camera _camera;

        public Shader DefaultShader { get { return _defaultShader; } }
        private Shader _defaultShader;

        public RectSelector Selector { get { return _selector; } }
        private RectSelector _selector;

        private MouseEventHandler _mouseEventHandler;

        internal bool IsInit { get { return _isInit; } }
        private bool _isInit;

        private int _signal;
        private int _frameSpan;
        private Timer _timer;
        private Stopwatch _watch;
        internal float ViewWidth;
        internal float ViewHeight;

        #region Matrix
        internal MatrixF TransformToDevice { get { return _transformToDevice; } }
        private MatrixF _transformToDevice;
        #endregion

        #region Buffer Object
        private uint[] _uniformBlockObj;

        internal uint TransformBlock { get { return _uniformBlockObj[0]; } }

        internal uint LightsBlock { get { return _uniformBlockObj[1]; } }
        #endregion

        public Vector3F ModelUpDirection
        {
            get { return _modelUpDirection; }
            set { _modelUpDirection = value; }
        }
        private Vector3F _modelUpDirection;

        public bool ZoomExtentWhenLoaded { get { return _zoomExtentWhenLoaded; } set { _zoomExtentWhenLoaded = value; } }
        private bool _zoomExtentWhenLoaded;

        public float ZoomSensitivity
        { 
            get { return _zoomSensitivity; } 
            set { _zoomSensitivity = Math.Min(Math.Max(0.1f, value), 10); }
        }
        private float _zoomSensitivity;

        public float RotationSensitivity
        {
            get { return _rotationSensitivity; }
            set { _rotationSensitivity = Math.Min(Math.Max(0.1f, value), 10); }
        }
        private float _rotationSensitivity;

        public bool RotateAroundMouse { get { return _rotateAroundMouse; } set { _rotateAroundMouse = value; } }
        private bool _rotateAroundMouse;
        #endregion

        #region Models
        public void AddModels(IEnumerable<GLModel3D> models)
        {
            MakeSureCurrentContext();
            foreach (var model in models)
                _AddModel(model);
            _Refresh();
        }

        public void AddModel(GLModel3D model)
        {
            MakeSureCurrentContext();
            _AddModel(model);
            _Refresh();
        }

        private void _AddModel(GLModel3D model)
        {
            if (_models.Contains(model))
                throw new InvalidOperationException("model has been added");

            if (model.Viewport != null)
                throw new InvalidOperationException("model has been a logical parent");

            _models.Add(model);
            model.Viewport = this;
            if (_isInit)
                model.Init();
        }

        public void RemoveModels(IEnumerable<GLModel3D> models)
        {
            MakeSureCurrentContext();
            foreach (var model in models)
                _RemoveModel(model);
            _Refresh();
        }

        public void RemoveModel(GLModel3D model)
        {
            MakeSureCurrentContext();
            _RemoveModel(model);
            _Refresh();
        }

        private void _RemoveModel(GLModel3D model)
        {
            if (!_models.Contains(model))
                throw new InvalidOperationException("model does not exist!");

            model.Clean();
            _models.Remove(model);
            model.Viewport = null;
        }

        public void AddLight(Light light)
        {
            if (_lights.Contains(light)) throw new InvalidOperationException("light has been added!");
            if (_lights.Count(l => l.Type == light.Type) == LightsCount) throw new InvalidOperationException($"max light number is {LightsCount}");

            _lights.Add(light);
            light.PropertyChanged += _OnLightPropertyChanged;
            _OnLightCollectionChanged();
        }

        public void RemoveLight(Light light)
        {
            if (!_lights.Contains(light)) throw new InvalidOperationException("light does not exist!");
            light.PropertyChanged -= _OnLightPropertyChanged;
            _lights.Remove(light);
            _OnLightCollectionChanged();
        }

        private void _OnLightCollectionChanged()
        {
            _UpdateLights();
            _Refresh();
        }

        private void _OnLightPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _UpdateLight(sender as Light);
            _Refresh();
        }

        private void _UpdateLight(Light light)
        {
            if (!_isInit) return;

            var lightsData = light.GetData();
            var offset = 0;
            var size = 0;
            var index = _lights.Where(l => l.Type == light.Type).IndexOfInner(light);
            switch (light.Type)
            {
                case LightType.Ambient:
                    size = AmbientLightSize / LightsCount;
                    offset = AmbientLightOffset + size * index;
                    break;
                case LightType.Direction:
                    size = DirLightSize / LightsCount;
                    offset = DirLightOffset + size * index;
                    break;
                case LightType.Point:
                    size = PointLightSize / LightsCount;
                    offset = PointLightOffset + size * index;
                    break;
                case LightType.Spot:
                    size = SpotLightSize / LightsCount;
                    offset = SpotLightOffset + size * index;
                    break;
            }
            MakeSureCurrentContext();
            BindBuffer(GL_UNIFORM_BUFFER, LightsBlock);
            BufferSubData(GL_UNIFORM_BUFFER, offset, size, lightsData.ToArray());
        }

        private void _UpdateLights()
        {
            if (!_isInit) return;

            int ambientLightCount = 0;
            int dirLightCount = 0;
            int pointLightCount = 0;
            int spotLightCount = 0;
            var ambientLightsData = new List<float>();
            var dirLightsData = new List<float>();
            var pointLightsData = new List<float>();
            var spotLightsData = new List<float>();

            foreach (var light in _lights)
            {
                var data = light.GetData();
                switch (light.Type)
                {
                    case LightType.Ambient:
                        ambientLightCount++;
                        ambientLightsData.AddRange(data);
                        break;
                    case LightType.Direction:
                        dirLightCount++;
                        dirLightsData.AddRange(data);
                        break;
                    case LightType.Point:
                        pointLightCount++;
                        pointLightsData.AddRange(data);
                        break;
                    case LightType.Spot:
                        spotLightCount++;
                        spotLightsData.AddRange(data);
                        break;
                }
            }

            MakeSureCurrentContext();
            BindBuffer(GL_UNIFORM_BUFFER, LightsBlock);
            BufferSubData(GL_UNIFORM_BUFFER, AmbientLightOffset, ambientLightsData.Count * sizeof(float), ambientLightsData.ToArray());
            BufferSubData(GL_UNIFORM_BUFFER, DirLightOffset, dirLightsData.Count * sizeof(float), dirLightsData.ToArray());
            BufferSubData(GL_UNIFORM_BUFFER, PointLightOffset, pointLightsData.Count * sizeof(float), pointLightsData.ToArray());
            BufferSubData(GL_UNIFORM_BUFFER, SpotLightOffset, spotLightsData.Count * sizeof(float), spotLightsData.ToArray());
            BufferSubData(GL_UNIFORM_BUFFER, SpotLightOffset + SpotLightSize, 4 * sizeof(int), new int[] { ambientLightCount, dirLightCount, pointLightCount, spotLightCount });
        }
        #endregion

        #region Init & Dispose
        private void _Init()
        {
            if (_isInit) return;
            _isInit = true;

            _transformToDevice = (MatrixF)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            _context = CreateContextCurrent(Handle);

            Init();
            Enable(GL_BLEND);
            Enable(GL_LINE_WIDTH);
            Enable(GL_DEPTH_TEST);
            Enable(GL_FRAMEBUFFER_SRGB); // Gamma Correction
            BlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            StencilMask(1);

            _CreateResource();
            _InitModels();
            _UpdateLights();
            _OnCameraPropertyChanged(null, EventArgs.Empty);
        }

        private void _CreateResource()
        {
            MakeSureCurrentContext();

            // create default shader
            _defaultShader = _GenerateShader(_defaultShadeSource);

            #region uniform Block
            _uniformBlockObj = new uint[2];
            GenBuffers(2, _uniformBlockObj);

            // transform uniform Block
            BindBuffer(GL_UNIFORM_BUFFER, TransformBlock);
            BufferData(GL_UNIFORM_BUFFER, _GetUniformBlockSize(TransformUniformBlockName), default(float[]), GL_DYNAMIC_DRAW);
            BindBufferBase(GL_UNIFORM_BUFFER, 0, TransformBlock);
            BindBuffer(GL_UNIFORM_BUFFER, 0);

            // lights uniform Block
            BindBuffer(GL_UNIFORM_BUFFER, LightsBlock);
            BufferData(GL_UNIFORM_BUFFER, _GetUniformBlockSize(LightsUniformBlockName), default(float[]), GL_STATIC_DRAW);
            BindBufferBase(GL_UNIFORM_BUFFER, 1, LightsBlock);
            BindBuffer(GL_UNIFORM_BUFFER, 0);
            #endregion
        }

        private void _DeleteResource()
        {
            _defaultShader.Dispose();
            DeleteBuffers(2, _uniformBlockObj);

            _models.ForEach(model => model.Dispose());
            _lights.ForEach(light => light.PropertyChanged -= _OnLightPropertyChanged);

            _models = null;
            _lights = null;
            _defaultShader = null;
            _uniformBlockObj = null;
        }

        private int _GetUniformBlockSize(string uniformBlockName)
        {
            switch (uniformBlockName)
            {
                case TransformUniformBlockName:
                    return 32 * sizeof(float);
                case LightsUniformBlockName:
                    return SpotLightOffset + SpotLightSize + 4 * sizeof(int);
            }
            return 0;
        }

        private void _InitModels()
        {
            foreach (var model in _models)
                model.Init();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _DeleteResource();
            Loaded -= _OnLoaded;
            _camera.PropertyChanged -= _OnCameraPropertyChanged;
            _selector.Dispose();
            _selector = null;
            _mouseEventHandler.Dispose();
            _mouseEventHandler = null;
        }
        #endregion

        #region Aliased
        internal void EnableAliased()
        {
            MakeSureCurrentContext();
            Disable(GL_LINE_SMOOTH);
            _Refresh();
        }

        internal void DisableAliased()
        {
            MakeSureCurrentContext();
            Enable(GL_LINE_SMOOTH);
            _Refresh();
        }
        #endregion

        #region OnRender
        public void Refresh()
        {
            _Refresh();
        }

        private void _Refresh()
        {
            if (!_isInit) return;

            if (Interlocked.Increment(ref _signal) == 1)
                _timer.Change(0, Timeout.Infinite);
        }

        private void _OnRender()
        {
            var before = _watch.ElapsedMilliseconds;

            _watch.Restart();
            var old = Volatile.Read(ref _signal);
            Dispatcher.Invoke(() => { _DispatchFrame(); }, DispatcherPriority.Render);
            _watch.Stop();

            var span = Math.Max(0, (int)(_watch.ElapsedMilliseconds - before));

            if (_frameSpan > span)
                Thread.Sleep(_frameSpan - span);

            if (Interlocked.CompareExchange(ref _signal, 0, old) != old)
                _timer.Change(0, Timeout.Infinite);
        }

        private void _DispatchFrame()
        {
            if (!IsLoaded) return;
            MakeSureCurrentContext();

            ClearColor(_red, _green, _blue, 1.0f);
            ClearStencil(0);
            Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

            _DrawModels();

            SwapBuffers(_context.HDC);
        }

        private void _DrawModels()
        {
            foreach (var model in _models)
            {
                var shader = model.CustomShader ?? _defaultShader;
                shader.Use();
                shader.SetVec3("viewPos", 1, new float[] { _camera.Position.X, _camera.Position.Y, _camera.Position.Z });
                model.OnRender(shader);
            }
        }
        #endregion

        #region Override
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
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

        /// <summary>
        /// Make sure to capture mouse events
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            _Init();
            _OnRenderSizeChanged((float)sizeInfo.NewSize.Width, (float)sizeInfo.NewSize.Height);
        }

        private void _OnRenderSizeChanged(float width, float height)
        {
            MakeSureCurrentContext();

            ViewWidth = Math.Max(width * _transformToDevice.M11, 1);
            ViewHeight = Math.Max(height * _transformToDevice.M11, 1);
            Viewport(0, 0, (int)ViewWidth, (int)ViewHeight);

            switch (_camera.Type)
            {
                case CameraType.Perspective:
                    _camera.SetPerspectiveParameters(_camera.FieldOfView, ViewWidth / ViewHeight);
                    break;
                case CameraType.Orthographic:
                    _camera.SetOrthographicParameters(ViewWidth, ViewHeight);
                    break;
            }
        }
        #endregion

        #region Private func
        private Shader _GenerateShader(string[] shaders)
        {
            var header = CreateGLSLHeader();
            var source = new List<ShaderSource>();
            string code;
            foreach (var shader in shaders)
            {
                using (var stream = new StreamReader(YOpenGL.Resources.OpenStream(ShaderSourcePrefix, shader)))
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
            return Shader.GenShader(source, this);
        }

        private void _OnCameraPropertyChanged(object sender, EventArgs e)
        {
            MakeSureCurrentContext();
            #region Update View and Projection Matrix
            BindBuffer(GL_UNIFORM_BUFFER, TransformBlock);
            BufferSubData(GL_UNIFORM_BUFFER, 0, 16 * sizeof(float), _camera.ViewMatrix.GetData());
            BufferSubData(GL_UNIFORM_BUFFER, 16 * sizeof(float), 16 * sizeof(float), _camera.ProjectionMatrix.GetData());
            #endregion
            _Refresh();
        }

        private void _OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_zoomExtentWhenLoaded)
                ZoomExtents();
        }
        #endregion

        #region Internel func
        internal void MakeSureCurrentContext()
        {
            if (!_isInit) return;
            GL.MakeSureCurrentContext(_context);
        }
        #endregion

        #region public func
        public Rect3F GetBounds()
        {
            var bounds = Rect3F.Empty;
            _models.ForEach(m => bounds.Union(m.Bounds));
            return bounds;
        }

        #region HitTest
        /// <returns>返回结果按Z-Depth排序</returns>
        public IEnumerable<HitResult> HitTest(PointF pointInWpf, float sensitive = 5)
        {
            return HitTestHelper.HitTest(this, pointInWpf, sensitive, false);
        }

        public HitResult HitTestTop(PointF pointInWpf, float sensitive = 5)
        {
            return HitTestHelper.HitTest(this, pointInWpf, sensitive, true).FirstOrDefault();
        }

        public IEnumerable<RectHitResult> HitTest(RectF rectInWpf, RectHitTestMode hitTestMode)
        {
            return HitTestHelper.HitTest(this, rectInWpf, hitTestMode);
        }
        #endregion

        #endregion

        #region Interface
        public void ShaderBinding(Shader shader)
        {
            UniformBlockBinding(shader.ID, GetUniformBlockIndex(shader.ID, TransformUniformBlockName), 0);
            UniformBlockBinding(shader.ID, GetUniformBlockIndex(shader.ID, LightsUniformBlockName), 1);
        }
        #endregion

        #region Transform
        /// <summary>
        /// 获取鼠标相对于该面板的位置，请调用此方法而不是<see cref="MouseEventArgs.GetPosition(IInputElement)"/>
        /// </summary>
        /// <returns></returns>
        public PointF GetPosition()
        {
            return (PointF)Mouse.GetPosition(this) * _transformToDevice;
        }

        public Matrix3F GetNDCToWPF()
        {
            return new Matrix3F(ViewWidth / 2, 0, 0, 0,
                                0, -ViewHeight / 2, 0, 0,
                                0, 0, 1, 0,
                                ViewWidth / 2, ViewHeight / 2, 0, 1);
        }

        public Matrix3F GetWPFToNDC()
        {
            return new Matrix3F(2 / ViewWidth, 0, 0, 0,
                                0, -2 / ViewHeight, 0, 0,
                                0, 0, 1, 0,
                                -1, 1, 0, 1);
        }

        /// <param name="point">Point in world coordinate</param>
        /// <returns>Point in wpf</returns>
        public PointF Point3DToPointInWpf(Point3F point)
        {
            var transformedP = Point3DToPointInWpfWithZDpeth(point);
            return new PointF(transformedP.X, transformedP.Y);
        }

        /// <param name="point">Point in world coordinate</param>
        /// <returns>Point in wpf</returns>
        public IEnumerable<PointF> Point3DToPointInWpf(IEnumerable<Point3F> points)
        {
            foreach (var point in Point3DToPointInWpfWithZDpeth(points))
                yield return new PointF(point.X, point.Y);
        }

        /// <param name="point">Point in world coordinate</param>
        /// <returns>Point in wpf</returns>
        public Point3F Point3DToPointInWpfWithZDpeth(Point3F point)
        {
            var transform = _camera.GetTotalTransform();
            transform.Append(GetNDCToWPF());
            return point * transform;
        }

        /// <param name="points">Points in world coordinate</param>
        /// <returns>Points in wpf</returns>
        public IEnumerable<Point3F> Point3DToPointInWpfWithZDpeth(IEnumerable<Point3F> points)
        {
            var transform = _camera.GetTotalTransform();
            transform.Append(GetNDCToWPF());
            foreach (var point in points)
                yield return point * transform;
        }

        /// <param name="pointInWpf">Point in wpf</param>
        /// <param name="ZDepth">Range In [-1, 1]</param>
        /// <returns>Point in world coordinate</returns>
        public Point3F? PointInWpfToPoint3D(PointF pointInWpf, float? ZDepth = null)
        {
            var ret = new Point3F(pointInWpf.X, pointInWpf.Y, 0);
            var transform = GetWPFToNDC();
            ret *= transform;
            //var near = new Point3F(ret.X, ret.Y, -0.99f);
            //var far = new Point3F(ret.X, ret.Y, 0.99f);
            var cameraTransform = _camera.GetTotalTransform();
            if (!ZDepth.HasValue)
            {
                var pp = _camera.Target * cameraTransform;
                ret.Z = pp.Z;
            }
            else ret.Z = ZDepth.Value;

            if (cameraTransform.HasInverse)
            {
                cameraTransform.Invert();
                ret *= cameraTransform;

                //near *= cameraTransform;
                //far *= cameraTransform;
                //var dir = far - near;
                //var normal = _camera.LookDirection;
                //var dn = Vector3F.DotProduct(normal, dir);

                //var u = Vector3F.DotProduct(normal, _camera.Target - near) / dn;
                //ret = near + (u * dir);
                return ret;
            }
            else return null;
        }

        #region View
        public void FitView(Vector3F lookDirection, Vector3F upDirection)
        {
            var bounds = GetBounds();
            var diagonal = new Vector3F(bounds.SizeX, bounds.SizeY, bounds.SizeZ);

            if (bounds.IsEmpty || diagonal.LengthSquared < double.Epsilon)
                return;

            FitView(bounds, lookDirection, upDirection);
        }

        public void FitView(Rect3F bounds, Vector3F lookDirection, Vector3F upDirection)
        {
            var diagonal = new Vector3F(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            var center = bounds.Location + (diagonal * 0.5f);
            var radius = diagonal.Length * 0.5f;
            FitView(center, radius, lookDirection, upDirection);
        }

        public void FitView(
            Point3F center,
            float radius,
            Vector3F lookDirection,
            Vector3F upDirection)
        {
            var dist = 0f;
            if (_camera.Type == CameraType.Perspective)
            {
                radius *= _camera.NearPlaneDistance;
                var pcam = _camera;
                var vfov = pcam.FieldOfView;
                float distv = radius / (float)Math.Tan(MathUtil.DegreesToRadians(0.5 * vfov));
                float hfov = vfov * ViewWidth / ViewHeight;

                float disth = radius / (float)Math.Tan(MathUtil.DegreesToRadians(0.5 * hfov));
                dist = Math.Max(disth, distv);
            }
            else
            {
                var newWidth = radius * 2;

                if (ViewWidth > ViewHeight)
                    newWidth = radius * 2 * ViewWidth / ViewHeight;
                dist = newWidth;

                var radio = newWidth / _camera.Width;
                _camera.SetOrthographicParameters(newWidth, _camera.Height * radio);
            }

            var dir = lookDirection;
            dir.Normalize();
            LookAt(center, dir * dist, upDirection);
        }

        public void LookAt(Point3F target, Vector3F newLookDirection, Vector3F newUpDirection)
        {
            var newPosition = target - newLookDirection;
            _camera.SetViewParameters(newPosition, newLookDirection, newUpDirection);
        }
        #endregion

        #region Zoom
        public void ZoomExtents()
        {
            ZoomExtents(GetBounds());
        }

        public void ZoomExtents(Rect3F bounds)
        {
            if (bounds.IsEmpty) return;

            FitView(bounds, _camera.LookDirection, _camera.UpDirection);
        }

        public void Zoom(float delta, PointF zoomAround)
        {
            var zoomAround3D = PointInWpfToPoint3D(zoomAround);
            if (zoomAround3D.HasValue)
                Zoom(delta, zoomAround3D.Value);
        }

        internal void Zoom(float delta, Point3F zoomAround)
        {
            switch (_camera.Type)
            {
                case CameraType.Perspective:
                    if (_camera.Mode == CameraMode.FixedPosition)
                        _ZoomByChangingFieldOfView(delta);
                    else _ZoomByChangingCameraPosition(delta, zoomAround);
                    break;
                case CameraType.Orthographic:
                    _ZoomByChangingCameraWidth(delta, zoomAround);
                    break;
            }
        }

        private void _ZoomByChangingCameraWidth(double delta, Point3F zoomAround)
        {
            if (delta < -0.5)
                delta = -0.5;

            switch (_camera.Mode)
            {
                case CameraMode.WalkAround:
                case CameraMode.Inspect:
                case CameraMode.FixedPosition:
                    _ChangeCameraDistance(delta, zoomAround);

                    var radio = (float)Math.Pow(2.5, delta);
                    _camera.SetOrthographicParameters(_camera.Width * radio, _camera.Height * radio);
                    break;
            }
        }

        private void _ZoomByChangingFieldOfView(float delta)
        {
            float fov = _camera.FieldOfView;
            float d = _camera.LookDirection.Length;
            float r = d * (float)Math.Tan(MathUtil.DegreesToRadians(0.5 * fov));

            fov *= 1 + (delta * 0.5f);
            if (fov < 10)
                fov = 10;

            if (fov > 60)
                fov = 60;

            _camera.SetPerspectiveParameters(fov);
            float d2 = r / (float)Math.Tan(MathUtil.DegreesToRadians(0.5 * fov));
            Vector3F newLookDirection = _camera.LookDirection;
            newLookDirection.Normalize();
            newLookDirection *= d2;
            Point3F target = _camera.Target;
            _camera.SetViewParameters(target - newLookDirection, newLookDirection);
        }

        private void _ZoomByChangingCameraPosition(float delta, Point3F zoomAround)
        {
            if (delta < -0.5f)
                delta = -0.5f;

            delta *= _zoomSensitivity;
            switch (_camera.Mode)
            {
                case CameraMode.Inspect:
                    _ChangeCameraDistance(delta, zoomAround);
                    break;
                case CameraMode.WalkAround:
                    _camera.SetViewParameters(_camera.Position - _camera.LookDirection * delta);
                    break;
            }
        }

        private void _ChangeCameraDistance(double delta, Point3F zoomAround)
        {
            var target = _camera.Target;
            var relativeTarget = zoomAround - target;
            var relativePosition = zoomAround - _camera.Position;

            var f = (float)Math.Pow(2.5, delta);
            var newRelativePosition = relativePosition * f;
            var newRelativeTarget = relativeTarget * f;

            var newTarget = zoomAround - newRelativeTarget;
            var newPosition = zoomAround - newRelativePosition;
            var newLookDirection = newTarget - newPosition;

            _camera.SetViewParameters(newPosition, newLookDirection);
        }
        #endregion

        #region Translate
        /// <summary>
        /// In world coordinate
        /// </summary>
        public void Translate(Vector3F delta)
        {
            if (_camera.Mode == CameraMode.FixedPosition) return;

            _camera.SetViewParameters(_camera.Position + delta);
        }

        /// <summary>
        /// In wpf coordinate
        /// </summary>
        public void Translate(VectorF delta)
        {
            var transform = _camera.GetTotalTransform();
            transform.Append(GetNDCToWPF());
            var transformedPosition = _camera.Target * transform;
            transformedPosition.X += delta.X;
            transformedPosition.Y += delta.Y;

            if (transform.HasInverse)
            {
                transform.Invert();
                transformedPosition *= transform;
                Translate(transformedPosition - _camera.Target);
            }
        }
        #endregion

        #region Rotate
        public void RotateTrackball(PointF p1, PointF p2, Point3F rotateAround)
        {
            // http://viewport3d.com/trackball.htm
            // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
            Vector3F v1 = MathUtil.ProjectToTrackball(p1, ViewWidth, ViewHeight);
            Vector3F v2 = MathUtil.ProjectToTrackball(p2, ViewWidth, ViewHeight);

            // transform the trackball coordinates to view space
            Vector3F viewZ = _camera.LookDirection;
            Vector3F viewX = Vector3F.CrossProduct(_camera.UpDirection, viewZ);
            Vector3F viewY = Vector3F.CrossProduct(viewX, viewZ);
            viewX.Normalize();
            viewY.Normalize();
            viewZ.Normalize();
            Vector3F u1 = viewZ * v1.Z + viewX * v1.X + viewY * v1.Y;
            Vector3F u2 = viewZ * v2.Z + viewX * v2.X + viewY * v2.Y;

            // Find the rotation axis and angle
            Vector3F axis = Vector3F.CrossProduct(u1, u2);
            if (axis.LengthSquared < 1e-8)
            {
                return;
            }

            double angle = Vector3F.AngleBetween(u1, u2);

            // Create the transform
            var delta = new QuaternionF(axis, -(float)angle * _rotationSensitivity * 5);
            var center = new Point3F();
            var rotate = Matrix3F.CreateRotationMatrix(ref delta, ref center);

            // Find vectors relative to the rotate-around point
            Vector3F relativeTarget = rotateAround - _camera.Target;
            Vector3F relativePosition = rotateAround - _camera.Position;

            // Rotate the relative vectors
            Vector3F newRelativeTarget = rotate.Transform(relativeTarget);
            Vector3F newRelativePosition = rotate.Transform(relativePosition);
            Vector3F newUpDirection = rotate.Transform(_camera.UpDirection);

            // Find new camera position
            var newTarget = rotateAround - newRelativeTarget;
            var newPosition = rotateAround - newRelativePosition;

            _camera.SetViewParameters(_camera.Mode == CameraMode.Inspect ? newPosition : _camera.Position, newTarget - newPosition, newUpDirection);
        }

        public void RotateTurntable(VectorF delta, Point3F rotateAround)
        {
            Vector3F relativeTarget = rotateAround - _camera.Target;
            Vector3F relativePosition = rotateAround - _camera.Position;

            Vector3F up = _modelUpDirection;
            Vector3F dir = _camera.LookDirection;
            dir.Normalize();

            Vector3F right = Vector3F.CrossProduct(dir, _camera.UpDirection);
            right.Normalize();

            float d = -0.5f;
            if (_camera.Mode != CameraMode.Inspect)
                d *= -0.2f;

            d *= _rotationSensitivity;

            var q1 = new QuaternionF(up, d * delta.X);
            var q2 = new QuaternionF(right, d * delta.Y);
            QuaternionF q = q1 * q2;

            var m = new Matrix3F();
            m.Rotate(q);

            Vector3F newUpDirection = m.Transform(_camera.UpDirection);

            Vector3F newRelativeTarget = m.Transform(relativeTarget);
            Vector3F newRelativePosition = m.Transform(relativePosition);

            Point3F newTarget = rotateAround - newRelativeTarget;
            Point3F newPosition = rotateAround - newRelativePosition;

            _camera.SetViewParameters(_camera.Mode == CameraMode.Inspect ? newPosition : _camera.Position, newTarget - newPosition, newUpDirection);
        }

        public void RotateTurnball(PointF p1, PointF p2, Point3F rotateAround)
        {
            VectorF delta = p2 - p1;

            Vector3F relativeTarget = rotateAround - _camera.Target;
            Vector3F relativePosition = rotateAround - _camera.Position;

            float d = -1;
            if (_camera.Mode != CameraMode.Inspect)
                d = 0.2f;

            d *= _rotationSensitivity;

            var q1 = new QuaternionF(_mouseEventHandler._rotationAxisX, d * delta.X);
            var q2 = new QuaternionF(_mouseEventHandler._rotationAxisY, d * delta.Y);
            QuaternionF q = q1 * q2;

            var m = new Matrix3F();
            m.Rotate(q);

            Vector3F newLookDir = m.Transform(_camera.LookDirection);
            Vector3F newUpDirection = m.Transform(_camera.UpDirection);

            Vector3F newRelativeTarget = m.Transform(relativeTarget);
            Vector3F newRelativePosition = m.Transform(relativePosition);

            Vector3F newRightVector = Vector3F.CrossProduct(newLookDir, newUpDirection);
            newRightVector.Normalize();
            Vector3F modUpDir = Vector3F.CrossProduct(newRightVector, newLookDir);
            modUpDir.Normalize();
            if ((newUpDirection - modUpDir).Length > 1e-8)
            {
                newUpDirection = modUpDir;
            }

            Point3F newTarget = rotateAround - newRelativeTarget;
            Point3F newPosition = rotateAround - newRelativePosition;
            Vector3F newLookDirection = newTarget - newPosition;

            _camera.SetViewParameters(_camera.Mode == CameraMode.Inspect ? newPosition : _camera.Position, newLookDirection, newUpDirection);
        }
        #endregion

        #endregion
    }
}