namespace Au.Types
{
#if false //rejected. More convenient to use, but makes code less clear. With Range everything is clear: null means "all"; for just start index use 'i..'; many users know Range, and would have to learn about RANGE.
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
	public record struct RANGE
	{
		int _start;
		int _endPlus1;

		/// <summary>
		/// Initializes a new <see cref="RANGE"/> instance with the specified starting and ending indexes.
		/// </summary>
		/// <param name="start">The start index of the range.</param>
		/// <param name="end">The end index of the range. The default value -1 means that the end index will be equal to the length of the collection (string, array, etc) with which will be used this variable.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>start</i> is less than 0 or <i>end</i> is less than -1.</exception>
		public RANGE(int start, int end = -1)
		{
			if(start < 0 || end < -1) throw new ArgumentOutOfRangeException();
			_start = start;
			_endPlus1 = end + 1;
		}

		/// <summary>
		/// Initializes a new <see cref="RANGE"/> instance with the specified starting and ending indexes specified using <b>Index</b>.
		/// </summary>
		public RANGE(Index start, Index end)
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
		public static implicit operator RANGE(int start) => new RANGE(start);

		///
		public static implicit operator RANGE(Range r) => new RANGE(r.Start, r.End);

		///
		public static implicit operator RANGE(Index start) => new RANGE(start, Index.End);

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
	}
#endif

	/// <summary>
	/// Contains x or y coordinate in screen or some other rectangle that can be specified in various ways: normal, reverse, fraction, center, max.
	/// Used for parameters of functions like <see cref="mouse.move"/>, <see cref="wnd.Move"/>.
	/// </summary>
	/// <remarks>
	/// To specify a normal coordinate (the origin is the left or top edge), assign an <b>int</b> value (implicit conversion from <b>int</b> to <b>Coord</b>).
	/// To specify a reverse coordinate (the origin is the right or bottom edge), use <see cref="Reverse"/> or a "from end" index like <c>^1</c>. It is towards the left or top edge, unless negative. Or use <see cref="Max"/> or <see cref="MaxInside"/>.
	/// To specify a "fraction of the rectangle" coordinate, use <see cref="Fraction"/> or a value of type float like <c>.5f</c>. Or use <see cref="Center"/>.
	/// The meaning of <c>default(Coord)</c> depends on function where used. Many functions interpret it as center (same as <c>Coord.Center</c> or <c>.5f</c>).
	/// Also there are functions to convert <b>Coord</b> to normal coodinates.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// mouse.move(100, 100); //left edge + 100, top edge + 100
	/// mouse.move(Coord.Reverse(100), 100); //right edge - 100, top edge + 100
	/// mouse.move(100, ^100); //left edge + 100, bottom edge - 100
	/// mouse.move(Coord.Fraction(.33), .9f); //left edge + 1/3 of the screen rectangle, top edge + 9/10
	/// mouse.move(Coord.Center, Coord.MaxInside); //x in center (left edge + 1/2), y by the bottom edge inside (Coord.Max would be outside)
	/// mouse.move(Coord.Reverse(-100), 1.1f); //right edge + 100, bottom edge + 0.1 of the rectangle
	/// 
	/// var w = wnd.find(1, "Untitled - Notepad", "Notepad");
	/// w.Move(.5f, 100, .5f, ^200); //x = center, y = 100, width = half of screen, height = screen height - 200
	/// ]]></code>
	/// </example>
	public record struct Coord
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
		public unsafe float FractionValue => BitConverter.Int32BitsToSingle((int)_v);

		/// <summary>
		/// Returns true if Type == None (no value assigned).
		/// </summary>
		public bool IsEmpty => Type == CoordType.None;

		Coord(CoordType type, int value) { _v = ((long)type << 32) | (uint)value; }
		//info: if type and value are constants, compiler knows the final value and does not use the << and | operators in the compiled code.

		/// <summary>
		/// Creates <b>Coord</b> of <b>Normal</b> type.
		/// </summary>
		//[MethodImpl(MethodImplOptions.NoInlining)] //makes bigger/slower
		public static implicit operator Coord(int v) => new(CoordType.Normal, v);

		/// <summary>
		/// Creates <b>Coord</b> of <b>Normal</b> or <b>Reverse</b> type. Reverse if the index is from end, like <c>^1</c>.
		/// </summary>
		public static implicit operator Coord(Index v) => new(v.IsFromEnd ? CoordType.Reverse : CoordType.Normal, v.Value);

		/// <summary>
		/// Creates <b>Coord</b> of <b>Fraction</b> type.
		/// </summary>
		public static implicit operator Coord(float v) => new(CoordType.Fraction, BitConverter.SingleToInt32Bits(v));

		//these would create Fraction
		///
		[Obsolete("The value must be of type int or float.", error: true), NoDoc]
		public static implicit operator Coord(uint f) => default;
		///
		[Obsolete("The value must be of type int or float.", error: true), NoDoc]
		public static implicit operator Coord(long f) => default;
		///
		[Obsolete("The value must be of type int or float.", error: true), NoDoc]
		public static implicit operator Coord(ulong f) => default;
		//tested: compiler does not allow to assign nint.

		/// <summary>
		/// Creates <b>Coord</b> of <b>Reverse</b> type.
		/// Value 0 is at the right or bottom, and does not belong to the rectangle. Positive values are towards left or top.
		/// Instead can be use "from end" index, for example argument <c>Coord.Reverse(1)</c> can be replaced with <c>^1</c>.
		/// </summary>
		public static Coord Reverse(int v) => new(CoordType.Reverse, v);

		/// <summary>
		/// Creates <b>Coord</b> of <b>Fraction</b> type.
		/// Value 0 is the left or top of the rectangle. Value 1.0 is the right or bottom of the rectangle. Values &lt;0 and &gt;=1.0 are outside of the rectangle.
		/// Instead can be used implicit conversion from float, for example argument <c>Coord.Fraction(.5)</c> can be replaced with <c>.5f</c>.
		/// </summary>
		public static unsafe Coord Fraction(double v) => (float)v;

		/// <summary>
		/// Returns <c>Fraction(0.5)</c>.
		/// </summary>
		/// <seealso cref="Fraction"/>
		public static Coord Center => .5f;

		/// <summary>
		/// Returns <c>Reverse(0)</c>. Same as <c>^0</c>.
		/// This point will be outside of the rectangle. See also <see cref="MaxInside"/>.
		/// </summary>
		/// <seealso cref="Reverse"/>
		public static Coord Max => Reverse(0);

		/// <summary>
		/// Returns <c>Reverse(1)</c>. Same as <c>^1</c>.
		/// This point will be inside of the rectangle, at the very right or bottom, assuming the rectangle is not empty.
		/// </summary>
		/// <seealso cref="Reverse"/>
		public static Coord MaxInside => Reverse(1);

		//rejected: this could be used like Coord.Max + 1. Too limited usage.
		//public static Coord operator +(Coord c, int i) { return ...; }

		static bool _NeedRect(Coord x, Coord y) {
			return (x.Type > CoordType.Normal) || (y.Type > CoordType.Normal);
		}

		/// <summary>
		/// Converts fractional/reverse coordinate to normal coordinate in a range.
		/// </summary>
		/// <param name="start">Start of range.</param>
		/// <param name="end">End of range.</param>
		public int NormalizeInRange(int start, int end) {
			return Type switch {
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
		public static POINT NormalizeInRect(Coord x, Coord y, RECT r, bool widthHeight = false, bool centerIfEmpty = false) {
			if (widthHeight) r.Move(0, 0);
			else if (centerIfEmpty) {
				if (x.IsEmpty) x = Center;
				if (y.IsEmpty) y = Center;
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
		public static POINT NormalizeInWindow(Coord x, Coord y, wnd w, bool nonClient = false, bool centerIfEmpty = false) {
			//info: don't need widthHeight parameter because client area left/top are 0. With non-client don't need in this library and probably not useful. But if need, caller can explicitly offset the rect before calling this func.

			if (centerIfEmpty) {
				if (x.IsEmpty) x = Center;
				if (y.IsEmpty) y = Center;
			}
			POINT p = default;
			if (!x.IsEmpty || !y.IsEmpty) {
				RECT r;
				if (nonClient) {
					w.GetRectIn(w, out r);
				} else if (_NeedRect(x, y)) {
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
		/// <param name="screen">If used, x y are relative to this screen. Default - primary screen. Example: <c>screen.index(1)</c>.</param>
		/// <param name="widthHeight">Use only width and height of the screen rectangle. If false, the function adds its offset (left and top, which can be nonzero if using the work area or a non-primary screen).</param>
		/// <param name="centerIfEmpty">If x or y is default(Coord), use Coord.Center.</param>
		public static POINT Normalize(Coord x, Coord y, bool workArea = false, screen screen = default, bool widthHeight = false, bool centerIfEmpty = false) {
			if (centerIfEmpty) {
				if (x.IsEmpty) x = Center;
				if (y.IsEmpty) y = Center;
			}
			POINT p = default;
			if (!x.IsEmpty || !y.IsEmpty) {
				RECT r;
				if (workArea || !screen.IsEmpty || _NeedRect(x, y)) {
					r = screen.GetRect(workArea);
					if (widthHeight) r.Move(0, 0);
				} else r = default;
				p.x = x.NormalizeInRange(r.left, r.right);
				p.y = y.NormalizeInRange(r.top, r.bottom);
			}
			return p;
		}

		///
		public override string ToString() {
			switch (Type) {
			case CoordType.Normal: return Value.ToString() + ", Normal";
			case CoordType.Reverse: return Value.ToString() + ", Reverse";
			case CoordType.Fraction: return FractionValue.ToS() + ", Fraction";
			default: return "default";
			}
		}
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
		public screen screen;
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
		/// <param name="screen">Can be used to specify a screen. Default - primary. Example: <c>screen.index(1)</c>.</param>
		/// <remarks>
		/// Also there is are implicit conversions from tuple (x, y) and POINT. Instead of <c>new PopupXY(x, y)</c> you can use <c>(x, y)</c>. Instead of <c>new PopupXY(p.x, p.y, false)</c> you can use <c>p</c> or <c>(POINT)p</c> .
		/// </remarks>
		public PopupXY(Coord x = default, Coord y = default, bool workArea = true, screen screen = default) {
			this.x = x; this.y = y; this.workArea = workArea; this.screen = screen;
		}

		/// <summary>
		/// Creates new <b>PopupXY</b> that specifies position in a rectangle. For example of the owner window.
		/// </summary>
		/// <param name="r">Rectangle relative to the primary screen.</param>
		/// <param name="x">X relative to the rectangle. Default - center.</param>
		/// <param name="y">Y relative to the rectangle. Default - center.</param>
		public static PopupXY In(RECT r, Coord x = default, Coord y = default) => new(x, y) { inRect = true, rect = r };

		/// <summary>
		/// Creates new <b>PopupXY</b> that specifies position relative to the work area of the primary screen.
		/// </summary>
		public static implicit operator PopupXY((Coord x, Coord y) p) => new(p.x, p.y, true);

		/// <summary>Creates new <b>PopupXY</b> that specifies position relative to the primary screen (not to the work area).</summary>
		public static implicit operator PopupXY(POINT p) => new(p.x, p.y, false);
		//info: this conversion can be used with PopupXY.Mouse.

		//public bool IsRawXY => !inRect && screen.IsNull && workArea == false && x.Type == Coord.CoordType.Normal && y.Type == Coord.CoordType.Normal;

		/// <summary>
		/// Gets point coordinates below mouse cursor, for showing a tooltip-like popup.
		/// </summary>
		public static POINT Mouse {
			get {
				var p = mouse.xy;

				var scr = screen.of(p);
				var rs = scr.Rect;
				int dy = Dpi.Scale(100, scr.Dpi);
				if (rs.bottom - p.y < dy) return (p.x, p.y - dy);

				int cy = Dpi.GetSystemMetrics(Api.SM_CYCURSOR, p);
				if (MouseCursor.GetCurrentVisibleCursor(out var c) && Api.GetIconInfo(c, out var u)) {
					if (u.hbmColor != default) Api.DeleteObject(u.hbmColor);
					Api.DeleteObject(u.hbmMask);

					//print.it(u.xHotspot, u.yHotspot);
					p.y += cy - u.yHotspot - 1; //not perfect, but better than just to add SM_CYCURSOR or some constant value.
					return p;
				}
				return (p.x, p.y + cy - 5);
			}
		}

		/// <summary>
		/// Gets <see cref="screen.Now"/> if not empty, else screen that contains the specified point.
		/// </summary>
		public screen GetScreen() {
			if (!screen.IsEmpty) return screen.Now;
			POINT p = inRect ? Coord.NormalizeInRect(x, y, rect, centerIfEmpty: true) : Coord.Normalize(x, y, workArea);
			return screen.of(p);
		}
	}

	/// <summary>
	/// Font name, size and style.
	/// If <b>Name</b> not set, will be used standard GUI font; then <b>Size</b> can be 0 to use size of standard GUI font.
	/// On high-DPI screen the font size will be scaled.
	/// </summary>
	public record class FontNSS(int Size = 0, string Name = null, bool Bold = false, bool Italic = false)
	{
		/// <summary>
		/// Creates font.
		/// </summary>
		/// <param name="dpi">DPI for scaling.</param>
		internal NativeFont_ CreateFont(DpiOf dpi) {
			if (Name == null) return new(dpi, Bold, Italic, Size);
			return new(dpi, Name, Size, Bold, Italic);
		}
	}

	/// <summary>
	/// Window handle.
	/// Used for function parameters where the function needs a window handle as <see cref="wnd"/> but also allows to pass a variable of any of these types: <b>System.Windows.DependencyObject</b> (WPF window or control), <b>System.Windows.Forms.Control</b> (<b>Form</b> or control), <b>IntPtr</b> (window handle).
	/// </summary>
	public struct AnyWnd
	{
		readonly object _o;
		AnyWnd(object o) { _o = o; }

		/// <summary> Assignment of a value of type wnd. </summary>
		public static implicit operator AnyWnd(wnd w) => new AnyWnd(w);
		/// <summary> Assignment of a window handle as IntPtr. </summary>
		public static implicit operator AnyWnd(IntPtr hwnd) => new AnyWnd((wnd)hwnd);
		/// <summary> Assignment of a value of type System.Windows.Forms.Control (Form or any control class). </summary>
		public static implicit operator AnyWnd(System.Windows.Forms.Control c) => new AnyWnd(c);
		/// <summary> Assignment of a value of type System.Windows.DependencyObject (WPF window or control). </summary>
		public static implicit operator AnyWnd(System.Windows.DependencyObject c) => c != null ? new AnyWnd(new object[] { c }) : default;

		/// <summary>
		/// Gets the window or control handle as wnd.
		/// Returns default(wnd) if not assigned.
		/// </summary>
		public wnd Hwnd => wnd.Internal_.FromObject(_o);

		/// <summary>
		/// true if this is default(AnyWnd).
		/// </summary>
		public bool IsEmpty => _o == null;
	}

	/// <summary>
	/// Used for function parameters to specify multiple strings.
	/// Contains a string like "One|Two|Three" or string[] or List&lt;string&gt;. Has implicit conversions from these types. Also constructor with params string[].
	/// </summary>
	public struct Strings
	{
		readonly object _o;
		Strings(object o) { _o = o; }

		///
		public Strings(params string[] a) { _o = a; }

		///
		public static implicit operator Strings(string s) => new((object)s);

		///
		public static implicit operator Strings(string[] e) => new(e);

		///
		public static implicit operator Strings(List<string> e) => new(e);

		//rejected. Shorter just by 'new'. Can make difficult to read code. No intellisense.
		/////
		//public static implicit operator Strings((string s1, string s2) t) => new(t.s1, t.s2);

		/////
		//public static implicit operator Strings((string s1, string s2, string s3) t) => new(t.s1, t.s2, t.s3);

		/////
		//public static implicit operator Strings((string s1, string s2, string s3, string s4) t) => new(t.s1, t.s2, t.s3, t.s4);

		//note: C# does not allow Strings(IEnumerable<string> e), because it is interface. Callers can use .ToArray().

		/// <summary>
		/// The raw value.
		/// </summary>
		public object Value => _o;

		/// <summary>
		/// Converts the value to string[].
		/// Note: don't modify array elements. If the caller passed an array, this function returns it, not a copy.
		/// </summary>
		public string[] ToArray() {
			return _o switch {
				string[] a => a,
				string s => s.Split('|'),
				List<string> a => a.ToArray(),
				_ => Array.Empty<string>(), //null
			};
		}
	}
}