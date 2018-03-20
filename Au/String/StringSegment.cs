//Modified version of Microsoft.Extensions.Primitives.StringSegment. It is from github; current .NET does not have it.
//Can be used instead of String.Split. Generates zero garbage. Faster (the github version with StringTokenizer was slower).

using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	//TODO: test all untested methods
	/// <summary>
	/// An optimized representation of a substring.
	/// </summary>
	/// <remarks>
	/// A StringSegment variable holds a reference to the string containing the substring (<see cref="Buffer"/>), the start index of the substring (<see cref="Offset"/>) and substring length (<see cref="Length"/>). A new string object is allocated only by functions that return a string, eg <see cref="Value"/>.
	/// One of ways to create StringSegment instances is to split a string with <see cref="String_.Segments_(string, string, SegFlags)"/>.
	/// Functions throw ArgumentOutOfRangeException, ArgumentNullException or NullReferenceException exception when an argument is invalid. The indexer throws IndexOutOfRangeException.
	/// Don't use the default constructor. Then Buffer is null and the behavior of functions is undefined. Other constructors throw exception if the string is null.
	/// </remarks>
	public struct StringSegment :IEquatable<StringSegment>, IEquatable<string>
	{
		string _buffer;
		int _offset, _length;

		/// <summary>
		/// Initializes an instance of the StringSegment struct.
		/// The segment will be whole buffer.
		/// </summary>
		/// <param name="buffer">Cannot be null.</param>
		public StringSegment(string buffer)
		{
			_buffer = buffer;
			_offset = 0;
			_length = buffer.Length; //NullReferenceException
		}

		/// <summary>
		/// Initializes an instance of the StringSegment struct.
		/// </summary>
		/// <param name="buffer">The string that contains this substring. Cannot be null.</param>
		/// <param name="offset">The offset of the segment within the buffer.</param>
		/// <param name="length">The length of the segment.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StringSegment(string buffer, int offset, int length)
		{
			if((uint)offset > (uint)buffer.Length || (uint)length > (uint)(buffer.Length - offset)) throw new ArgumentOutOfRangeException(); //and NullReferenceException if buffer null

			_buffer = buffer;
			_offset = offset;
			_length = length;
		}

		/// <summary>
		/// Initializes an instance of the StringSegment struct.
		/// The segment will be from offset to the end of buffer.
		/// </summary>
		/// <param name="buffer">The original string used as buffer.</param>
		/// <param name="offset">The offset of the segment within the buffer.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StringSegment(string buffer, int offset) : this(buffer, offset, buffer.Length - offset) { }

		/// <summary>
		/// Gets the string buffer of this StringSegment.
		/// See also <see cref="Value"/>.
		/// </summary>
		public string Buffer => _buffer;

		/// <summary>
		/// Gets or sets the start index within the buffer of this StringSegment.
		/// The setter also changes <see cref="Length"/>, so that <see cref="EndOffset"/> remains unchanged.
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
		/// Also changes <see cref="Length"/>, so that <see cref="EndOffset"/> remains unchanged.
		/// </summary>
		/// <param name="n">How much to increment. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="Offset"/> would be less than 0 or greater than <see cref="EndOffset"/>.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Offset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void OffsetAdd(int n) => Offset = _offset + n;

		/// <summary>
		/// Decrements <see cref="Offset"/>.
		/// Also changes <see cref="Length"/>, so that <see cref="EndOffset"/> remains unchanged.
		/// </summary>
		/// <param name="n">How much to decrement. Can be negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">New <see cref="Offset"/> would be less than 0 or greater than <see cref="EndOffset"/>.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Offset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void OffsetSub(int n) => Offset = _offset - n;

		/// <summary>
		/// Changes <see cref="Offset"/>.
		/// Also changes <see cref="Length"/>, so that <see cref="EndOffset"/> remains unchanged.
		/// </summary>
		/// <param name="n">New value.</param>
		/// <exception cref="ArgumentOutOfRangeException">n is less than 0 or greater than <see cref="EndOffset"/>.</exception>
		/// <remarks>
		/// This method can be used instead of the <see cref="Offset"/> setter when C# does not allow to call a property setter, for example with foreach variables.
		/// </remarks>
		public void OffsetSet(int n) => Offset = n;

		/// <summary>
		/// Gets or sets the length of this StringSegment.
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
		/// Gets or sets the end index within the buffer of this StringSegment. It's Offset + Length.
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
		/// Returns true if Length == 0.
		/// </summary>
		public bool IsEmpty()
		{
			return _length == 0;
		}

		/// <summary>
		/// Gets the value of this segment as a string.
		/// </summary>
		/// <remarks>
		/// <note type="note">Always creates new string object (substring of <see cref="Buffer"/>). See also <see cref="ValueCached"/>.</note>
		/// </remarks>
		public string Value
		{
			get
			{
				return _buffer.Substring(_offset, _length);
			}
		}

		/// <summary>
		/// Gets the value of this segment as a cached string.
		/// </summary>
		/// <remarks>
		/// Uses an internal thread-specific <see cref="Util.StringCache"/>. Use this function instead of <see cref="Value"/> to avoid much garbage when identical substring values are frequent.
		/// </remarks>
		public string ValueCached
		{
			get
			{
				return Util.StringCache.LibAdd(_buffer, _offset, _length);
			}
		}

		/// <summary>
		/// Returns <see cref="Value"/>.
		/// </summary>
		public override string ToString()
		{
			return Value;
		}

		///
		public override int GetHashCode()
		{
			return Convert_.HashFast(_buffer, _offset, _length);
		}

		/// <summary>
		/// Gets the char at a specified position in the current StringSegment.
		/// </summary>
		/// <param name="index">Character index in this StringSegment.</param>
		public char this[int index]
		{
			get
			{
				if((uint)index >= _length) throw new IndexOutOfRangeException();

				return _buffer[_offset + index]; //NullReferenceException, IndexOutOfRangeException
			}
		}

		/// <summary>
		/// Returns true if obj is StringSegment and its string value is equal to this variable.
		/// Compares only substrigs, not offsets. Uses StringComparison.Ordinal.
		/// </summary>
		public override bool Equals(object obj)
		{
			return obj is StringSegment && _Equals((StringSegment)obj, false);
		}

		/// <summary>
		/// Returns true if string values of this and other variable are equal.
		/// Compares only substrigs, not offsets. Uses StringComparison.Ordinal.
		/// </summary>
		/// <param name="other">A variable to compare with this variable.</param>
		public bool Equals(StringSegment other)
		{
			return _Equals(other, false);
		}

		/// <summary>
		/// Returns true if string values of this and other variable are equal, case insensitive.
		/// Compares only substrigs, not offsets. Uses StringComparison.OrdinalIgnoreCase.
		/// </summary>
		/// <param name="other">A variable to compare with this variable.</param>
		public bool EqualsI(StringSegment other)
		{
			return _Equals(other, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool _Equals(StringSegment s, bool ignoreCase)
		{
			int len = s.Length;
			if(len != _length) return false;
			return string.Compare(_buffer, _offset, s._buffer, s._offset, len, _StrComp(ignoreCase)) == 0;
		}

		StringComparison _StrComp(bool ignoreCase) => ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool _Equals(string s, bool ignoreCase)
		{
			int len = s.Length; //NullReferenceException
			if(len != _length) return false;
			return string.Compare(_buffer, _offset, s, 0, len, _StrComp(ignoreCase)) == 0;
		}

		/// <summary>
		/// Returns true if the specified string is equal to this StringSegment.
		/// Uses StringComparison.Ordinal.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		public bool Equals(string text)
		{
			return _Equals(text, false);
		}

		/// <summary>
		/// Returns true if the specified string is equal to this StringSegment, case-insensitive.
		/// Uses StringComparison.OrdinalIgnoreCase.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		public bool EqualsI(string text)
		{
			return _Equals(text, true);
		}

		/// <summary>
		/// Returns true if two specified StringSegment variables have the same string value.
		/// Compares only substrigs, not offsets. Uses StringComparison.Ordinal.
		/// </summary>
		/// <param name="left">The first StringSegment to compare.</param>
		/// <param name="right">The second StringSegment to compare.</param>
		public static bool operator ==(StringSegment left, StringSegment right)
		{
			return left._Equals(right, false);
		}

		/// <summary>
		/// Returns true if two specified StringSegment variables have different string values.
		/// Compares only substrigs, not offsets. Uses StringComparison.Ordinal.
		/// </summary>
		/// <param name="left">The first StringSegment to compare.</param>
		/// <param name="right">The second StringSegment to compare.</param>
		public static bool operator !=(StringSegment left, StringSegment right)
		{
			return left._Equals(right, true);
		}

		/// <summary>
		/// Creates a new StringSegment from the given string.
		/// </summary>
		/// <param name="value">The string to convert to a StringSegment. Cannot be null.</param>
		public static implicit operator StringSegment(string value)
		{
			return new StringSegment(value);
		}
		// PERF: Do NOT add a implicit converter from StringSegment to String. That would negate most of the perf safety.

		//public static explicit operator string(StringSegment seg)=>seg.Value;

		/// <summary>
		/// Returns true if the beginning of this StringSegment matches the specified string.
		/// Uses StringComparison.Ordinal or OrdinalIgnoreCase.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		public bool StartsWith(string text, bool ignoreCase = false)
		{
			var textLength = text.Length; //NullReferenceException
			if(_length < textLength) return false;

			return string.Compare(_buffer, _offset, text, 0, textLength, _StrComp(ignoreCase)) == 0;
		}

		/// <summary>
		/// Returns true if the end of this StringSegment matches the specified string.
		/// Uses StringComparison.Ordinal or OrdinalIgnoreCase.
		/// </summary>
		/// <param name="text">The string. Cannot be null.</param>
		/// <param name="ignoreCase">Case-insensitive.</param>
		public bool EndsWith(string text, bool ignoreCase = false)
		{
			var textLength = text.Length; //NullReferenceException
			if(_length < textLength) return false;

			return string.Compare(_buffer, _offset + _length - textLength, text, 0, textLength, _StrComp(ignoreCase)) == 0;
		}

		/// <summary>
		/// Gets a substring from this StringSegment.
		/// The substring starts at the position specified by offset and has the remaining length.
		/// </summary>
		/// <param name="offset">The zero-based starting character position of a substring in this StringSegment.</param>
		/// <param name="cached">Use an internal thread-specific <see cref="Util.StringCache"/>, to avoid much garbage when identical substring values are frequent.</param>
		public string Substring(int offset, bool cached = false)
		{
			return Substring(offset, _length - offset, cached);
		}

		/// <summary>
		/// Gets a substring from this StringSegment.
		/// </summary>
		/// <param name="offset">The zero-based starting character position of a substring in this StringSegment.</param>
		/// <param name="length">The number of characters in the substring.</param>
		/// <param name="cached">Use an internal thread-specific <see cref="Util.StringCache"/>, to avoid much garbage when identical substring values are frequent.</param>
		public string Substring(int offset, int length, bool cached = false)
		{
			if(offset < 0 || offset + length > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			offset += _offset;
			if(cached) return Util.StringCache.LibAdd(_buffer, offset, length);
			return _buffer.Substring(offset, length);
		}

		/// <summary>
		/// Gets a StringSegment that represents a substring from this StringSegment.
		/// The StringSegment starts at the position specified by offset and has the remaining length.
		/// </summary>
		/// <param name="offset">The zero-based starting character position of a substring in this StringSegment.</param>
		public StringSegment Subsegment(int offset)
		{
			return Subsegment(offset, _length - offset);
		}

		/// <summary>
		/// Gets a StringSegment that represents a substring from this StringSegment.
		/// </summary>
		/// <param name="offset">The zero-based starting character position of a substring in this StringSegment.</param>
		/// <param name="length">The number of characters in the substring.</param>
		public StringSegment Subsegment(int offset, int length)
		{
			if(offset < 0 || offset + length > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			return new StringSegment(_buffer, offset + _offset, length);
		}

		/// <summary>
		/// Gets the zero-based index of the first occurrence of the character c in this StringSegment.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="c">The character.</param>
		/// <param name="startIndex">The zero-based index position at which the search starts.</param>
		/// <param name="count">The number of characters to examine.</param>
		public int IndexOf(char c, int startIndex, int count)
		{
			if(startIndex < 0 || startIndex + count > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			var index = _buffer.IndexOf(c, startIndex + _offset, count);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Gets the zero-based index of the first occurrence of the character c in this StringSegment.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="c">The character.</param>
		/// <param name="startIndex">The zero-based index position at which the search starts.</param>
		public int IndexOf(char c, int startIndex)
		{
			return IndexOf(c, startIndex, _length - startIndex);
		}

		/// <summary>
		/// Gets the zero-based index of the first occurrence of the character c in this StringSegment.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="c">The character.</param>
		public int IndexOf(char c)
		{
			var index = _buffer.IndexOf(c, _offset, _length);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Gets the zero-based index of the first occurrence in this StringSegment of any specified characters.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="anyOf">One or more characters to seek.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <param name="count">The number of character positions to examine.</param>
		public int IndexOfAny(char[] anyOf, int startIndex, int count)
		{
			if(startIndex < 0 || startIndex + count > _length) throw new ArgumentOutOfRangeException(); //the rest will be validated in the called function

			int index = _buffer.IndexOfAny(anyOf, _offset + startIndex, count);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Gets the zero-based index of the first occurrence in this StringSegment of any specified characters.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="anyOf">One or more characters to seek.</param>
		/// <param name="startIndex">The search starting position.</param>
		public int IndexOfAny(char[] anyOf, int startIndex)
		{
			return IndexOfAny(anyOf, startIndex, _length - startIndex);
		}

		/// <summary>
		/// Gets the zero-based index of the first occurrence in this StringSegment of any specified characters.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="anyOf">One or more characters to seek.</param>
		public int IndexOfAny(char[] anyOf)
		{
			int index = _buffer.IndexOfAny(anyOf, _offset, _length);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Gets the zero-based index of the last occurrence in this StringSegment of any specified characters.
		/// Returns -1 if not found.
		/// </summary>
		/// <param name="value">The character.</param>
		public int LastIndexOf(char value)
		{
			var index = _buffer.LastIndexOf(value, _offset + _length - 1, _length);
			if(index >= 0) index -= _offset;
			return index;
		}

		/// <summary>
		/// Removes all leading and trailing whitespaces.
		/// </summary>
		public void Trim()
		{
			TrimStart();
			TrimEnd();
		}

		/// <summary>
		/// Removes all leading whitespaces (see <see cref="char.IsWhiteSpace(char)"/>).
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
		/// Returns a <see cref="SegParser"/> that will split this StringSegment into StringSegments when used with foreach.
		/// </summary>
		/// <param name="separators">A string containing characters that delimit the substrings in this StringSegment. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="flags"></param>
		/// <seealso cref="String_.Segments_(string, string, SegFlags)"/>
		public SegParser Split(string separators, SegFlags flags = 0)
		{
			return new SegParser(ref this, separators, flags);
		}
	}

	public static partial class String_
	{
		/// <summary>
		/// This function can be used with foreach to split this string into substrings as StringSegment variables.
		/// Returns a SSegmenter object that implements IEnumerable&lt;StringSegment&gt;.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="separators">A string containing characters that delimit the substrings in this string. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="flags"></param>
		/// <example>
		/// <code><![CDATA[
		/// string s = "one * two three ";
		/// foreach(var t in s.Segments_(" ")) Print(t);
		/// foreach(var t in s.Segments_(Separators.Word, SegFlags.NoEmpty)) Print(t);
		/// ]]></code>
		/// </example>
		public static SegParser Segments_(this string t, string separators, SegFlags flags = 0)
		{
			return new SegParser(t, separators, flags);
		}

		/// <summary>
		/// This function can be used with foreach to split the specified part of this string into substrings as StringSegment variables.
		/// Returns a SSegmenter object that implements IEnumerable&lt;StringSegment&gt;.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="startIndex">Start of the part of this string.</param>
		/// <param name="length">Length of the part of this string.</param>
		/// <param name="separators">A string containing characters that delimit the substrings in this string. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="flags"></param>
		public static SegParser Segments_(this string t, int startIndex, int length, string separators, SegFlags flags = 0)
		{
			var seg = new StringSegment(t, startIndex, length);
			return new SegParser(ref seg, separators, flags);
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Contains several string constants that can be used with some 'split string' functions of this library to specify separators.
	/// </summary>
	public static class Separators
	{
		//note: all strings are Base64 GUIDs and have length 22.

		/// <summary>
		/// Specifies that separators are all characters for which <see cref="char.IsWhiteSpace(char)"/> returns true.
		/// </summary>
		public const string Whitespace = "WZlkGrJUMUutrbSK3s6Crw";

		/// <summary>
		/// Specifies that separators are all characters for which <see cref="char.IsLetterOrDigit(char)"/> returns false.
		/// </summary>
		public const string Word = "CcVL0EtrK0ShqYWb4n1CmA";

		/// <summary>
		/// Specifies that separators are substrings "\r\n", as well as single ASCII line break characters '\r' and '\n'.
		/// </summary>
		public const string Line = "sKeg5AWCNkGTZDkWuyEa2g";
	}

	/// <summary>
	/// Flags for <see cref="String_.Segments_(string, string, SegFlags)"/> and some other functions.
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
	/// Splits a string or StringSegment into StringSegments.
	/// Used with foreach. Also used internally by some functions of this library, for example <see cref="String_.Split_(string, string, SegFlags)"/> and <see cref="String_.SplitLines_"/>.
	/// Normally you don't create Segmenter instances explicitly; instead use <see cref="String_.Segments_(string, string, SegFlags)"/> or <see cref="StringSegment.Split"/> with foreach.
	/// </summary>
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
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="Separators"/> constants.</param>
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
		/// Initializes this instance to split a StringSegment.
		/// </summary>
		/// <param name="seg">The StringSegment.</param>
		/// <param name="separators">A string containing characters that delimit substrings. Or one of <see cref="Separators"/> constants.</param>
		/// <param name="flags"></param>
		public SegParser(ref StringSegment seg, string separators, SegFlags flags = 0)
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
		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SegParser GetEnumerator() => this;

		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		IEnumerator<StringSegment> IEnumerable<StringSegment>.GetEnumerator() => this;

		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		IEnumerator IEnumerable.GetEnumerator() => this;

		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public StringSegment Current => new StringSegment(_s, _start, _end - _start);

		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		object IEnumerator.Current => Current;

		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
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
				if(ReferenceEquals(sep, Separators.Whitespace)) {
					for(; i < to; i++)
						if(char.IsWhiteSpace(s[i])) goto g1;
				} else if(ReferenceEquals(sep, Separators.Word)) {
					for(; i < to; i++)
						if(!char.IsLetterOrDigit(s[i])) goto g1;
				} else if(ReferenceEquals(sep, Separators.Line)) {
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

		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Dispose()
		{
			//rejected: normally this variable is not reused because GetEnumerator returns a copy.
			//_end = _sStart - 1;
		}

		/// <tocexclude />
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset()
		{
			_end = _sStart - 1;
		}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

		/// <summary>
		/// Returns segment values as string[].
		/// </summary>
		/// <param name="maxCount">The maximum number of substrings to get. If negative (default), gets all.</param>
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

		//rejected: has no sense. Can use LINQ ToArray.
		//public StringSegment[] ToSegmentArray(int maxCount = -1)
	}
}
