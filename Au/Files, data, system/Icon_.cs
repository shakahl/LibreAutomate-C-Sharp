using System;
using System.Collections.Generic;
using System.Text;
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
//using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Gets icons for files etc.
	/// </summary>
	/// <seealso cref="Util.IconsAsync"/>
	public static class Icon_
	{
		/// <summary>
		/// Gets file icon.
		/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, flags etc.
		/// </summary>
		/// <returns>Returns null if failed.</returns>
		/// <param name="file">
		/// Can be:
		/// - Path of any file or folder.
		/// - URL, like <c>"http://..."</c> or <c>"mailto:a@b.c"</c> or <c>"file:///path"</c>.
		/// - ITEMIDLIST like <c>":: HexEncodedITEMIDLIST"</c>. It can be of any file, folder, URL or virtual object like Control Panel. See <see cref="Shell.Pidl.ToHexString"/>.
		/// - Shell object parsing name, like <c>@"::{CLSID-1}\::{CLSID-2}"</c> or <c>@"shell:AppsFolder\WinStoreAppId"</c>.
		/// - File type like <c>".txt"</c> or URL protocol like <c>"http:"</c>.
		/// 
		/// If it is a file containing multiple icons (eg exe, dll), can be specified icon index like <c>"path,index"</c> or native icon resource id like <c>"path,-id"</c>.
		/// If not full path, the function will look in <see cref="Folders.ThisAppImages"/>. See also <see cref="GIFlags"/>.
		/// Supports environment variables (see <see cref="Path_.ExpandEnvVar"/>).
		/// </param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="IconSize"/>, cast to int.</param>
		/// <param name="flags"></param>
		public static Icon GetFileIcon(string file, int size, GIFlags flags = 0)
			=> HandleToIcon(GetFileIconHandle(file, size, flags), true);

		/// <summary>
		/// Gets file icon.
		/// More info: <see cref="GetFileIcon"/>.
		/// </summary>
		/// <returns>Returns null if failed.</returns>
		public static Bitmap GetFileIconImage(string file, int size, GIFlags flags = 0)
			=> HandleToImage(GetFileIconHandle(file, size, flags), true);

		/// <summary>
		/// Gets file icon.
		/// More info: <see cref="GetFileIcon"/>.
		/// </summary>
		/// <returns>Returns icon handle, or default(IntPtr) if failed. Later call <see cref="DestroyIconHandle"/> or some <b>HandleToX</b> function that will destroy it.</returns>
		/// <seealso cref="Wnd.More.GetIconHandle"/>
		public static IntPtr GetFileIconHandle(string file, int size, GIFlags flags = 0)
		{
			if(Empty(file)) return default;
			size = _NormalizeIconSizeParameter(size);
			file = Path_.ExpandEnvVar(file);

			//var perf = Perf.StartNew();
			IntPtr R = _GetFileIcon(file, size, flags);
			//perf.Next(); Print(perf.ToString(), file);
			//Print($"<><c 0xff0000>{file}</c>");
			return R;
		}

		/// <summary>
		/// Gets icon of a file or other shell object specified as ITEMIDLIST.
		/// </summary>
		/// <returns>Returns null if failed.</returns>
		/// <param name="pidl">ITEMIDLIST pointer (PIDL).</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="IconSize"/>, cast to int.</param>
		public static Icon GetPidlIcon(Shell.Pidl pidl, int size)
			=> HandleToIcon(GetPidlIconHandle(pidl, size), true);

		/// <summary>
		/// Gets icon of a file or other shell object specified as ITEMIDLIST.
		/// More info: <see cref="GetPidlIcon"/>.
		/// </summary>
		/// <returns>Returns null if failed.</returns>
		public static Bitmap GetPidlIconImage(Shell.Pidl pidl, int size)
			=> HandleToImage(GetPidlIconHandle(pidl, size), true);

		/// <summary>
		/// Gets icon of a file or other shell object specified as ITEMIDLIST.
		/// More info: <see cref="GetPidlIcon"/>.
		/// </summary>
		/// <returns>Returns icon handle, or default(IntPtr) if failed. Later call <see cref="DestroyIconHandle"/> or some <b>HandleToX</b> function that will destroy it.</returns>
		public static IntPtr GetPidlIconHandle(Shell.Pidl pidl, int size)
		{
			if(pidl?.IsNull ?? false) return default;
			size = _NormalizeIconSizeParameter(size);
			return _GetShellIcon(true, null, pidl, size);
		}

		static IntPtr _GetFileIcon(string file, int size, GIFlags flags)
		{
			IntPtr R = default;
			int index = 0;
			bool extractFromFile = false, isFileType = false, isURL = false, isShellPath = false, isPath = true;
			//bool getDefaultIfFails = 0!=(flags&IconFlags.DefaultIfFails);

			bool searchPath = 0 != (flags & GIFlags.SearchPath);

			if(0 == (flags & GIFlags.LiteralPath)) {
				//is ".ext" or "protocol:"?
				isFileType = Path_.LibIsExtension(file) || (isURL = Path_.LibIsProtocol(file));
				if(!isFileType) isURL = Path_.IsUrl(file);
				if(isFileType || isURL || (isShellPath = (file[0] == ':'))) isPath = false;
				if(isPath) {
					//get icon index from "path,index" and remove ",index"
					extractFromFile = ParseIconLocation(ref file, out index);

					if(!searchPath) {
						if(!Path_.IsFullPath(file)) file = Folders.ThisAppImages + file;
						file = Path_.LibNormalize(file, PNFlags.DontPrefixLongPath, noExpandEV: true);
					}
				}
			}

			if(isPath) {
				if(searchPath) {
					file = File_.SearchPath(file, Folders.ThisAppImages);
					if(file == null) return default; //ignore getDefaultIfFails
				}
				file = Path_.UnprefixLongPath(file);
			}

			if(isPath /*&& (extractFromFile || 0==(flags&IconFlags.Shell))*/) {
				int ext = 0;
				if(!extractFromFile && file.Length > 4) ext = file.Ends(true, ".exe", ".scr", ".ico", ".cur", ".ani");
				if(extractFromFile || ext > 0) {
					R = LoadIconHandle(file, index, size);
					if(R != default || extractFromFile) return R;
					switch(File_.ExistsAs(file, true)) {
					case FileDir.NotFound:
						return default;
					case FileDir.File:
						var siid = StockIcon.DOCNOASSOC;
						if(ext >= 1 && ext <= 2) siid = StockIcon.APPLICATION;
						return GetStockIconHandle(siid, size);
						//case FileDir.Directory: //folder name ends with .ico etc
					}
				} else if(file.Ends(".lnk", true)) {
					R = _GetLnkIcon(file, size);
					if(R != default) return R;
					//Print("_GetLnkIcon failed", file);
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
			string progId = isShellPath ? null : File_.More.GetFileTypeOrProtocolRegistryKey(file, isFileType, isURL);

			RegistryKey rk = (progId == null) ? null : Registry_.Open(progId, Registry.ClassesRoot);
			//Print(file, progId, isFileType, isURL, rk != null);

			if(rk == null) {
				//Unregistered file type/protocol, no extension, folder, ::{CLSID}, shell:AppsFolder\WinStoreAppId, or no progId key in HKCR
				//Print(@"unregistered", file, progId);
				if(progId != null) goto gr; //the file type is known, but no progid key in HKCR. Let shell API figure out. Rare.
				if(isExt || (isPath && File_.FileExists(file))) return GetStockIconHandle(StockIcon.DOCNOASSOC, size);
				goto gr;
			}

			//Registered file type/protocol.
			using(rk) {
				if(Registry_.KeyExists(@"ShellEx\IconHandler", rk)) {
					//Print(@"handler", file);
					goto gr;
				}

				string icon;
				if(Registry_.GetString(out icon, "", @"DefaultIcon", rk) && icon.Length > 0) {
					//Print("registry: DefaultIcon", file, icon);
					if(icon[0] == '@') icon = null; //eg @{Microsoft.Windows.Photos_16.622.13140.0_x64__8wekyb3d8bbwe?ms-resource://Microsoft.Windows.Photos/Files/Assets/PhotosLogoExtensions.png}
					else ParseIconLocation(ref icon, out index);
				} else if(Registry_.GetString(out icon, "", @"shell\open\command", rk) && icon.Length > 0) {
					//Print(@"registry: shell\open\command", file, icon);
					var a = icon.SegSplit((icon[0] == '\"') ? "\"" : " ", StringSplitOptions.RemoveEmptyEntries);
					icon = (a.Length == 0) ? null : a[0];
					if(icon.Ends("rundll32.exe", true)) icon = null;
				} else {
					icon = null;
					//Print("registry: no", file);
					//Usually shell API somehow gets icon.
					//For example also looks in .ext -> PerceivedType -> HKCR\SystemFileAssociations.
					//We can use AssocQueryString(ASSOCSTR_DEFAULTICON), but it is slow and not always gets correct icon.
				}

				//if(icon != null) Print(file, icon);

				if(icon == "%1") {
					//Print(file);
					if(isPath) icon = file;
					else icon = null;
				}

				if(icon != null) {
					icon = Path_.ExpandEnvVar(icon);
					if(!Path_.IsFullPath(icon)) icon = Folders.System + icon;
					R = GetFileIconHandleRaw(icon, index, size);
					if(R != default) return R;
				}
			}
			//}
			gr:
#endif
			return _GetShellIcon(!isExt, file, null, size);
		}

		//usePidl - if pidl not null/IsNull, use pidl, else convert file to PIDL. If false, pidl must be null.
		static IntPtr _GetShellIcon(bool usePidl, string file, Shell.Pidl pidl, int size, bool freePidl = false)
		{
			//info:
			//	We support everything that can have icon - path, URL, protocol (eg "http:"), file extension (eg ".txt"), shell item parsing name (eg "::{CLSID}"), "shell:AppsFolder\WinStoreAppId".
			//	We call PidlFromString here and pass it to SHGetFileInfo. It makes faster when using thread pool, because multiple threads can call PidlFromString (slow) simultaneously.
			//	PidlFromString does not support file extension. SHGetFileInfo does not support URL and protocol, unless used PIDL.
			//	SHGetFileInfo gets the most correct icons, but only of standard sizes, which also depends on DPI and don't know what.
			//	IExtractIcon for some types fails or gets wrong icon. Even cannot use it to get correct-size icons, because for most file types it uses system imagelists, which are DPI-dependent.
			//	SHMapPIDLToSystemImageListIndex+SHGetImageList also is not better.

			var pidl2 = pidl?.UnsafePtr ?? default;
			if(usePidl) {
				if(pidl2 == default) {
					pidl2 = Shell.Pidl.LibFromString(file);
					if(pidl2 == default) usePidl = false; else freePidl = true;
				}
			}

			if(!usePidl) {
				Debug.Assert(pidl2 == default && file != null);
				pidl2 = Marshal.StringToCoTaskMemUni(file);
				freePidl = true;
			}

			if(pidl2 == default) return default;

			//This is faster but fails with some files etc, randomly with others.
			//It means that shell API and/or extensions are not thread-safe, even if can run in MTA.
			//return _GetShellIcon2(pidl2, size, usePidl);

			IntPtr R = default;
			try {
				if(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
					R = _GetShellIcon2(usePidl, pidl2, size);
				} else {
					//tested: switching thread does not make slower. The speed depends mostly on locking, because then thread pool threads must wait.
					using(var work = Util.ThreadPoolSTA.CreateWork(null, o => { R = _GetShellIcon2(usePidl, pidl2, size); })) {
						work.Submit();
						work.Wait();
					}
				}
			}
			finally { if(freePidl) Marshal.FreeCoTaskMem(pidl2); }
			GC.KeepAlive(pidl);
			return R;
		}

		[HandleProcessCorruptedStateExceptions] //shell extensions may throw
		static IntPtr _GetShellIcon2(bool isPidl, IntPtr pidl, int size)
		{
			IntPtr R = default, il = default; int index = -1, ilIndex, realSize;

			if(size < (realSize = GetShellIconSize(IconSize.Small)) * 5 / 4) ilIndex = Api.SHIL_SMALL;
			else if(size < (realSize = GetShellIconSize(IconSize.Large)) * 5 / 4) ilIndex = Api.SHIL_LARGE;
			else if(size < 256) {
				ilIndex = Api.SHIL_EXTRALARGE; realSize = GetShellIconSize(IconSize.ExtraLarge);
				//info: cannot resize from 256 because GetIcon(SHIL_JUMBO) gives 48 icon if 256 icon unavailable. Getting real icon size is either impossible or quite difficult and slow (not tested).
			} else { ilIndex = Api.SHIL_JUMBO; realSize = 256; }

			//Need to lock this part, or randomly fails with some file types.
			lock("TK6Z4XiSxkGSfC14/or5Mw") {
				try {
					uint fl = Api.SHGFI_SYSICONINDEX | Api.SHGFI_SHELLICONSIZE;
					if(ilIndex == Api.SHIL_SMALL) fl |= Api.SHGFI_SMALLICON;
					if(isPidl) fl |= Api.SHGFI_PIDL; else fl |= Api.SHGFI_USEFILEATTRIBUTES;
					il = Api.SHGetFileInfo(pidl, 0, out var x, Api.SizeOf<Api.SHFILEINFO>(), fl);
					if(il != default) index = x.iIcon;
					//Marshal.Release(il); //undocumented, but without it IImageList refcount grows. Probably it's ok, because it is static, never deleted until process exits.
				}
				catch { Dbg.Print("exception"); }
			}
			if(index < 0) return default;

			//note: Getting icon from imagelist must be in STA thread too, else fails with some file types.
			//tested: This part works without locking. Using another lock here makes slower.

			try {
				if(ilIndex == Api.SHIL_SMALL || ilIndex == Api.SHIL_LARGE || _GetShellImageList(ilIndex, out il)) {
					//Print(il, Util.LibDebug_.GetComObjRefCount(il));
					R = Api.ImageList_GetIcon(il, index, 0);
					if(size != realSize && R != default) {
						//Print(size, realSize, index, file);
						R = Api.CopyImage(R, Api.IMAGE_ICON, size, size, Api.LR_COPYDELETEORG | Api.LR_COPYRETURNORG);
					}
				}
			}
			catch(Exception e) { Dbg.Print(e.Message); }
			//finally { if(il != default) Marshal.Release(il); }
			return R;
		}

		//Gets shortcut (.lnk) icon.
		//Much faster than other shell API.
		//Also gets correct icon where iextracticon fails and/or shgetfileinfo gets blank document icon, don't know why.
		//Usually fails only when target does not exist. Then iextracticon also fails, and shgetfileinfo gets blank document icon.
		//If fails, returns default(IntPtr). No exceptions.
		static IntPtr _GetLnkIcon(string file, int size)
		{
			try {
				using(var x = Shell.Shortcut.Open(file)) {
					var s = x.GetIconLocation(out int ii); if(s != null) return LoadIconHandle(s, ii, size);
					s = x.TargetPathRawMSI; if(s != null) return GetFileIconHandle(s, size);
					//Print("need IDList", file);
					using(var pidl = x.TargetPidl) return GetPidlIconHandle(pidl, size);
				}
			}
			catch { return default; }
		}

		//If size is in enum IconSize range (<=0), calls GetShellIconSize. If size is invalid, uses nearest valid value.
		static int _NormalizeIconSizeParameter(int size)
		{
			if(size > 256) size = 256; else if(size < (int)IconSize.Jumbo) size = (int)IconSize.SysSmall;
			if(size > 0) return size;
			return GetShellIconSize((IconSize)size);
		}

		/// <summary>
		/// Gets the width and height of shell icons of standard sizes - small, large, extra large and jumbo.
		/// Jumbo is always 256. Others depend on text size (DPI) that can be changed in Windows Settings. If text size is 100%, they usually are 16, 32, 48.
		/// </summary>
		public static int GetShellIconSize(IconSize size)
		{
			switch(size) {
			case IconSize.Small:
				if(_shellIconSizeSmall == 0) _shellIconSizeSmall = _GetShellIconSize(Api.SHIL_SMALL, 16);
				return _shellIconSizeSmall;
			case IconSize.Large:
				if(_shellIconSizeLarge == 0) _shellIconSizeLarge = _GetShellIconSize(Api.SHIL_LARGE, 32);
				return _shellIconSizeLarge;
			case IconSize.ExtraLarge:
				if(_shellIconSizeExtraLarge == 0) _shellIconSizeExtraLarge = _GetShellIconSize(Api.SHIL_EXTRALARGE, 48);
				return _shellIconSizeExtraLarge;
			case IconSize.Jumbo: return 256;
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
				if(0 == Api.SHGetImageList(ilIndex, Api.IID_IImageList, out R)) return true;
			}
			Debug.Assert(false);
			return false;
		}

		/// <summary>
		/// Extracts icon from file that contains it.
		/// </summary>
		/// <returns>Returns null if failed.</returns>
		/// <param name="file">.ico, .exe, .dll or other file that contains one or more icons. Also supports cursor files - .cur, .ani. Must be full path, without icon index. Supports environment variables (see <see cref="Path_.ExpandEnvVar"/>).</param>
		/// <param name="index">Icon index or negative icon resource id in the .exe/.dll file.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="IconSize"/>, cast to int.</param>
		public static unsafe Icon LoadIcon(string file, int index = 0, int size = 16)
			=> HandleToIcon(LoadIconHandle(file, index, size), true);

		/// <summary>
		/// Extracts icon from file that contains it.
		/// More info: <see cref="LoadIcon"/>.
		/// </summary>
		/// <returns>Returns icon handle, or default(IntPtr) if failed. Later call <see cref="DestroyIconHandle"/> or some <b>HandleToX</b> function that will destroy it.</returns>
		public static unsafe IntPtr LoadIconHandle(string file, int index = 0, int size = 16)
		{
			if(Empty(file)) return default;
			size = _NormalizeIconSizeParameter(size);

			//We use SHDefExtractIcon because of better quality resizing (although several times slower) which matches shell and imagelist resizing.
			//With .ico it matches LoadImage speed (without resizing). PrivateExtractIcons is slightly slower.

			IntPtr R = default;
			int hr = Api.SHDefExtractIcon(file, index, 0, &R, null, size);
			if(hr != 0) return default;
			return R;

			//if(Api.PrivateExtractIcons(file, index, size, size, out R, default, 1, 0) != 1) return default;
		}

		/// <summary>
		/// Gets a shell stock icon.
		/// </summary>
		/// <returns>Returns null if failed.</returns>
		/// <param name="icon">Shell stock icon id.</param>
		/// <param name="size">Icon width and height. Also can be enum <see cref="IconSize"/>, cast to int.</param>
		public static Icon GetStockIcon(StockIcon icon, int size)
			=> HandleToIcon(GetStockIconHandle(icon, size), true);

		/// <summary>
		/// Gets a shell stock icon.
		/// More info: <see cref="GetStockIcon"/>.
		/// </summary>
		/// <returns>Returns icon handle, or default(IntPtr) if failed. Later call <see cref="DestroyIconHandle"/> or some <b>HandleToX</b> function that will destroy it.</returns>
		public static unsafe IntPtr GetStockIconHandle(StockIcon icon, int size)
		{
			var x = new Api.SHSTOCKICONINFO(); x.cbSize = Api.SizeOf(x);
			if(0 != Api.SHGetStockIconInfo(icon, 0, ref x)) return default;
			var s = new string(x.szPath);
			return LoadIconHandle(s, x.iIcon, size);
			//note: don't cache, because of the limit of handles a process can have. Maybe only exe and document icons; maybe also folder and open folder.

			//tested: always gets 32x32 icon: Api.LoadImage(default, 32516, Api.IMAGE_ICON, 16, 16, Api.LR_SHARED); //OIC_INFORMATION
		}

		/// <summary>
		/// Gets <msdn>IDI_APPLICATION</msdn> icon from unmanaged resources of this program file.
		/// </summary>
		/// <returns>Returns null if there are no icons.</returns>
		/// <param name="size">Icon width and height. Also can be enum <see cref="IconSize"/>, cast to int.</param>
		public static Icon GetAppIcon(int size)
			=> HandleToIcon(GetAppIconHandle(size), false);

		/// <summary>
		/// Gets <msdn>IDI_APPLICATION</msdn> icon from unmanaged resources of this program file.
		/// More info: <see cref="GetAppIcon"/>.
		/// </summary>
		/// <returns>Returns native icon handle, or default(IntPtr) if there are no icons.</returns>
		/// <remarks>The icon is cached and protected from destroying, therefore don't need to destroy it, and not error to do it.</remarks>
		internal static IntPtr GetAppIconHandle(int size)
		{
			//var h = Util.ModuleHandle_.OfAppIcon();
			var h = Util.ModuleHandle_.OfProcessExe();
			if(h == default) return default;
			size = _NormalizeIconSizeParameter(size);
			return Api.LoadImage(h, Api.IDI_APPLICATION, Api.IMAGE_ICON, size, size, Api.LR_SHARED);

			//This is not 100% reliable because the icon id 32512 (IDI_APPLICATION) is undocumented.
			//I could not find a .NET method to get icon directly from native resources of assembly.
			//Could use the resource emumeration API...
			//info: MSDN lies that with LR_SHARED gets a cached icon regardless of size argument. Caches each size separately. Tested on Win 10, 7, XP.
		}

		/// <summary>
		/// Creates icon at run time.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="drawCallback">Called to draw icon. If null, the icon will be completely transparent.</param>
		public static Icon CreateIcon(int width, int height, Action<Graphics> drawCallback = null)
			=> HandleToIcon(CreateIconHandle(width, height, drawCallback), true);

		/// <summary>
		/// Creates icon at run time.
		/// More info: <see cref="CreateIcon"/>.
		/// </summary>
		/// <returns>Returns native icon handle. Later call <see cref="DestroyIconHandle"/> or some <b>HandleToX</b> function that will destroy it.</returns>
		public static IntPtr CreateIconHandle(int width, int height, Action<Graphics> drawCallback = null)
		{
			if(drawCallback != null) {
				using(var b = new Bitmap(width, height)) {
					using(var g = Graphics.FromImage(b)) {
						g.Clear(Color.Transparent); //optional, new bitmaps are transparent, but it is undocumented, and eg .NET Bitmap.MakeTransparent does it
						drawCallback(g);
						return b.GetHicon();
					}
				}
			} else {
				int nb = Math_.AlignUp(width, 32) / 8 * height;
				var aAnd = new byte[nb]; for(int i = 0; i < nb; i++) aAnd[i] = 0xff;
				var aXor = new byte[nb];
				return Api.CreateIcon(default, width, height, 1, 1, aAnd, aXor);

				//speed: ~20 mcs. About 10 times faster than above. Faster than CopyImage etc.
			}
		}

		/// <summary>
		/// Destroys native icon.
		/// Calls API <msdn>DestroyIcon</msdn>. Does nothing if iconHandle is default(IntPtr).
		/// </summary>
		public static void DestroyIconHandle(IntPtr iconHandle)
		{
			if(iconHandle != default) Api.DestroyIcon(iconHandle);
		}

		/// <summary>
		/// Converts unmanaged icon to Icon object.
		/// Returns null if hIcon is default(IntPtr).
		/// </summary>
		/// <param name="hIcon">Icon handle.</param>
		/// <param name="destroyIcon">If true (default), the returned variable owns the unmanaged icon and destroys it when disposing. If false, the returned variable just uses the unmanaged icon and will not destroy; the caller later should destroy it with <see cref="DestroyIconHandle"/>.</param>
		public static Icon HandleToIcon(IntPtr hIcon, bool destroyIcon = true)
		{
			if(hIcon == default) return null;
			var R = Icon.FromHandle(hIcon);
			if(destroyIcon) {
				var fi = typeof(Icon).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
				Debug.Assert(fi != null);
				fi?.SetValue(R, true);

				//Don't allow to exceed the process handle limit when the program does not dispose them.
				//Default limit for USER and GDI objects is 10000, min 200.
				//Icons are USER objects. Most icons also create 3 GDI objects, some 2. So a process can have max 3333 icons.
				//If GC starts working when pressure is 100 KB, then the number of icons is ~50 and GDI handles ~150.
				//We don't care about icon memory size.
				Util.GC_.AddObjectMemoryPressure(R, 2000);
			}
			return R;
		}

		/// <summary>
		/// Converts unmanaged icon to Bitmap object and destroys the unmanaged icon.
		/// Returns null if hIcon is default(IntPtr) or if fails to convert.
		/// </summary>
		/// <param name="hIcon">Icon handle.</param>
		/// <param name="destroyIcon">If true (default), destroys the unmanaged icon.</param>
		public static Bitmap HandleToImage(IntPtr hIcon, bool destroyIcon = true)
		{
			//note: don't use Bitmap.FromHicon. It just calls GdipCreateBitmapFromHICON which does not support alpha etc.

			if(hIcon == default) return null;
			//var perf = Perf.StartNew();
			Icon ic = Icon.FromHandle(hIcon);
			Bitmap im = null;
			try { im = ic.ToBitmap(); } catch(Exception e) { Dbg.Print(e.Message); }
			ic.Dispose();
			if(destroyIcon) Api.DestroyIcon(hIcon);
			//perf.NW();
			return im;
		}

		/// <summary>
		/// Parses icon location string.
		/// Returns true if it includes icon index or resource id.
		/// </summary>
		/// <param name="s">Icon location. Can be <c>"path,index"</c> or <c>"path,-id"</c> or just path. Receives path.</param>
		/// <param name="index">Receives the number or 0.</param>
		/// <remarks>Also supports path enclosed in double quotes like <c>"\"path\",index"</c>, and spaces between comma and index like <c>"path, index"</c>.</remarks>
		public static bool ParseIconLocation(ref string s, out int index)
		{
			index = 0;
			if(Empty(s)) return false;
			if(s[0] == '\"') s = s.Replace("\"", ""); //can be eg "path",index
			if(s.Length < 3) return false;
			if(!Char_.IsAsciiDigit(s[s.Length - 1])) return false;
			int i = s.LastIndexOf(','); if(i < 1) return false;
			index = s.ToInt(i + 1, out int e); if(e != s.Length) return false;
			s = s.Remove(i);
			return true;

			//note: API PathParseIconLocation has bugs. Eg splits "path,5moreText". Or from "path, 5" removes ", 5" and returns 0.
		}

		/// <summary>
		/// Gets icons of files etc as Bitmap. Uses 2-level cache - memory and file.
		/// </summary>
		/// <threadsafety static="true" instance="true"/>
		/// <seealso cref="Util.IconsAsync"/>
		public sealed class ImageCache :IDisposable
		{
			XElement _x;
			Hashtable _table;
			string _cacheFile;
			int _iconSize;
			bool _dirty;

			///
			public ImageCache(string cacheFile, int iconSize)
			{
				_cacheFile = cacheFile;
				_iconSize = iconSize;
			}

			///
			~ImageCache() { SaveCacheFileNow(); }

			/// <summary>
			/// Calls <see cref="SaveCacheFileNow"/>.
			/// </summary>
			public void Dispose()
			{
				SaveCacheFileNow();
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Saves to the cache file now, if need.
			/// Don't need to call this explicitly. It is called by Dispose.
			/// </summary>
			public void SaveCacheFileNow()
			{
				if(_dirty) {
					lock(this) {
						if(_dirty) {
							_dirty = false;
							File_.WaitIfLocked(() => _x.Save(_cacheFile));
						}
					}
				}
			}

			/// <summary>
			/// Clears the memory cache and deletes the cache file.
			/// </summary>
			public void ClearCache()
			{
				lock(this) {
					_dirty = false;
					_x = null;
					_table = null;
					File_.Delete(_cacheFile);
				}
			}

			/// <summary>
			/// Gets file icon as <b>Bitmap</b>.
			/// Returns null if the icon is not cached and failed to get it, eg file does not exist.
			/// </summary>
			/// <param name="file">Any file or folder.</param>
			/// <param name="useExt">
			/// Get file type icon, depending on filename extension. Use this to avoid getting separate image object for each file of same type.
			/// This is ignored if filename extension is ".ico" or ".exe" or starts with ".exe," or ".dll,".
			/// </param>
			/// <param name="giFlags">Flags for <see cref="GetFileIconImage"/>.</param>
			/// <param name="autoUpdate">
			/// If not null, the cached image will be auto-updated when changed. Then will be called this function. It can update the image in UI.
			/// How it works: If this function finds cached image, it sets timer that after ~50 ms loads that icon/image from file again and compares with the cached image. If different, updates the cache. Does it once, not periodically.
			/// Use only in UI threads. Does not work if this thread does not retrieve/dispatch posted messages.
			/// </param>
			/// <param name="auParam">Something to pass to the <i>autoUpdate</i> callback function.</param>
			/// <remarks>
			/// If the icon is in the memory cache, gets it from there.
			/// Else if it is in the file cache, gets it from there and adds to the memory cache.
			/// Else gets from file (uses <see cref="GetFileIconImage"/> and adds to the file cache and to the memory cache.
			/// </remarks>
			public Bitmap GetImage(string file, bool useExt, GIFlags giFlags = 0, Action<Bitmap, object> autoUpdate = null, object auParam = null)
			{
				if(useExt) {
					var ext = Path_.GetExtension(file);
					if(ext.Length == 0) {
						if(File_.ExistsAsDirectory(file)) ext = file;
						else ext = ".no-ext";
					} else {
						//ext = ext.Lower();
						if(ext.Eqi(".ico") || ext.Eqi(".exe") || ext.Starts(".exe,", true) || ext.Starts(".dll,", true)) ext = file;
					}
					file = ext;
				} else if(Path_.IsFullPathExpandEnvVar(ref file)) {
					file = Path_.LibNormalize(file, noExpandEV: true);
				}

				return _GetImage(file, giFlags, null, autoUpdate, auParam, true);
			}

			/// <summary>
			/// Gets any icon or image using callback function.
			/// Returns null if the icon is not cached and the callback function returns null.
			/// </summary>
			/// <param name="name">Some unique name. It is used to identify this image in cache.</param>
			/// <param name="callback">Called to get image. To convert icon handle to image, use <see cref="Icon_.HandleToImage"/>.</param>
			/// <param name="autoUpdate"></param>
			/// <param name="auParam"></param>
			/// <param name="auDispose">If true (default), auto-updating can dispose unused image returned by <i>callback</i>.</param>
			/// <remarks>
			/// If the icon is in the memory cache, gets it from there.
			/// Else if it is in the file cache, gets it from there and adds to the memory cache.
			/// Else calls callback function, which should return image, and adds to the file cache and to the memory cache.
			/// </remarks>
			public Bitmap GetImage(string name, Func<Bitmap> callback, Action<Bitmap, object> autoUpdate = null, object auParam = null, bool auDispose = true)
			{
				return _GetImage(name, 0, callback, autoUpdate, auParam, auDispose);
			}

			Bitmap _GetImage(string file, GIFlags giFlags, Func<Bitmap> callback, Action<Bitmap, object> autoUpdate, object auParam, bool auDispose)
			{
				bool cached = true;
				lock(this) {
					Bitmap R = null;

					//is in memory cache?
					if(_table == null) _table = new Hashtable(StringComparer.OrdinalIgnoreCase);
					else R = _table[file] as Bitmap;

					if(R == null) {
						//is in file cache?
						try {
							if(_x == null && File_.ExistsAsFile(_cacheFile)) {
								_x = ExtXml.LoadElement(_cacheFile);
								if(_iconSize != _x.Attribute_("size", 0) || Util.Dpi.BaseDPI != _x.Attribute_("dpi", 0)) {
									_x = null;
									Dbg.Print("info: cleared icon cache");
								}

								//FUTURE: Delete unused entries. Maybe try to auto-update changed icons.
								//	Not very important, because there is ClearCache.
							}
							if(_x != null) {
								var x = _x.Element_("i", "name", file, true);
								if(x != null) {
									using(var ms = new MemoryStream(Convert_.Base64Decode(x.Value), false)) {
										R = new Bitmap(ms);
									}
								}
							}
						}
						catch(Exception ex) {
							Dbg.Print(ex.Message);
						}

						if(R != null) {
							_table[file] = R; //add to memory cache
						} else if(_LoadImage(out R, file, giFlags, callback)) { //get file icon
							_AddImage(file, R, false); //add to file cache and memory cache
							cached = false;
						}
					}

					//auto-update
					if(cached && autoUpdate != null) {
						var d = new _AUData() {
							cache = this, oldImage = R, file = file, callback = callback,
							autoUpdated = autoUpdate, auParam = auParam, giFlags = giFlags, canDispose = auDispose,
						};
						_AutoUpdateAdd(d);
					}

					return R;
				}
			}

			bool _LoadImage(out Bitmap b, string file, GIFlags giFlags, Func<Bitmap> callback)
			{
				if(callback != null) b = callback();
				else b = GetFileIconImage(file, _iconSize, giFlags);
				return b != null;
			}

			void _AddImage(string file, Bitmap b, bool replace)
			{
				using(var ms = new MemoryStream()) {
					b.Save(ms, ImageFormat.Png);
					var s = Convert.ToBase64String(ms.ToArray());
					XElement d = null;
					if(replace) {
						d = _x?.Element_("i", "name", file, true);
						Debug.Assert(d != null); //possible, eg if called ClearCache after GetImage
						if(d != null) d.Value = s;
					}
					if(d == null) {
						d = new XElement("i", s);
						d.SetAttributeValue("name", file);
						if(_x == null) {
							_x = new XElement("images");
							_x.SetAttributeValue("size", _iconSize);
							_x.SetAttributeValue("dpi", Util.Dpi.BaseDPI);
						}
						_x.Add(d);
					}
				}
				_dirty = true;
				_table[file] = b; //add to memory cache
			}

			#region auto update

			[ThreadStatic] static Queue<_AUData> t_auList;
			[ThreadStatic] static Timer_ t_auTimer;

			static void _AutoUpdateAdd(_AUData d)
			{
				if(t_auTimer == null) {
					t_auTimer = new Timer_(t => _AutoUpdateTimer());
					t_auList = new Queue<_AUData>();
				}
				t_auList.Enqueue(d);
				t_auTimer.Start(50, false);
			}

			static void _AutoUpdateTimer()
			{
				var t = Time.PerfMilliseconds;
				while(t_auList.Count > 0) {
					if(Time.PerfMilliseconds - t > 20) return; //don't block UI thread
					var k = t_auList.Dequeue();
					k.cache._AutoUpdateSingle(k);
				}
				t_auTimer.Stop();
			}

			void _AutoUpdateSingle(_AUData k)
			{
				//FUTURE: optimize.
				//	Don't load ico/exe file if not modified.
				//	Don't do too frequently for same file.
				//	Skip duplicate paths.
				//	Don't do for files where getting icon is very slow.

				if(!_LoadImage(out var b, k.file, k.giFlags, k.callback)) return;
				if(b == k.oldImage) return; //callback returned the same object
				if(_CompareImages(b, k.oldImage)) {
					if(k.canDispose) b.Dispose();
					return;
				}
				lock(this) _AddImage(k.file, b, true);
				//k.autoUpdated?.Invoke(b, k.auParam);

				//Print(k.auParam);
				//Perf.First();
				k.autoUpdated?.Invoke(b, k.auParam);
				//Perf.NW();
			}

			class _AUData
			{
				public ImageCache cache;
				public Bitmap oldImage;
				public string file;
				public Func<Bitmap> callback;
				public Action<Bitmap, object> autoUpdated;
				public object auParam;
				public GIFlags giFlags;
				public bool canDispose;
			}

			static unsafe bool _CompareImages(Bitmap b1, Bitmap b2)
			{
				var pf = b1.PixelFormat;
				if(b1.Size != b2.Size || pf != b2.PixelFormat) return false;
				var r = new Rectangle(0, 0, b1.Width, b1.Height);
				var d1 = b1.LockBits(r, ImageLockMode.ReadOnly, pf);
				var d2 = b2.LockBits(r, ImageLockMode.ReadOnly, pf);
				try {
					if(d1.Stride != d2.Stride) return false;
					int n = Math.Abs(d1.Stride) * d1.Height;
					return 0 == Api.memcmp((void*)d1.Scan0, (void*)d2.Scan0, n);
				}
				finally {
					b1.UnlockBits(d1);
					b2.UnlockBits(d2);
				}
			}

			#endregion
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="Icon_.GetFileIcon"/> and similar functions.
	/// </summary>
	[Flags]
	public enum GIFlags
	{
		/// <summary>
		/// The 'file' argument is literal full path. Don't parse "path,index", don't support ".ext" (file type icon), don't make fully-qualified, etc.
		/// </summary>
		LiteralPath = 1,

		/// <summary>
		/// If file is not full path, call <see cref="File_.SearchPath"/>.
		/// Without this flag searches only in <see cref="Folders.ThisAppImages"/>; with this flag also searches there first.
		/// </summary>
		SearchPath = 2,

#if false
		/// <summary>
		/// Scale the specified size according to DPI (text size) specified in Windows Settings.
		/// </summary>
		DpiScale = 4, //rejected. In most cases can use standard-size icons from enum IconSize, they are DPI-scaled. Or pass size * Util.Dpi.BaseDPI.

		/// Use shell API for all file types, including exe and ico.
		Shell=8, //rejected because SHGetFileInfo does not get exe icon with shield overlay

		/// <summary>
		/// If file does not exist or fails to get its icon, get common icon for that file type, or default document icon if cannot get common icon.
		/// </summary>
		DefaultIfFails = 16, //rejected. Now for exe/ico/etc is like with shell API: if file exists, gets default icon (exe or document), else returns default(IntPtr).

		/// <summary>
		/// Used only with AsyncIcons class. If the thread pool has spare time, let it convert icon handle to Image object. The callback will receive either handle or Image, it must check both for default(IntPtr) and null. This is to make whole process as fast as possible.
		/// </summary>
		NeedImage = 128, //rejected because with our menu/toolbar almost always makes slower
#endif
	}

	/// <summary>
	/// Standard icon sizes.
	/// </summary>
	/// <seealso cref="Icon_.GetShellIconSize"/>
	public enum IconSize //note: this is not in Au.Types, because then difficult to find, because often need to cast to int
	{
		/// <summary>
		/// Icons displayed in window title bar and system notification area (tray). Usually 16x16 when normal DPI, the same as <b>Small</b>.
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

#pragma warning disable 1591 //missing XML documentation
	/// <summary>See <msdn>SHSTOCKICONID</msdn>.</summary>
	/// <seealso cref="Icon_.GetStockIcon"/>
	public enum StockIcon
	{
		DOCNOASSOC,
		DOCASSOC,
		APPLICATION,
		FOLDER,
		FOLDEROPEN,
		DRIVE525,
		DRIVE35,
		DRIVEREMOVE,
		DRIVEFIXED,
		DRIVENET,
		DRIVENETDISABLED,
		DRIVECD,
		DRIVERAM,
		WORLD,
		SERVER = 15,
		PRINTER,
		MYNETWORK,
		FIND = 22,
		HELP,
		SHARE = 28,
		LINK,
		SLOWFILE,
		RECYCLER,
		RECYCLERFULL,
		MEDIACDAUDIO = 40,
		LOCK = 47,
		AUTOLIST = 49,
		PRINTERNET,
		SERVERSHARE,
		PRINTERFAX,
		PRINTERFAXNET,
		PRINTERFILE,
		STACK,
		MEDIASVCD,
		STUFFEDFOLDER,
		DRIVEUNKNOWN,
		DRIVEDVD,
		MEDIADVD,
		MEDIADVDRAM,
		MEDIADVDRW,
		MEDIADVDR,
		MEDIADVDROM,
		MEDIACDAUDIOPLUS,
		MEDIACDRW,
		MEDIACDR,
		MEDIACDBURN,
		MEDIABLANKCD,
		MEDIACDROM,
		AUDIOFILES,
		IMAGEFILES,
		VIDEOFILES,
		MIXEDFILES,
		FOLDERBACK,
		FOLDERFRONT,
		SHIELD,
		WARNING,
		INFO,
		ERROR,
		KEY,
		SOFTWARE,
		RENAME,
		DELETE,
		MEDIAAUDIODVD,
		MEDIAMOVIEDVD,
		MEDIAENHANCEDCD,
		MEDIAENHANCEDDVD,
		MEDIAHDDVD,
		MEDIABLURAY,
		MEDIAVCD,
		MEDIADVDPLUSR,
		MEDIADVDPLUSRW,
		DESKTOPPC,
		MOBILEPC,
		USERS,
		MEDIASMARTMEDIA,
		MEDIACOMPACTFLASH,
		DEVICECELLPHONE,
		DEVICECAMERA,
		DEVICEVIDEOCAMERA,
		DEVICEAUDIOPLAYER,
		NETWORKCONNECT,
		INTERNET,
		ZIPFILE,
		SETTINGS,
		DRIVEHDDVD = 132,
		DRIVEBD,
		MEDIAHDDVDROM,
		MEDIAHDDVDR,
		MEDIAHDDVDRAM,
		MEDIABDROM,
		MEDIABDR,
		MEDIABDRE,
		CLUSTEREDDRIVE,
		MAX_ICONS = 181
	}
#pragma warning restore 1591


	//public struct Icon_
	//{
	//	public object Value { get; }

	//	Icon_(object o) { Value = o; }

	//	public static implicit operator Icon_(Icon icon) => new Icon_(icon);
	//	public static implicit operator Icon_(Image image) => new Icon_(image);
	//	public static implicit operator Icon_(string file) => new Icon_(file);
	//	public static implicit operator Icon_((string file, GIFlags flags) t) => new Icon_(t);
	//	public static implicit operator Icon_((string file, int index) t) => new Icon_(t);
	//	public static implicit operator Icon_(Shell.Pidl pidl) => new Icon_(pidl);
	//	public static implicit operator Icon_(StockIcon icon) => new Icon_(icon);

	//	public Icon GetIcon(int size)
	//	{
	//		if(GetIconOrImage(size) is Bitmap b) return Icon_.HandleToIcon(b.GetHicon());
	//		return null;
	//	}

	//	public object GetIconOrImage(int size)
	//	{
	//		switch(Value) {
	//		case Icon k: return k;
	//		case Image k: return k;
	//		case string k: return Icon_.GetFileIcon(k, size);
	//		case ValueTuple<string, GIFlags> k: return Icon_.GetFileIcon(k.Item1, size, k.Item2);
	//		case ValueTuple<string, int> k: return Icon_.LoadIcon(k.Item1, k.Item2, size);
	//		case Shell.Pidl k: return Icon_.GetPidlIcon(k, size);
	//		case StockIcon k: return Icon_.GetStockIcon(k, size);
	//		}
	//		return null;
	//	}
	//}
}
