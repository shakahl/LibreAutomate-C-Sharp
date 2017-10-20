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
//using System.Xml.Linq;
//using System.Xml.XPath;

using static Catkeys.NoClass;

namespace Catkeys.Types
{
	/// <summary>
	/// Types of function parameters and return values, base classes, exceptions.
	/// Class NetExtensions contains extension methods for various .NET classes.
	/// </summary>
	internal class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	/// <summary>Infrastructure.</summary>
	/// <tocexclude />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JustNull { }
	/// <summary>Infrastructure.</summary>
	/// <tocexclude />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JustNull2 { }

	//TODO: review usage. Maybe reject. Or instead use a struct containing member of 'object' type. Because now the calling code is huge and in most cases slower than with object.
	/// <summary>
	/// Used for method parameters that accept one of two types.
	/// </summary>
	/// <remarks>
	/// Some methods have one or several parameters that must be one of several types. There are several ways of implementing such functions.
	/// 1. Overloaded methods. However some programmers often don't like it, because in some cases need to create many overloads, then maintain their documentation. Also then method users have to spend much time to find the correct overload.
	/// 2. Generic methods. However in most cases the compiler cannot protect from using an unsupported type. Also then method users don't see the allowed types instantly in intellisense.
	/// 3. Using the Object type for the parameter. However then the compiler does not protect from using an unsupported type. Also then method users don't see the allowed types instantly in intellisense. Also slower, especially with value types (need boxing).
	/// 4. Using this type for the parameter. It does not have the above problems.
	/// 
	/// When a parameter is of type Types&lt;T1, T2&gt;, method users can pass only values of type T1 or T2, else compiler error. They see the allowed types instantly in intellisense.
	/// Also there are similar types that can be used to support more parameter types:
	/// <see cref="Types{T1, T2, T3}"/>
	/// <see cref="Types{T1, T2, T3, T4}"/>
	/// 
	/// To support null (for example for optional parameters), use nullable. See examples.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// static void Example1(Types<string, IEnumerable<string>> x)
	/// {
	/// 	switch(x.type) {
	/// 	case 1: PrintList("string", x.v1); break;
	/// 	case 2: PrintList("IEnumerable<string>", x.v2); break;
	/// 	default: throw new ArgumentException(); //0 if has default value, eg assigned default(Types<string, IEnumerable<string>>), unlikely
	/// 	}
	/// }
	/// 
	/// static void Example2(int param1, Types<int, double>? optionalParam = null)
	/// {
	/// 	var p = optionalParam.GetValueOrDefault();
	/// 	switch(p.type) {
	/// 	case 0: Print("null"); break;
	/// 	case 1: PrintList("int", p.v1); break;
	/// 	case 2: PrintList("double", p.v2); break;
	/// 	}
	/// }
	/// 
	/// static void TestExamples()
	/// {
	/// 	Example1("S");
	/// 	Example1(new string[] { "a", "b" });
	/// 	//Example1(5); //compiler error
	/// 	//Example1(null); //compiler error
	/// 	Example1((string)null);
	/// 	//Example1(default(Types<string, IEnumerable<string>>)); //the function throws exception
	/// 
	/// 	Example2(0, 5);
	/// 	Example2(0, 5.5);
	/// 	//Example2(0, "S"); //compiler error
	/// 	Example2(0, null);
	/// 	Example2(0);
	/// 	//Example2(0, default(Types<int, double>)); //the function throws exception
	/// }
	/// ]]></code>
	/// </example>
	[DebuggerStepThrough]
	[StructLayout(LayoutKind.Auto)]
	public struct Types<T1, T2>
	{
		/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), 0 if unassigned. </summary>
		public byte type;
		/// <summary> Value of type T1. Valid when type is 1. </summary>
		public T1 v1;
		/// <summary> Value of type T2. Valid when type is 2. </summary>
		public T2 v2;

		Types(T1 v) { type = 1; v1 = v; v2 = default; }
		Types(T2 v) { type = 2; v2 = v; v1 = default; }

		/// <summary> Assignment of a value of type T1. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)] //makes caller's native code much smaller, although slightly slower
		public static implicit operator Types<T1, T2>(T1 x) { return new Types<T1, T2>(x); }
		/// <summary> Assignment of a value of type T2. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2>(T2 x) { return new Types<T1, T2>(x); }

		//Prevent assigning null to a non-nullable Types variable when one of its types is a reference type, eg Types<string, int>.
		//Compiler allows to assign null to a struct only if exactly one operator is of a reference type.
		//This creates two problems: 1. Some functions allow null, some not. 2. Some type combinations allow null, some not.
		//Now compiler error if caller tries to pass null. If need to support null, let use nullable.
		//Need two operators because with single operator would allow to assign null when all value types, eg Types<double, int>.
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2>(JustNull n) => default;
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2>(JustNull2 n) => default;
	}

	/// <summary>
	/// Used for method parameters that accept one of three types.
	/// More info: <see cref="Types{T1, T2}"/>.
	/// </summary>
	[DebuggerStepThrough]
	[StructLayout(LayoutKind.Auto)]
	public struct Types<T1, T2, T3>
	{
		/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), 3 if T3 (v3 is valid), 0 if unassigned (unlikely). </summary>
		public byte type;
		/// <summary> Value of type T1. Valid when type is 1. </summary>
		public T1 v1;
		/// <summary> Value of type T2. Valid when type is 2. </summary>
		public T2 v2;
		/// <summary> Value of type T3. Valid when type is 3. </summary>
		public T3 v3;

		Types(T1 v) : this() { type = 1; v1 = v; }
		Types(T2 v) : this() { type = 2; v2 = v; }
		Types(T3 v) : this() { type = 3; v3 = v; }

		/// <summary> Assignment of a value of type T1. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2, T3>(T1 x) { return new Types<T1, T2, T3>(x); }
		/// <summary> Assignment of a value of type T2. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2, T3>(T2 x) { return new Types<T1, T2, T3>(x); }
		/// <summary> Assignment of a value of type T3. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2, T3>(T3 x) { return new Types<T1, T2, T3>(x); }

		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3>(JustNull n) => default;
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3>(JustNull2 n) => default;
	}

	/// <summary>
	/// Used for method parameters that accept one of four types.
	/// More info: <see cref="Types{T1, T2}"/>.
	/// </summary>
	[DebuggerStepThrough]
	[StructLayout(LayoutKind.Auto)]
	public struct Types<T1, T2, T3, T4>
	{
		/// <summary> Value type. 1 if T1 (v1 is valid), 2 if T2 (v2 is valid), and so on. 0 if unassigned (unlikely). </summary>
		public byte type;
		/// <summary> Value of type T1. Valid when type is 1. </summary>
		public T1 v1;
		/// <summary> Value of type T2. Valid when type is 2. </summary>
		public T2 v2;
		/// <summary> Value of type T3. Valid when type is 3. </summary>
		public T3 v3;
		/// <summary> Value of type T4. Valid when type is 4. </summary>
		public T4 v4;

		Types(T1 v) : this() { type = 1; v1 = v; }
		Types(T2 v) : this() { type = 2; v2 = v; }
		Types(T3 v) : this() { type = 3; v3 = v; }
		Types(T4 v) : this() { type = 4; v4 = v; }

		/// <summary> Assignment of a value of type T1. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2, T3, T4>(T1 x) { return new Types<T1, T2, T3, T4>(x); }
		/// <summary> Assignment of a value of type T2. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2, T3, T4>(T2 x) { return new Types<T1, T2, T3, T4>(x); }
		/// <summary> Assignment of a value of type T3. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2, T3, T4>(T3 x) { return new Types<T1, T2, T3, T4>(x); }
		/// <summary> Assignment of a value of type T4. </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types<T1, T2, T3, T4>(T4 x) { return new Types<T1, T2, T3, T4>(x); }

		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3, T4>(JustNull n) => default;
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types<T1, T2, T3, T4>(JustNull2 n) => default;
	}

#if false
	//This varsion also works well.
	//Good: smaller calling code. Very fast for reference types.
	//Bad: need boxing for value types. It is quite fast (~50% slower), but in some cases can create much garbage.
	[DebuggerStepThrough]
	[StructLayout(LayoutKind.Auto)]
	public struct Types2<T1, T2>
	{
		/// <summary> The assigned value. </summary>
		public object Value;

		/// <summary> Assignment of a value of type T1. </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)] //makes caller's native code much smaller, but then speed same as class or worse
		public static implicit operator Types2<T1, T2>(T1 x) { return new Types2<T1, T2>() { Value = x }; }
		/// <summary> Assignment of a value of type T2. </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)]
		public static implicit operator Types2<T1, T2>(T2 x) { return new Types2<T1, T2>() { Value = x }; }

		//Prevent assigning null to a non-nullable Types variable when one of its types is a reference type, eg Types<string, int>.
		//Compiler allows to assign null to a struct only if exactly one operator is of a reference type.
		//This creates two problems: 1. Some functions allow null, some not. 2. Some type combinations allow null, some not.
		//Now compiler error if caller tries to pass null. If need to support null, let use nullable.
		//Need two operators because with single operator would allow to assign null when all value types, eg Types<double, int>.
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types2<T1, T2>(CatkeysPrivate.JustNull n) => default;
		/// <summary>Infrastructure.</summary>
		public static implicit operator Types2<T1, T2>(CatkeysPrivate.JustNull2 n) => default;
	}
#endif

	/// <summary>
	/// Contains x or y coordinate. Used for parameters of functions like Mouse.Move, Wnd.Move.
	/// Allows to easily specify coordinates of these types: normal, reverse (from right or bottom of a rectangle), fractional (fraction of width or height of a rectangle), null.
	/// Also has functions to convert to normal coodinates.
	/// </summary>
	[DebuggerStepThrough]
	public struct Coord
	{
		/// <summary>
		/// Coord variable value type.
		/// </summary>
		public enum CoordType
		{
			/// <summary>
			/// No value. Assigned null or default(Coord).
			/// </summary>
			None,

			/// <summary>
			/// <see cref="Value"/> is pixel offset from left or top of a rectangle.
			/// </summary>
			Normal,

			/// <summary>
			/// <see cref="Value"/> is pixel offset from right or bottom of a rectangle, towards left or top.
			/// </summary>
			Reverse,

			/// <summary>
			/// <see cref="FractionValue"/> is fraction of a rectangle, where 0.0 is left or top, and 1.0 is right or bottom (outside of the rectangle).
			/// </summary>
			Fraction,
		}

		//Use single long field that packs int and CoordType.
		//If 2 fields (int and CoordType), 64-bit compiler creates huge calling code.
		//This version is good in 32-bit, very good in 64-bit. Even better than packing in single int (30 bits value and 2 bits type).
		//Don't use struct or union with both int and float fields. It creates slow and huge calling code.
		long _v;

		/// <summary>
		/// Value type.
		/// </summary>
		public CoordType Type { get => (CoordType)(_v >> 32); }

		/// <summary>
		/// Non-fraction value.
		/// </summary>
		public int Value { get => (int)_v; }

		/// <summary>
		/// Fraction value.
		/// </summary>
		public unsafe float FractionValue { get { int i = Value; return *(float*)&i; } }

		/// <summary>
		/// Returns true if Type == None (when assigned null or default(Coord)).
		/// </summary>
		public bool IsEmpty { get => Type == CoordType.None; }

		Coord(CoordType type, int value) { _v = ((long)type << 32) | (uint)value; }
		//info: if type and value are constants, compiler knows the final value and does not use the << and | operators in the compiled code.

		/// <summary>
		/// Creates Coord of Normal type.
		/// </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)] //makes bigger/slower
		public static implicit operator Coord(int v) => new Coord(CoordType.Normal, v);

		//rejected. Would be used when assigning uint, long, ulong. Or need functions for these too.
		///// <summary>
		///// Creates Coord of Fraction type.
		///// </summary>
		//public static implicit operator Coord(float v) => Fraction(v);

		/// <summary>
		/// Infrastructure. Allows to assign null.
		/// </summary>
		/// <param name="v">null.</param>
		public static implicit operator Coord(JustNull v) => new Coord();
		//TODO: remove. Now we have 'default'.

		/// <summary>
		/// Creates Coord of Reverse type.
		/// Value 0 is at the right or bottom, and does not belong to the rectangle. Positive values are towards left or top.
		/// </summary>
		public static Coord Reverse(int v)
		{
			return new Coord(CoordType.Reverse, v);
		}

		/// <summary>
		/// Creates Coord of Fraction type.
		/// Value 0.0 is the left or top of the rectangle. Value 1.0 is the right or bottom of the rectangle. Values &lt;0.0 and &gt;=1.0 are outside of the rectangle.
		/// </summary>
		public static unsafe Coord Fraction(double v)
		{
			float f = (float)v;
			return new Coord(CoordType.Fraction, *(int*)&f);
		}

		/// <summary>
		/// Returns <see cref="Fraction"/>(0.5).
		/// </summary>
		public static Coord Center { get => Fraction(0.5); }

		/// <summary>
		/// Returns <see cref="Reverse"/>(0).
		/// This point will be outside of the rectangle. See also <see cref="MaxInside"/>.
		/// </summary>
		public static Coord Max { get => Reverse(0); }

		/// <summary>
		/// Returns <see cref="Reverse"/>(1).
		/// This point will be inside of the rectangle, at the very right or bottom, assuming that the rectangle is not empty.
		/// </summary>
		public static Coord MaxInside { get => Reverse(1); }

		//rejected: this could be used like Coord.Max + 1. Too limited usage.
		//public static Coord operator +(Coord c, int i) { return ...; }

		static bool _NeedRect(Coord x, Coord y)
		{
			return (x.Type > CoordType.Normal) || (y.Type > CoordType.Normal);
		}

		int _Normalize(int min, int max)
		{
			switch(Type) {
			case CoordType.Normal: return min + Value;
			case CoordType.Reverse: return max - Value; //info: could subtract 1, then 0 would be in rectangle, but in some contexts it is not good
			case CoordType.Fraction: return min + (int)((max - min) * FractionValue);
			default: return 0;
			}
		}

		/// <summary>
		/// Converts fractional/reverse coordinates to normal coordinates in a rectangle.
		/// </summary>
		/// <param name="x">X coordinate relative to r. If default(Coord), returns 0 x.</param>
		/// <param name="y">Y coordinate relative to r. If default(Coord), returns 0 y.</param>
		/// <param name="r">The rectangle.</param>
		/// <param name="widthHeight">Use only width and height of r. If false, the function adds r offset (left and top).</param>
		public static Point NormalizeInRect(Coord x, Coord y, RECT r, bool widthHeight = false)
		{
			if(widthHeight) r.Offset(-r.left, -r.top);
			return new Point(x._Normalize(r.left, r.right), y._Normalize(r.top, r.bottom));
		}
		//CONSIDER: if default(Coord), use center.

		/// <summary>
		/// Returns normal coordinates relative to the primary screen. Converts fractional/reverse coordinates etc.
		/// </summary>
		/// <param name="x">X coordinate relative to the specified screen (default - primary). If default(Coord), returns 0 x.</param>
		/// <param name="y">Y coordinate relative to the specified screen (default - primary). If default(Coord), returns 0 y.</param>
		/// <param name="workArea">x y are relative to the work area, not entire screen.</param>
		/// <param name="screen">Screen of x y. If null, primary screen. See <see cref="Screen_.FromObject"/>.</param>
		/// <param name="widthHeight">Use only width and height of the screen rectangle. If false, the function adds its offset (left and top, which can be nonzero if using the work area or a non-primary screen).</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		public static Point Normalize(Coord x, Coord y, bool workArea = false, object screen = null, bool widthHeight = false)
		{
			var p = new Point();
			if(x.Type != CoordType.None || y.Type != CoordType.None) {
				Screen s = (screen == null) ? null : Screen_.FromObject(screen);
				RECT r;
				if(workArea || s != null || _NeedRect(x, y)) {
					if(s != null) r = workArea ? s.WorkingArea : s.Bounds;
					else r = workArea ? Screen_.WorkArea : Screen_.Rect;
					if(widthHeight) r.Offset(-r.left, -r.top);
				} else r = new RECT();
				p.X = x._Normalize(r.left, r.right);
				p.Y = y._Normalize(r.top, r.bottom);
			}
			return p;
		}

		/// <summary>
		/// Returns normal coordinates relative to the client area of a window. Converts fractional/reverse coordinates etc.
		/// </summary>
		/// <param name="x">X coordinate relative to the client area of w. If default(Coord), returns 0 x.</param>
		/// <param name="y">Y coordinate relative to the client area of w. If default(Coord), returns 0 y.</param>
		/// <param name="w">The window.</param>
		/// <param name="nonClient">x y are relative to the entire w rectangle, not to its client area.</param>
		public static Point NormalizeInWindow(Coord x, Coord y, Wnd w, bool nonClient = false)
		{
			//info: don't need widthHeight parameter because client area left/top are 0. With non-client don't need in this library and probably not useful. But if need, caller can explicitly offset the rect before calling this func.

			var p = new Point();
			if(x.Type != CoordType.None || y.Type != CoordType.None) {
				RECT r;
				if(nonClient) {
					w.GetRectInClientOf(w, out r);
				} else if(_NeedRect(x, y)) {
					r = w.ClientRect;
				} else r = new RECT();
				p.X = x._Normalize(r.left, r.right);
				p.Y = y._Normalize(r.top, r.bottom);
			}
			return p;
		}

		///
		public override string ToString()
		{
			switch(Type) {
			case CoordType.Normal: return Value.ToString() + ", Normal";
			case CoordType.Reverse: return Value.ToString() + ", Reverse";
			case CoordType.Fraction: return FractionValue.ToString_() + ", Fraction";
			default: return "null";
			}
		}
	}

	/// <summary>
	/// Contains a value of one of two types - Wnd or Control.
	/// Has implicit conversion operators: from Wnd and from Control. Also can be assigned null.
	/// Often used for function parameters that support both these types. You can pass Control or Wnd variables to such functions directly.
	/// </summary>
	[DebuggerStepThrough]
	public struct WndOrControl
	{
		object _o;

		/// <summary> Assignment of a value of type Wnd. </summary>
		public static implicit operator WndOrControl(Wnd w) { return new WndOrControl() { _o = w }; }
		/// <summary> Assignment of a value of type Control. </summary>
		public static implicit operator WndOrControl(Control c) { return new WndOrControl() { _o = c }; }

		/// <summary>
		/// Gets the window or control handle as Wnd.
		/// Returns default(Wnd) if null or nothing was assigned.
		/// </summary>
		public Wnd Wnd
		{
			[MethodImpl(MethodImplOptions.NoInlining)] //prevents loading Forms.dll when don't need
			get
			{
				if(_o == null) return default;
				if(_o is Control c) return (Wnd)c;
				return (Wnd)_o;
			}
		}

		/// <summary>
		/// true if assigned null or nothing.
		/// </summary>
		public bool IsNull { get => _o == null; }
	}

#if false //currently not used. Created for TaskDialog.ShowList, but used object instead.
	/// <summary>
	/// Contains a value of one of two types - string or IEnumerable&lt;string&gt;.
	/// Has implicit conversion operators: from string and from IEnumerable&lt;string&gt;.
	/// Often used for function parameters that support both these types.
	/// </summary>
	[DebuggerStepThrough]
	public struct StringOrList
	{
		/// <summary>
		/// The assigned value. It can be string, IEnumerable&lt;string&gt; or null.
		/// </summary>
		public object Value;

		/// <summary> string assignment. </summary>
		public static implicit operator StringOrList(string s) { return new StringOrList() { _o = s }; }
		///// <summary> IEnumerable&lt;string&gt; assignment. </summary>
		//public static implicit operator StringOrList(IEnumerable<string> e) { return new StringOrList() { _o = e }; } //error: "user-defined conversions to/from interfaces not allowed". Then why allowed when using generic?
		/// <summary> string[] assignment. </summary>
		public static implicit operator StringOrList(string[] e) { return new StringOrList() { _o = e }; }
		/// <summary> List&lt;string&gt; assignment. </summary>
		public static implicit operator StringOrList(List<string> e) { return new StringOrList() { _o = e }; }
	}
#endif
}
