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
		/// Contains single Control.
		/// </summary>
		partial class _Panel :_ContentNode, IPanel
		{
			internal string Name;
			internal _Tab ParentTab; //if this is a tabbed panel, ie is on a _Tab, not directly on a _Split
			internal bool HasDocument; //no caption etc
			internal bool HasToolbar; //small caption, maybe no splitter etc

			/// <summary>
			/// This ctor is used at startup, when adding from XML.
			/// </summary>
			internal _Panel(AuDockPanel manager, _Split parentSplit, XElement x, _Tab parentTab = null) : base(manager, parentSplit)
			{
				var name = x.Attr("name");
				var c = manager._initControls[name];

				_manager._aPanel.Add(this);

				this.Content = c;
				Debug.Assert(c.Name == name);
				this.Name = name;
				this.ParentTab = parentTab;

				if(c is ToolStrip ts) {
					_InitToolstrip(ts);
				} else if(x.HasAttr("doc")) {
					this.HasDocument = true;
				//	c.AccessibleRole = AccessibleRole.Document; //no, its child is DOCUMENT
				//} else {
				//	c.AccessibleRole = AccessibleRole.Pane;
				}
				c.AccessibleRole = AccessibleRole.Pane;

				this.InitDockStateFromXML(x);
			}

			internal override void Save(XmlWriter x)
			{
				x.WriteStartElement("panel");
				x.WriteAttributeString("name", this.Name);

				if(this.HasDocument) {
					x.WriteAttributeString("doc", "");
				}

				SaveDockStateToXml(x);

				x.WriteEndElement();
			}

			void _InitToolstrip(ToolStrip ts)
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

				ts.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
				_minHeight = ts.PreferredSize.Height; _minWidth = _minHeight - 1;

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

			internal override int MinimalWidth => _minWidth;
			internal override int MinimalHeight => _minHeight;
			int _minWidth, _minHeight;
		}

		#region IPanel

		public interface IPanel
		{
			/// <summary>
			/// The child Control.
			/// </summary>
			Control Content { get; }

			/// <summary>
			/// Panel caption text. Also tab button text.
			/// If null or not set, uses control's Name property.
			/// </summary>
			string Text { get; set; }

			/// <summary>
			/// Tooltip text.
			/// If null or not set, uses Text (default - control's Name property).
			/// </summary>
			string ToolTipText { get; set; }

			/// <summary>
			/// Image to display on tab button if it is too small for text.
			/// </summary>
			Image Image { get; set; }

			/// <summary>
			/// Sets commonly used properties. Can be used to avoid several function calls (Image, ToolTipText).
			/// </summary>
			/// <param name="toolTip">Tooltip text.</param>
			/// <param name="image">Image to display on tab button if it is too small for text.</param>
			void Init(string toolTip, Image image = null);

			/// <summary>
			/// The 'get' function returns true if the panel or its tab button is visible. Returns false if it is hidden or its parent tab group is hidden.
			/// The 'set' function makes the panel visible, in the most recent state (docked or floating). Activates its tab button if need.
			/// </summary>
			bool Visible { get; set; }

			/// <summary>
			/// Returns true if the panel is visible.
			/// Returns false if it is hidden or its parent tab group is hidden or it is an inactive tab.
			/// </summary>
			bool ReallyVisible { get; }

			/// <summary>
			/// The 'get' function returns true if the panel is floating and not hidden.
			/// The 'set' function: if true, makes this panel floating and visible if it is currently docked or hidden; if false, makes it docked if it is currently floating and visible.
			/// </summary>
			bool Floating { get; set; }

			/// <summary>
			/// Make the panel visible (like <see cref="Visible"/>) and focus its control.
			/// </summary>
			void Focus();
		}

		partial class _Panel
		{
			public Control Content { get; } //the control. Can be null, eg when there was XML node but no control for it; then this panel is hidden.

			public string Text
			{
				get => _text ?? this.Name;
				set { _text = value; if(!this.CaptionBounds.IsEmpty) this.InvalidateCaption(); }
			}
			string _text;

			public string ToolTipText
			{
				get => _tooltipText ?? Text;
				set { _tooltipText = value; }
			}
			string _tooltipText;

			public Image Image
			{
				get => _image;
				set { _image = value; if(!this.CaptionBounds.IsEmpty) this.InvalidateCaption(); }
			}
			Image _image;

			public void Init(string toolTip, Image image = null)
			{
				ToolTipText = toolTip;
				Image = image;
			}

			/// <summary>
			/// note: this is not just !IsHidden. Returns false if in a hidden tab group even if IsHidden is false.
			/// </summary>
			public bool Visible
			{
				get => _IsVisible(false);
				set { if(value) Show(); else Hide(); }
			}

			public bool ReallyVisible
			{
				get => _IsVisible(true);
			}

			/// <summary>
			/// Returns true if this isn't hidden and isn't in a hidden tab group.
			/// </summary>
			/// <param name="andContentVisible">And isn't an inactive tab.</param>
			bool _IsVisible(bool andContentVisible)
			{
				if(IsHidden) return false;
				if(IsFloating) return true;
				var gt = this.ParentTab; if(gt == null) return true;
				if(gt.IsHidden) return false;
				if(!andContentVisible) return true;
				//if(ContentOrNull == null) return false; //not null if active
				return this == gt.ActiveItem;
			}

			public bool Floating
			{
				get => this.IsFloating;
				set { if(value != this.IsFloating) this.SetDockState(this.IsFloating ? _DockState.Docked : _DockState.Floating); }
			}

			public void Focus() { Show(true); }
			#endregion

			internal override bool IsTabbedPanel => this.ParentTab != null;

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

				var r = this.CaptionBounds;
				bool colored = (!this.HasToolbar || this == _manager._hilitedTabButton || this.IsFloating);
				g.FillRectangle(colored ? t.brushCaptionBack : t.brushSplitter, r);

				string s = this.Text;
				if(!Empty(s) && !this.HasToolbar) {
					bool vert = this.IsVerticalCaption;
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
	}
}
