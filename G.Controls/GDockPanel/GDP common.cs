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
using System.Xml.Linq;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
	partial class GDockPanel
	{
		class _PainTools
		{
			internal Brush brushSplitter, brushCaptionText, brushCaptionBack, brushInactiveTabBack, brushActiveTabBack;
			internal StringFormat txtFormatHorz, txtFormatVert;
			bool _inited;

			internal _PainTools(GDockPanel manager)
			{
				if(!_inited) {
					brushSplitter = new SolidBrush(manager.BackColor);
					brushCaptionBack = Brushes.LightSteelBlue;
					brushInactiveTabBack = Brushes.Gainsboro;
					brushCaptionText = Brushes.Black;
					brushActiveTabBack = Brushes.WhiteSmoke;
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
					txtFormatHorz.Dispose();
					txtFormatVert.Dispose();
				}
			}
		}

		internal enum GDockState
		{
			//note: don't reorder. Default must be Docked; also saved in XML.

			Docked, //visible as part of the main window or a child GPanel of a floating GTab.
			Floating, //visible as direct child of a GFloat
			Hidden, //hidden. Always child of main window, even if previously was floating.
			AutoHide, //not implemented
			LastVisible = 100 //used just for "Show hidden panel" command, to pass to SetDockState, which then uses SavedVisibleDockState instead

			//GSplit can be only Docked or Hidden (when both children non-docked), and only in main window (cannot float etc).
		};

		//used only when docking, not as a state
		internal enum GDockHow
		{
			TabBefore, TabAfter, SplitLeft, SplitRight, SplitAbove, SplitBelow
		}

		/// <summary>
		/// Base of GSplit, GContentNode (GPanel and GTab) and GDummyNode.
		/// </summary>
		abstract partial class GNode
		{
			protected readonly GDockPanel _manager;
			internal GDockPanel Manager { get => _manager; }

			internal GSplit ParentSplit; //null if new panel added in this app version
			internal Rectangle Bounds; //in current parent Control client area
			internal GDockState DockState;

			internal GNode(GDockPanel manager, GSplit parentSplit)
			{
				_manager = manager;
				ParentSplit = parentSplit;
				//manager._nodes.Add(this);
			}

			/// <summary>
			/// Returns true if is docked in main window or in a floating tab.
			/// <seealso cref="GContentNode.IsDockedOn"/>.
			/// </summary>
			internal bool IsDocked { get => DockState == GDockState.Docked; }
			internal bool IsHidden { get => DockState == GDockState.Hidden; }

			internal virtual Control ParentControl { get => _manager; }
			internal virtual void Paint(Graphics g) { }
			internal virtual void UpdateLayout(Rectangle r) { }
			internal void UpdateLayout() { UpdateLayout(this.Bounds); }
			internal virtual Rectangle RectangleInScreen { get => _manager.RectangleToScreen(this.Bounds); }
			internal virtual int MinimalWidth { get => 0; }
			internal virtual int MinimalHeight { get => 0; }
			internal virtual void Save(XmlWriter x) { }
		}

		class GDummyNode :GNode
		{
			internal GDummyNode(GDockPanel manager, GSplit parentSplit) : base(manager, parentSplit)
			{
				this.DockState = GDockState.Hidden;
			}

			internal override void Save(XmlWriter x)
			{
				x.WriteStartElement("dummy");
				x.WriteEndElement();
			}
		}


		/// <summary>
		/// Base of GPanel and GTab.
		/// </summary>
		abstract class GContentNode :GNode
		{
			internal Rectangle CaptionBounds; //in current parent client area. If GTab, it is whole caption area (includes child panel buttons); else if GTab child panel and there are more visible siblings, only its button; else whole caption area.
			internal Rectangle SavedFloatingBounds; //when docked etc, contains bounds when was floating, to restore when floating again
			protected Control _parentControl;
			internal override Control ParentControl { get => _parentControl; } //_manager or a GFloat
			internal GDockState SavedVisibleDockState; //when hidden, in what state to show

			internal GContentNode(GDockPanel manager, GSplit parentSplit) : base(manager, parentSplit)
			{
				_parentControl = manager;
			}

			internal bool IsDockedOn(Control parent) { return this.IsDocked && this.ParentControl == parent; }

			internal bool IsFloating { get => DockState == GDockState.Floating; }
			//internal bool IsAutoHide { get => DockState == GDockState.AutoHide; }
			//internal bool IsFloatingOrAutoHide { get => IsFloating || IsAutoHide; }

			internal virtual bool IsTabbedPanel { get => false; }

			internal override Rectangle RectangleInScreen { get => this.ParentControl.RectangleToScreen(this.Bounds); }

			internal override void UpdateLayout(Rectangle r)
			{
				Debug.Assert(!this.IsHidden);
				this.Bounds = r;

				RECT rCont = r, rCap = rCont;
				var gt = this as GTab;
				var gp = this as GPanel;
				bool isTB = gp != null && gp.HasToolbar;
				int capThick = isTB ? _splitterWidth * 2 : _manager._CaptionHeight;
				bool noCaption = gp != null && gp.HasDocument;

				if(noCaption) {
					this.CaptionBounds = new Rectangle(r.Left, r.Top, 0, 0);
				} else {
					switch(this.CaptionAt) {
					case GCaptionEdge.Left: rCap.right = (rCont.left += capThick); break;
					case GCaptionEdge.Top: rCap.bottom = (rCont.top += capThick); break;
					case GCaptionEdge.Right: rCap.left = (rCont.right -= capThick); break;
					case GCaptionEdge.Bottom: rCap.top = (rCont.bottom -= capThick); break;
					}
					this.CaptionBounds = rCap;
				}
				if(!isTB) {
					//add border around content
					rCont.Inflate(-1, -1);
					if(rCont.right < rCont.left) rCont.right = rCont.left;
					if(rCont.bottom < rCont.top) rCont.bottom = rCont.top;
				}

				if(gt != null) {
					gt.UpdateItemsLayout(rCont);
				} else {
					gp.Content.Bounds = rCont;
					gp.OnSizeChanged(rCont.Width, rCont.Height);
				}

				this.InvalidateCaption();
			}

			internal void Invalidate()
			{
				if(this.IsHidden) return;
				this.ParentControl.Invalidate(this.Bounds, false);
			}

			internal virtual void InvalidateCaption()
			{
				if(this.IsHidden) return;
				RECT u = this.CaptionBounds;
				if(u.IsEmpty) return;

				//include content border
				switch(this.CaptionAt) {
				case GCaptionEdge.Left: u.right++; break;
				case GCaptionEdge.Top: u.bottom++; break;
				case GCaptionEdge.Right: u.left--; break;
				case GCaptionEdge.Bottom: u.top--; break;
				}

				this.ParentControl.Invalidate(u, false);
			}

			/// <summary>
			/// Returns true if x/y is in caption and this is visible and is a child of parent.
			/// For tabbed GPanel returns true only if x/y is in its tab button, unless there are no visible siblings.
			/// For GTab returns true even if on a tab button.
			/// </summary>
			internal virtual bool HitTestCaption(Control parent, int x, int y)
			{
				if(this.IsHidden || parent != this.ParentControl) return false;
				return CaptionBounds.Contains(x, y);
			}

			internal void ToggleDockedFloating()
			{
				if(this.IsDocked) SetDockState(GDockState.Floating);
				else if(this.IsFloating) SetDockState(GDockState.Docked);
			}

			internal void ShowContextMenu(Point p)
			{
				var gp = this as GPanel;
				var gt = this as GTab;
				bool isTab = gt != null;
				var state = this.DockState;
				var m = new CatMenu();
				m.CMS.Text = "Menu";

				//dock state
				m.Add("Float\tD-click, drag", o => this.SetDockState(GDockState.Floating)).Enabled = state != GDockState.Floating;
				m.Add("Dock    \tD-click, Alt+drag", o => this.SetDockState(GDockState.Docked)).Enabled = state != GDockState.Docked;
				//menu.Add("Auto Hide", o => this.SetDockState(GDockState.AutoHide)).Enabled = state != GDockState.AutoHide && !isTab; //not implemented
				m["Hide\tM-click"] = o => this.SetDockState(GDockState.Hidden);

				m.Separator();
				using(m.Submenu("Show Panel")) _manager.AddShowPanelsToMenu(m.LastMenuItem.DropDown, false);
				using(m.Submenu("Show Toolbar")) _manager.AddShowPanelsToMenu(m.LastMenuItem.DropDown, true);

				m.Separator();
				var k = (!this.IsTabbedPanel || this.IsFloating) ? this : gp.ParentTab;
				if(this.IsDockedOn(_manager)) {
					//fixed width/height
					var gs = k.ParentSplit;
					//using(m.Submenu("Fixed Size")) {
					bool fixedWidth = gs.IsChildFixedSize(k, true);
					m.Add("Fixed Width", o => gs.SetChildFixedSize(k, true, !fixedWidth)).Checked = fixedWidth;
					bool fixedHeight = gs.IsChildFixedSize(k, false);
					m.Add("Fixed Height", o => gs.SetChildFixedSize(k, false, !fixedHeight)).Checked = fixedHeight;
					//}
				}
				//caption edge
				using(m.Submenu("Caption At")) {
					m["Top"] = o => k._SetCaptionEdge(GCaptionEdge.Top); if(k.CaptionAt == GCaptionEdge.Top) m.LastMenuItem.Checked = true;
					m["Bottom"] = o => k._SetCaptionEdge(GCaptionEdge.Bottom); if(k.CaptionAt == GCaptionEdge.Bottom) m.LastMenuItem.Checked = true;
					m["Left"] = o => k._SetCaptionEdge(GCaptionEdge.Left); if(k.CaptionAt == GCaptionEdge.Left) m.LastMenuItem.Checked = true;
					m["Right"] = o => k._SetCaptionEdge(GCaptionEdge.Right); if(k.CaptionAt == GCaptionEdge.Right) m.LastMenuItem.Checked = true;
				}

				//test
				//m.Separator();
				//m["test"] = o =>
				//{
				//};

				//custom
				_manager.PanelContextMenu?.Invoke(new GDockPanelContextMenuEventArgs(gp, m));

				m.Show(this.ParentControl, p.X, p.Y);
			}

			/// <summary>
			/// Shows in the most recent visible state. Activates tab group panel.
			/// </summary>
			internal void Show()
			{
				this.SetDockState(GDockState.LastVisible);
			}

			/// <summary>
			/// Hides, does not close.
			/// </summary>
			internal void Hide()
			{
				this.SetDockState(GDockState.Hidden);
			}

			internal void SetDockState(GDockState state, bool onStartDrag = false)
			{
				//PrintList(this, Name);
				var gp = this as GPanel;

				if(state == GDockState.LastVisible) {
					if(!this.IsHidden) {
						if(this.IsTabbedPanel) {
							gp.ParentTab.SetDockState(GDockState.LastVisible);
							if(this.IsDocked) gp.ParentTab.SetActiveItem(gp);
						}
						if(this.ParentControl is GFloat gf) ((Wnd)gf).EnsureInScreen();
						return;
					}
					state = this.SavedVisibleDockState;
				}

				var prevState = this.DockState;
				if(state == prevState) return;

				if(this.ParentSplit == null && state == GDockState.Docked) { //new panel
					TaskDialog.ShowInfo("How to dock floating panels", "Alt+drag and drop.", owner: _manager.ParentForm);
					return;
				}

				bool isTab = gp == null;
				GTab gt = null, gtParent = null;
				if(isTab) gt = this as GTab; else gtParent = gp.ParentTab;

				this.DockState = state;

				//get RECT for floating now, because later this.ParentControl will change and even may be destroyed
				RECT rect = new RECT();
				if(state == GDockState.Floating) {
					if(!onStartDrag && !SavedFloatingBounds.IsEmpty_()) {
						rect = SavedFloatingBounds;
						rect.EnsureInScreen();
					} else if(this.ParentSplit != null) {
						rect = this.RectangleInScreen;
						Wnd.Misc.WindowRectFromClientRect(ref rect, Native.WS_POPUP | Native.WS_THICKFRAME, Native.WS_EX_TOOLWINDOW);
					} else { //new panel, empty bounds
						var mp = Mouse.XY;
						rect = new RECT(mp.x - 15, mp.y - 15, 300, 150, true);
						rect.EnsureInScreen();
					}
				}

				var panels = isTab ? gt.Items.FindAll(v => v.IsDocked) : new List<GPanel>(1) { gp };

				(isTab ? gt.ActiveItem : gp)?.Content.Hide();

				Action postAction = null;

				switch(prevState) {
				case GDockState.Docked:
					if(gtParent != null) gtParent.OnItemUndocked(gp, out postAction);
					else this.ParentSplit.OnChildUndocked(this);

					_manager.Invalidate(this.Bounds, true); //some controls don't redraw properly, eg ToolStripTextBox
					break;
				case GDockState.Floating:
					//case GDockState.AutoHide:
					var f = this.ParentControl as GFloat;
					var parent = (gtParent != null) ? gtParent.ParentControl : _manager;
					_parentControl = parent;
					foreach(var v in panels) {
						v._parentControl = parent;
						v.Content.Parent = parent;
					}
					if(prevState == GDockState.Floating) this.SavedFloatingBounds = f.Bounds;
					f.Close();
					break;
				}

				switch(state) {
				case GDockState.Docked:
					if(gtParent != null) gtParent.OnItemDocked(gp);
					else this.ParentSplit.OnChildDocked(this);
					break;
				case GDockState.Floating:
					var f = new GFloat(_manager, this);
					this._parentControl = f;
					foreach(var v in panels) {
						v._parentControl = f;
						v.Content.Parent = f;
					}

					f.Bounds = rect;
					f.Show(_manager.ParentForm);
					break;
				//case GDockState.AutoHide:
				//	break;
				case GDockState.Hidden:
					this.SavedVisibleDockState = prevState;
					break;
				}

				if(state != GDockState.Hidden) (isTab ? gt.ActiveItem : gp)?.Content.Show();

				postAction?.Invoke();
				//_manager.Invalidate(true);

				if(prevState != GDockState.Hidden) _manager._OnMouseLeave_Common(this.ParentControl); //calls _UnhiliteTabButton and _HideTooltip
			}

			/// <summary>
			/// Docks this GPanel or GTab in an existing or new GTab or GSplit.
			/// If need, creates new GTab or GSplit in gcTarget place and adds gcTarget and this to it. Else reorders if need.
			/// This can be a new GPanel, with null ParentSplit and ParentTab, dock state not Docked.
			/// </summary>
			/// <param name="gcTarget">New sibling GPanel (side can be any) or sibling GTab (when side is SplitX) or parent GTab (when side is TabX).</param>
			/// <param name="side">Specifies whether to add on a GTab or GSplit, and at which side of gcTarget.</param>
			internal void DockBy(GContentNode gcTarget, GDockHow side)
			{
				var gpThis = this as GPanel;
				var gtThisParent = gpThis?.ParentTab;
				var gsThisParent = this.ParentSplit;
				var gsTargetParent = gcTarget.ParentSplit;

				if(side == GDockHow.TabBefore || side == GDockHow.TabAfter) {
					var gpTarget = gcTarget as GPanel;
					GTab gtTargetParent = (gpTarget != null) ? gpTarget.ParentTab : gcTarget as GTab;
					bool after = side == GDockHow.TabAfter;
					bool sameTargetTab = false;

					if(gtTargetParent != null) {
						gtTargetParent.AddOrReorderItem(gpThis, gpTarget, after);
						if(gtThisParent == gtTargetParent) sameTargetTab = true;
					} else {
						var gtNew = new GTab(_manager, gsTargetParent, after ? gpTarget : gpThis, after ? gpThis : gpTarget);
						gsTargetParent.ReplaceChild(gpTarget, gtNew);
						gtNew.Bounds = gpTarget.Bounds;
						gtNew.CaptionAt = gpTarget.CaptionAt;
					}

					if(!sameTargetTab) {
						this.ParentSplit = gsTargetParent;
						if(gtThisParent != null) {
							gtThisParent.OnItemRemoved(gpThis);
						} else {
							gsThisParent?.OnChildRemoved(this);
						}
					}
				} else {
					if(gcTarget.IsTabbedPanel) gcTarget = (gcTarget as GPanel).ParentTab;
					bool after = side == GDockHow.SplitRight || side == GDockHow.SplitBelow;
					bool verticalSplit = side == GDockHow.SplitLeft || side == GDockHow.SplitRight;

					if(gsTargetParent == gsThisParent && gtThisParent == null) {
						//just change vertical/horizontal or/and swap with sibling
						gsThisParent.RepositionChild(this, verticalSplit, after);
					} else {
						var gsNew = new GSplit(_manager, gsTargetParent, after ? gcTarget : this, after ? this : gcTarget, verticalSplit);
						gsTargetParent.ReplaceChild(gcTarget, gsNew);
						gsNew.Bounds = gcTarget.Bounds;

						if(gtThisParent != null) {
							gpThis.ParentTab = null;
							gtThisParent.OnItemRemoved(gpThis);
						} else {
							gsThisParent?.OnChildRemoved(this);
						}
					}
				}

				SetDockState(GDockState.Docked, false);
			}

			internal void OnMouseLeftDown()
			{
				POINT p = Mouse.XY;
				if(Api.DragDetect((Wnd)this.ParentControl, p)) {
					if(!this.IsFloating) this.SetDockState(GDockState.Floating, true);
					var d = (this.ParentControl as GFloat)?.Drag(p);
					if(d != null) DockBy(d.gc, d.side);
				}
			}

			internal void InitDockStateFromXML(XElement x)
			{
				Enum.TryParse(x.Attribute_("state"), out this.DockState);
				bool hide = x.HasAttribute_("hide"), floating = this.DockState == GDockState.Floating;
				if(hide || floating) {
					this.SavedVisibleDockState = this.DockState;
					this.DockState = GDockState.Hidden;
					switch(this) {
					case GPanel gp:
						gp.Content.Visible = false;
						break;
					case GTab gt:
						foreach(var v in gt.Items) v.Content.Visible = false;
						break;
					}
					if(!hide) {
						PaintEventHandler eh = null;
						eh = (object sender, PaintEventArgs e) =>
							{
								_manager.Paint -= eh;
								//Print(1);
								//SetDockState(GDockState.Floating);
								Time.SetTimer(200, true, tt => SetDockState(GDockState.Floating));
							};
						_manager.Paint += eh;
					}
				}
				this.SavedFloatingBounds = _RectFromString(x.Attribute_("rectFloating"));
				if(!this.IsTabbedPanel || floating) Enum.TryParse(x.Attribute_("captionAt"), out this.CaptionAt);
			}

			internal void SaveDockStateToXml(XmlWriter x)
			{
				switch(this.DockState) {
				case GDockState.Hidden:
					x.WriteAttributeString("hide", "");
					x.WriteAttributeString("state", this.SavedVisibleDockState.ToString());
					break;
				case GDockState.Docked:
					break;
				default:
					x.WriteAttributeString("state", this.DockState.ToString());
					break;
				}

				var r = (this.DockState == GDockState.Floating) ? this.ParentControl.Bounds : this.SavedFloatingBounds;
				if(!r.IsEmpty) x.WriteAttributeString("rectFloating", _RectToString(r));

				if(this.CaptionAt != default(GCaptionEdge) && (!this.IsTabbedPanel || this.IsFloating))
					x.WriteAttributeString("captionAt", this.CaptionAt.ToString());
			}

			static string _RectToString(Rectangle r)
			{
				return $"{r.X} {r.Y} {r.Width} {r.Height}";
			}

			static Rectangle _RectFromString(string s)
			{
				var r = default(Rectangle);
				if(s != null) {
					r.X = s.ToInt32_(0, out int i);
					r.Y = s.ToInt32_(i, out i);
					r.Width = s.ToInt32_(i, out i);
					r.Height = s.ToInt32_(i, out i);
				}
				return r;
			}

			internal enum GCaptionEdge { Left, Right, Top, Bottom }
			internal GCaptionEdge CaptionAt;
			internal bool IsVerticalCaption { get { return CaptionAt <= GCaptionEdge.Right; } }

			void _SetCaptionEdge(GCaptionEdge edge)
			{
				CaptionAt = edge;
				this.UpdateLayout();
				this.Invalidate();
				//make sure that floating panel caption is in screen, else cannot move it until restarting this app
				if(this.ParentControl is GFloat gf) ((Wnd)gf).EnsureInScreen();
			}
		}
	}
}
