using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Au.Types;
using static Au.NoClass;

#pragma warning disable 1591 //missing XML documentation

namespace Au.Types
{
	/// <summary>
	/// Windows API types and constants used with public functions (parameters etc) of this library.
	/// Also several helper functions.
	/// </summary>
	[DebuggerStepThrough]
	[CLSCompliant(false)]
	public static unsafe partial class Native
	{
		/// <summary>
		/// Calls API <msdn>SetLastError</msdn>(0), which clears the Windows API error code of this thread.
		/// Need it before calling some functions if you want to use <see cref="GetError"/> or <see cref="GetErrorMessage()"/>.
		/// </summary>
		public static void ClearError()
		{
			Api.SetLastError(0);
		}

		/// <summary>
		/// Calls API <msdn>SetLastError</msdn>, which sets the Windows API error code of this thread.
		/// </summary>
		public static void SetError(int errorCode)
		{
			Api.SetLastError(errorCode);
		}

		/// <summary>
		/// Gets the Windows API error code of this thread.
		/// Calls <see cref="Marshal.GetLastWin32Error"/>.
		/// </summary>
		/// <remarks>
		/// Many Windows API functions, when failed, set an error code. Code 0 means no error. It is stored in an internal thread-specific int variable. But only if the API declaration's DllImport attribute has SetLastError = true.
		/// Some functions of this library simply call these API functions and don't throw exception when API fail. For example, most Wnd propery-get functions.
		/// When failed, they return false/0/null/empty. Then you can call <b>Native.GetError</b> to get the error code. Also you can use <see cref="GetErrorMessage()"/>.
		/// 
		/// Most of these functions set the code only when failed, and don't clear old error code when succeeded. Therefore may need to call <see cref="ClearError"/> before.
		///
		/// Windows API error code definitions and documentation are not included in this library. You can look for them in API function documentation on the internet.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = Wnd.Find("Notepag");
		/// //if(w.Is0) return; //assume you don't use this
		/// Native.ClearError();
		/// bool enabled = w.IsEnabled; //returns true if enabled, false if disabled or failed
		/// int e = Native.GetError();
		/// if(e != 0) { Print(e, Native.GetErrorMessage(e)); return; } //1400, Invalid window handle
		/// Print(enabled);
		/// ]]></code>
		/// </example>
		public static int GetError()
		{
			return Marshal.GetLastWin32Error();
		}

		/// <summary>
		/// Gets the text message of the Windows API error code of this thread.
		/// Returns null if the code is 0.
		/// The string always ends with ".".
		/// </summary>
		public static string GetErrorMessage()
		{
			return GetErrorMessage(GetError());
		}

		/// <summary>
		/// Gets the text message of a Windows API error code.
		/// Returns null if errorCode is 0.
		/// The string always ends with ".".
		/// </summary>
		public static string GetErrorMessage(int errorCode)
		{
			if(errorCode == 0) return null;
			if(errorCode == 1) return "The requested data or action is unavailable. (0x1)."; //or ERROR_INVALID_FUNCTION, but it's rare
			string s = "Unknown exception";
			char* p = null;
			const uint fl = Api.FORMAT_MESSAGE_FROM_SYSTEM | Api.FORMAT_MESSAGE_ALLOCATE_BUFFER | Api.FORMAT_MESSAGE_IGNORE_INSERTS;
			int r = Api.FormatMessage(fl, default, errorCode, 0, &p, 0, default);
			if(p != null) {
				while(r > 0 && p[r - 1] <= ' ') r--;
				if(r > 0) {
					if(p[r - 1] == '.') r--;
					s = new string(p, 0, r);
				}
				Api.LocalFree(p);
			}
			s = $"{s} (0x{errorCode:X}).";
			return s;
		}

		/// <summary><msdn>MSG</msdn></summary>
		/// <tocexclude />
		public struct MSG
		{
			public Wnd hwnd;
			public uint message;
			public LPARAM wParam;
			public LPARAM lParam;
			public uint time;
			public POINT pt;

			public override string ToString()
			{
				return System.Windows.Forms.Message.Create(hwnd.Handle, (int)message, wParam, lParam).ToString();
			}
		}

		/// <summary><msdn>GUITHREADINFO</msdn></summary>
		/// <tocexclude />
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

#pragma warning disable 649, 169 //field never assigned/used
		/// <summary><msdn>CREATESTRUCT</msdn></summary>
		/// <remarks>
		/// lpszClass is unavailable, because often it is atom. Instead use <see cref="Wnd.ClassName"/>.
		/// </remarks>
		/// <tocexclude />
		public struct CREATESTRUCT
		{
			public IntPtr lpCreateParams;
			public IntPtr hInstance;
			public IntPtr hMenu;
			public Wnd hwndParent;
			public int cy;
			public int cx;
			public int y;
			public int x;
			public uint style;
			LPARAM _lpszName;
			LPARAM _lpszClass;
			public uint dwExStyle;

			public unsafe string lpszName => _lpszName == default ? null : new string((char*)_lpszName);

			//tested and documented: hook can change only x y cx cy.
		}
#pragma warning restore 649, 169

		/// <summary><msdn>MOUSEHOOKSTRUCT</msdn></summary>
		/// <tocexclude />
		public struct MOUSEHOOKSTRUCT
		{
			public POINT pt;
			public Wnd hwnd;
			public uint wHitTestCode;
			public LPARAM dwExtraInfo;
		}

		/// <summary><msdn>CWPSTRUCT</msdn></summary>
		/// <tocexclude />
		public struct CWPSTRUCT
		{
			public LPARAM lParam;
			public LPARAM wParam;
			public uint message;
			public Wnd hwnd;
		}

		/// <summary><msdn>CWPRETSTRUCT</msdn></summary>
		/// <tocexclude />
		public struct CWPRETSTRUCT
		{
			public LPARAM lResult;
			public LPARAM lParam;
			public LPARAM wParam;
			public uint message;
			public Wnd hwnd;
		}

		/// <summary><msdn>SIGDN</msdn></summary>
		/// <tocexclude />
		public enum SIGDN :uint
		{
			NORMALDISPLAY,
			PARENTRELATIVEPARSING = 0x80018001,
			DESKTOPABSOLUTEPARSING = 0x80028000,
			PARENTRELATIVEEDITING = 0x80031001,
			DESKTOPABSOLUTEEDITING = 0x8004C000,
			FILESYSPATH = 0x80058000,
			URL = 0x80068000,
			PARENTRELATIVEFORADDRESSBAR = 0x8007C001,
			PARENTRELATIVE = 0x80080001,
			PARENTRELATIVEFORUI = 0x80094001
		}

		/// <summary><msdn>WNDPROC</msdn></summary>
		/// <tocexclude />
		public delegate LPARAM WNDPROC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam);

		/// <summary><msdn>SetWindowPos</msdn></summary>
		/// <tocexclude />
		[Flags]
		public enum SWP :uint
		{
			NOSIZE = 0x1,
			NOMOVE = 0x2,
			NOZORDER = 0x4,
			NOREDRAW = 0x8,
			NOACTIVATE = 0x10,
			FRAMECHANGED = 0x20,
			SHOWWINDOW = 0x40,
			HIDEWINDOW = 0x80,
			NOCOPYBITS = 0x100,
			NOOWNERZORDER = 0x200,
			NOSENDCHANGING = 0x400,
			DEFERERASE = 0x2000,
			ASYNCWINDOWPOS = 0x4000,
		}

		/// <summary><msdn>SetWindowPos</msdn></summary>
		/// <tocexclude />
		public enum HWND
		{
			TOP = 0,
			BOTTOM = 1,
			TOPMOST = -1,
			NOTOPMOST = -2,
			MESSAGE = -3,
			BROADCAST = 0xffff,
		}

		/// <summary><msdn>GetWindowLong</msdn></summary>
		/// <tocexclude />
		public static class GWL
		{
			/// <exclude />
			public const int WNDPROC = -4;
			/// <exclude />
			public const int USERDATA = -21;
			/// <exclude />
			public const int STYLE = -16;
			/// <exclude />
			public const int ID = -12;
			/// <exclude />
			public const int HWNDPARENT = -8;
			/// <exclude />
			public const int HINSTANCE = -6;
			/// <exclude />
			public const int EXSTYLE = -20;
			//info: also there are GWLP_, but their values are the same.

			//#define DWLP_MSGRESULT  0
			//#define DWLP_DLGPROC    DWLP_MSGRESULT + sizeof(LRESULT)
			//#define DWLP_USER       DWLP_DLGPROC + sizeof(DLGPROC)

			/// <exclude />
			public const int DWL_MSGRESULT = 0;
			/// <exclude />
			public const int DWL_DLGPROC_32 = 4;
			/// <exclude />
			public const int DWL_DLGPROC_64 = 8;
			/// <exclude />
			public const int DWL_USER_32 = 8;
			/// <exclude />
			public const int DWL_USER_64 = 16;

			/// <exclude />
			public static int DWLP_DLGPROC => IntPtr.Size;
			/// <exclude />
			public static int DWLP_USER => IntPtr.Size * 2;
		}

		/// <summary><msdn>WNDCLASSEX</msdn>, <msdn>GetClassLong</msdn></summary>
		/// <tocexclude />
		public static class GCL
		{
			/// <exclude />
			public const int ATOM = -32;
			/// <exclude />
			public const int WNDPROC = -24;
			/// <exclude />
			public const int STYLE = -26;
			/// <exclude />
			public const int MENUNAME = -8;
			/// <exclude />
			public const int HMODULE = -16;
			/// <exclude />
			public const int HICONSM = -34;
			/// <exclude />
			public const int HICON = -14;
			/// <exclude />
			public const int HCURSOR = -12;
			/// <exclude />
			public const int HBRBACKGROUND = -10;
			/// <exclude />
			public const int CBWNDEXTRA = -18;
			/// <exclude />
			public const int CBCLSEXTRA = -20;
			//info: also there are GCLP_, but their values are the same.
		}

		/// <summary><msdn>CreateWindowEx</msdn></summary>
		/// <tocexclude />
		[Flags]
		public enum WS :uint
		{
			POPUP = 0x80000000,
			CHILD = 0x40000000,
			MINIMIZE = 0x20000000,
			VISIBLE = 0x10000000,
			DISABLED = 0x08000000,
			CLIPSIBLINGS = 0x04000000,
			CLIPCHILDREN = 0x02000000,
			MAXIMIZE = 0x01000000,
			BORDER = 0x00800000,
			DLGFRAME = 0x00400000,
			VSCROLL = 0x00200000,
			HSCROLL = 0x00100000,
			SYSMENU = 0x00080000,
			THICKFRAME = 0x00040000,
			MINIMIZEBOX = 0x00020000,
			GROUP = 0x00020000,
			MAXIMIZEBOX = 0x00010000,
			TABSTOP = 0x00010000,
			CAPTION = BORDER | DLGFRAME,
			OVERLAPPEDWINDOW = CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
			POPUPWINDOW = POPUP | BORDER | SYSMENU,
		}

		/// <summary><msdn>CreateWindowEx</msdn></summary>
		/// <tocexclude />
		[Flags]
		public enum WS_EX :uint
		{
			DLGMODALFRAME = 0x00000001,
			NOPARENTNOTIFY = 0x00000004,
			TOPMOST = 0x00000008,
			ACCEPTFILES = 0x00000010,
			TRANSPARENT = 0x00000020,
			MDICHILD = 0x00000040,
			TOOLWINDOW = 0x00000080,
			WINDOWEDGE = 0x00000100,
			CLIENTEDGE = 0x00000200,
			CONTEXTHELP = 0x00000400,
			//LEFT = 0x00000000,
			RIGHT = 0x00001000,
			//LTRREADING = 0x00000000,
			RTLREADING = 0x00002000,
			//RIGHTSCROLLBAR = 0x00000000,
			LEFTSCROLLBAR = 0x00004000,
			CONTROLPARENT = 0x00010000,
			STATICEDGE = 0x00020000,
			APPWINDOW = 0x00040000,
			LAYERED = 0x00080000,
			NOINHERITLAYOUT = 0x00100000,
			NOREDIRECTIONBITMAP = 0x00200000,
			LAYOUTRTL = 0x00400000,
			COMPOSITED = 0x02000000,
			NOACTIVATE = 0x08000000,
		}

		/// <summary><msdn>SendMessageTimeout</msdn></summary>
		/// <tocexclude />
		[Flags]
		public enum SMTO :uint
		{
			BLOCK = 0x0001,
			ABORTIFHUNG = 0x0002,
			NOTIMEOUTIFNOTHUNG = 0x0008,
			ERRORONEXIT = 0x0020,
		}
	}
}
