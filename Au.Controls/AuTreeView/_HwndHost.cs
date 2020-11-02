#define BUFFERED_API
//#define BUFFERED_NET //similar speed and memory
//without buffered flickers and slower

using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
using Au.Util;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Media;
using System.Diagnostics;


namespace Au.Controls {
public unsafe partial class AuTreeView {
	/// <summary>
	/// Gets native control handle.
	/// </summary>
	public AWnd Wnd => _hh.Wnd;
	
	partial class _HwndHost : HwndHost {
		const string c_winClassName="AuTreeView";
		readonly AuTreeView _tv;
		AWnd _w;
		
		public AWnd Wnd => _w;
		
		static _HwndHost() {
			AWnd.More.RegisterWindowClass(c_winClassName);
		}
		
		public _HwndHost(AuTreeView tv) {
			_tv=tv;
		}

		protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
			var wParent=(AWnd)hwndParent.Handle;
			#if true
			_w=AWnd.More.CreateWindow(_WndProc, c_winClassName, null, WS.CHILD|WS.CLIPCHILDREN, 0, 0, 0, 10, 10, wParent);
			#else //the below code works, but not on Win7. The above code works if WS.DISABLED or WM_NCHITTEST returns HTTRANSPARENT. WPF can remove WS.DISABLED.
			_w=AWnd.More.CreateWindow(_WndProc, c_winClassName, null, WS.CHILD|WS.CLIPCHILDREN, WS2.TRANSPARENT|WS2.LAYERED, 0, 0, 10, 10, wParent);
	//		_w.SetTransparency(true, 255);
			Api.SetLayeredWindowAttributes(_w, 0, 0, 0);
			#endif
			#if BUFFERED_API
			BufferedPaintInit(); //fast
			#endif
			
			return new HandleRef(this, _w.Handle);
		}

		protected override void DestroyWindowCore(HandleRef hwnd) {
			Api.DestroyWindow(_w);
		}

		LPARAM _WndProc(AWnd w, int msg, LPARAM wParam, LPARAM lParam) {
	//		var pmo=new PrintMsgOptions(Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_MOUSEMOVE, Api.WM_NCMOUSEMOVE, 0x10c1);
	//		if(AWnd.More.PrintMsg(out string s, _w, msg, wParam, lParam, pmo)) AOutput.Write("<><c green>"+s+"<>");
			
			switch(msg) {
			case Api.WM_NCDESTROY:
				_w=default;
				#if BUFFERED_API
				BufferedPaintUnInit();
				#endif
				break;
			case Api.WM_NCHITTEST:
				return Api.HTTRANSPARENT;
			case Api.WM_PAINT:
				var r = w.ClientRect; //never mind: should draw only the invalidated rect (ps.rcPaint). It saves ~1% CPU.
				Api.BeginPaint(w, out var ps);
				try {
				#if BUFFERED_NET
					using var bg = System.Drawing.BufferedGraphicsManager.Current.Allocate(ps.hdc, r);
					var g = bg.Graphics;
					var dc=g.GetHdc();
					try { _tv._Render(dc, r); }
					finally { g.ReleaseHdc(dc); }
					bg.Render();
				#elif BUFFERED_API
					BP_PAINTPARAMS pp=default; pp.cbSize=sizeof(BP_PAINTPARAMS);
					var hb=BeginBufferedPaint(ps.hdc, r, BP_BUFFERFORMAT.BPBF_TOPDOWNDIB, ref pp, out var hdc); //BPBF_COMPATIBLEBITMAP slower //tested: works with 16 and 8 bit colors too
					if(hb!=default) {
						try { _tv._Render(hdc, r); }
						finally { EndBufferedPaint(hb, true); }
					}
				#else
					_tv._Render(ps.hdc, r);
				#endif
				}
				finally { Api.EndPaint(w, ps); }
				return default;
			case Api.WM_GETOBJECT:
				return _WmGetobject(wParam, (AccOBJID)(uint)lParam);
			}
			
			var R = Api.DefWindowProc(w, msg, wParam, lParam);
			
			
			return R;
		}

		protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			if(msg==Api.WM_GETOBJECT) return default; //WPF steals it
			return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
		}

#if BUFFERED_API
	[DllImport("uxtheme.dll", PreserveSig=true)]
internal static extern int BufferedPaintInit();
[DllImport("uxtheme.dll", PreserveSig=true)]
internal static extern int BufferedPaintUnInit();
[DllImport("uxtheme.dll")]
internal static extern IntPtr BeginBufferedPaint(IntPtr hdcTarget, in RECT prcTarget, BP_BUFFERFORMAT dwFormat, ref BP_PAINTPARAMS pPaintParams, out IntPtr phdc);
[DllImport("uxtheme.dll", PreserveSig=true)]
internal static extern int EndBufferedPaint(IntPtr hBufferedPaint, bool fUpdateTarget);
internal enum BP_BUFFERFORMAT {
	BPBF_COMPATIBLEBITMAP,
	BPBF_DIB,
	BPBF_TOPDOWNDIB,
	BPBF_TOPDOWNMONODIB
}
internal struct BP_PAINTPARAMS {
	public int cbSize;
	public uint dwFlags;
	public RECT* prcExclude;
//	public BLENDFUNCTION* pBlendFunction;
	uint pBlendFunction;
}
//internal struct BLENDFUNCTION {
//	public byte BlendOp;
//	public byte BlendFlags;
//	public byte SourceConstantAlpha;
//	public byte AlphaFormat;
//}
#endif

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
//			AOutput.Write(e.Property);
			if(_tv?._lePopup!=null && e.Property.Name=="IsVisible" && e.NewValue is bool y && !y) _tv.EndEditLabel(true);
			base.OnPropertyChanged(e);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo e) {
			_tv._HhSizeChanged(e);
			base.OnRenderSizeChanged(e);
		}
	}

	void _HhSizeChanged(SizeChangedInfo e) {
		_width=(int)Math.Ceiling(e.NewSize.Width*_dpi/96);
		_height=(int)Math.Ceiling(e.NewSize.Height*_dpi/96);
	}
}
}
