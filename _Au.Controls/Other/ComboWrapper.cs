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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
using System.Windows.Forms.VisualStyles;

using Au;
using Au.Types;

namespace Au.Controls
{

	public unsafe class ComboWrapper
	{
		Native.SUBCLASSPROC _wndProc; //keeps delegate from GC; also for RemoveWindowSubclass
		EventHandler _handleCreated; //keeps this instance from GC
		Control _c;
		AWnd _hwnd;
		int _buttonWidth; //width of buttons (arrow+image)
		RECT _border; //border widths in all sides
		Bitmap _buttonImage; //if not null, adds image button
		bool _noArrow; //no arrow button
		byte _isPressed, //flags: 1 - arrow pressed; 2 - image pressed
			_isHot; //flags: 1 - arrow hot; 2 - image hot

		//protected override void Dispose(bool disposing)
		//{
		//	if(disposing) {
		//		//if(_buttonImage!=null) { _buttonImage.Dispose(); _buttonImage = null; } //no, allow to share it
		//	}
		//}
		//~ComboWrapper()
		//{
		//	AOutput.Write("~ComboWrapper");
		//	//Dispose(false);
		//}

		public ComboWrapper(Control c)
		{
			_c = c;
			_c.HandleCreated += _handleCreated = (unu, sed) => _Subclass();
			if(_c.IsHandleCreated) _Subclass();
		}

		void _Subclass()
		{
			_wndProc = _WndProc;

			_hwnd = (AWnd)_c;
			//AOutput.Write(_hwnd);
			Api.SetWindowSubclass(_hwnd, _wndProc, 40159885);
			_Redraw(true); //need WM_NCCALCSIZE etc
		}

		LPARAM _WndProc(AWnd w, int msg, LPARAM wParam, LPARAM lParam, LPARAM uIdSubclass, IntPtr dwRefData)
		{
			//AWnd.More.PrintMsg(w, msg, wParam, lParam);

			switch(msg) {
			case Api.WM_NCCALCSIZE: _OnNcCalcSize(w, msg, wParam, lParam); return 0; //adds nonclient area for our buttons
			case Api.WM_NCPAINT: if(_Paint(w, msg, wParam, lParam)) return 0; break;
			case Api.WM_THEMECHANGED: _Redraw(false); break;
			case Api.WM_NCLBUTTONDOWN: if(_OnNcLbuttonDown(lParam)) return 0; break;
			case Api.WM_NCMOUSEMOVE: if(_OnNcMouseMove(false, lParam)) return 0; break;
			case Api.WM_NCMOUSELEAVE: _OnNcMouseMove(true); break;
			case Api.WM_NCHITTEST: if(0 != _IsCursorInButton(lParam)) return 5; break; //HTMENU
			case Api.WM_MBUTTONDOWN: _OnMbuttonDown(); break; //clear with Undo
			case Api.WM_SYSKEYDOWN: if(_OnSysKeyDown(wParam)) return 0; break;
			}

			var R = Api.DefSubclassProc(w, msg, wParam, lParam);

			if(msg == Api.WM_NCDESTROY) {
				//AOutput.Write("WM_NCDESTROY");
				Api.RemoveWindowSubclass(w, _wndProc, 40159885);
				if(!_c.RecreatingHandle) _c.HandleCreated -= _handleCreated; //allow GC-collect _c and this
			}

			return R;
		}

		//Adds nonclient space at the right, to draw our buttons.
		void _OnNcCalcSize(AWnd w, int msg, LPARAM wParam, LPARAM lParam)
		{
			ref RECT r = ref *(RECT*)lParam;
			RECT p = r;
			Api.DefSubclassProc(w, msg, wParam, lParam);
			_border.left = r.left - p.left; _border.top = r.top - p.top; _border.right = p.right - r.right; _border.bottom = p.bottom - r.bottom;
			if(_border.right > _border.left) _border.right = _border.left; //vert scrollbar is inside

			if(_noArrow) _buttonWidth = 0; else _buttonWidth = Util.ADpi.ScaleInt(16); //left border is on image's left
			if(_buttonImage != null) _buttonWidth += _buttonImage.Width + 3; //+ margins=2 + left border=1
			if(_buttonWidth > 0) {
				if(_border.right == 2 && VisualStyleRenderer.IsSupported) {
					_border.right = 1; r.right++;
					if(_border.top == 2) _border.top = 1;
					if(_border.bottom == 2) _border.bottom = 1;
				}

				r.right -= _buttonWidth;
			}

			//tested:
			//wm_nccalcsize is sent after wm_nccreate (wParam=0) and later (wParam=1) on each resizing, style change etc.
			//The base proc always just sets the first rect. Probably just calls defwindowproc. Returns 0.
			//If wParam is 0, coords are screen, else parent client.
			//When wParam is 1, getwindowrect gives old rect.
		}

		bool _Paint(AWnd w, int msg, LPARAM wParam, LPARAM lParam)
		{
			if(_buttonWidth == 0) return false;
			RECT rb, rw;
			_GetRects(&rb, &rw, 2);

			//Let the base Edit control draw its nonclient first.
			//Exclude our button region from the update region. Need it to avoid flickering etc.
			if(msg == Api.WM_NCPAINT) {
				IntPtr hrgn = wParam;
				int buttonRgnType, baseRgnType;
				if(hrgn == (IntPtr)1) buttonRgnType = baseRgnType = SIMPLEREGION; //paint whole window
				else { //wParam is region handle; are our buttons included?
					RECT r = rb; r.Offset(rw.left, rw.top); IntPtr hrButton = Api.CreateRectRgnIndirect(r); //create region for buttons
					buttonRgnType = Api.CombineRgn(hrButton, hrgn, hrButton, Api.RGN_AND);
					baseRgnType = Api.CombineRgn(hrgn, hrgn, hrButton, Api.RGN_DIFF);
					Api.DeleteObject(hrButton);
				}
				if(baseRgnType != NULLREGION) Api.DefSubclassProc(w, msg, wParam, lParam);
				//AOutput.Write(baseRgnType, buttonRgnType, hrgn);
				if(buttonRgnType == NULLREGION) return true; //our buttons excluded
			}

			using var dc = new Util.WindowDC_(Api.GetWindowDC(_hwnd), _hwnd);
			if(Api.IntersectClipRect(dc, rb.left, rb.top, rb.right, rb.bottom) == NULLREGION) return true;

			if(!_hwnd.IsEnabled()) { Api.FillRect(dc, rb, (IntPtr)(Api.COLOR_BTNFACE + 1)); return true; }

			int bWidth = _buttonImage == null ? 0 : _buttonImage.Width + 3;
			if(!_noArrow) {
				RECT r = rb; if(_buttonImage != null) r.right -= bWidth;
				_PaintButton(dc, false, r, _isHot == 1, _isPressed == 1);
			}
			if(_buttonImage != null) {
				rb.left = rb.right - bWidth;
				_PaintButton(dc, true, rb, _isHot == 2, _isPressed == 2);
			}

			return true;
		}

		void _Paint() => _Paint(default, default, default, default);

		const int NULLREGION = 1; //empty
		const int SIMPLEREGION = 2; //rect
									//0 error, 3 complex

		void _PaintButton(IntPtr dc, bool isButton, in RECT r, bool isHot, bool isPressed)
		{
			using var bg = BufferedGraphicsManager.Current.Allocate(dc, r);
			var g = bg.Graphics;
			//background
			ColorInt c = 0xFFFFFF; if(isPressed) c = 0xCCE4F7; else if(isHot) c = 0xE5F1FB;
			var brush = c == 0xFFFFFF ? Brushes.White : new SolidBrush((Color)c);
			g.FillRectangle(brush, r);
			if(c != 0xFFFFFF) brush.Dispose();
			//image or arrow
			if(isButton) {
				g.DrawImageUnscaled(_buttonImage, r.left + 2, (r.top + r.bottom + 1 - _buttonImage.Height) / 2);
			} else if(VisualStyleRenderer.IsSupported) {
				new VisualStyleRenderer("COMBOBOX", 7, 1).DrawBackground(g, r);
			} else {
				ControlPaint.DrawComboButton(g, r, ButtonState.Flat);
			}
			//border
			if(isHot || isPressed) {
				using var pen = new Pen((Color)(c = 0x0078d7));
				int x = r.left;
				g.DrawLine(pen, x, r.top, x, r.bottom);
				if(!isButton && _buttonImage != null) {
					x = r.right - 1;
					g.DrawLine(pen, x, r.top, x, r.bottom);
				}
			}
			bg.Render();
			//never mind: should get theme colors. Now using Win10 ComboBox colors.
		}

		//rButton - receives buttons rect in window or screen.
		//rWindow - receives window rect in window (left and top are 0) or screen.
		//flags: 1 buttons in screen, 2 window in screen.
		void _GetRects(RECT* rButton = null, RECT* rWindow = null, int flags = 0)
		{
			RECT rw = _hwnd.Rect, rs = rw, r;
			rw.Offset(-rw.left, -rw.top);
			if(rButton != null) {
				r = 0 != (flags & 1) ? rs : rw;
				*rButton = new RECT(r.right - _border.right - _buttonWidth, r.top + _border.top, r.right - _border.right, r.bottom - _border.bottom, false);
			}
			if(rWindow != null) *rWindow = 0 != (flags & 2) ? rs : rw;
		}

		//If ncChanged, calls SetWindowPos(SWP_FRAMECHANGED), so that wm_nccalcsize would be sent. Else calls RedrawWindow.
		void _Redraw(bool ncChanged)
		{
			if(ncChanged) Api.SetWindowPos(_hwnd, default, 0, 0, 0, 0, Native.SWP.NOMOVE | Native.SWP.NOSIZE | Native.SWP.NOZORDER | Native.SWP.NOACTIVATE | Native.SWP.FRAMECHANGED);
			else Api.RedrawWindow(_hwnd, flags: Api.RDW_FRAME | Api.RDW_INVALIDATE);
		}

		//Returns: 0 not, 1 in arrow, 2 in image.
		byte _IsCursorInButton(LPARAM xyScreen)
		{
			if(_buttonWidth == 0) return 0;
			POINT p = (AMath.LoShort(xyScreen), AMath.HiShort(xyScreen));
			RECT rb; _GetRects(&rb, null, 1);
			if(!rb.Contains(p)) return 0;
			if(_buttonImage != null) { rb.right -= _buttonImage.Width + 3; if(!rb.Contains(p)) return 2; }
			return 1;
		}

		bool _OnNcMouseMove(bool onLeave, LPARAM xyScreen = default)
		{
			if(_buttonWidth == 0) return false;
			byte prevHot = _isHot;
			if(onLeave) {
				if(_isHot == 0) return false;
				_isHot = 0;
			} else {
				_isHot = _IsCursorInButton(xyScreen);
				if(_isHot != 0 && prevHot == 0) { //need WM_NCMOUSELEAVE
					var tm = new Api.TRACKMOUSEEVENT(_hwnd, Api.TME_LEAVE | Api.TME_NONCLIENT);
					Api.TrackMouseEvent(ref tm);
				}
			}
			if(_isHot != prevHot) _Paint();
			return _isHot != 0;
			//info: disabled windows don't receive mouse messages, it's good
		}

		bool _OnNcLbuttonDown(LPARAM xyScreen)
		{
			byte bc = _IsCursorInButton(xyScreen);
			if(bc == 0) return false;
			Api.SetFocus(_hwnd);
			if(_isPressed == 0) {
				_isPressed = bc;
				_Paint();
				if(bc == 1) { //arrow
					ArrowButtonPressed?.Invoke(this, EventArgs.Empty); //the client eg may show a modal popup menu
					_isPressed = 0;
					_Paint();
				} else {
					bool dropped = Util.ADragDrop.SimpleDragDrop(_hwnd);
					_isPressed = 0;
					_Paint();
					if(dropped) {
						POINT p = AMouse.XY;
						if(2 == _IsCursorInButton(AMath.MakeUint(p.x, p.y)))
							ImageButtonClicked?.Invoke(this, EventArgs.Empty);
					}
				}
			}
			return true;
		}

		void _OnMbuttonDown()
		{
			switch(_c) {
			case AuScintilla c:
				if(!c.Z.IsReadonly) c.Z.ClearText();
				break;
			case TextBox c:
				if(!c.ReadOnly) { c.SelectAll(); c.Paste(""); }
				break;
			}
		}

		//Returns true if processes the key (arrows etc).
		bool _OnSysKeyDown(LPARAM wParam)
		{
			KKey vk = (KKey)(int)wParam;
			if(_isPressed == 0) {
				switch(vk) {
				case KKey.Down: ArrowButtonPressed?.Invoke(this, EventArgs.Empty); return true;
				case KKey.Right: ImageButtonClicked?.Invoke(this, EventArgs.Empty); return true;
				}
			}
			return false;
		}

		[DefaultValue(false)]
		public bool NoArrow {
			get => _noArrow;
			set {
				if(value != _noArrow) {
					_noArrow = value;
					if(_c.IsHandleCreated) _Redraw(true);
				}
			}
		}

		[DefaultValue(null)]
		public Bitmap ButtonImage {
			get => _buttonImage;
			set {
				if(value != _buttonImage) {
					bool ncChanged = (value == null) != (_buttonImage == null);
					_buttonImage = value;
					if(_c.IsHandleCreated) _Redraw(ncChanged);
				}
			}
		}

		public event EventHandler ArrowButtonPressed;

		public event EventHandler ImageButtonClicked;
	}
}
