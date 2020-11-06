using Au;
using Au.Types;
using Au.Controls;
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
using System.Drawing;
using System.Linq;
using System.Windows.Input;

partial class FilesModel
{
	public class FilesView : AuTreeView
	{
		public FilesView()
		{
			this.MultiSelect = true;
			this.AllowDrop = true;

			App.Model.TreeControl = this;

			ItemActivated += _ItemActivated;
			ItemClick += _ItemClick;
		}

		public void SetItems() {
			base.SetItems(App.Model.Root.Children(), false);
		}

		private void _ItemActivated(object sender, TVItemEventArgs e) {
			var f = e.Item as FileNode;
			if (!f.IsFolder) App.Model._SetCurrentFile(f);
		}

		private void _ItemClick(object sender, TVItemEventArgs e) {
			if (e.ModifierKeys != 0) return;
			var f = e.Item as FileNode;
			switch (e.MouseButton) {
			case MouseButton.Right:
				Dispatcher.InvokeAsync(() => App.Model._ItemRightClicked(f));
				break;
			case MouseButton.Middle:
				if (!f.IsFolder) App.Model.CloseFile(f, true);
				break;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			App.Model._OnKeyDown(e);
		}

		//#region drag-drop

		//bool _drag;
		//TreeNodeAdv[] _dragNodes;

		//protected override void OnItemDrag(MouseButtons buttons, object item)
		//{
		//	if(buttons == MouseButtons.Left) DoDragDropSelectedNodes(DragDropEffects.Move | DragDropEffects.Copy);
		//	base.OnItemDrag(buttons, item);
		//}

		//protected override void OnDragEnter(DragEventArgs e)
		//{
		//	_drag = false; _dragNodes = null;
		//	e.Effect = 0;

		//	//can drop TreeNodeAdv and files
		//	TreeNodeAdv[] nodes = null;
		//	if(e.Data.GetDataPresent(typeof(TreeNodeAdv[]))) {
		//		nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
		//		if(nodes[0].Tree != this) return;
		//	} else if(e.Data.GetDataPresent(DataFormats.FileDrop, false)) {
		//	} else return;

		//	var effect = _GetEffect(e, nodes != null); if(effect == 0) return;
		//	e.Effect = effect;

		//	_drag = true; _dragNodes = nodes;

		//	base.OnDragEnter(e);
		//}

		//static DragDropEffects _GetEffect(DragEventArgs e, bool nodes)
		//{
		//	var effect = e.AllowedEffect;
		//	bool copy = 0 != (e.KeyState & 8);
		//	if(nodes) effect &= copy ? DragDropEffects.Copy : DragDropEffects.Move;
		//	else if(copy) effect &= DragDropEffects.Copy;
		//	return effect;
		//}

		//protected override void OnDragOver(DragEventArgs e)
		//{
		//	if(!_drag) return;
		//	//ADebug.PrintFunc();

		//	base.OnDragOver(e); //set drop position, auto-scroll

		//	e.Effect = 0;
		//	var effect = _GetEffect(e, _dragNodes != null); if(effect == 0) return;
		//	bool copy = 0 != (e.KeyState & 8);

		//	var nTarget = this.DropPosition.Node; if(nTarget == null) return;
		//	var fTarget = nTarget.Tag as FileNode;
		//	bool isFolder = fTarget.IsFolder;
		//	bool isInside = this.DropPosition.Position == NodePosition.Inside;

		//	//prevent selecting whole non-folder item. Make either above or below.
		//	if(isFolder) this.DragDropBottomEdgeSensivity = this.DragDropTopEdgeSensivity = 0.3f; //default
		//	else this.DragDropBottomEdgeSensivity = this.DragDropTopEdgeSensivity = 0.51f;

		//	//can drop here?
		//	if(!copy && _dragNodes != null) {
		//		foreach(TreeNodeAdv n in _dragNodes) {
		//			var f = n.Tag as FileNode;
		//			if(!f.CanMove(fTarget, (FNPosition)this.DropPosition.Position)) return;
		//		}
		//	}

		//	//expand-collapse folder on right-click. However this does not work when dragging files, because Explorer then ends the drag-drop.
		//	if(isFolder && isInside) {
		//		var ks = e.KeyState & 3;
		//		if(ks == 3 && _dragKeyStateForFolderExpand != 3) {
		//			if(nTarget.IsExpanded) nTarget.Collapse(); else nTarget.Expand();
		//		}
		//		_dragKeyStateForFolderExpand = ks;
		//	}

		//	e.Effect = effect;
		//}

		//int _dragKeyStateForFolderExpand;

		//protected override void OnDragDrop(DragEventArgs e)
		//{
		//	if(!_drag) return;
		//	base.OnDragDrop(e);
		//	string[] files = null;
		//	if(_dragNodes == null) {
		//		files = e.Data.GetData(DataFormats.FileDrop, false) as string[]; if(files == null) return;
		//	}
		//	_model._OnDragDrop(_dragNodes, files, 0 != (e.KeyState & 8), DropPosition);
		//}

		//protected override void OnDragLeave(EventArgs e)
		//{
		//	if(!_drag) return;
		//	base.OnDragLeave(e);
		//}

		//#endregion
	}
}
