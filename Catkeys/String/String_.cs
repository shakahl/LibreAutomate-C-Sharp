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

using Catkeys.Types;
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
	public static unsafe partial class String_
	{
		/// <summary>
		/// Calls <see cref="String.Equals(string, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static bool Equals_(this string t, string value, bool ignoreCase = false)
		{
			return ignoreCase ? _EqualsI2(t, value) : t.Equals(value);
		}

		static bool _EqualsI2(string t, string s)
		{
			if(t == null) throw new NullReferenceException();
			if(s == null || t.Length != s.Length) return false;
			fixed (char* a = t, b = s) return _EqualsI(a, b, t.Length);
			//fixed (char* a = t, b = s) return 0 != Cpp.Cpp_StringEqualsI(a, b, t.Length); //faster with long strings, but much slower with short
		}

		static bool _EqualsI(char* a, char* b, int len)
		{
			if(len != 0) {
				//optimization: at first compare case-sensitive, as much as possible.
				//	never mind: in 32-bit process this is not the fastest code (too few registers). But makes much faster anyway.
				//	tested: strings don't have to be aligned at 4 or 8.
				while(len >= 12) {
					if(*(long*)a != *(long*)b) break;
					if(*(long*)(a + 4) != *(long*)(b + 4)) break;
					if(*(long*)(a + 8) != *(long*)(b + 8)) break;
					a += 12; b += 12; len -= 12;
				}

				var table = Util.LibTables.LowerCase;

				for(int i = 0; i < len; i++) {
					int c1 = a[i], c2 = b[i];
					if(c1 != c2 && table[c1] != table[c2]) goto gFalse;
				}
			}
			//throw new Exception(); //used to debug, because breakpoints somehow don't work here
			return true;
			gFalse: return false;

			//TODO: use this in all functions, especially where now uses String.Compare, it is slow.
		}

		/// <summary>
		/// Calls <see cref="Equals_(string, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int Equals_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.Equals_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Calls <see cref="String.EndsWith(string, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static bool EndsWith_(this string t, string value, bool ignoreCase = false)
		{
			return t.EndsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls <see cref="EndsWith_(string, string, bool)"/> for each string specified in the argument list until it returns true.
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
		/// Calls <see cref="String.StartsWith(string, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static bool StartsWith_(this string t, string value, bool ignoreCase = false)
		{
			return t.StartsWith(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls <see cref="StartsWith_(string, string, bool)"/> for each string specified in the argument list until it returns true.
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
		/// Calls <see cref="string.Compare(string, int, string, int, int, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static bool EqualsAt_(this string t, int startIndex, string value, bool ignoreCase = false)
		{
			return 0 == string.Compare(t, startIndex, value, 0, value.Length, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			//tested: with Ordinal 60% slower than Equals. Slightly faster than CompareOrdinal.
		}

		/// <summary>
		/// Calls <see cref="EqualsAt_(string, int, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int EqualsAt_(this string t, int startIndex, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.EqualsAt_(startIndex, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Calls <see cref="String.IndexOf(string, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static int IndexOf_(this string t, string value, bool ignoreCase = false)
		{
			return t.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Calls <see cref="String.IndexOf(string, int, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static int IndexOf_(this string t, string value, int startIndex, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Calls <see cref="String.IndexOf(string, int, int, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static int IndexOf_(this string t, string value, int startIndex, int count, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Returns <c>IndexOfAny(anyOf.ToCharArray())</c>.
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf)
		{
			return t.IndexOfAny(anyOf.ToCharArray());
		}
		/// <summary>
		/// Returns <c>IndexOfAny(anyOf.ToCharArray(), startIndex)</c>.
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf, int startIndex)
		{
			return t.IndexOfAny(anyOf.ToCharArray(), startIndex);
		}
		/// <summary>
		/// Returns <c>IndexOfAny(anyOf.ToCharArray(), startIndex, count)</c>.
		/// </summary>
		public static int IndexOfAny_(this string t, string anyOf, int startIndex, int count)
		{
			return t.IndexOfAny(anyOf.ToCharArray(), startIndex, count);
			//FUTURE: create own code. Now ToCharArray is slow and generates garbage.
		}

		/// <summary>
		/// Calls <see cref="String.LastIndexOf(string, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static int LastIndexOf_(this string t, string value, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Calls <see cref="String.LastIndexOf(string, int, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static int LastIndexOf_(this string t, string value, int startIndex, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		/// <summary>
		/// Calls <see cref="String.LastIndexOf(string, int, int, StringComparison)"/> with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase.
		/// </summary>
		public static int LastIndexOf_(this string t, string value, int startIndex, int count, bool ignoreCase = false)
		{
			return t.LastIndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		//rejected: rarely used. If need, can use "any".ToCharArray().
		///// <summary>
		///// Returns LastIndexOfAny(anyOf.ToCharArray()).
		///// </summary>
		//public static int LastIndexOfAny_(this string t, string anyOf)
		//{
		//	return t.LastIndexOfAny(anyOf.ToCharArray());
		//}
		///// <summary>
		///// Returns LastIndexOfAny(anyOf.ToCharArray(), startIndex).
		///// </summary>
		//public static int LastIndexOfAny_(this string t, string anyOf, int startIndex)
		//{
		//	return t.LastIndexOfAny(anyOf.ToCharArray(), startIndex);
		//}
		///// <summary>
		///// Returns LastIndexOfAny(anyOf.ToCharArray(), startIndex, count).
		///// </summary>
		//public static int LastIndexOfAny_(this string t, string anyOf, int startIndex, int count)
		//{
		//	return t.LastIndexOfAny(anyOf.ToCharArray(), startIndex, count);
		//}

		/// <summary>
		/// Returns <see cref="String.Length"/>. If this is null, returns 0.
		/// </summary>
		public static int Length_(this string t) => t?.Length ?? 0;

		/// <summary>
		/// Splits this string into substrings using the specified separators.
		/// Returns string[].
		/// </summary>
		/// <param name="t"></param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="maxCount">The maximum number of substrings to get. If negative, gets all.</param>
		/// <param name="flags"></param>
		/// <seealso cref="Segments_(string, string, SegFlags)"/>
		/// <seealso cref="SplitLines_"/>
		public static string[] Split_(this string t, string separators, int maxCount, SegFlags flags = 0)
		{
			var x = new SegParser(t, separators, flags);
			return x.ToStringArray(maxCount);
		}

		/// <summary>
		/// Splits this string into substrings using the specified separators.
		/// Returns string[].
		/// </summary>
		/// <param name="t"></param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="flags"></param>
		/// <seealso cref="Segments_(string, string, SegFlags)"/>
		/// <seealso cref="SplitLines_"/>
		public static string[] Split_(this string t, string separators, SegFlags flags = 0)
		{
			var x = new SegParser(t, separators, flags);
			return x.ToStringArray();
		}

		/// <summary>
		/// Splits this string into lines using separators "\r\n", "\n", "\r".
		/// Returns string[].
		/// </summary>
		/// <param name="t"></param>
		/// <param name="noEmptyLines">Don't get empty lines.</param>
		/// <remarks>
		/// Calls <see cref="Split_(string, string, int, SegFlags)"/> with separators = Separators.Line, maxCount = -1 (all lines), flags = noEmptyLines ? SegFlags.NoEmpty : 0.
		/// </remarks>
		/// <seealso cref="Segments_(string, string, SegFlags)"/>
		public static string[] SplitLines_(this string t, bool noEmptyLines = false)
		{
			return Split_(t, Separators.Line, noEmptyLines ? SegFlags.NoEmpty : 0);
		}

		/// <summary>
		/// Returns the number of lines.
		/// Counts line separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t"></param>
		/// <param name="preferMore">Add 1 if the string ends with a line separator or its length is 0.</param>
		public static int CountLines_(this string t, bool preferMore = false)
		{
			if(Empty(t)) return preferMore ? 1 : 0;
			int i = 0, n = 1;
			for(; i < t.Length; i++) {
				char c = t[i];
				if(c > '\r') continue;
				if(c == '\r') {
					if(++i == t.Length || t[i] != '\n') i--; //single \r ?
					n++;
				} else if(c == '\n') n++;
			}
			if(!preferMore) switch(t[i - 1]) { case '\n': case '\r': n--; break; }
			return n;
		}

		/// <summary>
		/// Calls <see cref="String.ToLowerInvariant"/>.
		/// </summary>
		public static string ToLower_(this string t)
		{
			return t.ToLowerInvariant();
		}

		/// <summary>
		/// Calls <see cref="String.ToUpperInvariant"/>.
		/// </summary>
		public static string ToUpper_(this string t)
		{
			return t.ToUpperInvariant();
		}

		/// <summary>
		/// Returns <c>Trim(trimChars.ToCharArray())</c>.
		/// </summary>
		public static string Trim_(this string t, string trimChars)
		{
			return t.Trim(trimChars.ToCharArray());
		}

		/// <summary>
		/// Returns <c>TrimEnd(trimChars.ToCharArray())</c>.
		/// </summary>
		public static string TrimEnd_(this string t, string trimChars)
		{
			return t.TrimEnd(trimChars.ToCharArray());
		}

		/// <summary>
		/// Returns <c>TrimStart(trimChars.ToCharArray())</c>.
		/// </summary>
		public static string TrimStart_(this string t, string trimChars)
		{
			return t.TrimStart(trimChars.ToCharArray());
			//FUTURE: create own code. Now ToCharArray is slow and generates garbage.
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
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
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
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
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
			int i = startIndex; char c = default;

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
		/// If this string contains a number at startIndex, gets that number as int, also gets the string part that follows it, and returns true.
		/// For example, for string "25text" or "25 text" gets num = 25, tail = "text".
		/// Everything else is the same as with <see cref="ToInt32_(string, int, out int)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="num">Receives the number. Receives 0 if no number.</param>
		/// <param name="tail">Receives the string part that follows the number, or "". Receives null if no number. Can be this variable.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		public static bool ToIntAndString_(this string t, out int num, out string tail, int startIndex = 0)
		{
			num = ToInt32_(t, startIndex, out int end);
			if(end == 0) {
				tail = null;
				return false;
			}
			if(end < t.Length && t[end] == ' ') end++;
			tail = t.Substring(end);
			return true;
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
		/// Calls <see cref="Regex.IsMatch(string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// </summary>
		public static bool RegexIs_(this string t, string pattern, RegexOptions options = 0)
		{
			return Regex.IsMatch(t, pattern, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls <see cref="RegexIs_(string, string, RegexOptions)"/> for each regular expression pattern specified in the argument list until it returns true.
		/// Returns 1-based index of matching pattern, or 0 if none.
		/// </summary>
		public static int RegexIs_(this string t, RegexOptions options, params string[] patterns)
		{
			for(int i = 0; i < patterns.Length; i++) if(t.RegexIs_(patterns[i], options)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Calls <see cref="Regex.Match(string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// </summary>
		public static Match RegexMatch_(this string t, string pattern, RegexOptions options = 0)
		{
			return Regex.Match(t, pattern, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls <see cref="Regex.Matches(string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// </summary>
		public static MatchCollection RegexMatches_(this string t, string pattern, RegexOptions options = 0)
		{
			return Regex.Matches(t, pattern, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls <see cref="Regex.Match(string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// Returns the match position in this string, or -1. If group is not 0, it is submatch position.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="pattern"></param>
		/// <param name="group">0 or a group number. For example, if pattern is "one(two)" and group is 1, returns "two" position.</param>
		/// <param name="options"></param>
		public static int RegexIndexOf_(this string t, string pattern, int group = 0, RegexOptions options = 0)
		{
			return RegexIndexOf_(t, pattern, out int unused, group, options);
		}

		/// <summary>
		/// Calls <see cref="Regex.Match(string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant. Also gets match length.
		/// Returns the match position in this string, or -1. If group is not 0 - submatch position.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="pattern"></param>
		/// <param name="length">Receives match length. If group is not 0 - submatch length.</param>
		/// <param name="group">0 or a group number. For example, if pattern is "one(two)" and group is 1, gets "two" length and returns "two" position.</param>
		/// <param name="options"></param>
		public static int RegexIndexOf_(this string t, string pattern, out int length, int group = 0, RegexOptions options = 0)
		{
			length = 0;
			var m = Regex.Match(t, pattern, options | RegexOptions.CultureInvariant);
			if(!m.Success) return -1;
			if(group == 0) { length = m.Length; return m.Index; }
			var g = m.Groups[group];
			length = g.Length;
			return g.Index;
		}

		/// <summary>
		/// Calls <see cref="Regex.Match(string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant. Also gets the match string.
		/// Returns the match position in this string, or -1. If group is not 0 - submatch position.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="pattern"></param>
		/// <param name="match">Receives the match string. If group is not 0 - submatch.</param>
		/// <param name="group">0 or a group number. For example, if pattern is "one(two)" and group is 1, gets "two" and returns "two" position.</param>
		/// <param name="options"></param>
		public static int RegexIndexOf_(this string t, string pattern, out string match, int group = 0, RegexOptions options = 0)
		{
			match = null;
			int i = RegexIndexOf_(t, pattern, out int len, group, options);
			if(i >= 0) match = t.Substring(i, len);
			return i;
		}

		/// <summary>
		/// Calls <see cref="Regex.Replace(string, string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// </summary>
		public static string RegexReplace_(this string t, string pattern, string replacement, RegexOptions options = 0)
		{
			return Regex.Replace(t, pattern, replacement, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls <see cref="Regex.Replace(string, string, MatchEvaluator, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// </summary>
		public static string RegexReplace_(this string t, string pattern, MatchEvaluator evaluator, RegexOptions options = 0)
		{
			return Regex.Replace(t, pattern, evaluator, options | RegexOptions.CultureInvariant);
		}

		/// <summary>
		/// Calls <see cref="Regex.Replace(string, string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// Gets the result string and returns the number of replacements made.
		/// </summary>
		public static int RegexReplace_(this string t, out string result, string pattern, string replacement, RegexOptions options = 0)
		{
			int n = 0;
			result = Regex.Replace(t, pattern, m => { n++; return m.Result(replacement); }, options | RegexOptions.CultureInvariant);
			return n;
		}

		/// <summary>
		/// Calls <see cref="Regex.Replace(string, string, string, RegexOptions)"/> and adds RegexOptions.CultureInvariant.
		/// Gets the result string and returns the number of replacements made.
		/// This overload has parameter 'count' (max number of replacements).
		/// </summary>
		public static int RegexReplace_(this string t, out string result, string pattern, string replacement, int count, RegexOptions options = 0)
		{
			var x = new Regex(pattern, options | RegexOptions.CultureInvariant);
			int n = 0;
			result = x.Replace(t, m => { n++; return m.Result(replacement); }, count);
			return n;
		}

		/// <summary>
		/// Returns a new string in which a specified string replaces a specified count of characters at a specified position in this instance.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">startIndex or startIndex+count is outside of this string bounds.</exception>
		public static string ReplaceAt_(this string t, int startIndex, int count, string value)
		{
			return t.Remove(startIndex, count).Insert(startIndex, value);

			//slightly slower, 1 more allocations
			//return t.Substring(0, startIndex) + value + t.Substring(startIndex + count);

			//maybe less garbage (didn't measure), but slightly slower
			//var s = Util.LibStringBuilderCache.Acquire();
			//if(startIndex != 0) s.Append(t, 0, startIndex);
			//s.Append(value);
			//int i = startIndex + count, n = t.Length - i;
			//if(n != 0) s.Append(t, i, n);
			//return s.ToStringCached_();
		}

		/// <summary>
		/// Replaces some characters to C# escape sequences.
		/// Replaces these characters: '\\', '\"', '\t', '\n', '\r' and all in range 0-31.
		/// If the string contains these characters, replaces and returns new string. Else returns this string.
		/// </summary>
		public static string Escape_(this string t)
		{
			if(t.Length == 0) return t;
			int i;
			for(i = 0; i < t.Length; i++) {
				var c = t[i];
				if(c < ' ' || c == '\\' || c == '\"') break;
				//tested: Unicode line-break chars in most controls don't break lines, therefore don't need to escape
			}
			if(i == t.Length) return t;
			var s = Util.LibStringBuilderCache.Acquire(t.Length + t.Length / 16 + 100);
			for(i = 0; i < t.Length; i++) {
				var c = t[i];
				if(c < ' ') {
					switch(c) {
					case '\t': s.Append("\\t"); break;
					case '\n': s.Append("\\n"); break;
					case '\r': s.Append("\\r"); break;
					default: s.Append("\\u"); s.Append(((ushort)c).ToString("x4")); break;
					}
				} else if(c == '\\') s.Append("\\\\");
				else if(c == '\"') s.Append("\\\"");
				else s.Append(c);
			}
			return s.ToStringCached_();
		}
	}

}
