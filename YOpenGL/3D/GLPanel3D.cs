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

namespace YOpenGL._3D
{
    public class GLPanel3D : HwndHost, IGLContext, IDisposable
    {
        public const string TransformUniformBlockName = "Matrices";
        private const string ShaderSourcePrefix = "YOpenGL._3D.Shaders.";
        private static readonly string[] _defaultShadeSource = new string[] { "default.vert", "default.frag" };

        public GLPanel3D(Color backgroundColor, float frameRate = 60)
        {
            _isInit = false;
            _timer = new Timer(_OnRender);
            _watch = new Stopwatch();
            _frameSpan = Math.Max(1, (int)(1000 / frameRate));
            _camera = new Camera(this, CameraType.Perspective, 1000, 800, 1, float.PositiveInfinity, new Point3F(0, 0, 100), new Vector3F(0, 0, -100), new Vector3F(0, 1, 0));
            _models = new List<GLModel3D>();
            _mouseEventHandler = new MouseEventHandler(this);
            _zoomSensitivity = 1;
            _rotationSensitivity = 1;
            _modelUpDirection = new Vector3F(0, 0, 1);
            _zoomExtentOnLoaded = true;
            _rotateAroundMouseDownPoint = true;

            Focusable = true;
            BackgroundColor = backgroundColor;
            Loaded += _OnLoaded;
        }

        private void _OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_zoomExtentOnLoaded)
                ZoomExtents();
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

        public ContextHandle Context { get { return _context; } }
        private ContextHandle _context;

        public Camera Camera { get { return _camera; } }
        private Camera _camera;

        public Shader DefaultShader { get { return _defaultShader; } }
        private Shader _defaultShader;

        private MouseEventHandler _mouseEventHandler;

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
        private uint[] _transformUniformBlockObj;
        #endregion
        public Vector3F ModelUpDirection
        {
            get { return _modelUpDirection; }
            set { _modelUpDirection = value; }
        }
        private Vector3F _modelUpDirection;

        public float ZoomSensitivity 
        { 
            get { return _zoomSensitivity; } 
            set { _zoomSensitivity = Math.Min(Math.Max(0.1f, value), 100); }
        }
        private float _zoomSensitivity;

        public float RotationSensitivity
        {
            get { return _rotationSensitivity; }
            set { _rotationSensitivity = Math.Min(Math.Max(0.1f, value), 100); }
        }
        private float _rotationSensitivity;

        public bool RotateAroundMouseDownPoint { get { return _rotateAroundMouseDownPoint; } set { _rotateAroundMouseDownPoint = value; } }
        private bool _rotateAroundMouseDownPoint;

        public bool ZoomExtentOnLoaded { get { return _zoomExtentOnLoaded; } set { _zoomExtentOnLoaded = value; } }
        private bool _zoomExtentOnLoaded;
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
                throw new InvalidOperationException("model does not exist in collection");

            model.Clean();
            _models.Remove(model);
            model.Viewport = null;
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
            Enable(GL_FRAMEBUFFER_SRGB); // Gamma Correction
            BlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            StencilMask(1);

            _CreateResource();
            _InitModels();
        }

        private void _CreateResource()
        {
            MakeSureCurrentContext();

            // create default shader
            _defaultShader = _GenerateShader(_defaultShadeSource);

            // for transform
            _transformUniformBlockObj = new uint[1];
            GenBuffers(1, _transformUniformBlockObj);
            BindBuffer(GL_UNIFORM_BUFFER, _transformUniformBlockObj[0]);
            BufferData(GL_UNIFORM_BUFFER, 32 * sizeof(float), default(float[]), GL_STATIC_DRAW);
            BindBufferBase(GL_UNIFORM_BUFFER, 0, _transformUniformBlockObj[0]);
            BindBuffer(GL_UNIFORM_BUFFER, 0);
        }

        private void _InitModels()
        {
            foreach (var model in _models)
                model.Init();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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

            var span = (int)(_watch.ElapsedMilliseconds - before);

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

            #region Update View and Projection Matrix
            BindBuffer(GL_UNIFORM_BUFFER, _transformUniformBlockObj[0]);
            BufferSubData(GL_UNIFORM_BUFFER, 0, 16 * sizeof(float), _camera.ViewMatrix.GetData());
            BufferSubData(GL_UNIFORM_BUFFER, 16 * sizeof(float), 16 * sizeof(float), _camera.ProjectionMatrix.GetData());
            #endregion

            _DrawModels();

            SwapBuffers(_context.HDC);
        }

        private void _DrawModels()
        {
            foreach (var model in _models)
            {
                var shader = model.Shader ?? _defaultShader;
                shader.Use();
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
            switch (_camera.Type)
            {
                case CameraType.Perspective:
                    _camera.SetPerspectiveParameters(_camera.FieldOfView, ViewWidth / ViewHeight);
                    break;
                case CameraType.Orthographic:
                    _camera.SetOrthographicParameters(ViewWidth, ViewHeight);
                    break;
            }

            ZoomExtents();
            Viewport(0, 0, (int)ViewWidth, (int)ViewHeight);
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
        #endregion

        #region Interface
        public void ShaderBinding(Shader shader)
        {
            UniformBlockBinding(shader.ID, GetUniformBlockIndex(shader.ID, TransformUniformBlockName), 0);
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
        public PointF Point3DToPoint2D(Point3F point)
        {
            var transform = _camera.GetTotalTransform();
            transform.Append(GetNDCToWPF());
            var transformedP = transform.Transform(point);
            return new PointF(transformedP.X, transformedP.Y);
        }

        /// <param name="point">Point in wpf</param>
        /// <param name="ZDepth">Range In [-1, 1]</param>
        /// <returns>Point in world coordinate</returns>
        public Point3F? Point2DToPoint3D(PointF point, float? ZDepth = null)
        {
            var ret = new Point3F(point.X, point.Y, 0);
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
            if (_camera.Type == CameraType.Perspective)
            {
                var pcam = _camera;
                var vfov = pcam.FieldOfView;
                float distv = radius / (float)Math.Tan(0.5 * vfov * Math.PI / 180);
                float hfov = vfov * ViewWidth / ViewHeight;

                float disth = radius / (float)Math.Tan(0.5 * hfov * Math.PI / 180);
                float dist = Math.Max(disth, distv);
                var dir = lookDirection;
                dir.Normalize();
                LookAt(center, dir * dist, upDirection);
                return;
            }
            else
            {
                LookAt(center, lookDirection, upDirection);
                var newWidth = radius * 2;

                if (ViewWidth > ViewHeight)
                    newWidth = radius * 2 * ViewWidth / ViewHeight;

                var radio = newWidth / _camera.Width;
                _camera.SetOrthographicParameters(newWidth, _camera.Height * radio);
            }
        }

        public void LookAt(Point3F target, Vector3F newLookDirection, Vector3F newUpDirection)
        {
            var newPosition = target - newLookDirection;
            _camera.SetViewParameters(newPosition, newLookDirection, newUpDirection);
            _Refresh();
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
            var zoomAround3D = Point2DToPoint3D(zoomAround);
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
            _Refresh();
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
            _Refresh();
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
            _Refresh();
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
            _Refresh();
        }

        public void RotateTurnball(PointF p1, PointF p2, Point3F rotateAround)
        {
            VectorF delta = p2 - p1;

            Vector3F relativeTarget = rotateAround - _camera.Target;
            Vector3F relativePosition = rotateAround - _camera.Position;

            float d = -1;
            if (_camera.Mode != CameraMode.Inspect)
                d = 0.2f;

            d *= this.RotationSensitivity;

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
            _Refresh();
        }
        #endregion

        #endregion
    }
}