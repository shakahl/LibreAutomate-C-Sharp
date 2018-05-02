﻿using System;
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
#pragma warning disable 660, 661 //no Equals()

	/// <summary>
	/// Similar to IntPtr (can be 32-bit or 64-bit), but more useful for usually-non-pointer values, eg wParam/lParam of SendMessage.
	/// Unlike IntPtr:
	///		Has implicit casts from/to most integral types.
	///		Does not check overflow when casting from uint etc. IntPtr throws exception on overflow, which can create strange bugs.
	/// </summary>
	/// <remarks>
	///	There is no struct WPARAM. Use LPARAM instead, because it is the same in all cases except when casting to long or ulong (ambigous signed/unsigned).
	///	There is no cast operators for enum. When need, cast through int or uint. For Wnd cast through IntPtr.
	/// </remarks>
	[DebuggerStepThrough]
	//[Serializable]
	public unsafe struct LPARAM :IEquatable<LPARAM>, IComparable<LPARAM>
	{
#pragma warning disable 1591 //XML doc
		//[NonSerialized]
		void* _v; //Not IntPtr, because it throws exception on overflow when casting from uint etc.

		LPARAM(void* v) { _v = v; }

		//LPARAM = int etc
		public static implicit operator LPARAM(void* x) { return new LPARAM(x); }
		public static implicit operator LPARAM(IntPtr x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(UIntPtr x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(int x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(uint x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(sbyte x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(byte x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(short x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(ushort x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(char x) { return new LPARAM((void*)(ushort)x); }
		public static implicit operator LPARAM(long x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(ulong x) { return new LPARAM((void*)x); }
		public static implicit operator LPARAM(bool x) { return new LPARAM((void*)(x ? 1 : 0)); }
		//public static implicit operator LPARAM(Enum x) { return new LPARAM((void*)(int)x); } //error
		public static implicit operator LPARAM(Keys x) { return new LPARAM((void*)(int)x); }
		//int etc = LPARAM
		public static implicit operator void* (LPARAM x) { return x._v; }
		public static implicit operator IntPtr(LPARAM x) { return (IntPtr)x._v; }
		public static implicit operator UIntPtr(LPARAM x) { return (UIntPtr)x._v; }
		public static implicit operator int(LPARAM x) { return (int)x._v; }
		public static implicit operator uint(LPARAM x) { return (uint)x._v; }
		public static implicit operator sbyte(LPARAM x) { return (sbyte)x._v; }
		public static implicit operator byte(LPARAM x) { return (byte)x._v; }
		public static implicit operator short(LPARAM x) { return (short)x._v; }
		public static implicit operator ushort(LPARAM x) { return (ushort)x._v; }
		public static implicit operator char(LPARAM x) { return (char)(ushort)x._v; }
		public static implicit operator long(LPARAM x) { return (long)x._v; }
		public static implicit operator ulong(LPARAM x) { return (ulong)x._v; }
		public static implicit operator bool(LPARAM x) { return x._v != null; }

		public static bool operator ==(LPARAM a, LPARAM b) { return a._v == b._v; }
		public static bool operator !=(LPARAM a, LPARAM b) { return a._v != b._v; }

		//TODO: need to overload operator < etc, because now gives unexpected results.

		public override string ToString() => ((IntPtr)_v).ToString();

		public override int GetHashCode() => (int)_v;

		public override bool Equals(object other) => other is LPARAM && (LPARAM)other == this;

		public bool Equals(LPARAM other) => _v == other._v;

		public int CompareTo(LPARAM other) => _v == other._v ? 0 : (_v < other._v ? -1 : 1);

		//ISerializable implementation.
		//Need it because default serialization: 1. Gets only public members. 2. Exception if void*. 3. If would work, would format like <...><_v>value</_v></...>, but we need <...>value</...>.
		//Rejected, because it loads System.Xml.dll and 2 more dlls. Rarely used.
		//public XmlSchema GetSchema() { return null; }
		//public void ReadXml(XmlReader reader) { _v = (void*)reader.ReadElementContentAsLong(); }
		//public void WriteXml(XmlWriter writer) { writer.WriteValue((long)_v); }
#pragma warning restore 1591 //XML doc
	}

	/// <summary>
	/// Contains rectangle coordinates.
	/// </summary>
	/// <remarks>
	/// This type can be used with Windows API functions. The .NET <b>Rectangle</b>/<b>Rect</b>/<b>Int32Rect</b> can't, because their fields are different.
	/// This type has implicit conversions from/to <b>Rectangle</b>.
	/// </remarks>
	[DebuggerStepThrough]
	[Serializable]
	public struct RECT
	{
#pragma warning disable 1591 //XML doc
		public int left, top, right, bottom;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="rightOrWidth">right or width, depending on <paramref name="useWidthHeight"/>.</param>
		/// <param name="bottomOrHeight">bottom or height, depending on <paramref name="useWidthHeight"/>.</param>
		/// <param name="useWidthHeight">If true, rightOrWidth/bottomOrHeight are width/height. Else right/bottom.</param>
		public RECT(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight)
		{
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if(useWidthHeight) { right += left; bottom += top; }
		}

		public static implicit operator RECT(Rectangle r) { return new RECT(r.Left, r.Top, r.Width, r.Height, true); }
		public static implicit operator Rectangle(RECT r) { return new Rectangle(r.left, r.top, r.Width, r.Height); }

		public static bool operator ==(RECT r1, RECT r2) { return r1.left == r2.left && r1.right == r2.right && r1.top == r2.top && r1.bottom == r2.bottom; }
		public static bool operator !=(RECT r1, RECT r2) { return !(r1 == r2); }

		/// <summary>
		/// Sets fields like the constructor <see cref="RECT(int,int,int,int,bool)"/>.
		/// </summary>
		public void Set(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight)
		{
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if(useWidthHeight) { right += left; bottom += top; }
		}

		/// <summary>
		/// Returns true if all fields == 0.
		/// </summary>
		public bool Is0 => left == 0 && top == 0 && right == 0 && bottom == 0;

		/// <summary>
		/// Returns true if the rectangle is empty or invalid: <c>right&lt;=left || bottom&lt;=top;</c>
		/// </summary>
		public bool IsEmpty => right <= left || bottom <= top;

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
		public int CenterX => (int)(((long)left + right) / 2);

		/// <summary>
		/// Gets vertical center.
		/// </summary>
		public int CenterY => (int)(((long)top + bottom) / 2);

		/// <summary>
		/// Returns true if this rectangle contains the specified point.
		/// </summary>
		public bool Contains(int x, int y) { return x >= left && x < right && y >= top && y < bottom; }

		/// <summary>
		/// Returns true if this rectangle contains the specified point.
		/// </summary>
		public bool Contains(Point p) { return Contains(p.X, p.Y); }

		/// <summary>
		/// Returns true if this rectangle contains entire specified rectangle.
		/// </summary>
		public bool Contains(RECT r2) { return r2.left >= left && r2.top >= top && r2.right <= right && r2.bottom <= bottom; }

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
		public bool Intersect(RECT r2) { return Api.IntersectRect(out this, ref this, ref r2); }

		/// <summary>
		/// Returns the intersection rectangle of two rectangles.
		/// If they don't intersect, returns empty rectangle (IsEmpty would return true).
		/// </summary>
		public static RECT Intersect(RECT r1, RECT r2) { Api.IntersectRect(out RECT r, ref r1, ref r2); return r; }

		/// <summary>
		/// Returns true if this rectangle and another rectangle intersect.
		/// </summary>
		public bool IntersectsWith(RECT r2) { return Api.IntersectRect(out _, ref this, ref r2); }

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
		public bool Union(RECT r2) { return Api.UnionRect(out this, ref this, ref r2); }

		/// <summary>
		/// Returns the union of two rectangles.
		/// Union is the smallest rectangle that contains two full rectangles.
		/// If either rectangle is empty (Width or Height is &lt;=0), the result is another rectangle. If both empty - empty rectangle.
		/// </summary>
		public static RECT Union(RECT r1, RECT r2) { Api.UnionRect(out RECT r, ref r1, ref r2); return r; }

		/// <summary>
		/// Moves this rectangle to the specified coordinates in the specified screen, and ensures that whole rectangle is in screen.
		/// Final rectangle coordinates are relative to the primary screen.
		/// </summary>
		/// <param name="x">X coordinate in the specified screen. If null - screen center. You also can use Coord.Reverse etc.</param>
		/// <param name="y">Y coordinate in the specified screen. If null - screen center. You also can use Coord.Reverse etc.</param>
		/// <param name="screen">Use this screen (see <see cref="Screen_.FromObject"/>). If null (default), uses the primary screen.</param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <param name="ensureInScreen">If part of rectangle is not in screen, move and/or resize it so that entire rectangle would be in screen. Default true.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		/// <remarks>
		/// This function can be used to calculate new window location before creating it. If window already exists, use <see cref="Wnd.MoveInScreen"/>.
		/// </remarks>
		public void MoveInScreen(Coord x, Coord y, object screen = null, bool workArea = true, bool ensureInScreen = true)
		{
			Wnd.Lib.MoveInScreen(false, x, y, false, default, ref this, screen, workArea, ensureInScreen);
		}

		/// <summary>
		/// Adjusts this rectangle to ensure that whole rectangle is in screen.
		/// Initial and final rectangle coordinates are relative to the primary screen.
		/// </summary>
		/// <param name="screen">Use this screen (see <see cref="Screen_.FromObject"/>). If null (default), uses screen of the rectangle (or nearest).</param>
		/// <param name="workArea">Use the work area, not whole screen. Default true.</param>
		/// <exception cref="ArgumentOutOfRangeException">Invalid screen index.</exception>
		/// <remarks>
		/// This function can be used to calculate new window location before creating it. If window already exists, use <see cref="Wnd.EnsureInScreen"/>.
		/// </remarks>
		public void EnsureInScreen(object screen = null, bool workArea = true)
		{
			Wnd.Lib.MoveInScreen(true, null, null, false, default, ref this, screen, workArea, true);
		}

		public override string ToString()
		{
			return $"{{L={left} T={top} W={Width} H={Height}}}";
			//note: don't change the format. Some functions parse it.

			//don't need R B. Rarely useful, just makes more difficult to read W H.
			//return $"{{L={left} T={top} R={right} B={bottom}  W={Width} H={Height}}}";
		}
#pragma warning restore 1591 //XML doc
	}
#pragma warning restore 660, 661

	/// <summary>
	/// Color, as int in 0xAARRGGBB format.
	/// Can convert from/to <see cref="Color"/>, Windows native COLORREF (0xBBGGRR), from string.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct ColorInt
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
		public ColorInt(int colorARGB, bool makeOpaque)
		{
			if(makeOpaque) colorARGB |= unchecked((int)0xFF000000);
			this.color = colorARGB;
		}

		/// <summary>
		/// Creates ColorInt from color value in 0xRRGGBB format.
		/// Makes opaque (alpha 0xFF).
		/// </summary>
		public static implicit operator ColorInt(int color) => new ColorInt(color, true);

		/// <summary>
		/// Creates ColorInt from <see cref="Color"/>.
		/// </summary>
		public ColorInt(Color color) { this.color = color.ToArgb(); }

		/// <summary>
		/// Creates ColorInt from <see cref="Color"/>.
		/// </summary>
		public static implicit operator ColorInt(Color color) => new ColorInt(color.ToArgb(), false);

		/// <summary>
		/// Creates ColorInt from color name (<see cref="Color.FromName(string)"/>) or string in "0xRRGGBB" or "#RRGGBB" format.
		/// </summary>
		/// <remarks>
		/// If s is a hex number that contains 6 or less hex digits, makes opaque (alpha 0xFF).
		/// If s is null or invalid, sets c.color = 0 and returns false.
		/// </remarks>
		public static bool FromString(string s, out ColorInt c)
		{
			c.color = 0;
			if(s == null || s.Length < 2) return false;
			if(s[0] == '0' && s[1] == 'x') {
				c.color = s.ToInt32_(0, out int len);
				if(len < 3) return false;
				if(len <= 8) c.color |= unchecked((int)0xFF000000);
			} else if(s[0] == '#') {
				c.color = s.ToInt32_(1, out int end, STIFlags.IsHexWithout0x);
				if(end < 2) return false;
				if(end <= 7) c.color |= unchecked((int)0xFF000000);
			} else {
				c.color = Color.FromName(s).ToArgb();
				if(c.color == 0) return false; //invalid is 0, black is 0xFF000000
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
		public int ToBGR(bool zeroAlpha = true)
		{
			var r = SwapRB(color);
			if(zeroAlpha) r &= 0xFFFFFF;
			return r;
		}

		/// <summary>
		/// Creates <see cref="Color"/> from ColorInt.
		/// </summary>
		public static implicit operator Color(ColorInt c) => Color.FromArgb(c.color);

		/// <summary>
		/// Creates int from ColorInt.
		/// </summary>
		public static implicit operator int(ColorInt c) => c.color;

		/// <summary>
		/// Converts color from ARGB (0xAARRGGBB) to ABGR (0xAABBGGRR) or vice versa (swaps the red and blue bytes).
		/// ARGB is used in .NET, GDI+ and HTML/CSS.
		/// ABGR is used by most Windows native API.
		/// </summary>
		public static int SwapRB(int color)
		{
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
		public float Brightness0to255()
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
		public ColorInt AdjustLuminance(int n, bool totalRange = false)
		{
			uint u = (uint)color;
			return new ColorInt((int)(Api.ColorAdjustLuma(u & 0xffffff, n, !totalRange) | (u & 0xFF000000)), false);
			//tested: with SwapRB the same.
		}

#pragma warning disable 1591 //XML doc
		public static bool operator ==(ColorInt a, ColorInt b) => a.color == b.color;
		public static bool operator !=(ColorInt a, ColorInt b) => a.color != b.color;
		public static bool operator ==(ColorInt a, int b) => a.color == b;
		public static bool operator !=(ColorInt a, int b) => a.color != b;
		public static bool operator ==(ColorInt a, Color b) => a.color == b.ToArgb();
		public static bool operator !=(ColorInt a, Color b) => a.color != b.ToArgb();

		public override bool Equals(object obj) => obj is ColorInt && this == (ColorInt)obj;
		public override int GetHashCode() => color;
		public override string ToString() => "0x" + color.ToString("X");
#pragma warning restore 1591 //XML doc
	}

	[DebuggerStepThrough]
	internal unsafe struct VARIANT :IDisposable
	{
		public Api.VARENUM vt; //ushort
		ushort _u1;
		uint _u2;
		public LPARAM value;
		public LPARAM value2;
		//note: cannot use FieldOffset because of different 32/64 bit size

		public VARIANT(int x) : this() { vt = Api.VARENUM.VT_I4; value = x; }
		public VARIANT(string x) : this() { vt = Api.VARENUM.VT_BSTR; value = Marshal.StringToBSTR(x); }

		public static implicit operator VARIANT(int x) { return new VARIANT(x); }
		public static implicit operator VARIANT(string x) { return new VARIANT(x); }

		public int ValueInt { get { Debug.Assert(vt == Api.VARENUM.VT_I4); return value; } }
		public BSTR ValueBstr { get { Debug.Assert(vt == Api.VARENUM.VT_BSTR); return BSTR.AttachBSTR((char*)value); } }

		/// <summary>
		/// Calls VariantClear.
		/// </summary>
		public void Dispose()
		{
			_Clear();
		}

		void _Clear()
		{
			if(vt >= Api.VARENUM.VT_BSTR) Api.VariantClear(ref this);
			else vt = 0; //info: VariantClear just sets vt=0 and does not clear other members
		}

		/// <summary>
		/// Converts to string.
		/// Does not cache.
		/// </summary>
		public override string ToString()
		{
			return _ToString(noCache: true);
		}

		string _ToString(bool noCache)
		{
			switch(vt) {
			case Api.VARENUM.VT_BSTR: return value == default ? null : ValueBstr.ToString();
			case Api.VARENUM.VT_I4: return value.ToString();
			case 0: case Api.VARENUM.VT_NULL: return null;
			}
			VARIANT v2 = default;
			uint lcid = 0x409; //invariant
			switch(vt & (Api.VARENUM)0xff) { case Api.VARENUM.VT_DATE: case Api.VARENUM.VT_DISPATCH: lcid = 0x400; break; } //LOCALE_USER_DEFAULT
			if(0 != Api.VariantChangeTypeEx(ref v2, ref this, lcid, 2, Api.VARENUM.VT_BSTR)) return null; //2 VARIANT_ALPHABOOL
			return v2.value == default ? null : v2.ValueBstr.ToStringAndDispose(noCache);
		}

		/// <summary>
		/// Converts to string.
		/// By default adds to our string cache.
		/// Disposes this VARIANT.
		/// </summary>
		public string ToStringAndDispose(bool noCache = false)
		{
			var r = _ToString(noCache);
			Dispose();
			return r;
		}
	}

	[DebuggerStepThrough]
	internal unsafe struct BSTR :IDisposable
	{
		char* _p;

		BSTR(char* p) => _p = p;

		//rejected: too easy to forget to dispose
		//public static implicit operator string(BSTR b)
		//{
		//	var p = b._p; if(p == null) return null;
		//	return Util.StringCache.LibAdd(p, SysStringLen(p));
		//}

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
		/// Does not dispose. Doen not cache.
		/// </summary>
		public override string ToString()
		{
			var p = _p; if(p == null) return null;
			return Marshal.PtrToStringBSTR((IntPtr)_p);
		}

		/// <summary>
		/// Converts to string and frees the BSTR (calls Dispose()).
		/// By default adds to our string cache.
		/// </summary>
		public string ToStringAndDispose(bool noCache = false)
		{
			var p = _p; if(p == null) return null;
			int len = Api.SysStringLen(p); if(len == 0) return "";

			//rejected:
			//Some objects can return BSTR containing '\0's. Then probably the rest of string is garbage. I never noticed this but saw comments. Better allow '\0's, because in some cases it can be valid string. When invalid, it will not harm too much.
			//int len2 = Util.LibCharPtr.Length(p, len); Debug_.PrintIf(len2 != len, "BSTR with '\\0'"); len = len2;

			string r = noCache ? Marshal.PtrToStringUni((IntPtr)p, len) : Util.StringCache.LibAdd(p, len);
			Dispose();
			return r;
		}

		public void Dispose()
		{
			var t = _p;
			if(t != null) {
				_p = null;
				Api.SysFreeString(t);
			}
		}
	}

}