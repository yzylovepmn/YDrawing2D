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
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

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

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll")]
        internal static extern IntPtr DispatchMessage(ref MSG msg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll")]
        internal static extern Boolean TranslateMessage(ref MSG lpMsg);

        [DllImport("User32.dll")]
        internal static extern Int32 GetMessage(ref MSG msg,
            IntPtr windowHandle, int messageFilterMin, int messageFilterMax);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("kernel32.dll")]
        internal static extern void SetLastError(Int32 dwErrCode);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern Int32 SetWindowLongInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, Int32 dwNewLong);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtrInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, IntPtr dwNewLong);

        internal static IntPtr SetWindowLong(IntPtr handle, WindowProcedure newValue)
        {
            return SetWindowLong(handle, GetWindowLongOffsets.WNDPROC, Marshal.GetFunctionPointerForDelegate(newValue));
        }

        internal static IntPtr SetWindowLong(IntPtr handle, GetWindowLongOffsets item, IntPtr newValue)
        {
            // SetWindowPos defines its error condition as an IntPtr.Zero retval and a non-0 GetLastError.
            // We need to SetLastError(0) to ensure we are not detecting on older error condition (from another function).

            IntPtr retval = IntPtr.Zero;
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                retval = new IntPtr(SetWindowLongInternal(handle, item, newValue.ToInt32()));
            }
            else
            {
                retval = SetWindowLongPtrInternal(handle, item, newValue);
            }

            if (retval == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new InvalidOperationException(String.Format("Failed to modify window border. Error: {0}", error));
                }
            }

            return retval;
        }

        public const int
              WS_CHILD = 0x40000000,
              WS_VISIBLE = 0x10000000,
              WS_VSCROLL = 0x00200000,
              WS_BORDER = 0x00800000,
              WS_CLIPSIBLINGS = 0x04000000,
              WS_CLIPCHILDREN = 0x02000000,
              WS_TABSTOP = 0x00010000,
              WS_GROUP = 0x00020000;

        public const int WM_WINDOWPOSCHANGED = 0x0047;
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        public const int WM_NCMOUSEMOVE = 0xa0;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int WM_NCLBUTTONUP = 0xA2;
        public const int WM_NCLBUTTONDBLCLK = 0xA3;
        public const int WM_NCRBUTTONDOWN = 0xA4;
        public const int WM_NCRBUTTONUP = 0xA5;
        public const int WM_CAPTURECHANGED = 0x0215;
        public const int WM_EXITSIZEMOVE = 0x0232;
        public const int WM_ENTERSIZEMOVE = 0x0231;
        public const int WM_MOVE = 0x0003;
        public const int WM_MOVING = 0x0216;
        public const int WM_KILLFOCUS = 0x0008;
        public const int WM_SETFOCUS = 0x0007;
        public const int WM_ACTIVATE = 0x0006;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_INITMENUPOPUP = 0x0117;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;

        public const int WA_INACTIVE = 0x0000;

        public const int WM_SYSCOMMAND = 0x0112;
        // These are the wParam of WM_SYSCOMMAND
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_RESTORE = 0xF120;
    }

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
        /// the system posts the request to the thread that owns the window. This prevents the calling thread from
        /// blocking its execution while other threads process the request.</summary>
        /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
        SynchronousWindowPosition = 0x4000,
        /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
        /// <remarks>SWP_DEFERERASE</remarks>
        DeferErase = 0x2000,
        /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
        /// <remarks>SWP_DRAWFRAME</remarks>
        DrawFrame = 0x0020,
        /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
        /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
        /// is sent only when the window's size is being changed.</summary>
        /// <remarks>SWP_FRAMECHANGED</remarks>
        FrameChanged = 0x0020,
        /// <summary>Hides the window.</summary>
        /// <remarks>SWP_HIDEWINDOW</remarks>
        HideWindow = 0x0080,
        /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
        /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        /// parameter).</summary>
        /// <remarks>SWP_NOACTIVATE</remarks>
        DoNotActivate = 0x0010,
        /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
        /// contents of the client area are saved and copied back into the client area after the window is sized or
        /// repositioned.</summary>
        /// <remarks>SWP_NOCOPYBITS</remarks>
        DoNotCopyBits = 0x0100,
        /// <summary>Retains the current position (ignores X and Y parameters).</summary>
        /// <remarks>SWP_NOMOVE</remarks>
        IgnoreMove = 0x0002,
        /// <summary>Does not change the owner window's position in the Z order.</summary>
        /// <remarks>SWP_NOOWNERZORDER</remarks>
        DoNotChangeOwnerZOrder = 0x0200,
        /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
        /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
        /// window uncovered as a result of the window being moved. When this flag is set, the application must
        /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
        /// <remarks>SWP_NOREDRAW</remarks>
        DoNotRedraw = 0x0008,
        /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
        /// <remarks>SWP_NOREPOSITION</remarks>
        DoNotReposition = 0x0200,
        /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
        /// <remarks>SWP_NOSENDCHANGING</remarks>
        DoNotSendChangingEvent = 0x0400,
        /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
        /// <remarks>SWP_NOSIZE</remarks>
        IgnoreResize = 0x0001,
        /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
        /// <remarks>SWP_NOZORDER</remarks>
        IgnoreZOrder = 0x0004,
        /// <summary>Displays the window.</summary>
        /// <remarks>SWP_SHOWWINDOW</remarks>
        ShowWindow = 0x0040,
    }

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr WindowProcedure(IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam);

    internal enum GetWindowLongOffsets : int
    {
        WNDPROC = (-4),
        HINSTANCE = (-6),
        HWNDPARENT = (-8),
        STYLE = (-16),
        EXSTYLE = (-20),
        USERDATA = (-21),
        ID = (-12),
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        internal IntPtr HWnd;
        internal WindowMessage Message;
        internal IntPtr WParam;
        internal IntPtr LParam;
        internal uint Time;
        internal POINT Point;

        public override string ToString()
        {
            return String.Format("msg=0x{0:x} ({1}) hwnd=0x{2:x} wparam=0x{3:x} lparam=0x{4:x} pt=0x{5:x}", (int)Message, Message.ToString(), HWnd.ToInt32(), WParam.ToInt32(), LParam.ToInt32(), Point);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        internal int X;
        internal int Y;

        internal POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        internal PointF ToPoint()
        {
            return new PointF(X, Y);
        }

        public override string ToString()
        {
            return "Point {" + X.ToString() + ", " + Y.ToString() + ")";
        }
    }

    internal enum WindowMessage : int
    {
        NULL = 0x0000,
        CREATE = 0x0001,
        DESTROY = 0x0002,
        MOVE = 0x0003,
        SIZE = 0x0005,
        ACTIVATE = 0x0006,
        SETFOCUS = 0x0007,
        KILLFOCUS = 0x0008,
        //              internal const uint SETVISIBLE           = 0x0009;
        ENABLE = 0x000A,
        SETREDRAW = 0x000B,
        SETTEXT = 0x000C,
        GETTEXT = 0x000D,
        GETTEXTLENGTH = 0x000E,
        PAINT = 0x000F,
        CLOSE = 0x0010,
        QUERYENDSESSION = 0x0011,
        QUIT = 0x0012,
        QUERYOPEN = 0x0013,
        ERASEBKGND = 0x0014,
        SYSCOLORCHANGE = 0x0015,
        ENDSESSION = 0x0016,
        //              internal const uint SYSTEMERROR          = 0x0017;
        SHOWWINDOW = 0x0018,
        CTLCOLOR = 0x0019,
        WININICHANGE = 0x001A,
        SETTINGCHANGE = 0x001A,
        DEVMODECHANGE = 0x001B,
        ACTIVATEAPP = 0x001C,
        FONTCHANGE = 0x001D,
        TIMECHANGE = 0x001E,
        CANCELMODE = 0x001F,
        SETCURSOR = 0x0020,
        MOUSEACTIVATE = 0x0021,
        CHILDACTIVATE = 0x0022,
        QUEUESYNC = 0x0023,
        GETMINMAXINFO = 0x0024,
        PAINTICON = 0x0026,
        ICONERASEBKGND = 0x0027,
        NEXTDLGCTL = 0x0028,
        //              internal const uint ALTTABACTIVE         = 0x0029;
        SPOOLERSTATUS = 0x002A,
        DRAWITEM = 0x002B,
        MEASUREITEM = 0x002C,
        DELETEITEM = 0x002D,
        VKEYTOITEM = 0x002E,
        CHARTOITEM = 0x002F,
        SETFONT = 0x0030,
        GETFONT = 0x0031,
        SETHOTKEY = 0x0032,
        GETHOTKEY = 0x0033,
        //              internal const uint FILESYSCHANGE        = 0x0034;
        //              internal const uint ISACTIVEICON         = 0x0035;
        //              internal const uint QUERYPARKICON        = 0x0036;
        QUERYDRAGICON = 0x0037,
        COMPAREITEM = 0x0039,
        //              internal const uint TESTING              = 0x003a;
        //              internal const uint OTHERWINDOWCREATED = 0x003c;
        GETOBJECT = 0x003D,
        //                      internal const uint ACTIVATESHELLWINDOW        = 0x003e;
        COMPACTING = 0x0041,
        COMMNOTIFY = 0x0044,
        WINDOWPOSCHANGING = 0x0046,
        WINDOWPOSCHANGED = 0x0047,
        POWER = 0x0048,
        COPYDATA = 0x004A,
        CANCELJOURNAL = 0x004B,
        NOTIFY = 0x004E,
        INPUTLANGCHANGEREQUEST = 0x0050,
        INPUTLANGCHANGE = 0x0051,
        TCARD = 0x0052,
        HELP = 0x0053,
        USERCHANGED = 0x0054,
        NOTIFYFORMAT = 0x0055,
        CONTEXTMENU = 0x007B,
        STYLECHANGING = 0x007C,
        STYLECHANGED = 0x007D,
        DISPLAYCHANGE = 0x007E,
        GETICON = 0x007F,

        // Non-Client messages
        SETICON = 0x0080,
        NCCREATE = 0x0081,
        NCDESTROY = 0x0082,
        NCCALCSIZE = 0x0083,
        NCHITTEST = 0x0084,
        NCPAINT = 0x0085,
        NCACTIVATE = 0x0086,
        GETDLGCODE = 0x0087,
        SYNCPAINT = 0x0088,
        //              internal const uint SYNCTASK       = 0x0089;
        NCMOUSEMOVE = 0x00A0,
        NCLBUTTONDOWN = 0x00A1,
        NCLBUTTONUP = 0x00A2,
        NCLBUTTONDBLCLK = 0x00A3,
        NCRBUTTONDOWN = 0x00A4,
        NCRBUTTONUP = 0x00A5,
        NCRBUTTONDBLCLK = 0x00A6,
        NCMBUTTONDOWN = 0x00A7,
        NCMBUTTONUP = 0x00A8,
        NCMBUTTONDBLCLK = 0x00A9,
        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        NCXBUTTONDOWN = 0x00ab,
        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        NCXBUTTONUP = 0x00ac,
        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        NCXBUTTONDBLCLK = 0x00ad,

        INPUT = 0x00FF,

        KEYDOWN = 0x0100,
        KEYFIRST = 0x0100,
        KEYUP = 0x0101,
        CHAR = 0x0102,
        DEADCHAR = 0x0103,
        SYSKEYDOWN = 0x0104,
        SYSKEYUP = 0x0105,
        SYSCHAR = 0x0106,
        SYSDEADCHAR = 0x0107,
        KEYLAST = 0x0108,
        IME_STARTCOMPOSITION = 0x010D,
        IME_ENDCOMPOSITION = 0x010E,
        IME_COMPOSITION = 0x010F,
        IME_KEYLAST = 0x010F,
        INITDIALOG = 0x0110,
        COMMAND = 0x0111,
        SYSCOMMAND = 0x0112,
        TIMER = 0x0113,
        HSCROLL = 0x0114,
        VSCROLL = 0x0115,
        INITMENU = 0x0116,
        INITMENUPOPUP = 0x0117,
        //              internal const uint SYSTIMER       = 0x0118;
        MENUSELECT = 0x011F,
        MENUCHAR = 0x0120,
        ENTERIDLE = 0x0121,
        MENURBUTTONUP = 0x0122,
        MENUDRAG = 0x0123,
        MENUGETOBJECT = 0x0124,
        UNINITMENUPOPUP = 0x0125,
        MENUCOMMAND = 0x0126,

        CHANGEUISTATE = 0x0127,
        UPDATEUISTATE = 0x0128,
        QUERYUISTATE = 0x0129,

        //              internal const uint LBTRACKPOINT     = 0x0131;
        CTLCOLORMSGBOX = 0x0132,
        CTLCOLOREDIT = 0x0133,
        CTLCOLORLISTBOX = 0x0134,
        CTLCOLORBTN = 0x0135,
        CTLCOLORDLG = 0x0136,
        CTLCOLORSCROLLBAR = 0x0137,
        CTLCOLORSTATIC = 0x0138,
        MOUSEMOVE = 0x0200,
        MOUSEFIRST = 0x0200,
        LBUTTONDOWN = 0x0201,
        LBUTTONUP = 0x0202,
        LBUTTONDBLCLK = 0x0203,
        RBUTTONDOWN = 0x0204,
        RBUTTONUP = 0x0205,
        RBUTTONDBLCLK = 0x0206,
        MBUTTONDOWN = 0x0207,
        MBUTTONUP = 0x0208,
        MBUTTONDBLCLK = 0x0209,
        MOUSEWHEEL = 0x020A,
        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        XBUTTONDOWN = 0x020B,
        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        XBUTTONUP = 0x020C,
        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        XBUTTONDBLCLK = 0x020D,
        /// <summary>
        /// Windows Vista and higher only.
        /// </summary>
        MOUSEHWHEEL = 0x020E,
        PARENTNOTIFY = 0x0210,
        ENTERMENULOOP = 0x0211,
        EXITMENULOOP = 0x0212,
        NEXTMENU = 0x0213,
        SIZING = 0x0214,
        CAPTURECHANGED = 0x0215,
        MOVING = 0x0216,
        //              internal const uint POWERBROADCAST   = 0x0218;
        DEVICECHANGE = 0x0219,
        MDICREATE = 0x0220,
        MDIDESTROY = 0x0221,
        MDIACTIVATE = 0x0222,
        MDIRESTORE = 0x0223,
        MDINEXT = 0x0224,
        MDIMAXIMIZE = 0x0225,
        MDITILE = 0x0226,
        MDICASCADE = 0x0227,
        MDIICONARRANGE = 0x0228,
        MDIGETACTIVE = 0x0229,
        /* D&D messages */
        //              internal const uint DROPOBJECT     = 0x022A;
        //              internal const uint QUERYDROPOBJECT  = 0x022B;
        //              internal const uint BEGINDRAG      = 0x022C;
        //              internal const uint DRAGLOOP       = 0x022D;
        //              internal const uint DRAGSELECT     = 0x022E;
        //              internal const uint DRAGMOVE       = 0x022F;
        MDISETMENU = 0x0230,
        ENTERSIZEMOVE = 0x0231,
        EXITSIZEMOVE = 0x0232,
        DROPFILES = 0x0233,
        MDIREFRESHMENU = 0x0234,
        IME_SETCONTEXT = 0x0281,
        IME_NOTIFY = 0x0282,
        IME_CONTROL = 0x0283,
        IME_COMPOSITIONFULL = 0x0284,
        IME_SELECT = 0x0285,
        IME_CHAR = 0x0286,
        IME_REQUEST = 0x0288,
        IME_KEYDOWN = 0x0290,
        IME_KEYUP = 0x0291,
        NCMOUSEHOVER = 0x02A0,
        MOUSEHOVER = 0x02A1,
        NCMOUSELEAVE = 0x02A2,
        MOUSELEAVE = 0x02A3,
        CUT = 0x0300,
        COPY = 0x0301,
        PASTE = 0x0302,
        CLEAR = 0x0303,
        UNDO = 0x0304,
        RENDERFORMAT = 0x0305,
        RENDERALLFORMATS = 0x0306,
        DESTROYCLIPBOARD = 0x0307,
        DRAWCLIPBOARD = 0x0308,
        PAINTCLIPBOARD = 0x0309,
        VSCROLLCLIPBOARD = 0x030A,
        SIZECLIPBOARD = 0x030B,
        ASKCBFORMATNAME = 0x030C,
        CHANGECBCHAIN = 0x030D,
        HSCROLLCLIPBOARD = 0x030E,
        QUERYNEWPALETTE = 0x030F,
        PALETTEISCHANGING = 0x0310,
        PALETTECHANGED = 0x0311,
        HOTKEY = 0x0312,
        PRINT = 0x0317,
        PRINTCLIENT = 0x0318,
        HANDHELDFIRST = 0x0358,
        HANDHELDLAST = 0x035F,
        AFXFIRST = 0x0360,
        AFXLAST = 0x037F,
        PENWINFIRST = 0x0380,
        PENWINLAST = 0x038F,
        APP = 0x8000,
        USER = 0x0400,

        // Our "private" ones
        MOUSE_ENTER = 0x0401,
        ASYNC_MESSAGE = 0x0403,
        REFLECT = USER + 0x1c00,
        CLOSE_INTERNAL = USER + 0x1c01,

        // NotifyIcon (Systray) Balloon messages
        BALLOONSHOW = USER + 0x0002,
        BALLOONHIDE = USER + 0x0003,
        BALLOONTIMEOUT = USER + 0x0004,
        BALLOONUSERCLICK = USER + 0x0005
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