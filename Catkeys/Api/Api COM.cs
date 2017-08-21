using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Catkeys
{
	static unsafe partial class Api
	{
		internal struct STRRET
		{
			public uint uType;

			[StructLayout(LayoutKind.Explicit)]
			public struct TYPE_1
			{
				[FieldOffset(0)]
				public IntPtr pOleStr;
				[FieldOffset(0)]
				public uint uOffset;
				[FieldOffset(0)]
				public fixed sbyte cStr[260];
			}
			public TYPE_1 _2;
		}

		internal static Guid IID_IShellFolder = new Guid(0x000214E6, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

		[ComImport, Guid("000214E6-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IShellFolder
		{
			[PreserveSig] int ParseDisplayName(Wnd hwnd, IntPtr pbc, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, uint* pchEaten, out IntPtr ppidl, uint* pdwAttributes);
			[PreserveSig] int EnumObjects(Wnd hwnd, uint grfFlags, out IEnumIDList ppenumIDList);
			[PreserveSig] int BindToObject(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig] int BindToStorage(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig] int CompareIDs(LPARAM lParam, IntPtr pidl1, IntPtr pidl2);
			[PreserveSig] int CreateViewObject(Wnd hwndOwner, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig] int GetAttributesOf(uint cidl, [MarshalAs(UnmanagedType.LPArray)] [In] IntPtr[] apidl, ref uint rgfInOut);
			[PreserveSig] //int GetUIObjectOf(Wnd hwndOwner, uint cidl, [MarshalAs(UnmanagedType.LPArray)] [In] IntPtr[] apidl, [In] ref Guid riid, IntPtr rgfReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
			int GetUIObjectOf(Wnd hwndOwner, uint cidl, IntPtr* apidl, [In] ref Guid riid, IntPtr rgfReserved, [MarshalAs(UnmanagedType.Interface)] out object ppv);
			[PreserveSig] int GetDisplayNameOf(IntPtr pidl, uint uFlags, out STRRET pName);
			[PreserveSig] int SetNameOf(Wnd hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string pszName, uint uFlags, out IntPtr ppidlOut);
		}

		internal static Guid IID_IShellItem = new Guid(0x43826D1E, 0xE718, 0x42EE, 0xBC, 0x55, 0xA1, 0xE2, 0x61, 0xC3, 0x7B, 0xFE);

		[ComImport, Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IShellItem
		{
			[PreserveSig] int BindToHandler(IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv); //IBindCtx
			[PreserveSig] int GetParent(out IShellItem ppsi);
			[PreserveSig] int GetDisplayName(Native.SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
			[PreserveSig] int GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
			[PreserveSig] int Compare(IShellItem psi, uint hint, out int piOrder);
		}

		[ComImport, Guid("000214F2-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IEnumIDList
		{
			[PreserveSig] int Next(int celt, [MarshalAs(UnmanagedType.LPArray)] [Out] IntPtr[] rgelt, out int pceltFetched);
			[PreserveSig] int Skip(int celt);
			[PreserveSig] int Reset();
			[PreserveSig] int Clone(out IEnumIDList ppenum);
		}

		//internal const uint GIL_OPENICON = 0x1;
		//internal const uint GIL_FORSHELL = 0x2;
		//internal const uint GIL_ASYNC = 0x20;
		//internal const uint GIL_DEFAULTICON = 0x40;
		//internal const uint GIL_FORSHORTCUT = 0x80;
		//internal const uint GIL_CHECKSHIELD = 0x200;
		//internal const uint GIL_SIMULATEDOC = 0x1;
		//internal const uint GIL_PERINSTANCE = 0x2;
		//internal const uint GIL_PERCLASS = 0x4;
		//internal const uint GIL_NOTFILENAME = 0x8;
		//internal const uint GIL_DONTCACHE = 0x10;
		//internal const uint GIL_SHIELD = 0x200;
		//internal const uint GIL_FORCENOSHIELD = 0x400;

		//internal static Guid IID_IExtractIcon = new Guid(0x000214FA, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

		//[ComImport, Guid("000214fa-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		//internal interface IExtractIcon
		//{
		//	[PreserveSig] int GetIconLocation(uint uFlags, char* pszIconFile, int cchMax, out int piIndex, out uint pwFlags);
		//	[PreserveSig] int Extract([MarshalAs(UnmanagedType.LPWStr)] string pszFile, int nIconIndex, IntPtr* phiconLarge, IntPtr* phiconSmall, int nIconSize);
		//}

		[ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IShellLink
		{
			[PreserveSig] int GetPath(char* pszFile, int cch, IntPtr pfd = default(IntPtr), uint fFlags = 0);
			[PreserveSig] int GetIDList(out IntPtr ppidl);
			[PreserveSig] int SetIDList(IntPtr pidl);
			[PreserveSig] int GetDescription(char* pszName, int cch);
			[PreserveSig] int SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
			[PreserveSig] int GetWorkingDirectory(char* pszDir, int cch);
			[PreserveSig] int SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
			[PreserveSig] int GetArguments(char* pszArgs, int cch);
			[PreserveSig] int SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
			[PreserveSig] int GetHotkey(out ushort pwHotkey);
			[PreserveSig] int SetHotkey(ushort wHotkey);
			[PreserveSig] int GetShowCmd(out int piShowCmd);
			[PreserveSig] int SetShowCmd(int iShowCmd);
			[PreserveSig] int GetIconLocation(char* pszIconPath, int cch, out int piIcon);
			[PreserveSig] int SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
			[PreserveSig] int SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved = 0);
			[PreserveSig] int Resolve(Wnd hwnd, uint fFlags);
			[PreserveSig] int SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
		}

		[ComImport, Guid("00021401-0000-0000-C000-000000000046"), ClassInterface(ClassInterfaceType.None)]
		internal class ShellLink { }

		[ComImport, Guid("0000010b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IPersistFile
		{
			// IPersist
			[PreserveSig] int GetClassID(out Guid pClassID);
			// IPersistFile
			[PreserveSig] int IsDirty();
			[PreserveSig] int Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
			[PreserveSig] int Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);
			[PreserveSig] int SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
			[PreserveSig] int GetCurFile(out IntPtr ppszFileName);
		}

		//see also VARIANT in Struct.cs
		internal struct PROPVARIANT :IDisposable
		{
			public VARENUM vt; //ushort
			public ushort _u1;
			public uint _u2;
			public LPARAM value;
			public LPARAM value2;

			/// <summary>
			/// Calls PropVariantClear.
			/// </summary>
			public void Dispose()
			{
				PropVariantClear(ref this);
			}
		}

		internal struct PROPERTYKEY
		{
			public Guid fmtid;
			public uint pid;
		}

		internal static Guid IID_IPropertyStore = new Guid(0x886D8EEB, 0x8CF2, 0x4446, 0x8D, 0x02, 0xCD, 0xBA, 0x1D, 0xBD, 0xCF, 0x99);

		[ComImport, Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IPropertyStore
		{
			[PreserveSig] int GetCount(out int cProps);
			[PreserveSig] int GetAt(int iProp, out PROPERTYKEY pkey);
			[PreserveSig] int GetValue([In] ref PROPERTYKEY key, out PROPVARIANT pv);
			[PreserveSig] int SetValue([In] ref PROPERTYKEY key, [In] ref PROPVARIANT propvar);
			[PreserveSig] int Commit();
		}

		//internal struct IMAGEINFO
		//{
		//	public IntPtr hbmImage;
		//	public IntPtr hbmMask;
		//	public int Unused1;
		//	public int Unused2;
		//	public RECT rcImage;
		//}

		//note: this is used in the lib, even if IImageList isn't.
		internal static Guid IID_IImageList = new Guid(0x46EB5926, 0x582E, 0x4017, 0x9F, 0xDF, 0xE8, 0x99, 0x8D, 0xAA, 0x09, 0x50);

		//[ComImport, Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		//internal interface IImageList
		//{
		//	[PreserveSig] int Add(IntPtr hbmImage, IntPtr hbmMask, out int pi);
		//	[PreserveSig] int ReplaceIcon(int i, IntPtr hicon, out int pi);
		//	[PreserveSig] int SetOverlayImage(int iImage, int iOverlay);
		//	[PreserveSig] int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
		//	[PreserveSig] int AddMasked(IntPtr hbmImage, uint crMask, out int pi);
		//	[PreserveSig] int Draw(IntPtr pimldp); //ref IMAGELISTDRAWPARAMS
		//	[PreserveSig] int Remove(int i);
		//	[PreserveSig] int GetIcon(int i, uint flags, out IntPtr picon);
		//	[PreserveSig] int GetImageInfo(int i, out IMAGEINFO pImageInfo);
		//	[PreserveSig] int Copy(int iDst, [MarshalAs(UnmanagedType.IUnknown)] Object punkSrc, int iSrc, uint uFlags);
		//	[PreserveSig] int Merge(int i1, [MarshalAs(UnmanagedType.IUnknown)] Object punk2, int i2, int dx, int dy, [In] ref Guid riid, out IntPtr ppv);
		//	[PreserveSig] int Clone([In] ref Guid riid, out IntPtr ppv);
		//	[PreserveSig] int GetImageRect(int i, out RECT prc);
		//	[PreserveSig] int GetIconSize(out int cx, out int cy);
		//	[PreserveSig] int SetIconSize(int cx, int cy);
		//	[PreserveSig] int GetImageCount(out int pi);
		//	[PreserveSig] int SetImageCount(int uNewCount);
		//	[PreserveSig] int SetBkColor(uint clrBk, out uint pclr);
		//	[PreserveSig] int GetBkColor(out uint pclr);
		//	[PreserveSig] int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
		//	[PreserveSig] int EndDrag();
		//	[PreserveSig] int DragEnter(Wnd hwndLock, int x, int y);
		//	[PreserveSig] int DragLeave(Wnd hwndLock);
		//	[PreserveSig] int DragMove(int x, int y);
		//	[PreserveSig] int SetDragCursorImage([MarshalAs(UnmanagedType.IUnknown)] Object punk, int iDrag, int dxHotspot, int dyHotspot);
		//	[PreserveSig] int DragShowNolock([MarshalAs(UnmanagedType.Bool)] bool fShow);
		//	[PreserveSig] int GetDragImage(out Point ppt, out Point pptHotspot, [In] ref Guid riid, out IntPtr ppv);
		//	[PreserveSig] int GetItemFlags(int i, out uint dwFlags);
		//	[PreserveSig] int GetOverlayImage(int iOverlay, out int piIndex);
		//}

		[ComImport, Guid("00020400-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		internal interface IDispatch
		{
		}

		[ComImport, Guid("618736e0-3c3d-11cf-810c-00aa00389b71"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IAccessible
		{
			// IDispatch
			[PreserveSig] int unused1();
			[PreserveSig] int unused2();
			[PreserveSig] int unused3();
			[PreserveSig] int unused4();
			// IAccessible
			[PreserveSig] int get_accParent(out IDispatch ppdispParent);
			[PreserveSig] int get_accChildCount(out int pcountChildren);
			[PreserveSig] int get_accChild(object varChild, out IDispatch ppdispChild);
			[PreserveSig] int get_accName(object varChild, out string pszName);
			[PreserveSig] int get_accValue(object varChild, out string pszValue);
			[PreserveSig] int get_accDescription(object varChild, out string pszDescription);
			[PreserveSig] int get_accRole(object varChild, out object pvarRole);
			[PreserveSig] int get_accState(object varChild, out object pvarState);
			[PreserveSig] int get_accHelp(object varChild, out string pszHelp);
			[PreserveSig] int get_accHelpTopic(out string pszHelpFile, object varChild, out int pidTopic);
			[PreserveSig] int get_accKeyboardShortcut(object varChild, out string pszKeyboardShortcut);
			[PreserveSig] int get_accFocus(out object pvarChild);
			[PreserveSig] int get_accSelection(out object pvarChildren);
			[PreserveSig] int get_accDefaultAction(object varChild, out string pszDefaultAction);
			[PreserveSig] int accSelect(int flagsSelect, object varChild);
			[PreserveSig] int accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, object varChild);
			//[PreserveSig] int accLocation(out int pxLeft, out int pyTop, out int pcxWidth, out int pcyHeight, [MarshalAs(UnmanagedType.Struct)] int varChild); //error. Tried to avoid int boxing.
			[PreserveSig] int accNavigate(int navDir, object varStart, out object pvarEndUpAt);
			[PreserveSig] int accHitTest(int xLeft, int yTop, out object pvarChild);
			[PreserveSig] int accDoDefaultAction(object varChild);
			[PreserveSig] int put_accName(object varChild, string szName);
			[PreserveSig] int put_accValue(object varChild, string szValue);
		}

		internal static Guid IID_IAccessible = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

		[DllImport("oleacc.dll", PreserveSig = true)]
		internal static extern int AccessibleObjectFromWindow(Wnd hwnd, uint dwId, [In] ref Guid riid, out IAccessible ppvObject);

		[DllImport("oleacc.dll", PreserveSig = true)]
		internal static extern int WindowFromAccessibleObject(IAccessible param1, out Wnd phwnd);

		internal const uint WINEVENT_OUTOFCONTEXT = 0x0;
		internal const uint WINEVENT_SKIPOWNTHREAD = 0x1;
		internal const uint WINEVENT_SKIPOWNPROCESS = 0x2;
		internal const uint WINEVENT_INCONTEXT = 0x4;
		internal const int CHILDID_SELF = 0;
		internal const int INDEXID_OBJECT = 0;
		internal const int INDEXID_CONTAINER = 0;
		internal const uint OBJID_WINDOW = 0x0;
		internal const uint OBJID_SYSMENU = 0xFFFFFFFF;
		internal const uint OBJID_TITLEBAR = 0xFFFFFFFE;
		internal const uint OBJID_MENU = 0xFFFFFFFD;
		internal const uint OBJID_CLIENT = 0xFFFFFFFC;
		internal const uint OBJID_VSCROLL = 0xFFFFFFFB;
		internal const uint OBJID_HSCROLL = 0xFFFFFFFA;
		internal const uint OBJID_SIZEGRIP = 0xFFFFFFF9;
		internal const uint OBJID_CARET = 0xFFFFFFF8;
		internal const uint OBJID_CURSOR = 0xFFFFFFF7;
		internal const uint OBJID_ALERT = 0xFFFFFFF6;
		internal const uint OBJID_SOUND = 0xFFFFFFF5;
		internal const uint OBJID_QUERYCLASSNAMEIDX = 0xFFFFFFF4;
		internal const uint OBJID_NATIVEOM = 0xFFFFFFF0;
		internal const uint EVENT_MIN = 0x1;
		internal const uint EVENT_MAX = 0x7FFFFFFF;
		internal const uint EVENT_SYSTEM_SOUND = 0x1;
		internal const uint EVENT_SYSTEM_ALERT = 0x2;
		internal const uint EVENT_SYSTEM_FOREGROUND = 0x3;
		internal const uint EVENT_SYSTEM_MENUSTART = 0x4;
		internal const uint EVENT_SYSTEM_MENUEND = 0x5;
		internal const uint EVENT_SYSTEM_MENUPOPUPSTART = 0x6;
		internal const uint EVENT_SYSTEM_MENUPOPUPEND = 0x7;
		internal const uint EVENT_SYSTEM_CAPTURESTART = 0x8;
		internal const uint EVENT_SYSTEM_CAPTUREEND = 0x9;
		internal const uint EVENT_SYSTEM_MOVESIZESTART = 0xA;
		internal const uint EVENT_SYSTEM_MOVESIZEEND = 0xB;
		internal const uint EVENT_SYSTEM_CONTEXTHELPSTART = 0xC;
		internal const uint EVENT_SYSTEM_CONTEXTHELPEND = 0xD;
		internal const uint EVENT_SYSTEM_DRAGDROPSTART = 0xE;
		internal const uint EVENT_SYSTEM_DRAGDROPEND = 0xF;
		internal const uint EVENT_SYSTEM_DIALOGSTART = 0x10;
		internal const uint EVENT_SYSTEM_DIALOGEND = 0x11;
		internal const uint EVENT_SYSTEM_SCROLLINGSTART = 0x12;
		internal const uint EVENT_SYSTEM_SCROLLINGEND = 0x13;
		internal const uint EVENT_SYSTEM_SWITCHSTART = 0x14;
		internal const uint EVENT_SYSTEM_SWITCHEND = 0x15;
		internal const uint EVENT_SYSTEM_MINIMIZESTART = 0x16;
		internal const uint EVENT_SYSTEM_MINIMIZEEND = 0x17;
		internal const uint EVENT_SYSTEM_DESKTOPSWITCH = 0x20;
		internal const uint EVENT_SYSTEM_SWITCHER_APPGRABBED = 0x24;
		internal const uint EVENT_SYSTEM_SWITCHER_APPOVERTARGET = 0x25;
		internal const uint EVENT_SYSTEM_SWITCHER_APPDROPPED = 0x26;
		internal const uint EVENT_SYSTEM_SWITCHER_CANCELLED = 0x27;
		internal const uint EVENT_SYSTEM_IME_KEY_NOTIFICATION = 0x29;
		internal const uint EVENT_SYSTEM_END = 0xFF;
		internal const uint EVENT_OEM_DEFINED_START = 0x101;
		internal const uint EVENT_OEM_DEFINED_END = 0x1FF;
		internal const uint EVENT_UIA_EVENTID_START = 0x4E00;
		internal const uint EVENT_UIA_EVENTID_END = 0x4EFF;
		internal const uint EVENT_UIA_PROPID_START = 0x7500;
		internal const uint EVENT_UIA_PROPID_END = 0x75FF;
		internal const uint EVENT_CONSOLE_CARET = 0x4001;
		internal const uint EVENT_CONSOLE_UPDATE_REGION = 0x4002;
		internal const uint EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003;
		internal const uint EVENT_CONSOLE_UPDATE_SCROLL = 0x4004;
		internal const uint EVENT_CONSOLE_LAYOUT = 0x4005;
		internal const uint EVENT_CONSOLE_START_APPLICATION = 0x4006;
		internal const uint EVENT_CONSOLE_END_APPLICATION = 0x4007;
		internal const uint CONSOLE_APPLICATION_16BIT = 0x0;
		internal const uint CONSOLE_CARET_SELECTION = 0x1;
		internal const uint CONSOLE_CARET_VISIBLE = 0x2;
		internal const uint EVENT_CONSOLE_END = 0x40FF;
		internal const uint EVENT_OBJECT_CREATE = 0x8000;
		internal const uint EVENT_OBJECT_DESTROY = 0x8001;
		internal const uint EVENT_OBJECT_SHOW = 0x8002;
		internal const uint EVENT_OBJECT_HIDE = 0x8003;
		internal const uint EVENT_OBJECT_REORDER = 0x8004;
		internal const uint EVENT_OBJECT_FOCUS = 0x8005;
		internal const uint EVENT_OBJECT_SELECTION = 0x8006;
		internal const uint EVENT_OBJECT_SELECTIONADD = 0x8007;
		internal const uint EVENT_OBJECT_SELECTIONREMOVE = 0x8008;
		internal const uint EVENT_OBJECT_SELECTIONWITHIN = 0x8009;
		internal const uint EVENT_OBJECT_STATECHANGE = 0x800A;
		internal const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
		internal const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
		internal const uint EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D;
		internal const uint EVENT_OBJECT_VALUECHANGE = 0x800E;
		internal const uint EVENT_OBJECT_PARENTCHANGE = 0x800F;
		internal const uint EVENT_OBJECT_HELPCHANGE = 0x8010;
		internal const uint EVENT_OBJECT_DEFACTIONCHANGE = 0x8011;
		internal const uint EVENT_OBJECT_ACCELERATORCHANGE = 0x8012;
		internal const uint EVENT_OBJECT_INVOKED = 0x8013;
		internal const uint EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014;
		internal const uint EVENT_OBJECT_CONTENTSCROLLED = 0x8015;
		internal const uint EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016;
		internal const uint EVENT_OBJECT_CLOAKED = 0x8017;
		internal const uint EVENT_OBJECT_UNCLOAKED = 0x8018;
		internal const uint EVENT_OBJECT_LIVEREGIONCHANGED = 0x8019;
		internal const uint EVENT_OBJECT_HOSTEDOBJECTSINVALIDATED = 0x8020;
		internal const uint EVENT_OBJECT_DRAGSTART = 0x8021;
		internal const uint EVENT_OBJECT_DRAGCANCEL = 0x8022;
		internal const uint EVENT_OBJECT_DRAGCOMPLETE = 0x8023;
		internal const uint EVENT_OBJECT_DRAGENTER = 0x8024;
		internal const uint EVENT_OBJECT_DRAGLEAVE = 0x8025;
		internal const uint EVENT_OBJECT_DRAGDROPPED = 0x8026;
		internal const uint EVENT_OBJECT_IME_SHOW = 0x8027;
		internal const uint EVENT_OBJECT_IME_HIDE = 0x8028;
		internal const uint EVENT_OBJECT_IME_CHANGE = 0x8029;
		internal const uint EVENT_OBJECT_TEXTEDIT_CONVERSIONTARGETCHANGED = 0x8030;
		internal const uint EVENT_OBJECT_END = 0x80FF;
		internal const uint EVENT_AIA_START = 0xA000;
		internal const uint EVENT_AIA_END = 0xAFFF;
	}
}
