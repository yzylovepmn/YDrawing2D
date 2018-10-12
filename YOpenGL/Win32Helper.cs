using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace YOpenGL
{
    public static class Win32Helper
    {
        [DllImport("kernel32.dll")]
        public extern static IntPtr LoadLibrary(string path);
        [DllImport("kernel32.dll")]
        public extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
        [DllImport("kernel32.dll")]
        public extern static bool FreeLibrary(IntPtr lib);

        [DllImport("user32.dll", ExactSpelling = false, EntryPoint = "GetDC", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll", ExactSpelling = false, EntryPoint = "ChoosePixelFormat", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern int ChoosePixelFormat(IntPtr hdc, IntPtr ppfd);

        [DllImport("gdi32.dll", ExactSpelling = false, EntryPoint = "SetPixelFormat", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetPixelFormat(IntPtr hdc, int format, IntPtr ppfd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglCreateContext", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr CreateContext(IntPtr hDc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglDeleteContext", ExactSpelling = true, SetLastError = true)]
        internal extern static Boolean DeleteContext(IntPtr oldContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglGetCurrentContext", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetCurrentContext();

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglMakeCurrent", ExactSpelling = true, SetLastError = true)]
        internal extern static Boolean MakeCurrent(IntPtr hDc, IntPtr newContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglGetCurrentDC", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetCurrentDC();

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglGetProcAddress", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetProcAddress(String lpszProc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglGetProcAddress", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetProcAddress(IntPtr lpszProc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglGetPixelFormat", ExactSpelling = true, SetLastError = true)]
        internal extern static int GetPixelFormat(IntPtr hdc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglSwapBuffers", ExactSpelling = true, SetLastError = true)]
        internal extern static Boolean SwapBuffers(IntPtr hdc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(GLFunc.OpenGLName, EntryPoint = "wglShareLists", ExactSpelling = true, SetLastError = true)]
        internal extern static Boolean ShareLists(IntPtr hrcSrvShare, IntPtr hrcSrvSource);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PixelFormatDescriptor
    {
        internal short Size;
        internal short Version;
        internal PixelFormatDescriptorFlags Flags;
        internal PixelType PixelType;
        internal byte ColorBits;
        internal byte RedBits;
        internal byte RedShift;
        internal byte GreenBits;
        internal byte GreenShift;
        internal byte BlueBits;
        internal byte BlueShift;
        internal byte AlphaBits;
        internal byte AlphaShift;
        internal byte AccumBits;
        internal byte AccumRedBits;
        internal byte AccumGreenBits;
        internal byte AccumBlueBits;
        internal byte AccumAlphaBits;
        internal byte DepthBits;
        internal byte StencilBits;
        internal byte AuxBuffers;
        internal byte LayerType;
        private byte Reserved;
        internal int LayerMask;
        internal int VisibleMask;
        internal int DamageMask;
    }

    internal enum PixelType : byte
    {
        RGBA = 0,
        INDEXED = 1
    }

    [Flags]
    internal enum PixelFormatDescriptorFlags : int
    {
        // PixelFormatDescriptor flags
        DOUBLEBUFFER = 0x01,
        STEREO = 0x02,
        DRAW_TO_WINDOW = 0x04,
        DRAW_TO_BITMAP = 0x08,
        SUPPORT_GDI = 0x10,
        SUPPORT_OPENGL = 0x20,
        GENERIC_FORMAT = 0x40,
        NEED_PALETTE = 0x80,
        NEED_SYSTEM_PALETTE = 0x100,
        SWAP_EXCHANGE = 0x200,
        SWAP_COPY = 0x400,
        SWAP_LAYER_BUFFERS = 0x800,
        GENERIC_ACCELERATED = 0x1000,
        SUPPORT_DIRECTDRAW = 0x2000,
        SUPPORT_COMPOSITION = 0x8000,

        // PixelFormatDescriptor flags for use in ChoosePixelFormat only
        DEPTH_DONTCARE = unchecked((int)0x20000000),
        DOUBLEBUFFER_DONTCARE = unchecked((int)0x40000000),
        STEREO_DONTCARE = unchecked((int)0x80000000)
    }
}