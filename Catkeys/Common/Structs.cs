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

//using System.Xml.Serialization;
//using System.Xml;
//using System.Xml.Schema;

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
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
	public unsafe struct LPARAM //:IXmlSerializable
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
		//public static implicit operator LPARAM(WPARAM x) { return new LPARAM(x); }
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

		public override string ToString() { return ((IntPtr)_v).ToString(); }

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
	/// Unlike Rectangle, which contains fields for width and height and therefore cannot be used with Windows API functions, RECT contains fields for right and bottom and can be used with Windows API.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public struct RECT
	{
#pragma warning disable 1591 //XML doc
		public int left, top, right, bottom;

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

		public void Set(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight)
		{
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if(useWidthHeight) { right += left; bottom += top; }
		}

		/// <summary>
		/// Sets all fields to 0.
		/// </summary>
		public void SetEmpty() { left = right = top = bottom = 0; }

		/// <summary>
		/// Returns true if this rectangle is empty or invalid:
		/// return right&lt;=left || bottom&lt;=top;
		/// </summary>
		public bool IsEmpty { get => right <= left || bottom <= top; }

		/// <summary>
		/// Gets or sets width.
		/// </summary>
		public int Width { get => right - left; set { right = left + value; } }

		/// <summary>
		/// Gets or sets height.
		/// </summary>
		public int Height { get => bottom - top; set { bottom = top + value; } }

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
		/// Makes this rectangle bigger or smaller:
		/// left-=dx; right+=dx; top-=dy; bottom+=dy;
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
		public static RECT Intersect(RECT r1, RECT r2) { RECT r; Api.IntersectRect(out r, ref r1, ref r2); return r; }

		/// <summary>
		/// Returns true if this rectangle and another rectangle intersect.
		/// </summary>
		public bool IntersectsWith(RECT r2) { RECT r; return Api.IntersectRect(out r, ref this, ref r2); }

		/// <summary>
		/// Moves this rectangle by the specified offsets:
		/// left+=dx; right+=dx; top+=dy; bottom+=dy;
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
		public static RECT Union(RECT r1, RECT r2) { RECT r; Api.UnionRect(out r, ref r1, ref r2); return r; }

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
			Wnd.LibMoveInScreen(false, x, y, false, default(Wnd), ref this, screen, workArea, ensureInScreen);
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
			Wnd.LibMoveInScreen(true, null, null, false, default(Wnd), ref this, screen, workArea, true);
		}

		///// <summary>
		///// RECT with all fields 0.
		///// </summary>
		//public static readonly RECT Empty;

		public override string ToString() { return $"{{L={left} T={top} R={right} B={bottom}  Width={Width} Height={Height}}}"; }
#pragma warning restore 1591 //XML doc
	}
#pragma warning restore 660, 661

	[DebuggerStepThrough]
	internal struct VARIANT :IDisposable
	{
		public Api.VARENUM vt; //ushort
		public ushort _u1;
		public uint _u2;
		public LPARAM value;
		public LPARAM value2;
		//note: cannot use FieldOffset because of different 32/64 bit size

		public VARIANT(int x) : this() { vt = Api.VARENUM.VT_I4; value = x; }
		public VARIANT(string x) : this() { vt = Api.VARENUM.VT_BSTR; value = Marshal.StringToBSTR(x); }

		public static implicit operator VARIANT(int x) { return new VARIANT(x); }
		public static implicit operator VARIANT(string x) { return new VARIANT(x); }

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
	}

}
