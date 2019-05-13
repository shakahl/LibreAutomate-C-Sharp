//Modified version of Microsoft.Extensions.Primitives.StringSegment. It is from github; current .NET does not have it.
//Can be used instead of String.Split, especially when you want less garbage. Faster (the github version with StringTokenizer was slower).

using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	//FUTURE: test more. Now not all functions tested. But wait, maybe new C# will have native slices etc.

	/// <summary>
	/// An optimized representation of a substring (any part of a string).
	/// </summary>
	/// <remarks>
	/// The main properties are: <see cref="Buffer"/> - the string containing the substring; <see cref="Offset"/> - the start index of the substring in the string; <see cref="Length"/> substring length. There is no string object for the substring itself; however <see cref="Value"/> and some other functions allocate and return a new string object.
	/// 
	/// One of ways to create <b>StringSegment</b> instances is to split a string with <see cref="ExtString.Segments(string, string, SegFlags)"/>.
	/// 
	/// Don't use the default constructor (parameterless). Then <b>Buffer</b> is null and the behavior of functions is undefined. Other constructors throw exception if the string is null.
	/// 
	/// Functions throw <b>ArgumentOutOfRangeException</b>, <b>ArgumentNullException</b>, <b>NullReferenceException</b> or <b>IndexOutOfRangeException</b> exception when an argument is invalid.
	/// </remarks>
	public struct StringSegment :IEquatable<StringSegment>, IEquatable<string>
	{
		string _buffer;
		int _offset, _length;

		/// <summary>
		/// Sets the substring = whole <i>buffer</i>.
		/// </summary>
		/// <param name="buffer">The string that contains this substring. Cannot be null.</param>
		public StringSegment(string buffer)
		{
			_buffer = buffer;
			_offset = 0;
			_length = buffer.Length; //NullReferenceException
		}

		/// <summary>
		/// Sets the substring = part of <i>buffer</i> that starts at index <i>offset</i> and has length <i>length</i>.
		/// </summary>
		/// <param name="buffer">The string that contains this substring. Cannot be null.</param>
		/// <param name="offset">The offset of the substring in <i>buffer</i>.</param>
		/// <param name="length">The length of the substring.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StringSegment(string buffer, int offset, int length)
		{
			if((uint)offset > buffer.Length || (uint)length > buffer.Length - offset) throw new ArgumentOutOfRangeException(); //and NullReferenceException if buffer null

			_buffer = buffer;
			_offset = offset;
			_length = length;
		}

		/// <summary>
		/// Sets the substring = part of <i>buffer</i> that starts at index <i>offset</i>.
		/// </summary>
		/// <param name="buffer">The string that contains this substring. Cannot be null.</param>
		/// <param name="offset">The offset of the substring in <i>buffer</i>.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StringSegment(string buffer, int offset) : this(buffer, offset, buffer.Length - offset) { }

		/// <summary>
		/// Gets the string that contains this substring.
		/// </summary>
		public string Buffer => _buffer;

		/// <summary>
		/// Gets or sets the start index of this substring within <see cref="Buffer"/>.
		/// The setter also changes <see cref="Length"/>, but not <see cref="EndOffset"/>.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The setter throws if value is less than 0 or greater than <see cref="EndOffset"/>.</exception>
		public int Offset
		{
			get => _offset;
			set
			{
				var e = _offset + _length;
				if((uint)value > e) throw new ArgumentOutOfRangeException();
				_length = e - value; _offset = value;
			}
		}

		/// <summary>
		/// Increments <see cref="Offset"/>.
		/// Also changes <see cref="Length"/>, but not <see cref="EndOffset"/>.
		/// </summary>
		/// <param name="n">How much to increment. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="Offset"/> would be less than 0 or greater than <see cref="EndOffset"/>.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Offset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void OffsetAdd(int n) => Offset = _offset + n;

		/// <summary>
		/// Decrements <see cref="Offset"/>.
		/// Also changes <see cref="Length"/>, but not <see cref="EndOffset"/>.
		/// </summary>
		/// <param name="n">How much to decrement. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="Offset"/> would be less than 0 or greater than <see cref="EndOffset"/>.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Offset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void OffsetSub(int n) => Offset = _offset - n;

		/// <summary>
		/// Changes <see cref="Offset"/>.
		/// Also changes <see cref="Length"/>, but not <see cref="EndOffset"/>.
		/// </summary>
		/// <param name="n">New value.</param>
		/// <exception cref="ArgumentOutOfRangeException">n is less than 0 or greater than <see cref="EndOffset"/>.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Offset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void OffsetSet(int n) => Offset = n;

		/// <summary>
		/// Gets or sets the length of the substring.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The setter throws if value is less than 0 or new <see cref="EndOffset"/> would be greater than buffer length.</exception>
		public int Length
		{
			get => _length;
			set
			{
				if(value < 0 || _offset + value > _buffer.Length) throw new ArgumentOutOfRangeException();
				_length = value;
			}
		}

		/// <summary>
		/// Increments <see cref="Length"/>.
		/// </summary>
		/// <param name="n">How much to increment. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="Length"/> would be less than 0 or new <see cref="EndOffset"/> would be greater than buffer length.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Length"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void LengthAdd(int n) => Length = _length + n;

		/// <summary>
		/// Decrements <see cref="Length"/>.
		/// </summary>
		/// <param name="n">How much to decrement. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="Length"/> would be less than 0 or new <see cref="EndOffset"/> would be greater than buffer length.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Length"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void LengthSub(int n) => Length = _length - n;

		/// <summary>
		/// Changes <see cref="Length"/>.
		/// </summary>
		/// <param name="n">New value.</param>
		/// <exception cref="ArgumentOutOfRangeException">n is less than 0 or new <see cref="EndOffset"/> would be greater than buffer length.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Length"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void LengthSet(int n) => Length = n;

		/// <summary>
		/// Gets or sets the end index of this substring within <see cref="Buffer"/>. It's <see cref="Offset"/> + <see cref="Length"/>.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">The setter throws if value is less than <see cref="Offset"/> or greater than buffer length.</exception>
		public int EndOffset
		{
			get => _offset + _length;
			set
			{
				int len = value - _offset;
				if(len < 0 || value > _buffer.Length) throw new ArgumentOutOfRangeException();
				_length = len;
			}
		}

		/// <summary>
		/// Increments <see cref="EndOffset"/>.
		/// </summary>
		/// <param name="n">How much to increment. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="EndOffset"/> would be less than <see cref="Offset"/> or greater than buffer length.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="EndOffset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void EndAdd(int n) => EndOffset += n;

		/// <summary>
		/// Decrements <see cref="EndOffset"/>.
		/// </summary>
		/// <param name="n">How much to decrement. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="EndOffset"/> would be less than <see cref="Offset"/> or greater than buffer length.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="EndOffset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void EndSub(int n) => EndOffset -= n;

		/// <summary>
		/// Changes <see cref="EndOffset"/>.
		/// </summary>
		/// <param name="n">New value.</param>
		/// <exception cref="ArgumentOutOfRangeException">n is less than <see cref="Offset"/> or greater than buffer length.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="EndOffset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void EndSet(int n) => EndOffset = n;

		/// <summary>
		/// Returns true if <b>Length</b> == 0.
		/// </summary>
		public bool IsEmpty() => _length == 0;

		/// <summary>
		/// Gets the value of this substring as a new string.
		/// </summary>
		/// <remarks>
		/// <note>Always creates new string object (substring of <see cref="Buffer"/>). See also <see cref="ValueCached"/>.</note>
		/// </remarks>
		public string Value => _buffer.Substring(_offset, _length);

		/// <summary>
		/// Gets the value of this substring as a cached string.
		/// </summary>
		/// <remarks>
		/// Can be used instead of <see cref="Value"/> to avoid much garbage when identical substring values are frequent.
		/// Uses a <see cref="Util.StringCache"/>.
		/// </remarks>
		public string ValueCached => Util.StringCache.LibAdd(_buffer, _offset, _length);

		/// <summary>
		/// Returns <see cref="Value"/>.
		/// </summary>
		public override string ToString() => Value;

		///
		public override int GetHashCode() => AConvert.HashFast(_buffer, _offset, _length);

		/// <summary>
		/// Gets the character at a specified position in this substring.
		/// </summary>
		/// <param name="index">Character index in this substring.</param>
		/// <exception cref="IndexOutOfRangeException">index is not in this substring, even if it is in <see cref="Buffer"/>.</exception>
		public char this[int index]
		{
			get
			{
				if((uint)index >= _length) throw new IndexOutOfRangeException();

				return _buffer[_offset + index]; //NullReferenceException, IndexOutOfRangeException
			}
		}

		/// <summary>
		/// Returns true if <i>obj</i> is <b>StringSegment</b> and its value is equal to that of this variable.
		/// Compares only substrigs, not offsets.
		/// </summary>
		public override bool Equals(object obj) => obj is StringSegment && _Equals((StringSegment)obj, false);

		/// <summary>
		/// Returns true if values of this and other variable are equal.
		/// Compares only substrigs, not offsets.
		/// </summary>
		/// <param name="other">A variable to compare with this variable.</param>
		public bool Equals(StringSegment other) => _Equals(other, false); //IEquatable<StringSegment>

		/// <summary>
		/// Returns true if values of this and other variable are equal, case insensitive.
		/// Compares only substrigs, not offsets.
		/// </summary>
		/// <param name="other">A variable to compare with this variable.</param>
		public bool Eqi(StringSegment other) => _Equals(other, true);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool _Equals(StringSegment s, bool ignoreCase)
		{
			int len = s.Length;
			if(len != _length) return false;
			return ExtString.LibEq(_buffer, _offset, s._buffer, s._offset, len, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool _Equals(string s, bool ignoreCase) => _buffer.Eq(_offset, s, ignoreCase);

		/// <summary>
		/// Returns true if the specified string is equal to this substring.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		public bool Equals(string text) => _Equals(text, false); //IEquatable<string>

		/// <summary>
		/// Returns true if the specified string is equal to this substring, case-insensitive.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		public bool Eqi(string text) => _Equals(text, true);

		/// <summary>
		/// Compares two substrings and returns true if their values are equal.
		/// Compares only substrigs, not offsets.
		/// </summary>
		public static bool operator ==(StringSegment left, StringSegment right) => left._Equals(right, false);

		/// <summary>
		/// Compares two substrings and returns true if their values are not equal.
		/// Compares only substrigs, not offsets.
		/// </summary>
		public static bool operator !=(StringSegment left, StringSegment right) => left._Equals(right, true);

		/// <summary>
		/// Creates a new <b>StringSegment</b> from the given string.
		/// </summary>
		/// <param name="value">The string. Cannot be null.</param>
		public static implicit operator StringSegment(string value) => new StringSegment(value);

		//public static explicit operator string(StringSegment seg) => seg.Value;

		/// <summary>
		/// Returns true if the beginning of this substring matches the specified string.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		public bool Starts(string text, bool ignoreCase = false)
		{
			if(_length < text.Length) return false; //NullReferenceException
			return _buffer.Eq(_offset, text, ignoreCase);
		}

		/// <summary>
		/// Returns true if the end of this substring matches the specified string.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		public bool Ends(string text, bool ignoreCase = false)
		{
			var len = text.Length; //NullReferenceException
			if(_length < len) return false;
			return _buffer.Eq(_offset + _length - len, text, ignoreCase);
		}

		/// <summary>
		/// Gets a substring of this substring, as string.
		/// </summary>
		/// <param name="offset">The start index in this substring.</param>
		/// <param name="cached">Use a <see cref="Util.StringCache"/>, to avoid much garbage when identical substring values are frequent.</param>
		public string Substring(int offset, bool cached = false)
		{
			return Substring(offset, _length - offset, cached);
		}

		/// <summary>
		/// Gets a substring of this substring, as string.
		/// </summary>
		/// <param name="offset">The start index in this substring.</param>
		/// <param name="length">The number of characters.</param>
		/// <param name="cached">Use a <see cref="Util.StringCache"/>, to avoid much garbage when identical substring values are frequent.</param>
		public string Substring(int offset, int length, bool cached = false)
		{
			if(offset < 0 || offset + length > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			offset += _offset;
			if(cached) return Util.StringCache.LibAdd(_buffer, offset, length);
			return _buffer.Substring(offset, length);
		}

		/// <summary>
		/// Gets a substring of this substring, as <b>StringSegment</b>.
		/// </summary>
		/// <param name="offset">The start index in this substring.</param>
		public StringSegment Subsegment(int offset)
		{
			return Subsegment(offset, _length - offset);
		}

		/// <summary>
		/// Gets a substring of this substring, as <b>StringSegment</b>.
		/// </summary>
		/// <param name="offset">The start index in this substring.</param>
		/// <param name="length">The number of characters.</param>
		public StringSegment Subsegment(int offset, int length)
		{
			if(offset < 0 || offset + length > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			return new StringSegment(_buffer, offset + _offset, length);
		}

#if false //rarely used. Use string functions for it.
		/// <summary>
		/// Finds a character. Returns its index in this substring, or -1 if not found.
		/// </summary>
		/// <param name="c">The character.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="count">The number of characters to examine.</param>
		public int IndexOf(char c, int startIndex, int count)
		{
			if(startIndex < 0 || startIndex + count > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			var index = _buffer.IndexOf(c, startIndex + _offset, count);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Finds a character. Returns its index in this substring, or -1 if not found.
		/// </summary>
		/// <param name="c">The character.</param>
		/// <param name="startIndex">The search starting position.</param>
		public int IndexOf(char c, int startIndex)
		{
			return IndexOf(c, startIndex, _length - startIndex);
		}

		/// <summary>
		/// Finds a character. Returns its index in this substring, or -1 if not found.eturns -1 if not found.
		/// </summary>
		/// <param name="c">The character.</param>
		public int IndexOf(char c)
		{
			var index = _buffer.IndexOf(c, _offset, _length);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Finds a character. Returns its index in this substring, or -1 if not found.
		/// Searches right-to-left.
		/// </summary>
		/// <param name="c">The character.</param>
		public int LastIndexOf(char c)
		{
			var index = _buffer.LastIndexOf(c, _offset + _length - 1, _length);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Finds the first occurence in this substring of any character in (or not in) a character set. Returns its index in this substring, or -1 if not found.
		/// </summary>
		/// <param name="chars">Characters to find.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="count">The number of character positions to examine.</param>
		/// <param name="not">Find character not specified in <i>chars</i>.</param>
		public int FindChars(string chars, int startIndex, int count, bool not = false)
		{
			if(startIndex < 0 || startIndex + count > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			int index = _buffer.FindChars(chars, _offset + startIndex, count, not);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Finds the first occurence in this substring of any character in (or not in) a character set. Returns its index in this substring, or -1 if not found.
		/// </summary>
		/// <param name="chars">Characters to find.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="not">Find character not specified in <i>chars</i>.</param>
		public int FindChars(string chars, int startIndex, bool not = false)
		{
			return FindChars(chars, startIndex, _length - startIndex, not);
		}

		/// <summary>
		/// Finds the first occurence in this substring of any character in (or not in) a character set. Returns its index in this substring, or -1 if not found.
		/// </summary>
		/// <param name="chars">Characters to find.</param>
		/// <param name="not">Find character not specified in <i>chars</i>.</param>
		public int FindChars(string chars, bool not = false)
		{
			int index = _buffer.FindChars(chars, _offset, _length, not);
			if(index >= 0) index -= _offset;
			return index;
		}
#endif

		/// <summary>
		/// Removes all leading and trailing whitespaces (see <see cref="char.IsWhiteSpace"/>).
		/// </summary>
		public void Trim()
		{
			TrimStart();
			TrimEnd();
		}

		/// <summary>
		/// Removes all leading whitespaces (see <see cref="char.IsWhiteSpace"/>).
		/// </summary>
		public void TrimStart()
		{
			string b = _buffer; int o = _offset, e = o + _length;
			while(o < e && char.IsWhiteSpace(b[o])) o++;
			_length = e - o; _offset = o;
		}

		/// <summary>
		/// Removes all trailing whitespaces (see <see cref="char.IsWhiteSpace(char)"/>).
		/// </summary>
		public void TrimEnd()
		{
			string b = _buffer; int o = _offset, e = o + _length;
			while(e > o && char.IsWhiteSpace(b[e - 1])) e--;
			_length = e - o;
		}

		/// <summary>
		/// Returns a <see cref="SegParser"/> that will split this substring into substrings as <b>StringSegment</b> variables when used with foreach.
		/// </summary>
		/// <param name="separators">Characters that delimit the substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="flags"></param>
		/// <seealso cref="ExtString.Segments(string, string, SegFlags)"/>
		public SegParser Split(string separators, SegFlags flags = 0)
		{
			return new SegParser(this, separators, flags);
		}
	}

	public static partial class ExtString
	{
		/// <summary>
		/// This function can be used with foreach to split this string into substrings as <see cref="StringSegment"/> variables.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="separators">Characters that delimit the substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="flags"></param>
		/// <example>
		/// <code><![CDATA[
		/// string s = "one * two three ";
		/// foreach(var t in s.Segments(" ")) Print(t);
		/// foreach(var t in s.Segments(SegSep.Word, SegFlags.NoEmpty)) Print(t);
		/// ]]></code>
		/// </example>
		public static SegParser Segments(this string t, string separators, SegFlags flags = 0)
		{
			return new SegParser(t, separators, flags);
		}

		/// <summary>
		/// This function can be used with foreach to split the specified part of this string into substrings as <see cref="StringSegment"/> variables.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="startIndex">The start of the part of this string.</param>
		/// <param name="length">The length of the part of this string.</param>
		/// <param name="separators">Characters that delimit the substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="flags"></param>
		public static SegParser Segments(this string t, int startIndex, int length, string separators, SegFlags flags = 0)
		{
			var seg = new StringSegment(t, startIndex, length);
			return new SegParser(seg, separators, flags);
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Contains several string constants that can be used with some 'split string' functions of this library to specify separators.
	/// </summary>
	public static class SegSep
	{
		/// <summary>
		/// Specifies that separators are spaces, tabs, newlines and other characters for which <see cref="char.IsWhiteSpace(char)"/> returns true.
		/// </summary>
		public const string Whitespace = "SSlkGrJUMUutrbSK3s6Crw";

		/// <summary>
		/// Specifies that separators are all characters for which <see cref="char.IsLetterOrDigit(char)"/> returns false.
		/// </summary>
		public const string Word = "WWVL0EtrK0ShqYWb4n1CmA";

		/// <summary>
		/// Specifies that separators are substrings "\r\n", as well as single characters '\r' and '\n'.
		/// </summary>
		public const string Line = "LLeg5AWCNkGTZDkWuyEa2g";

		//note: all must be of length 22.
	}

	/// <summary>
	/// Flags for <see cref="ExtString.Segments(string, string, SegFlags)"/> and some other functions.
	/// </summary>
	[Flags]
	public enum SegFlags :byte
	{
		/// <summary>
		/// Don't return empty substrings.
		/// For example, is string is "one  two " and separators is " ", return {"one", "two"} instead of {"one", "", "two", ""}.
		/// </summary>
		NoEmpty = 1,

		//rejected. Rarely used, makes slower, let use String.Split.
		///// <summary>
		///// The separators argument is string separator, not separator characters.
		///// </summary>
		//StringSeparator = ,
	}

	/// <summary>
	/// Splits a string or <b>StringSegment</b> into substrings as <see cref="StringSegment"/> variables.
	/// </summary>
	/// <remarks>
	/// Used with foreach. Also used internally by some functions of this library, for example <see cref="ExtString.SegSplit(string, string, SegFlags)"/> and <see cref="ExtString.SegLines"/>.
	/// Normally you don't create <b>SegParser</b> instances explicitly; instead use <see cref="ExtString.Segments(string, string, SegFlags)"/> or <see cref="StringSegment.Split"/> with foreach.
	/// </remarks>
	public struct SegParser :IEnumerable<StringSegment>, IEnumerator<StringSegment>
	{
		readonly string _separators;
		readonly string _s;
		readonly int _sStart, _sEnd;
		int _start, _end;
		ushort _sepLength;
		SegFlags _flags;

		/// <summary>
		/// Initializes this instance to split a string.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="flags"></param>
		public SegParser(string s, string separators, SegFlags flags = 0)
		{
			_separators = separators;
			_s = s;
			_sStart = 0;
			_start = 0;
			_sEnd = s.Length;
			_end = -1;
			_sepLength = 1;
			_flags = flags;
		}

		/// <summary>
		/// Initializes this instance to split a <see cref="StringSegment"/>.
		/// </summary>
		/// <param name="seg">The StringSegment.</param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="SegSep"/> constants.</param>
		/// <param name="flags"></param>
		public SegParser(in StringSegment seg, string separators, SegFlags flags = 0)
		{
			_separators = separators;
			_s = seg.Buffer;
			_sStart = seg.Offset;
			_sEnd = _sStart + seg.Length;
			_end = _sStart - 1;
			_start = 0;
			_sepLength = 1;
			_flags = flags;
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
		[NoDoc]
		public SegParser GetEnumerator() => this;

		IEnumerator<StringSegment> IEnumerable<StringSegment>.GetEnumerator() => this;

		IEnumerator IEnumerable.GetEnumerator() => this;

		[NoDoc]
		public StringSegment Current => new StringSegment(_s, _start, _end - _start);

		object IEnumerator.Current => Current;

		[NoDoc]
		public bool MoveNext()
		{
			gStart:
			int i = _end + _sepLength, to = _sEnd;
			if(i > to) return false;
			_start = i;
			string s = _s, sep = _separators;
			switch(sep.Length) {
			case 1: {
					var c = sep[0];
					for(; i < to; i++) if(s[i] == c) goto g1;
				}
				break;
			case 22:
				if(ReferenceEquals(sep, SegSep.Whitespace)) {
					for(; i < to; i++)
						if(char.IsWhiteSpace(s[i])) goto g1;
				} else if(ReferenceEquals(sep, SegSep.Word)) {
					for(; i < to; i++)
						if(!char.IsLetterOrDigit(s[i])) goto g1;
				} else if(ReferenceEquals(sep, SegSep.Line)) {
					_sepLength = 1;
					for(; i < to; i++) {
						var c = s[i];
						if(c > '\r') continue;
						if(c == '\r') goto g2; else if(c == '\n') goto g1;
					}
					break;
					g2:
					if(i < to - 1 && s[i + 1] == '\n') _sepLength = 2;
					break;
				} else goto default;
				break;
			default: {
					for(; i < to; i++) {
						var c = s[i];
						for(int j = 0; j < sep.Length; j++) if(c == sep[j]) goto g1; //speed: reverse slower
					}
				}
				break;
			}
			g1:
			_end = i;
			if(i == _start && 0 != (_flags & SegFlags.NoEmpty)) goto gStart;
			return true;
		}

		[NoDoc]
		public void Dispose()
		{
			//rejected: normally this variable is not reused because GetEnumerator returns a copy.
			//_end = _sStart - 1;
		}

		[NoDoc]
		public void Reset()
		{
			_end = _sStart - 1;
		}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// Returns segment values as string[].
		/// </summary>
		/// <param name="maxCount">The maximal number of substrings to get. If negative (default), gets all.</param>
		public unsafe string[] ToStringArray(int maxCount = -1)
		{
			//All this big code is just to make this function as fast as String.Split or faster. Also less garbage.
			//	Simple code is slower when substrings are very short. Now same speed when short, faster when normal or long.
			//	With short substrings, the parsing code takes less than half of time. Creating strings and arrays is the slow part.
			//	As an intermediate buffer we use a threadlocal array. stackalloc slower and cannot be string[]. Native memory much slower. WeakReference too slow.

			string[] a1 = t_a1 ?? (t_a1 = new string[c_a1Size]); //at first use the threadlocal array. When it is filled, use a2.
			List<string> a2 = null;

			int n = 0;
			for(; MoveNext(); n++) { //slightly faster than foreach
				if(maxCount >= 0 && n == maxCount) break;
				int start = _start, len = _end - start;
				var s = _s.Substring(start, len);
				if(n < c_a1Size) {
					a1[n] = s;
				} else {
					if(a2 == null) a2 = new List<string>(c_a1Size);
					a2.Add(s);
				}
			}
			_end = _sStart - 1; //reset, because this variable can be reused later. never mind: try/finally; it makes slightly slower.

			var r = new string[n];
			for(int i = 0, to = Math.Min(n, c_a1Size); i < to; i++) {
				r[i] = a1[i];
				a1[i] = null;
			}
			for(int i = c_a1Size; i < r.Length; i++) {
				r[i] = a2[i - c_a1Size];
			}
			return r;
		}

		const int c_a1Size = 50; //400 bytes, in 32-bit 200
		[ThreadStatic] static string[] t_a1; //tested: with WeakReference cannot make fast enough.

		//rejected: not useful. Can use LINQ ToArray.
		//public StringSegment[] ToSegmentArray(int maxCount = -1)
	}
}
