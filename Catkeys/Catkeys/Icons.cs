using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
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
	public static class Icons
	{
		[Flags]
		public enum IconFlag
		{
			/// <summary>
			/// The 'file' argument is literal full path. Don't parse "path,index", don't support ".ext" (file type icon), don't make fully-qualified, etc.
			/// </summary>
			LiteralPath = 1,

			/// <summary>
			/// If file is not full path, call SearchPath(). Without this flag searches only in Folders.App.
			/// </summary>
			SearchPath = 2,

			/// <summary>
			/// If file does not exist or fails to get its icon, get common icon for that file type, or default document icon if cannot get common icon.
			/// </summary>
			//DefaultIfFails = 4, //rejected. Now for exe/ico/etc is like with shell API: if file exists, gets default icon (exe or document), else returns Zero.

			/// <summary>
			/// Used only with AsyncIcons class. If the thread pool has spare time, let it convert icon handle to Image object. The callback will receive either handle or Image, it must check both for Zero and null. This is to make whole process as fast as possible.
			/// </summary>
			//NeedImage = 128, //rejected because with our menu/toolbar almost always makes slower

			/// Use shell API for all file types, including exe and ico.
			//Shell=8, //rejected because SHGetFileInfo does not get exe icon with shield overlay
		}

		/// <summary>
		/// Gets file icon.
		/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, flags etc.
		/// Calls GetIconHandle() and converts to Image. Returns null if failed, for example if the file does not exist.
		/// Later call Dispose().
		/// </summary>
		/// <param name="file">
		/// Any file, folder, URL like "http://..." or "shell:..." etc, shell item like "::{CLSID}" or "::{CLSID1}\::{CLSID2}". Also can be a file type like ".txt" or a protocol like "http:".
		/// If it is a file containing multiple icons (eg exe, dll), can be specified icon index like "path,index" or native icon resource id like "path,-id".
		/// If not full path, the file must be in program's folder.
		/// </param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="flags"><see cref="IconFlag"/></param>
		public static Image GetIconImage(string file, int size, IconFlag flags = 0)
		{
			return HandleToImage(GetIconHandle(file, size, flags));
		}

		/// <summary>
		/// Gets icon of a file or other shell object specified by its ITEMIDLIST pointer.
		/// Calls GetIconHandle() and converts to Image. Returns null if failed.
		/// Later call Dispose().
		/// </summary>
		/// <param name="pidl">ITEMIDLIST pointer.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="flags">Can be IconFlag.BlankIfFails. See <see cref="IconFlag"/>.</param>
		public static Image GetIconImage(IntPtr pidl, int size, IconFlag flags = 0)
		{
			return HandleToImage(GetIconHandle(pidl, size, flags));
		}

		/// <summary>
		/// Converts unmanaged icon to Image object and destroys the unmanaged icon.
		/// Returns null if hi is Zero or fails to convert.
		/// </summary>
		/// <param name="hi">Icon handle.</param>
		public static Image HandleToImage(IntPtr hi)
		{
			if(hi == Zero) return null;
			//var perf = new Perf.Inst(true);
			Icon ic = Icon.FromHandle(hi);
			Image im = null;
			try { im = ic.ToBitmap(); } catch(Exception e) { OutDebug(e.Message); }
			ic.Dispose();
			Api.DestroyIcon(hi);
			//perf.NW();
			return im;
		}

		/// <summary>
		/// Gets file icon.
		/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, flags etc.
		/// Returns icon handle. Returns Zero if failed, for example if the file does not exist.
		/// Later call Api.DestroyIcon().
		/// </summary>
		/// <param name="file">
		/// Any file, folder, URL like "http://..." or "shell:..." etc, shell item like "::{CLSID}" or "::{CLSID1}\::{CLSID2}". Also can be a file type like ".txt" or a protocol like "http:".
		/// If it is a file containing multiple icons (eg exe, dll), can be specified icon index like "path,index" or native icon resource id like "path,-id".
		/// If not full path, the file must be in program's folder.
		/// </param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="flags"><see cref="IconFlag"/></param>
		public static IntPtr GetIconHandle(string file, int size, IconFlag flags = 0)
		{
			if(Empty(file)) return Zero;
			size = _NormalizeIconSizeParameter(size);
			file = Path_.ExpandEnvVar(file);

			//if(file[0] == '<') {
			//	//TODO: either use or remove this code
			//	if(!file.StartsWith_("<idlist:")) return Zero;
			//	int i = file.IndexOf('>'); if(i < 9) return Zero;
			//	IntPtr pidl=Folders.VirtualITEMIDLIST...
			//	return _GetShellIcon(pidl, size, true);
			//}

			//var perf = new Perf.Inst(true);
			IntPtr R = _GetFileIcon(file, size, flags);
			//perf.Next(); OutList(perf.Times, file);
			//Out($"<><c 0xff0000>{file}</c>");
			return R;
		}

		/// <summary>
		/// Gets icon of a file or other shell object specified by its ITEMIDLIST pointer.
		/// Returns icon handle. Returns Zero if failed.
		/// Later call Api.DestroyIcon().
		/// </summary>
		/// <param name="pidl">ITEMIDLIST pointer.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="flags">Can be IconFlag.BlankIfFails. See <see cref="IconFlag"/>.</param>
		public static IntPtr GetIconHandle(IntPtr pidl, int size, IconFlag flags = 0)
		{
			if(pidl == Zero) return Zero;
			size = _NormalizeIconSizeParameter(size);
			return _GetShellIcon(true, null, pidl, size, false);
		}

		internal static IntPtr _GetFileIcon(string file, int size, IconFlag flags)
		{
			IntPtr R = Zero, pidl = Zero;
			int index = 0;
			bool extractFromFile = false, isFileType = false, isURL = false, isCLSID = false, isPath = true;
			//bool getDefaultIfFails = flags.HasFlag(IconFlag.DefaultIfFails);

			bool searchPath = flags.HasFlag(IconFlag.SearchPath);

			if(!flags.HasFlag(IconFlag.LiteralPath)) {
				//is ".ext" or "protocol:"?
				isFileType = Files.Misc.PathIsExtension(file) || (isURL = Files.Misc.PathIsProtocol(file));
				if(!isFileType) isURL = Path_.IsURL(file);
				if(isFileType || isURL || (isCLSID = (file[0] == ':'))) isPath = false;
				if(isPath) {
					//get icon index from "path,index" and remove ",index"
					extractFromFile = ParseIconLocation(ref file, out index);

					if(!searchPath) file = Path_.MakeFullPath(file);
				}
			}

			if(searchPath && isPath) {
				file = Files.SearchPath(file);
				if(file == null) return Zero; //ignore getDefaultIfFails
			}

			if(isPath /*&& (extractFromFile || !flags.HasFlag(IconFlag.Shell))*/) {
				int ext = 0;
				if(!extractFromFile && file.Length > 4) ext = file.EndsWith_(true, ".exe", ".scr", ".ico", ".cur", ".ani");
				if(extractFromFile || ext > 0) {
					R = GetIconHandleRaw(file, index, size);
					if(R != Zero || extractFromFile) return R;
					switch(Files.FileOrDirectoryExists(file)) {
					case 0:
						return Zero;
					//if(!getDefaultIfFails) return Zero;
					//goto case 1;
					case 1:
						var siid = Api.SHSTOCKICONID.SIID_DOCNOASSOC;
						if(ext >= 1 && ext <= 2) siid = Api.SHSTOCKICONID.SIID_APPLICATION;
						return GetShellStockIconHandle(siid, size);
						//case 2: //folder name ends with .ico etc
					}
				} else if(file.EndsWith_(".lnk", true)) {
					R = _GetLnkIcon(file, size);
					if(R != Zero) return R;
					//OutList("_GetLnkIcon failed", file);
				}

				//note: here we don't cache icons.
				//Fast enough for where we use this. OS file buffers remain in memory for some time.
				//Where need, should instead use imagelists or some external cache that saves eg full toolbar bitmap.
				//SHGetFileInfo has its own cache. In some cases it makes faster (except first time in process), but using it to get all icons is much slower.
			}

			bool isExt = isFileType && !isURL;

			//Can use this code to avoid slow shell API if possible.
			//In some test cases can make ~2 times faster (with thread pool), especially in MTA thread.
			//But now, after other optimizations applied, in real life makes faster just 10-20%.
#if false
			//if(!flags.HasFlag(IconFlag.Shell)){
			string progId = isCLSID ? null : Files.Misc.GetFileTypeOrProtocolRegistryKey(file, isFileType, isURL);

			RegistryKey rk = (progId == null) ? null : Registry_.Open(progId, Registry.ClassesRoot);
			//OutList(file, progId, isFileType, isURL, rk != null);

			if(rk == null) {
				//Unregistered file type/protocol, no extension, folder, ::{CLSID}, shell:WinStoreApp, or no progId key in HKCR
				//OutList(@"unregistered", file, progId);
				if(progId != null) goto gr; //the file type is known, but no progid key in HKCR. Let shell API figure out. Rare.
				if(isExt || (isPath && Files.FileExists(file))) return GetShellStockIconHandle(Api.SHSTOCKICONID.SIID_DOCNOASSOC, size);
				goto gr;
			}

			//Registered file type/protocol.
			using(rk) {
				if(Registry_.KeyExists(@"ShellEx\IconHandler", rk)) {
					//OutList(@"handler", file);
					goto gr;
				}

				string icon;
				if(Registry_.GetString(out icon, "", @"DefaultIcon", rk) && icon.Length > 0) {
					//OutList("registry: DefaultIcon", file, icon);
					if(icon[0] == '@') icon = null; //eg @{Microsoft.Windows.Photos_16.622.13140.0_x64__8wekyb3d8bbwe?ms-resource://Microsoft.Windows.Photos/Files/Assets/PhotosLogoExtensions.png}
					else ParseIconLocation(ref icon, out index);
				} else if(Registry_.GetString(out icon, "", @"shell\open\command", rk) && icon.Length > 0) {
					//OutList(@"registry: shell\open\command", file, icon);
					var a = icon.Split_((icon[0] == '\"') ? "\"" : " ", StringSplitOptions.RemoveEmptyEntries);
					icon = (a.Length == 0) ? null : a[0];
					if(icon.EndsWith_("rundll32.exe", true)) icon = null;
				} else {
					icon = null;
					//OutList("registry: no", file);
					//Usually shell API somehow gets icon.
					//For example also looks in .ext -> PerceivedType -> HKCR\SystemFileAssociations.
					//We can use AssocQueryString(ASSOCSTR_DEFAULTICON), but it is slow and not always gets correct icon.
				}

				//if(icon != null) OutList(file, icon);

				if(icon == "%1") {
					//Out(file);
					if(isPath) icon = file;
					else icon = null;
				}

				if(icon != null) {
					icon = Path_.ExpandEnvVar(icon);
					if(!Path_.IsFullPath(icon)) icon = Folders.System + icon;
					R = GetIconHandleRaw(icon, index, size);
					if(R != Zero) return R;
				}
			}
			//}
			gr:
#endif
			return _GetShellIcon(!isExt, file, Zero, size);
		}

		//usePidl - if pidl not Zero, use pidl, else convert file to PIDL. If false, pidl must be Zero.
		static IntPtr _GetShellIcon(bool usePidl, string file, IntPtr pidl, int size, bool freePidl = false)
		{
			//info:
			//	We support everything that can have icon - path, URL (including "shell:WinStoreApp"), protocol (eg "http:"), file extension (eg ".txt"), shell item parsing name (eg "::{CLSID}").
			//	We call PidlFromString here and pass it to SHGetFileInfo. It makes faster when using thread pool, because multiple threads can call PidlFromString (slow) simultaneously.
			//	PidlFromString does not support file extension. SHGetFileInfo does not support URL and protocol, unless used PIDL.
			//	SHGetFileInfo gets the most correct icons, but only of standard sizes, which also depends on DPI and don't know what.
			//	IExtractIcon for some types fails or gets wrong icon. Even cannot use it to get correct-size icons, because for most file types it uses system imagelists, which are DPI-dependent.
			//	SHMapPIDLToSystemImageListIndex+SHGetImageList also is not better.

			if(usePidl) {
				if(pidl == Zero) {
					pidl = Files.Misc.PidlFromString(file);
					if(pidl == Zero) usePidl = false; else freePidl = true;
				}
			}

			if(!usePidl) {
				Debug.Assert(pidl == Zero && file != null);
				pidl = Marshal.StringToCoTaskMemUni(file);
				freePidl = true;
			}

			if(pidl == Zero) return Zero;

			//This is faster but fails with some files etc, randomly with others.
			//It means that shell API and/or extensions are not thread-safe, even if can run in MTA.
			//return _GetShellIcon2(pidl, size, usePidl);

			IntPtr R = Zero;
			try {
				if(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
					R = _GetShellIcon2(usePidl, pidl, size);
				} else {
					//tested: switching thread does not make slower. The speed depends mostly on locking, because then thread pool threads must wait.
					using(var work = Util.ThreadPoolSTA.CreateWork(null, o => { R = _GetShellIcon2(usePidl, pidl, size); })) {
						work.Submit();
						work.Wait();
					}
				}
			}
			finally { if(freePidl) Marshal.FreeCoTaskMem(pidl); }
			return R;
		}

		[HandleProcessCorruptedStateExceptions] //shell extensions may throw
		static IntPtr _GetShellIcon2(bool isPidl, IntPtr pidl, int size)
		{
			IntPtr R = Zero, il = Zero; int index = -1, ilIndex, realSize;

			if(size < (realSize = GetShellIconSize(ShellSize.Small)) * 5 / 4) ilIndex = Api.SHIL_SMALL;
			else if(size < (realSize = GetShellIconSize(ShellSize.Large)) * 5 / 4) ilIndex = Api.SHIL_LARGE;
			else if(size < 256) {
				ilIndex = Api.SHIL_EXTRALARGE; realSize = GetShellIconSize(ShellSize.ExtraLarge);
				//info: cannot resize from 256 because GetIcon(SHIL_JUMBO) gives 48 icon if 256 icon unavailable. Getting real icon size is either impossible or quite difficult and slow (not tested).
			} else { ilIndex = Api.SHIL_JUMBO; realSize = 256; }

			//Need to lock this part, or randomly fails with some file types.
			lock ("TK6Z4XiSxkGSfC14/or5Mw") {
				try {
					uint fl = Api.SHGFI_SYSICONINDEX | Api.SHGFI_SHELLICONSIZE;
					if(ilIndex == Api.SHIL_SMALL) fl |= Api.SHGFI_SMALLICON;
					if(isPidl) fl |= Api.SHGFI_PIDL; else fl |= Api.SHGFI_USEFILEATTRIBUTES;
					var x = new Api.SHFILEINFO();
					il = Api.SHGetFileInfo(pidl, 0, ref x, Api.SizeOf(x), fl);
					if(il != Zero) index = x.iIcon;
					//Marshal.Release(il); //undocumented, but without it IImageList refcount grows. Probably it's ok, because it is static, never deleted until process exits.
				}
				catch { OutDebug("exception"); }
			}
			if(index < 0) return Zero;

			//note: Getting icon from imagelist must be in STA thread too, else fails with some file types.
			//tested: This part works without locking. Using another lock here makes slower.

			try {
				if(ilIndex == Api.SHIL_SMALL || ilIndex == Api.SHIL_LARGE || _GetShellImageList(ilIndex, out il)) {
					//OutList(il, Util.Debug_.GetComObjRefCount(il));
					R = Api.ImageList_GetIcon(il, index, 0);
					if(size != realSize && R != Zero) {
						//OutList(size, realSize, index, file);
						R = Api.CopyImage(R, Api.IMAGE_ICON, size, size, Api.LR_COPYDELETEORG | Api.LR_COPYRETURNORG);
					}
				}
			}
			catch(Exception e) { OutDebug(e.Message); }
			//finally { if(il != Zero) Marshal.Release(il); }
			return R;
		}

		//Gets shortcut (.lnk) icon.
		//Much faster than other shell API.
		//Also gets correct icon where iextracticon fails and/or shgetfileinfo gets blank document icon, don't know why.
		//Usually fails only when target does not exist. Then iextracticon also fails, and shgetfileinfo gets blank document icon.
		static IntPtr _GetLnkIcon(string file, int size)
		{
			var psl = new Api.ShellLink() as Api.IShellLink;
			var ppf = psl as Api.IPersistFile;
			try {
				if(ppf == null || 0 != ppf.Load(file, Api.STGM_READ)) return Zero;
				var sb = new StringBuilder(1024); //1024 (INFOTIPSIZE) is max buffer length that a property can require
				int ii;
				if(0 == psl.GetIconLocation(sb, 1024, out ii) && sb.Length > 0) {
					var R = GetIconHandleRaw(sb.ToString(), ii, size);
					if(R != Zero) return R;
				}
				sb.EnsureCapacity(1024);
				if(0 == psl.GetPath(sb, 1024, Zero, 0) && sb.Length > 0)
					return _GetFileIcon(sb.ToString(), size, IconFlag.LiteralPath);
				IntPtr pidl;
				if(0 == psl.GetIDList(out pidl))
					return _GetShellIcon(true, null, pidl, size, true);
				return Zero;
			}
			finally {
				Api.ReleaseComObject(ppf);
				Api.ReleaseComObject(psl);
			}
		}

		//If size is in enum ShellSize range (<=0), calls GetShellIconSize. If size is invalid, uses nearest valid value.
		static int _NormalizeIconSizeParameter(int size)
		{
			if(size > 256) size = 256; else if(size < (int)ShellSize.Jumbo) size = (int)ShellSize.Small;
			if(size > 0) return size;
			return GetShellIconSize((ShellSize)size);
		}

		public enum ShellSize
		{
			/// <summary>
			/// Small icons displayed in Explorer folders. Default 16x16.
			/// </summary>
			Small = 0,
			/// <summary>
			/// Large icons displayed in Explorer folders. Default 32x32.
			/// </summary>
			Large = -1,
			/// <summary>
			/// Extra large icons displayed in Explorer folders. Default 48x48.
			/// </summary>
			ExtraLarge = -2,
			/// <summary>
			/// 256x256 icons displayed in Explorer folders.
			/// </summary>
			Jumbo = -3,

			//It seems this is obsolete. Now always the same as shell small icons.
			//On Win XP can be changed in CP -> Display -> Appearance -> Advanced -> Caption Buttons. On Win7 it does not change icon size. On Win10 completely hidden.
			//Instead can use SystemInformation.SmallIconSize.
			///// <summary>
			///// Window title bar icons. Default 16x16.
			///// </summary>
			//Window = -4
		}

		/// <summary>
		/// Gets the width and height of shell icons of standard sizes - small, large, extra large and jumbo.
		/// The first four sizes depend on text size (DPI) that can be changed in Control Panel. If text size is 100%, they usually are 16, 32, 48.
		/// Jumbo is always 256.
		/// </summary>
		/// <param name="shellSize"><see cref="ShellSize"/></param>
		public static int GetShellIconSize(ShellSize shellSize)
		{
			switch(shellSize) {
			case ShellSize.Jumbo: return 256;
			case ShellSize.Large:
				if(_shellIconSizeLarge == 0) _shellIconSizeLarge = _GetShellIconSize(Api.SHIL_LARGE, 32);
				return _shellIconSizeLarge;
			case ShellSize.ExtraLarge:
				if(_shellIconSizeExtraLarge == 0) _shellIconSizeExtraLarge = _GetShellIconSize(Api.SHIL_EXTRALARGE, 48);
				return _shellIconSizeExtraLarge;
			default:
				if(_shellIconSizeSmall == 0) _shellIconSizeSmall = _GetShellIconSize(Api.SHIL_SMALL, 16);
				return _shellIconSizeSmall;
			}
		}

		static int _shellIconSizeSmall, _shellIconSizeLarge, _shellIconSizeExtraLarge;

		static int _GetShellIconSize(int ilIndex, int def)
		{
			IntPtr il; int R;
			if(_GetShellImageList(ilIndex, out il) && Api.ImageList_GetIconSize(il, out R, out R)) return R;
			Debug.Assert(false);
			return def;
		}

		static bool _GetShellImageList(int ilIndex, out IntPtr R)
		{
			lock ("vK6Z4XiSxkGSfC14/or5Mw") { //strange, the API fails if called simultaneously by multiple threads
				if(0 == Api.SHGetImageList(ilIndex, ref Api.IID_IImageList, out R)) return true;
			}
			Debug.Assert(false);
			return false;
		}

		/// <summary>
		/// Extracts icon directly from file that contains it.
		/// Returns icon handle. Returns Zero if failed.
		/// Later call Api.DestroyIcon().
		/// </summary>
		/// <param name="file">.ico, .exe, .dll or other file that contains one or more icons. Also supports cursor files - .cur, .ani. Must be full path, without icon index. Supports environment variables.</param>
		/// <param name="index">Icon index or negative icon resource id in the .exe/.dll file.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		public static unsafe IntPtr GetIconHandleRaw(string file, int index, int size)
		{
			if(Empty(file)) return Zero;
			size = _NormalizeIconSizeParameter(size);

			//We use SHDefExtractIcon because of better quality resizing (although several times slower) which matches shell and imagelist resizing.
			//With .ico it matches LoadImage speed (without resizing). PrivateExtractIcons is slightly slower.

			IntPtr R = Zero;
			int hr = Api.SHDefExtractIcon(file, index, 0, &R, null, (uint)size);
			if(hr != 0) return Zero;
			return R;

			//if(Api.PrivateExtractIcons(file, index, size, size, out R, Zero, 1, 0) != 1) return Zero;
		}

		/// <summary>
		/// Gets a shell stock icon handle.
		/// </summary>
		/// <param name="icon">Shell stock icon id. For example Api.SHSTOCKICONID.SIID_APPLICATION.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		public static unsafe IntPtr GetShellStockIconHandle(Api.SHSTOCKICONID icon, int size)
		{
			size = _NormalizeIconSizeParameter(size);
			var x = new Api.SHSTOCKICONINFO(); x.cbSize = Api.SizeOf(x);
			if(0 != Api.SHGetStockIconInfo(icon, 0, ref x)) return Zero;
			var s = new string(x.szPath);
			return GetIconHandleRaw(s, x.iIcon, size);
			//CONSIDER: cache. At least exe and document icons; maybe also folder and open folder.
		}

		/// <summary>
		/// Gets native icon handle of the entry assembly of current appdomain.
		/// It is the assembly icon, not an icon from managed resources.
		/// Returns Zero if the assembly is without icon. You can use GetShellStockIconHandle(Api.SHSTOCKICONID.SIID_APPLICATION) to get default exe icon.
		/// Don't need to destroy the icon.
		/// </summary>
		/// <param name="size">Icon width and height.</param>
		public static IntPtr GetAppIconHandle(int size)
		{
			IntPtr hinst = Util.Misc.GetModuleHandleOfAppDomainEntryAssembly(); if(hinst == Zero) return Zero;
			return Api.LoadImage(hinst, Api.IDI_APPLICATION, Api.IMAGE_ICON, size, size, Api.LR_SHARED);

			//This is not 100% reliable because the icon id 32512 (IDI_APPLICATION) is undocumented.
			//I could not find a .NET method to get icon directly from native resources of assembly.
			//Could use the resource emumeration API...
			//Never mind. Anyway, we use hInstance/resId with MessageBoxIndirect (which does not support handles) etc.
			//info: MSDN says that LR_SHARED gets cached icon regardless of size argument, but it is not true. Caches each size separately. Tested on Win 10, 7, XP.
		}

		/// <summary>
		/// Creates completely transparent monochrome icon.
		/// Returns icon handle.
		/// Later call Api.DestroyIcon().
		/// </summary>
		public static IntPtr CreateBlankIcon(int width, int height)
		{
			int nb = Calc.AlignUp(width, 32) / 8 * height;
			var aAnd = new byte[nb]; for(int i = 0; i < nb; i++) aAnd[i] = 0xff;
			var aXor = new byte[nb];
			return Api.CreateIcon(Zero, width, height, 1, 1, aAnd, aXor);

			//speed: 5-10 mcs. Faster than CopyImage etc.
		}

		//Rarely used. Better call Api.LoadImage directly.
		///// <summary>
		///// Loads cursor from file.
		///// Returns cursor handle. Returns Zero if failed.
		///// Later call Api.DestroyCursor().
		///// </summary>
		///// <param name="file">.cur or .ani file.</param>
		///// <param name="size">Width and height. If 0, uses system default size.</param>
		///// <remarks>
		///// When need a standard system cursor, instead use the Cursors class or Api.LoadCursor.
		///// When need a cursor from resources, instead use .NET class Cursor; also it can load some cursor files.
		///// </remarks>
		//public static IntPtr GetCursorHandle(string file, int size = 0)
		//{
		//	uint fl = Api.LR_LOADFROMFILE; if(size == 0) fl |= Api.LR_DEFAULTSIZE;
		//	return Api.LoadImage(Zero, Path_.MakeFullPath(file), Api.IMAGE_CURSOR, size, size, fl);
		//}

		/// <summary>
		/// Parses icon location string.
		/// Returns true if it includes icon index or resource id.
		/// </summary>
		/// <param name="s">Icon location. Can be "path,index" or "path,-id" or just path. Receives path.</param>
		/// <param name="index">Receives the number or 0.</param>
		/// <remarks>Also supports path enclosed in double quotes like "\"path\",index", and spaces between comma and index like "path, index".</remarks>
		public static bool ParseIconLocation(ref string s, out int index)
		{
			index = 0;
			if(Empty(s)) return false;
			if(s[0] == '\"') s = s.Replace("\"", ""); //can be eg "path",index
			if(s.Length < 3) return false;
			if(!Calc.IsDigit(s[s.Length - 1])) return false;
			int e, i = s.LastIndexOf(','); if(i < 1) return false;
			index = s.ToInt_(i + 1, out e); if(e != s.Length) return false;
			s = s.Remove(i);
			return true;

			//note: PathParseIconLocation is buggy. Eg splits "path,5moreText". Or from "path, 5" removes ", 5" and returns 0.
		}

		/// <summary>
		/// For <see cref="AsyncIcons.Add"/>. 
		/// </summary>
		public struct AsyncIn
		{
			public string file;
			public object obj;

			public AsyncIn(string file, object obj) { this.file = file; this.obj = obj; }
		}

		/// <summary>
		/// For <see cref="AsyncIcons.AsyncCallback"/>. 
		/// </summary>
		public class AsyncResult
		{
			/// <summary>file passed to Add().</summary>
			public string file;
			/// <summary>obj passed to Add().</summary>
			public object obj;
			/// <summary>Icon handle. You can use <see cref="HandleToImage"/> if need Image; else finally call Api.DestroyIcon(). Can be Zero.</summary>
			public IntPtr hIcon;
			/// <summary>Icon converted to Image object, if used IconFlag.NeedImage and the thread pool decided to convert handle to Image. You should call Dispose() when finished using it. Can be null.</summary>
			//public Image image;

			public AsyncResult(string file, object obj) { this.file = file; this.obj = obj; }
		}

		/// <summary>
		/// For <see cref="AsyncIcons.GetAllAsync"/>. 
		/// </summary>
		/// <param name="result">Contains icon Image or handle, as well as the input parameters. <see cref="AsyncResult"/></param>
		/// <param name="objCommon">objCommon passed to GetAllAsync().</param>
		/// <param name="nLeft">How many icons is still to get. Eg 0 if this is the last icon.</param>
		public delegate void AsyncCallback(AsyncResult result, object objCommon, int nLeft);

		/// <summary>
		/// Gets file icons asynchronously.
		/// Use to avoid waiting until all icons are extracted before displaying them in a UI (menu etc).
		/// Instead you show the UI without icons, and then asynchronously receive icons when they are extracted.
		/// At first call Add() for each file. Then call GetAllAsync().
		/// Create a callback function of type AsyncCallback and pass its delegate to GetAllAsync().
		/// </summary>
		public class AsyncIcons :IDisposable
		{

			//never mind:
			//	Could instead use Dictionary<string, List<object>>, to avoid extracting icon multiple times for the same path or even file type (difficult).
			//	But better is simpler code. Who wants max speed, in most cases can use a saved imagelist instead of getting multiple icons each time.

			List<AsyncIn> _files = new List<AsyncIn>();
			List<_AsyncWork> _works = new List<_AsyncWork>();

			/// <summary>
			/// Adds a file path to an internal collection.
			/// </summary>
			/// <param name="file">File path etc to pass to <see cref="GetIconHandle"/>.</param>
			/// <param name="obj">Something to pass to your callback function together with icon handle for this file.</param>
			public void Add(string file, object obj)
			{
				if(Empty(file)) return;
				_files.Add(new AsyncIn(file, obj));
			}

			/// <summary>
			/// Adds multiple file paths to an internal collection.
			/// The same as calling Add(string, object) multiple times.
			/// This function copies the list.
			/// </summary>
			public void Add(IEnumerable<AsyncIn> files)
			{
				if(files != null) _files.AddRange(files);
			}

			/// <summary>
			/// Gets the number of items added with Add().
			/// GetAllAsync() sets it = 0.
			/// </summary>
			public int Count { get { return _files.Count; } }

			/// <summary>
			/// Starts getting icons of files added with Add().
			/// After this function returns, icons are asynchronously extracted with <see cref="GetIconHandle"/>, and callback called with icon handle (or Zero if failed).
			/// The callback is called in this thread. This thread must have a message loop (eg Application.Run()).
			/// If you'll need more icons, you can call Add() and GetAllAsync() again with the same _AsyncIcons instance, even if getting old icons if still not finished.
			/// </summary>
			/// <param name="callback">A callback function delegate.</param>
			/// <param name="iconSize">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
			/// <param name="flags"><see cref="IconFlag"/></param>
			/// <param name="objCommon">Something to pass to callback functions.</param>
			public void GetAllAsync(AsyncCallback callback, int iconSize, IconFlag flags = 0, object objCommon = null)
			{
				if(_files.Count == 0) return;
				var work = new _AsyncWork();
				_works.Add(work);
				work.GetAllAsync(this, callback, iconSize, flags, objCommon);
				_files.Clear();
			}

			//We use List<_AsyncWork> to support getting additional icons after GetAllAsync() was already called and possibly still executing.
			//Example:
			//	Client adds icons with Add() and calls GetAllAsync().
			//	But then client wants to get more icons with the same _AsyncIcons instance. Again calls Add() and GetAllAsync().
			//	For each GetAllAsync() we add new _AsyncWork to the list and let it get the new icons.
			//	Using the same _AsyncIcons would be difficult because everything is executing async in multiple threads.
			class _AsyncWork
			{
				AsyncIcons _host;
				AsyncCallback _callback;
				int _iconSize;
				IconFlag _iconFlags;
				object _objCommon;
				int _counter;
				volatile int _nPending;
				bool _canceled;

				internal void GetAllAsync(AsyncIcons host, AsyncCallback callback, int iconSize, IconFlag flags, object objCommon)
				{
					Debug.Assert(_callback == null); //must be called once

					_host = host;
					_callback = callback;
					_iconSize = iconSize;
					_iconFlags = flags;
					_objCommon = objCommon;
					_counter = _host._files.Count;

					using(new Util.LibEnsureWindowsFormsSynchronizationContext()) {
						foreach(var v in _host._files) {
							if(!Empty(v.file)) _GetIconAsync(new AsyncResult(v.file, v.obj));
						}
					}
				}

#if true
				void _GetIconAsync(AsyncResult state)
				{
					Util.ThreadPoolSTA.SubmitCallback(state, d =>
					{ //this code runs in a thread pool thread
						if(_canceled) {
							d.completionCallback = null;
							return;
						}
						//WaitMS(10);
						var k = d.state as AsyncResult;
						k.hIcon = GetIconHandle(k.file, _iconSize, _iconFlags);

						//var hi = GetIconHandle(k.file, _iconSize, _iconFlags);
						//if(_iconFlags.HasFlag(IconFlag.NeedImage) && _nPending>20) { /*Out(_nPending);*/ k.image = HandleToImage(hi); } else k.hIcon = hi;

						//Out("1");

						//Prevent overflowing the message queue and the number of icon handles.
						//Because bad things start when eg toolbar icon count is more than 3000 and they are extracted faster than consumed.
						//But don't make the threshold too low, because then may need to wait unnecessarily, and it makes slower.
						if(Interlocked.Increment(ref _nPending) >= 900) {
							//Out(_nPending);
							//var perf = new Perf.Inst(true);
							WaitMS(10);
							//while(_nPending >= 900) WaitMS(10);
							//perf.NW();
						}
					}, o =>
					{ //this code runs in the caller's thread
						Interlocked.Decrement(ref _nPending);

						//Out("2");

						var k = o as AsyncResult;
						if(_canceled) {
							Api.DestroyIcon(k.hIcon);
						} else {
							_callback(k, _objCommon, --_counter); //even if hIcon == Zero, it can be useful
							if(_counter == 0) {
								_host._works.Remove(this);
								_host = null;
								Debug.Assert(_nPending == 0);
							}
						}
					});
				}
#elif false
				async void _GetIconAsync(AsyncResult state)
				{
					var task = Task.Factory.StartNew(() =>
					{
						if(_canceled) return;
						//WaitMS(500);
						var k = state;
						k.hIcon = GetIconHandle(k.file, _iconSize, _iconFlags);
					}, CancellationToken.None, TaskCreationOptions.None, _staTaskScheduler);
					await task;

					//async continuation

					if(_canceled) {
						Api.DestroyIcon(state.hIcon);
					} else {
						_callback(state, _objCommon); //even if hi == Zero, it can be useful
						if(--_counter == 0) {
							if(_onFinished != null) _onFinished(_objCommon);
							_host._works.Remove(this);
							_host = null;
						}
					}
				}

				static readonly System.Threading.Tasks.Schedulers.StaTaskScheduler _staTaskScheduler = new System.Threading.Tasks.Schedulers.StaTaskScheduler(4); //tested: without StaTaskScheduler would be 4 threads. With 3 the UI thread is slightly faster.
#else
				async void _GetIconAsync(AsyncResult state)
				{
					var task = Task.Run(() =>
					{
						if(_canceled) return;
						//WaitMS(500);
						var k = state;
						k.hIcon = GetIconHandle(k.file, _iconSize, _iconFlags);
					});
					await task;

					//async continuation

					if(_canceled) {
						Api.DestroyIcon(state.hIcon);
					} else {
						_callback(state, _objCommon); //even if hi == Zero, it can be useful
						if(--_counter == 0) {
							if(_onFinished != null) _onFinished(_objCommon);
							_host._works.Remove(this);
							_host = null;
						}
					}
				}
#endif

				internal void Cancel()
				{
					_canceled = true;
				}
			}

			/// <summary>
			/// Clears the internal collection of file paths added with Add().
			/// </summary>
			public void Clear()
			{
				_files.Clear();
			}

			/// <summary>
			/// Stops getting icons and calling callback functions.
			/// </summary>
			public void Cancel()
			{
				//Out(_works.Count);
				if(_works.Count == 0) return;
				foreach(var work in _works) work.Cancel();
				_works.Clear();
			}

			/// <summary>
			/// Calls Cancel().
			/// </summary>
			public void Dispose()
			{
				Cancel();
			}

			~AsyncIcons() { Dispose(); }
		}
	}
}
