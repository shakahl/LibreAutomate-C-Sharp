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
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	public partial struct AWnd
	{
		#region top-level or child

		//FUTURE: test when this process is not DPI-aware:
		//	Coordinate-related API on Win7 high DPI.
		//	AO location on Win10 with different DPI of monitors.

		/// <summary>
		/// Gets visible top-level window or control from point.
		/// </summary>
		/// <param name="p">
		/// Coordinates relative to the primary screen.
		/// Tip: When need coordinates relative to another screen or/and the work area, use <see cref="Coord.Normalize"/> or tuple (x, y, workArea) etc. Example: <c>var a = AWnd.FromXY((x, y, true));</c>. Also when need <see cref="Coord.Reverse"/> etc.
		/// </param>
		/// <param name="flags"></param>
		/// <remarks>
		/// Alternatively can be used API <msdn>WindowFromPoint</msdn>, <msdn>ChildWindowFromPointEx</msdn> or <msdn>RealChildWindowFromPoint</msdn>, but all they have various limitations and are not very useful in automation scripts.
		/// This function gets non-transparent controls that are behind (in the Z order) transparent controls (group button, tab control etc); supports more control types than <msdn>RealChildWindowFromPoint</msdn>. Also does not skip disabled controls. All this is not true with flag Raw.
		/// This function is not very fast, probably 0.3 - 1 ms.
		/// </remarks>
		public static AWnd FromXY(POINT p, WXYFlags flags = 0)
		{
			bool needW = 0 != (flags & WXYFlags.NeedWindow);
			bool needC = 0 != (flags & WXYFlags.NeedControl);
			if(needW && needC) throw new ArgumentException("", "flags");
			AWnd w;

			if(needW) {
				w = Api.RealChildWindowFromPoint(Api.GetDesktopWindow(), p); //much faster than WindowFromPoint
				if(!w.HasExStyle(WS_EX.TRANSPARENT | WS_EX.LAYERED)) return w; //fast. Windows that have both these styles are mouse-transparent.
				return Api.WindowFromPoint(p).Window; //ChildWindowFromPointEx would be faster, but less reliable

				//info:
				//WindowFromPoint is the most reliable. It skips really transparent top-level windows (TL). Unfortunately it skips disabled controls (but not TL).
				//ChildWindowFromPointEx with CWP_SKIPINVISIBLE|CWP_SKIPTRANSPARENT skips all with WS_EX_TRANSPARENT, although without WS_EX_LAYERED they aren't actually transparent.
				//RealChildWindowFromPoint does not skip transparent TL. It works like ChildWindowFromPointEx(CWP_SKIPINVISIBLE).
				//None of the above API prefers a really visible control that is under a transparent part of a sibling control. RealChildWindowFromPoint does it only for group buttons, but not for tab controls etc.
				//All API skip windows that have a hole etc in window region at that point.
				//None of the API uses WM_NCHITTEST+HTTRANSPARENT. Tested only with TL of other processes.
				//AccessibleObjectFromPoint.get_Parent in most cases gets the most correct window/control, but it is dirty, unreliable and often very slow, because sends WM_GETOBJECT etc.
				//speed:
				//RealChildWindowFromPoint is the fastest.
				//ChildWindowFromPointEx is slower.
				//WindowFromPoint is 4 times slower; .Window does not make significantly slower.
				//AccessibleObjectFromPoint.get_Parent often is of RealChildWindowFromPoint speed, but often much slower than all others.
				//IUIAutomation.FromPoint very slow. Getting window handle from it is not easy, > 0.5 ms.
			}

			w = Api.WindowFromPoint(p);
			if(w.Is0) return w;

			if(0 != (flags & WXYFlags.Raw)) {
				if(needW) w = w.Window; else if(needC && w == w.Window) w = default;
				return w;
			}

			AWnd t = w.Get.DirectParent; //need parent because need to call realchildwindowfrompoint on it, else for group box would go through the slow way of detecting transparen control
			if(!t.Is0) w = t;

			t = w._ChildFromXY(p, false, true);
			if(t.Is0) t = w;
			if(needC && t == w) return default;
			return t;
		}
		//rejected: FromXY(Coord, Coord, ...). Coord makes no sense; could be int int, but it's easy to create POINT from it.

		/// <summary>
		/// Gets visible top-level window or control from mouse cursor position.
		/// More info: <see cref="FromXY"/>.
		/// </summary>
		public static AWnd FromMouse(WXYFlags flags = 0)
		{
			return FromXY(AMouse.XY, flags);
		}

		#endregion

		#region child

		/// <summary>
		/// Gets child control from point.
		/// Returns default(AWnd) if the point is not in a child control or not in the client area of this window.
		/// </summary>
		/// <param name="x">X coordinate in the client area of this window. Can be <see cref="Coord.Reverse"/> etc.</param>
		/// <param name="y">Y coordinate in the client area of this window. Can be <b>Coord.Reverse</b> etc.</param>
		/// <param name="screenXY">The coordinates are relative to the pimary screen, not to the client area.</param>
		/// <param name="directChild">Get direct child, not a child of a child and so on.</param>
		/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
		public AWnd ChildFromXY(Coord x, Coord y, bool screenXY = false, bool directChild = false)
		{
			ThrowIfInvalid();
			POINT p = screenXY ? Coord.Normalize(x, y) : Coord.NormalizeInWindow(x, y, this);
			return _ChildFromXY(p, directChild, screenXY);
		}

		/// <param name="p">Coordinates in the client area of this window.</param>
		/// <param name="screenXY"></param>
		/// <param name="directChild"></param>
		/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
		public AWnd ChildFromXY(POINT p, bool screenXY = false, bool directChild = false)
		{
			ThrowIfInvalid();
			return _ChildFromXY(p, directChild, screenXY);
		}

		//Returns child or default(AWnd).
		AWnd _ChildFromXY(POINT p, bool directChild, bool screenXY)
		{
			AWnd R = _TopChildWindowFromPointSimple(this, p, directChild, screenXY);
			if(R.Is0) return R;

			//Test whether it is a transparent control, like tab, covering other controls.
			//RealChildWindowFromPoint does it only for group button.

			if(R.HasExStyle(WS_EX.MDICHILD)) return R;

			if(!screenXY) Api.ClientToScreen(this, ref p);
			g1:
			RECT r = R.Rect;
			for(AWnd t = R; ;) {
				t = Api.GetWindow(t, Api.GW_HWNDNEXT); if(t.Is0) break;
				RECT rr = t.Rect;
				if(!rr.Contains(p)) continue;
				if(!t.IsVisible) continue;
				if(rr.Width * rr.Height > r.Width * r.Height || rr == r) continue; //bigger than R, or equal

				//is R transparent?
				//Print("WM_NCHITTEST", R);
				if(R.SendTimeout(100, out var ht, Api.WM_NCHITTEST, 0, AMath.MakeUint(p.x, p.y))) {
					if((int)ht != Api.HTTRANSPARENT) break;
				} else {
					//break;
					if(ALastError.Code != Api.ERROR_ACCESS_DENIED) break; //higher UAC level?
					if(rr.left <= r.left || rr.top <= r.top || rr.right >= r.right || rr.bottom >= r.bottom) break; //R must fully cover t, like a tab or group control
				}

				//now we know that R is transparent and there is another control behind
				if(!directChild) //that control can contain a child in that point, so let's find it
				{
					R = _TopChildWindowFromPointSimple(t, p, false, true);
					//Print(R);
					if(!R.Is0) goto g1;
				}
				R = t;
			}

			return R;
		}

		//Returns child or default(AWnd).
		static AWnd _TopChildWindowFromPointSimple(AWnd w, POINT p, bool directChild, bool screenXY)
		{
			if(screenXY && !Api.ScreenToClient(w, ref p)) return default;

			for(AWnd R = default; ;) {
				AWnd t = _RealChildWindowFromPoint_RtlAware(w, p);
				if(directChild) return t;
				if(t.Is0 || !w.MapClientToClientOf(t, ref p)) return R;
				R = w = t;
			}
		}

		//Returns direct child or default(AWnd).
		static AWnd _RealChildWindowFromPoint_RtlAware(AWnd w, POINT p)
		{
			if(w.HasExStyle(WS_EX.LAYOUTRTL) && Api.GetClientRect(w, out var rc)) { p.x = rc.right - p.x; }
			AWnd R = Api.RealChildWindowFromPoint(w, p);
			return R == w ? default : R;
		}

		#endregion

		/// <summary>
		/// Get sibling control in space: left, right, above or below.
		/// Returns default(AWnd) if there is no sibling.
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="distance">Distance from this control (from its edge) in the specified direction. Default 10.</param>
		/// <param name="edgeOffset">
		/// Distance in perpendicular direction, along the specified edge. Default 5.
		/// If <i>direction</i> is <b>Left</b> or <b>Right</b>, 0 is the top edge, 1 is 1 pixel down, -1 is 1 pixel up, and so on.
		/// If <i>direction</i> is <b>Above</b> or <b>Below</b>, 0 is the left edge, 1 is 1 pixel to the right, -1 is 1 pixel to the left, and so on.
		/// </param>
		/// <param name="topChild">If at that point is a visible child or descendant of the sibling, get that child/descendant. Default false.</param>
		/// <exception cref="AuWndException">This variable is invalid (window not found, closed, etc).</exception>
		/// <remarks>
		/// This function is used mostly with controls, but supports top-level windows too.
		/// </remarks>
		AWnd _SiblingXY(_SibXY direction, int distance = 10, int edgeOffset = 5, bool topChild = false)
		{
			AWnd w = Get.DirectParent;
			if(!(w.Is0 ? GetRect(out RECT r) : GetRectIn(w, out r))) ThrowUseNative(); //note: most other AWnd 'get' functions don't throw, but here it's better to throw.
			POINT p = default;
			switch(direction) {
			case _SibXY.Left: p = (r.left - distance, r.top + edgeOffset); break;
			case _SibXY.Right: p = (r.right + distance, r.top + edgeOffset); break;
			case _SibXY.Above: p = (r.left + edgeOffset, r.top - distance); break;
			case _SibXY.Below: p = (r.left + edgeOffset, r.bottom + distance); break;
			}
			//Print(p); if(w.Is0) AMouse.Move(p); else AMouse.Move(w, p.x, p.y);
			AWnd R = w.Is0
				? FromXY(p, topChild ? 0 : WXYFlags.NeedWindow)
				: w._ChildFromXY(p, !topChild, false);
			return R == this ? default : R; //cannot return self, but can return child if topChild, it's ok
		}

		enum _SibXY { Left, Right, Above, Below }
	}
}

namespace Au.Types
{
	/// <summary>
	/// Flags for <see cref="AWnd.FromXY"/> and <see cref="AWnd.FromMouse"/>.
	/// </summary>
	[Flags]
	public enum WXYFlags
	{
		/// <summary>
		/// Need top-level window. If at that point is a control, gets its top-level parent.
		/// Don't use together with NeedControl.
		/// If none of flags NeedWindow and NeedControl are specified, the function gets exactly what is at that point (control or top-level window).
		/// </summary>
		NeedWindow = 1,

		/// <summary>
		/// Need a control (child window). Returns default(AWnd) if there is no control at that point.
		/// Don't use together with NeedWindow.
		/// If none of flags NeedWindow and NeedControl are specified, the function gets exactly what is at that point (control or top-level window).
		/// </summary>
		NeedControl = 2,

		/// <summary>
		/// Just call API <msdn>WindowFromPoint</msdn>.
		/// Faster but less accurate with controls. Does not see disabled controls, does not prefer non-transparent controls.
		/// Not used with flag NeedWindow.
		/// </summary>
		Raw = 4,
	}
}
