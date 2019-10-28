//Small extension classes for .NET classes. Except those that have own files.
//Naming:
//	Class name: related .NET class name with _ suffix.
//	Extension method name: related .NET method name with _ suffix. Or new name with _ suffix.
//	Static method name: any name without _ suffix.

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
using System.Linq;
using System.Security; //for XML comments
using System.Globalization;

using static Au.AStatic;

//note: be careful when adding functions to this class. See comments in ExtensionMethods_Forms.cs.

namespace Au
{
	/// <summary>
	/// Adds extension methods for some .NET classes.
	/// </summary>
	public static partial class AExtensions
	{
		#region value types

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="double.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this double t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="float.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this float t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="decimal.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this decimal t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Returns true if t.Width &lt;= 0 || t.Height &lt;= 0.
		/// This extension method has been added because Rectangle.IsEmpty returns true only when all fields are 0, which is not very useful.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmptyRect(this System.Drawing.Rectangle t)
		{
			return t.Width <= 0 || t.Height <= 0;
		}

		/// <summary>
		/// Calls <see cref="Range.GetOffsetAndLength"/> and returns start and end instead of start and length.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="length"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static (int start, int end) GetStartEnd(this Range t, int length)
		{
			var v = t.GetOffsetAndLength(length);
			return (v.Offset, v.Offset + v.Length);
		}

		//rejected: too simple. We have Print(uint), also can use $"0x{t:X}" or "0x" + t.ToString("X").
		///// <summary>
		///// Converts int to hexadecimal string like "0x3A".
		///// </summary>
		//public static string ToHex_(this int t)
		//{
		//	return "0x" + t.ToString("X");
		//}

		//rejected. Cannot use 'break' etc. Better add UI to create 'for' loop to "repeat n times".
		///// <summary>
		///// Executes code this number of times. Can be used instead of 'for' to "repeat code n times".
		///// </summary>
		///// <param name="t">This variable. Specifies the number of times to execute code.</param>
		///// <param name="code">Lambda function containing code.</param>
		///// <example>
		///// <code><![CDATA[
		///// for(int i = 0; i < 3; i++) {
		///// 	Print(1);
		///// }
		///// 
		///// //this can be used instead of the above code with 'for'
		///// 3.Times(() => {
		///// 	Print(2);
		///// });
		///// ]]></code>
		///// </example>
		//public static void Times(this int t, Action code)
		//{
		//	for(int i = 0; i < t; i++) {
		//		code();
		//	}
		//}

		///// <summary>
		///// Executes code this number of times. Can be used instead of 'for' to "repeat code n times".
		///// Use this overload when need a counter variable.
		///// </summary>
		///// <param name="t">This variable. Specifies the number of times to execute code.</param>
		///// <param name="code">Lambda function containing code. The parameter is a counter variable; starts from 0.</param>
		///// <example>
		///// <code><![CDATA[
		///// for(int i = 0; i < 3; i++) {
		///// 	Print(i);
		///// }
		///// 
		///// //this can be used instead of the above code with 'for'
		///// 3.Times(i => {
		///// 	Print(i);
		///// });
		///// ]]></code>
		///// </example>
		//public static void Times(this int t, Action<int> code)
		//{
		//	for(int i = 0; i < t; i++) {
		//		code(i);
		//	}
		//}

		#endregion

		#region enum

		/// <summary>
		/// Returns true if this enum variable has all flag bits specified in <i>flag</i>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flag">One or more flags.</param>
		/// <remarks>
		/// The same as code <c>(t &amp; flag) == flag</c> or <b>Enum.HasFlag</b>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if true //Enum.HasFlag used to be slow, but now compiler for it creates the same code as with operator
		public static unsafe bool Has<T>(this T t, T flag) where T : unmanaged, Enum => t.HasFlag(flag);
#else //the same speed, although compiler may create more code
		public static unsafe bool Has<T>(this T t, T flag) where T : unmanaged, Enum
		{
			switch(sizeof(T)) {
			case 4: {
				var a = Unsafe.As<T, uint>(ref t);
				var b = Unsafe.As<T, uint>(ref flag);
				return (a & b) == b;
			}
			case 8: {
				var a = Unsafe.As<T, ulong>(ref t);
				var b = Unsafe.As<T, ulong>(ref flag);
				return (a & b) == b;
			}
			case 2: {
				var a = Unsafe.As<T, ushort>(ref t);
				var b = Unsafe.As<T, ushort>(ref flag);
				return (a & b) == b;
			}
			default: {
				var a = Unsafe.As<T, byte>(ref t);
				var b = Unsafe.As<T, byte>(ref flag);
				return (a & b) == b;
			}
			}
			//compiler removes the switch/case, because sizeof(T) is const
		}
#endif

		/// <summary>
		/// Returns true if this enum variable has one or more flag bits specified in <i>flags</i>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flags">One or more flags.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe bool HasAny<T>(this T t, T flags) where T : unmanaged, Enum
		{
			switch(sizeof(T)) {
			case 4: {
				var a = Unsafe.As<T, uint>(ref t);
				var b = Unsafe.As<T, uint>(ref flags);
				return (a & b) != 0;
			}
			case 8: {
				var a = Unsafe.As<T, ulong>(ref t);
				var b = Unsafe.As<T, ulong>(ref flags);
				return (a & b) != 0;
			}
			case 2: {
				var a = Unsafe.As<T, ushort>(ref t);
				var b = Unsafe.As<T, ushort>(ref flags);
				return (a & b) != 0;
			}
			default: {
				var a = Unsafe.As<T, byte>(ref t);
				var b = Unsafe.As<T, byte>(ref flags);
				return (a & b) != 0;
			}
			}
		}

		/// <summary>
		/// Adds or removes a flag.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flag">One or more flags to add or remove.</param>
		/// <param name="add">If true, adds flag, else removes flag.</param>
		public static unsafe void SetFlag<T>(ref this T t, T flag, bool add) where T : unmanaged, Enum
		{
			//FUTURE: use Unsafe.

			T tt = t;
			switch(sizeof(T)) {
			case 4: {
				int a = *(int*)&tt, b = *(int*)&flag;
				if(add) a |= b; else a &= ~b;
				*(int*)&tt = a;
			}
			break;
			case 8: {
				long a = *(long*)&tt, b = *(long*)&flag;
				if(add) a |= b; else a &= ~b;
				*(long*)&tt = a;
			}
			break;
			case 2: {
				int a = *(ushort*)&tt, b = *(ushort*)&flag;
				if(add) a |= b; else a &= ~b;
				*(ushort*)&tt = (ushort)a;
			}
			break;
			default: {
				Debug.Assert(sizeof(T) == 1);
				int a = *(byte*)&tt, b = *(byte*)&flag;
				if(add) a |= b; else a &= ~b;
				*(byte*)&tt = (byte)a;
			}
			break;
			}
			t = tt;
		}

		#endregion

		#region array

		/// <summary>
		/// Creates a copy of this array with one or more removed elements.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static T[] RemoveAt<T>(this T[] t, int index, int count = 1)
		{
			if((uint)index > t.Length || count < 0 || index + count > t.Length) throw new ArgumentOutOfRangeException();
			int n = t.Length - count;
			var r = new T[n];
			for(int i = 0; i < index; i++) r[i] = t[i];
			for(int i = index; i < n; i++) r[i] = t[i + count];
			return r;
		}

		/// <summary>
		/// Creates a copy of this array with one inserted element.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static T[] InsertAt<T>(this T[] t, int index, T value = default)
		{
			if((uint)index > t.Length) throw new ArgumentOutOfRangeException();
			var r = new T[t.Length + 1];
			for(int i = 0; i < index; i++) r[i] = t[i];
			for(int i = index; i < t.Length; i++) r[i + 1] = t[i];
			r[index] = value;
			return r;
		}

		/// <summary>
		/// Creates a copy of this array with several inserted elements.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="index"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static T[] InsertAt<T>(this T[] t, int index, params T[] values)
		{
			if((uint)index > t.Length) throw new ArgumentOutOfRangeException();
			int n = values?.Length ?? 0; if(n == 0) return t;

			var r = new T[t.Length + n];
			for(int i = 0; i < index; i++) r[i] = t[i];
			for(int i = index; i < t.Length; i++) r[i + n] = t[i];
			for(int i = 0; i < n; i++) r[i + index] = values[i];
			return r;
		}

		internal static unsafe void WriteInt(this byte[] t, int x, int index)
		{
			if(index < 0 || index > t.Length - 4) throw new ArgumentOutOfRangeException();
			fixed(byte* p = t) *(int*)(p + index) = x;
		}

		internal static unsafe int ReadInt(this byte[] t, int index)
		{
			if(index < 0 || index > t.Length - 4) throw new ArgumentOutOfRangeException();
			fixed(byte* p = t) return *(int*)(p + index);
		}

		#endregion

		#region IEnumerable

		/// <summary>
		/// Removes items based on a predicate. For example, all items that have certain value.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="t"></param>
		/// <param name="predicate"></param>
		public static void RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> t, Func<KeyValuePair<TKey, TValue>, bool> predicate)
		{
			foreach(var k in t.Where(predicate).Select(kv => kv.Key).ToList()) { t.Remove(k); }
		}

		#endregion

		#region StringBuilder

		/// <summary>
		/// Appends string as new correctly formatted sentence.
		/// Returns this.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="s"></param>
		/// <param name="noUcase">Don't make the first character uppercase.</param>
		/// <remarks>
		/// If s is null or "", does nothing.
		/// If this is not empty, appends space.
		/// If s starts with a lowercase character, makes it uppercase, unless this ends with a character other than '.'.
		/// Appends '.' if s does not end with '.', ';', ':', ',', '!' or '?'.
		/// </remarks>
		public static StringBuilder AppendSentence(this StringBuilder t, string s, bool noUcase = false)
		{
			if(!Empty(s)) {
				bool makeUcase = !noUcase && Char.IsLower(s[0]);
				if(t.Length > 0) {
					if(makeUcase && t[t.Length - 1] != '.') makeUcase = false;
					t.Append(' ');
				}
				if(makeUcase) { t.Append(Char.ToUpper(s[0])).Append(s, 1, s.Length - 1); } else t.Append(s);
				switch(s[s.Length - 1]) {
				case '.': case ';': case ':': case ',': case '!': case '?': break;
				default: t.Append('.'); break;
				}
			}
			return t;
		}

		#endregion

		#region internal


		#endregion
	}
}
