using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public static class GL
    {
        internal static ContextHandle hdcHandle = ContextHandle.Zero;
        internal static IntPtr HDC;

        public static void SetUp(IntPtr hwnd)
        {
            if (hdcHandle != ContextHandle.Zero)
                Win32Helper.DeleteContext(hdcHandle.Handle);

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

                hdcHandle = new ContextHandle(Win32Helper.CreateContext(HDC));//建立OpenGL渲染上下文

                Win32Helper.MakeCurrent(HDC, hdcHandle.Handle);//激活当前渲染上下文
            }
        }
    }
}