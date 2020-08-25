using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Aga.Controls.Properties;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeCheckBox :InteractiveControl
	{
		public static int ImageSize { get { return Au.Util.ADpi.Scale(13); } }

		private Bitmap _check;
		private Bitmap _uncheck;
		private Bitmap _unknown;

		#region Properties

		private bool _threeState;
		[DefaultValue(false)]
		public bool ThreeState
		{
			get { return _threeState; }
			set { _threeState = value; }
		}

		#endregion

		public NodeCheckBox()
		{
		}

		public NodeCheckBox(string propertyName)
		{
			DataPropertyName = propertyName;
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			var wh = ImageSize;
			return new Size(wh, wh);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			CheckState state = GetCheckState(node);
			Rectangle r = GetBounds(node, context);
			var wh = ImageSize;
			r = new Rectangle(r.X, r.Y, wh, wh);
			if(Application.RenderWithVisualStyles) {
				VisualStyleRenderer renderer;
				if(state == CheckState.Indeterminate)
					renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.MixedNormal);
				else if(state == CheckState.Checked)
					renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.CheckedNormal);
				else
					renderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedNormal);
				renderer.DrawBackground(context.Graphics, r);
			} else {
				Image img;
				if(state == CheckState.Indeterminate)
					img = _unknown ?? (_unknown = Resources.unknown);
				else if(state == CheckState.Checked)
					img = _check ?? (_check = Resources.check);
				else
					img = _uncheck ?? (_uncheck = Resources.uncheck);
				context.Graphics.DrawImage(img, r);
			}
		}

		protected virtual CheckState GetCheckState(TreeNodeAdv node)
		{
			object obj = GetValue(node);
			if(obj is CheckState)
				return (CheckState)obj;
			else if(obj is bool)
				return (bool)obj ? CheckState.Checked : CheckState.Unchecked;
			else
				return CheckState.Unchecked;
		}

		protected virtual void SetCheckState(TreeNodeAdv node, CheckState value)
		{
			if(DataPropertyName == null) {
				SetValue(node, value);
				OnCheckStateChanged(node);
			} else {
				Type type = GetPropertyType(node);
				if(type == typeof(CheckState)) {
					SetValue(node, value);
					OnCheckStateChanged(node);
				} else if(type == typeof(bool)) {
					SetValue(node, value != CheckState.Unchecked);
					OnCheckStateChanged(node);
				}
			}
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if(args.Button == MouseButtons.Left && IsEditEnabled(args.Node)) {
				//DrawContext context = new DrawContext();
				//context.Bounds = args.ControlBounds;
				//Rectangle rect = GetBounds(args.Node, context);
				//if(rect.Contains(args.ViewLocation)) {
				CheckState state = GetCheckState(args.Node);
				state = GetNewState(state);
				SetCheckState(args.Node, state);
				Parent.UpdateView();
				args.Handled = true;
				//}
			}
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			if(args.Button == MouseButtons.Left && IsEditEnabled(args.Node)) {
				args.Handled = true;
			}
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			args.Handled = true;
		}

		private CheckState GetNewState(CheckState state)
		{
			if(state == CheckState.Indeterminate)
				return CheckState.Unchecked;
			else if(state == CheckState.Unchecked)
				return CheckState.Checked;
			else
				return ThreeState ? CheckState.Indeterminate : CheckState.Unchecked;
		}

		public override void KeyDown(KeyEventArgs args)
		{
			if(args.KeyCode == Keys.Space && EditEnabled) {
				Parent.BeginUpdate();
				try {
					if(Parent.CurrentNode != null) {
						CheckState value = GetNewState(GetCheckState(Parent.CurrentNode));
						foreach(TreeNodeAdv node in Parent.Selection)
							if(IsEditEnabled(node))
								SetCheckState(node, value);
					}
				}
				finally {
					Parent.EndUpdate();
				}
				args.Handled = true;
			}
		}

		//au: TreePathEventArgs -> TreeViewAdvEventArgs
		public event EventHandler<TreeViewAdvEventArgs> CheckStateChanged;
		protected void OnCheckStateChanged(TreeNodeAdv node)
		{
			if(CheckStateChanged != null)
				CheckStateChanged(this, new TreeViewAdvEventArgs(node));
		}

	}
}
