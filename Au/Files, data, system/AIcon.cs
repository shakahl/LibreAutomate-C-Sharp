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
using System.Reflection.Emit;
using System.Drawing;
//using System.Linq;

using Au.Types;
using Au.Util;

//TODO: static option to measure and print time. Need to debug some slow icons.

namespace Au
{
	/// <summary>
	/// Gets icons for files etc. Contains native icon handle.
	/// </summary>
	/// <remarks>
	/// Native icons must be destroyed, it is very important. An <b>AIcon</b> variable destroys its native icon when disposing it. To dispose, call <b>Dispose</b> or use <b>using</b> statement. Note: it's not a reference type and does not have a finalizer that could be called by GC. But usually you need not native handle but <b>Icon</b>, <b>Bitmap</b> or <b>ImageSource</b>. Then use <see cref="ToGdipIcon"/>, <see cref="ToGdipBitmap"/> or <see cref="ToWpfImage"/>. By default they dispose the native icon and clear the <b>AIcon</b> variable; then don't need to dispose it.
	/// </remarks>
	public struct AIcon : IDisposable
	{
		IntPtr _handle;

		/// <summary>
		/// Sets native icon handle.
		/// The icon will be destroyed when disposing this variable or when converting to object of other type.
		/// </summary>
		public AIcon(IntPtr hicon) { _handle = hicon; }

		/// <summary>
		/// Destroys native icon handle.
		/// </summary>
		public void Dispose() {
			if (_handle != default) { Api.DestroyIcon(_handle); _handle = default; }
		}

		/// <summary>
		/// Returns true if <b>Handle</b>==default(IntPtr).
		/// </summary>
		public bool Is0 => _handle == default;

		/// <summary>
		/// Gets native icon handle.
		/// </summary>
		public IntPtr Handle => _handle;

		/// <summary>
		/// Gets native icon handle.
		/// </summary>
		public static implicit operator IntPtr(AIcon icon) => icon._handle;

		/// <summary>
		/// Gets file icon.
		/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, etc.
		/// </summary>
		/// <returns>Returns default(AIcon) if failed.</returns>
		/// <param name="file">
		/// Can be:
		/// - Path of any file or folder. Supports environment variables.
		/// - Any shell object, like <c>":: ITEMIDLIST"</c>, <c>@"::{CLSID-1}\::{CLSID-2}"</c>, <c>@"shell:AppsFolder\WinStoreAppId"</c>.
		/// - File type like <c>".txt"</c>, or protocol like <c>"http:"</c>. Use <c>"."</c> to get forder icon.
		/// - Path with icon resource index or negative id, like "c:\file.dll,4", "c:\file.exe,-4".
		/// - URL.
		/// </param>
		/// <param name="size">Icon width and height. Default 16.</param>
		/// <param name="flags"></param>
		/// <remarks>
		/// If not full path, uses <see cref="AFolders.ThisAppImages"/> and <see cref="AFile.SearchPath"/>.
		/// 
		/// ITEMIDLIST can be of any file, folder, URL or a non-filesystem shell object. See <see cref="APidl.ToHexString"/>.
		/// </remarks>
		public static AIcon OfFile(string file, int size = 16, IconGetFlags flags = 0) {
			using var ds = new _DebugSpeed(file);
			return _OfFile(file, _NormalizeIconSizeArgument(size), flags);
		}

		static AIcon _OfFile(string file, int size = 16, IconGetFlags flags = 0) {
			if (file.NE()) return default;
			file = APath.ExpandEnvVar(file);
			return new AIcon(_GetFileIcon(file, size, flags));
		}

		/// <summary>
		/// Gets icon of a file or other shell object specified as ITEMIDLIST.
		/// </summary>
		/// <returns>Returns default(AIcon) if failed.</returns>
		/// <param name="pidl">ITEMIDLIST pointer (PIDL).</param>
		/// <param name="size">Icon width and height. Default 16.</param>
		public static AIcon OfPidl(APidl pidl, int size = 16) {
			using var ds = new _DebugSpeed(pidl);
			return _OfPidl(pidl, _NormalizeIconSizeArgument(size));
		}

		static AIcon _OfPidl(APidl pidl, int size) {
			if (pidl?.IsNull ?? false) return default;
			return new AIcon(_GetShellIcon(true, null, pidl, size));
		}

		static IntPtr _GetFileIcon(string file, int size, IconGetFlags flags) {
			IntPtr R = default;
			int index = 0;
			bool extractFromFile = false, isFileType = false, isURL = false, isShellPath = false, isPath = true;
			//bool getDefaultIfFails = 0!=(flags&IconGetFlags.DefaultIfFails);

			bool searchPath = 0 == (flags & IconGetFlags.DontSearch);

			if (0 == (flags & IconGetFlags.LiteralPath)) {
				//is ".ext" or "protocol:"?
				isFileType = APath.IsExtension_(file) || (isURL = APath.IsProtocol_(file));
				if (!isFileType) isURL = APath.IsUrl(file);
				if (isFileType || isURL || (isShellPath = (file[0] == ':'))) isPath = false;
				if (isPath) {
					//get icon index from "path,index" and remove ",index"
					extractFromFile = ParsePathIndex(file, out file, out index);

					if (!searchPath) {
						if (!APath.IsFullPath(file)) file = AFolders.ThisAppImages + file;
						file = APath.Normalize_(file, PNFlags.DontPrefixLongPath, noExpandEV: true);
					}
				}
			}

			if (isPath) {
				if (searchPath) {
					file = AFile.SearchPath(file, AFolders.ThisAppImages);
					if (file == null) return default; //ignore getDefaultIfFails
				}
				file = APath.UnprefixLongPath(file);
			}

			if (isPath /*&& (extractFromFile || 0==(flags&IconGetFlags.Shell))*/) {
				int ext = 0;
				if (!extractFromFile && file.Length > 4) ext = file.Ends(true, ".ico", ".exe", ".scr"/*, ".cur", ".ani"*/);
				if (extractFromFile || ext > 0) {
					R = _Load(file, index, size);
					if (R != default || extractFromFile) return R;
					switch (AFile.ExistsAs(file, true)) {
					case FileDir.NotFound:
						return default;
					case FileDir.File:
						return Stock(ext == 2 || ext == 3 ? StockIcon.APPLICATION : StockIcon.DOCNOASSOC, size);
						//case FileDir.Directory: //folder name ends with .ico etc
					}
				} else if (file.Ends(".lnk", true)) {
					R = _GetLnkIcon(file, size);
					if (R != default) return R;
					//AOutput.Write("_GetLnkIcon failed", file);
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
			//if(0==(flags&IconGetFlags.Shell)){
			string progId = isShellPath ? null : AFile.More.GetFileTypeOrProtocolRegistryKey(file, isFileType, isURL);

			RegistryKey rk = (progId == null) ? null : ARegistry.Open(progId, Registry.ClassesRoot);
			//AOutput.Write(file, progId, isFileType, isURL, rk != null);

			if(rk == null) {
				//Unregistered file type/protocol, no extension, folder, ::{CLSID}, shell:AppsFolder\WinStoreAppId, or no progId key in HKCR
				//AOutput.Write(@"unregistered", file, progId);
				if(progId != null) goto gr; //the file type is known, but no progid key in HKCR. Let shell API figure out. Rare.
				if(isExt || (isPath && AFile.FileExists(file))) return GetStockIconHandle(StockIcon.DOCNOASSOC, size);
				goto gr;
			}

			//Registered file type/protocol.
			using(rk) {
				if(ARegistry.KeyExists(@"ShellEx\IconHandler", rk)) {
					//AOutput.Write(@"handler", file);
					goto gr;
				}

				string icon;
				if(ARegistry.GetString(out icon, "", @"DefaultIcon", rk) && icon.Length > 0) {
					//AOutput.Write("registry: DefaultIcon", file, icon);
					if(icon[0] == '@') icon = null; //eg @{Microsoft.Windows.Photos_16.622.13140.0_x64__8wekyb3d8bbwe?ms-resource://Microsoft.Windows.Photos/Files/Assets/PhotosLogoExtensions.png}
					else ParseIconLocation(ref icon, out index);
				} else if(ARegistry.GetString(out icon, "", @"shell\open\command", rk) && icon.Length > 0) {
					//AOutput.Write(@"registry: shell\open\command", file, icon);
					var a = icon.SegSplit((icon[0] == '\"') ? "\"" : " ", StringSplitOptions.RemoveEmptyEntries);
					icon = (a.Length == 0) ? null : a[0];
					if(icon.Ends("rundll32.exe", true)) icon = null;
				} else {
					icon = null;
					//AOutput.Write("registry: no", file);
					//Usually shell API somehow gets icon.
					//For example also looks in .ext -> PerceivedType -> HKCR\SystemFileAssociations.
					//We can use AssocQueryString(ASSOCSTR_DEFAULTICON), but it is slow and not always gets correct icon.
				}

				//if(icon != null) AOutput.Write(file, icon);

				if(icon == "%1") {
					//AOutput.Write(file);
					if(isPath) icon = file;
					else icon = null;
				}

				if(icon != null) {
					icon = APath.ExpandEnvVar(icon);
					if(!APath.IsFullPath(icon)) icon = AFolders.System + icon;
					R = LoadIconHandle(icon, index, size);
					if(R != default) return R;
				}
			}
			//}
			gr:
#endif
			return _GetShellIcon(!isExt, file, null, size);
		}

		//usePidl - if pidl not null/IsNull, use pidl, else convert file to PIDL. If false, pidl must be null.
		static IntPtr _GetShellIcon(bool usePidl, string file, APidl pidl, int size, bool freePidl = false) {
			//info:
			//	We support everything that can have icon - path, URL, protocol (eg "http:"), file extension (eg ".txt"), shell item parsing name (eg "::{CLSID}"), "shell:AppsFolder\WinStoreAppId".
			//	We call PidlFromString here and pass it to SHGetFileInfo. It makes faster when using thread pool, because multiple threads can call PidlFromString (slow) simultaneously.
			//	PidlFromString does not support file extension. SHGetFileInfo does not support URL and protocol, unless used PIDL.
			//	SHGetFileInfo gets the most correct icons, but only of standard sizes, which also depends on DPI and don't know what.
			//	IExtractIcon for some types fails or gets wrong icon. Even cannot use it to get correct-size icons, because for most file types it uses system imagelists, which are DPI-dependent.
			//	SHMapPIDLToSystemImageListIndex+SHGetImageList also is not better.

			//FUTURE: make faster "shell:...". Now eg 100 ms for Settings first time (50 ms APidl.FromString_ and 50 ms SHGetFileInfo).
			//	Alternatives (not tested): https://stackoverflow.com/questions/32122679/getting-icon-of-modern-windows-app-from-a-desktop-application
			//	Not a big problem when using caching.
			//	Also some icons have incorrect background, eg Calculator.

			var pidl2 = pidl?.UnsafePtr ?? default;
			if (usePidl) {
				if (pidl2 == default) {
					pidl2 = APidl.FromString_(file);
					if (pidl2 == default) usePidl = false; else freePidl = true;
				}
			}

			if (!usePidl) {
				Debug.Assert(pidl2 == default && file != null);
				pidl2 = Marshal.StringToCoTaskMemUni(file);
				freePidl = true;
			}

			if (pidl2 == default) return default;

			//This is faster but fails with some files etc, randomly with others.
			//It means that shell API and/or extensions are not thread-safe, even if can run in MTA.
			//return _GetShellIcon2(pidl2, size, usePidl);

			IntPtr R = default;
			try {
				if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
					R = _GetShellIcon2(usePidl, pidl2, size);
				} else {
					//tested: switching thread does not make slower. The speed depends mostly on locking, because then thread pool threads must wait.
#if true
					R = Task.Factory.StartNew(() => _GetShellIcon2(usePidl, pidl2, size), default, 0, StaTaskScheduler_.Default).Result;
#else //old code, uses ThreadPoolSTA_
					using var work = ThreadPoolSTA_.CreateWork(null, o => { R = _GetShellIcon2(usePidl, pidl2, size); });
					work.Submit();
					work.Wait();
#endif
				}
			}
			finally { if (freePidl) Marshal.FreeCoTaskMem(pidl2); }
			GC.KeepAlive(pidl);
			return R;
		}

		static IntPtr _GetShellIcon2(bool isPidl, IntPtr pidl, int size) {
			IntPtr R = default, il = default; int index = -1, ilIndex, realSize;

			if (size < (realSize = 16) * 5 / 4) ilIndex = Api.SHIL_SMALL;
			else if (size < (realSize = 32) * 5 / 4) ilIndex = Api.SHIL_LARGE;
			else if (size < 256) {
				ilIndex = Api.SHIL_EXTRALARGE; realSize = 48;
				//info: cannot resize from 256 because GetIcon(SHIL_JUMBO) gives 48 icon if 256 icon unavailable. Getting real icon size is either impossible or quite difficult and slow (not tested).
			} else { ilIndex = Api.SHIL_JUMBO; realSize = 256; }

			using var dac = new ADpi.AwarenessContext(ADpi.AwarenessContext.DPI_AWARENESS_CONTEXT_UNAWARE);

			//Need to lock this part, or randomly fails with some file types.
			lock ("TK6Z4XiSxkGSfC14/or5Mw") {
				try {
					uint fl = Api.SHGFI_SYSICONINDEX | Api.SHGFI_SHELLICONSIZE;
					if (ilIndex == Api.SHIL_SMALL) fl |= Api.SHGFI_SMALLICON;
					if (isPidl) fl |= Api.SHGFI_PIDL; else fl |= Api.SHGFI_USEFILEATTRIBUTES;
					il = Api.SHGetFileInfo(pidl, 0, out var x, Api.SizeOf<Api.SHFILEINFO>(), fl);
					if (il != default) index = x.iIcon;
					//Marshal.Release(il); //undocumented, but without it IImageList refcount grows. Probably it's ok, because it is static, never deleted until process exits.
				}
				catch { ADebug.Print("exception"); }
				//Shell extensions may throw.
				//By default .NET does not allow to handle eg access violation exceptions.
				//	Previously we would add [HandleProcessCorruptedStateExceptions], but Core ignores it.
				//	Now our AppHost sets environment variable COMPlus_legacyCorruptedStateExceptionsPolicy=1 before loading runtime.
				//	Or could move the API call to the C++ dll.
			}
			if (index < 0) return default;

			//note: Getting icon from imagelist must be in STA thread too, else fails with some file types.
			//tested: This part works without locking. Using another lock here makes slower.

			try {
				if (ilIndex == Api.SHIL_SMALL || ilIndex == Api.SHIL_LARGE || _GetShellImageList(ilIndex, out il)) {
					//AOutput.Write(il, ADebug.GetComObjRefCount(il));
					R = Api.ImageList_GetIcon(il, index, 0);
					if (size != realSize && R != default) {
						//AOutput.Write(size, realSize, index, file);
						R = Api.CopyImage(R, Api.IMAGE_ICON, size, size, Api.LR_COPYDELETEORG | Api.LR_COPYRETURNORG);
					}
				}
			}
			catch (Exception e) { ADebug.Print(e.Message); }
			//finally { if(il != default) Marshal.Release(il); }
			return R;

			static bool _GetShellImageList(int ilIndex, out IntPtr R) {
				lock ("vK6Z4XiSxkGSfC14/or5Mw") { //the API fails if called simultaneously by multiple threads
					if (0 == Api.SHGetImageList(ilIndex, Api.IID_IImageList, out R)) return true;
				}
				Debug.Assert(false);
				return false;
			}
		}

		//Gets shortcut (.lnk) icon.
		//Much faster than other shell API.
		//Also gets correct icon where iextracticon fails and/or shgetfileinfo gets blank document icon, don't know why.
		//Usually fails only when target does not exist. Then iextracticon also fails, and shgetfileinfo gets blank document icon.
		//If fails, returns default(IntPtr). No exceptions.
		static IntPtr _GetLnkIcon(string file, int size) {
			try {
				using var x = AShortcutFile.Open(file);
				var s = x.GetIconLocation(out int ii); if (s != null) return _Load(s, ii, size);
				s = x.TargetPathRawMSI; if (s != null) return _OfFile(s, size, IconGetFlags.DontSearch);
				//AOutput.Write("need IDList", file);
				using (var pidl = x.TargetPidl) return _OfPidl(pidl, size);
			}
			catch { return default; }
		}

		/// <summary>
		/// Extracts icon directly from file that contains it.
		/// </summary>
		/// <returns>Returns default(AIcon) if failed.</returns>
		/// <param name="file">.ico, .exe, .dll or other file that contains one or more icons. Also supports cursor files - .cur, .ani. Must be full path, without icon index. Supports environment variables (see <see cref="APath.ExpandEnvVar"/>).</param>
		/// <param name="index">Icon index or negative icon resource id in the .exe/.dll file.</param>
		/// <param name="size">Icon width and height. Default 16.</param>
		public static unsafe AIcon Load(string file, int index = 0, int size = 16) {
			using var ds = new _DebugSpeed(file);
			return _Load(file, index, _NormalizeIconSizeArgument(size));
		}

		static unsafe AIcon _Load(string file, int index, int size) {
			//We use SHDefExtractIcon because of better quality resizing (although several times slower) which matches shell and imagelist resizing.
			//With .ico it matches LoadImage speed (without resizing). PrivateExtractIcons is slightly slower.

			if (file.NE()) return default;
			IntPtr hi = default;
			int hr = Api.SHDefExtractIcon(file, index, 0, &hi, null, size);
			if (hr != 0) return default;
			return new AIcon(hi);

			//if(Api.PrivateExtractIcons(file, index, size, size, out R, default, 1, 0) != 1) return default;
		}

		/// <summary>
		/// Gets a shell stock icon.
		/// </summary>
		/// <returns>Returns default(AIcon) if failed.</returns>
		/// <param name="icon">Shell stock icon id.</param>
		/// <param name="size">Icon width and height. Default 16.</param>
		public static unsafe AIcon Stock(StockIcon icon, int size = 16) {
			var x = new Api.SHSTOCKICONINFO(); x.cbSize = Api.SizeOf(x);
			if (0 != Api.SHGetStockIconInfo(icon, 0, ref x)) return default;
			var s = new string(x.szPath);
			return Load(s, x.iIcon, size);
			//note: don't cache, because of the limit of handles a process can have. Maybe only exe and document icons; maybe also folder and open folder.

			//tested: always gets 32x32 icon: Api.LoadImage(default, 32516, Api.IMAGE_ICON, 16, 16, Api.LR_SHARED); //OIC_INFORMATION
		}

		/// <summary>
		/// Gets <msdn>IDI_APPLICATION</msdn> icon from unmanaged resources of this program file.
		/// </summary>
		/// <returns>Returns default(AIcon) if there are no icons.</returns>
		/// <param name="size">Icon width and height. Default 16.</param>
		/// <remarks>
		/// The icon is cached and protected from destroying, therefore don't need to destroy it, and not error to do it.
		/// </remarks>
		public static AIcon OfThisApp(int size = 16) {
			var h = AProcess.ExeModuleHandle;
			if (h == default) return default;
			size = _NormalizeIconSizeArgument(size);
			return new AIcon(Api.LoadImage(h, Api.IDI_APPLICATION, Api.IMAGE_ICON, size, size, Api.LR_SHARED));

			//This is not 100% reliable because the icon id 32512 (IDI_APPLICATION) is undocumented.
			//I could not find a .NET method to get icon directly from native resources of assembly.
			//Could use the resource emumeration API...
			//info: MSDN lies that with LR_SHARED gets a cached icon regardless of size argument. Caches each size separately. Tested on Win 10, 7, XP.
		}

		/// <summary>
		/// Gets icon that is displayed in window title bar and in taskbar button.
		/// </summary>
		/// <returns>Returns default(AIcon) if failed.</returns>
		/// <param name="w">Window of any process.</param>
		/// <param name="size">Icon width and height. Default 16.</param>
		public static AIcon OfWindow(AWnd w, int size = 16) {
			//int size = Api.GetSystemMetrics(large ? Api.SM_CXICON : Api.SM_CXSMICON);

			//support Windows Store apps
			if (1 == AWnd.Internal_.GetWindowsStoreAppId(w, out var appId, true)) {
				IntPtr hi = OfFile(appId, size, IconGetFlags.DontSearch);
				if (hi != default) return new AIcon(hi);
			}

			bool large = size >= 24; //SHOULDDO: make high-DPI-aware. How?
			bool ok = w.SendTimeout(2000, out LPARAM R, Api.WM_GETICON, large);
			if (R == 0 && ok) w.SendTimeout(2000, out R, Api.WM_GETICON, !large);
			if (R == 0) R = AWnd.More.GetClassLong(w, large ? Native.GCL.HICON : Native.GCL.HICONSM);
			if (R == 0) R = AWnd.More.GetClassLong(w, large ? Native.GCL.HICONSM : Native.GCL.HICON);
			//tested this code with DPI 125%. Small icon of most windows match DPI (20), some 16, some 24.
			//tested: undocumented API InternalGetWindowIcon does not get icon of winstore app.

			//Copy, because will DestroyIcon, also it resizes if need.
			if (R != default) return new AIcon(Api.CopyImage(R, Api.IMAGE_ICON, size, size, 0));
			return default;
		}

		/// <summary>
		/// Creates icon at run time.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="drawCallback">Called to draw icon. If null, the icon will be completely transparent.</param>
		public static AIcon Create(int width, int height, Action<Graphics> drawCallback = null) {
			IntPtr hi;
			if (drawCallback != null) {
				using var b = new Bitmap(width, height);
				using var g = Graphics.FromImage(b);
				g.Clear(Color.Transparent); //optional, new bitmaps are transparent, but it is undocumented, and eg .NET Bitmap.MakeTransparent does it
				drawCallback(g);
				hi = b.GetHicon();
			} else {
				int nb = AMath.AlignUp(width, 32) / 8 * height;
				var aAnd = new byte[nb]; for (int i = 0; i < nb; i++) aAnd[i] = 0xff;
				var aXor = new byte[nb];
				hi = Api.CreateIcon(default, width, height, 1, 1, aAnd, aXor);

				//speed: ~20 mcs. About 10 times faster than above. Faster than CopyImage etc.
			}
			return new AIcon(hi);
		}

		///// <summary>
		///// Destroys native icon.
		///// Calls API <msdn>DestroyIcon</msdn>. Does nothing if iconHandle is default(IntPtr).
		///// </summary>
		//public static void DestroyIconHandle(IntPtr iconHandle)
		//{
		//	if(iconHandle != default) Api.DestroyIcon(iconHandle);
		//}

		/// <summary>
		/// Converts native icon to GDI+ icon object.
		/// Returns null if <i>Handle</i> is default(IntPtr).
		/// </summary>
		/// <param name="destroyNative">
		/// If true (default), the returned variable owns the native icon and destroys it when disposing; also clears this variable and don't need to dispose it.
		/// If false, the returned variable just uses the native icon handle of this variable and will not destroy; later will need to dispose this variable.
		/// </param>
		public Icon ToGdipIcon(bool destroyNative = true) {
			if (_handle == default) return null;
			var R = Icon.FromHandle(_handle);
			if (destroyNative) { LetObjectDestroyIconOrCursor_(R); _handle = default; }
			return R;
		}

		internal static void LetObjectDestroyIconOrCursor_(object o) {
			var ty = o.GetType(); //Icon or Cursor
			var fi = ty.GetField("_ownHandle", BindingFlags.NonPublic | BindingFlags.Instance); //new Icon code
			if (fi == null) fi = ty.GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance); //Cursor, old Icon code
			Debug.Assert(fi != null);
			fi?.SetValue(o, true);

			//Don't allow to exceed the process handle limit when the program does not dispose them.
			//Default limit for USER and GDI objects is 10000, min 200.
			//Icons are USER objects. Most icons also create 3 GDI objects, some 2. So a process can have max 3333 icons.
			//If GC starts working when pressure is 100 KB, then the number of icons is ~50 and GDI handles ~150.
			//We don't care about icon memory size.
			AGC.AddObjectMemoryPressure(o, 2000);
		}

		/// <summary>
		/// Converts native icon to GDI+ bitmap object.
		/// Returns null if <i>Handle</i> is default(IntPtr) or if fails to convert.
		/// </summary>
		/// <param name="destroyNative">
		/// If true (default), destroys the native icon object; also clears this variable and don't need to dispose it.
		/// If false, later will need to dispose this variable.
		/// </param>
		public Bitmap ToGdipBitmap(bool destroyNative = true) {
			//note: don't use Bitmap.FromHicon. It just calls GdipCreateBitmapFromHICON which does not support alpha etc.

			if (_handle == default) return null;
			Icon ic = Icon.FromHandle(_handle);
			Bitmap im = null;
			try { im = ic.ToBitmap(); }
			catch (Exception e) { AWarning.Write(e.ToString(), -1); }
			ic.Dispose();
			if (destroyNative) Dispose();
			return im;
		}

		/// <summary>
		/// Converts native icon to WPF image object.
		/// Returns null if <i>Handle</i> is default(IntPtr) or if fails to convert.
		/// </summary>
		/// <param name="destroyNative">
		/// If true (default), destroys the native icon object; also clears this variable and don't need to dispose it.
		/// If false, later will need to dispose this variable.
		/// </param>
		public System.Windows.Media.Imaging.BitmapSource ToWpfImage(bool destroyNative = true) {
			if (_handle == default) return null;
			try { return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(_handle, default, default); }
			catch (Exception e) { AWarning.Write(e.ToString(), -1); return null; }
			finally { if (destroyNative) Dispose(); }
		}

		/// <summary>
		/// Parses icon location string.
		/// Returns true if it includes icon index or resource id.
		/// </summary>
		/// <param name="s">
		/// Icon location. Can be <c>"path,index"</c> or <c>"path,-id"</c> or just path.
		/// Supports path enclosed in "" like <c>"\"path\",index"</c>, and spaces between comma and index like <c>"path, index"</c>.
		/// </param>
		/// <param name="path">Receives path without index and without "". Can be same variable as <i>s</i>.</param>
		/// <param name="index">Receives index/id or 0.</param>
		public static bool ParsePathIndex(string s, out string path, out int index) {
			path = s; index = 0;
			if (s.NE()) return false;
			if (s[0] == '\"') path = s = s.Replace("\"", ""); //can be eg "path",index
			if (s.Length < 3) return false;
			if (!AChar.IsAsciiDigit(s[^1])) return false;
			int i = s.LastIndexOf(','); if (i < 1) return false;
			index = s.ToInt(i + 1, out int e); if (e != s.Length) return false;
			path = s.Remove(i);
			return true;

			//note: API PathParseIconLocation has bugs. Eg splits "path,5moreText". Or from "path, 5" removes ", 5" and returns 0.
		}

		static int _NormalizeIconSizeArgument(int size) {
			if (size == 0) return 16;
			return (uint)size <= 256 ? size : throw new ArgumentOutOfRangeException("size", "Must be 0 - 256");
		}

		//rejected. Not per-monitor-DPI-aware, etc. Better use 16 etc and then auto-scale.
		///// <summary>
		///// Gets size of small icons displayed in UI.
		///// Depends on DPI; 16 when DPI 100%.
		///// </summary>
		//public static int SizeSmall => ADpi.OfThisProcess / 6; //eg 96/6=16

		///// <summary>
		///// Gets size of large icons displayed in UI.
		///// Depends on DPI; 32 when DPI 100%.
		///// </summary>
		//public static int SizeLarge => ADpi.OfThisProcess / 3;

		//static int _SizeExtraLarge => ADpi.OfThisProcess / 2;

		//tested: shell imagelist icon sizes match these.
		//note: don't use GetSystemMetrics(SM_CXSMICON/SM_CXICON). They are for other purposes, eg window title bar, tray icon. On Win7 they can be different because can be changed in Control Panel. Used by SystemInformation.SmallIconSize etc.

		/// <summary>
		/// If not 0, "get icon" functions of this class will print (in editor's output) their execution time in milliseconds when it &gt;= this value.
		/// </summary>
		/// <remarks>
		/// Icons are mostly used in toolbars and menus. Getting icons of some files can be slow. For example if antivirus program scans the file. Toolbars and menus that use slow icons may start with a noticeable delay. Use this property to find too slow icons. Then you can replace them with fast icons, for example .ico files.
		/// </remarks>
		public static int DebugSpeed { get; set; }

		struct _DebugSpeed : IDisposable
		{
			long _time;
			object _file; //string or APidl

			public _DebugSpeed(object file) {
				if (DebugSpeed > 0) {
					_file = file;
					_time = ATime.PerfMilliseconds;
				} else {
					_time = 0;
					_file = null;
				}
				//SHOULDDO: implement global icon cache here. File-based.
			}

			public void Dispose() {
				if (_time != 0) {
					long t = ATime.PerfMilliseconds - _time;
					if (t >= DebugSpeed) {
						//if (_file is APidl p) _file = p.ToShellString(Native.SIGDN.NORMALDISPLAY);
						AOutput.Write($"AIcon.DebugSpeed: {t} ms, {_file}");
					}
				}
			}
		}

		/// <summary>
		/// Gets icon path from code that contains string like <c>@"c:\windows\system32\notepad.exe"</c> or <c>@"%AFolders.System%\notepad.exe"</c> or URL/shell.
		/// Also supports code patterns like <c>AFolders.System + "notepad.exe"</c> or <c>AFolders.Virtual.RecycleBin</c>.
		/// Returns null if no such string/pattern.
		/// </summary>
		internal static string IconPathFromCode_(MethodInfo mi, out bool cs) {
			//support code pattern like 'AFolders.System + "notepad.exe"'.
			//	Opcodes: call(AFolders.System), ldstr("notepad.exe"), FolderPath.op_Addition.
			//also code pattern like 'AFolders.System' or 'AFolders.Virtual.RecycleBin'.
			//	Opcodes: call(AFolders.System), FolderPath.op_Implicit(FolderPath to string).
			//also code pattern like 'AFile.TryRun("notepad.exe")'.
			//AOutput.Write(mi.Name);
			cs = false;
			int i = 0, patternStart = -1; MethodInfo f1 = null; string filename = null, filename2 = null;
			try {
				var reader = new ILReader(mi);
				foreach (var instruction in reader.Instructions) {
					if (++i > 100) break;
					var op = instruction.Op;
					//AOutput.Write(op);
					if (op == OpCodes.Nop) {
						i--;
					} else if (op == OpCodes.Ldstr) {
						var s = instruction.Data as string;
						//AOutput.Write(s);
						if (i == patternStart + 1) filename = s;
						else {
							if (APath.IsFullPathExpandEnvVar(ref s)) return s; //eg AFile.TryRun(@"%AFolders.System%\notepad.exe");
							if (APath.IsShellPathOrUrl_(s)) return s;
							filename = null; patternStart = -1;
							if (i == 1) filename2 = s;
						}
					} else if (op == OpCodes.Call && instruction.Data is MethodInfo f && f.IsStatic) {
						//AOutput.Write(f, f.DeclaringType, f.Name, f.MemberType, f.ReturnType, f.GetParameters().Length);
						var dt = f.DeclaringType;
						if (dt == typeof(AFolders) || dt == typeof(AFolders.Virtual)) {
							if (f.ReturnType == typeof(FolderPath) && f.GetParameters().Length == 0) {
								//AOutput.Write(1);
								f1 = f;
								patternStart = i;
							}
						} else if (dt == typeof(FolderPath)) {
							if (i == patternStart + 2 && f.Name == "op_Addition") {
								//AOutput.Write(2);
								var fp = (FolderPath)f1.Invoke(null, null);
								if ((string)fp == null) return null;
								return fp + filename;
							} else if (i == patternStart + 1 && f.Name == "op_Implicit" && f.ReturnType == typeof(string)) {
								//AOutput.Write(3);
								return (FolderPath)f1.Invoke(null, null);
							}
						}
					}
				}
				if (filename2 != null) {
					if (filename2.Ends(".exe", true)) return AFile.SearchPath(filename2);
					if (cs = filename2.Ends(".cs", true)) return filename2;
				}
			}
			catch (Exception ex) { ADebug.Print(ex); }
			return null;
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="AIcon.OfFile"/> and similar functions.
	/// </summary>
	[Flags]
	public enum IconGetFlags
	{
		/// <summary>
		/// The <i>file</i> argument is literal full path. Don't parse "path,index", don't support ".ext" (file type icon), don't make fully-qualified, etc.
		/// </summary>
		LiteralPath = 1,

		/// <summary>
		/// Don't call <see cref="AFile.SearchPath"/>.
		/// </summary>
		DontSearch = 2,

#if false
		/// Use shell API for all file types, including exe and ico.
		Shell=8, //rejected because SHGetFileInfo does not get exe icon with shield overlay

		/// <summary>
		/// If file does not exist or fails to get its icon, get common icon for that file type, or default document icon if cannot get common icon.
		/// </summary>
		DefaultIfFails = 16, //rejected. Now for exe/ico/etc is like with shell API: if file exists, gets default icon (exe or document), else returns default(IntPtr).
#endif
	}

#pragma warning disable 1591 //missing XML documentation
	/// <summary>See <see cref="AIcon.Stock"/>, <msdn>SHSTOCKICONID</msdn>.</summary>
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
}
