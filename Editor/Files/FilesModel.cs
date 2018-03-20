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
using System.Linq;
using System.Xml.Linq;
//using System.Xml.XPath;
using System.Collections;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

partial class FilesModel :ITreeModel
{
	public readonly TreeViewAdv TV;
	public readonly FileNode Root;
	public readonly XElement Xml;
	public readonly string CollectionFile;
	public readonly string FilesDirectory;
	public readonly string StateFile;
	public readonly Dictionary<string, FileNode> GuidMap;
	public readonly List<FileNode> OpenFiles;
	public readonly AutoSave Save;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="c">Tree control. Can be null, for example when importing collection.</param>
	/// <param name="file">Collection file (XML).</param>
	/// <exception cref="ArgumentException">Invalid or not full path.</exception>
	/// <exception cref="Exception">XElement.Load exceptions. And possibly more.</exception>
	public FilesModel(TreeViewAdv c, string file)
	{
		TV = c;
		CollectionFile = Path_.Normalize(file);
		var colDir = Path_.GetDirectoryPath(CollectionFile, true);
		this.FilesDirectory = colDir + "files";
		Files.CreateDirectory(this.FilesDirectory);
		this.StateFile = colDir + "state.xml";

		Xml = XElement.Load(CollectionFile);

		GuidMap = new Dictionary<string, FileNode>();
		Root = new FileNode(this, Xml, true); //recursively creates whole model tree

		if(TV != null) { //null when importing
			OpenFiles = new List<FileNode>();
			Save = new AutoSave(this);
			_InitClickSelect();
			_InitDragDrop();
			_InitWatcher();
		}

		//var r = Root;
		//Print(r.Name, r.Name == null);
		//Print(r.Parent, r.Parent == null);
		//Print(r.Index);
		//Print(r.ItemPath, r.ItemPath== null);
		//Print(r.FilePath);
	}

	public void Dispose()
	{
		if(Save != null) { //null when importing

			//Save.AllNowIfNeed(); //owner FilesPanel calls this before calling this func. Because may need more code in between.
			Save.Dispose();
			_UninitWatcher();
			_UninitClickSelect();
			_UninitDragDrop();
			_UninitNodeControls();
		}
	}

	//void _Clear()
	//{
	//	Root.Xml.RemoveNodes();
	//	GuidMap.Clear();
	//	OnStructureChanged();
	//}

	#region node controls

	NodeIcon _ncIcon;
	NodeTextBox _ncName;

	public static Icons.FileIconCache IconCache = new Icons.FileIconCache(Folders.ThisAppDataLocal + @"fileIconCache.xml", (int)Icons.ShellSize.SysSmall);

	//Called by FilesPanel
	public void InitNodeControls(NodeIcon icon, NodeTextBox name)
	{
		_ncIcon = icon;
		_ncName = name;
		_ncIcon.ValueNeeded = _ncIcon_ValueNeeded;
		_ncName.ValueNeeded = node => (node.Tag as FileNode).Name;
		_ncName.ValuePushed = (node, value) => { (node.Tag as FileNode).FileRename(value as string, false); };
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

		if(_myClipboard.Contains(f)) return EResources.GetImageUseCache("cut");

		bool leaf = node.IsLeaf;
#if true
		//var p = new Perf.Inst(true);
		if(leaf) {
			bool isPath = f.IsLink(out string s);
			if(!(isPath ? s : f.Name).EndsWith_(".cs", true)) {
				//p.Next();
				if(!isPath) s = f.FilePath;
				var r = IconCache.GetImage(s, true);
				//r.SetResolution(96f, 96f);
				//Print(r.Size, r.PhysicalDimension, r.HorizontalResolution, r.VerticalResolution);
				//p.NW();
				return r;
			}
		}
#endif
		return EResources.GetImageUseCache(leaf ? "CS_16x" : (node.IsExpanded ? "FolderOpen_16x" : "Folder_16x"));
	}

	private void _ncName_DrawText(object sender, DrawEventArgs e)
	{
		var f = e.Node.Tag as FileNode;
		if(f == _currentFile) {
			e.Font = Stock.FontBold;
			//e.TextColor = Color.DarkBlue;
		}
	}
	//Brush _brushOpen;
	//private void _controlName_DrawText(object sender, DrawEventArgs e)
	//{
	//	var f = e.Node.Tag as FileNode;
	//	if(f == _currentFile) {
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
		//Print("GetChildren", f);
		return f.Children;
	}

	public bool IsLeaf(object nodeTag)
	{
		var f = nodeTag as FileNode;
		//Print("IsLeaf", f);
		return !f.IsFolder;
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

	public FileNode FindFileOrFolder(string name)
	{
		return Root.FindDescendantFileOrFolder(name);
	}

	public FileNode FindFile(string name)
	{
		return Root.FindDescendantFile(name);
	}

	public FileNode FindFolder(string name)
	{
		return Root.FindDescendantFolder(name);
	}

	public FileNode FindByGUID(string guid)
	{
		if(GuidMap.TryGetValue(guid, out var f)) return f;
		Debug_.Print("GUID not found: " + guid);
		return null;
	}

	#endregion

	#region click, select, current, selected

	void _InitClickSelect()
	{
		TV.NodeMouseClick += _TV_NodeMouseClick;
		TV.SelectionChanged += _TV_SelectionChanged;
		TV.KeyDown += _TV_KeyDown;
		TV.Expanded += _TV_Expanded;
		TV.Collapsed += _TV_Expanded;
	}

	void _UninitClickSelect()
	{
		TV.NodeMouseClick -= _TV_NodeMouseClick;
		TV.SelectionChanged -= _TV_SelectionChanged;
		TV.KeyDown -= _TV_KeyDown;
		TV.Expanded -= _TV_Expanded;
		TV.Collapsed -= _TV_Expanded;
	}

	private void _TV_SelectionChanged(object sender, SelectionReason reason)
	{
		_selectOnClick_FileNode = null;
		switch(reason) {
		case SelectionReason.NoSelection: return;
		case SelectionReason.Multi:
			_selectOnClick_multi = !_disableSetCurrentFile && TV.SelectedNodes.Count > 1 && Control.MouseButtons == MouseButtons.Left;
			return;
		case SelectionReason.Other: break;
		default: //mouse, key
			if((Control.ModifierKeys & Keys.Alt) != 0) return;
			break;
		}
		Debug.Assert(TV.SelectedNodes.Count == 1);

		var f = TV.SelectedNode.Tag as FileNode;
		switch(reason) {
		case SelectionReason.LeftClick:
			if(Control.MouseButtons != MouseButtons.None) _selectOnClick_FileNode = f;
			//else _SetCurrentFile(f); //in some cases this is called on button up, after the click event, eg when clicked a selected item in multi-selection (unselected others)
			break;
		case SelectionReason.RightClick:
			break;
		default: //key, other
			_SetCurrentFile(f);
			break;
		}

		//TODO: don't use this to select file etc. Select only on click and key.
	}

	//Variable to save selected node received in SelectionChanged event.
	//The event usually is on mouse button down, but we want to defer our action until NodeMouseClick event (on mouse button up).
	FileNode _selectOnClick_FileNode;

	//to prevent Ctrl+drag multiple unintended
	bool _selectOnClick_multi;

	private void _TV_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
	{
		//Print(e.Button, e.ModifierKeys);
		_selectOnClick_multi = false;
		if(e.ModifierKeys != 0) return;
		var f = e.Node.Tag as FileNode;
		switch(e.Button) {
		case MouseButtons.Left:
			if(_selectOnClick_FileNode != null) {
				var fSel = _selectOnClick_FileNode; _selectOnClick_FileNode = null;
				if(f == fSel) { //if the same as was in _TV_SelectionChanged event, usually on mouse button down
					_SetCurrentFile(f);
				}
			}
			break;
		case MouseButtons.Right:
			//_ItemRightClicked(f);
			TV.BeginInvoke(new Action(() => _ItemRightClicked(f)));
			break;
		case MouseButtons.Middle:
			CloseFile(f);
			break;
		}
	}

	/// <summary>
	/// Closes an open file.
	/// If f is open, closes it.
	/// Saves if need, removes from OpenItems, deselects in treeview.
	/// When closing current file, does not select another.
	/// </summary>
	/// <param name="f">Can be any item or null. Does nothing if it is null, folder or not open.</param>
	public void CloseFile(FileNode f)
	{
		if(f == null) return;

		if(f == _currentFile) _SetCurrentFile(null);
		else if(OpenFiles.Remove(f)) {
			Panels.Open.UpdateList();
			Panels.Open.UpdateCurrent(_currentFile);
			Save.StateLater();
		} else return;

		SelectDeselectItem(f, false); //else cannot click-open again
	}

	FileNode _currentFile;
	bool _disableSetCurrentFile;

	/// <summary>
	/// Closes current file in editor, saves its text.
	/// If f is not null, opens it in editor, adds to OpenFiles.
	/// Sets _currentFile, saves state, updates UI.
	/// </summary>
	/// <param name="f">
	/// Can be any.
	/// Does nothing if f is folder or f==_currentFile or _disableSetCurrentFile==true.
	/// If null, closes current and removes from OpenFiles.</param>
	void _SetCurrentFile(FileNode f)
	{
		if(_disableSetCurrentFile && _currentFile != null) return;
		if(f == _currentFile) return;
		//Print(f);
		if(f != null && f.IsFolder) return;

		Save.TextNowIfNeed();
		if(!Panels.Editor.Open(f)) {
			if(OpenFiles.Remove(f)) {
				Panels.Open.UpdateList();
				Save.StateLater();
			}
			return;
		}

		var fPrev = _currentFile;
		_currentFile = f;

		if(fPrev != null) TV.UpdateNode(fPrev.TreeNodeAdv);

		if(f != null) {
			TV.UpdateNode(f.TreeNodeAdv);

			var of = OpenFiles;
			int iLast = of.Count - 1;
			if(!(iLast >= 0 && of[iLast] == f)) {
				of.Remove(f);
				of.Add(f);
				Panels.Open.UpdateList();
			}
		} else {
			OpenFiles.Remove(fPrev);
			Panels.Open.UpdateList();
		}

		Panels.Open.UpdateCurrent(f);

		Save.StateLater();
	}

	/// <summary>
	/// Gets the current file (which is open in the currently ative code editor).
	/// </summary>
	public FileNode CurrentFile
	{
		get => _currentFile;
	}

	/// <summary>
	/// Selects the node and opens its file in the code editor.
	/// </summary>
	/// <param name="f">Can be null to set 'no current file, empty editor'.</param>
	/// <param name="doNotChangeSelection"></param>
	public void SetCurrentFile(FileNode f, bool doNotChangeSelection = false)
	{
		if(!doNotChangeSelection && f != null) {
			_disableSetCurrentFile = true;
			f.SelectSingle();
			_disableSetCurrentFile = false;
		}
		if(_currentFile != f) _SetCurrentFile(f);
	}

	public FileNode[] SelectedItems { get => TV.SelectedNodes.Select(tn => tn.Tag as FileNode).ToArray(); }

	/// <summary>
	/// Selects or deselects item in treeview. Does not set current file etc. Does not deselect others.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="select"></param>
	public void SelectDeselectItem(FileNode f, bool select)
	{
		_disableSetCurrentFile = true; //note: even when deselecting, if there is single selected item left, would set current file
		f.TreeNodeAdv.IsSelected = select;
		_disableSetCurrentFile = false;
	}

	void _ItemRightClicked(FileNode f)
	{
		var m = Strips.ddFile;

		ToolStripDropDownClosedEventHandler onClosed = null;
		onClosed = (sender, e) =>
		  {
			  (sender as ToolStripDropDownMenu).Closed -= onClosed;
			  _msgLoop.Stop();
		  };
		m.Closed += onClosed;

		m.ShowAsContextMenu_();
		_msgLoop.Loop();
		if(TV.SelectedNodes.Count < 2) {
			if(_currentFile == null) TV.ClearSelection(); else _currentFile.SelectSingle();
		}
	}
	Au.Util.MessageLoop _msgLoop = new Au.Util.MessageLoop();

	private void _TV_Expanded(object sender, TreeViewAdvEventArgs e)
	{
		if(e.Node.Level == 0) return;
		Save.StateLater();
	}

	#endregion

	#region hotkeys, new, delete, open/close, cut/copy/paste

	private void _TV_KeyDown(object sender, KeyEventArgs e)
	{
		switch(e.KeyData) {
		case Keys.Delete: DeleteSelected(); break;
		case Keys.Control | Keys.X: CutCopySelected(true); break;
		case Keys.Control | Keys.C: CutCopySelected(false); break;
		case Keys.Control | Keys.V: Paste(); break;
		}
	}

	/// <summary>
	/// Gets the place where item should be added in operations such as new, paste, import.
	/// </summary>
	FileNode _GetInsertPos(out NodePosition pos, bool askIntoFolder = false)
	{
		FileNode r;
		var c = TV.CurrentNode;
		if(c == null) { //probably empty treeview
			r = Root;
			pos = NodePosition.Inside;
		} else {
			r = c.Tag as FileNode;
			if(askIntoFolder && r.IsFolder && c.IsSelected && TV.SelectedNodes.Count == 1 && AuDialog.ShowYesNo("Into the folder?", owner: TV)) pos = NodePosition.Inside;
			else if(r.Next == null) pos = NodePosition.After; //usually we want to add after the last, not before
			else pos = NodePosition.Before;
		}
		return r;
	}

	public void NewItem(FileNode.NewItemTemplate template)
	{
		var target = _GetInsertPos(out var pos);
		var f = FileNode.NewItem(this, target, pos, template);
		if(f == null) return;
		if(template != FileNode.NewItemTemplate.Folder) SetCurrentFile(f);
	}

	public void RenameSelected()
	{
		//if(TV.SelectedNodes.Count != 1) return; //let edit current node, like F2 does
		(TV.NodeControls[1] as NodeTextBox).BeginEdit();
	}

	public void DeleteSelected()
	{
		var a = SelectedItems; if(a.Length < 1) return;

		//confirmation
		bool isLink = false; foreach(var f in a) if(f.IsLink()) isLink = true;
		var r = AuDialog.ShowEx("Deleting",
			string.Join("\n", a.Select(f => f.IsLink(out var target) ? $"{f.Name} ({target})" : f.Name))
			+ "\n\nThe file will be moved to the Recycle Bin, if possible.",
			"1 OK|0 Cancel", owner: TV, checkBox: "Don't delete file" + (isLink ? "|check" : null));
		if(r.Button == 0) return;

		//get paths of files to delete
		string[] paths = null;
		if(!r.IsChecked) paths = a.Select(f => f.FilePath).ToArray();

		//close file (if folder - descendants), delete XML, don't delete files now
		foreach(var f in a) {
			f.FileDelete(true, true); //info: and saves everything, now and/or later
		}

		//delete files
		if(paths != null) {
			//Delete in another thread, because deleting to the Recycle Bin is very slow, it's annoying.
			//	Using single call for multiple files is faster, but still too slow.
			//	If fails, it's not so important, therefore this can be after deleting XML.
			Task.Run(() =>
			{
				try { Files.Delete(paths, true); }
				catch(Exception ex) { Print(ex.Message); return; }
			});
		}
	}

	public void CutCopySelected(bool cut)
	{
		if(!_myClipboard.Set(SelectedItems, cut)) return;
		//var d = new DataObject(string.Join("\r\n", _cutCopyNodes.Select(f => f.Name)));
		//Clipboard.SetDataObject(d);
		Clipboard.SetText(string.Join("\r\n", _myClipboard.nodes.Select(f => f.Name)));
	}

	struct _MyClipboard
	{
		public FileNode[] nodes;
		public bool cut;

		public bool Set(FileNode[] nodes, bool cut)
		{
			if(nodes == null || nodes.Length == 0) { Clear(); return false; }
			this.nodes = nodes;
			this.cut = cut;
			nodes[0].Control.Invalidate(); //draw "cut" icon
			return true;
		}

		public void Clear()
		{
			if(nodes == null) return;
			if(nodes.Length > 0) nodes[0].Control.Invalidate(); //was "cut" icon
			nodes = null;
		}

		public bool IsEmpty { get => nodes == null; }

		public bool Contains(FileNode f) { return !IsEmpty && nodes.Contains(f); }
	}
	_MyClipboard _myClipboard;

	public void Paste()
	{
		if(_myClipboard.IsEmpty) return;
		var target = _GetInsertPos(out var pos, true);
		_MultiCopyMove(!_myClipboard.cut, _myClipboard.nodes, target, pos);
		_myClipboard.Clear();
	}

	public void SelectedCopyPath(bool full)
	{
		var a = SelectedItems; if(a.Length == 0) return;
		Clipboard.SetText(string.Join("\r\n", a.Select(f => full ? f.FilePath : f.ItemPath)));
	}

	/// <summary>
	/// Opens the selected item(s) in our editor or in default app or selects in Explorer.
	/// </summary>
	/// <param name="how">1 open, 2 open in new window (not impl), 3 open in default app, 4 select in Explorer.</param>
	public void OpenSelected(int how)
	{
		var a = SelectedItems; if(a.Length == 0) return;
		foreach(var f in a) {
			switch(how) {
			case 1:
				if(f.IsFolder) f.TreeNodeAdv.Expand();
				else this.SetCurrentFile(f);
				break;
			case 2:
				if(f.IsFolder) continue;
				//FUTURE
				break;
			case 3:
				Shell.Run(f.FilePath);
				break;
			case 4:
				Shell.SelectFileInExplorer(f.FilePath);
				break;
			}
		}
	}

	/// <summary>
	/// Closes selected or all items, or collapses folders.
	/// Used to implement menu File -> Open/Close.
	/// </summary>
	/// <param name="how">1 close selected file(s) and current file, 2 close all, 3 collapse folders.</param>
	/// <remarks>
	/// When how is 1: Closes selected files. If there are no selected files, closes current file. Does not collapse selected folders.
	/// When how is 2: Closes all files and collapses folders.
	/// </remarks>
	public void CloseEtc(int how)
	{
		switch(how) {
		case 1:
			bool isSelected = false;
			foreach(var f in SelectedItems) {
				if(!f.IsFolder) { CloseFile(f); isSelected = true; }
			}
			if(!isSelected) CloseFile(_currentFile);
			break;
		case 2:
			foreach(var f in OpenFiles.ToArray()) CloseFile(f);
			TV.CollapseAll();
			break;
		case 3:
			TV.CollapseAll();
			break;
		}
	}

	#endregion

	#region import, move, copy

	/// <summary>
	/// Imports one or more files into the collection.
	/// </summary>
	/// <param name="a">Files. If null, shows dialog to select files.</param>
	public void ImportFiles(string[] a = null)
	{
		if(a == null) {
			Print("Info: To import files, you can also drag and drop from a folder window.");
			var d = new OpenFileDialog();
			d.Multiselect = true;
			d.Title = "Import files to the collection";
			if(d.ShowDialog(MainForm) != DialogResult.OK) return;
			a = d.FileNames;
		}

		var target = _GetInsertPos(out var pos);
		_ImportFiles(false, a, target, pos);
	}

	/// <summary>
	/// Imports another collection into this collection.
	/// </summary>
	/// <param name="collDir">Collection directory. If null, shows dialog to select.</param>
	/// <param name="target">If null, calls _GetInsertPos.</param>
	/// <param name="pos">Used when target is not null.</param>
	public void ImportCollection(string collDir = null, FileNode target = null, NodePosition pos = 0)
	{
		string xmlFile;
		if(collDir != null) xmlFile = collDir + @"\files.xml";
		else {
			var d = new OpenFileDialog() { Title = "Import collection", Filter = "files.xml|files.xml" };
			if(d.ShowDialog(MainForm) != DialogResult.OK) return;
			collDir = Path_.GetDirectoryPath(xmlFile = d.FileName);
		}

		try {
			//create new folder for collection's items
			if(target == null) target = _GetInsertPos(out pos);
			target = FileNode.NewItem(this, target, pos, FileNode.NewItemTemplate.Folder, Path_.GetFileName(collDir));
			if(target == null) return;

			var m = new FilesModel(null, xmlFile);
			var a = m.Xml.Elements().Select(t => FileNode.FromX(t)).ToArray();
			_MultiCopyMove(true, a, target, NodePosition.Inside, true);
			m.Dispose();

			target.SelectSingle(); //TODO: async, because if rightclicked, will restore selection

			Print($"Info: Imported collection '{collDir}' to folder '{target.Name}'.\r\n\t{GetSecurityInfo()}");
		}
		catch(Exception ex) { Print(ex.Message); }
	}

	void _MultiCopyMove(bool copy, FileNode[] a, FileNode target, NodePosition pos, bool importingCollection = false)
	{
		_disableSetCurrentFile = true;
		TV.ClearSelection();
		TV.BeginUpdate();
		try {
			bool movedCurrentFile = false;
			var a2 = new List<FileNode>(a.Length);
			foreach(var f in (pos == NodePosition.After) ? a.Reverse() : a) {
				if(!importingCollection && !this.IsMyFileNode(f)) continue; //deleted?
				if(copy) {
					var fCopied = f.FileCopy(target, pos, this);
					if(fCopied != null) a2.Add(fCopied);
				} else {
					if(!f.FileMove(target, pos)) continue;
					a2.Add(f);
					if(!movedCurrentFile && _currentFile != null) {
						if(f == _currentFile || (f.IsFolder && f.ContainsDescendant(_currentFile))) movedCurrentFile = true;
					}
				}
			}
			if(movedCurrentFile) TV.EnsureVisible(_currentFile.TreeNodeAdv);
			if(pos != NodePosition.Inside || target.TreeNodeAdv.IsExpanded) {
				foreach(var f in a2) f.TreeNodeAdv.IsSelected = true;
			}
		}
		catch(Exception ex) { Print(ex.Message); }
		finally { TV.EndUpdate(); _disableSetCurrentFile = false; }

		//info: don't need to schedule saving here. FileCopy and FileMove did it.
	}

	void _ImportFiles(bool copy, string[] a, FileNode target, NodePosition pos)
	{
		bool fromCollectionDir = false, dirsDropped = false;
		for(int i = 0; i < a.Length; i++) {
			var s = a[i] = Path_.Normalize(a[i]);
			if(s.IndexOf_(@"\$RECYCLE.BIN\", true) > 0) {
				AuDialog.ShowEx("Files from Recycle Bin", $"At first restore the file to the <a href=\"{this.FilesDirectory}\">collection folder</a> or other normal folder.",
					icon: DIcon.Info, owner: TV, onLinkClick: e => Shell.TryRun(e.LinkHref));
				return;
			}
			var fd = this.FilesDirectory;
			if(!fromCollectionDir) {
				if(s.StartsWith_(fd, true) && (s.Length == fd.Length || s[fd.Length] == '\\')) fromCollectionDir = true;
				else if(!dirsDropped) dirsDropped = Files.ExistsAsDirectory(s);
			}
		}
		int r;
		if(copy) {
			if(fromCollectionDir) {
				AuDialog.ShowInfo("Files from collection folder", "Ctrl not supported."); //not implemented
				return;
			}
			r = 2; //copy
		} else if(fromCollectionDir) {
			r = 3; //move
		} else {
			string ins1 = dirsDropped ? "\nFolders not supported." : null;
			r = AuDialog.ShowEx("Import files", string.Join("\n", a),
			$"1 Add as a link to the external file{ins1}|2 Copy to the collection folder|3 Move to the collection folder|0 Cancel",
			flags: DFlags.CommandLinks | DFlags.Wider, owner: TV, footerText: GetSecurityInfo(true));
			if(r == 0) return;
		}

		var newParent = (pos == NodePosition.Inside) ? target : target.Parent;
		bool select = pos != NodePosition.Inside || target.TreeNodeAdv.IsExpanded;
		_disableSetCurrentFile = true;
		if(select) TV.ClearSelection();
		TV.BeginUpdate();
		try {
			var newParentPath = newParent.FilePath;
			int nf1 = Xml.Descendants("f").Count(), nd1 = Xml.Descendants("d").Count();

			foreach(var s in a) {
				bool isDir;
				var itIs = Files.ExistsAs2(s, true);
				if(itIs == FileDir2.File) isDir = false;
				else if(itIs == FileDir2.Directory && r != 1) isDir = true;
				else continue; //skip symlinks or if does not exist

				var name = Path_.GetFileName(s);
				var x = new XElement(isDir ? "d" : "f", new XAttribute("n", name));
				if(r == 1) {
					x.SetAttributeValue("path", s); //CONSIDER: unexpand
				} else {
					//var newPath = newParentPath + "\\" + name;
					if(fromCollectionDir) { //already exists?
						var relPath = s.Substring(this.FilesDirectory.Length);
						var fExists = this.FindFileOrFolder(relPath);
						if(fExists != null) {
							fExists.FileMove(target, pos);
							continue;
						}
					}
					if(isDir) _AddDirToXml(s, x);
					try {
						if(r == 2) Files.CopyTo(s, newParentPath, IfExists.Fail);
						else Files.MoveTo(s, newParentPath, IfExists.Fail);
					}
					catch(Exception ex) { Print(ex.Message); continue; }
				}
				var k = new FileNode(this, x);
				target.AddChildOrSibling(k, pos);
				if(select) k.TreeNodeAdv.IsSelected = true;
			}

			int nf2 = Xml.Descendants("f").Count(), nd2 = Xml.Descendants("d").Count();
			int nf = nf2 - nf1, nd = nd2 - nd1;
			if(nf + nd > 0) Print($"Info: Imported {nf} files and {nd} folders.\r\n\t{GetSecurityInfo()}");
		}
		catch(Exception ex) { Print(ex.Message); }
		finally { TV.EndUpdate(); _disableSetCurrentFile = false; }
		Save.CollectionLater();

		void _AddDirToXml(string path, XElement x)
		{
			foreach(var u in Files.EnumDirectory(path, FEFlags.UseRawPath | FEFlags.SkipHiddenSystem)) {
				bool isDir = u.IsDirectory;
				var x2 = new XElement(isDir ? "d" : "f", new XAttribute("n", u.Name));
				x.Add(x2);
				if(isDir) _AddDirToXml(u.FullPath, x2);
			}

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
		if(e.Button != MouseButtons.Left) return;
		if(_selectOnClick_multi) return; //prevent Ctrl+drag multiple unintended
		TV.DoDragDropSelectedNodes(DragDropEffects.Move | DragDropEffects.Copy);
	}

	private void _TV_DragOver(object sender, DragEventArgs e)
	{
		_selectOnClick_FileNode = null;

		e.Effect = DragDropEffects.None;
		var effect = e.AllowedEffect;
		bool copy = (e.KeyState & 8) != 0;
		if(copy) effect &= ~(DragDropEffects.Link | DragDropEffects.Move);
		if(0 == (effect & (DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Copy))) return;

		var nTarget = TV.DropPosition.Node; if(nTarget == null) return;

		//can drop TreeNodeAdv and files
		TreeNodeAdv[] nodes = null;
		if(e.Data.GetDataPresent(typeof(TreeNodeAdv[]))) {
			nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
			if(nodes?[0].Tree != TV) return;
			if(!copy) effect &= ~DragDropEffects.Copy;
		} else if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
		} else return;

		var fTarget = nTarget.Tag as FileNode;
		bool isFolder = fTarget.IsFolder;
		bool isInside = TV.DropPosition.Position == NodePosition.Inside;

		//prevent selecting whole non-folder item. Make either above or below.
		if(isFolder) TV.DragDropBottomEdgeSensivity = TV.DragDropTopEdgeSensivity = 0.3f; //default
		else TV.DragDropBottomEdgeSensivity = TV.DragDropTopEdgeSensivity = 0.51f;

		//can drop here?
		if(!copy && nodes != null) {
			foreach(TreeNodeAdv n in nodes) {
				var f = n.Tag as FileNode;
				if(!f.CanMove(fTarget, TV.DropPosition.Position)) return;
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

	private void _TV_DragDrop(object sender, DragEventArgs e)
	{
		bool copy = (e.KeyState & 8) != 0;
		var pos = TV.DropPosition.Position;
		var target = TV.DropPosition.Node.Tag as FileNode;
		if(e.Data.GetDataPresent(typeof(TreeNodeAdv[]))) {
			var a = (e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[]).Select(tn => tn.Tag as FileNode).ToArray();
			_MultiCopyMove(copy, a, target, pos);
		} else if(e.Data.GetDataPresent(DataFormats.FileDrop)) {
			var a = (string[])e.Data.GetData(DataFormats.FileDrop);
			if(a.Length == 1 && IsCollectionDirectory(a[0])) {
				switch(AuDialog.ShowEx("Collection", a[0],
					"1 Open collection|2 Import collection|0 Cancel",
					flags: DFlags.Wider, footerText: GetSecurityInfo(true))) {
				case 1: Timer_.After(1, t => Panels.Files.LoadCollection(a[0])); break;
				case 2: ImportCollection(a[0], target, pos); break;
				}
				return;
			}
			_ImportFiles(copy, a, target, pos);
		}
	}

	#endregion

	#region export

	/// <summary>
	/// Shows dialog to get path for new or exporting collection.
	/// Returns collection's directory path.
	/// Does not create any files/directories.
	/// </summary>
	/// <param name="name">Default name of the collection.</param>
	/// <param name="location">Default parent directory of the main directory of the collection.</param>
	public string GetDirectoryPathForNewCollection(string name = null, string location = null)
	{
		var f = new _FormNewCollection();
		f.textName.Text = name;
		f.textLocation.Text = location ?? Folders.ThisAppDocuments;
		if(f.ShowDialog(TV) != DialogResult.OK) return null;
		return f.textPath.Text;
	}

	public bool ExportSelected(string location = null)
	{
		var a = SelectedItems; if(a.Length < 1) return false;

		string name = a[0].Name; if(!a[0].IsFolder) name = Path_.GetFileNameWithoutExtension(name);

		if(a.Length == 1 && a[0].IsFolder && a[0].HasChildren) a = a[0].Children.ToArray();

		var collDir = GetDirectoryPathForNewCollection(name, location);
		if(collDir == null) return false;

		var x = new XElement("files");
		foreach(var f in a) {
			var xx = new XElement(f.Xml);
			foreach(var v in xx.DescendantsAndSelf()) v.SetAttributeValue("g", null);
			x.Add(xx);
		}
		//Print(x);

		string filesDir = collDir + @"\files";
		try {
			Files.CreateDirectory(filesDir);
			foreach(var f in a) {
				if(!f.IsLink()) Files.CopyTo(f.FilePath, filesDir);
			}
			x.Save(collDir + @"\files.xml");
		}
		catch(Exception ex) {
			Print(ex.Message);
			return false;
		}

		Shell.SelectFileInExplorer(collDir);
		return true;
	}

	#endregion

	#region watch folder

	FileSystemWatcher _watcher;

	void _InitWatcher()
	{
		_watcher = new FileSystemWatcher(this.FilesDirectory);
		_watcher.IncludeSubdirectories = true;
		_watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
		//_watcher.EnableRaisingEvents = true;
		_watcher.Changed += _watcher_Changed;
		_watcher.Created += _watcher_Created;
		_watcher.Deleted += _watcher_Deleted;
		_watcher.Renamed += _watcher_Renamed;
	}

	void _UninitWatcher()
	{
		//_watcher.Changed -= _watcher_Changed;
		//_watcher.Created -= _watcher_Created;
		//_watcher.Deleted -= _watcher_Deleted;
		//_watcher.Renamed -= _watcher_Renamed;
		_watcher.Dispose();
	}

	private void _watcher_Renamed(object sender, RenamedEventArgs e)
	{
		Print(e.ChangeType, e.OldName, e.Name, e.OldFullPath, e.FullPath);
	}

	private void _watcher_Deleted(object sender, FileSystemEventArgs e)
	{
		Print(e.ChangeType, e.Name, e.FullPath);
	}

	private void _watcher_Created(object sender, FileSystemEventArgs e)
	{
		Print(e.ChangeType, e.Name, e.FullPath);
	}

	private void _watcher_Changed(object sender, FileSystemEventArgs e)
	{
		Print(e.ChangeType, e.Name, e.FullPath);
	}

	#endregion

	#region util
	/// <summary>
	/// Returns true if FileNode f is not null and belongs to this FilesModel and is not deleted.
	/// </summary>
	public bool IsMyFileNode(FileNode f) { return Root.ContainsDescendant(f); }

	/// <summary>
	/// Returns true if s is path of a collection directory.
	/// </summary>
	public static bool IsCollectionDirectory(string s)
	{
		string xmlFile = s + @"\files.xml";
		if(Files.ExistsAsFile(xmlFile) && Files.ExistsAsDirectory(s + @"\files")) {
			try { return XElement.Load(xmlFile).Name == "files"; } catch { }
		}
		return false;
	}

	/// <summary>
	/// Security info string.
	/// </summary>
	public static string GetSecurityInfo(bool withShieldIcon = false)
	{
		var s = "Security info: Unknown files can contain malicious code - virus, spyware, etc. It is safe to import, open and edit files if you don't run and don't compile them. Triggers are inactive until run/compile.";
		return withShieldIcon ? ("v|" + s) : s;
	}

	#endregion
}
