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
//using System.Linq;
using System.Xml.Linq;
using System.Xml;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Controls
{
	public partial class AuDockPanel
	{
		/// <summary>
		/// Contains 2 _Node (_Panel, _Tab, _Split or _DummyNode) and a splitter.
		/// </summary>
		partial class _Split :_Node
		{
			internal _Node Child1, Child2; //_Panel or _Split or _Tab
			internal Rectangle SplitterBounds; //valid only when both children are docked
			internal bool IsVerticalSplit;
			internal int SplitterWidth;
			int _dockedChildCount; //if 0, this is hidden; else if 1, splitter is hidden and docked child bounds = this bounds
			bool _isFraction, _isWidth1;
			float _fraction; //used if _isFraction
			int _width; //used if !_isFraction; if _isWidth1, it is Child1 width, else Child2 with

			/// <summary>
			/// This ctor is used at startup, when adding from XML.
			/// </summary>
			internal _Split(AuDockPanel manager, _Split parentSplit, XElement x) : base(manager, parentSplit)
			{
				manager._aSplit.Add(this);

				if(!x.HasAttr("hor")) IsVerticalSplit = true;
				int k = x.Attr("splitter", -1); if(k < 0 || k > 20) k = _splitterWidth;
				this.SplitterWidth = k;

				//SHOULDDO: use DPI-dependent units, not pixels. Especially if form size depends on DPI.
				if(!(_isFraction = x.Attr(out _fraction, "f")) && !(_isWidth1 = x.Attr(out _width, "w1"))) _width = x.Attr("w2", 1);

				foreach(var xe in x.Elements()) {
					_Node gn = null;
					switch(xe.Name.LocalName) {
					case "panel":
						gn = new _Panel(manager, this, xe);
						break;
					case "split":
						gn = new _Split(manager, this, xe);
						break;
					case "tab":
						gn = new _Tab(manager, this, xe);
						break;
					case "dummy":
						gn = new _DummyNode(_manager, this);
						break;
					default: continue;
					}

					if(gn.IsDocked) _dockedChildCount++;

					if(Child1 == null) Child1 = gn; else { Child2 = gn; break; }
				}
				if(Child2 == null) throw new Exception();

				if(_dockedChildCount == 0) this.DockState = _DockState.Hidden;
			}

			internal override void Save(XmlWriter x)
			{
				x.WriteStartElement("split");
				if(!IsVerticalSplit) x.WriteAttributeString("hor", "");

				if(_isFraction) x.WriteAttributeString("f", _fraction.ToStringInvariant());
				else if(_isWidth1) x.WriteAttributeString("w1", _width.ToString());
				else x.WriteAttributeString("w2", _width.ToString());

				if(this.SplitterWidth != _splitterWidth) x.WriteAttributeString("splitter", this.SplitterWidth.ToString());

				for(int i = 0; i < 2; i++) {
					var gn = i == 0 ? Child1 : Child2;
					gn.Save(x);
				}

				x.WriteEndElement();
			}

			/// <summary>
			/// This ctor is used when a floating _ContentNode dropped on a docked _ContentNode.
			/// child1 or child2 must be docked, but not both.
			/// </summary>
			internal _Split(AuDockPanel manager, _Split parentSplit, _Node child1, _Node child2, bool isVertical) : base(manager, parentSplit)
			{
				manager._aSplit.Add(this);

				Child1 = child1;
				Child2 = child2;
				child1.ParentSplit = child2.ParentSplit = this;
				Debug.Assert(child1.IsDocked != child2.IsDocked);
				_dockedChildCount = 1;
				IsVerticalSplit = isVertical;
				SplitterWidth = _splitterWidth;
				_isFraction = true;
				_fraction = 0.5F;
			}

			/// <summary>
			/// Puts gn sibling in parent split of this, in place of this.
			/// Makes this invalid and removes from _aSplit.
			/// Does not update layout.
			/// </summary>
			/// <param name="gn">One of children. The caller is removing it from this.</param>
			internal void OnChildRemoved(_Node gn)
			{
				_AssertIsChild(gn);
				Debug.Assert(!gn.IsDocked);
				var gnOther = gn == Child1 ? Child2 : Child1;
				if(this == _manager._firstSplit) {
					var gs = gnOther as _Split;
					if(gs == null) {
						//gnOther is _Tab, because the user moved all panels to a single _Tab. We cannot delete this (root) _Split.
						var gd = new _DummyNode(_manager, this);
						if(gn == Child1) Child1 = gd; else Child2 = gd;
						//never mind: when saving layout to XML, should remove this dummy node if can. Not necessary, because impossible to create multiple dummy nodes.
						return;
					}
					gs.ParentSplit = null;
					_manager._firstSplit = gs;
				} else {
					this.ParentSplit.ReplaceChild(this, gnOther);
				}
#if DEBUG
				this.Child1 = this.Child2 = null; //to catch invalid usage of this after calling this function
#endif
				_manager._aSplit.Remove(this);
			}

			/// <summary>
			/// Replaces child gnOld with gnNew.
			/// Sets gnNew.ParentSplit = this.
			/// </summary>
			internal void ReplaceChild(_Node gnOld, _Node gnNew)
			{
				_AssertIsChild(gnOld);
				if(gnOld == Child1) Child1 = gnNew; else Child2 = gnNew;
				gnNew.ParentSplit = this;
			}

			/// <summary>
			/// Changes IsVerticalSplit, Child1 and Child2 if need.
			/// Does not update layout.
			/// </summary>
			internal void RepositionChild(_Node gn, bool vertically, bool afterSibling)
			{
				_AssertIsChild(gn);
				this.IsVerticalSplit = vertically;
				var gnOther = gn == Child1 ? Child2 : Child1;
				if(afterSibling) { Child1 = gnOther; Child2 = gn; } else { Child2 = gnOther; Child1 = gn; }
			}

			internal override void UpdateLayout(Rectangle r)
			{
				Debug.Assert(this.IsDocked || this.ParentSplit == null);
				this.Bounds = r;
				if(_dockedChildCount == 2) {
					int w1, w2, wFull = IsVerticalSplit ? r.Width : r.Height, wNoSplitter = wFull - this.SplitterWidth;
					if(wNoSplitter < 0) wNoSplitter = 0;
					bool swap = false;
					if(_isFraction) {
						w1 = (int)(wFull * _fraction);
					} else {
						w1 = _width;
						swap = !_isWidth1;
					}
					if(w1 > wNoSplitter) w1 = wNoSplitter; else if(w1 < 0) w1 = 0;
					w2 = wNoSplitter - w1;
					if(swap) { int t = w1; w1 = w2; w2 = t; }

					//apply minimal child width or height, because may contain toolbars
					int min1 = _MinimalChildWidthOrHeight(Child1), min2 = _MinimalChildWidthOrHeight(Child2);
					//Print(min1, min2);
					if(swap) {
						if(min1 > w1) { w1 = Math.Min(min1, wFull); w2 = wNoSplitter - w1; }
						if(min2 > w2) { w2 = Math.Min(min2, wFull); w1 = wNoSplitter - w2; } //min2 has priority
					} else {
						if(min2 > w2) { w2 = Math.Min(min2, wFull); w1 = wNoSplitter - w2; }
						if(min1 > w1) { w1 = Math.Min(min1, wFull); w2 = wNoSplitter - w1; } //min1 has priority
					}
					if(w1 < 0) w1 = 0; if(w2 < 0) w2 = 0;
					int wSplitter = Math.Min(this.SplitterWidth, wFull - w1 - w2);

					Rectangle r1, r2;
					if(IsVerticalSplit) {
						r1 = new Rectangle(r.Left, r.Top, w1, r.Height);
						r2 = new Rectangle(r.Right - w2, r.Top, w2, r.Height);
						SplitterBounds = new Rectangle(r1.Right, r1.Top, wSplitter, r1.Height);
					} else {
						r1 = new Rectangle(r.Left, r.Top, r.Width, w1);
						r2 = new Rectangle(r.Left, r.Bottom - w2, r.Width, w2);
						SplitterBounds = new Rectangle(r1.Left, r1.Bottom, r1.Width, wSplitter);
					}

					Child1.UpdateLayout(r1);
					Child2.UpdateLayout(r2);
				} else if(Child1.IsDocked) {
					Child1.UpdateLayout(r);
				} else if(Child2.IsDocked) {
					Child2.UpdateLayout(r);
				}
			}

			internal override int MinimalWidth
			{
				get
				{
					if(_dockedChildCount == 0) return 0;
					int v1 = 0, v2 = 0;
					if(Child1.IsDocked) v1 = Child1.MinimalWidth;
					if(Child2.IsDocked) v2 = Child2.MinimalWidth;
					if(!this.IsVerticalSplit) return Math.Max(v1, v2);
					int R = v1 + v2; if(R != 0) R += this.SplitterWidth;
					return R;
				}
			}

			internal override int MinimalHeight
			{
				get
				{
					if(_dockedChildCount == 0) return 0;
					int v1 = 0, v2 = 0;
					if(Child1.IsDocked) v1 = Child1.MinimalHeight;
					if(Child2.IsDocked) v2 = Child2.MinimalHeight;
					if(this.IsVerticalSplit) return Math.Max(v1, v2);
					int R = v1 + v2; if(R != 0) R += this.SplitterWidth;
					return R;
				}
			}

			int _MinimalChildWidthOrHeight(_Node gn)
			{
				if(!gn.IsDocked) return 0;
				return this.IsVerticalSplit ? gn.MinimalWidth : gn.MinimalHeight;
			}

			/// <summary>
			/// Called on mouse left button down on splitter.
			/// Returns when the drag operation ends.
			/// </summary>
			internal void DragSplitter()
			{
				Debug.Assert(_dockedChildCount == 2);
				_manager.Cursor = this.IsVerticalSplit ? Cursors.VSplit : Cursors.HSplit;
				bool vert = this.IsVerticalSplit;
				var p = _manager.MouseClientXY();
				var offset = vert ? (p.x - this.SplitterBounds.X) : (p.y - this.SplitterBounds.Y);
				Au.Util.DragDrop.SimpleDragDrop(_manager, MButtons.Left, d =>
				{
					if(d.Msg.message != Api.WM_MOUSEMOVE) return;
					p = _manager.MouseClientXY();
					var b = this.Bounds;
					int xy, loBound, hiBound, widHei;
					if(vert) {
						xy = p.x; loBound = b.Left; hiBound = b.Right; widHei = b.Width;
					} else {
						xy = p.y; loBound = b.Top; hiBound = b.Bottom; widHei = b.Height;
					}
					xy -= offset;
					int min1 = _MinimalChildWidthOrHeight(Child1), min2 = _MinimalChildWidthOrHeight(Child2) + this.SplitterWidth;
					int lim1 = loBound + min1, lim2 = hiBound - min2;
					if(xy < lim1 && xy > lim2) return;
					if(xy < lim1) xy = lim1; else if(xy > lim2) xy = lim2;
					int w1 = xy - loBound, w2 = hiBound - xy - this.SplitterWidth;
					if(_isFraction) _fraction = (float)w1 / widHei; else _width = _isWidth1 ? w1 : w2;

					UpdateLayout();
					this.Invalidate();

					//info: currently we invalidate the rectangles to draw in OnPaint. Could instead draw now, but it flickers because background often is displayed before we draw text; need BufferedGraphics, like Control does internally on WM_PAINT if has a double-buffer style.
				});
				_manager.ResetCursor();
			}

			/// <summary>
			/// Called from AuDockPanel.OnPaint, for its _firstSplit.
			/// Paints all descendants.
			/// </summary>
			internal override void Paint(Graphics g)
			{
				if(IsSplitterVisible) {
					var t = _manager._paintTools;
					g.FillRectangle(t.brushSplitter, this.SplitterBounds);
				}
				if(Child1.IsDocked) Child1.Paint(g);
				if(Child2.IsDocked) Child2.Paint(g);
			}

			internal void Invalidate()
			{
				Debug.Assert(!this.IsHidden);
				_manager.Invalidate(this.Bounds);
			}

			internal bool HitTestSplitter(int x, int y)
			{
				if(!IsSplitterVisible) return false;
				return SplitterBounds.Contains(x, y);
			}

			internal bool IsSplitterVisible => _dockedChildCount == 2;

			internal void OnChildUndocked(_Node gn)
			{
				_AssertIsChild(gn);
				Debug.Assert(_dockedChildCount > 0);

				if(--_dockedChildCount == 0) {
					this.DockState = _DockState.Hidden;
					if(this.ParentSplit != null) this.ParentSplit.OnChildUndocked(this);
					else _manager.Invalidate(this.Bounds); //not this.Invalidate(); because now already Hidden
				} else {
					_manager._UpdateLayout(true); //not this.UpdateLayout() because may need to apply minimal layouts
				}
			}

			internal void OnChildDocked(_Node gn)
			{
				_AssertIsChild(gn);
				Debug.Assert(_dockedChildCount < 2);

				if(++_dockedChildCount == 1) {
					this.DockState = _DockState.Docked;
					this.ParentSplit?.OnChildDocked(this);
				}

				_manager._UpdateLayout(true); //not this.UpdateLayout() because may need to apply minimal layouts
			}

			/// <summary>
			/// Sets a child (of this, or of an ancestor if need) to have fixed width or height.
			/// Used for context menu.
			/// </summary>
			internal void SetChildFixedSize(_Node gn, bool useWidth, bool fixedSize)
			{
				//gn = _GetChildNotTabbedPanel(gn);
				_AssertIsChild(gn);

				bool vert = IsVerticalSplit;
				if(vert != useWidth || _dockedChildCount < 2) {
					this.ParentSplit?.SetChildFixedSize(this, useWidth, fixedSize);
					return;
				}

				_isFraction = !fixedSize;
				_isWidth1 = gn == Child1;
				int w, w1, w2;
				if(vert) { w = this.Bounds.Width; w1 = Child1.Bounds.Width; w2 = Child2.Bounds.Width; } else { w = this.Bounds.Height; w1 = Child1.Bounds.Height; w2 = Child2.Bounds.Height; }
				if(w < 1) return;
				if(_isFraction) {
					_fraction = (float)w1 / w;
				} else {
					_width = _isWidth1 ? w1 : w2;
				}
			}

			/// <summary>
			/// Gets whether a child (of this, or of an ancestor if need) has fixed width or height.
			/// Used for context menu.
			/// </summary>
			internal bool IsChildFixedSize(_Node gn, bool useWidth)
			{
				//gn = _GetChildNotTabbedPanel(gn);
				_AssertIsChild(gn);

				if(IsVerticalSplit != useWidth || _dockedChildCount < 2) {
					var gs = this.ParentSplit;
					if(gs == null) return false;
					return gs.IsChildFixedSize(this, useWidth);
				}

				if(_isFraction) return false;
				return (gn == Child1) == _isWidth1;
			}

			internal void ShowContextMenu(Point p)
			{
				var m = new AMenu();
				using(m.Submenu("Width")) {
					for(int i = 1, k = this.SplitterWidth; i <= 10; i++)
						m.Add(i.ToString(), o =>
						{
							this.SplitterWidth = o.Item.Text.ToInt();
							_manager._UpdateLayout(true); //not this.UpdateLayout() because may need to apply minimal layouts
						}).Checked = k == 0;
				}

				m.Show(_manager, p.X, p.Y);
			}

			[Conditional("DEBUG")]
			void _AssertIsChild(_Node gn)
			{
				Debug.Assert(gn == Child1 || gn == Child2);
			}
		}
	}
}
