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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using Au;
using Au.Types;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

partial class FilesModel
{
	public class TreeViewFiles : TreeViewAdv
	{
		FilesModel _model;
		public new FilesModel Model {
			get => _model;
			set { base.Model = _model = value; }
		}

		public TreeViewFiles()
		{
			this.BorderStyle = BorderStyle.None;
			this.Dock = DockStyle.Fill;

			//this.AutoRowHeight = false; this.RowHeight = 25; //RowHeight ignored if AutoRowHeight true
			//this.BackgroundImageLayout = ImageLayout.Center;
			//this.DefaultToolTipProvider = null;
			//this.GridLineStyle = GridLineStyle.Vertical;
			//this.LineColor = SystemColors.ControlDark; //default
			//this.LoadOnDemand = false;
			this.LoadOnDemand = true; //saves 2 ms for 1000 items
			this.SelectionMode = TreeSelectionMode.MultiSameParent;
			//this.SelectionMode = TreeSelectionMode.Single;
			//this.SelectionMode = TreeSelectionMode.Multi; //creates problems
			this.ShowNodeToolTips = true;
			this.FullRowSelect = true;
			//this.HideSelection = true;
			this.ShowPlusMinus = false; this.ShowLines = false; //also disables editing on 2 clicks, it is more annoying than useful

			//this.ShiftFirstNode = true;
			//this.Indent /= 2;
			//this.ShowLines = false;
			//this.BackColor = Color.DarkGreen;
			//this.ForeColor = Color.Yellow;
			//this.LineColor = Color.Red;
			this.AllowDrop = true;
			this.DisplayDraggingNodes = true;

#if TEST_MANY_COLUMNS
			this.GridLineStyle = GridLineStyle.HorizontalAndVertical;
			this.AllowColumnReorder = true;
#endif
			_AddNodeControls();

			//ATimer.After(1500, _ => {//TODO
			//	Font = new Font("Segoe UI", 20f);
			//	this.AutoRowHeight = false;
			//	this.RowHeight *= 2;
			//});
		}

		Font _fontBold;
		protected override void OnFontChanged(EventArgs e) {
			_fontBold=null;
			base.OnFontChanged(e);
		}

		#region node controls

#if TEST_MANY_COLUMNS
		TreeColumn _columnName;
		TreeColumn _columnGUID;
		TreeColumn _columnCombo;
		TreeColumn _columnComboEnum;
		TreeColumn _columnDecimal;
		TreeColumn _columnInteger;
		TreeColumn _columnUpDown;
#endif

		//use prefix _cc for "column control"
		NodeIcon _ccIcon;
		NodeTextBox _ccName;
#if TEST_MANY_COLUMNS
		NodeCheckBox _ccCheck;
		NodeTextBox _ccGUID;
		NodeComboBox _ccCombo;
		NodeComboBox _ccComboEnum;
		NodeDecimalTextBox _ccDecimal;
		NodeIntegerTextBox _ccInteger;
		NodeNumericUpDown _ccUpDown;
#endif

		void _AddNodeControls()
		{
#if TEST_MANY_COLUMNS
			this.UseColumns = true;

			_columnName = new TreeColumn("Name", 200);
			this.Columns.Add(_columnName);
#endif

			_ccIcon = new NodeIcon();
			this.NodeControls.Add(_ccIcon);
			_ccIcon.LeftMargin = 5; //default 1
			_ccIcon.ScaleMode = ImageScaleMode.ScaleUp;
			_ccIcon.DpiStretch = true;
			_ccIcon.ValueNeeded = _ccIcon_ValueNeeded;

			_ccName = new NodeTextBox();
			this.NodeControls.Add(_ccName);
			_ccName.EditEnabled = true;
			_ccName.ValueNeeded = node => (node.Tag as FileNode).DisplayName;
			_ccName.ValuePushed = (node, value) => { (node.Tag as FileNode).FileRename(value as string, true); };
			_ccName.FontNeeded = node => node.Tag == _model.CurrentFile ? (_fontBold ??= new Font(this.Font, FontStyle.Bold)) : this.Font;
			_ccName.DrawText += _ccName_DrawText;

#if TEST_MANY_COLUMNS
			_ccIcon.ParentColumn = _columnName;
			_ccName.ParentColumn = _columnName;


			_columnGUID = new TreeColumn("GUID", 100);
			//_columnGUID.IsVisible = false;
			//_columnGUID.MinColumnWidth = 25; _columnGUID.MaxColumnWidth = 200;
			//_columnGUID.TextAlign = HorizontalAlignment.Center; //header text
			//_columnGUID.TooltipText = "column tooltip";
			//_columnGUID.Sortable = true;
			//_columnGUID.SortOrder = SortOrder.Ascending;
			this.Columns.Add(_columnGUID);

			_ccCheck = new NodeCheckBox();
			this.NodeControls.Add(_ccCheck);
			_ccCheck.DataPropertyName = "Checked";
			_ccCheck.EditEnabled = true;
			_ccCheck.LeftMargin = 2;
			_ccCheck.ParentColumn = _columnGUID;
			//_nodeCheckBox.ThreeState = true;
			//_nodeCheckBox.CheckStateChanged += _nodeCheckBox_CheckStateChanged;

			_ccGUID = new NodeTextBox();
			this.NodeControls.Add(_ccGUID);
			_ccGUID.ParentColumn = _columnGUID;
			_ccGUID.DataPropertyName = "GUID";
			//_controlGUID.LeftMargin = 3; //default
			_ccGUID.EditEnabled = true; _ccGUID.EditOnClick = true;
			//_ccGUID.TrimMultiLine = true;
			_ccGUID.MultilineEdit = true;
			//_ccGUID.Font = new Font("Courier New", 8);
			//this.Font = new Font("Courier New", 8);

			_columnCombo = new TreeColumn("Combo", 100);
			//_columnCombo.IsVisible = false;
			this.Columns.Add(_columnCombo);
			_columnComboEnum = new TreeColumn("Combo Enum", 100);
			this.Columns.Add(_columnComboEnum);
			_columnDecimal = new TreeColumn("Decimal", 60);
			this.Columns.Add(_columnDecimal);
			_columnInteger = new TreeColumn("Integer", 60);
			this.Columns.Add(_columnInteger);
			_columnUpDown = new TreeColumn("UpDown", 100);
			this.Columns.Add(_columnUpDown);

			_ccCombo = new NodeComboBox();
			this.NodeControls.Add(_ccCombo);
			_ccCombo.ParentColumn = _columnCombo;
			_ccCombo.DataPropertyName = "Combo";
			_ccCombo.EditEnabled = true; _ccCombo.EditOnClick = true;
			_ccCombo.EditableCombo = true;
			_ccCombo.TextAlign = HorizontalAlignment.Right;
			var dd = _ccCombo.DropDownItems;
			dd.Add("one"); dd.Add(2); dd.Add(3.5); dd.Add(4); dd.Add(5); dd.Add(6); dd.Add(7); dd.Add(8); dd.Add(9); dd.Add(10);
			dd.Add("one"); dd.Add(2); dd.Add(3.5); dd.Add(4); dd.Add(5); dd.Add(6); dd.Add(7); dd.Add(8); dd.Add(9); dd.Add(10);
			//_ccCombo.CreatingEditor += _ccCombo_CreatingEditor;

			_ccComboEnum = new NodeComboBox();
			this.NodeControls.Add(_ccComboEnum);
			_ccComboEnum.ParentColumn = _columnComboEnum;
			_ccComboEnum.DataPropertyName = "ComboEnum";
			_ccComboEnum.EditEnabled = true; _ccComboEnum.EditOnClick = true;
			//_ccComboEnum.CreatingEditor += (_, _) => AOutput.Write("ce");

			_ccDecimal = new NodeDecimalTextBox();
			this.NodeControls.Add(_ccDecimal);
			_ccDecimal.ParentColumn = _columnDecimal;
			_ccDecimal.DataPropertyName = "Decimal";
			_ccDecimal.EditEnabled = true; _ccDecimal.EditOnClick = true;

			_ccInteger = new NodeIntegerTextBox();
			this.NodeControls.Add(_ccInteger);
			_ccInteger.ParentColumn = _columnInteger;
			_ccInteger.DataPropertyName = "Integer";
			_ccInteger.EditEnabled = true; _ccInteger.EditOnClick = true;

			_ccUpDown = new NodeNumericUpDown();
			this.NodeControls.Add(_ccUpDown);
			_ccUpDown.ParentColumn = _columnUpDown;
			_ccUpDown.DataPropertyName = "UpDown";
			_ccUpDown.EditEnabled = true; _ccUpDown.EditOnClick = true;
#endif

			foreach(var tc in this.NodeControls.OfType<BaseTextControl>()) {
				tc.Trimming = StringTrimming.EllipsisCharacter; //add "..." if too long. But does not work if multiline.
			}
		}

#if TEST_MANY_COLUMNS
		private void _ccCombo_CreatingEditor(object sender, EditEventArgs e)
		{
			//AOutput.Write(e.Control);
			var c = e.Control as ComboBox;
			var dd = c.Items; dd.Add("one"); dd.Add(2); dd.Add(3.5); dd.Add(4); dd.Add(5); dd.Add(6); dd.Add(7); dd.Add(8); dd.Add(9); dd.Add(10);
		}
		//private void _nodeCheckBox_CheckStateChanged(object sender, TreeViewAdvEventArgs e)
		//{
		//	AOutput.Write(node);
		//}
#endif

		private object _ccIcon_ValueNeeded(TreeNodeAdv node)
		{
			var f = node.Tag as FileNode;
			//AOutput.Write(f);
			Debug.Assert(node.IsLeaf != f.IsFolder);

			if(_model.IsInPrivateClipboard(f, out bool cut)) return EdResources.GetImageUseCache(cut ? nameof(Au.Editor.Resources.Resources.cut) : nameof(Au.Editor.Resources.Resources.copy));
			return f.GetIcon(node.IsExpanded);
		}

		private void _ccName_DrawText(object sender, DrawEventArgs e)
		{
			var f = e.Node.Tag as FileNode;
			if(f.IsFolder) return;
			if(f == _model.CurrentFile) {
				//e.TextColor = Color.DarkBlue;
				if(e.Node.IsSelected && e.Context.DrawSelection == DrawSelectionMode.None && _IsTextBlack)
					e.BackgroundBrush = Brushes.LightGoldenrodYellow; //yellow text rect in selected-inactive
			}
		}

		protected override void OnRowDraw(PaintEventArgs e, TreeNodeAdv node, ref DrawContext context, int row, Rectangle rowRect)
		{
			var f = node.Tag as FileNode;
			if(f.IsFolder) return;
			if(!node.IsSelected && _model.OpenFiles.Contains(f)) { //draw yellow background for open files
				var g = e.Graphics;
				var r = rowRect; //why width 0?
				r.X = OffsetX; r.Width = ClientSize.Width;
				if(_IsTextBlack) g.FillRectangle(Brushes.LightGoldenrodYellow, r);
				//if(f == _model.CurrentFile) {
				//	r.Width--; r.Height--;
				//	g.DrawRectangle(SystemPens.ControlDark, r);
				//}
			}
			base.OnRowDraw(e, node, ref context, row, rowRect);
		}

		static bool _IsTextBlack => (uint)SystemColors.WindowText.ToArgb() == 0xFF000000; //if not high-contrast theme

		#endregion

		#region drag-drop

		bool _drag;
		TreeNodeAdv[] _dragNodes;

		protected override void OnItemDrag(MouseButtons buttons, object item)
		{
			if(buttons == MouseButtons.Left) DoDragDropSelectedNodes(DragDropEffects.Move | DragDropEffects.Copy);
			base.OnItemDrag(buttons, item);
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			_drag = false; _dragNodes = null;
			e.Effect = 0;

			//can drop TreeNodeAdv and files
			TreeNodeAdv[] nodes = null;
			if(e.Data.GetDataPresent(typeof(TreeNodeAdv[]))) {
				nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
				if(nodes[0].Tree != this) return;
			} else if(e.Data.GetDataPresent(DataFormats.FileDrop, false)) {
			} else return;

			var effect = _GetEffect(e, nodes != null); if(effect == 0) return;
			e.Effect = effect;

			_drag = true; _dragNodes = nodes;

			base.OnDragEnter(e);
		}

		static DragDropEffects _GetEffect(DragEventArgs e, bool nodes)
		{
			var effect = e.AllowedEffect;
			bool copy = 0 != (e.KeyState & 8);
			if(nodes) effect &= copy ? DragDropEffects.Copy : DragDropEffects.Move;
			else if(copy) effect &= DragDropEffects.Copy;
			return effect;
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			if(!_drag) return;
			//ADebug.PrintFunc();

			base.OnDragOver(e); //set drop position, auto-scroll

			e.Effect = 0;
			var effect = _GetEffect(e, _dragNodes != null); if(effect == 0) return;
			bool copy = 0 != (e.KeyState & 8);

			var nTarget = this.DropPosition.Node; if(nTarget == null) return;
			var fTarget = nTarget.Tag as FileNode;
			bool isFolder = fTarget.IsFolder;
			bool isInside = this.DropPosition.Position == NodePosition.Inside;

			//prevent selecting whole non-folder item. Make either above or below.
			if(isFolder) this.DragDropBottomEdgeSensivity = this.DragDropTopEdgeSensivity = 0.3f; //default
			else this.DragDropBottomEdgeSensivity = this.DragDropTopEdgeSensivity = 0.51f;

			//can drop here?
			if(!copy && _dragNodes != null) {
				foreach(TreeNodeAdv n in _dragNodes) {
					var f = n.Tag as FileNode;
					if(!f.CanMove(fTarget, (FNPosition)this.DropPosition.Position)) return;
				}
			}

			//expand-collapse folder on right-click. However this does not work when dragging files, because Explorer then ends the drag-drop.
			if(isFolder && isInside) {
				var ks = e.KeyState & 3;
				if(ks == 3 && _dragKeyStateForFolderExpand != 3) {
					if(nTarget.IsExpanded) nTarget.Collapse(); else nTarget.Expand();
				}
				_dragKeyStateForFolderExpand = ks;
			}

			e.Effect = effect;
		}

		int _dragKeyStateForFolderExpand;

		protected override void OnDragDrop(DragEventArgs e)
		{
			if(!_drag) return;
			base.OnDragDrop(e);
			string[] files = null;
			if(_dragNodes == null) {
				files = e.Data.GetData(DataFormats.FileDrop, false) as string[]; if(files == null) return;
			}
			_model._OnDragDrop(_dragNodes, files, 0 != (e.KeyState & 8), DropPosition);
		}

		protected override void OnDragLeave(EventArgs e)
		{
			if(!_drag) return;
			base.OnDragLeave(e);
		}

		#endregion
	}
}
