using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class BaseTextControl :EditableControl
	{
		private TextFormatFlags _baseFormatFlags;
		private TextFormatFlags _formatFlags;
		private StringFormat _format;

		#region Properties

		private Font _font = null;
		public Font Font
		{
			get
			{
				if(_font == null)
					return this.Parent.Font;
				else
					return _font;
			}
			set
			{
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
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set
			{
				_textAlign = value;
				SetFormatFlags();
			}
		}

		private StringTrimming _trimming = StringTrimming.None;
		[DefaultValue(StringTrimming.None)]
		public StringTrimming Trimming
		{
			get { return _trimming; }
			set
			{
				_trimming = value;
				SetFormatFlags();
			}
		}

		private bool _displayHiddenContentInToolTip = true;
		[DefaultValue(true)]
		public bool DisplayHiddenContentInToolTip
		{
			get { return _displayHiddenContentInToolTip; }
			set { _displayHiddenContentInToolTip = value; }
		}

		[DefaultValue(false)]
		public bool TrimMultiLine
		{
			get;
			set;
		}

		#endregion

		protected BaseTextControl()
		{
			_format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces);
			_baseFormatFlags = TextFormatFlags.PreserveGraphicsClipping |
						   TextFormatFlags.PreserveGraphicsTranslateTransform;
			SetFormatFlags();
			LeftMargin = 3;
		}

		private void SetFormatFlags()
		{
			_format.Alignment = TextHelper.TranslateAligment(TextAlign);
			_format.Trimming = Trimming;

			_formatFlags = _baseFormatFlags | TextHelper.TranslateAligmentToFlag(TextAlign)
				| TextHelper.TranslateTrimmingToFlag(Trimming);
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
			Font font = GetDrawingFont(node, context, label);
			Size s = TextRenderer.MeasureText(label, font);

			if(!s.IsEmpty)
				return s;
			else
				return new Size(10, Font.Height);
		}

		protected Font GetDrawingFont(TreeNodeAdv node, DrawContext context, string label)
		{
			Font font = context.Font;
			if(DrawTextMustBeFired(node)) {
				DrawEventArgs args = new DrawEventArgs(node, this, context, label);
				args.Font = context.Font;
				OnDrawText(args);
				font = args.Font;
			}
			return font;
		}

		protected void SetEditControlProperties(Control control, TreeNodeAdv node)
		{
			string label = GetLabel(node);
			DrawContext context = new DrawContext();
			context.Font = Font;
			control.Font = GetDrawingFont(node, context, label);
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

			//Catkeys: TextRenderer.DrawText bug workaround: use DrawString, or backColor.
			if(context.IsMemoryBitmap) {
				//using(var brush = new SolidBrush(textColor)) context.Graphics.DrawString(label, font, brush, bounds);
				TextRenderer.DrawText(context.Graphics, label, font, bounds, textColor, node.Tree.BackColor, _formatFlags);
			} else {
				TextRenderer.DrawText(context.Graphics, label, font, bounds, textColor, _formatFlags);
			}
		}

		private static Dictionary<Color, Brush> _brushes = new Dictionary<Color, Brush>();
		private static Brush GetFrush(Color color)
		{
			Brush br;
			if(_brushes.ContainsKey(color))
				br = _brushes[color];
			else {
				br = new SolidBrush(color);
				_brushes.Add(color, br);
			}
			return br;
		}

		private void CreateBrushes(TreeNodeAdv node, DrawContext context, string text, out Brush backgroundBrush, out Color textColor, out Font font, ref string label)
		{
			textColor = node.Tree.ForeColor;
			backgroundBrush = null;
			font = context.Font;
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
				args.Font = font;

				OnDrawText(args);

				textColor = args.TextColor;
				backgroundBrush = args.BackgroundBrush;
				font = args.Font;
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
			return res;
		}

		public void SetLabel(TreeNodeAdv node, string value)
		{
			SetValue(node, value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if(disposing) {
				_format.Dispose();
			}
		}

		/// <summary>
		/// Fires when control is going to draw a text. Can be used to change text or back color
		/// </summary>
		public event EventHandler<DrawEventArgs> DrawText;
		protected virtual void OnDrawText(DrawEventArgs args)
		{
			TreeViewAdv tree = args.Node.Tree;
			if(tree != null)
				tree.FireDrawControl(args);
			if(DrawText != null)
				DrawText(this, args);
		}

		protected virtual bool DrawTextMustBeFired(TreeNodeAdv node)
		{
			return DrawText != null || (node.Tree != null && node.Tree.DrawControlMustBeFired());
		}
	}
}
