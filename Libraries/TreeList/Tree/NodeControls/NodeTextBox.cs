using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeTextBox :BaseTextControl
	{
		private const int MinTextBoxWidth = 30;

		public NodeTextBox()
		{
		}

		protected override Size CalculateEditorSize(EditorContext context)
		{
			Size z;
			if(Parent.UseColumns)
				z = context.Bounds.Size;
			else {
				Size size = GetLabelSize(context.CurrentNode, context.DrawContext, _label);
				int width = Math.Max(size.Width + Font.Height, MinTextBoxWidth); // reserve a place for new typed character
				z = new Size(width, size.Height);
			}
			if(MultilineEdit) z.Height = Math.Max(z.Height, Au.Util.Dpi.ScaleInt(100));
			return z;
		}

		public override void KeyDown(KeyEventArgs args)
		{
			if(args.KeyCode == Keys.F2 && Parent.CurrentNode != null && EditEnabled) {
				args.Handled = true;
				BeginEdit();
			}
		}

		protected override Control CreateEditor(TreeNodeAdv node)
		{
			TextBox textBox = CreateTextBox();
			if(MultilineEdit) {
				textBox.Multiline = true;
				textBox.WordWrap = false;
				textBox.ScrollBars = ScrollBars.Both;
			}
			textBox.TextAlign = TextAlign;
			//textBox.Text = GetLabel(node); //would apply TrimMultiLine
			textBox.Text = GetValue(node)?.ToString();
			textBox.BorderStyle = BorderStyle.FixedSingle;
			textBox.TextChanged += EditorTextChanged;
			textBox.KeyDown += EditorKeyDown;
			_label = textBox.Text;
			SetEditControlProperties(textBox, node);
			return textBox;
		}

		protected virtual TextBox CreateTextBox()
		{
			return new TextBox();
		}

		protected override void DisposeEditor(Control editor)
		{
			var textBox = editor as TextBox;
			textBox.TextChanged -= EditorTextChanged;
			textBox.KeyDown -= EditorKeyDown;
		}

		private string _label;
		private void EditorTextChanged(object sender, EventArgs e)
		{
			var textBox = sender as TextBox;
			_label = textBox.Text;
			Parent.UpdateEditorBounds();
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			var label = (editor as TextBox).Text;
			string oldLabel = GetLabel(node);
			if(oldLabel != label) {
				SetLabel(node, label);
				_UpdateRowHeight(node, oldLabel, label);
				OnLabelChanged(node.Tag, oldLabel, label);
			}
		}

		void _UpdateRowHeight(TreeNodeAdv node, string sOld, string sNew)
		{
			if(this.TrimMultiLine || !this.MultilineEdit) return;
			int n1 = sOld.CountLines_(true), n2 = sNew.CountLines_(true);
			if(n1 == n2) return;
			node.Height = null;
			this.Parent.SmartFullUpdate();
		}

		public override void Cut(Control control)
		{
			(control as TextBox).Cut();
		}

		public override void Copy(Control control)
		{
			(control as TextBox).Copy();
		}

		public override void Paste(Control control)
		{
			(control as TextBox).Paste();
		}

		public override void Delete(Control control)
		{
			var textBox = control as TextBox;
			int len = Math.Max(textBox.SelectionLength, 1);
			if(textBox.SelectionStart < textBox.Text.Length) {
				int start = textBox.SelectionStart;
				textBox.Text = textBox.Text.Remove(textBox.SelectionStart, len);
				textBox.SelectionStart = start;
			}
		}

		public bool MultilineEdit { get; set; }

		public event EventHandler<LabelEventArgs> LabelChanged;
		protected void OnLabelChanged(object subject, string oldLabel, string newLabel)
		{
			if(LabelChanged != null)
				LabelChanged(this, new LabelEventArgs(subject, oldLabel, newLabel));
		}
	}
}
