using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text;

using Catkeys;
using static Catkeys.NoClass;

#pragma warning disable 1591 //missing XML documentation

namespace Catkeys
{
	/// <summary>
	/// Windows API used with public functions (parameters etc) of this library.
	/// </summary>
	/// <tocexclude />
	[DebuggerStepThrough]
	[CLSCompliant(false)]
	public static unsafe partial class Native
	{
		/// <tocexclude />
		public struct MSG { public Wnd hwnd; public uint message; public LPARAM wParam; public LPARAM lParam; public uint time; public POINT pt; }

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

		/// <tocexclude />
		public struct WINDOWPLACEMENT
		{
			public uint length;
			public uint flags; //WPF_
			public int showCmd; //SW_
			public POINT ptMinPosition;
			public POINT ptMaxPosition;
			public RECT rcNormalPosition;
		}

		/// <tocexclude />
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

		/// <tocexclude />
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

		/// <tocexclude />
		public delegate LPARAM WNDPROC(Wnd w, uint msg, LPARAM wParam, LPARAM lParam);

	}
}
