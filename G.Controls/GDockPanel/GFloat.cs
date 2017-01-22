using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
	partial class GDockPanel
	{
		/// <summary>
		/// Floating parent form of a GPanel.
		/// </summary>
		partial class GFloat :Form
		{
			GDockPanel _manager;
			GContentNode _gc; //a GPanel or GTab for which this GFloat is a temporary parent
			_DockIndicator _dockIndic; //calculates and displays dock indicator rectangles while the user Alt+drags this GFloat window. At other time null.
			bool _hasToolbar;

			internal GFloat(GDockPanel manager, GContentNode gc)
			{
				_manager = manager;
				_gc = gc;

				var gp = _gc as GPanel;
				_hasToolbar = (gp != null && gp.HasToolbar && gp.Content is CatToolStrip); //never mind: does not support MenuStrip like toolbars

				this.SuspendLayout();
				this.StartPosition = FormStartPosition.Manual;
				this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				this.AutoScaleMode = _manager.ParentForm.AutoScaleMode;
				this.Font = _manager.Font;
				this.Text = gc.Text;
				this.ResumeLayout();

				var f = _manager.ParentForm;
				f.VisibleChanged += ManagerForm_VisibleChanged;
				//f.EnabledChanged += ManagerForm_EnabledChanged; //in most cases does not work. Instead the main form on WM_ENABLE calls EnableDisableAllFloatingWindows.
			}

			protected override void Dispose(bool disposing)
			{
				_manager.ParentForm.VisibleChanged -= ManagerForm_VisibleChanged;
				base.Dispose(disposing);
			}

			private void ManagerForm_VisibleChanged(object sender, EventArgs e)
			{
				var f = sender as Form;
				this.Visible=f.Visible;
			}

			protected override CreateParams CreateParams
			{
				get
				{
					//This prevents resizing the window 3 times. But need to set FormBorderStyle, or will be wider.
					//note: this func is called several times, first time before ctor
					var p = base.CreateParams;
					p.Style = unchecked((int)(Api.WS_POPUP | Api.WS_THICKFRAME | Api.WS_CLIPCHILDREN));
					var ex = Api.WS_EX_TOOLWINDOW;
					if(_hasToolbar) ex |= Api.WS_EX_NOACTIVATE;
					p.ExStyle = (int)ex;
					return p;
				}
			}

			/// <summary>
			/// 1. Prevents activating window when showing. 2. Allows to show ToolTip for inactive window.
			/// </summary>
			protected override bool ShowWithoutActivation { get { return _hasToolbar; } }

			internal void OnReplacedChild(GContentNode newChild)
			{
				_gc = newChild;
			}

			protected override void WndProc(ref Message m)
			{
				if(_manager._WndProcBefore_Common(this, ref m)) return;

				//Util.Debug_.PrintMsg(ref m);
				//LPARAM WP = m.WParam, LP = m.LParam;

				switch((uint)m.Msg) {
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

			bool _sizeChanged;
			protected override void OnClientSizeChanged(EventArgs e)
			{
				_sizeChanged = true;
				var r = this.ClientRectangle;
				//PrintList(IsHandleCreated, r);
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
				GDockPanel _manager;
				GFloat _float;
				//these are calculated by OnFloatMoved() while moving the GFloat window:
				RECT _rect, _rectTabButton;
				DockTarget _target;
				bool _isTargetValid;

				internal _DockIndicator(GDockPanel manager, GFloat gfloat)
				{
					_manager = manager;
					_float = gfloat;
					_target = new DockTarget();

					this.SuspendLayout();
					this.FormBorderStyle = FormBorderStyle.None;
					this.StartPosition = FormStartPosition.Manual;
					this.AutoScaleMode = AutoScaleMode.None;
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
						k.ExStyle = (int)(Api.WS_EX_LAYERED | Api.WS_EX_NOACTIVATE | Api.WS_EX_TOOLWINDOW);
						k.Style = unchecked((int)(Api.WS_POPUP));
						return k;
					}
				}

				protected override bool ShowWithoutActivation { get { return true; } }

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

				internal void OnFloatMoved(POINT p)
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
				bool _OnFloatMoved(POINT p, out RECT r, out RECT rb)
				{
					r = rb = new RECT();

					var firstSplit = _manager._firstSplit;
					if(!firstSplit.Bounds.Contains(p)) return false;
					if(firstSplit.IsHidden) return false; //never mind, can dock without drag-drop
					var gc = this._float._gc;
					bool isThisTab = gc is GTab; //don't allow to add to a tab group
					bool isThisTB = !isThisTab && (gc as GPanel).HasToolbar;

					var panels = _manager._aPanel.FindAll(v => v.IsDockedOn(_manager));
					var tabs = _manager._aTab.FindAll(v => v.IsDockedOn(_manager));

					foreach(var gp in panels) {
						if(!gp.Bounds.Contains(p)) continue;

						//look in GPanel captions and tab buttons
						if(gp.CaptionBounds.Contains(p)) {
							if(isThisTab || isThisTB || gp.HasToolbar) return false;
							var gt = gp.ParentTab;
							//r = gp.Content.Bounds; //better don't display the big blue rect
							rb = gp.CaptionBounds;
							if(gt != null && gt.ShowsTabButtons) _target.side=DockSide.TabBefore;
							else rb = _CalcNewTabButtonRectInFullCaption(gp, rb, p); //sets _target.side
							_target.gc = gp;
							return true;
						}
						//look in GPanel content, except tabbed panels
						if(gp.IsTabbedPanel) continue;
						if(!isThisTB && gp.HasToolbar) continue; //don't allow to drop panels on toolbars, or would be problems because of minimal height
						r = _CalcDockRectPart(gp.Bounds, p); //sets _target.side
						_target.gc = gp;
						return true;
					}

					foreach(var gt in tabs) {
						if(!gt.Bounds.Contains(p)) continue;

						//look in GTab captions except child tab buttons
						if(gt.CaptionBounds.Contains(p)) {
							if(isThisTab || isThisTB) return false;
							if(gt.DockedItemCount > 0) {
								//r = gt.ActiveItem.Content.Bounds; //better don't display the big blue rect
								rb = gt.CaptionBoundsExceptButtons;
								_target.side = DockSide.TabAfter;
								_target.gc = gt.Items.Last();
							} else {
								rb = gt.CaptionBounds;
								_target.side=DockSide.TabBefore;
								_target.gc = gt;
							}
							return true;
						}
						//look in GTab rect
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
				RECT _CalcDockRectPart(RECT r, POINT p)
				{
					DockSide side;
					int wid = r.Width, hei = r.Height, wid2 = wid / 2, hei2 = hei / 2, dx = p.x - r.left, dy = p.y - r.top;
					double k = wid / (double)hei;
					if(dx < wid2) side = DockSide.SplitLeft; else { side = DockSide.SplitRight; dx = wid - dx; }
					if(dy < hei2) { if(dy * k < dx) side = DockSide.SplitAbove; } else { if((hei - dy) * k < dx) side = DockSide.SplitBelow; }
					switch(_target.side = side) {
					case DockSide.SplitLeft: r.right -= wid2; break;
					case DockSide.SplitAbove: r.bottom -= hei2; break;
					case DockSide.SplitRight: r.left += wid2; break;
					case DockSide.SplitBelow: r.top += hei2; break;
					}
					return r;
				}

				/// <summary>
				/// Returns a half of r (gp full caption, not just tab button), depending on where p is (even if not in r).
				/// Sets _target.side.
				/// </summary>
				RECT _CalcNewTabButtonRectInFullCaption(GPanel gp, RECT r, POINT p)
				{
					bool after;
					if(gp.IsVerticalCaption) {
						int mid = (r.top + r.bottom) / 2;
						if(after = (p.y >= mid)) r.top = mid; else r.bottom = mid;
					} else {
						int mid = (r.left + r.right) / 2;
						if(after = (p.x >= mid)) r.left = mid; else r.right = mid;
					}
					_target.side = after ? DockSide.TabAfter : DockSide.TabBefore;
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
			/// GFloat.Drag() and _DockIndicator.OnFloatDropped() return this if can be docked.
			/// </summary>
			internal class DockTarget
			{
				/// <summary>
				/// The target.
				/// Note: when dockAsTab is true, it currently may be tabbed or not; if tabbed, may be docked or not.
				/// </summary>
				internal GContentNode gc;
				/// <summary>Specifies whether to add on a GTab or GSplit, and at which side of gc.</summary>
				internal DockSide side;
			}

			internal DockTarget Drag(POINT p)
			{
				bool canDock = false;
				DockTarget target = null;

				Wnd w = (Wnd)this;
				RECT r = w.Rect;
				POINT offs = new POINT(p.x - r.left, p.y - r.top);
				bool ok = Catkeys.Util.DragDrop.SimpleDragDrop(w, MouseButtons.Left, d =>
				  {
					  if(d.Msg.message != Api.WM_MOUSEMOVE) return;

					  p = Mouse.XY;
					  w.MoveRaw(p.x - offs.x, p.y - offs.y);

					  if(!canDock && ModifierKeys.HasFlag(Keys.Alt)) {
						  canDock = true;
						  //this.AllowTransparency = true; this.Opacity = 0.5; //exception
						  ((Wnd)this).Transparency(true, 0.5);
						  _dockIndic = new _DockIndicator(_manager, this);
						  _dockIndic.Show(this);
					  }
					  if(canDock) _dockIndic.OnFloatMoved(_manager.PointToClient(p));
				  });

				if(canDock) {
					((Wnd)this).Transparency(false);
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
