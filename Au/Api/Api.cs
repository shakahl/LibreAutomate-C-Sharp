using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

[module: DefaultCharSet(CharSet.Unicode)] //change default DllImport CharSet from ANSI to Unicode

//[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32|DllImportSearchPath.UserDirectories)]

[assembly: InternalsVisibleTo("Au.Controls, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Tools, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Editor, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Tasks, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Compiler, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("Au.Triggers, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("SdkConverter, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("TreeList, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]
[assembly: InternalsVisibleTo("LineBreaks, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d7836581375ad28892abd6476a89a68f879d2df07404cfcddf2899cd05616f8fb45c9bab78b972a2ca99339af3774b0a2b6f2a5768acdf2995a255106943fffa9aa65d66a37829f7ebbc7c0ffc75b6d2bf95c1964ec84774834c07438584125afdfb58b77b5411c1401589adbefadef502893b8c8cff8b682b05043703ca479e")]

namespace Au.Types
{
	[DebuggerStepThrough]
	[System.Security.SuppressUnmanagedCodeSecurity]
	internal static unsafe partial class Api
	{
		#region util

		/// <summary>
		/// Gets the native size of a struct variable.
		/// Returns (uint)Marshal.SizeOf(typeof(T)).
		/// Speed: the same (in Release config) as Marshal.SizeOf(typeof(T)), and 2 times faster than Marshal.SizeOf(v).
		/// </summary>
		internal static uint SizeOf<T>(T v) { return (uint)Marshal.SizeOf(typeof(T)); }

		/// <summary>
		/// Gets the native size of a type.
		/// Returns (uint)Marshal.SizeOf(typeof(T)).
		/// </summary>
		internal static uint SizeOf<T>() { return (uint)Marshal.SizeOf(typeof(T)); }

		/// <summary>
		/// Gets dll module handle (Api.GetModuleHandle) or loads dll (Api.LoadLibrary), and returns unmanaged exported function address (Api.GetProcAddress).
		/// See also: GetDelegate.
		/// </summary>
		internal static IntPtr GetProcAddress(string dllName, string funcName)
		{
			IntPtr hmod = GetModuleHandle(dllName);
			if(hmod == default) { hmod = LoadLibrary(dllName); if(hmod == default) return hmod; }

			return GetProcAddress(hmod, funcName);
		}

		/// <summary>
		/// Calls <see cref="GetProcAddress(string, string)"/> (loads dll or gets handle) and <see cref="Marshal.GetDelegateForFunctionPointer"/>.
		/// </summary>
		internal static bool GetDelegate<T>(out T deleg, string dllName, string funcName) where T : class
		{
			deleg = null;
			IntPtr fa = GetProcAddress(dllName, funcName); if(fa == default) return false;
			deleg = Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(fa, typeof(T)));
			return deleg != null;
		}

		/// <summary>
		/// Calls API <see cref="GetProcAddress(IntPtr, string)"/> and <see cref="Marshal.GetDelegateForFunctionPointer"/>.
		/// </summary>
		internal static bool GetDelegate<T>(out T deleg, IntPtr hModule, string funcName) where T : class
		{
			deleg = null;
			IntPtr fa = GetProcAddress(hModule, funcName); if(fa == default) return false;
			deleg = Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(fa, typeof(T)));
			return deleg != null;
		}

		/// <summary>
		/// Calls <see cref="Marshal.GetDelegateForFunctionPointer"/>.
		/// </summary>
		/// <typeparam name="T">Delegate type.</typeparam>
		/// <param name="f">Unmanaged function address.</param>
		/// <param name="deleg">Receives managed delegate of type T.</param>
		internal static void GetDelegate<T>(IntPtr f, out T deleg) where T : class
		{
			deleg = Unsafe.As<T>(Marshal.GetDelegateForFunctionPointer(f, typeof(T)));
		}

		/// <summary>
		/// If o is not null, calls <see cref="Marshal.ReleaseComObject"/>.
		/// </summary>
		internal static void ReleaseComObject<T>(T o) where T : class
		{
			if(o != null) Marshal.ReleaseComObject(o);
		}

		#endregion

		#region gdi32

		[DllImport("gdi32.dll")] //this and many other GDI functions don't use SetLastError
		internal static extern bool DeleteObject(IntPtr ho);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);

		internal const int RGN_AND = 1;
		internal const int RGN_OR = 2;
		internal const int RGN_XOR = 3;
		internal const int RGN_DIFF = 4;
		internal const int RGN_COPY = 5;

		[DllImport("gdi32.dll")]
		internal static extern int CombineRgn(IntPtr hrgnDst, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int iMode);

		[DllImport("gdi32.dll")]
		internal static extern bool SetRectRgn(IntPtr hrgn, int left, int top, int right, int bottom);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr GetDC(Wnd hWnd);

		//[DllImport("user32.dll", SetLastError = true)]
		//internal static extern IntPtr GetWindowDC(Wnd hWnd);

		[DllImport("user32.dll")] //note: no SetLastError = true
		internal static extern int ReleaseDC(Wnd hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr h);

		[DllImport("gdi32.dll", EntryPoint = "GetObjectW")]
		internal static extern int GetObject(IntPtr h, int c, void* pv);

		[DllImport("gdi32.dll")] //tested: does not set last error
		internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

		[DllImport("gdi32.dll")]
		internal static extern int GetDeviceCaps(IntPtr hdc, int index);

		[DllImport("gdi32.dll", EntryPoint = "GetTextExtentPoint32W")]
		internal static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int c, out Size psizl);

		[DllImport("gdi32.dll", EntryPoint = "CreateFontW")]
		internal static extern IntPtr CreateFont(int cHeight, int cWidth = 0, int cEscapement = 0, int cOrientation = 0, int cWeight = 0, int bItalic = 0, int bUnderline = 0, int bStrikeOut = 0, int iCharSet = 0, int iOutPrecision = 0, int iClipPrecision = 0, int iQuality = 0, int iPitchAndFamily = 0, string pszFaceName = null);

		//[DllImport("user32.dll", EntryPoint = "CharUpperBuffW")]
		//internal static unsafe extern int CharUpperBuff(char* lpsz, int cchLength);

		[DllImport("user32.dll")]
		internal static extern int FillRect(IntPtr hDC, ref RECT lprc, IntPtr hbr);

		[DllImport("gdi32.dll")] //tested: in some cases does not set last error even if returns false
		internal static extern bool BitBlt(IntPtr hdc, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

		internal struct BITMAPINFOHEADER
		{
			public int biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public int biCompression;
			public int biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public int biClrUsed;
			public int biClrImportant;
		}

		[DllImport("gdi32.dll")]
		internal static extern int GetDIBits(IntPtr hdc, IntPtr hbm, int start, int cLines, void* lpvBits, BITMAPINFOHEADER* lpbmi, uint usage);

		internal const int WHITE_BRUSH = 0;
		internal const int LTGRAY_BRUSH = 1;
		internal const int GRAY_BRUSH = 2;
		internal const int DKGRAY_BRUSH = 3;
		internal const int BLACK_BRUSH = 4;
		internal const int NULL_BRUSH = 5;
		internal const int HOLLOW_BRUSH = 5;
		internal const int WHITE_PEN = 6;
		internal const int BLACK_PEN = 7;
		internal const int NULL_PEN = 8;
		internal const int OEM_FIXED_FONT = 10;
		internal const int ANSI_FIXED_FONT = 11;
		internal const int ANSI_VAR_FONT = 12;
		internal const int SYSTEM_FONT = 13;
		internal const int DEVICE_DEFAULT_FONT = 14;
		internal const int DEFAULT_PALETTE = 15;
		internal const int SYSTEM_FIXED_FONT = 16;
		internal const int DEFAULT_GUI_FONT = 17;
		internal const int DC_BRUSH = 18;
		internal const int DC_PEN = 19;

		[DllImport("gdi32.dll")]
		internal static extern IntPtr GetStockObject(int i);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateSolidBrush(uint color);

		[DllImport("gdi32.dll")]
		internal static extern IntPtr CreateRectRgnIndirect(ref RECT lprect);

		[DllImport("gdi32.dll")]
		internal static extern bool FrameRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr, int w, int h);




		#endregion

		#region advapi32

		internal const uint TOKEN_WRITE = STANDARD_RIGHTS_WRITE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT;
		internal const uint TOKEN_SOURCE_LENGTH = 8;
		internal const uint TOKEN_READ = STANDARD_RIGHTS_READ | TOKEN_QUERY;
		internal const uint TOKEN_QUERY_SOURCE = 16;
		internal const uint TOKEN_QUERY = 8;
		internal const uint TOKEN_IMPERSONATE = 4;
		internal const uint TOKEN_EXECUTE = STANDARD_RIGHTS_EXECUTE;
		internal const uint TOKEN_DUPLICATE = 2;
		internal const uint TOKEN_AUDIT_SUCCESS_INCLUDE = 1;
		internal const uint TOKEN_AUDIT_SUCCESS_EXCLUDE = 2;
		internal const uint TOKEN_AUDIT_FAILURE_INCLUDE = 4;
		internal const uint TOKEN_AUDIT_FAILURE_EXCLUDE = 8;
		internal const uint TOKEN_ASSIGN_PRIMARY = 1;
		internal const uint TOKEN_ALL_ACCESS_P = STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE | TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT;
		internal const uint TOKEN_ALL_ACCESS = TOKEN_ALL_ACCESS_P | TOKEN_ADJUST_SESSIONID;
		internal const uint TOKEN_ADJUST_SESSIONID = 256;
		internal const uint TOKEN_ADJUST_PRIVILEGES = 32;
		internal const uint TOKEN_ADJUST_GROUPS = 64;
		internal const uint TOKEN_ADJUST_DEFAULT = 128;

		[DllImport("advapi32.dll", SetLastError = true)]
		internal static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

		internal enum TOKEN_INFORMATION_CLASS
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
		internal static extern bool GetTokenInformation(HandleRef TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, void* TokenInformation, uint TokenInformationLength, out uint ReturnLength);

		[DllImport("advapi32.dll")]
		internal static extern byte* GetSidSubAuthorityCount(IntPtr pSid);

		[DllImport("advapi32.dll")]
		internal static extern uint* GetSidSubAuthority(IntPtr pSid, uint nSubAuthority);

		[DllImport("advapi32.dll")]
		internal static extern int RegSetValueEx(IntPtr hKey, string lpValueName, int Reserved, Microsoft.Win32.RegistryValueKind dwType, void* lpData, int cbData);

		[DllImport("advapi32.dll")]
		internal static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, IntPtr Reserved, out Microsoft.Win32.RegistryValueKind dwType, void* lpData, ref int cbData);

		[StructLayout(LayoutKind.Sequential)]
		internal sealed class SECURITY_ATTRIBUTES :IDisposable
		{
			public int nLength;
			public void* lpSecurityDescriptor;
			public int bInheritHandle;

			/// <summary>
			/// Creates SECURITY_ATTRIBUTES that allows UAC low integrity level processes to open the kernel object.
			/// </summary>
			public SECURITY_ATTRIBUTES()
			{
				nLength = IntPtr.Size * 3;
				if(!ConvertStringSecurityDescriptorToSecurityDescriptor("D:NO_ACCESS_CONTROLS:(ML;;NW;;;LW)", 1, out lpSecurityDescriptor)) throw new AuException(0, "SECURITY_ATTRIBUTES");
			}

			public void Dispose()
			{
				if(lpSecurityDescriptor != null) {
					LocalFree(lpSecurityDescriptor);
					lpSecurityDescriptor = null;
				}
			}

			~SECURITY_ATTRIBUTES() => Dispose();

			public static SECURITY_ATTRIBUTES Common = new SECURITY_ATTRIBUTES();
		}

		[DllImport("advapi32.dll", EntryPoint = "ConvertStringSecurityDescriptorToSecurityDescriptorW", SetLastError = true)]
		internal static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(string StringSecurityDescriptor, uint StringSDRevision, out void* SecurityDescriptor, uint* SecurityDescriptorSize = null);

		//[DllImport("advapi32.dll", EntryPoint = "ConvertSecurityDescriptorToStringSecurityDescriptorW")]
		//internal static extern bool ConvertSecurityDescriptorToStringSecurityDescriptor(void* SecurityDescriptor, uint RequestedStringSDRevision, uint SecurityInformation, out char* StringSecurityDescriptor, out uint StringSecurityDescriptorLen);




		#endregion

		#region shell32

		//[DllImport("shell32.dll")]
		//internal static extern bool IsUserAnAdmin();

		internal const uint SHGFI_ICON = 0x000000100;     // get icon;
		internal const uint SHGFI_DISPLAYNAME = 0x000000200;     // get display name;
		internal const uint SHGFI_TYPENAME = 0x000000400;     // get type name;
		internal const uint SHGFI_ATTRIBUTES = 0x000000800;     // get attributes;
		internal const uint SHGFI_ICONLOCATION = 0x000001000;     // get icon location;
		internal const uint SHGFI_EXETYPE = 0x000002000;     // return exe type;
		internal const uint SHGFI_SYSICONINDEX = 0x000004000;     // get system icon index;
		internal const uint SHGFI_LINKOVERLAY = 0x000008000;     // put a link overlay on icon;
		internal const uint SHGFI_SELECTED = 0x000010000;     // show icon in selected state;
		internal const uint SHGFI_ATTR_SPECIFIED = 0x000020000;     // get only specified attributes;
		internal const uint SHGFI_LARGEICON = 0x000000000;     // get large icon;
		internal const uint SHGFI_SMALLICON = 0x000000001;     // get small icon;
		internal const uint SHGFI_OPENICON = 0x000000002;     // get open icon;
		internal const uint SHGFI_SHELLICONSIZE = 0x000000004;     // get shell size icon;
		internal const uint SHGFI_PIDL = 0x000000008;     // pszPath is a pidl;
		internal const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute;
		internal const uint SHGFI_ADDOVERLAYS = 0x000000020;     // apply the appropriate overlays;
		internal const uint SHGFI_OVERLAYINDEX = 0x000000040;     // Get the index of the overlay;

		internal struct SHFILEINFO
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
		internal static extern LPARAM SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

		[DllImport("shell32.dll", EntryPoint = "SHGetFileInfoW")]
		internal static extern LPARAM SHGetFileInfo(IntPtr pidl, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetDesktopFolder(out IShellFolder ppshf);

		[DllImport("shell32.dll")]
		internal static extern int SHParseDisplayName(string pszName, IntPtr pbc, out IntPtr pidl, uint sfgaoIn, uint* psfgaoOut);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetNameFromIDList(IntPtr pidl, Native.SIGDN sigdnName, out string ppszName);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHCreateShellItem(IntPtr pidlParent, IShellFolder psfParent, IntPtr pidl, out IShellItem ppsi);
		//This classic API supports absolute PIDL and parent+relative PIDL.
		//There are 2 newer API - SHCreateItemFromIDList (absoulte) and SHCreateItemWithParent (parent+relative). They can get IShellItem2 too, which is currently not useful here. Same speed.

		//[DllImport("shell32.dll", PreserveSig = true)]
		//internal static extern int SHCreateItemFromIDList(IntPtr pidl, ref Guid riid, out IShellItem ppv); //or IShellItem2

		//[DllImport("shell32.dll", PreserveSig = true)]
		//internal static extern int SHBindToParent(IntPtr pidl, ref Guid riid, out IShellFolder ppv, out IntPtr ppidlLast);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetPropertyStoreForWindow(Wnd hwnd, ref Guid riid, out IPropertyStore ppv);

		internal static PROPERTYKEY PKEY_AppUserModel_ID = new PROPERTYKEY() { fmtid = new Guid(0x9F4C2855, 0x9F79, 0x4B39, 0xA8, 0xD0, 0xE1, 0xD4, 0x2D, 0xE1, 0xD5, 0xF3), pid = 5 };

		[DllImport("shell32.dll")]
		internal static extern IntPtr* CommandLineToArgvW(string lpCmdLine, out int pNumArgs);

		internal struct NOTIFYICONDATA
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

		internal const uint NIN_SELECT = 0x400;
		internal const uint NINF_KEY = 0x1;
		internal const uint NIN_KEYSELECT = 0x401;
		internal const uint NIN_BALLOONSHOW = 0x402;
		internal const uint NIN_BALLOONHIDE = 0x403;
		internal const uint NIN_BALLOONTIMEOUT = 0x404;
		internal const uint NIN_BALLOONUSERCLICK = 0x405;
		internal const uint NIN_POPUPOPEN = 0x406;
		internal const uint NIN_POPUPCLOSE = 0x407;
		internal const uint NIM_ADD = 0x0;
		internal const uint NIM_MODIFY = 0x1;
		internal const uint NIM_DELETE = 0x2;
		internal const uint NIM_SETFOCUS = 0x3;
		internal const uint NIM_SETVERSION = 0x4;
		internal const int NOTIFYICON_VERSION = 3;
		internal const int NOTIFYICON_VERSION_4 = 4;
		internal const uint NIF_MESSAGE = 0x1;
		internal const uint NIF_ICON = 0x2;
		internal const uint NIF_TIP = 0x4;
		internal const uint NIF_STATE = 0x8;
		internal const uint NIF_INFO = 0x10;
		internal const uint NIF_GUID = 0x20;
		internal const uint NIF_REALTIME = 0x40;
		internal const uint NIF_SHOWTIP = 0x80;
		internal const uint NIS_HIDDEN = 0x1;
		internal const uint NIS_SHAREDICON = 0x2;
		internal const uint NIIF_NONE = 0x0;
		internal const uint NIIF_INFO = 0x1;
		internal const uint NIIF_WARNING = 0x2;
		internal const uint NIIF_ERROR = 0x3;
		internal const uint NIIF_USER = 0x4;
		internal const uint NIIF_ICON_MASK = 0xF;
		internal const uint NIIF_NOSOUND = 0x10;
		internal const uint NIIF_LARGE_ICON = 0x20;
		internal const uint NIIF_RESPECT_QUIET_TIME = 0x80;

		[DllImport("shell32.dll", EntryPoint = "Shell_NotifyIconW")]
		internal static extern bool Shell_NotifyIcon(uint dwMessage, ref NOTIFYICONDATA lpData);

		//internal struct SHSTOCKICONINFO
		//{
		//	public uint cbSize;
		//	public IntPtr hIcon;
		//	public int iSysImageIndex;
		//	public int iIcon;
		//	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		//	public string szPath;
		//}

		internal struct SHSTOCKICONINFO
		{
			public uint cbSize;
			public IntPtr hIcon;
			public int iSysImageIndex;
			public int iIcon;
			public fixed char szPath[260];
		}

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHGetStockIconInfo(Native.SHSTOCKICONID siid, uint uFlags, ref SHSTOCKICONINFO psii);

		[DllImport("shell32.dll", EntryPoint = "#6", PreserveSig = true)]
		internal static extern int SHDefExtractIcon(string pszIconFile, int iIndex, uint uFlags, IntPtr* phiconLarge, IntPtr* phiconSmall, int nIconSize);

		internal const int SHIL_LARGE = 0;
		internal const int SHIL_SMALL = 1;
		internal const int SHIL_EXTRALARGE = 2;
		internal const int SHIL_SYSSMALL = 3;
		internal const int SHIL_JUMBO = 4;

		//[DllImport("shell32.dll", EntryPoint = "#727", PreserveSig = true)]
		//internal static extern int SHGetImageList(int iImageList, ref Guid riid, out IImageList ppvObj);
		[DllImport("shell32.dll", EntryPoint = "#727", PreserveSig = true)]
		internal static extern int SHGetImageList(int iImageList, ref Guid riid, out IntPtr ppvObj);

		internal const uint SHCNE_RENAMEITEM = 0x1;
		internal const uint SHCNE_CREATE = 0x2;
		internal const uint SHCNE_DELETE = 0x4;
		internal const uint SHCNE_MKDIR = 0x8;
		internal const uint SHCNE_RMDIR = 0x10;
		internal const uint SHCNE_MEDIAINSERTED = 0x20;
		internal const uint SHCNE_MEDIAREMOVED = 0x40;
		internal const uint SHCNE_DRIVEREMOVED = 0x80;
		internal const uint SHCNE_DRIVEADD = 0x100;
		internal const uint SHCNE_NETSHARE = 0x200;
		internal const uint SHCNE_NETUNSHARE = 0x400;
		internal const uint SHCNE_ATTRIBUTES = 0x800;
		internal const uint SHCNE_UPDATEDIR = 0x1000;
		internal const uint SHCNE_UPDATEITEM = 0x2000;
		internal const uint SHCNE_SERVERDISCONNECT = 0x4000;
		internal const uint SHCNE_UPDATEIMAGE = 0x8000;
		internal const uint SHCNE_DRIVEADDGUI = 0x10000;
		internal const uint SHCNE_RENAMEFOLDER = 0x20000;
		internal const uint SHCNE_FREESPACE = 0x40000;
		internal const uint SHCNE_EXTENDED_EVENT = 0x4000000;
		internal const uint SHCNE_ASSOCCHANGED = 0x8000000;
		internal const uint SHCNE_DISKEVENTS = 0x2381F;
		internal const uint SHCNE_GLOBALEVENTS = 0xC0581E0;
		internal const uint SHCNE_ALLEVENTS = 0x7FFFFFFF;
		internal const uint SHCNE_INTERRUPT = 0x80000000;

		internal const uint SHCNF_IDLIST = 0x0;
		internal const uint SHCNF_DWORD = 0x3;
		internal const uint SHCNF_PATH = 0x5;
		internal const uint SHCNF_PRINTER = 0x6;
		internal const uint SHCNF_FLUSH = 0x1000;
		internal const uint SHCNF_FLUSHNOWAIT = 0x3000;
		internal const uint SHCNF_NOTIFYRECURSIVE = 0x10000;

		[DllImport("shell32.dll")]
		internal static extern void SHChangeNotify(uint wEventId, uint uFlags, string dwItem1, string dwItem2);

		internal const uint SEE_MASK_CONNECTNETDRV = 0x80;
		internal const uint SEE_MASK_NOZONECHECKS = 0x800000;
		internal const uint SEE_MASK_UNICODE = 0x4000;
		internal const uint SEE_MASK_FLAG_NO_UI = 0x400;
		internal const uint SEE_MASK_INVOKEIDLIST = 0xC;
		internal const uint SEE_MASK_NOCLOSEPROCESS = 0x40;
		internal const uint SEE_MASK_NOASYNC = 0x100;
		internal const uint SEE_MASK_NO_CONSOLE = 0x8000;
		internal const uint SEE_MASK_HMONITOR = 0x200000;
		internal const uint SEE_MASK_WAITFORINPUTIDLE = 0x2000000;

		internal struct SHELLEXECUTEINFO
		{
			public uint cbSize;
			public uint fMask;
			public Wnd hwnd;
			public string lpVerb;
			public string lpFile;
			public string lpParameters;
			public string lpDirectory;
			public int nShow;
			public IntPtr hInstApp;
			public IntPtr lpIDList;
			public string lpClass;
			public IntPtr hkeyClass;
			public uint dwHotKey;
			public IntPtr hMonitor;
			public IntPtr hProcess;
		}

		[DllImport("shell32.dll", EntryPoint = "ShellExecuteExW", SetLastError = true)]
		internal static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO pExecInfo);

		[DllImport("shell32.dll", PreserveSig = true)]
		internal static extern int SHOpenFolderAndSelectItems(HandleRef pidlFolder, uint cidl, IntPtr[] apidl, uint dwFlags);

		[DllImport("shell32.dll")]
		internal static extern int ILGetSize(IntPtr pidl);

		internal const uint FO_MOVE = 0x1;
		internal const uint FO_COPY = 0x2;
		internal const uint FO_DELETE = 0x3;
		internal const uint FO_RENAME = 0x4;

		internal const uint FOF_MULTIDESTFILES = 0x1;
		internal const uint FOF_CONFIRMMOUSE = 0x2;
		internal const uint FOF_SILENT = 0x4;
		internal const uint FOF_RENAMEONCOLLISION = 0x8;
		internal const uint FOF_NOCONFIRMATION = 0x10;
		internal const uint FOF_WANTMAPPINGHANDLE = 0x20;
		internal const uint FOF_ALLOWUNDO = 0x40;
		internal const uint FOF_FILESONLY = 0x80;
		internal const uint FOF_SIMPLEPROGRESS = 0x100;
		internal const uint FOF_NOCONFIRMMKDIR = 0x200;
		internal const uint FOF_NOERRORUI = 0x400;
		internal const uint FOF_NOCOPYSECURITYATTRIBS = 0x800;
		internal const uint FOF_NORECURSION = 0x1000;
		internal const uint FOF_NO_CONNECTED_ELEMENTS = 0x2000;
		internal const uint FOF_WANTNUKEWARNING = 0x4000;
		internal const uint FOF_NORECURSEREPARSE = 0x8000;
		internal const uint FOF_NO_UI = 0x614;

		internal struct SHFILEOPSTRUCT
		{
			public Wnd hwnd;
			public uint wFunc;
			public string pFrom;
			public string pTo;
			public ushort fFlags;

			//workaround for this problem: the 32-bit version of SHFILEOPSTRUCT uses Pack = 1, ie no 2-byte gap after fFlags.
			//	I don't want to use two versions. Then also would need two versions of code that use this struct.
			//	The last two members are not useful, but we need fAnyOperationsAborted. This workaround gets it through a property function.
			//	Update: we don't need fAnyOperationsAborted. We use FOF_SILENT therefore cannot be aborted. And it is unreliable.
			//		But the workaround is tested, on both platformas.

#if use_fAnyOperationsAborted
				//public bool fAnyOperationsAborted; //BOOL
				ushort _fAnyOperationsAborted_32, _fAnyOperationsAborted_common, _fAnyOperationsAborted_64;
				public bool fAnyOperationsAborted
				{
					get
					{
						if(_fAnyOperationsAborted_common != 0) return true;
						var v = Ver.Is64BitProcess ? _fAnyOperationsAborted_64 : _fAnyOperationsAborted_32;
						return v != 0;
					}
				}
#else
			private int fAnyOperationsAborted;
#endif
			private IntPtr hNameMappings;
			private string lpszProgressTitle;
			//these are private and not used, because would be at invalid offsets on 32-bit
		}

		//internal struct SHFILEOPSTRUCT
		//{
		//	public Wnd hwnd;
		//	public uint wFunc;
		//	public string pFrom;
		//	public string pTo;
		//	public ushort fFlags;
		//	public bool fAnyOperationsAborted;
		//	public IntPtr hNameMappings;
		//	public string lpszProgressTitle;
		//}

		//[StructLayout(LayoutKind.Sequential, Pack = 1)]
		//internal struct SHFILEOPSTRUCT__32
		//{
		//	public Wnd hwnd;
		//	public uint wFunc;
		//	public string pFrom;
		//	public string pTo;
		//	public ushort fFlags;
		//	public bool fAnyOperationsAborted;
		//	public IntPtr hNameMappings;
		//	public string lpszProgressTitle;
		//}

		[DllImport("shell32.dll", EntryPoint = "SHFileOperationW")]
		internal static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);

		[DllImport("shell32.dll", EntryPoint = "DragQueryFileW")]
		internal static extern int DragQueryFile(IntPtr hDrop, int iFile, char* lpszFile, uint cch);



		#endregion

		#region shlwapi

		[DllImport("shlwapi.dll", EntryPoint = "#176", PreserveSig = true)]
		internal static extern int IUnknown_QueryService(IntPtr punk, ref Guid guidService, ref Guid riid, out IntPtr ppvOut);
		//internal static extern int IUnknown_QueryService(IntPtr punk, ref Guid guidService, ref Guid riid, void* ppvOut);
		//internal static extern int IUnknown_QueryService([MarshalAs(UnmanagedType.IUnknown)] object punk, ref Guid guidService, ref Guid riid, out IntPtr ppvOut);

		[DllImport("shlwapi.dll", EntryPoint = "PathIsDirectoryEmptyW")]
		internal static extern bool PathIsDirectoryEmpty(string pszPath);
		//speed: slightly faster than with EnumDirectory.

		[DllImport("shlwapi.dll")]
		internal static extern uint ColorAdjustLuma(uint clrRGB, int n, bool fScale);

		//internal enum ASSOCSTR
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
		//internal static extern int AssocQueryString(uint flags, ASSOCSTR str, string pszAssoc, string pszExtra, char* pszOut, ref int pcchOut);




		#endregion

		#region comctl32

		[DllImport("comctl32.dll")]
		internal static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);

		[DllImport("comctl32.dll")]
		internal static extern bool ImageList_GetIconSize(IntPtr himl, out int cx, out int cy);




		#endregion

		#region oleaut32

		[DllImport("oleaut32.dll", EntryPoint = "#6")]
		internal static extern void SysFreeString(char* bstrString);

		[DllImport("oleaut32.dll", EntryPoint = "#7")]
		internal static extern int SysStringLen(char* pbstr);

		[DllImport("oleaut32.dll", EntryPoint = "#4")]
		internal static extern BSTR SysAllocStringLen(string strIn, int len);

		[DllImport("oleaut32.dll", EntryPoint = "#2")]
		internal static extern BSTR SysAllocString(char* psz);

		[DllImport("oleaut32.dll", EntryPoint = "#147", PreserveSig = true)]
		internal static extern int VariantChangeTypeEx(ref VARIANT pvargDest, ref VARIANT pvarSrc, uint lcid, ushort wFlags, VARENUM vt);

		[DllImport("oleaut32.dll", EntryPoint = "#9", PreserveSig = true)]
		internal static extern int VariantClear(ref VARIANT pvarg);




		#endregion

		#region ole32

		[DllImport("ole32.dll", PreserveSig = true)]
		internal static extern int PropVariantClear(ref PROPVARIANT pvar);

		internal enum APTTYPE
		{
			APTTYPE_CURRENT = -1,
			APTTYPE_STA,
			APTTYPE_MTA,
			APTTYPE_NA,
			APTTYPE_MAINSTA
		}

		[DllImport("ole32.dll", PreserveSig = true)]
		internal static extern int CoGetApartmentType(out APTTYPE pAptType, out int pAptQualifier);

		[DllImport("ole32.dll", PreserveSig = true)]
		internal static extern int OleInitialize(IntPtr pvReserved);




		#endregion

		#region oleacc

		//internal static Guid IID_IAccessible = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

		//internal static Guid IID_IAccessible2 = new Guid(0xE89F726E, 0xC4F4, 0x4c19, 0xBB, 0x19, 0xB6, 0x47, 0xD7, 0xFA, 0x84, 0x78);

		//[DllImport("oleacc.dll", PreserveSig = true)]
		//internal static extern int AccessibleObjectFromWindow(Wnd hwnd, AccOBJID dwId, ref Guid riid, out IntPtr ppvObject);

		//[DllImport("oleacc.dll", PreserveSig = true)]
		//internal static extern int WindowFromAccessibleObject(IntPtr iacc, out Wnd phwnd);

		//[DllImport("oleacc.dll", PreserveSig = true)]
		//internal static extern int AccessibleObjectFromPoint(Point ptScreen, out IntPtr ppacc, out VARIANT pvarChild);

		[DllImport("oleacc.dll", PreserveSig = true)]
		internal static extern int AccessibleObjectFromEvent(Wnd hwnd, int dwId, int dwChildId, out IntPtr ppacc, out VARIANT pvarChild);

		[DllImport("oleacc.dll")]
		internal static extern IntPtr GetProcessHandleFromHwnd(Wnd hwnd);

		internal delegate void WINEVENTPROC(IntPtr hWinEventHook, AccEVENT event_, Wnd hwnd, int idObject, int idChild, int idEventThread, int dwmsEventTime);

		[DllImport("user32.dll")]
		internal static extern IntPtr SetWinEventHook(AccEVENT eventMin, AccEVENT eventMax, IntPtr hmodWinEventProc, WINEVENTPROC pfnWinEventProc, int idProcess, int idThread, LibAccHookFlags dwFlags);

		[DllImport("user32.dll")]
		internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);




		#endregion

		#region msvcrt

		//don't use, because for eg "0xffffffff" returns int.Max instead of -1.
		//[DllImport("msvcrt.dll", EntryPoint = "wcstol", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int strtoi(char* s, char** endPtr = null, int radix = 0);

		//don't use the u API because they return 1 if the value is too big and the string contains '-'.
		//[DllImport("msvcrt.dll", EntryPoint = "wcstoul", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern uint strtoui(char* s, char** endPtr = null, int radix = 0);

		[DllImport("msvcrt.dll", EntryPoint = "_wcstoi64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern long strtoi64(char* s, char** endPtr = null, int radix = 0);
		//info: ntdll also has wcstol, wcstoul, _wcstoui64, but not _wcstoi64.

		//[DllImport("msvcrt.dll", EntryPoint = "_wcstoui64", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern ulong strtoui64(char* s, char** endPtr = null, int radix = 0);

		[DllImport("msvcrt.dll", EntryPoint = "_strtoi64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern long strtoi64(byte* s, byte** endPtr = null, int radix = 0);

		//not used
		//[DllImport("msvcrt.dll", EntryPoint = "_strtoui64", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern long strtoui64(byte* s, byte** endPtr = null, int radix = 0);

		//This is used when working with char*. With C# strings use String_.ToInt32 etc.
		internal static int strtoi(char* s, char** endPtr = null, int radix = 0)
		{
			return (int)strtoi64(s, endPtr, radix);
		}

		//This is used with UTF-8 text.
		internal static int strtoi(byte* s, byte** endPtr = null, int radix = 0)
		{
			return (int)strtoi64(s, endPtr, radix);
		}

#if false //not used, because we have String_.ToInt32 etc, which has no overflow problems. But it supports only decimal and hex, not any radix.
		/// <summary>
		/// Converts part of string to int.
		/// Returns the int value.
		/// Returns 0 if the string is null, "" or does not begin with a number; then numberEndIndex will be = startIndex.
		/// </summary>
		/// <param name="s">String.</param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends.</param>
		/// <param name="radix">If 0, parses the string as hexadecimal number if begins with "0x", as octal if begins with "0", else as decimal. Else it can be 2 to 36. Examples: 10 - parse as decimal (don't support "0x" etc); 16 - as hexadecimal (eg returns 26 if string is "1A" or "0x1A"); 2 - as binary (eg returns 5 if string is "101").</param>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is invalid.</exception>
		internal static int strtoi(string s, int startIndex, out int numberEndIndex, int radix = 0)
		{
			int R = 0, len = s == null ? 0 : s.Length - startIndex;
			if(len < 0) throw new ArgumentOutOfRangeException("startIndex");
			if(len != 0)
				fixed (char* p = s) {
					char* t = p + startIndex, e = t;
					R = strtoi(t, &e, radix);
					len = (int)(e - t);
				}
			numberEndIndex = startIndex + len;
			return R;
		}

		/// <summary>
		/// Converts part of string to long.
		/// Returns the long value.
		/// Returns 0 if the string is null, "" or does not begin with a number; then numberEndIndex will be = startIndex.
		/// </summary>
		/// <param name="s">String.</param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends.</param>
		/// <param name="radix">If 0, parses the string as hexadecimal number if begins with "0x", as octal if begins with "0", else as decimal. Else it can be 2 to 36. Examples: 10 - parse as decimal (don't support "0x" etc); 16 - as hexadecimal (eg returns 26 if string is "1A" or "0x1A"); 2 - as binary (eg returns 5 if string is "101").</param>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is invalid.</exception>
		internal static long strtoi64(string s, int startIndex, out int numberEndIndex, int radix = 0)
		{
			long R = 0;
			int len = s == null ? 0 : s.Length - startIndex;
			if(len < 0) throw new ArgumentOutOfRangeException("startIndex");
			if(len != 0)
				fixed (char* p = s) {
					char* t = p + startIndex, e = t;
					R = strtoi64(t, &e, radix);
					len = (int)(e - t);
				}
			numberEndIndex = startIndex + len;
			return R;
		}

		internal static int strtoi(string s, int startIndex = 0, int radix = 0)
		{
			return strtoi(s, startIndex, out _, radix);
		}

		internal static long strtoi64(string s, int startIndex = 0, int radix = 0)
		{
			return strtoi64(s, startIndex, out _, radix);
		}
#endif

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern char* _ltoa(int value, byte* s, int radix);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void* memcpy(void* to, void* from, LPARAM n);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void* memmove(void* to, void* from, LPARAM n);

		[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void* memset(void* ptr, int ch, LPARAM n);




		#endregion

		#region dwmapi

		[DllImport("dwmapi.dll")]
		internal static extern int DwmGetWindowAttribute(Wnd hwnd, int dwAttribute, out int pvAttribute, int cbAttribute);

		[DllImport("dwmapi.dll")]
		internal static extern int DwmGetWindowAttribute(Wnd hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);




		#endregion

		#region ntdll

		[DllImport("ntdll.dll")]
		internal static extern uint NtQueryTimerResolution(out uint maxi, out uint mini, out uint current);
		//info: NtSetTimerResolution can set min 0.5 ms resolution. timeBeginPeriod min 1.

		internal struct MD5_CTX
		{
			long _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11;
			public long r1, r2;
			//public fixed byte r[16]; //same speed, maybe slightly slower
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
			//public byte[] r; //slow like .NET API
		}

		[DllImport("ntdll.dll")]
		internal static extern void MD5Init(out MD5_CTX context);

		[DllImport("ntdll.dll")]
		internal static extern void MD5Update(ref MD5_CTX context, byte[] data, int dataLen);

		[DllImport("ntdll.dll")]
		internal static extern void MD5Final(ref MD5_CTX context);




		#endregion

		#region other

		[DllImport("uxtheme.dll", PreserveSig = true)]
		internal static extern int SetWindowTheme(Wnd hwnd, string pszSubAppName, string pszSubIdList);


		[DllImport("msi.dll", EntryPoint = "#217")]
		internal static extern int MsiGetShortcutTarget(string szShortcutPath, char* szProductCode, char* szFeatureId, char* szComponentCode);

		[DllImport("msi.dll", EntryPoint = "#173")]
		internal static extern int MsiGetComponentPath(char* szProduct, char* szComponent, [Out] char[] lpPathBuf, ref int pcchBuf);


		[DllImport("winmm.dll")]
		internal static extern uint timeBeginPeriod(uint uPeriod);

		[DllImport("winmm.dll")]
		internal static extern uint timeEndPeriod(uint uPeriod);


		[DllImport("hhctrl.ocx", EntryPoint = "HtmlHelpW")]
		internal static extern Wnd HtmlHelp(Wnd hwndCaller, string pszFile, uint uCommand, LPARAM dwData);



		#endregion

	}
}
