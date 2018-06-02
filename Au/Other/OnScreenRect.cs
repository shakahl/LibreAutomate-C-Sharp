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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Shows a transparent rectangle on screen.
	/// </summary>
	/// <remarks>
	/// Creates a temporary partially transparent window, and draws rectangle in it.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// using(var x = new OnScreenRect()) {
	/// 	x.Rect = new RECT(100, 100, 200, 200, true);
	/// 	x.Color = Color.SlateBlue;
	/// 	x.Thickness = 4;
	/// 	x.Show(true);
	/// 	for(int i = 0; i < 6; i++) {
	/// 		300.ms();
	/// 		x.Visible = !x.Visible;
	/// 	}
	/// }
	/// ]]></code>
	/// </example>
	public class OnScreenRect :IDisposable
	{
		static OnScreenRect()
		{
			Wnd.Misc.MyWindow.RegisterClass(c_className);
		}
		const string c_className = "Au.OSR";

		///
		public OnScreenRect()
		{
			_w = new WndClass(this);
		}
		WndClass _w;

		/// <summary>
		/// Destroys the rectangle window.
		/// </summary>
		public void Dispose()
		{
			if(!_IsWindow) return;
			_w.Destroy();
		}

		/// <summary>
		/// Gets the rectangle window.
		/// </summary>
		public Wnd Handle => _w.Handle;

		bool _IsWindow => !_w.Handle.Is0;

		void _Redraw()
		{
			if(!_IsWindow || !_visible) return;
			var w = _w.Handle;
			Api.InvalidateRect(w, default, false);
			Api.UpdateWindow(w);
		}

		/// <summary>
		/// Gets or sets rectangle color.
		/// </summary>
		/// <remarks>
		/// Don't use white. It is used to create transparent interior when Opacity is 0 (default).
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// x.Color = 0xFF0000; //red
		/// x.Color = Color.Orange;
		/// ]]></code>
		/// </example>
		public ColorInt Color
		{
			get => _color;
			set { if(value != _color) { _color = value; _Redraw(); } }
		}
		ColorInt _color;

		/// <summary>
		/// Gets or sets rectangle frame width.
		/// Used only if Opacity is 0 (default).
		/// </summary>
		public int Thickness
		{
			get => _thickness;
			set { if(value != _thickness) { _thickness = value; _Redraw(); } }
		}
		int _thickness = 3;

		/// <summary>
		/// Gets or sets the opacity of the rectangle, from 0 to 1.
		/// If 0 (default) draws opaque frame and completely transparent interior. Else draws filled rectangle.
		/// </summary>
		public double Opacity
		{
			get => _opacity;
			set
			{
				var v = Math.Min(Math.Max(value, 0.0), 1.0);
				if(v == _opacity) return;
				bool was0 = _opacity == 0;
				_opacity = v;
				if(!_IsWindow) return;
				_SetOpacity();
				if((v == 0) != was0) _Redraw();
			}
		}
		double _opacity;

		void _SetOpacity()
		{
			var w = _w.Handle;
			if(_opacity > 0) Api.SetLayeredWindowAttributes(w, 0, (byte)(uint)(_opacity * 255), 2);
			else Api.SetLayeredWindowAttributes(w, 0xffffff, 0, 1); //white is transparent
		}

		/// <summary>
		/// Gets or sets rectangle position in screen.
		/// If the rectangle window is already created, moves it.
		/// </summary>
		public RECT Rect
		{
			get => _r;
			set
			{
				if(value == _r) return;
				_r = value;
				if(_IsWindow) _w.Handle.SetWindowPos(Native.SWP_NOACTIVATE, _r.left, _r.top, _r.Width, _r.Height, Native.HWND_TOPMOST);
			}
		}
		RECT _r;

		/// <summary>
		/// Gets or sets whether the rectangle is visible.
		/// The 'set' function calls <see cref="Show"/>.
		/// </summary>
		public bool Visible
		{
			get => _visible;
			set => Show(value);
		}
		bool _visible;

		/// <summary>
		/// Shows or hides the rectangle.
		/// </summary>
		/// <param name="show">If true, creates the rectangle window or just makes visible. Else hides the window; does not destroy.</param>
		public void Show(bool show)
		{
			if(!_IsWindow) {
				if(!show) return;
				_CreateWindow();
			}
			var w = _w.Handle;
			w.ShowLL(show);
			if(show) {
				_w.Handle.ZorderTopmost();
				Api.UpdateWindow(w);
			}
		}

		void _CreateWindow()
		{
			var es = Native.WS_EX_TOOLWINDOW | Native.WS_EX_TOPMOST | Native.WS_EX_LAYERED | Native.WS_EX_TRANSPARENT | Native.WS_EX_NOACTIVATE;
			_w.Create(c_className, null, Native.WS_POPUP, es, _r.left, _r.top, _r.Width, _r.Height);
			_SetOpacity();
		}

		class WndClass :Wnd.Misc.MyWindow
		{
			OnScreenRect _osr;

			public WndClass(OnScreenRect osr)
			{
				_osr = osr;
			}

			public override LPARAM WndProc(Wnd w, uint message, LPARAM wParam, LPARAM lParam)
			{
				LPARAM R = 0;
				if(!_osr._WndProc_Before(w, message, wParam, lParam, ref R)) {
					R = base.WndProc(w, message, wParam, lParam);
				}
				return R;
			}
		}

		//returns true to not call base.WndProc
		bool _WndProc_Before(Wnd w, uint message, LPARAM wParam, LPARAM lParam, ref LPARAM R)
		{
			switch(message) {
			case Api.WM_SHOWWINDOW:
				_visible = wParam != 0;
				break;
			case Api.WM_PAINT:
				_OnPaint();
				return true;
			}

			return false;
		}

		void _OnPaint()
		{
			//Print("paint");

			var w = _w.Handle;
			var dc = Api.BeginPaint(w, out var ps);
			var r = w.ClientRect;
			bool isBlack = _color == 0;
			var brush = isBlack ? Api.GetStockObject(Api.BLACK_BRUSH) : Api.CreateSolidBrush((uint)_color.ToBGR());
			if(_opacity > 0) {
				Api.FillRect(dc, r, brush);
			} else {
				Api.FillRect(dc, r, Api.GetStockObject(Api.WHITE_BRUSH)); //transparent

				var hr = Api.CreateRectRgnIndirect(r);
				Api.FrameRgn(dc, hr, brush, _thickness, _thickness);
				Api.DeleteObject(hr);
			}
			if(!isBlack) Api.DeleteObject(brush);
			Api.EndPaint(w, ps);
		}
	}
}
