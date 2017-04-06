//#define TEST_FINDFIRSTFILEEX

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
using System.Linq;

//using VB = Microsoft.VisualBasic.FileIO;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// File system functions.
	/// Works with files and directories. Disk drives like @"C:\" or "C:" are directories too.
	/// Extends .NET file system classes such as File and Directory.
	/// Many functions of this class can be used instead of existing similar .NET functions that are slow, limited or unreliable.
	/// Most functions support only full path, and throw ArgumentException if passed a filename or relative path, ie in "current directory". Using current directory is unsafe; it was relevant only in DOS era.
	/// Most functions support extended-length paths (longer than 259). Such local paths should have @"\\?\" prefix, like @"\\?\C:\...". Such network path should be like @"\\?\UNC\server\share\...". See <see cref="Path_.PrefixLongPath(string)"/>. Most functions support long paths even without prefix.
	/// </summary>
	public static partial class Files
	{
		#region attributes, exists, search, enum

		/// <summary>
		/// Adds SEM_FAILCRITICALERRORS to the error mode of this process, as MSDN recommends. Does this once in appdomain.
		/// It is to avoid unnecessary message boxes when an API tries to access an ejected CD/DVD etc.
		/// </summary>
		static void _DisableDeviceNotReadyMessageBox()
		{
			if(_disabledDeviceNotReadyMessageBox) return;
			var em = Api.GetErrorMode();
			if(0 == (em & Api.SEM_FAILCRITICALERRORS)) Api.SetErrorMode(em | Api.SEM_FAILCRITICALERRORS);
			_disabledDeviceNotReadyMessageBox = true;
		}
		static bool _disabledDeviceNotReadyMessageBox;

		/// <summary>
		/// File or directory properties. Used with <see cref="GetProperties"/>.
		/// </summary>
		public struct FileProperties
		{
			///
			public FileAttributes Attributes;
			///<summary>File size. For directories it is usually 0.</summary>
			public long Size;
			///
			public DateTime LastWriteTimeUtc;
			///
			public DateTime CreationTimeUtc;
			///<summary>Note: this is unreliable. The operating system may not record this time automatically.</summary>
			public DateTime LastAccessTimeUtc;
		}

		/// <summary>
		/// Flags for <see cref="GetAttributes"/> and some other functions.
		/// </summary>
		[Flags]
		public enum GAFlags
		{
			///<summary>Pass path to the API as it is, without any normalizing and validating.</summary>
			RawPath = 1,
			///<summary>
			///If failed, return false and don't throw exception.
			///Then, if you need error info, you can use <see cref="Native.GetError"/>. If the file/directory does not exist, it will return ERROR_FILE_NOT_FOUND or ERROR_PATH_NOT_FOUND or ERROR_NOT_READY.
			///If failed and the native error code is ERROR_ACCESS_DENIED or ERROR_SHARING_VIOLATION, the returned attributes will be (FileAttributes)(-1). The file probably exists but is protected so that this process cannot access and use it. Else attributes will be 0.
			///</summary>
			DoNotThrow = 2,
		}

		/// <summary>
		/// Gets file or directory attributes, size and times.
		/// Returns false if the file/directory does not exist.
		/// Calls API <msdn>GetFileAttributesEx</msdn>.
		/// </summary>
		/// <param name="path">Full path. Supports @"\.." etc. If flag RawPath not used, supports environment variables and non-pefixed extended-lenght path.</param>
		/// <param name="properties">Receives properties.</param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Not full path. Not thrown if used flag RawPath.</exception>
		/// <exception cref="CatException">The file/directory exist but failed to get its properties. Not thrown if used flag DoNotThrow.</exception>
		/// <remarks>
		/// For symbolic links etc, gets properties of the link, not of its target.
		/// You can also get most of these properties with <see cref="EnumDirectory"/>.
		/// </remarks>
		public static bool GetProperties(string path, out FileProperties properties, GAFlags flags = 0)
		{
			properties = new FileProperties();
			if(0 == (flags & GAFlags.RawPath)) path = Path_.LibExpandEV_CheckFullPath_PrefixLong(path); //don't need LibNormalizeExpandEV, the API itself supports .. etc
			_DisableDeviceNotReadyMessageBox();
			Api.WIN32_FILE_ATTRIBUTE_DATA d;
			if(!Api.GetFileAttributesEx(path, 0, out d)) {
				properties.Attributes = _GetAttributesOnError(path, flags);
				return false;
			}
			properties.Attributes = (FileAttributes)d.dwFileAttributes;
			properties.Size = (long)d.nFileSizeHigh << 32 | d.nFileSizeLow;
			properties.LastWriteTimeUtc = DateTime.FromFileTimeUtc(d.ftLastWriteTime);
			properties.CreationTimeUtc = DateTime.FromFileTimeUtc(d.ftCreationTime);
			properties.LastAccessTimeUtc = DateTime.FromFileTimeUtc(d.ftLastAccessTime);
			return true;
		}

		/// <summary>
		/// Gets file or directory attributes.
		/// Returns false if the file/directory does not exist.
		/// Calls API <msdn>GetFileAttributes</msdn>.
		/// </summary>
		/// <param name="path">Full path. Supports @"\.." etc. If flag RawPath not used, supports environment variables and non-pefixed extended-lenght path.</param>
		/// <param name="attributes">Receives attributes, or 0 if failed.</param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Not full path. Not thrown if used flag RawPath.</exception>
		/// <exception cref="CatException">Failed. Not thrown if used flag DoNotThrow.</exception>
		/// <remarks>
		/// For symbolic links etc, gets properties of the link, not of its target.
		/// </remarks>
		public static bool GetAttributes(string path, out FileAttributes attributes, GAFlags flags = 0)
		{
			attributes = 0;
			if(0 == (flags & GAFlags.RawPath)) path = Path_.LibExpandEV_CheckFullPath_PrefixLong(path); //don't need LibNormalizeExpandEV, the API itself supports .. etc
			_DisableDeviceNotReadyMessageBox();
			uint a = Api.GetFileAttributes(path);
			if(a == Api.INVALID_FILE_ATTRIBUTES) {
				attributes = _GetAttributesOnError(path, flags);
				return false;
			}
			attributes = (FileAttributes)a;
			return true;
		}

		static FileAttributes _GetAttributesOnError(string path, GAFlags flags)
		{
			var ec = Native.GetError();
			if(!(ec == Api.ERROR_FILE_NOT_FOUND || ec == Api.ERROR_PATH_NOT_FOUND || ec == Api.ERROR_NOT_READY)) {
				if(0 == (flags & GAFlags.DoNotThrow)) throw new CatException(ec, $"*get file attributes: '{path}'");
				if(ec == Api.ERROR_ACCESS_DENIED || ec == Api.ERROR_SHARING_VIOLATION) return (FileAttributes)(-1);
				//tested:
				//	If the file is in a protected directory, ERROR_ACCESS_DENIED.
				//	If the path is to a non-existing file in a protected directory, ERROR_FILE_NOT_FOUND.
				//	ERROR_SHARING_VIOLATION for C:\pagefile.sys etc.
			}
			return 0;
		}

		/// <summary>
		/// File system entry type - file, directory, and whether it exists.
		/// Returned by <see cref="ExistsAs"/>.
		/// </summary>
		public enum ItIs
		{
			/// <summary>Does not exist, or failed to get attributes.</summary>
			NotFound = 0,
			/// <summary>Is file, or symbolic link to a file.</summary>
			File = 1,
			/// <summary>Is directory, or symbolic link to a directory.</summary>
			Directory = 2,
		}

		/// <summary>
		/// Gets file system entry type - file, directory, and whether it exists.
		/// Returns NotFound (0) if does not exist or if fails to get attributes.
		/// Calls API <msdn>GetFileAttributes</msdn>.
		/// </summary>
		/// <param name="path">Full path. Supports @"\.." etc. If rawPath is false (default), supports environment variables and non-pefixed extended-lenght path.</param>
		/// <param name="rawPath">Pass path to the API as it is, without any normalizing and validating.</param>
		/// <exception cref="ArgumentException">Not full path. Not thrown if rawPath is true.</exception>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>. If you need exception when fails to get attributes, instead use <see cref="GetAttributes"/>; it returns false if the file/directory does not exist, and throws exception if fails to get attributes; directories have Directory attribute.
		/// </remarks>
		public static ItIs ExistsAs(string path, bool rawPath = false)
		{
			var flags = GAFlags.DoNotThrow; if(rawPath) flags |= GAFlags.RawPath;
			if(!GetAttributes(path, out var a, flags)) return ItIs.NotFound;
			var R = (0 != (a & FileAttributes.Directory)) ? ItIs.Directory : ItIs.File;
			return R;
		}

		/// <summary>
		/// File system entry type - file, directory, symbolic link, whether it exists and is accessible.
		/// Returned by <see cref="ExistsAs2"/>.
		/// The enum value NotFound is 0; AccessDenied is negative ((int)0x80000000); other values are greater than 0.
		/// </summary>
		public enum ItIs2
		{
			/// <summary>Does not exist.</summary>
			NotFound = 0,
			/// <summary>Is file. Attributes: Directory no, ReparsePoint no.</summary>
			File = 1,
			/// <summary>Is directory. Attributes: Directory yes, ReparsePoint no.</summary>
			Directory = 2,
			/// <summary>Is symbolic link to a file. Attributes: Directory no, ReparsePoint yes.</summary>
			SymLinkFile = 5,
			/// <summary>Is symbolic link to a directory, or is a mounted folder. Attributes: Directory yes, ReparsePoint yes.</summary>
			SymLinkDirectory = 6,
			/// <summary>Exists but this process cannot access it and get attributes.</summary>
			AccessDenied = int.MinValue,
		}

		/// <summary>
		/// Gets file system entry type - file, directory, symbolic link, whether it exists and is accessible.
		/// Returns NotFound (0) if does not exist. Returns AccessDenied (&lt; 0) if exists but this process cannot access it and get attributes.
		/// Calls API <msdn>GetFileAttributes</msdn>.
		/// The same as <see cref="ExistsAs"/> but provides more complete result. In most cases you can use ExistsAs, it's simpler.
		/// </summary>
		/// <param name="path">Full path. Supports @"\.." etc. If rawPath is false (default), supports environment variables and non-pefixed extended-lenght path.</param>
		/// <param name="rawPath">Pass path to the API as it is, without any normalizing and validating.</param>
		/// <exception cref="ArgumentException">Not full path. Not thrown if rawPath is true.</exception>
		/// <remarks>
		/// Supports <see cref="Native.GetError"/>. If you need exception when fails to get attributes, instead use <see cref="GetAttributes"/>; it returns false if the file/directory does not exist, and throws exception if fails to get attributes; directories have Directory attribute, symbolic links and mounted folders have ReparsePoint attribute.
		/// </remarks>
		public static ItIs2 ExistsAs2(string path, bool rawPath = false)
		{
			var flags = GAFlags.DoNotThrow; if(rawPath) flags |= GAFlags.RawPath;
			if(!GetAttributes(path, out var a, flags))
				return (a == (FileAttributes)(-1)) ? ItIs2.AccessDenied : ItIs2.NotFound;
			var R = (0 != (a & FileAttributes.Directory)) ? ItIs2.Directory : ItIs2.File;
			if(0 != (a & FileAttributes.ReparsePoint)) R |= (ItIs2)4;
			return R;
		}

		/// <summary>
		/// Returns true if file or directory exists.
		/// Returns false if does not exist or if fails to get its attributes. Supports <see cref="Native.GetError"/>.
		/// Calls <see cref="ExistsAs"/>, which calls API <msdn>GetFileAttributes</msdn>.
		/// </summary>
		/// <param name="path">Full path. Supports @"\.." etc, environment variables and non-pefixed extended-lenght path.</param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		/// <remarks>
		/// Does not throw exception when the file/directory exists but this process cannot access it. If you need exception, instead use <see cref="GetAttributes"/>; it returns false if the file/directory does not exist, and throws exception if exists but cannot be accessed; directories have Directory attribute.
		/// For symbolic links etc, returns true if the link exists. Does not care whether its target exists.
		/// </remarks>
		public static bool ExistsAsAny(string path)
		{
			return ExistsAs(path) != ItIs.NotFound;
		}

		/// <summary>
		/// Returns true if file exists and is not a directory.
		/// Returns false if does not exist or if fails to get its attributes. Supports <see cref="Native.GetError"/>.
		/// Calls <see cref="ExistsAs"/>, which calls API <msdn>GetFileAttributes</msdn>.
		/// </summary>
		/// <param name="path">Full path. Supports @"\.." etc, environment variables and non-pefixed extended-lenght path.</param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		/// <remarks>
		/// Does not throw exception when the file/directory exists but this process cannot access it. If you need exception, instead use <see cref="GetAttributes"/>; it returns false if the file/directory does not exist, and throws exception if exists but cannot be accessed; directories have Directory attribute.
		/// For symbolic links etc, returns true if the link exists and its target is not a directory. Does not care whether its target exists.
		/// </remarks>
		public static bool ExistsAsFile(string path)
		{
			var R = ExistsAs(path);
			if(R == ItIs.File) return true;
			if(R != ItIs.NotFound) Native.ClearError();
			return false;
		}

		/// <summary>
		/// Returns true if directory (folder or drive) exists.
		/// Returns false if does not exist or if fails to get its attributes. Supports <see cref="Native.GetError"/>.
		/// Calls <see cref="ExistsAs"/>, which calls API <msdn>GetFileAttributes</msdn>.
		/// </summary>
		/// <param name="path">Full path. Supports @"\.." etc, environment variables and non-pefixed extended-lenght path.</param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		/// <remarks>
		/// Does not throw exception when the file/directory exists but this process cannot access it. If you need exception, instead use <see cref="GetAttributes"/>; it returns false if the file/directory does not exist, and throws exception if exists but cannot be accessed; directories have Directory attribute.
		/// For symbolic links etc, returns true if the link exists and its target is a directory. Does not care whether its target exists.
		/// </remarks>
		public static bool ExistsAsDirectory(string path)
		{
			var R = ExistsAs(path);
			if(R == ItIs.Directory) return true;
			if(R != ItIs.NotFound) Native.ClearError();
			return false;
		}

		/// <summary>
		/// Finds file or directory and returns fully-qualified path.
		/// Returns null if cannot be found.
		/// If the path argument is full path, calls <see cref="ExistsAsAny"/> and returns normalized path if exists, null if not.
		/// Else searches in these places:
		///	1. dirs, if used.
		/// 2. <see cref="Folders.ThisApp"/>.
		/// 3. Calls API <msdn>SearchPath</msdn>, which searches in process directory, Windows system directories, current directory, PATH environment variable.
		/// 4. If path ends with ".exe", tries to get path from registry "App Paths" keys.
		/// </summary>
		/// <param name="path">Full or relative path or just filename with extension. Supports network paths too.</param>
		/// <param name="dirs">0 or more directories where to search.</param>
		public static string SearchPath(string path, params string[] dirs)
		{
			if(Empty(path)) return null;

			if(Path_.IsFullPath(path)) {
				path = Path_.LibNormalizeExpandEV(path, Path_.NormalizeFlags.TrimEndSeparator);
				if(ExistsAsAny(path)) return path;
				return null;
			}

			if(dirs != null) {
				foreach(var d in dirs) {
					if(Empty(d)) continue;
					var s = Path_.Combine(d, path);
					if(ExistsAsAny(s)) return s;
				}
			}

			{
				var s = Folders.ThisApp + path;
				if(ExistsAsAny(s)) return s;
			}

			var sb = new StringBuilder(300);
			if(0 != Api.SearchPath(null, path, null, 300, sb, Zero)) return sb.ToString();

			if(path.EndsWith_(".exe", true)) {
				string rk = @"Software\Microsoft\Windows\CurrentVersion\App Paths\" + path;
				if(Registry_.GetString(out path, "", rk) || Registry_.GetString(out path, "", rk, Registry.LocalMachine)) {
					path = Path_.LibNormalizeExpandEV(path.Trim('\"'));
					if(ExistsAsAny(path)) return path;
				}
			}

			return null;
		}

		/// <summary>
		/// flags for <see cref="EnumDirectory"/>.
		/// </summary>
		[Flags]
		public enum EDFlags
		{
			/// <summary>
			/// Enumerate subdirectories too.
			/// </summary>
			AndSubdirectories = 1,
			/// <summary>
			/// Also enumerate symbolic link and mounted folder target directories. Use with AndSubdirectories.
			/// </summary>
			AndSymbolicLinkSubdirectories = 2,
			/// <summary>
			/// Skip files and subdirectories that have Hidden attribute.
			/// </summary>
			SkipHidden = 4,
			/// <summary>
			/// Skip files and subdirectories that have Hidden and System attributes (both).
			/// </summary>
			SkipHiddenSystem = 8,
			/// <summary>
			/// If fails to enumerate a directory because of its security settings, throw exception or call errorHandler.
			/// </summary>
			FailIfAccessDenied = 0x10,
			/// <summary>
			/// Don't call <see cref="Path_.Normalize"/>(directoryPath) and don't throw exception for non-full path.
			/// </summary>
			RawPath = 0x20,
			/// <summary>
			/// Temporarily disable file system redirection in this thread of this 32-bit process running on 64-bit Windows.
			/// Then you can enumerate the 64-bit System32 folder in your 32-bit process.
			/// Uses API <msdn>Wow64DisableWow64FsRedirection</msdn>.
			/// For vice versa (in 64-bit process enumerate the 32-bit System folder), instead use path Folders.SystemX86.
			/// </summary>
			DisableRedirection = 0x40,
			/// <summary>
			/// Let <see cref="EDFile.Name"/> be path relative to the specified directory path. Like @"\name.txt", @"\subdirectory\name.txt".
			/// </summary>
			NeedRelativePaths = 0x80,
		}

		/// <summary>
		/// Contains name and other main properties of a file or subdirectory retrieved by <see cref="EnumDirectory"/>.
		/// The values are not changed after creating the variable.
		/// </summary>
		public class EDFile
		{
			///
			internal EDFile(string name, string fullPath, ref _Api.WIN32_FIND_DATA d, int level)
			{
				Name = name; FullPath = fullPath;
				Attributes = (FileAttributes)d.dwFileAttributes;
				Size = (long)d.nFileSizeHigh << 32 | d.nFileSizeLow;
				LastWriteTimeUtc = _DateTimeFromFILETIME(d.ftLastWriteTime); //fast, sizeof 8
				CreationTimeUtc = _DateTimeFromFILETIME(d.ftCreationTime);
				_level = (short)level;
			}

			///
			public string Name { get; }

			///
			public string FullPath { get; }

			/// <summary>
			/// Returns FullPath.
			/// </summary>
			public static implicit operator string(EDFile f) { return f?.FullPath; }

			/// <summary>
			/// Returns file size. For directories it is usually 0.
			/// </summary>
			public long Size { get; }

			///
			public DateTime LastWriteTimeUtc { get; }

			///
			public DateTime CreationTimeUtc { get; }

			///
			public FileAttributes Attributes { get; }

			/// <summary>
			/// Returns true if is directory or symbolic link to a directory or mounted folder.
			/// </summary>
			public bool IsDirectory { get { return (Attributes & FileAttributes.Directory) != 0; } }

			/// <summary>
			/// Descendant level.
			/// 0 if direct child of directoryPath, 1 if child of child, an so on.
			/// </summary>
			public int Level { get { return _level; } }
			short _level;

			/// <summary>
			/// Call this function if don't want to enumerate children of this subdirectory.
			/// </summary>
			public void SkipThisDirectory() { _skip = true; }
			internal bool _skip;

			DateTime _DateTimeFromFILETIME(Api.FILETIME ft) { return DateTime.FromFileTimeUtc((long)ft.dwHighDateTime << 32 | ft.dwLowDateTime); }

			public override string ToString()
			{
				return FullPath;
			}
		}

		/// <summary>
		/// Gets names and other info of files and subdirectories in the specified directory.
		/// Returns an enumerable collection of <see cref="EDFile"/> objects containing the info.
		/// By default gets only direct children. Use flag <see cref="EDFlags.AndSubdirectories"/> to get all descendants.
		/// </summary>
		/// <param name="directoryPath">Full path of the directory.</param>
		/// <param name="flags"></param>
		/// <param name="errorHandler">
		/// A callback function to call when fails to get children of a subdirectory, when using flag EDFlags.AndSubdirectories.
		/// It receives the subdirectory path. It can call <see cref="Native.GetError"/> and throw an exception.
		/// If it does not throw an exception, the enumeration continues as if the directory is empty.
		/// If errorHandler not used, then throws exception.
		/// Read more in Remarks.
		/// </param>
		/// <exception cref="ArgumentException">directoryPath is invalid path or not full path.</exception>
		/// <exception cref="DirectoryNotFoundException">directoryPath directory does not exist.</exception>
		/// <exception cref="CatException">Failed to get children of directoryPath or of a subdirectory. Read more in Remarks.</exception>
		/// <remarks>
		/// Uses API <msdn>FindFirstFile</msdn>.
		/// 
		/// The paths that this function gets are normalized, ie may not start with directoryPath string. Expanded environment variables, "..", DOS path etc.
		/// Supports paths longer than 259 characters length (max path length). The retrieved paths in this case have @"\\?\" prefix (see <see cref="Path_.PrefixLongPath(string)"/>).
		/// For symbolic links and mounted folders, gets info of the link/folder and not of its target.
		/// 
		/// These errors are ignored:
		/// 1. Access denied (usually because of security permissions), unless used flag FailIfAccessDenied.
		/// 2. Missing target directory of a symbolic link or mounted folder.
		/// When an error is ignored, the function works as if the directory is empty; does not throw exception and does not call errorHandler.
		/// 
		/// Enumeration of a subdirectory starts immediately after the subdirectory itself is retrieved.
		/// </remarks>
		public static IEnumerable<EDFile> EnumDirectory(string directoryPath, EDFlags flags = 0, Action<string> errorHandler = null)
		{
			bool rawPath = 0 != (flags & EDFlags.RawPath);
			string path = directoryPath;
			if(!rawPath) path = Path_.Normalize(path, Path_.NormalizeFlags.TrimEndSeparator);
			else if(path.EndsWith_('\\')) path = path.Remove(path.Length - 1);

			_DisableDeviceNotReadyMessageBox();

			var d = new _Api.WIN32_FIND_DATA();
			IntPtr hfind = Zero;
			var stack = new Stack<_EDStackEntry>();
			bool isFirst = true;
			uint attr = 0;
			int basePathLength = path.Length;
			bool redirected = false; IntPtr redirValue = Zero;

			try {
				if((flags & EDFlags.DisableRedirection) != 0 && !Environment.Is64BitProcess && Environment.Is64BitOperatingSystem)
					redirected = _Api.Wow64DisableWow64FsRedirection(out redirValue);

				for(;;) {
					if(isFirst) {
						isFirst = false;
						var path2 = ((path.Length <= 257) ? path : Path_.PrefixLongPath(path)) + @"\*";
#if TEST_FINDFIRSTFILEEX
						hfind = _Api.FindFirstFileEx(path2, _Api.FINDEX_INFO_LEVELS.FindExInfoBasic, out d, 0, Zero, 0);
						//speed: FindFirstFileEx 0-2 % slower. FindExInfoBasic makes 0-2% faster. FIND_FIRST_EX_LARGE_FETCH makes 1-50% slower.
#else
						hfind = _Api.FindFirstFile(path2, out d);
#endif
						if(hfind == (IntPtr)(-1)) {
							hfind = Zero;
							var ec = Native.GetError();
							//PrintList(ec, Native.GetErrorMessage(ec), path);
							bool itsOK = false;
							switch(ec) {
							case Api.ERROR_FILE_NOT_FOUND:
							case Api.ERROR_NO_MORE_FILES:
							case Api.ERROR_NOT_READY:
								//rare, because most directories have "." and "..".
								//FindFirstFileEx sets ERROR_NO_MORE_FILES for some.
								//FindFirstFile could set ERROR_FILE_NOT_FOUND (documented), but was never in my tests.
								//ERROR_NOT_READY when trying to access an ejected CD/DVD drive etc.
								itsOK = true;
								break;
							case Api.ERROR_ACCESS_DENIED:
								itsOK = (flags & EDFlags.FailIfAccessDenied) == 0;
								break;
							case Api.ERROR_PATH_NOT_FOUND: //the directory not found, or symlink target directory is missing
							case Api.ERROR_DIRECTORY: //it is file, not directory. Also noticed for some entries in Recycle Bin (we skip it now). Error text is "The directory name is invalid".
							case Api.ERROR_BAD_NETPATH: //eg \\COMPUTER\MissingFolder
								if(stack.Count == 0 && !ExistsAsDirectory(path))
									throw new DirectoryNotFoundException($"Directory not found: '{path}'. {Native.GetErrorMessage(ec)}");
								//itsOK = (attr & Api.FILE_ATTRIBUTE_REPARSE_POINT) != 0;
								itsOK = true; //or maybe the subdirectory was deleted after we retrieved it
								break;
							case Api.ERROR_INVALID_NAME: //eg contains invalid characters
							case Api.ERROR_BAD_NET_NAME: //eg \\COMPUTER
								if(stack.Count == 0)
									throw new ArgumentException(Native.GetErrorMessage(ec));
								itsOK = true;
								break;
							}
							if(!itsOK) {
								if(errorHandler == null || stack.Count == 0) throw new CatException(ec, $"*enumerate directory '{path}'");
								Api.SetLastError(ec); //the above code possibly changed it, although currently it doesn't
								errorHandler(path);
							}
						}
					} else {
						if(!_Api.FindNextFile(hfind, out d)) {
							Debug.Assert(Native.GetError() == Api.ERROR_NO_MORE_FILES);
							_Api.FindClose(hfind);
							hfind = Zero;
						}
					}

					if(hfind == Zero) {
						if(stack.Count == 0) break;
						var t = stack.Pop();
						hfind = t.hfind; path = t.path;
						continue;
					}

					var name = d.Name;
					if(name == null) continue; //".", ".."
					attr = d.dwFileAttributes;
					bool isDir = (attr & Api.FILE_ATTRIBUTE_DIRECTORY) != 0;

					if((flags & EDFlags.SkipHidden) != 0 && (attr & Api.FILE_ATTRIBUTE_HIDDEN) != 0) continue;
					const uint hidSys = Api.FILE_ATTRIBUTE_HIDDEN | Api.FILE_ATTRIBUTE_SYSTEM;
					if((attr & hidSys) == hidSys) {
						if((flags & EDFlags.SkipHiddenSystem) != 0) continue;
						//skip Recycle Bin. It is useless, also may fail to enum some subdirs.
						if(isDir && name[0] == '$' && name.Equals_("$Recycle.Bin", true) && path.EndsWith_(':')) continue;
					}

					var fullPath = path + @"\" + name;
					if(0 != (flags & EDFlags.NeedRelativePaths)) name = fullPath.Substring(basePathLength);

					//prepend @"\\?\" etc if need. Don't change fullPath length, because then would be difficult to get relative path.
					var fp2 = Path_.PrefixLongPathIfNeed(fullPath);

					var r = new EDFile(name, fp2, ref d, stack.Count);
					yield return r;

					if(!isDir || (flags & EDFlags.AndSubdirectories) == 0 || r._skip) continue;
					if((attr & Api.FILE_ATTRIBUTE_REPARSE_POINT) != 0 && (flags & EDFlags.AndSymbolicLinkSubdirectories) == 0) continue;
					stack.Push(new _EDStackEntry() { hfind = hfind, path = path });
					hfind = Zero; path = fullPath;
					isFirst = true;
				}
			}
			finally {
				if(hfind != Zero) _Api.FindClose(hfind);
				while(stack.Count > 0) _Api.FindClose(stack.Pop().hfind);

				if(redirected) _Api.Wow64RevertWow64FsRedirection(redirValue);
			}
		}

		struct _EDStackEntry { internal IntPtr hfind; internal string path; }

		internal static unsafe partial class _Api
		{
			internal struct WIN32_FIND_DATA
			{
				public uint dwFileAttributes;
				public Api.FILETIME ftCreationTime;
				public Api.FILETIME ftLastAccessTime;
				public Api.FILETIME ftLastWriteTime;
				public uint nFileSizeHigh;
				public uint nFileSizeLow;
				public uint dwReserved0;
				public uint dwReserved1;
				public fixed char cFileName[260];
				public fixed char cAlternateFileName[14];

				internal unsafe string Name
				{
					get
					{
						fixed (char* p = cFileName) {
							if(p[0] == '.') {
								if(p[1] == '\0') return null;
								if(p[1] == '.' && p[2] == '\0') return null;
							}
							return new String(p);
						}
					}
				}
			}

			[DllImport("kernel32.dll", EntryPoint = "FindFirstFileW", SetLastError = true)]
			internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

			[DllImport("kernel32.dll", EntryPoint = "FindNextFileW", SetLastError = true)]
			internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

			[DllImport("kernel32.dll")]
			internal static extern bool FindClose(IntPtr hFindFile);

#if TEST_FINDFIRSTFILEEX
			internal enum FINDEX_INFO_LEVELS
			{
				FindExInfoStandard,
				FindExInfoBasic,
				FindExInfoMaxInfoLevel
			}

			internal const uint FIND_FIRST_EX_LARGE_FETCH = 0x2;

			[DllImport("kernel32.dll", EntryPoint = "FindFirstFileExW")]
			internal static extern IntPtr FindFirstFileEx(string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId, out WIN32_FIND_DATA lpFindFileData, int fSearchOp, IntPtr lpSearchFilter, uint dwAdditionalFlags);
#endif

			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern bool Wow64DisableWow64FsRedirection(out IntPtr OldValue);

			[DllImport("kernel32.dll")]
			internal static extern bool Wow64RevertWow64FsRedirection(IntPtr OlValue);
		}

		#endregion

		#region move, copy, rename, delete

		enum _FileOpType { Rename, Move, Copy, }

		/// <summary>
		/// What to do if the destibnation directory contains a file or directory with the same name as the source file or directory when copying, moving or renaming.
		/// Used with <see cref="Copy"/>, <see cref="Move"/> and similar functions.
		/// When renaming or moving, if the destination is the same as the source, these options are ignored and the destination is simply renamed. For example when renaming "file.txt" to "FILE.TXT".
		/// </summary>
		public enum IfExists
		{
			/// <summary>Throw exception. Default.</summary>
			Fail,
			/// <summary>Delete destination.</summary>
			Delete,
			/// <summary>Rename (backup) destination.</summary>
			RenameExisting,
			/// <summary>
			/// If destination directory exists, merge the source directory into it, replacing existing files.
			/// If destination file exists, deletes it.
			/// If destination directory exists and source is file, fails.
			/// </summary>
			MergeDirectory,
#if not_implemented
			/// <summary>Copy/move with a different name.</summary>
			RenameNew,
			/// <summary>Display a dialog asking the user what to do.</summary>
			AskUser,
#endif
		}

		static unsafe void _FileOp(_FileOpType opType, bool into, string path1, string path2, IfExists ifExists)
		{
			string opName = (opType == _FileOpType.Rename) ? "rename" : ((opType == _FileOpType.Move) ? "move" : "copy");
			path1 = _PreparePath(path1);
			var type1 = ExistsAs2(path1, true);
			if(type1 <= 0) throw new FileNotFoundException($"Failed to {opName}. File not found: '{path1}'");

			if(opType == _FileOpType.Rename) {
				opType = _FileOpType.Move;
				if(Path_.IsInvalidFileName(path2)) throw new ArgumentException("Invalid filename");
				path2 = _RemoveFilename(path1) + "\\" + path2;
			} else {
				string path2Parent;
				if(into) {
					path2Parent = _PreparePath(path2);
					path2 = path2Parent + "\\" + _GetFilename(path1);
				} else {
					path2 = _PreparePath(path2);
					path2Parent = _RemoveFilename(path2, true);
				}
				if(path2Parent != null && !ExistsAsDirectory(path2Parent)) {
					try { Directory.CreateDirectory(path2Parent); }
					catch(Exception ex) { throw new CatException("*create directory", ex); }
				}
			}

			bool ok = false, copy = opType == _FileOpType.Copy, deleteSource = false, mergeDirectory = false;
			var del = new _SafeDeleteExistingDirectory();
			try {
				if(ifExists == IfExists.MergeDirectory && type1 != ItIs2.Directory) ifExists = IfExists.Fail;

				if(ifExists == IfExists.Fail) {
					//API will fail if exists. We don't use use API flags 'replace existing'.
				} else {
					//Delete, RenameExisting, IfExists
					//bool deleted = false;
					var existsAs = ExistsAs2(path2, true);
					switch(existsAs) {
					case ItIs2.NotFound:
						//deleted = true;
						break;
					case ItIs2.AccessDenied:
						break;
					default:
						if(Misc.LibIsSameFile(path1, path2)) {
							//eg renaming "file.txt" to "FILE.txt"
							DebugPrint("same file");
							//deleted = true;
							//copy will fail, move will succeed
						} else if(ifExists == IfExists.MergeDirectory && (existsAs == ItIs2.Directory || existsAs == ItIs2.SymLinkDirectory)) {
							if(type1 == ItIs2.Directory || type1 == ItIs2.SymLinkDirectory) {
								//deleted = true;
								mergeDirectory = true;
								if(!copy) { copy = true; deleteSource = true; }
							} // else API will fail. We refuse to replace a directory with a file.
						} else if(ifExists == IfExists.RenameExisting || existsAs == ItIs2.Directory) {
							//deleted = 
							del.Rename(path2, ifExists == IfExists.RenameExisting);
							//Rename to a temp name. Finally delete if ok (if !RenameExisting), undo if failed.
							//It also solves this problem: if we delete the directory now, need to ensure that it does not delete the source directory, which is quite difficult.
						} else {
							//deleted = 0 ==
							_DeleteLL(path2, existsAs == ItIs2.SymLinkDirectory);
						}
						break;
					}
					//if(!deleted) throw new CatException(Api.ERROR_FILE_EXISTS, $"*{opName}"); //don't need, later API will fail
				}

				if(!copy) {
					//note: don't use MOVEFILE_COPY_ALLOWED, because then moving directory to another drive fails with ERROR_ACCESS_DENIED and we don't know that the reason is different drive
					if(ok = _Api.MoveFileEx(path1, path2, 0)) return;
					if(Native.GetError() == Api.ERROR_NOT_SAME_DEVICE) {
						copy = true;
						deleteSource = true;
					}
				}

				if(copy) {
					if(type1 == ItIs2.Directory) {
						try {
							_CopyDirectory(path1, path2, mergeDirectory);
							ok = true;
						}
						catch(Exception ex) when(opType != _FileOpType.Copy) {
							throw new CatException($"*{opName}", ex);
							//FUTURE: test when it is ThreadAbortException
						}
					} else {
						if(type1 == ItIs2.SymLinkDirectory)
							ok = _Api.CreateDirectoryEx(path1, path2, Zero);
						else
							ok = _Api.CopyFileEx(path1, path2, null, Zero, null, _Api.COPY_FILE_FAIL_IF_EXISTS | _Api.COPY_FILE_COPY_SYMLINK);
					}
				}

				if(!ok) throw new CatException(0, $"*{opName}");

				if(deleteSource) {
					try {
						_Delete(path1, false);
					}
					catch(Exception ex) {
						if(!path1.EndsWith_(':')) //moving drive contents. Deleted contents but cannot delete drive.
							Output.Warning($"Failed to delete '{path1}' after copying it to another drive. {ex.Message}");
						//throw new CatException("*move", ex); //don't. MoveFileEx also succeeds even if fails to delete source.
					}
				}
			}
			finally {
				//TaskDialog.Show();
				del.Finally(ok);
			}
		}

		//note: if merge, the destination directory must exist
		static unsafe void _CopyDirectory(string path1, string path2, bool merge)
		{
			string s1 = null, s2 = null;
			if(!merge && !_Api.CreateDirectoryEx(path1, path2, Zero)) goto ge;
			var edFlags = EDFlags.AndSubdirectories | EDFlags.NeedRelativePaths | EDFlags.FailIfAccessDenied | EDFlags.RawPath;
			var a = EnumDirectory(path1, edFlags).ToArray();
			foreach(var f in a) {
				bool ok;
				s1 = f.FullPath; s2 = path2 + f.Name;
				//PrintList(s1, s2);
				//continue;
				if(f.IsDirectory) {
					if(merge) switch(ExistsAs(s2)) {
						case ItIs.Directory: continue; //never mind: check symbolic link mismatch
						case ItIs.File: _DeleteLL(s2, false); break;
						}

					ok = _Api.CreateDirectoryEx(s1, s2, Zero);
				} else {
					if(merge && GetAttributes(s2, out var attr, GAFlags.DoNotThrow | GAFlags.RawPath)) {
						var badAttr = FileAttributes.ReadOnly | FileAttributes.Hidden;
						if(0 != (attr & FileAttributes.Directory)) _Delete(s2, false);
						else if(0 != (attr & badAttr)) Api.SetFileAttributes(s2, (uint)(attr & ~badAttr));
					}

					uint fl = _Api.COPY_FILE_COPY_SYMLINK; if(!merge) fl |= _Api.COPY_FILE_FAIL_IF_EXISTS;
					ok = _Api.CopyFileEx(s1, s2, null, Zero, null, fl);
				}
				if(!ok) goto ge;
			}
			return;
			ge:
			throw new CatException(0, $"*copy directory '{path1}'. Current item: '{(s1 != null ? s1 : path1)}'");
			//never mind: wrong API error code if path1 and path2 is the same directory.
		}

		struct _SafeDeleteExistingDirectory
		{
			string _oldPath, _tempPath;
			bool _doNotDelete;

			internal bool Rename(string path, bool doNotDelete = false)
			{
				if(path.Length >= 250) path = Path_.PrefixLongPath(path);
				string tempPath = null;
				int iFN = _FindFilename(path);
				string s1 = path.Remove(iFN) + "old", s2 = " " + path.Substring(iFN);
				for(int i = 1; ; i++) {
					tempPath = s1 + i + s2;
					if(ExistsAs2(tempPath, true) == ItIs2.NotFound) break;
				}
				if(!_Api.MoveFileEx(path, tempPath, 0)) return false;
				_oldPath = path; _tempPath = tempPath; _doNotDelete = doNotDelete;
				return true;
			}

			internal bool Finally(bool succeeded)
			{
				if(_tempPath == null) return true;
				if(!succeeded) {
					if(!_Api.MoveFileEx(_tempPath, _oldPath, 0)) return false;
				} else if(!_doNotDelete) {
					try { _Delete(_tempPath, false); } catch { return false; }
				}
				_oldPath = _tempPath = null;
				return true;
			}
		}

		/// <summary>
		/// Renames file or directory.
		/// </summary>
		/// <param name="path">Full path.</param>
		/// <param name="newName">New name without path. Example: "name.txt".</param>
		/// <param name="ifExists"></param>
		/// <exception cref="ArgumentException">
		/// path is not full path (see <see cref="Path_.IsFullPath"/>).
		/// newName is invalid filename.
		/// </exception>
		/// <exception cref="FileNotFoundException">The file (path) does not exist or cannot be found.</exception>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// Uses API <msdn>MoveFileEx</msdn>.
		/// </remarks>
		public static void Rename(string path, string newName, IfExists ifExists = IfExists.Delete)
		{
			_FileOp(_FileOpType.Rename, false, path, newName, ifExists);
		}

		/// <summary>
		/// Moves (changes path of) file or directory.
		/// </summary>
		/// <param name="path">Full path.</param>
		/// <param name="newPath">
		/// New full path.
		/// <note type="note">It is not the new parent directory. Use <see cref="MoveTo"/> for it.</note>
		/// </param>
		/// <param name="ifExists"></param>
		/// <exception cref="ArgumentException">path or newPath is not full path (see <see cref="Path_.IsFullPath"/>).</exception>
		/// <exception cref="FileNotFoundException">The source file (path) does not exist or cannot be found.</exception>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// In most cases uses API <msdn>MoveFileEx</msdn>. It's fast, because don't need to copy files.
		/// In these cases copies/deletes: destination is on another drive; need to merge directories.
		/// When need to copy, does not copy the security properties; sets default security properties.
		/// 
		/// Creates the destination directory if does not exist (see <see cref="Directory.CreateDirectory(String)"/>).
		/// If path and newPath share the same parent directory, just renames the file.
		/// </remarks>
		public static void Move(string path, string newPath, IfExists ifExists = IfExists.Delete)
		{
			_FileOp(_FileOpType.Move, false, path, newPath, ifExists);
		}

		/// <summary>
		/// Moves file or directory into another directory.
		/// </summary>
		/// <param name="path">Full path.</param>
		/// <param name="newDirectory">New parent directory.</param>
		/// <param name="ifExists"></param>
		/// <exception cref="ArgumentException">
		/// path or newDirectory is not full path (see <see cref="Path_.IsFullPath"/>).
		/// path is drive. To move drive content, use <see cref="Move"/>.
		/// </exception>
		/// <exception cref="FileNotFoundException">The source file (path) does not exist or cannot be found.</exception>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// In most cases uses API <msdn>MoveFileEx</msdn>. It's fast, because don't need to copy files.
		/// In these cases copies/deletes: destination is on another drive; need to merge directories.
		/// When need to copy, does not copy the security properties; sets default security properties.
		/// 
		/// Creates the destination directory if does not exist (see <see cref="Directory.CreateDirectory(String)"/>).
		/// </remarks>
		public static void MoveTo(string path, string newDirectory, IfExists ifExists = IfExists.Delete)
		{
			_FileOp(_FileOpType.Move, true, path, newDirectory, ifExists);
		}

		/// <summary>
		/// Copies file or directory.
		/// </summary>
		/// <param name="path">Full path.</param>
		/// <param name="newPath">
		/// New full path.
		/// <note type="note">It is not the new parent directory. Use <see cref="CopyTo"/> for it.</note>
		/// </param>
		/// <param name="ifExists"></param>
		/// <exception cref="ArgumentException">path or newPath is not full path (see <see cref="Path_.IsFullPath"/>).</exception>
		/// <exception cref="FileNotFoundException">The source file (path) does not exist or cannot be found.</exception>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// Uses API <msdn>CopyFileEx</msdn>.
		/// On Windows 7 does not copy the security properties; sets default security properties.
		/// Creates the destination directory if does not exist (see <see cref="Directory.CreateDirectory(String)"/>).
		/// </remarks>
		public static void Copy(string path, string newPath, IfExists ifExists = IfExists.Delete)
		{
			_FileOp(_FileOpType.Copy, false, path, newPath, ifExists);
		}

		/// <summary>
		/// Copies file or directory into another directory.
		/// </summary>
		/// <param name="path">Full path.</param>
		/// <param name="newDirectory">New parent directory.</param>
		/// <param name="ifExists"></param>
		/// <exception cref="ArgumentException">
		/// path or newDirectory is not full path (see <see cref="Path_.IsFullPath"/>).
		/// path is drive. To copy drive content, use <see cref="Copy"/>.
		/// </exception>
		/// <exception cref="FileNotFoundException">The source file (path) does not exist or cannot be found.</exception>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// Uses API <msdn>CopyFileEx</msdn>.
		/// On Windows 7 does not copy the security properties; sets default security properties.
		/// Creates the destination directory if does not exist (see <see cref="Directory.CreateDirectory(String)"/>).
		/// </remarks>
		public static void CopyTo(string path, string newDirectory, IfExists ifExists = IfExists.Delete)
		{
			_FileOp(_FileOpType.Copy, true, path, newDirectory, ifExists);
		}

		/// <summary>
		/// Deletes file or directory.
		/// Does nothing (no exception) if it does not exist or cannot be found.
		/// </summary>
		/// <param name="path">Full path.</param>
		/// <param name="useRecycleBin">Send to Recycle Bin. Note: it is much slower.</param>
		/// <exception cref="ArgumentException">path is not full path (see <see cref="Path_.IsFullPath"/>).</exception>
		/// <exception cref="CatException">Failed.</exception>
		/// <remarks>
		/// If directory, also deletes all its files and subdirectories. If fails to delete some, tries to delete as many as possible.
		/// Does not fail if is read-only or contains read-only files.
		/// 
		/// Some reasons why this function can fail:
		/// 1. The file is open (in any process). Or a file in the directory is open.
		/// 2. This process does not have security permissions to access or delete the file or directory or some of its descendants.
		/// 3. The directory is (or contains) the "current directory" (in any process).
		/// </remarks>
		public static void Delete(string path, bool useRecycleBin)
		{
			path = _PreparePath(path);
			_Delete(path, useRecycleBin);
		}

		static ItIs2 _Delete(string path, bool useRecycleBin)
		{
			var type = ExistsAs2(path, true);
			if(type == ItIs2.NotFound) return type;
			if(type == ItIs2.AccessDenied) throw new CatException(0, "*delete");

			if(useRecycleBin) {
				if(!_DeleteShell(path, true)) throw new CatException("*delete");
			} else {
				int ec = 0;
				if(type == ItIs2.Directory) {
					var dirs = new List<string>();
					foreach(var f in EnumDirectory(path, EDFlags.AndSubdirectories | EDFlags.RawPath)) {
						if(f.IsDirectory) dirs.Add(f.FullPath);
						else _DeleteLL(f.FullPath, false); //delete as many as possible
					}
					for(int i = dirs.Count - 1; i >= 0; i--) {
						_DeleteLL(dirs[i], true); //delete as many as possible
					}
					ec = _DeleteLL(path, true);
					if(ec == 0) {
						//notify shell. Else, if it was open in Explorer, it shows an error message box.
						//Info: .NET does not notify; SHFileOperation does.
						_ShellNotify(Api.SHCNE_RMDIR, path);
						return type;
					}
					DebugPrint("Using _DeleteShell.");
					if(_DeleteShell(path, false)) return type;
				} else {
					ec = _DeleteLL(path, type == ItIs2.SymLinkDirectory);
					if(ec == 0) return type;
				}
				if(ExistsAs2(path, true) != ItIs2.NotFound) throw new CatException(ec, "*delete");

				//info:
				//RemoveDirectory fails if not empty.
				//Directory.Delete fails if a descendant is read-only, etc.
				//Also both fail if a [now deleted] subfolder containing files was open in Explorer. Workaround: sleep(10) and retry.
				//_DeleteShell does not have these problems. But it is very slow.
				//But all fail if it is current directory in any process. If in current process, _DeleteShell succeeds; it makes current directory = its parent.
			}
			return type;
		}

		static int _DeleteLL(string path, bool dir)
		{
			//PrintList(dir, path);
			if(dir ? _Api.RemoveDirectory(path) : _Api.DeleteFile(path)) return 0;
			var ec = Native.GetError();
			if(ec == Api.ERROR_ACCESS_DENIED) {
				var a = Api.GetFileAttributes(path);
				if(a != Api.INVALID_FILE_ATTRIBUTES && 0 != (a & Api.FILE_ATTRIBUTE_READONLY)) {
					Api.SetFileAttributes(path, a & ~Api.FILE_ATTRIBUTE_READONLY);
					if(dir ? _Api.RemoveDirectory(path) : _Api.DeleteFile(path)) return 0;
					ec = Native.GetError();
				}
			}
			if(ec == Api.ERROR_DIR_NOT_EMPTY && _Api.PathIsDirectoryEmpty(path)) {
				//see comments above about Explorer
				DebugPrint("ERROR_DIR_NOT_EMPTY when empty");
				for(int i = 0; i < 5; i++) {
					Time.Sleep(10);
					if(_Api.RemoveDirectory(path)) return 0;
				}
				ec = Native.GetError();
			}
			if(ec == Api.ERROR_FILE_NOT_FOUND || ec == Api.ERROR_PATH_NOT_FOUND) return 0;
			DebugPrint("_DeleteLL failed. " + Native.GetErrorMessage(ec) + "  " + path
				+ "   Children: " + string.Join(" | ", EnumDirectory(path).Select(f => f.Name)));
			return ec;

			//never mind: .NET also calls DeleteVolumeMountPoint if it is a mounted folder.
			//	But I did not find in MSDN doc that need to do it before calling removedirectory. I think OS should unmount automatically.
			//	Tested on Win10, works without unmounting explicitly. Even Explorer updates its current folder without notification.
		}

		static bool _DeleteShell(string path, bool recycle)
		{
			if(path.IndexOfAny_("*?") >= 0) throw new ArgumentException("*? not supported.");
			var x = new _Api.SHFILEOPSTRUCT();
			x.wFunc = _Api.FO_DELETE;
			var f = _Api.FOF_NO_UI | _Api.FOF_NO_CONNECTED_ELEMENTS; if(recycle) f |= _Api.FOF_ALLOWUNDO;
			x.fFlags = (ushort)f;
			x.pFrom = path + "\0";
			x.hwnd = Wnd.Misc.WndRoot;
			var r = _Api.SHFileOperation(ref x);
			return r == 0 && !x.fAnyOperationsAborted;

			//Also tested IFileOperation, but it is even slower. Also it requires STA, which is not a big problem.
		}

		/// <summary>
		/// Calls <see cref="EnumDirectory"/> and returns sum of all file sizes.
		/// If using default flags, it includes sizes of all descendant files (in all subdirectories).
		/// </summary>
		/// <param name="path">Full path.</param>
		/// <param name="flags">EnumDirectory flags.</param>
		/// <exception cref="Exception"><see cref="EnumDirectory"/> exceptions. By default, no exceptions if used full path and the directory exists.</exception>
		/// <remarks>
		/// This function is slow if the directory is large.
		/// Don't use this function for files (throws exception) and drives (instead use <see cref="DriveInfo"/>, it's fast and includes Recycle Bin size).
		/// If this process cannot access some subdirectories (security), their sizes are not included. There is a flag to throw excption instead.
		/// </remarks>
		public static long CalculateDirectorySize(string path, EDFlags flags = EDFlags.AndSubdirectories)
		{
			return EnumDirectory(path, flags).Sum(f => f.Size);
		}

		static void _ShellNotify(uint @event, string path, string path2 = null)
		{
			Task.Run(() => Api.SHChangeNotify(@event, Api.SHCNF_PATH, path, path2));
		}

		/// <summary>
		/// Expands environment variables. Throws ArgumentException if not full path. Normalizes. Trims ending '\\'.
		/// </summary>
		static string _PreparePath(string path)
		{
			path = Path_.ExpandEnvVar(path);
			if(!Path_.IsFullPath(path)) throw new ArgumentException($"Not full path: '{path}'.");
			return Path_.LibNormalizeNoExpandEV(path, Path_.NormalizeFlags.TrimEndSeparator);
		}

		static char[] _sep = new char[] { '\\', '/' };

		/// <summary>
		/// Finds filename, eg @"b.txt" in @"c:\a\b.txt".
		/// </summary>
		/// <exception cref="ArgumentException">'\\' not found. If noException, instead returns -1.</exception>
		static int _FindFilename(string path, bool noException = false)
		{
			int R = path.LastIndexOfAny(_sep);
			if(R < 0) {
				if(noException) return -1;
				throw new ArgumentException("No filename in path.");
			}
			return R + 1;
		}

		/// <summary>
		/// Removes filename, eg @"c:\a\b.txt" -> @"c:\a".
		/// </summary>
		/// <exception cref="ArgumentException">'\\' not found. If noException, instead returns null.</exception>
		static string _RemoveFilename(string path, bool noException = false)
		{
			int i = _FindFilename(path, noException); if(i < 0) return null;
			return path.Remove(i - 1);
		}

		/// <summary>
		/// Gets filename, eg @"c:\a\b.txt" -> @"b.txt".
		/// </summary>
		/// <exception cref="ArgumentException">'\\' not found. If noException, instead returns null.</exception>
		static string _GetFilename(string path, bool noException = false)
		{
			int i = _FindFilename(path, noException); if(i < 0) return null;
			return path.Substring(i);
		}

		static unsafe partial class _Api
		{
			internal const uint FO_MOVE = 0x1;
			internal const uint FO_COPY = 0x2;
			internal const uint FO_DELETE = 0x3;
			internal const uint FO_RENAME = 0x4;

			internal const uint FOF_MULTIDESTFILES = 0x1;
			internal const uint FOF_CONFIRMMOUSE = 0x2;
			internal const uint FOF_SILENT = 0x4;
			internal const uint FOF_RENAMEONCOLLISION = 0x8;
			internal const uint FOF_NOCONFIRMATION = 0x10;
			internal const uint FOF_WANTMAPPINGHANDLE = 0x20;
			internal const uint FOF_ALLOWUNDO = 0x40;
			internal const uint FOF_FILESONLY = 0x80;
			internal const uint FOF_SIMPLEPROGRESS = 0x100;
			internal const uint FOF_NOCONFIRMMKDIR = 0x200;
			internal const uint FOF_NOERRORUI = 0x400;
			internal const uint FOF_NOCOPYSECURITYATTRIBS = 0x800;
			internal const uint FOF_NORECURSION = 0x1000;
			internal const uint FOF_NO_CONNECTED_ELEMENTS = 0x2000;
			internal const uint FOF_WANTNUKEWARNING = 0x4000;
			internal const uint FOF_NORECURSEREPARSE = 0x8000;
			internal const uint FOF_NO_UI = 0x614;

			internal struct SHFILEOPSTRUCT
			{
				public Wnd hwnd;
				public uint wFunc;
				public string pFrom;
				public string pTo;
				public ushort fFlags;
				public bool fAnyOperationsAborted;
				public IntPtr hNameMappings;
				public string lpszProgressTitle;
			}

			[StructLayout(LayoutKind.Sequential, Pack = 1)]
			internal struct SHFILEOPSTRUCT__32
			{
				public Wnd hwnd;
				public uint wFunc;
				public string pFrom;
				public string pTo;
				public ushort fFlags;
				public bool fAnyOperationsAborted;
				public IntPtr hNameMappings;
				public string lpszProgressTitle;
			}

			[DllImport("shell32.dll", EntryPoint = "SHFileOperationW")]
			internal static extern int SHFileOperation(ref SHFILEOPSTRUCT lpFileOp);


			internal const uint MOVEFILE_REPLACE_EXISTING = 0x1;
			internal const uint MOVEFILE_COPY_ALLOWED = 0x2;
			internal const uint MOVEFILE_DELAY_UNTIL_REBOOT = 0x4;
			internal const uint MOVEFILE_WRITE_THROUGH = 0x8;
			internal const uint MOVEFILE_CREATE_HARDLINK = 0x10;
			internal const uint MOVEFILE_FAIL_IF_NOT_TRACKABLE = 0x20;

			[DllImport("kernel32.dll", EntryPoint = "MoveFileExW", SetLastError = true)]
			internal static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, uint dwFlags);

			//[DllImport("kernel32.dll", EntryPoint = "CopyFileW", SetLastError = true)]
			//internal static extern bool CopyFile(string lpExistingFileName, string lpNewFileName, bool bFailIfExists);

			internal const uint COPY_FILE_FAIL_IF_EXISTS = 0x1;
			internal const uint COPY_FILE_RESTARTABLE = 0x2;
			internal const uint COPY_FILE_OPEN_SOURCE_FOR_WRITE = 0x4;
			internal const uint COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 0x8;
			internal const uint COPY_FILE_COPY_SYMLINK = 0x800;
			internal const uint COPY_FILE_NO_BUFFERING = 0x1000;

			[DllImport("kernel32.dll", EntryPoint = "CopyFileExW", SetLastError = true)]
			internal static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, LPPROGRESS_ROUTINE lpProgressRoutine, IntPtr lpData, int* pbCancel, uint dwCopyFlags);

			internal delegate uint LPPROGRESS_ROUTINE(long TotalFileSize, long TotalBytesTransferred, long StreamSize, long StreamBytesTransferred, uint dwStreamNumber, uint dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

			[DllImport("kernel32.dll", EntryPoint = "DeleteFileW", SetLastError = true)]
			internal static extern bool DeleteFile(string lpFileName);

			[DllImport("kernel32.dll", EntryPoint = "RemoveDirectoryW", SetLastError = true)]
			internal static extern bool RemoveDirectory(string lpPathName);

			[DllImport("kernel32.dll", EntryPoint = "CreateDirectoryExW", SetLastError = true)]
			internal static extern bool CreateDirectoryEx(string lpTemplateDirectory, string lpNewDirectory, IntPtr lpSecurityAttributes); //ref SECURITY_ATTRIBUTES

			[DllImport("kernel32.dll", EntryPoint = "CreateFileW", SetLastError = true)]
			internal static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

			internal struct BY_HANDLE_FILE_INFORMATION
			{
				public uint dwFileAttributes;
				public Api.FILETIME ftCreationTime;
				public Api.FILETIME ftLastAccessTime;
				public Api.FILETIME ftLastWriteTime;
				public uint dwVolumeSerialNumber;
				public uint nFileSizeHigh;
				public uint nFileSizeLow;
				public uint nNumberOfLinks;
				public uint nFileIndexHigh;
				public uint nFileIndexLow;
			}

			[DllImport("kernel32.dll", SetLastError = true)]
			internal static extern bool GetFileInformationByHandle(IntPtr hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

			[DllImport("shlwapi.dll", EntryPoint = "PathIsDirectoryEmptyW")]
			internal static extern bool PathIsDirectoryEmpty(string pszPath);
			//speed: slightly faster than with EnumDirectory.
		}
		#endregion
	}
}
