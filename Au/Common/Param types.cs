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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using static Au.NoClass;

namespace Au.Types
{
	/// <summary>
	/// This namespace contains:
	/// <list type="bullet">
	/// <item>Types of function parameters and return values.</item>
	/// <item>Exception classes, some base classes, some other types.</item>
	/// <item>Extension methods for various .NET classes.</item>
	/// </list>
	/// </summary>
	[CompilerGenerated()]
	class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	//currently not used
	///// <summary>Infrastructure.</summary>
	///// <tocexclude />
	//[EditorBrowsable(EditorBrowsableState.Never)]
	//public class JustNull { }

	///// <summary>Infrastructure.</summary>
	///// <tocexclude />
	//[EditorBrowsable(EditorBrowsableState.Never)]
	//public class JustNull2 { }

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
			/// No value. The variable is default(Coord).
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
		public CoordType Type => (CoordType)(_v >> 32);

		/// <summary>
		/// Non-fraction value.
		/// </summary>
		public int Value => (int)_v;

		/// <summary>
		/// Fraction value.
		/// </summary>
		public unsafe float FractionValue { get { int i = Value; return *(float*)&i; } }

		/// <summary>
		/// Returns true if Type == None (when assigned default(Coord)).
		/// </summary>
		public bool IsEmpty => Type == CoordType.None;

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
		/// Returns <c>Fraction(0.5)</c>.
		/// </summary>
		/// <seealso cref="Fraction"/>
		public static Coord Center => Fraction(0.5);

		/// <summary>
		/// Returns <c>Reverse(0)</c>.
		/// This point will be outside of the rectangle. See also <see cref="MaxInside"/>.
		/// </summary>
		/// <seealso cref="Reverse"/>
		public static Coord Max => Reverse(0);

		/// <summary>
		/// Returns <c>Reverse(1)</c>.
		/// This point will be inside of the rectangle, at the very right or bottom, assuming the rectangle is not empty.
		/// </summary>
		/// <seealso cref="Reverse"/>
		public static Coord MaxInside => Reverse(1);

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
		/// <param name="x">X coordinate relative to r.</param>
		/// <param name="y">Y coordinate relative to r.</param>
		/// <param name="r">The rectangle.</param>
		/// <param name="widthHeight">Use only width and height of r. If false, the function adds r offset (left and top).</param>
		/// <param name="centerIfEmpty">If x or y is default(Coord), use Coord.Center. Not used with widthHeight.</param>
		public static Point NormalizeInRect(Coord x, Coord y, RECT r, bool widthHeight = false, bool centerIfEmpty = false)
		{
			if(widthHeight) r.Offset(-r.left, -r.top);
			else if(centerIfEmpty) {
				if(x.IsEmpty) x = Center;
				if(y.IsEmpty) y = Center;
			}
			return new Point(x._Normalize(r.left, r.right), y._Normalize(r.top, r.bottom));
		}

		/// <summary>
		/// Returns normal coordinates relative to the client area of a window. Converts fractional/reverse coordinates etc.
		/// </summary>
		/// <param name="x">X coordinate relative to the client area of w.</param>
		/// <param name="y">Y coordinate relative to the client area of w.</param>
		/// <param name="w">The window.</param>
		/// <param name="nonClient">x y are relative to the entire w rectangle, not to its client area.</param>
		/// <param name="centerIfEmpty">If x or y is default(Coord), use Coord.Center.</param>
		public static Point NormalizeInWindow(Coord x, Coord y, Wnd w, bool nonClient = false, bool centerIfEmpty = false)
		{
			//info: don't need widthHeight parameter because client area left/top are 0. With non-client don't need in this library and probably not useful. But if need, caller can explicitly offset the rect before calling this func.

			if(centerIfEmpty) {
				if(x.IsEmpty) x = Center;
				if(y.IsEmpty) y = Center;
			}
			Point p = default;
			if(!x.IsEmpty || !y.IsEmpty) {
				RECT r;
				if(nonClient) {
					w.GetRectInClientOf(w, out r);
				} else if(_NeedRect(x, y)) {
					r = w.ClientRect;
				} else r = default;
				p.X = x._Normalize(r.left, r.right);
				p.Y = y._Normalize(r.top, r.bottom);
			}
			return p;
		}

		/// <summary>
		/// Returns normal coordinates relative to the primary screen. Converts fractional/reverse coordinates etc.
		/// </summary>
		/// <param name="x">X coordinate relative to the specified screen (default - primary).</param>
		/// <param name="y">Y coordinate relative to the specified screen (default - primary).</param>
		/// <param name="co">Can be used to specify screen (see <see cref="Screen_.FromObject"/>) and/or whether x y are relative to the work area.</param>
		/// <param name="widthHeight">Use only width and height of the screen rectangle. If false, the function adds its offset (left and top, which can be nonzero if using the work area or a non-primary screen).</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		public static Point Normalize(Coord x, Coord y, CoordOptions co = null, bool widthHeight = false)
		{
			Point p = default;
			if(!x.IsEmpty || !y.IsEmpty) {
				object scr = co?.Screen;
				bool workArea = co?.WorkArea ?? false;
				RECT r;
				if(workArea || scr != null || _NeedRect(x, y)) {
					r = Screen_.GetRect(scr, workArea);
					if(widthHeight) r.Offset(-r.left, -r.top);
				} else r = default;
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
			default: return "default";
			}
		}
	}

	/// <summary>
	/// Screen and other options for functions that accept coordinates as <see cref="Coord"/>.
	/// </summary>
	public class CoordOptions
	{
		/// <summary>
		/// Screen index or anything that can be converted to <see cref="Screen"/> with <see cref="Screen_.FromObject"/>.
		/// </summary>
		public object Screen { get; set; }

		/// <summary>
		/// If true, coordinates are relative to the work area, not to whole screen.
		/// </summary>
		public bool WorkArea { get; set; }

		//rejected: too much options, rarely used.
		//public RECT? Rect { get; set; }

		///
		public CoordOptions() { }

		///
		public CoordOptions(bool workArea, object screen = null)
		{
			WorkArea = workArea;
			Screen = screen;
		}

		//CONSIDER: implicit conversion operators.
	}

	/// <summary>
	/// Window handle.
	/// Used for function parameters where the function needs a window handle as <see cref="Au.Wnd"/> but also allows to pass a variable of any of these types: System.Windows.Forms.Control (Form or any control class), System.Windows.Window (WPF window), IntPtr (window handle).
	/// </summary>
	[DebuggerStepThrough]
	public struct AnyWnd
	{
		object _o;
		AnyWnd(object o) { _o = o; }

		/// <summary> Assignment of a value of type Wnd. </summary>
		public static implicit operator AnyWnd(Wnd w) => new AnyWnd(w);
		/// <summary> Assignment of a value of type Wnd. </summary>
		public static implicit operator AnyWnd(IntPtr hwnd) => new AnyWnd((Wnd)hwnd);
		/// <summary> Assignment of a value of type System.Windows.Forms.Control (Form or any control class). </summary>
		public static implicit operator AnyWnd(Control c) => new AnyWnd(c);
		/// <summary> Assignment of a value of type System.Windows.Window (WPF window). </summary>
		public static implicit operator AnyWnd(System.Windows.Window w) => new AnyWnd(new System.Windows.Interop.WindowInteropHelper(w));

		/// <summary>
		/// Gets the window or control handle as Wnd.
		/// Returns default(Wnd) if not assigned.
		/// </summary>
		public Wnd Wnd
		{
			[MethodImpl(MethodImplOptions.NoInlining)] //prevents loading Forms dll when don't need
			get
			{
				if(_o == null) return default;
				if(_o is Wnd) return (Wnd)_o;
				if(_o is Control c) return (Wnd)c;
				return _Wpf();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)] //prevents loading WPF dlls when don't need
		Wnd _Wpf() => (Wnd)((System.Windows.Interop.WindowInteropHelper)_o).Handle;

		/// <summary>
		/// true if not assigned.
		/// </summary>
		public bool IsEmpty => _o == null;
	}

#if false //currently not used
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
