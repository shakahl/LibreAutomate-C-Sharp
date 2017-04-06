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
	public static class Path_
	{
		/// <summary>
		/// If path starts with '%' character, expands environment variables enclosed in %, else just returns path.
		/// </summary>
		/// <param name="path">Any string. Can be null.</param>
		public static string ExpandEnvVar(string path)
		{
			if(path == null || path.Length < 3 || path[0] != '%') return path;
			//return Environment.ExpandEnvironmentVariables(path); //very slow, even if there are no '%'

			int n = path.Length + 100, r;
			var b = new StringBuilder(n);
			for(;;) {
				r = _Api.ExpandEnvironmentStrings(path, b, n);
				//PrintList(n, r, b.Length, b);
				if(r <= n) break;
				b.EnsureCapacity(n = r);
			}
			var R = b.ToString();
			if(R == path) return R;
			return ExpandEnvVar(R); //can be %envVar2% in envVar1 value
		}

		internal static string LibGetEnvVar(string name)
		{
			int n = 200, r;
			var b = new StringBuilder(n);
			for(;;) {
				r = _Api.GetEnvironmentVariable(name, b, n);
				//PrintList(n, r, b.Length, b);
				if(r == 0) return "";
				if(r <= n) break;
				b.EnsureCapacity(n = r);
			}
			return b.ToString();
		}

		internal static bool LibEnvVarExists(string name)
		{
			return 0 != _Api.GetEnvironmentVariable(name, null, 0);
		}

		static class _Api
		{
			[DllImport("kernel32.dll", EntryPoint = "GetEnvironmentVariableW", SetLastError = true)]
			internal static extern int GetEnvironmentVariable(string lpName, [Out] StringBuilder lpBuffer, int nSize);

			[DllImport("kernel32.dll", EntryPoint = "ExpandEnvironmentStringsW")]
			internal static extern int ExpandEnvironmentStrings(string lpSrc, [Out] StringBuilder lpDst, int nSize);
		}

		/// <summary>
		/// Returns true if path matches one of these wildcard patterns:
		///		@"\\*" - network path or \\?\.
		///		@"?:\*" - local path. Here ? is A-Z, a-z.
		///		@"?:" - drive name. Here ? is A-Z, a-z.
		///		@"%*%*" - existing environment variable (usually contains full path).
		///		@"&lt;*&gt;*" - special string that can be used with some functions of this library.
		///		@":*" - shell item parsing name, for example "::{CLSID}".
		///	Supports '/' too.
		/// </summary>
		/// <param name="path">Any string. Can be null.</param>
		/// <param name="orURL">Also return true if path looks like a URL (any protocol), eg "http://abc" or "http:" or "shell:abc". Note: don't use "filename:stream" unless it is full path.</param>
		public static bool IsFullPath(string path, bool orURL = false)
		{
			int len = (path == null) ? 0 : path.Length;

			if(len >= 2) {
				char c = path[0];
				switch(c) {
				case '<': return path.IndexOf('>', 1) > 1;
				case '%':
					int i = path.IndexOf('%', 1);
					return i > 1 && LibEnvVarExists(path.Substring(1, i - 1)); //quite fast
				case '\\': return path[1] == '\\';
				case '/': return path[1] == '/';
				case ':': return true;
				}

				if((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')) {
					if(path[1] == ':') return len == 2 || path[2] == '\\' || path[2] == '/'; //info: returns false if eg "c:abc" which means "abc" in current directory of drive "c:"

					//is URL (any protocol)?
					if(IsURL(path)) return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Makes fully-qualified path.
		/// If path is not full path, returns <see cref="Folders.ThisApp"/> + path.
		/// Calls <see cref="Combine">Combine</see>, which expands environment variables, processes @"..\" etc.
		/// </summary>
		/// <param name="path">Full path or filename or relative path.</param>
		public static string MakeFullPath(string path)
		{
			return Combine(null, path);
		}

		/// <summary>
		/// Combines two paths and makes fully-qualified path.
		/// </summary>
		/// <remarks>
		///	Returns fully-qualified path, with processed @"..\" etc, not relative, not short (DOS), with replaced environment variables if starts with "%".
		///	If the return value would not be full path, prepends <see cref="Folders.ThisApp"/>.
		/// If s1 and s2 are null or "", returns "".
		///	No exceptions.
		/// Similar to Path.Combine (.NET function), but not the same.
		/// </remarks>
		public static string Combine(string s1, string s2)
		{
			string R = null;
			int len2 = (s2 == null) ? 0 : s2.TrimStart(_sep).Length;

			if(Empty(s1)) {
				if(len2 == 0) return "";
				R = s2.TrimEnd(_sep);
			} else {
				if(len2 == 0) R = s1.TrimEnd(_sep);
				else if(s1.EndsWith_("\\")) R = s1 + s2;
				else R = s1 + @"\" + s2;
			}

			R = ExpandEnvVar(R);

			if(!IsFullPath(R)) R = Folders.ThisApp + R;
			else if(R[0] == ':' || R[0] == '<' || R[0] == '%') return R;

			return LibNormalizeNoExpandEV(R);
		}

		static char[] _sep = new char[] { '\\', '/' };

		/// <summary>
		/// flags for <see cref="Normalize"/>.
		/// </summary>
		[Flags]
		public enum NormalizeFlags
		{
			/// <summary>Remove '\\' character at the end. Even if then will be "c:".</summary>
			TrimEndSeparator = 1,
			/// <summary>Don't call <see cref="PrefixLongPath"/> when longer than 259 characters.</summary>
			DoNotPrefix = 2,
			/// <summary>Don't call GetLongPathName when contains '~' character.</summary>
			DoNotExpandDosPath = 4,
		}

		/// <summary>
		/// Makes normal full path from path that can contain special substrings etc.
		/// At first, if path starts with '%' character, expands environment variables.
		/// Then, if path is not full path, combines it with defaultParentDirectory or throws exception.
		/// Then calls API <msdn>GetFullPathName</msdn>. It replaces '/' to '\\', replaces multiple '\\' to single (where need), processes @"\.." etc, trims spaces, adds '\' to "c:", etc.
		/// If contains '~' character and not used flag DoNotExpandDosPath, calls API <msdn>GetLongPathName</msdn>. It converts short DOS path to normal path, if possible, for example @"c:\progra~1" to @"c:\program files". It is the slowest part.
		/// If used flag TrimEndSeparator, removes '\\' character at the end.
		/// If longer than 259 characters and no flag DoNotPrefix, adds @"\\?\" prefix (see <see cref="PrefixLongPath"/>).
		/// </summary>
		/// <param name="path">Path of a file or directory, which can exist or not.</param>
		/// <param name="flags"></param>
		/// <param name="defaultParentDirectory">If path is not full path, combine it with defaultParentDirectory to make full path.</param>
		/// <exception cref="ArgumentException">path is not full path, and defaultParentDirectory is null or "".</exception>
		/// <remarks>
		/// Similar to <see cref="Path.GetFullPath"/>. Main differences: this function expands environment variables, does not support current directory, supports @"\\?\path longer than 260 characters", does not throw exceptions when [it thinks that] path is invalid.
		/// </remarks>
		public static string Normalize(string path, NormalizeFlags flags = 0, string defaultParentDirectory = null)
		{
			path = ExpandEnvVar(path);
			if(!IsFullPath(path)) {
				if(Empty(defaultParentDirectory)) throw new ArgumentException("Expected full path.");
				path = defaultParentDirectory + @"\" + path;
				if(!IsFullPath(path)) throw new ArgumentException("Expected full path.");
			}
			return LibNormalizeNoExpandEV(path, flags);
		}

		/// <summary>
		/// Same as <see cref="Normalize"/>, but skips 's = Path_.ExpandEnvVar(s)' and 'if(!Path_.IsFullPath(s)) ...'.
		/// s should be full path. If not full, combines with current directory.
		/// </summary>
		internal static unsafe string LibNormalizeNoExpandEV(string s, NormalizeFlags flags = 0)
		{
			//note: although slower, call GetFullPathName always, not just when contains @"..\" etc.
			//	Because it does many things (see Normalize doc), not all documented.
			//	We still ~2 times faster than Path.GetFullPath.
			int na = s.Length + 10;
			var b = new StringBuilder(na);
			int nr = (int)Api.GetFullPathName(s, (uint)na, b, null);
			if(nr > 0 && nr < na) s = b.ToString();

			if(0 == (flags & NormalizeFlags.DoNotExpandDosPath) && s.IndexOf('~') > 0) {
				na = Math.Max(s.Length + 100, 300);
				for(;;) {
					b.EnsureCapacity(na);
					nr = (int)Api.GetLongPathName(s, b, (uint)na);
					if(nr < 1) break;
					if(nr > na) { na = nr; continue; }
					s = b.ToString();
					break;
				}
			}

			if(0 != (flags & NormalizeFlags.TrimEndSeparator) && s.EndsWith_('\\')) s = s.Remove(s.Length - 1);
			if(0 == (flags & NormalizeFlags.DoNotPrefix)) s = PrefixLongPathIfNeed(s);

			return s;
		}

		/// <summary>
		/// Same as <see cref="Normalize"/>, but skips 'if(!Path_.IsFullPath(s)) ...'.
		/// s should be full path. If not full, combines with current directory.
		/// </summary>
		internal static unsafe string LibNormalizeExpandEV(string s, NormalizeFlags flags = 0)
		{
			return LibNormalizeNoExpandEV(ExpandEnvVar(s), flags);
		}

		/// <summary>
		/// Calls ExpandEnvVar. Throws if !IsFullPath(path). Calls PrefixLongPath if path.Length >= 260.
		/// </summary>
		/// <param name="path"></param>
		/// <exception cref="ArgumentException">Not full path.</exception>
		internal static string LibExpandEV_CheckFullPath_PrefixLong(string path)
		{
			path = ExpandEnvVar(path);
			if(!IsFullPath(path)) throw new ArgumentException("Expected full path.");
			path = PrefixLongPathIfNeed(path);
			return path;
		}

		/// <summary>
		/// If path is not null/"" and does not start with @"\\?\", prepends @"\\?\".
		/// If path is network path (like @"\\computer\folder\..."), makes like @"\\?\UNC\computer\folder\...".
		/// </summary>
		/// <param name="path"></param>
		/// <remarks>
		/// Windows API kernel functions support extended-length paths, ie longer than 259 characters. But the path must have this prefix. Windows API shell functions don't support it.
		/// </remarks>
		public static string PrefixLongPath(string path)
		{
			if(!Empty(path) && !path.StartsWith_(@"\\?\")) {
				if(path.StartsWith_("\\")) path = path.ReplaceAt_(0, 1, @"\\?\UNC");
				else path = @"\\?\" + path;
			}
			return path;
		}

		/// <summary>
		/// Calls <see cref="PrefixLongPath"/> if path is not null and its length is greater than 259 characters.
		/// </summary>
		/// <param name="path"></param>
		public static string PrefixLongPathIfNeed(string path)
		{
			if(path != null && path.Length >= 260) path = PrefixLongPath(path);
			return path;
		}

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
				return path.Length >= 3 && path.EndsWith_(':');
			}
		}
	}
}
