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
using System.Globalization;

using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Adds extension methods to System.String.
	/// Also adds StringComparison.Ordinal[IgnoreCase] versions of .NET String methods that use StringComparison.CurrentCulture by default. See https://msdn.microsoft.com/en-us/library/ms973919.aspx
	/// Extension method names have suffix _.
	/// Most of these extension methods throw NullReferenceException if called for a string variable that is null.
	/// </summary>
	//[DebuggerStepThrough]
	public static partial class String_
	{
		/// <summary>
		/// Returns Equals(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static bool Equals_(this string t, string value, bool ignoreCase = false)
		{
			return t.Equals(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls Equals_(strings[i], ignoreCase) for each string specified in the argument list until it returns true.
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
		/// Calls EndsWith_(strings[i], ignoreCase) for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int EndsWith_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.EndsWith_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if ends with the specified character.
		/// Fast, case-sensitive.
		/// </summary>
		public static bool EndsWith_(this string t, char value)
		{
			int i = t.Length - 1;
			return i >= 0 && t[i] == value;
		}

		/// <summary>
		/// Returns StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal).
		/// </summary>
		public static bool StartsWith_(this string t, string value, bool ignoreCase = false)
		{
			return t.StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls StartsWith_(strings[i], ignoreCase) for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int StartsWith_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.StartsWith_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if starts with the specified character.
		/// Fast, case-sensitive.
		/// </summary>
		public static bool StartsWith_(this string t, char value)
		{
			return t.Length > 0 && t[0] == value;
		}

		/// <summary>
		/// Compares part of this string with another string and returns true if matches.
		/// Calls <see cref="string.Compare(string, int, string, int, int, StringComparison)"/>.
		/// </summary>
		public static bool EqualsPart_(this string t, int startIndex, string value, bool ignoreCase = false)
		{
			return 0 == string.Compare(t, startIndex, value, 0, value.Length, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls EqualsPart_(startIndex, strings[i], ignoreCase) for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int EqualsPart_(this string t, int startIndex, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.EqualsPart_(startIndex, strings[i], ignoreCase)) return i + 1;
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
		static readonly string[] _newlines = { "\r\n", "\n", "\r" }; //error if const. Not public because elements can be changed.

		/// <summary>
		/// Returns the number of lines.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="preferMore">If the number of lines is ambiguous, return the bigger. It is ambiguous in these cases: 1. Length is 0. 2. Ends with '\n'.</param>
		public static int LineCount_(this string t, bool preferMore = false)
		{
			if(Empty(t)) return preferMore ? 1 : 0;
			int i = 0, n = 1;
			for(; i < t.Length; i++) {
				if(t[i] == '\n') n++;
			}
			if(!preferMore && t[i - 1] == '\n') n--;
			return n;
		}

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

		/// <summary>
		/// Converts part of string to int.
		/// Returns the int value, or 0 if fails to convert.
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// Unlike int.Parse and Convert.ToInt32:
		///		The number in string can be followed by more text, like "123text".
		///		Has startIndex parameter that allows to get number from middle, like "text123text".
		///		Gets the end of the number part.
		///		No exception when cannot convert.
		///		Supports hexadecimal format, like "0x1E" or "0x1e".
		///		Much faster.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends. If fails to convert, receives 0.</param>
		/// <remarks>
		/// The number can begin with ASCII spaces, tabs or newlines, like " 5".
		/// The number can be with "-" or "+", like "-5", but not like "- 5".
		/// Fails if the number is greater than +- uint.MaxValue (0xffffffff).
		/// The return value becomes negative if the number is greater than int.MaxValue, for example "0xffffffff" is -1, but it becomes correct if assigned to uint (need cast).
		/// Does not support non-integer numbers; for example, for "3.5E4" returns 3 and sets numberEndIndex=1.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 or greater than string length.</exception>
		public static int ToInt32_(this string t, int startIndex, out int numberEndIndex)
		{
			return (int)_ToInt(t, startIndex, out numberEndIndex, false);
		}

		/// <summary>
		/// This overload does not have parameter numberEndIndex.
		/// </summary>
		public static int ToInt32_(this string t, int startIndex = 0)
		{
			return (int)_ToInt(t, startIndex, out var i, false);
		}

		/// <summary>
		/// This overload does not have parameters.
		/// </summary>
		public static int ToInt32_(this string t)
		{
			return (int)_ToInt(t, 0, out var i, false);
		}

		/// <summary>
		/// Converts part of string to long.
		/// Returns the long value, or 0 if fails to convert.
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// Unlike long.Parse and Convert.ToInt64:
		///		The number in string can be followed by more text, like "123text".
		///		Has startIndex parameter that allows to get number from middle, like "text123text".
		///		Gets the end of the number part.
		///		No exception when cannot convert.
		///		Supports hexadecimal format, like "0x1E" or "0x1e".
		///		Much faster.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="startIndex">Offset in string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends. If fails to convert, receives 0.</param>
		/// <remarks>
		/// The number can begin with ASCII spaces, tabs or newlines, like " 5".
		/// The number can be with "-" or "+", like "-5", but not like "- 5".
		/// Fails if the number is greater than +- ulong.MaxValue (0xffffffffffffffff).
		/// The return value becomes negative if the number is greater than long.MaxValue, for example "0xffffffffffffffff" is -1, but it becomes correct if assigned to ulong (need cast).
		/// Does not support non-integer numbers; for example, for "3.5E4" returns 3 and sets numberEndIndex=1.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 or greater than string length.</exception>
		public static long ToInt64_(this string t, int startIndex, out int numberEndIndex)
		{
			return _ToInt(t, startIndex, out numberEndIndex, true);
		}

		/// <summary>
		/// This overload does not have parameter numberEndIndex.
		/// </summary>
		public static long ToInt64_(this string t, int startIndex = 0)
		{
			return _ToInt(t, startIndex, out var i, true);
		}

		/// <summary>
		/// This overload does not have parameters.
		/// </summary>
		public static long ToInt64_(this string t)
		{
			return _ToInt(t, 0, out var i, true);
		}

		static long _ToInt(string t, int startIndex, out int numberEndIndex, bool toLong)
		{
			numberEndIndex = 0;
			int len = t == null ? 0 : t.Length;
			if(startIndex < 0 || startIndex > len) throw new ArgumentOutOfRangeException("startIndex");
			int i = startIndex; char c = default(char);

			//skip spaces
			for(; ; i++) {
				if(i == len) return 0;
				c = t[i];
				if(c > ' ') break;
				if(c == ' ') continue;
				if(c < '\t' || c > '\r') break; //\t \n \v \f \r
			}

			//skip -+
			bool minus = false;
			if(c == '-' || c == '+') {
				if(++i == len) return 0;
				if(c == '-') minus = true;
				c = t[i];
			}

			long R = 0; //result

			//is hex?
			bool isHex = false;
			if(c == '0' && i <= len - 3) {
				c = t[++i];
				if(c == 'x' || c == 'X') { i++; isHex = true; } else i--;
			}

			//skip '0'
			int i0 = i;
			while(i < len && t[i] == '0') i++;

			int nDigits = 0, nMaxDigits;
			if(isHex) {
				nMaxDigits = toLong ? 16 : 8;
				for(; i < len; i++) {
					int k = _CharHexToDec(t[i]); if(k < 0) break;
					if(++nDigits > nMaxDigits) return 0;
					R = (R << 4) + k;
				}
				if(i == i0) i--; //0xNotHex (decimal 0)

			} else { //decimal or not a number
				nMaxDigits = toLong ? 20 : 10;
				for(; i < len; i++) {
					int k = t[i] - '0'; if(k < 0 || k > 9) break;
					R = R * 10 + k;
					//is too long?
					if(++nDigits >= nMaxDigits) {
						if(nDigits > nMaxDigits) return 0;
						if(toLong) {
							if(string.CompareOrdinal(t, i + 1 - nDigits, "18446744073709551615", 0, nDigits) > 0) return 0;
						} else {
							if(R > uint.MaxValue) return 0;
						}
					}
				}
				if(i == i0) return 0; //not a number
			}

			if(minus) R = -R;
			numberEndIndex = i;
			return R;
		}

		static int _CharHexToDec(char c)
		{
			if(c >= '0' && c <= '9') return c - '0';
			if(c >= 'A' && c <= 'F') return c - ('A' - 10);
			if(c >= 'a' && c <= 'f') return c - ('a' - 10);
			return -1;
		}

		/// <summary>
		/// Converts string to int (calls <see cref="ToInt32_(string, int, out int)"/>) and gets string part that follows the number.
		/// For example, for string "123text" sets the tail variable = "text" and returns 123.
		/// For string like "123" sets tail = "".
		/// If fails to convert, sets tail = null.
		/// Skips 1 ASCII space or tab character after the number part. For example, for string "123 text" sets tail = "text", not " text".
		/// Everything else is the same as with ToInt32_.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="tail">A string variable. Can be this variable.</param>
		public static int ToIntAndString_(this string t, out string tail)
		{
			int eon, R = ToInt32_(t, 0, out eon);

			if(eon == 0)
				tail = null;
			else {
				if(eon < t.Length) { char c = t[eon]; if(c == ' ' || c == '\t') eon++; }
				if(eon < t.Length) tail = t.Substring(eon); else tail = "";
			}

			return R;
		}

		/// <summary>
		/// Converts string to double.
		/// Calls <see cref="double.Parse(string, NumberStyles, IFormatProvider)"/> with CultureInfo.InvariantCulture and NumberStyles.Float|NumberStyles.AllowThousands.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="canThrow">If true, exception if the string is not a valid number or is null. If false, then returns 0.</param>
		public static double ToDouble_(this string t, bool canThrow = false)
		{
			if(canThrow) return double.Parse(t, CultureInfo.InvariantCulture);
			return double.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double R) ? R : 0.0;
		}

		/// <summary>
		/// Converts string to float.
		/// Calls <see cref="float.Parse(string, NumberStyles, IFormatProvider)"/> with CultureInfo.InvariantCulture and NumberStyles.Float|NumberStyles.AllowThousands.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="canThrow">If true, exception if the string is not a valid number or is null. If false, then returns 0.</param>
		public static float ToFloat_(this string t, bool canThrow = false)
		{
			if(canThrow) return float.Parse(t, CultureInfo.InvariantCulture);
			return float.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float R) ? R : 0.0F;
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
		/// Returns true if this string matches regular expression pattern.
		/// Calls Regex.IsMatch(this, pattern, options|RegexOptions.CultureInvariant).
		/// </summary>
		public static bool RegexIs_(this string t, string pattern, RegexOptions options = 0)
		{
			return Regex.IsMatch(t, pattern, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls RegexIs_(patterns[i], options) for each regular expression pattern specified in the argument list until it returns true.
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

		/// <summary>
		/// Calls result = Regex.Replace(t, pattern, replacement, options|RegexOptions.CultureInvariant).
		/// Returns the number of replacements made.
		/// </summary>
		public static int RegexReplace_(this string t, out string result, string pattern, string replacement, RegexOptions options = 0)
		{
			int n = 0;
			result = Regex.Replace(t, pattern, me => { n++; return me.Result(replacement); }, options | RegexOptions.CultureInvariant);
			return n;
		}

		/// <summary>
		/// This overload has parameter 'count' (max number of replacements).
		/// Returns the number of replacements made.
		/// </summary>
		public static int RegexReplace_(this string t, out string result, string pattern, string replacement, int count, RegexOptions options = 0)
		{
			var x = new Regex(pattern, options | RegexOptions.CultureInvariant);
			int n = 0;
			result = x.Replace(t, (m) => { n++; return m.Result(replacement); }, count);
			return n;
		}

		/// <summary>
		/// Returns a new string in which a specified string replaces a spcified count of characters at a specified position in this instance.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">startIndex or startIndex+count is outside of this string bounds.</exception>
		public static string ReplaceAt_(this string t, int startIndex, int count, string value)
		{
			return t.Remove(startIndex, count).Insert(startIndex, value);

			//almost the same speed, 1 more allocations
			//return t.Substring(0, startIndex) + value + t.Substring(startIndex + count);

			//2.5 times slower
			//var s = new StringBuilder(t);
			//s.Remove(startIndex, count);
			//s.Insert(startIndex, value);
			//return s.ToString();
		}
	}

}
