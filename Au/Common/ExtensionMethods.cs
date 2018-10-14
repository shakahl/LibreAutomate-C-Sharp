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
using System.Xml.Linq;
using System.Security; //for XML comments
using System.Globalization;

using static Au.NoClass;

//note: be careful when adding functions to this class. See comments in ExtensionMethods_Forms.cs.

namespace Au.Types
{
	/// <summary>
	/// Adds extension methods to some .NET classes.
	/// </summary>
	public static partial class ExtensionMethods
	{
		#region value types

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="double.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToString_(this double t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="float.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToString_(this float t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Converts double to string.
		/// Uses invariant culture, therefore decimal point is always '.', not ',' etc.
		/// Calls <see cref="decimal.ToString(string, IFormatProvider)"/>.
		/// </summary>
		public static string ToString_(this decimal t, string format = null)
		{
			return t.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// Returns true if t.Width &lt;= 0 || t.Height &lt;= 0.
		/// This extension method has been added because Rectangle.IsEmpty returns true only when all fields are 0, which is not very useful.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEmpty_(this System.Drawing.Rectangle t)
		{
			return t.Width <= 0 || t.Height <= 0;
		}

		//rejected: too simple. We have Print(uint), also can use $"0x{t:X}" or "0x" + t.ToString("X").
		///// <summary>
		///// Converts int to hexadecimal string like "0x3A".
		///// </summary>
		//public static string ToHex_(this int t)
		//{
		//	return "0x" + t.ToString("X");
		//}

		#endregion

		#region enum

		/// <summary>
		/// Returns true if this enum variable has all flag bits specified in <paramref name="flag"/>.
		/// The same as Enum.HasFlag, but much faster.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flag">One or more flags.</param>
		public static unsafe bool Has_<T>(this T t, T flag) where T : unmanaged, Enum
		{
			//return (t & flag) == flag; //error, although C# 7.3 supports Enum constraint.
			//return ((int)t & (int)flag) == (int)flag; //error too
			switch(sizeof(T)) {
			case 4:
				int t4 = *(int*)&t, f4 = *(int*)&flag;
				return (t4 & f4) == f4;
			case 8:
				long t8 = *(long*)&t, f8 = *(long*)&flag;
				return (t8 & f8) == f8;
			case 2:
				int t2 = *(ushort*)&t, f2 = *(ushort*)&flag;
				return (t2 & f2) == f2;
			default:
				Debug.Assert(sizeof(T) == 1);
				int t1 = *(byte*)&t, f1 = *(byte*)&flag;
				return (t1 & f1) == f1;
			}
			//This is not so nicely optimized as with Unsafe.dll, but the switch is optimized away. Native code contains only the case for T size. In other funcs too.
		}

		/// <summary>
		/// Returns true if this enum variable has one or more flag bits specified in <paramref name="flags"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flags">One or more flags.</param>
		public static unsafe bool HasAny_<T>(this T t, T flags) where T : unmanaged, Enum
		{
			switch(sizeof(T)) {
			case 4:
				return (*(int*)&t & *(int*)&flags) != 0;
			case 8:
				return (*(long*)&t & *(long*)&flags) != 0;
			case 2:
				return (*(ushort*)&t & *(ushort*)&flags) != 0;
			default:
				Debug.Assert(sizeof(T) == 1);
				return (*(byte*)&t & *(byte*)&flags) != 0;
			}
		}

		/// <summary>
		/// Adds or removes a flag.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="flag">One or more flags to add or remove.</param>
		/// <param name="add">If true, adds flag, else removes flag.</param>
		public static unsafe void SetFlag_<T>(ref this T t, T flag, bool add) where T : unmanaged, Enum
		{
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
		public static T[] RemoveAt_<T>(this T[] t, int index, int count = 1)
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
		public static T[] Insert_<T>(this T[] t, int index, T value = default)
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
		public static T[] Insert_<T>(this T[] t, int index, params T[] values)
		{
			if((uint)index > t.Length) throw new ArgumentOutOfRangeException();
			int n = values?.Length ?? 0; if(n == 0) return t;

			var r = new T[t.Length + n];
			for(int i = 0; i < index; i++) r[i] = t[i];
			for(int i = index; i < t.Length; i++) r[i + n] = t[i];
			for(int i = 0; i < n; i++) r[i + index] = values[i];
			return r;
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
