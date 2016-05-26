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
		/// Any file, folder, or a file type like ".txt".
		/// Icon index can be specified: "path,index".
		/// Native icon resource id can be specified: "path,-id".
		/// Icon index or resource id can be specified if the file contains multiple icons, eg an exe or dll file.
		/// </param>
		/// <param name="sizeBig">Icon width and height. Max 256.</param>
		/// <param name="flags"><see cref="IconFlag"/></param>
		public static IntPtr GetIconHandle(string file, int size = 16, IconFlag flags = 0)
		{
			if((uint)size > 256) throw new ArgumentOutOfRangeException("size");
			if(size == 0) size = 16;

			if(!Empty(file)) {
				IntPtr R = _Icon_Get(file, size, flags);
				if(R != Zero) return R;
			}

			if(flags.HasFlag(IconFlag.BlankIfFails)) return _Icon_CreateEmpty(size, size);

			return Zero;
		}

		static IntPtr _Icon_Get(string file, int size, IconFlag flags)
		{
			IntPtr R = Zero, pidl = Zero;
			int index = 0;
			bool getFileTypeIcon = false, noShell = flags.HasFlag(IconFlag.NoShellIcon);

			try {
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
					//if(!file.StartsWith_("<idlist:")) return Zero;
					//int i = file.IndexOf('>'); if(i<9) return Zero;
					////pidl=Folders.VirtualITEMIDLIST.
				} else {
					bool triedToExtract = false;
					if(index == 0 && file.EndsWith_(".ico", true)) {
						triedToExtract = true;
						R = Api.LoadImage(Zero, file, Api.IMAGE_ICON, size, size, Api.LR_LOADFROMFILE); //2 times faster than PrivateExtractIconsW
					} else if(noShell || file.EndsWith_(".exe", true)) {
						triedToExtract = true;
						PrivateExtractIconsW(file, index, size, size, out R, Zero, 1, 0); //slightly faster than iextracticon etc
					}
					if(triedToExtract) {
						if(R != Zero) return R;
						if(flags.HasFlag(IconFlag.FileTypeIconIfFails)) getFileTypeIcon = true; //iextracticon would fail
						else if(noShell) return Zero;
					}
				}

				if(!getFileTypeIcon) {
					//TODO
				}

				gShellIcon:
				uint shFlags = size >= 24 ? Api.SHGFI_LARGEICON : Api.SHGFI_SMALLICON;
				if(getFileTypeIcon) shFlags |= Api.SHGFI_USEFILEATTRIBUTES;
				IntPtr il; var x = new Api.SHFILEINFO();
				if(pidl != Zero) {
					shFlags |= Api.SHGFI_PIDL;
					il = Api.SHGetFileInfo(pidl, 0, ref x, Api.SizeOf(x), shFlags | Api.SHGFI_SYSICONINDEX);
				} else {
					il = Api.SHGetFileInfo(file, 0, ref x, Api.SizeOf(x), shFlags | Api.SHGFI_SYSICONINDEX);
				}
				if(il == Zero) {
					if(flags.HasFlag(IconFlag.FileTypeIconIfFails) && !getFileTypeIcon) { getFileTypeIcon = true; goto gShellIcon; }
					return Zero;
				}

				return Api.ImageList_GetIcon(il, x.iIcon, 0);
				//note: extracting directly (SHGFI_ICON) adds overlays, also is slower

				//problem on Vista/7:
				//If DPI>96, most document icons are distorted, because shell imagelist icons are not of standard size. Eg small icons may be 20x20.
				//Currently we ignore this. Most icons still are not so bad. Anyway, in Explorer they are also distorted, so the user is prepared for it.
				//Could instead try to get icon location from registry, but then problems:
				//	Shell icon of some files depend on file contents or don't know what. Shell icon handlers would not be involved.
				//	Need caching, because if there are thousands of files of same type in a folder...
			} finally { if(pidl != Zero) Marshal.FreeCoTaskMem(pidl); }
		}

		static int _Icon_GetSpec(string file, out string iconFile, out int index, out IntPtr hi, int size, IntPtr pidl)
		{
			iconFile = null; index = 0; hi = Zero;

			return 0;
		}

		public static int GetIconHandle(IntPtr pidl, int size = 16)
		{
			IntPtr R = Zero;


			return 0;
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
			return CreateIcon(Zero, width, height, 1, 1, aAnd, aXor);

			//speed: 5-10 mcs. Faster than CopyImage, CopyIcon, DuplicateIcon.
		}

		[DllImport("user32.dll")]
		static extern IntPtr CreateIcon(IntPtr hInstance, int nWidth, int nHeight, byte cPlanes, byte cBitsPixel, byte[] lpbANDbits, byte[] lpbXORbits);

		[DllImport("user32.dll")]
		static extern uint PrivateExtractIconsW(string szFileName, int nIconIndex, int cxIcon, int cyIcon, out IntPtr phicon, IntPtr piconid, uint nIcons, uint flags);

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


		#endregion
	}
}
