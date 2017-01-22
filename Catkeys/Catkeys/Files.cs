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
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Extends the .NET class Path.
	/// </summary>
	public static class Path_
	{
		/// <summary>
		/// If path starts with '%' character, expands environment variables enclosed in %, else just returns path.
		/// </summary>
		/// <seealso cref="Environment.ExpandEnvironmentVariables"/>
		public static string ExpandEnvVar(string path)
		{
			if(Empty(path) || path[0] != '%') return path;
			return Environment.ExpandEnvironmentVariables(path); //very slow, even if there are no '%'

			//this would make > 2 times faster, but never mind, in this case better is smaller code
			//int n = path.Length + 100, r;
			//var b = new StringBuilder(n);
			//for(;;) {
			//	r = (int)ExpandEnvironmentStrings(path, b, (uint)n);
			//	//PrintList(n, r, b.Length, b);
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
		///		@"&lt;*&gt;*" - special string that can be used with some functions of this library.
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
		/// If path is not full path, returns Folders.ThisApp+path.
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
		///	If the return value would not be full path, prepends Folders.ThisApp.
		/// If s1 and s2 are null or "", returns "".
		///	No exceptions.
		/// </remarks>
		public static string Combine(string s1, string s2)
		{
			string R = null;
			int len2 = (s2 == null) ? 0 : s2.Length;

			if(Empty(s1)) {
				if(len2 == 0) return "";
				R = s2;
			} else {
				if(len2 == 0) R = s1.TrimEnd('\\');
				else if(s2[0] == '\\') R = s1.TrimEnd('\\') + s2;
				else if(s1.EndsWith_("\\")) R = s1 + s2;
				else R = s1 + @"\" + s2;
			}

			R = ExpandEnvVar(R);

			if(!IsFullPath(R)) R = Folders.ThisApp + R;
			else if(R[0] == ':' || R[0] == '<' || R[0] == '%') return R;

			return _CorrectDotsAndDOS(R);
		}

		internal static string _CorrectDotsAndDOS(string path)
		{
			Debug.Assert(!Empty(path));
			if(path.IndexOf_(@".\") >= 0 || path.EndsWith_(".") || path.IndexOf('~') >= 0) {
				try { path = Path.GetFullPath(path); } catch { }
			}
			return path;
		}

		//Better don't use such things. If need, a function can instead have a parameter for it.
		//public static string ThreadDefaultDirectory
		//{
		//	get { return _threadDefDir ?? (_threadDefDir = Folders.ThisApp); }
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

		/// <summary>
		/// Replaces characters that cannot be used in file names.
		/// Also corrects other forms of invalid or problematic filename (except invalid length): trims spaces and other blank characters; replaces "." at the end; prepends "@" if a reserved name like "CON" or "CON.txt".
		/// Returns valid filename. Returns null if name is null. Returns "" if name is "" or contains just blank characters.
		/// Does not check name length; no correction or exception if it is 0 (or null) or greater than max filename length which is less than max path length 259.
		/// </summary>
		/// <param name="name">Initial filename.</param>
		/// <param name="invalidCharReplacement">A string that will replace each invalid character. Default "-".</param>
		public static string CorrectFileName(string name, string invalidCharReplacement = "-")
		{
			if(name != null) {
				name = name.Trim();
				if(name.Length > 0) {
					name = name.RegexReplace_(@"\.$|[\\/|<>?*:""\x00-\x1f]", invalidCharReplacement).Trim();
					if(name.RegexIs_(@"(?i)^(CON|PRN|AUX|NUL|COM\d|LPT\d)(\.|$)")) name = "@" + name;
				}
			}
			return name;
		}

		internal class Internal
		{
			/// <summary>
			/// Returns true if path is like ".ext" and the ext part does not contain characters ".\\/:".
			/// </summary>
			internal static bool IsExtension(string path)
			{
				if(path == null || path.Length < 2 || path[0] != '.') return false;
				return path.IndexOfAny_(".\\/:", 1) < 0;
			}

			/// <summary>
			/// Returns true if path is like "protocol:" and not like "c:".
			/// </summary>
			internal static bool IsProtocol(string path)
			{
				if(path == null) return false;
				return path.Length >= 3 && path[path.Length - 1] == ':';
			}
		}
	}

	/// <summary>
	/// File-system functions.
	/// </summary>
	//[DebuggerStepThrough]
	public static partial class Files
	{
		#region exists, search

		/// <summary>
		/// Returns non-zero if file or directory (folder or drive) exists: 1 file, 2 directory.
		/// Returns 0 if does not exist or path is empty or invalid. No exceptions.
		/// Uses Api.GetFileAttributes().
		/// </summary>
		/// <param name="path">Full path. Supports "%EnvVar%path" and "path\..\path".</param>
		public static int FileOrDirectory(string path)
		{
			if(Empty(path)) return 0;
			int r = (int)Api.GetFileAttributes(Path_.ExpandEnvVar(path));
			if(r == -1) return 0;
			var a = (FileAttributes)r;
			return a.HasFlag(FileAttributes.Directory) ? 2 : 1;
		}

		/// <summary>
		/// Returns true if file or directory exists.
		/// Uses FileOrDirectory(), which calls Api.GetFileAttributes().
		/// </summary>
		/// <param name="path">Full path. Supports "%EnvVar%path" and "path\..\path".</param>
		public static bool FileOrDirectoryExists(string path)
		{
			return FileOrDirectory(path) != 0;
		}

		/// <summary>
		/// Returns true if file exists and is not a directory.
		/// Uses FileOrDirectory(), which calls Api.GetFileAttributes().
		/// </summary>
		/// <param name="path">Full path. Supports "%EnvVar%path" and "path\..\path".</param>
		public static bool FileExists(string path)
		{
			return FileOrDirectory(path) == 1;
		}

		/// <summary>
		/// Returns true if directory (folder or drive) exists.
		/// Uses FileOrDirectory(), which calls Api.GetFileAttributes().
		/// </summary>
		/// <param name="path">Full path. Supports "%EnvVar%path" and "path\..\path".</param>
		public static bool DirectoryExists(string path)
		{
			return FileOrDirectory(path) == 2;
		}

		/// <summary>
		/// Finds file or directory and returns full and fully-qualified path.
		/// Returns null if cannot be found.
		/// If the path argument is not full path, searches in these places:
		///	1. defDirs, if used.
		/// 2. <see cref="Folders.ThisApp"/>.
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
				if(FileOrDirectoryExists(path)) return path;
				return null;
			}

			if(dirs != null) {
				foreach(var d in dirs) {
					if(Empty(d)) continue;
					var s = Path_.Combine(d, path);
					if(FileOrDirectoryExists(s)) return s;
				}
			}

			{
				var s = Folders.ThisApp + path;
				if(FileOrDirectoryExists(s)) return s;
			}

			var sb = new StringBuilder(300);
			if(0 != Api.SearchPath(null, path, null, 300, sb, Zero)) return sb.ToString();

			if(path.EndsWith_(".exe", true)) {
				string rk = @"Software\Microsoft\Windows\CurrentVersion\App Paths\" + path;
				if(Registry_.GetString(out path, "", rk) || Registry_.GetString(out path, "", rk, Registry.LocalMachine)) {
					path = Path_.MakeFullPath(path.Trim('\"'));
					if(FileOrDirectoryExists(path)) return path;
				}
			}

			return null;
		}

		#endregion

		/// <summary>
		/// Creates shell shortcuts (.lnk files) and gets shortcut properties.
		/// </summary>
		public class LnkShortcut :IDisposable
		{
			//info: name Shortcut used in .NET.
			//TODO: document exceptions.

			Api.IShellLink _isl;
			Api.IPersistFile _ipf;
			string _lnkPath;
			bool _isOpen;
			bool _changedHotkey;

			/// <summary>
			/// Releases internally used COM objects (IShellLink, IPersistFile).
			/// </summary>
			public void Dispose()
			{
				if(_isl != null) {
					Api.ReleaseComObject(_ipf); _ipf = null;
					Api.ReleaseComObject(_isl); _isl = null;
				}
			}
			//~LnkShortcut() { Dispose(); } //don't need, we have only COM objects, GC will release them anyway

			/// <summary>
			/// Returns the internally used IShellLink COM interface.
			/// </summary>
			internal Api.IShellLink IShellLink { get { return _isl; } }
			//This could be public, but then need to make IShellLink public. It is defined in a non-standard way. Never mind, it is not important.

			LnkShortcut(string lnkPath, uint mode)
			{
				_isl = new Api.ShellLink() as Api.IShellLink;
				_ipf = _isl as Api.IPersistFile;
				_lnkPath = lnkPath;
				if(mode != Api.STGM_WRITE && (mode == Api.STGM_READ || FileExists(_lnkPath))) {
					int hr = _ipf.Load(_lnkPath, mode);
					if(hr != 0) throw new COMException("Failed to open .lnk file.", hr);
					_isOpen = true;
				}
			}

			/// <summary>
			/// Creates a new instance of the LnkShortcut class that can be used to get shortcut properties.
			/// Exception if shortcut file does not exist or cannot open it for read access.
			/// </summary>
			/// <param name="lnkPath">LnkShortcut (.lnk) file path.</param>
			public static LnkShortcut Open(string lnkPath)
			{
				return new LnkShortcut(lnkPath, Api.STGM_READ);
			}

			/// <summary>
			/// Creates a new instance of the LnkShortcut class that can be used to create or replace a shortcut file.
			/// You can set properties and finally call Save().
			/// If the shortcut file already exists, Save() replaces it.
			/// </summary>
			/// <param name="lnkPath">LnkShortcut (.lnk) file path.</param>
			public static LnkShortcut Create(string lnkPath)
			{
				return new LnkShortcut(lnkPath, Api.STGM_WRITE);
			}

			/// <summary>
			/// Creates a new instance of the LnkShortcut class that can be used to create or modify a shortcut file.
			/// Exception if file exists but cannot open it for read-write access.
			/// You can get and set properties and finally call Save().
			/// If the shortcut file already exists, Save() updates it.
			/// </summary>
			/// <param name="lnkPath">LnkShortcut (.lnk) file path.</param>
			public static LnkShortcut OpenOrCreate(string lnkPath)
			{
				return new LnkShortcut(lnkPath, Api.STGM_READWRITE);
			}

			/// <summary>
			/// Saves the LnkShortcut variable properties to the shortcut file.
			/// </summary>
			public void Save()
			{
				if(_changedHotkey && !_isOpen && FileExists(_lnkPath)) _UnregisterHotkey(_lnkPath);

				int hr = _ipf.Save(_lnkPath, true);
				if(hr != 0) throw new COMException("Failed to save .lnk file.", hr);
				//Marshal.ThrowExceptionForHR()
			}

			/// <summary>
			/// Gets or sets shortcut target path.
			/// This property is null if target isn't a file system object, eg Control Panel or URL; use TargetIDList or TargetURL.
			/// </summary>
			/// <remarks>The 'get' function gets path with expanded environment variables. If possible, it corrects the target of MSI shortcuts and 64-bit Program Files shortcuts where IShellLink.GetPath() lies.</remarks>
			public string TargetPath
			{
				get
				{
					var sb = new StringBuilder(300);
					if(0 != _isl.GetPath(sb, 300)) return null;
					return _CorrectPath(sb, true);
				}
				set
				{
					_isl.SetPath(value);
				}
			}

			/// <summary>
			/// Gets shortcut target path and does not correct wrong MSI shortcut target.
			/// </summary>
			public string TargetPathRawMSI
			{
				get
				{
					var sb = new StringBuilder(300);
					if(0 != _isl.GetPath(sb, 300)) return null;
					return _CorrectPath(sb);
				}
			}

			/// <summary>
			/// Gets or sets a non-file-system target (eg Control Panel) through its ITEMIDLIST.
			/// Use Marshal.FreeCoTaskMem() to free the return value of the 'get' function.
			/// </summary>
			/// <remarks>
			/// Also can be used for any target type, but gets raw value, for example MSI shortcut target is incorrect.
			/// Most but not all shortcuts have this property; returns Zero if the shortcut does not have it.
			/// </remarks>
			public IntPtr TargetIDList
			{
				get
				{
					IntPtr pidl; if(0 != _isl.GetIDList(out pidl)) return Zero;
					return pidl;
				}
				set
				{
					_isl.SetIDList(value);
				}
			}

			/// <summary>
			/// Gets or sets a URL target.
			/// Note: it is a .lnk shortcut, not a .url shortcut.
			/// The 'get' function returns string "file:///..." if target is a file.
			/// </summary>
			public string TargetURL
			{
				get
				{
					IntPtr pidl; if(0 != _isl.GetIDList(out pidl)) return null;
					try { return Misc.PidlToString(pidl, Native.SIGDN.SIGDN_URL); } finally { Marshal.FreeCoTaskMem(pidl); }
				}
				set
				{
					var pidl = Misc.PidlFromString(value); if(pidl == Zero) throw new CatException();
					try { _isl.SetIDList(pidl); } finally { Marshal.FreeCoTaskMem(pidl); }
				}
			}

			/// <summary>
			/// Gets or sets target of any type - file/folder path, virtual shell object parsing name, URL.
			/// The string can be used with the shell execute function.
			/// Virtual object string can be like "::{CLSID}".
			/// </summary>
			public string TargetAnyType
			{
				get
				{
					var R = TargetPath; if(R != null) return R; //support MSI etc
					IntPtr pidl; if(0 != _isl.GetIDList(out pidl)) return null;
					try { return Misc.PidlToString(pidl); } finally { Marshal.FreeCoTaskMem(pidl); }
				}
				set
				{
					var pidl = Misc.PidlFromString(value); if(pidl == Zero) throw new CatException();
					try { _isl.SetIDList(pidl); } finally { Marshal.FreeCoTaskMem(pidl); }
				}
			}

			/// <summary>
			/// Gets custom icon file path and icon index.
			/// Returns null if the shortcut does not have a custom icon (then you see its target icon).
			/// </summary>
			/// <param name="iconIndex">Receives 0 or icon index or negative icon resource id.</param>
			public string GetIconLocation(out int iconIndex)
			{
				var sb = new StringBuilder(300);
				if(0 != _isl.GetIconLocation(sb, 300, out iconIndex)) return null;
				return _CorrectPath(sb);
			}

			/// <summary>
			/// Sets icon file path and icon index.
			/// </summary>
			/// <param name="path"></param>
			/// <param name="iconIndex">0 or icon index or negative icon resource id.</param>
			public void SetIconLocation(string path, int iconIndex = 0)
			{
				_isl.SetIconLocation(path, iconIndex);
			}

			/// <summary>
			/// Gets or sets the working directory path (Start in).
			/// </summary>
			public string WorkingDirectory
			{
				get
				{
					var sb = new StringBuilder(300);
					if(0 != _isl.GetWorkingDirectory(sb, 300)) return null;
					return _CorrectPath(sb);
				}
				set
				{
					_isl.SetWorkingDirectory(value);
				}
			}

			/// <summary>
			/// Gets or sets the command-line arguments.
			/// </summary>
			public string Arguments
			{
				get
				{
					var sb = new StringBuilder(1024);
					if(0 != _isl.GetArguments(sb, 1024) || 0 == sb.Length) return null;
					return sb.ToString();
				}
				set
				{
					_isl.SetArguments(value);
				}
			}

			/// <summary>
			/// Gets or sets the description text (Comment).
			/// </summary>
			public string Description
			{
				get
				{
					var sb = new StringBuilder(1024);
					if(0 != _isl.GetDescription(sb, 1024) || 0 == sb.Length) return null; //info: in my tests was E_FAIL for 1 shortcut (Miracast)
					return sb.ToString();
				}
				set
				{
					_isl.SetDescription(value);
				}
			}

			/// <summary>
			/// Gets or sets hotkey.
			/// Example: <c>x.Hotkey = Keys.Control | Keys.Alt | Keys.E;</c>
			/// </summary>
			public Keys Hotkey
			{
				get
				{
					ushort k2; if(0 != _isl.GetHotkey(out k2)) return 0;
					uint k = k2;
					return (Keys)((k & 0xFF) | ((k & 0x700) << 8));
				}
				set
				{
					uint k = (uint)value;
					_isl.SetHotkey((ushort)((k & 0xFF) | ((k & 0x70000) >> 8)));
					_changedHotkey = true;
				}
			}

			/// <summary>
			/// Gets or sets the window show state.
			/// The value can be Api.SW_SHOWNORMAL (default), Api.SW_SHOWMAXIMIZED or Api.SW_SHOWMINIMIZED.
			/// Most programs ignore it.
			/// </summary>
			public int ShowState
			{
				get { int R; if(0 != _isl.GetShowCmd(out R)) R = Api.SW_SHOWNORMAL; return R; }
				set { _isl.SetShowCmd(value); }
			}

			//Not implemented wrappers for these IShellLink methods:
			//SetRelativePath, Resolve - not useful.
			//All are easy to call through the IShellLink property.

			#region public static

			/// <summary>
			/// Gets shortcut target path or URL or virtual shell object parsing name.
			/// Uses <see cref="TargetAnyType"/>.
			/// </summary>
			/// <param name="lnkPath">LnkShortcut (.lnk) file path.</param>
			public static string GetTarget(string lnkPath)
			{
				return Open(lnkPath).TargetAnyType;
			}

			/// <summary>
			/// If shortcut file exists, unregisters its hotkey and deletes it.
			/// </summary>
			/// <param name="lnkPath">.lnk file path.</param>
			public static void Delete(string lnkPath)
			{
				if(!FileExists(lnkPath)) return;
				_UnregisterHotkey(lnkPath);
				File.Delete(lnkPath);
			}

			#endregion
			#region private

			static void _UnregisterHotkey(string lnkPath)
			{
				Debug.Assert(FileExists(lnkPath));
				using(var x = OpenOrCreate(lnkPath)) {
					var k = x.Hotkey;
					if(k != 0) {
						x.Hotkey = 0;
						x.Save();
					}
				}
			}

			string _CorrectPath(StringBuilder sb, bool fixMSI = false)
			{
				if(sb.Length == 0) return null;
				string R = sb.ToString();

				if(!fixMSI) {
					R = Path_.ExpandEnvVar(R);
				} else if(R.IndexOf_(@"\Installer\{") > 0) {
					//For MSI shortcuts GetPath gets like "C:\WINDOWS\Installer\{90110409-6000-11D3-8CFE-0150048383C9}\accicons.exe".
					var product = new StringBuilder(40);
					var component = new StringBuilder(40);
					if(0 != _Api.MsiGetShortcutTarget(_lnkPath, product, null, component)) return null;
					//note: for some shortcuts MsiGetShortcutTarget gets empty component. Then MsiGetComponentPath fails.
					//	On my PC was 1 such shortcut - Microsoft Office Excel Viewer.lnk in start menu.
					//	Could not find a workaround.

					int n = 300; sb.EnsureCapacity(n);
					int hr = _Api.MsiGetComponentPath(product.ToString(), component.ToString(), sb, ref n);
					if(hr < 0 || sb.Length == 0) return null; //eg not installed, just advertised

					R = sb.ToString();
					//note: can be a registry key instead of file path. No such shortcuts on my PC.
				}

				//GetPath problem: replaces "c:\program files" to "c:\program files (x86)".
				//These don't help: SLGP_RAWPATH, GetIDList, disabled redirection.
				//GetWorkingDirectory and GetIconLocation get raw path, and envronment variables such as %ProgramFiles% are expanded to (x86) in 32-bit process.
				if(!Environment.Is64BitProcess && Environment.Is64BitOperatingSystem) {
					if(_pf == null) { string s = Folders.ProgramFilesX86; _pf = s + "\\"; }
					if(R.StartsWith_(_pf, true) && !FileOrDirectoryExists(R)) {
						var s2 = R.Remove(_pf.Length - 7, 6);
						if(FileOrDirectoryExists(s2)) R = s2;
						//info: "C:\\Program Files (x86)\\" in English, "C:\\Programme (x86)\\" in German etc.
						//never mind: System32 folder also has similar problem, because of redirection.
						//note: ShellExecuteEx also has this problem.
					}
				}

				return R;
			}
			static string _pf;

			string _GetMsiShortcutTarget()
			{
				var product = new StringBuilder(40);
				var component = new StringBuilder(40);
				if(0 != _Api.MsiGetShortcutTarget(_lnkPath, product, null, component)) return null;
				//note: for some shortcuts MsiGetShortcutTarget gets empty component. Then MsiGetComponentPath fails.
				//	On my PC was 1 such shortcut - Microsoft Office Excel Viewer.lnk in start menu.
				//	Could not find a workaround.

				int n = 300; var sb = new StringBuilder(n);
				int hr = _Api.MsiGetComponentPath(product.ToString(), component.ToString(), sb, ref n);
				if(hr < 0 || sb.Length == 0) return null; //eg not installed, just advertised

				return sb.ToString();
				//note: can be a registry key instead of file path. No such shortcuts on my PC.
			}

			static partial class _Api
			{
				[DllImport("msi.dll", EntryPoint = "#217")]
				public static extern int MsiGetShortcutTarget(string szShortcutPath, [Out] StringBuilder szProductCode, [Out] StringBuilder szFeatureId, [Out] StringBuilder szComponentCode);

				[DllImport("msi.dll", EntryPoint = "#173")]
				public static extern int MsiGetComponentPath(string szProduct, string szComponent, [Out] StringBuilder lpPathBuf, ref int pcchBuf);

				//MsiGetComponentPath returns:
				//public enum INSTALLSTATE
				//{
				//	INSTALLSTATE_NOTUSED = -7,
				//	INSTALLSTATE_BADCONFIG,
				//	INSTALLSTATE_INCOMPLETE,
				//	INSTALLSTATE_SOURCEABSENT,
				//	INSTALLSTATE_MOREDATA,
				//	INSTALLSTATE_INVALIDARG,
				//	INSTALLSTATE_UNKNOWN,
				//	INSTALLSTATE_BROKEN,
				//	INSTALLSTATE_ADVERTISED,
				//	INSTALLSTATE_REMOVED = 1,
				//	INSTALLSTATE_ABSENT,
				//	INSTALLSTATE_LOCAL,
				//	INSTALLSTATE_SOURCE,
				//	INSTALLSTATE_DEFAULT
				//}
			}

			#endregion
		}

		/// <summary>
		/// Miscellaneous functions.
		/// </summary>
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
			/// <param name="pidl"></param>
			/// <param name="stringType">
			/// A value from the <see cref="Native.SIGDN"/> enumeration that specifies the type of string to retrieve.
			/// With the default value returns string that can be passed to PidlFromString. It can be a path, URL, "::{CLSID}", etc.
			/// Other often used values:
			/// Native.SIGDN.SIGDN_FILESYSPATH - returns null if pidl is not of a file system object.
			/// Native.SIGDN.SIGDN_URL - returns null if pidl is not of a URL or path. If path, returns its URL form, like "file:///C:/a/b.txt".
			/// Native.SIGDN.SIGDN_NORMALDISPLAY - returns string that is best to display in UI but cannot be passed to PidlFromString.
			/// </param>
			public static string PidlToString(IntPtr pidl, Native.SIGDN stringType = Native.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING)
			{
				string R;
				if(0 != Api.SHGetNameFromIDList(pidl, stringType, out R)) return null;
				return R;
			}

			#endregion

#if false //currently not used
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
#endif
		}
	}
}
