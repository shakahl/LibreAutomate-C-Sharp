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
	public unsafe partial class AuTreeView
	{
		/// <summary>
		/// Gets native control handle.
		/// </summary>
		public AWnd Wnd => _hh?.Hwnd ?? default;

		partial class _HwndHost : HwndHost
		{
			const string c_winClassName = "AuTreeView";
			readonly AuTreeView _tv;
			AWnd _w;

			public AWnd Hwnd => _w;

			static _HwndHost() {
				AWnd.More.RegisterWindowClass(c_winClassName);
			}

			public _HwndHost(AuTreeView tv) {
				_tv = tv;
			}

			protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
				var wParent = (AWnd)hwndParent.Handle;
#if true
				AWnd.More.CreateWindow(_WndProc, c_winClassName, null, WS.CHILD | WS.CLIPCHILDREN, 0, 0, 0, 10, 10, wParent);
#else //the below code works, but not on Win7. The above code works if WS.DISABLED or WM_NCHITTEST returns HTTRANSPARENT. WPF can remove WS.DISABLED.
			_w=AWnd.More.CreateWindow(_WndProc, c_winClassName, null, WS.CHILD|WS.CLIPCHILDREN, WS2.TRANSPARENT|WS2.LAYERED, 0, 0, 10, 10, wParent);
	//		_w.SetTransparency(true, 255);
			Api.SetLayeredWindowAttributes(_w, 0, 0, 0);
#endif

				return new HandleRef(this, _w.Handle);
			}

			protected override void DestroyWindowCore(HandleRef hwnd) {
				Api.DestroyWindow(_w);
			}

			LPARAM _WndProc(AWnd w, int msg, LPARAM wParam, LPARAM lParam) {
				//var pmo = new PrintMsgOptions(Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_NCMOUSEMOVE, 0x10c1);
				//if (AWnd.More.PrintMsg(out string s, _w, msg, wParam, lParam, pmo)) AOutput.Write("<><c green>" + s + "<>");

				switch (msg) {
				case Api.WM_NCCREATE:
					_w = w;
					BufferedPaint.Init();
					break;
				case Api.WM_NCDESTROY:
					_w = default;
					BufferedPaint.Uninit();
					break;
				case Api.WM_NCHITTEST:
					return Api.HTTRANSPARENT; //workaround for focus problems and closing parent Popup on click
				case Api.WM_PAINT:
					//never mind: should draw only the invalidated rect (ps.rcPaint). It saves ~1% CPU.
					//without buffered flickers and slower. Tested .NET buffered paint, similar speed and memory.
					using (var bp = new BufferedPaint(w, true)) _tv._Render(bp.DC, bp.Rect);
					return default;
				case Api.WM_SHOWWINDOW when wParam == 1:
					if (_tv._ensureVisibleIndex > 0) _tv.EnsureVisible(_tv._ensureVisibleIndex);
					break;
				}

				return Api.DefWindowProc(w, msg, wParam, lParam);
			}

			protected override nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
				if (msg == Api.WM_GETOBJECT) { //not in _WndProc, because WPF steals it if passed to base.WndProc
					handled = true;
					return (_acc ??= new _Accessible(_tv)).WmGetobject(wParam, lParam);
				}
				return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
			}

			//protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() => null; //removes unused object from MSAA tree, but then no UIA
			_Accessible _acc;

			protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
				//AOutput.Write(e.Property);
				if (_tv?._lePopup != null && e.Property.Name == "IsVisible" && e.NewValue is bool y && !y) _tv.EndEditLabel(true);
				base.OnPropertyChanged(e);
			}

			protected override void OnRenderSizeChanged(SizeChangedInfo e) {
				_tv._HhSizeChanged(e);
				base.OnRenderSizeChanged(e);
			}
		}

		void _HhSizeChanged(SizeChangedInfo e) {
			_width = (int)Math.Ceiling(e.NewSize.Width * _dpi / 96);
			_height = (int)Math.Ceiling(e.NewSize.Height * _dpi / 96);
		}
	}
}
