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
//using System.Xml.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	/// <summary>
	/// Extends <see cref="Path"/>.
	/// </summary>
	public static unsafe class APath
	{
		/// <summary>
		/// If path starts with <c>"%"</c> or <c>"\"%"</c>, expands environment variables enclosed in %, else just returns path.
		/// Also supports known folder names, like <c>"%AFolders.Documents%"</c>. More info in Remarks.
		/// </summary>
		/// <param name="path">Any string. Can be null.</param>
		/// <remarks>
		/// Supports known folder names. See <see cref="AFolders"/>.
		/// Example: <c>@"%AFolders.Documents%\file.txt"</c>.
		/// Example: <c>@"%AFolders.Virtual.ControlPanel%" //gets ":: HexEncodedITEMIDLIST"</c>.
		/// Usually known folders are used like <c>string path = AFolders.Documents + "file.txt"</c>. It's easier and faster. However it cannot be used when you want to store paths in text files, registry, etc. Then this feature is useful.
		/// To get known folder path, this function calls <see cref="AFolders.GetFolder"/>.
		///
		/// This function is called by many functions of classes <b>APath</b>, <b>AFile</b>, <b>AExec</b>, <b>AIcon</b>, some others, therefore all they support environment variables and known folders in path string.
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

			//support known folders, like @"%AFolders.Documents%\..."
			if(i > 12 && s.Starts("%AFolders.")) {
				var k = AFolders.GetFolder(s.Substring(10, i - 10));
				if(k != null) return k + s.Substring(i + 1);
				return s; //TODO: dangerous to return such string. Eg then can create file like @"%AFolders.Documents%\..." in current directory. Should throw exception or show warning and return invalid path.

				//CONSIDER: or/also support like @"%<Documents>%\..." or @"<Documents>\..." or @"%.Documents%\..." or @"%%Documents%%\..."
			}

			for(int na = s.Length + 100; ;) {
				var b = Util.AMemoryArray.LibChar(ref na);
				int nr = Api.ExpandEnvironmentStrings(s, b, na);
				if(nr > na) na = nr;
				else if(nr > 0) {
					var R = b.ToString(nr - 1);
					if(R == s) return R;
					return ExpandEnvVar(R); //can be %envVar2% in envVar1 value
				} else return s;
			}
		}

		/// <summary>
		/// Gets environment variable's value.
		/// Returns "" if variable not found.
		/// Does not support AFolders.X.
		/// </summary>
		/// <param name="name">Case-insensitive name. Without %.</param>
		/// <remarks>
		/// Environment variable values cannot be "" or null. Setting empty value removes the variable.
		/// </remarks>
		internal static string LibGetEnvVar(string name)
		{
			for(int na = 300; ;) {
				var b = Util.AMemoryArray.LibChar(ref na);
				int nr = Api.GetEnvironmentVariable(name, b, na);
				if(nr > na) na = nr; else return (nr == 0) ? "" : b.ToString(nr);
			}
		}

		/// <summary>
		/// Returns true if environment variable exists.
		/// </summary>
		/// <param name="name">Case-insensitive name.</param>
		internal static bool LibEnvVarExists(string name)
		{
			return 0 != Api.GetEnvironmentVariable(name, null, 0);
		}

		/// <summary>
		/// Returns true if the string is full path, like <c>@"C:\a\b.txt"</c> or <c>@"C:"</c> or <c>@"\\server\share\..."</c>:
		/// </summary>
		/// <param name="path">Any string. Can be null.</param>
		/// <remarks>
		/// Returns true if <i>path</i> matches one of these wildcard patterns:
		/// - <c>@"?:\*"</c> - local path, like <c>@"C:\a\b.txt"</c>. Here ? is A-Z, a-z.
		/// - <c>@"?:"</c> - drive name, like <c>@"C:"</c>. Here ? is A-Z, a-z.
		/// - <c>@"\\*"</c> - network path, like <c>@"\\server\share\..."</c>. Or has prefix <c>@"\\?\"</c>.
		/// 
		/// Supports <c>'/'</c> characters too.
		/// 
		/// Supports only file-system paths. Returns false if path is URL (<see cref="IsUrl"/>) or starts with <c>"::"</c>.
		/// 
		/// If path starts with <c>"%environmentVariable%"</c>, shows warning and returns false. You should at first expand environment variables with <see cref="ExpandEnvVar"/> or instead use <see cref="IsFullPathExpandEnvVar"/>.
		/// </remarks>
		public static bool IsFullPath(string path)
		{
			var s = path;
			int len = s.Lenn();

			if(len >= 2) {
				if(s[1] == ':' && AChar.IsAsciiAlpha(s[0])) {
					return len == 2 || LibIsSepChar(s[2]);
					//info: returns false if eg "c:abc" which means "abc" in current directory of drive "c:"
				}
				switch(s[0]) {
				case '\\':
				case '/':
					return LibIsSepChar(s[1]);
				case '%':
#if true
					if(!ExpandEnvVar(s).Starts('%'))
						PrintWarning("Path starts with %environmentVariable%. Use APath.IsFullPathExpandEnvVar instead.");
#else
					s = ExpandEnvVar(s); //quite fast. 70% slower than just LibEnvVarExists, but reliable.
					return !s.Starts('%') && IsFullPath(s);
#endif
					break;
				}
			}

			return false;
		}

		/// <summary>
		/// Expands environment variables and calls <see cref="IsFullPath"/>.
		/// Returns true if the string is full path, like <c>@"C:\a\b.txt"</c> or <c>@"C:"</c> or <c>@"\\server\share\..."</c>:
		/// </summary>
		/// <param name="path">
		/// Any string. Can be null.
		/// If starts with '%' character, calls <see cref="IsFullPath"/> with expanded environment variables (<see cref="ExpandEnvVar"/>). If it returns true, replaces the passed variable with the expanded path string.
		/// </param>
		/// <remarks>
		/// Returns true if <i>path</i> matches one of these wildcard patterns:
		/// - <c>@"?:\*"</c> - local path, like <c>@"C:\a\b.txt"</c>. Here ? is A-Z, a-z.
		/// - <c>@"?:"</c> - drive name, like <c>@"C:"</c>. Here ? is A-Z, a-z.
		/// - <c>@"\\*"</c> - network path, like <c>@"\\server\share\..."</c>. Or has prefix <c>@"\\?\"</c>.
		/// Supports '/' characters too.
		/// Supports only file-system paths. Returns false if path is URL (<see cref="IsUrl"/>) or starts with <c>"::"</c>.
		/// </remarks>
		public static bool IsFullPathExpandEnvVar(ref string path)
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
		/// If the string does not start with a drive or network folder path, returns 0 or prefix length (<c>@"\\?\"</c> or <c>@"\\?\UNC\"</c>).
		/// </summary>
		/// <param name="path">Full path or any string. Can be null. Should not be <c>@"%environmentVariable%\..."</c>.</param>
		/// <remarks>
		/// Supports prefixes <c>@"\\?\"</c> and <c>@"\\?\UNC\"</c>.
		/// Supports separators <c>'\\'</c> and <c>'/'</c>.
		/// </remarks>
		public static int GetRootLength(string path)
		{
			var s = path;
			int i = 0, len = (s == null) ? 0 : s.Length;
			if(len >= 2) {
				switch(s[1]) {
				case ':':
					if(AChar.IsAsciiAlpha(s[i])) {
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
		/// </summary>
		/// <param name="s">A URL or path or any string. Can be null.</param>
		/// <remarks>
		/// URL examples: <c>"http:"</c> (returns 5), <c>"http://www.x.com"</c> (returns 5), <c>"file:///path"</c> (returns 5), <c>"shell:etc"</c> (returns 6).
		/// 
		/// The protocol can be unknown. The function just checks string format, which is an ASCII alpha character followed by one or more ASCII alpha-numeric, '.', '-', '+' characters, followed by ':' character.
		/// </remarks>
		public static int GetUrlProtocolLength(string s)
		{
			int len = (s == null) ? 0 : s.Length;
			if(len > 2 && AChar.IsAsciiAlpha(s[0]) && s[1] != ':') {
				for(int i = 1; i < len; i++) {
					var c = s[i];
					if(c == ':') return i + 1;
					if(!(AChar.IsAsciiAlphaDigit(c) || c == '.' || c == '-' || c == '+')) break;
				}
			}
			return 0;
			//info: API PathIsURL lies, like most shlwapi.dll functions.
		}

		/// <summary>
		/// Returns true if the string starts with a URL protocol name (existing or not) and ':' character.
		/// Calls <see cref="GetUrlProtocolLength"/> and returns true if it's not 0.
		/// </summary>
		/// <param name="s">A URL or path or any string. Can be null.</param>
		/// <remarks>
		/// URL examples: <c>"http:"</c>, <c>"http://www.x.com"</c>, <c>"file:///path"</c>, <c>"shell:etc"</c>.
		/// </remarks>
		public static bool IsUrl(string s)
		{
			return 0 != GetUrlProtocolLength(s);
		}

		/// <summary>
		/// Combines two path parts using character <c>'\\'</c>. For example directory path and file name.
		/// </summary>
		/// <param name="s1">First part. Usually a directory.</param>
		/// <param name="s2">Second part. Usually a filename or relative path.</param>
		/// <param name="s2CanBeFullPath">s2 can be full path. If it is, ignore s1 and return s2 with expanded environment variables. If false (default), simply combines s1 and s2.</param>
		/// <param name="prefixLongPath">Call <see cref="PrefixLongPathIfNeed"/> which may prepend <c>@"\\?\"</c> if the result path is very long. Default true.</param>
		/// <remarks>
		/// If s1 and s2 are null or "", returns "". Else if s1 is null or "", returns s2. Else if s2 is null or "", returns s1.
		/// Similar to System.IO.Path.Combine. Main differences: does not throw exceptions; has some options.
		/// Does not expand environment variables. For it use <see cref="ExpandEnvVar"/> before, or <see cref="Normalize"/> instead. Path that starts with an environment variable is considerd not full path.
		/// </remarks>
		/// <seealso cref="Normalize"/>
		public static string Combine(string s1, string s2, bool s2CanBeFullPath = false, bool prefixLongPath = true)
		{
			string r;
			if(Empty(s1)) r = s2 ?? "";
			else if(Empty(s2)) r = s1 ?? "";
			else if(s2CanBeFullPath && IsFullPath(s2)) r = s2;
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
			if(prefixLongPath) r = PrefixLongPathIfNeed(r);
			return r;
		}

		/// <summary>
		/// Combines two path parts.
		/// Unlike <see cref="Combine"/>, fails if some part is empty or <c>@"\"</c> or if s2 is <c>@"\\"</c>. Also does not check s2 full path.
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

		/// <summary>
		/// Returns true if character <c>c == '\\' || c == '/'</c>.
		/// </summary>
		internal static bool LibIsSepChar(char c) { return c == '\\' || c == '/'; }

		/// <summary>
		/// Returns true if ends with ':' preceded by a drive letter, like "C:" or "more\C:", but not like "moreC:".
		/// </summary>
		/// <param name="s">Can be null.</param>
		/// <param name="length">Use when want to check drive at a middle, not at the end. Eg returns true if s is <c>@"C:\more"</c> and length is 2.</param>
		static bool _EndsWithDriveWithoutSep(string s, int length = -1)
		{
			if(s == null) return false;
			int i = ((length < 0) ? s.Length : length) - 1;
			if(i < 1 || s[i] != ':') return false;
			if(!AChar.IsAsciiAlpha(s[--i])) return false;
			if(i > 0 && !LibIsSepChar(s[i - 1])) return false;
			return true;
		}

		/// <summary>
		/// Ensures that s either ends with a valid drive path (eg <c>@"C:\"</c> but not "C:") or does not end with <c>'\\'</c> or <c>'/'</c> (unless would become empty if removed).
		/// </summary>
		/// <param name="s">Can be null.</param>
		static string _AddRemoveSep(string s)
		{
			if(s != null) {
				int i = s.Length - 1;
				if(i > 0) {
					if(LibIsSepChar(s[i]) && s[i - 1] != ':') {
						var s2 = s.TrimChars(@"\/");
						if(s2.Length != 0) s = s2;
					}
					if(_EndsWithDriveWithoutSep(s)) s += "\\";
				}
			}
			return s;
		}

		/// <summary>
		/// Makes normal full path from path that can contain special substrings etc.
		/// </summary>
		/// <param name="path">Any path.</param>
		/// <param name="defaultParentDirectory">If path is not full path, combine it with defaultParentDirectory to make full path.</param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">path is not full path, and <i>defaultParentDirectory</i> is not used or does not make it full path.</exception>
		/// <remarks>
		/// The sequence of actions:
		/// 1. If path starts with '%' character, expands environment variables and special folder names. See <see cref="ExpandEnvVar"/>.
		/// 2. If path is not full path but looks like URL, and used flag CanBeUrl, returns path.
		/// 3. If path is not full path, and defaultParentDirectory is not null/"", combines path with ExpandEnvVar(defaultParentDirectory).
		/// 4. If path is not full path, throws exception.
		/// 5. Calls API <msdn>GetFullPathName</msdn>. It replaces <c>'/'</c> with <c>'\\'</c>, replaces multiple <c>'\\'</c> with single (where need), processes <c>@"\.."</c> etc, trims spaces, etc.
		/// 6. If no flag DoNotExpandDosPath, if path looks like a short DOS path version (contains <c>'~'</c> etc), calls API <msdn>GetLongPathName</msdn>. It converts short DOS path to normal path, if possible, for example <c>@"c:\progra~1"</c> to <c>@"c:\program files"</c>. It is slow. It converts path only if the file exists.
		/// 7. If no flag DoNotRemoveEndSeparator, removes <c>'\\'</c> character at the end, unless it is like <c>@"C:\"</c>.
		/// 8. Appends <c>'\\'</c> character if ends with a drive name (eg <c>"C:"</c> -> <c>@"C:\"</c>).
		/// 9. If no flag DoNotPrefixLongPath, calls <see cref="PrefixLongPathIfNeed"/>, which adds <c>@"\\?\"</c> etc prefix if path is very long.
		/// 
		/// Similar to <see cref="Path.GetFullPath"/>. Main differences: this function expands environment variables, does not support relative paths, supports <c>@"\\?\very long path"</c>, trims <c>'\\'</c> at the end if need, does not throw exceptions when it thinks that path is invalid (except when path is not full).
		/// </remarks>
		public static string Normalize(string path, string defaultParentDirectory = null, PNFlags flags = 0)
		{
			path = ExpandEnvVar(path);
			if(!IsFullPath(path)) { //note: not EEV
				if(0 != (flags & PNFlags.CanBeUrlOrShell)) if(LibIsShellPath(path) || IsUrl(path)) return path;
				if(Empty(defaultParentDirectory)) goto ge;
				path = LibCombine(ExpandEnvVar(defaultParentDirectory), path);
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
		internal static string LibNormalize(string s, PNFlags flags = 0, bool noExpandEV = false)
		{
			if(!Empty(s)) {
				if(!noExpandEV) s = ExpandEnvVar(s);
				Debug.Assert(!LibIsShellPath(s) && !IsUrl(s));

				if(_EndsWithDriveWithoutSep(s)) s += "\\"; //API would append current directory

				//note: although slower, call GetFullPathName always, not just when contains @"..\" etc.
				//	Because it does many things (see Normalize doc), not all documented.
				//	We still ~2 times faster than Path.GetFullPath.
				for(int na = 300; ;) {
					var b = Util.AMemoryArray.LibChar(ref na);
					int nr = Api.GetFullPathName(s, na, b, null);
					if(nr > na) na = nr; else { if(nr > 0) s = b.ToString(nr); break; }
				}

				if(0 == (flags & PNFlags.DontExpandDosPath) && LibIsPossiblyDos(s)) s = LibExpandDosPath(s);

				if(0 == (flags & PNFlags.DontRemoveEndSeparator)) s = _AddRemoveSep(s);
				else if(_EndsWithDriveWithoutSep(s)) s += "\\";

				if(0 == (flags & PNFlags.DontPrefixLongPath)) s = PrefixLongPathIfNeed(s);
			}
			return s;
		}

		/// <summary>
		/// Prepares path for passing to API that support "..", DOS path etc.
		/// Calls ExpandEnvVar, _AddRemoveSep, PrefixLongPathIfNeed. Optionally throws if !IsFullPath(path).
		/// </summary>
		/// <exception cref="ArgumentException">Not full path (only if throwIfNotFullPath is true).</exception>
		internal static string LibNormalizeMinimally(string path, bool throwIfNotFullPath)
		{
			var s = ExpandEnvVar(path);
			Debug.Assert(!LibIsShellPath(s) && !IsUrl(s));
			if(throwIfNotFullPath && !IsFullPath(s)) throw new ArgumentException($"Not full path: '{path}'.");
			s = _AddRemoveSep(s);
			s = PrefixLongPathIfNeed(s);
			return s;
		}

		/// <summary>
		/// Prepares path for passing to .NET file functions.
		/// Calls ExpandEnvVar, _AddRemoveSep. Throws if !IsFullPath(path).
		/// </summary>
		/// <exception cref="ArgumentException">Not full path.</exception>
		internal static string LibNormalizeForNET(string path)
		{
			var s = ExpandEnvVar(path);
			Debug.Assert(!LibIsShellPath(s) && !IsUrl(s));
			if(!IsFullPath(s)) throw new ArgumentException($"Not full path: '{path}'.");
			return _AddRemoveSep(s);
		}

		/// <summary>
		/// Calls API GetLongPathName.
		/// Does not check whether s contains '~' character etc. Note: the API is slow.
		/// </summary>
		/// <param name="s">Can be null.</param>
		internal static string LibExpandDosPath(string s)
		{
			if(!Empty(s)) {
				for(int na = 300; ;) {
					var b = Util.AMemoryArray.LibChar(ref na);
					int nr = Api.GetLongPathName(s, b, na);
					if(nr > na) na = nr; else { if(nr > 0) s = b.ToString(nr); break; }
				}
			}
			return s;
			//CONSIDER: the API fails if the file does not exist.
			//	Workaround: if filename does not contain '~', pass only the part that contains.
		}

		/// <summary>
		/// Returns true if pathOrFilename looks like a DOS filename or path.
		/// Examples: <c>"abcde~12"</c>, <c>"abcde~12.txt"</c>, <c>@"c:\path\abcde~12.txt"</c>, <c>"c:\abcde~12\path"</c>.
		/// </summary>
		/// <param name="s">Can be null.</param>
		internal static bool LibIsPossiblyDos(string s)
		{
			//Print(s);
			if(s != null && s.Length >= 8) {
				for(int i = 0; (i = s.IndexOf('~', i + 1)) > 0;) {
					int j = i + 1, k = 0;
					for(; k < 6 && j < s.Length; k++, j++) if(!AChar.IsAsciiDigit(s[j])) break;
					if(k == 0) continue;
					char c = j < s.Length ? s[j] : '\\';
					if(c == '\\' || c == '/' || (c == '.' && j == s.Length - 4)) {
						for(j = i; j > 0; j--) {
							c = s[j - 1]; if(c == '\\' || c == '/') break;
						}
						if(j == i - (7 - k)) return true;
					}
				}
			}
			return false;
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
		/// If path is full path (see <see cref="IsFullPath"/>) and does not start with <c>@"\\?\"</c>, prepends <c>@"\\?\"</c>.
		/// If path is network path (like <c>@"\\computer\folder\..."</c>), makes like <c>@"\\?\UNC\computer\folder\..."</c>.
		/// </summary>
		/// <param name="path">
		/// Path. Can be null.
		/// Must not start with <c>"%environmentVariable%"</c>. This function does not expand it. See <see cref="ExpandEnvVar"/>.
		/// </param>
		/// <remarks>
		/// Windows API kernel functions support extended-length paths, ie longer than 259 characters. But the path must have this prefix. Windows API shell functions don't support it.
		/// </remarks>
		public static string PrefixLongPath(string path)
		{
			var s = path;
			if(IsFullPath(s) && 0 == _GetPrefixLength(s)) {
				if(s.Length >= 2 && LibIsSepChar(s[0]) && LibIsSepChar(s[1])) s = s.ReplaceAt(0, 2, @"\\?\UNC\");
				else s = @"\\?\" + s;
			}
			return s;
		}

		/// <summary>
		/// Calls <see cref="PrefixLongPath"/> if path is longer than <see cref="MaxDirectoryPathLength"/> (247).
		/// </summary>
		/// <param name="path">
		/// Path. Can be null.
		/// Must not start with <c>"%environmentVariable%"</c>. This function does not expand it. See <see cref="ExpandEnvVar"/>.
		/// </param>
		public static string PrefixLongPathIfNeed(string path)
		{
			if(path != null && path.Length > MaxDirectoryPathLength) path = PrefixLongPath(path);
			return path;

			//info: MaxDirectoryPathLength is max length supported by API CreateDirectory.
		}

		/// <summary>
		/// If path starts with <c>@"\\?\"</c> prefix, removes it.
		/// If path starts with <c>@"\\?\UNC\"</c> prefix, removes <c>@"?\UNC\"</c>.
		/// </summary>
		/// <param name="path">
		/// Path. Can be null.
		/// Must not start with <c>"%environmentVariable%"</c>. This function does not expand it. See <see cref="ExpandEnvVar"/>.
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
		/// If s starts with <c>@"\\?\UNC\"</c>, returns 8.
		/// Else if starts with <c>@"\\?\"</c>, returns 4.
		/// Else returns 0.
		/// </summary>
		/// <param name="s">Can be null.</param>
		static int _GetPrefixLength(string s)
		{
			if(s == null) return 0;
			int len = s.Length;
			if(len >= 4 && s[2] == '?' && LibIsSepChar(s[0]) && LibIsSepChar(s[1]) && LibIsSepChar(s[3])) {
				if(len >= 8 && LibIsSepChar(s[7]) && s.Eq(4, "UNC", true)) return 8;
				return 4;
			}
			return 0;
		}

		/// <summary>
		/// Maximal file (not directory) path length supported by all functions (native, .NET and this library).
		/// For longer paths need <c>@"\\?\"</c> prefix. It is supported by most native kernel API (but not shell API) and by most functions of this library.
		/// </summary>
		public const int MaxFilePathLength = 259;
		/// <summary>
		/// Maximal directory path length supported by all functions (native, .NET and this library).
		/// For longer paths need <c>@"\\?\"</c> prefix. It is supported by most native kernel API (but not shell API) and by most functions of this library.
		/// </summary>
		public const int MaxDirectoryPathLength = 247;

		/// <summary>
		/// Replaces characters that cannot be used in file names.
		/// Returns valid filename. However it can be too long (itself or when combined with a directory path).
		/// </summary>
		/// <param name="name">Initial filename.</param>
		/// <param name="invalidCharReplacement">A string that will replace each invalid character. Default <c>"-"</c>.</param>
		/// <remarks>
		/// Also corrects other forms of invalid or problematic filename: trims spaces and other blank characters; replaces <c>"."</c> at the end; prepends <c>"@"</c> if a reserved name like <c>"CON"</c> or <c>"CON.txt"</c>; returns <c>"-"</c> if name is null/empty/whitespace.
		/// </remarks>
		public static string CorrectFileName(string name, string invalidCharReplacement = "-")
		{
			if(name == null || (name = name.Trim()).Length == 0) return "-";
			name = name.RegexReplace(_rxInvalidFN1, invalidCharReplacement).Trim();
			if(name.Regex(_rxInvalidFN2)) name = "@" + name;
			return name;
		}

		const string _rxInvalidFN1 = @"\.$|[\\/|<>?*:""\x00-\x1f]";
		const string _rxInvalidFN2 = @"(?i)^(CON|PRN|AUX|NUL|COM\d|LPT\d)(\.|$)";

		/// <summary>
		/// Returns true if name cannot be used for a file name, eg contains <c>'\\'</c> etc characters or is empty.
		/// More info: <see cref="CorrectFileName"/>.
		/// </summary>
		/// <param name="name">Any string. Example: "name.txt". Can be null.</param>
		public static bool IsInvalidFileName(string name)
		{
			if(name == null || (name = name.Trim()).Length == 0) return true;
			return name.Regex(_rxInvalidFN1) || name.Regex(_rxInvalidFN2);
		}

		/// <summary>
		/// Gets filename with or without extension.
		/// Returns "" if there is no filename.
		/// Returns null if path is null.
		/// </summary>
		/// <param name="path">Path or filename. Can be null.</param>
		/// <param name="withoutExtension">Remove extension, unless <i>path</i> ends with <c>'\\'</c> or <c>'/'</c>.</param>
		/// <remarks>
		/// Similar to <see cref="Path.GetFileName"/> and <see cref="Path.GetFileNameWithoutExtension"/>. Some differences: does not throw exceptions; if ends with <c>'\\'</c> or <c>'/'</c>, gets part before it, eg <c>"B"</c> from <c>@"C:\A\B\"</c>.
		/// 
		/// Supports separators <c>'\\'</c> and <c>'/'</c>.
		/// Also supports URL and shell parsing names like <c>@"::{CLSID-1}\0\::{CLSID-2}"</c>.
		/// 
		/// Examples:
		/// 
		/// | path | result
		/// | - | -
		/// | <c>@"C:\A\B\file.txt"</c> | <c>"file.txt"</c>
		/// | <c>"file.txt"</c> | <c>"file.txt"</c>
		/// | <c>"file"</c> | <c>"file"</c>
		/// | <c>@"C:\A\B"</c> | <c>"B"</c>
		/// | <c>@"C:\A\B\"</c> | <c>"B"</c>
		/// | <c>@"C:\A\/B\/"</c> | <c>"B"</c>
		/// | <c>@"C:\"</c> | <c>""</c>
		/// | <c>@"C:"</c> | <c>""</c>
		/// | <c>@"\\network\share"</c> | <c>"share"</c>
		/// | <c>@"C:\aa\file.txt:alt.stream"</c> | <c>"file.txt:alt.stream"</c>
		/// | <c>"http://a.b.c"</c> | <c>"a.b.c"</c>
		/// | <c>"::{A}\::{B}"</c> | <c>"::{B}"</c>
		/// | <c>""</c> | <c>""</c>
		/// | <c>null</c> | <c>null</c>
		/// 
		/// Examples when <i>withoutExtension</i> true:
		/// 
		/// | path | result
		/// | - | -
		/// | <c>@"C:\A\B\file.txt"</c> | <c>"file"</c>
		/// | <c>"file.txt"</c> | <c>"file"</c>
		/// | <c>"file"</c> | <c>"file"</c>
		/// | <c>@"C:\A\B"</c> | <c>"B"</c>
		/// | <c>@"C:\A\B\"</c> | <c>"B"</c>
		/// | <c>@"C:\A\B.B\"</c> | <c>"B.B"</c>
		/// | <c>@"C:\aa\file.txt:alt.stream"</c> | <c>"file.txt:alt"</c>
		/// | <c>"http://a.b.c"</c> | <c>"a.b"</c>
		/// </remarks>
		public static string GetFileName(string path, bool withoutExtension = false)
		{
			return _GetPathPart(path, withoutExtension ? _PathPart.NameWithoutExt : _PathPart.NameWithExt);
		}

		/// <summary>
		/// Gets filename extension, like <c>".txt"</c>.
		/// Returns "" if there is no extension.
		/// Returns null if path is null.
		/// </summary>
		/// <param name="path">Path or filename. Can be null.</param>
		/// <remarks>
		/// Supports separators <c>'\\'</c> and <c>'/'</c>.
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
		/// <param name="path">Path or filename. Can be null.</param>
		/// <param name="pathWithoutExtension">Receives path part without the extension. Can be the same variable as path.</param>
		public static string GetExtension(string path, out string pathWithoutExtension)
		{
			var ext = GetExtension(path);
			if(ext != null && ext.Length > 0) pathWithoutExtension = path.RemoveSuffix(ext.Length);
			else pathWithoutExtension = path;
			return ext;
		}

		/// <summary>
		/// Finds filename extension, like <c>".txt"</c>.
		/// Returns '.' character index, or -1 if there is no extension.
		/// </summary>
		/// <param name="path">Path or filename. Can be null.</param>
		/// <remarks>
		/// Returns -1 if <c>'.'</c> is before <c>'\\'</c> or <c>'/'</c>.
		/// </remarks>
		public static int FindExtension(string path)
		{
			if(path == null) return -1;
			int i;
			for(i = path.Length - 1; i >= 0; i--) {
				switch(path[i]) {
				case '.': return i;
				case '\\': case '/': /*case ':':*/ return -1;
				}
			}
			return i;
		}

		/// <summary>
		/// Removes filename part from path. By default also removes separator (<c>'\\'</c> or <c>'/'</c>) if it is not after drive name (eg <c>"C:"</c>).
		/// Returns "" if the string is a filename.
		/// Returns null if the string is null or a root (like <c>@"C:\"</c> or <c>"C:"</c> or <c>@"\\server\share"</c> or <c>"http:"</c>).
		/// </summary>
		/// <param name="path">Path or filename. Can be null.</param>
		/// <param name="withSeparator">Don't remove separator character(s) (<c>'\\'</c> or <c>'/'</c>). See examples.</param>
		/// <remarks>
		/// Similar to <see cref="Path.GetDirectoryName"/>. Some differences: does not throw exceptions; skips <c>'\\'</c> or <c>'/'</c> at the end (eg from <c>@"C:\A\B\"</c> gets <c>@"C:\A"</c>, not <c>@"C:\A\B"</c>); does not expand DOS path; much faster.
		/// 
		/// Parses raw string. You may want to <see cref="Normalize"/> it at first.
		/// 
		/// Supports separators <c>'\\'</c> and <c>'/'</c>.
		/// Also supports URL and shell parsing names like <c>@"::{CLSID-1}\0\::{CLSID-2}"</c>.
		/// 
		/// Examples:
		/// 
		/// | path | result
		/// | - | -
		/// | <c>@"C:\A\B\file.txt"</c> | <c>@"C:\A\B"</c>
		/// | <c>"file.txt"</c> | <c>""</c>
		/// | <c>@"C:\A\B\"</c> | <c>@"C:\A"</c>
		/// | <c>@"C:\A\/B\/"</c> | <c>@"C:\A"</c>
		/// | <c>@"C:\"</c> | <c>null</c>
		/// | <c>@"\\network\share"</c> | <c>null</c>
		/// | <c>"http:"</c> | <c>null</c>
		/// | <c>@"C:\aa\file.txt:alt.stream"</c> | <c>"C:\aa"</c>
		/// | <c>"http://a.b.c"</c> | <c>"http:"</c>
		/// | <c>"::{A}\::{B}"</c> | <c>"::{A}"</c>
		/// | <c>""</c> | <c>""</c>
		/// | <c>null</c> | <c>null</c>
		/// 
		/// Examples when <i>withSeparator</i> true:
		/// 
		/// | path | result
		/// | - | -
		/// | <c>@"C:\A\B"</c> | <c>@"C:\A\"</c> (not <c>@"C:\A"</c>)
		/// | <c>"http://x.y"</c> | <c>"http://"</c> (not <c>"http:"</c>)
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
		/// Returns true if s is like <c>".ext"</c> and the ext part does not contain characters <c>.\\/:</c>.
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
			return s != null && s.Ends(':') && GetUrlProtocolLength(s) == s.Length;
		}

		/// <summary>
		/// Gets path with unique filename for a new file or directory. 
		/// If the specified path is of an existing file or directory, returns path where the filename part is modified like "file 2.txt", "file 3.txt" etc. Else returns unchanged path.
		/// </summary>
		/// <param name="path">Suggested full path.</param>
		/// <param name="isDirectory">The path is for a directory. The number is always appended at the very end, not before .extension.</param>
		public static string MakeUnique(string path, bool isDirectory)
		{
			if(!AFile.ExistsAsAny(path)) return path;
			string ext = isDirectory ? null : GetExtension(path, out path);
			for(int i = 2; ; i++) {
				var s = path + " " + i + ext;
				if(!AFile.ExistsAsAny(s)) return s;
			}
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// flags for <see cref="APath.Normalize"/>.
	/// </summary>
	[Flags]
	public enum PNFlags
	{
		/// <summary>Don't call API <msdn>GetLongPathName</msdn>.</summary>
		DontExpandDosPath = 1,

		/// <summary>Don't call <see cref="APath.PrefixLongPathIfNeed"/>.</summary>
		DontPrefixLongPath = 2,

		/// <summary>Don't remove <c>\</c> character at the end.</summary>
		DontRemoveEndSeparator = 4,

		/// <summary>If path is not a file-system path but looks like URL (eg <c>"http:..."</c> or <c>"file:..."</c>) or starts with <c>"::"</c>, don't throw exception and don't process more (only expand environment variables).</summary>
		CanBeUrlOrShell = 8,
	}
}
