using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Xml.Linq;
using System.Drawing.Imaging;

using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Gets icons for files etc.
	/// </summary>
	//[DebuggerStepThrough]
	public static class Icons
	{
		/// <summary>
		/// Used with get-icon functions.
		/// </summary>
		/// <tocexclude />
		[Flags]
		public enum IconFlags
		{
			/// <summary>
			/// The 'file' argument is literal full path. Don't parse "path,index", don't support ".ext" (file type icon), don't make fully-qualified, etc.
			/// </summary>
			LiteralPath = 1,

			/// <summary>
			/// If file is not full path, call <see cref="Files.SearchPath">Files.SearchPath</see>. Without this flag searches only in <see cref="Folders.ThisApp"/>.
			/// </summary>
			SearchPath = 2,

#if false
			/// <summary>
			/// Scale the specified size according to DPI (text size) specified in Control Panel.
			/// </summary>
			DpiScale = 4, //rejected. In most cases can use standard-size icons from enum ShellSize, they are DPI-scaled. Or pass size * Util.Dpi.BaseDPI.

			/// Use shell API for all file types, including exe and ico.
			Shell=8, //rejected because SHGetFileInfo does not get exe icon with shield overlay

			/// <summary>
			/// If file does not exist or fails to get its icon, get common icon for that file type, or default document icon if cannot get common icon.
			/// </summary>
			DefaultIfFails = 16, //rejected. Now for exe/ico/etc is like with shell API: if file exists, gets default icon (exe or document), else returns Zero.

			/// <summary>
			/// Used only with AsyncIcons class. If the thread pool has spare time, let it convert icon handle to Image object. The callback will receive either handle or Image, it must check both for Zero and null. This is to make whole process as fast as possible.
			/// </summary>
			NeedImage = 128, //rejected because with our menu/toolbar almost always makes slower
#endif
		}

		/// <summary>
		/// Gets file icon.
		/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, flags etc.
		/// Calls <see cref="GetFileIconHandle"/> and converts to Bitmap. Returns null if failed, for example if the file does not exist.
		/// Later call Dispose().
		/// </summary>
		/// <param name="file">
		/// Any file, folder, URL like "http://..." or "shell:..." etc, shell item like "::{CLSID}" or "::{CLSID1}\::{CLSID2}". Also can be a file type like ".txt" or a protocol like "http:".
		/// If it is a file containing multiple icons (eg exe, dll), can be specified icon index like "path,index" or native icon resource id like "path,-id".
		/// If not full path, the file must be in program's folder.
		/// </param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="flags"><see cref="IconFlags"/></param>
		public static Bitmap GetFileIconImage(string file, int size, IconFlags flags = 0)
		{
			return HandleToImage(GetFileIconHandle(file, size, flags));
		}

		/// <summary>
		/// Gets icon of a file or other shell object specified by its ITEMIDLIST pointer.
		/// Calls <see cref="GetPidlIconHandle"/> and converts to Bitmap. Returns null if failed.
		/// Later call Dispose().
		/// </summary>
		/// <param name="pidl">ITEMIDLIST pointer.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="freePidl">Call Marshal.FreeCoTaskMem(pidl).</param>
		public static Bitmap GetPidlIconImage(IntPtr pidl, int size, bool freePidl = false)
		{
			return HandleToImage(GetPidlIconHandle(pidl, size, freePidl));
		}

		/// <summary>
		/// Converts unmanaged icon to Bitmap object and destroys the unmanaged icon.
		/// Returns null if hi is Zero or fails to convert.
		/// </summary>
		/// <param name="hi">Icon handle.</param>
		public static Bitmap HandleToImage(IntPtr hi)
		{
			//note: don't use Bitmap.FromHicon. It just calls GdipCreateBitmapFromHICON which does not support alpha etc. Icon.ToBitmap works around it.

			if(hi == Zero) return null;
			//var perf = new Perf.Inst(true);
			Icon ic = Icon.FromHandle(hi);
			Bitmap im = null;
			try { im = ic.ToBitmap(); } catch(Exception e) { DebugPrint(e.Message); }
			ic.Dispose();
			Api.DestroyIcon(hi);
			//perf.NW();
			return im;
		}

		/// <summary>
		/// Gets file icon.
		/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, flags etc.
		/// Returns icon handle. Returns Zero if failed, for example if the file does not exist.
		/// Later call <see cref="DestroyIconHandle"/>.
		/// </summary>
		/// <param name="file">
		/// Any file, folder, URL like "http://..." or "shell:..." etc, shell item like "::{CLSID}" or "::{CLSID1}\::{CLSID2}". Also can be a file type like ".txt" or a protocol like "http:".
		/// If it is a file containing multiple icons (eg exe, dll), can be specified icon index like "path,index" or native icon resource id like "path,-id".
		/// If not full path, the file must be in program's folder.
		/// </param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="flags"><see cref="IconFlags"/></param>
		/// <seealso cref="Wnd.Misc.GetIconHandle"/>
		public static IntPtr GetFileIconHandle(string file, int size, IconFlags flags = 0)
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
			//perf.Next(); PrintList(perf.Times, file);
			//Print($"<><c 0xff0000>{file}</c>");
			return R;
		}

		/// <summary>
		/// Gets icon of a file or other shell object specified by its ITEMIDLIST pointer.
		/// Returns icon handle. Returns Zero if failed.
		/// Later call <see cref="DestroyIconHandle"/>.
		/// </summary>
		/// <param name="pidl">ITEMIDLIST pointer.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		/// <param name="freePidl">Call Marshal.FreeCoTaskMem(pidl).</param>
		public static IntPtr GetPidlIconHandle(IntPtr pidl, int size, bool freePidl = false)
		{
			if(pidl == Zero) return Zero;
			size = _NormalizeIconSizeParameter(size);
			return _GetShellIcon(true, null, pidl, size, freePidl);
		}

		internal static IntPtr _GetFileIcon(string file, int size, IconFlags flags)
		{
			IntPtr R = Zero, pidl = Zero;
			int index = 0;
			bool extractFromFile = false, isFileType = false, isURL = false, isCLSID = false, isPath = true;
			//bool getDefaultIfFails = 0!=(flags&IconFlags.DefaultIfFails);

			bool searchPath = 0 != (flags & IconFlags.SearchPath);

			if(0 == (flags & IconFlags.LiteralPath)) {
				//is ".ext" or "protocol:"?
				isFileType = Path_.Internal.IsExtension(file) || (isURL = Path_.Internal.IsProtocol(file));
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

			if(isPath /*&& (extractFromFile || 0==(flags&IconFlags.Shell))*/) {
				int ext = 0;
				if(!extractFromFile && file.Length > 4) ext = file.EndsWith_(true, ".exe", ".scr", ".ico", ".cur", ".ani");
				if(extractFromFile || ext > 0) {
					R = GetFileIconHandleRaw(file, index, size);
					if(R != Zero || extractFromFile) return R;
					switch(Files.ExistsAs(file)) {
					case Files.ItIs.NotFound:
						return Zero;
					case Files.ItIs.File:
						var siid = Native.SHSTOCKICONID.SIID_DOCNOASSOC;
						if(ext >= 1 && ext <= 2) siid = Native.SHSTOCKICONID.SIID_APPLICATION;
						return GetShellStockIconHandle(siid, size);
						//case Files.ItIs.Directory: //folder name ends with .ico etc
					}
				} else if(file.EndsWith_(".lnk", true)) {
					R = _GetLnkIcon(file, size);
					if(R != Zero) return R;
					//PrintList("_GetLnkIcon failed", file);
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
			//if(0==(flags&IconFlags.Shell)){
			string progId = isCLSID ? null : Files.Misc.GetFileTypeOrProtocolRegistryKey(file, isFileType, isURL);

			RegistryKey rk = (progId == null) ? null : Registry_.Open(progId, Registry.ClassesRoot);
			//PrintList(file, progId, isFileType, isURL, rk != null);

			if(rk == null) {
				//Unregistered file type/protocol, no extension, folder, ::{CLSID}, shell:WinStoreApp, or no progId key in HKCR
				//PrintList(@"unregistered", file, progId);
				if(progId != null) goto gr; //the file type is known, but no progid key in HKCR. Let shell API figure out. Rare.
				if(isExt || (isPath && Files.FileExists(file))) return GetShellStockIconHandle(Native.SHSTOCKICONID.SIID_DOCNOASSOC, size);
				goto gr;
			}

			//Registered file type/protocol.
			using(rk) {
				if(Registry_.KeyExists(@"ShellEx\IconHandler", rk)) {
					//PrintList(@"handler", file);
					goto gr;
				}

				string icon;
				if(Registry_.GetString(out icon, "", @"DefaultIcon", rk) && icon.Length > 0) {
					//PrintList("registry: DefaultIcon", file, icon);
					if(icon[0] == '@') icon = null; //eg @{Microsoft.Windows.Photos_16.622.13140.0_x64__8wekyb3d8bbwe?ms-resource://Microsoft.Windows.Photos/Files/Assets/PhotosLogoExtensions.png}
					else ParseIconLocation(ref icon, out index);
				} else if(Registry_.GetString(out icon, "", @"shell\open\command", rk) && icon.Length > 0) {
					//PrintList(@"registry: shell\open\command", file, icon);
					var a = icon.Split_((icon[0] == '\"') ? "\"" : " ", StringSplitOptions.RemoveEmptyEntries);
					icon = (a.Length == 0) ? null : a[0];
					if(icon.EndsWith_("rundll32.exe", true)) icon = null;
				} else {
					icon = null;
					//PrintList("registry: no", file);
					//Usually shell API somehow gets icon.
					//For example also looks in .ext -> PerceivedType -> HKCR\SystemFileAssociations.
					//We can use AssocQueryString(ASSOCSTR_DEFAULTICON), but it is slow and not always gets correct icon.
				}

				//if(icon != null) PrintList(file, icon);

				if(icon == "%1") {
					//Print(file);
					if(isPath) icon = file;
					else icon = null;
				}

				if(icon != null) {
					icon = Path_.ExpandEnvVar(icon);
					if(!Path_.IsFullPath(icon)) icon = Folders.System + icon;
					R = GetFileIconHandleRaw(icon, index, size);
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
			lock("TK6Z4XiSxkGSfC14/or5Mw") {
				try {
					uint fl = Api.SHGFI_SYSICONINDEX | Api.SHGFI_SHELLICONSIZE;
					if(ilIndex == Api.SHIL_SMALL) fl |= Api.SHGFI_SMALLICON;
					if(isPidl) fl |= Api.SHGFI_PIDL; else fl |= Api.SHGFI_USEFILEATTRIBUTES;
					var x = new Api.SHFILEINFO();
					il = Api.SHGetFileInfo(pidl, 0, ref x, Api.SizeOf(x), fl);
					if(il != Zero) index = x.iIcon;
					//Marshal.Release(il); //undocumented, but without it IImageList refcount grows. Probably it's ok, because it is static, never deleted until process exits.
				}
				catch { DebugPrint("exception"); }
			}
			if(index < 0) return Zero;

			//note: Getting icon from imagelist must be in STA thread too, else fails with some file types.
			//tested: This part works without locking. Using another lock here makes slower.

			try {
				if(ilIndex == Api.SHIL_SMALL || ilIndex == Api.SHIL_LARGE || _GetShellImageList(ilIndex, out il)) {
					//PrintList(il, Util.LibDebug_.GetComObjRefCount(il));
					R = Api.ImageList_GetIcon(il, index, 0);
					if(size != realSize && R != Zero) {
						//PrintList(size, realSize, index, file);
						R = Api.CopyImage(R, Api.IMAGE_ICON, size, size, Api.LR_COPYDELETEORG | Api.LR_COPYRETURNORG);
					}
				}
			}
			catch(Exception e) { DebugPrint(e.Message); }
			//finally { if(il != Zero) Marshal.Release(il); }
			return R;
		}

		//Gets shortcut (.lnk) icon.
		//Much faster than other shell API.
		//Also gets correct icon where iextracticon fails and/or shgetfileinfo gets blank document icon, don't know why.
		//Usually fails only when target does not exist. Then iextracticon also fails, and shgetfileinfo gets blank document icon.
		//If fails, returns Zero. No exceptions.
		static IntPtr _GetLnkIcon(string file, int size)
		{
			try {
				using(var x = Files.LnkShortcut.Open(file)) {
					var s = x.GetIconLocation(out int ii); if(s != null) return GetFileIconHandleRaw(s, ii, size);
					s = x.TargetPathRawMSI; if(s != null) return GetFileIconHandle(s, size);
					//PrintList("need IDList", file);
					return GetPidlIconHandle(x.TargetIDList, size, true);
				}
			}
			catch { return Zero; }
		}

		//If size is in enum ShellSize range (<=0), calls GetShellIconSize. If size is invalid, uses nearest valid value.
		static int _NormalizeIconSizeParameter(int size)
		{
			if(size > 256) size = 256; else if(size < (int)ShellSize.Jumbo) size = (int)ShellSize.SysSmall;
			if(size > 0) return size;
			return GetShellIconSize((ShellSize)size);
		}

		/// <summary>
		/// Standard icon sizes.
		/// </summary>
		/// <tocexclude />
		public enum ShellSize
		{
			/// <summary>
			/// Icons displayed in window title bar and system notification area (tray). Usually 16x16 when normal DPI, the same as Small.
			/// </summary>
			SysSmall = 0,
			/// <summary>
			/// Small icons displayed in Explorer folders. Usually 16x16 when normal DPI.
			/// </summary>
			Small = -1,
			/// <summary>
			/// Large icons displayed in Explorer folders. Usually 32x32 when normal DPI.
			/// </summary>
			Large = -2,
			/// <summary>
			/// Extra large icons displayed in Explorer folders. Usually 48x48 when normal DPI.
			/// </summary>
			ExtraLarge = -3,
			/// <summary>
			/// 256x256 icons displayed in Explorer folders.
			/// </summary>
			Jumbo = -4,

			//It seems SysSmall/SysLarge are obsolete. Now always the same as shell small icons.
			//On Win XP can be changed in CP -> Display -> Appearance -> Advanced -> Caption Buttons. On Win7 it does not change icon size. On Win10 completely hidden.
			//But to get it we use SystemInformation.SmallIconSize or Api.GetSystemMetrics(Api.SM_CXSMICON/SM_CXICON) which is faster etc than through shell imagelist.
		}

		/// <summary>
		/// Gets the width and height of shell icons of standard sizes - small, large, extra large and jumbo.
		/// Jumbo is always 256. Others depend on text size (DPI) that can be changed in Control Panel. If text size is 100%, they usually are 16, 32, 48.
		/// </summary>
		/// <param name="shellSize"><see cref="ShellSize"/></param>
		public static int GetShellIconSize(ShellSize shellSize)
		{
			switch(shellSize) {
			case ShellSize.Small:
				if(_shellIconSizeSmall == 0) _shellIconSizeSmall = _GetShellIconSize(Api.SHIL_SMALL, 16);
				return _shellIconSizeSmall;
			case ShellSize.Large:
				if(_shellIconSizeLarge == 0) _shellIconSizeLarge = _GetShellIconSize(Api.SHIL_LARGE, 32);
				return _shellIconSizeLarge;
			case ShellSize.ExtraLarge:
				if(_shellIconSizeExtraLarge == 0) _shellIconSizeExtraLarge = _GetShellIconSize(Api.SHIL_EXTRALARGE, 48);
				return _shellIconSizeExtraLarge;
			case ShellSize.Jumbo: return 256;
			}
			//SysSmall
			return Api.GetSystemMetrics(Api.SM_CXSMICON); //SystemInformation.SmallIconSize calls the same, but for both cx and cy
		}

		static int _shellIconSizeSmall, _shellIconSizeLarge, _shellIconSizeExtraLarge;

		static int _GetShellIconSize(int ilIndex, int def)
		{
			if(_GetShellImageList(ilIndex, out IntPtr il) && Api.ImageList_GetIconSize(il, out int R, out R)) return R;
			Debug.Assert(false);
			return def;
		}

		static bool _GetShellImageList(int ilIndex, out IntPtr R)
		{
			lock("vK6Z4XiSxkGSfC14/or5Mw") { //the API fails if called simultaneously by multiple threads
				if(0 == Api.SHGetImageList(ilIndex, ref Api.IID_IImageList, out R)) return true;
			}
			Debug.Assert(false);
			return false;
		}

		/// <summary>
		/// Extracts icon directly from file that contains it.
		/// Returns icon handle. Returns Zero if failed.
		/// Later call <see cref="DestroyIconHandle"/>.
		/// </summary>
		/// <param name="file">.ico, .exe, .dll or other file that contains one or more icons. Also supports cursor files - .cur, .ani. Must be full path, without icon index. Supports environment variables.</param>
		/// <param name="index">Icon index or negative icon resource id in the .exe/.dll file.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		public static unsafe IntPtr GetFileIconHandleRaw(string file, int index = 0, int size = 16)
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
		/// <param name="icon">Shell stock icon id. For example Native.SHSTOCKICONID.SIID_APPLICATION.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		public static unsafe IntPtr GetShellStockIconHandle(Native.SHSTOCKICONID icon, int size)
		{
			var x = new Api.SHSTOCKICONINFO(); x.cbSize = Api.SizeOf(x);
			if(0 != Api.SHGetStockIconInfo(icon, 0, ref x)) return Zero;
			var s = new string(x.szPath);
			return GetFileIconHandleRaw(s, x.iIcon, size);
			//CONSIDER: cache. At least exe and document icons; maybe also folder and open folder.
		}

		/// <summary>
		/// Gets the first native icon handle of the entry assembly of current appdomain.
		/// Returns Zero if there are no icons.
		/// It is not an icon from managed resources.
		/// The icon is cached and protected from destroying, therefore don't need to destroy it, and not error to do it.
		/// </summary>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		public static IntPtr GetAppIconHandle(int size)
		{
			IntPtr hinst = Util.ModuleHandle.OfAppDomainEntryAssembly(); if(hinst == Zero) return Zero;
			size = _NormalizeIconSizeParameter(size);
			return Api.LoadImage(hinst, Api.IDI_APPLICATION, Api.IMAGE_ICON, size, size, Api.LR_SHARED);

			//This is not 100% reliable because the icon id 32512 (IDI_APPLICATION) is undocumented.
			//I could not find a .NET method to get icon directly from native resources of assembly.
			//Could use the resource emumeration API...
			//info: MSDN says that LR_SHARED gets cached icon regardless of size argument, but it is not true. Caches each size separately. Tested on Win 10, 7, XP.
		}

		/// <summary>
		/// Gets the first native icon handle of the program file of this process.
		/// Returns Zero if there are no icons.
		/// It is not an icon from managed resources.
		/// The icon is cached and protected from destroying, therefore don't need to destroy it, and not error to do it.
		/// </summary>
		/// <param name="size">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
		public static IntPtr GetProcessExeIconHandle(int size)
		{
			IntPtr hinst = Util.ModuleHandle.OfProcessExe(); if(hinst == Zero) return Zero;
			size = _NormalizeIconSizeParameter(size);
			return Api.LoadImage(hinst, Api.IDI_APPLICATION, Api.IMAGE_ICON, size, size, Api.LR_SHARED);
		}

		/// <summary>
		/// Creates completely transparent monochrome icon.
		/// Returns icon handle.
		/// Later call <see cref="DestroyIconHandle"/>.
		/// </summary>
		public static IntPtr CreateBlankIcon(int width, int height)
		{
			int nb = Calc.AlignUp(width, 32) / 8 * height;
			var aAnd = new byte[nb]; for(int i = 0; i < nb; i++) aAnd[i] = 0xff;
			var aXor = new byte[nb];
			return Api.CreateIcon(Zero, width, height, 1, 1, aAnd, aXor);

			//speed: 5-10 mcs. Faster than CopyImage etc.
		}

		/// <summary>
		/// Destroys native icon.
		/// Calls API <msdn>DestroyIcon</msdn>. Does nothing if iconHandle is Zero.
		/// </summary>
		public static void DestroyIconHandle(IntPtr iconHandle)
		{
			if(iconHandle != Zero) Api.DestroyIcon(iconHandle);
		}

		//Rarely used. Better call Api.LoadImage directly.
		///// <summary>
		///// Loads cursor from file.
		///// Returns cursor handle. Returns Zero if failed.
		///// Later call API DestroyCursor.
		///// </summary>
		///// <param name="file">.cur or .ani file.</param>
		///// <param name="size">Width and height. If 0, uses system default size.</param>
		///// <remarks>
		///// When need a standard system cursor, instead use the Cursors class or API LoadCursor.
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
			index = s.ToInt32_(i + 1, out e); if(e != s.Length) return false;
			s = s.Remove(i);
			return true;

			//note: PathParseIconLocation is buggy. Eg splits "path,5moreText". Or from "path, 5" removes ", 5" and returns 0.
		}

		/// <summary>
		/// For <see cref="AsyncIcons.Add(IEnumerable{AsyncIn})"/>. 
		/// </summary>
		/// <tocexclude />
		public struct AsyncIn
		{
#pragma warning disable 1591 //XML doc
			public string file;
			public object obj;

			public AsyncIn(string file, object obj) { this.file = file; this.obj = obj; }
#pragma warning restore 1591 //XML doc
		}

		/// <summary>
		/// For <see cref="AsyncCallback"/>. 
		/// </summary>
		/// <tocexclude />
		public class AsyncResult
		{
			/// <summary>file passed to AsyncIcons.Add().</summary>
			public string file;
			/// <summary>obj passed to AsyncIcons.Add().</summary>
			public object obj;
			/// <summary>Icon handle. You can use <see cref="HandleToImage"/> if need Image; else finally call <see cref="DestroyIconHandle"/>. Can be Zero.</summary>
			public IntPtr hIcon;
			/// <summary>Icon converted to Image object, if used IconFlags.NeedImage and the thread pool decided to convert handle to Image. You should call Dispose() when finished using it. Can be null.</summary>
			//public Image image;

			public AsyncResult(string file, object obj) { this.file = file; this.obj = obj; }
		}

		/// <summary>
		/// For <see cref="AsyncIcons.GetAllAsync"/>. 
		/// </summary>
		/// <param name="result">Contains icon Image or handle, as well as the input parameters. <see cref="AsyncResult"/></param>
		/// <param name="objCommon">objCommon passed to <see cref="AsyncIcons.GetAllAsync">GetAllAsync</see>.</param>
		/// <param name="nLeft">How many icons is still to get. Eg 0 if this is the last icon.</param>
		/// <tocexclude />
		public delegate void AsyncCallback(AsyncResult result, object objCommon, int nLeft);

		/// <summary>
		/// Gets file icons asynchronously.
		/// Use to avoid waiting until all icons are extracted before displaying them in a UI (menu etc).
		/// Instead you show the UI without icons, and then asynchronously receive icons when they are extracted.
		/// At first call <see cref="Add(string, object)"/> for each file. Then call <see cref="GetAllAsync">GetAllAsync</see>.
		/// Create a callback function of type <see cref="AsyncCallback"/> and pass its delegate to GetAllAsync.
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
			/// <param name="file">File path etc to pass to <see cref="GetFileIconHandle"/>.</param>
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
			public int Count { get => _files.Count; }

			/// <summary>
			/// Starts getting icons of files added with Add().
			/// After this function returns, icons are asynchronously extracted with <see cref="GetFileIconHandle"/>, and callback called with icon handle (or Zero if failed).
			/// The callback is called in this thread. This thread must have a message loop (eg Application.Run()).
			/// If you'll need more icons, you can call Add() and GetAllAsync() again with the same AsyncIcons instance, even if getting old icons if still not finished.
			/// </summary>
			/// <param name="callback">A callback function delegate.</param>
			/// <param name="iconSize">Icon width and height. Also can be enum <see cref="ShellSize"/>, cast to int.</param>
			/// <param name="flags"><see cref="IconFlags"/></param>
			/// <param name="objCommon">Something to pass to callback functions.</param>
			public void GetAllAsync(AsyncCallback callback, int iconSize, IconFlags flags = 0, object objCommon = null)
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
			//	But then client wants to get more icons with the same AsyncIcons instance. Again calls Add() and GetAllAsync().
			//	For each GetAllAsync() we add new _AsyncWork to the list and let it get the new icons.
			//	Using the same AsyncIcons would be difficult because everything is executing async in multiple threads.
			class _AsyncWork
			{
				AsyncIcons _host;
				AsyncCallback _callback;
				int _iconSize;
				IconFlags _iconFlags;
				object _objCommon;
				int _counter;
				volatile int _nPending;
				bool _canceled;

				internal void GetAllAsync(AsyncIcons host, AsyncCallback callback, int iconSize, IconFlags flags, object objCommon)
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
						//Thread.Sleep(10);
						var k = d.state as AsyncResult;
						k.hIcon = GetFileIconHandle(k.file, _iconSize, _iconFlags);

						//var hi = GetFileIconHandle(k.file, _iconSize, _iconFlags);
						//if(0!=(_iconFlags&IconFlags.NeedImage) && _nPending>20) { /*Print(_nPending);*/ k.image = HandleToImage(hi); } else k.hIcon = hi;

						//Print("1");

						//Prevent overflowing the message queue and the number of icon handles.
						//Because bad things start when eg toolbar icon count is more than 3000 and they are extracted faster than consumed.
						//But don't make the threshold too low, because then may need to wait unnecessarily, and it makes slower.
						if(Interlocked.Increment(ref _nPending) >= 900) {
							//Print(_nPending);
							//var perf = new Perf.Inst(true);
							Thread.Sleep(10);
							//while(_nPending >= 900) Thread.Sleep(10);
							//perf.NW();
						}
					}, o =>
					{ //this code runs in the caller's thread
						Interlocked.Decrement(ref _nPending);

						//Print("2");

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
						//Thread.Sleep(500);
						var k = state;
						k.hIcon = GetFileIconHandle(k.file, _iconSize, _iconFlags);
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
						//Thread.Sleep(500);
						var k = state;
						k.hIcon = GetFileIconHandle(k.file, _iconSize, _iconFlags);
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
				//Print(_works.Count);
				if(_works.Count == 0) return;
				foreach(var work in _works) work.Cancel();
				_works.Clear();
			}

			/// <summary>
			/// Calls <see cref="Cancel"/>.
			/// </summary>
			public void Dispose()
			{
				Cancel();
			}

			/// <summary>
			/// 
			/// </summary>
			~AsyncIcons() { Dispose(); }
		}

		/// <summary>
		/// Gets file icons.
		/// Uses 2-level cache - memory and file.
		/// </summary>
		/// <remarks>
		/// Can be used as static variables.
		/// </remarks>
		public class FileIconCache :IDisposable
		{
			XElement _x;
			Hashtable _table;
			string _cacheFile;
			int _iconSize;
			bool _dirty;
			const string _lockString = "8Ljf1bOY7kiDXutQw6O75Q";

			/// <summary>
			/// Remembers cacheFile and iconSize.
			/// </summary>
			/// <param name="cacheFile"></param>
			/// <param name="iconSize"></param>
			public FileIconCache(string cacheFile, int iconSize)
			{
				_cacheFile = cacheFile;
				_iconSize = iconSize;
			}

			///
			~FileIconCache() { Dispose(); }

			/// <summary>
			/// Calls <see cref="SaveCacheFileNow"/>.
			/// </summary>
			public void Dispose()
			{
				SaveCacheFileNow();
			}

			/// <summary>
			/// Saves to the cache file now, if need.
			/// Don't need to call this explicitly. It is called by Dispose.
			/// </summary>
			public void SaveCacheFileNow()
			{
				if(_dirty) {
					lock(_lockString) {
						if(_dirty) {
							_dirty = false;
							_x.Save(_cacheFile);
						}
					}
				}
			}

			/// <summary>
			/// Clears the memory cache and deletes the cache file.
			/// </summary>
			public void ClearCache()
			{
				lock(_lockString) {
					_dirty = false;
					File.Delete(_cacheFile);
					_x = null;
					_table = null;
				}
			}

			/// <summary>
			/// Gets file icon as Bitmap.
			/// If it is in the memory cache, gets it from there.
			/// Else if it is in the file cache, gets it from there and adds to the memory cache.
			/// Else gets from file (uses <see cref="Icons.GetFileIconImage"/> and adds to the file cache and to the memory cache.
			/// Returns null if GetFileIconImage failed, eg file does not exist.
			/// </summary>
			/// <param name="file">Any file or folder.</param>
			/// <param name="useExt">
			/// Get file type icon, depending on filename extension. Use this to avoid getting separate image object for each file of same type.
			/// This is ignored if filename extension is ".ico" or ".exe" or starts with ".exe," or ".dll,".
			/// </param>
			/// <remarks>
			/// Thread-safe. The variable can be static, and this function can be called from any thread without locking.
			/// </remarks>
			[MethodImpl(MethodImplOptions.NoOptimization)] //for startup speed when not ngened
			public Bitmap GetImage(string file, bool useExt)
			{
				if(useExt) {
					var ext = Path.GetExtension(file);
					if(ext.Length == 0) {
						if(Files.ExistsAsDirectory(file)) ext = file;
						else ext = ".no-ext";
					} else {
						//ext = ext.ToLower_();
						if(ext.Equals_(".ico", true) || ext.Equals_(".exe", true) || ext.StartsWith_(".exe,", true) || ext.StartsWith_(".dll,", true)) ext = file;
					}
					file = ext;
				}

				lock(_lockString) {
					//is in memory cache?
					if(_table == null) _table = new Hashtable(StringComparer.OrdinalIgnoreCase);
					else { var o = _table[file]; if(o != null) return o as Bitmap; }

					//is in file cache?
					Bitmap R = null;
					try {
						if(_x == null && Files.ExistsAsFile(_cacheFile)) {
							_x = XElement.Load(_cacheFile);
							if(_iconSize != _x.Attribute_("size", 0) || Util.Dpi.BaseDPI != _x.Attribute_("dpi", 0)) {
								_x = null;
								DebugPrint("info: cleared icon cache");
							}

							//TODO: Delete unused entries. Maybe try to auto-update changed icons.
						}
						if(_x != null) {
							var x = _x.Element_("i", "name", file, true);
							if(x != null) {
								using(var ms = new MemoryStream(Convert.FromBase64String(x.Value))) {
									R = new Bitmap(ms);
								}
							}
						}
					}
					catch(Exception ex) {
						DebugPrint(ex.Message);
					}

					if(R == null) {
						//get file icon and add to file cache
						R = Icons.GetFileIconImage(file, _iconSize);
						if(R != null) {
							using(var ms = new MemoryStream()) {
								R.Save(ms, ImageFormat.Png);
								var s = Convert.ToBase64String(ms.ToArray());
								var d = new XElement("i", s);
								d.SetAttributeValue("name", file);
								if(_x == null) {
									_x = new XElement("images");
									_x.SetAttributeValue("size", _iconSize);
									_x.SetAttributeValue("dpi", Util.Dpi.BaseDPI);
								}
								_x.Add(d);
							}
							_dirty = true;
						}
					}

					//add to memory cache
					if(R != null) _table[file] = R;
					return R;
				}
			}
		}
	}
}
