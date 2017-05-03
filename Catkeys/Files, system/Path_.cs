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
//using System.Xml.Linq;
//using System.Xml.XPath;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Extends the .NET class Path.
	/// </summary>
	public static unsafe class Path_
	{
		/// <summary>
		/// If path starts with "%" or "\"%", expands environment variables enclosed in %, else just returns path.
		/// Also supports known folder names, like "%Folders.Documents%". More info in Remarks.
		/// </summary>
		/// <param name="path">Any string. Can be null.</param>
		/// <remarks>
		/// Supports known folder names as you can get with <see cref="Folders"/> class properties.
		/// Example: @"%Folders.Documents%\file.txt".
		/// Example: @"%Folders.Virtual.ControlPanel%" //gets ":: HexEncodedITEMIDLIST".
		/// Usually known folders are used like <c>string path = Folders.Documents + "file.txt"</c>. It's easier and faster. However it cannot be used when you want to store paths in text files, registry, etc. Then this feature is useful.
		/// To get known folder path, this function calls <see cref="Folders.GetFolder"/>.
		///
		/// This function is called by many functions of classes Path_, Files, Shell, Icons, some others, therefore all they support environment variables and known folders in path string.
		/// </remarks>
		public static string ExpandEnvVar(string path)
		{
			var s = path;
			if(s == null || s.Length < 3) return s;
			if(s[0] != '%') {
				if(s[0] == '\"' && s[1] == '%') return "\"" + ExpandEnvVar(s.Substring(1));
				return s;
			}
			int i = s.IndexOf('%', 1); if(i < 2) return s;
			//return Environment.ExpandEnvironmentVariables(s); //5 times slower

			//support known folders, like @"%Folders.Documents%\..."
			if(i >= 12 && s.StartsWith_("%Folders.")) {
				var k = Folders.GetFolder(s.Substring(9, i - 9));
				if(k != null) return k + s.Substring(i + 1);
				return s;
			}

			var b = Util.LibCharBuffer.Common; int na = b.Max(s.Length + 100);
			g1: int nr = _Api.ExpandEnvironmentStrings(s, b.Alloc(na), na);
			if(nr > na) { na = nr; goto g1; }
			if(nr == 0) return s;
			var R = b.ToString(nr - 1);
			if(R == s) return R;
			return ExpandEnvVar(R); //can be %envVar2% in envVar1 value
		}

		/// <summary>
		/// Gets environment variable's value.
		/// Returns "" if variable not found.
		/// Does not support Folders.X.
		/// </summary>
		/// <param name="name">Case-insensitive name.</param>
		/// <remarks>
		/// Environment variable values cannot be "" or null. Setting empty value removes the variable.
		/// </remarks>
		internal static string LibGetEnvVar(string name)
		{
			var b = Util.LibCharBuffer.Common; int na = b.Max(300);
			g1: int nr = _Api.GetEnvironmentVariable(name, b.Alloc(na), na);
			if(nr > na) { na = nr; goto g1; }
			if(nr == 0) return "";
			return b.ToString(nr);
		}

		/// <summary>
		/// Returns true if environment variable exists.
		/// </summary>
		/// <param name="name">Case-insensitive name.</param>
		/// <returns></returns>
		internal static bool LibEnvVarExists(string name)
		{
			return 0 != _Api.GetEnvironmentVariable(name, null, 0);
		}

		static class _Api
		{
			[DllImport("kernel32.dll", EntryPoint = "GetEnvironmentVariableW", SetLastError = true)]
			internal static extern int GetEnvironmentVariable(string lpName, char* lpBuffer, int nSize);

			[DllImport("kernel32.dll", EntryPoint = "ExpandEnvironmentStringsW")]
			internal static extern int ExpandEnvironmentStrings(string lpSrc, char* lpDst, int nSize);
		}

		/// <summary>
		/// Returns true if path matches one of these wildcard patterns:
		///		@"?:\*" - local path, like @"C:\a\b.txt". Here ? is A-Z, a-z.
		///		@"?:" - drive name, like @"C:". Here ? is A-Z, a-z. Note: it is considered full path in this library, but for .NET and Windows API it is not full path.
		///		@"\\*" - network path, like @"\\server\share\...". Or has prefix @"\\?\".
		/// </summary>
		/// <param name="path">Any string. Can be null.</param>
		/// <remarks>
		/// Supports '/' characters too.
		/// Supports only file-system paths. Returns false if path is URL (use <see cref="IsUrl"/> instead) or starts with "::".
		/// If path starts with "%environmentVariable%", returns false, and in DEBUG configuration shows warning. You should at first expand environment variables with <see cref="ExpandEnvVar"/> or instead use <see cref="IsFullPathEEV"/>.
		/// </remarks>
		public static bool IsFullPath(string path)
		{
			var s = path;
			int len = (s == null) ? 0 : s.Length;

			if(len >= 2) {
				if(s[1] == ':' && Calc.IsAlpha(s[0])) {
					return len == 2 || LibIsSepChar(s[2]);
					//info: returns false if eg "c:abc" which means "abc" in current directory of drive "c:"
				}
				switch(s[0]) {
				case '\\':
				case '/':
					return LibIsSepChar(s[1]);
				case '%':
					DebugPrint("IsFullPath: path starts with '%' character. Call ExpandEnvVar at first, or use IsFullPathEEV instead.");
					break;
					//s = ExpandEnvVar(s); //quite fast. 70% slower than just LibEnvVarExists, but reliable.
					//return !s.StartsWith_('%') && IsFullPath(s);
				}
			}

			return false;
		}

		/// <summary>
		/// Calls <see cref="IsFullPath"/> with expanded environment variables.
		/// Returns true if path matches one of these wildcard patterns:
		///		@"?:\*" - local path, like @"C:\a\b.txt". Here ? is A-Z, a-z.
		///		@"?:" - drive name, like @"C:". Here ? is A-Z, a-z. Note: it is considered full path in this library, but for .NET and Windows API it is not full path.
		///		@"\\*" - network path, like @"\\server\share\...". Or has prefix @"\\?\".
		/// </summary>
		/// <param name="path">
		/// Any string. Can be null.
		/// If starts with '%' character, calls <see cref="IsFullPath"/> with expanded environment variables (<see cref="ExpandEnvVar"/>). If it returns true, replaces the passed variable with the expanded path string.
		/// </param>
		/// <remarks>
		/// Supports '/' characters too.
		/// Supports only file-system paths. Returns false if path is URL (use <see cref="IsUrl"/> instead) or starts with "::".
		/// </remarks>
		public static bool IsFullPathEEV(ref string path)
		{
			var s = path;
			if(s == null || s.Length < 2) return false;
			if(s[0] != '%') return IsFullPath(s);
			s = ExpandEnvVar(s);
			if(s[0] == '%') return false;
			if(!IsFullPath(s)) return false;
			path = s;
			return true;
		}

		/// <summary>
		/// Gets the length of the drive or network folder part in path, including its separator if any.
		/// If the string does not start with a drive or network folder path, returns 0 or prefix length (@"\\?\" or @"\\?\UNC\").
		/// </summary>
		/// <param name="path">Full path or any string. Can be null. Should not be "%environmentVariable%\...".</param>
		/// <remarks>
		/// Supports prefixes @"\\?\" and @"\\?\UNC\".
		/// Supports separators '\\' and '/'.
		/// </remarks>
		public static int GetRootLength(string path)
		{
			var s = path;
			int i = 0, len = (s == null) ? 0 : s.Length;
			if(len >= 2) {
				switch(s[1]) {
				case ':':
					if(Calc.IsAlpha(s[i])) {
						int j = i + 2;
						if(len == j) return j;
						if(LibIsSepChar(s[j])) return j + 1;
						//else invalid
					}
					break;
				case '\\':
				case '/':
					if(LibIsSepChar(s[0])) {
						i = _GetPrefixLength(s);
						if(i == 0) i = 2; //no prefix
						else if(i == 4) {
							if(len >= 6 && s[5] == ':') goto case ':'; //like @"\\?\C:\..."
							break; //invalid, no UNC
						} //else like @"\\?\UNC\server\share\..."
						int i0 = i, nSep = 0;
						for(; i < len && nSep < 2; i++) {
							char c = s[i];
							if(LibIsSepChar(c)) nSep++;
							else if(c == ':') return i0;
							else if(c == '0') break;
						}
					}
					break;
				}
			}
			return i;
		}

		/// <summary>
		/// Gets the length of the URL protocol name (also known as URI scheme) in string, including ':'.
		/// If the string does not start with a protocol name, returns 0.
		/// URL examples: "http:" (returns 5), "http://www.x.com" (returns 5), "file:///path" (returns 5), "shell:etc" (returns 6).
		/// The protocol can be unknown, the function just checks string format, which is an ASCII alpha character followed by one or more ASCII alpha-numeric, '.', '-', '+' characters, followed by ':' character.
		/// </summary>
		/// <param name="s">A URL or path or any string. Can be null.</param>
		public static int GetUrlProtocolLength(string s)
		{
			int len = (s == null) ? 0 : s.Length;
			if(len > 2 && Calc.IsAlpha(s[0]) && s[1] != ':') {
				for(int i = 1; i < len; i++) {
					var c = s[i];
					if(c == ':') return i + 1;
					if(!(Calc.IsAlpha(c) || Calc.IsDigit(c) || c == '.' || c == '-' || c == '+')) break;
				}
			}
			return 0;
			//info: API PathIsURL lies, like most shlwapi.dll functions.
		}

		/// <summary>
		/// Returns true if the string starts with a URL protocol name (existing or not) and ':' character.
		/// URL examples: "http:", "http://www.x.com", "file:///path", "shell:etc".
		/// Calls <see cref="GetUrlProtocolLength"/> and returns true if it's not 0.
		/// </summary>
		/// <param name="s">A URL or path or any string. Can be null.</param>
		public static bool IsUrl(string s)
		{
			return 0 != GetUrlProtocolLength(s);
		}

		/// <summary>
		/// flags for <see cref="Combine"/>.
		/// </summary>
		[Flags]
		public enum CombineFlags
		{
			/// <summary>
			/// s2 can be full path. If it is, ignore s1 and return s2 with expanded environment variables.
			/// If this flag not used, simply combines s1 and s2, creating invalid path if s2 is full path.
			/// </summary>
			s2CanBeFullPath = 1,

			/// <summary>
			/// Don't call <see cref="PrefixLongPathIfNeed"/> which may prepend @"\\?\" if the result path is very long.
			/// </summary>
			DoNotPrefixLongPath = 2,
		}

		/// <summary>
		/// Combines two path parts, for example directory path and file name.
		/// Joins them using character '\\' so that there will not be two '\\' or '/'.
		/// </summary>
		/// <param name="s1">First path. Usually a directory.</param>
		/// <param name="s2">Second path. Usually a filename or relative path. Use flag s2CanBeFullPath if it can be full path.</param>
		/// <param name="flags"></param>
		/// <remarks>
		/// If s1 and s2 are null or "", returns "". Else if s1 is null or "", returns s2. Else if s2 is null or "", returns s1.
		/// Similar to Path.Combine (.NET function). Main differences: does not throw exceptions; does not ignore s1 if s2 is a relative path starting with @"\" or "/".
		/// See also <see cref="Normalize"/>.
		/// </remarks>
		public static string Combine(string s1, string s2, CombineFlags flags = 0)
		{
			string r;
			if(Empty(s1)) r = s2 ?? "";
			else if(Empty(s2)) r = s1 ?? "";
			else if(0 != (flags & CombineFlags.s2CanBeFullPath) && IsFullPathEEV(ref s2)) r = s2;
			else {
				int k = 0;
				if(LibIsSepChar(s1[s1.Length - 1])) k |= 1;
				if(LibIsSepChar(s2[0])) k |= 2;
				switch(k) {
				case 0: r = s1 + @"\" + s2; break;
				case 3: r = s1 + s2.Substring(1); break;
				default: r = s1 + s2; break;
				}
			}
			if(0 == (flags & CombineFlags.DoNotPrefixLongPath)) r = PrefixLongPathIfNeed(r);
			return r;
		}

		/// <summary>
		/// Combines two path parts.
		/// Unlike Path_.Combine, fails if some part is empty or @"\" or if s2 is @"\\". Also does not check s2 full path.
		/// If fails, throws exception or returns null (if noException).
		/// </summary>
		internal static string LibCombine(string s1, string s2, bool noException = false)
		{
			if(!Empty(s1) && !Empty(s2)) {
				int k = 0;
				if(LibIsSepChar(s1[s1.Length - 1])) {
					if(s1.Length == 1) goto ge;
					k |= 1;
				}
				if(LibIsSepChar(s2[0])) {
					if(s2.Length == 1 || LibIsSepChar(s2[1])) goto ge;
					k |= 2;
				}
				string r;
				switch(k) {
				case 0: r = s1 + @"\" + s2; break;
				case 3: r = s1 + s2.Substring(1); break;
				default: r = s1 + s2; break;
				}
				return PrefixLongPathIfNeed(r);
			}
			ge:
			if(noException) return null;
			throw new ArgumentException("Empty filename or path.");
		}

		static char[] _sep = new char[] { '\\', '/' };

		/// <summary>
		/// Returns true if character c == '\\' || c == '/'.
		/// </summary>
		internal static bool LibIsSepChar(char c) { return c == '\\' || c == '/'; }

		/// <summary>
		/// Returns true if ends with ':' preceded by a drive letter, like "C:" or "more\C:", but not like "moreC:".
		/// </summary>
		/// <param name="s">Can be null.</param>
		/// <param name="length">Use when want to check drive at a middle, not at the end. Eg returns true if s is @"C:\more" and length is 2.</param>
		static bool _EndsWithDriveWithoutSep(string s, int length = -1)
		{
			if(s == null) return false;
			int i = ((length < 0) ? s.Length : length) - 1;
			if(i < 1 || s[i] != ':') return false;
			if(!Calc.IsAlpha(s[--i])) return false;
			if(i > 0 && !LibIsSepChar(s[i - 1])) return false;
			return true;
		}

		/// <summary>
		/// Ensures that s either ends with a valid drive path (eg @"C:\" but not "C:") or does not end with '\\' or '/' (unless would become empty if removed).
		/// </summary>
		/// <param name="s">Can be null.</param>
		static string _AddRemoveSep(string s)
		{
			if(s != null) {
				int i = s.Length - 1;
				if(i > 0) {
					if(LibIsSepChar(s[i]) && s[i - 1] != ':') {
						var s2 = s.TrimEnd(_sep);
						if(s2.Length != 0) s = s2;
					}
					if(_EndsWithDriveWithoutSep(s)) s = s + "\\";
				}
			}
			return s;
		}

		/// <summary>
		/// flags for <see cref="Normalize"/>.
		/// </summary>
		[Flags]
		public enum NormalizeFlags
		{
			/// <summary>Don't call API <msdn>GetLongPathName</msdn>.</summary>
			DoNotExpandDosPath = 1,
			/// <summary>Don't call <see cref="PrefixLongPathIfNeed"/>.</summary>
			DoNotPrefixLongPath = 2,
			/// <summary>Don't remove '\\' character at the end.</summary>
			DoNotRemoveEndSeparator = 4,
			/// <summary>If path is not a file-system path but looks like URL (eg "http:..." or "file:...") or starts with "::", don't throw exception and don't process more (only expand environment variables).</summary>
			CanBeUrlOrShell = 8,
		}

		/// <summary>
		/// Makes normal full path from path that can contain special substrings etc.
		/// The sequence of actions:
		/// 1. If path starts with '%' character, expands environment variables (see <see cref="Path_.ExpandEnvVar"/>).
		/// 2. If path is not full path but looks like URL, and used flag CanBeUrl, returns path.
		/// 3. If path is not full path, and defaultParentDirectory is not null/"", combines path with defaultParentDirectory.
		/// 4. If path is not full path, throws exception.
		/// 5. Calls API <msdn>GetFullPathName</msdn>. It replaces '/' to '\\', replaces multiple '\\' to single (where need), processes @"\.." etc, trims spaces, etc.
		/// 6. If no flag DoNotExpandDosPath, if contains '~' character, calls API <msdn>GetLongPathName</msdn>. It converts short DOS path to normal path, if possible, for example @"c:\progra~1" to @"c:\program files". It is slow. It converts path only if the file exists.
		/// 7. If no flag DoNotRemoveEndSeparator, removes '\\' character at the end, unless it is a drive path (eg @"C:\").
		/// 8. Appends '\\' character if ends with a drive name (eg "C:" -> @"C:\").
		/// 9. If no flag DoNotPrefixLongPath, calls <see cref="PrefixLongPathIfNeed"/>, which adds @"\\?\" etc prefix if path is very long.
		/// </summary>
		/// <param name="path">Any path.</param>
		/// <param name="defaultParentDirectory">If path is not full path, combine it with defaultParentDirectory to make full path.</param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">path is not full path, and defaultParentDirectory is not used or does not make it full path.</exception>
		/// <remarks>
		/// Similar to <see cref="Path.GetFullPath"/>. Main differences: this function expands environment variables, does not support relative paths, supports @"\\?\very long path", trims '\\' at the end if need, does not throw exceptions when [it thinks that] path is invalid (except when path is not full).
		/// </remarks>
		public static string Normalize(string path, string defaultParentDirectory = null, NormalizeFlags flags = 0)
		{
			path = ExpandEnvVar(path);
			if(!IsFullPath(path)) { //note: not EEV
				if(0 != (flags & NormalizeFlags.CanBeUrlOrShell)) if(LibIsShellPath(path) || IsUrl(path)) return path;
				if(Empty(defaultParentDirectory)) goto ge;
				path = LibCombine(defaultParentDirectory, path);
				if(!IsFullPath(path)) goto ge;
			}

			return LibNormalize(path, flags, true);
			ge:
			throw new ArgumentException($"Not full path: '{path}'.");
		}

		/// <summary>
		/// Same as <see cref="Normalize"/>, but skips full-path checking.
		/// s should be full path. If not full and not null/"", combines with current directory.
		/// </summary>
		internal static string LibNormalize(string s, NormalizeFlags flags = 0, bool noExpandEV = false)
		{
			if(!Empty(s)) {
				if(!noExpandEV) s = ExpandEnvVar(s);
				Debug.Assert(!LibIsShellPath(s) && !IsUrl(s));

				if(_EndsWithDriveWithoutSep(s)) s = s + "\\"; //API would append current directory

				//note: although slower, call GetFullPathName always, not just when contains @"..\" etc.
				//	Because it does many things (see Normalize doc), not all documented.
				//	We still ~2 times faster than Path.GetFullPath.
				var b = Util.LibCharBuffer.Common; int na = b.Max(s.Length + 10);
				g1: int nr = Api.GetFullPathName(s, na, b.Alloc(na), null);
				if(nr > na) { na = nr; goto g1; }
				if(nr > 0) s = b.ToString(nr);

				if(0 == (flags & NormalizeFlags.DoNotExpandDosPath) && s.IndexOf('~') > 0) s = LibExpandDosPath(s);
				b.Compact();

				if(0 == (flags & NormalizeFlags.DoNotRemoveEndSeparator)) s = _AddRemoveSep(s);
				else if(_EndsWithDriveWithoutSep(s)) s = s + "\\";

				if(0 == (flags & NormalizeFlags.DoNotPrefixLongPath)) s = PrefixLongPathIfNeed(s);
			}
			return s;
		}

		/// <summary>
		/// Calls API GetLongPathName.
		/// Currently does not check whether s contains '~' character. Note: the API is slow.
		/// </summary>
		/// <param name="s">Can be null.</param>
		internal static string LibExpandDosPath(string s)
		{
			if(!Empty(s)) {
				var b = Util.LibCharBuffer.Common; int na = b.Max(300);
				g1: int nr = Api.GetLongPathName(s, b.Alloc(na), na);
				if(nr > na) { na = nr; goto g1; }
				if(nr > 0) s = b.ToString(nr);
			}
			return s;
			//CONSIDER: the API fails if the file does not exist.
			//	Workaround: if filename does not contain '~', pass only the part that contains.
			//	Also, call API only if parts containing '~' are 8 or 12 char long.
		}

		/// <summary>
		/// Prepares path for passing to API that support "..", DOS path etc.
		/// Calls ExpandEnvVar, _AddRemoveSep, PrefixLongPathIfNeed. Optionally throws if !IsFullPath(path).
		/// </summary>
		/// <exception cref="ArgumentException">Not full path (only if throwIfNotFullPath is true).</exception>
		internal static string LibNormalizeMinimally(string path, bool throwIfNotFullPath)
		{
			var s = path;
			s = ExpandEnvVar(s);
			Debug.Assert(!LibIsShellPath(s) && !IsUrl(s));
			if(throwIfNotFullPath && !IsFullPath(s)) throw new ArgumentException("Expected full path.");
			s = _AddRemoveSep(s);
			s = PrefixLongPathIfNeed(s);
			return s;
		}

		/// <summary>
		/// Returns true if starts with "::".
		/// </summary>
		/// <param name="s">Can be null.</param>
		internal static bool LibIsShellPath(string s)
		{
			return s != null && s.Length >= 2 && s[0] == ':' && s[1] == ':';
		}

		/// <summary>
		/// If path is full path (see <see cref="IsFullPath"/>) and does not start with @"\\?\", prepends @"\\?\".
		/// If path is network path (like @"\\computer\folder\..."), makes like @"\\?\UNC\computer\folder\...".
		/// </summary>
		/// <param name="path">
		/// Path. Can be null.
		/// Must not start with "%environmentVariable%", this function does not expand it. See <see cref="ExpandEnvVar"/>.
		/// </param>
		/// <remarks>
		/// Windows API kernel functions support extended-length paths, ie longer than 259 characters. But the path must have this prefix. Windows API shell functions don't support it.
		/// </remarks>
		public static string PrefixLongPath(string path)
		{
			var s = path;
			if(IsFullPath(s) && 0 == _GetPrefixLength(s)) {
				if(s.Length >= 2 && LibIsSepChar(s[0]) && LibIsSepChar(s[1])) s = s.ReplaceAt_(0, 2, @"\\?\UNC\");
				else s = @"\\?\" + s;
			}
			return s;
		}

		/// <summary>
		/// Calls <see cref="PrefixLongPath"/> if path is longer than <see cref="MaxDirectoryPathLength"/> (247).
		/// </summary>
		/// <param name="path">
		/// Path. Can be null.
		/// Must not start with "%environmentVariable%", this function does not expand it. See <see cref="ExpandEnvVar"/>.
		/// </param>
		public static string PrefixLongPathIfNeed(string path)
		{
			if(path != null && path.Length > MaxDirectoryPathLength) path = PrefixLongPath(path);
			return path;

			//info: MaxDirectoryPathLength is max length supported by API CreateDirectory.
		}

		/// <summary>
		/// If path starts with @"\\?\" prefix, removes it.
		/// If path starts with @"\\?\UNC\" prefix, removes @"?\UNC\".
		/// </summary>
		/// <param name="path">
		/// Path. Can be null.
		/// Must not start with "%environmentVariable%", this function does not expand it. See <see cref="ExpandEnvVar"/>.
		/// </param>
		public static string UnprefixLongPath(string path)
		{
			if(!Empty(path)) {
				switch(_GetPrefixLength(path)) {
				case 4: return path.Substring(4);
				case 8: return path.Remove(2, 6);
				}
			}
			return path;
		}

		/// <summary>
		/// If s starts with @"\\?\UNC\", returns 8.
		/// Else if starts with @"\\?\", returns 4.
		/// Else returns 0.
		/// </summary>
		/// <param name="s">Can be null.</param>
		static int _GetPrefixLength(string s)
		{
			if(s == null) return 0;
			int len = s.Length;
			if(len >= 4 && s[2] == '?' && LibIsSepChar(s[0]) && LibIsSepChar(s[1]) && LibIsSepChar(s[3])) {
				if(len >= 8 && LibIsSepChar(s[7]) && s.EqualsPart_(4, "UNC", true)) return 8;
				return 4;
			}
			return 0;
		}

		/// <summary>
		/// Maximal file (not directory) path length supported by all functions (native, .NET and this library).
		/// For longer paths need @"\\?\" prefix. It is supported by most native kernel API (but not shell API) and by most functions of this library.
		/// </summary>
		public const int MaxFilePathLength = 259;
		/// <summary>
		/// Maximal directory path length supported by all functions (native, .NET and this library).
		/// For longer paths need @"\\?\" prefix. It is supported by most native kernel API (but not shell API) and by most functions of this library.
		/// </summary>
		public const int MaxDirectoryPathLength = 247;

		/// <summary>
		/// Replaces characters that cannot be used in file names.
		/// Also corrects other forms of invalid or problematic filename: trims spaces and other blank characters; replaces "." at the end; prepends "@" if a reserved name like "CON" or "CON.txt"; returns "-" if name is null/empty/whitespace.
		/// Returns valid filename. However it can be too long (itself or when combined with a directory path).
		/// </summary>
		/// <param name="name">Initial filename.</param>
		/// <param name="invalidCharReplacement">A string that will replace each invalid character. Default "-".</param>
		public static string CorrectFileName(string name, string invalidCharReplacement = "-")
		{
			if(name == null || (name = name.Trim()).Length == 0) return "-";
			name = name.RegexReplace_(_rxInvalidFN1, invalidCharReplacement).Trim();
			if(name.RegexIs_(_rxInvalidFN2)) name = "@" + name;
			return name;
		}

		const string _rxInvalidFN1 = @"\.$|[\\/|<>?*:""\x00-\x1f]";
		const string _rxInvalidFN2 = @"(?i)^(CON|PRN|AUX|NUL|COM\d|LPT\d)(\.|$)";

		/// <summary>
		/// Returns true if name cannot be used for a file name, eg contains '\\' etc characters or is empty.
		/// More info: <see cref="CorrectFileName"/>.
		/// </summary>
		/// <param name="name">Any string. Can be null. Example: "name.txt".</param>
		public static bool IsInvalidFileName(string name)
		{
			if(name == null || (name = name.Trim()).Length == 0) return true;
			return name.RegexIs_(_rxInvalidFN1) || name.RegexIs_(_rxInvalidFN2);
		}

		/// <summary>
		/// Gets filename with extension.
		/// Returns "" if there is no filename.
		/// Returns null if path is null.
		/// </summary>
		/// <param name="path">Path or filename.</param>
		/// <remarks>
		/// Similar to <see cref="Path.GetFileName"/>. Some diferences: does not throw exceptions; if ends with '\\' or '/', gets part before it, eg "B" from @"C:\A\B\".
		/// Supports separators '\\' and '/'.
		/// Also supports URL and shell parsing names like @"::{CLSID-1}\0\::{CLSID-2}".
		/// Example paths and results:
		/// <code>
		/// @"C:\A\B\file.txt" -> "file.txt".
		/// "file.txt" -> "file.txt".
		/// "file" -> "file".
		/// @"C:\A\B" -> "B".
		/// @"C:\A\B\" -> "B".
		/// @"C:\A\/B\/" -> "B".
		/// @"C:\" -> "".
		/// @"C:" -> "".
		/// @"\\network\share" -> "share".
		/// @"C:\aa\file.txt:alt.stream" -> "file.txt:alt.stream".
		/// "http://a.b.c" -> "a.b.c".
		/// "::{A}\::{B}" -> "::{B}".
		/// "" -> "".
		/// null -> null.
		/// </code>
		/// </remarks>
		public static string GetFileName(string path)
		{
			return _GetPathPart(path, _PathPart.NameWithExt);
		}

		/// <summary>
		/// Gets filename without extension.
		/// Returns "" if there is no filename.
		/// Returns null if path is null.
		/// </summary>
		/// <param name="path">Path or filename.</param>
		/// <remarks>
		/// Similar to <see cref="Path.GetFileNameWithoutExtension"/> Some diferences: does not throw exceptions; if ends with '\\' or '/', gets part before it, eg "B" from @"C:\A\B\" (then does not remove extension from it because it is a directory name).
		/// Supports separators '\\' and '/'.
		/// Also supports URL and shell parsing names like @"::{CLSID-1}\0\::{CLSID-2}".
		/// Example paths and results:
		/// <code>
		/// @"C:\A\B\file.txt" -> "file".
		/// "file.txt" -> "file".
		/// "file" -> "file".
		/// @"C:\A\B" -> "B".
		/// @"C:\A\B\" -> "B".
		/// @"C:\A\B.B\" -> "B.B".
		/// @"C:\" -> "".
		/// @"C:" -> "".
		/// @"\\network\share" -> "share".
		/// @"C:\aa\file.txt:alt.stream" -> "file.txt:alt".
		/// "http://a.b.c" -> "a.b".
		/// "::{A}\::{B}" -> "::{B}".
		/// "" -> "".
		/// null -> null.
		/// </code>
		/// </remarks>
		public static string GetFileNameWithoutExtension(string path)
		{
			return _GetPathPart(path, _PathPart.NameWithoutExt);
		}

		/// <summary>
		/// Gets filename extension, like ".txt".
		/// Returns "" if there is no extension.
		/// Returns null if path is null.
		/// </summary>
		/// <param name="path">Path or filename.</param>
		/// <remarks>
		/// Supports separators '\\' and '/'.
		/// Like <see cref="Path.GetExtension"/>, but does not throw exceptions.
		/// </remarks>
		public static string GetExtension(string path)
		{
			return _GetPathPart(path, _PathPart.Ext);
		}

		/// <summary>
		/// Gets filename extension and path part without the extension.
		/// More info: <see cref="GetExtension(string)"/>.
		/// </summary>
		/// <param name="path">Path or filename.</param>
		/// <param name="pathWithoutExtension">Receives path part without the extension. Can be the same variable as path.</param>
		public static string GetExtension(string path, out string pathWithoutExtension)
		{
			var ext = GetExtension(path);
			if(ext != null && ext.Length > 0) pathWithoutExtension = path.Remove(path.Length - ext.Length);
			else pathWithoutExtension = path;
			return ext;
		}

		/// <summary>
		/// Removes filename part from path. By default also removes separator ('\\' or '/') if it is not after drive name (eg "C:").
		/// Returns "" if the string is a filename.
		/// Returns null if the string is a root (like @"C:\" or "C:" or @"\\server\share" or "http:").
		/// Returns null if the string is null.
		/// </summary>
		/// <param name="path">Path or filename.</param>
		/// <param name="withSeparator">
		/// Don't remove the separator character(s) ('\\' or '/').
		/// Examples: from @"C:\A\B" gets @"C:\A\", not @"C:\A"; from "http://x.y" gets "http://", not "http:".</param>
		/// <remarks>
		/// Similar to <see cref="Path.GetDirectoryName"/>. Some diferences: does not throw exceptions; skips '\\' or '/' at the end (eg from @"C:\A\B\" gets @"C:\A", not @"C:\A\B"); does not expand DOS path; much faster.
		/// Parses raw string. You may want to <see cref="Normalize"/> it at first.
		/// Supports separators '\\' and '/'.
		/// Also supports URL and shell parsing names like @"::{CLSID-1}\0\::{CLSID-2}".
		/// Example paths and results (withSeparator=false):
		/// <code>
		/// @"C:\A\B\file.txt" -> @"C:\A\B".
		/// "file.txt" -> "".
		/// @"C:\A\B\" -> @"C:\A".
		/// @"C:\A\/B\/" -> @"C:\A".
		/// @"C:\" -> null.
		/// @"\\network\share" -> null.
		/// "http:" -> null.
		/// @"C:\aa\file.txt:alt.stream" -> "C:\aa".
		/// "http://a.b.c" -> "http:".
		/// "::{A}\::{B}" -> "::{A}".
		/// "" -> "".
		/// null -> null.
		/// </code>
		/// </remarks>
		public static string GetDirectoryPath(string path, bool withSeparator = false)
		{
			return _GetPathPart(path, _PathPart.Dir, withSeparator);
		}

		enum _PathPart { Dir, NameWithExt, NameWithoutExt, Ext, };

		static string _GetPathPart(string s, _PathPart what, bool withSeparator = false)
		{
			if(s == null) return null;
			int len = s.Length, i, iExt = -1;

			//rtrim '\\' and '/' etc
			for(i = len; i > 0 && LibIsSepChar(s[i - 1]); i--) {
				if(what == _PathPart.Ext) return "";
				if(what == _PathPart.NameWithoutExt) what = _PathPart.NameWithExt;
			}
			len = i;

			//if ends with ":" or @":\", it is either drive or URL root or invalid
			if(len > 0 && s[len - 1] == ':' && !LibIsShellPath(s)) return (what == _PathPart.Dir) ? null : "";

			//find '\\' or '/'. Also '.' if need.
			//Note: we don't split at ':', which could be used for alt stream or URL port or in shell parsing name as non-separator. This library does not support paths like "C:relative path".
			while(--i >= 0) {
				char c = s[i];
				if(c == '.') {
					if(what < _PathPart.NameWithoutExt) continue;
					if(iExt < 0) iExt = i;
					if(what == _PathPart.Ext) break;
				} else if(c == '\\' || c == '/') {
					break;
				}
			}
			if(iExt >= 0 && iExt == len - 1) iExt = -1; //eg ends with ".."
			if(what == _PathPart.NameWithoutExt && iExt < 0) what = _PathPart.NameWithExt;

			switch(what) {
			case _PathPart.Ext:
				if(iExt >= 0) return s.Substring(iExt);
				break;
			case _PathPart.NameWithExt:
				len -= ++i; if(len == 0) return "";
				return s.Substring(i, len);
			case _PathPart.NameWithoutExt:
				i++;
				return s.Substring(i, iExt - i);
			case _PathPart.Dir:
				//skip multiple separators
				if(!withSeparator && i > 0) {
					for(; i > 0; i--) { var c = s[i - 1]; if(!(c == '\\' || c == '/')) break; }
					if(i == 0) return null;
				}
				if(i > 0) {
					//returns null if i is in root
					int j = GetRootLength(s); if(j > 0 && LibIsSepChar(s[j - 1])) j--;
					if(i < j) return null;

					if(withSeparator || _EndsWithDriveWithoutSep(s, i)) i++;
					return s.Remove(i);
				}
				break;
			}
			return "";
		}

		/// <summary>
		/// Returns true if s is like ".ext" and the ext part does not contain characters ".\\/:".
		/// </summary>
		/// <param name="s">Can be null.</param>
		internal static bool LibIsExtension(string s)
		{
			if(s == null || s.Length < 2 || s[0] != '.') return false;
			for(int i = 1; i < s.Length; i++) {
				switch(s[i]) { case '.': case '\\': case '/': case ':': return false; }
			}
			return true;
		}

		/// <summary>
		/// Returns true if s is like "protocol:" and not like "c:" or "protocol:more".
		/// </summary>
		/// <param name="s">Can be null.</param>
		internal static bool LibIsProtocol(string s)
		{
			return s != null && s.EndsWith_(':') && GetUrlProtocolLength(s) == s.Length;
		}

		/// <summary>
		/// Gets path with unique filename for a new file or directory. 
		/// If the specified path is of an existing file or directory, returns path where the filename part is modified like "file 2.txt", "file 3.txt" etc. Else returns unchanged path.
		/// </summary>
		/// <param name="path">Suggested full path.</param>
		/// <param name="isDirectory">The path is for a directory. The number is always appended at the very end, not before .extension.</param>
		public static string MakeUnique(string path, bool isDirectory)
		{
			if(!Files.ExistsAsAny(path)) return path;
			string ext = isDirectory ? null : GetExtension(path, out path);
			for(int i = 2; ; i++) {
				var s = path + " " + i + ext;
				if(!Files.ExistsAsAny(s)) return s;
			}
		}
	}
}
