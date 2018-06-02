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

		/// <summary><msdn>SHSTOCKICONID</msdn></summary>
		/// <tocexclude />
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

		/// <summary><msdn>WNDPROC</msdn></summary>
		/// <tocexclude />
		public delegate LPARAM WNDPROC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam);

		//SWP_

		/// <exclude />
		public const uint SWP_NOSIZE = 0x1;
		/// <exclude />
		public const uint SWP_NOMOVE = 0x2;
		/// <exclude />
		public const uint SWP_NOZORDER = 0x4;
		/// <exclude />
		public const uint SWP_NOREDRAW = 0x8;
		/// <exclude />
		public const uint SWP_NOACTIVATE = 0x10;
		/// <exclude />
		public const uint SWP_FRAMECHANGED = 0x20;
		/// <exclude />
		public const uint SWP_SHOWWINDOW = 0x40;
		/// <exclude />
		public const uint SWP_HIDEWINDOW = 0x80;
		/// <exclude />
		public const uint SWP_NOCOPYBITS = 0x100;
		/// <exclude />
		public const uint SWP_NOOWNERZORDER = 0x200;
		/// <exclude />
		public const uint SWP_NOSENDCHANGING = 0x400;
		/// <exclude />
		public const uint SWP_DEFERERASE = 0x2000;
		/// <exclude />
		public const uint SWP_ASYNCWINDOWPOS = 0x4000;

		//HWND_

		/// <exclude />
		public static Wnd HWND_TOP => (Wnd)(LPARAM)0;
		/// <exclude />
		public static Wnd HWND_BOTTOM => (Wnd)(LPARAM)1;
		/// <exclude />
		public static Wnd HWND_TOPMOST => (Wnd)(LPARAM)(-1);
		/// <exclude />
		public static Wnd HWND_NOTOPMOST => (Wnd)(LPARAM)(-2);
		/// <exclude />
		public static Wnd HWND_MESSAGE => (Wnd)(LPARAM)(-3);
		/// <exclude />
		public static Wnd HWND_BROADCAST => (Wnd)(LPARAM)0xffff;

		//GWL_, DWL_, DWLP_

		/// <exclude />
		public const int GWL_WNDPROC = -4;
		/// <exclude />
		public const int GWL_USERDATA = -21;
		/// <exclude />
		public const int GWL_STYLE = -16;
		/// <exclude />
		public const int GWL_ID = -12;
		/// <exclude />
		public const int GWL_HWNDPARENT = -8;
		/// <exclude />
		public const int GWL_HINSTANCE = -6;
		/// <exclude />
		public const int GWL_EXSTYLE = -20;
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

		//GCW_

		/// <exclude />
		public const int GCW_ATOM = -32;
		/// <exclude />
		public const int GCL_WNDPROC = -24;
		/// <exclude />
		public const int GCL_STYLE = -26;
		/// <exclude />
		public const int GCL_MENUNAME = -8;
		/// <exclude />
		public const int GCL_HMODULE = -16;
		/// <exclude />
		public const int GCL_HICONSM = -34;
		/// <exclude />
		public const int GCL_HICON = -14;
		/// <exclude />
		public const int GCL_HCURSOR = -12;
		/// <exclude />
		public const int GCL_HBRBACKGROUND = -10;
		/// <exclude />
		public const int GCL_CBWNDEXTRA = -18;
		/// <exclude />
		public const int GCL_CBCLSEXTRA = -20;
		//info: also there are GCLP_, but their values are the same.

		//WS_

		/// <exclude />
		public const uint WS_POPUP = 0x80000000;
		/// <exclude />
		public const uint WS_CHILD = 0x40000000;
		/// <exclude />
		public const uint WS_MINIMIZE = 0x20000000;
		/// <exclude />
		public const uint WS_VISIBLE = 0x10000000;
		/// <exclude />
		public const uint WS_DISABLED = 0x08000000;
		/// <exclude />
		public const uint WS_CLIPSIBLINGS = 0x04000000;
		/// <exclude />
		public const uint WS_CLIPCHILDREN = 0x02000000;
		/// <exclude />
		public const uint WS_MAXIMIZE = 0x01000000;
		/// <exclude />
		public const uint WS_BORDER = 0x00800000;
		/// <exclude />
		public const uint WS_DLGFRAME = 0x00400000;
		/// <exclude />
		public const uint WS_VSCROLL = 0x00200000;
		/// <exclude />
		public const uint WS_HSCROLL = 0x00100000;
		/// <exclude />
		public const uint WS_SYSMENU = 0x00080000;
		/// <exclude />
		public const uint WS_THICKFRAME = 0x00040000;
		/// <exclude />
		public const uint WS_GROUP = 0x00020000;
		/// <exclude />
		public const uint WS_TABSTOP = 0x00010000;
		/// <exclude />
		public const uint WS_MINIMIZEBOX = 0x00020000;
		/// <exclude />
		public const uint WS_MAXIMIZEBOX = 0x00010000;
		/// <exclude />
		public const uint WS_CAPTION = WS_BORDER | WS_DLGFRAME;
		/// <exclude />
		public const uint WS_OVERLAPPEDWINDOW = WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
		/// <exclude />
		public const uint WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU;

		//WS_EX_

		/// <exclude />
		public const uint WS_EX_DLGMODALFRAME = 0x00000001;
		/// <exclude />
		public const uint WS_EX_NOPARENTNOTIFY = 0x00000004;
		/// <exclude />
		public const uint WS_EX_TOPMOST = 0x00000008;
		/// <exclude />
		public const uint WS_EX_ACCEPTFILES = 0x00000010;
		/// <exclude />
		public const uint WS_EX_TRANSPARENT = 0x00000020;
		/// <exclude />
		public const uint WS_EX_MDICHILD = 0x00000040;
		/// <exclude />
		public const uint WS_EX_TOOLWINDOW = 0x00000080;
		/// <exclude />
		public const uint WS_EX_WINDOWEDGE = 0x00000100;
		/// <exclude />
		public const uint WS_EX_CLIENTEDGE = 0x00000200;
		/// <exclude />
		public const uint WS_EX_CONTEXTHELP = 0x00000400;
		/// <exclude />
		public const uint WS_EX_RIGHT = 0x00001000;
		/// <exclude />
		public const uint WS_EX_LEFT = 0x00000000;
		/// <exclude />
		public const uint WS_EX_RTLREADING = 0x00002000;
		/// <exclude />
		public const uint WS_EX_LTRREADING = 0x00000000;
		/// <exclude />
		public const uint WS_EX_LEFTSCROLLBAR = 0x00004000;
		/// <exclude />
		public const uint WS_EX_RIGHTSCROLLBAR = 0x00000000;
		/// <exclude />
		public const uint WS_EX_CONTROLPARENT = 0x00010000;
		/// <exclude />
		public const uint WS_EX_STATICEDGE = 0x00020000;
		/// <exclude />
		public const uint WS_EX_APPWINDOW = 0x00040000;
		/// <exclude />
		public const uint WS_EX_LAYERED = 0x00080000;
		/// <exclude />
		public const uint WS_EX_NOINHERITLAYOUT = 0x00100000;
		/// <exclude />
		public const uint WS_EX_LAYOUTRTL = 0x00400000;
		/// <exclude />
		public const uint WS_EX_COMPOSITED = 0x02000000;
		/// <exclude />
		public const uint WS_EX_NOACTIVATE = 0x08000000;
		/// <exclude />
		public const uint WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

		//SMTO_

		/// <exclude />
		public const uint SMTO_BLOCK = 0x0001;
		/// <exclude />
		public const uint SMTO_ABORTIFHUNG = 0x0002;
		/// <exclude />
		public const uint SMTO_NOTIMEOUTIFNOTHUNG = 0x0008;
		/// <exclude />
		public const uint SMTO_ERRORONEXIT = 0x0020;
	}
}
