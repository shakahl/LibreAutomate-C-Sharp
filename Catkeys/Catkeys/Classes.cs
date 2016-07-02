//Misc small classes and structs.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
using System.Windows.Forms;

using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

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
	public unsafe struct LPARAM :IXmlSerializable
	{
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
		public static implicit operator int (LPARAM x) { return (int)x._v; }
		public static implicit operator uint (LPARAM x) { return (uint)x._v; }
		public static implicit operator sbyte (LPARAM x) { return (sbyte)x._v; }
		public static implicit operator byte (LPARAM x) { return (byte)x._v; }
		public static implicit operator short (LPARAM x) { return (short)x._v; }
		public static implicit operator ushort (LPARAM x) { return (ushort)x._v; }
		public static implicit operator char (LPARAM x) { return (char)(ushort)x._v; }
		public static implicit operator long (LPARAM x) { return (long)x._v; }
		public static implicit operator ulong (LPARAM x) { return (ulong)x._v; }
		public static implicit operator bool (LPARAM x) { return x._v != null; }

		public static bool operator ==(LPARAM a, LPARAM b) { return a._v == b._v; }
		public static bool operator !=(LPARAM a, LPARAM b) { return a._v != b._v; }

		public override string ToString() { return ((int)_v).ToString(); }

		//IXmlSerializable implementation.
		//Need it because default serialization: 1. Gets only public members. 2. Exception if void*. 3. If would work, would format like <...><_v>value</_v></...>, but we need <...>value</...>.
		public XmlSchema GetSchema() { return null; }
		public void ReadXml(XmlReader reader) { _v = (void*)reader.ReadElementContentAsLong(); }
		public void WriteXml(XmlWriter writer) { writer.WriteValue((long)_v); }
	}

	/// <summary>
	/// Contains point coordinates.
	/// The same as System.Drawing.Point.
	/// </summary>
	public struct POINT
	{
		public int x, y;

		public POINT(int x, int y) { this.x = x; this.y = y; }

		public static implicit operator POINT(System.Drawing.Point p) { return new POINT(p.X, p.Y); }
		public static implicit operator System.Drawing.Point(POINT p) { return new System.Drawing.Point(p.x, p.y); }

		public static bool operator ==(POINT p1, POINT p2) { return p1.x == p2.x && p1.y == p2.y; }
		public static bool operator !=(POINT p1, POINT p2) { return !(p1 == p2); }

		public override string ToString() { return $"{{x={x} y={y}}}"; }

		/// <summary>
		/// POINT with all fields 0.
		/// </summary>
		public static readonly POINT Empty;
	}

	/// <summary>
	/// Contains width and height.
	/// The same as System.Drawing.Size.
	/// </summary>
	public struct SIZE
	{
		public int cx, cy;

		public SIZE(int cx, int cy) { this.cx = cx; this.cy = cy; }

		public static implicit operator SIZE(System.Drawing.Size z) { return new SIZE(z.Width, z.Height); }
		public static implicit operator System.Drawing.Size(SIZE z) { return new System.Drawing.Size(z.cx, z.cy); }

		public static bool operator ==(SIZE s1, SIZE s2) { return s1.cx == s2.cx && s1.cy == s2.cy; }
		public static bool operator !=(SIZE s1, SIZE s2) { return !(s1 == s2); }

		public override string ToString() { return $"{{cx={cx} cy={cy}}}"; }

		/// <summary>
		/// SIZE with all fields 0.
		/// </summary>
		public static readonly SIZE Empty;
	}

	/// <summary>
	/// Contains rectangle coordinates.
	/// Unlike System.Drawing.Rectangle, which contains fields for width and height and therefore cannot be used with Windows API functions, RECT contains fields for right and bottom and can be used with Windows API.
	/// </summary>
	public struct RECT
	{
		public int left, top, right, bottom;

		public RECT(int left, int top, int rightOrWidth, int bottomOrHeight, bool useWidthHeight)
		{
			this.left = left; this.top = top;
			right = rightOrWidth; bottom = bottomOrHeight;
			if(useWidthHeight) { right += left; bottom += top; }
		}

		public static implicit operator RECT(System.Drawing.Rectangle r) { return new RECT(r.Left, r.Top, r.Width, r.Height, true); }
		public static implicit operator System.Drawing.Rectangle(RECT r) { return new System.Drawing.Rectangle(r.left, r.top, r.Width, r.Height); }

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
		/// return right˂=left || bottom˂=top;
		/// </summary>
		public bool IsEmpty { get { return right <= left || bottom <= top; } }

		/// <summary>
		/// Gets or sets width.
		/// </summary>
		public int Width { get { return right - left; } set { right = left + value; } }

		/// <summary>
		/// Gets or sets height.
		/// </summary>
		public int Height { get { return bottom - top; } set { bottom = top + value; } }

		/// <summary>
		/// Returns true if this rectangle contains the specified point.
		/// </summary>
		public bool Contains(int x, int y) { return x >= left && x < right && y >= top && y < bottom; }

		/// <summary>
		/// Returns true if this rectangle contains entire specified rectangle.
		/// </summary>
		public bool Contains(RECT r2) { return r2.left >= left && r2.top >= top && r2.right <= right && r2.bottom <= bottom; }

		/// <summary>
		/// Returns true if this rectangle and another rectangle intersect.
		/// </summary>
		public bool Intersects(RECT r2) { RECT r; return Api.IntersectRect(out r, ref this, ref r2); }

		/// <summary>
		/// Virtually moves the rectangle:
		/// left+=dx; right+=dx; top+=dy; bottom+=dy;
		/// </summary>
		public void Offset(int dx, int dy) { left += dx; right += dx; top += dy; bottom += dy; }

		/// <summary>
		/// Makes the rectangle bigger or smaller:
		/// left-=dx; right+=dx; top-=dy; bottom+=dy;
		/// Note: negative coordinates can make the rectangle invalid (right˂left or bottom˂top).
		/// </summary>
		public void Inflate(int dx, int dy) { left -= dx; right += dx; top -= dy; bottom += dy; }

		/// <summary>
		/// Returns the intersection rectangle of two rectangles.
		/// If they don't intersect, returns empty rectangle (IsEmpty would return true).
		/// </summary>
		public static RECT Intersect(RECT r1, RECT r2) { RECT r; Api.IntersectRect(out r, ref r1, ref r2); return r; }

		/// <summary>
		/// Returns the rectangle that would contain two entire rectangles.
		/// </summary>
		public static RECT Union(RECT r1, RECT r2) { RECT r; Api.UnionRect(out r, ref r1, ref r2); return r; }

		/// <summary>
		/// RECT with all fields 0.
		/// </summary>
		public static readonly RECT Empty;

		public override string ToString() { return $"{{L={left} T={top} R={right} B={bottom}  Width={Width} Height={Height}}}"; }
	}
#pragma warning restore 660, 661

	////Instead use classes Screen and Screen_.
	//public static class DisplayMonitor
	//{
	//	public const int Primary = 0;
	//	public const int OfMouse = -1;
	//	public const int OfActiveWindow = -2;

	//	static IntPtr _GetMonitor(object monitor, out RECT rMonitor, out RECT rWorkArea, bool bGetRect)
	//	{
	//		var mi = new Api.MONITORINFO(); mi.cbSize=Api.SizeOf(mi);
	//		IntPtr hmon = Zero; bool bHaveRect = false;

	//		if(monitor==null) { } //primary
	//		else if(monitor is Wnd) {
	//			hmon=Api.MonitorFromWindow((Wnd)monitor, Api.MONITOR_DEFAULTTONEAREST);
	//		} else if(monitor is IntPtr) { //HMONITOR
	//			hmon=(IntPtr)monitor;
	//			bHaveRect=Api.GetMonitorInfo(hmon, ref mi); //because can be invalid HMONITOR
	//			if(!bHaveRect) hmon=Zero; //if fails, will get primary
	//		} else if(monitor is int) {
	//			int index = (int)monitor;
	//			if(index>0) {
	//				Api.EnumDisplayMonitors(Zero, Zero,
	//					delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rMonitor_, LPARAM dwData)
	//					{
	//						if(!Api.GetMonitorInfo(hMonitor, ref mi) || Api.MonitorFromRect(ref mi.rcMonitor, 0)!=hMonitor) return 1; //MonitorFromRect filters out pseudo monitors
	//						if((--index)!=0) return 1;
	//						hmon=hMonitor; bHaveRect=true;
	//						return 0;
	//					}
	//					, Zero);
	//			}
	//			else if(index==OfMouse) { //-1
	//				POINT p; Api.GetCursorPos(out p);
	//				hmon=Api.MonitorFromPoint(p, Api.MONITOR_DEFAULTTONEAREST);
	//			} else if(index==OfActiveWindow) { //-2
	//				hmon=Api.MonitorFromWindow(Wnd.Get.Active(), Api.MONITOR_DEFAULTTONEAREST);
	//			} //else Primary (0) or invalid
	//		} else if(monitor is POINT) {
	//			hmon=Api.MonitorFromPoint((POINT)monitor, Api.MONITOR_DEFAULTTONEAREST);
	//		} else if(monitor is RECT) {
	//			RECT r_ = (RECT)monitor;
	//			hmon=Api.MonitorFromRect(ref r_, Api.MONITOR_DEFAULTTONEAREST);
	//		} else throw new ArgumentException("monitor argument type mismatch");

	//		if(hmon==Zero) hmon=Api.MonitorFromWindow(Wnd0, Api.MONITOR_DEFAULTTOPRIMARY); //get primary monitor

	//		if(bGetRect) {
	//			if(!bHaveRect) Api.GetMonitorInfo(hmon, ref mi);
	//			rMonitor=mi.rcMonitor; rWorkArea=mi.rcWork;
	//		} else rMonitor=rWorkArea=new RECT();

	//		return hmon;

	//		//notes:
	//		//Strange, but this function is 2-4 times faster than GetSystemmetrics*2 or SystemParametersInfo SPI_GETWORKAREA.
	//	}

	//	/// <summary>
	//	/// Gets display monitor handle from index, point, rectangle, window, etc.
	//	/// If the specified object is not in a display monitor, gets nearest display monitor.
	//	/// </summary>
	//	/// <param name="monitor">Monitor index (int, > 0), or window (Wnd), or POINT, or RECT, or monitor handle (IntPtr), or DisplayMonitor.Primary/OfMouse/OfActiveWindow. If null, 0, or invalid, gets primary.</param>
	//	public static IntPtr GetHandle(object monitor)
	//	{
	//		RECT r, rw;
	//		return _GetMonitor(monitor, out r, out rw, false);
	//	}

	//	/// <summary>
	//	/// Gets display monitor handle from index, point, rectangle, window, etc. Also gets its full and work-area rectangles.
	//	/// If the specified object is not in a display monitor, gets nearest display monitor.
	//	/// </summary>
	//	/// <param name="monitor">Monitor index (int, > 0), or window (Wnd), or POINT, or RECT, or monitor handle (IntPtr), or DisplayMonitor.Primary/OfMouse/OfActiveWindow. If null, 0, or invalid, gets primary.</param>
	//	public static IntPtr GetHandleAndRectangles(object monitor, out RECT rMonitor, out RECT rWorkArea)
	//	{
	//		return _GetMonitor(monitor, out rMonitor, out rWorkArea, true);
	//	}

	//	/// <summary>
	//	/// Gets display monitor rectangle from index, point, rectangle, window, etc.
	//	/// If the specified object is not in a display monitor, gets nearest display monitor.
	//	/// </summary>
	//	/// <param name="monitor">Monitor index (int, > 0), or window (Wnd), or POINT, or RECT, or monitor handle (IntPtr), or DisplayMonitor.Primary/OfMouse/OfActiveWindow. If null, 0, or invalid, gets primary.</param>
	//	public static RECT GetRectangle(object monitor)
	//	{
	//		RECT r, rw;
	//		_GetMonitor(monitor, out r, out rw, false);
	//		return r;
	//	}

	//	/// <summary>
	//	/// Gets display monitor work area rectangle from index, point, rectangle, window, etc.
	//	/// If the specified object is not in a display monitor, gets nearest display monitor.
	//	/// </summary>
	//	/// <param name="monitor">Monitor index (int, > 0), or window (Wnd), or POINT, or RECT, or monitor handle (IntPtr), or DisplayMonitor.Primary/OfMouse/OfActiveWindow. If null, 0, or invalid, gets primary.</param>
	//	public static RECT GetWorkAreaRectangle(object monitor)
	//	{
	//		RECT r, rw;
	//		_GetMonitor(monitor, out r, out rw, false);
	//		return rw;
	//	}
	//}

	/// <summary>
	/// Stores an x or y coordinate as pixels or as a fraction of some rectangle.
	/// Can be used for function parameters. Accepts values of type int or float.
	/// </summary>
	public class Coord
	{
		public int coord;
		public float fraction;

		public bool IsFraction { get { return fraction != 0f; } }

		public Coord(int coord) { this.coord = coord; }
		public Coord(float fraction) { this.fraction = fraction; }
		public Coord(Coord z) { coord = z.coord; fraction = z.fraction; }

		public static implicit operator Coord(int coord) { return new Coord(coord); }
		public static implicit operator Coord(float fraction) { return new Coord(fraction); }

		/// <summary>
		/// Sets coord = GetNormalized(min, max). Sets IsFraction = false.
		/// </summary>
		public void Normalize(int min, int max)
		{
			coord = GetNormalized(min, max);
			fraction = 0f;
		}

		/// <summary>
		/// If IsFraction == false, returns coord + min.
		/// Else calculates and returns non-fraction coordinate: (int)((max - min) * fraction) + min.
		/// </summary>
		public int GetNormalized(int min, int max)
		{
			return (IsFraction ? (int)((max - min) * fraction) : coord) + min;
		}

		/// <summary>
		/// Returns new POINT(x, y), relative to the primary screen, where fractional x and/or y are converted to pixels.
		/// x or y can be null, then the returned x or y value will be 0.
		/// </summary>
		/// <param name="workArea">If false, coordinates are in primary screen, else in its work area.</param>
		/// <param name="widthHeight">Don't add work area offset. Use when x and y are width and height.</param>
		public static POINT GetNormalizedInScreen(Coord x, Coord y, bool workArea = false, bool widthHeight = false)
		{
			var p = new POINT();
			if(x != null || y != null) {
				if(workArea) {
					RECT r = Screen_.WorkArea; if(widthHeight) r.Offset(-r.left, -r.top);
					if(x != null) p.x = x.GetNormalized(r.left, r.right);
					if(y != null) p.y = y.GetNormalized(r.top, r.bottom);
				} else {
					if(x != null) p.x = x.IsFraction ? x.GetNormalized(0, Screen_.Width) : x.coord;
					if(y != null) p.y = y.IsFraction ? y.GetNormalized(0, Screen_.Height) : y.coord;
				}
			}
			return p;
		}

		/// <summary>
		/// Returns new POINT(x, y), relative to the window w client area, where fractional x and/or y are converted to pixels.
		/// x or y can be null, then the returned x or y value will be 0.
		/// </summary>
		public static POINT GetNormalizedInWindowClientArea(Coord x, Coord y, Wnd w)
		{
			var p = new POINT();
			if(x != null || y != null) {
				if((x != null && x.IsFraction) || (y != null && y.IsFraction)) {
					RECT r = w.ClientRect;
					if(x != null) p.x = x.GetNormalized(0, r.right);
					if(y != null) p.y = y.GetNormalized(0, r.bottom);
				} else {
					if(x != null) p.x = x.coord;
					if(y != null) p.y = y.coord;
				}
			}
			return p;
		}

		public override string ToString()
		{
			return IsFraction ? fraction.ToString() : coord.ToString();
		}
	}

}
