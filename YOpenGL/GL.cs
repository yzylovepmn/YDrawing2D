using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace YOpenGL
{
    public static class GL
    {
        private static int _frameSpan;
        private static long _lastFrameTime;
        private static Stopwatch _stopwatch;
        private static Dictionary<IntPtr, ContextHandle> _contexts;
        private static ContextHandle _current;

        static GL()
        {
            _contexts = new Dictionary<IntPtr, ContextHandle>();
        }

        internal static GLVersion Version;

        public static event EventHandler DispatchFrame = delegate { };

        public static ContextHandle CreateContextCurrent(IntPtr hwnd)
        {
            var context = ContextHandle.Zero;
            var hdc = Win32Helper.GetDC(hwnd);//取得设备上下文
            PixelFormatDescriptor pfd;
            pfd.Size = 40;
            pfd.Version = 1;
            pfd.Flags = PixelFormatDescriptorFlags.DRAW_TO_WINDOW | PixelFormatDescriptorFlags.SUPPORT_OPENGL | PixelFormatDescriptorFlags.DOUBLEBUFFER;
            pfd.PixelType = PixelType.RGBA;
            pfd.ColorBits = 32;
            pfd.DepthBits = 16;

            int pixelFormat;
            unsafe
            {
                PixelFormatDescriptor* ppfd = &pfd;
                pixelFormat = Win32Helper.ChoosePixelFormat(hdc, (IntPtr)ppfd);//选择像素格式
                Win32Helper.SetPixelFormat(hdc, pixelFormat, (IntPtr)ppfd);//设置像素格式

                context = new ContextHandle(hdc, Win32Helper.CreateContext(hdc));//建立OpenGL渲染上下文

                MakeCurrentContext(context);//激活当前渲染上下文

                if (!_contexts.ContainsKey(context.HDC))
                    _contexts.Add(context.HDC, context);
                else _contexts[context.HDC] = context;
            }
            return context;
        }

        public static void MakeCurrentContext(ContextHandle context)
        {
            Win32Helper.MakeCurrent(context.HDC, context.Handle);//激活当前渲染上下文
            _current = context;
        }

        public static void MakeSureCurrentContext(ContextHandle context)
        {
            if (context != _current)
                MakeCurrentContext(context);
        }

        public static void DeleteContext(ContextHandle context)
        {
            if (_contexts.ContainsKey(context.HDC))
                _contexts.Remove(context.HDC);

            if (context != ContextHandle.Zero)
                Win32Helper.DeleteContext(context.Handle);

            if (_contexts.Count == 0)
                GLFunc.Dispose();
        }

        #region For Game Render
        public static void Start(int frameRate = 60)
        {
            _frameSpan = Math.Max(1, 1000 / frameRate);

            if (_stopwatch == null)
                _stopwatch = new Stopwatch();
            _stopwatch.Restart();
            CompositionTarget.Rendering += OnRendering;
        }

        private static void OnRendering(object sender, EventArgs e)
        {
            var tick = _stopwatch.ElapsedMilliseconds;
            if (tick - _lastFrameTime < _frameSpan && _lastFrameTime != 0)
                return;
            _lastFrameTime = tick;
            DispatchFrame(null, new EventArgs());
        }

        public static void Stop()
        {
            _stopwatch.Stop();
            CompositionTarget.Rendering -= OnRendering;
        }
        #endregion

        #region Shader
        public static string CreateGLSLHeader()
        {
            if (Version >= GLVersion.MinimumSupportedVersion)
                return string.Format("#version {0}{1}0 core\r\n", Version.Major, Version.Minor);

            return null;
        }
        #endregion
    }
}