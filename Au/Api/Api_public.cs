using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Au.Types;

#pragma warning disable 649, 169 //field never assigned/used

namespace Au
{
	/// <summary>
	/// Gets, sets or clears the last error code of Windows API. Gets error text.
	/// </summary>
	/// <remarks>
	/// Many Windows API functions, when failed, set an error code. Code 0 means no error. It is stored in an internal thread-specific int variable. But only if the API declaration's DllImport attribute has SetLastError = true.
	/// 
	/// Some functions of this library simply call these API functions and don't throw exception when API fail. For example, most <see cref="AWnd"/> propery-get functions.
	/// When failed, they return false/0/null/empty. Then you can use <see cref="Code"/> to get the error code or <see cref="Message"/> to get error text.
	/// 
	/// Most of functions set error code only when failed, and don't clear the old error code when succeeded. Therefore may need to call <see cref="Clear"/> before.
	///
	/// Windows API error code definitions and documentation are not included in this library. You can look for them in API function documentation on the internet.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// AWnd w = AWnd.Find("Notepag");
	/// ALastError.Clear();
	/// bool enabled = w.IsEnabled; //returns true if enabled, false if disabled or failed
	/// if(!enabled && ALastError.Code != 0) { AOutput.Write(ALastError.Message); return; } //1400, Invalid window handle
	/// AOutput.Write(enabled);
	/// ]]></code>
	/// </example>
	[DebuggerStepThrough]
	public static class ALastError
	{
		/// <summary>
		/// Calls API <msdn>SetLastError</msdn>(0), which clears the Windows API last error code of this thread.
		/// </summary>
		/// <remarks>
		/// Need it before calling some functions if you want to use <see cref="Code"/> or <see cref="Message"/>.
		/// The same as <c>ALastError.Code = 0;</c>.
		/// </remarks>
		public static void Clear() => Api.SetLastError(0);

		/// <summary>
		/// Gets (<see cref="Marshal.GetLastWin32Error"/>) or sets (API <msdn>SetLastError</msdn>) the Windows API last error code of this thread.
		/// </summary>
		public static int Code {
			get => Marshal.GetLastWin32Error();
			set => Api.SetLastError(value);
		}

		/// <summary>
		/// Gets the text message of the Windows API last error code of this thread.
		/// Returns null if the code is 0.
		/// </summary>
		/// <remarks>
		/// The string always ends with ".".
		/// </remarks>
		public static string Message => MessageFor(Code);

		/// <summary>
		/// Gets the text message of a Windows API error code.
		/// Returns null if errorCode is 0.
		/// </summary>
		/// <remarks>
		/// The string always ends with ".".
		/// </remarks>
		public static unsafe string MessageFor(int errorCode) {
			if (errorCode == 0) return null;
			if (errorCode == 1) return "The requested data or action is unavailable. (0x1)."; //or ERROR_INVALID_FUNCTION, but it's rare
			string s = "Unknown exception";
			char* p = null;
			const uint fl = Api.FORMAT_MESSAGE_FROM_SYSTEM | Api.FORMAT_MESSAGE_ALLOCATE_BUFFER | Api.FORMAT_MESSAGE_IGNORE_INSERTS;
			int r = Api.FormatMessage(fl, default, errorCode, 0, &p, 0, default);
			if (p != null) {
				while (r > 0 && p[r - 1] <= ' ') r--;
				if (r > 0) {
					if (p[r - 1] == '.') r--;
					s = new string(p, 0, r);
				}
				Api.LocalFree(p);
			}
			s = $"{s} (0x{errorCode:X}).";
			return s;
		}
	}
}

#pragma warning disable 1591 //missing XML documentation

namespace Au.Types
{
	/// <summary>
	/// Window styles.
	/// </summary>
	/// <remarks>
	/// Reference: <msdn>Window Styles</msdn>.
	/// Here names are without prefix WS_. For example, instead of WS_BORDER use WS.BORDER. Not included constants that are 0 (eg WS_TILED) or are duplicate (eg WS_SIZEBOX is same as WS_THICKFRAME) or consist of multiple other constants (eg WS_TILEDWINDOW).
	/// </remarks>
	[Flags]
	public enum WS : uint
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
		//these can cause bugs and confusion because consist of several styles. Not useful in C#, because used only to create native windows.
		//OVERLAPPEDWINDOW = CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
		//POPUPWINDOW = POPUP | BORDER | SYSMENU,
	}

	/// <summary>
	/// Window extended styles.
	/// </summary>
	/// <remarks>
	/// Reference: <msdn>Extended Window Styles</msdn>.
	/// Here names are without prefix WS_EX_. For example, instead of WS_EX_TOOLWINDOW use WSE.TOOLWINDOW. Not included constants that are 0 (eg WS_EX_LEFT).
	/// </remarks>
	[Flags]
	public enum WSE : uint
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

	/// <summary>API <msdn>MSG</msdn></summary>
	public struct MSG //WinMSG
	{
		public AWnd hwnd;
		public int message;
		public LPARAM wParam;
		public LPARAM lParam;
		public int time;
		public POINT pt;

		public override string ToString() {
			AWnd.More.PrintMsg(out string s, this, new() { Indent = false, Number = false });
			return s;
		}

		public static implicit operator MSG(in System.Windows.Forms.Message m)
			=> new MSG { hwnd = (AWnd)m.HWnd, message = m.Msg, wParam = m.WParam, lParam = m.LParam };

		public static implicit operator MSG(in System.Windows.Interop.MSG m)
			=> new MSG { hwnd = (AWnd)m.hwnd, message = m.message, wParam = m.wParam, lParam = m.lParam, time = m.time, pt = (m.pt_x, m.pt_y) };
	}

	/// <summary><see cref="GUITHREADINFO"/> flags.</summary>
	[Flags]
	public enum GTIFlags
	{
		CARETBLINKING = 0x1,
		INMOVESIZE = 0x2,
		INMENUMODE = 0x4,
		SYSTEMMENUMODE = 0x8,
		POPUPMENUMODE = 0x10,
	}

	/// <summary>API <msdn>GUITHREADINFO</msdn></summary>
	public struct GUITHREADINFO
	{
		public int cbSize;
		public GTIFlags flags;
		public AWnd hwndActive;
		public AWnd hwndFocus;
		public AWnd hwndCapture;
		public AWnd hwndMenuOwner;
		public AWnd hwndMoveSize;
		public AWnd hwndCaret;
		public RECT rcCaret;
	}

	/// <summary>API <msdn>CREATESTRUCT</msdn></summary>
	public unsafe struct CREATESTRUCT
	{
		public LPARAM lpCreateParams;
		public IntPtr hInstance;
		public LPARAM hMenu;
		public AWnd hwndParent;
		public int cy;
		public int cx;
		public int y;
		public int x;
		public WS style;
		public char* lpszName;
		public char* lpszClass;
		public WSE dwExStyle;

		public ReadOnlySpan<char> Name => lpszName == default ? default
			: new ReadOnlySpan<char>(lpszName, Util.CharPtr_.Length(lpszName));
		//public string Name => lpszName == default ? null : new string(lpszName);

		/// <summary>
		/// If lpszClass is atom, returns string with # prefix and atom value, like "#32770".
		/// </summary>
		public ReadOnlySpan<char> ClassName => (nuint)lpszClass < 0x10000 ? "#" + ((int)lpszClass).ToStringInvariant()
			: new ReadOnlySpan<char>(lpszClass, Util.CharPtr_.Length(lpszClass));
		//public string ClassName => (nuint)lpszClass < 0x10000 ? "#" + ((int)lpszClass).ToStringInvariant() : new string(lpszClass);

		//tested and documented: CBT hook can change only x y cx cy.
	}

	/// <summary>API <msdn>SIGDN</msdn></summary>
	public enum SIGDN : uint
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

	/// <summary>API <msdn>SetWindowPos</msdn> flags. Can be used with <see cref="AWnd.SetWindowPos"/>.</summary>
	/// <remarks>The _X flags are undocumented.</remarks>
	[Flags]
	[CLSCompliant(false)]
	public enum SWPFlags : uint
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
		_NOCLIENTSIZE = 0x800,
		_NOCLIENTMOVE = 0x1000,
		DEFERERASE = 0x2000,
		ASYNCWINDOWPOS = 0x4000,
		_STATECHANGED = 0x8000,
		//_10000000 = 0x10000000,
		_KNOWNFLAGS = 0xffff,

		//the undocumented flags would break ToString() if not defined
	}

	/// <summary>
	/// Special window handle values. Can be used with <see cref="AWnd.SetWindowPos"/>.
	/// See API <msdn>SetWindowPos</msdn>.
	/// </summary>
	public enum SpecHWND
	{
		TOP = 0,
		BOTTOM = 1,
		TOPMOST = -1,
		NOTOPMOST = -2,
		MESSAGE = -3,
		BROADCAST = 0xffff,
	}

	/// <summary>
	/// Window long constants. Used with <see cref="AWnd.GetWindowLong"/> and <see cref="AWnd.SetWindowLong"/>.
	/// See API <msdn>GetWindowLong</msdn>. See also API <msdn>SetWindowSubclass</msdn>.
	/// </summary>
	public static class GWLong
	{
		public const int WNDPROC = -4;
		public const int USERDATA = -21;
		public const int STYLE = -16;
		public const int ID = -12;
		public const int HWNDPARENT = -8;
		public const int HINSTANCE = -6;
		public const int EXSTYLE = -20;
		//info: also there are GWLP_, but their values are the same.

		public static class DWL
		{
			public static readonly int MSGRESULT = 0;
			public static readonly int DLGPROC = IntPtr.Size;
			public static readonly int USER = IntPtr.Size * 2;
		}
	}

	/// <summary>
	/// Window class long constants. Used with <see cref="AWnd.More.GetClassLong"/>.
	/// See API <msdn>WNDCLASSEX</msdn>, <msdn>GetClassLong</msdn>.
	/// </summary>
	public static class GCLong
	{
		public const int ATOM = -32;
		public const int WNDPROC = -24;
		public const int STYLE = -26;
		public const int MENUNAME = -8;
		public const int HMODULE = -16;
		public const int HICONSM = -34;
		public const int HICON = -14;
		public const int HCURSOR = -12;
		public const int HBRBACKGROUND = -10;
		public const int CBWNDEXTRA = -18;
		public const int CBCLSEXTRA = -20;
		//info: also there are GCLP_, but their values are the same.
	}

	/// <summary>API <msdn>WNDPROC</msdn></summary>
	public delegate LPARAM WNDPROC(AWnd w, int msg, LPARAM wParam, LPARAM lParam);

	/// <summary>API <msdn>SendMessageTimeout</msdn> flags. Used with <see cref="AWnd.SendTimeout"/>.</summary>
	[Flags]
	public enum SMTFlags : uint
	{
		BLOCK = 0x0001,
		ABORTIFHUNG = 0x0002,
		NOTIMEOUTIFNOTHUNG = 0x0008,
		ERRORONEXIT = 0x0020,
	}

	/// <summary>API <msdn>DrawTextEx</msdn> format flags.</summary>
	[Flags]
	public enum TFFlags
	{
		CENTER = 0x1,
		RIGHT = 0x2,
		VCENTER = 0x4,
		BOTTOM = 0x8,
		WORDBREAK = 0x10,
		SINGLELINE = 0x20,
		EXPANDTABS = 0x40,
		TABSTOP = 0x80,
		NOCLIP = 0x100,
		EXTERNALLEADING = 0x200,
		CALCRECT = 0x400,
		NOPREFIX = 0x800,
		INTERNAL = 0x1000,
		EDITCONTROL = 0x2000,
		PATH_ELLIPSIS = 0x4000,
		END_ELLIPSIS = 0x8000,
		MODIFYSTRING = 0x10000,
		RTLREADING = 0x20000,
		WORD_ELLIPSIS = 0x40000,
		NOFULLWIDTHCHARBREAK = 0x80000,
		HIDEPREFIX = 0x100000,
		PREFIXONLY = 0x200000
	}
}
