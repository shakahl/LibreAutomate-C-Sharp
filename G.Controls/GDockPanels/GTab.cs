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
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using Catkeys.Winapi;

namespace G.Controls
{
	partial class GDockPanels
	{
		class GTab :GContentNode
		{
			internal List<GPanel> Tabs;
			internal GPanel ActiveTab;
			//internal int TabButtonStyle;
			int _dockedChildCount; //if 0, hide this; else if 1, draw this like GPanel
			bool _tooSmall;
			bool _onlyIcons;

			internal GTab(_InitData d, XmlElement x, GSplit parentSplit) : base(d.manager, parentSplit)
			{
				d.manager._aTab.Add(this);

				Tabs = new List<GPanel>();
				var xnodes = x.SelectNodes("panel");
				int ati = x.GetAttribute("active").ToInt_(), n = xnodes.Count;
				if(ati < 0 || ati >= n) ati = 0;
				for(int i = 0; i < n; i++) {
					var gp = new GPanel(d, (XmlElement)xnodes[i], parentSplit);
					gp.ParentTab = this;
					Tabs.Add(gp);
					if(i == ati) this.ActiveTab = gp; else gp.Content.Visible = false;
				}
				_dockedChildCount = n;
			}

			internal override string Name { get { return this.ActiveTab.Name; } }

			internal override void Paint(Graphics g)
			{
				Debug.Assert(!this.IsHidden);

				if(_dockedChildCount == 1) {
					base.Paint(g);
					return;
				}

				int childCaptionsWidth = _CalcChildCaptionBounds(g);

				var t = _manager._paintTools;
				bool vert = IsVerticalCaption;
				if(!_tooSmall)
					for(int i = 0; i < Tabs.Count; i++) {
						var gp = Tabs[i];
						if(!gp.IsDocked) continue;
						//bool isLast = i == Tabs.Count - 1;
						bool isActive = gp == this.ActiveTab;

						Brush brushBack, brushText;
						Rectangle r = gp.CaptionBounds;
						int tabWid = vert ? r.Height : r.Width;
						if(isActive) {
							brushBack = t.brushCaptionText;
							brushText = t.brushActiveTabText;
							r.X++; r.Y++;
							if(vert) r.Height -= 2; else r.Width -= 2;
						} else {
							brushBack = (gp == _manager._hilitedTabButton) ? t.brushInactiveTabHighlightBack : t.brushInactiveTabBack;
							brushText = t.brushCaptionText;
						}
						g.FillRectangle(brushBack, r);

						if(_onlyIcons && gp.CaptionImage != null) {
							g.DrawImageUnscaled(gp.CaptionImage, r.Left + 1, r.Top + 1);
						}

						if(!_onlyIcons) {
							string s = gp.Content.Text;
							var tf = vert ? t.txtFormatVert : t.txtFormatHorz;
							//g.DrawString(s, _manager.Font, brushText, r.Left + 1, r.Top + 1, tf);
							g.DrawString(s, _manager.Font, brushText, r, tf);
						}
					}

				//draw own caption (after child captions)
				Rectangle b = this.CaptionBounds;
				int o = childCaptionsWidth;
				if(vert) { b.Height -= o; b.Y += o; } else { b.Width -= o; b.X += o; }
				g.FillRectangle(t.brushCaptionBack, b);
			}

			/// <summary>
			/// Returns child captions width.
			/// </summary>
			int _CalcChildCaptionBounds(Graphics g)
			{
				bool vert = IsVerticalCaption;
				Rectangle b = this.CaptionBounds;
				int capHeight = vert ? b.Width : b.Height;
				int capWidth = (vert ? b.Height : b.Width) - capHeight; //-capHeight reserves some space not for tab buttons
				int i, n = Tabs.Count, buttonWidth = 0, offset = 0;
				int maxButtonWidth = capWidth / _dockedChildCount;
				if(_tooSmall = maxButtonWidth < 4) maxButtonWidth = 0;
				_onlyIcons = maxButtonWidth < capHeight * 2;
				var a = new int[n];
				if(!_tooSmall) {
					for(i = 0; i < n; i++, offset += buttonWidth) {
						var gp = Tabs[i];
						if(!gp.IsDocked || _tooSmall) {
							buttonWidth = 0;
						} else if(_onlyIcons) {
							buttonWidth = maxButtonWidth;
						} else {
							string s = gp.Content.Text;
							var textSize = g.MeasureString(s, _manager.Font, capWidth, _manager._paintTools.txtFormatHorz);
							buttonWidth = (int)textSize.Width + 4;
						}
						a[i] = buttonWidth;
					}

					//trim biggest buttons until all buttons fit in capWidth
					while(offset > capWidth) {
						int max = a.Max() - 1;
						for(i = 0, offset = 0; i < n; offset += a[i++]) if(a[i] > max) a[i] = max;
					}

					a[_IndexOf(this.ActiveTab)] += 2; //border
				}

				for(i = 0, offset = 0; i < n; i++, offset += buttonWidth) {
					buttonWidth = a[i];
					Tabs[i].CaptionBounds = vert
						? new Rectangle(b.Left, b.Top + offset, b.Width, buttonWidth)
						: new Rectangle(b.Left + offset, b.Top, buttonWidth, b.Height);
				}

				return offset;
			}

			internal void UpdateChildrenLayout(Rectangle rContent)
			{
				foreach(var v in Tabs) {
					if(!v.IsDocked) continue;
					v.Bounds = this.Bounds;
					v.CaptionBounds = Rectangle.Empty;
					v.Content.Bounds = rContent;
				}
			}

			int _IndexOf(GPanel gp)
			{
				for(int i = 0; i < Tabs.Count; i++) if(gp == Tabs[i]) return i;
				Debug.Assert(false);
				return -1;
			}

			internal override bool HitTestCaption(Control parent, int x, int y)
			{
				return base.HitTestCaption(parent, x, y) && _dockedChildCount>1;
			}

			internal void OnMouseDownTabButton(GPanel gp, MouseButtons mb)
			{
				if(mb == MouseButtons.Left) _SetActiveTab(gp);
			}

			void _SetActiveTab(GPanel gp)
			{
				if(gp == this.ActiveTab) return;
				this.ActiveTab.Content.Hide();
				this.ActiveTab = gp;
				this.ActiveTab.Content.Show();
				this.InvalidateCaption();
			}

			///// <summary>
			///// Unlike SetDockState(GDockState.Hidden), 
			///// </summary>
			//internal void Hide()
			//{

			//}

			internal void OnChildUndocked(GPanel gp)
			{
				Debug.Assert(_dockedChildCount > 0);
				if(--_dockedChildCount < 1) {
					SetDockState(GDockState.Hidden);
					return;
				}

				if(gp == this.ActiveTab) {
					this.ActiveTab = Tabs.Find(v => v != gp && v.IsDocked);
					this.ActiveTab.Content.Show();
				}
				this.InvalidateCaption();
			}

			internal void OnChildDocked(GPanel gp)
			{
				SetDockState(GDockState.Docked);
				_dockedChildCount++;
				UpdateLayout(this.Bounds);
				_SetActiveTab(gp);
			}
		}
	}
}
