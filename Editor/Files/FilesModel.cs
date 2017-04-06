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
using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;
using System.Collections;

using Catkeys;
using static Catkeys.NoClass;

using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

class FilesModel :ITreeModel
{
	public readonly TreeViewAdv TV;
	public readonly FileNode Root;
	public readonly XElement Xml;
	public readonly string CollectionFile;
	public readonly string FilesDirectory;
	public readonly Dictionary<string, FileNode> GuidMap;

	public FilesModel(TreeViewAdv c, string file)
	{
		TV = c;
		CollectionFile = file;
		this.FilesDirectory = Path.GetDirectoryName(CollectionFile) + @"\files";

		Xml = XElement.Load(CollectionFile);

		GuidMap = new Dictionary<string, FileNode>();
		Root = new FileNode(this, Xml, true); //recursively creates whole model tree

		_InitOpenSave();
		_InitClickSelect();
		_InitDragDrop();
	}

	public void Dispose()
	{
		//SaveCollectionNowIfDirty(); //owner FilesPanel calls this implicitly. It also calls Dispose.
		_UninitOpenSave();
		_UninitClickSelect();
		_UninitDragDrop();
		_UninitNodeControls();
	}

	#region open save collection

	void _InitOpenSave()
	{
		Program.Timer1s += _program_Timer1s;
		EForm.MainForm.VisibleChanged += _mainForm_VisibleChanged;
	}

	void _UninitOpenSave()
	{
		Program.Timer1s -= _program_Timer1s;
		EForm.MainForm.VisibleChanged -= _mainForm_VisibleChanged;
	}

	public bool SaveCollectionNow()
	{
		_saveAfterS = 0;
		try {
			//Print("saving");
			Perf.First();
			Xml.Save(CollectionFile);
			Perf.NW();
			return true;
		}
		catch(Exception ex) { //XElement.Save exceptions are undocumented
			TaskDialog.ShowError("Failed to save", CollectionFile, expandedText: ex.Message);
			_saveAfterS = 60;
			return false;
		}
	}

	public void SaveCollectionNowIfDirty()
	{
		if(_saveAfterS > 0) SaveCollectionNow();
	}

	public void SaveCollectionLater(int afterS = 5)
	{
		if(_saveAfterS < 1 || _saveAfterS > afterS) _saveAfterS = afterS;
	}

	int _saveAfterS;

	private void _program_Timer1s()
	{
		if(_saveAfterS > 0 && --_saveAfterS == 0) SaveCollectionNow();
	}

	private void _mainForm_VisibleChanged(object sender, EventArgs e)
	{
		if(!EForm.MainForm.Visible) SaveCollectionNowIfDirty();
	}

	//info: this is before Dispose
	//protected override void OnHandleDestroyed(EventArgs e)
	//{
	//	PrintFunc();
	//}

	#endregion

	#region node controls

	NodeIcon _ncIcon;
	NodeTextBox _ncName;

	public static Icons.FileIconCache IconCache = new Icons.FileIconCache(Folders.ThisAppDataLocal + @"fileIconCache.xml", (int)Icons.ShellSize.SysSmall);
	static Font _fontBold;

	//Called by FilesPanel
	public void InitNodeControls(NodeIcon icon, NodeTextBox name)
	{
		_ncIcon = icon;
		_ncName = name;
		_ncIcon.ValueNeeded = _ncIcon_ValueNeeded;
		_ncName.ValueNeeded = node => (node.Tag as FileNode).Name;
		_ncName.ValuePushed = (node, value) => { (node.Tag as FileNode).Rename(value as string, false); };
		_ncName.DrawText += _ncName_DrawText;
	}

	void _UninitNodeControls()
	{
		_ncName.DrawText -= _ncName_DrawText;
	}

	private object _ncIcon_ValueNeeded(TreeNodeAdv node)
	{
		var f = node.Tag as FileNode;
		//Print(f);
		bool leaf = node.IsLeaf;
#if true
		//var p = new Catkeys.Perf.Inst(true);
		if(leaf && f.Xml.Attribute_(out string s, "path") && !s.EndsWith_(".cs", true)) {
			//p.Next();
			var r = IconCache.GetImage(s, true);
			//r.SetResolution(96f, 96f);
			//PrintList(r.Size, r.PhysicalDimension, r.HorizontalResolution, r.VerticalResolution);
			//p.NW();
			return r;
		}
#endif
		return EResources.GetImage(leaf ? "CS_16x" : (node.IsExpanded ? "FolderOpen_16x" : "Folder_16x"));
	}

	private void _ncName_DrawText(object sender, DrawEventArgs e)
	{
		var f = e.Node.Tag as FileNode;
		if(f == _selectedFile) {
			if(_fontBold == null) _fontBold = new Font(TV.Font, FontStyle.Bold);
			e.Font = _fontBold;
			//e.TextColor = Color.DarkBlue;
		}
	}
	//Brush _brushOpen;
	//private void _controlName_DrawText(object sender, DrawEventArgs e)
	//{
	//	var f = e.Node.Tag as FileNode;
	//	if(f == _selectedFile) {
	//		if(_brushOpen == null) _brushOpen = new SolidBrush(Color.Beige);
	//		e.BackgroundBrush = _brushOpen;
	//		e.TextColor = Color.DarkBlue;
	//	}
	//}

	#endregion

	#region ITreeModel

	public IEnumerable GetChildren(object nodeTag)
	{
		if(nodeTag == null) return Root.Children;
		var f = nodeTag as FileNode;
		//PrintList("GetChildren", f);
		return f.Children;
	}

	public bool IsLeaf(object nodeTag)
	{
		var f = nodeTag as FileNode;
		//PrintList("IsLeaf", f);
		return !f.HasChildren;
	}

	public event EventHandler<TreeModelEventArgs> NodesChanged;
	public event EventHandler<TreeModelEventArgs> NodesInserted;
	public event EventHandler<TreeModelEventArgs> NodesRemoved;
	public event EventHandler<TreePathEventArgs> StructureChanged;

	/// <summary>
	/// Call this to update control view when changed node data (text, image, checked, color, etc).
	/// </summary>
	/// <param name="f"></param>
	/// <param name="justInvalidateRow">Just invalidate row rectangle. Much faster. Use when don't need to change row height.</param>
	internal void OnNodeChanged(FileNode f, bool justInvalidateRow = false)
	{
		if(justInvalidateRow) TV.UpdateNode(f.TreeNodeAdv);
		else NodesChanged?.Invoke(this, _TreeModelEventArgs(f));
	}

	internal void OnNodeRemoved(FileNode f)
	{
		NodesRemoved?.Invoke(this, _TreeModelEventArgs(f));
	}

	internal void OnNodeInserted(FileNode f)
	{
		NodesInserted?.Invoke(this, _TreeModelEventArgs(f));
	}

	public void OnStructureChanged()
	{
		StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
	}

	TreeModelEventArgs _TreeModelEventArgs(FileNode node)
	{
		return new TreeModelEventArgs(node.Parent.TreePath, new int[] { node.Index }, new object[] { node });
	}

	#endregion

	#region find

	public FileNode FindFile(string name)
	{
		return Root.FindDescendantFile(name);
	}

	public FileNode FindFolder(string name)
	{
		return Root.FindDescendantFolder(name);
	}

	public FileNode FindFileOrFolder(string name)
	{
		return Root.FindDescendantFileOrFolder(name);
	}

	public FileNode FindByGUID(string guid)
	{
		if(GuidMap.TryGetValue(guid, out var f)) return f;
		DebugPrint("GUID not found: " + guid);
		return null;
	}

	#endregion

	#region click node

	FileNode _selectedFile;

	void _InitClickSelect()
	{
		TV.NodeMouseClick += _TV_NodeMouseClick;
		TV.SelectionChanged += _TV_SelectionChanged;
	}

	void _UninitClickSelect()
	{
		TV.NodeMouseClick -= _TV_NodeMouseClick;
		TV.SelectionChanged -= _TV_SelectionChanged;
	}

	private void _TV_SelectionChanged(object sender, SelectionReason reason)
	{
		//return;
		//Print(reason); return;
		//PrintList(TV.SelectedNode, TV.CurrentNode);
		_selectOnClick_MouseButton = 0;
		switch(reason) {
		case SelectionReason.NoSelection: case SelectionReason.Multi: return;
		case SelectionReason.Other: break;
		default: if((Control.ModifierKeys & Keys.Alt) != 0) return; break; //mouse, key
		}
		Debug.Assert(TV.SelectedNodes.Count == 1);

		var f = TV.SelectedNode.Tag as FileNode;
		switch(reason) {
		case SelectionReason.LeftClick:
			_selectOnClick_MouseButton = MouseButtons.Left;
			_selectOnClick_FileNode = f;
			break;
		case SelectionReason.RightClick:
			_selectOnClick_MouseButton = MouseButtons.Right;
			_selectOnClick_FileNode = f;
			break;
		default:
			_SelectItem(f);
			break;
		}
	}

	//Variables to save selected node and mouse button that are received in SelectionChanged event.
	//The event usually is on mouse button down, but we want to defer our action until NodeMouseClick event (on mouse button up).
	MouseButtons _selectOnClick_MouseButton;
	FileNode _selectOnClick_FileNode;

	private void _TV_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
	{
		//Print(e.ControlBounds);
		//Print(_selectOnClick_MouseButton);
		if(_selectOnClick_MouseButton != 0) {
			var mb = _selectOnClick_MouseButton; _selectOnClick_MouseButton = 0;
			var f = _selectOnClick_FileNode; _selectOnClick_FileNode = null;
			//var fNow = TV.SelectedNode?.Tag;
			var fNow = e.Node.Tag;
			if(fNow == f) { //if still the same as was in _TV_SelectionChanged event, usually on mouse button down
				switch(mb) {
				case MouseButtons.Left:
					_SelectItem(f);
					break;
				case MouseButtons.Right:
					_ItemRightClicked(f);
					break;
				}
			}
		}
	}

	void _SelectItem(FileNode f)
	{
		if(f == _selectedFile) return;
		//Print(f);
		if(f.IsFolder) {
		} else {
			try {
				var s = File.ReadAllText(f.FilePath);
				EForm.MainForm.Code.Text = s;
			}
			catch(Exception ex) {
				Print(ex);
				return;
			}
			var sfPrev = _selectedFile;
			_selectedFile = f;
			if(sfPrev != null) TV.UpdateNode(sfPrev.TreeNodeAdv);
			TV.UpdateNode(f.TreeNodeAdv);
		}
	}

	void _ItemRightClicked(FileNode f)
	{
		if(f != _selectedFile) {
			var m = new CatMenu();
			m["One"] = o => Print(1);
			m["Two"] = o => Print(2);
			//m.ModalAlways = true; //does not work
			m.CMS.Closed += (unu, sed) => { if(_selectedFile == null) TV.ClearSelection(); else _selectedFile.Select(); };
			m.Show();
			//if(_selectedFile == null) TV.ClearSelection(); else _selectedFile.Select();
		}
	}

	#endregion

	#region drag-drop

	void _InitDragDrop()
	{
		TV.ItemDrag += _TV_ItemDrag;
		TV.DragOver += _TV_DragOver;
		TV.DragDrop += _TV_DragDrop;
	}

	void _UninitDragDrop()
	{
		TV.ItemDrag -= _TV_ItemDrag;
		TV.DragOver -= _TV_DragOver;
		TV.DragDrop -= _TV_DragDrop;
	}

	private void _TV_ItemDrag(object sender, ItemDragEventArgs e)
	{
		TV.DoDragDropSelectedNodes(DragDropEffects.Move);
	}

	private void _TV_DragOver(object sender, DragEventArgs e)
	{
		_selectOnClick_MouseButton = 0;
		e.Effect = DragDropEffects.None;
		if(!e.Data.GetDataPresent(typeof(TreeNodeAdv[]))) return;
		TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
		if(nodes[0].Tree != TV) return;
		var nTarget = TV.DropPosition.Node;
		if(nTarget == null) {
			//if(!TV.Wnd_().ClientRectInScreen.Contains(e.X, e.Y)) return;
			return;
		} else {
			var fTarget = nTarget.Tag as FileNode;
			bool isFolder = fTarget.IsFolder;
			bool isInside = TV.DropPosition.Position == NodePosition.Inside;

			if(isFolder) TV.DragDropBottomEdgeSensivity = TV.DragDropTopEdgeSensivity = 0.3f;
			else TV.DragDropBottomEdgeSensivity = TV.DragDropTopEdgeSensivity = 0.51f;

			foreach(TreeNodeAdv n in nodes) {
				var f = n.Tag as FileNode;
				if(!f.CanMove(fTarget, TV.DropPosition.Position)) return;
			}

			if(isFolder && isInside) {
				var ks = e.KeyState & 3;
				if(ks == 3 && _dragKeyStateForFolderExpand != 3) {
					if(nTarget.IsExpanded) nTarget.Collapse(); else nTarget.Expand();
				}
				_dragKeyStateForFolderExpand = ks;
			}
		}

		e.Effect = e.AllowedEffect;
	}

	int _dragKeyStateForFolderExpand;

	private void _TV_DragDrop(object sender, DragEventArgs e)
	{
		TV.BeginUpdate();
		try {
			TreeNodeAdv[] nodes = (TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]));
			var nTarget = TV.DropPosition.Node;
			var fTarget = nTarget.Tag as FileNode;
			bool droppedTheSelectedFile = false;
			foreach(TreeNodeAdv n in nodes) {
				var f = n.Tag as FileNode;
				f.Move(fTarget, TV.DropPosition.Position);
				if(f == _selectedFile) droppedTheSelectedFile = true;
			}
			if(droppedTheSelectedFile) {
				if(TV.DropPosition.Position == NodePosition.Inside) nTarget.IsExpanded = true;
				_selectedFile.Select();
			}
		}
		catch(Exception ex) { DebugPrint(ex); }
		finally { TV.EndUpdate(); }
	}

	#endregion

	//public void Clear()
	//{
	//	Root.Remove();
	//}
}
