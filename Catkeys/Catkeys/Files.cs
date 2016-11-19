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

		/// <summary>
		/// Replaces characters that cannot be used in file names.
		/// Also corrects other forms of invalid or problematic filename (except invalid length): trims spaces and other blank characters; replaces "." at the end; prepends "@" if just an extension like ".txt" or a reserved name like "CON" or "CON.txt".
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
					if(name.RegexIs_(@"(?i)^(CON|PRN|AUX|NUL|COM\d|LPT\d)(\.|$)|^\.[^\.]+$")) name = "@" + name;
				}
			}
			return name;
		}
	}

	//[DebuggerStepThrough]
	public static partial class Files
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
