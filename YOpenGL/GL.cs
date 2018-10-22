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
        private static ContextHandle _hdcHandle = ContextHandle.Zero;
        private static int _frameSpan;
        private static long _lastFrameTime;
        private static Stopwatch _stopwatch;

        internal static IntPtr HDC;
        internal static GLVersion Version;

        public static event EventHandler DispatchFrame = delegate { };

        public static void MakeContextCurrent(IntPtr hwnd)
        {
            DeleteContext();

            HDC = Win32Helper.GetDC(hwnd);//取得设备上下文
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
                pixelFormat = Win32Helper.ChoosePixelFormat(HDC, (IntPtr)ppfd);//选择像素格式
                Win32Helper.SetPixelFormat(HDC, pixelFormat, (IntPtr)ppfd);//设置像素格式

                _hdcHandle = new ContextHandle(Win32Helper.CreateContext(HDC));//建立OpenGL渲染上下文

                Win32Helper.MakeCurrent(HDC, _hdcHandle.Handle);//激活当前渲染上下文
            }
        }

        public static void DeleteContext()
        {
            if (_hdcHandle != ContextHandle.Zero)
                Win32Helper.DeleteContext(_hdcHandle.Handle);
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
            if (Version >= GLVersion.FromNumber(3, 3))
                return string.Format("#version {0}{1}0 core\r\n", Version.Major, Version.Minor);

            if (Version == GLVersion.FromNumber(2, 0))
                return "#version 110\r\n";

            if (Version == GLVersion.FromNumber(2, 1))
                return "#version 120\r\n";

            if (Version == GLVersion.FromNumber(3, 0))
                return "#version 130 core\r\n";

            if (Version == GLVersion.FromNumber(3, 1))
                return "#version 140 core\r\n";

            if (Version == GLVersion.FromNumber(3, 2))
                return "#version 150 core\r\n";
            return null;
        }
        #endregion
    }
}