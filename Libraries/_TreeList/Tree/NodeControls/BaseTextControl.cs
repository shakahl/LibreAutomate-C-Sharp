using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class BaseTextControl : EditableControl
	{
		private TextFormatFlags _formatFlags;

		#region Properties

		private Font _font = null;
		public Font Font {
			get {
				if(_font == null)
					return this.Parent.Font;
				else
					return _font;
			}
			set {
				if(value == this.Parent.Font)
					_font = null;
				else
					_font = value;
			}
		}

		protected bool ShouldSerializeFont()
		{
			return (_font != null);
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign {
			get { return _textAlign; }
			set {
				_textAlign = value;
				SetFormatFlags();
			}
		}

		private StringTrimming _trimming = StringTrimming.None;
		[DefaultValue(StringTrimming.None)]
		public StringTrimming Trimming {
			get { return _trimming; }
			set {
				_trimming = value;
				SetFormatFlags();
			}
		}

		private bool _displayHiddenContentInToolTip = true;
		[DefaultValue(true)]
		public bool DisplayHiddenContentInToolTip {
			get { return _displayHiddenContentInToolTip; }
			set { _displayHiddenContentInToolTip = value; }
		}

		[DefaultValue(false)]
		public bool TrimMultiLine {
			get;
			set;
		}

		#endregion

		protected BaseTextControl()
		{
			SetFormatFlags();
			LeftMargin = 1; //au: was 3
		}

		private void SetFormatFlags()
		{
			_formatFlags = TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.PreserveGraphicsTranslateTransform
				| TextHelper.TranslateAligmentToFlag(TextAlign)
				| TextHelper.TranslateTrimmingToFlag(Trimming)
				| TextFormatFlags.NoPrefix; //au
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return GetLabelSize(node, context);
		}

		protected Size GetLabelSize(TreeNodeAdv node, DrawContext context)
		{
			return GetLabelSize(node, context, GetLabel(node));
		}

		protected Size GetLabelSize(TreeNodeAdv node, DrawContext context, string label)
		{
			CheckThread();

			//au:
			//Font font = GetDrawingFont(node, context, label);
			Font font = FontNeeded?.Invoke(node) ?? context.Font;

			bool empty = string.IsNullOrEmpty(label); if(empty) label = " "; //au
			Size s = TextRenderer.MeasureText(label, font
				, new Size(int.MaxValue, int.MaxValue), _formatFlags); //au
			if(empty) s.Width = 0;
			s.Height += 2; if(s.Height < 17) s.Height = 17; //au
			return s;
		}

		//au: instead use FontNeeded. This adds too much overhead, because used many times for a node, mostly when calculating row height.
		//protected Font GetDrawingFont(TreeNodeAdv node, DrawContext context, string label)
		//{
		//	Font font = context.Font;
		//	if(DrawTextMustBeFired(node)) {
		//		DrawEventArgs args = new DrawEventArgs(node, this, context, label);
		//		args.Font = context.Font;
		//		OnDrawText(args);
		//		font = args.Font;
		//	}
		//	return font;
		//}

		/// <summary>
		/// Invoked when calculating row height and before drawing.
		/// </summary>
		public Func<TreeNodeAdv, Font> FontNeeded;

		protected void SetEditControlProperties(Control control, TreeNodeAdv node)
		{
			//au:
			//string label = GetLabel(node);
			//DrawContext context = new DrawContext();
			//context.Font = Font;
			//control.Font = GetDrawingFont(node, context, label);
			control.Font = FontNeeded?.Invoke(node) ?? Font;
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			if(context.CurrentEditorOwner == this && node == Parent.CurrentNode)
				return;

			string label = GetLabel(node);
			Rectangle bounds = GetBounds(node, context);
			Rectangle focusRect = new Rectangle(bounds.X, context.Bounds.Y,
				bounds.Width, context.Bounds.Height);

			Brush backgroundBrush;
			Color textColor;
			Font font;
			CreateBrushes(node, context, label, out backgroundBrush, out textColor, out font, ref label);

			if(backgroundBrush != null)
				context.Graphics.FillRectangle(backgroundBrush, focusRect);
			if(context.DrawFocus) {
				focusRect.Width--;
				focusRect.Height--;
				var pen = new Pen((context.DrawSelection == DrawSelectionMode.None) ? SystemColors.ControlText : SystemColors.Control);
				pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
				context.Graphics.DrawRectangle(pen, focusRect);
				pen.Dispose();
			}

			if(CustomDrawText != null) {
				//au: custom draw
				CustomDrawText(new CustomDrawTextArgs(node, context, bounds));
			} else {
				//using(var brush = new SolidBrush(textColor)) context.Graphics.DrawString(label, font, brush, bounds);
				if(context.IsMemoryBitmap) { //au: TextRenderer.DrawText bug workaround: use backColor.
					TextRenderer.DrawText(context.Graphics, label, font, bounds, textColor, node.Tree.BackColor, _formatFlags);
				} else {
					TextRenderer.DrawText(context.Graphics, label, font, bounds, textColor, _formatFlags);
				}
			}
			//au: when drawing frequently, eg scrolling, TextRenderer.DrawText adds several magabytes of used memory.
			//	It seems it is later released by GC and then does not grow again. Multiple controls don't add more memory than single.
			//	Graphics.DrawString and GDI API don't have this problem.
			//Also TextRenderer.DrawText is several times slower than Graphics.DrawString. Makes OnPaint ~3 times slower, eg 5 -> 15 ms.
			//	In more recent test, makes only 50% slower, eg 6 -> 9 ms. TextRenderer.DrawText is ~50% of OnPaint time. Another 25% is TextRenderer.MeasureText, called 2 times.
			//	GDI API DrawText speed is same as of TextRenderer.DrawText.
		}

		private void CreateBrushes(TreeNodeAdv node, DrawContext context, string text, out Brush backgroundBrush, out Color textColor, out Font font, ref string label)
		{
			textColor = node.Tree.ForeColor;
			backgroundBrush = null;
			font = FontNeeded?.Invoke(node) ?? context.Font; //args.Font = font; //au: was font = context.Font;
			if(context.DrawSelection == DrawSelectionMode.Active) {
				textColor = SystemColors.HighlightText;
				backgroundBrush = SystemBrushes.Highlight;
			} else if(context.DrawSelection == DrawSelectionMode.Inactive) {
				textColor = SystemColors.WindowText;
				backgroundBrush = SystemBrushes.Control;
			} else if(context.DrawSelection == DrawSelectionMode.FullRowSelect)
				textColor = SystemColors.HighlightText;

			if(!context.Enabled)
				textColor = SystemColors.GrayText;


			if(DrawTextMustBeFired(node)) {
				DrawEventArgs args = new DrawEventArgs(node, this, context, text);
				args.Text = label;
				args.TextColor = textColor;
				args.BackgroundBrush = backgroundBrush;

				OnDrawText(args);

				textColor = args.TextColor;
				backgroundBrush = args.BackgroundBrush;
				//font = args.Font; //au
				label = args.Text;
			}
		}

		public string GetLabel(TreeNodeAdv node)
		{
			if(node != null && node.Tag != null) {
				object obj = GetValue(node);
				if(obj != null)
					return FormatLabel(obj);
			}
			return string.Empty;
		}

		protected virtual string FormatLabel(object obj)
		{
			var res = obj.ToString();
			if(TrimMultiLine && res != null) {
				int i = res.IndexOf('\n');
				if(i >= 0) {
					if(i > 0 && res[i - 1] == '\r') i--;
					res = res.Remove(i) + "...";
				}
			}
			return res ?? "";//Au:??""
		}

		public void SetLabel(TreeNodeAdv node, string value)
		{
			SetValue(node, value);
		}

		/// <summary>
		/// Fires when control is going to draw a text. Can be used to change text or back color
		/// </summary>
		public event EventHandler<DrawEventArgs> DrawText;
		void OnDrawText(DrawEventArgs args) //au: removed 'protected virtual'
		{
			//au: see comments in TreeViewAdv.cs
			//TreeViewAdv tree = args.Node.Tree;
			//if(tree != null)
			//	tree.FireDrawControl(args);
			if(DrawText != null)
				DrawText(this, args);
		}

		bool DrawTextMustBeFired(TreeNodeAdv node) //au: removed 'protected virtual'
		{
			//au: see comments in TreeViewAdv.cs
			//return DrawText != null || (node.Tree != null && node.Tree.DrawControlMustBeFired());
			return DrawText != null && (NeedDrawTextEvent?.Invoke(node) ?? true);
		}

		/// <summary>
		/// If not null, invoked before DrawText event, and does not fire the event if you return false.
		/// It is for performance.
		/// </summary>
		public Func<TreeNodeAdv, bool> NeedDrawTextEvent;

		//au: custom draw
		public Action<CustomDrawTextArgs> CustomDrawText { get; set; }
	}

	//au: custom draw
	public class CustomDrawTextArgs : NodeEventArgs
	{
		public DrawContext Context { get; }

		public Rectangle Bounds { get; }

		//public EditableControl Control { get; }

		public CustomDrawTextArgs(TreeNodeAdv node, /*EditableControl control, */DrawContext context, Rectangle bounds)
			: base(node)
		{
			//Control = control;
			Context = context;
			Bounds = bounds;
		}

	}
}
