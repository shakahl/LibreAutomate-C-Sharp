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
	/// Adds extension methods to <see cref="string"/>.
	/// </summary>
	/// <remarks>
	/// Some .NET String methods use StringComparison.CurrentCulture by default, while others don't. It is confusing, dangerous (easy to make bugs), slower and rarely useful. Microsoft recommends to specify StringComparison.Ordinal[IgnoreCase] explicitly. See https://msdn.microsoft.com/en-us/library/ms973919.aspx. This class adds their versions that use StringComparison.Ordinal[IgnoreCase]. Same name, with prefix "_". For example, StartsWith uses StringComparison.CurrentCulture, and StartsWith_ uses StringComparison.Ordinal[IgnoreCase].
	/// </remarks>
	public static unsafe partial class String_
	{
		/// <summary>
		/// Compares this and other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		public static bool Equals_(this string t, string value, bool ignoreCase = false)
		{
			int len = t.Length; //NullReferenceException
			if(value is null || len != value.Length) return false;
			//if(ReferenceEquals(t, value)) return true; //rare
			fixed (char* a = t, b = value) return _Equals(a, b, len, ignoreCase);
		}
		//TODO: Equals_(string value), EqualsI_(string value), Equals_(bool ignoreCase = false, params string[] strings).

		static bool _Equals(char* a, char* b, int len, bool ignoreCase)
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
		/// Calls <see cref="Equals_(string, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		public static int Equals_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.Equals_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Compares part of this string with other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="startIndex"/>.</exception>
		public static bool EqualsAt_(this string t, int startIndex, string value, bool ignoreCase = false)
		{
			if((uint)startIndex > t.Length) throw new ArgumentOutOfRangeException(); //and NullReferenceException
			int n = value?.Length ?? throw new ArgumentNullException();
			if(n > t.Length - startIndex) return false;
			fixed (char* a = t, b = value) return _Equals(a + startIndex, b, n, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="EqualsAt_(string, int, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <exception cref="ArgumentNullException">A string in <paramref name="strings"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="startIndex"/>.</exception>
		public static int EqualsAt_(this string t, int startIndex, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.EqualsAt_(startIndex, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		//For StringSegment.
		internal static bool LibEqualsAt_(string s1, int i1, string s2, int i2, int len, bool ignoreCase)
		{
			fixed (char* a = s1, b = s2) return _Equals(a + i1, b + i2, len, ignoreCase);
		}

		/// <summary>
		/// Compares the end of this string with other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static bool EndsWith_(this string t, string value, bool ignoreCase = false)
		{
			int tLen = t.Length; //NullReferenceException
			int n = value?.Length ?? throw new ArgumentNullException(); if(n > tLen) return false;
			fixed (char* a = t, b = value) return _Equals(a + tLen - n, b, n, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="EndsWith_(string, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <exception cref="ArgumentNullException">A string in <paramref name="strings"/> is null.</exception>
		public static int EndsWith_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.EndsWith_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string ends with the specified character.
		/// Fast, case-sensitive.
		/// </summary>
		public static bool EndsWith_(this string t, char value)
		{
			int i = t.Length - 1;
			return i >= 0 && t[i] == value;
		}

		/// <summary>
		/// Compares the beginning of this string with other string. Returns true if equal. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static bool StartsWith_(this string t, string value, bool ignoreCase = false)
		{
			int tLen = t.Length; //NullReferenceException
			int n = value?.Length ?? throw new ArgumentNullException(); if(n > tLen) return false;
			fixed (char* a = t, b = value) return _Equals(a, b, n, ignoreCase);
		}

		/// <summary>
		/// Calls <see cref="StartsWith_(string, string, bool)"/> for each string specified in the argument list until it returns true.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <exception cref="ArgumentNullException">A string in <paramref name="strings"/> is null.</exception>
		public static int StartsWith_(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(t.StartsWith_(strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string starts with the specified character.
		/// Fast, case-sensitive.
		/// </summary>
		public static bool StartsWith_(this string t, char value)
		{
			return t.Length > 0 && t[0] == value;
		}

		/// <summary>
		/// Calls <see cref="string.IndexOf(string, StringComparison)"/>. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static int IndexOf_(this string t, string value, bool ignoreCase = false)
		{
			return t.IndexOf(value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls <see cref="string.IndexOf(string, int, StringComparison)"/>. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="startIndex"/>.</exception>
		public static int IndexOf_(this string t, string value, int startIndex, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Calls <see cref="string.IndexOf(string, int, int, StringComparison)"/>. Uses ordinal comparison.
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="startIndex"/> or <paramref name="count"/>.</exception>
		public static int IndexOf_(this string t, string value, int startIndex, int count, bool ignoreCase = false)
		{
			return t.IndexOf(value, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		//rejected: LastIndexOf_. Rarely used.
		//rejected: IndexOfAny_, LastIndexOfAny_ and Trim_ that would use string instead of char[]. Not so often used. For speed/garbage it's better to use static char[].

		/// <summary>
		/// Returns <see cref="string.Length"/>. If this string is null, returns 0.
		/// </summary>
		public static int Length_(this string t) => t?.Length ?? 0;

		/// <summary>
		/// Splits this string into substrings using the specified separators.
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
		public static int LineCount_(this string t, bool preferMore = false)
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
		/// Converts character index in whole string to line index and character index in that line.
		/// Returns 0-based line index.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="index">Character index in whole string.</param>
		/// <param name="indexInLine">Receives 0-based character index in that line.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static int LineIndex_(this string t, int index, out int indexInLine)
		{
			if((uint)index > t.Length) throw new ArgumentOutOfRangeException();
			int line = 0, lineStart = 0;
			for(int i = 0; i < index; i++) {
				char c = t[i];
				if(c > '\r') continue;
				if(c != '\n') {
					if(c != '\r') continue;
					if(i < t.Length - 1 && t[i + 1] == '\n') continue;
				}

				lineStart = i + 1;
				line++;
			}
			indexInLine = index - lineStart;
			return line;
		}

		/// <summary>
		/// Calls <see cref="string.ToLowerInvariant"/>.
		/// </summary>
		public static string ToLower_(this string t)
		{
			return t.ToLowerInvariant();
		}

		/// <summary>
		/// Calls <see cref="string.ToUpperInvariant"/>.
		/// </summary>
		public static string ToUpper_(this string t)
		{
			return t.ToUpperInvariant();
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
		public static int ToInt_(this string t, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out numberEndIndex, false, flags);
		}

		/// <summary>
		/// This <see cref="ToInt_(string, int, out int, STIFlags)"/> overload does not have parameter numberEndIndex.
		/// </summary>
		public static int ToInt_(this string t, int startIndex, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out _, false, flags);
		}

		/// <summary>
		/// This <see cref="ToInt_(string, int, out int, STIFlags)"/> overload does not have parameters.
		/// </summary>
		public static int ToInt_(this string t)
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
		public static long ToLong_(this string t, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			return _ToInt(t, startIndex, out numberEndIndex, true, flags);
		}

		/// <summary>
		/// This <see cref="ToLong_(string, int, out int, STIFlags)"/> overload does not have parameter numberEndIndex.
		/// </summary>
		public static long ToLong_(this string t, int startIndex, STIFlags flags = 0)
		{
			return _ToInt(t, startIndex, out _, true, flags);
		}

		/// <summary>
		/// This <see cref="ToLong_(string, int, out int, STIFlags)"/> overload does not have parameters.
		/// </summary>
		public static long ToLong_(this string t)
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
			if(i > startIndex && 0 != (flags & STIFlags.DoNotSkipSpaces)) return 0;

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
		/// Returns a new string in which a specified string replaces a specified count of characters at a specified position in this instance.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <paramref name="startIndex"/> or <paramref name="count"/>.</exception>
		public static string ReplaceAt_(this string t, int startIndex, int count, string value)
		{
			return t.Remove(startIndex, count).Insert(startIndex, value);

			//slightly slower, 1 more allocations
			//return t.Substring(0, startIndex) + value + t.Substring(startIndex + count);

			//maybe less garbage (didn't measure), but slightly slower
			//using(new Util.LibStringBuilder(out var b)) {
			//	if(startIndex != 0) b.Append(t, 0, startIndex);
			//	b.Append(value);
			//	int i = startIndex + count, n = t.Length - i;
			//	if(n != 0) b.Append(t, i, n);
			//	return b.ToString();
			//}
		}

		/// <summary>
		/// If this string is longer than <paramref name="limit"/>, returns its substring 0 to <paramref name="limit"/>-1 with appended '…' character.
		/// Else returns this string.
		/// </summary>
		public static string Limit_(this string t, int limit)
		{
			int k = t.Length;
			if(limit < 1) limit = 1;
			if(k <= limit) return t;
			return t.Remove(limit - 1) + "…";
		}
		//CONSIDER: if string looks like path, insert "…" in the middle.
		//	Don't need it in Escape_, because path cannot contain characters that need to be escaped.

		/// <summary>
		/// Replaces some characters with C# escape sequences.
		/// Replaces these characters: '\\', '\"', '\t', '\n', '\r' and all in range 0-31.
		/// If the string contains these characters, replaces and returns new string. Else returns this string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="limit">If the final string is longer than <paramref name="limit"/>, get its substring 0 to <paramref name="limit"/>-1 with appended '…' character. The enclosing "" are not counted.</param>
		/// <param name="quote">Enclose in "".</param>
		public static string Escape_(this string t, int limit = 0, bool quote = false)
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
			if(limit > 0) t = Limit_(t, limit);
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

		/// <summary>
		/// Replaces all "'" with "''".
		/// </summary>
		public static string SqlEscape_(this string t) => t.Replace("'", "''");

		/// <summary>
		/// Returns true if this string is "" or contains only ASCII characters.
		/// </summary>
		public static bool IsAscii_(this string t)
		{
			for(int i = 0; i < t.Length; i++) {
				if(t[i] > 0x7f) return false;
			}
			return true;
		}

		/// <summary>
		/// Converts this string to '\0'-terminated char[].
		/// </summary>
		public static char[] ToCharArray_(this string t)
		{
			var c = new char[t.Length + 1];
			for(int i = 0; i < t.Length; i++) c[i] = t[i];
			return c;
		}

		internal static class Lib
		{
			/// <summary>{ '\r', '\n' }</summary>
			internal static char[] lineSep = new char[] { '\r', '\n' };

			/// <summary>{ '\\', '/' }</summary>
			internal static char[] pathSep = new char[] { '\\', '/' };

			///// <summary>{ '*', '?' }</summary>
			//internal static char[] wildcard = new char[] { '*', '?' };

		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for String_.ToIntX functions.
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
		DoNotSkipSpaces = 4,
	}

}