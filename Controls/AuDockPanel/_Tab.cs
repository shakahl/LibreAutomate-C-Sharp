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
using System.Xml.Linq;
using System.Xml;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	public partial class AuDockPanel
	{
		/// <summary>
		/// Contains multiple _Panel.
		/// </summary>
		partial class _Tab : _ContentNode
		{
			internal List<_Panel> Items; //all child panels, including hidden, floating etc
			internal _Panel ActiveItem; //null if _dockedItemCount is 0 or if there is no active (currently impossible)
			int _dockedItemCount; //if 0, this is hidden; else if 1, draws caption like _Panel
			bool _tooSmall; //too small to display tab buttons
			bool _onlyIcons; //display only icons in tab buttons (too small to display text)

			/// <summary>
			/// This ctor is used at startup, when adding from XML.
			/// </summary>
			internal _Tab(AuDockPanel manager, _Split parentSplit, XElement x) : base(manager, parentSplit)
			{
				manager._aTab.Add(this);

				int iAct = x.Attr("active", 0);
				var xPanels = x.Elements("panel");
				this.Items = new List<_Panel>();

				int i = 0;
				foreach(var xx in xPanels) {
					var gp = new _Panel(manager, parentSplit, xx, this);
					this.Items.Add(gp);
					if(gp.IsDocked) {
						_dockedItemCount++;
						if(i == iAct || this.ActiveItem == null) this.ActiveItem = gp; //if iAct invalid, let the first docked panel be active
					}
					i++;
				}

				foreach(var gp in this.Items) {
					if(gp.IsDocked && gp != this.ActiveItem) gp.Content.Visible = false;
				}

				this.InitDockStateFromXML(x);
			}

			internal override void Save(XmlWriter x)
			{
				x.WriteStartElement("tab");

				SaveDockStateToXml(x);

				if(this.ActiveItem != null) {
					int i = this.IndexOf(this.ActiveItem);
					if(i > 0) x.WriteAttributeString("active", i.ToString());
				}

				foreach(var gp in this.Items) {
					gp.Save(x);
				}

				x.WriteEndElement();
			}

			/// <summary>
			/// This ctor is used when a floating _Panel dropped on a docked non-tabbed _Panel caption.
			/// item1 or item2 must be docked, but not both.
			/// </summary>
			internal _Tab(AuDockPanel manager, _Split parentSplit, _Panel item1, _Panel item2) : base(manager, parentSplit)
			{
				manager._aTab.Add(this);

				this.Items = new List<_Panel>() { item1, item2 };
				item1.ParentTab = item2.ParentTab = this;
				Debug.Assert(item1.IsDocked != item2.IsDocked);
				_dockedItemCount = 1;
				this.SetActiveItem(item1.IsDocked ? item1 : item2);
			}

			//internal string Text => this.ActiveItem?.Text;

			public override string ToString()
			{
				return this.AccName;
			}

			internal int DockedItemCount => _dockedItemCount;

			internal bool ShowsTabButtons => _dockedItemCount >= 2;

			internal Rectangle CaptionBoundsExceptButtons {
				get {
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
							//shift button rect 1 pixel towards content, and add 1-pixel border at other edges
							switch(this.CaptionAt) {
							case CaptionEdge.Left: r.X++; r.Y++; r.Height -= 2; break;
							case CaptionEdge.Top: r.Y++; r.X++; r.Width -= 2; break;
							case CaptionEdge.Right: r.X--; r.Y++; r.Height -= 2; break;
							case CaptionEdge.Bottom: r.Y--; r.X++; r.Width -= 2; break;
							}
						} else {
							brushBack = (gp == _manager._hilitedTabButton) ? t.brushActiveTabBack : t.brushInactiveTabBack;

							if(i < n - 1 && this.Items[i + 1] != this.ActiveItem) {
								if(vert) r.Height--; else r.Width--;
							}
						}
						g.FillRectangle(brushBack, r);

						if(_onlyIcons && gp.Image != null) {
							//var z = gp.Image.Size;
							var z = Au.Util.ADpi.SmallIconSize;
							if(z.width > r.Width) z.width = r.Width;
							if(z.height > r.Height) z.height = r.Height;
							var x = (r.Left + r.Right - z.width) / 2;
							var y = (r.Top + r.Bottom - z.height) / 2;
							g.DrawImage(gp.Image, x, y, z.width, z.height);
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
						} else if(_onlyIcons && gp.Image != null) {
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
					v.Bounds = this.Bounds;
					v.CaptionBounds = this.CaptionBounds; //will calc tab button bounds later on Paint, because then Graphics is available, need it to measure strings
					v.CaptionAt = this.CaptionAt;
					v.Content.Bounds = rContent;
				}
			}

			internal int IndexOf(_Panel gp)
			{
				int i = this.Items.IndexOf(gp);
				Debug.Assert(i >= 0);
				return i;
			}

			internal void OnMouseDownTabButton(_Panel gp, MouseButtons mb)
			{
				_AssertIsChild(gp);
				if(!gp.IsDocked) return;
				//if(mb == MouseButtons.Left)
				SetActiveItem(gp, true);
			}

			/// <summary>
			/// Sets ActiveItem, shows its contents, hides previous ActiveItem contents, invalidate caption, calls DockedDocumentPanelActivated event.
			/// </summary>
			/// <param name="gp">Can be null to deactivate all.</param>
			/// <param name="focusControl"></param>
			internal void SetActiveItem(_Panel gp, bool focusControl = false)
			{
				_AssertIsDockedChildOrNull(gp);
				if(focusControl) focusControl = gp.Focusable;
				if(gp != this.ActiveItem) {
					var ai = this.ActiveItem;
					this.ActiveItem = gp;
					gp?.Content.Show();
					var oldCont = ai?.Content;
					if(focusControl) gp.Content?.Focus();
					else if(_manager.FocusControlOnUndockEtc != null && (oldCont?.ContainsFocus ?? false)) _manager.FocusControlOnUndockEtc.Focus();
					oldCont?.Hide();
					this.InvalidateCaption();
				} else if(focusControl) gp.Content?.Focus();
			}

			internal void OnItemUndocked(_Panel gp, out Action postAction)
			{
				postAction = null;
				_AssertIsChild(gp);
				Debug.Assert(_dockedItemCount > 0);

				if(--_dockedItemCount < 1) {
					this.SetActiveItem(null);
					postAction = () => SetDockState(_DockState.Hidden);
					return;
				}

				if(gp == this.ActiveItem) {
					this.SetActiveItem(this.Items.Find(v => v != gp && v.IsDocked));
				}

				//if this is floating, and just 1 other docked item left, dock this. Else would be problems.
				if(_dockedItemCount == 1 && this.IsFloating) {
					postAction = () => SetDockState(_DockState.Docked); //because this func is called by SetDockState, it must execute postAction later
					return;
				}

				this.InvalidateCaption();
			}

			internal void OnItemDocked(_Panel gp, bool setActive = true)
			{
				_AssertIsChild(gp);
				if(this.IsHidden) SetDockState(_DockState.Docked);
				_dockedItemCount++;
				UpdateLayout();
				if(setActive || _dockedItemCount == 1) SetActiveItem(gp);
			}

			/// <summary>
			/// Removes a child panel from Items.
			/// If single item left, moves it to this parent and invalidates/removes this.
			/// </summary>
			internal void OnItemRemoved(_Panel gp)
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
			internal void AddOrReorderItem(_Panel gp, _Panel target, bool after)
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

			bool _IsChild(_Panel gp)
			{
				return this.Items.IndexOf(gp) >= 0;
			}

			[Conditional("DEBUG")]
			void _AssertIsChild(_Panel gp)
			{
				Debug.Assert(_IsChild(gp));
			}

			[Conditional("DEBUG")]
			void _AssertIsDockedChild(_Panel gp)
			{
				Debug.Assert(_IsChild(gp));
				Debug.Assert(gp.IsDocked);
			}

			[Conditional("DEBUG")]
			void _AssertIsDockedChildOrNull(_Panel gp)
			{
				if(gp != null) _AssertIsDockedChild(gp);
			}
		}
	}
}
