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
using System.Xml.Linq;
using System.Xml;

using Au;
using Au.Types;
using static Au.AStatic;

namespace Au.Controls
{
	public partial class AuDockPanel
	{
		class _PainTools
		{
			internal Brush brushSplitter, brushCaptionText, brushCaptionBack, brushInactiveTabBack, brushActiveTabBack;
			internal StringFormat txtFormatHorz, txtFormatVert;
			bool _inited;

			internal _PainTools(AuDockPanel manager)
			{
				if(!_inited) {
					brushSplitter = new SolidBrush(manager.BackColor);
					brushCaptionBack = Brushes.LightSteelBlue;
					brushInactiveTabBack = Brushes.Gainsboro;
					brushCaptionText = Brushes.Black;
					brushActiveTabBack = Brushes.WhiteSmoke;
					txtFormatHorz = new StringFormat(StringFormatFlags.NoWrap);
					txtFormatVert = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical);
					txtFormatHorz.LineAlignment = txtFormatVert.LineAlignment = StringAlignment.Center;
					//txtFormatVert.Trimming = StringTrimming.EllipsisCharacter; //.NET bug with vertical: displays a rectangle or nothing, instead of ...
					//txtFormatHorz.Trimming = StringTrimming.EllipsisCharacter; //no. The string is short, and the ... just hides useful characters.
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

		enum _DockState
		{
			//note: don't reorder. Default must be Docked; also saved in XML.

			Docked, //visible as part of the main window or a child _Panel of a floating _Tab.
			Floating, //visible as direct child of a _Float
			Hidden, //hidden. Always child of main window, even if previously was floating.
			AutoHide, //not implemented
			LastVisible = 100 //used just for "Show hidden panel" command, to pass to SetDockState, which then uses SavedVisibleDockState instead

			//_Split can be only Docked or Hidden (when both children non-docked), and only in main window (cannot float etc).
		};

		//used only when docking, not as a state
		enum _DockHow
		{
			TabBefore, TabAfter, SplitLeft, SplitRight, SplitAbove, SplitBelow
		}

		/// <summary>
		/// Base of _Split, _ContentNode (_Panel and _Tab) and _DummyNode.
		/// </summary>
		abstract partial class _Node
		{
			protected readonly AuDockPanel _manager;
			internal AuDockPanel Manager => _manager;

			internal _Split ParentSplit; //null if new panel added in this app version
			internal Rectangle Bounds; //in current parent Control client area
			internal _DockState DockState;

			internal _Node(AuDockPanel manager, _Split parentSplit)
			{
				_manager = manager;
				ParentSplit = parentSplit;
				//manager._nodes.Add(this);
			}

			/// <summary>
			/// Returns true if is docked in main window or in a floating tab.
			/// </summary>
			/// <seealso cref="_ContentNode.IsDockedOn"/>
			internal bool IsDocked => DockState == _DockState.Docked;
			internal bool IsHidden => DockState == _DockState.Hidden;

			internal virtual Control ParentControl => _manager;
			internal virtual void Paint(Graphics g) { }
			internal virtual void UpdateLayout(Rectangle r) { }
			internal void UpdateLayout() { UpdateLayout(this.Bounds); }
			internal virtual Rectangle RectangleInScreen => _manager.RectangleToScreen(this.Bounds);
			internal virtual int MinimalWidth => 0;
			internal virtual int MinimalHeight => 0;
			internal virtual void Save(XmlWriter x) { }
		}

		class _DummyNode : _Node
		{
			internal _DummyNode(AuDockPanel manager, _Split parentSplit) : base(manager, parentSplit)
			{
				this.DockState = _DockState.Hidden;
			}

			internal override void Save(XmlWriter x)
			{
				x.WriteStartElement("dummy");
				x.WriteEndElement();
			}
		}


		/// <summary>
		/// Base of _Panel and _Tab.
		/// </summary>
		abstract class _ContentNode : _Node
		{
			internal Rectangle CaptionBounds; //in current parent client area. If _Tab, it is whole caption area (includes child panel buttons); else if _Tab child panel and there are more visible siblings, only its button; else whole caption area.
			internal Rectangle SavedFloatingBounds; //when docked etc, contains bounds when was floating, to restore when floating again
			protected Control _parentControl;
			internal override Control ParentControl => _parentControl; //_manager or a _Float
			internal _DockState SavedVisibleDockState; //when hidden, in what state to show

			internal _ContentNode(AuDockPanel manager, _Split parentSplit) : base(manager, parentSplit)
			{
				_parentControl = manager;
			}

			internal bool IsDockedOn(Control parent) { return this.IsDocked && this.ParentControl == parent; }

			internal bool IsFloating => DockState == _DockState.Floating;
			//internal bool IsAutoHide => DockState == _DockState.AutoHide;
			//internal bool IsFloatingOrAutoHide => IsFloating || IsAutoHide;

			internal virtual bool IsTabbedPanel => false;

			internal override Rectangle RectangleInScreen => this.ParentControl.RectangleToScreen(this.Bounds);

			internal override void UpdateLayout(Rectangle r)
			{
				Debug.Assert(!this.IsHidden);
				this.Bounds = r;

				RECT rCont = r, rCap = rCont;
				var gt = this as _Tab;
				var gp = this as _Panel;
				bool isTB = gp != null && gp.HasToolbar;
				int capThick = isTB ? _splitterWidth * 2 : _manager._CaptionHeight;
				bool noCaption = gp != null && gp.HasDocument;

				if(noCaption) {
					this.CaptionBounds = new Rectangle(r.Left, r.Top, 0, 0);
				} else {
					switch(this.CaptionAt) {
					case CaptionEdge.Left: rCap.right = (rCont.left += capThick); break;
					case CaptionEdge.Top: rCap.bottom = (rCont.top += capThick); break;
					case CaptionEdge.Right: rCap.left = (rCont.right -= capThick); break;
					case CaptionEdge.Bottom: rCap.top = (rCont.bottom -= capThick); break;
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
				this.ParentControl.Invalidate(this.Bounds);
			}

			internal virtual void InvalidateCaption()
			{
				if(this.IsHidden) return;
				RECT u = this.CaptionBounds;
				if(u.IsEmpty) return;

				//include content border
				switch(this.CaptionAt) {
				case CaptionEdge.Left: u.right++; break;
				case CaptionEdge.Top: u.bottom++; break;
				case CaptionEdge.Right: u.left--; break;
				case CaptionEdge.Bottom: u.top--; break;
				}

				this.ParentControl.Invalidate(u);
			}

			/// <summary>
			/// Returns true if x/y is in caption and this is visible and is a child of parent.
			/// For tabbed _Panel returns true only if x/y is in its tab button, unless there are no visible siblings.
			/// For _Tab returns true even if on a tab button.
			/// </summary>
			internal virtual bool HitTestCaption(Control parent, int x, int y)
			{
				if(this.IsHidden || parent != this.ParentControl) return false;
				return CaptionBounds.Contains(x, y);
			}

			internal void ToggleDockedFloating()
			{
				if(this.IsDocked) SetDockState(_DockState.Floating);
				else if(this.IsFloating) SetDockState(_DockState.Docked);
			}

			internal void ShowContextMenu(Point p)
			{
				var gp = this as _Panel;
				bool isTab = this is _Tab gt;
				var state = this.DockState;
				var m = new AMenu();
				m.Control.Text = "Menu";

				//dock state
				m.Add("Float\tD-click, drag", o => this.SetDockState(_DockState.Floating)).Enabled = state != _DockState.Floating;
				m.Add("Dock    \tD-click, Alt+drag", o => this.SetDockState(_DockState.Docked)).Enabled = state != _DockState.Docked;
				//menu.Add("Auto Hide", o => this.SetDockState(_DockState.AutoHide)).Enabled = state != _DockState.AutoHide && !isTab; //not implemented
				m["Hide\tM-click"] = o => this.SetDockState(_DockState.Hidden);

				m.Separator();
				using(m.Submenu("Show Panel")) _manager.ZAddShowPanelsToMenu(m.LastMenuItem.DropDown, false);
				using(m.Submenu("Show Toolbar")) _manager.ZAddShowPanelsToMenu(m.LastMenuItem.DropDown, true);

				m.Separator();
				var k = (!this.IsTabbedPanel || this.IsFloating) ? this : gp.ParentTab;
				//caption edge
				using(m.Submenu("Caption At")) {
					m["Top"] = o => k._SetCaptionEdge(CaptionEdge.Top); if(k.CaptionAt == CaptionEdge.Top) m.LastMenuItem.Checked = true;
					m["Bottom"] = o => k._SetCaptionEdge(CaptionEdge.Bottom); if(k.CaptionAt == CaptionEdge.Bottom) m.LastMenuItem.Checked = true;
					m["Left"] = o => k._SetCaptionEdge(CaptionEdge.Left); if(k.CaptionAt == CaptionEdge.Left) m.LastMenuItem.Checked = true;
					m["Right"] = o => k._SetCaptionEdge(CaptionEdge.Right); if(k.CaptionAt == CaptionEdge.Right) m.LastMenuItem.Checked = true;
				}
				//fixed width/height
				if(this.IsDockedOn(_manager)) {
					_AddFixedSize(k.ParentSplit, k);
					void _AddFixedSize(_Split gs, _Node gn)
					{
						if(gs.IsSplitterVisible) {
							bool fixedSize = gs.IsChildFixedSize(gn);
							m.Add(gs.IsVerticalSplit ? "Fixed Width" : "Fixed Height", o => gs.SetChildFixedSize(gn, !fixedSize)).Checked = fixedSize;
						}
						var gs2 = gs.ParentSplit;
						if(gs2 != null) {
							using(m.Submenu("Container")) {
								m.LastMenuItem.DropDown.Opened += (unu, sed) => {
									var osd = new AOsdRect { Rect = _manager.RectangleToScreen(gs.Bounds), Color = 0x00c000 };
									osd.Show();
									ATimer.After(1000, _ => osd.Dispose());
								};
								_AddFixedSize(gs2, gs);
							}
						}
					}
				}

				//test
				//m.Separator();
				//m["test"] = o =>
				//{
				//};

				//custom
				_manager.ZPanelContextMenu?.Invoke(new ZContextMenuEventArgs(gp, m));

				m.Show(this.ParentControl, p.X, p.Y);
			}

			//internal void ShowContextMenu(Point p)
			//{
			//	var gp = this as _Panel;
			//	bool isTab = this is _Tab gt;
			//	var state = this.DockState;
			//	var m = new AMenu();
			//	m.Control.Text = "Menu";

			//	//dock state
			//	m.Add("Float\tD-click, drag", o => this.SetDockState(_DockState.Floating)).Enabled = state != _DockState.Floating;
			//	m.Add("Dock    \tD-click, Alt+drag", o => this.SetDockState(_DockState.Docked)).Enabled = state != _DockState.Docked;
			//	//menu.Add("Auto Hide", o => this.SetDockState(_DockState.AutoHide)).Enabled = state != _DockState.AutoHide && !isTab; //not implemented
			//	m["Hide\tM-click"] = o => this.SetDockState(_DockState.Hidden);

			//	m.Separator();
			//	using(m.Submenu("Show Panel")) _manager.AddShowPanelsToMenu(m.LastMenuItem.DropDown, false);
			//	using(m.Submenu("Show Toolbar")) _manager.AddShowPanelsToMenu(m.LastMenuItem.DropDown, true);

			//	m.Separator();
			//	var k = (!this.IsTabbedPanel || this.IsFloating) ? this : gp.ParentTab;
			//	if(this.IsDockedOn(_manager)) {
			//		//fixed width/height
			//		var gs = k.ParentSplit;
			//		//using(m.Submenu("Fixed Size")) {
			//		bool fixedWidth = gs.IsChildFixedSize(k, true);
			//		m.Add("Fixed Width", o => gs.SetChildFixedSize(k, true, !fixedWidth)).Checked = fixedWidth;
			//		bool fixedHeight = gs.IsChildFixedSize(k, false);
			//		m.Add("Fixed Height", o => gs.SetChildFixedSize(k, false, !fixedHeight)).Checked = fixedHeight;
			//		//}
			//	}
			//	//caption edge
			//	using(m.Submenu("Caption At")) {
			//		m["Top"] = o => k._SetCaptionEdge(CaptionEdge.Top); if(k.CaptionAt == CaptionEdge.Top) m.LastMenuItem.Checked = true;
			//		m["Bottom"] = o => k._SetCaptionEdge(CaptionEdge.Bottom); if(k.CaptionAt == CaptionEdge.Bottom) m.LastMenuItem.Checked = true;
			//		m["Left"] = o => k._SetCaptionEdge(CaptionEdge.Left); if(k.CaptionAt == CaptionEdge.Left) m.LastMenuItem.Checked = true;
			//		m["Right"] = o => k._SetCaptionEdge(CaptionEdge.Right); if(k.CaptionAt == CaptionEdge.Right) m.LastMenuItem.Checked = true;
			//	}

			//	//test
			//	//m.Separator();
			//	//m["test"] = o =>
			//	//{
			//	//};

			//	//custom
			//	_manager.PanelContextMenu?.Invoke(new DPContextMenuEventArgs(gp, m));

			//	m.Show(this.ParentControl, p.X, p.Y);
			//}

			/// <summary>
			/// Shows in the most recent visible state. Activates tab group panel.
			/// </summary>
			internal void Show(bool focus = false)
			{
				this.SetDockState(_DockState.LastVisible);
				if(focus) {
					var gt = this as _Tab;
					var gp = gt?.ActiveItem ?? (this as _Panel);
					gp?.Content?.Focus();
				}
			}

			/// <summary>
			/// Hides, does not close.
			/// </summary>
			internal void Hide()
			{
				this.SetDockState(_DockState.Hidden);
			}

			internal void SetDockState(_DockState state, bool onStartDrag = false)
			{
				//Print(this, Name);
				var gp = this as _Panel;

				if(state == _DockState.LastVisible) {
					if(!this.IsHidden) {
						if(this.IsTabbedPanel) {
							gp.ParentTab.SetDockState(_DockState.LastVisible);
							if(this.IsDocked) gp.ParentTab.SetActiveItem(gp);
						}
						if(this.ParentControl is _Float gf) ((AWnd)gf).EnsureInScreen();
						return;
					}
					state = this.SavedVisibleDockState;
				}

				var prevState = this.DockState;
				if(state == prevState) return;

				if(this.ParentSplit == null && state == _DockState.Docked) { //new panel
					ADialog.ShowInfo("How to dock floating panels", "Alt+drag and drop.", owner: _manager);
					return;
				}

				bool isTab = gp == null;
				_Tab gt = null, gtParent = null;
				if(isTab) gt = this as _Tab; else gtParent = gp.ParentTab;

				this.DockState = state;

				//get RECT for floating now, because later this.ParentControl will change and even may be destroyed
				RECT rect = new RECT();
				if(state == _DockState.Floating) {
					if(!onStartDrag && !SavedFloatingBounds.IsEmptyRect()) {
						if(SavedFloatingBounds.X == int.MinValue) { //specified only width and height
							var mp = AMouse.XY;
							rect = new RECT(mp.x - 15, mp.y - 15, SavedFloatingBounds.Width, SavedFloatingBounds.Height);
						} else rect = SavedFloatingBounds;
						rect.EnsureInScreen();
					} else if(this.ParentSplit != null) {
						rect = this.RectangleInScreen;
						AWnd.More.WindowRectFromClientRect(ref rect, WS.POPUP | WS.THICKFRAME, WS2.TOOLWINDOW);
					} else { //new panel, empty bounds
						var mp = AMouse.XY;
						rect = new RECT(mp.x - 15, mp.y - 15, 300, 150);
						rect.EnsureInScreen();
					}
				}

				var panels = isTab ? gt.Items.FindAll(v => v.IsDocked) : new List<_Panel>(1) { gp };

				//(isTab ? gt.ActiveItem : gp)?.Content.Hide();
				var gtp = isTab ? gt.ActiveItem : gp;
				if(gtp != null) {
					if(state != _DockState.Docked && _manager.ZFocusControlOnUndockEtc != null && gtp.Content.ContainsFocus) _manager.ZFocusControlOnUndockEtc.Focus();
					gtp.Content.Hide();
				}

				Action postAction = null;

				switch(prevState) {
				case _DockState.Docked:
					if(gtParent != null) gtParent.OnItemUndocked(gp, out postAction);
					else this.ParentSplit.OnChildUndocked(this);

					_manager.Invalidate(this.Bounds, true); //some controls don't redraw properly, eg ToolStripTextBox
					break;
				case _DockState.Floating:
					//case _DockState.AutoHide:
					var f = this.ParentControl as _Float;
					var parent = (gtParent != null) ? gtParent.ParentControl : _manager;
					_parentControl = parent;
					foreach(var v in panels) _ChangeParent(v, parent);
					if(prevState == _DockState.Floating) this.SavedFloatingBounds = f.Bounds;
					f.Close();
					break;
				}

				switch(state) {
				case _DockState.Docked:
					if(gtParent != null) gtParent.OnItemDocked(gp);
					else this.ParentSplit.OnChildDocked(this);
					break;
				case _DockState.Floating:
					var f = new _Float(_manager, this);
					this._parentControl = f;
					foreach(var v in panels) _ChangeParent(v, f);

					f.Bounds = rect;
					f.Show(_manager.TopLevelControl);
					break;
				//case _DockState.AutoHide:
				//	break;
				case _DockState.Hidden:
					this.SavedVisibleDockState = prevState;
					break;
				}

				if(state != _DockState.Hidden) (isTab ? gt.ActiveItem : gp)?.Content.Show();

				postAction?.Invoke();
				//_manager.Invalidate(true);

				if(prevState != _DockState.Hidden) _manager._OnMouseLeave_Common(this.ParentControl); //calls _UnhiliteTabButton and _HideTooltip
			}

			void _ChangeParent(_Panel panel, Control parent)
			{
				panel._parentControl = parent;
				var content = panel.Content;
				content.Parent = parent;

				//restore tooltips
				if((content is UserControl uc) && (uc.GetType().GetField("components", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(uc) is IContainer co)) {
					foreach(var tt in co.Components.OfType<ToolTip>()) {
						_Controls(uc);
						void _Controls(Control parent)
						{
							foreach(Control c in parent.Controls) {
								var s = tt.GetToolTip(c);
								if(!Empty(s)) {
									//Print($"<>{c}, <c blue>{s}<>");
									tt.SetToolTip(c, null);
									tt.SetToolTip(c, s);
								}
								_Controls(c);
							}
						}
					}
				}
			}

			/// <summary>
			/// Docks this _Panel or _Tab in an existing or new _Tab or _Split.
			/// If need, creates new _Tab or _Split in gcTarget place and adds gcTarget and this to it. Else reorders if need.
			/// This can be a new _Panel, with null ParentSplit and ParentTab, dock state not Docked.
			/// </summary>
			/// <param name="gcTarget">New sibling _Panel (side can be any) or sibling _Tab (when side is SplitX) or parent _Tab (when side is TabX).</param>
			/// <param name="side">Specifies whether to add on a _Tab or _Split, and at which side of gcTarget.</param>
			internal void DockBy(_ContentNode gcTarget, _DockHow side)
			{
				var gpThis = this as _Panel;
				var gtThisParent = gpThis?.ParentTab;
				var gsThisParent = this.ParentSplit;
				var gsTargetParent = gcTarget.ParentSplit;

				if(side == _DockHow.TabBefore || side == _DockHow.TabAfter) {
					var gpTarget = gcTarget as _Panel;
					_Tab gtTargetParent = (gpTarget != null) ? gpTarget.ParentTab : gcTarget as _Tab;
					bool after = side == _DockHow.TabAfter;
					bool sameTargetTab = false;

					if(gtTargetParent != null) {
						gtTargetParent.AddOrReorderItem(gpThis, gpTarget, after);
						if(gtThisParent == gtTargetParent) sameTargetTab = true;
					} else {
						var gtNew = new _Tab(_manager, gsTargetParent, after ? gpTarget : gpThis, after ? gpThis : gpTarget);
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
					if(gcTarget.IsTabbedPanel) gcTarget = (gcTarget as _Panel).ParentTab;
					bool after = side == _DockHow.SplitRight || side == _DockHow.SplitBelow;
					bool verticalSplit = side == _DockHow.SplitLeft || side == _DockHow.SplitRight;

					if(gsTargetParent == gsThisParent && gtThisParent == null) {
						//just change vertical/horizontal or/and swap with sibling
						gsThisParent.RepositionChild(this, verticalSplit, after);
					} else {
						var gsNew = new _Split(_manager, gsTargetParent, after ? gcTarget : this, after ? this : gcTarget, verticalSplit);
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

				SetDockState(_DockState.Docked, false);
			}

			internal void OnMouseLeftDown()
			{
				POINT p = AMouse.XY;
				if(Api.DragDetect((AWnd)this.ParentControl, p)) {
					if(!this.IsFloating) this.SetDockState(_DockState.Floating, true);
					var d = (this.ParentControl as _Float)?.Drag(p);
					if(d != null) DockBy(d.gc, d.side);
				}
			}

			internal void InitDockStateFromXML(XElement x)
			{
				Enum.TryParse(x.Attr("state"), out this.DockState);
				bool hide = x.HasAttr("hide"), floating = this.DockState == _DockState.Floating;
				if(hide || floating) {
					this.SavedVisibleDockState = this.DockState;
					this.DockState = _DockState.Hidden;
					switch(this) {
					case _Panel gp:
						gp.Content.Visible = false;
						break;
					case _Tab gt:
						foreach(var v in gt.Items) v.Content.Visible = false;
						break;
					}
					if(!hide) {
						PaintEventHandler eh = null;
						eh = (object sender, PaintEventArgs e) => {
							_manager.Paint -= eh;
							//SetDockState(_DockState.Floating);
							ATimer.After(200, _ => SetDockState(_DockState.Floating));
						};
						_manager.Paint += eh;
					}
				}
				this.SavedFloatingBounds = _RectFromString(x.Attr("rectFloating"));
				if(!this.IsTabbedPanel || floating) Enum.TryParse(x.Attr("captionAt"), out this.CaptionAt);
			}

			internal void SaveDockStateToXml(XmlWriter x)
			{
				switch(this.DockState) {
				case _DockState.Hidden:
					x.WriteAttributeString("hide", "");
					x.WriteAttributeString("state", this.SavedVisibleDockState.ToString());
					break;
				case _DockState.Docked:
					break;
				default:
					x.WriteAttributeString("state", this.DockState.ToString());
					break;
				}

				var r = (this.DockState == _DockState.Floating) ? this.ParentControl.Bounds : this.SavedFloatingBounds;
				if(!r.IsEmpty) x.WriteAttributeString("rectFloating", _RectToString(r));

				if(this.CaptionAt != default && (!this.IsTabbedPanel || this.IsFloating))
					x.WriteAttributeString("captionAt", this.CaptionAt.ToString());
			}

			static string _RectToString(Rectangle r)
			{
				if(r.X == int.MinValue) return $"{r.Width} {r.Height}";
				return $"{r.X} {r.Y} {r.Width} {r.Height}";
			}

			static Rectangle _RectFromString(string s)
			{
				Rectangle r = default;
				if(s != null) {
					r.X = s.ToInt(0, out int i);
					r.Y = s.ToInt(i, out i);
					r.Width = s.ToInt(i, out i);
					if(i > 0) r.Height = s.ToInt(i);
					else r = new Rectangle(int.MinValue, 0, r.X, r.Y);
					//If saved 4 values, it is x y width height. If 2 values, it is width height, and we'll calc x y when used.
				}
				return r;
			}

			internal enum CaptionEdge { Left, Right, Top, Bottom }
			internal CaptionEdge CaptionAt;
			internal bool IsVerticalCaption { get { return CaptionAt <= CaptionEdge.Right; } }

			void _SetCaptionEdge(CaptionEdge edge)
			{
				CaptionAt = edge;
				this.UpdateLayout();
				this.Invalidate();
				//make sure that floating panel caption is in screen, else cannot move it until restarting this app
				if(this.ParentControl is _Float gf) ((AWnd)gf).EnsureInScreen();
			}
		}
	}
}
