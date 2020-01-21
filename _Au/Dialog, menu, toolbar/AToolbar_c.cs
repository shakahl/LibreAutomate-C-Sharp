//#define SYSTEM_RENDERER //good: button highlighting looks like of most toolbars. bad: alpha-blends button highlighting, almost invisible if dark background; highligted buttons are different than drop-down menu items.

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
using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	public partial class AToolbar
	{
		class _ToolStrip : Util.AToolStrip, _IAuToolStrip
		{
			AToolbar _tb;
			[ThreadStatic] static TBRenderer t_renderer;

			_Settings _sett => _tb._sett;

			internal _ToolStrip(AToolbar tb)
			{
				_tb = tb;

				//like in ctors of Form and ToolStripDropDown
				this.Visible = false; //else SetTopLevel(true) creates handle and tries to activate
				this.SetTopLevel(true);

				this.SuspendLayout();
				this.AutoSize = false;
				this.Stretch = false;
				this.Dock = DockStyle.None;
				this.LayoutStyle = ToolStripLayoutStyle.Flow;
				this.GripStyle = ToolStripGripStyle.Hidden;
				this.Padding = default;
				//this.BackColor = SystemColors.ButtonFace; //default
				this.Renderer = t_renderer ??= new TBRenderer();
			}

			protected override CreateParams CreateParams {
				get {
					var p = base.CreateParams;
					if(_tb != null) { //this prop is called 3 times, first time before ctor
						var es = WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE;
						if(_tb._topmost) es |= WS_EX.TOPMOST;
						if(_tb._transparency != default) es |= WS_EX.LAYERED;
						p.ExStyle = (int)es;

						var style = ((WS)p.Style & ~(WS.CHILD | WS.CLIPSIBLINGS | WS.VISIBLE)) | WS.POPUP | _BorderStyle(_sett.border);
						p.Style |= (int)style;
					}
					return p;
				}
			}

			WS _BorderStyle(TBBorder b) => b switch
			{
				TBBorder.None => 0,
				TBBorder.ThreeD => WS.DLGFRAME,
				TBBorder.Thick => WS.THICKFRAME,
				TBBorder.Caption => WS.CAPTION | WS.THICKFRAME,
				TBBorder.CaptionX => WS.CAPTION | WS.THICKFRAME | WS.SYSMENU,
				_ => WS.BORDER
			};

			internal void SetBorder(TBBorder value)
			{
				int _Pad(TBBorder b) => b >= TBBorder.Width2 && b <= TBBorder.Width4 ? (int)b - 1 : 0;
#if true
				int pOld = _Pad(_tb._border), pNew = _Pad(value), pDif = pNew - pOld;
				if(pDif != 0) this.Padding = new Padding(pNew);
				var w = this.Hwnd();
				if(!w.Is0) {
					RECT r = w.ClientRectInScreen; r.Inflate(pDif, pDif); r.Normalize(false);
					const WS mask = WS.CAPTION | WS.THICKFRAME | WS.SYSMENU;
					WS s1 = w.Style, s2 = _BorderStyle(value);
					if(s2 != (s1 & mask)) w.SetStyle((s1 & ~mask) | s2);
					//preserve client size and position
					Api.AdjustWindowRectEx(ref r, _BorderStyle(value), false, WS_EX.TOOLWINDOW);
					w.MoveLL(r.left, r.top, r.Width, r.Height, Native.SWP.FRAMECHANGED);
				}
#else //this simpler code does not preserve client size and position
				int pOld = _Pad(_tb._border), pNew = _Pad(value);
				if(pNew != pOld) this.Padding = new Padding(pNew);
				var w = this.Hwnd();
				if(!w.Is0) {
					const WS mask = WS.CAPTION | WS.THICKFRAME | WS.SYSMENU;
					WS s1 = w.Style, s2 = _BorderStyle(value);
					if(s2 != (s1 & mask)) w.SetStyle((s1 & ~mask) | s2, updateNC: true, updateClient: true);
				}
#endif
			}

			protected override unsafe void WndProc(ref Message m)
			{
				//AWnd.More.PrintMsg(m, new PrintMsgOptions(Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH, Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_NCMOUSEMOVE, Api.WM_MOUSEMOVE));
				//AWnd.More.PrintMsg(m, new PrintMsgOptions(Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH));

				switch(m.Msg) {
				case Api.WM_CREATE:
					_WmCreate();
					break;
				case Api.WM_DESTROY:
					_WmDestroy();
					break;
				case Api.WM_NCPAINT:
					if(_WmNcpaint((AWnd)m.HWnd)) return; //draws border if need
					break;
				case Api.WM_NCHITTEST:
					if(_WmNchittest(ref m)) return; //returns a hittest code to move or resize if need
					break;
				case Api.WM_NCLBUTTONDOWN:
					//workaround for: Windows tries to activate this window when moving or sizing it, unless this process is not allowed to activate windows.
					//	Usually this window would not become the foreground window, but it receives wm_activateapp, wm_activate, wm_setfocus, and is moved to the top of Z order.
					//	tested: LockSetForegroundWindow does not work.
					//	This code better would be under WM_SYSCOMMAND, but then works only when sizing. When moving, activates before WM_SYSCOMMAND.
					int ht = (int)m.WParam;
					if(ht == Api.HTCAPTION || (ht >= Api.HTSIZEFIRST && ht <= Api.HTSIZELAST)) {
						using(AHookWin.ThreadCbt(d => d.code == HookData.CbtEvent.ACTIVATE))
							base.WndProc(ref m);
						return;
					}
					break;
				case Api.WM_ENTERSIZEMOVE:
					_tb._InMoveSize(true);
					break;
				case Api.WM_EXITSIZEMOVE:
					_tb._InMoveSize(false);
					break;
				case Api.WM_LBUTTONDOWN:
				case Api.WM_RBUTTONDOWN:
				case Api.WM_MBUTTONDOWN:
					var tb1 = _tb._satPlanet ?? _tb;
					if(tb1.IsOwned && !tb1.MiscFlags.Has(TBFlags.DontActivateOwner)) {
						tb1.OwnerWindow.ActivateLL();
						//never mind: sometimes flickers. Here tb1._Zorder() does not help. The OBJECT_REORDER hook zorders when need. This feature is rarely used.
					}
					break;
				case Api.WM_MOUSEMOVE:
				case Api.WM_NCMOUSEMOVE:
					_tb._SatMouse();
					break;
				case Api.WM_WINDOWPOSCHANGING:
					_tb._OnWindowPosChanging(ref *(Api.WINDOWPOS*)m.LParam);
					break;
				case Api.WM_CONTEXTMENU:
					_ContextMenu(this);
					break;
				}

				base.WndProc(ref m);

				switch(m.Msg) {
				case Api.WM_WINDOWPOSCHANGED:
					_tb._OnWindowPosChanged(in *(Api.WINDOWPOS*)m.LParam);
					break;
				case Api.WM_DISPLAYCHANGE:
					_tb._OnDisplayChanged();
					break;
				case Api.WM_PAINT:
					_paintedOnce = true;
					//APerf.NW();
					break;
				}
			}

			void _WmCreate()
			{
				var tr = _tb._transparency;
				if(tr != default) this.Hwnd().SetTransparency(true, tr.opacity, tr.colorRGB);
			}

			void _WmDestroy()
			{
				Print("destroy", _tb._satList != null);
				_sett.Dispose();
				//PROBLEM: not called if thread ends without closing the toolbar window.
				//	Then will save settings only on process exit. Will not save if process terminated, eg by the 'end task' command.
				//	SHOULDDO: the 'end task' command should be more intelligent. At least should save settings.
			}

			//Alternative to returning HTCAPTION on WM_NCITTEST.
			//void _DragMoveWindow()
			//{
			//	_tb._InMoveSize(true);
			//	try {
			//		var w = (AWnd)this;
			//		var pp = AMouse.XY;
			//		Util.ADragDrop.SimpleDragDrop(w, MButtons.Left, o => {
			//			if(o.Msg.message != Api.WM_MOUSEMOVE) return;
			//			var p = AMouse.XY; if(p == pp) return;
			//			var r = w.Rect;
			//			w.MoveLL(r.left + (p.x - pp.x), r.top + (p.y - pp.y));
			//			pp = p;
			//		});
			//	}
			//	finally { _tb._InMoveSize(false); }
			//}

			bool _WmNchittest(ref Message m)
			{
				int h;
				if(ModifierKeys == Keys.Shift) { //move
					h = Api.HTCAPTION;
				} else { //resize?
					LPARAM xy = m.LParam;
					int x = AMath.LoShort(xy), y = AMath.HiShort(xy);
					if(_tb.Sizable) {
						int b;
						switch(_tb._border) {
						case TBBorder.Width1: b = 1; break;
						case TBBorder.Width2: b = 2; break;
						case TBBorder.Width3: case TBBorder.ThreeD: b = 3; break;
						case TBBorder.Width4: b = 4; break;
						default: return false;
						}
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
					} else {
						if(_tb._border < TBBorder.Thick) return false;
						this.Hwnd().LibGetWindowInfo(out var k);
						k.rcWindow.Inflate(-k.cxWindowBorders, -k.cyWindowBorders);
						if(k.rcWindow.Contains(x, y)) return false;
						h = Api.HTBORDER;
					}
				}
				m.Result = (IntPtr)h;
				return true;
			}

			bool _WmNcpaint(AWnd w)
			{
				if(_tb.BorderColor == 0 || _tb._border < TBBorder.Width1 || _tb._border > TBBorder.Width4) return false;
				using var dc = new Util.LibWindowDC(Api.GetWindowDC(w), w);
				using var g = Graphics.FromHdc(dc);
				var r = w.Rect; r.Offset(-r.left, -r.top);
				ControlPaint.DrawBorder(g, r, (Color)_tb.BorderColor, ButtonBorderStyle.Solid);
				return true;
			}

			//protected override void OnSizeChanged(EventArgs e)
			//{
			//	base.OnSizeChanged(e);
			//}

			bool _IAuToolStrip.PaintedOnce => _paintedOnce;
			bool _paintedOnce;

			void _ContextMenu(ToolStrip ts) //ts is either this or the overflow dropdown
			{
				if(this.ContextMenuStrip == null) {
					var m = new AMenu { MultiShow = true };
					m.Control.RenderMode = ToolStripRenderMode.System;

					if(CanGoToEdit_) {
						m["Edit"] = o => _tb.GoToEdit_(_cmItem);
						m.Separator();
					}
					using(var sub = m.Submenu("Anchor")) {
						_AddAnchor(TBAnchor.None);
						_AddAnchor(TBAnchor.TopLeft);
						_AddAnchor(TBAnchor.TopRight);
						_AddAnchor(TBAnchor.BottomLeft);
						_AddAnchor(TBAnchor.BottomRight);
						_AddAnchor(TBAnchor.Top);
						_AddAnchor(TBAnchor.Bottom);
						_AddAnchor(TBAnchor.Left);
						_AddAnchor(TBAnchor.Right);
						_AddAnchor(TBAnchor.All);

						void _AddAnchor(TBAnchor an) => m.Add(an.ToString(), _ => _tb.Anchor = an).Tag = an;

						sub.MenuItem.DropDown.Opening += (sender, _) => {
							var dd = sender as ToolStripDropDownMenu;
							foreach(ToolStripMenuItem v in dd.Items) if(v.Tag is TBAnchor an) v.Checked = an == _tb._anchor;
						};
					}
					using(var sub = m.Submenu("Border")) {
						_AddBorder(TBBorder.None);
						_AddBorder(TBBorder.Width1);
						_AddBorder(TBBorder.Width2);
						_AddBorder(TBBorder.Width3);
						_AddBorder(TBBorder.Width4);
						_AddBorder(TBBorder.ThreeD);
						_AddBorder(TBBorder.Thick);
						_AddBorder(TBBorder.Caption);
						_AddBorder(TBBorder.CaptionX);

						void _AddBorder(TBBorder b) => m.Add(b.ToString(), _ => _tb.Border = b).Tag = b;

						sub.MenuItem.DropDown.Opening += (sender, _) => {
							var dd = sender as ToolStripDropDownMenu;
							foreach(ToolStripMenuItem v in dd.Items) if(v.Tag is TBBorder b) v.Checked = b == _tb._border;
						};
					}
					var sizable = m.Add("Sizable", _ => _tb.Sizable ^= true);
					var autoSize = m.Add("AutoSize", _ => _tb.AutoSize ^= true);
					m.Control.Opening += (sender, _) => {
						var dd = sender as ToolStripDropDownMenu;
						sizable.Checked = _tb.Sizable;
						autoSize.Checked = _tb.AutoSize;
					};

					//this is an example of a form-like sumenu. Or maybe better create a UserControl-based class in the form designer.
					//m.Submenu("AutoSize", m => {
					//	var ddm = m.CurrentAddMenu;
					//	ddm.ShowImageMargin = false;
					//	int x = 80, cx = 70;
					//	var panel = new Panel { Width = x + cx + 3, Height = 86 };
					//	var cc = panel.Controls;
					//	cc.Add(new Label { Text = "Wrap width", Width = x });
					//	cc.Add(new Label { Text = "Max height", Width = x, Top = 30 });
					//	var wrapWidth = new NumericUpDown { Left = x, Width = cx, Maximum = 1000000, Increment = 50, Value = _tb.AutoSize.WrapWidth };
					//	var maxHeight = new NumericUpDown { Top = 30, Left = x, Width = cx, Maximum = 1000000, Increment = 50, Value = _tb.AutoSize.MaxHeight };
					//	cc.Add(wrapWidth);
					//	cc.Add(maxHeight);
					//	var ok = new Button { Top = 60, Left = x, Width = cx, Text = "Apply" };
					//	ok.Click += (_, __) => _tb.AutoSize = new TBAutoSize((int)wrapWidth.Value, (int)maxHeight.Value);
					//	cc.Add(ok);
					//	if(Util.ADpi.BaseDPI != 96) {
					//		float scale = Util.ADpi.BaseDPI / 96f;
					//		panel.Scale(new SizeF(scale, scale));
					//	}
					//	var h = new ToolStripControlHost(panel);
					//	m.Add(h);
					//});

					m.Separator();
					m["Close"] = o => _tb.Close();

					this.ContextMenuStrip = m.Control;
				}
				this.ContextMenuStrip.Items[0].Visible = !ATask.WndMsg.Is0;
				_cmItem = ts.GetItemAt(ts.MouseClientXY());
			}
			ToolStripItem _cmItem;

			#region overflow

			protected override void OnLayoutStyleChanged(EventArgs e)
			{
				base.OnLayoutStyleChanged(e);
				if(!(_tb?._constructed ?? false) || _overflow != null) return;
				switch(LayoutStyle) {
				case ToolStripLayoutStyle.HorizontalStackWithOverflow:
				case ToolStripLayoutStyle.VerticalStackWithOverflow:
					var ob = this.OverflowButton;
					ob.DropDown = _overflow = new _ToolStripOverflow(this, ob);
					break;
				}
			}
			_ToolStripOverflow _overflow;

			class _ToolStripOverflow : ToolStripOverflow
			{
				_ToolStrip _ts;

				internal _ToolStripOverflow(_ToolStrip ts, ToolStripItem parentItem) : base(parentItem) { _ts = ts; }

				//workaround for .NET bug: the default ToolStripOverflow window has a taskbar button and is activated on click.
				protected override CreateParams CreateParams {
					get {
						var p = base.CreateParams;
						p.ExStyle |= (int)(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE);
						return p;
					}
				}

				protected override unsafe void WndProc(ref Message m)
				{
					switch(m.Msg) {
					case Api.WM_MOUSEACTIVATE:
						m.Result = (IntPtr)Api.MA_NOACTIVATE;
						return;
					case Api.WM_CONTEXTMENU:
						_ts._ContextMenu(this);
						this.ContextMenuStrip = _ts.ContextMenuStrip;
						break;
					}

					base.WndProc(ref m);
				}
			}

			#endregion
		}
	}
}
