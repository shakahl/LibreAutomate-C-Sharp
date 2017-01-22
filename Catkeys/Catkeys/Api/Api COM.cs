using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 1591 //missing XML documentation

namespace Catkeys
{
	static unsafe partial class Api
	{
		public struct STRRET
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

		public static Guid IID_IShellFolder = new Guid(0x000214E6, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

		[ComImport, Guid("000214E6-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IShellFolder
		{
			[PreserveSig]
			int ParseDisplayName(Wnd hwnd, IntPtr pbc, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, uint* pchEaten, out IntPtr ppidl, uint* pdwAttributes);
			[PreserveSig]
			int EnumObjects(Wnd hwnd, uint grfFlags, out IEnumIDList ppenumIDList);
			[PreserveSig]
			int BindToObject(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig]
			int BindToStorage(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig]
			int CompareIDs(LPARAM lParam, IntPtr pidl1, IntPtr pidl2);
			[PreserveSig]
			int CreateViewObject(Wnd hwndOwner, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig]
			int GetAttributesOf(uint cidl, [MarshalAs(UnmanagedType.LPArray)] [In] IntPtr[] apidl, ref uint rgfInOut);
			[PreserveSig]
			//int GetUIObjectOf(Wnd hwndOwner, uint cidl, [MarshalAs(UnmanagedType.LPArray)] [In] IntPtr[] apidl, [In] ref Guid riid, IntPtr rgfReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
			int GetUIObjectOf(Wnd hwndOwner, uint cidl, IntPtr* apidl, [In] ref Guid riid, IntPtr rgfReserved, [MarshalAs(UnmanagedType.Interface)] out object ppv);
			[PreserveSig]
			int GetDisplayNameOf(IntPtr pidl, uint uFlags, out STRRET pName);
			[PreserveSig]
			int SetNameOf(Wnd hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string pszName, uint uFlags, out IntPtr ppidlOut);
		}

		[ComImport, Guid("000214F2-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IEnumIDList
		{
			[PreserveSig]
			int Next(uint celt, [MarshalAs(UnmanagedType.LPArray)] [Out] IntPtr[] rgelt, out uint pceltFetched);
			[PreserveSig]
			int Skip(uint celt);
			[PreserveSig]
			int Reset();
			[PreserveSig]
			int Clone(out IEnumIDList ppenum);
		}

		public const uint GIL_OPENICON = 0x1;
		public const uint GIL_FORSHELL = 0x2;
		public const uint GIL_ASYNC = 0x20;
		public const uint GIL_DEFAULTICON = 0x40;
		public const uint GIL_FORSHORTCUT = 0x80;
		public const uint GIL_CHECKSHIELD = 0x200;
		public const uint GIL_SIMULATEDOC = 0x1;
		public const uint GIL_PERINSTANCE = 0x2;
		public const uint GIL_PERCLASS = 0x4;
		public const uint GIL_NOTFILENAME = 0x8;
		public const uint GIL_DONTCACHE = 0x10;
		public const uint GIL_SHIELD = 0x200;
		public const uint GIL_FORCENOSHIELD = 0x400;

		public static Guid IID_IExtractIcon = new Guid(0x000214FA, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

		[ComImport, Guid("000214fa-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IExtractIcon
		{
			[PreserveSig]
			int GetIconLocation(uint uFlags, [Out] StringBuilder pszIconFile, uint cchMax, out int piIndex, out uint pwFlags);
			[PreserveSig]
			int Extract([MarshalAs(UnmanagedType.LPWStr)] string pszFile, uint nIconIndex, IntPtr* phiconLarge, IntPtr* phiconSmall, uint nIconSize);
		}

		[ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IShellLink
		{
			//info:
			//Let .NET throw exceptions for 'set' functions. Else my code would have to throw anyway.
			//Use [PreserveSig] for 'get' functions to avoid exception handling. Eg in my tests for 1 shortcut GetDescription returned E_FAIL.

			[PreserveSig]
			int GetPath([Out] StringBuilder pszFile, int cch, IntPtr pfd=default(IntPtr), uint fFlags=0);
			[PreserveSig]
			int GetIDList(out IntPtr ppidl);
			void SetIDList(IntPtr pidl);
			[PreserveSig]
			int GetDescription([Out] StringBuilder pszName, int cch);
			void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
			[PreserveSig]
			int GetWorkingDirectory([Out] StringBuilder pszDir, int cch);
			void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
			[PreserveSig]
			int GetArguments([Out] StringBuilder pszArgs, int cch);
			void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
			[PreserveSig]
			int GetHotkey(out ushort pwHotkey);
			void SetHotkey(ushort wHotkey);
			[PreserveSig]
			int GetShowCmd(out int piShowCmd);
			void SetShowCmd(int iShowCmd);
			[PreserveSig]
			int GetIconLocation([Out] StringBuilder pszIconPath, int cch, out int piIcon);
			void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
			void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved=0);
			void Resolve(Wnd hwnd, uint fFlags);
			void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
		}

		[ComImport, Guid("00021401-0000-0000-C000-000000000046"), ClassInterface(ClassInterfaceType.None)]
		public class ShellLink { }

		[ComImport, Guid("0000010b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IPersistFile
		{
			// IPersist
			[PreserveSig]
			int GetClassID(out Guid pClassID);
			// IPersistFile
			[PreserveSig]
			int IsDirty();
			[PreserveSig]
			int Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
			[PreserveSig]
			int Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [MarshalAs(UnmanagedType.Bool)] bool fRemember);
			[PreserveSig]
			int SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
			[PreserveSig]
			int GetCurFile(out IntPtr ppszFileName);
		}

		public struct PROPERTYKEY
		{
			public Guid fmtid;
			public uint pid;
		}

		public struct PROPVARIANT_LPARAM
		{
			public ushort vt;
			public ushort wReserved1;
			public ushort wReserved2;
			public ushort wReserved3;
			public LPARAM value;
			public IntPtr _u1;
		}

		public static Guid IID_IPropertyStore = new Guid(0x886D8EEB, 0x8CF2, 0x4446, 0x8D, 0x02, 0xCD, 0xBA, 0x1D, 0xBD, 0xCF, 0x99);

		[ComImport, Guid("886d8eeb-8cf2-4446-8d02-cdba1dbdcf99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IPropertyStore
		{
			[PreserveSig]
			int GetCount(out uint cProps);
			[PreserveSig]
			int GetAt(uint iProp, out PROPERTYKEY pkey);
			[PreserveSig]
			int GetValue([In] ref PROPERTYKEY key, out PROPVARIANT_LPARAM pv);
			[PreserveSig]
			int SetValue([In] ref PROPERTYKEY key, [In] ref PROPVARIANT_LPARAM propvar);
			[PreserveSig]
			int Commit();
		}

		public struct IMAGEINFO
		{
			public IntPtr hbmImage;
			public IntPtr hbmMask;
			public int Unused1;
			public int Unused2;
			public RECT rcImage;
		}

		public static Guid IID_IImageList = new Guid(0x46EB5926, 0x582E, 0x4017, 0x9F, 0xDF, 0xE8, 0x99, 0x8D, 0xAA, 0x09, 0x50);

		[ComImport, Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IImageList
		{
			[PreserveSig]
			int Add(IntPtr hbmImage, IntPtr hbmMask, out int pi);
			[PreserveSig]
			int ReplaceIcon(int i, IntPtr hicon, out int pi);
			[PreserveSig]
			int SetOverlayImage(int iImage, int iOverlay);
			[PreserveSig]
			int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
			[PreserveSig]
			int AddMasked(IntPtr hbmImage, uint crMask, out int pi);
			[PreserveSig]
			int Draw(IntPtr pimldp); //ref IMAGELISTDRAWPARAMS
			[PreserveSig]
			int Remove(int i);
			[PreserveSig]
			int GetIcon(int i, uint flags, out IntPtr picon);
			[PreserveSig]
			int GetImageInfo(int i, out IMAGEINFO pImageInfo);
			[PreserveSig]
			int Copy(int iDst, [MarshalAs(UnmanagedType.IUnknown)] Object punkSrc, int iSrc, uint uFlags);
			[PreserveSig]
			int Merge(int i1, [MarshalAs(UnmanagedType.IUnknown)] Object punk2, int i2, int dx, int dy, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig]
			int Clone([In] ref Guid riid, out IntPtr ppv);
			[PreserveSig]
			int GetImageRect(int i, out RECT prc);
			[PreserveSig]
			int GetIconSize(out int cx, out int cy);
			[PreserveSig]
			int SetIconSize(int cx, int cy);
			[PreserveSig]
			int GetImageCount(out int pi);
			[PreserveSig]
			int SetImageCount(uint uNewCount);
			[PreserveSig]
			int SetBkColor(uint clrBk, out uint pclr);
			[PreserveSig]
			int GetBkColor(out uint pclr);
			[PreserveSig]
			int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
			[PreserveSig]
			int EndDrag();
			[PreserveSig]
			int DragEnter(Wnd hwndLock, int x, int y);
			[PreserveSig]
			int DragLeave(Wnd hwndLock);
			[PreserveSig]
			int DragMove(int x, int y);
			[PreserveSig]
			int SetDragCursorImage([MarshalAs(UnmanagedType.IUnknown)] Object punk, int iDrag, int dxHotspot, int dyHotspot);
			[PreserveSig]
			int DragShowNolock([MarshalAs(UnmanagedType.Bool)] bool fShow);
			[PreserveSig]
			int GetDragImage(out POINT ppt, out POINT pptHotspot, [In] ref Guid riid, out IntPtr ppv);
			[PreserveSig]
			int GetItemFlags(int i, out uint dwFlags);
			[PreserveSig]
			int GetOverlayImage(int iOverlay, out int piIndex);
		}

	}
}
