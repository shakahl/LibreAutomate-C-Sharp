using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.Drawing;
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
	public static class Path_
	{
		/// <summary>
		/// If path starts with '%' character, expands environment variables enclosed in %, else just returns path.
		/// </summary>
		/// <seealso cref="Environment.ExpandEnvironmentVariables"/>.
		public static string ExpandEnvVar(string path)
		{
			if(Empty(path) || path[0] != '%') return path;
			return Environment.ExpandEnvironmentVariables(path); //very slow, even if there are no '%'

			//this would make > 2 times faster, but never mind, in this case better is smaller code
			//int n = path.Length + 100, r;
			//var b = new StringBuilder(n);
			//for(;;) {
			//	r = (int)ExpandEnvironmentStrings(path, b, (uint)n);
			//	//OutList(n, r, b.Length, b);
			//	if(r <= n) break;
			//	b.EnsureCapacity(n = r);
			//}
			//return b.ToString();
		}

		/// <summary>
		/// Returns true if path matches one of these wildcard patterns:
		///		@"\\*" - network path.
		///		@"[A-Z]:\*" - local path.
		///		@"[A-Z]:" - drive name.
		///		@"%*%*" - environment variable (usually contains a full path).
		///		@"˂*˃*" - special string that can be used with some functions of this library.
		///		@":*" - eg shell object CLSID like "::{CLSID}"
		///	Also returns true if path looks like a URL (any protocol), eg "http://abc" or "http:" or "shell:abc". Note: don't use "filename:stream" unless it is full path.
		/// </summary>
		public static bool IsFullPath(string path)
		{
			int len = (path == null) ? 0 : path.Length;

			if(len >= 2) {
				char c = path[0];
				switch(c) {
				case '<': return path.IndexOf('>', 1) > 1;
				case '%': return path.IndexOf('%', 1) > 1;
				case '\\': return path[1] == '\\';
				case ':': return true;
				}

				if((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
					if(path[1] == ':') return len == 2 || path[2] == '\\'; //info: returns false if eg "c:abc" which means "abc" in current directory of drive "c:"

					//is URL (any protocol)?
					if(IsURL(path)) return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Makes fully-qualified path.
		/// If path is not full path, returns App+path.
		/// Expands environment variables, processes @"..\" etc (calls Combine()).
		/// </summary>
		/// <param name="path">Full path or filename or relative path.</param>
		public static string MakeFullPath(string path)
		{
			return Combine(null, path);
		}

		/// <summary>
		/// Combines two paths and makes fully-qualified path. Similar to Path.Combine(), but not the same.
		/// </summary>
		/// <remarks>
		///	Returns fully-qualified path, with processed @"..\" etc, not relative, not short (DOS), with replaced environment variables if starts with "%".
		///	If the return value would not be full path, prepends Folders.App.
		/// If s1 and s2 are null or "", returns "".
		///	No exceptions.
		/// </remarks>
		public static string Combine(string s1, string s2)
		{
			string R = null;
			int len = (s2 == null) ? 0 : s2.Length;

			if(Empty(s1)) {
				if(len == 0) return "";
				R = s2;
			} else {
				if(len == 0) R = s1.TrimEnd('\\');
				else if(s2[0] == '\\') R = s1.TrimEnd('\\') + s2;
				else if(s1.EndsWith_("\\")) R = s1 + s2;
				else R = s1 + @"\" + s2;
			}

			R = ExpandEnvVar(R);

			if(!IsFullPath(R)) R = Folders.App + R;
			else if(R[0] == ':' || R[0] == '<' || R[0] == '%') return R;

			if(R.IndexOf_(@".\") >= 0 || R.EndsWith_(".") || R.IndexOf('~') >= 0) {
				try { R = Path.GetFullPath(R); } catch { }
			}

			return R;
		}

		//Better don't use such things. If need, a function can instead have a parameter for it.
		//public static string ThreadDefaultDirectory
		//{
		//	get { return _threadDefDir ?? (_threadDefDir = Folders.App); }
		//	set { _threadDefDir = value; }
		//}
		//[ThreadStatic]
		//static string _threadDefDir;

		/// <summary>
		/// Returns true if path is or begins with an internet protocol, eg "http://www.x.com" or "http:".
		/// The protocol can be unknown, the function just checks string format.
		/// </summary>
		/// <param name="path">A file path or URL or any other string.</param>
		public static bool IsURL(string path)
		{
			return !Empty(path) && path.IndexOf(':') > 1 && Api.PathIsURL(path);
			//info: returns true if begins with "xx:" where xx is 2 or more of alphanumeric, '.', '-' and '+' characters.
		}

	}

	//[DebuggerStepThrough]
	public static class Files
	{
		#region exists, search

		/// <summary>
		/// Returns non-zero if file or directory exists: 1 if file, 2 if directory (folder or drive).
		/// Returns 0 if does not exist or path is empty or invalid. No exceptions.
		/// Uses Api.GetFileAttributes().
		/// </summary>
		/// <param name="path">Full path. Supports "%EnvVar%path" and "path\..\path".</param>
		public static int FileOrDirectoryExists(string path)
		{
			if(Empty(path)) return 0;
			int r = (int)Api.GetFileAttributes(Path_.ExpandEnvVar(path));
			if(r == -1) return 0;
			var a = (FileAttributes)r;
			return a.HasFlag(FileAttributes.Directory) ? 2 : 1;
		}

		/// <summary>
		/// Returns true if file exists and is not a directory.
		/// Uses FileOrDirectoryExists(), which calls Api.GetFileAttributes().
		/// </summary>
		/// <param name="path">Full path. Supports "%EnvVar%path" and "path\..\path".</param>
		public static bool FileExists(string path)
		{
			return FileOrDirectoryExists(path) == 1;
		}

		/// <summary>
		/// Returns true if directory (folder or drive) exists.
		/// Uses FileOrDirectoryExists(), which calls Api.GetFileAttributes().
		/// </summary>
		/// <param name="path">Full path. Supports "%EnvVar%path" and "path\..\path".</param>
		public static bool DirectoryExists(string path)
		{
			return FileOrDirectoryExists(path) == 2;
		}

		/// <summary>
		/// Finds file or directory and returns full and fully-qualified path.
		/// Returns null if cannot be found.
		/// If the path argument is not full path, searches in these places:
		///	1. defDirs, if used.
		/// 2. <see cref="Folders.App"/>.
		/// 3. Calls Api.SearchPath(), which searches in process directory, Windows system directories, current directory, PATH environment variable.
		/// 4. If .exe, tries to get path from registry "App Paths" keys.
		/// </summary>
		/// <param name="path">Full or relative path or just filename with extension. Supports network paths.</param>
		/// <param name="dirs">0 or more directories where to search.</param>
		public static string SearchPath(string path, params string[] dirs)
		{
			if(Empty(path)) return null;

			if(Path_.IsFullPath(path)) {
				path = Path_.MakeFullPath(path); //make fully-qualified, ie without %envvar%, \..\, ~.
				if(0 != FileOrDirectoryExists(path)) return path;
				return null;
			}

			if(dirs != null) {
				foreach(var d in dirs) {
					if(Empty(d)) continue;
					var s = Path_.Combine(d, path);
					if(0 != FileOrDirectoryExists(s)) return s;
				}
			}

			{
				var s = Folders.App + path;
				if(0 != FileOrDirectoryExists(s)) return s;
			}

			var sb = new StringBuilder(300);
			if(0 != Api.SearchPath(null, path, null, 300, sb, Zero)) return sb.ToString();

			if(path.EndsWith_(".exe", true)) {
				string rk = @"Software\Microsoft\Windows\CurrentVersion\App Paths\" + path;
				if(Registry_.GetString(out path, "", rk) || Registry_.GetString(out path, "", rk, Registry.LocalMachine)) {
					path = Path_.MakeFullPath(path.Trim('\"'));
					if(0 != FileOrDirectoryExists(path)) return path;
				}
			}

			return null;
		}

		#endregion

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
				DefaultIfFails = 4,

				/// <summary>
				/// If fails to get true icon, create blank (completely transparent) icon. You can use 'file' argument = null/"" if need just blank icon.
				/// </summary>
				BlankIfFails = 8,
			}










			//TODO: remove these

			public static IntPtr GetIconHandle2(string file, int size = 16, IconFlag flags = 0)
			{
				IntPtr pidl = Zero;
				if(file[0] != '.') {
					pidl = Misc.PidlFromString(file);
					if(pidl == Zero) return Zero;
				}

				try {
					uint fl = Api.SHGFI_SYSICONINDEX;
					if(size < 24) fl |= Api.SHGFI_SMALLICON;
					var x = new Api.SHFILEINFO();
					IntPtr il;
					if(pidl != Zero) il = Api.SHGetFileInfo(pidl, 0, ref x, Api.SizeOf(x), fl | Api.SHGFI_PIDL);
					else il = Api.SHGetFileInfo(file, 0, ref x, Api.SizeOf(x), fl | Api.SHGFI_USEFILEATTRIBUTES);
					if(il == Zero) return Zero;
					return Api.ImageList_GetIcon(il, x.iIcon, 0);
				}
				finally { Marshal.FreeCoTaskMem(pidl); }
			}

			public static IntPtr GetIconHandle3(string file, int size = 16, IconFlag flags = 0)
			{
				IntPtr pidl = Misc.PidlFromString(file);
				if(pidl == Zero) return Zero;

				return _GetPidlIcon(pidl, size, true);
			}

			[DllImport("shell32.dll", EntryPoint = "#77")]
			public static extern unsafe int SHMapPIDLToSystemImageListIndex(Api.IShellFolder pshf, IntPtr pidl, int* piIndexSel);

			[DllImport("shell32.dll", EntryPoint = "#727", PreserveSig = true)]
			public static extern int SHGetImageList(int iImageList, [In] ref Guid riid, out IntPtr ppvObj);

			public const int SHIL_LARGE = 0;
			public const int SHIL_SMALL = 1;
			public const int SHIL_EXTRALARGE = 2;
			public const int SHIL_SYSSMALL = 3;
			public const int SHIL_JUMBO = 4;

			public static Guid IID_IImageList = new Guid(0x46EB5926, 0x582E, 0x4017, 0x9F, 0xDF, 0xE8, 0x99, 0x8D, 0xAA, 0x09, 0x50);

			public static unsafe IntPtr GetIconHandle4(string file, int size = 16, IconFlag flags = 0)
			{
				IntPtr pidl = Misc.PidlFromString(file);
				if(pidl == Zero) return Zero;

				Api.IShellFolder folder = null;
				try {
					IntPtr pidlItem;
					int hr = Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem);
					if(0 != hr) { OutDebug($"{pidl}, {Marshal.GetExceptionForHR(hr)?.Message}"); return Zero; }

					int ii = SHMapPIDLToSystemImageListIndex(folder, pidlItem, null);
					//Out(ii);
					if(ii < 0) return Zero;

					size = size < 24 ? SHIL_SMALL : SHIL_LARGE;
					IntPtr il;
					if(0 != SHGetImageList(size, ref IID_IImageList, out il)) return Zero;

					return Api.ImageList_GetIcon(il, ii, 0);
				}
				finally {
					Api.ReleaseComObject(folder);
					Marshal.FreeCoTaskMem(pidl);
				}
			}

			[Flags]
			public enum COINIT :uint
			{
				COINIT_APARTMENTTHREADED = 0x2,
				COINIT_MULTITHREADED = 0x0,
				COINIT_DISABLE_OLE1DDE = 0x4,
				COINIT_SPEED_OVER_MEMORY = 0x8
			}

			[DllImport("ole32.dll", PreserveSig = true)]
			public static extern int CoInitializeEx(IntPtr pvReserved, COINIT dwCoInit);

			[DllImport("ole32.dll")]
			public static extern void CoUninitialize();















			/// <summary>
			/// Gets file icon.
			/// Extracts icon directly from the file, or gets shell icon, depending on file type, icon index, flags etc.
			/// Returns icon handle. Returns Zero if failed, for example when the file does not exist.
			/// Later call Api.DestroyIcon().
			/// </summary>
			/// <param name="file">
			/// Any file, folder, URL like "http://..." or "shell:..." etc, shell item like "::{CLSID}" or "::{CLSID1}\::{CLSID2}". Also can be a file type like ".txt" or a protocol like "http:".
			/// If it is a file containing multiple icons (eg exe, dll), can be specified icon index like "path,index" or native icon resource id like "path,-id".
			/// If not full path, the file must be in program's folder.
			/// </param>
			/// <param name="size">Icon width and height. Max 256.</param>
			/// <param name="flags"><see cref="IconFlag"/></param>
			public static IntPtr GetIconHandle(string file, int size = 16, IconFlag flags = 0)
			{
				if((uint)size > 256) throw new ArgumentOutOfRangeException("size");
				if(size == 0) size = 16;

				if(!Empty(file)) {
					file = Path_.ExpandEnvVar(file);

					//if(file[0] == '<') {
					//	//TODO: either use or remove this code
					//	if(!file.StartsWith_("<idlist:")) return Zero;
					//	int i = file.IndexOf('>'); if(i < 9) return Zero;
					//	IntPtr pidl=Folders.VirtualITEMIDLIST...
					//	return _GetPidlIcon(pidl, size, true);
					//}

					//var perf = new Perf.Inst(true);
					IntPtr R = _GetFileIcon(file, size, flags);
					//perf.Next(); OutList(perf.Times, file);
					if(R != Zero) return R;
				}

				if(flags.HasFlag(IconFlag.BlankIfFails)) return CreateBlankIcon(size, size);

				return Zero;
			}

			/// <summary>
			/// Gets icon of a file or other shell object specified by its ITEMIDLIST pointer.
			/// Returns icon handle. Returns Zero if failed.
			/// Later call Api.DestroyIcon().
			/// </summary>
			/// <param name="pidl">ITEMIDLIST pointer.</param>
			/// <param name="size">Icon width and height. Max 256.</param>
			/// <param name="flags">Can be IconFlag.BlankIfFails. See <see cref="IconFlag"/>.</param>
			public static IntPtr GetIconHandle(IntPtr pidl, int size = 16, IconFlag flags = 0)
			{
				if((uint)size > 256) throw new ArgumentOutOfRangeException("size");
				if(size == 0) size = 16;

				if(pidl != Zero) {
					IntPtr R = _GetPidlIcon(pidl, size, false);
					if(R != Zero) return R;
				}

				if(flags.HasFlag(IconFlag.BlankIfFails)) return CreateBlankIcon(size, size);

				return Zero;
			}

			internal static IntPtr _GetFileIcon(string file, int size, IconFlag flags)
			{
				IntPtr R = Zero, pidl = Zero;
				int index = 0;
				bool extractFromFile = false, isFileType = false, isURL = false, isCLSID = false, isPath = true;
				bool getDefaultIfFails = flags.HasFlag(IconFlag.DefaultIfFails);

				bool searchPath = flags.HasFlag(IconFlag.SearchPath);

				if(!flags.HasFlag(IconFlag.LiteralPath)) {
					//is ".ext" or "protocol:"?
					isFileType = Misc.PathIsExtension(file) || (isURL = Misc.PathIsProtocol(file));
					if(!isFileType) isURL = Path_.IsURL(file);
					if(isFileType || isURL || (isCLSID = (file[0] == ':'))) isPath = false;
					if(isPath) {
						//get icon index from "path,index" and remove ",index"
						extractFromFile = ParseIconLocation(ref file, out index);

						if(!searchPath) file = Path_.MakeFullPath(file);
					}
				}

				if(searchPath && isPath) {
					file = SearchPath(file);
					if(file == null) return Zero; //ignore getDefaultIfFails
				}

				if(isPath) {
					int ext = 0;
					if(!extractFromFile && file.Length > 4) ext = file.EndsWith_(true, ".exe", ".scr", ".ico", ".cur", ".ani");
					if(extractFromFile || ext > 0) {
						R = GetIconHandleRaw(file, index, size);
						if(R != Zero || extractFromFile) return R;
						switch(FileOrDirectoryExists(file)) {
						case 0:
							if(!getDefaultIfFails) return Zero;
							goto case 1;
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

				//} else if(isFileType && !isURL) {
				//	uint n = 300; var sb = new StringBuilder((int)n);
				//	if(0 == Api.AssocQueryString(0, Api.ASSOCSTR.ASSOCSTR_DEFAULTICON, file, null, sb, ref n)) {
				//		var icon = sb.ToString();
				//		OutList(file, icon);
				//		ParseIconLocation(ref icon, out index);
				//		return GetIconHandleRaw(icon, index, size);
				//	}
				}

				string progId = isCLSID ? null : Misc.GetFileTypeOrProtocolRegistryKey(file, isFileType, isURL);

				//if(isFileType && progId != null) {
				//	CoUninitialize(); CoInitializeEx(Zero, COINIT.COINIT_APARTMENTTHREADED | COINIT.COINIT_DISABLE_OLE1DDE);
				//	//OutList(file, progId + ":");
				//	return GetIconHandle2(file, size);
				//	//return GetIconHandle4(progId + ":", size);
				//}

				RegistryKey rk = (progId == null) ? null : Registry_.Open(progId, Registry.ClassesRoot);
				//OutList(file, progId, isFileType, isURL, rk != null);

				if(rk == null) {
					//unregistered file type/protocol, no extension, folder, ::{CLSID}, shell:...
					pidl = Misc.PidlFromString(file); //TODO: what if isFileType?
													  //OutList("<><c 0xF00000>Unregistered", file, progId, pidl, "</c>");
					R = _GetPidlIcon(pidl, size, true);
					if(R != Zero) return R;
					bool getDoc = isPath ? FileExists(file) : (getDefaultIfFails && !isCLSID);
					if(getDoc) return GetShellStockIconHandle(Api.SHSTOCKICONID.SIID_DOCNOASSOC, size);
					return Zero;
				}

				//registered file type/protocol
				using(rk) {
					//Try icon handler first. It fails for some file types.
					//But if isFileType, it cannot get icon if DefaultIcon is "%1", eg exe/ico/cur/ani/msc.
					bool hasHandler = Registry_.KeyExists(@"ShellEx\IconHandler", rk);
					if(hasHandler) {
						pidl = _PidlFromString(file, progId, isFileType);
						//OutList("<><c 0xF00000>Handler", file, progId, pidl, "</c>");
						R = _GetPidlIcon(pidl, size, true);
						if(R != Zero) return R;
						pidl = Zero;
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
						if(R != Zero || isFileType || isURL) return R;
						if(!getDefaultIfFails && !FileExists(file)) return Zero;
						return GetShellStockIconHandle(Api.SHSTOCKICONID.SIID_DOCNOASSOC, size);
					}

					if(!hasHandler) pidl = _PidlFromString(file, progId, isFileType);
					if(pidl == Zero && getDefaultIfFails && !isFileType) pidl = _PidlFromString(null, progId, true);
					//OutList("<><c 0xF00000>Shell", file, progId, pidl, "</c>");
					R = _GetPidlIcon(pidl, size, true);
					if(R != Zero) return R;

					if(getDefaultIfFails) return GetShellStockIconHandle(Api.SHSTOCKICONID.SIID_DOCNOASSOC, size);
				}

				//We need PIDL for other shell API. Getting it is rather slow, as well as other shell API.
				//Therefore we use all the above code to avoid shell API if possible.
				//We use shell API only for unregistered types/protocols, folders, types that have icon handler shell extensions, some other.
				//We get PIDL before lock/thread. Then faster when using a thread pool, because threads can get PIDL simultaneously.
				//tested: Similar speed as SHGetFileInfo, probably it also gets PIDL. But it does not support icons of any size.

				return Zero;
			}

			static IntPtr _GetPidlIcon(IntPtr pidl, int size, bool freePidl)
			{
				if(pidl == Zero) return Zero;
				try {
					lock ("TK6Z4XiSxkGSfC14/or5Mw") {
						//if(true) {
						if(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
							//Out("STA");
							return _GetPidlIcon2(pidl, size);
						} else {
							//Out("MTA");
							unsafe
							{
								var p = &Util.LibProcessMemory.Ptr->icons;
								if(p->eventSet == Zero) {
									p->eventSet = Api.CreateEvent(Zero, false, false, null);
									p->eventWait = Api.CreateEvent(Zero, false, false, null);

									bool isThisDomainDefault;
									var dd = Util.Misc.GetDefaultAppDomain(out isThisDomainDefault);
									if(isThisDomainDefault) _CreateIconThread();
									else dd.DoCallBack(() => _CreateIconThread()); //because the thread and _IconThread must persist when current non-default domain unloaded
								}

								p->result = Zero;
								p->pidl = pidl; p->size = size;

								Api.SetEvent(p->eventSet);
								Api.WaitForSingleObject(p->eventWait, Api.INFINITE);
								//Api.SignalObjectAndWait(p->eventSet, p->eventWait, Api.INFINITE, false); //why this is often much slower?

								return p->result;
							}
						}
					}
				}
				finally { if(freePidl) Marshal.FreeCoTaskMem(pidl); }
			}

			//in Util.LibProcessMemory
			internal struct ProcessVariables
			{
				internal IntPtr eventSet, eventWait, result, pidl;
				internal int size;
			}

			static void _CreateIconThread()
			{
				var thread = new Thread(_IconThread);
				thread.IsBackground = true;
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}

			//runs in default appdomain
			static unsafe void _IconThread()
			{
				var p = &Util.LibProcessMemory.Ptr->icons;
				for(;;) {
					Api.WaitForSingleObject(p->eventSet, Api.INFINITE);
					p->result = _GetPidlIcon2(p->pidl, p->size);
					Api.SetEvent(p->eventWait);
				}

				//note: cannot use a window and send message, because then some shell extensions return error like "cannot make outbound call while thread is dispatching a synchronous...".
			}

			[HandleProcessCorruptedStateExceptions] //shell extensions may throw anything
			static unsafe IntPtr _GetPidlIcon2(IntPtr pidl, int size)
			{
#if false
				Api.IShellFolder folder = null;
				try {
					IntPtr pidlItem;
					int hr = Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem);
					if(0 != hr) { OutDebug($"{pidl}, {Marshal.GetExceptionForHR(hr)?.Message}"); return Zero; }

					int ii = SHMapPIDLToSystemImageListIndex(folder, pidlItem, null);
					//Out(ii);
					if(ii < 0) return Zero;

					size = size < 24 ? SHIL_SMALL : SHIL_LARGE;
					IntPtr il;
					if(0 != SHGetImageList(size, ref IID_IImageList, out il)) return Zero;

					return Api.ImageList_GetIcon(il, ii, 0);
				}
				finally {
					Api.ReleaseComObject(folder);
				}
#else
				IntPtr R = Zero;
				Api.IShellFolder folder = null;
				Api.IExtractIcon eic = null;
				try {
					IntPtr pidlItem;
					int hr = Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem);
					if(0 != hr) { OutDebug($"{pidl}, {Marshal.GetExceptionForHR(hr)?.Message}"); return Zero; }

					object o;
					hr = folder.GetUIObjectOf(Wnd0, 1, &pidlItem, Api.IID_IExtractIcon, Zero, out o);
					//if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr)?.Message}"); return Zero; }
					if(0 != hr) {
						if(hr != Api.REGDB_E_CLASSNOTREG) OutDebug($"{pidl}, {Marshal.GetExceptionForHR(hr)?.Message}");
						return Zero;
					}
					eic = o as Api.IExtractIcon;

					var sb = new StringBuilder(300); int ii; uint fl;
					hr = eic.GetIconLocation(0, sb, 300, out ii, out fl);
					if(0 != hr) { OutDebug($"{pidl}, {Marshal.GetExceptionForHR(hr)?.Message}"); return Zero; }
					string loc = sb.ToString();

					//OutList(pidl, loc, ii);

					if((fl & (Api.GIL_NOTFILENAME | Api.GIL_SIMULATEDOC)) == 0) {
						R = GetIconHandleRaw(loc, ii, size);
						if(R != Zero) return R;
					}

					//hr = eic.Extract(loc, (uint)ii, &R, null, (uint)size); //no
					//note:
					//	Some shell extensions (eg .sln) ignore size and always return 32x32 in phiconLarge and 16x16 in phiconSmall.
					//	But how then Explorer displays correct icons? Maybe uses SHMapPIDLToSystemImageListIndex/SHGetImageList. Or IExtractImage. Probably it uses other (probably private) functions.

					IntPtr* hiSmall = null, hiBig = null;
					if(size < 24) { hiSmall = &R; size = 32; } else hiBig = &R;
					hr = eic.Extract(loc, (uint)ii, hiBig, hiSmall, Calc.MakeUint(size, 16)); //TODO: may have 20 icon

					if(0 != hr) { OutDebug($"{pidl}, {Marshal.GetExceptionForHR(hr)?.Message}, location={loc}"); return Zero; }

					//OutList(pidl, loc, ii);

					//if(size != 16 && size != 32) { //TODO: test with high DPI

					//	//var perf = new Perf.Inst(true);
					//	IntPtr R2 = Api.CopyImage(R, Api.IMAGE_ICON, size, size, Api.LR_COPYDELETEORG | Api.LR_COPYRETURNORG | Api.LR_COPYFROMRESOURCE);
					//	//perf.NW();
					//	//OutList(R, R2);
					//	R = R2;
					//}
				}
				catch(Exception e) { OutDebug($"pidl={pidl}, {e.Message}, {e.TargetSite}"); }
				finally {
					Api.ReleaseComObject(eic);
					Api.ReleaseComObject(folder);
				}
				return R;
#endif
				//TODO: test high DPI
			}

			static IntPtr _PidlFromString(string file, string progId, bool isFileType)
			{
				if(isFileType) file = progId + ":";
				return Misc.PidlFromString(file);
			}

			//Much faster than other shell API
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
						return _GetPidlIcon(pidl, size, true);
					return Zero;
				}
				finally {
					Api.ReleaseComObject(ppf);
					Api.ReleaseComObject(psl);
				}
			}

			/// <summary>
			/// Extracts icon directly from file that contains it.
			/// Returns icon handle. Returns Zero if failed.
			/// Later call Api.DestroyIcon().
			/// </summary>
			/// <param name="file">.ico, .exe, .dll, .icl or other file that contains one or more icons. Also supports cursor files - .cur, .ani. Must be full path, without icon index. Supports environment variables.</param>
			/// <param name="index">Icon index or negative icon resource id in the .exe/.dll/.icl file.</param>
			/// <param name="size">Icon width and height.</param>
			public static IntPtr GetIconHandleRaw(string file, int index = 0, int size = 16)
			{
				if(Empty(file)) return Zero;
				file = Path_.ExpandEnvVar(file); //tested on Win10: PrivateExtractIcons supports env var, LoadImage doesn't.
				if(index == 0 && file.EndsWith_(".ico", true)) { //LoadImage is 2 times faster than PrivateExtractIcons
					return Api.LoadImage(Zero, file, Api.IMAGE_ICON, size, size, Api.LR_LOADFROMFILE);
				}
				IntPtr R = Zero;
				if(Api.PrivateExtractIcons(file, index, size, size, out R, Zero, 1, 0) < 1) return Zero;
				return R;
			}

			/// <summary>
			/// Gets a shell stock icon handle.
			/// </summary>
			/// <param name="icon">Shell stock icon id.</param>
			/// <param name="size">Icon width and height.</param>
			public static IntPtr GetShellStockIconHandle(Api.SHSTOCKICONID icon, int size = 16)
			{
				int i; var s = _GetShellStockIconLocation(out i, icon);
				return s == null ? Zero : GetIconHandleRaw(s, i, size);
			}

			static unsafe string _GetShellStockIconLocation(out int index, Api.SHSTOCKICONID icon)
			{
				index = 0;
				var x = new Api.SHSTOCKICONINFO(); x.cbSize = Api.SizeOf(x);
				if(0 != Api.SHGetStockIconInfo(icon, 0, ref x)) return null;
				index = x.iIcon;
				return new string(x.szPath);
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

			/// <summary>
			/// Loads cursor from file.
			/// Returns cursor handle. Returns Zero if failed.
			/// Later call Api.DestroyCursor().
			/// </summary>
			/// <param name="file">.cur or .ani file.</param>
			public static IntPtr GetCursorHandle(string file)
			{
				return Api.LoadImage(Zero, Path_.MakeFullPath(file), Api.IMAGE_CURSOR, 32, 32, Api.LR_LOADFROMFILE);
			}

			/// <summary>
			/// Parses icon location string.
			/// Returns true if it includes icon index or resource id.
			/// </summary>
			/// <param name="location">Icon location. Can be "path,index" or "path,-id" or just path. Receives path.</param>
			/// <param name="index">Receives the number or 0.</param>
			/// <remarks>Also supports path enclosed in double quotes like "\"path\",index", and spaces between comma and index like "path, index".</remarks>
			public static bool ParseIconLocation(ref string location, out int index)
			{
				index = 0;
				var s = location;
				if(Empty(s)) return false;
				if(s[0] == '\"') s = s.Replace("\"", ""); //can be eg "path",index
				if(s.Length < 3) return false;
				if(!Calc.IsDigit(s[s.Length - 1])) return false;
				int e, i = s.LastIndexOf(','); if(i < 1) return false;
				index = s.ToInt_(i + 1, out e); if(e != s.Length) return false;
				location = s.Remove(i);
				return true;

				//note: PathParseIconLocation is buggy. Eg splits "path,5moreText". Or from "path, 5" removes ", 5" and returns 0.
			}

			/// <summary>
			/// Gets file icons asynchronously.
			/// Use to avoid waiting until all icons are extracted before displaying them in a UI (menu etc).
			/// Instead you show the UI without icons, and then asynchronously receive icons when they are extracted.
			/// At first call Add() for each file. Then call GetAllAsync().
			/// Create a callback function of type AsyncCallback and pass its delegate to GetAllAsync().
			/// </summary>
			public class AsyncIcons :IDisposable
			{
				public struct FileObj
				{
					public string file;
					public object obj;

					public FileObj(string file, object obj) { this.file = file; this.obj = obj; }
				}

				//never mind:
				//	Could instead use Dictionary<string, List<object>>, to avoid extracting icon multiple times for the same path or even file type (difficult).
				//	But better is simpler code. If one wants max speed, in most cases can use a saved imagelist instead of getting multiple icons each time.

				List<FileObj> _files = new List<FileObj>();
				List<_AsyncWork> _works = new List<_AsyncWork>();

				/// <summary>
				/// Adds a file path to an internal collection.
				/// </summary>
				/// <param name="file">File path etc to pass to <see cref="GetIconHandle"/>.</param>
				/// <param name="obj">Something to pass to your callback function together with icon handle for this file.</param>
				public void Add(string file, object obj)
				{
					if(Empty(file)) return;
					_files.Add(new FileObj(file, obj));
				}

				/// <summary>
				/// Adds multiple file paths to an internal collection.
				/// The same as calling Add(string, object) multiple times.
				/// This function copies the list.
				/// </summary>
				public void Add(IEnumerable<FileObj> files)
				{
					if(files != null) _files.AddRange(files);
				}

				/// <summary>
				/// Gets the number of items added with Add().
				/// GetAllAsync() sets it = 0.
				/// </summary>
				public int Count { get { return _files.Count; } }

				/// <summary>
				/// Used with GetAllAsync().
				/// </summary>
				/// <param name="workId">Work id, as returned by GetAllAsync().</param>
				/// <param name="hIcon">Icon handle. Will need Api.DestroyIcon(). Can be Zero.</param>
				/// <param name="objCommon">objCommon passed to GetAllAsync().</param>
				public delegate void AsyncCallback(IntPtr hIcon, object obj, object objCommon);

				/// <summary>
				/// Used with GetAllAsync().
				/// </summary>
				/// <param name="objCommon">objCommon passed to GetAllAsync().</param>
				public delegate void AsyncFinished(object objCommon);

				/// <summary>
				/// Starts getting icons of files added with Add().
				/// After this function returns, icons are asynchronously extracted with <see cref="GetIconHandle"/>, and callback called with icon handle (or Zero if failed).
				/// The callback is called in this thread. This thread must have a message loop (eg Application.Run()).
				/// If you'll need more icons, you can call Add() and GetAllAsync() again with the same _AsyncIcons instance, even if getting old icons if still not finished.
				/// </summary>
				/// <param name="callback">A callback function delegate.</param>
				/// <param name="iconSize">Icon width and height.</param>
				/// <param name="flags"><see cref="IconFlag"/></param>
				/// <param name="onFinished">A callback function to call once when finished getting all icons, except when canceled.</param>
				/// <param name="objCommon">Something to pass to callback functions.</param>
				public void GetAllAsync(AsyncCallback callback, int iconSize = 16, IconFlag flags = 0, AsyncFinished onFinished = null, object objCommon = null)
				{
					if(_files.Count == 0) return;
					var work = new _AsyncWork();
					_works.Add(work);
					work.GetAllAsync(this, callback, iconSize, flags, onFinished, objCommon);
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
					AsyncFinished _onFinished;
					object _objCommon;
					CancellationTokenSource _cancel;
					uint _tid;
					int _counter;

					internal void GetAllAsync(AsyncIcons host, AsyncCallback callback, int iconSize, IconFlag flags, AsyncFinished onFinished, object objCommon)
					{
						using(new Util.LibEnsureWindowsFormsSynchronizationContext()) {
							_host = host;
							_callback = callback;
							_iconSize = iconSize;
							_iconFlags = flags;
							_onFinished = onFinished;
							_objCommon = objCommon;
							_cancel = new CancellationTokenSource();
							_tid = Api.GetCurrentThreadId();
							_counter = _host._files.Count;

							foreach(var v in _host._files) {
								if(!Empty(v.file)) _GetIconAsync(v.file, v.obj);
							}
						}
					}

					async void _GetIconAsync(string file, object obj)
					{
						var cancTok = _cancel.Token;
						var task = Task.Run(() =>
						{
							if(cancTok.IsCancellationRequested) return Zero;
							//WaitMS(500);
							var R = GetIconHandle(file, _iconSize, _iconFlags);
							return R;
						}, cancTok);
						await task;

						//async continuation

						IntPtr hi = task.Result; bool badThread = false;
						if(cancTok.IsCancellationRequested || (badThread = (Api.GetCurrentThreadId() != _tid))) {
							Api.DestroyIcon(hi);
							if(badThread) OutDebug("wrong thread");
						} else {
							_callback(hi, obj, _objCommon); //even if hi == Zero, it can be useful
															//OutList(hi, _counter - 1);
							if(--_counter == 0) {
								if(_onFinished != null) _onFinished(_objCommon);
								_host._works.Remove(this);
								_host = null;
							}
						}
					}

					internal void Cancel()
					{
						if(_cancel == null) return;
						_cancel.Cancel();
						_cancel = null;
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

		public static class Misc
		{

			#region pidl

			/// <summary>
			/// Converts file path or URL or shell object parsing name to PIDL.
			/// Later call Marshal.FreeCoTaskMem();
			/// </summary>
			public static unsafe IntPtr PidlFromString(string s)
			{
				if(Empty(s)) return Zero;
				IntPtr R;
				if(0 != Api.SHParseDisplayName(s, Zero, out R, 0, null)) return Zero;
				return R;
				//the same as
				//Api.IShellFolder isf; if(0 != Api.SHGetDesktopFolder(out isf)) return Zero;
				//try { if(0 != isf.ParseDisplayName(Wnd0, Zero, s, null, out R, null)) return Zero; } finally { Marshal.ReleaseComObject(isf); }
			}

			/// <summary>
			/// Converts PIDL to file path or URL or shell object parsing name.
			/// Returns null if pidl is Zero or failed.
			/// </summary>
			/// <param name="stringType">
			/// A value from the <see cref="Api.SIGDN"/> enumeration that specifies the type of string to retrieve.
			/// With the default value returns string that can be passed to PidlFromString. It can be a path, URL, "::{CLSID}", etc.
			/// Other often used values:
			/// Api.SIGDN.SIGDN_FILESYSPATH - returns null if pidl is not of a file system object.
			/// Api.SIGDN.SIGDN_URL - returns null if pidl is not of a URL or path. If path, returns its URL form, like "file:///C:/a/b.txt".
			/// Api.SIGDN.SIGDN_NORMALDISPLAY - returns string that is best to display in UI but cannot be passed to PidlFromString.
			/// </param>
			public static string PidlToString(IntPtr pidl, Api.SIGDN stringType = Api.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING)
			{
				string R;
				if(0 != Api.SHGetNameFromIDList(pidl, stringType, out R)) return null;
				return R;
			}

			#endregion

			/// <summary>
			/// Gets HKEY_CLASSES_ROOT registry key of file type or protocol.
			/// The key usually contains subkeys "shell", "DefaultIcon", sometimes "shellex" and more.
			/// For example, for ".txt" can return "txtfile", for ".cs" - "VisualStudio.cs.14.0".
			/// Looks in "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts" and in HKEY_CLASSES_ROOT.
			/// Returns null if the type/protocol is not registered.
			/// Returns null if fileType does not end with ".extension" and does not start with "protocol:"; also if starts with "shell:".
			/// </summary>
			/// <param name="fileType">
			/// File type extension like ".txt" or protocol like "http:".
			/// Can be full path or URL; the function gets extension or protocol from the string.
			/// Can start with %environment variable%.
			/// </param>
			/// <param name="isFileType">Don't parse fileType, it does not contain full path or URL or environment variables. It is ".ext" or "protocol:".</param>
			/// <param name="isURL">fileType is URL or protocol like "http:". Used only if isFileType == true, ie it is protocol.</param>
			internal static string GetFileTypeOrProtocolRegistryKey(string fileType, bool isFileType, bool isURL)
			{
				if(!isFileType) fileType = GetExtensionOrProtocol(fileType, out isURL);
				else if(isURL) fileType = fileType.Remove(fileType.Length - 1); //"proto:" -> "proto"
				if(Empty(fileType)) return null;

				string R, userChoiceKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + fileType + @"\UserChoice";
				if(Registry_.GetString(out R, "ProgId", userChoiceKey)) return R;
				if(isURL) return fileType;
				if(Registry_.GetString(out R, "", fileType, Registry.ClassesRoot)) return R;
				return null;

				//note: IQueryAssociations.GetKey is very slow.
			}

			/// <summary>
			/// Gets file path extension like ".txt" or URL protocol like "http".
			/// Returns null if path does not end with ".extension" and does not start with "protocol:"; also if starts with "shell:".
			/// </summary>
			/// <param name="path">File path or URL. Can be just extension like ".txt" or protocol like "http:".</param>
			/// <param name="isProtocol">Receives true if URL or protocol.</param>
			internal static string GetExtensionOrProtocol(string path, out bool isProtocol)
			{
				isProtocol = false;
				if(Empty(path)) return null;
				if(!PathIsExtension(path)) {
					int i = path.IndexOf(':');
					if(i > 1) {
						path = path.Remove(i); //protocol
						if(path == "shell") return null; //eg "shell:AppsFolder\Microsoft.WindowsCalculator_8wekyb3d8bbwe!App"
						isProtocol = true;
					} else {
						try { path = Path.GetExtension(path); } catch { return null; }
						if(Empty(path)) return null;
					}
				}
				return path;
			}

			/// <summary>
			/// Returns true if path is like ".ext" and the ext part does not contain characters ".\\/:".
			/// </summary>
			internal static bool PathIsExtension(string path)
			{
				if(path == null || path.Length < 2 || path[0] != '.') return false;
				return path.IndexOfAny_(".\\/:", 1) < 0;
			}

			/// <summary>
			/// Returns true if path is like "protocol:" and not like "c:".
			/// </summary>
			internal static bool PathIsProtocol(string path)
			{
				if(path == null) return false;
				return path.Length >= 3 && path[path.Length - 1] == ':';
			}
		}
	}
}
