using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;

using Au;
using Au.Types;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeComboBox :BaseTextControl
	{
		#region Properties

		//Preferred width of the combo-box control when not using columns, at normal DPI.
		[DefaultValue(120)]
		public int EditorWidth { get; set; } = 120;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		[Editor(typeof(StringCollectionEditor), typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<object> DropDownItems { get; }

		#endregion

		public event EventHandler<EditEventArgs> CreatingEditor;

		public NodeComboBox()
		{
			DropDownItems = new List<object>();
		}

		protected override Size CalculateEditorSize(EditorContext context)
		{
			var width = Parent.UseColumns ? context.Bounds.Width : ADpi.Scale(EditorWidth);
			var height = (context.Editor is CheckedListBox c) ? (ADpi.Scale(c.PreferredHeight) + c.Items.Count * 2) : context.Bounds.Height;
			return new Size(width, height);
		}

		protected override Control CreateEditor(TreeNodeAdv node)
		{
			Control c;
			object value = GetValue(node);
			if(IsCheckedListBoxRequired(node))
				c = CreateCheckedListBox(node);
			else
				c = CreateCombo(node);
			SetEditControlProperties(c, node);
			OnCreatingEditor(new EditEventArgs(node, c));
			return c;
		}

		protected override void DisposeEditor(Control editor)
		{
			if(editor is ComboBox c) {
				if(EditableCombo) {
					c.KeyDown -= EditorKeyDown;
					c.SelectedValueChanged -= EditorDropDownClosed;
				} else c.DropDownClosed -= EditorDropDownClosed;
			}
		}

		protected virtual void OnCreatingEditor(EditEventArgs args)
		{
			CreatingEditor?.Invoke(this, args);
		}

		protected virtual bool IsCheckedListBoxRequired(TreeNodeAdv node)
		{
			object value = GetValue(node);
			if(value != null) {
				Type t = value.GetType();
				if(t.IsEnum) return t.IsDefined(typeof(FlagsAttribute), false);
			}
			return false;
		}

		public bool EditableCombo { get; set; }

		private Control CreateCombo(TreeNodeAdv node)
		{
			ComboBox c = new ComboBox();
			if(DropDownItems != null)
				c.Items.AddRange(DropDownItems.ToArray());
			var v = GetValue(node);
			if(EditableCombo) {
				c.DropDownStyle = ComboBoxStyle.DropDown;
				c.Text = v?.ToString();
				c.KeyDown += EditorKeyDown;
				c.SelectedValueChanged += EditorDropDownClosed; //cannot use DropDownClosed, then control text still not changed etc
			} else {
				c.DropDownStyle = ComboBoxStyle.DropDownList;
				c.SelectedItem = v;
				c.DropDownClosed += EditorDropDownClosed;
			}
			return c;
		}

		private Control CreateCheckedListBox(TreeNodeAdv node)
		{
			CheckedListBox listBox = new CheckedListBox();
			listBox.CheckOnClick = true;

			object value = GetValue(node);
			Type enumType = GetEnumType(node);
			foreach(object obj in Enum.GetValues(enumType)) {
				if((int)obj == 0) continue;
				if(enumType.GetField(obj.ToString()).IsDefined(typeof(BrowsableAttribute), false)) continue;
				listBox.Items.Add(obj, IsContain(value, obj));
			}
			return listBox;
		}

		protected virtual Type GetEnumType(TreeNodeAdv node)
		{
			object value = GetValue(node);
			return value.GetType();
		}

		private bool IsContain(object value, object enumElement)
		{
			if(value == null || enumElement == null)
				return false;
			if(value.GetType().IsEnum) {
				int i1 = (int)value;
				int i2 = (int)enumElement;
				return (i1 & i2) == i2;
			} else {
				var arr = value as object[];
				foreach(object obj in arr)
					if((int)obj == (int)enumElement)
						return true;
				return false;
			}
		}

		protected override string FormatLabel(object obj)
		{
			if(obj is object[] arr) return string.Join(", ", arr);
			return base.FormatLabel(obj);
		}

		void EditorDropDownClosed(object sender, EventArgs e)
		{
			EndEdit(true);
		}

		public override void UpdateEditor(Control control)
		{
			if(control is ComboBox)
				(control as ComboBox).DroppedDown = true;
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
			if(editor is ComboBox combo) {
				if(combo.DropDownStyle == ComboBoxStyle.DropDown)
					SetValue(node, combo.Text);
				else
					SetValue(node, combo.SelectedItem);
			} else {
				var listBox = editor as CheckedListBox;
				Type type = GetEnumType(node);
				if(IsFlags(type)) {
					int res = 0;
					foreach(object obj in listBox.CheckedItems)
						res |= (int)obj;
					object val = Enum.ToObject(type, res);
					SetValue(node, val);
				} else {
					List<object> list = new List<object>();
					foreach(object obj in listBox.CheckedItems)
						list.Add(obj);
					SetValue(node, list.ToArray());
				}
			}
		}

		private bool IsFlags(Type type)
		{
			object[] atr = type.GetCustomAttributes(typeof(FlagsAttribute), false);
			return atr.Length == 1;
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			if(args.Node != null && args.Node.IsSelected) //Workaround of specific ComboBox control behavior
				base.MouseUp(args);
		}
	}
}
