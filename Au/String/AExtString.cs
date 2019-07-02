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
using static Au.AStatic;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Adds extension methods for <see cref="String"/>.
	/// </summary>
	/// <remarks>
	/// Some .NET <see cref="String"/> methods use <see cref="StringComparison.CurrentCulture"/> by default, while others use ordinal or invariant comparison. It is confusing (difficult to remember), dangerous (easy to make bugs), slower and rarely useful.
	/// Microsoft recommends to specify <b>StringComparison.Ordinal[IgnoreCase]</b> explicitly. See https://msdn.microsoft.com/en-us/library/ms973919.aspx.
	/// This class adds their versions that use <b>StringComparison.Ordinal[IgnoreCase]</b>. Same or similar name, usually a shorter version.
	/// For example, <b>EndsWith</b> (of .NET) uses <b>StringComparison.CurrentCulture</b>, and <b>Ends</b> (of this class) uses <b>StringComparison.Ordinal</b>.
	/// 
	/// This class also adds more methods.
	/// You also can find string functions in other classes of this library, including <see cref="AStringUtil"/>, <see cref="ARegex"/>, <see cref="AChar"/>, <see cref="APath"/>, <see cref="AStringSegment"/>, <see cref="ACsv"/>, <see cref="AKeys.More"/>, <see cref="AConvert"/>, <see cref="AHash"/>.
	/// </remarks>
	public static unsafe partial class AExtString
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
				var table = LibTables.LowerCase;

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
		/// Compares this and other string. Returns true if equal.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="s">Other string. Can be null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		/// <seealso cref="string.Compare"/>
		/// <seealso cref="string.CompareOrdinal"/>
		public static bool Eq(this string t, string s, bool ignoreCase = false)
		{
			if(t == null) return s == null;
			int len = t.Length;
			if(s == null || len != s.Length) return false;
			if(ReferenceEquals(t, s)) return true;
			fixed (char* a = t, b = s) return _Eq(a, b, len, ignoreCase);
		}

		/// <summary>
		/// Compares this strings with multiple strings.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="strings">Other strings. Strings can be null.</param>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Eq(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(Eq(t, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Compares part of this string with other string. Returns true if equal.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="startIndex">Offset in this string. If invalid, returns false.</param>
		/// <param name="s">Other string.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		/// <seealso cref="string.Compare"/>
		/// <seealso cref="string.CompareOrdinal"/>
		public static bool Eq(this string t, int startIndex, string s, bool ignoreCase = false)
		{
			if(t == null) return false;
			if((uint)startIndex > t.Length) return false;
			int n = s?.Length ?? throw new ArgumentNullException();
			if(n > t.Length - startIndex) return false;
			fixed (char* a = t, b = s) return _Eq(a + startIndex, b, n, ignoreCase);
		}

		/// <summary>
		/// Compares part of this string with multiple strings.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="startIndex">Offset in this string. If invalid, returns false.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="strings">Other strings.</param>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Eq(this string t, int startIndex, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(Eq(t, startIndex, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		//For AStringSegment.
		internal static bool LibEq(string s1, int i1, string s2, int i2, int len, bool ignoreCase)
		{
			fixed (char* a = s1, b = s2) return _Eq(a + i1, b + i2, len, ignoreCase);
		}

		/// <summary>
		/// Compares this and other string ignoring case (case-insensitive). Returns true if equal.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="s">Other string. Can be null.</param>
		/// <remarks>
		/// Calls <see cref="Eq(string, string, bool)"/> with <i>ignoreCase</i> true.
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static bool Eqi(this string t, string s) => Eq(t, s, true);

		//rejected. Rarely used. Or also need Startsi, Endsi, Findi...
		//public static bool Eqi(this string t, int startIndex, string s) => Eq(t, startIndex, s, true);

		/// <summary>
		/// Compares the end of this string with other string. Returns true if equal.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="s">Other string.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static bool Ends(this string t, string s, bool ignoreCase = false)
		{
			if(t == null) return false;
			int tLen = t.Length;
			int n = s?.Length ?? throw new ArgumentNullException();
			if(n > tLen) return false;
			fixed (char* a = t, b = s) return _Eq(a + tLen - n, b, n, ignoreCase);
		}

		/// <summary>
		/// Compares the end of this string with multiple strings.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="strings">Other strings.</param>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Ends(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(Ends(t, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string ends with the specified character.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="c">The character.</param>
		public static bool Ends(this string t, char c)
		{
			if(t == null) return false;
			int i = t.Length - 1;
			return i >= 0 && t[i] == c;
		}

		/// <summary>
		/// Compares the beginning of this string with other string. Returns true if equal.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="s">Other string.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static bool Starts(this string t, string s, bool ignoreCase = false)
		{
			if(t == null) return false;
			int tLen = t.Length;
			int n = s?.Length ?? throw new ArgumentNullException();
			if(n > tLen) return false;
			fixed (char* a = t, b = s) return _Eq(a, b, n, ignoreCase);
		}

		/// <summary>
		/// Compares the beginning of this string with multiple strings.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="strings">Other strings.</param>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Starts(this string t, bool ignoreCase = false, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(Starts(t, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string starts with the specified character.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="c">The character.</param>
		public static bool Starts(this string t, char c)
		{
			if(t == null) return false;
			return t.Length > 0 && t[0] == c;
		}

		/// <summary>
		/// Finds substring in this string. Returns its zero-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string. If null, returns -1.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Calls <see cref="string.IndexOf(string, StringComparison)"/>.
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Find(this string t, string s, bool ignoreCase = false)
		{
			return t?.IndexOf(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? -1;
		}

		/// <summary>
		/// Finds substring in part of this string. Returns its zero-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string. If null, returns -1.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i>.</exception>
		/// <remarks>
		/// Calls <see cref="string.IndexOf(string, int, StringComparison)"/>.
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Find(this string t, string s, int startIndex, bool ignoreCase = false)
		{
			return t?.IndexOf(s, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? -1;
		}

		/// <summary>
		/// Finds substring in part of this string. Returns its zero-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string. If null, returns -1.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="count">The number of characters to examine.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i> or <i>count</i>.</exception>
		/// <remarks>
		/// Calls <see cref="string.IndexOf(string, int, int, StringComparison)"/>.
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Find(this string t, string s, int startIndex, int count, bool ignoreCase = false)
		{
			return t?.IndexOf(s, startIndex, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? -1;
		}

		/// <summary>
		/// Returns true if this string contains the specified substring.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Calls <see cref="string.IndexOf(string, StringComparison)"/>.
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static bool Has(this string t, string s, bool ignoreCase = false)
		{
			return (t?.IndexOf(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? -1) >= 0;
		}

		/// <summary>
		/// Returns true if this string contains the specified character.
		/// </summary>
		/// <param name="t">This string. If null, returns false.</param>
		/// <param name="c">Character to find.</param>
		public static bool Has(this string t, char c)
		{
			return (t?.IndexOf(c) ?? -1) >= 0;
		}

		/// <summary>
		/// Finds the first occurence in this string of any character in (or not in) a character set. Returns its zero-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string. If null, returns -1.</param>
		/// <param name="chars">Characters to find.</param>
		/// <param name="startIndex">The search starting position. Default 0.</param>
		/// <param name="count">The number of characters to examine. If -1 (default), until the end of string.</param>
		/// <param name="not">Find character not specified in <i>chars</i>.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i> or <i>count</i>.</exception>
		public static int FindChars(this string t, string chars, int startIndex = 0, int count = -1, bool not = false)
		{
			if(t == null) return -1;
			if(count == -1) count = t.Length - startIndex;
			if((uint)startIndex > t.Length || (uint)count > t.Length - startIndex) throw new ArgumentOutOfRangeException();
			if(chars == null) throw new ArgumentNullException();
			if(not) {
				for(int i = startIndex, n = startIndex + count; i < n; i++) {
					char c = t[i];
					for(int j = 0; j < chars.Length; j++) if(chars[j] == c) goto g1;
					return i;
					g1:;
				}
			} else {
				for(int i = startIndex, n = startIndex + count; i < n; i++) {
					char c = t[i];
					for(int j = 0; j < chars.Length; j++) if(chars[j] == c) return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Finds the last occurence in this string of any character in (or not in) a character set. Returns its zero-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string. If null, returns -1.</param>
		/// <param name="chars">Characters to find.</param>
		/// <param name="not">Find character not specified in <i>chars</i>.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		public static int FindLastChars(this string t, string chars, bool not = false)
		{
			if(t == null) return -1;
			int len = t.Length;
			if(chars == null) throw new ArgumentNullException();
			if(not) {
				for(int i = len - 1; i >= 0; i--) {
					char c = t[i];
					for(int j = 0; j < chars.Length; j++) if(chars[j] == c) goto g1;
					return i;
					g1:;
				}
			} else {
				for(int i = len - 1; i >= 0; i--) {
					char c = t[i];
					for(int j = 0; j < chars.Length; j++) if(chars[j] == c) return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Removes specified characters for the start and/or end of this string. Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters to remove.</param>
		/// <param name="start">Remove from start. Default true.</param>
		/// <param name="end">Remove from end. Default true.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		public static string TrimChars(this string t, string chars, bool start = true, bool end = true)
		{
			int from = 0, to = t.Length;
			if(end) to = FindLastChars(t, chars, not: true) + 1;
			if(start) from = FindChars(t, chars, 0, to, not: true);
			if(from == 0 && to == t.Length) return t;
			return t.Substring(from, to - from);
		}

		/// <summary>
		/// Finds whole word. Returns its zero-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string. If null, returns -1.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="otherWordChars">Additional word characters, for which <see cref="Char.IsLetterOrDigit"/> returns false. For example "_".</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i>.</exception>
		/// <remarks>
		/// If <i>s</i> starts with a word character finds substring that is not preceded by a word character.
		/// If <i>s</i> ends with a word character, finds substring that is not followed by a word character.
		/// Word characters are those for which <see cref="Char.IsLetterOrDigit"/> returns true plus those specified in <i>otherWordChars</i>.
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int FindWord(this string t, string s, int startIndex = 0, bool ignoreCase = false, string otherWordChars = null)
		{
			if(t == null) return -1;
			if((uint)startIndex > t.Length) throw new ArgumentOutOfRangeException();
			if(s == null) throw new ArgumentNullException();
			int lens = s.Length; if(lens == 0) return 0; //like IndexOf and Find

			bool wordStart = _IsWordChar(s, 0, false, otherWordChars),
				wordEnd = _IsWordChar(s, lens - 1, true, otherWordChars);

			for(int i = startIndex, iMax = t.Length - lens; i <= iMax; i++) {
				i = t.Find(s, i, t.Length - i, ignoreCase);
				if(i < 0) break;
				if(wordStart && i > 0 && _IsWordChar(t, i - 1, true, otherWordChars)) continue;
				if(wordEnd && i < iMax && _IsWordChar(t, i + lens, false, otherWordChars)) continue;
				return i;
			}
			return -1;

			static bool _IsWordChar(string s, int i, bool expandLeft, string otherWordChars)
			{
				char c = s[i];
				if(c >= '\uD800' && c <= '\uDFFF') { //Unicode surrogates
					if(expandLeft) {
						if(Char.IsLowSurrogate(s[i])) return i > 0 && Char.IsHighSurrogate(s[i - 1]) && Char.IsLetterOrDigit(s, i - 1);
					} else {
						if(Char.IsHighSurrogate(s[i])) return i < s.Length - 1 && Char.IsLowSurrogate(s[i + 1]) && Char.IsLetterOrDigit(s, i);
					}
				} else {
					if(Char.IsLetterOrDigit(c)) return true;
					if(otherWordChars?.Has(c) ?? false) return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Returns <see cref="string.Length"/>. Returns 0 if this string is null.
		/// </summary>
		/// <param name="t">This string.</param>
		public static int Lenn(this string t) => t?.Length ?? 0;

		/// <summary>
		/// Splits this string into substrings using the specified separators.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="maxCount">The maximal number of substrings to get. If negative, gets all. Else if there are more substrings, the last element will contain single substring, unlike with <see cref="String.Split"/>.</param>
		/// <param name="flags"></param>
		/// <seealso cref="Segments(string, string, SegFlags)"/>
		/// <seealso cref="SegLines"/>
		/// <seealso cref="SegParser"/>
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
		/// <param name="t">This string.</param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="flags"></param>
		public static string[] SegSplit(this string t, string separators, SegFlags flags = 0)
		{
			var x = new SegParser(t, separators, flags);
			return x.ToStringArray();
		}

		/// <summary>
		/// Splits this string into lines using separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="maxCount">The maximal number of substrings to get. If negative, gets all. Else if there are more lines, the last element will contain single line, unlike with <see cref="String.Split"/></param>
		/// <param name="noEmptyLines">Don't need empty lines.</param>
		/// <remarks>
		/// Uses this code: <c>return t.SegSplit(SegSep.Line, noEmptyLines ? SegFlags.NoEmpty : 0);</c>.
		/// See <see cref="SegSplit(string, string, int, SegFlags)"/>, <see cref="SegSep.Line"/>, <see cref="SegFlags.NoEmpty"/>.
		/// </remarks>
		public static string[] SegLines(this string t, int maxCount, bool noEmptyLines = false)
		{
			return SegSplit(t, SegSep.Line, maxCount, noEmptyLines ? SegFlags.NoEmpty : 0);
		}

		/// <summary>
		/// Splits this string into lines using separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="noEmptyLines">Don't need empty lines.</param>
		/// <remarks>
		/// Uses this code: <c>return t.SegSplit(SegSep.Line, noEmptyLines ? SegFlags.NoEmpty : 0);</c>.
		/// See <see cref="SegSplit(string, string, SegFlags)"/>, <see cref="SegSep.Line"/>, <see cref="SegFlags.NoEmpty"/>.
		/// </remarks>
		public static string[] SegLines(this string t, bool noEmptyLines = false)
		{
			return SegSplit(t, SegSep.Line, noEmptyLines ? SegFlags.NoEmpty : 0);
		}

		/// <summary>
		/// Returns the number of lines.
		/// Counts line separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="preferMore">Add 1 if the string ends with a line separator or its length is 0.</param>
		/// <seealso cref="AStringUtil.LineAndColumn"/>
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
		/// Converts this string to lower case. Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <remarks>
		/// Calls <see cref="string.ToLowerInvariant"/>.
		/// </remarks>
		public static string Lower(this string t) => t.ToLowerInvariant();

		/// <summary>
		/// Converts this string to upper case. Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <remarks>
		/// Calls <see cref="string.ToUpperInvariant"/>.
		/// </remarks>
		public static string Upper(this string t) => t.ToUpperInvariant();

		/// <summary>
		/// Converts this string or only the first character to upper case. Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="firstCharOnly">Convert only the first character.</param>
		/// <remarks>
		/// Calls <see cref="string.ToUpperInvariant"/>.
		/// </remarks>
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

		#region ToNumber

		/// <summary>
		/// Converts part of this string to int.
		/// Returns the int value, or 0 if fails to convert.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends. If fails to convert, receives 0.</param>
		/// <param name="flags"></param>
		/// <remarks>
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// 
		/// Unlike <b>int.Parse</b> and <b>Convert.ToInt32</b>:
		///		The number in string can be followed by more text, like <c>"123text"</c>.
		///		Has <i>startIndex</i> parameter that allows to get number from middle, like <c>"text123text"</c>.
		///		Gets the end of the number part.
		///		No exception when cannot convert.
		///		Supports hexadecimal format, like <c>"0x1A"</c>, case-insensitive.
		///		Much faster.
		///	
		/// The number in string can begin with ASCII spaces, tabs or newlines, like <c>" 5"</c>.
		/// The number in string can be with <c>"-"</c> or <c>"+"</c>, like <c>"-5"</c>, but not like <c>"- 5"</c>.
		/// Fails if the number is greater than +- <b>uint.MaxValue</b> (0xffffffff).
		/// The return value becomes negative if the number is greater than <b>int.MaxValue</b>, for example <c>"0xffffffff"</c> is -1, but it becomes correct if assigned to uint (need cast).
		/// Does not support non-integer numbers; for example, for <c>"3.5E4"</c> returns 3 and sets <c>numberEndIndex=startIndex+1</c>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static int ToInt(this string t, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out numberEndIndex, false, flags);
		}

		/// <summary>
		/// Converts part of this string to int.
		/// Returns the int value, or 0 if fails to convert.
		/// </summary>
		public static int ToInt(this string t, int startIndex, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out _, false, flags);
		}

		/// <summary>
		/// Converts the start of this string to int.
		/// Returns the int value, or 0 if fails to convert.
		/// </summary>
		public static int ToInt(this string t, STIFlags flags = 0)
		{
			return (int)_ToInt(t, 0, out _, false, flags);
		}

		/// <summary>
		/// Converts part of this string to long.
		/// Returns the long value, or 0 if fails to convert.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in string where the number part ends. If fails to convert, receives 0.</param>
		/// <param name="flags"></param>
		/// <remarks>
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// 
		/// Unlike <b>long.Parse</b> and <b>Convert.ToInt64</b>:
		///		The number in string can be followed by more text, like <c>"123text"</c>.
		///		Has <i>startIndex</i> parameter that allows to get number from middle, like <c>"text123text"</c>.
		///		Gets the end of the number part.
		///		No exception when cannot convert.
		///		Supports hexadecimal format, like <c>"0x1A"</c>, case-insensitive.
		///		Much faster.
		///	
		/// The number in string can begin with ASCII spaces, tabs or newlines, like <c>" 5"</c>.
		/// The number in string can be with <c>"-"</c> or <c>"+"</c>, like <c>"-5"</c>, but not like <c>"- 5"</c>.
		/// Fails if the number is greater than +- <b>ulong.MaxValue</b> (0xffffffffffffffff).
		/// The return value becomes negative if the number is greater than <b>long.MaxValue</b>, for example <c>"0xffffffffffffffff"</c> is -1, but it becomes correct if assigned to ulong (need cast).
		/// Does not support non-integer numbers; for example, for <c>"3.5E4"</c> returns 3 and sets <c>numberEndIndex=startIndex+1</c>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static long ToInt64(this string t, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			return _ToInt(t, startIndex, out numberEndIndex, true, flags);
		}

		/// <summary>
		/// Converts part of this string to long.
		/// Returns the long value, or 0 if fails to convert.
		/// </summary>
		public static long ToInt64(this string t, int startIndex, STIFlags flags = 0)
		{
			return _ToInt(t, startIndex, out _, true, flags);
		}

		/// <summary>
		/// Converts the start of this string to long.
		/// Returns the long value, or 0 if fails to convert.
		/// </summary>
		public static long ToInt64(this string t, STIFlags flags = 0)
		{
			return _ToInt(t, 0, out _, true, flags);
		}

		static long _ToInt(string t, int startIndex, out int numberEndIndex, bool toLong, STIFlags flags)
		{
			numberEndIndex = 0;
			if(t == null) return 0;
			int len = t.Length;
			if((uint)startIndex > len) throw new ArgumentOutOfRangeException("startIndex");
			int i = startIndex; char c;

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
		/// Converts this string to double.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="canThrow">If true, exception if the string is not a valid number or is null. If false (default), then returns 0.</param>
		/// <remarks>
		/// Calls <see cref="double.Parse(string, NumberStyles, IFormatProvider)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b> and <see cref="NumberStyles"/> <b>Float</b> and <b>AllowThousands</b>.
		/// </remarks>
		public static double ToDouble(this string t, bool canThrow = false)
		{
			if(canThrow) return double.Parse(t, CultureInfo.InvariantCulture);
			return double.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double R) ? R : 0.0;
		}

		/// <summary>
		/// Converts this string to float.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="canThrow">If true, exception if the string is not a valid number or is null. If false (default), then returns 0.</param>
		/// <remarks>
		/// Calls <see cref="float.Parse(string, NumberStyles, IFormatProvider)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b> and <see cref="NumberStyles"/> <b>Float</b> and <b>AllowThousands</b>.
		/// </remarks>
		public static float ToFloat(this string t, bool canThrow = false)
		{
			if(canThrow) return float.Parse(t, CultureInfo.InvariantCulture);
			return float.TryParse(t, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float R) ? R : 0.0F;
		}

		#endregion

		/// <summary>
		/// Replaces part of this string with other string. Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="startIndex">Offset in this string.</param>
		/// <param name="count">Count of characters to replace.</param>
		/// <param name="s">The replacement string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i> or <i>count</i>.</exception>
		/// <remarks>
		/// More info: <see cref="string.Remove(int, int)"/>, <see cref="string.Insert"/>.
		/// </remarks>
		public static string ReplaceAt(this string t, int startIndex, int count, string s)
		{
			return t.Remove(startIndex, count).Insert(startIndex, s);

			//slightly slower, 1 more allocations
			//return t.Substring(0, startIndex) + s + t.Substring(startIndex + count);

			//maybe less garbage (didn't measure), but slightly slower
			//using(new LibStringBuilder(out var b)) {
			//	if(startIndex != 0) b.Append(t, 0, startIndex);
			//	b.Append(s);
			//	int i = startIndex + count, n = t.Length - i;
			//	if(n != 0) b.Append(t, i, n);
			//	return b.ToString();
			//}
		}

		/// <summary>
		/// Removes <i>count</i> characters from the end of this string.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="count">Count of characters to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static string RemoveSuffix(this string t, int count) => t.Remove(t.Length - count);

		/// <summary>
		/// Removes <i>suffix</i> string from the end.
		/// Returns the result string. Returns this string if does not end with <i>suffix</i>.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="suffix">Substring to remove.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>suffix</i> is null.</exception>
		public static string RemoveSuffix(this string t, string suffix, bool ignoreCase = false)
		{
			if(!t.Ends(suffix, ignoreCase)) return t;
			return RemoveSuffix(t, suffix.Length);
		}

		/// <summary>
		/// Removes <i>suffix</i> character from the end.
		/// Returns the result string. Returns this string if does not end with <i>suffix</i>.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="suffix">Character to remove.</param>
		/// <exception cref="ArgumentNullException"><i>suffix</i> is null.</exception>
		public static string RemoveSuffix(this string t, char suffix)
		{
			if(!t.Ends(suffix)) return t;
			return RemoveSuffix(t, 1);
		}

		/// <summary>
		/// If this string is longer than <i>limit</i>, returns its substring 0 to <i>limit</i>-1 with appended '…' character.
		/// Else returns this string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="limit">Maximal length of the result string. If less than 1, is used 1.</param>
		/// <param name="middle">Let "…" be in middle. For example it is useful when the string is a path, to avoid removing filename.</param>
		public static string Limit(this string t, int limit, bool middle = false)
		{
			int k = t.Length;
			if(limit < 1) limit = 1;
			if(k <= limit) return t;
			limit--;
			if(!middle) return t.Remove(limit) + "…";
			return t.ReplaceAt(limit / 2, k - limit, "…");
		}

		/// <summary>
		/// Replaces unsafe characters with C# escape sequences.
		/// If the string contains these characters, replaces and returns new string. Else returns this string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="limit">If the final string is longer than <i>limit</i>, get its substring 0 to <i>limit</i>-1 with appended '…' character. The enclosing "" are not counted.</param>
		/// <param name="quote">Enclose in "".</param>
		/// <remarks>
		/// Replaces these characters: <c>'\\'</c>, <c>'\"'</c>, <c>'\t'</c>, <c>'\n'</c>, <c>'\r'</c> and all in range 0-31.
		/// </remarks>
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
			using(new LibStringBuilder(out var b, len + len / 16 + 100)) {
				if(quote) b.Append('\"');
				for(i = 0; i < len; i++) {
					var c = t[i];
					if(c < ' ') {
						switch(c) {
						case '\t': b.Append("\\t"); break;
						case '\n': b.Append("\\n"); break;
						case '\r': b.Append("\\r"); break;
						case '\0': b.Append("\\0"); break;
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

		/// <summary>
		/// Replaces C# escape sequences to characters in this string.
		/// Returns true if successful. Returns false if the string contains an invalid or unsupported escape sequence.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="result">Receives the result string. It is this string if there are no escape sequences or if fails.</param>
		/// <remarks>
		/// Supports all escape sequences of <see cref="Escape"/>: \\ \" \t \n \r \0 \uXXXX.
		/// Does not support \a \b \f \v \' \xXXXX \UXXXXXXXX.
		/// </remarks>
		public static bool Unescape(this string t, out string result)
		{
			result = t;
			int i = t.IndexOf('\\');
			if(i < 0) return true;

			using(new LibStringBuilder(out var b, t.Length)) {
				b.Append(t, 0, i);

				for(; i < t.Length; i++) {
					char c = t[i];
					if(c == '\\') {
						if(++i == t.Length) return false;
						switch(c = t[i]) {
						case '\\': case '\"': break;
						case 't': c = '\t'; break;
						case 'n': c = '\n'; break;
						case 'r': c = '\r'; break;
						case '0': c = '\0'; break;
						//case 'a': c = '\a'; break;
						//case 'b': c = '\b'; break;
						//case 'f': c = '\f'; break;
						//case 'v': c = '\v'; break;
						//case '\'': break;
						//also we don't support U and x
						case 'u':
							if(!_Uni(t, ++i, 4, out int u)) return false;
							c = (char)u;
							i += 3;
							break;
						default: return false;
						}
					}
					b.Append(c);
				}

				result = b.ToString();
				return true;
			}

			static bool _Uni(string t, int i, int maxLen, out int R)
			{
				R = 0;
				int to = i + maxLen; if(to > t.Length) return false;
				for(; i < to; i++) {
					int k = _CharHexToDec(t[i]); if(k < 0) return false;
					R = (R << 4) + k;
				}
				return true;
			}
		}

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
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="AExtString.ToInt"/> and similar functions.
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