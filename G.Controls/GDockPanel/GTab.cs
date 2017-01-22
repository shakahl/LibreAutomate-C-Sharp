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

namespace G.Controls
{
	partial class GDockPanel
	{
		/// <summary>
		/// Contains multiple GPanel.
		/// </summary>
		partial class GTab :GContentNode
		{
			internal List<GPanel> Items; //all child panels, including hidden, floating etc
			internal GPanel ActiveItem; //null if _dockedItemCount is 0 or if there is no active (currently impossible)
			int _dockedItemCount; //if 0, this is hidden; else if 1, draws caption like GPanel
			bool _tooSmall; //too small to display tab buttons
			bool _onlyIcons; //display only icons in tab buttons (too small to display text)

			/// <summary>
			/// This ctor is used at startup, when adding from XML.
			/// </summary>
			internal GTab(GDockPanel manager, GSplit parentSplit, XmlElement x) : base(manager, parentSplit)
			{
				manager._aTab.Add(this);

				int iAct = x.Attribute_("active", 0);
				var xnodes = x.SelectNodes("panel"); int n = xnodes.Count;
				this.Items = new List<GPanel>(n);

				for(int i = 0; i < n; i++) {
					var gp = new GPanel(manager, parentSplit, xnodes[i] as XmlElement);
					gp.ParentTab = this;
					this.Items.Add(gp);
					if(gp.IsDocked) {
						_dockedItemCount++;
						if(i == iAct || this.ActiveItem == null) this.ActiveItem = gp; //if iAct invalid, let the first docked panel be active
					}
				}

				foreach(var gp in this.Items) {
					if(gp.IsDocked && gp != this.ActiveItem) gp.Content.Visible = false;
				}
			}

			/// <summary>
			/// This ctor is used when a floating GPanel dropped on a docked non-tabbed GPanel caption.
			/// item1 or item2 must be docked, but not both.
			/// </summary>
			internal GTab(GDockPanel manager, GSplit parentSplit, GPanel item1, GPanel item2) : base(manager, parentSplit)
			{
				manager._aTab.Add(this);

				this.Items = new List<GPanel>() { item1, item2 };
				item1.ParentTab = item2.ParentTab = this;
				Debug.Assert(item1.IsDocked != item2.IsDocked);
				_dockedItemCount = 1;
				this.SetActiveItem(item1.IsDocked ? item1 : item2);
			}

			internal override string Text { get { return this.ActiveItem?.Text; } }

			internal int DockedItemCount { get { return _dockedItemCount; } }

			internal bool ShowsTabButtons { get { return _dockedItemCount >= 2; } }

			internal Rectangle CaptionBoundsExceptButtons
			{
				get
				{
					var b = this.CaptionBounds;
					int o = _buttonsWidth;
					if(this.IsVerticalCaption) { b.Height -= o; b.Y += o; } else { b.Width -= o; b.X += o; }
					return b;
				}
			}
			int _buttonsWidth;

			internal override void Paint(Graphics g)
			{
				Debug.Assert(!this.IsHidden);

				if(!ShowsTabButtons) {
					//info: currently cannot be _dockedItemCount==0, because then is hidden.

					Rectangle b = this.CaptionBounds;
					if(_dockedItemCount == 1) this.ActiveItem.CaptionBounds = b;
					_buttonsWidth = this.IsVerticalCaption ? b.Height : b.Width;

					if(_dockedItemCount > 0) this.ActiveItem.Paint(g);
					//else g.FillRectangle(_manager._paintTools.brushCaptionBack, this.CaptionBounds);
					return;
				}

				_buttonsWidth = _CalcButtonsBounds(g);

				var t = _manager._paintTools;
				bool vert = this.IsVerticalCaption;
				if(!_tooSmall)
					for(int i = 0, n = this.Items.Count; i < n; i++) {
						var gp = this.Items[i];
						if(!gp.IsDocked) continue;
						bool isActive = gp == this.ActiveItem;

						Brush brushBack;
						Rectangle r = gp.CaptionBounds; //button only
						int tabWid = vert ? r.Height : r.Width;
						if(isActive) {
							brushBack = t.brushActiveTabBack;
							r.X++; r.Y++;
							if(vert) r.Height -= 2; else r.Width -= 2;
						} else {
							brushBack = (gp == _manager._hilitedTabButton) ? t.brushActiveTabBack : t.brushInactiveTabBack;

							if(i < n - 1 && this.Items[i + 1] != this.ActiveItem) {
								if(vert) r.Height--; else r.Width--;
							}
						}
						g.FillRectangle(brushBack, r);

						if(_onlyIcons && gp.ImageIndex >= 0) {
							var img = _manager._imageList.ImageSize;
							var x = (r.Left + r.Right - img.Width) / 2;
							var y = (r.Top + r.Bottom - img.Height) / 2;
							//g.DrawImageUnscaled(, x, y);
							_manager._imageList.Draw(g, x, y, gp.ImageIndex);
							//never mind: should clip
						} else {
							string s = gp.Text;
							var tf = vert ? t.txtFormatVert : t.txtFormatHorz;
							if(vert) r.Inflate(0, -4); else r.Inflate(-4, 0);
							g.DrawString(s, _manager.Font, t.brushCaptionText, r, tf);
						}
					}

				//draw own caption (after buttons)
				g.FillRectangle(t.brushCaptionBack, this.CaptionBoundsExceptButtons);
			}

			/// <summary>
			/// Returns buttons width.
			/// </summary>
			int _CalcButtonsBounds(Graphics g)
			{
				Debug.Assert(ShowsTabButtons);

				Rectangle b = this.CaptionBounds;
				bool vert = IsVerticalCaption;
				int capHeight = vert ? b.Width : b.Height;
				int capWidth = (vert ? b.Height : b.Width) - capHeight; //-capHeight reserves some space not for tab buttons
				var tabs = this.Items.FindAll(v => v.IsDocked);
				int i, n = tabs.Count, buttonWidth = 0, offset = 0, iActive = -1;
				int maxButtonWidth = capWidth / n;
				if(_tooSmall = maxButtonWidth < 4) maxButtonWidth = 0;
				_onlyIcons = maxButtonWidth < capHeight * 2.5;
				var a = new int[n];
				if(!_tooSmall) {
					for(i = 0; i < n; i++, offset += buttonWidth) {
						var gp = tabs[i];
						if(_tooSmall) {
							buttonWidth = 0;
						} else if(_onlyIcons && gp.ImageIndex >= 0) {
							buttonWidth = maxButtonWidth;
						} else {
							string s = gp.Text;
							var z = g.MeasureString(s, _manager.Font, capWidth, _manager._paintTools.txtFormatHorz); //fast
							buttonWidth = (int)z.Width + 10;
						}
						a[i] = buttonWidth;
						if(gp == this.ActiveItem) iActive = i;
					}

					//trim biggest buttons until all buttons fit in capWidth
					while(offset > capWidth) {
						int max = a.Max() - 1;
						for(i = 0, offset = 0; i < n; offset += a[i++]) if(a[i] > max) a[i] = max;
					}

					if(iActive >= 0) a[iActive] += 2; //border
				}

				for(i = 0, offset = 0; i < n; i++, offset += buttonWidth) {
					buttonWidth = a[i];
					tabs[i].CaptionBounds = vert
						? new Rectangle(b.Left, b.Top + offset, b.Width, buttonWidth)
						: new Rectangle(b.Left + offset, b.Top, buttonWidth, b.Height);
				}

				return offset;
			}

			internal void UpdateItemsLayout(Rectangle rContent)
			{
				foreach(var v in this.Items) {
					if(!v.IsDocked) continue;
					v.IsVerticalCaption = this.IsVerticalCaption;
					v.Bounds = this.Bounds;
					v.CaptionBounds = this.CaptionBounds; //will calc tab button bounds later on Paint, because then Graphics is available, need it to measure strings
					v.Content.Bounds = rContent;
				}
			}

			internal int IndexOf(GPanel gp)
			{
				int i = this.Items.IndexOf(gp);
				Debug.Assert(i >= 0);
				return i;
			}

			internal void OnMouseDownTabButton(GPanel gp, MouseButtons mb)
			{
				_AssertIsChild(gp);
				if(!gp.IsDocked) return;
				//if(mb == MouseButtons.Left)
				SetActiveItem(gp);
			}

			/// <summary>
			/// Sets ActiveItem, shows its contents, hides previous ActiveItem contents, invalidate caption, calls DockedDocumentPanelActivated event.
			/// </summary>
			/// <param name="gp">Can be null to deactivate all.</param>
			internal void SetActiveItem(GPanel gp)
			{
				_AssertIsDockedChildOrNull(gp);
				if(gp == this.ActiveItem) return;
				this.ActiveItem?.Content.Hide();
				this.ActiveItem = gp;
				this.ActiveItem?.Content.Show();
				this.InvalidateCaption();
			}

			internal void OnItemUndocked(GPanel gp, out Action postAction)
			{
				postAction = null;
				_AssertIsChild(gp);
				Debug.Assert(_dockedItemCount > 0);

				if(--_dockedItemCount < 1) {
					this.SetActiveItem(null);
					postAction = () => SetDockState(GDockState.Hidden);
					return;
				}

				if(gp == this.ActiveItem) {
					this.SetActiveItem(this.Items.Find(v => v != gp && v.IsDocked));
				}

				//if this is floating, and just 1 other docked item left, dock this. Else would be problems.
				if(_dockedItemCount == 1 && this.IsFloating) {
					postAction = () => SetDockState(GDockState.Docked); //because this func is called by SetDockState, it must execute postAction later
					return;
				}

				this.InvalidateCaption();
			}

			internal void OnItemDocked(GPanel gp, bool setActive=true)
			{
				_AssertIsChild(gp);
				if(this.IsHidden) SetDockState(GDockState.Docked);
				_dockedItemCount++;
				UpdateLayout(this.Bounds);
				if(setActive || _dockedItemCount == 1) SetActiveItem(gp);
			}

			/// <summary>
			/// Removes a child panel from Items.
			/// If single item left, moves it to this parent and invalidates/removes this.
			/// </summary>
			internal void OnItemRemoved(GPanel gp)
			{
				_AssertIsChild(gp);
				Debug.Assert(this.Items.Count > 1);
				this.Items.Remove(gp);
				if(this.Items.Count > 1) return;

				var gpLast = this.Items[0];
				gpLast.ParentTab = null;
				this.ParentSplit.ReplaceChild(this, gpLast);
#if DEBUG
				this.Items = null; //to catch invalid usage of this after calling this function
#endif
				_manager._aTab.Remove(this);
			}

			/// <summary>
			/// If gp is child of this, moves it to the place before or after target (a child of this).
			/// Else just inserts gp there.
			/// If target is null, adds to the end (does nothing if gp is child).
			/// Does not update layout.
			/// </summary>
			internal void AddOrReorderItem(GPanel gp, GPanel target, bool after)
			{
				bool gpIsChild = _IsChild(gp);
				if(target == null) {
					if(!gpIsChild) this.Items.Add(gp);
				} else {
					_AssertIsChild(target);
					int iTo = this.IndexOf(target);
					if(after) iTo++;
					if(gpIsChild) {
						int iFrom = this.IndexOf(gp);
						if(iFrom < iTo) iTo--;
						this.Items.RemoveAt(iFrom);
					}
					this.Items.Insert(iTo, gp);
				}
				gp.ParentTab = this;
			}

			bool _IsChild(GPanel gp)
			{
				return this.Items.IndexOf(gp) >= 0;
			}

			[Conditional("DEBUG")]
			void _AssertIsChild(GPanel gp)
			{
				Debug.Assert(_IsChild(gp));
			}

			[Conditional("DEBUG")]
			void _AssertIsDockedChild(GPanel gp)
			{
				Debug.Assert(_IsChild(gp));
				Debug.Assert(gp.IsDocked);
			}

			[Conditional("DEBUG")]
			void _AssertIsDockedChildOrNull(GPanel gp)
			{
				if(gp != null) _AssertIsDockedChild(gp);
			}
		}
	}
}
