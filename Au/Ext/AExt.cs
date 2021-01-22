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
using System.Linq;
using System.Security; //for XML comments
using System.Globalization;


//note: be careful when adding functions to this class. See comments in ExtensionMethods_Forms.cs.

namespace Au
{
	/// <summary>
	/// Adds extension methods for some .NET classes.
	/// </summary>
	public static unsafe partial class AExtensions
	{
		#region value types

		/// <summary>
		/// Converts to int with rounding.
		/// Calls <see cref="Convert.ToInt32(double)"/>.
		/// </summary>
		public static int ToInt(this double t) => Convert.ToInt32(t);

		/// <summary>
		/// Converts to int with rounding.
		/// Calls <see cref="Convert.ToInt32(float)"/>.
		/// </summary>
		public static int ToInt(this float t) => Convert.ToInt32(t);

		/// <summary>
		/// Converts to int with rounding.
		/// Calls <see cref="Convert.ToInt32(decimal)"/>.
		/// </summary>
		public static int ToInt(this decimal t) => Convert.ToInt32(t);

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="double.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this double t, string format = null) {
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="float.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this float t, string format = null) {
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="decimal.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this decimal t, string format = null) {
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts int to string.
		/// Uses invariant culture, therefore minus sign is always ASCII '-', not '−' etc.
		/// Calls <see cref="int.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this int t, string format = null) {
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts long to string.
		/// Uses invariant culture, therefore minus sign is always ASCII '-', not '−' etc.
		/// Calls <see cref="double.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this long t, string format = null) {
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts nint to string.
		/// Uses invariant culture, therefore minus sign is always ASCII '-', not '−' etc.
		/// Calls <see cref="IntPtr.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToStringInvariant(this nint t, string format = null) {
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}
		//cref not nint.ToString because DocFX does not support it.

		/// <summary>
		/// Returns true if t.Width &lt;= 0 || t.Height &lt;= 0.
		/// Note: <b>Rectangle.IsEmpty</b> returns true only when all fields are 0.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmptyRect(this System.Drawing.Rectangle t) {
			return t.Width <= 0 || t.Height <= 0;
		}

		/// <summary>
		/// Calls <see cref="Range.GetOffsetAndLength"/> and returns start and end instead of start and length.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="length"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static (int start, int end) GetStartEnd(this Range t, int length) {
			var v = t.GetOffsetAndLength(length);
			return (v.Offset, v.Offset + v.Length);
		}

		/// <summary>
		/// If this is null, returns <c>(0, length)</c>. Else calls <see cref="Range.GetOffsetAndLength"/> and returns start and end instead of start and length.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="length"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static (int start, int end) GetStartEnd(this Range? t, int length)
			=> t?.GetStartEnd(length) ?? (0, length);

		/// <summary>
		/// If this is null, returns <c>(0, length)</c>. Else calls <see cref="Range.GetOffsetAndLength"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="length"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static (int Offset, int Length) GetOffsetAndLength(this Range? t, int length)
			=> t?.GetOffsetAndLength(length) ?? (0, length);

		//currently not used. Creates shorter string than ToString.
		///// <summary>
		///// Converts this <b>Guid</b> to Base-64 string.
		///// </summary>
		//public static string ToBase64(this Guid t) => Convert.ToBase64String(new ReadOnlySpan<byte>((byte*)&t, sizeof(Guid)));

		//rejected: too simple. We have AOutput.Write(uint), also can use $"0x{t:X}" or "0x" + t.ToString("X").
		///// <summary>
		///// Converts int to hexadecimal string like "0x3A".
		///// </summary>
		//public static string ToHex(this int t)
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
		///// 	AOutput.Write(1);
		///// }
		///// 
		///// //this can be used instead of the above code with 'for'
		///// 3.Times(() => {
		///// 	AOutput.Write(2);
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
		///// 	AOutput.Write(i);
		///// }
		///// 
		///// //this can be used instead of the above code with 'for'
		///// 3.Times(i => {
		///// 	AOutput.Write(i);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static long _ToLong<T>(T v) where T : unmanaged, Enum {
			if (sizeof(T) == 4) return *(int*)&v;
			if (sizeof(T) == 8) return *(long*)&v;
			if (sizeof(T) == 2) return *(short*)&v;
			return *(byte*)&v;
			//Compiler removes the if(sizeof(T) == n) and code that is unused with that size, because sizeof(T) is const.
			//Faster than with switch(sizeof(T)). It seems the switch code is considered too big to be inlined.
		}

		//same. Was faster when tested in the past.
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//static long _ToLong2<T>(T v) where T : unmanaged, Enum
		//{
		//	if(sizeof(T) == 4) return Unsafe.As<T, int>(ref v);
		//	if(sizeof(T) == 8) return Unsafe.As<T, long>(ref v);
		//	if(sizeof(T) == 2) return Unsafe.As<T, short>(ref v);
		//	return Unsafe.As<T, byte>(ref v);
		//}

		/// <summary>
		/// Returns true if this enum variable has all flag bits specified in <i>flag</i>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flag">One or more flags.</param>
		/// <remarks>
		/// The same as code <c>(t &amp; flag) == flag</c> or <b>Enum.HasFlag</b>.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this T t, T flag) where T : unmanaged, Enum {
#if false //Enum.HasFlag used to be slow, but now compiler for it creates the same code as with operator
			return t.HasFlag(flag);
			//However cannot use this because of JIT compiler bug: in some cases Has returns true when no flag.
			//Noticed it in TriggerActionThreads.Run in finally{} of actionWrapper, code opt.flags.Has(TOFlags.SingleInstance).
			//It was elusive, difficult to debug, only in Release, and only after some time/times, when tiered JIT fully optimizes.
			//When Has returned true, AOutput.Write showed that flags is 0.
			//No bug if HasFlag called directly, not in extension method.
#elif true //slightly slower than Enum.HasFlag and code as with operator
			var m = _ToLong(flag);
			return (_ToLong(t) & m) == m;
#else //slower
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
#endif
		}

		/// <summary>
		/// Returns true if this enum variable has one or more flag bits specified in <i>flags</i>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flags">One or more flags.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool HasAny<T>(this T t, T flags) where T : unmanaged, Enum {
			return (_ToLong(t) & _ToLong(flags)) != 0;
		}

		//slower
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//public static bool HasAny5<T>(this T t, T flags) where T : unmanaged, Enum
		//{
		//	if(sizeof(T) == 4) return (*(int*)&t & *(int*)&flags) != 0;
		//	if(sizeof(T) == 8) return (*(long*)&t & *(long*)&flags) != 0;
		//	if(sizeof(T) == 2) return (*(short*)&t & *(short*)&flags) != 0;
		//	return (*(byte*)&t & *(byte*)&flags) != 0;
		//}

		/// <summary>
		/// Adds or removes a flag.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flag">One or more flags to add or remove.</param>
		/// <param name="add">If true, adds flag, else removes flag.</param>
		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
		public static void SetFlag<T>(ref this T t, T flag, bool add) where T : unmanaged, Enum {
			long a = _ToLong(t), b = _ToLong(flag);
			if (add) a |= b; else a &= ~b;
			t = *(T*)&a;
		}

		//rejected. Rarely used. Adds many garbage in compiled documentation for enums.
		//	Can istead write: if(e is EnumX.Val1 or EnumX.Val2 ...). But currently problems with intellisense; works better with (): if(e is (EnumX.Val1 or EnumX.Val2 ...)).
//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, T v1, T v2) where T : unmanaged, Enum //could be IConvertible, it includes int etc, but also double, float, bool, DateTime and any IConvertible struct of any size. Types other than enum and int actually are not useful.
//		{
//			var a = _ToLong(t);
//			return a == _ToLong(v1) || a == _ToLong(v2);
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, T v1, T v2, T v3) where T : unmanaged, Enum {
//			var a = _ToLong(t);
//			return a == _ToLong(v1) || a == _ToLong(v2) || a == _ToLong(v3);
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, T v1, T v2, T v3, T v4) where T : unmanaged, Enum {
//			//return t.Equals(v1) || t.Equals(v2) || t.Equals(v3) || t.Equals(v4); //very slow, slower than with params
//			var a = _ToLong(t);
//			return a == _ToLong(v1) || a == _ToLong(v2) || a == _ToLong(v3) || a == _ToLong(v4);
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, T v1, T v2, T v3, T v4, T v5) where T : unmanaged, Enum {
//			var a = _ToLong(t);
//			return a == _ToLong(v1) || a == _ToLong(v2) || a == _ToLong(v3) || a == _ToLong(v4) || a == _ToLong(v5);
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, T v1, T v2, T v3, T v4, T v5, T v6) where T : unmanaged, Enum {
//			var a = _ToLong(t);
//			return a == _ToLong(v1) || a == _ToLong(v2) || a == _ToLong(v3) || a == _ToLong(v4) || a == _ToLong(v5) || a == _ToLong(v6);
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, T v1, T v2, T v3, T v4, T v5, T v6, T v7) where T : unmanaged, Enum {
//			var a = _ToLong(t);
//			return a == _ToLong(v1) || a == _ToLong(v2) || a == _ToLong(v3) || a == _ToLong(v4) || a == _ToLong(v5) || a == _ToLong(v6) || a == _ToLong(v7);
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, T v1, T v2, T v3, T v4, T v5, T v6, T v7, T v8) where T : unmanaged, Enum {
//			var a = _ToLong(t);
//			return a == _ToLong(v1) || a == _ToLong(v2) || a == _ToLong(v3) || a == _ToLong(v4) || a == _ToLong(v5) || a == _ToLong(v6) || a == _ToLong(v7) || a == _ToLong(v8);
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn<T>(this T t, params T[] values) where T : unmanaged, Enum {
//#if true //slightly faster, especially with multitiered JIT. The slow part is creating the params array.
//			var a = _ToLong(t);
//			for (int i = 0; i < values.Length; i++) {
//				if (a == _ToLong(values[i])) return true;
//			}
//			return false;
//#else
//        return values.Contains(t);
//#endif
//		}

//		//same for int

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, int v1, int v2) {
//			return t == v1 || t == v2;
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, int v1, int v2, int v3) {
//			return t == v1 || t == v2 || t == v3;
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, int v1, int v2, int v3, int v4) {
//			return t == v1 || t == v2 || t == v3 || t == v4;
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, int v1, int v2, int v3, int v4, int v5) {
//			return t == v1 || t == v2 || t == v3 || t == v4 || t == v5;
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, int v1, int v2, int v3, int v4, int v5, int v6) {
//			return t == v1 || t == v2 || t == v3 || t == v4 || t == v5 || t == v6;
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, int v1, int v2, int v3, int v4, int v5, int v6, int v7) {
//			return t == v1 || t == v2 || t == v3 || t == v4 || t == v5 || t == v6 || t == v7;
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, int v1, int v2, int v3, int v4, int v5, int v6, int v7, int v8) {
//			return t == v1 || t == v2 || t == v3 || t == v4 || t == v5 || t == v6 || t == v7 || t == v8;
//		}

//		/// <summary>Returns true if this is equal to a value in list.</summary>
//		[MethodImpl(MethodImplOptions.AggressiveOptimization)]
//		public static bool IsIn(int t, params int[] values) {
//			for (int i = 0; i < values.Length; i++) {
//				if (t == values[i]) return true;
//			}
//			return false;
//		}

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
		public static T[] RemoveAt<T>(this T[] t, int index, int count = 1) {
			if ((uint)index > t.Length || count < 0 || index + count > t.Length) throw new ArgumentOutOfRangeException();
			int n = t.Length - count;
			var r = new T[n];
			for (int i = 0; i < index; i++) r[i] = t[i];
			for (int i = index; i < n; i++) r[i] = t[i + count];
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
		public static T[] InsertAt<T>(this T[] t, int index, T value = default) {
			if ((uint)index > t.Length) throw new ArgumentOutOfRangeException();
			var r = new T[t.Length + 1];
			for (int i = 0; i < index; i++) r[i] = t[i];
			for (int i = index; i < t.Length; i++) r[i + 1] = t[i];
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
		public static T[] InsertAt<T>(this T[] t, int index, params T[] values) {
			if ((uint)index > t.Length) throw new ArgumentOutOfRangeException();
			int n = values?.Length ?? 0; if (n == 0) return t;

			var r = new T[t.Length + n];
			for (int i = 0; i < index; i++) r[i] = t[i];
			for (int i = index; i < t.Length; i++) r[i + n] = t[i];
			for (int i = 0; i < n; i++) r[i + index] = values[i];
			return r;
		}

		internal static void WriteInt(this byte[] t, int x, int index) {
			if (index < 0 || index > t.Length - 4) throw new ArgumentOutOfRangeException();
			fixed (byte* p = t) *(int*)(p + index) = x;
		}

		internal static int ReadInt(this byte[] t, int index) {
			if (index < 0 || index > t.Length - 4) throw new ArgumentOutOfRangeException();
			fixed (byte* p = t) return *(int*)(p + index);
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
		public static void RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> t, Func<KeyValuePair<TKey, TValue>, bool> predicate) {
			foreach (var k in t.Where(predicate).Select(kv => kv.Key).ToList()) { t.Remove(k); }
		}

		/// <summary>
		/// Returns <b>Length</b>, or 0 if null.
		/// </summary>
		internal static int Lenn_<T>(this T[] t) => t?.Length ?? 0;
		//internal static int Lenn_(this System.Collections.ICollection t) => t?.Count ?? 0; //slower, as well as Array

		/// <summary>
		/// Returns <b>Count</b>, or 0 if null.
		/// </summary>
		internal static int Lenn_<T>(this List<T> t) => t?.Count ?? 0;

		/// <summary>
		/// Returns true if null or <b>Length</b> == 0.
		/// </summary>
		internal static bool NE_<T>(this T[] t) => (t?.Length ?? 0) == 0;

		/// <summary>
		/// Returns true if null or <b>Count</b> == 0.
		/// </summary>
		internal static bool NE_<T>(this List<T> t) => (t?.Count ?? 0) == 0;

		/// <summary>
		/// Efficiently recursively gets descendants of this tree.
		/// <see href="https://stackoverflow.com/a/30441479/2547338"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <param name="childSelector"></param>
		internal static IEnumerable<T> Descendants_<T>(this IEnumerable<T> t, Func<T, IEnumerable<T>> childSelector) {
			var stack = new Stack<IEnumerator<T>>();
			var enumerator = t.GetEnumerator();

			try {
				while (true) {
					if (enumerator.MoveNext()) {
						T element = enumerator.Current;
						yield return element;

						var e = childSelector(element)?.GetEnumerator();
						if (e != null) {
							stack.Push(enumerator);
							enumerator = e;
						}
					} else if (stack.Count > 0) {
						enumerator.Dispose();
						enumerator = stack.Pop();
					} else {
						yield break;
					}
				}
			}
			finally {
				enumerator.Dispose();

				while (stack.Count > 0) // Clean up in case of an exception.
				{
					enumerator = stack.Pop();
					enumerator.Dispose();
				}
			}
		}

		/// <summary>
		/// Efficiently recursively gets descendants of this tree.
		/// <see href="https://stackoverflow.com/a/30441479/2547338"/>
		/// </summary>
		/// <param name="t"></param>
		/// <param name="childSelector"></param>
		internal static System.Collections.IEnumerable Descendants_(this System.Collections.IEnumerable t, Func<object, System.Collections.IEnumerable> childSelector) {
			var stack = new Stack<System.Collections.IEnumerator>();
			var enumerator = t.GetEnumerator();

			while (true) {
				if (enumerator.MoveNext()) {
					object element = enumerator.Current;
					yield return element;

					var e = childSelector(element)?.GetEnumerator();
					if (e != null) {
						stack.Push(enumerator);
						enumerator = e;
					}
				} else if (stack.Count > 0) {
					enumerator = stack.Pop();
				} else {
					yield break;
				}
			}
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
		public static StringBuilder AppendSentence(this StringBuilder t, string s, bool noUcase = false) {
			if (!s.NE()) {
				bool makeUcase = !noUcase && Char.IsLower(s[0]);
				if (t.Length > 0) {
					if (makeUcase && t[^1] != '.') makeUcase = false;
					t.Append(' ');
				}
				if (makeUcase) { t.Append(Char.ToUpper(s[0])).Append(s, 1, s.Length - 1); } else t.Append(s);
				switch (s[^1]) {
				case '.': case ';': case ':': case ',': case '!': case '?': break;
				default: t.Append('.'); break;
				}
			}
			return t;
		}

		#endregion
	}
}
