//#define SYSTEM_RENDERER //good: button highlighting looks like of most toolbars. bad: alpha-blends button highlighting, almost invisible if dark background; highligted buttons are different than drop-down menu items.

using Au.Types;
using Au.Util;
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
using System.Linq;

namespace Au
{
	public partial class AToolbar
	{
		/// <summary>
		/// The type of <see cref="Control"/>. Inherits from <see cref="ToolStrip"/>.
		/// </summary>
		public class ToolStripWindow : AToolStrip, _IAuToolStrip
		{
			[ThreadStatic] static TBRenderer t_renderer;
			AToolbar _tb;

			///
			protected AToolbar Toolbar => _tb;

			_Settings _sett => _tb._sett;

			///
			public ToolStripWindow() {
				//like in ctors of Form and ToolStripDropDown
				this.Visible = false; //else SetTopLevel(true) creates handle and tries to activate
				this.SetTopLevel(true);

				this.SuspendLayout();
				this.AutoSize = false;
				this.Stretch = false;
				this.Dock = DockStyle.None;
				this.GripStyle = ToolStripGripStyle.Hidden;
				this.Padding = default;
				//this.BackColor = SystemColors.ButtonFace; //default
				this.Renderer = t_renderer ??= new TBRenderer();
				this.Name = "AToolbar.Control";
			}

			internal void Ctor2_(AToolbar tb) {
				_tb = tb;
				this.Text = tb._name;
			}

			///
			protected override CreateParams CreateParams {
				get {
					var p = base.CreateParams;
					if (_tb != null) {
						var es = WS2.TOOLWINDOW | WS2.NOACTIVATE;
						if (_tb._topmost) es |= WS2.TOPMOST;
						if (_tb._transparency != default) es |= WS2.LAYERED;
						p.ExStyle = (int)es;

						var style = ((WS)p.Style & ~(WS.CHILD | WS.CLIPSIBLINGS | WS.VISIBLE)) | WS.POPUP | _BorderStyle(_sett.border);
						p.Style |= (int)style;
					}
					return p;
				}
			}

			WS _BorderStyle(TBBorder b) => b switch {
				TBBorder.None => 0,
				TBBorder.ThreeD => WS.DLGFRAME,
				TBBorder.Thick => WS.THICKFRAME,
				TBBorder.Caption => WS.CAPTION | WS.THICKFRAME,
				TBBorder.CaptionX => WS.CAPTION | WS.THICKFRAME | WS.SYSMENU,
				_ => WS.BORDER
			};

			internal void SetBorder(TBBorder value) {
				static int _Pad(TBBorder b) => b >= TBBorder.Width2 && b <= TBBorder.Width4 ? (int)b - 1 : 0;
#if true
				int pOld = _Pad(_tb._border), pNew = _Pad(value), pDif = pNew - pOld;
				if (pDif != 0) this.Padding = new Padding(pNew);
				var w = this.Hwnd();
				if (!w.Is0) {
					RECT r = w.ClientRectInScreen; r.Inflate(pDif, pDif); r.Normalize(false);
					const WS mask = WS.CAPTION | WS.THICKFRAME | WS.SYSMENU;
					WS s1 = w.Style, s2 = _BorderStyle(value);
					if (s2 != (s1 & mask)) w.SetStyle((s1 & ~mask) | s2);
					//preserve client size and position
					ADpi.AdjustWindowRectEx(r, ref r, _BorderStyle(value), WS2.TOOLWINDOW);
					w.MoveLL(r, Native.SWP.FRAMECHANGED);
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
				//SHOULDDO: both above codes are incorrect. Must resize in direction that depends on Anchor and not on client rect. See _AutoSize.
			}

			///
			protected override unsafe void WndProc(ref Message m) {
				//if(_tb.Name=="ddd") AWnd.More.PrintMsg(m, new PrintMsgOptions(Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH, Api.WM_NCHITTEST, Api.WM_SETCURSOR, Api.WM_NCMOUSEMOVE, Api.WM_MOUSEMOVE));
				//AWnd.More.PrintMsg(m, new PrintMsgOptions(Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH));

				switch (m.Msg) {
				case Api.WM_CREATE:
					_WmCreate();
					break;
				case Api.WM_DESTROY:
					_WmDestroy();
					break;
				case Api.WM_NCPAINT:
					if (_WmNcpaint((AWnd)m.HWnd)) return; //draws border if need
					break;
				case Api.WM_NCHITTEST:
					if (_WmNchittest(ref m)) return; //returns a hittest code to move or resize if need
					break;
				case Api.WM_NCLBUTTONDOWN:
					//workaround for: Windows tries to activate this window when moving or sizing it, unless this process is not allowed to activate windows.
					//	Usually this window would not become the foreground window, but it receives wm_activateapp, wm_activate, wm_setfocus, and is moved to the top of Z order.
					//	tested: LockSetForegroundWindow does not work.
					//	This code better would be under WM_SYSCOMMAND, but then works only when sizing. When moving, activates before WM_SYSCOMMAND.
					int ht = (int)m.WParam;
					if (ht == Api.HTCAPTION || (ht >= Api.HTSIZEFIRST && ht <= Api.HTSIZELAST)) {
						using (AHookWin.ThreadCbt(d => d.code == HookData.CbtEvent.ACTIVATE))
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
					var tb1 = _tb._SatPlanetOrThis;
					if (tb1.IsOwned && !tb1.MiscFlags.Has(TBFlags.DontActivateOwner)) {
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
				}

				base.WndProc(ref m);

				switch (m.Msg) {
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
				case Api.WM_RBUTTONUP:
				case Api.WM_NCRBUTTONUP:
					OnContextMenu(this);
					break;
				}
			}

			void _WmCreate() {
				ACursor.SetArrowCursor_();
				var tr = _tb._transparency;

				//AOutput.Write(Bounds);
				//TODO: create where should be, not at 0 0, because wrong DPI.
				//	Then set ImageScalingSize like in _ContextMenuStrip.OnOpening.
				//TODO: on DPI changed scale images and text.

				if (tr != default) this.Hwnd().SetTransparency(true, tr.opacity, tr.colorKey);
			}

			void _WmDestroy() {
				if (RecreatingHandle) {
					ADebug.Print("RecreateHandle not supported. Will not work satellite etc.");
					return;
				}
				_destroyed = true;
				if (!Disposing) Dispose();
				_tb._SatDestroying();
				_sett.Dispose();
				//PROBLEM: not called if thread ends without closing the toolbar window.
				//	Then will save settings only on process exit. Will not save if process terminated, eg by the 'end task' command.
				//	SHOULDDO: the 'end task' command should be more intelligent. At least should save settings.
			}

			bool _destroyed;

			///
			protected override void DestroyHandle() {
				if (!_destroyed) base.DestroyHandle();
			}

			///
			protected override void Dispose(bool disposing) {
				_tb._Dispose(disposing);
				base.Dispose(disposing);
			}

			//Alternative to returning HTCAPTION on WM_NCITTEST.
			//void _DragMoveWindow()
			//{
			//	_tb._InMoveSize(true);
			//	try {
			//		var w = (AWnd)this;
			//		var pp = AMouse.XY;
			//		ADragDrop.SimpleDragDrop(w, MButtons.Left, o => {
			//			if(o.Msg.message != Api.WM_MOUSEMOVE) return;
			//			var p = AMouse.XY; if(p == pp) return;
			//			var r = w.Rect;
			//			w.MoveLL(r.left + (p.x - pp.x), r.top + (p.y - pp.y));
			//			pp = p;
			//		});
			//	}
			//	finally { _tb._InMoveSize(false); }
			//}

			bool _WmNchittest(ref Message m) {
				int h;
				if (ModifierKeys == Keys.Shift) { //move
					h = Api.HTCAPTION;
				} else { //resize?
					if (_tb._border == TBBorder.None || (!_tb.Sizable && _tb._border < TBBorder.Thick)) return false;
					var w = this.Hwnd();
					LPARAM xy = m.LParam;
					int x = AMath.LoShort(xy), y = AMath.HiShort(xy);
					if (_tb.Sizable) {
						RECT r; int b;
						if (_tb._border < TBBorder.ThreeD) {
							b = (int)_tb._border;
							r = w.Rect;
						} else {
							w.GetWindowInfo_(out var k);
							r = k.rcWindow;
							b = k.cxWindowBorders;
						}
						int bx = Math.Min(b, r.Width / 2), by = Math.Min(b, r.Height / 2);
						int x1 = r.left + bx, x2 = r.right - bx - 1, y1 = r.top + by, y2 = r.bottom - by - 1;
						if (x < x1) {
							h = y < y1 ? Api.HTTOPLEFT : (y > y2 ? Api.HTBOTTOMLEFT : Api.HTLEFT);
						} else if (x > x2) {
							h = y < y1 ? Api.HTTOPRIGHT : (y > y2 ? Api.HTBOTTOMRIGHT : Api.HTRIGHT);
						} else if (y < y1) {
							h = Api.HTTOP;
						} else if (y > y2) {
							h = Api.HTBOTTOM;
						} else return false;
					} else { //disable resizing if border is natively sizable
						if (_tb._border < TBBorder.Thick) return false;
						w.GetWindowInfo_(out var k);
						k.rcWindow.Inflate(-k.cxWindowBorders, -k.cyWindowBorders);
						if (k.rcWindow.Contains(x, y)) return false;
						h = Api.HTBORDER;
					}
				}
				m.Result = (IntPtr)h;
				return true;
			}

			bool _WmNcpaint(AWnd w) {
				if (_tb.BorderColor == 0 || _tb._border < TBBorder.Width1 || _tb._border > TBBorder.Width4) return false;
				using var dc = new WindowDC_(Api.GetWindowDC(w), w);
				using var g = Graphics.FromHdc(dc);
				var r = w.Rect; r.Offset(-r.left, -r.top);
				ControlPaint.DrawBorder(g, r, (Color)_tb.BorderColor, ButtonBorderStyle.Solid);
				return true;
			}

			bool _IAuToolStrip.PaintedOnce => _paintedOnce;
			bool _paintedOnce;

			/// <summary>
			/// Called on right-click. Shows context menu.
			/// </summary>
			/// <param name="ts">This control or the overflow dropdown.</param>
			protected virtual void OnContextMenu(ToolStrip ts) {
				var no = _tb.NoMenu;
				if (no.Has(TBNoMenu.Menu)) return;
				var m = new AMenu();

				if (!no.Has(TBNoMenu.Edit) && AScriptEditor.Available) {
					m["Edit"] = o => _tb.GoToEdit_(_cmItem);
					m.Separator();
				}
				if (!no.Has(TBNoMenu.Anchor)) {
					using (m.Submenu("Anchor")) {
						_AddAnchor(TBAnchor.None);
						_AddAnchor(TBAnchor.TopLeft);
						_AddAnchor(TBAnchor.TopRight);
						_AddAnchor(TBAnchor.BottomLeft);
						_AddAnchor(TBAnchor.BottomRight);
						_AddAnchor(TBAnchor.TopLR);
						_AddAnchor(TBAnchor.BottomLR);
						_AddAnchor(TBAnchor.LeftTB);
						_AddAnchor(TBAnchor.RightTB);
						_AddAnchor(TBAnchor.All);
						m.Separator();
						_AddAnchor(TBAnchor.OppositeEdgeX);
						_AddAnchor(TBAnchor.OppositeEdgeY);

						void _AddAnchor(TBAnchor an) {
							var k = m.Add(an.ToString(), _ => {
								var ta = _tb.Anchor;
								if (an < TBAnchor.OppositeEdgeX) ta = (ta & ~TBAnchor.All) | an; else ta ^= an;
								_tb.Anchor = ta;
							});
							if (an < TBAnchor.OppositeEdgeX) {
								k.IsChecked = _tb._anchor.WithoutFlags() == an;
							} else {
								k.IsChecked = _tb._anchor.Has(an);
								k.IsDisabled = _GetInvalidAnchorFlags(_tb._anchor).Has(an);
							}
							//if(_tb._IsSatellite) k.ToolTipText = "Note: You may want to set anchor of the owner toolbar instead. Anchor of this auto-hide toolbar is relative to the owner toolbar.";
						}
					}
				}
				if (!no.Has(TBNoMenu.Layout) && _tb.Layout != TBLayout.Table) {
					using (m.Submenu("Layout")) {
						_AddLayout(TBLayout.Flow);
						if (!_tb._hasGroups) _AddLayout(TBLayout.Horizontal);
						_AddLayout(TBLayout.Vertical);

						void _AddLayout(TBLayout tl) {
							m.Add(tl.ToString(), _ => _tb.Layout = tl).IsChecked = tl == _tb.Layout;
						}
					}
				}
				if (!no.Has(TBNoMenu.Border)) {
					using (m.Submenu("Border")) {
						_AddBorder(TBBorder.None);
						_AddBorder(TBBorder.Width1);
						_AddBorder(TBBorder.Width2);
						_AddBorder(TBBorder.Width3);
						_AddBorder(TBBorder.Width4);
						_AddBorder(TBBorder.ThreeD);
						_AddBorder(TBBorder.Thick);
						_AddBorder(TBBorder.Caption);
						_AddBorder(TBBorder.CaptionX);

						void _AddBorder(TBBorder b) {
							m.Add(b.ToString(), _ => _tb.Border = b).IsChecked = b == _tb._border;
						}
					}
				}
				if (!no.Has(TBNoMenu.Sizable)) m.Add("Sizable", _ => _tb.Sizable ^= true).IsChecked = _tb.Sizable;
				if (!no.Has(TBNoMenu.AutoSize)) m.Add("AutoSize", _ => _tb.AutoSize ^= true).IsChecked = _tb.AutoSize;
				if (!no.Has(TBNoMenu.More)) {
					using (m.Submenu("More")) {
						if (_tb._SatPlanetOrThis.IsOwned) _AddFlag(TBFlags.DontActivateOwner);
						_AddFlag(TBFlags.HideIfFullScreen);

						void _AddFlag(TBFlags f) {
							var tb = _tb._SatPlanetOrThis;
							m.Add(_EnumToString(f), _ => tb.MiscFlags ^= f).IsChecked = tb.MiscFlags.Has(f);
						}

						static string _EnumToString(Enum e) {
							var s = e.ToString().RegexReplace(@"(?<=[^A-Z])(?=[A-Z])", " ");
							s = s.Replace("Dont", "Don't");
							return s;
						}
					}
				}

				if (!no.Has(TBNoMenu.Toolbars)) m.Add("Toolbars...", o => new _Form().Show());
				if (!no.Has(TBNoMenu.Close)) {
					m.Separator();
					m.Add("Close", o => _tb._Close(true));
				}

				//m["test RecreateHandle"] = o => this.RecreateHandle();

				_cmItem = ts.GetItemAt(ts.MouseClientXY());
				m.Show(ts);
			}
			ToolStripItem _cmItem;

			internal bool ContextMenuVisible_ { get; private set; }

			#region overflow

			///
			protected override void OnLayoutStyleChanged(EventArgs e) {
				if (_tb?._constructed ?? false) {
					_tb._OnLayoutStyleChanged();
					if (_overflow == null) {
						switch (LayoutStyle) {
						case ToolStripLayoutStyle.HorizontalStackWithOverflow:
						case ToolStripLayoutStyle.VerticalStackWithOverflow:
							var ob = this.OverflowButton;
							ob.DropDown = _overflow = new _ToolStripOverflow(this, ob);
							PerformLayout(); //workaround for: if changed layout after Show, the overflow strip is empty
							break;
						}
					}
					_tb._AutoSize();
				}
				base.OnLayoutStyleChanged(e);
			}
			_ToolStripOverflow _overflow;

			class _ToolStripOverflow : ToolStripOverflow
			{
				ToolStripWindow _ts;

				internal _ToolStripOverflow(ToolStripWindow ts, ToolStripItem parentItem) : base(parentItem) { _ts = ts; }

				//workaround for .NET bug: the default ToolStripOverflow window has a taskbar button and is activated on click.
				protected override CreateParams CreateParams {
					get {
						var p = base.CreateParams;
						p.ExStyle |= (int)(WS2.TOOLWINDOW | WS2.NOACTIVATE);
						return p;
					}
				}

				protected override unsafe void WndProc(ref Message m) {
					switch (m.Msg) {
					case Api.WM_MOUSEACTIVATE:
						m.Result = (IntPtr)Api.MA_NOACTIVATE;
						return;
					case Api.WM_CONTEXTMENU:
						_ts.OnContextMenu(this);
						break;
					}

					base.WndProc(ref m);
				}
			}

			#endregion
		}
	}
}
