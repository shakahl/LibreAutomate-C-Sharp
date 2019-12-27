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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	public partial class AToolbar
	{
		class _ToolStrip : Util.AToolStrip, _IAuToolStrip
		{
			AToolbar _tb;
			//AMenu _menu;

			//public AMenu ContextMenu => _menu;

			_Settings _sett => _tb._sett;

			public _ToolStrip(AToolbar tb)
			{
				_tb = tb;
				this.SuspendLayout();
				this.AutoSize = false;
				this.Stretch = false;
				this.Dock = DockStyle.None;
				this.LayoutStyle = ToolStripLayoutStyle.Flow;
				this.GripStyle = ToolStripGripStyle.Hidden;
				this.BackColor = SystemColors.ButtonFace;
				this.Renderer = new _Renderer();
				this.Padding = default;
			}

			public void Create()
			{
				this.SetTopLevel(true); //makes both true: Created, IsHandleCreated
				ResumeLayout();
			}

			protected override CreateParams CreateParams {
				get {
					var p = base.CreateParams;
					if(_tb != null) { //this prop is called 3 times, first time before ctor
						var es = WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE | WS_EX.TOPMOST;
						p.ExStyle = (int)es;

						var style = ((WS)p.Style & ~(WS.CHILD | WS.CLIPSIBLINGS | WS.VISIBLE)) | WS.POPUP | _BorderStyle(Border);
						p.Style |= (int)style;
					}
					return p;
				}
			}

			static WS _BorderStyle(TBBorder b) => b switch
			{
				TBBorder.None => 0,
				TBBorder.Fixed3D => WS.DLGFRAME,
				TBBorder.Sizable3D => WS.DLGFRAME,
				TBBorder.Sizable => WS.THICKFRAME,
				TBBorder.FixedWithCaption => WS.CAPTION,
				TBBorder.FixedWithCaptionX => WS.CAPTION | WS.SYSMENU,
				TBBorder.SizableWithCaption => WS.CAPTION | WS.THICKFRAME,
				TBBorder.SizableWithCaptionX => WS.CAPTION | WS.THICKFRAME | WS.SYSMENU,
				_ => WS.BORDER
			};

			public TBBorder Border {
				get => _border;
				set {
					if(value == _border) return;
#if true
					int _Pad(TBBorder b) => b switch { TBBorder.Fixed2 => 1, TBBorder.Sizable2 => 1, TBBorder.Fixed3 => 2, TBBorder.Sizable3 => 2, TBBorder.Fixed4 => 3, TBBorder.Sizable4 => 3, _ => 0 };
					int pOld = _Pad(_border), pNew = _Pad(value), pDif = pNew - pOld;
					if(pDif != 0) this.Padding = new Padding(pNew);
					if(Created) {
						var w = (AWnd)this;
						RECT r = w.ClientRectInScreen; r.Inflate(pDif, pDif); r.Normalize(false);
						const WS mask = WS.CAPTION | WS.THICKFRAME | WS.SYSMENU;
						WS s1 = w.Style, s2 = _BorderStyle(value);
						if(s2 != (s1 & mask)) w.SetStyle((s1 & ~mask) | s2);
						//preserve client size and position
						Api.AdjustWindowRectEx(ref r, _BorderStyle(value), false, WS_EX.TOOLWINDOW);
						w.MoveLL(r.left, r.top, r.Width, r.Height, Native.SWP.FRAMECHANGED);
					}
#else //this simpler code does not preserve client size and position
					int _Pad(TBBorder b) => b switch { TBBorder.Fixed2 => 1, TBBorder.Sizable2 => 1, TBBorder.Fixed3 => 2, TBBorder.Sizable3 => 2, _ => 0 };
					int pOld = _Pad(_border), pNew = _Pad(value);
					if(pNew != pOld) this.Padding = new Padding(pNew);
					if(Created) {
						const WS mask = WS.CAPTION | WS.THICKFRAME | WS.SYSMENU;
						var w = (AWnd)this;
						WS s1 = w.Style, s2 = _BorderStyle(value);
						if(s2 != (s1 & mask)) w.SetStyle((s1 & ~mask) | s2, updateNC: true, updateClient: true);
					}
#endif
					_border = value;
					if(_tb._constructed) _sett.border = value;
				}
			}
			TBBorder _border;

			protected override unsafe void WndProc(ref Message m)
			{
				//Print(m);
				switch(m.Msg) {
				case Api.WM_NCHITTEST:
					if(_WmNchittest(ref m)) return; //returns a hittest code to resize window if need
					break;
				case Api.WM_LBUTTONDOWN when ModifierKeys == Keys.Shift:
					_DragMoveWindow();
					return;
				case Api.WM_CONTEXTMENU:
					_ContextMenu();
					break;
				case Api.WM_DESTROY:
					_WmDestroy();
					break;
				}

				base.WndProc(ref m);

				switch(m.Msg) {
				case Api.WM_EXITSIZEMOVE:
					_sett.bounds = Bounds;
					break;
				case Api.WM_PAINT:
					_paintedOnce = true;
					//APerf.NW();
					break;
				case Api.WM_NCPAINT:
					if(m.WParam == (IntPtr)1) _WmNcpaint((AWnd)m.HWnd);
					break;
				}
			}

			void _WmDestroy()
			{
				_sett.bounds = Bounds;
				_sett.Dispose();
			}

			void _ContextMenu()
			{
				if(this.ContextMenuStrip == null) {
					var m = new AMenu { MultiShow = true };
					m["Edit"] = o => _tb.GoToEdit_(_contextItem);
					m["Close"] = o => _tb.Close();

					this.ContextMenuStrip = m.Control;
				}
				this.ContextMenuStrip.Items[0].Visible = !ATask.WndMsg.Is0;
				_contextItem = GetItemAt(this.MouseClientXY());
			}
			ToolStripItem _contextItem;

			void _DragMoveWindow()
			{
				var p = AMouse.XY;
				var w = (AWnd)this;
				if(Api.DragDetect(w, p)) {
					var pp = AMouse.XY;
					Util.ADragDrop.SimpleDragDrop(w, MButtons.Left, o => {
						if(o.Msg.message != Api.WM_MOUSEMOVE) return;
						p = AMouse.XY; if(p == pp) return;
						var r = w.Rect;
						w.MoveLL(r.left + (p.x - pp.x), r.top + (p.y - pp.y));
						pp = p;
					});
					_sett.bounds = Bounds;
				}
			}

			bool _WmNchittest(ref Message m)
			{
				int b;
				switch(Border) {
				case TBBorder.Sizable1: b = 1; break;
				case TBBorder.Sizable2: b = 2; break;
				case TBBorder.Sizable3: case TBBorder.Sizable3D: b = 3; break;
				case TBBorder.Sizable4: b = 4; break;
				default: return false;
				}
				//if(ModifierKeys != Keys.Shift) return false;
				LPARAM xy = m.LParam;
				int h, x = AMath.LoShort(xy), y = AMath.HiShort(xy);
				var r = ((AWnd)this).Rect;
				int bx = Math.Min(b, r.Width / 2), by = Math.Min(b, r.Height / 2);
				int x1 = r.left + bx, x2 = r.right - bx - 1, y1 = r.top + by, y2 = r.bottom - by - 1;
				if(x < x1) {
					h = y < y1 ? Api.HTTOPLEFT : (y > y2 ? Api.HTBOTTOMLEFT : Api.HTLEFT);
				} else if(x > x2) {
					h = y < y1 ? Api.HTTOPRIGHT : (y > y2 ? Api.HTBOTTOMRIGHT : Api.HTRIGHT);
				} else if(y < y1) {
					h = Api.HTTOP;
				} else if(y > y2) {
					h = Api.HTBOTTOM;
				} else return false;
				m.Result = (IntPtr)h;
				return true;
			}

			void _WmNcpaint(AWnd w)
			{
				if(_borderColor == 0 || _border < TBBorder.Fixed1 || _border > TBBorder.Sizable4) return;
				using var dc = new Util.LibWindowDC(Api.GetWindowDC(w), w);
				using var g = Graphics.FromHdc(dc);
				var r = w.Rect; r.Offset(-r.left, -r.top);
				ControlPaint.DrawBorder(g, r, (Color)_borderColor, ButtonBorderStyle.Solid);
			}

			public ColorInt BorderColor {
				get => _borderColor;
				set {
					if(value == _borderColor) return;
					_borderColor = value;
					if(_tb._constructed) _sett.borderColor = value;
					if(Created) unsafe { Api.RedrawWindow((AWnd)this, flags: Api.RDW_FRAME | Api.RDW_INVALIDATE); }
				}
			}
			ColorInt _borderColor;

			//protected override void OnSizeChanged(EventArgs e)
			//{
			//	base.OnSizeChanged(e);
			//}

			bool _IAuToolStrip.PaintedOnce => _paintedOnce;
			bool _paintedOnce;

			//ToolStrip _IAuToolStrip.ToolStrip => this;
		}

		class _Renderer : ToolStripProfessionalRenderer
		{
			public _Renderer()
			{
				RoundedEdges = false;
			}

			protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
			{
				if(!e.Item.AutoSize) {
					var c = e.ToolStrip;
					var k = e.Item as ToolStripSeparator;
					var g = e.Graphics;
					int y = k.Height; y = y > 3 ? y / 2 : 0;
					int width = c.ClientSize.Width;
					g.DrawLine(SystemPens.ControlDark, 2, y, width - c.Padding.Horizontal - 3, y);
					var s = k.Name;
					if(!Empty(s)) {
						var f = TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix;
						var r = new Rectangle(0, 1, width, k.Height);
						TextRenderer.DrawText(g, " " + s + " ", c.Font, r, SystemColors.GrayText, c.BackColor, f);
					}
				} else {
					base.OnRenderSeparator(e);
				}
			}
		}
	}
}
