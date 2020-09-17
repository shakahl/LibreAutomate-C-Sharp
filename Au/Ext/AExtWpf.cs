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
using System.Windows.Media;

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

		/// <summary>
		/// Enumerates visual descendant objects, including parts of composite controls, and calls callback function <i>f</i> for each.
		/// When <i>f</i> returns true, stops and returns that object. Returns null if <i>f</i> does not return true.
		/// </summary>
		public static DependencyObject FindVisualDescendant(this DependencyObject t, Func<DependencyObject, bool> f) {
			for (int i = 0, n = VisualTreeHelper.GetChildrenCount(t); i < n; i++) {
				var v = VisualTreeHelper.GetChild(t, i);
				if (f(v)) return v;
				v = FindVisualDescendant(v, f);
				if (v != null) return v;
			}
			return null;
		}

		/// <summary>
		/// Finds object in chain of visual tree objects that includes <see cref="RoutedEventArgs.Source"/> and its ancestors until <i>sender</i> (including).
		/// </summary>
		/// <param name="t">The event arguments parameter of an event handler.</param>
		/// <param name="sender">The <i>sender</i> parameter of the event handler.</param>
		/// <param name="f">Function that returns true for the wanted object.</param>
		public static DependencyObject SourceOrAncestor(this RoutedEventArgs t, object sender, Func<DependencyObject, bool> f) {
			for (var v = t.Source as DependencyObject; v != null; v = VisualTreeHelper.GetParent(v)) {
				if (f(v)) return v;
				if (v == sender) break;
			}
			return null;
		}

		/// <summary>
		/// Finds object of type T (or inherited) in chain of visual tree objects that includes <see cref="RoutedEventArgs.Source"/> and its ancestors until <i>sender</i> (including).
		/// </summary>
		/// <param name="t">The event arguments parameter of an event handler.</param>
		/// <param name="sender">The <i>sender</i> parameter of the event handler.</param>
		/// <param name="orSender">If object of type T not found, return <i>sender</i>. If false (default), returns null.</param>
		public static DependencyObject SourceOfType<T>(this RoutedEventArgs t, object sender, bool orSender = false) where T : DependencyObject {
			for (var v = t.Source as DependencyObject; v != null; v = VisualTreeHelper.GetParent(v)) {
				if (v is T r1) return r1;
				if (v == sender) return orSender ? v : null;
			}
			return null;
		}

		/// <summary>
		/// Finds object of type T1 or T2 (or inherited) in chain of visual tree objects that includes <see cref="RoutedEventArgs.Source"/> and its ancestors until <i>sender</i> (including).
		/// </summary>
		/// <param name="t">The event arguments parameter of an event handler.</param>
		/// <param name="sender">The <i>sender</i> parameter of the event handler.</param>
		/// <param name="orSender">If object of type T1 or T2 not found, return <i>sender</i>. If false (default), returns null.</param>
		public static DependencyObject SourceOfType<T1, T2>(this RoutedEventArgs t, object sender, bool orSender = false) where T1 : DependencyObject where T2 : DependencyObject {
			for (var v = t.Source as DependencyObject; v != null; v = VisualTreeHelper.GetParent(v)) {
				if (v is T1 r1) return r1;
				if (v is T2 r2) return r2;
				if (v == sender) return orSender ? v : null;
			}
			return null;
		}

		/// <summary>
		/// Gets rectangle of this element in screen coordinates.
		/// </summary>
		public static RECT RectInScreen(this FrameworkElement t) {
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
						double f = 96d / AScreen.Of(x, y).Dpi;
						if (!stc.Has(SizeToContent.Width)) t.Width = r.Width * f;
						if (!stc.Has(SizeToContent.Height)) t.Height = r.Height * f;
					}
				}

				t.WindowStartupLocation = WindowStartupLocation.Manual;
				t.Left = double.NaN; t.Top = double.NaN; //default location, somewhere near top-left of primary screen or owner's screen
				if (wstate == WindowState.Minimized) t.ShowActivated = false;

				//The first events I know where window handle is already created are SourceInitialized and Loaded.
				//	They can be in any order.
				//	For example, if using SizeToContent, Loaded is before, and in both events the window is invisible.
				//	Else Loaded is after, and then the window is already visible.
				t.SourceInitialized += (_, _) => {
					var w = t.Hwnd();
					var v = AScreen.Of(x, y).Info;
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
							if(c->style!=0 && !c->style.Has(WS.CHILD)) { //note: this does not work if ShowInTaskbar = false. Then WPF creates a "Hidden Window" before, even if owner specified.
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
		/// - Sets location etc later, in <b>SourceInitialized</b> event.
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
		/// - Sets location etc later, in <b>SourceInitialized</b> event.
		/// - All this happens while the window is still invisible.
		/// </remarks>
		public static void SetRect(this Window t, RECT r) => _Move(t, 0, 0, r, true);

		/// <summary>
		/// Inserts row and adjusts row indices of children that are in other rows.
		/// </summary>
		public static void InsertRow(this Grid t, int index, RowDefinition d) {
			_GridShift(t, true, index, 1);
			t.RowDefinitions.Insert(index, d);
		}

		/// <summary>
		/// Inserts column and adjusts column indices of children that are in other columns.
		/// </summary>
		public static void InsertColumn(this Grid t, int index, ColumnDefinition d) {
			_GridShift(t, false, index, 1);
			t.ColumnDefinitions.Insert(index, d);
		}

		/// <summary>
		/// Removes row and adjusts row indices of children that are in other rows.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="index"></param>
		/// <param name="removeChildren">Remove children that are in that row.</param>
		public static void RemoveRow(this Grid t, int index, bool removeChildren) {
			if (removeChildren) _GridRemoveRowColChildren(t, true, index);
			t.RowDefinitions.RemoveAt(index);
			_GridShift(t, true, index, -1);
		}

		/// <summary>
		/// Removes column and adjusts column indices of children that are in other columns.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="index"></param>
		/// <param name="removeChildren">Remove children that are in that column.</param>
		public static void RemoveColumn(this Grid t, int index, bool removeChildren) {
			if (removeChildren) _GridRemoveRowColChildren(t, false, index);
			t.ColumnDefinitions.RemoveAt(index);
			_GridShift(t, false, index, -1);
		}

		/// <summary>
		/// Removes a child element and its row from this grid. Adjusts row indices of children that are in other rows.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="e"></param>
		/// <param name="removeOtherElements">Also remove other elements that are in that row.</param>
		public static void RemoveRow(this Grid t, UIElement e, bool removeOtherElements) {
			int i = Grid.GetRow(e);
			_GridRemoveChild(t, e);
			RemoveRow(t, i, removeOtherElements);
		}

		/// <summary>
		/// Removes a child element and its column from this grid. Adjusts column indices of children that are in other columns.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="e"></param>
		/// <param name="removeOtherElements">Also remove other elements that are in that column.</param>
		public static void RemoveColumn(this Grid t, UIElement e, bool removeOtherElements) {
			int i = Grid.GetColumn(e);
			_GridRemoveChild(t, e);
			RemoveColumn(t, i, removeOtherElements);
		}

		static void _GridShift(Grid g, bool rows, int startIndex, int shift) {
			if (startIndex >= (rows ? g.RowDefinitions.Count : g.ColumnDefinitions.Count)) return;
			foreach (UIElement e in g.Children) {
				int k = rows ? Grid.GetRow(e) : Grid.GetColumn(e);
				if (k < startIndex) continue;
				k += shift;
				if (rows) Grid.SetRow(e, k); else Grid.SetColumn(e, k);
			}
		}

		static void _GridRemoveRowColChildren(Grid g, bool row, int index) {
			var cc = g.Children;
			for (int i = cc.Count; --i >= 0;) {
				var e = cc[i];
				int rc = row ? Grid.GetRow(e) : Grid.GetColumn(e);
				if (rc == index) _GridRemoveChild(g, e);
			}
		}

		static void _GridRemoveChild(Grid g, UIElement e) {
			g.Children.Remove(e);
			Grid.SetRow(e, 0);
			Grid.SetColumn(e, 0);
		}

		/// <summary>
		/// Adds a child element in specified row/column.
		/// </summary>
		public static void AddChild(this Grid g, UIElement e, int row, int column) {
			Grid.SetRow(e, row);
			Grid.SetColumn(e, column);
			g.Children.Add(e);
		}
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
