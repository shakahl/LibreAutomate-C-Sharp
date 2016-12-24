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
		class GSplit :GNode
		{
			internal GNode Child1, Child2; //GPanel (or GDocument) or GSplit or GTab
			Rectangle _splitterBounds; //valid only when both children are docked
			internal bool IsVerticalSplit;
			int _dockedChildCount; //if 0, hide this (as a child of the parent GSplit); else if 1, hide splitter and set docked child bounds = this bounds
			bool _isFraction, _isWidth1;
			float _fraction; //used if _isFraction
			int _width; //used if !_isFraction; if _isWidth1, it is Child1 width, else Child2 with

			internal GSplit(_InitData d, XmlElement x, GSplit parentSplit) : base(d.manager, parentSplit)
			{
				d.manager._aSplit.Add(this);

				if(x.GetAttribute("vh") == "v") IsVerticalSplit = true;
				if(_isFraction = x.HasAttribute("f")) _fraction = x.GetAttribute("f").ToFloat_(true);
				else if(_isWidth1 = x.HasAttribute("w1")) _width = x.GetAttribute("w1").ToInt_();
				else _width = x.GetAttribute("w2").ToInt_();

				foreach(XmlNode xn in x) {
					var xe = xn as XmlElement; if(xe == null) continue;
					GNode gn = null;
					switch(xe.Name) {
					case "split":
						gn = new GSplit(d, xe, this);
						break;
					case "tab":
						gn = new GTab(d, xe, this);
						break;
					case "panel":
						gn = new GPanel(d, xe, this);
						break;
					case "document":
						gn = new GDocument(d, xe, this);
						break;
					default: continue;
					}

					if(Child1 == null) Child1 = gn; else { Child2 = gn; break; }
				}
				if(Child2 == null) throw new Exception();

				_dockedChildCount = 2;
			}

			internal override void UpdateLayout(Rectangle r)
			{
				Debug.Assert(this.IsDocked || this.ParentSplit==null);
				this.Bounds = r;
				if(_dockedChildCount == 2) {
					int w1, w2, wFull = IsVerticalSplit ? r.Width : r.Height, wNoSplitter = wFull - _splitterWidth;
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

					Rectangle r1, r2;
					if(IsVerticalSplit) {
						r1 = new Rectangle(r.Left, r.Top, w1, r.Height);
						r2 = new Rectangle(r.Right - w2, r.Top, w2, r.Height);
						_splitterBounds = new Rectangle(r1.Right, r1.Top, _splitterWidth, r1.Height);
					} else {
						r1 = new Rectangle(r.Left, r.Top, r.Width, w1);
						r2 = new Rectangle(r.Left, r.Bottom - w2, r.Width, w2);
						_splitterBounds = new Rectangle(r1.Left, r1.Bottom, r1.Width, _splitterWidth);
					}

					Child1.UpdateLayout(r1);
					Child2.UpdateLayout(r2);
				} else if(Child1.IsDocked) {
					Child1.UpdateLayout(r);
				} else if(Child2.IsDocked) {
					Child2.UpdateLayout(r);
				}
			}

			internal void UpdateLayout()
			{
				UpdateLayout(this.Bounds);
			}

			/// <summary>
			/// Called on mouse left button down on splitter.
			/// Returns when the drag operation ends.
			/// </summary>
			internal void DragSplitter()
			{
				Debug.Assert(_dockedChildCount == 2);
				bool vert = this.IsVerticalSplit;
				Wnd w = ((Wnd)_manager);
				var p = w.MouseClientXY;
				var offset = vert ? (p.x - this._splitterBounds.X) : (p.y - this._splitterBounds.Y);
				ControlUtil.SimpleDragDrop(w, MouseButtons.Left, d =>
				{
					if(d.Msg.message != Api.WM_MOUSEMOVE) return;
					p = w.MouseClientXY;
					var b = this.Bounds;
					if(vert) {
						p.x -= offset;
						if(p.x < b.Left + _splitterWidth || p.x > b.Right - _splitterWidth * 2) return;
						int w1 = p.x - b.Left, w2 = b.Right - p.x - _splitterWidth;
						if(_isFraction) _fraction = (float)w1 / b.Width; else _width = _isWidth1 ? w1 : w2;
					} else {
						p.y -= offset;
						if(p.y < b.Top + _splitterWidth || p.y > b.Bottom - _splitterWidth * 2) return;
						int w1 = p.y - b.Top, w2 = b.Bottom - p.y - _splitterWidth;
						if(_isFraction) _fraction = (float)w1 / b.Height; else _width = _isWidth1 ? w1 : w2;
					}

					UpdateLayout();
					this.Invalidate();

					//info: currently we invalidate the rectangles to draw in OnPaint. Could instead draw now, but it flickers because background often is displayed before we draw text; need BufferedGraphics, like Control does internally on WM_PAINT if has a double-buffer style.
				});
			}

			/// <summary>
			/// Called from GDockPanel.OnPaint, for its _firstSplit.
			/// Paints all descendants.
			/// </summary>
			internal override void Paint(Graphics g)
			{
				if(_dockedChildCount == 2) {
					var t = _manager._paintTools;
					g.FillRectangle(t.brushSplitter, this._splitterBounds);
				}
				if(Child1.IsDocked) Child1.Paint(g);
				if(Child2.IsDocked) Child2.Paint(g);
			}

			internal void Invalidate()
			{
				Debug.Assert(!this.IsHidden);
				_manager.Invalidate(this.Bounds, false);
			}

			internal bool HitTestSplitter(int x, int y)
			{
				if(_dockedChildCount < 2) return false;
				return _splitterBounds.Contains(x, y);
			}

			[Conditional("DEBUG")]
			void _AssertIsChild(GNode gn)
			{
				Debug.Assert(gn == Child1 || gn == Child2);
			}

			internal void OnChildUndocked(GNode gn)
			{
				_AssertIsChild(gn);
				Debug.Assert(_dockedChildCount > 0);

				if(--_dockedChildCount == 0) {
					this.DockState = GDockState.Hidden;
					if(this.ParentSplit != null) this.ParentSplit.OnChildUndocked(this);
					else _manager.Invalidate(this.Bounds, false); //not this.Invalidate(); because now already Hidden
				} else {
					UpdateLayout();
					this.Invalidate();
				}
			}

			internal void OnChildDocked(GNode gn)
			{
				_AssertIsChild(gn);
				Debug.Assert(_dockedChildCount < 2);

				if(++_dockedChildCount == 1) {
					this.DockState = GDockState.Docked;
					this.ParentSplit?.OnChildDocked(this);
				}

				UpdateLayout();
				this.Invalidate();
			}
		}
	}
}
