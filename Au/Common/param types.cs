using Au.Util;
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
//using System.Linq;

namespace Au.Types
{
#if false //rejected. More convenient to use, but makes code less clear. With Range everything is clear: null means "all"; for just start index use 'i..'; many users know Range, and would have to learn about ARange.
	/// <summary>
	/// Represents a range that has start and end indexes.
	/// </summary>
	/// <remarks>
	/// Similar to <see cref="Range"/>. Main differences:
	/// 1. The default value is whole range (like <see cref="Range.All"/>), not empty range.
	/// 2. Has implicit conversions from int and <see cref="Index"/>. It sets the start.
	/// 
	/// Used for parameters of functions that allow to specify a range (part) of a string, array or other collection. Callers can specify a range or just start index. Or callers can omit the optional parameter to use whole collection. The called function retrieves real start/end indexes with <see cref="GetRealRange"/>.
	/// </remarks>
	public struct ARange : IEquatable<ARange>
	{
		int _start;
		int _endPlus1;

		/// <summary>
		/// Initializes a new <see cref="ARange"/> instance with the specified starting and ending indexes.
		/// </summary>
		/// <param name="start">The start index of the range.</param>
		/// <param name="end">The end index of the range. The default value -1 means that the end index will be equal to the length of the collection (string, array, etc) with which will be used this variable.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>start</i> is less than 0 or <i>end</i> is less than -1.</exception>
		public ARange(int start, int end = -1)
		{
			if(start < 0 || end < -1) throw new ArgumentOutOfRangeException();
			_start = start;
			_endPlus1 = end + 1;
		}

		/// <summary>
		/// Initializes a new <see cref="ARange"/> instance with the specified starting and ending indexes specified using <b>Index</b>.
		/// </summary>
		public ARange(Index start, Index end)
		{
			_start = start.IsFromEnd ? ~start.Value : start.Value;
			_endPlus1 = (end.IsFromEnd ? ~end.Value : end.Value) + 1;
		}

		//rejected. Use GetRealRange instead. Now caller may not know how to interpret negative values.
		//public int Start {
		//	get => _start;
		//	//set => _start = value; //not useful. Let it be immutable, like Range.
		//}

		//public int End {
		//	get => _endPlus1 - 1;
		//	//set => _endPlus1 = value + 1;
		//}

		/// <summary>
		/// Gets real start and end offsets in a collection (string, array, etc) of specified length.
		/// </summary>
		/// <param name="length">Length of collection with which will be used this range.</param>
		/// <exception cref="ArgumentOutOfRangeException">The result range is invalid: start is less than 0 or greater than <i>length</i>, or end is less than start or greater than length.</exception>
		public (int start, int end) GetRealRange(int length)
		{
			//if(_start == 0 && _endPlus1 == 0) return (0, length); //does not make faster
			int start = _start >= 0 ? _start : length + _start + 1;
			int end = _endPlus1 > 0 ? _endPlus1 - 1 : length + _endPlus1;
			if((uint)end > (uint)length || (uint)start > (uint)end) throw new ArgumentOutOfRangeException();
			return (start, end);
		}

		///
		public static implicit operator ARange(int start) => new ARange(start);

		///
		public static implicit operator ARange(Range r) => new ARange(r.Start, r.End);

		///
		public static implicit operator ARange(Index start) => new ARange(start, Index.End);

		///
		public override string ToString()
		{
			string op1 = _start >= 0 ? null : "^";
			string num1 = _start == 0 ? null : (_start > 0 ? _start : ~_start).ToString();
			string op2 = _endPlus1 >= 0 ? null : "^";
			int end = _endPlus1 - 1;
			string num2 = _endPlus1 == 0 ? null : (_endPlus1 > 0 ? end : ~end).ToString();
			return op1 + num1 + ".." + op2 + num2;
		}

		///
		public override bool Equals(object value) => value is ARange r && Equals(r);

		///
		public bool Equals(ARange other) => other._start == _start && other._endPlus1 == _endPlus1;

		///
		public override int GetHashCode() => HashCode.Combine(_start.GetHashCode(), _endPlus1.GetHashCode());
	}
#endif

	/// <summary>
	/// Contains x or y coordinate in screen or some other rectangle that can be specified in various ways: normal, reverse, fraction, center, max.
	/// Used for parameters of functions like <see cref="AMouse.Move"/>, <see cref="AWnd.Move"/>.
	/// </summary>
	/// <remarks>
	/// To specify a normal coordinate, assign an <b>int</b> value (implicit conversion from <b>int</b> to <b>Coord</b>). Else use static functions such as <b>Reverse</b>, <b>Fraction</b> (or assign float), <b>Center</b>, <b>Max</b>, <b>MaxInside</b>.
	/// Also there are functions to convert <b>Coord</b> to normal coodinates.
	/// </remarks>
	[DebuggerStepThrough]
	public struct Coord : IEquatable<Coord>
	{
		//Use single long field that packs int and CoordType.
		//If 2 fields (int and CoordType), 64-bit compiler creates huge calling code.
		//This version is good in 32-bit, very good in 64-bit. Even better than packing in single int (30 bits value and 2 bits type).
		//Don't use struct or union with both int and float fields. It creates slow and huge calling code.
		readonly long _v;

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

		/// <summary>
		/// Creates Coord of Fraction type.
		/// </summary>
		public static implicit operator Coord(float v) => Fraction(v);

		///
		[Obsolete("The value must be of type int or float.", error: true), NoDoc]
		public static implicit operator Coord(uint f) => default;
		///
		[Obsolete("The value must be of type int or float.", error: true), NoDoc]
		public static implicit operator Coord(long f) => default;
		///
		[Obsolete("The value must be of type int or float.", error: true), NoDoc]
		public static implicit operator Coord(ulong f) => default;

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
		/// Instead can be used implicit conversion from float, for example argument <c>Coord.Fraction(0.5)</c> can be replaced with <c>0.5f</c>.
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

		/// <summary>
		/// Converts fractional/reverse coordinate to normal coordinate in a range.
		/// </summary>
		/// <param name="start">Start of range.</param>
		/// <param name="end">End of range.</param>
		public int NormalizeInRange(int start, int end)
		{
			return Type switch
			{
				CoordType.Normal => start + Value,
				CoordType.Reverse => end - Value,
				CoordType.Fraction => start + (int)((end - start) * FractionValue),
				_ => 0,
			};
		}

		/// <summary>
		/// Converts fractional/reverse coordinates to normal coordinates in a rectangle.
		/// </summary>
		/// <param name="x">X coordinate relative to r.</param>
		/// <param name="y">Y coordinate relative to r.</param>
		/// <param name="r">The rectangle.</param>
		/// <param name="widthHeight">Use only width and height of r. If false (default), the function adds r offset (left and top).</param>
		/// <param name="centerIfEmpty">If x or y is default(Coord), use Coord.Center. Not used with widthHeight.</param>
		public static POINT NormalizeInRect(Coord x, Coord y, RECT r, bool widthHeight = false, bool centerIfEmpty = false)
		{
			if(widthHeight) r.Offset(-r.left, -r.top);
			else if(centerIfEmpty) {
				if(x.IsEmpty) x = Center;
				if(y.IsEmpty) y = Center;
			}
			return (x.NormalizeInRange(r.left, r.right), y.NormalizeInRange(r.top, r.bottom));
		}

		/// <summary>
		/// Returns normal coordinates relative to the client area of a window. Converts fractional/reverse coordinates etc.
		/// </summary>
		/// <param name="x">X coordinate relative to the client area of w.</param>
		/// <param name="y">Y coordinate relative to the client area of w.</param>
		/// <param name="w">The window.</param>
		/// <param name="nonClient">x y are relative to the entire w rectangle, not to its client area.</param>
		/// <param name="centerIfEmpty">If x or y is default(Coord), use Coord.Center.</param>
		public static POINT NormalizeInWindow(Coord x, Coord y, AWnd w, bool nonClient = false, bool centerIfEmpty = false)
		{
			//info: don't need widthHeight parameter because client area left/top are 0. With non-client don't need in this library and probably not useful. But if need, caller can explicitly offset the rect before calling this func.

			if(centerIfEmpty) {
				if(x.IsEmpty) x = Center;
				if(y.IsEmpty) y = Center;
			}
			POINT p = default;
			if(!x.IsEmpty || !y.IsEmpty) {
				RECT r;
				if(nonClient) {
					w.GetRectIn(w, out r);
				} else if(_NeedRect(x, y)) {
					r = w.ClientRect;
				} else r = default;
				p.x = x.NormalizeInRange(r.left, r.right);
				p.y = y.NormalizeInRange(r.top, r.bottom);
			}
			return p;
		}

		/// <summary>
		/// Returns normal coordinates relative to the primary screen. Converts fractional/reverse coordinates etc.
		/// </summary>
		/// <param name="x">X coordinate relative to the specified screen (default - primary).</param>
		/// <param name="y">Y coordinate relative to the specified screen (default - primary).</param>
		/// <param name="workArea">x y are relative to the work area.</param>
		/// <param name="screen">If used, x y are relative to this screen. Default - primary screen.</param>
		/// <param name="widthHeight">Use only width and height of the screen rectangle. If false, the function adds its offset (left and top, which can be nonzero if using the work area or a non-primary screen).</param>
		/// <param name="centerIfEmpty">If x or y is default(Coord), use Coord.Center.</param>
		public static POINT Normalize(Coord x, Coord y, bool workArea = false, AScreen screen = default, bool widthHeight = false, bool centerIfEmpty = false)
		{
			if(centerIfEmpty) {
				if(x.IsEmpty) x = Center;
				if(y.IsEmpty) y = Center;
			}
			POINT p = default;
			if(!x.IsEmpty || !y.IsEmpty) {
				RECT r;
				if(workArea || !screen.IsNull || _NeedRect(x, y)) {
					r = screen.GetScreenHandle().GetRect(workArea);
					if(widthHeight) r.Offset(-r.left, -r.top);
				} else r = default;
				p.x = x.NormalizeInRange(r.left, r.right);
				p.y = y.NormalizeInRange(r.top, r.bottom);
			}
			return p;
		}

		///
		public override string ToString()
		{
			switch(Type) {
			case CoordType.Normal: return Value.ToString() + ", Normal";
			case CoordType.Reverse: return Value.ToString() + ", Reverse";
			case CoordType.Fraction: return FractionValue.ToStringInvariant() + ", Fraction";
			default: return "default";
			}
		}

		///
		public bool Equals(Coord other) => other._v == _v; //IEquatable
		///
		public static bool operator ==(Coord a, Coord b) => a._v == b._v;
		///
		public static bool operator !=(Coord a, Coord b) => a._v != b._v;
		///
		public override int GetHashCode() => _v.GetHashCode();
	}

	/// <summary>
	/// <see cref="Coord"/> variable value type.
	/// </summary>
	public enum CoordType
	{
		/// <summary>
		/// No value. The variable is default(Coord).
		/// </summary>
		None,

		/// <summary>
		/// <see cref="Coord.Value"/> is pixel offset from left or top of a rectangle.
		/// </summary>
		Normal,

		/// <summary>
		/// <see cref="Coord.Value"/> is pixel offset from right or bottom of a rectangle, towards left or top.
		/// </summary>
		Reverse,

		/// <summary>
		/// <see cref="Coord.FractionValue"/> is fraction of a rectangle, where 0.0 is left or top, and 1.0 is right or bottom (outside of the rectangle).
		/// </summary>
		Fraction,
	}

	/// <summary>
	/// Can be used to specify coordinates for various popup windows, like <c>new PopupXY(x, y)</c>, <c>(x, y)</c>, <c>PopupXY.In(rectangle)</c>, <c>PopupXY.Mouse</c>.
	/// </summary>
	public class PopupXY
	{
#pragma warning disable 1591 //XML doc
		public Coord x, y;
		public AScreen screen;
		public bool workArea;
		public bool inRect;
		public RECT rect;
#pragma warning restore 1591 //XML doc

		/// <summary>
		/// Sets position and/or screen.
		/// </summary>
		/// <param name="x">X relative to the screen or work area. Default - center.</param>
		/// <param name="y">X relative to the screen or work area. Default - center.</param>
		/// <param name="workArea">x y are relative to the work area of the screen.</param>
		/// <param name="screen">Can be used to specify a screen. Default - primary.</param>
		/// <remarks>
		/// Also there is are implicit conversions from tuple (x, y) and POINT. Instead of <c>new PopupXY(x, y)</c> you can use <c>(x, y)</c>. Instead of <c>new PopupXY(p.x, p.y, false)</c> you can use <c>p</c> or <c>(POINT)p</c> .
		/// </remarks>
		public PopupXY(Coord x = default, Coord y = default, bool workArea = true, AScreen screen = default)
		{
			this.x = x; this.y = y; this.workArea = workArea; this.screen = screen;
		}

		/// <summary>
		/// Creates new <b>PopupXY</b> that specifies position in a rectangle. For example of the owner window, like <c>PopupXY.In(myForm.Bounds)</c>.
		/// </summary>
		/// <param name="r">Rectangle relative to the primary screen.</param>
		/// <param name="x">X relative to the rectangle. Default - center.</param>
		/// <param name="y">Y relative to the rectangle. Default - center.</param>
		public static PopupXY In(RECT r, Coord x = default, Coord y = default) => new PopupXY(x, y) { inRect = true, rect = r };

		/// <summary>
		/// Creates new <b>PopupXY</b> that specifies position relative to the work area of the primary screen.
		/// </summary>
		public static implicit operator PopupXY((Coord x, Coord y) p) => new PopupXY(p.x, p.y, true);

		/// <summary>Creates new <b>PopupXY</b> that specifies position relative to the primary screen (not to the work area).</summary>
		public static implicit operator PopupXY(POINT p) => new PopupXY(p.x, p.y, false);
		//info: this conversion can be used with PopupXY.Mouse.

		//public bool IsRawXY => !inRect && screen.IsNull && workArea == false && x.Type == Coord.CoordType.Normal && y.Type == Coord.CoordType.Normal;

		/// <summary>
		/// Gets point coordinates below mouse cursor, for showing a tooltip-like popup.
		/// </summary>
		public static POINT Mouse
		{
			get
			{
				var p = AMouse.XY;
				int cy = ADpi.GetSystemMetrics(Api.SM_CYCURSOR, p);
				if(ACursor.GetCurrentVisibleCursor(out var c) && Api.GetIconInfo(c, out var u)) {
					if(u.hbmColor != default) Api.DeleteObject(u.hbmColor);
					if(u.hbmMask != default) Api.DeleteObject(u.hbmMask);

					//AOutput.Write(u.xHotspot, u.yHotspot);
					p.y += cy - u.yHotspot - 1; //not perfect, but better than just to add SM_CYCURSOR or some constant value.
					return p;
				}
				return (p.x, p.y + cy - 5);
			}
		}

		/// <summary>
		/// Gets <see cref="ScreenHandle"/> specified in <see cref="screen"/>. If not specified, gets that of the screen that contains the specified point.
		/// </summary>
		public ScreenHandle GetScreen()
		{
			if(!screen.IsNull) return screen.GetScreenHandle();
			POINT p = inRect ? Coord.NormalizeInRect(x, y, rect, centerIfEmpty: true) : Coord.Normalize(x, y, workArea);
			return AScreen.Of(p);
		}
	}

	/// <summary>
	/// Window handle.
	/// Used for function parameters where the function needs a window handle as <see cref="AWnd"/> but also allows to pass a variable of any of these types: System.Windows.Forms.Control (Form or control), System.Windows.DependencyObject (WPF window or control), IntPtr (window handle).
	/// </summary>
	[DebuggerStepThrough]
	public struct AnyWnd
	{
		readonly object _o;
		AnyWnd(object o) { _o = o; }

		/// <summary> Assignment of a value of type AWnd. </summary>
		public static implicit operator AnyWnd(AWnd w) => new AnyWnd(w);
		/// <summary> Assignment of a window handle as IntPtr. </summary>
		public static implicit operator AnyWnd(IntPtr hwnd) => new AnyWnd((AWnd)hwnd);
		/// <summary> Assignment of a value of type System.Windows.Forms.Control (Form or any control class). </summary>
		public static implicit operator AnyWnd(System.Windows.Forms.Control c) => new AnyWnd(c);
		/// <summary> Assignment of a value of type System.Windows.DependencyObject (WPF window or control). </summary>
		public static implicit operator AnyWnd(System.Windows.DependencyObject c) => new AnyWnd(new object[] { c });

		/// <summary>
		/// Gets the window or control handle as AWnd.
		/// Returns default(AWnd) if not assigned.
		/// </summary>
		public AWnd Wnd => AWnd.Internal_.FromObject(_o);

		/// <summary>
		/// true if this is default(AnyWnd).
		/// </summary>
		public bool IsEmpty => _o == null;
	}

	/// <summary>
	/// Used for function parameters to specify multiple strings.
	/// Contains a string like "One|Two|Three" or string[] or List&lt;string&gt;. Has implicit conversions from these types.
	/// </summary>
	[DebuggerStepThrough]
	public struct DStringList //with prefix D because this was created for ADialog
	{
		readonly object _o;
		DStringList(object o) { _o = o; }

		/// <summary> Assignment of a value of type string. </summary>
		public static implicit operator DStringList(string s) => new DStringList(s);

		/// <summary> Assignment of a value of type string[]. </summary>
		public static implicit operator DStringList(string[] e) => new DStringList(e);

		/// <summary> Assignment of a value of type List&lt;string&gt;. </summary>
		public static implicit operator DStringList(List<string> e) => new DStringList(e);

		/// <summary>
		/// The raw value.
		/// </summary>
		public object Value => _o;

		/// <summary>
		/// Converts the value to string[].
		/// </summary>
		public string[] ToArray()
		{
			return _o switch
			{
				string s => s.Split('|'),
				string[] a => a,
				List<string> a => a.ToArray(),
				_ => Array.Empty<string>(), //null
			};
		}
	}
}
