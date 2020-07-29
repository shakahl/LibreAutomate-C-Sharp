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
using System.Drawing;
//using System.Linq;


namespace Au.Types
{
	/// <summary>
	/// Similar to IntPtr (can be 32-bit or 64-bit), but more useful for usually-non-pointer values, eg WPARAM/LPARAM/LRESULT of API SendMessage.
	/// </summary>
	/// <remarks>
	/// Unlike IntPtr:
	/// - Has implicit casts from most integral types. And explicit casts to.
	///	- Does not check overflow when casting from uint etc. IntPtr throws exception on overflow, which can create bugs.
	///	
	///	There is no struct WPARAM. Use LPARAM instead, because it is the same in all cases except when casting to long or ulong (ambigous signed/unsigned).
	///	There is no cast operators for enum. When need, cast through int or uint. For AWnd cast through IntPtr.
	/// </remarks>
	[DebuggerStepThrough]
	//[Serializable]
	public unsafe struct LPARAM : IEquatable<LPARAM>, IComparable<LPARAM>
	{
#pragma warning disable 1591 //XML doc
		//[NonSerialized]
		void* _v; //Not IntPtr, because it throws exception on overflow when casting from uint etc.

		LPARAM(void* v) { _v = v; }

		//LPARAM = int etc
		public static implicit operator LPARAM(void* x) => new LPARAM(x);
		public static implicit operator LPARAM(IntPtr x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(UIntPtr x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(int x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(uint x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(sbyte x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(byte x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(short x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(ushort x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(char x) => new LPARAM((void*)(ushort)x);
		public static implicit operator LPARAM(long x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(ulong x) => new LPARAM((void*)x);
		public static implicit operator LPARAM(bool x) => new LPARAM((void*)(x ? 1 : 0));
		//also would be good to have LPARAM(Enum x), but C# does not allow generic operators.

		//int etc = LPARAM
		public static implicit operator void*(LPARAM x) => x._v;
		public static implicit operator IntPtr(LPARAM x) => (IntPtr)x._v;
		public static explicit operator UIntPtr(LPARAM x) => (UIntPtr)x._v;
		public static explicit operator int(LPARAM x) => (int)x._v;
		public static explicit operator uint(LPARAM x) => (uint)x._v;
		public static explicit operator sbyte(LPARAM x) => (sbyte)x._v;
		public static explicit operator byte(LPARAM x) => (byte)x._v;
		public static explicit operator short(LPARAM x) => (short)x._v;
		public static explicit operator ushort(LPARAM x) => (ushort)x._v;
		public static explicit operator char(LPARAM x) => (char)(ushort)x._v;
		public static explicit operator long(LPARAM x) => (long)x._v;
		public static explicit operator ulong(LPARAM x) => (ulong)x._v;
		public static implicit operator bool(LPARAM x) => x._v != null;
		//note: don't use implicit. It's unsafe. Eg arithmetic and other operators then implicitly convert LPARAM operands to int, although must be long.

		public static bool operator ==(LPARAM a, LPARAM b) => a._v == b._v;
		public static bool operator !=(LPARAM a, LPARAM b) => a._v != b._v;
		public static bool operator <(LPARAM a, LPARAM b) => a._v < b._v;
		public static bool operator >(LPARAM a, LPARAM b) => a._v > b._v;
		public static bool operator <=(LPARAM a, LPARAM b) => a._v <= b._v;
		public static bool operator >=(LPARAM a, LPARAM b) => a._v >= b._v;
		public static LPARAM operator +(LPARAM a, int b) => (byte*)a._v + b;
		public static LPARAM operator -(LPARAM a, int b) => (byte*)a._v - b;

		public override string ToString() => ((IntPtr)_v).ToString();

		public override int GetHashCode() => (int)_v;

		public override bool Equals(object obj) => obj is LPARAM && (LPARAM)obj == this;

		/// <summary>
		/// Returns true if other == this.
		/// Implements IEquatable. Prevents boxing when used as a key of a collection.
		/// </summary>
		public bool Equals(LPARAM other) => _v == other._v;

		/// <summary>
		/// Implements IComparable. It allows to sort a collection.
		/// </summary>
		public int CompareTo(LPARAM other) => _v == other._v ? 0 : (_v < other._v ? -1 : 1);

		//ISerializable implementation.
		//Need it because default serialization: 1. Gets only public members. 2. Exception if void*. 3. If would work, would format like <...><_v>value</_v></...>, but we need <...>value</...>.
		//Rejected, because it loads System.Xml.dll and 2 more dlls. Rarely used.
		//public XmlSchema GetSchema() => null;
		//public void ReadXml(XmlReader reader) { _v = (void*)reader.ReadElementContentAsLong(); }
		//public void WriteXml(XmlWriter writer) { writer.WriteValue((long)_v); }
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Point coordinates x y.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct POINT : IEquatable<POINT>
	{
#pragma warning disable 1591, 3008 //XML doc, CLS-compliant
		public int x, y;

		public POINT(int x, int y) { this.x = x; this.y = y; }

		public static implicit operator POINT((int x, int y) t) => new POINT(t.x, t.y);

		public static implicit operator POINT(Point p) => new POINT(p.X, p.Y);
		public static explicit operator POINT(PointF p) => new POINT(checked((int)p.X), checked((int)p.Y));
		public static explicit operator POINT(System.Windows.Point p) => new POINT(checked((int)p.X), checked((int)p.Y));

		public static implicit operator Point(POINT p) => new Point(p.x, p.y);
		public static implicit operator PointF(POINT p) => new PointF(p.x, p.y);
		public static implicit operator System.Windows.Point(POINT p) => new System.Windows.Point(p.x, p.y);

		//rejected
		///// <summary>Specifies position relative to the primary screen or its work area. Calls <see cref="Coord.Normalize"/> with <i>centerIfEmpty</i> true.</summary>
		//public static implicit operator POINT((Coord x, Coord y, bool workArea) t) => _Coord(t.x, t.y, t.workArea, default);
		///// <summary>Specifies position relative to the specified screen or its work area. Calls <see cref="Coord.Normalize"/> with <i>centerIfEmpty</i> true.</summary>
		//public static implicit operator POINT((Coord x, Coord y, AScreen screen, bool workArea) t) => _Coord(t.x, t.y, t.workArea, t.screen);
		///// <summary>Specifies position in the specified rectangle which is relative to the primary screen. Calls <see cref="Coord.NormalizeInRect"/> with <i>centerIfEmpty</i> true.</summary>
		//public static implicit operator POINT((RECT r, Coord x, Coord y) t) => Coord.NormalizeInRect(t.x, t.y, t.r, centerIfEmpty: true);
		//static POINT _Coord(Coord x, Coord y, bool workArea, AScreen screen) => Coord.Normalize(x, y, workArea, screen, centerIfEmpty: true);

		//maybe in the future
		///// <summary>
		///// Converts <see cref="Coord"/> coordinates into real coodinates.
		///// Calls <see cref="Coord.Normalize"/> with <i>centerIfEmpty</i> true.
		///// </summary>
		//public static POINT Normalize(Coord x, Coord y, bool workArea = false, AScreen screen = default)
		//	=> Coord.Normalize(x, y, workArea, screen, centerIfEmpty: true);

		//public static POINT NormalizeIn(RECT r, Coord x = default, Coord y = default)
		//	=> Coord.NormalizeInRect(x, y, r, centerIfEmpty: true);

		public static bool operator ==(POINT p1, POINT p2) => p1.x == p2.x && p1.y == p2.y;
		public static bool operator !=(POINT p1, POINT p2) => !(p1 == p2);

		public override int GetHashCode() => HashCode.Combine(x, y);

		public bool Equals(POINT other) => this == other; //IEquatable

		/// <summary>Adds x and y to this.x and this.y.</summary>
		public void Offset(int x, int y) { this.x += x; this.y += y; }

		/// <summary>Returns <c>new POINT(p.x + d.x, p.y + d.y)</c>.</summary>
		public static POINT operator +(POINT p, (int x, int y) d) => new POINT(p.x + d.x, p.y + d.y);

		public void Deconstruct(out int x, out int y) { x = this.x; y = this.y; }

		public override string ToString() => $"{{x={x} y={y}}}";

		//properties for JSON serialization
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int _X { get => x; set { x = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int _Y { get => y; set { y = value; } }
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Width and height.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct SIZE : IEquatable<SIZE>
	{
#pragma warning disable 1591, 3008 //XML doc, CLS-compliant
		public int width, height;

		public SIZE(int width, int height) { this.width = width; this.height = height; }

		public static implicit operator SIZE((int width, int height) t) => new SIZE(t.width, t.height);

		public static implicit operator SIZE(Size z) => new SIZE(z.Width, z.Height);
		public static explicit operator SIZE(SizeF z) => new SIZE(checked((int)z.Width), checked((int)z.Height));
		public static explicit operator SIZE(System.Windows.Size z) => new SIZE(checked((int)z.Width), checked((int)z.Height));

		public static implicit operator Size(SIZE z) => new Size(z.width, z.height);
		public static implicit operator SizeF(SIZE z) => new SizeF(z.width, z.height);
		public static implicit operator System.Windows.Size(SIZE z) => new System.Windows.Size(z.width, z.height);

		public static bool operator ==(SIZE s1, SIZE s2) => s1.width == s2.width && s1.height == s2.height;
		public static bool operator !=(SIZE s1, SIZE s2) => !(s1 == s2);

		public override int GetHashCode() => HashCode.Combine(width, height);

		public bool Equals(SIZE other) => this == other; //IEquatable

		/// <summary>Returns <c>new SIZE(z.width + d.x, z.height + d.y)</c>.</summary>
		public static SIZE operator +(SIZE z, (int x, int y) d) => new SIZE(z.width + d.x, z.height + d.y);

		public void Deconstruct(out int width, out int height) { width = this.width; height = this.height; }

		public override string ToString() => $"{{cx={width} cy={height}}}";

		//properties for JSON serialization
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int _W { get => width; set { width = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int _H { get => height; set { height = value; } }
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Rectangle coordinates left top right bottom.
	/// </summary>
	/// <remarks>
	/// This type can be used with Windows API functions. The .NET <b>Rectangle</b> etc can't, because their fields are different.
	/// Has implicit conversions from/to <b>Rectangle</b> and <b>RectangleF</b>.
	/// </remarks>
	[DebuggerStepThrough]
	[Serializable]
	public struct RECT : IEquatable<RECT>
	{
#pragma warning disable 1591, 3008 //XML doc, CLS-compliant
		public int left, top, right, bottom;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="rightOrWidth">right or width, depending on <i>useWidthHeight</i>.</param>
		/// <param name="bottomOrHeight">bottom or height, depending on <i>useWidthHeight</i>.</param>
		/// <param name="useWidthHeight">If true (default), <i>rightOrWidth</i>/<i>bottomOrHeight</i> are width/height. Else right/bottom.</param>
		public RECT(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight = true) {
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if (useWidthHeight) { right += left; bottom += top; }
		}

		/// <summary>
		/// Creates new <b>RECT</b> from tuple containing left, top, width and height.
		/// </summary>
		public static implicit operator RECT((int L, int T, int W, int H) t) => new RECT(t.L, t.T, t.W, t.H, true);
		//public static implicit operator RECT((int L, int T, int RW, int BH, bool wh) t) => new RECT(t.L, t.T, t.RW, t.BH, t.wh);

		public static implicit operator RECT(Rectangle r) => new RECT(r.Left, r.Top, r.Width, r.Height, true);
		public static explicit operator RECT(RectangleF r) { checked { return new RECT((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height, true); } }
		public static explicit operator RECT(System.Windows.Rect r) { checked { return new RECT((int)r.Left, (int)r.Top, (int)r.Width, (int)r.Height, true); } }

		public static implicit operator Rectangle(RECT r) => new Rectangle(r.left, r.top, r.Width, r.Height);
		public static implicit operator RectangleF(RECT r) => new RectangleF(r.left, r.top, r.Width, r.Height);
		public static implicit operator System.Windows.Rect(RECT r) => new System.Windows.Rect(r.left, r.top, r.Width, r.Height);

		public static bool operator ==(RECT r1, RECT r2) => r1.left == r2.left && r1.right == r2.right && r1.top == r2.top && r1.bottom == r2.bottom;
		public static bool operator !=(RECT r1, RECT r2) => !(r1 == r2);

		public override int GetHashCode() => HashCode.Combine(left, top, right, bottom);

		public bool Equals(RECT other) => this == other; //IEquatable

		/// <summary>
		/// Sets fields like the constructor <see cref="RECT(int,int,int,int,bool)"/>.
		/// </summary>
		public void Set(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight = true) {
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if (useWidthHeight) { right += left; bottom += top; }
		}

		/// <summary>
		/// Returns true if all fields == 0.
		/// </summary>
		public bool Is0 => left == 0 && top == 0 && right == 0 && bottom == 0;

		/// <summary>
		/// Returns true if the rectangle is empty or invalid: <c>right&lt;=left || bottom&lt;=top;</c>
		/// </summary>
		public bool IsEmpty => right <= left || bottom <= top;

		//properties for JSON serialization. Must be before Width and Height, else would be set after them.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int _X { get => left; set { left = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int _Y { get => top; set { top = value; } }

		/// <summary>
		/// Gets or sets width.
		/// </summary>
		public int Width { get => right - left; set { right = left + value; } }

		/// <summary>
		/// Gets or sets height.
		/// </summary>
		public int Height { get => bottom - top; set { bottom = top + value; } }

		/// <summary>
		/// Gets horizontal center.
		/// </summary>
		public int CenterX => left + (right - left) / 2;
		//public int CenterX => (int)(((long)left + right) / 2);

		/// <summary>
		/// Gets vertical center.
		/// </summary>
		public int CenterY => top + (bottom - top) / 2;
		//public int CenterY => (int)(((long)top + bottom) / 2);

		/// <summary>
		/// Returns true if this rectangle contains the specified point.
		/// </summary>
		public bool Contains(int x, int y) => x >= left && x < right && y >= top && y < bottom;

		/// <summary>
		/// Returns true if this rectangle contains the specified point.
		/// </summary>
		public bool Contains(POINT p) => Contains(p.x, p.y);

		/// <summary>
		/// Returns true if this rectangle contains entire specified rectangle.
		/// </summary>
		public bool Contains(RECT r2) => r2.left >= left && r2.top >= top && r2.right <= right && r2.bottom <= bottom;

		/// <summary>
		/// Makes this rectangle bigger or smaller: <c>left-=dx; right+=dx; top-=dy; bottom+=dy;</c>
		/// Use negative dx/dy to make the rectangle smaller. Note: too big negative dx/dy can make it invalid (right&lt;left or bottom&lt;top).
		/// </summary>
		public void Inflate(int dx, int dy) { left -= dx; right += dx; top -= dy; bottom += dy; }

		/// <summary>
		/// Replaces this rectangle with the intersection of itself and the specified rectangle.
		/// Returns true if the rectangles intersect.
		/// If they don't intersect, makes this RECT empty (IsEmpty would return true).
		/// </summary>
		public bool Intersect(RECT r2) => Api.IntersectRect(out this, this, r2);

		/// <summary>
		/// Returns the intersection rectangle of two rectangles.
		/// If they don't intersect, returns empty rectangle (IsEmpty would return true).
		/// </summary>
		public static RECT Intersect(RECT r1, RECT r2) { Api.IntersectRect(out RECT r, r1, r2); return r; }

		/// <summary>
		/// Returns true if this rectangle and another rectangle intersect.
		/// </summary>
		public bool IntersectsWith(RECT r2) => Api.IntersectRect(out _, this, r2);

		/// <summary>
		/// Moves this rectangle by the specified offsets: <c>left+=dx; right+=dx; top+=dy; bottom+=dy;</c>
		/// Negative dx moves to the left. Negative dy moves up.
		/// </summary>
		public void Offset(int dx, int dy) { left += dx; right += dx; top += dy; bottom += dy; }

		/// <summary>
		/// Replaces this rectangle with the union of itself and the specified rectangle.
		/// Union is the smallest rectangle that contains two full rectangles.
		/// Returns true if finally this rectangle is not empty.
		/// If either rectangle is empty (Width or Height is &lt;=0), the result is another rectangle. If both empty - empty rectangle.
		/// </summary>
		public bool Union(RECT r2) => Api.UnionRect(out this, this, r2);

		/// <summary>
		/// Returns the union of two rectangles.
		/// Union is the smallest rectangle that contains two full rectangles.
		/// If either rectangle is empty (Width or Height is &lt;=0), the result is another rectangle. If both empty - empty rectangle.
		/// </summary>
		public static RECT Union(RECT r1, RECT r2) { Api.UnionRect(out RECT r, r1, r2); return r; }

		/// <summary>
		/// If width or height are negative, modifies this rectangle so that they would not be negative.
		/// </summary>
		/// <param name="swap">true - swap right/left, bottom/top; false - set right = left, bottom = top.</param>
		public void Normalize(bool swap) {
			if (right < left) { if (swap) AMath.Swap(ref left, ref right); else right = left; }
			if (bottom < top) { if (swap) AMath.Swap(ref top, ref bottom); else bottom = top; }
		}

		/// <summary>
		/// Moves this rectangle to the specified coordinates in the specified screen, and ensures that whole rectangle is in screen.
		/// Final rectangle coordinates are relative to the primary screen.
		/// </summary>
		/// <param name="x">X coordinate in the specified screen. If default(Coord) - center. Can be <see cref="Coord.Reverse"/> etc.</param>
		/// <param name="y">Y coordinate in the specified screen. If default(Coord) - center. Can be <see cref="Coord.Reverse"/> etc.</param>
		/// <param name="screen">Use this screen. If default, uses the primary screen.</param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <param name="ensureInScreen">If part of rectangle is not in screen, move and/or resize it so that entire rectangle would be in screen. Default true.</param>
		/// <remarks>
		/// This function can be used to calculate new window location before creating it. If window already exists, use <see cref="AWnd.MoveInScreen"/>.
		/// </remarks>
		public void MoveInScreen(Coord x, Coord y, AScreen screen = default, bool workArea = true, bool ensureInScreen = true) {
			AWnd.Internal_.MoveInScreen(false, x, y, false, default, ref this, screen, workArea, ensureInScreen);
		}

		/// <summary>
		/// Moves this rectangle to the specified coordinates in another rectangle <i>r</i>.
		/// </summary>
		/// <param name="r">Another rectangle.</param>
		/// <param name="x">X coordinate relative to <i>r</i>. Default - center. Can be <see cref="Coord.Reverse"/> etc.</param>
		/// <param name="y">Y coordinate relative to <i>r</i>. Default - center. Can be <see cref="Coord.Reverse"/> etc.</param>
		/// <param name="ensureInRect">If part of rectangle is not in <i>r</i>, move and/or resize it so that entire rectangle would be in <i>r</i>.</param>
		public void MoveInRect(RECT r, Coord x = default, Coord y = default, bool ensureInRect = false) {
			AWnd.Internal_.MoveInScreen(false, x, y, false, default, ref this, default, false, ensureInRect, r);
		}

		/// <summary>
		/// Adjusts this rectangle to ensure that whole rectangle is in screen.
		/// Initial and final rectangle coordinates are relative to the primary screen.
		/// </summary>
		/// <param name="screen">Use this screen (see <see cref="AScreen"/>). If default, uses screen of the rectangle (or nearest).</param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <remarks>
		/// This function can be used to calculate new window location before creating it. If window already exists, use <see cref="AWnd.EnsureInScreen"/>.
		/// </remarks>
		public void EnsureInScreen(AScreen screen = default, bool workArea = true) {
			AWnd.Internal_.MoveInScreen(true, default, default, false, default, ref this, screen, workArea, true);
		}

		public override string ToString() {
			return $"{{L={left} T={top} W={Width} H={Height}}}";
			//note: don't change the format. Some functions parse it.

			//don't need R B. Rarely useful, just makes more difficult to read W H.
			//return $"{{L={left} T={top} R={right} B={bottom}  W={Width} H={Height}}}";
		}
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Color, as int in 0xAARRGGBB format.
	/// Can convert from/to <see cref="Color"/>, <see cref="System.Windows.Media.Color"/>, int (0xAARRGGBB), Windows native COLORREF (0xBBGGRR), string.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct ColorInt : IEquatable<ColorInt>
	{
		/// <summary>
		/// Color value in 0xAARRGGBB format.
		/// </summary>
		public int color;

		/// <summary>
		/// Creates ColorInt from color value in 0xAARRGGBB format.
		/// </summary>
		/// <param name="colorARGB"></param>
		/// <param name="makeOpaque">Set alpha = 0xFF.</param>
		public ColorInt(int colorARGB, bool makeOpaque) {
			if (makeOpaque) colorARGB |= unchecked((int)0xFF000000);
			this.color = colorARGB;
		}

		/// <summary>
		/// Creates ColorInt from int color value in 0xRRGGBB format.
		/// Makes opaque (alpha 0xFF).
		/// </summary>
		public static implicit operator ColorInt(int color) => new ColorInt(color, true);

		/// <summary>
		/// Creates ColorInt from uint color value in 0xRRGGBB format.
		/// Makes opaque (alpha 0xFF).
		/// </summary>
		public static implicit operator ColorInt(uint color) => new ColorInt((int)color, true);

		/// <summary>
		/// Creates ColorInt from <see cref="Color"/>.
		/// </summary>
		public static implicit operator ColorInt(Color color) => new ColorInt(color.ToArgb(), false);

		/// <summary>
		/// Creates ColorInt from <see cref="System.Windows.Media.Color"/>.
		/// </summary>
		public static implicit operator ColorInt(System.Windows.Media.Color color)
			=> new ColorInt((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B, false);

		/// <summary>
		/// Creates ColorInt from color name (<see cref="Color.FromName(string)"/>) or string "0xRRGGBB" or "#RRGGBB".
		/// </summary>
		/// <remarks>
		/// If s is a hex number that contains 6 or less hex digits, makes opaque (alpha 0xFF).
		/// If s is null or invalid, sets c.color = 0 and returns false.
		/// </remarks>
		public static bool FromString(string s, out ColorInt c) {
			c.color = 0;
			if (s == null || s.Length < 2) return false;
			if (s[0] == '0' && s[1] == 'x') {
				c.color = s.ToInt(0, out int end);
				if (end < 3) return false;
				if (end <= 8) c.color |= unchecked((int)0xFF000000);
			} else if (s[0] == '#') {
				c.color = s.ToInt(1, out int end, STIFlags.IsHexWithout0x);
				if (end < 2) return false;
				if (end <= 7) c.color |= unchecked((int)0xFF000000);
			} else {
				c.color = Color.FromName(s).ToArgb();
				if (c.color == 0) return false; //invalid is 0, black is 0xFF000000
			}
			return true;
		}

		/// <summary>
		/// Creates ColorInt (0xRRGGBB) from Windows native COLORREF (0xBBGGRR).
		/// </summary>
		/// <param name="colorBGR">Color in 0xBBGGRR format.</param>
		/// <param name="makeOpaque">Set alpha = 0xFF.</param>
		public static ColorInt FromBGR(int colorBGR, bool makeOpaque) => new ColorInt(SwapRB(colorBGR), makeOpaque);

		/// <summary>
		/// Creates Windows native COLORREF (0xBBGGRR) from ColorInt (0xRRGGBB).
		/// Returns color in COLORREF format. Does not modify this variable.
		/// </summary>
		/// <param name="zeroAlpha">Set the alpha byte = 0.</param>
		public int ToBGR(bool zeroAlpha = true) {
			var r = SwapRB(color);
			if (zeroAlpha) r &= 0xFFFFFF;
			return r;
		}

		/// <summary>Creates int from ColorInt.</summary>
		public static explicit operator int(ColorInt c) => c.color;

		/// <summary>Creates int from ColorInt.</summary>
		public static explicit operator uint(ColorInt c) => (uint)c.color;

		/// <summary>Creates <see cref="Color"/> from ColorInt.</summary>
		public static explicit operator Color(ColorInt c) => Color.FromArgb(c.color);

		/// <summary>Creates <see cref="System.Windows.Media.Color"/> from ColorInt.</summary>
		public static explicit operator System.Windows.Media.Color(ColorInt c) {
			uint k = (uint)c.color;
			return System.Windows.Media.Color.FromArgb((byte)(k >> 24), (byte)(k >> 16), (byte)(k >> 8), (byte)k);
		}

#pragma warning disable 1591 //XML doc
		public static bool operator ==(ColorInt a, ColorInt b) => a.color == b.color;
		public static bool operator !=(ColorInt a, ColorInt b) => a.color != b.color;

		public bool Equals(ColorInt other) => other.color == color;

		public override bool Equals(object obj) => obj is ColorInt && this == (ColorInt)obj;
		public override int GetHashCode() => color;
		public override string ToString() => "#" + color.ToString("X8");

		//property for JSON serialization
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int _C { get => color; set { color = value; } }
#pragma warning restore 1591 //XML doc

		/// <summary>
		/// Converts color from ARGB (0xAARRGGBB) to ABGR (0xAABBGGRR) or vice versa (swaps the red and blue bytes).
		/// ARGB is used in .NET, GDI+ and HTML/CSS.
		/// ABGR is used by most Windows API.
		/// </summary>
		public static int SwapRB(int color) {
			return (color & unchecked((int)0xff00ff00)) | ((color << 16) & 0xff0000) | ((color >> 16) & 0xff);
		}

		/// <summary>
		/// Calculates color's perceived brightness.
		/// Returns a value in range 0 (brightness of black color) to 1 (brightness of white color).
		/// </summary>
		/// <remarks>
		/// Unlike Color.GetBrightness, this function gives different weights for red, green and blue components.
		/// Does not use alpha.
		/// </remarks>
		internal float GetPerceivedBrightness() //FUTURE: make public if really useful
		{
			uint u = (uint)color;
			uint R = u >> 16 & 0xff, G = u >> 8 & 0xff, B = u & 0xff;
			return (float)(Math.Sqrt(R * R * .299 + G * G * .587 + B * B * .114) / 255);
		}

		/// <summary>
		/// Changes color's luminance (makes darker or brighter).
		/// Returns new color. Does not modify this variable.
		/// </summary>
		/// <param name="n">The luminance in units of 0.1 percent of the range (which depends on totalRange). Can be from -1000 to 1000.</param>
		/// <param name="totalRange">If true, n is in the whole luminance range (from minimal to maximal possible). If false, n is in the range from current luminance of the color to the maximal (if n positive) or minimal (if n negative) luminance.</param>
		/// <remarks>
		/// Calls API <msdn>ColorAdjustLuma</msdn>.
		/// Does not change hue and saturation. Does not use alpha.
		/// </remarks>
		internal ColorInt AdjustLuminance(int n, bool totalRange = false) {
			uint u = (uint)color;
			u = Api.ColorAdjustLuma(u & 0xffffff, n, !totalRange) | (u & 0xFF000000);
			return new ColorInt((int)u, false);
			//tested: with SwapRB the same.
		}
		//FUTURE: make public if really useful.
		//	Or remove.
		//	Or also add ColorRGBToHLS and ColorHLSToRGB.
		//	It seems incompatible with Color.GetBrightness etc.
	}

	[DebuggerStepThrough]
	internal unsafe struct VARIANT : IDisposable
	{
		public Api.VARENUM vt; //ushort
		ushort _u1;
		uint _u2;
		public LPARAM value;
		public LPARAM value2;
		//note: cannot use FieldOffset because of different 32/64 bit size

		public VARIANT(int x) : this() { vt = Api.VARENUM.VT_I4; value = x; }
		public VARIANT(string x) : this() { vt = Api.VARENUM.VT_BSTR; value = Marshal.StringToBSTR(x); }

		public static implicit operator VARIANT(int x) => new VARIANT(x);
		public static implicit operator VARIANT(string x) => new VARIANT(x);

		public int ValueInt { get { Debug.Assert(vt == Api.VARENUM.VT_I4); return (int)value; } }
		public BSTR ValueBstr { get { Debug.Assert(vt == Api.VARENUM.VT_BSTR); return BSTR.AttachBSTR((char*)value); } }

		/// <summary>
		/// Calls VariantClear.
		/// </summary>
		public void Dispose() {
			_Clear();
		}

		void _Clear() {
			if (vt >= Api.VARENUM.VT_BSTR) Api.VariantClear(ref this);
			else vt = 0; //info: VariantClear just sets vt=0 and does not clear other members
		}

		/// <summary>
		/// Converts to string.
		/// </summary>
		public override string ToString() {
			return _ToString();
		}

		string _ToString() {
			switch (vt) {
				case Api.VARENUM.VT_BSTR: return value == default ? null : ValueBstr.ToString();
				case Api.VARENUM.VT_I4: return value.ToString();
				case 0: case Api.VARENUM.VT_NULL: return null;
			}
			VARIANT v2 = default;
			uint lcid = 0x409; //invariant
			switch (vt & (Api.VARENUM)0xff) { case Api.VARENUM.VT_DATE: case Api.VARENUM.VT_DISPATCH: lcid = 0x400; break; } //LOCALE_USER_DEFAULT
			if (0 != Api.VariantChangeTypeEx(ref v2, this, lcid, 2, Api.VARENUM.VT_BSTR)) return null; //2 VARIANT_ALPHABOOL
			return v2.value == default ? null : v2.ValueBstr.ToStringAndDispose();
		}

		/// <summary>
		/// Converts to string.
		/// Disposes this VARIANT.
		/// </summary>
		public string ToStringAndDispose() {
			var r = _ToString();
			Dispose();
			return r;
		}
	}

	[DebuggerStepThrough]
	internal unsafe struct BSTR : IDisposable
	{
		char* _p;

		BSTR(char* p) => _p = p;

		public static explicit operator BSTR(string s) => new BSTR((char*)Marshal.StringToBSTR(s));
		public static explicit operator LPARAM(BSTR b) => b._p;

		public static BSTR AttachBSTR(char* bstr) => new BSTR(bstr);

		public static BSTR CopyFrom(char* anyString) => anyString == null ? default : Api.SysAllocString(anyString);

		public static BSTR Alloc(int len) => Api.SysAllocStringLen(null, len);

		public char* Ptr => _p;

		/// <summary>
		/// Returns true if the string is null.
		/// </summary>
		public bool Is0 => _p == null;

		public int Length => _p == null ? 0 : Api.SysStringLen(_p);

		/// <summary>
		/// Unsafe.
		/// </summary>
		public char this[int i] => _p[i];

		/// <summary>
		/// Converts to string.
		/// Does not dispose.
		/// </summary>
		public override string ToString() {
			var p = _p; if (p == null) return null;
			return Marshal.PtrToStringBSTR((IntPtr)_p);
		}

		/// <summary>
		/// Converts to string and disposes.
		/// </summary>
		public string ToStringAndDispose() {
			var p = _p; if (p == null) return null;
			int len = Api.SysStringLen(p);

			//rejected:
			//Some objects can return BSTR containing '\0's. Then probably the rest of string is garbage. I never noticed this but saw comments. Better allow '\0's, because in some cases it can be valid string. When invalid, it will not harm too much.
			//int len2 = Util.CharPtr_.Length(p, len); ADebug.PrintIf(len2 != len, "BSTR with '\\0'"); len = len2;

			string r = len == 0 ? "" : new string(p, 0, len);
			Dispose();
			return r;
		}

		public void Dispose() {
			var t = _p;
			if (t != null) {
				_p = null;
				Api.SysFreeString(t);
			}
		}
	}

}
