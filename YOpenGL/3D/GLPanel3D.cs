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
    public enum OperateMode
    {
        Disable,
        Free,
        RotateOnly,
        TranslateOnly
    }

    public class GLPanel3D : HwndHost, INotifyPropertyChanged, IGLContext, IDisposable
    {
        public const string TransformUniformBlockName = "Matrices";
        public const string LightsUniformBlockName = "Lights";
        private const string ShaderSourcePrefix = "YOpenGL._3D.Shaders.";
        private static readonly string[] _defaultShadeSource = new string[] { "default.vert", "default.frag" };
        private static readonly string[] _postProcessShadeSource = new string[] { "PostProcess.vert", "PostProcess.frag" };

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
            _visuals = new List<GLVisual3D>();
            _lights = new List<Light>();
            _camera = new Camera(this, CameraType.Perspective, 1000, 800, 10, float.PositiveInfinity, new Point3F(0, 0, 1), new Vector3F(0, 0, -1), new Vector3F(0, 1, 0));
            _camera.PropertyChanged += _OnCameraPropertyChanged;
            _mouseEventHandler = new MouseEventHandler(this);
            _mouseEventHandler.AttachEvents(this);
            _selector = new RectSelector(this);

            _zoomExtentWhenLoaded = true;
            _rotateAroundMouse = false;
            _isRotateEnable = true;
            _isTranslateEnable = true;
            _isZoomEnable = true;
            _zoomSensitivity = 1;
            _rotationSensitivity = 1;
            _modelUpDirection = new Vector3F(0, 0, 1);
            _minOrthographicCameraWidth = 10;
            _maxOrthographicCameraWidth = 10000;
            _minPerspectiveCameraDistance = 10;
            _maxPerspectiveCameraDistance = 5000;
            _operateMode = OperateMode.Free;

            BackgroundColor = backgroundColor;
            Focusable = true;

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

        public IEnumerable<GLVisual3D> Visuals { get { return _visuals; } }
        private List<GLVisual3D> _visuals;

        public IEnumerable<Light> Lights { get { return _lights; } }
        private List<Light> _lights;

        public ContextHandle Context { get { return _context; } }
        private ContextHandle _context;

        public Camera Camera { get { return _camera; } }
        private Camera _camera;

        private Shader _defaultShader;
        private Shader _postProcessShader;

        public RectSelector Selector { get { return _selector; } }
        private RectSelector _selector;

        public MouseEventHandler EventHook { get { return _mouseEventHandler; } }
        private MouseEventHandler _mouseEventHandler;

        internal bool IsInit { get { return _isInit; } }
        private bool _isInit;

        private bool _isRenderSizeChanging;
        private int _signal;
        private int _frameSpan;
        private Timer _timer;
        private Stopwatch _watch;
        public float ViewWidth;
        public float ViewHeight;

        #region Matrix
        private MatrixF _transformToDevice;
        private Matrix3F _totalTransform;
        #endregion

        #region Buffer Object
        private uint[] _uniformBlockObj;

        internal uint TransformBlock { get { return _uniformBlockObj[0]; } }

        internal uint LightsBlock { get { return _uniformBlockObj[1]; } }
        #endregion

        #region Texture
        private uint[] _texture_dash;
        #endregion

        #region MSAA
        private uint[] _fbo;
        private uint[] _texture_msaa;
        private uint[] _rbo;
        #endregion

        #region Post Process
        private uint[] _post_vao;
        private uint[] _post_vbo;
        #endregion

        public Vector3F ModelUpDirection
        {
            get { return _modelUpDirection; }
            set
            {
                if (_modelUpDirection != value)
                {
                    _modelUpDirection = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("ModelUpDirection"));
                }
            }
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

        public bool RotateAroundMouse
        {
            get { return _rotateAroundMouse; }
            set
            {
                if (_rotateAroundMouse != value)
                {
                    _rotateAroundMouse = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("RotateAroundMouse"));
                }
            }
        }
        private bool _rotateAroundMouse;

        public bool IsTranslateEnable
        {
            get { return _isTranslateEnable; }
            set
            {
                if (_isTranslateEnable != value)
                {
                    _isTranslateEnable = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsTranslateEnable"));
                }
            }
        }
        private bool _isTranslateEnable;

        public bool IsRotateEnable
        {
            get { return _isRotateEnable; }
            set
            {
                if (_isRotateEnable != value)
                {
                    _isRotateEnable = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsRotateEnable"));
                }
            }
        }
        private bool _isRotateEnable;

        public bool IsZoomEnable
        {
            get { return _isZoomEnable; }
            set
            {
                if (_isZoomEnable != value)
                {
                    _isZoomEnable = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsZoomEnable"));
                }
            }
        }
        private bool _isZoomEnable;

        public float MaxOrthographicCameraWidth
        {
            get { return _maxOrthographicCameraWidth; }
            set
            {
                if (_maxOrthographicCameraWidth != value)
                {
                    _maxOrthographicCameraWidth = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxOrthographicCameraWidth"));
                }
            }
        }
        private float _maxOrthographicCameraWidth;

        public float MinOrthographicCameraWidth
        {
            get { return _minOrthographicCameraWidth; }
            set
            {
                if (_minOrthographicCameraWidth != value)
                {
                    _minOrthographicCameraWidth = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MinOrthographicCameraWidth"));
                }
            }
        }
        private float _minOrthographicCameraWidth;

        public float MaxPerspectiveCameraDistance
        {
            get { return _maxPerspectiveCameraDistance; }
            set
            {
                if (_maxPerspectiveCameraDistance != value)
                {
                    _maxPerspectiveCameraDistance = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxPerspectiveCameraDistance"));
                }
            }
        }
        private float _maxPerspectiveCameraDistance;

        public float MinPerspectiveCameraDistance
        {
            get { return _minPerspectiveCameraDistance; }
            set 
            {
                if (_minPerspectiveCameraDistance != value)
                {
                    _minPerspectiveCameraDistance = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("MinPerspectiveCameraDistance"));
                }
            }
        }
        private float _minPerspectiveCameraDistance;

        public OperateMode OperateMode
        {
            get { return _operateMode; }
            set 
            { 
                if (_operateMode != value)
                {
                    _operateMode = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("OperateMode"));
                }
            }
        }
        private OperateMode _operateMode;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler OnViewportChanged = delegate { };
        #endregion

        #region Models
        public void AddVisuals(IEnumerable<GLVisual3D> visuals, bool isRefresh = true)
        {
            MakeSureCurrentContext();
            foreach (var visual in visuals)
                _AddVisual(visual);
            if (isRefresh)
                _Refresh();
        }

        public void AddVisual(GLVisual3D visual, bool isRefresh = true)
        {
            MakeSureCurrentContext();
            _AddVisual(visual);
            if (isRefresh)
                _Refresh();
        }

        private void _AddVisual(GLVisual3D visual)
        {
            if (_visuals.Contains(visual))
                throw new InvalidOperationException("The visual has been added");

            if (visual.Viewport != null)
                throw new InvalidOperationException("The visual has a logical parent");

            _visuals.Add(visual);
            visual.Viewport = this;
            if (_isInit)
                visual.Init();
        }

        public void RemoveAllVisuals()
        {
            MakeSureCurrentContext();
            foreach (var visual in _visuals)
            {
                visual.Clean();
                visual.Viewport = null;
            }
            _visuals.Clear();
            _Refresh();
        }

        public void RemoveVisuals(IEnumerable<GLVisual3D> visuals, bool isRefresh = true)
        {
            MakeSureCurrentContext();
            foreach (var visual in visuals)
                _RemoveVisual(visual);
            if (isRefresh)
                _Refresh();
        }

        public void RemoveVisual(GLVisual3D visual, bool isRefresh = true)
        {
            MakeSureCurrentContext();
            _RemoveVisual(visual);
            if (isRefresh)
                _Refresh();
        }

        private void _RemoveVisual(GLVisual3D visual)
        {
            if (!_visuals.Contains(visual))
                throw new InvalidOperationException("model does not exist!");

            visual.Clean();
            _visuals.Remove(visual);
            visual.Viewport = null;
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

            MakeSureCurrentContext();
            Init();
            Enable(GL_BLEND);
            Enable(GL_LINE_WIDTH);
            Enable(GL_DEPTH_TEST);
            Enable(GL_FRAMEBUFFER_SRGB); // Gamma Correction
            BlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
            StencilMask(1);
            EnableAliased();

            _CreateResource();
            _InitVisuals();
            _UpdateLights();
            _OnCameraPropertyChanged(null, EventArgs.Empty);
        }

        private void _CreateResource()
        {
            // create default shader
            _defaultShader = _GenerateShader(_defaultShadeSource);

            // create PostProcessResource
            _CreatePostProcessResource();

            // for dash line texture
            _texture_dash = new uint[1];
            GenTextures(1, _texture_dash);
            BindTexture(GL_TEXTURE_1D, _texture_dash[0]);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_WRAP_S, GL_REPEAT);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            TexParameteri(GL_TEXTURE_1D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            BindTexture(GL_TEXTURE_1D, 0);

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

            // DestroyPostProcessResource
            _DestroyPostProcessResource();

            DeleteTextures(1, _texture_dash);
            DeleteBuffers(2, _uniformBlockObj);

            _visuals.ForEach(visual => visual.Dispose());
            _lights.ForEach(light => light.PropertyChanged -= _OnLightPropertyChanged);

            _visuals = null;
            _lights = null;
            _defaultShader = null;
            _uniformBlockObj = null;
            _texture_dash = null;
        }

        private void _CreatePostProcessResource()
        {
            _postProcessShader = _GenerateShader(_postProcessShadeSource);
            _post_vao = new uint[1];
            _post_vbo = new uint[1];
            GenVertexArrays(1, _post_vao);
            GenBuffers(1, _post_vbo);

            BindVertexArray(_post_vao[0]);
            BindBuffer(GL_ARRAY_BUFFER, _post_vbo[0]);
            float[] quadVertices = {
                -1.0f,  1.0f,  0.0f, 1.0f,
                -1.0f, -1.0f,  0.0f, 0.0f,
                 1.0f, -1.0f,  1.0f, 0.0f,

                -1.0f,  1.0f,  0.0f, 1.0f,
                 1.0f, -1.0f,  1.0f, 0.0f,
                 1.0f,  1.0f,  1.0f, 1.0f
            };
            BufferData(GL_ARRAY_BUFFER, quadVertices.Length * sizeof(float), quadVertices, GL_STATIC_DRAW);
            EnableVertexAttribArray(0);
            VertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 4 * sizeof(float), 0);
            EnableVertexAttribArray(1);
            VertexAttribPointer(1, 2, GL_FLOAT, GL_FALSE, 4 * sizeof(float), 2 * sizeof(float));
            BindVertexArray(0);
        }

        private void _DestroyPostProcessResource()
        {
            _postProcessShader.Dispose();
            DeleteVertexArrays(1, _post_vao);
            DeleteBuffers(1, _post_vbo);
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
            RenderbufferStorageMultisample(GL_RENDERBUFFER, 4, GL_DEPTH24_STENCIL8, (int)(SystemParameters.PrimaryScreenWidth * _transformToDevice.M11), (int)(SystemParameters.PrimaryScreenHeight * _transformToDevice.M11));
            FramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, _rbo[0]);
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

        private int _GetUniformBlockSize(string uniformBlockName)
        {
            switch (uniformBlockName)
            {
                case TransformUniformBlockName:
                    return 48 * sizeof(float);
                case LightsUniformBlockName:
                    return SpotLightOffset + SpotLightSize + 4 * sizeof(int);
            }
            return 0;
        }

        private void _InitVisuals()
        {
            foreach (var visual in _visuals)
                visual.Init();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _isInit = false;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;

            MakeSureCurrentContext();
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
        public void EnableAliased()
        {
            MakeSureCurrentContext();
            //Disable(GL_LINE_SMOOTH);
            _DeleteFrameBuffer();
            _Refresh();
        }

        public void DisableAliased()
        {
            MakeSureCurrentContext();
            //Enable(GL_LINE_SMOOTH);
            _CreateFrameBuffer();
            _Refresh();
        }
        #endregion

        #region OnRender
        public void Refresh()
        {
            _Refresh();
        }

        private async void _Refresh()
        {
            await Task.Factory.StartNew(() => 
            {
                if (!_isInit) return;

                if (Interlocked.Increment(ref _signal) == 1)
                    _timer.Change(0, Timeout.Infinite);
            });
        }

        private void _OnRender()
        {
            _watch.Restart();
            var old = Volatile.Read(ref _signal);
            Dispatcher.Invoke(() => { _DispatchFrame(); }, DispatcherPriority.Render);
            _watch.Stop();

            var span = (int)_watch.ElapsedMilliseconds;

            if (_frameSpan > span)
                Thread.Sleep(_frameSpan - span);

            if (Interlocked.CompareExchange(ref _signal, 0, old) != old)
                _timer.Change(0, Timeout.Infinite);
        }

        private void _DispatchFrame()
        {
            if (!IsLoaded) return;
            MakeSureCurrentContext();

            if (_fbo != null)
                BindFramebuffer(GL_FRAMEBUFFER, _fbo[0]);

            ClearColor(_red, _green, _blue, 1.0f);
            ClearStencil(0);
            Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

            _DrawVisuals();

            if (_fbo != null)
            {
                //BindFramebuffer(GL_READ_FRAMEBUFFER, _fbo[0]);
                //BindFramebuffer(GL_DRAW_FRAMEBUFFER, 0);
                //BlitFramebuffer(0, 0, (int)ViewWidth, (int)ViewHeight, 0, 0, (int)ViewWidth, (int)ViewHeight, GL_COLOR_BUFFER_BIT, GL_NEAREST);
                _DrawPostQuad();
            }

            SwapBuffers(_context.HDC);
        }

        private void _DrawVisuals()
        {
            var shader = _defaultShader;
            shader.Use();
            shader.SetVec3("viewPos", 1, new float[] { _camera.Position.X, _camera.Position.Y, _camera.Position.Z });
            BindTexture(GL_TEXTURE_1D, _texture_dash[0]);

            foreach (var visual in _visuals)
                visual.OnRender(shader);
        }

        private void _DrawPostQuad()
        {
            BindFramebuffer(GL_FRAMEBUFFER, 0);
            ClearColor(_red, _green, _blue, 1.0f);
            Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

            _postProcessShader.Use();
            BindVertexArray(_post_vao[0]);
            ActiveTexture(GL_TEXTURE1);
            BindTexture(GL_TEXTURE_2D_MULTISAMPLE, _texture_msaa[0]);
            _postProcessShader.SetVec2("screenSize", 1, new float[] { ViewWidth, ViewHeight });
            _postProcessShader.SetInt("textureMS", 1);
            DrawArrays(GL_TRIANGLES, 0, 6);
            BindVertexArray(0);
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
            _isRenderSizeChanging = true;
            base.OnRenderSizeChanged(sizeInfo);
            _Init();
            _OnRenderSizeChanged((float)sizeInfo.NewSize.Width, (float)sizeInfo.NewSize.Height);
            _isRenderSizeChanging = false;
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
                    _camera.SetOrthographicParameters(ViewWidth / ViewHeight * _camera.Height, _camera.Height);
                    break;
            }

            _UpdateTotalTransform();
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
            if (!_isInit) return;

            MakeSureCurrentContext();
            _UpdateTotalTransform(true);
            #region Update View and Projection Matrix
            BindBuffer(GL_UNIFORM_BUFFER, TransformBlock);
            BufferSubData(GL_UNIFORM_BUFFER, 0, 16 * sizeof(float), _camera.ViewMatrix.GetData());
            BufferSubData(GL_UNIFORM_BUFFER, 16 * sizeof(float), 16 * sizeof(float), _camera.ProjectionMatrix.GetData());
            BufferSubData(GL_UNIFORM_BUFFER, 32 * sizeof(float), 16 * sizeof(float), _camera.TotalTransform.GetData());
            #endregion
            if (!_isRenderSizeChanging)
                _Refresh();
        }

        private void _OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_zoomExtentWhenLoaded)
                ZoomExtents();
        }

        private void _UpdateTotalTransform(bool isInvokeByCamera = false)
        {
            _totalTransform = _camera.TotalTransform * GetNDCToWPF();

            if (!isInvokeByCamera || !_isRenderSizeChanging)
            {
                OnViewportChanged(this, EventArgs.Empty);
                _UpdateDashedModels();
            }
        }

        private void _UpdateDashedModels()
        {
            foreach (var visual in _visuals.Where(v => v.IsVisible && v.Model != null))
                visual.Model.UpdateDistance();
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
            foreach (var visual in _visuals.Where(visual => visual.IsVisible && visual.Model != null))
                bounds.Union(visual.Model.Bounds);
            return bounds;
        }

        public void CullFace(bool isEnable)
        {
            MakeSureCurrentContext();
            if (isEnable)
                Enable(GL_CULL_FACE);
            else Disable(GL_CULL_FACE);
        }

        #region HitTest
        /// <returns>返回结果按Z-Depth排序</returns>
        public IEnumerable<HitResult> HitTest(PointF pointInWpf, float sensitive = 5)
        {
            return HitTestHelper.HitTest(this, pointInWpf, sensitive);
        }

        public HitResult HitTestTop(PointF pointInWpf, float sensitive = 5)
        {
            return HitTestHelper.HitTest(this, pointInWpf, sensitive).FirstOrDefault();
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

        public VectorF Vector3DToVectorInWpf(Vector3F vector)
        {
            var vecInWpf = vector * _totalTransform;
            return new VectorF(vecInWpf.X, vecInWpf.Y);
        }

        public IEnumerable<VectorF> Vector3DToVectorInWpf(IEnumerable<Vector3F> vectors)
        {
            foreach (var vector in vectors)
                yield return Vector3DToVectorInWpf(vector);
        }

        /// <param name="point">Point in world coordinate</param>
        /// <returns>Point in wpf</returns>
        public Point3F Point3DToPointInWpfWithZDpeth(Point3F point)
        {
            //if (_camera.Type == CameraType.Orthographic)
                return point * _totalTransform;
            //else
            //{
            //    var pp = (Point4F)point * _totalTransform;
            //    return new Point3F(pp.X / pp.W, pp.Y / pp.W, pp.Z / pp.W);
            //}
        }

        /// <param name="points">Points in world coordinate</param>
        /// <returns>Points in wpf</returns>
        public IEnumerable<Point3F> Point3DToPointInWpfWithZDpeth(IEnumerable<Point3F> points)
        {
            //if (_camera.Type == CameraType.Orthographic)
            //{
                foreach (var point in points)
                    yield return point * _totalTransform;
            //}
            //else
            //{
            //    foreach (var point in points)
            //    {
            //        var pp = (Point4F)point * _totalTransform;
            //        yield return new Point3F(pp.X / pp.W, pp.Y / pp.W, pp.Z / pp.W);
            //    }
            //}
        }

        public bool CanPointInWpfTransformToPoint3D()
        {
            return _camera.TotalTransformReverse.HasValue;
        }

        /// <param name="pointInWpf">Point in wpf</param>
        /// <param name="zDepth">Range In [-1, 1]</param>
        /// <returns>Point in world coordinate</returns>
        public Point3F? PointInWpfToPoint3D(PointF pointInWpf, float? zDepth = null)
        {
            var ret = new Point3F(pointInWpf.X, pointInWpf.Y, 0);
            var transform = GetWPFToNDC();
            ret *= transform;
            var cameraTransformReverse = _camera.TotalTransformReverse;

            if (!zDepth.HasValue)
            {
                var p = _camera.Target * _camera.TotalTransform;
                ret.Z = p.Z;
            }
            else ret.Z = zDepth.Value;

            if (cameraTransformReverse.HasValue)
            {
                ret *= cameraTransformReverse.Value;
                return ret;
            }
            else return null;
        }

        public Vector3F? VectorInWpfToVector3D(VectorF vectorInWpf, float? zDepth = null)
        {
            var ret = new Vector3F(vectorInWpf.X, vectorInWpf.Y, 0);
            var transform = GetWPFToNDC();
            ret *= transform;
            var cameraTransformReverse = _camera.TotalTransformReverse;

            if (!zDepth.HasValue)
            {
                var p = _camera.Target * _camera.TotalTransform;
                ret.Z = p.Z;
            }
            else ret.Z = zDepth.Value;

            if (cameraTransformReverse.HasValue)
            {
                ret *= cameraTransformReverse.Value;
                return ret;
            }
            else return null;
        }

        #region View
        public void FitView(Vector3F lookDirection, Vector3F upDirection)
        {
            FitView(GetBounds(), lookDirection, upDirection);
        }

        public void FitView(GLVisual3D visual)
        {
            FitView(visual, _camera.LookDirection, _camera.UpDirection);
        }

        public void FitView(GLVisual3D visual, Vector3F lookDirection, Vector3F upDirection)
        {
            if (visual.Model != null)
                FitView(visual.Model.Bounds, lookDirection, upDirection);
        }

        public void FitView(Rect3F bounds, Vector3F lookDirection, Vector3F upDirection)
        {
            if (bounds.IsVolumeEmpty)
                return;

            var diagonal = new Vector3F(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            if (diagonal.LengthSquared < 1e-10)
                return;

            var center = bounds.Location + (diagonal * 0.5f);
            var radius = diagonal.Length * 0.5f;
            FitView(center, radius, lookDirection, upDirection);
        }

        internal void FitView(
            Point3F center,
            float radius,
            Vector3F lookDirection,
            Vector3F upDirection)
        {
            var dist = 0f;
            var newWidth = radius * 2;
            _camera.Lock();
            if (_camera.Type == CameraType.Perspective)
            {
                var fov = radius / (_camera.NearPlaneDistance * 2);
                fov = (float)MathUtil.RadiansToDegrees(Math.Atan(fov)) * 2;
                _camera.SetPerspectiveParameters((float)fov);
                dist = _camera.NearPlaneDistance * 2;
            }
            else
            {
                if (ViewWidth > ViewHeight)
                    newWidth = radius * 2 * ViewWidth / ViewHeight;
                newWidth = Math.Max(_minOrthographicCameraWidth, Math.Min(_maxOrthographicCameraWidth, newWidth));
                dist = newWidth;

                var radio = newWidth / _camera.Width;
                _camera.SetOrthographicParameters(newWidth, _camera.Height * radio);
            }
            _camera.UnLock();
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
            FitView(bounds, _camera.LookDirection, _camera.UpDirection);
        }

        public void Zoom(float delta, PointF zoomAround)
        {
            if (!_isZoomEnable) return;
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
                    var radio = (float)Math.Pow(2.5, delta);
                    var newWidth = _camera.Width * radio;

                    if (newWidth > _maxOrthographicCameraWidth || newWidth < _minOrthographicCameraWidth)
                    {
                        MathUtil.Clamp(ref newWidth, _minOrthographicCameraWidth, _maxOrthographicCameraWidth);
                        radio = newWidth / _camera.Width;
                        delta = Math.Log(radio, 2.5);
                    }

                    if (radio != 1)
                    {
                        _camera.Lock();
                        _ChangeCameraPosition(delta, zoomAround);
                        _camera.UnLock();
                        _camera.SetOrthographicParameters(newWidth, _camera.Height * radio);
                    }
                    break;
            }
        }

        private void _ZoomByChangingFieldOfView(float delta)
        {
            float fov = _camera.FieldOfView;

            fov *= 1 + (delta * 0.5f);
            if (fov < 5)
                fov = 5;

            if (fov > 120)
                fov = 120;

            _camera.SetPerspectiveParameters(fov);
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

            var newLookDirection = newRelativePosition - newRelativeTarget;
            var len = newLookDirection.Length;
            if (len < _minPerspectiveCameraDistance || len > _maxPerspectiveCameraDistance)
            {
                var oldLen = len;
                MathUtil.Clamp(ref len, _minPerspectiveCameraDistance, _maxPerspectiveCameraDistance);
                var radio = len / oldLen;
                newRelativePosition *= radio;
                newRelativeTarget *= radio;
                newLookDirection = newRelativePosition - newRelativeTarget;
            }
            var newPosition = zoomAround - newRelativePosition;

            _camera.SetViewParameters(newPosition, newLookDirection);
        }

        private void _ChangeCameraPosition(double delta, Point3F zoomAround)
        {
            var view = _camera.ViewMatrix;

            var target = _camera.Target;
            var relativePosition = zoomAround - _camera.Position;

            var f = (float)Math.Pow(2.5, delta);
            var newRelativePosition = relativePosition * f;
            var newPosition = zoomAround - newRelativePosition;
            newPosition *= view;
            newPosition.Z = 0;
            view.Invert();
            newPosition *= view;

            _camera.SetViewParameters(newPosition, _camera.LookDirection);
        }
        #endregion

        #region Translate
        /// <summary>
        /// In world coordinate
        /// </summary>
        public void Translate(Vector3F delta)
        {
            if (_camera.Mode == CameraMode.FixedPosition || !_isTranslateEnable) return;

            _camera.SetViewParameters(_camera.Position + delta);
        }

        /// <summary>
        /// In wpf coordinate
        /// </summary>
        public void Translate(VectorF delta)
        {
            if (_camera.Mode == CameraMode.FixedPosition || !_isTranslateEnable) return;

            var transformedPosition = Point3DToPointInWpf(_camera.Target);
            transformedPosition.X += delta.X;
            transformedPosition.Y += delta.Y;

            var newTarget = PointInWpfToPoint3D(transformedPosition);
            if (newTarget.HasValue)
                Translate(newTarget.Value - _camera.Target);
        }
        #endregion

        #region Rotate
        public void RotateTrackball(PointF p1, PointF p2, Point3F rotateAround)
        {
            if (!_isRotateEnable) return;

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
            if (!_isRotateEnable) return;

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
            if (!_isRotateEnable) return;

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