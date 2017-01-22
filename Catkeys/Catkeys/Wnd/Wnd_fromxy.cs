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

using Catkeys;
using static Catkeys.NoClass;

namespace Catkeys
{
	public partial struct Wnd
	{
		#region top-level or child

		//TODO: test when this process is not DPI-aware:
		//	Coordinate-related API on Win7 high DPI.
		//	Acc location on Win10 with different DPI of monitors.

		/// <summary>
		/// Gets visible top-level window or visible enabled control from point.
		/// This function just calls Api.WindowFromPoint() and returns its return value.
		/// Use FromXY instead if you need "real" controls (include disabled controls, prefer non-transparent controls) or if you want only top-level or only child windows.
		/// </summary>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		public static Wnd FromXYRaw(int x, int y)
		{
			return Api.WindowFromPoint(new POINT(x, y));
		}
		/// <summary>
		/// Gets visible top-level window or visible enabled control from point.
		/// From other overload differs only by the parameter type.
		/// </summary>
		/// <param name="p">X and Y coordinates.</param>
		public static Wnd FromXYRaw(POINT p)
		{
			return Api.WindowFromPoint(p);
		}

		/// <summary>
		/// Gets visible top-level window or control from point.
		/// Unlike FromXYRaw(), this function gets non-transparent controls that are behind (in the Z order) transparent controls (eg a group button or a tab control).
		/// Also this function supports fractional coordinates and does not skip disabled controls.
		/// </summary>
		/// <param name="x">X coordinate. Can be int (pixels) or float (fraction of primary screen).</param>
		/// <param name="y">Y coordinate. Can be int (pixels) or float (fraction of primary screen).</param>
		/// <param name="control">
		/// If true, gets control; returns Wnd0 if there is no control at that point.
		/// If false, gets top-level window; if at that point is a control, gets its top-level parent.
		/// If omitted or null, gets exactly what is at that point (control or top-level window).
		/// </param>
		public static Wnd FromXY(Coord x, Coord y, bool? control = null)
		{
			return FromXY(Coord.GetNormalizedInScreen(x, y), control);
		}

		/// <summary>
		/// Gets visible top-level window or control from point.
		/// From other overload differs only by the parameter type.
		/// </summary>
		public static Wnd FromXY(POINT p, bool? control = null)
		{
			if(control != null && !control.Value) return _ToplevelWindowFromPoint(p);

			Wnd w = Api.WindowFromPoint(p);
			if(w.Is0) return w;

			Wnd t = w.DirectParent; //need parent because need to call realchildwindowfrompoint on it, else for group box would go through the slow way of detecting transparen control
			if(!t.Is0) w = t;

			t = w._ChildFromXY(p, false, true);
			if(t.Is0) t = w;
			if(control != null && t == w) return Wnd0;
			return t;
		}

		static Wnd _ToplevelWindowFromPoint(POINT p)
		{
			Wnd w = Api.RealChildWindowFromPoint(Api.GetDesktopWindow(), p);
			if(!w.HasExStyle(Api.WS_EX_TRANSPARENT | Api.WS_EX_LAYERED)) return w; //fast. Windows that have both these styles are mouse-transparent.
			return Api.WindowFromPoint(p).ToplevelParentOrThis; //ChildWindowFromPointEx would be faster, but less reliable

			//info:
			//WindowFromPoint is the most reliable. It skips really transparent top-level windows (TL). Unfortunately it skips disabled controls (but not TL).
			//ChildWindowFromPointEx with CWP_SKIPINVISIBLE|CWP_SKIPTRANSPARENT skips all with WS_EX_TRANSPARENT, although without WS_EX_LAYERED they aren't actually transparent.
			//RealChildWindowFromPoint does not skip transparent TL. It works like ChildWindowFromPointEx(CWP_SKIPINVISIBLE).
			//None of the above API prefers a really visible control that is under a transparent part of a sibling control. RealChildWindowFromPoint does it only for group buttons, but not for tab controls etc.
			//All API skip windows that have a hole etc in window region at that point.
			//None of the API uses WM_NCHITTEST+HTTRANSPARENT. Tested only with TL of other processes.
			//AccessibleObjectFromPoint.get_Parent in most cases gets the most correct window/control, but it is dirty, unreliable and often very slow, because sends WM_GETOBJECT etc.
			//speed:
			//RealChildWindowFromPoint is the fastest. About 5 mcs.
			//ChildWindowFromPointEx is 50% slower.
			//WindowFromPoint is 4 times slower; ToplevelParentOrThis does not make significantly slower.
			//AccessibleObjectFromPoint.get_Parent often is of RealChildWindowFromPoint speed, but often much slower than all others.
			//IUIAutomationElement.ElementFromPoint super slow, 6-10 ms. Getting window handle from it is not easy, > 0.5 ms.
		}

		//These are slower with some controls, faster with others.
		//public static Wnd FromXY2(POINT p, bool? control = null)
		//{
		//	Wnd w = _ToplevelWindowFromPoint(p);
		//	Debug.Assert(!w.Is0);
		//	if(w.Is0) return w;

		//	if(control != null && !control.Value) return w;

		//	Wnd t = w._ChildFromXY(p, false, true);
		//	if(t.Is0) t = w;
		//	if(control != null && t == w) return Wnd0;
		//	return t;
		//}

		//public static Wnd FromXY3(POINT p, bool? control = null)
		//{
		//	Wnd w = _ToplevelWindowFromPoint(p);
		//	if(w.Is0) return w;

		//	if(control != null && !control.Value) return w;

		//	Wnd t = w._ChildFromXY(p, false, true);
		//	if(t.Is0) t = w;
		//	if(control != null && t == w) return Wnd0;
		//	return t;
		//}

		/// <summary>
		/// Gets visible top-level window or control from mouse cursor position.
		/// Calls FromXY(Mouse.XY, control).
		/// </summary>
		/// <param name="control">
		/// If true, gets control; returns Wnd0 if there is no control at that point.
		/// If false, gets top-level window; if at that point is a control, gets its top-level parent.
		/// If omitted or null, gets exactly what is at that point (control or top-level window).
		/// </param>
		public static Wnd FromMouse(bool? control = null)
		{
			return FromXY(Mouse.XY, control);
		}

		#endregion

		#region child

		/// <summary>
		/// Gets child control from point.
		/// Returns Wnd0 if the point is not within a child or is outside this window.
		/// By default, x y must be relative to the client area of this window.
		/// </summary>
		/// <param name="x">X coordinate. Can be int (pixels) or float (fraction of primary screen).</param>
		/// <param name="y">Y coordinate. Can be int (pixels) or float (fraction of primary screen).</param>
		/// <param name="directChild">Get direct child, not a child of a child and so on.</param>
		/// <param name="screenXY">x y are relative to the pimary screen.</param>
		/// <exception cref="CatException">When this window is invalid (not found, closed, etc).</exception>
		/// <seealso cref="Wnd.FromXY(Coord, Coord, bool?)"/>
		public Wnd ChildFromXY(Coord x, Coord y, bool directChild = false, bool screenXY = false)
		{
			ValidateThrow();
			POINT p = screenXY ? Coord.GetNormalizedInScreen(x, y) : Coord.GetNormalizedInWindowClientArea(x, y, this);
			return _ChildFromXY(p, directChild, screenXY);
		}

		//Returns child or Wnd0.
		Wnd _ChildFromXY(POINT p, bool directChild, bool screenXY)
		{
			Wnd R = _TopChildWindowFromPointSimple(this, p, directChild, screenXY);
			if(R.Is0) return R;

			//Test whether it is a transparent control, like tab, covering other controls.
			//RealChildWindowFromPoint does it only for group button.

			if(R.HasExStyle(Api.WS_EX_MDICHILD)) return R;

			if(!screenXY) Api.ClientToScreen(this, ref p);
			g1:
			RECT r = R.Rect;
			for(Wnd t = R; ;) {
				t = Api.GetWindow(t, Api.GW_HWNDNEXT); if(t.Is0) break;
				RECT rr = t.Rect;
				if(!rr.Contains(p.x, p.y)) continue;
				if(!t.IsVisible) continue;
				if(rr.Width * rr.Height > r.Width * r.Height || rr == r) continue; //bigger than R, or equal

				//is R transparent?
				//PrintList("WM_NCHITTEST", R);
				LPARAM ht;
				//if(!R.SendTimeout(100, out ht, Api.WM_NCHITTEST, 0, Calc.MakeUint(p.x, p.y)) || (int)ht != Api.HTTRANSPARENT) break;
				if(R.SendTimeout(100, out ht, Api.WM_NCHITTEST, 0, Calc.MakeUint(p.x, p.y))) {
					if((int)ht != Api.HTTRANSPARENT) break;
				} else {
					//break;
					if(Marshal.GetLastWin32Error() != Api.ERROR_ACCESS_DENIED) break; //higher UAC level?
					if(rr.left <= r.left || rr.top <= r.top || rr.right >= r.right || rr.bottom >= r.bottom) break; //R must fully cover t, like a tab or group control
				}

				//now we know that R is transparent and there is another control behind
				if(!directChild) //that control can contain a child in that point, so we must find it
				{
					R = _TopChildWindowFromPointSimple(t, p, false, true);
					//Print(R);
					if(!R.Is0) goto g1;
				}
				R = t;
			}

			return R;
		}

		//Returns child or Wnd0.
		static Wnd _TopChildWindowFromPointSimple(Wnd w, POINT p, bool directChild, bool screenXY)
		{
			if(screenXY && !Api.ScreenToClient(w, ref p)) return Wnd0;

			for(Wnd R = Wnd0; ;) {
				Wnd t = _RealChildWindowFromPoint_RtlAware(w, p);
				if(directChild) return t;
				if(t.Is0 || !w.MapClientToClientOf(t, ref p)) return R;
				R = w = t;
			}
		}

		//Returns direct child or Wnd0.
		static Wnd _RealChildWindowFromPoint_RtlAware(Wnd w, POINT p)
		{
			RECT rc; if(w.HasExStyle(Api.WS_EX_LAYOUTRTL) && Api.GetClientRect(w, out rc)) { p.x = rc.right - p.x; }
			Wnd R = Api.RealChildWindowFromPoint(w, p);
			return R == w ? Wnd0 : R;
		}

		#endregion
	}
}
