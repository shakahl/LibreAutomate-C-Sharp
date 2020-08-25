using Au.Types;
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

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Controls;

namespace Au
{
	/// <summary>
	/// Adds extension methods for some WPF classes.
	/// </summary>
	public static class AExtWpf
	{
		/// <summary>
		/// Gets native window handle of this <b>Window</b> or <b>Popup</b>, or container window handle of this child object.
		/// Returns <c>default(AWnd)</c> if: called before creating or after closing real window; failed; <i>t</i> is null.
		/// </summary>
		/// <param name="t"></param>
		public static AWnd Hwnd(this DependencyObject t) {
			switch (t) {
			case null: return default;
			case Window w: return (AWnd)new WindowInteropHelper(w).Handle; //FromDependencyObject works too, but this is usually slightly faster
			case Popup p: t = p.Child; if (t == null) return default; break; //FromVisual(Popup) returns null; or maybe owner window, not tested.
			}
			if (PresentationSource.FromDependencyObject(t) is HwndSource hs) return (AWnd)hs.Handle;
			return default;
		}
		//rejected: notPopup. Not useful.
		///// <summary>
		///// Gets window handle of this <b>Window</b>, <b>Popup</b> or container window handle of this child object.
		///// Returns <c>default(AWnd)</c> if: called before creating real window; failed; <i>t</i> is null.
		///// </summary>
		///// <param name="t"></param>
		///// <param name="notPopup">If this is <b>Popup</b> or in a <b>Popup</b>, get handle of popup's owner <b>Window</b>.</param>
		//public static AWnd Hwnd(this DependencyObject t, bool notPopup = false)
		//{
		//	switch(t) {
		//	case null: return default;
		//	case Window w: return (AWnd)new WindowInteropHelper(w).Handle; //FromDependencyObject works too, but this is usually slightly faster
		//	case Popup p when !notPopup: t = p.Child; if(t == null) return default; break; //FromVisual(Popup) returns null; or maybe owner window, not tested.
		//	}
		//	if(notPopup) {
		//		var w = Window.GetWindow(t); if(w == null) return default; //if Popup or in Popup, gets owner WIndow
		//		return (AWnd)new WindowInteropHelper(w).Handle;
		//	}
		//	if(PresentationSource.FromDependencyObject(t) is HwndSource hs) return (AWnd)hs.Handle;
		//	return default;
		//}

		internal static RECT RectToScreen_(this FrameworkElement t) {
			Point p1 = t.PointToScreen(default), p2 = t.PointToScreen(new Point(t.ActualWidth, t.ActualHeight));
			return new RECT(p1.X.ToInt(), p1.Y.ToInt(), p2.X.ToInt(), p2.Y.ToInt(), false);
		}

		/// <summary>
		/// Returns true if <see cref="ToggleButton.IsChecked"/> == true.
		/// </summary>
		/// <param name="t"></param>
		public static bool IsCheck(this CheckBox t) => t.IsChecked.GetValueOrDefault();

#if true
		static void _Move(Window t, int x, int y, in RECT r, bool andSize) {
			var wstate = t.WindowState;
			if (wstate != WindowState.Normal) t.WindowState = WindowState.Normal;
			if (t.IsLoaded) {
				var w = t.Hwnd();
				if (w.Is0) throw new ObjectDisposedException("Window");
				if (andSize) w.MoveLL(r); else w.MoveLL(x, y);
			} else {
				//tested: don't need this for Popup. Its PlacementRectangle uses physical pixels.

				if (andSize) {
					x = r.left; y = r.top;
					var stc = t.SizeToContent;
					if (stc != SizeToContent.WidthAndHeight) {
						double f = 96d / AScreen.Of(new POINT(x, y)).Dpi;
						if (!stc.Has(SizeToContent.Width)) t.Width = r.Width * f;
						if (!stc.Has(SizeToContent.Height)) t.Height = r.Height * f;
					}
				}

				t.WindowStartupLocation = WindowStartupLocation.Manual;
				t.Left = 0; t.Top = 0;
				if (wstate == WindowState.Minimized) t.ShowActivated = false;

				t.Loaded += (_, _) => {
					var w = t.Hwnd();
					var v = AScreen.Of(new POINT(x, y)).GetInfo();
					var rs = v.workArea;
					if (!v.isPrimary) {
						using var h = AHookWin.ThreadCbt(k => k.code == HookData.CbtEvent.ACTIVATE); //workaround for WPF bug: activates window when DPI changes
						w.MoveLL(rs.left, rs.top); //let DPI-scale
					}
					var rw = w.Rect;
					x = Math.Clamp(x, rs.left, Math.Max(rs.right - rw.Width, rs.left));
					y = Math.Clamp(y, rs.top, Math.Max(rs.bottom - rw.Height, rs.top));
					w.MoveLL(x, y);
					//speed: when moving to a screen with different DPI, total time is same.

					if (wstate != WindowState.Normal) {
						if (wstate == WindowState.Maximized) t.SizeToContent = SizeToContent.Manual;
						t.WindowState = wstate;
					} else if (!t.ShowActivated) {
						w.ZorderTop();
					}
				};
			}
		}
#else //slightly faster when moved to a different-dpi screen, but dirtier, I don't like creating window handle before showing window
		static void _Move(Window t, int x, int y, in RECT r, bool andSize) {
			var wstate=t.WindowState;
			if(wstate!=WindowState.Normal) t.WindowState=WindowState.Normal;
			if(t.IsLoaded) {
				var w=t.Hwnd();
				if(w.Is0) throw new ObjectDisposedException("Window");
				if(andSize) w.MoveLL(r); else w.MoveLL(x, y);
			} else {
				var screen=AScreen.Of(new POINT(x, y));
				var si=screen.GetInfo();
				var rs=si.workArea;
			
				if(andSize) {
					x=r.left; y=r.top;
					var stc=t.SizeToContent;
					if(stc!=SizeToContent.WidthAndHeight) {
						double f=96d/screen.Dpi;
						if(!stc.Has(SizeToContent.Width)) t.Width=r.Width*f;
						if(!stc.Has(SizeToContent.Height)) t.Height=r.Height*f;
					}
				}
			
				t.WindowStartupLocation=WindowStartupLocation.Manual;
				t.Left=double.NaN; t.Top=double.NaN;
				t.Loaded+=(_,_)=> {
					var w=t.Hwnd();
					var rw=w.Rect;
					x=Math.Clamp(x, rs.left, Math.Max(rs.right-rw.Width, rs.left));
					y=Math.Clamp(y, rs.top, Math.Max(rs.bottom-rw.Height, rs.top));
					w.MoveLL(x, y);
				
					if(wstate!=WindowState.Normal) {
						if(wstate==WindowState.Maximized) t.SizeToContent=SizeToContent.Manual;
						t.WindowState=wstate;
					} else if(!t.ShowActivated) {
						w.ZorderTop();
					}
				};
			
				if(!si.isPrimary) {
					using var h=AHookWin.ThreadCbt(d=> {
						if(d.code== HookData.CbtEvent.CREATEWND) unsafe {
							var w=d.CreationInfo(out var c, out _);
							if(c->style!=0 && !c->style.Has(WS.CHILD)) {
								AOutput.Write(c->x, c->y, c->cx, c->cy, c->hwndParent, c->style, c->lpszClass, c->lpszName);
				//				d.hook.Unhook();
								c->x=rs.left; c->y=rs.top;
								//the hook receives 2 windows. At first the true window and then some other HwndWrapper* with 0 x y cx cy style parent. The second is never visibe.
								//We use the 'c->style!=0' to ignore it. The real window always has some styles. There is no 100% reliable and clean way to recognize the real window.
								//Don't unhook, because future .NET versions may create more windows, maybe some before the real window. Or in some conditions.
							}
						}
						return false;
					});

					new WindowInteropHelper(t).EnsureHandle();
				}
			}
		}
#endif

		/// <summary>
		/// Sets window startup location before showing it for first time. Also can move already loaded window.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in screen. Physical pixels.</param>
		/// <param name="y">Y coordinate in screen. Physical pixels.</param>
		/// <remarks>
		/// The unit is physical pixels. WPF provides <b>Left</b> and <b>Top</b> properties, but the unit is logical pixels, therefore cannot set exact location on high DPI screens, especially if there are mutiple screens with different DPI.
		/// 
		/// If the window is already loaded, just ensures it is not maximized/minimized and calls <see cref="AWnd.MoveLL"/>.
		/// Else:
		/// - Sets window location for normal state (not minimized/maximized).
		/// - Ensures that entire window or its top-left part is in screen that contains <i>x y</i> or is nearest.
		/// - Changes these window properties if need: <b>WindowStartupLocation</b>, <b>Left</b>, <b>Top</b>, <b>ShowActivated</b>.
		/// - To set location etc uses the <b>Loaded</b> event. The event handler runs later.
		/// - All this happens while the window is still invisible.
		/// </remarks>
		public static void SetXY(this Window t, int x, int y) => _Move(t, x, y, default, false);

		/// <summary>
		/// Sets window startup rectangle (location and size) before showing it for first time. Also can move/resize already loaded window.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="r">Rectangle in screen. Physical pixels.</param>
		/// <remarks>
		/// The unit is physical pixels. WPF provides <b>Left</b>, <b>Top</b>, <b>Width</b> and <b>Height</b> properties, but the unit is logical pixels, therefore cannot set exact rectangle on high DPI screens, especially if there are mutiple screens with different DPI.
		/// 
		/// If the window is already loaded, just ensures it is not maximized/minimized and calls <see cref="AWnd.MoveLL"/>.
		/// Else:
		/// - Sets window rectangle for normal state (not minimized/maximized).
		/// - Ensures that entire window or its top-left part is in screen that contains the top-left of <i>r</i> or is nearest.
		/// - Changes these window properties if need: <b>WindowStartupLocation</b>, <b>Left</b>, <b>Top</b>, <b>Width</b>, <b>Height</b>, <b>ShowActivated</b>. Does not change <b>SizeToContent</b>, therefore may not change width or/and height.
		/// - To set location etc uses the <b>Loaded</b> event. The event handler runs later.
		/// - All this happens while the window is still invisible.
		/// </remarks>
		public static void SetRect(this Window t, RECT r) => _Move(t, 0, 0, r, true);
	}
}

//namespace Au.Types
//{
//	///
//	public enum EnsureIn
//	{
//		///
//		None,
//		///
//		Screen,
//		///
//		WorkArea
//	}
//}
