using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

//Change default DllImport CharSet from Ansi to Unicode.
[module: DefaultCharSet(CharSet.Unicode)]
//[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32|DllImportSearchPath.UserDirectories)]

namespace Catkeys.Winapi
{
	[DebuggerStepThrough]
	[CLSCompliant(false)]
	public static unsafe partial class Api
	{
		public static uint SizeOf<T>(T v) { return (uint)Marshal.SizeOf(typeof(T)); }
		//speed: in Release same as with ref and same as plain Marshal.SizeOf(typeof(TYPE)). The object overload is almost 2 times slower (need boxing etc).

		//Tried to make function that creates new struct and sets its first int member = sizeof struct. But cannot get address of generic parameter.
		//public static void StructInitSize<T>(out T v) where T :struct
		//{
		//	v=new T();
		//	int* cbSize = &v; //error when generic
		//	*cbSize=Marshal.SizeOf(typeof(T));
		//}

		/// <summary>
		/// If o is not null, calls <see cref="Marshal.ReleaseComObject"/>.
		/// </summary>
		public static void ReleaseComObject(object o)
		{
			if(o != null) Marshal.ReleaseComObject(o);
		}

		//USER32

		public struct COPYDATASTRUCT
		{
			public LPARAM dwData;
			public int cbData;
			public IntPtr lpData;

			public COPYDATASTRUCT(LPARAM dwData, int cbData, IntPtr lpData)
			{
				this.dwData = dwData; this.cbData = cbData; this.lpData = lpData;
			}
		}

		[DllImport("user32.dll")]
		public static extern bool IsWindow(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsWindowVisible(Wnd hWnd);

		public const int SW_HIDE = 0;
		public const int SW_SHOWNORMAL = 1;
		public const int SW_SHOWMINIMIZED = 2;
		public const int SW_SHOWMAXIMIZED = 3;
		//public const int SW_SHOWNOACTIVATE = 4; //restores min/max window
		public const int SW_SHOW = 5;
		public const int SW_MINIMIZE = 6;
		public const int SW_SHOWMINNOACTIVE = 7;
		public const int SW_SHOWNA = 8;
		public const int SW_RESTORE = 9;
		public const int SW_SHOWDEFAULT = 10;
		public const int SW_FORCEMINIMIZE = 11;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShowWindow(Wnd hWnd, int SW_X);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsWindowEnabled(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnableWindow(Wnd hWnd, bool bEnable);

		[DllImport("user32.dll", EntryPoint = "FindWindowW")]
		public static extern Wnd FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", EntryPoint = "FindWindowExW")]
		public static extern Wnd FindWindowEx(Wnd hWndParent, Wnd hWndChildAfter, string lpszClass, string lpszWindow);

		public delegate LPARAM WNDPROC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam);
		public delegate int DLGPROC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam);

		public struct WNDCLASSEX
		{
			public uint cbSize;
			public uint style;
			public IntPtr lpfnWndProc; //not WNDPROC to avoid auto-marshaling where don't need. Use Marshal.GetFunctionPointerForDelegate/GetDelegateForFunctionPointer.
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance;
			public IntPtr hIcon;
			public IntPtr hCursor;
			public IntPtr hbrBackground;
			public IntPtr lpszMenuName;
			public IntPtr lpszClassName; //not string because CLR would call CoTaskMemFree
			public IntPtr hIconSm;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

		[DllImport("user32.dll", EntryPoint = "GetClassInfoExW", SetLastError = true)]
		public static extern ushort GetClassInfoEx(IntPtr hInstance, string lpszClass, ref WNDCLASSEX lpwcx);

		[DllImport("user32.dll", EntryPoint = "UnregisterClassW", SetLastError = true)]
		public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

		[DllImport("user32.dll", EntryPoint = "UnregisterClassW", SetLastError = true)]
		public static extern bool UnregisterClass(uint classAtom, IntPtr hInstance);

		[DllImport("user32.dll", EntryPoint = "CreateWindowExW", SetLastError = true)]
		public static extern Wnd CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, Wnd hWndParent, LPARAM hMenu, IntPtr hInstance, LPARAM lpParam);

		[DllImport("user32.dll", EntryPoint = "DefWindowProcW")]
		public static extern LPARAM DefWindowProc(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll", EntryPoint = "CallWindowProcW")]
		public static extern LPARAM CallWindowProc(WNDPROC lpPrevWndFunc, Wnd hWnd, uint Msg, LPARAM wParam, LPARAM lParam);

		[DllImport("user32.dll")]
		public static extern bool DestroyWindow(Wnd hWnd);

		[DllImport("user32.dll")]
		public static extern void PostQuitMessage(int nExitCode);

		//[DllImport("user32.dll", EntryPoint = "MessageBoxW", SetLastError = true)]
		//public static extern int MessageBox(Wnd hWnd, string text, string caption, int options);

		public struct MSG { public Wnd hwnd; public uint message; public LPARAM wParam; public LPARAM lParam; public uint time; public POINT pt; }

		[DllImport("user32.dll")]
		public static extern int GetMessage(out MSG lpMsg, Wnd hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

		[DllImport("user32.dll")]
		public static extern bool TranslateMessage(ref MSG lpMsg);

		[DllImport("user32.dll")]
		public static extern LPARAM DispatchMessage(ref MSG lpmsg);

		[DllImport("user32.dll")]
		public static extern bool WaitMessage();

		public const uint PM_NOREMOVE = 0x0;
		public const uint PM_REMOVE = 0x1;
		public const uint PM_NOYIELD = 0x2;
		public const uint PM_QS_SENDMESSAGE = 0x400000;
		public const uint PM_QS_POSTMESSAGE = 0x980000;
		public const uint PM_QS_PAINT = 0x200000;
		public const uint PM_QS_INPUT = 0x1C070000;

		[DllImport("user32.dll", EntryPoint = "PeekMessageW")]
		public static extern bool PeekMessage(out MSG lpMsg, Wnd hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

		//public const int WH_MSGFILTER = -1;
		//public const int WH_JOURNALRECORD = 0;
		//public const int WH_JOURNALPLAYBACK = 1;
		//public const int WH_KEYBOARD = 2;
		public const int WH_GETMESSAGE = 3;
		//public const int WH_CALLWNDPROC = 4;
		public const int WH_CBT = 5;
		//public const int WH_SYSMSGFILTER = 6;
		//public const int WH_MOUSE = 7;
		//public const int WH_DEBUG = 9;
		//public const int WH_SHELL = 10;
		//public const int WH_FOREGROUNDIDLE = 11;
		//public const int WH_CALLWNDPROCRET = 12;
		public const int WH_KEYBOARD_LL = 13;
		public const int WH_MOUSE_LL = 14;

		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(int WH_X, HOOKPROC lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll")]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll")]
		public static extern LPARAM CallNextHookEx(IntPtr hhk, int nCode, LPARAM wParam, LPARAM lParam);

		public const int HCBT_MOVESIZE = 0;
		public const int HCBT_MINMAX = 1;
		//public const int HCBT_QS = 2;
		public const int HCBT_CREATEWND = 3;
		public const int HCBT_DESTROYWND = 4;
		public const int HCBT_ACTIVATE = 5;
		public const int HCBT_CLICKSKIPPED = 6;
		public const int HCBT_KEYSKIPPED = 7;
		public const int HCBT_SYSCOMMAND = 8;
		public const int HCBT_SETFOCUS = 9;

		public const int HC_ACTION = 0;

		public delegate LPARAM HOOKPROC(int HCBT_X, LPARAM wParam, LPARAM lParam);

		public const int GA_PARENT = 1;
		public const int GA_ROOT = 2;
		public const int GA_ROOTOWNER = 3;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd GetAncestor(Wnd hwnd, uint GA_X);

		[DllImport("user32.dll")]
		public static extern Wnd GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(Wnd hWnd);

		[DllImport("user32.dll")]
		public static extern bool AllowSetForegroundWindow(uint dwProcessId);

		public const uint LSFW_LOCK = 1;
		public const uint LSFW_UNLOCK = 2;

		[DllImport("user32.dll")]
		public static extern bool LockSetForegroundWindow(uint LSFW_X);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd SetFocus(Wnd hWnd);

		[DllImport("user32.dll")]
		public static extern Wnd GetFocus();

		[DllImport("user32.dll")]
		public static extern Wnd SetActiveWindow(Wnd hWnd);

		[DllImport("user32.dll")]
		public static extern Wnd GetActiveWindow();

		public const uint SWP_NOSIZE = 0x1;
		public const uint SWP_NOMOVE = 0x2;
		public const uint SWP_NOZORDER = 0x4;
		public const uint SWP_NOREDRAW = 0x8;
		public const uint SWP_NOACTIVATE = 0x10;
		public const uint SWP_FRAMECHANGED = 0x20;
		public const uint SWP_SHOWWINDOW = 0x40;
		public const uint SWP_HIDEWINDOW = 0x80;
		public const uint SWP_NOCOPYBITS = 0x100;
		public const uint SWP_NOOWNERZORDER = 0x200;
		public const uint SWP_NOSENDCHANGING = 0x400;
		public const uint SWP_DEFERERASE = 0x2000;
		public const uint SWP_ASYNCWINDOWPOS = 0x4000;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(Wnd hWnd, Wnd hWndInsertAfter, int X, int Y, int cx, int cy, uint SWP_X);

		#region WindowLong, ClassLong

		public const int GWL_WNDPROC = -4;
		public const int GWL_USERDATA = -21;
		public const int GWL_STYLE = -16;
		public const int GWL_ID = -12;
		public const int GWL_HWNDPARENT = -8;
		public const int GWL_HINSTANCE = -6;
		public const int GWL_EXSTYLE = -20;
		//info: also there are GWLP_, but their values are the same.

		//#define DWLP_MSGRESULT  0
		//#define DWLP_DLGPROC    DWLP_MSGRESULT + sizeof(LRESULT)
		//#define DWLP_USER       DWLP_DLGPROC + sizeof(DLGPROC)
		public const int DWLP_MSGRESULT = 0;
		public static int DWLP_DLGPROC { get { return IntPtr.Size; } }
		public static int DWLP_USER { get { return IntPtr.Size * 2; } }
		public const int DWL_USER = 8;

		public const int GCW_ATOM = -32;
		public const int GCL_WNDPROC = -24;
		public const int GCL_STYLE = -26;
		public const int GCL_MENUNAME = -8;
		public const int GCL_HMODULE = -16;
		public const int GCL_HICONSM = -34;
		public const int GCL_HICON = -14;
		public const int GCL_HCURSOR = -12;
		public const int GCL_HBRBACKGROUND = -10;
		public const int GCL_CBWNDEXTRA = -18;
		public const int GCL_CBCLSEXTRA = -20;
		//info: also there are GCLP_, but their values are the same.

		#endregion

		[DllImport("user32.dll")]
		public static extern bool InvalidateRect(Wnd hWnd, [In] ref RECT lpRect, bool bErase);

		[DllImport("user32.dll")]
		public static extern bool InvalidateRect(Wnd hWnd, IntPtr lpRect, bool bErase);

		[DllImport("user32.dll")]
		public static extern bool FlashWindow(Wnd hWnd, bool bInvert);

		public struct FLASHWINFO
		{
			public uint cbSize;
			public Wnd hwnd;
			public uint dwFlags;
			public uint uCount;
			public uint dwTimeout;
		}

		[DllImport("user32.dll")]
		public static extern bool FlashWindowEx(ref FLASHWINFO pfwi);

		public const int GW_OWNER = 4;
		public const int GW_MAX = 6;
		public const int GW_HWNDPREV = 3;
		public const int GW_HWNDNEXT = 2;
		public const int GW_HWNDLAST = 1;
		public const int GW_HWNDFIRST = 0;
		public const int GW_ENABLEDPOPUP = 6;
		public const int GW_CHILD = 5;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd GetWindow(Wnd hWnd, uint GW_X);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd GetTopWindow(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd GetParent(Wnd hWnd);

		[DllImport("user32.dll")]
		public static extern Wnd GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern Wnd GetShellWindow();

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd GetLastActivePopup(Wnd hWnd);

		[DllImport("user32.dll")]
		public static extern bool IntersectRect(out RECT lprcDst, ref RECT lprcSrc1, ref RECT lprcSrc2);

		[DllImport("user32.dll")]
		public static extern bool UnionRect(out RECT lprcDst, ref RECT lprcSrc1, ref RECT lprcSrc2);

		//Gets DPI physical cursor pos, ie always in pixels.
		//The classic GetCursorPos API gets logical pos. Also it has a bug: randomly gets physical pos, even for same point.
		//Make sure that the process is DPI-aware.
		[DllImport("user32.dll", EntryPoint = "GetPhysicalCursorPos")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		[DllImport("user32.dll", EntryPoint = "LoadImageW")]
		public static extern IntPtr LoadImage(IntPtr hInst, string name, uint type, int cx, int cy, uint LR_X);
		[DllImport("user32.dll", EntryPoint = "LoadImageW")]
		public static extern IntPtr LoadImage(IntPtr hInst, LPARAM resId, uint type, int cx, int cy, uint LR_X);

		[DllImport("user32.dll")]
		public static extern IntPtr CopyImage(IntPtr h, uint type, int cx, int cy, uint flags);

		[DllImport("user32.dll")]
		public static extern IntPtr CopyIcon(IntPtr hIcon); //3 times slower than CopyImage. But CopyImage maybe cannot be used to copy icons retrieved from other processes, eg with WM_GETICON.

		[DllImport("user32.dll")]
		public static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("user32.dll")]
		public static extern bool DestroyCursor(IntPtr hCursor);

		//public const int MONITOR_DEFAULTTONULL = 0;
		//public const int MONITOR_DEFAULTTOPRIMARY = 1;
		//public const int MONITOR_DEFAULTTONEAREST = 2;

		//[DllImport("user32.dll")]
		//public static extern IntPtr MonitorFromWindow(Wnd hwnd, uint dwFlags);

		//[DllImport("user32.dll")]
		//public static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

		//[DllImport("user32.dll")]
		//public static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

		//public struct MONITORINFO
		//{
		//	public uint cbSize;
		//	public RECT rcMonitor;
		//	public RECT rcWork;
		//	public uint dwFlags;
		//}

		//[DllImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
		//public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

		//public delegate int MONITORENUMPROC(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rMonitor, LPARAM dwData);

		//[DllImport("user32.dll")]
		//public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MONITORENUMPROC lpfnEnum, LPARAM dwData);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowRect(Wnd hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetClientRect(Wnd hWnd, out RECT lpRect);

		public const uint WPF_SETMINPOSITION = 1;
		public const uint WPF_RESTORETOMAXIMIZED = 2;
		public const uint WPF_ASYNCWINDOWPLACEMENT = 4;

		public struct WINDOWPLACEMENT
		{
			public uint length;
			public uint flags; //WPF_
			public int showCmd; //SW_
			public POINT ptMinPosition;
			public POINT ptMaxPosition;
			public RECT rcNormalPosition;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowPlacement(Wnd hWnd, ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPlacement(Wnd hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		public struct WINDOWINFO
		{
			public uint cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public uint dwStyle;
			public uint dwExStyle;
			public uint dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			public ushort atomWindowType;
			public ushort wCreatorVersion;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetWindowInfo(Wnd hwnd, ref WINDOWINFO pwi);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsZoomed(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsIconic(Wnd hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(Wnd hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsWindowUnicode(Wnd hWnd);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool IsWow64Process(IntPtr hProcess, out int Wow64Process);


		[DllImport("user32.dll", EntryPoint = "GetPropW", SetLastError = true)]
		public static extern LPARAM GetProp(Wnd hWnd, string lpString);

		[DllImport("user32.dll", EntryPoint = "GetPropW", SetLastError = true)]
		//public static extern LPARAM GetProp(Wnd hWnd, [MarshalAs(UnmanagedType.SysInt)] ushort atom); //exception, must be U2
		public static extern LPARAM GetProp(Wnd hWnd, LPARAM atom);

		[DllImport("user32.dll", EntryPoint = "SetPropW", SetLastError = true)]
		public static extern bool SetProp(Wnd hWnd, string lpString, LPARAM hData);

		[DllImport("user32.dll", EntryPoint = "SetPropW", SetLastError = true)]
		public static extern bool SetProp(Wnd hWnd, LPARAM atom, LPARAM hData);

		[DllImport("user32.dll", EntryPoint = "RemovePropW", SetLastError = true)]
		public static extern LPARAM RemoveProp(Wnd hWnd, string lpString);

		[DllImport("user32.dll", EntryPoint = "RemovePropW", SetLastError = true)]
		public static extern LPARAM RemoveProp(Wnd hWnd, LPARAM atom);

		public delegate bool PROPENUMPROCEX(Wnd hwnd, IntPtr lpszString, LPARAM hData, LPARAM dwData);

		[DllImport("user32.dll", EntryPoint = "EnumPropsExW")]
		public static extern int EnumPropsEx(Wnd hWnd, PROPENUMPROCEX lpEnumFunc, LPARAM lParam);

		public delegate int WNDENUMPROC(Wnd hwnd, LPARAM lParam);

		[DllImport("user32.dll")]
		public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, LPARAM lParam);

		[DllImport("user32.dll")]
		public static extern bool EnumThreadWindows(uint dwThreadId, WNDENUMPROC lpfn, LPARAM lParam);

		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(Wnd hWndParent, WNDENUMPROC lpEnumFunc, LPARAM lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd GetDlgItem(Wnd hDlg, int nIDDlgItem);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetDlgCtrlID(Wnd hWnd);

		[DllImport("user32.dll", EntryPoint = "RegisterWindowMessageW")]
		public static extern uint RegisterWindowMessage(string lpString);

		[DllImport("user32.dll")]
		public static extern Wnd WindowFromPoint(POINT Point);

		[DllImport("user32.dll")]
		public static extern bool IsChild(Wnd hWndParent, Wnd hWnd);

		#region GetSystemMetrics, SystemParametersInfo

		public const int SM_YVIRTUALSCREEN = 77;
		public const int SM_XVIRTUALSCREEN = 76;
		public const int SM_TABLETPC = 86;
		public const int SM_SWAPBUTTON = 23;
		public const int SM_STARTER = 88;
		public const int SM_SLOWMACHINE = 73;
		public const int SM_SHUTTINGDOWN = 8192;
		public const int SM_SHOWSOUNDS = 70;
		public const int SM_SERVERR2 = 89;
		public const int SM_SECURE = 44;
		public const int SM_SAMEDISPLAYFORMAT = 81;
		public const int SM_RESERVED4 = 27;
		public const int SM_RESERVED3 = 26;
		public const int SM_RESERVED2 = 25;
		public const int SM_RESERVED1 = 24;
		public const int SM_REMOTESESSION = 4096;
		public const int SM_REMOTECONTROL = 8193;
		public const int SM_PENWINDOWS = 41;
		public const int SM_NETWORK = 63;
		public const int SM_MOUSEWHEELPRESENT = 75;
		public const int SM_MOUSEPRESENT = 19;
		public const int SM_MIDEASTENABLED = 74;
		public const int SM_MENUDROPALIGNMENT = 40;
		public const int SM_MEDIACENTER = 87;
		public const int SM_IMMENABLED = 82;
		public const int SM_DEBUG = 22;
		public const int SM_DBCSENABLED = 42;
		public const int SM_CYVTHUMB = 9;
		public const int SM_CYVSCROLL = 20;
		public const int SM_CYVIRTUALSCREEN = 79;
		public const int SM_CYSMSIZE = 53;
		public const int SM_CYSMICON = 50;
		public const int SM_CYSMCAPTION = 51;
		public const int SM_CYSIZEFRAME = SM_CYFRAME;
		public const int SM_CYSIZE = 31;
		public const int SM_CYSCREEN = 1;
		public const int SM_CYMINTRACK = 35;
		public const int SM_CYMINSPACING = 48;
		public const int SM_CYMINIMIZED = 58;
		public const int SM_CYMIN = 29;
		public const int SM_CYMENUSIZE = 55;
		public const int SM_CYMENUCHECK = 72;
		public const int SM_CYMENU = 15;
		public const int SM_CYMAXTRACK = 60;
		public const int SM_CYMAXIMIZED = 62;
		public const int SM_CYKANJIWINDOW = 18;
		public const int SM_CYICONSPACING = 39;
		public const int SM_CYICON = 12;
		public const int SM_CYHSCROLL = 3;
		public const int SM_CYFULLSCREEN = 17;
		public const int SM_CYFRAME = 33;
		public const int SM_CYFOCUSBORDER = 84;
		public const int SM_CYFIXEDFRAME = SM_CYDLGFRAME;
		public const int SM_CYEDGE = 46;
		public const int SM_CYDRAG = 69;
		public const int SM_CYDOUBLECLK = 37;
		public const int SM_CYDLGFRAME = 8;
		public const int SM_CYCURSOR = 14;
		public const int SM_CYCAPTION = 4;
		public const int SM_CYBORDER = 6;
		public const int SM_CXVSCROLL = 2;
		public const int SM_CXVIRTUALSCREEN = 78;
		public const int SM_CXSMSIZE = 52;
		public const int SM_CXSMICON = 49;
		public const int SM_CXSIZEFRAME = SM_CXFRAME;
		public const int SM_CXSIZE = 30;
		public const int SM_CXSCREEN = 0;
		public const int SM_CXMINTRACK = 34;
		public const int SM_CXMINSPACING = 47;
		public const int SM_CXMINIMIZED = 57;
		public const int SM_CXMIN = 28;
		public const int SM_CXMENUSIZE = 54;
		public const int SM_CXMENUCHECK = 71;
		public const int SM_CXMAXTRACK = 59;
		public const int SM_CXMAXIMIZED = 61;
		public const int SM_CXICONSPACING = 38;
		public const int SM_CXICON = 11;
		public const int SM_CXHTHUMB = 10;
		public const int SM_CXHSCROLL = 21;
		public const int SM_CXFULLSCREEN = 16;
		public const int SM_CXFRAME = 32;
		public const int SM_CXFOCUSBORDER = 83;
		public const int SM_CXFIXEDFRAME = SM_CXDLGFRAME;
		public const int SM_CXEDGE = 45;
		public const int SM_CXDRAG = 68;
		public const int SM_CXDOUBLECLK = 36;
		public const int SM_CXDLGFRAME = 7;
		public const int SM_CXCURSOR = 13;
		public const int SM_CXBORDER = 5;
		public const int SM_CMOUSEBUTTONS = 43;
		public const int SM_CMONITORS = 80;
		public const int SM_CMETRICS = 90;
		public const int SM_CLEANBOOT = 67;
		public const int SM_CARETBLINKINGENABLED = 8194;
		public const int SM_ARRANGE = 56;

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int nIndex);

		public const int SPI_SETWORKAREA = 47;
		public const int SPI_SETWHEELSCROLLLINES = 105;
		public const int SPI_SETUIEFFECTS = 4159;
		public const int SPI_SETTOOLTIPFADE = 4121;
		public const int SPI_SETTOOLTIPANIMATION = 4119;
		public const int SPI_SETTOGGLEKEYS = 53;
		public const int SPI_SETSTICKYKEYS = 59;
		public const int SPI_SETSOUNDSENTRY = 65;
		public const int SPI_SETSNAPTODEFBUTTON = 96;
		public const int SPI_SETSHOWSOUNDS = 57;
		public const int SPI_SETSHOWIMEUI = 111;
		public const int SPI_SETSERIALKEYS = 63;
		public const int SPI_SETSELECTIONFADE = 4117;
		public const int SPI_SETSCREENSAVETIMEOUT = 15;
		public const int SPI_SETSCREENSAVERRUNNING = 97;
		public const int SPI_SETSCREENSAVEACTIVE = 17;
		public const int SPI_SETSCREENREADER = 71;
		public const int SPI_SETPOWEROFFTIMEOUT = 82;
		public const int SPI_SETPOWEROFFACTIVE = 86;
		public const int SPI_SETPENWINDOWS = 49;
		public const int SPI_SETNONCLIENTMETRICS = 42;
		public const int SPI_SETMOUSEVANISH = 4129;
		public const int SPI_SETMOUSETRAILS = 93;
		public const int SPI_SETMOUSESPEED = 113;
		public const int SPI_SETMOUSESONAR = 4125;
		public const int SPI_SETMOUSEKEYS = 55;
		public const int SPI_SETMOUSEHOVERWIDTH = 99;
		public const int SPI_SETMOUSEHOVERTIME = 103;
		public const int SPI_SETMOUSEHOVERHEIGHT = 101;
		public const int SPI_SETMOUSECLICKLOCKTIME = 8201;
		public const int SPI_SETMOUSECLICKLOCK = 4127;
		public const int SPI_SETMOUSEBUTTONSWAP = 33;
		public const int SPI_SETMOUSE = 4;
		public const int SPI_SETMINIMIZEDMETRICS = 44;
		public const int SPI_SETMENUUNDERLINES = SPI_SETKEYBOARDCUES;
		public const int SPI_SETMENUSHOWDELAY = 107;
		public const int SPI_SETMENUFADE = 4115;
		public const int SPI_SETMENUDROPALIGNMENT = 28;
		public const int SPI_SETMENUANIMATION = 4099;
		public const int SPI_SETLOWPOWERTIMEOUT = 81;
		public const int SPI_SETLOWPOWERACTIVE = 85;
		public const int SPI_SETLISTBOXSMOOTHSCROLLING = 4103;
		public const int SPI_SETLANGTOGGLE = 91;
		public const int SPI_SETKEYBOARDSPEED = 11;
		public const int SPI_SETKEYBOARDPREF = 69;
		public const int SPI_SETKEYBOARDDELAY = 23;
		public const int SPI_SETKEYBOARDCUES = 4107;
		public const int SPI_SETICONTITLEWRAP = 26;
		public const int SPI_SETICONTITLELOGFONT = 34;
		public const int SPI_SETICONS = 88;
		public const int SPI_SETICONMETRICS = 46;
		public const int SPI_SETHOTTRACKING = 4111;
		public const int SPI_SETHIGHCONTRAST = 67;
		public const int SPI_SETHANDHELD = 78;
		public const int SPI_SETGRIDGRANULARITY = 19;
		public const int SPI_SETGRADIENTCAPTIONS = 4105;
		public const int SPI_SETFOREGROUNDLOCKTIMEOUT = 8193;
		public const int SPI_SETFOREGROUNDFLASHCOUNT = 8197;
		public const int SPI_SETFONTSMOOTHINGTYPE = 8203;
		public const int SPI_SETFONTSMOOTHINGORIENTATION = 8211;
		public const int SPI_SETFONTSMOOTHINGCONTRAST = 8205;
		public const int SPI_SETFONTSMOOTHING = 75;
		public const int SPI_SETFOCUSBORDERWIDTH = 8207;
		public const int SPI_SETFOCUSBORDERHEIGHT = 8209;
		public const int SPI_SETFLATMENU = 4131;
		public const int SPI_SETFILTERKEYS = 51;
		public const int SPI_SETFASTTASKSWITCH = 36;
		public const int SPI_SETDROPSHADOW = 4133;
		public const int SPI_SETDRAGWIDTH = 76;
		public const int SPI_SETDRAGHEIGHT = 77;
		public const int SPI_SETDRAGFULLWINDOWS = 37;
		public const int SPI_SETDOUBLECLKWIDTH = 29;
		public const int SPI_SETDOUBLECLKHEIGHT = 30;
		public const int SPI_SETDOUBLECLICKTIME = 32;
		public const int SPI_SETDESKWALLPAPER = 20;
		public const int SPI_SETDESKPATTERN = 21;
		public const int SPI_SETDEFAULTINPUTLANG = 90;
		public const int SPI_SETCURSORSHADOW = 4123;
		public const int SPI_SETCURSORS = 87;
		public const int SPI_SETCOMBOBOXANIMATION = 4101;
		public const int SPI_SETCARETWIDTH = 8199;
		public const int SPI_SETBORDER = 6;
		public const int SPI_SETBLOCKSENDINPUTRESETS = 4135;
		public const int SPI_SETBEEP = 2;
		public const int SPI_SETANIMATION = 73;
		public const int SPI_SETACTIVEWNDTRKZORDER = 4109;
		public const int SPI_SETACTIVEWNDTRKTIMEOUT = 8195;
		public const int SPI_SETACTIVEWINDOWTRACKING = 4097;
		public const int SPI_SETACCESSTIMEOUT = 61;
		public const int SPI_LANGDRIVER = 12;
		public const int SPI_ICONVERTICALSPACING = 24;
		public const int SPI_ICONHORIZONTALSPACING = 13;
		public const int SPI_GETWORKAREA = 48;
		public const int SPI_GETWINDOWSEXTENSION = 92;
		public const int SPI_GETWHEELSCROLLLINES = 104;
		public const int SPI_GETUIEFFECTS = 4158;
		public const int SPI_GETTOOLTIPFADE = 4120;
		public const int SPI_GETTOOLTIPANIMATION = 4118;
		public const int SPI_GETTOGGLEKEYS = 52;
		public const int SPI_GETSTICKYKEYS = 58;
		public const int SPI_GETSOUNDSENTRY = 64;
		public const int SPI_GETSNAPTODEFBUTTON = 95;
		public const int SPI_GETSHOWSOUNDS = 56;
		public const int SPI_GETSHOWIMEUI = 110;
		public const int SPI_GETSERIALKEYS = 62;
		public const int SPI_GETSELECTIONFADE = 4116;
		public const int SPI_GETSCREENSAVETIMEOUT = 14;
		public const int SPI_GETSCREENSAVERRUNNING = 114;
		public const int SPI_GETSCREENSAVEACTIVE = 16;
		public const int SPI_GETSCREENREADER = 70;
		public const int SPI_GETPOWEROFFTIMEOUT = 80;
		public const int SPI_GETPOWEROFFACTIVE = 84;
		public const int SPI_GETNONCLIENTMETRICS = 41;
		public const int SPI_GETMOUSEVANISH = 4128;
		public const int SPI_GETMOUSETRAILS = 94;
		public const int SPI_GETMOUSESPEED = 112;
		public const int SPI_GETMOUSESONAR = 4124;
		public const int SPI_GETMOUSEKEYS = 54;
		public const int SPI_GETMOUSEHOVERWIDTH = 98;
		public const int SPI_GETMOUSEHOVERTIME = 102;
		public const int SPI_GETMOUSEHOVERHEIGHT = 100;
		public const int SPI_GETMOUSECLICKLOCKTIME = 8200;
		public const int SPI_GETMOUSECLICKLOCK = 4126;
		public const int SPI_GETMOUSE = 3;
		public const int SPI_GETMINIMIZEDMETRICS = 43;
		public const int SPI_GETMENUUNDERLINES = SPI_GETKEYBOARDCUES;
		public const int SPI_GETMENUSHOWDELAY = 106;
		public const int SPI_GETMENUFADE = 4114;
		public const int SPI_GETMENUDROPALIGNMENT = 27;
		public const int SPI_GETMENUANIMATION = 4098;
		public const int SPI_GETLOWPOWERTIMEOUT = 79;
		public const int SPI_GETLOWPOWERACTIVE = 83;
		public const int SPI_GETLISTBOXSMOOTHSCROLLING = 4102;
		public const int SPI_GETKEYBOARDSPEED = 10;
		public const int SPI_GETKEYBOARDPREF = 68;
		public const int SPI_GETKEYBOARDDELAY = 22;
		public const int SPI_GETKEYBOARDCUES = 4106;
		public const int SPI_GETICONTITLEWRAP = 25;
		public const int SPI_GETICONTITLELOGFONT = 31;
		public const int SPI_GETICONMETRICS = 45;
		public const int SPI_GETHOTTRACKING = 4110;
		public const int SPI_GETHIGHCONTRAST = 66;
		public const int SPI_GETGRIDGRANULARITY = 18;
		public const int SPI_GETGRADIENTCAPTIONS = 4104;
		public const int SPI_GETFOREGROUNDLOCKTIMEOUT = 8192;
		public const int SPI_GETFOREGROUNDFLASHCOUNT = 8196;
		public const int SPI_GETFONTSMOOTHINGTYPE = 8202;
		public const int SPI_GETFONTSMOOTHINGORIENTATION = 8210;
		public const int SPI_GETFONTSMOOTHINGCONTRAST = 8204;
		public const int SPI_GETFONTSMOOTHING = 74;
		public const int SPI_GETFOCUSBORDERWIDTH = 8206;
		public const int SPI_GETFOCUSBORDERHEIGHT = 8208;
		public const int SPI_GETFLATMENU = 4130;
		public const int SPI_GETFILTERKEYS = 50;
		public const int SPI_GETFASTTASKSWITCH = 35;
		public const int SPI_GETDROPSHADOW = 4132;
		public const int SPI_GETDRAGFULLWINDOWS = 38;
		public const int SPI_GETDESKWALLPAPER = 115;
		public const int SPI_GETDEFAULTINPUTLANG = 89;
		public const int SPI_GETCURSORSHADOW = 4122;
		public const int SPI_GETCOMBOBOXANIMATION = 4100;
		public const int SPI_GETCARETWIDTH = 8198;
		public const int SPI_GETBORDER = 5;
		public const int SPI_GETBLOCKSENDINPUTRESETS = 4134;
		public const int SPI_GETBEEP = 1;
		public const int SPI_GETANIMATION = 72;
		public const int SPI_GETACTIVEWNDTRKZORDER = 4108;
		public const int SPI_GETACTIVEWNDTRKTIMEOUT = 8194;
		public const int SPI_GETACTIVEWINDOWTRACKING = 4096;
		public const int SPI_GETACCESSTIMEOUT = 60;

		public const uint SPIF_UPDATEINIFILE = 0x1;
		public const uint SPIF_SENDCHANGE = 0x2;

		[DllImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
		public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, LPARAM pvParam, uint fWinIni);

		[DllImport("user32.dll", EntryPoint = "SystemParametersInfoW")]
		public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, void* pvParam, uint fWinIni);

		#endregion

		[DllImport("user32.dll")]
		public static extern Wnd RealChildWindowFromPoint(Wnd hwndParent, POINT ptParentClientCoords);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ScreenToClient(Wnd hWnd, ref POINT lpPoint);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ClientToScreen(Wnd hWnd, ref POINT lpPoint);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(Wnd hWndFrom, Wnd hWndTo, ref POINT lpPoints, uint cPoints = 1);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(Wnd hWndFrom, Wnd hWndTo, ref RECT lpPoints, uint cPoints = 2);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(Wnd hWndFrom, Wnd hWndTo, void* lpPoints, uint cPoints);

		public struct GUITHREADINFO
		{
			public uint cbSize;
			public uint flags;
			public Wnd hwndActive;
			public Wnd hwndFocus;
			public Wnd hwndCapture;
			public Wnd hwndMenuOwner;
			public Wnd hwndMoveSize;
			public Wnd hwndCaret;
			public RECT rcCaret;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO pgui);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[Flags]
		public enum IKFlag :uint
		{
			Extended = 1, Up = 2, Unicode = 4, Scancode = 8
		};

		public struct INPUTKEY
		{
			LPARAM _type;
			public ushort wVk;
			public ushort wScan;
			public IKFlag dwFlags;
			public uint time;
			public LPARAM dwExtraInfo;
			int _u1, _u2; //need INPUT size

			public INPUTKEY(int vk, int sc, IKFlag flags = 0)
			{
				_type = INPUT_KEYBOARD;
				wVk = (ushort)vk; wScan = (ushort)sc; dwFlags = flags;
				time = 0; dwExtraInfo = CatkeysExtraInfo;
				_u2 = _u1 = 0;
				Debug.Assert(Size == INPUTMOUSE.Size);
			}

			public static readonly int Size = Marshal.SizeOf(typeof(INPUTKEY));

			public const uint CatkeysExtraInfo = 0xA1427fa5;
			const int INPUT_KEYBOARD = 1;
		}

		[Flags]
		public enum IMFlag :uint
		{
			Move = 1,
			LeftDown = 2, LeftUp = 4,
			RightDown = 8, RightUp = 16,
			MiddleDown = 32, MiddleUp = 64,
			X1Down = 0x80, X1Up = 0x100,
			X2Down = 0x80000080, X2Up = 0x80000100,
			Wheel = 0x0800, HWheel = 0x01000,
			NoCoalesce = 0x2000,
			VirtualdDesktop = 0x4000,
			Absolute = 0x8000
		};

		public struct INPUTMOUSE
		{
			LPARAM _type;
			public int dx;
			public int dy;
			public int mouseData;
			public IMFlag dwFlags;
			public uint time;
			public LPARAM dwExtraInfo;

			public INPUTMOUSE(IMFlag flags, int x = 0, int y = 0, int wheelTicks = 0)
			{
				_type = INPUT_MOUSE;
				dx = x; dy = y; dwFlags = flags; mouseData = wheelTicks * 120;
				time = 0; dwExtraInfo = CatkeysExtraInfo;
				if((dwFlags & (IMFlag.X1Down | IMFlag.X2Up)) != 0) {
					mouseData = ((dwFlags & (IMFlag)0x80000000U) != 0) ? 2 : 1;
					dwFlags &= (IMFlag)0x7fffffff;
				}
			}

			public static readonly int Size = Marshal.SizeOf(typeof(INPUTMOUSE));

			public const uint CatkeysExtraInfo = 0xA1427fa5;
			const int INPUT_MOUSE = 0;
		}

		//[DllImport("user32.dll", SetLastError = true)]
		//public static extern uint SendInput(uint cInputs, ref INPUTKEY pInputs, int cbSize);
		//[DllImport("user32.dll", SetLastError = true)]
		//public static extern uint SendInput(uint cInputs, [In] INPUTKEY[] pInputs, int cbSize);
		//[DllImport("user32.dll", SetLastError = true)]
		//public static extern uint SendInput(uint cInputs, ref INPUTMOUSE pInputs, int cbSize);
		//[DllImport("user32.dll", SetLastError = true)]
		//public static extern uint SendInput(uint cInputs, [In] INPUTMOUSE[] pInputs, int cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(int cInputs, void* pInputs, int cbSize);

		public static bool SendInputKey(ref INPUTKEY ik)
		{
			fixed (void* p = &ik)
			{
				return SendInput(1, p, INPUTKEY.Size) != 0;
			}
		}

		public static bool SendInputKey(INPUTKEY[] ik)
		{
			if(ik == null || ik.Length == 0) return false;
			fixed (void* p = ik)
			{
				return SendInput(ik.Length, p, INPUTKEY.Size) != 0;
			}
		}

		public static bool SendInputMouse(ref INPUTMOUSE ik)
		{
			fixed (void* p = &ik)
			{
				return SendInput(1, p, INPUTMOUSE.Size) != 0;
			}
		}

		public static bool SendInputMouse(INPUTMOUSE[] ik)
		{
			if(ik == null || ik.Length == 0) return false;
			fixed (void* p = ik)
			{
				return SendInput(ik.Length, p, INPUTMOUSE.Size) != 0;
			}
		}

		[DllImport("user32.dll")]
		public static extern bool IsHungAppWindow(Wnd hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetLayeredWindowAttributes(Wnd hwnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr CreateIcon(IntPtr hInstance, int nWidth, int nHeight, byte cPlanes, byte cBitsPixel, byte[] lpbANDbits, byte[] lpbXORbits);

		[DllImport("user32.dll", EntryPoint = "PrivateExtractIconsW")]
		public static extern uint PrivateExtractIcons(string szFileName, int nIconIndex, int cxIcon, int cyIcon, [Out] IntPtr[] phicon, IntPtr piconid, uint nIcons, uint flags);
		[DllImport("user32.dll", EntryPoint = "PrivateExtractIconsW")]
		public static extern uint PrivateExtractIcons(string szFileName, int nIconIndex, int cxIcon, int cyIcon, out IntPtr phicon, IntPtr piconid, uint nIcons, uint flags);

		[DllImport("user32.dll", EntryPoint = "LoadCursorW")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, string lpCursorName);

		[DllImport("user32.dll", EntryPoint = "LoadCursorW")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

		public delegate void TIMERPROC(Wnd param1, uint param2, LPARAM param3, uint param4);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern LPARAM SetTimer(Wnd hWnd, LPARAM nIDEvent, uint uElapse, TIMERPROC lpTimerFunc);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool KillTimer(Wnd hWnd, LPARAM uIDEvent);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern Wnd SetParent(Wnd hWndChild, Wnd hWndNewParent);

		[DllImport("user32.dll")]
		public static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);

		[DllImport("user32.dll")]
		public static extern bool ChangeWindowMessageFilter(uint message, uint dwFlag);

		[DllImport("user32.dll")]
		public static extern short GetKeyState(int nVirtKey);

		public struct WINDOWPOS
		{
			public Wnd hwnd;
			public Wnd hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public uint flags;
		}

		[DllImport("user32.dll")]
		public static extern uint MsgWaitForMultipleObjects(uint nCount, [In] IntPtr[] pHandles, bool fWaitAll, uint dwMilliseconds, uint dwWakeMask);

		[DllImport("user32.dll")]
		public static extern uint MsgWaitForMultipleObjects(uint nCount, ref IntPtr pHandles, bool fWaitAll, uint dwMilliseconds, uint dwWakeMask);

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(Wnd hWnd, int id, uint fsModifiers, uint vk);

		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(Wnd hWnd, int id);

		[DllImport("user32.dll")]
		public static extern bool EndMenu();

		[DllImport("user32.dll")]
		public static extern bool ValidateRect(Wnd hWnd, [In] ref RECT lpRect);
		[DllImport("user32.dll")]
		public static extern bool ValidateRect(Wnd hWnd, LPARAM zero = default(LPARAM));

		[DllImport("user32.dll")]
		public static extern bool GetUpdateRect(Wnd hWnd, out RECT lpRect, bool bErase);
		[DllImport("user32.dll")]
		public static extern bool GetUpdateRect(Wnd hWnd, LPARAM zero, bool bErase);

		public const int ERROR = 0;
		public const int NULLREGION = 1;
		public const int SIMPLEREGION = 2;
		public const int COMPLEXREGION = 3;

		[DllImport("user32.dll")]
		public static extern int GetUpdateRgn(Wnd hWnd, IntPtr hRgn, bool bErase);

		[DllImport("user32.dll")]
		public static extern bool InvalidateRgn(Wnd hWnd, IntPtr hRgn, bool bErase);








		//GDI32

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr ho);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

		public const int RGN_AND = 1;
		public const int RGN_OR = 2;
		public const int RGN_XOR = 3;
		public const int RGN_DIFF = 4;
		public const int RGN_COPY = 5;

		[DllImport("gdi32.dll")]
		public static extern int CombineRgn(IntPtr hrgnDst, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int iMode);

		[DllImport("gdi32.dll")]
		public static extern bool SetRectRgn(IntPtr hrgn, int left, int top, int right, int bottom);





		//KERNEL32

		//Call Api.SetLastError(0) before calling an API function. Call Marshal.GetLastWin32Error() after calling the API function.
		[DllImport("kernel32.dll")]
		public static extern void SetLastError(uint dwErrCode);

		[DllImport("kernel32.dll", EntryPoint = "SetDllDirectoryW", SetLastError = true)]
		public static extern bool SetDllDirectory(string lpPathName);

		[DllImport("kernel32.dll")]
		public static extern long GetTickCount64();

		[DllImport("kernel32.dll", EntryPoint = "CreateEventW", SetLastError = true)]
		public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

		[DllImport("kernel32.dll")]
		public static extern bool SetEvent(IntPtr hEvent);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

		//[DllImport("kernel32.dll")]
		//public static extern uint SignalObjectAndWait(IntPtr hObjectToSignal, IntPtr hObjectToWaitOn, uint dwMilliseconds, bool bAlertable);
		//note: don't know why, this often is much slower than setevent/waitforsingleobject.

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetCurrentThread();

		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentProcessId();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		[DllImport("kernel32.dll", EntryPoint = "OpenFileMappingW", SetLastError = true)]
		public static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, LPARAM dwNumberOfBytesToMap);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

		[DllImport("kernel32.dll")]
		public static extern bool SetProcessWorkingSetSize(IntPtr hProcess, LPARAM dwMinimumWorkingSetSize, LPARAM dwMaximumWorkingSetSize);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW")]
		public static extern IntPtr GetModuleHandle(string name);
		//see also Util.Misc.GetModuleHandleOf(Type|Assembly).

		[DllImport("kernel32.dll", EntryPoint = "LoadLibraryW")]
		public static extern IntPtr LoadLibrary([In] string lpLibFileName);

		[DllImport("kernel32.dll", BestFitMapping = false)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, [In] [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

		/// <summary>
		/// Gets dll module handle (Api.GetModuleHandle) or loads dll (Api.LoadLibrary), and returns unmanaged exported function address (Api.GetProcAddress).
		/// </summary>
		/// <param name="dllName"></param>
		/// <param name="funcName"></param>
		/// <seealso cref="GetDelegate"/>
		public static IntPtr GetProcAddress(string dllName, string funcName)
		{
			IntPtr hmod = GetModuleHandle(dllName);
			if(hmod == default(IntPtr)) { hmod = LoadLibrary(dllName); if(hmod == default(IntPtr)) return hmod; }

			return GetProcAddress(hmod, funcName);
		}

		public const uint PROCESS_TERMINATE = 0x0001;
		public const uint PROCESS_CREATE_THREAD = 0x0002;
		public const uint PROCESS_SET_SESSIONID = 0x0004;
		public const uint PROCESS_VM_OPERATION = 0x0008;
		public const uint PROCESS_VM_READ = 0x0010;
		public const uint PROCESS_VM_WRITE = 0x0020;
		public const uint PROCESS_DUP_HANDLE = 0x0040;
		public const uint PROCESS_CREATE_PROCESS = 0x0080;
		public const uint PROCESS_SET_QUOTA = 0x0100;
		public const uint PROCESS_SET_INFORMATION = 0x0200;
		public const uint PROCESS_QUERY_INFORMATION = 0x0400;
		public const uint PROCESS_SUSPEND_RESUME = 0x0800;
		public const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
		public const uint PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF;
		public const uint DELETE = 0x00010000;
		public const uint READ_CONTROL = 0x00020000;
		public const uint WRITE_DAC = 0x00040000;
		public const uint WRITE_OWNER = 0x00080000;
		public const uint SYNCHRONIZE = 0x00100000;
		public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
		public const uint STANDARD_RIGHTS_READ = READ_CONTROL;
		public const uint STANDARD_RIGHTS_WRITE = READ_CONTROL;
		public const uint STANDARD_RIGHTS_EXECUTE = READ_CONTROL;
		public const uint STANDARD_RIGHTS_ALL = 0x001F0000;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", EntryPoint = "GlobalFindAtomW")]
		public static extern ushort GlobalFindAtom(string lpString);

		[DllImport("kernel32.dll", EntryPoint = "FindAtomW")]
		public static extern ushort FindAtom(string lpString);

		[DllImport("kernel32.dll", EntryPoint = "AddAtomW")]
		public static extern ushort AddAtom(string lpString);

		[DllImport("kernel32.dll", EntryPoint = "GetLongPathNameW")]
		public static extern uint GetLongPathName(string lpszShortPath, [Out] StringBuilder lpszLongPath, uint cchBuffer);

		public const uint TH32CS_SNAPHEAPLIST = 0x00000001;
		public const uint TH32CS_SNAPPROCESS = 0x00000002;
		public const uint TH32CS_SNAPTHREAD = 0x00000004;
		public const uint TH32CS_SNAPMODULE = 0x00000008;
		public const uint TH32CS_SNAPMODULE32 = 0x00000010;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

		[DllImport("kernel32.dll")]
		public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		[DllImport("kernel32.dll")]
		public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		public struct PROCESSENTRY32
		{
			public uint dwSize;
			public uint cntUsage;
			public uint th32ProcessID;
			public IntPtr th32DefaultHeapID;
			public uint th32ModuleID;
			public uint cntThreads;
			public uint th32ParentProcessID;
			public int pcPriClassBase;
			public uint dwFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szExeFile;
		};

		[DllImport("kernel32.dll")]
		public static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

		public const uint PAGE_NOACCESS = 0x1;
		public const uint PAGE_READONLY = 0x2;
		public const uint PAGE_READWRITE = 0x4;
		public const uint PAGE_WRITECOPY = 0x8;
		public const uint PAGE_EXECUTE = 0x10;
		public const uint PAGE_EXECUTE_READ = 0x20;
		public const uint PAGE_EXECUTE_READWRITE = 0x40;
		public const uint PAGE_EXECUTE_WRITECOPY = 0x80;
		public const uint PAGE_GUARD = 0x100;
		public const uint PAGE_NOCACHE = 0x200;
		public const uint PAGE_WRITECOMBINE = 0x400;

		public const uint MEM_COMMIT = 0x1000;
		public const uint MEM_RESERVE = 0x2000;
		public const uint MEM_DECOMMIT = 0x4000;
		public const uint MEM_RELEASE = 0x8000;
		public const uint MEM_RESET = 0x80000;
		public const uint MEM_TOP_DOWN = 0x100000;
		public const uint MEM_WRITE_WATCH = 0x200000;
		public const uint MEM_PHYSICAL = 0x400000;
		public const uint MEM_RESET_UNDO = 0x1000000;
		public const uint MEM_LARGE_PAGES = 0x20000000;

		[DllImport("kernel32.dll")]
		public static extern IntPtr VirtualAlloc(IntPtr lpAddress, LPARAM dwSize, uint flAllocationType = MEM_COMMIT | MEM_RESERVE, uint flProtect = PAGE_EXECUTE_READWRITE);

		[DllImport("kernel32.dll")]
		public static extern bool VirtualFree(IntPtr lpAddress, LPARAM dwSize = default(LPARAM), uint dwFreeType = MEM_RELEASE);

		[DllImport("kernel32.dll")]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, LPARAM dwSize, uint flAllocationType = MEM_COMMIT | MEM_RESERVE, uint flProtect = PAGE_EXECUTE_READWRITE);

		[DllImport("kernel32.dll")]
		public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, LPARAM dwSize = default(LPARAM), uint dwFreeType = MEM_RELEASE);

		[DllImport("kernel32.dll", EntryPoint = "GetFileAttributesW")]
		public static extern uint GetFileAttributes(string lpFileName);

		[DllImport("kernel32.dll", EntryPoint = "SearchPathW")]
		public static extern uint SearchPath(string lpPath, string lpFileName, string lpExtension, uint nBufferLength, [Out] StringBuilder lpBuffer, IntPtr lpFilePart);

		public const uint BASE_SEARCH_PATH_ENABLE_SAFE_SEARCHMODE = 0x1;
		public const uint BASE_SEARCH_PATH_DISABLE_SAFE_SEARCHMODE = 0x10000;
		public const uint BASE_SEARCH_PATH_PERMANENT = 0x8000;

		[DllImport("kernel32.dll")]
		public static extern bool SetSearchPathMode(uint Flags);

		public const uint SEM_FAILCRITICALERRORS = 0x1;

		[DllImport("kernel32.dll")]
		public static extern uint SetErrorMode(uint uMode);

		[DllImport("kernel32.dll")]
		public static extern bool SetThreadPriority(IntPtr hThread, int nPriority);










		//ADVAPI32

		public const uint TOKEN_WRITE = STANDARD_RIGHTS_WRITE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT;
		public const uint TOKEN_SOURCE_LENGTH = 8;
		public const uint TOKEN_READ = STANDARD_RIGHTS_READ | TOKEN_QUERY;
		public const uint TOKEN_QUERY_SOURCE = 16;
		public const uint TOKEN_QUERY = 8;
		public const uint TOKEN_IMPERSONATE = 4;
		public const uint TOKEN_EXECUTE = STANDARD_RIGHTS_EXECUTE;
		public const uint TOKEN_DUPLICATE = 2;
		public const uint TOKEN_AUDIT_SUCCESS_INCLUDE = 1;
		public const uint TOKEN_AUDIT_SUCCESS_EXCLUDE = 2;
		public const uint TOKEN_AUDIT_FAILURE_INCLUDE = 4;
		public const uint TOKEN_AUDIT_FAILURE_EXCLUDE = 8;
		public const uint TOKEN_ASSIGN_PRIMARY = 1;
		public const uint TOKEN_ALL_ACCESS_P = STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT;
		public const uint TOKEN_ALL_ACCESS = TOKEN_ALL_ACCESS_P | TOKEN_ADJUST_SESSIONID;
		public const uint TOKEN_ADJUST_SESSIONID = 256;
		public const uint TOKEN_ADJUST_PRIVILEGES = 32;
		public const uint TOKEN_ADJUST_GROUPS = 64;
		public const uint TOKEN_ADJUST_DEFAULT = 128;

		[DllImport("advapi32.dll")]
		public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

		public enum TOKEN_INFORMATION_CLASS
		{
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin,
			TokenElevationType,
			TokenLinkedToken,
			TokenElevation,
			TokenHasRestrictions,
			TokenAccessInformation,
			TokenVirtualizationAllowed,
			TokenVirtualizationEnabled,
			TokenIntegrityLevel,
			TokenUIAccess,
			TokenMandatoryPolicy,
			TokenLogonSid,
			//Win8
			TokenIsAppContainer,
			TokenCapabilities,
			TokenAppContainerSid,
			TokenAppContainerNumber,
			TokenUserClaimAttributes,
			TokenDeviceClaimAttributes,
			TokenRestrictedUserClaimAttributes,
			TokenRestrictedDeviceClaimAttributes,
			TokenDeviceGroups,
			TokenRestrictedDeviceGroups,
			TokenSecurityAttributes,
			TokenIsRestricted,
			TokenProcessTrustLevel,
			TokenPrivateNameSpace,
			MaxTokenInfoClass  // MaxTokenInfoClass should always be the last enum
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, void* TokenInformation, uint TokenInformationLength, out uint ReturnLength);

		[DllImport("advapi32.dll")]
		public static extern byte* GetSidSubAuthorityCount(IntPtr pSid);

		[DllImport("advapi32.dll")]
		public static extern uint* GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

		[DllImport("advapi32.dll")]
		public static extern int RegSetValueEx(IntPtr hKey, string lpValueName, int Reserved, Microsoft.Win32.RegistryValueKind dwType, void* lpData, int cbData);

		[DllImport("advapi32.dll")]
		public static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, IntPtr Reserved, out Microsoft.Win32.RegistryValueKind dwType, void* lpData, ref int cbData);








		//SHELL32

		//[DllImport("shell32.dll")]
		//public static extern bool IsUserAnAdmin();

		public const uint SHGFI_ICON = 0x000000100;     // get icon;
		public const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name;
		public const uint SHGFI_TYPENAME = 0x000000400;     // get type name;
		public const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes;
		public const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location;
		public const uint SHGFI_EXETYPE = 0x000002000;     // return exe type;
		public const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index;
		public const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon;
		public const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state;
		public const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes;
		public const uint SHGFI_LARGEICON = 0x000000000;     // get large icon;
		public const uint SHGFI_SMALLICON = 0x000000001;     // get small icon;
		public const uint SHGFI_OPENICON = 0x000000002;     // get open icon;
		public const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon;
		public const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl;
		public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute;
		public const uint SHGFI_ADDOVERLAYS = 0x000000020;     // apply the appropriate overlays;
		public const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay;

		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW")]
		public static extern LPARAM SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW")]
		public static extern LPARAM SHGetFileInfo(IntPtr pidl, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

		[DllImport("shell32.dll", PreserveSig = true)]
		public static extern int SHGetDesktopFolder(out IShellFolder ppshf);

		[DllImport("shell32.dll")]
		public static extern int SHParseDisplayName(string pszName, IntPtr pbc, out IntPtr pidl, uint sfgaoIn, uint* psfgaoOut);

		[Flags]
		public enum SIGDN :uint
		{
			SIGDN_NORMALDISPLAY,
			SIGDN_PARENTRELATIVEPARSING = 0x80018001,
			SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
			SIGDN_PARENTRELATIVEEDITING = 0x80031001,
			SIGDN_DESKTOPABSOLUTEEDITING = 0x8004C000,
			SIGDN_FILESYSPATH = 0x80058000,
			SIGDN_URL = 0x80068000,
			SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007C001,
			SIGDN_PARENTRELATIVE = 0x80080001,
			SIGDN_PARENTRELATIVEFORUI = 0x80094001
		}

		[DllImport("shell32.dll", PreserveSig = true)]
		public static extern int SHGetNameFromIDList(IntPtr pidl, SIGDN sigdnName, out string ppszName);

		[DllImport("shell32.dll", PreserveSig = true)]
		public static extern int SHBindToParent(IntPtr pidl, [In] ref Guid riid, out IShellFolder ppv, out IntPtr ppidlLast);

		[DllImport("shell32.dll", PreserveSig = true)]
		public static extern int SHGetPropertyStoreForWindow(Wnd hwnd, [In] ref Guid riid, out IPropertyStore ppv);

		public static PROPERTYKEY PKEY_AppUserModel_ID = new PROPERTYKEY() { fmtid = new Guid(0x9F4C2855, 0x9F79, 0x4B39, 0xA8, 0xD0, 0xE1, 0xD4, 0x2D, 0xE1, 0xD5, 0xF3), pid = 5 };

		[DllImport("shell32.dll")]
		public static extern IntPtr* CommandLineToArgvW(string lpCmdLine, out int pNumArgs);

		public struct NOTIFYICONDATA
		{
			public uint cbSize;
			public Wnd hWnd;
			public uint uID;
			public uint uFlags;
			public uint uCallbackMessage;
			public IntPtr hIcon;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szTip;
			public uint dwState;
			public uint dwStateMask;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szInfo;

			[StructLayout(LayoutKind.Explicit)]
			public struct TYPE_1
			{
				[FieldOffset(0)]
				public uint uTimeout;
				[FieldOffset(0)]
				public uint uVersion;
			}
			public TYPE_1 _11;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string szInfoTitle;
			public uint dwInfoFlags;
			public Guid guidItem;
			public IntPtr hBalloonIcon;
		}

		public const uint NIN_SELECT = 0x400;
		public const uint NINF_KEY = 0x1;
		public const uint NIN_KEYSELECT = 0x401;
		public const uint NIN_BALLOONSHOW = 0x402;
		public const uint NIN_BALLOONHIDE = 0x403;
		public const uint NIN_BALLOONTIMEOUT = 0x404;
		public const uint NIN_BALLOONUSERCLICK = 0x405;
		public const uint NIN_POPUPOPEN = 0x406;
		public const uint NIN_POPUPCLOSE = 0x407;
		public const uint NIM_ADD = 0x0;
		public const uint NIM_MODIFY = 0x1;
		public const uint NIM_DELETE = 0x2;
		public const uint NIM_SETFOCUS = 0x3;
		public const uint NIM_SETVERSION = 0x4;
		public const int NOTIFYICON_VERSION = 3;
		public const int NOTIFYICON_VERSION_4 = 4;
		public const uint NIF_MESSAGE = 0x1;
		public const uint NIF_ICON = 0x2;
		public const uint NIF_TIP = 0x4;
		public const uint NIF_STATE = 0x8;
		public const uint NIF_INFO = 0x10;
		public const uint NIF_GUID = 0x20;
		public const uint NIF_REALTIME = 0x40;
		public const uint NIF_SHOWTIP = 0x80;
		public const uint NIS_HIDDEN = 0x1;
		public const uint NIS_SHAREDICON = 0x2;
		public const uint NIIF_NONE = 0x0;
		public const uint NIIF_INFO = 0x1;
		public const uint NIIF_WARNING = 0x2;
		public const uint NIIF_ERROR = 0x3;
		public const uint NIIF_USER = 0x4;
		public const uint NIIF_ICON_MASK = 0xF;
		public const uint NIIF_NOSOUND = 0x10;
		public const uint NIIF_LARGE_ICON = 0x20;
		public const uint NIIF_RESPECT_QUIET_TIME = 0x80;

		[DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW")]
		public static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

		public enum SHSTOCKICONID
		{
			SIID_DOCNOASSOC,
			SIID_DOCASSOC,
			SIID_APPLICATION,
			SIID_FOLDER,
			SIID_FOLDEROPEN,
			SIID_DRIVE525,
			SIID_DRIVE35,
			SIID_DRIVEREMOVE,
			SIID_DRIVEFIXED,
			SIID_DRIVENET,
			SIID_DRIVENETDISABLED,
			SIID_DRIVECD,
			SIID_DRIVERAM,
			SIID_WORLD,
			SIID_SERVER = 15,
			SIID_PRINTER,
			SIID_MYNETWORK,
			SIID_FIND = 22,
			SIID_HELP,
			SIID_SHARE = 28,
			SIID_LINK,
			SIID_SLOWFILE,
			SIID_RECYCLER,
			SIID_RECYCLERFULL,
			SIID_MEDIACDAUDIO = 40,
			SIID_LOCK = 47,
			SIID_AUTOLIST = 49,
			SIID_PRINTERNET,
			SIID_SERVERSHARE,
			SIID_PRINTERFAX,
			SIID_PRINTERFAXNET,
			SIID_PRINTERFILE,
			SIID_STACK,
			SIID_MEDIASVCD,
			SIID_STUFFEDFOLDER,
			SIID_DRIVEUNKNOWN,
			SIID_DRIVEDVD,
			SIID_MEDIADVD,
			SIID_MEDIADVDRAM,
			SIID_MEDIADVDRW,
			SIID_MEDIADVDR,
			SIID_MEDIADVDROM,
			SIID_MEDIACDAUDIOPLUS,
			SIID_MEDIACDRW,
			SIID_MEDIACDR,
			SIID_MEDIACDBURN,
			SIID_MEDIABLANKCD,
			SIID_MEDIACDROM,
			SIID_AUDIOFILES,
			SIID_IMAGEFILES,
			SIID_VIDEOFILES,
			SIID_MIXEDFILES,
			SIID_FOLDERBACK,
			SIID_FOLDERFRONT,
			SIID_SHIELD,
			SIID_WARNING,
			SIID_INFO,
			SIID_ERROR,
			SIID_KEY,
			SIID_SOFTWARE,
			SIID_RENAME,
			SIID_DELETE,
			SIID_MEDIAAUDIODVD,
			SIID_MEDIAMOVIEDVD,
			SIID_MEDIAENHANCEDCD,
			SIID_MEDIAENHANCEDDVD,
			SIID_MEDIAHDDVD,
			SIID_MEDIABLURAY,
			SIID_MEDIAVCD,
			SIID_MEDIADVDPLUSR,
			SIID_MEDIADVDPLUSRW,
			SIID_DESKTOPPC,
			SIID_MOBILEPC,
			SIID_USERS,
			SIID_MEDIASMARTMEDIA,
			SIID_MEDIACOMPACTFLASH,
			SIID_DEVICECELLPHONE,
			SIID_DEVICECAMERA,
			SIID_DEVICEVIDEOCAMERA,
			SIID_DEVICEAUDIOPLAYER,
			SIID_NETWORKCONNECT,
			SIID_INTERNET,
			SIID_ZIPFILE,
			SIID_SETTINGS,
			SIID_DRIVEHDDVD = 132,
			SIID_DRIVEBD,
			SIID_MEDIAHDDVDROM,
			SIID_MEDIAHDDVDR,
			SIID_MEDIAHDDVDRAM,
			SIID_MEDIABDROM,
			SIID_MEDIABDR,
			SIID_MEDIABDRE,
			SIID_CLUSTEREDDRIVE,
			SIID_MAX_ICONS = 181
		}

		//public struct SHSTOCKICONINFO
		//{
		//	public uint cbSize;
		//	public IntPtr hIcon;
		//	public int iSysImageIndex;
		//	public int iIcon;
		//	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		//	public string szPath;
		//}

		public struct SHSTOCKICONINFO
		{
			public uint cbSize;
			public IntPtr hIcon;
			public int iSysImageIndex;
			public int iIcon;
			public fixed char szPath[260];
		}

		[DllImport("shell32.dll", PreserveSig = true)]
		public static extern int SHGetStockIconInfo(SHSTOCKICONID siid, uint uFlags, ref SHSTOCKICONINFO psii);

		[DllImport("shell32.dll", EntryPoint = "#6", PreserveSig = true)]
		public static extern int SHDefExtractIcon(string pszIconFile, int iIndex, uint uFlags, IntPtr* phiconLarge, IntPtr* phiconSmall, uint nIconSize);

		public const int SHIL_LARGE = 0;
		public const int SHIL_SMALL = 1;
		public const int SHIL_EXTRALARGE = 2;
		public const int SHIL_SYSSMALL = 3;
		public const int SHIL_JUMBO = 4;

		[DllImport("shell32.dll", EntryPoint = "#727", PreserveSig = true)]
		public static extern int SHGetImageList(int iImageList, [In] ref Guid riid, out IImageList ppvObj);
		//TODO: remove unused
		[DllImport("shell32.dll", EntryPoint = "#727", PreserveSig = true)]
		public static extern int SHGetImageList(int iImageList, [In] ref Guid riid, out IntPtr ppvObj);






		//SHLWAPI

		[DllImport("shlwapi.dll", EntryPoint = "PathIsURLW")]
		public static extern bool PathIsURL(string pszPath);

		//public enum ASSOCSTR
		//{
		//	ASSOCSTR_COMMAND = 1,
		//	ASSOCSTR_EXECUTABLE,
		//	ASSOCSTR_FRIENDLYDOCNAME,
		//	ASSOCSTR_FRIENDLYAPPNAME,
		//	ASSOCSTR_NOOPEN,
		//	ASSOCSTR_SHELLNEWVALUE,
		//	ASSOCSTR_DDECOMMAND,
		//	ASSOCSTR_DDEIFEXEC,
		//	ASSOCSTR_DDEAPPLICATION,
		//	ASSOCSTR_DDETOPIC,
		//	ASSOCSTR_INFOTIP,
		//	ASSOCSTR_QUICKTIP,
		//	ASSOCSTR_TILEINFO,
		//	ASSOCSTR_CONTENTTYPE,
		//	ASSOCSTR_DEFAULTICON,
		//	ASSOCSTR_SHELLEXTENSION,
		//	ASSOCSTR_DROPTARGET,
		//	ASSOCSTR_DELEGATEEXECUTE,
		//	ASSOCSTR_SUPPORTED_URI_PROTOCOLS,
		//	ASSOCSTR_PROGID,
		//	ASSOCSTR_APPID,
		//	ASSOCSTR_APPPUBLISHER,
		//	ASSOCSTR_APPICONREFERENCE,
		//	ASSOCSTR_MAX
		//}

		//[DllImport("shlwapi.dll", PreserveSig = true, EntryPoint = "AssocQueryStringW")]
		//public static extern int AssocQueryString(uint flags, ASSOCSTR str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, ref uint pcchOut);






		//COMCTL32

		public delegate LPARAM SUBCLASSPROC(Wnd hWnd, uint msg, LPARAM wParam, LPARAM lParam, LPARAM uIdSubclass, IntPtr dwRefData);

		[DllImport("comctl32.dll", EntryPoint = "#410")]
		public static extern bool SetWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass, IntPtr dwRefData);

		[DllImport("comctl32.dll", EntryPoint = "#411")] //this is exported only by ordinal
		public static extern bool GetWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass, out IntPtr pdwRefData);

		[DllImport("comctl32.dll", EntryPoint = "#412")]
		public static extern bool RemoveWindowSubclass(Wnd hWnd, SUBCLASSPROC pfnSubclass, LPARAM uIdSubclass);

		[DllImport("comctl32.dll", EntryPoint = "#413")]
		public static extern LPARAM DefSubclassProc(Wnd hWnd, uint uMsg, LPARAM wParam, LPARAM lParam);

		[DllImport("comctl32.dll")]
		public static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

		[DllImport("comctl32.dll")]
		public static extern bool ImageList_GetIconSize(IntPtr himl, out int cx, out int cy);
		//TODO: remove if unused.











		//OLE32

		[DllImport("ole32.dll", PreserveSig = true)]
		public static extern int PropVariantClear(ref PROPVARIANT_LPARAM pvar);

		[Flags]
		public enum COWAIT_FLAGS :uint
		{
			COWAIT_DEFAULT,
			COWAIT_WAITALL,
			COWAIT_ALERTABLE,
			COWAIT_INPUTAVAILABLE = 4,
			COWAIT_DISPATCH_CALLS = 8,
			COWAIT_DISPATCH_WINDOW_MESSAGES = 0x10
		}

		[DllImport("ole32.dll", PreserveSig = true)]
		public static extern int CoWaitForMultipleHandles(COWAIT_FLAGS dwFlags, uint dwTimeout, uint cHandles, [In] IntPtr[] pHandles, out uint lpdwindex);

		[DllImport("ole32.dll", PreserveSig = true)]
		public static extern int CoWaitForMultipleHandles(COWAIT_FLAGS dwFlags, uint dwTimeout, uint cHandles, ref IntPtr pHandles, out uint lpdwindex);










		//MSVCRT

		/// <summary>
		/// This overload has different parameter types.
		/// </summary>
		[DllImport("msvcrt.dll", EntryPoint = "wcstol", CallingConvention = CallingConvention.Cdecl)]
		public static extern int strtoi(char* s, out char* endPtr, int numberBase = 0);

		/// <summary>
		/// This overload has different parameter types.
		/// </summary>
		public static uint strtoui(char* s, out char* endPtr, int numberBase = 0)
		{
			long k = strtoi64(s, out endPtr, numberBase);
			return k < -int.MaxValue ? 0u : (k > uint.MaxValue ? uint.MaxValue : (uint)k);
		}
		//note: don't use the u API because they return 1 if the value is too big and the string contains '-'.
		//[DllImport("msvcrt.dll", EntryPoint = "wcstoul", CallingConvention = CallingConvention.Cdecl)]
		//public static extern uint strtoui(char* s, out char* endPtr, int _base = 0);
		[DllImport("msvcrt.dll", EntryPoint = "_wcstoui64", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong strtoui64(char* s, out char* endPtr, int _base = 0);

		/// <summary>
		/// This overload has different parameter types.
		/// </summary>
		[DllImport("msvcrt.dll", EntryPoint = "_wcstoi64", CallingConvention = CallingConvention.Cdecl)]
		public static extern long strtoi64(char* s, out char* endPtr, int numberBase = 0);
		//info: ntdll also has wcstol, wcstoul, _wcstoui64, but not _wcstoi64.

		/// <summary>
		/// Converts part of string to int.
		/// Returns the int value.
		/// Returns 0 if the string is null, "" or does not begin with a number; then numberEndIndex will be = startIndex.
		/// Returns int.MaxValue or int.MinValue if the value is not in int range; then numberEndIndex will also include all number characters that follow the valid part.
		/// </summary>
		/// <param name="s">String.</param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends.</param>
		/// <param name="numberBase">If 0, parses the string as hexadecimal number if begins with "0x", as octal if begins with "0", else as decimal. Else it can be 2 to 36. Examples: 10 - parse as decimal (don't support "0x" etc); 16 - as hexadecimal (eg returns 26 if string is "1A" or "0x1A"); 2 - as binary (eg returns 5 if string is "101").</param>
		/// <exception cref="ArgumentOutOfRangeException">When startIndex is invalid.</exception>
		public static int strtoi(string s, int startIndex, out int numberEndIndex, int numberBase = 0)
		{
			int R = 0, len = s == null ? 0 : s.Length - startIndex;
			if(len < 0) throw new ArgumentOutOfRangeException("startIndex");
			if(len != 0)
				fixed (char* p = s)
				{
					char* t = p + startIndex, e = t;
					R = strtoi(t, out e, numberBase);
					len = (int)(e - t);
				}
			numberEndIndex = startIndex + len;
			return R;
		}

		/// <summary>
		/// Converts part of string to uint.
		/// Returns the uint value.
		/// Returns 0 if the string is null, "" or does not begin with a number; then numberEndIndex will be = startIndex.
		/// Returns uint.MaxValue (0xffffffff) or uint.MinValue (0) if the value is not in uint range; then numberEndIndex will also include all number characters that follow the valid part.
		/// Supports negative number values -1 to -int.MaxValue, for example converts string "-1" to 0xffffffff.
		/// </summary>
		/// <param name="s">String.</param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends.</param>
		/// <param name="numberBase">If 0, parses the string as hexadecimal number if begins with "0x", as octal if begins with "0", else as decimal. Else it can be 2 to 36. Examples: 10 - parse as decimal (don't support "0x" etc); 16 - as hexadecimal (eg returns 26 if string is "1A" or "0x1A"); 2 - as binary (eg returns 5 if string is "101").</param>
		/// <exception cref="ArgumentOutOfRangeException">When startIndex is invalid.</exception>
		public static uint strtoui(string s, int startIndex, out int numberEndIndex, int numberBase = 0)
		{
			uint R = 0;
			int len = s == null ? 0 : s.Length - startIndex;
			if(len < 0) throw new ArgumentOutOfRangeException("startIndex");
			if(len != 0)
				fixed (char* p = s)
				{
					char* t = p + startIndex, e = t;
					R = strtoui(t, out e, numberBase);
					len = (int)(e - t);
				}
			numberEndIndex = startIndex + len;
			return R;
		}

		/// <summary>
		/// Converts part of string to long.
		/// Returns the long value.
		/// Returns 0 if the string is null, "" or does not begin with a number; then numberEndIndex will be = startIndex.
		/// Returns long.MaxValue or long.MinValue if the value is not in long range; then numberEndIndex will also include all number characters that follow the valid part.
		/// </summary>
		/// <param name="s">String.</param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends.</param>
		/// <param name="numberBase">If 0, parses the string as hexadecimal number if begins with "0x", as octal if begins with "0", else as decimal. Else it can be 2 to 36. Examples: 10 - parse as decimal (don't support "0x" etc); 16 - as hexadecimal (eg returns 26 if string is "1A" or "0x1A"); 2 - as binary (eg returns 5 if string is "101").</param>
		/// <exception cref="ArgumentOutOfRangeException">When startIndex is invalid.</exception>
		public static long strtoi64(string s, int startIndex, out int numberEndIndex, int numberBase = 0)
		{
			long R = 0;
			int len = s == null ? 0 : s.Length - startIndex;
			if(len < 0) throw new ArgumentOutOfRangeException("startIndex");
			if(len != 0)
				fixed (char* p = s)
				{
					char* t = p + startIndex, e = t;
					R = strtoi64(t, out e, numberBase);
					len = (int)(e - t);
				}
			numberEndIndex = startIndex + len;
			return R;
		}

		/// <summary>
		/// This overload does not have parameter 'out int numberEndIndex'.
		/// </summary>
		public static int strtoi(string s, int startIndex = 0, int numberBase = 0)
		{
			int len;
			return strtoi(s, startIndex, out len, numberBase);
		}

		/// <summary>
		/// This overload does not have parameter 'out int numberEndIndex'.
		/// </summary>
		public static uint strtoui(string s, int startIndex = 0, int numberBase = 0)
		{
			int len;
			return strtoui(s, startIndex, out len, numberBase);
		}

		/// <summary>
		/// This overload does not have parameter 'out int numberEndIndex'.
		/// </summary>
		public static long strtoi64(string s, int startIndex = 0, int numberBase = 0)
		{
			int len;
			return strtoi64(s, startIndex, out len, numberBase);
		}

		/// <summary>
		/// This overload does not have parameter 'out char* endPtr'.
		/// </summary>
		public static int strtoi(char* s, int numberBase = 0)
		{
			char* endPtr;
			return strtoi(s, out endPtr, numberBase);
		}

		/// <summary>
		/// This overload does not have parameter 'out char* endPtr'.
		/// </summary>
		public static uint strtoui(char* s, int numberBase = 0)
		{
			char* endPtr;
			return strtoui(s, out endPtr, numberBase);
		}

		/// <summary>
		/// This overload does not have parameter 'out char* endPtr'.
		/// </summary>
		public static long strtoi64(char* s, int numberBase = 0)
		{
			char* endPtr;
			return strtoi64(s, out endPtr, numberBase);
		}




		//DWMAPI

		[DllImport("dwmapi.dll")]
		public static extern int DwmGetWindowAttribute(Wnd hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);
		[DllImport("dwmapi.dll")]
		public static extern int DwmGetWindowAttribute(Wnd hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);





		//OTHER

		[DllImport("uxtheme.dll", PreserveSig = true)]
		public static extern int SetWindowTheme(Wnd hwnd, string pszSubAppName, string pszSubIdList);





		//UTIL

		public static bool GetDelegate<T>(out T deleg, string dllName, string funcName) where T : class
		{
			deleg = null;
			IntPtr fa = GetProcAddress(dllName, funcName); if(fa == default(IntPtr)) return false;
			//deleg = (T)Marshal.GetDelegateForFunctionPointer(fa, typeof(T)); //error
			Type t = typeof(T);
			deleg = (T)Convert.ChangeType(Marshal.GetDelegateForFunctionPointer(fa, t), t);
			return deleg != null;
		}

		public static bool GetDelegate<T>(out T deleg, IntPtr hModule, string funcName) where T : class
		{
			deleg = null;
			IntPtr fa = GetProcAddress(hModule, funcName); if(fa == default(IntPtr)) return false;
			//deleg = (T)Marshal.GetDelegateForFunctionPointer(fa, typeof(T)); //error
			Type t = typeof(T);
			deleg = (T)Convert.ChangeType(Marshal.GetDelegateForFunctionPointer(fa, t), t);
			return deleg != null;
		}
	}
}
