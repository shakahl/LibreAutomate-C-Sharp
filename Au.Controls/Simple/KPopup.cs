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
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Au.Util;
//using System.Linq;

namespace Au.Controls
{
	public class KPopup
	{
		HwndSource _hs;
		AWnd _w;
		bool _inSizeMove;

		public AWnd Hwnd => _w;

		public HwndSource HwndSource => _hs;

		/// <summary>
		/// Gets or sets <see cref="HwndSource.RootVisual"/>.
		/// </summary>
		public Visual Content { get => _hs.RootVisual; set { _hs.RootVisual = value; } }

		public KPopup(WS style = WS.POPUP | WS.THICKFRAME, WS2 exStyle = WS2.TOOLWINDOW | WS2.NOACTIVATE, bool shadow = false) {
			var p = new HwndSourceParameters {
				WindowStyle = (int)style,
				ExtendedWindowStyle = (int)exStyle,
				WindowClassStyle = shadow && !style.Has(WS.THICKFRAME) ? (int)Api.CS_DROPSHADOW : 0,
				HwndSourceHook = _Hook
			};
			_hs = new HwndSource(p) { SizeToContent = default };
		}

		unsafe nint _Hook(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
			AWnd.More.PrintMsg((AWnd)hwnd, msg, wParam, lParam);
			//if (msg == Api.WM_ACTIVATE && wParam != 0) AOutput.Write("ACTIVATE");

			switch (msg) {
			case Api.WM_NCCREATE:
				_w = (AWnd)hwnd;
				break;
			case Api.WM_NCDESTROY:
				Destroyed?.Invoke(this, EventArgs.Empty);
				_w = default;
				_hs = null;
				break;
			case Api.WM_WINDOWPOSCHANGED:
				var wp = (Api.WINDOWPOS*)lParam;
				//AOutput.Write(wp->flags & Native.SWP._KNOWNFLAGS, _w.IsVisible);
				if (wp->flags.Has(Native.SWP.HIDEWINDOW)) Hidden?.Invoke(this, EventArgs.Empty);
				if (!wp->flags.Has(Native.SWP.NOSIZE) && _inSizeMove) _size = (SIZE)ADpi.Unscale((wp->cx, wp->cy), _w);
				break;
			case Api.WM_ENTERSIZEMOVE:
				_inSizeMove = true;
				break;
			case Api.WM_EXITSIZEMOVE:
				_inSizeMove = false;
				break;
			case Api.WM_MOUSEACTIVATE:
				//OS ignores WS_EX_NOACTIVATE if the active window is of this thread. Workaround: on WM_MOUSEACTIVATE return MA_NOACTIVATE.
				handled = true;
				switch (AMath.HiShort(lParam)) {
				case Api.WM_MBUTTONDOWN:
					Hide(); //never mind: we probably don't receive this message if our thread is inactive
					return Api.MA_NOACTIVATEANDEAT;
				}
				return Api.MA_NOACTIVATE;
			case Api.WM_NCLBUTTONDOWN:
				//OS activates when clicked in non-client area, eg when moving or resizing. Workaround: on WM_NCLBUTTONDOWN suppress activation with a CBT hook.
				//When moving or resizing, WM_NCLBUTTONDOWN returns when moving/resizing ends. On resizing would activate on mouse button up.
				var wa = AWnd.ThisThread.Active;
				if (wa != default && wa != _w) {
					handled = true;
					using (AHookWin.ThreadCbt(d => d.code == HookData.CbtEvent.ACTIVATE && d.Hwnd == _w))
						Api.DefWindowProc(_w, msg, wParam, lParam);
				}
				break;
			case Api.WM_DPICHANGED:
				_hs.DpiChangedWorkaround();
				break;
			}
			return 0;
		}

		/// <summary>
		/// Desired window size. WPF logical pixels.
		/// Actual size can be smaller if would not fit in screen.
		/// </summary>
		public SIZE Size {
			get => _size;
			set {
				_size = value;
				if (_w.IsVisible) {
					var z = ADpi.Scale(_size, _w);
					_w.ResizeLL(z.width, z.height);
				}
			}
		}
		SIZE _size;

		public bool ExactSize { get; set; }

		public bool ExactSide { get; set; }

		public void ShowByRect(FrameworkElement owner, Dock side, RECT? rScreen = null) {
			RECT r = rScreen ?? owner?.RectInScreen() ?? throw new ArgumentException("owner and rScreen are null");
			var ow = owner?.Hwnd().Window ?? default;
			_ShowByRect(ow, r, side);
		}

		public void ShowByRect(KPopup owner, Dock side, RECT? rScreen = null) {
			RECT r = rScreen ?? owner?._w.Rect ?? throw new ArgumentException("owner and rScreen are null");
			var ow = owner?._w ?? default;
			_ShowByRect(ow, r, side);
		}

		void _ShowByRect(AWnd owner, RECT r, Dock side) {
			//could use API CalculatePopupWindowPosition instead of this quite big code, but it is not exactly what need here.

			var screen = AScreen.Of(r);
			var rs = screen.WorkArea;
			int dpi = screen.Dpi;
			var size = ADpi.Scale(Size, dpi);
			size.height = Math.Min(size.height, rs.Height); size.width = Math.Min(size.width, rs.Width);

			int spaceT = r.top - rs.top, spaceB = rs.bottom - r.bottom, spaceL = r.left - rs.left, spaceR = rs.right - r.right;
			if (!ExactSide) {
				switch (side) {
				case Dock.Left:
					if (size.width > spaceL && spaceR > spaceL) side = Dock.Right;
					break;
				case Dock.Right:
					if (size.width > spaceR && spaceL > spaceR) side = Dock.Left;
					break;
				case Dock.Top:
					if (size.height > spaceT && spaceB > spaceT) side = Dock.Bottom;
					break;
				default:
					if (size.height > spaceB && spaceT > spaceB) side = Dock.Top;
					break;
				}
			}
			if (!ExactSize) {
				switch (side) {
				case Dock.Left:
					if (size.width > spaceL) size.width = Math.Max(spaceL, size.width / 2);
					break;
				case Dock.Right:
					if (size.width > spaceR) size.width = Math.Max(spaceR, size.width / 2);
					break;
				case Dock.Top:
					if (size.height > spaceT) size.height = Math.Max(spaceT, size.height / 2);
					break;
				default:
					if (size.height > spaceB) size.height = Math.Max(spaceB, size.height / 2);
					break;
				}
			}
			RECT m = default;
			switch (side) {
			case Dock.Left:
				m.left = Math.Max(r.left - size.width, rs.left); m.top = r.top;
				break;
			case Dock.Right:
				m.left = Math.Min(r.right, rs.right - size.width); m.top = r.top;
				break;
			case Dock.Top:
				m.left = r.left; m.top = Math.Max(r.top - size.height, rs.top);
				break;
			default:
				m.left = r.left; m.top = Math.Min(r.bottom, rs.bottom - size.height);
				break;
			}
			m.Width = size.width; m.Height = size.height;

			_inSizeMove = false;
			_w.MoveLL(m);
			if (_w.OwnerWindow != owner) _w.OwnerWindow = owner;
			AOutput.Write(_w.OwnerWindow, _w.ZorderIsAbove(_w.OwnerWindow));
			if (IsVisible) return;
			_w.ShowLL(true);
		}

		public void Hide(bool destroy = false) {
			if (destroy) _hs.Dispose();
			else _w.ShowLL(false);
		}

		public bool IsVisible => _w.IsVisible;

		public event EventHandler Hidden;

		public event EventHandler Destroyed;
	}
}
