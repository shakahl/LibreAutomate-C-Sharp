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
using System.Drawing.Drawing2D;
using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	public partial class AuDockPanel
	{
		/// <summary>
		/// Floating parent form of a _Panel.
		/// </summary>
		partial class _Float :Form
		{
			AuDockPanel _manager;
			_ContentNode _gc; //a _Panel or _Tab for which this _Float is a temporary parent
			_DockIndicator _dockIndic; //calculates and displays dock indicator rectangles while the user Alt+drags this _Float window. At other time null.
			bool _hasToolbar;

			internal _Float(AuDockPanel manager, _ContentNode gc)
			{
				_manager = manager;
				_gc = gc;

				var gp = _gc as _Panel;

				//we'll use this to prevent window activation on click, for toolbar and menubar classes that support it
				_hasToolbar = (gp != null && gp.HasToolbar && (gp.Content is Util.AToolStrip || gp.Content is Util.AMenuStrip));

				this.SuspendLayout();
				this.Font = _manager.Font;
				this.AutoScaleMode = AutoScaleMode.None;
				this.StartPosition = FormStartPosition.Manual;
				this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
				this.MinimumSize = new Size(34, 30);
				this.Text = gc.ToString();
				this.ResumeLayout();

				var f = _manager.TopLevelControl;
				f.VisibleChanged += ManagerForm_VisibleChanged;
				//f.EnabledChanged += ManagerForm_EnabledChanged; //in most cases does not work. Instead the main form on WM_ENABLE calls EnableDisableAllFloatingWindows.
			}

			protected override void Dispose(bool disposing)
			{
				var f = _manager.TopLevelControl;
				if(f != null) f.VisibleChanged -= ManagerForm_VisibleChanged;
				base.Dispose(disposing);
			}

			private void ManagerForm_VisibleChanged(object sender, EventArgs e)
			{
				var f = sender as Form;
				this.Visible = f.Visible;
			}

			protected override CreateParams CreateParams
			{
				get
				{
					//This prevents resizing the window 3 times. But need to set FormBorderStyle, or will be wider.
					//note: this func is called several times, first time before ctor
					var p = base.CreateParams;
					p.Style = unchecked((int)(WS.POPUP | WS.THICKFRAME | WS.CLIPCHILDREN));
					var ex = WS2.TOOLWINDOW;
					if(_hasToolbar) ex |= WS2.NOACTIVATE;
					p.ExStyle = (int)ex;
					return p;
				}
			}

			/// <summary>
			/// 1. Prevents activating window when showing. 2. Allows to show ToolTip for inactive window.
			/// </summary>
			protected override bool ShowWithoutActivation => true;

			protected override void OnGotFocus(EventArgs e)
			{
				(_gc as _Panel)?.Content?.Focus();
				//Normally the control is automatically focused, and this func not called.
				//But not if the control contains a native child control.
			}

			internal void OnReplacedChild(_ContentNode newChild)
			{
				_gc = newChild;
			}

			protected override void WndProc(ref Message m)
			{
				if(_manager._WndProcBefore_Common(this, ref m)) return;

				switch(m.Msg) {
				case Api.WM_CREATE:
					if(!_sizeChanged) OnClientSizeChanged(null); //OnClientSizeChanged not called if the size is = default form size (300, 300)
					break;
				case Api.WM_MOUSEACTIVATE:
					if(_hasToolbar) {
						m.Result = (IntPtr)Api.MA_NOACTIVATE;
						return;
					}
					break;
				}

				base.WndProc(ref m);

				_manager._WndProcAfter_Common(this, ref m);
			}

			protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
			{
				if(base.ProcessCmdKey(ref msg, keyData)) return true;
				if(keyData == Keys.Escape) {
					_manager.ZFocusControlOnUndockEtc?.Focus();
				}
				return false;
			}

			bool _sizeChanged;
			protected override void OnClientSizeChanged(EventArgs e)
			{
				_sizeChanged = true;
				var r = this.ClientRectangle;
				//Print(IsHandleCreated, r);
				if(r.Width > 0 && r.Height > 0) _gc.UpdateLayout(r); //not r.IsEmpty, because can be negative Width/Height
				if(e != null) base.OnClientSizeChanged(e);
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				_manager._OnPaint_Common(e);
				_gc.Paint(e.Graphics);

				base.OnPaint(e);
			}

			protected override void OnMouseLeave(EventArgs e)
			{
				_manager._OnMouseLeave_Common(this);
				base.OnMouseLeave(e);
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				_manager._OnMouseDown_Common(this, e);
				base.OnMouseDown(e);
			}

			protected override void OnMouseUp(MouseEventArgs e)
			{
				_manager._OnMouseUp_Common(this, e);
				base.OnMouseUp(e);
			}

			/// <summary>
			/// Calculates and shows transparent rectangles where the floating panel can be docked when dragging it.
			/// </summary>
			class _DockIndicator :Form
			{
				AuDockPanel _manager;
				_Float _float;
				//these are calculated by OnFloatMoved() while moving the _Float window:
				RECT _rect, _rectTabButton;
				DockTarget _target;
				bool _isTargetValid;

				internal _DockIndicator(AuDockPanel manager, _Float gfloat)
				{
					_manager = manager;
					_float = gfloat;
					_target = new DockTarget();

					this.SuspendLayout();
					this.AutoScaleMode = AutoScaleMode.None;
					this.FormBorderStyle = FormBorderStyle.None;
					this.StartPosition = FormStartPosition.Manual;
					this.Bounds = _manager._firstSplit.RectangleInScreen;
					this.Opacity = 0.3;
					this.TransparencyKey = Color.Yellow;
					this.SetStyle(ControlStyles.Opaque, true);
					this.ResumeLayout(false);
				}

				protected override CreateParams CreateParams
				{
					get
					{
						CreateParams k = base.CreateParams;
						k.ExStyle = (int)(WS2.LAYERED | WS2.NOACTIVATE | WS2.TOOLWINDOW);
						k.Style = unchecked((int)(WS.POPUP));
						return k;
					}
				}

				protected override bool ShowWithoutActivation => true;

				protected override void OnPaint(PaintEventArgs e)
				{
					var g = e.Graphics;
					g.Clear(Color.Yellow);
					if(!_rect.IsEmpty) {
						var brush = new SolidBrush(Color.DodgerBlue);
						g.FillRectangle(brush, _rect);
						brush.Dispose();
					}
					if(!_rectTabButton.IsEmpty) {
						var pen = new Pen(Color.Red, 3);
						g.DrawRectangle(pen, _rectTabButton);
						pen.Dispose();
					}
				}

				internal void OnFloatMoved(Point p)
				{
					RECT r, rb;
					_isTargetValid = _OnFloatMoved(p, out r, out rb);
					if(r != _rect || rb != _rectTabButton) {
						_rect = r;
						_rectTabButton = rb;
						Invalidate();
					}
				}

				/// <summary>
				/// Everything is in _manager client area.
				/// </summary>
				bool _OnFloatMoved(Point p, out RECT r, out RECT rb)
				{
					r = rb = new RECT();

					var firstSplit = _manager._firstSplit;
					if(!firstSplit.Bounds.Contains(p)) return false;
					if(firstSplit.IsHidden) return false; //never mind, can dock without drag-drop
					var gc = this._float._gc;
					bool isThisTab = gc is _Tab; //don't allow to add to a tab group
					bool isThisTB = !isThisTab && (gc as _Panel).HasToolbar;

					var panels = _manager._aPanel.FindAll(v => v.IsDockedOn(_manager));
					var tabs = _manager._aTab.FindAll(v => v.IsDockedOn(_manager));

					foreach(var gp in panels) {
						if(!gp.Bounds.Contains(p)) continue;

						//look in _Panel captions and tab buttons
						if(gp.CaptionBounds.Contains(p)) {
							if(isThisTab || isThisTB || gp.HasToolbar) return false;
							var gt = gp.ParentTab;
							//r = gp.Content.Bounds; //better don't display the big blue rect
							rb = gp.CaptionBounds;
							if(gt != null && gt.ShowsTabButtons) _target.side = _DockHow.TabBefore;
							else rb = _CalcNewTabButtonRectInFullCaption(gp, rb, p); //sets _target.side
							_target.gc = gp;
							return true;
						}
						//look in _Panel content, except tabbed panels
						if(gp.IsTabbedPanel) continue;
						if(!isThisTB && gp.HasToolbar) continue; //don't allow to drop panels on toolbars, or would be problems because of minimal height
						r = _CalcDockRectPart(gp.Bounds, p); //sets _target.side
						_target.gc = gp;
						return true;
					}

					foreach(var gt in tabs) {
						if(!gt.Bounds.Contains(p)) continue;

						//look in _Tab captions except child tab buttons
						if(gt.CaptionBounds.Contains(p)) {
							if(isThisTab || isThisTB) return false;
							if(gt.DockedItemCount > 0) {
								//r = gt.ActiveItem.Content.Bounds; //better don't display the big blue rect
								rb = gt.CaptionBoundsExceptButtons;
								_target.side = _DockHow.TabAfter;
								_target.gc = gt.Items.Last();
							} else {
								rb = gt.CaptionBounds;
								_target.side = _DockHow.TabBefore;
								_target.gc = gt;
							}
							return true;
						}
						//look in _Tab rect
						r = _CalcDockRectPart(gt.Bounds, p); //sets _target.side
						_target.gc = gt;
						return true;
					}

					//outer //never mind
					//var splits = _manager._aSplit.FindAll(v => v.IsDocked);

					return false;
				}

				/// <summary>
				/// Returns the left, top, right or bottom half of r, depending on where p is in it.
				/// Sets _target.side.
				/// </summary>
				RECT _CalcDockRectPart(RECT r, Point p)
				{
					_DockHow side;
					int wid = r.Width, hei = r.Height, wid2 = wid / 2, hei2 = hei / 2, dx = p.X - r.left, dy = p.Y - r.top;
					double k = wid / (double)hei;
					if(dx < wid2) side = _DockHow.SplitLeft; else { side = _DockHow.SplitRight; dx = wid - dx; }
					if(dy < hei2) { if(dy * k < dx) side = _DockHow.SplitAbove; } else { if((hei - dy) * k < dx) side = _DockHow.SplitBelow; }
					switch(_target.side = side) {
					case _DockHow.SplitLeft: r.right -= wid2; break;
					case _DockHow.SplitAbove: r.bottom -= hei2; break;
					case _DockHow.SplitRight: r.left += wid2; break;
					case _DockHow.SplitBelow: r.top += hei2; break;
					}
					return r;
				}

				/// <summary>
				/// Returns a half of r (gp full caption, not just tab button), depending on where p is (even if not in r).
				/// Sets _target.side.
				/// </summary>
				RECT _CalcNewTabButtonRectInFullCaption(_Panel gp, RECT r, Point p)
				{
					bool after;
					if(gp.IsVerticalCaption) {
						int mid = (r.top + r.bottom) / 2;
						if(after = (p.Y >= mid)) r.top = mid; else r.bottom = mid;
					} else {
						int mid = (r.left + r.right) / 2;
						if(after = (p.X >= mid)) r.left = mid; else r.right = mid;
					}
					_target.side = after ? _DockHow.TabAfter : _DockHow.TabBefore;
					return r;
				}

				internal DockTarget OnFloatDropped()
				{
					if(!_isTargetValid) return null;
					//Print($"{_target.gc?.Name}, dockAsTab={_target.dockAsTab}, verticalSplit={_target.verticalSplit}, after={_target.after}");
					return _target;
				}
			} //class _DockIndicator

			/// <summary>
			/// _Float.Drag() and _DockIndicator.OnFloatDropped() return this if can be docked.
			/// </summary>
			internal class DockTarget
			{
				/// <summary>
				/// The target.
				/// Note: when dockAsTab is true, it currently may be tabbed or not; if tabbed, may be docked or not.
				/// </summary>
				internal _ContentNode gc;
				/// <summary>Specifies whether to add on a _Tab or _Split, and at which side of gc.</summary>
				internal _DockHow side;
			}

			internal DockTarget Drag(Point p)
			{
				bool canDock = false;
				DockTarget target = null;

				AWnd w = (AWnd)this;
				RECT r = w.Rect;
				Point offs = new Point(p.X - r.left, p.Y - r.top);
				bool ok = Au.Util.ADragDrop.SimpleDragDrop(w, MButtons.Left, d =>
				  {
					  if(d.Msg.message != Api.WM_MOUSEMOVE) return;

					  p = AMouse.XY;
					  w.MoveLL(p.X - offs.X, p.Y - offs.Y);

					  if(!canDock && ModifierKeys.HasFlag(Keys.Alt)) {
						  canDock = true;
						  //this.AllowTransparency = true; this.Opacity = 0.5; //exception
						  ((AWnd)this).SetTransparency(true, 128);
						  _dockIndic = new _DockIndicator(_manager, this);
						  _dockIndic.Show(this);
					  }
					  if(canDock) _dockIndic.OnFloatMoved(_manager.PointToClient(p));
				  });

				if(canDock) {
					((AWnd)this).SetTransparency(false);
					_dockIndic.Close();
					if(ok) {
						target = _dockIndic.OnFloatDropped();
					}
					_dockIndic = null;
				}

				return target;
			}
		}
	}
}
