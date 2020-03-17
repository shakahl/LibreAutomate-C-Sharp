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
//using System.Linq;
using System.Globalization;

using Au.Types;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Adds extension methods for <see cref="String"/>.
	/// </summary>
	/// <remarks>
	/// Some .NET <see cref="String"/> methods use <see cref="StringComparison.CurrentCulture"/> by default, while others use ordinal or invariant comparison. It is confusing (difficult to remember), dangerous (easy to make bugs), slower and rarely useful.
	/// Microsoft recommends to specify <b>StringComparison.Ordinal[IgnoreCase]</b> explicitly. See https://msdn.microsoft.com/en-us/library/ms973919.aspx.
	/// This class adds ordinal comparison versions of these methods. Same or similar name, for example <b>Ends</b> for <b>EndsWith</b>.
	/// This class also adds more methods.
	/// You also can find string functions in other classes of this library, including <see cref="AStringUtil"/>, <see cref="ARegex"/>, <see cref="AChar"/>, <see cref="APath"/>, <see cref="ACsv"/>, <see cref="AKeys.More"/>, <see cref="AConvert"/>, <see cref="AHash"/>.
	/// </remarks>
	public static unsafe partial class AExtString
	{
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
			return ignoreCase ? string.Equals(t, s, StringComparison.OrdinalIgnoreCase) : string.Equals(t, s);
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
		public static int Eq(this string t, bool ignoreCase, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(Eq(t, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Compares part of this string with other string. Returns true if equal.
		/// </summary>
		/// <param name="t">This string.</param>
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
			int n = s?.Length ?? throw new ArgumentNullException();
			if((uint)startIndex > t.Length || n > t.Length - startIndex) return false;
			var span = t.AsSpan(startIndex, n);
			if(!ignoreCase) return span.SequenceEqual(s);
			return span.Equals(s, StringComparison.OrdinalIgnoreCase);
			//Faster than string.Compare[Ordinal].
			//With Tables_.LowerCase similar speed. Depends on whether match. 
		}

		/// <summary>
		/// Compares part of this string with multiple strings.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <param name="t">This string.</param>
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

		/// <summary>
		/// Returns true if the specified character is at the specified position in this string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="index">Offset in this string. If invalid, returns false.</param>
		/// <param name="c">The character.</param>
		public static bool Eq(this string t, int index, char c)
		{
			if((uint)index >= t.Length) return false;
			return t[index] == c;
		}

		/// <summary>
		/// Compares this and other string ignoring case (case-insensitive). Returns true if equal.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="s">Other string. Can be null.</param>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static bool Eqi(this string t, string s) => string.Equals(t, s, StringComparison.OrdinalIgnoreCase);

		//rejected. Rarely used. Or also need Startsi, Endsi, Findi, etc.
		//public static bool Eqi(this string t, int startIndex, string s) => Eq(t, startIndex, s, true);

		/// <summary>
		/// Compares the end of this string with other string. Returns true if equal.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="s">Other string.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static bool Ends(this string t, string s, bool ignoreCase = false)
		{
			//return t?.EndsWith(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? false; //slower
			int n = s?.Length ?? throw new ArgumentNullException();
			if(n > t.Length) return false;
			var span = t.AsSpan(t.Length - n);
			if(!ignoreCase) return span.SequenceEqual(s);
			return span.Equals(s, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Compares the end of this string with multiple strings.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="strings">Other strings.</param>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Ends(this string t, bool ignoreCase, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(Ends(t, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string ends with the specified character.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="c">The character.</param>
		public static bool Ends(this string t, char c)
		{
			int i = t.Length - 1;
			return i >= 0 && t[i] == c;
		}

		/// <summary>
		/// Compares the beginning of this string with other string. Returns true if equal.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="s">Other string.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static bool Starts(this string t, string s, bool ignoreCase = false)
		{
			//return t?.StartsWith(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) ?? false; //slower
			int n = s?.Length ?? throw new ArgumentNullException();
			if(n > t.Length) return false;
			var span = t.AsSpan(0, n);
			if(!ignoreCase) return span.SequenceEqual(s);
			return span.Equals(s, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Compares the beginning of this string with multiple strings.
		/// Returns 1-based index of matching string, or 0 if none.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="strings">Other strings.</param>
		/// <exception cref="ArgumentNullException">A string in <i>strings</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Starts(this string t, bool ignoreCase, params string[] strings)
		{
			for(int i = 0; i < strings.Length; i++) if(Starts(t, strings[i], ignoreCase)) return i + 1;
			return 0;
		}

		/// <summary>
		/// Returns true if this string starts with the specified character.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="c">The character.</param>
		public static bool Starts(this string t, char c)
		{
			return t.Length > 0 && t[0] == c;
		}

		/// <summary>
		/// Finds substring in this string. Returns its 0-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Find(this string t, string s, bool ignoreCase = false)
		{
			return t.IndexOf(s, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}
		//FUTURE: if ignoreCase, maybe use our case table for Find, Eq, etc. Because now 12 times slower. But Find(ignoreCase: true) where speed is very important are rare.

		/// <summary>
		/// Finds substring in part of this string. Returns its 0-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="startIndex">The search start index.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i>.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Find(this string t, string s, int startIndex, bool ignoreCase = false)
		{
			return t.IndexOf(s, startIndex, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Finds substring in part of this string. Returns its 0-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="range">The search range.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <remarks>
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int Find(this string t, string s, Range range, bool ignoreCase = false)
		{
			var (start, count) = range.GetOffsetAndLength(t.Length);
			return t.IndexOf(s, start, count, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		/// <summary>
		/// Finds the first character specified in <i>chars</i>. Returns its index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters.</param>
		/// <param name="startOfRange">The start index of the search range. Default 0.</param>
		/// <param name="endOfRange">The end index of the search range. If -1 (default), the length of this string.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startOfRange</i> or <i>endOfRange</i>.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static int FindAny(this string t, string chars, int startOfRange = 0, int endOfRange = -1)
		{
			int len = (endOfRange >= 0 ? endOfRange : t.Length) - startOfRange;
			int r = t.AsSpan(startOfRange, len).IndexOfAny(chars);
			return r < 0 ? r : r + startOfRange;
		}

		//Span does not have a fast 'not' function. Can be used TrimStart, but much slower.
		//tested: > 2 times slower with Range instead of startOfRange/endOfRange.

		/// <summary>
		/// Finds the first character not specified in <i>chars</i>. Returns its index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters.</param>
		/// <param name="startOfRange">The start index of the search range. Default 0.</param>
		/// <param name="endOfRange">The end index of the search range. If -1 (default), the length of this string.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startOfRange</i> or <i>endOfRange</i>.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static int FindNot(this string t, string chars, int startOfRange = 0, int endOfRange = -1)
		{
			int len = (endOfRange >= 0 ? endOfRange : t.Length) - startOfRange;
			int r = t.AsSpan(startOfRange, len).IndexOfNot(chars);
			return r < 0 ? r : r + startOfRange;
		}

		/// <summary>
		/// Finds the first character not specified in <i>chars</i>. Returns its index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static int IndexOfNot(this ReadOnlySpan<char> t, string chars)
		{
			if(chars == null) throw new ArgumentNullException();
			for(int i = 0; i < t.Length; i++) {
				char c = t[i];
				for(int j = 0; j < chars.Length; j++) if(chars[j] == c) goto g1;
				return i;
				g1:;
			}
			return -1;
		}

		/// <summary>
		/// Finds the last character not specified in <i>chars</i>. Returns its index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static int LastIndexOfNot(this ReadOnlySpan<char> t, string chars)
		{
			if(chars == null) throw new ArgumentNullException();
			for(int i = t.Length; --i >= 0;) {
				char c = t[i];
				for(int j = 0; j < chars.Length; j++) if(chars[j] == c) goto g1;
				return i;
				g1:;
			}
			return -1;
		}

		/// <summary>
		/// Finds the last character specified in <i>chars</i> (searches right to left). Returns its index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters.</param>
		/// <param name="startOfRange">The start index of the search range. Default 0.</param>
		/// <param name="endOfRange">The end index of the search range. If -1 (default), the length of this string.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startOfRange</i> or <i>endOfRange</i>.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static int FindLastAny(this string t, string chars, int startOfRange = 0, int endOfRange = -1)
		{
			int len = (endOfRange >= 0 ? endOfRange : t.Length) - startOfRange;
			int r = t.AsSpan(startOfRange, len).LastIndexOfAny(chars);
			return r < 0 ? r : r + startOfRange;
		}

		/// <summary>
		/// Finds the last character not specified in <i>chars</i> (searches right to left). Returns its index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters.</param>
		/// <param name="startOfRange">The start index of the search range. Default 0.</param>
		/// <param name="endOfRange">The end index of the search range. If -1 (default), the length of this string.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startOfRange</i> or <i>endOfRange</i>.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static int FindLastNot(this string t, string chars, int startOfRange = 0, int endOfRange = -1)
		{
			int len = (endOfRange >= 0 ? endOfRange : t.Length) - startOfRange;
			int r = t.AsSpan(startOfRange, len).LastIndexOfNot(chars);
			return r < 0 ? r : r + startOfRange;
		}

		/// <summary>
		/// Removes specified characters from the start and end of this string.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters to remove.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static string Trim(this string t, string chars)
		{
			var span = t.AsSpan().Trim(chars);
			return span.Length == t.Length ? t : new string(span);
		}

		/// <summary>
		/// Removes specified characters from the start of this string.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters to remove.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static string TrimStart(this string t, string chars)
		{
			var span = t.AsSpan().TrimStart(chars);
			return span.Length == t.Length ? t : new string(span);
		}

		/// <summary>
		/// Removes specified characters from the end of this string.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="chars">Characters to remove.</param>
		/// <exception cref="ArgumentNullException"><i>chars</i> is null.</exception>
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
		public static string TrimEnd(this string t, string chars)
		{
			var span = t.AsSpan().TrimEnd(chars);
			return span.Length == t.Length ? t : new string(span);
		}

		/// <summary>
		/// Finds whole word. Returns its 0-based index, or -1 if not found.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="s">Subtring to find.</param>
		/// <param name="range">The search range.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		/// <param name="otherWordChars">Additional word characters, for which <see cref="Char.IsLetterOrDigit"/> returns false. For example "_".</param>
		/// <exception cref="ArgumentNullException"><i>s</i> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <remarks>
		/// If <i>s</i> starts with a word character finds substring that is not preceded by a word character.
		/// If <i>s</i> ends with a word character, finds substring that is not followed by a word character.
		/// Word characters are those for which <see cref="Char.IsLetterOrDigit"/> returns true plus those specified in <i>otherWordChars</i>.
		/// Uses ordinal comparison (does not depend on current culture/locale).
		/// </remarks>
		public static int FindWord(this string t, string s, Range? range = null, bool ignoreCase = false, string otherWordChars = null)
		{
			var (start, end) = range?.GetStartEnd(t.Length) ?? (0, t.Length);
			int lens = s?.Length ?? throw new ArgumentNullException();
			if(lens == 0) return 0; //like IndexOf and Find

			bool wordStart = _IsWordChar(s, 0, false, otherWordChars),
				wordEnd = _IsWordChar(s, lens - 1, true, otherWordChars);

			for(int i = start, iMax = end - lens; i <= iMax; i++) {
				i = t.IndexOf(s, i, end - i, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
				if(i < 0) break;
				if(wordStart && i > 0 && _IsWordChar(t, i - 1, true, otherWordChars)) continue;
				if(wordEnd && i < iMax && _IsWordChar(t, i + lens, false, otherWordChars)) continue;
				return i;
			}
			return -1;

			static bool _IsWordChar(string s, int i, bool expandLeft, string otherWordChars)
			{
				//SHOULDDO: use Rune
				char c = s[i];
				if(c >= '\uD800' && c <= '\uDFFF') { //Unicode surrogates
					if(expandLeft) {
						if(Char.IsLowSurrogate(s[i])) return i > 0 && Char.IsHighSurrogate(s[i - 1]) && Char.IsLetterOrDigit(s, i - 1);
					} else {
						if(Char.IsHighSurrogate(s[i])) return i < s.Length - 1 && Char.IsLowSurrogate(s[i + 1]) && Char.IsLetterOrDigit(s, i);
					}
				} else {
					if(Char.IsLetterOrDigit(c)) return true;
					if(otherWordChars?.Contains(c) ?? false) return true;
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
		/// Returns true if this string is null or empty ("").
		/// </summary>
		/// <param name="t">This string.</param>
		public static bool NE(this string t) => t==null ? true : t.Length == 0;

		//rejected. Too simple. Not so often used. Could name AsSpan, then in completion lists it is joined with the .NET extension method, but then in our editor it is the first in the list.
		///// <summary>
		///// Creates a new read-only span of this string using tuple (int start, int end).
		///// Can be used with <see cref="Segments"/>, which returns such tuples in foreach.
		///// </summary>
		//public static ReadOnlySpan<char> SegAsSpan(this string t, (int start, int end) se) => t.AsSpan(se.start, se.end - se.start);
		///// <seealso cref="SegAsSpan"/>

		/// <summary>
		/// This function can be used with foreach to split this string into substrings as start/end offsets.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="separators">Characters that delimit the substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="flags"></param>
		/// <param name="range">Part of this string to split.</param>
		/// <example>
		/// <code><![CDATA[
		/// string s = "one * two three ";
		/// foreach(var t in s.Segments(" ")) AOutput.Write(t);
		/// foreach(var t in s.Segments(SegSep.Word, SegFlags.NoEmpty)) AOutput.Write(t);
		/// ]]></code>
		/// </example>
		/// <seealso cref="SegSplit"/>
		/// <seealso cref="SegLines"/>
		public static SegParser Segments(this string t, string separators, SegFlags flags = 0, Range? range = null)
		{
			return new SegParser(t, separators, flags, range);
		}

		/// <summary>
		/// Splits this string into substrings using the specified separators.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="maxCount">The maximal number of substrings to get. If negative, gets all. Else if there are more substrings, the last element will contain single substring, unlike with <see cref="String.Split"/>.</param>
		/// <param name="flags"></param>
		/// <param name="range">Part of this string to split.</param>
		/// <seealso cref="Segments"/>
		/// <seealso cref="SegLines"/>
		/// <seealso cref="SegParser"/>
		/// <seealso cref="string.Split"/>
		/// <seealso cref="string.Join"/>
		public static string[] SegSplit(this string t, string separators, SegFlags flags = 0, int maxCount = -1, Range? range = null)
		{
			var x = new SegParser(t, separators, flags, range);
			return x.ToStringArray(maxCount);
		}

		/// <summary>
		/// Splits this string into lines using separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="noEmptyLines">Don't need empty lines.</param>
		/// <param name="maxCount">The maximal number of substrings to get. If negative, gets all. Else if there are more lines, the last element will contain single line, unlike with <see cref="String.Split"/></param>
		/// <param name="range">Part of this string to split.</param>
		/// <seealso cref="Segments"/>
		/// <seealso cref="SegSep.Line"/>
		public static string[] SegLines(this string t, bool noEmptyLines = false, int maxCount = -1, Range? range = null)
		{
			return SegSplit(t, SegSep.Line, noEmptyLines ? SegFlags.NoEmpty : 0, maxCount, range);
		}

		/// <summary>
		/// Returns the number of lines.
		/// Counts line separators "\r\n", "\n", "\r".
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="preferMore">Add 1 if the string ends with a line separator or its length is 0.</param>
		/// <seealso cref="AStringUtil.LineAndColumn"/>
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
		/// Converts this string to lower case.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <remarks>
		/// Calls <see cref="string.ToLowerInvariant"/>.
		/// </remarks>
		public static string Lower(this string t) => t.ToLowerInvariant();

		/// <summary>
		/// Converts this string to upper case.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <remarks>
		/// Calls <see cref="string.ToUpperInvariant"/>.
		/// </remarks>
		public static string Upper(this string t) => t.ToUpperInvariant();

		/// <summary>
		/// Converts this string or only the first character to upper case or all words to title case.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="how"></param>
		/// <param name="culture">Culture, for example <c>CultureInfo.CurrentCulture</c>. If null (default) uses invariant culture.</param>
		public static unsafe string Upper(this string t, SUpper how, CultureInfo culture = null)
		{
			if(how == SUpper.FirstChar) {
				if(t.Length == 0 || !char.IsLower(t, 0)) return t;
				var r = Rune.GetRuneAt(t, 0);
				r = culture != null ? Rune.ToUpper(r, culture) : Rune.ToUpperInvariant(r);
				int n = r.IsBmp ? 1 : 2;
				var m = new Span<char>(&r, n);
				if(n == 2) r.EncodeToUtf16(m);
				return string.Concat(m, t.AsSpan(n));
			}
			var ti = (culture ?? CultureInfo.InvariantCulture).TextInfo;
			t = t ?? throw new NullReferenceException();
			if(how == SUpper.TitleCase) return ti.ToTitleCase(t);
			return ti.ToUpper(t);
		}

		#region ToNumber

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		static long _ToInt(string t, int startIndex, out int numberEndIndex, bool toLong, STIFlags flags)
		{
			numberEndIndex = 0;

			int len = t.Lenn();
			if((uint)startIndex > len) throw new ArgumentOutOfRangeException("startIndex");
			int i = startIndex;
			char c;

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

			//is hex?
			bool isHex = false;
			switch(flags & (STIFlags.NoHex | STIFlags.IsHexWithout0x)) {
			case 0:
				if(c == '0' && i <= len - 3) {
					c = t[++i];
					if(isHex = (c == 'x' || c == 'X')) i++; else i--;
				}
				break;
			case STIFlags.IsHexWithout0x:
				isHex = true;
				break;
			}

			//skip '0'
			int i0 = i;
			while(i < len && t[i] == '0') i++;

			long R = 0; //result

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static int _CharHexToDec(char c)
		{
			if(c >= '0' && c <= '9') return c - '0';
			if(c >= 'A' && c <= 'F') return c - ('A' - 10);
			if(c >= 'a' && c <= 'f') return c - ('a' - 10);
			return -1;
		}

		/// <summary>
		/// Converts part of this string to int number and gets the number end index.
		/// Returns the number, or 0 if fails to convert.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in this string where the number part ends. If fails to convert, receives 0.</param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		/// <remarks>
		/// Fails to convert when string is null, "", does not begin with a number or the number is too big.
		/// 
		/// Unlike <b>int.Parse</b> and <b>Convert.ToInt32</b>:
		///		The number in string can be followed by more text, like <c>"123text"</c>.
		///		Has <i>startIndex</i> parameter that allows to get number from middle, like <c>"text123text"</c>.
		///		Gets the end of the number part.
		///		No exception when cannot convert.
		///		The number can be decimal (like <c>"123"</c>) or hexadecimal (like <c>"0x1A"</c>); don't need separate flags for each style.
		///		Faster.
		///	
		/// The number in string can start with ASCII whitespace (spaces, newlines, etc), like <c>" 5"</c>.
		/// The number in string can be with <c>"-"</c> or <c>"+"</c>, like <c>"-5"</c>, but not like <c>"- 5"</c>.
		/// Fails if the number is greater than +- <b>uint.MaxValue</b> (0xffffffff).
		/// The return value becomes negative if the number is greater than <b>int.MaxValue</b>, for example <c>"0xffffffff"</c> is -1, but it becomes correct if assigned to uint (need cast).
		/// Does not support non-integer numbers; for example, for <c>"3.5E4"</c> returns 3 and sets <c>numberEndIndex=startIndex+1</c>.
		/// </remarks>
		public static int ToInt(this string t, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out numberEndIndex, false, flags);
		}

		/// <summary>
		/// Converts part of this string to int number.
		/// Returns the number, or 0 if fails to convert.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static int ToInt(this string t, int startIndex = 0, STIFlags flags = 0)
		{
			return (int)_ToInt(t, startIndex, out _, false, flags);
		}

		/// <summary>
		/// Converts part of this string to int number and gets the number end index.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in this string where the number part ends. If fails to convert, receives 0.</param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out int result, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			result = (int)_ToInt(t, startIndex, out numberEndIndex, false, flags);
			return numberEndIndex != 0;
		}

#pragma warning disable CS3006 // Overloaded method differing only in ref or out, or in array rank, is not CLS-compliant
		/// <summary>
		/// Converts part of this string to int number.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out int result, int startIndex = 0, STIFlags flags = 0)
			=> ToInt(t, out result, startIndex, out _, flags);
#pragma warning restore CS3006 // Overloaded method differing only in ref or out, or in array rank, is not CLS-compliant

		/// <summary>
		/// Converts part of this string to uint number and gets the number end index.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out uint result, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			result = (uint)_ToInt(t, startIndex, out numberEndIndex, false, flags);
			return numberEndIndex != 0;
		}

		/// <summary>
		/// Converts part of this string to uint number.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out uint result, int startIndex = 0, STIFlags flags = 0)
			=> ToInt(t, out result, startIndex, out _, flags);

		/// <summary>
		/// Converts part of this string to long number and gets the number end index.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out long result, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			result = _ToInt(t, startIndex, out numberEndIndex, true, flags);
			return numberEndIndex != 0;
		}

		/// <summary>
		/// Converts part of this string to long number.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out long result, int startIndex = 0, STIFlags flags = 0)
			=> ToInt(t, out result, startIndex, out _, flags);

		/// <summary>
		/// Converts part of this string to ulong number and gets the number end index.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out ulong result, int startIndex, out int numberEndIndex, STIFlags flags = 0)
		{
			result = (ulong)_ToInt(t, startIndex, out numberEndIndex, true, flags);
			return numberEndIndex != 0;
		}

		/// <summary>
		/// Converts part of this string to ulong number.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToInt(this string t, out ulong result, int startIndex = 0, STIFlags flags = 0)
			=> ToInt(t, out result, startIndex, out _, flags);

#if true
		/// <summary>
		/// Converts this string or its part to double number.
		/// Returns the number, or 0 if fails to convert.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="range">Part of this string or null (default).</param>
		/// <param name="style">The permitted number format in the string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <exception cref="ArgumentException">Invalid <i>style</i>.</exception>
		/// <remarks>
		/// Calls <see cref="double.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out double)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b>.
		/// Fails if the string is null or "" or isn't a valid floating-point number.
		/// Examples of valid numbers: "12", " -12.3 ", ".12", "12.", "12E3", "12.3e-45", "1,234.5" (with style <c>NumberStyles.Float | NumberStyles.AllowThousands</c>). String like "2text" is invalid, unless range is <c>0..1</c>.
		/// </remarks>
		public static double ToNumber(this string t, Range? range = null, NumberStyles style = NumberStyles.Float)
		{
			ToNumber(t, out double r, range, style);
			return r;
		}

		/// <summary>
		/// Converts this string or its part to double number.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="range">Part of this string or null (default).</param>
		/// <param name="style">The permitted number format in the string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <exception cref="ArgumentException">Invalid <i>style</i>.</exception>
		/// <remarks>
		/// Calls <see cref="double.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out double)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b>.
		/// </remarks>
		public static bool ToNumber(this string t, out double result, Range? range = null, NumberStyles style = NumberStyles.Float)
		{
			return double.TryParse(_NumSpan(t, range), style, CultureInfo.InvariantCulture, out result);
		}

		/// <summary>
		/// Converts this string or its part to float number.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="range">Part of this string or null (default).</param>
		/// <param name="style">The permitted number format in the string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <exception cref="ArgumentException">Invalid <i>style</i>.</exception>
		/// <remarks>
		/// Calls <see cref="float.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out float)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b>.
		/// </remarks>
		public static bool ToNumber(this string t, out float result, Range? range = null, NumberStyles style = NumberStyles.Float)
		{
			return float.TryParse(_NumSpan(t, range), style, CultureInfo.InvariantCulture, out result);
		}

		/// <summary>
		/// Converts this string or its part to int number.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="range">Part of this string or null (default).</param>
		/// <param name="style">The permitted number format in the string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <exception cref="ArgumentException">Invalid <i>style</i>.</exception>
		/// <remarks>
		/// Calls <see cref="int.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out int)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b>.
		/// </remarks>
		public static bool ToNumber(this string t, out int result, Range? range = null, NumberStyles style = NumberStyles.Integer)
		{
			return int.TryParse(_NumSpan(t, range), style, CultureInfo.InvariantCulture, out result);

			//note: exception if NumberStyles.Integer | NumberStyles.AllowHexSpecifier.
			//	Can parse either decimal or hex, not any.
			//	Does not support "0x". With AllowHexSpecifier eg "11" is 17, but "0x11" is invalid.
		}

		/// <summary>
		/// Converts this string or its part to uint number.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="range">Part of this string or null (default).</param>
		/// <param name="style">The permitted number format in the string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <exception cref="ArgumentException">Invalid <i>style</i>.</exception>
		/// <remarks>
		/// Calls <see cref="uint.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out uint)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b>.
		/// </remarks>
		public static bool ToNumber(this string t, out uint result, Range? range = null, NumberStyles style = NumberStyles.Integer)
		{
			return uint.TryParse(_NumSpan(t, range), style, CultureInfo.InvariantCulture, out result);
		}

		/// <summary>
		/// Converts this string or its part to long number.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="range">Part of this string or null (default).</param>
		/// <param name="style">The permitted number format in the string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <exception cref="ArgumentException">Invalid <i>style</i>.</exception>
		/// <remarks>
		/// Calls <see cref="long.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out long)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b>.
		/// </remarks>
		public static bool ToNumber(this string t, out long result, Range? range = null, NumberStyles style = NumberStyles.Integer)
		{
			return long.TryParse(_NumSpan(t, range), style, CultureInfo.InvariantCulture, out result);
		}

		/// <summary>
		/// Converts this string or its part to ulong number.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="range">Part of this string or null (default).</param>
		/// <param name="style">The permitted number format in the string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>range</i>.</exception>
		/// <exception cref="ArgumentException">Invalid <i>style</i>.</exception>
		/// <remarks>
		/// Calls <see cref="ulong.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out ulong)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b>.
		/// </remarks>
		public static bool ToNumber(this string t, out ulong result, Range? range = null, NumberStyles style = NumberStyles.Integer)
		{
			return ulong.TryParse(_NumSpan(t, range), style, CultureInfo.InvariantCulture, out result);
		}

		static ReadOnlySpan<char> _NumSpan(string t, Range? range)
		{
			if(t == null) return default;
			var (start, len) = range?.GetOffsetAndLength(t.Length) ?? (0, t.Length);
			return t.AsSpan(start, len);
		}
#else
		/// <summary>
		/// Converts part of this string to double number and gets the number end index.
		/// Returns the number, or 0 if fails to convert.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in this string where the number part ends. If fails to convert, receives 0.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		/// <remarks>
		/// Calls <see cref="double.TryParse(ReadOnlySpan{char}, NumberStyles, IFormatProvider, out double)"/> with <see cref="CultureInfo"/> <b>InvariantCulture</b> and <see cref="NumberStyles"/> <b>Float</b> and <b>AllowThousands</b>.
		/// Fails if this string is null or "" or does not contain a valid floating-point number at the specified start index.
		/// Examples of valid numbers: "12", " -12.3", ".12", "12.", "1,234.5", "12E3", "12.3e-45". String like "89text" is valid too; then <i>numberEndIndex</i> recives 2. String "text89text" is valid if <i>startIndex</i> is 4 or 5; then <i>numberEndIndex</i> receives 6.
		/// </remarks>
		public static double ToNumber(this string t, int startIndex, out int numberEndIndex)
		{
			ToNumber(t, out double r, startIndex, out numberEndIndex);
			return r;
		}

		/// <summary>
		/// Converts part of this string to double number.
		/// Returns the number, or 0 if fails to convert.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static double ToNumber(this string t, int startIndex = 0)
		{
			ToNumber(t, out double r, startIndex, out _);
			return r;
		}

		/// <summary>
		/// Converts part of this string to double number and gets the number end index.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in this string where the number part ends. If fails to convert, receives 0.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToNumber(this string t, out double result, int startIndex, out int numberEndIndex)
		{
			if(_ScanFloatNumber(t, startIndex, out numberEndIndex)) {
				var span = t.AsSpan(startIndex, numberEndIndex - startIndex);
				if(double.TryParse(span, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)) return true;
				ADebug.Print("TryParse");
			}
			result = 0.0;
			numberEndIndex = 0;
			return false;
		}

		/// <summary>
		/// Converts part of this string to double number.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToNumber(this string t, out double result, int startIndex = 0)
			=> ToNumber(t, out result, startIndex, out _);

		/// <summary>
		/// Converts part of this string to float number and gets the number end index.
		/// Returns false if fails.
		/// </summary>
		/// <param name="t">This string. Can be null.</param>
		/// <param name="result">Receives the result number.</param>
		/// <param name="startIndex">Offset in this string where to start parsing.</param>
		/// <param name="numberEndIndex">Receives offset in this string where the number part ends. If fails to convert, receives 0.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToNumber(this string t, out float result, int startIndex, out int numberEndIndex)
		{
			if(_ScanFloatNumber(t, startIndex, out numberEndIndex)) {
				var span = t.AsSpan(startIndex, numberEndIndex - startIndex);
				if(float.TryParse(span, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result)) return true;
				ADebug.Print("TryParse");
			}
			result = 0f;
			numberEndIndex = 0;
			return false;
		}

		/// <summary>
		/// Converts part of this string to float number.
		/// Returns false if fails.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>startIndex</i> is less than 0 or greater than string length.</exception>
		public static bool ToNumber(this string t, out float result, int startIndex = 0)
			=> ToNumber(t, out result, startIndex, out _);

		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		static bool _ScanFloatNumber(string t, int i, out int end)
		{
			end = 0;

			int len = t.Lenn();
			if((uint)i > len) throw new ArgumentOutOfRangeException("startIndex");

			while(i < len && _IsWhite(t[i])) i++;
			if(i < len) switch(t[i]) { case '-': case '+': i++; break; }
			if(i == len) return false;

			bool isDot;
			if(_IsDigit(t[i])) {
				for(++i; i < len && (_IsDigit(t[i]) || t[i] == ',');) i++;
				if(isDot = (i < len && t[i] == '.')) i++;
			} else if(t[i] == '.' && ++i < len && _IsDigit(t[i])) {
				i++;
				isDot = true;
			} else return false;

			if(isDot) {
				while(i < len && _IsDigit(t[i])) i++;
			}

			if(i < len && (t[i] == 'E' || t[i] == 'e')) {
				if(++i < len) switch(t[i]) { case '-': case '+': i++; break; }
				int i0 = i; while(i < len && _IsDigit(t[i])) i++;
				if(i == i0) return false;
			}

			end = i;
			return true;

			//from .NET Core source
			static bool _IsWhite(int ch) => ch == 0x20 || (uint)(ch - 0x09) <= (0x0D - 0x09) ? true : false;
			static bool _IsDigit(int ch) => ((uint)ch - '0') <= 9;
		}
		//static ARegex s_rxNum = new ARegex(@"\s*[-+]?(?:\d[\d,]*\.?\d*|\.\d+)(?:[Ee][-+]?\d+)?", RXFlags.ANCHORED); //with regex 5 times slower
#endif

		#endregion

		/// <summary>
		/// Replaces part of this string with other string.
		/// Returns the result string.
		/// </summary>
		/// <param name="t">This string.</param>
		/// <param name="startIndex">Offset in this string.</param>
		/// <param name="count">Count of characters to replace.</param>
		/// <param name="s">The replacement string.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid <i>startIndex</i> or <i>count</i>.</exception>
		public static string ReplaceAt(this string t, int startIndex, int count, string s)
		{
			return string.Concat(t.AsSpan(0, startIndex), s, t.AsSpan(startIndex + count));
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
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
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
			using(new StringBuilder_(out var b, len + len / 16 + 100)) {
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
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static bool Unescape(this string t, out string result)
		{
			result = t;
			int i = t.IndexOf('\\');
			if(i < 0) return true;

			using(new StringBuilder_(out var b, t.Length)) {
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
		//public static string SqlEscape(this string t) => t.Replace("'", "''");

		//rejected. Rarely used.
		///// <summary>
		///// Converts this string to '\0'-terminated char[].
		///// </summary>
		//public static char[] ToArrayAnd0(this string t)
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
		//public static byte[] ToUtf8And0(this string t) => AConvert.Utf8FromString(t);
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
		/// Fail if string starts with a whitespace character.
		/// </summary>
		DontSkipSpaces = 4,
	}

	/// <summary>
	/// Used with <see cref="AExtString.Upper(string, SUpper, CultureInfo)"/>
	/// </summary>
	public enum SUpper
	{
		/// <summary>
		/// Convert all characters to upper case.
		/// </summary>
		AllChars,

		/// <summary>
		/// Convert only the first character to upper case.
		/// </summary>
		FirstChar,

		/// <summary>
		/// Convert the first character of each word to upper case and other characters to lower case.
		/// Calls <see cref="TextInfo.ToTitleCase"/>.
		/// </summary>
		TitleCase,
	}
}