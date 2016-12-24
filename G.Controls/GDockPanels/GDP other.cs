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
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	partial class GDockPanels
	{
		class _PainTools
		{
			const int _captionColor = 0x7D5A46;
			const int _inactiveTabBackColor = 0x906C56;

			internal Brush brushSplitter, brushCaptionBack, brushInactiveTabBack, brushCaptionText, brushActiveTabText, brushInactiveTabHighlightBack;
			internal StringFormat txtFormatHorz, txtFormatVert;
			bool _inited;

			internal _PainTools(GDockPanels manager)
			{
				if(!_inited) {
					brushSplitter = new SolidBrush(manager.BackColor);
					var colCapBack = Calc.ColorFromRGB(0x465A7D);
					brushCaptionBack = new SolidBrush(colCapBack);
					brushInactiveTabBack = new SolidBrush(ControlPaint.Light(colCapBack, 0.5F));
					brushInactiveTabHighlightBack = new SolidBrush(ControlPaint.Light(colCapBack, 0.7F));
					brushCaptionText = Brushes.White;
					brushActiveTabText = Brushes.Black;
					txtFormatHorz = new StringFormat(StringFormatFlags.NoWrap);
					txtFormatVert = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical);
					//txtFormatHorz.Trimming = txtFormatVert.Trimming = StringTrimming.EllipsisCharacter; //.NET bug with vertical: displays a rectangle or nothing, instead of ...
					_inited = true;
				}
			}

			internal void Dispose()
			{
				if(_inited) {
					_inited = false;
					brushSplitter.Dispose();
					brushCaptionBack.Dispose();
					brushInactiveTabBack.Dispose();
					brushInactiveTabHighlightBack.Dispose();
					//brushCaptionText.Dispose();
					//brushActiveTabText.Dispose();
					txtFormatHorz.Dispose();
					txtFormatVert.Dispose();
				}
			}
		}

		class GNode
		{
			protected GDockPanels _manager;
			internal GSplit ParentSplit;
			internal Rectangle Bounds; //in current parent client area
			internal GDockState DockState;

			internal GNode(GDockPanels manager, GSplit parentSplit)
			{
				_manager = manager;
				ParentSplit = parentSplit;
				//manager._nodes.Add(this);
			}

			/// <summary>
			/// Returns true if is docked in main window or in a floating tab.
			/// <seealso cref="GContentNode.IsDockedOn"/>.
			/// </summary>
			internal bool IsDocked { get { return DockState == GDockState.Docked; } }
			internal bool IsHidden { get { return DockState == GDockState.Hidden; } }

			internal virtual void Paint(Graphics g) { }
			internal virtual void UpdateLayout(Rectangle r) { }
		}

		class GContentNode :GNode
		{
			internal Rectangle CaptionBounds; //in current parent client area
			internal Rectangle SavedDockedBounds; //when floating etc, contains bounds when was docked in main window, to restore when docking again
			internal bool IsVerticalCaption;
			internal GFloat ParentFloat;

			internal GContentNode(GDockPanels manager, GSplit parentSplit) : base(manager, parentSplit)
			{

			}

			internal virtual string Name { get; set; }

			internal Control ParentControl { get { return ParentFloat ?? (Control)_manager; } }

			internal bool IsDockedOn(Control parent) { return this.IsDocked && this.ParentControl == parent; }

			internal bool IsFloating { get { return DockState == GDockState.Floating; } }
			internal bool IsAutoHide { get { return DockState == GDockState.AutoHide; } }

			internal override void UpdateLayout(Rectangle r)
			{
				Debug.Assert(!this.IsHidden);
				this.Bounds = r;

				bool isDoc = this is GDocument;
				int capThick = _manager._captionWidth;
				bool vertCap = this.IsVerticalCaption = isDoc ? false : (r.Width > r.Height + capThick);
				int capWid = 0, capHei = 0;
				if(vertCap) capWid = capThick; else capHei = capThick;
				var cb = new Rectangle(r.Left + capWid, r.Top + capHei, r.Width - capWid, r.Height - capHei);
				cb.Inflate(-1, -1); if(cb.Width < 0) cb.Width = 0; if(cb.Height < 0) cb.Height = 0; //border
				this.CaptionBounds = new Rectangle(r.Left, r.Top, vertCap ? capWid : r.Width, vertCap ? r.Height : capHei);

				var gt = this as GTab;
				if(gt != null) {
					gt.UpdateChildrenLayout(cb);
				} else {
					var gp = this as GPanel;
					Debug.Assert(gp.ParentTab == null);
					gp.Content.Bounds = cb;
				}

				this.InvalidateCaption();
			}

			internal void Invalidate()
			{
				Debug.Assert(!this.IsHidden);
				this.ParentControl.Invalidate(this.Bounds, false);
			}

			internal void InvalidateCaption()
			{
				Debug.Assert(!this.IsHidden);
				var u = this.CaptionBounds; if(IsVerticalCaption) u.Width++; else u.Height++; //border
				this.ParentControl.Invalidate(u, false);
			}

			internal virtual bool HitTestCaption(Control parent, int x, int y)
			{
				_AssertParent(parent);
				if(this.IsHidden || parent != this.ParentControl) return false;
				return CaptionBounds.Contains(x, y);
			}

			internal override void Paint(Graphics g)
			{
				Debug.Assert(!this.IsHidden);

				bool isDoc = this is GDocument;
				var t = _manager._paintTools;
				bool vert = this.IsVerticalCaption;

				var cb = this.CaptionBounds;
				g.FillRectangle(t.brushCaptionBack, cb);

				string s = Name;
				var tf = vert ? t.txtFormatVert : t.txtFormatHorz;
				//g.DrawString(s, _manager.Font, t.brushCaptionText, cb.Left + 1, cb + 1, tf);
				g.DrawString(s, _manager.Font, t.brushCaptionText, cb, tf);
				//info: TextRenderer.DrawText cannot draw vertical.
				//tested: with "Segoe UI" font same quality as TextRenderer.DrawText or ExtTextOut.

				int siz = 16;
				Rectangle r = vert ? new Rectangle(0, cb.Bottom - siz, siz, siz) : new Rectangle(cb.Right - siz, 0, siz, siz);
				//ControlPaint.DrawComboButton(g, r, ButtonState.Flat); //bad
				//ControlPaint.DrawCaptionButton(g, r, CaptionButton.Close, ButtonState.Flat); //bad
			}

			internal void ShowContextMenu(Point p)
			{
				bool isTab = this is GTab;
				ContextMenu m = null;
				var mFloat = new MenuItem("Float\tDClick, Drag", (unu, sed) => this.SetDockState(GDockState.Floating));
				var mDock = new MenuItem("Dock\tDClick, Alt+Drag", (unu, sed) => this.SetDockState(GDockState.Docked));
				var mAH = new MenuItem("Auto Hide", (unu, sed) => this.SetDockState(GDockState.AutoHide));
				var mHide = new MenuItem("Hide", (unu, sed) => this.SetDockState(GDockState.Hidden));
				var state = this.DockState;
				mFloat.Enabled = state != GDockState.Floating;
				mDock.Enabled = state != GDockState.Docked;
				mAH.Enabled = state != GDockState.AutoHide && !isTab;
				mHide.Enabled = state != GDockState.Hidden && !isTab;
				m = new ContextMenu(new MenuItem[] { mFloat, mDock, mAH, mHide, new MenuItem("test", (unu, sed) => Out("item")) });
				m.Show(this.ParentControl, p);
				Application.DoEvents(); m.Dispose();
			}

			internal void SetDockState(GDockState state)
			{
				var prevState = this.DockState;
				if(state == prevState) return;
				this.DockState = state;

				var gp = this as GPanel;
				var gt = this as GTab;
				var parentTab = gp?.ParentTab;

				if(gp != null) gp.Content.Hide();
				else gt.Tabs.ForEach(v => { if(v.IsDocked) v.Content.Hide(); });

				switch(prevState) {
				case GDockState.Docked:
					this.SavedDockedBounds = this.Bounds;

					if(parentTab != null) parentTab.OnChildUndocked(gp);
					else this.ParentSplit.OnChildUndocked(this);
					break;
				case GDockState.Floating:
				case GDockState.AutoHide:
					if(gp != null) gp.Content.Parent = _manager;
					else gt.Tabs.ForEach(v => { if(v.ParentFloat == this.ParentFloat) { v.ParentFloat = null; v.Content.Parent = _manager; } });
					this.ParentFloat.Close();
					//this.ParentFloat.Dispose(); //TODO: destroys the window?
					this.ParentFloat = null;
					break;
				}

				switch(state) {
				case GDockState.Docked:
					this.Bounds = this.SavedDockedBounds;

					if(parentTab != null) parentTab.OnChildDocked(gp);
					else this.ParentSplit.OnChildDocked(this);
					break;
				case GDockState.Floating:
					RECT r = _manager.RectangleToScreen(this.Bounds);
					Wnd.Misc.WindowRectFromClientRect(ref r, Api.WS_POPUP | Api.WS_THICKFRAME, Api.WS_EX_TOOLWINDOW);
					var f = new GFloat(_manager, this);
					f.Bounds = r;

					this.ParentFloat = f;
					if(gp != null) gp.Content.Parent = f;
					else gt.Tabs.ForEach(v => { if(v.ParentFloat == null) { v.ParentFloat = f; v.Content.Parent = f; } });

					f.Show(_manager.TopLevelControl);
					break;
				case GDockState.AutoHide:
					break;
				case GDockState.Hidden:
					break;
				}

				if(state != GDockState.Hidden) {
					if(gp != null) gp.Content.Show();
					else gt.Tabs.ForEach(v => { if(v.IsDocked) v.Content.Show(); });
				}
			}
		}
	}
}
