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
using System.Globalization;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Adds extension methods to <see cref="String"/>.
	/// </summary>
	/// <remarks>
	/// Some .NET <see cref="String"/> methods use <see cref="StringComparison.CurrentCulture"/> by default, while others use ordinal or invariant comparison. It is confusing (difficult to remember), dangerous (easy to make bugs), slower and rarely useful.
	/// Microsoft recommends to specify <b>StringComparison.Ordinal[IgnoreCase]</b> explicitly. See https://msdn.microsoft.com/en-us/library/ms973919.aspx.
	/// This class adds their versions that use <i>StringComparison.Ordinal[IgnoreCase]</i>. Same or similar name, usually a shorter version.
	/// For example, <b>EndsWith</b> (of .NET) uses <b>StringComparison.CurrentCulture</b>, and <b>Ends</b> (of this class) uses <b>StringComparison.Ordinal</b>.
	/// 
	/// This class also adds more methods.
	/// The nested class <see cref="More"/> contains some less often used static methods.
	/// You also can find string manipulation methods in other classes of this library, for example <see cref="AConvert"/>, <see cref="Keyb.More"/>.
	/// </remarks>
	public static unsafe partial class ExtString //TODO: ExtString
	{
		static bool _Eq(char* a, char* b, int len, bool ignoreCase)
		{
			//never mind: in 32-bit process this is not the fastest code (too few registers).
			//tested: strings don't have to be aligned at 4 or 8.
			//ignoreCase optimization: at first compare case-sensitive, as much as possible.
			while(len >= 12) {
				if(*(long*)a != *(long*)b) break;
				if(*(long*)(a + 4) != *(long*)(b + 4)) break;
				if(*(long*)(a + 8) != *(long*)(b + 8)) break;
				a += 12; b += 12; len -= 12;
			}

			if(ignoreCase) {
				var table = Util.LibTables.LowerCase;

				for(int i = 0; i < len; i++) {
					int c1 = a[i], c2 = b[i];
					if(c1 != c2 && table[c1] != table[c2]) goto gFalse;
				}
			} else {
				for(int i = 0; i < len; i++) {
					if(a[i] != b[i]) goto gFalse;
				}
			}
			return true;
			gFalse: return false;
		}

		/// <summary>
		/// Compares this and other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <seealso cref="string.Compare"/>
		/// <seealso cref="string.CompareOrdinal"/>
		public static bool Eq(this string t, string s, bool ignoreCase = false)
		{
			int len = t.Length; //NullReferenceException
			if(s is null || len != s.Length) return false;
			if(ReferenceEquals(t, s)) return true;
			fixed (char* a = t, b = s) return _Eq(a, b, len, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="Eq(string, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int Eq(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.Eq(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Compares this and other string ignoring case. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <remarks>Calls <see cref="Eq(string, string, bool)"/> with <i>ignoreCase</i> true.</remarks>
		public static bool Eqi(this string t, string s) => Eq(t, s, true);

		/// <summary>
		/// Compares part of this string with other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i>.</exception>
		/// <seealso cref="string.Compare"/>
		/// <seealso cref="string.CompareOrdinal"/>
		public static bool EqAt(this string t, int startIndex, string s, bool ignoreCase = false)
		{
			if((uint)startIndex > t.Length) throw new ArgumentOutOfRangeException(); //and NullReferenceException
			int n = s?.Length ?? throw new ArgumentNullException();
			if(n > t.Length - startIndex) return false;
			fixed (char* a = t, b = s) return _Eq(a + startIndex, b, n, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="EqAt(string, int, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i>.</exception>
		public static int EqAt(this string t, int startIndex, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.EqAt(startIndex, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		//For StringSegment.
		internal static bool LibSubEq(string s1, int i1, string s2, int i2, int len, bool ignoreCase)
		{
			fixed (char* a = s1, b = s2) return _Eq(a + i1, b + i2, len, ignoreCase);
		}

		/// <summary>
		/// Compares the end of this string with other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		public static bool Ends(this string t, string s, bool ignoreCase = false)
		{
			int tLen = t.Length; //NullReferenceException
			int n = s?.Length ?? throw new ArgumentNullException(); if(n > tLen) return false;
			fixed (char* a = t, b = s) return _Eq(a + tLen - n, b, n, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="Ends(string, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		public static int Ends(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.Ends(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string ends with the specified character.
		/// Fast, case-sensitive.
		/// </summary>
		public static bool Ends(this string t, char c)
		{
			int i = t.Length - 1;
			return i >= 0 && t[i] == c;
		}

		/// <summary>
		/// Compares the beginning of this string with other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		public static bool Starts(this string t, string s, bool ignoreCase = false)
		{
			int tLen = t.Length; //NullReferenceException
			int n = s?.Length ?? throw new ArgumentNullException(); if(n > tLen) return false;
			fixed (char* a = t, b = s) return _Eq(a, b, n, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="Starts(string, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		public static int Starts(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.Starts(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string starts with the specified character.
		/// Fast, case-sensitive.
		/// </summary>
		public static bool Starts(this string t, char c)
		{
			return t.Length > 0 && t[0] == c;
		}

		/// <summary>
		/// Calls <see cref="string.IndexOf(string, StringComparison)"/>. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		public static int Index(this string t, string s, bool ignoreCase = false)
		{
			return t.IndexOf(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls <see cref="string.IndexOf(string, int, StringComparison)"/>. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i>.</exception>
		public static int Index(this string t, string s, int startIndex, bool ignoreCase = false)
		{
			return t.IndexOf(s, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls <see cref="string.IndexOf(string, int, int, StringComparison)"/>. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i> or <i>count</i>.</exception>
		public static int Index(this string t, string s, int startIndex, int count, bool ignoreCase = false)
		{
			return t.IndexOf(s, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		//rejected: LastIndex. Rarely used.
		//rejected: IndexOfAny, LastIndexOfAny and Trim that would use string instead of char[]. Not so often used. For speed/garbage it's better to use static char[].

		/// <summary>
		/// Returns true if this string contains string <i>s</i>. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Uses <see cref="string.IndexOf(string, int, StringComparison)"/>.
		/// </remarks>
		public static bool Has(this string t, string s, bool ignoreCase = false)
		{
			return t.IndexOf(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
		}
		//TODO: apply: replace Index and IndexOf.

		/// <summary>
		/// Returns true if this string contains character <i>c</i>.
		/// </summary>
		public static bool Has(this string t, char c)
		{
			return t.IndexOf(c) >= 0;
		}

		/// <summary>
		/// Returns <see cref="string.Length"/>. Returns 0 if this string is null.
		/// </summary>
		public static int Lenn(this string t) => t?.Length ?? 0;

		/// <summary>
		/// Splits this string into substrings using the specified separators.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="maxCount">The maximal number of substrings to get. If negative, gets all.</param>
		/// <param name="flags"></param>
		/// <seealso cref="Segments(string, string, SegFlags)"/>
		/// <seealso cref="SegSplitLines"/>
		/// <seealso cref="string.Split"/>
		/// <seealso cref="string.Join"/>
		public static string[] SegSplit(this string t, string separators, int maxCount, SegFlags flags = 0)
		{
			var x = new SegParser(t, separators, flags);
			return x.ToStringArray(maxCount);
		}

		/// <summary>
		/// Splits this string into substrings using the specified separators.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="flags"></param>
		public static string[] SegSplit(this string t, string separators, SegFlags flags = 0)
		{
			var x = new SegParser(t, separators, flags);
			return x.ToStringArray();
		}

		/// <summary>
		/// Splits this string into lines using separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t"></param>
		/// <param name="noEmptyLines">Don't get empty lines.</param>
		/// <remarks>
		/// Calls <see cref="SegSplit(string, string, int, SegFlags)"/> with separators = Separators.Line, maxCount = -1 (all lines), flags = noEmptyLines ? SegFlags.NoEmpty : 0.
		/// </remarks>
		/// <seealso cref="Segments(string, string, SegFlags)"/>
		public static string[] SegSplitLines(this string t, bool noEmptyLines = false)
		{
			return SegSplit(t, Separators.Line, noEmptyLines ? SegFlags.NoEmpty : 0);
		}

		/// <summary>
		/// Returns the number of lines.
		/// Counts line separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t"></param>
		/// <param name="preferMore">Add 1 if the string ends with a line separator or its length is 0.</param>
		public static int LineCount(this string t, bool preferMore = false)
		{
			if(t.Length == 0) return preferMore ? 1 : 0;
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
		/// Calls <see cref="string.ToLowerInvariant"/>.
		/// </summary>
		public static string Lower(this string t) => t.ToLowerInvariant();

		/// <summary>
		/// Calls <see cref="string.ToUpperInvariant"/>.
		/// </summary>
		public static string Upper(this string t) => t.ToUpperInvariant();

		/// <summary>
		/// Converts the string or only the first character (or Unicode surrogate pair) to upper case.
		/// Calls <see cref="string.ToUpperInvariant"/>.
		/// </summary>
		public static unsafe string Upper(this string t, bool firstCharOnly)
		{
			if(!firstCharOnly) return t.ToUpperInvariant();
			if(!char.IsLower(t, 0)) return t;
			if(t.Length >= 2 && char.IsHighSurrogate(t[0]) && char.IsLowSurrogate(t[1]))
				return t.Remove(2).ToUpperInvariant() + t.Substring(2);
			var u = string.Copy(t);
			fixed (char* p = u) *p = char.ToUpperInvariant(*p);
			return u;
		}

		/// <summary>
		/// Converts part of string to int.
		/// Returns the int value, or 0 if fails to convert.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends. If fails to convert, receives 0.</param>
		/// <param name="flags"></param>
		/// <remarks>
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// 
		/// Unlike int.Parse and Convert.ToInt32:
		///		The number in string can be followed by more text, like "123text".
		///		Has startIndex parameter that allows to get number from middle, like "text123text".
		///		Gets the end of the number part.
		///		No exception when cannot convert.
		///		Supports hexadecimal format, like "0x1A", case-insensitive.
		///		Much faster.
		///	
		/// The number in string can begin with ASCII spaces, tabs or newlines, like " 5".
		/// The number in string can be with "-" or "+", like "-5", but not like "- 5".
		/// Fails if the number is greater than +- uint.MaxValue (0xffffffff).
		/// The return value becomes negative if the number is greater than int.MaxValue, for example "0xffffffff" is -1, but it becomes correct if assigned to uint (need cast).
		/// Does not support non-integer numbers; for example, for "3.5E4" returns 3 and sets numberEndIndex=startIndex+1.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 or greater than string length.</exception>
		public static int ToInt(this string t, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out numberEndIndex, false, flags);
		}

		/// <summary>
		/// This overload does not have parameter <i>numberEndIndex</i>.
		/// </summary>
		public static int ToInt(this string t, int startIndex, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out _, false, flags);
		}

		/// <summary>
		/// This overload does not have parameters.
		/// </summary>
		public static int ToInt(this string t)
		{
			return (int)_ToInt(t, 0, out _, false, 0);
		}

		/// <summary>
		/// Converts part of string to long.
		/// Returns the long value, or 0 if fails to convert.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends. If fails to convert, receives 0.</param>
		/// <param name="flags"></param>
		/// <remarks>
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// 
		/// Unlike long.Parse and Convert.ToInt64:
		///		The number in string can be followed by more text, like "123text".
		///		Has startIndex parameter that allows to get number from middle, like "text123text".
		///		Gets the end of the number part.
		///		No exception when cannot convert.
		///		Supports hexadecimal format, like "0x1A", case-insensitive.
		///		Much faster.
		///	
		/// The number in string can begin with ASCII spaces, tabs or newlines, like " 5".
		/// The number in string can be with "-" or "+", like "-5", but not like "- 5".
		/// Fails if the number is greater than +- ulong.MaxValue (0xffffffffffffffff).
		/// The return value becomes negative if the number is greater than long.MaxValue, for example "0xffffffffffffffff" is -1, but it becomes correct if assigned to ulong (need cast).
		/// Does not support non-integer numbers; for example, for "3.5E4" returns 3 and sets numberEndIndex=startIndex+1.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">startIndex is less than 0 or greater than string length.</exception>
		public static long ToInt64(this string t, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			return _ToInt(t, startIndex, out numberEndIndex, true, flags);
		}

		/// <summary>
		/// This <see cref="ToInt64(string, int, out int, STIFlags)"/> overload does not have parameter numberEndIndex.
		/// </summary>
		public static long ToInt64(this string t, int startIndex, STIFlags flags = 0)
		{
			return _ToInt(t, startIndex, out _, true, flags);
		}

		/// <summary>
		/// This <see cref="ToInt64(string, int, out int, STIFlags)"/> overload does not have parameters.
		/// </summary>
		public static long ToInt64(this string t)
		{
			return _ToInt(t, 0, out _, true, 0);
		}

		static long _ToInt(string t, int startIndex, out int numberEndIndex, bool toLong, STIFlags flags)
		{
			numberEndIndex = 0;
			int len = t == null ? 0 : t.Length;
			if((uint)startIndex > len) throw new ArgumentOutOfRangeException("startIndex");
			int i = startIndex; char c = default;

			//skip spaces
			for(; ; i++) {
				if(i == len) return 0;
				c = t[i];
				if(c > ' ') break;
				if(c == ' ') continue;
				if(c < '\t' || c > '\r') break; //\t \n \v \f \r
			}
			if(i > startIndex && 0 != (flags & STIFlags.DontSkipSpaces)) return 0;

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
			switch(flags & (STIFlags.NoHex | STIFlags.IsHexWithout0x)) {
			case 0:
				if(c == '0' && i <= len - 3) {
					c = t[++i];
					if(c == 'x' || c == 'X') { i++; isHex = true; } else i--;
				}
				break;
			case STIFlags.IsHexWithout0x:
				isHex = true;
				break;
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
		/// Converts string to double.
		/// Calls <see cref="double.Parse(string, NumberStyles, IFormatProvider)"/> with CultureInfo.InvariantCulture and NumberStyles.Float|NumberStyles.AllowThousands.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="canThrow">If true, exception if the string is not a valid number or is null. If false, then returns 0.</param>
		public static double ToDouble(this string t, bool canThrow = false)
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
		public static float ToFloat(this string t, bool canThrow = false)
		{
			if(canThrow) return float.Parse(t, CultureInfo.InvariantCulture);
			return float.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float R) ? R : 0.0F;
		}

		/// <summary>
		/// Returns a new string in which a specified string replaces a specified count of characters at a specified position in this instance.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i> or <i>count</i>.</exception>
		public static string ReplaceAt(this string t, int startIndex, int count, string s)
		{
			return t.Remove(startIndex, count).Insert(startIndex, s);

			//slightly slower, 1 more allocations
			//return t.Substring(0, startIndex) + s + t.Substring(startIndex + count);

			//maybe less garbage (didn't measure), but slightly slower
			//using(new Util.LibStringBuilder(out var b)) {
			//	if(startIndex != 0) b.Append(t, 0, startIndex);
			//	b.Append(s);
			//	int i = startIndex + count, n = t.Length - i;
			//	if(n != 0) b.Append(t, i, n);
			//	return b.ToString();
			//}
		}

		/// <summary>
		/// Removes <i>count</i> characters from the end of this string.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static string RemoveSuffix(this string t, int count) => t.Remove(t.Length - count);

		/// <summary>
		/// If this string is longer than <i>limit</i>, returns its substring 0 to <i>limit</i>-1 with appended '…' character.
		/// Else returns this string.
		/// </summary>
		public static string Limit(this string t, int limit)
		{
			int k = t.Length;
			if(limit < 1) limit = 1;
			if(k <= limit) return t;
			return t.Remove(limit - 1) + "…";
		}
		//TODO: bool isPath=false: insert "…" in middle.
		//	Don't need it in Escape, because path cannot contain characters that need to be escaped.

		/// <summary>
		/// Replaces some characters with C# escape sequences.
		/// Replaces these characters: <c>'\\'</c>, <c>'\"'</c>, <c>'\t'</c>, <c>'\n'</c>, <c>'\r'</c> and all in range 0-31.
		/// If the string contains these characters, replaces and returns new string. Else returns this string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="limit">If the final string is longer than <i>limit</i>, get its substring 0 to <i>limit</i>-1 with appended '…' character. The enclosing "" are not counted.</param>
		/// <param name="quote">Enclose in "".</param>
		public static string Escape(this string t, int limit = 0, bool quote = false)
		{
			int i, len = t.Length;
			if(len == 0) return quote ? "\"\"" : t;

			if(limit > 0) {
				if(len > limit) len = limit - 1; else limit = 0;
			}

			for(i = 0; i < len; i++) {
				var c = t[i];
				if(c < ' ' || c == '\\' || c == '\"') goto g1;
				//tested: Unicode line-break chars in most controls don't break lines, therefore don't need to escape
			}
			if(limit > 0) t = Limit(t, limit);
			if(quote) t = "\"" + t + "\"";
			return t;
			g1:
			using(new Util.LibStringBuilder(out var b, len + len / 16 + 100)) {
				if(quote) b.Append('\"');
				for(i = 0; i < len; i++) {
					var c = t[i];
					if(c < ' ') {
						switch(c) {
						case '\t': b.Append("\\t"); break;
						case '\n': b.Append("\\n"); break;
						case '\r': b.Append("\\r"); break;
						default: b.Append("\\u").Append(((ushort)c).ToString("x4")); break;
						}
					} else if(c == '\\') b.Append("\\\\");
					else if(c == '\"') b.Append("\\\"");
					else b.Append(c);

					if(limit > 0 && b.Length - (quote ? 1 : 0) >= len) break;
				}

				if(limit > 0) b.Append('…');
				if(quote) b.Append('\"');
				return b.ToString();
			}
		}
		//FUTURE: Unescape()

		//rejected
		///// <summary>
		///// Replaces all "'" with "''".
		///// </summary>
		//public static string SqlEscape_(this string t) => t.Replace("'", "''");

		//rejected. Rarely used. If need, add to More.
		///// <summary>
		///// Converts this string to '\0'-terminated char[].
		///// </summary>
		//public static char[] AToArrayAnd0(this string t)
		//{
		//	var c = new char[t.Length + 1];
		//	for(int i = 0; i < t.Length; i++) c[i] = t[i];
		//	return c;
		//}

		//rejected. Better call AConvert.Utf8FromString directly.
		///// <summary>
		///// Converts this string to '\0'-terminated UTF8 string as byte[].
		///// </summary>
		///// <remarks>
		///// Calls <see cref="AConvert.Utf8FromString"/>.
		///// </remarks>
		///// <seealso cref="AConvert.Utf8ToString"/>
		///// <seealso cref="Encoding.UTF8"/>
		//public static byte[] AToUtf8And0(this string t) => AConvert.Utf8FromString(t);

		internal static class Lib
		{
			/// <summary><c>{ '\r', '\n' }</c></summary>
			internal static readonly char[] lineSep = { '\r', '\n' };

			/// <summary><c>{ '\\', '/' }</c></summary>
			internal static readonly char[] pathSep = { '\\', '/' };

			///// <summary><c>{ '*', '?' }</c></summary>
			//internal static readonly char[] wildcard = { '*', '?' };

		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="ExtString.ToInt"/> and similar functions.
	/// </summary>
	[Flags]
	public enum STIFlags
	{
		/// <summary>
		/// Don't support hexadecimal numbers (numbers with prefix "0x").
		/// </summary>
		NoHex = 1,

		/// <summary>
		/// The number in string is hexadecimal without a prefix, like "1A".
		/// </summary>
		IsHexWithout0x = 2,

		/// <summary>
		/// If string starts with a space, return 0.
		/// For example, if string is " 5" return 0, not 5. 
		/// </summary>
		DontSkipSpaces = 4,
	}

}