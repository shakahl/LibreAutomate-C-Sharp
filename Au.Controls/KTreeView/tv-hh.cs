using Au.Types;
using Au.Util;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Diagnostics;

namespace Au.Controls
{
	public unsafe partial class KTreeView : HwndHost
	{
		const string c_winClassName = "KTreeView";
		AWnd _w;
		bool _hasHwnd;

		public AWnd Hwnd => _w;

		static KTreeView() {
			AWnd.More.RegisterWindowClass(c_winClassName);
		}

		protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
			var wParent = (AWnd)hwndParent.Handle;
#if true
			AWnd.More.CreateWindow(_wndProc = _WndProc, false, c_winClassName, null, WS.CHILD | WS.CLIPCHILDREN, 0, 0, 0, 10, 10, wParent);
#else //the below code works, but not on Win7. The above code works if WS.DISABLED or WM_NCHITTEST returns HTTRANSPARENT. WPF can remove WS.DISABLED.
			_w=AWnd.More.CreateWindow(_wndProc = _WndProc, false, c_winClassName, null, WS.CHILD|WS.CLIPCHILDREN, WS2.TRANSPARENT|WS2.LAYERED, 0, 0, 10, 10, wParent);
	//		_w.SetTransparency(true, 255);
			Api.SetLayeredWindowAttributes(_w, 0, 0, 0);
#endif

			return new HandleRef(this, _w.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd) {
			Api.DestroyWindow(_w);
		}

		Native.WNDPROC _wndProc;
		LPARAM _WndProc(AWnd w, int msg, LPARAM wParam, LPARAM lParam) {
			//var pmo = new PrintMsgOptions(Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_NCMOUSEMOVE, 0x10c1);
			//if (AWnd.More.PrintMsg(out string s, _w, msg, wParam, lParam, pmo)) AOutput.Write("<><c green>" + s + "<>");

			if (_vscroll.WndProc(w, msg, wParam, lParam) || _hscroll.WndProc(w, msg, wParam, lParam)) return default;

			switch (msg) {
			case Api.WM_NCCREATE:
				_w = w;
				_hasHwnd = true;
				_SetDpiAndItemSize(ADpi.OfWindow(_w));
				ABufferedPaint.Init();
				break;
			case Api.WM_NCDESTROY:
				_w = default;
				_hasHwnd = false;
				_acc?.Dispose(); _acc = null;
				ABufferedPaint.Uninit();
				break;
			case Api.WM_PAINT:
				using (var bp = new ABufferedPaint(w, true)) _Render(bp.DC, bp.UpdateRect);
				return default;
			case Api.WM_SHOWWINDOW when wParam == 1:
				if (_ensureVisibleIndex > 0) EnsureVisible(_ensureVisibleIndex);
				break;
			case Api.WM_SIZE:
				_width = AMath.LoWord(lParam);
				_height = AMath.HiWord(lParam);
				_Measure();
				break;
			case Api.WM_SYSCOMMAND when ((uint)wParam & 0xfff0) is Api.SC_VSCROLL or Api.SC_HSCROLL: //note: Windows bug: swapped SC_VSCROLL and SC_HSCROLL
				try {
					_inScrollbarScroll = true;
					return Api.DefWindowProc(w, msg, wParam, lParam);
				}
				finally {
					_ScrollEnded();
					_inScrollbarScroll = false;
				}
			}

			var R = Api.DefWindowProc(w, msg, wParam, lParam);

			switch (msg) {
			case Api.WM_NCHITTEST:
				if (R == Api.HTCLIENT) R = Api.HTTRANSPARENT; //workaround for focus problems and closing parent Popup on click
				break;
			}

			return R;
		}

		protected override nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
			if (msg == Api.WM_GETOBJECT) { //not in _WndProc, because WPF steals it if passed to base.WndProc
				handled = true;
				return (_acc ??= new _Accessible(this)).WmGetobject(wParam, lParam);
			}
			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

		//protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() => null; //removes unused object from MSAA tree, but then no UIA
		_Accessible _acc;

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			//AOutput.Write(e.Property);
			if (_lePopup != null && e.Property.Name == "IsVisible" && e.NewValue is bool y && !y) EndEditLabel(true);
			base.OnPropertyChanged(e);
		}
	}
}
