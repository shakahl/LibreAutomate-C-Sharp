using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
//using System.IO;
//using System.Windows.Forms;

//for LikeEx_
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	/// <summary>
	/// Adds extension methods to System.String.
	/// Also adds StringComparison.Ordinal[IgnoreCase] versions of .NET String methods that use StringComparison.CurrentCulture by default. See https://msdn.microsoft.com/en-us/library/ms973919.aspx
	/// Extension method names have suffix _.
	/// </summary>
	[DebuggerStepThrough]
	public static partial class String_
	{
		/// <summary>
		/// Returns Equals(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// The same as the .NET String.Equals, but provided for consistency and to make immediately clear that it uses ordinal, not current culture like part of .NET String methods.
		/// </summary>
		public static bool Equals_(this string t, string value, bool ignoreCase = false)
		{
			return t.Equals(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls Equals_() for each string specified in the argument list until one matches this string.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int Equals_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.Equals_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static bool EndsWith_(this string t, string value, bool ignoreCase = false)
		{
			return t.EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls EndsWith_() for each string specified in the argument list until one matches this string.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int EndsWith_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.EndsWith_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static bool StartsWith_(this string t, string value, bool ignoreCase = false)
		{
			return t.StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls StartsWith_() for each string specified in the argument list until one matches this string.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int StartsWith_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.StartsWith_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int IndexOf_(this string t, string value, bool ignoreCase = false)
		{
			return t.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns IndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int IndexOf_(this string t, string value, int startIndex, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns IndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int IndexOf_(this string t, string value, int startIndex, int count, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns IndexOfAny(anyOf.ToCharArray()).
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf)
		{
			return t.IndexOfAny(anyOf.ToCharArray());
		}
		/// <summary>
		/// Returns IndexOfAny(anyOf.ToCharArray(), startIndex).
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf, int startIndex)
		{
			return t.IndexOfAny(anyOf.ToCharArray(), startIndex);
		}
		/// <summary>
		/// Returns IndexOfAny(anyOf.ToCharArray(), startIndex, count).
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf, int startIndex, int count)
		{
			return t.IndexOfAny(anyOf.ToCharArray(), startIndex, count);
		}

		/// <summary>
		/// Returns LastIndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int LastIndexOf_(this string t, string value, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns LastIndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int LastIndexOf_(this string t, string value, int startIndex, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Returns LastIndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static int LastIndexOf_(this string t, string value, int startIndex, int count, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns LastIndexOfAny(anyOf.ToCharArray()).
		/// </summary>
		public static int LastIndexOfAny_(this string t, string anyOf)
		{
			return t.LastIndexOfAny(anyOf.ToCharArray());
		}
		/// <summary>
		/// Returns LastIndexOfAny(anyOf.ToCharArray(), startIndex).
		/// </summary>
		public static int LastIndexOfAny_(this string t, string anyOf, int startIndex)
		{
			return t.LastIndexOfAny(anyOf.ToCharArray(), startIndex);
		}
		/// <summary>
		/// Returns LastIndexOfAny(anyOf.ToCharArray(), startIndex, count).
		/// </summary>
		public static int LastIndexOfAny_(this string t, string anyOf, int startIndex, int count)
		{
			return t.LastIndexOfAny(anyOf.ToCharArray(), startIndex, count);
		}

		/// <summary>
		/// Returns Split(separators.ToCharArray()).
		/// </summary>
		public static string[] Split_(this string t, string separators)
		{
			return t.Split(separators.ToCharArray());
		}
		/// <summary>
		/// Returns Split(separators.ToCharArray(), count).
		/// </summary>
		public static string[] Split_(this string t, string separators, int count)
		{
			return t.Split(separators.ToCharArray(), count);
		}
		/// <summary>
		/// Returns Split(separators.ToCharArray(), options).
		/// </summary>
		public static string[] Split_(this string t, string separators, StringSplitOptions options)
		{
			return t.Split(separators.ToCharArray(), options);
		}
		/// <summary>
		/// Returns Split(separators.ToCharArray(), count, options).
		/// </summary>
		public static string[] Split_(this string t, string separators, int count, StringSplitOptions options)
		{
			return t.Split(separators.ToCharArray(), count, options);
		}

		/// <summary>
		/// Returns array of lines. Line separators can be \r\n or \n or \r.
		/// </summary>
		public static string[] SplitLines_(this string t, bool removeEmptyLines = false)
		{
			return t.Split(_newlines, removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
		}
		static readonly string[] _newlines = { "\r\n", "\n", "\r" }; //error if const; not public because elements can be changed

		/// <summary>
		/// Returns ToLowerInvariant().
		/// </summary>
		public static string ToLower_(this string t)
		{
			return t.ToLowerInvariant();
		}

		/// <summary>
		/// Returns ToUpperInvariant().
		/// </summary>
		public static string ToUpper_(this string t)
		{
			return t.ToUpperInvariant();
		}

		/// <summary>
		/// Returns Trim(trimChars.ToCharArray()).
		/// </summary>
		public static string Trim_(this string t, string trimChars)
		{
			return t.Trim(trimChars.ToCharArray());
		}
		//This is not in System.String.

		/// <summary>
		/// Returns TrimEnd(trimChars.ToCharArray()).
		/// </summary>
		public static string TrimEnd_(this string t, string trimChars)
		{
			return t.TrimEnd(trimChars.ToCharArray());
		}

		/// <summary>
		/// Returns TrimStart(trimChars.ToCharArray()).
		/// </summary>
		public static string TrimStart_(this string t, string trimChars)
		{
			return t.TrimStart(trimChars.ToCharArray());
		}

		static int _CharHexToDec(char c)
		{
			if(c >= '0' && c <= '9') return c - '0';
			if(c >= 'A' && c <= 'F') return c - ('A' - 10);
			if(c >= 'a' && c <= 'f') return c - ('a' - 10);
			return -1;
		}

		/// <summary>
		/// Converts part of string to int.
		/// Returns the int value, or 0 if fails to convert.
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// Unlike int.Parse and Convert.ToInt32:
		///		The number in string can be followed by more text, like "123text".
		///		Gets the end of the number part.
		///		This overload has startIndex parameter.
		///		No exceptions. If fails to convert, sets numberEndIndex = 0 and returns 0.
		///		Supports hexadecimal format, like "0x1E" or "0x1e".
		///		Ignores any culture info.
		///		7-9 times faster.
		/// </summary>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends. If fails to convert, receives 0.</param>
		/// <remarks>
		/// The number can begin with ASCII spaces, tabs or newlines, like " 5".
		/// The number can be with "-", like "-5", but not with "+" or "- ".
		/// For decimal numbers, fails if the value is less than int.MinValue or greater than int.MaxValue.
		/// For hexadecimal numbers, fails if the value is more than uint.MaxValue (0xffffffff).
		/// Does not support non-integer numbers; for example, for "3.5E4" returns 3 and sets length=1.
		/// This is the main overload. Other overloads just call it.
		/// </remarks>
		public static int ToInt_(this string t, int startIndex, out int numberEndIndex)
		{
			numberEndIndex = 0;
			if(t == null) return 0;
			int i, k, n = 0; bool minus = false;

			for(i = startIndex; i < t.Length; i++) if(t[i] != ' ' && t[i] != '\t' && t[i] != '\r' && t[i] != '\n') break; //skip spaces

			if(i < t.Length && t[i] == '-') { i++; minus = true; } //minus

			long R = 0; //result

			if(i <= t.Length - 3 && t[i] == '0' && t[i + 1] == 'x') { //hex
				for(i += 2; i < t.Length; i++) {
					k = _CharHexToDec(t[i]); if(k < 0) break;
					R = R * 16 + k;
					if(++n > 8) return 0; //too long for hex int?
				}
				if(n == 0) i--; //0xNotHex (decimal 0)
				else if(minus) R = -R;
			} else { //decimal or not a number
				for(; i < t.Length; i++) {
					k = t[i] - '0'; if(k < 0 || k > 9) break;
					R = R * 10 + k;
					if(++n > 10) return 0; //too long for decimal int?
				}
				if(n == 0) return 0; //not a number

				if(minus) { R = -R; if(R < int.MinValue) return 0; } else if(R > int.MaxValue) return 0; //too big for int?
			}

			numberEndIndex = i;
			return (int)R;
		}

		/// <summary>
		/// The same as the main overload, but without startIndex parameter.
		/// </summary>
		public static int ToInt_(this string t, out int numberEndIndex)
		{
			return ToInt_(t, 0, out numberEndIndex);
		}

		/// <summary>
		/// The same as the main overload, but without numberEndIndex parameter.
		/// </summary>
		public static int ToInt_(this string t, int startIndex)
		{
			int eon;
			return ToInt_(t, startIndex, out eon);
		}

		/// <summary>
		/// The same as the main overload, but without startIndex and numberEndIndex parameters.
		/// </summary>
		public static int ToInt_(this string t)
		{
			int eon;
			return ToInt_(t, 0, out eon);
		}

		/// <summary>
		/// Converts string to int and gets string part followed by the number.
		/// For example, for string "123text", sets the tail argument to "text" and returns 123.
		/// For only-number string like "123", sets tail to "".
		/// If fails to convert, sets tail to null.
		/// Skips 1 ASCII space or tab character after the number part. For example, for string "123 text" sets tail = "text", not " text".
		/// Everything else is the same as for the main overload.
		/// </summary>
		/// <param name="tail">A string variable. Can be this variable.</param>
		public static int ToInt_(this string t, out string tail)
		{
			int eon, R = ToInt_(t, out eon);

			if(eon == 0)
				tail = null;
			else {
				if(eon < t.Length) { char c = t[eon]; if(c == ' ' || c == '\t') eon++; }
				if(eon < t.Length) tail = t.Substring(eon); else tail = "";
			}

			return R;
		}

		/// <summary>
		/// If this string is longer than length, returns its substring 0 to length - 3 with appended "...".
		/// Else returns this string.
		/// </summary>
		public static string Limit_(this string t, int length)
		{
			int k = t.Length;
			if(k <= length || length < 4) return t;
			return t.Remove(length - 3) + "...";
		}

		/// <summary>
		/// Returns true if this string matches wildcard pattern, which can contain these special characters:
		///   * - 0 or more characters;
		///   ? - 1 character;
		///   ?* - 1 or more characters;
		///   ** - literal *;
		///   *? - literal ?.
		/// </summary>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="useCurrentCulture">Use current culture. If false, uses ordinal matching which does not depend on a culture.</param>
		/// <remarks>
		/// Both "" and null match pattern "". This variable can be null because this is an extension method.
		/// Uses class WildString, but does not support "[options]". You can use the class when comparing multiple strings with the same pattern, it will be faster.
		/// </remarks>
		public static bool Like_(this string t, string pattern, bool ignoreCase = false, bool useCurrentCulture = false)
		{
			var x = new WildString(pattern, WildStringType.Wildcard, ignoreCase, useCurrentCulture);
			return x.Match(t, true);
		}

		/// <summary>
		/// Calls Like_() (other overload) for each wildcard pattern specified in the argument list until one matches this string.
		/// Returns 1-based index of matching pattern, or 0 if none.
		/// </summary>
		public static int Like_(this string t, bool ignoreCase = false, params string[] patterns)
		{
			for(int i = 0; i < patterns.Length; i++) if(t.Like_(patterns[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Calls Microsoft.VisualBasic.CompilerServices.Operators.LikeString().
		/// More info: https://msdn.microsoft.com/en-us/library/swf8kaxw%28v=vs.100%29.aspx
		/// Wildcards: * (0 or more characters), ? (any 1 character), # (any digit), [chars], [!chars]. Escape sequences: [*], [?], [#], [[].
		/// Usually slower than Like_() (especially when ignoreCase is true).
		/// Throws exception if pattern is invalid (eg "text[text").
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)] //prevent compiling Operators.LikeString (slow first time) when actually not used
		public static bool LikeEx_(this string t, string pattern, bool ignoreCase = false)
		{
			return Operators.LikeString(t, pattern, ignoreCase ? CompareMethod.Text : CompareMethod.Binary);
		}
		//Speed for 1000 times (@"C:\Users\G\Documents\dictionary.xls", "*.xls"):
		//	This 85, regex 830, compiled regex 440, regex with WildcardToRegex 1200, QM2 FindRX 400, QM2 MatchW ~30 (matchw 70 + rep 35).
		//Not tested System.Management.Automation.WildcardPattern, because assembly System.Management.Automation is not installed. It seems need to install it through NuGet.
		//Exception if pattern contains unclosed [, for example "abc[5" (must be "abc[5]" or "abc[[]5"). Also if pattern is "?*" and string is empty (why?).
		//Like_() (which is used eg to compare window class names etc) does not support # (number) and [charset] that are supported by LikeEx_().
		//	Usually they are too limited to be useful. It's better to keep Like_ as simple as possible, and use Regex when need something more.
		//	For example, if we support #[, users that are unaware/forget of this would use window class name like "#32770", or with [ characters, and wonder why it does not work.

		/// <summary>
		/// Calls LikeEx_() for each wildcard pattern specified in the argument list until one matches this string.
		/// Returns 1-based index of matching pattern, or 0 if none.
		/// </summary>
		public static int LikeEx_(this string t, bool ignoreCase = false, params string[] patterns)
		{
			for(int i = 0; i < patterns.Length; i++) if(t.LikeEx_(patterns[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string matches regular expression pattern.
		/// Calls Regex.IsMatch(this, pattern, options|RegexOptions.CultureInvariant).
		/// </summary>
		public static bool RegexIs_(this string t, string pattern, RegexOptions options = 0)
		{
			return Regex.IsMatch(t, pattern, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls RegexIs_() for each regular expression pattern specified in the argument list until one matches this string.
		/// Returns 1-based index of matching pattern, or 0 if none.
		/// </summary>
		public static int RegexIs_(this string t, RegexOptions options, params string[] patterns)
		{
			for(int i = 0; i < patterns.Length; i++) if(t.RegexIs_(patterns[i], options)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Calls Regex.Match(this, pattern, options|RegexOptions.CultureInvariant).
		/// Returns its return value.
		/// </summary>
		public static Match RegexMatch_(this string t, string pattern, RegexOptions options = 0)
		{
			return Regex.Match(t, pattern, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls Regex.Matches(this, pattern, options|RegexOptions.CultureInvariant).
		/// Returns its return value.
		/// </summary>
		public static MatchCollection RegexMatches_(this string t, string pattern, RegexOptions options = 0)
		{
			return Regex.Matches(t, pattern, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls Regex.Replace(t, pattern, replacement, options|RegexOptions.CultureInvariant).
		/// Returns its return value.
		/// </summary>
		public static string RegexReplace_(this string t, string pattern, string replacement, RegexOptions options = 0)
		{
			return Regex.Replace(t, pattern, replacement, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls Regex.Replace(t, pattern, evaluator, options|RegexOptions.CultureInvariant).
		/// Returns its return value.
		/// </summary>
		public static string RegexReplace_(this string t, string pattern, MatchEvaluator evaluator, RegexOptions options = 0)
		{
			return Regex.Replace(t, pattern, evaluator, options | RegexOptions.CultureInvariant);
		}
	}

}
