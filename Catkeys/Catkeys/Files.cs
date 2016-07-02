using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
//using System.Reflection;
//using System.Linq;
using System.Runtime.ExceptionServices;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public static class Files
	{
		#region icon, cursor

		[Flags]
		public enum IconFlag
		{
			/// <summary>
			/// The 'file' argument is literal full path. Don't parse "path,index", don't support ".ext" (file type icon), don't make fully-qualified, etc.
			/// </summary>
			LiteralPath = 1,

			/// <summary>
			/// Always extract icon from the file. Don't get shell icon.
			/// Without this flag extracts only from ico and exe files, or if index specified.
			/// Don't use this flag if need a file type icon.
			/// </summary>
			NoShellIcon = 2,

			/// <summary>
			/// If fails to get true icon, get file type icon.
			/// </summary>
			FileTypeIconIfFails = 4,

			/// <summary>
			/// If fails to get true icon, create blank (completely transparent) icon. You can use 'file' argument = null/"" if need just the blank icon.
			/// </summary>
			BlankIfFails = 8,
		}

		/// <summary>
		/// Gets file icon.
		/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, flags etc.
		/// Returns icon handle. Returns Zero if failed, for example when the file does not exist.
		/// Later call Api.DestroyIcon().
		/// </summary>
		/// <param name="file">
		/// Any file, folder, URL like "http://..." or "shell:..." etc, shell item like "::{CLSID}" or "::{CLSID1}\::{CLSID2}". Also can be a file type like ".txt" and a protocol like "http:".
		/// If it is a file containing multiple icons (eg exe, dll), can be specified icon index like "path,index" or native icon resource id like "path,-id".
		/// If not full path, the file must be in program's folder.
		/// </param>
		/// <param name="size">Icon width and height. Max 256.</param>
		/// <param name="flags"><see cref="IconFlag"/></param>
		public static IntPtr GetIconHandle(string file, int size = 16, IconFlag flags = 0)
		{
			if(!Empty(file)) {
				IntPtr R = _Icon_Get(file, Zero, size, flags);
				if(R != Zero) return R;
			}

			if(flags.HasFlag(IconFlag.BlankIfFails)) return _Icon_CreateEmpty(size, size);

			return Zero;
		}

		/// <summary>
		/// Gets icon of a file or other shell object specified by its ITEMIDLIST pointer.
		/// </summary>
		/// <param name="pidl">ITEMIDLIST pointer.</param>
		/// <param name="size">Icon width and height. Max 256.</param>
		/// <param name="flags"><see cref="IconFlag"/>Can be 0 or IconFlag.BlankIfFails.</param>
		public static IntPtr GetIconHandle(IntPtr pidl, int size = 16, IconFlag flags = 0)
		{
			IntPtr R = _Icon_Get(null, pidl, size, flags);
			if(R != Zero) return R;

			if(flags.HasFlag(IconFlag.BlankIfFails)) return _Icon_CreateEmpty(size, size);

			return Zero;
		}

		static IntPtr _Icon_Get(string file, IntPtr pidl, int size, IconFlag flags)
		{
			if((uint)size > 256) throw new ArgumentOutOfRangeException("size");
			if(size == 0) size = 16;

			IntPtr R = Zero;
			int index = 0;
			bool getFileTypeIcon = false, noShell = flags.HasFlag(IconFlag.NoShellIcon);

			if(pidl == Zero) {
				if(!flags.HasFlag(IconFlag.LiteralPath)) {
					if(!noShell && file[0] == '.' && file.IndexOf('.', 1) < 0) {
						//get file type icon with shgetfileinfo
						getFileTypeIcon = true;
						goto gShellIcon;
					}

					//get icon index from "path,index" and remove ",index" from path
					index = _Icon_GetIndex(ref file);
					if(index != 0) noShell = true;

					file = Folders.FullPath(file);
				}

				if(file[0] == '<') {
					if(noShell) return Zero;
					//TODO: either use or remove this code
					if(!file.StartsWith_("<idlist:")) return Zero;
					int i = file.IndexOf('>'); if(i < 9) return Zero;
					//pidl=Folders.VirtualITEMIDLIST...
					R = _Icon_Get(null, pidl, size, flags);
					Marshal.FreeCoTaskMem(pidl);
					return R;
				} else {
					bool triedToExtract = false;
					if(index == 0 && file.EndsWith_(".ico", true)) {
						triedToExtract = true;
						R = Api.LoadImage(Zero, file, Api.IMAGE_ICON, size, size, Api.LR_LOADFROMFILE); //2 times faster than PrivateExtractIcons
					} else if(noShell || file.EndsWith_(".exe", true)) {
						triedToExtract = true;
						Api.PrivateExtractIcons(file, index, size, size, out R, Zero, 1, 0); //slightly faster than iextracticon etc
					}
					if(triedToExtract) {
						if(R != Zero) return R;
						if(flags.HasFlag(IconFlag.FileTypeIconIfFails)) getFileTypeIcon = true; //iextracticon would fail
						else if(noShell) return Zero;
					}
				}
			}

			if(!getFileTypeIcon) {
				R = _Icon_GetSpec(file, pidl, size);
				if(R != Zero) return R;
			}

			gShellIcon:
			uint shFlags = size >= 24 ? Api.SHGFI_LARGEICON | Api.SHGFI_SYSICONINDEX : Api.SHGFI_SMALLICON | Api.SHGFI_SYSICONINDEX;
			if(getFileTypeIcon) shFlags |= Api.SHGFI_USEFILEATTRIBUTES;
			IntPtr il; var x = new Api.SHFILEINFO(); uint siz = Api.SizeOf(x);
			if(pidl != Zero) {
				il = Api.SHGetFileInfo(pidl, 0, ref x, siz, shFlags | Api.SHGFI_PIDL);
			} else {
				il = Api.SHGetFileInfo(file, 0, ref x, siz, shFlags);
				if(il == Zero && flags.HasFlag(IconFlag.FileTypeIconIfFails) && !getFileTypeIcon) { getFileTypeIcon = true; goto gShellIcon; }
			}
			if(il == Zero) return Zero;

			return Api.ImageList_GetIcon(il, x.iIcon, 0);
			//note: extracting directly (SHGFI_ICON) adds overlays, also is slower

			//problem:
			//If DPI>96, most document icons are distorted, because shell imagelist icons are not of standard size. Eg small icons may be 20x20.
			//Currently we ignore this. Most icons still are not so bad. Anyway, in Explorer they are also distorted, so the user is prepared for it.
			//Could instead try to get icon location from registry, but then problems:
			//	Shell icon of some files depend on file contents or don't know what. Shell icon handlers would not be involved.
			//	Need caching, because if there are thousands of files of same type in a folder...
		}

		/// <summary>
		/// Gets shell icon of a file or protocol etc where SHGetFileInfo would fail.
		/// Also can get icons of sizes other than 16 or 32.
		/// Cannot get file extension icons.
		/// If pidl is not Zero, uses it and ignores file, else uses file.
		/// Returns Zero if failed.
		/// </summary>
		[HandleProcessCorruptedStateExceptions]
		static unsafe IntPtr _Icon_GetSpec(string file, IntPtr pidl, int size)
		{
			IntPtr R = Zero;
			bool freePidl = false;
			Api.IShellFolder folder = null;
			Api.IExtractIcon eic = null;
			try { //possible exceptions in shell32.dll or in shell extensions
				if(pidl == Zero) {
					pidl = PidlFromString(file);
					if(pidl == Zero) return Zero;
					freePidl = true;
				}

				IntPtr pidlItem;
				if(0 != Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem)) return Zero;

				object o;
				if(0 != folder.GetUIObjectOf(Wnd0, 1, &pidlItem, Api.IID_IExtractIcon, Zero, out o)) return Zero;
				eic = o as Api.IExtractIcon;

				var sb = new StringBuilder(300); int ii; uint fl;
				if(0 != eic.GetIconLocation(0, sb, 300, out ii, out fl)) return Zero;
				string loc = sb.ToString();

				if((fl & (Api.GIL_NOTFILENAME | Api.GIL_SIMULATEDOC)) != 0 || 1 != Api.PrivateExtractIcons(loc, ii, size, size, out R, Zero, 1, 0)) {
					IntPtr* hiSmall = null, hiBig = null;
					if(size < 24) { hiSmall = &R; size = 32; } else hiBig = &R;
					if(0 != eic.Extract(loc, (uint)ii, hiBig, hiSmall, Calc.MakeUint(size, 16))) return Zero;
				}
			}
			catch { }
			finally {
				if(freePidl) Marshal.FreeCoTaskMem(pidl);
				if(folder != null) Marshal.ReleaseComObject(folder);
				if(eic != null) Marshal.ReleaseComObject(eic);
			}
			return R;
		}

		/// <summary>
		/// If s ends with ",number" or ",-number", removes it and returns the number.
		/// </summary>
		internal static int _Icon_GetIndex(ref string s)
		{
			if(Empty(s)) return 0;
			if(!Calc.IsDigit(s[s.Length - 1])) return 0;
			int i = s.LastIndexOf(','); if(i < 0 || !(Calc.IsDigit(s[i + 1]) || s[i + 1] == '-')) return 0;
			int e, R = s.ToInt_(i + 1, out e);
			if(e != s.Length) return 0;
			s = s.Remove(i);
			return R;

			//note: PathParseIconLocation is buggy, eg from "c:\file.txt, 5" removes ", 5" and returns 0.
		}

		/// <summary>
		/// Creates completely transparent monochrome icon.
		/// </summary>
		internal static IntPtr _Icon_CreateEmpty(int width, int height)
		{
			int nb = Calc.AlignUp(width, 32) / 8 * height;
			var aAnd = new byte[nb]; for(int i = 0; i < nb; i++) aAnd[i] = 0xff;
			var aXor = new byte[nb];
			return Api.CreateIcon(Zero, width, height, 1, 1, aAnd, aXor);

			//speed: 5-10 mcs. Faster than CopyImage, CopyIcon, DuplicateIcon.
		}

		/// <summary>
		/// Loads cursor.
		/// Returns cursor handle. Returns Zero if failed.
		/// Later call Api.DestroyCursor().
		/// </summary>
		/// <param name="file">.cur or .ani file.</param>
		public static IntPtr GetCursorHandle(string file)
		{
			return Api.LoadImage(Zero, Folders.FullPath(file), Api.IMAGE_CURSOR, 32, 32, Api.LR_LOADFROMFILE);
		}

		#endregion

		#region pidl

		/// <summary>
		/// Converts file path or shell object display name to PIDL.
		/// Later call Marshal.FreeCoTaskMem();
		/// </summary>
		public static unsafe IntPtr PidlFromString(string s)
		{
			if(Empty(s)) return Zero;
			IntPtr R;
			if(0 != Api.SHParseDisplayName(s, Zero, out R, 0, null)) return Zero;
			//the same as
			//Api.IShellFolder isf; if(0 != Api.SHGetDesktopFolder(out isf)) return Zero;
			//if(0 != isf.ParseDisplayName(Wnd0, Zero, s, null, out R, null)) return Zero;
			return R;
		}


		#endregion
	}
}
