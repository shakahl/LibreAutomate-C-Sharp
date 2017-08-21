using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using Catkeys;
using static Catkeys.NoClass;

namespace Aga.Controls.Tree.NodeControls
{
	public abstract class EditableControl :InteractiveControl
	{
		private Timer_ _timer;
		private bool _editFlag;

		#region Properties

		private bool _editOnClick = false;
		[DefaultValue(false)]
		public bool EditOnClick
		{
			get { return _editOnClick; }
			set { _editOnClick = value; }
		}

		#endregion

		protected EditableControl()
		{
		}

		public void SetEditorBounds(EditorContext context)
		{
			Size size = CalculateEditorSize(context);
			context.Editor.Bounds = new Rectangle(context.Bounds.X, context.Bounds.Y,
				Math.Min(size.Width, context.Bounds.Width),
				Math.Min(size.Height, Parent.ClientSize.Height - context.Bounds.Y)
			);
		}

		protected abstract Size CalculateEditorSize(EditorContext context);

		protected virtual bool CanEdit(TreeNodeAdv node)
		{
			return (node.Tag != null) && IsEditEnabled(node);
		}

		public void BeginEdit()
		{
			if(Parent != null && Parent.CurrentNode != null && CanEdit(Parent.CurrentNode)) {
				CancelEventArgs args = new CancelEventArgs();
				OnEditorShowing(args);
				if(!args.Cancel) {
					var editor = CreateEditor(Parent.CurrentNode);
					Parent.DisplayEditor(editor, this);
				}
			}
		}

		public void EndEdit(bool applyChanges)
		{
			if(Parent != null)
				if(Parent.HideEditor(applyChanges))
					OnEditorHided();
		}

		public virtual void UpdateEditor(Control control)
		{
		}

		internal void ApplyChanges(TreeNodeAdv node, Control editor)
		{
			DoApplyChanges(node, editor);
			OnChangesApplied();
		}

		internal void DoDisposeEditor(Control editor)
		{
			DisposeEditor(editor);
		}

		protected void EditorKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape)
				EndEdit(false);
			else if(e.KeyCode == Keys.Enter && e.Modifiers == 0)
				EndEdit(true);
		}

		protected abstract void DoApplyChanges(TreeNodeAdv node, Control editor);

		protected abstract Control CreateEditor(TreeNodeAdv node);

		protected abstract void DisposeEditor(Control editor);

		public virtual void Cut(Control control)
		{
		}

		public virtual void Copy(Control control)
		{
		}

		public virtual void Paste(Control control)
		{
		}

		public virtual void Delete(Control control)
		{
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			_editFlag = (!EditOnClick && args.Button == MouseButtons.Left
				&& args.ModifierKeys == Keys.None && args.Node.IsSelected
				&& Parent.SelectedNodes.Count == 1
				&& Parent.ShowPlusMinus);
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			if(args.Node.IsSelected) {
				if(EditOnClick && args.Button == MouseButtons.Left && args.ModifierKeys == Keys.None) {
					Parent.ItemDragMode = false;
					BeginEdit();
					args.Handled = true;
				} else if(_editFlag)// && args.Node.IsSelected)
					_StartTimer();
			}
		}

		void _StartTimer()
		{
			if(_timer == null)
				_timer = new Timer_(t =>
				{
					if(_editFlag) BeginEdit();
					_editFlag = false;
				});
			_timer.Start(500, true);
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			_editFlag = false;
			_timer?.Stop();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if(disposing)
				_timer?.Stop();
		}

		#region Events

		public event CancelEventHandler EditorShowing;
		protected void OnEditorShowing(CancelEventArgs args)
		{
			if(EditorShowing != null)
				EditorShowing(this, args);
		}

		public event EventHandler EditorHided;
		protected void OnEditorHided()
		{
			if(EditorHided != null)
				EditorHided(this, EventArgs.Empty);
		}

		public event EventHandler ChangesApplied;
		protected void OnChangesApplied()
		{
			if(ChangesApplied != null)
				ChangesApplied(this, EventArgs.Empty);
		}

		#endregion
	}
}
