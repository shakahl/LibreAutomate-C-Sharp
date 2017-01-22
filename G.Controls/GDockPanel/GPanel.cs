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
//using System.Linq;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;

namespace G.Controls
{
	partial class GDockPanel
	{
		/// <summary>
		/// Contains single Control.
		/// </summary>
		partial class GPanel :GContentNode
		{
			internal Control Content; //the control. Can be null, eg when there was XML node but no control for it; then this panel is hidden.
			internal string Name;
			internal GTab ParentTab; //if this is a tabbed panel, ie is on a GTab, not directly on a GSplit
			internal int ImageIndex; //small tab button image index in imagelist
			internal bool HasDocument; //no caption etc
			internal bool HasToolbar; //small caption, maybe no splitter etc

			/// <summary>
			/// This ctor is used at startup, when adding from XML.
			/// </summary>
			internal GPanel(GDockPanel manager, GSplit parentSplit, XmlElement x) : base(manager, parentSplit)
			{
				var name = x.Attributes["name"].Value;
				//Control c;
				//if(!manager._initControls.TryGetValue(name, out c)) {
				//	this.DockState = GDockState.Hidden;
				//	return;
				//}
				var c = manager._initControls[name];

				_manager._aPanel.Add(this);

				this.Content = c;
				this.Name = c.Name;

				var ts = c as ToolStrip;
				if(ts != null) {
					_InitToolstrip(ts, x);
				} else if(x.HasAttribute("doc")) {
					this.HasDocument = true;
					c.AccessibleRole = AccessibleRole.Document;
				} else {
					c.AccessibleRole = AccessibleRole.Pane;
				}

				if(!this.HasDocument) {
					this.Text = x.Attribute_("text", c.Text);
					this.ToolTipText = x.Attribute_("tooltip");
				}

				this.ImageIndex = -1;
				ImageList il = _manager._imageList;
				if(il != null) {
					int ii = x.Attribute_("image", -1);
					if(ii >= 0 && ii < il.Images.Count) this.ImageIndex = ii;
					Debug.Assert(ii < il.Images.Count);
				}

				this.InitDockStateFromXML(x);
			}

			void _InitToolstrip(ToolStrip ts, XmlElement x)
			{
				this.HasToolbar = true;
				ts.SuspendLayout();
				ts.AutoSize = false;
				ts.Stretch = false;
				ts.GripStyle = ToolStripGripStyle.Hidden;
				ts.CanOverflow = true;
				if(ts is MenuStrip) {
					foreach(ToolStripItem v in ts.Items) v.Overflow = ToolStripItemOverflow.AsNeeded;
				}

				//OnSizeChanged will change it, but now we need _minWidth.
				//TODO: now GSplit requres _minWidth/_minHeight before OnSizeChanged. What if must be vertical at startup?
				ts.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
				_minHeight = ts.PreferredSize.Height; _minWidth = 12;

				ts.ResumeLayout(false);
			}

			internal void OnSizeChanged(int width, int height)
			{
				if(!HasToolbar) return;
				var ts = this.Content as ToolStrip;
				Size p = ts.PreferredSize;
				var oldLS = ts.LayoutStyle;
				ToolStripLayoutStyle newLS = oldLS;
				int k;
				_minWidth = _minHeight = 12;
				if(ts.Orientation == Orientation.Vertical) k = _minWidth = p.Width; else k = _minHeight = p.Height;
				k *= 2;
				if(width > k && height > k) newLS = ToolStripLayoutStyle.Flow;
				else if(width > k) newLS = ToolStripLayoutStyle.HorizontalStackWithOverflow;
				else if(height > k) newLS = ToolStripLayoutStyle.VerticalStackWithOverflow;
				if(newLS != oldLS) ts.LayoutStyle = newLS;
			}

			int _minWidth, _minHeight;
			internal override int MinimalWidth { get { return _minWidth; } }
			internal override int MinimalHeight { get { return _minHeight; } }

			/// <summary>
			/// Returns 1 if caption should be horizontal, 2 if vertical, 0 if any.
			/// </summary>
			internal int PreferredCaptionHorzVert
			{
				get
				{
					if(HasToolbar) {
						var ts = this.Content as ToolStrip;
						switch(ts.LayoutStyle) {
						case ToolStripLayoutStyle.HorizontalStackWithOverflow: return 2;
						case ToolStripLayoutStyle.VerticalStackWithOverflow: return 1;
						}
					}
					return 0;
				}
			}

			string _text;
			internal override string Text { get { return _text; } set { _text = value; this.InvalidateCaption(); } }

			string _tooltipText;
			internal string ToolTipText { get { return _tooltipText ?? Text; } set { _tooltipText = value; } }

			internal override bool IsTabbedPanel { get { return this.ParentTab != null; } }

			/// <summary>
			/// Returns true if this isn't hidden and isn't in a hidden tab group.
			/// </summary>
			/// <param name="andContentVisible">And isn't an inactive tab.</param>
			internal bool IsReallyVisible(bool andContentVisible)
			{
				if(IsHidden) return false;
				var gt = this.ParentTab;
				if(gt == null) return true;
				if(gt.IsHidden) return false;
				if(!andContentVisible) return true;
				//if(ContentOrNull == null) return false; //not null if active
				return this == gt.ActiveItem;
			}

			internal override void InvalidateCaption()
			{
				if(this.IsHidden || this.HasDocument) return;
				var gt = this.ParentTab;
				if(gt != null) gt.InvalidateCaption(); //eg need to resize tab button when Text changed
				else base.InvalidateCaption();
			}

			internal override void Paint(Graphics g)
			{
				Debug.Assert(!this.IsHidden);
				if(this.HasDocument) return;

				var t = _manager._paintTools;
				bool vert = this.IsVerticalCaption;

				var r = this.CaptionBounds;
				g.FillRectangle((!this.HasToolbar || this == _manager._hilitedTabButton || this.IsFloating) ? t.brushCaptionBack : t.brushSplitter, r);

				string s = this.Text;
				if(!Empty(s) && !this.HasToolbar) {
					var tf = vert ? t.txtFormatVert : t.txtFormatHorz;
					if(vert) r.Inflate(0, -2); else r.Inflate(-2, 0);
					g.DrawString(s, _manager.Font, t.brushCaptionText, r, tf);
					//info: TextRenderer.DrawText cannot draw vertical.
					//tested: with "Segoe UI" font same quality as TextRenderer.DrawText or ExtTextOut.
				}
			}

			public override string ToString()
			{
				return this.Text;
			}
		}

		//class GDocument :GPanel
		//{
		//}
	}
}
