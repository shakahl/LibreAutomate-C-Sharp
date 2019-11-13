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
using System.Collections;

using Au;
using Au.Types;
using static Au.AStatic;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

partial class FilesModel : ITreeModel, Au.Compiler.IWorkspaceFiles
{
	TreeViewFiles _control;
	public TreeViewFiles TreeControl => _control;
	public readonly FileNode Root;
	public readonly int WorkspaceSN; //sequence number of workspace open in this process: 1, 2...
	static int s_workspaceSN;
	public readonly string WorkspaceFile;
	public readonly string WorkspaceDirectory;
	public readonly string WorkspaceName;
	public readonly string FilesDirectory;
	public readonly string TempDirectory;
	public readonly AutoSave Save;
	readonly Dictionary<uint, FileNode> _idMap;
	public readonly List<FileNode> OpenFiles;
	readonly string _dbFile;
	public readonly ASqlite DB;
	public readonly string SettingsFile;
	public readonly XElement SettingsXml;
	//readonly TriggersUI _triggers;
	readonly bool _importing;
	readonly bool _initedFully;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="c">Tree control. Can be null, for example when importing workspace.</param>
	/// <param name="file">Workspace file (XML).</param>
	/// <exception cref="ArgumentException">Invalid or not full path.</exception>
	/// <exception cref="Exception">XElement.Load exceptions. And possibly more.</exception>
	public FilesModel(TreeViewFiles c, string file)
	{
		_importing = c == null;
		_control = c;
		WorkspaceFile = APath.Normalize(file);
		WorkspaceDirectory = APath.GetDirectoryPath(WorkspaceFile);
		WorkspaceName = APath.GetFileName(WorkspaceDirectory);
		FilesDirectory = WorkspaceDirectory + @"\files";
		TempDirectory = WorkspaceDirectory + @"\.temp";
		if(!_importing) {
			WorkspaceSN = ++s_workspaceSN;
			AFile.CreateDirectory(FilesDirectory);
			Save = new AutoSave(this);
		}
		_idMap = new Dictionary<uint, FileNode>();

		Root = FileNode.Load(WorkspaceFile, this); //recursively creates whole model tree; caller handles exceptions

		if(!_importing) {
			_dbFile = WorkspaceDirectory + @"\state.db";
			try {
				DB = new ASqlite(_dbFile, sql:
					//"PRAGMA journal_mode=WAL;" + //no, it does more bad than good
					"CREATE TABLE IF NOT EXISTS _misc (key TEXT PRIMARY KEY, data TEXT);" +
					"CREATE TABLE IF NOT EXISTS _editor (id INTEGER PRIMARY KEY, top INTEGER, pos INTEGER, lines BLOB);"
					);
			}
			catch(Exception ex) {
				Print($"Failed to open file '{_dbFile}'. Will not load/save workspace state: lists of open files, expanded folders, markers, folding, etc.\r\n\t{ex.ToStringWithoutStack()}");
			}

			SettingsFile = WorkspaceDirectory + @"\settings.xml";
			try {
				if(!AFile.ExistsAsFile(SettingsFile)) AFile.SaveText(SettingsFile, "<settings/>");
				SettingsXml = AExtXml.LoadElem(SettingsFile);
			}
			catch(Exception ex) {
				Print($"Failed to open file '{SettingsFile}'. Will not load/save workspace settings: startup scripts, etc.\r\n\t{ex.ToStringWithoutStack()}");
			}

			OpenFiles = new List<FileNode>();
			_InitClickSelect();
			_InitWatcher();
			//_triggers = new TriggersUI(this);

			AFolders.Workspace = WorkspaceDirectory;
		}
		_initedFully = true;
	}

	public void Dispose()
	{
		if(_importing) return;
		if(_initedFully) {
			//_triggers.Dispose();
			Program.Tasks.OnWorkspaceClosed();
			//Save.AllNowIfNeed(); //owner FilesPanel calls this before calling this func. Because may need more code in between.
		}
		Save?.Dispose();
		if(_initedFully) {
			_UninitWatcher();
			_UninitClickSelect();
			DB?.Dispose();
		}
		_control = null;
	}

	#region ITreeModel

	IEnumerable ITreeModel.GetChildren(object nodeTag)
	{
		if(nodeTag == null) return Root.Children();
		var f = nodeTag as FileNode;
		return f.Children();
	}

	bool ITreeModel.IsLeaf(object nodeTag)
	{
		var f = nodeTag as FileNode;
		return !f.IsFolder;
	}

	public event EventHandler<TreeModelEventArgs> NodesChanged;
	public event EventHandler<TreeModelEventArgs> NodesInserted;
	public event EventHandler<TreeModelEventArgs> NodesRemoved;
	public event EventHandler<TreePathEventArgs> StructureChanged;

	/// <summary>
	/// Call this to update control row view when need to change row height.
	/// To just redraw without changing height use f.UpdateControlRow instead, it's faster.
	/// </summary>
	internal void OnNodeChanged(FileNode f)
	{
		NodesChanged?.Invoke(this, _TreeModelEventArgs(f));
	}

	internal void OnNodeRemoved(FileNode f)
	{
		NodesRemoved?.Invoke(this, _TreeModelEventArgs(f));
	}

	internal void OnNodeInserted(FileNode f)
	{
		NodesInserted?.Invoke(this, _TreeModelEventArgs(f));
	}

	internal void OnStructureChanged()
	{
		StructureChanged?.Invoke(this, new TreePathEventArgs(TreePath.Empty));
	}

	TreeModelEventArgs _TreeModelEventArgs(FileNode node)
	{
		return new TreeModelEventArgs(node.Parent.TreePath, new int[] { node.Index }, new object[] { node });
	}

	#endregion

	#region Au.Compiler.IWorkspaceFiles

	public object IwfCompilerContext { get; set; }

	public string IwfFilesDirectory => FilesDirectory;

	public string IwfWorkspaceDirectory => WorkspaceDirectory;

	public Au.Compiler.IWorkspaceFile IwfFindById(uint id) => FindById(id);

	#endregion

	#region find, id

	/// <summary>
	/// Finds file or folder by name or @"\relative path" or id.
	/// </summary>
	/// <param name="name">
	/// Can be:
	/// Name, like "name.cs".
	/// Relative path like @"\name.cs" or @"\subfolder\name.cs".
	/// &lt;id&gt; - enclosed <see cref="FileNode.IdString"/>, or <see cref="FileNode.IdStringWithWorkspace"/>.
	/// 
	/// Case-insensitive. If enclosed in &lt;&gt;, can be followed by any text.
	/// </param>
	/// <param name="folder">true - folder, false - file, null - any (prefer file if not id and not relative).</param>
	public FileNode Find(string name, bool? folder)
	{
		if(Empty(name)) return null;
		if(name[0] == '<') { name.ToInt(out long id, 1); return FindById(id); }
		return Root.FindDescendant(name, folder);
		//rejected: support name without extension.
	}

	/// <summary>
	/// Calls <see cref="Find(string, bool?)"/>(name, false).
	/// </summary>
	public FileNode FindScript(string name)
	{
		var f = Find(name, false);
		if(f == null && !name.Ends(".cs", true)) f = Find(name + ".cs", false);
		return f;
	}

	/// <summary>
	/// Adds id/f to the dictionary that is used by <see cref="FindById"/> etc.
	/// If id is 0 or duplicate, generates new.
	/// Returns id or the generated id.
	/// </summary>
	public uint AddGetId(FileNode f, uint id = 0)
	{
		g1:
		if(id == 0) {
			//Normally we don't reuse ids of deleted items.
			//	Would be problems with something that we cannot/fail/forget to delete when deleting items.
			//	We save MaxId in XML: <files max-i="MaxId">.
			id = ++MaxId;
			if(id == 0) { //if new item created every 8 s, we have 1000 years, but anyway
				for(uint u = 1; u < uint.MaxValue; u++) if(!_idMap.ContainsKey(u)) { MaxId = u - 1; break; } //fast
				goto g1;
			} else if(_idMap.ContainsKey(id)) { //damaged XML file, or maybe a bug?
				ADebug.Print("id already exists:" + id);
				MaxId = _idMap.Keys.Max();
				id = 0;
				goto g1;
			}
			Save?.WorkspaceLater(); //null when importing this workspace
		}
		try { _idMap.Add(id, f); }
		catch(ArgumentException) {
			PrintWarning($"Duplicate id of '{f.Name}'. Creating new.");
			id = 0;
			goto g1;
		}
		return id;
	}

	/// <summary>
	/// Current largest id, used to generate new id.
	/// The root FileNode's ctor reads it from XML attribute 'max-i' and sets this property.
	/// </summary>
	public uint MaxId { get; set; }

	/// <summary>
	/// Finds file or folder by its <see cref="FileNode.Id"/>.
	/// Returns null if id is 0 or not found.
	/// id can contain <see cref="WorkspaceSN"/> in high-order int.
	/// </summary>
	public FileNode FindById(long id)
	{
		int idc = (int)(id >> 32); if(idc != 0 && idc != WorkspaceSN) return null;
		uint idf = (uint)id;
		if(idf == 0) return null;
		if(_idMap.TryGetValue(idf, out var f)) {
			ADebug.PrintIf(f == null, "deleted: " + idf);
			return f;
		}
		ADebug.Print("id not found: " + idf);
		return null;
	}

	/// <summary>
	/// Finds file or folder by its <see cref="FileNode.IdString"/>.
	/// Note: it must not be as returned by <see cref="FileNode.IdStringWithWorkspace"/>.
	/// </summary>
	public FileNode FindById(string id)
	{
		id.ToInt(out long n);
		return FindById(n);
	}

	/// <summary>
	/// Finds file or folder by its file path (<see cref="FileNode.FilePath"/>).
	/// </summary>
	/// <param name="path">Full path of a file in this workspace or of a linked external file.</param>
	public FileNode FindByFilePath(string path)
	{
		var d = FilesDirectory;
		if(path.Length > d.Length && path.Starts(d, true) && path[d.Length] == '\\') //is in workspace folder
			return Root.FindDescendant(path.Substring(d.Length), null);
		foreach(var f in Root.Descendants()) if(f.IsLink && path.Eqi(f.LinkTarget)) return f;
		return null;
	}

	/// <summary>
	/// Finds all files (and not folders) that have the specified name.
	/// Returns empty array if not found.
	/// </summary>
	/// <param name="name">File name, like "name.cs". If starts with backslash, works like <see cref="Find"/>. Does not support <see cref="FileNode.IdStringWithWorkspace"/> string and filename without extension.</param>
	public FileNode[] FindAllFiles(string name)
	{
		return Root.FindAllDescendantFiles(name);
	}

	#endregion

	#region click, open/close, select, current, selected

	void _InitClickSelect()
	{
		_control.NodeMouseClick += _TV_NodeMouseClick;
		_control.KeyDown += _TV_KeyDown;
		_control.Expanded += _TV_Expanded;
		_control.Collapsed += _TV_Expanded;
	}

	void _UninitClickSelect()
	{
		_control.NodeMouseClick -= _TV_NodeMouseClick;
		_control.KeyDown -= _TV_KeyDown;
		_control.Expanded -= _TV_Expanded;
		_control.Collapsed -= _TV_Expanded;
	}

	private void _TV_NodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
	{
		//Print(e.Button, e.ModifierKeys);
		if(e.ModifierKeys != 0) return;
		var f = e.Node.Tag as FileNode;
		switch(e.Button) {
		case MouseButtons.Left:
			if(_currentFile != f) _SetCurrentFile(f);
			break;
		case MouseButtons.Right:
			_control.BeginInvoke(new Action(() => _ItemRightClicked(f)));
			break;
		case MouseButtons.Middle:
			CloseFile(f, true);
			break;
		}
	}

	/// <summary>
	/// Returns true if f is null or isn't in this workspace or is deleted.
	/// </summary>
	public bool IsAlien(FileNode f) => f?.Model != this || f.IsDeleted;

	/// <summary>
	/// Closes f if open.
	/// Saves text if need, removes from OpenItems, deselects in treeview.
	/// </summary>
	/// <param name="f">Can be any item or null. Does nothing if it is null, folder or not open.</param>
	/// <param name="activateOther">When closing current file, if there are more open files, activate another open file.</param>
	public bool CloseFile(FileNode f, bool activateOther)
	{
		if(IsAlien(f)) return false;
		var of = OpenFiles;
		if(!of.Remove(f)) return false;

		Panels.Editor.ZClose(f);
		SelectDeselectItem(f, false);

		if(f == _currentFile) {
			if(activateOther && of.Count > 0 && _SetCurrentFile(of[0])) return true; //and don't select
			_currentFile = null;
			Program.MainForm.ZSetTitle();
		}
		f.UpdateControlRow();

		Panels.Open.ZUpdateList();
		Panels.Open.ZUpdateCurrent(_currentFile);
		Save.StateLater();

		return true;
	}

	/// <summary>
	/// Closes specified files that are open.
	/// </summary>
	/// <param name="files">Any IEnumerable except OpenFiles.</param>
	public void CloseFiles(IEnumerable<FileNode> files)
	{
		if(files == OpenFiles) files = OpenFiles.ToArray();
		bool closeCurrent = false;
		foreach(var f in files) if(f == _currentFile) closeCurrent = true; else CloseFile(f, false);
		if(closeCurrent) CloseFile(_currentFile, true);
	}

	/// <summary>
	/// Called by <see cref="PanelFiles.ZLoadWorkspace"/> before opening another workspace and disposing this.
	/// Saves all, closes documents, sets _currentFile = null.
	/// </summary>
	public void UnloadingWorkspace()
	{
		Save.AllNowIfNeed();
		_currentFile = null;
		Panels.Editor.ZCloseAll(saveTextIfNeed: false);
		OpenFiles.Clear();
		Panels.Open.ZUpdateList();
		Program.MainForm.ZSetTitle();
	}

	/// <summary>
	/// Gets the current file. It is open/active in the code editor.
	/// </summary>
	public FileNode CurrentFile => _currentFile;
	FileNode _currentFile;

	/// <summary>
	/// Selects the node and opens its file in the code editor.
	/// Returns false if failed to select, for example if f is a folder.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="doNotChangeSelection"></param>
	public bool SetCurrentFile(FileNode f, bool doNotChangeSelection = false, bool newFile = false)
	{
		if(IsAlien(f)) return false;
		if(!doNotChangeSelection) f.SelectSingle();
		if(_currentFile != f) _SetCurrentFile(f, newFile);
		return _currentFile == f;
	}

	/// <summary>
	/// If f!=_currentFile and not folder:
	///		Opens it in editor, adds to OpenFiles, sets _currentFile, saves state later, updates UI.
	///		Saves and hides current document.
	///	Returns false if fails to read file or if f is folder.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="newFile">Should be true if opening the file first time after creating.</param>
	bool _SetCurrentFile(FileNode f, bool newFile = false)
	{
		Debug.Assert(!IsAlien(f));
		if(f == _currentFile) return true;
		//Print(f);
		if(f.IsFolder) return false;

		if(_currentFile != null) Save.TextNowIfNeed();

		var fPrev = _currentFile;
		_currentFile = f;

		if(!Panels.Editor.ZOpen(f, newFile)) {
			_currentFile = fPrev;
			if(OpenFiles.Contains(f)) Panels.Open.ZUpdateCurrent(_currentFile);
			return false;
		}

		fPrev?.UpdateControlRow();
		_currentFile?.UpdateControlRow();

		var of = OpenFiles;
		of.Remove(f);
		of.Insert(0, f);
		Panels.Open.ZUpdateList();
		Panels.Open.ZUpdateCurrent(f);
		Save.StateLater();

		Program.MainForm.ZSetTitle();

		return true;
	}

	public FileNode[] SelectedItems => _control.SelectedNodes.Select(tn => tn.Tag as FileNode).ToArray();

	/// <summary>
	/// Selects or deselects item in treeview. Does not set current file etc. Does not deselect others.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="select"></param>
	public void SelectDeselectItem(FileNode f, bool select)
	{
		if(IsAlien(f)) return;
		f.IsSelected = select;
	}

	void _ItemRightClicked(FileNode f)
	{
		if(IsAlien(f)) return;
		var m = Strips.ddFile;

		ToolStripDropDownClosedEventHandler onClosed = null;
		onClosed = (sender, e) => {
			(sender as ToolStripDropDownMenu).Closed -= onClosed;
			_msgLoop.Stop();
		};
		m.Closed += onClosed;

		_inContextMenu = true;
		try {
			m.ZShowAsContextMenu();
			_msgLoop.Loop();
			if(_control == null) return; //loaded another workspace
			if(f != _currentFile && _control.SelectedNodes.Count < 2) {
				if(_currentFile == null) _control.ClearSelection();
				//else if(_control.SelectedNode == f.TreeNodeAdv) _currentFile.SelectSingle(); //no. Breaks renaming, etc. We'll do it on editor focused.
				//else the action selected another file or folder
			}
		}
		finally { _inContextMenu = false; }
	}
	Au.Util.AMessageLoop _msgLoop = new Au.Util.AMessageLoop();
	bool _inContextMenu;

	//Called when editor control focused, etc.
	public void EnsureCurrentSelected()
	{
		//if(_currentFile != null && _control.SelectedNode?.Tag != _currentFile && _control.SelectedNodes.Count < 2) _currentFile.SelectSingle();
		if(_currentFile != null && _control.SelectedNode?.Tag != _currentFile) _currentFile.SelectSingle();
	}

	private void _TV_Expanded(object sender, TreeViewAdvEventArgs e)
	{
		if(e.Node.Level == 0) return;
		Save.StateLater();
	}

	/// <summary>
	/// Selects the node, opens its file in the code editor, optionally goes to the specified position or line or line/column.
	/// Returns false if failed to select, for example if f is a folder.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="line">If not negative, goes to this 0-based line.</param>
	/// <param name="columnOrPos">If not negative, goes to this 0-based position in text (if line negative) or to this 0-based column in line.</param>
	public bool OpenAndGoTo(FileNode f, int line = -1, int columnOrPos = -1)
	{
		bool wasOpen = _currentFile == f;
		if(!SetCurrentFile(f)) return false;
		var doc = Panels.Editor.ZActiveDoc;
		doc.Focus();
		if(line >= 0 || columnOrPos >= 0) {
			var z = doc.Z;
			if(line >= 0) {
				int i = z.LineStart(false, line);
				if(columnOrPos > 0) i = doc.Pos8(doc.Pos16(i) + columnOrPos); //not SCI_FINDCOLUMN, it calculates tabs
				columnOrPos = i;
			}
			if(wasOpen) z.GoToPos(false, columnOrPos);
			else ATimer.After(10, _ => z.GoToPos(false, columnOrPos));
			//info: scrolling works better with async when now opened the file. Or with doevents; not with BeginInvoke.
		}
		return true;
	}

	/// <summary>
	/// Finds file and calls <see cref="OpenAndGoTo(FileNode, int, int)"/>. Does nothing if not found.
	/// </summary>
	public bool OpenAndGoTo(string file, int line = -1, int columnOrPos = -1)
	{
		var f = FindScript(file); if(f == null) return false;
		return OpenAndGoTo(f, line, columnOrPos);
	}

	/// <summary>
	/// Finds file or folder and selects the node. If not folder, opens its file in the code editor, optionally goes to the specified position or line or line/column.
	/// Returns false if failed to find or select.
	/// </summary>
	/// <param name="fileOrFolder">See <see cref="Find"/>.</param>
	/// <param name="line1Based">If not empty, goes to this 1-based line.</param>
	/// <param name="column1BasedOrPos">If not empty, goes to this 0-based position in text (if line empty) or to this 1-based column in line.</param>
	/// <remarks>
	/// If column1BasedOrPos or line1Based not empty, searches only files, not folders.
	/// </remarks>
	public bool OpenAndGoTo(string fileOrFolder, string line1Based = null, string column1BasedOrPos = null)
	{
		var f = Empty(line1Based) && Empty(column1BasedOrPos) ? Find(fileOrFolder, null) : FindScript(fileOrFolder);
		if(f == null) return false;
		if(f.IsFolder) {
			f.SelectSingle();
			return true;
		}
		int line = Empty(line1Based) ? -1 : line1Based.ToInt() - 1;
		int columnOrPos = -1; if(!Empty(column1BasedOrPos)) columnOrPos = column1BasedOrPos.ToInt() - (line < 0 ? 0 : 1);
		return OpenAndGoTo(f, line, columnOrPos);
	}

	#endregion

	#region hotkeys, new, delete, open/close (menu commands), cut/copy/paste, properties

	private void _TV_KeyDown(object sender, KeyEventArgs e)
	{
		switch(e.KeyData) {
		case Keys.Enter: OpenSelected(1); break;
		case Keys.Delete: DeleteSelected(); break;
		case Keys.Control | Keys.X: CutCopySelected(true); break;
		case Keys.Control | Keys.C: CutCopySelected(false); break;
		case Keys.Control | Keys.V: Paste(); break;
		case Keys.Escape: _myClipboard.Clear(); break;
		}
	}

	/// <summary>
	/// Gets the place where item should be added in operations such as new, paste, import.
	/// </summary>
	FileNode _GetInsertPos(out NodePosition pos, bool askIntoFolder = false)
	{
		//CONSIDER: use _inContextMenu here. If it is false, and askIntoFolder, show dialog with 2 or 3 options: top, above selected, in selected folder.

		FileNode r;
		var c = _control.CurrentNode;
		if(c == null) { //empty treeview?
			r = Root;
			pos = NodePosition.Inside;
		} else {
			r = c.Tag as FileNode;
			if(askIntoFolder && r.IsFolder && c.IsSelected && _control.SelectedNodes.Count == 1 && ADialog.ShowYesNo("Into the folder?", owner: _control)) pos = NodePosition.Inside;
			else if(r.Next == null) pos = NodePosition.After; //usually we want to add after the last, not before
			else pos = NodePosition.Before;
		}
		return r;
	}

	/// <summary>
	/// Creates new item at the top of the tree or at the context menu position.
	/// Opens file or selects folder. Optionally begins renaming.
	/// Calls <see cref="FileNode.NewItem"/>.
	/// </summary>
	public FileNode NewItem(string template, string name = null, bool beginRenaming = false, EdNewFileText text = null)
	{
		var pos = NodePosition.Inside;
		var target = _inContextMenu ? _GetInsertPos(out pos) : null;
		return NewItem(target, pos, template, name, beginRenaming, text);
	}

	/// <summary>
	/// Creates new item at the specified position.
	/// Opens file, or selects folder, or opens main file of project folder. Optionally begins renaming.
	/// Calls <see cref="FileNode.NewItem"/>.
	/// </summary>
	public FileNode NewItem(FileNode target, NodePosition pos, string template, string name = null, bool beginRenaming = false, EdNewFileText text = null)
	{
		var f = FileNode.NewItem(this, target, pos, template, name);
		if(f == null) return null;

		if(f.IsFolder) {
			if(f.IsProjectFolder(out var main) && main != null) SetCurrentFile(f = main, newFile: true); //open the main file of the new project folder
			else f.SelectSingle(); //select the new folder
		} else SetCurrentFile(f, newFile: true); //open the new file

		if(text != null && f == CurrentFile) {
			string s = text.meta + (text.replaceTemplate ? null : f.GetText()) + text.text;
			Panels.Editor.ZActiveDoc.Z.SetText(s);
		}

		if(beginRenaming && f.IsSelected) RenameSelected();
		return f;
	}

	public void RenameSelected()
	{
		//if(_control.SelectedNodes.Count != 1) return; //let edit current node, like F2 does
		(_control.NodeControls[1] as NodeTextBox).BeginEdit();
	}

	public void DeleteSelected()
	{
		var a = SelectedItems; if(a.Length < 1) return;

		//confirmation
		var text = string.Join("\n", a.Select(f => f.Name));
		var expandedText = "The file will be deleted, unless it is external.\r\nWill use Recycle Bin, if possible.";
		var r = ADialog.ShowEx("Deleting", text, "1 OK|0 Cancel", owner: _control, checkBox: "Don't delete file", expandedText: expandedText);
		if(r.Button == 0) return;

		foreach(var f in a) {
			_Delete(f, doNotDeleteFile: r.IsChecked); //info: and saves everything, now and/or later
		}
	}

	bool _Delete(FileNode f, bool doNotDeleteFile = false, bool tryRecycleBin = true, bool canDeleteLinkTarget = false)
	{
		var e = f.Descendants(true);

		CloseFiles(e);

		if(!doNotDeleteFile && (canDeleteLinkTarget || !f.IsLink)) {
			if(!TryFileOperation(() => AFile.Delete(f.FilePath, tryRecycleBin), deletion: true)) return false;
			//FUTURE: move to folder 'deleted'. Moving to RB is very slow. No RB if in removable drive etc.
		} else {
			string s1 = doNotDeleteFile ? "File not deleted:" : "The deleted item was a link to";
			Print($"<>Info: {s1} <explore>{f.FilePath}<>");
		}

		foreach(var k in e) {
			if(_myClipboard.Contains(k)) _myClipboard.Clear();
			try { DB?.Execute("DELETE FROM _editor WHERE id=?", k.Id); } catch(SLException ex) { ADebug.Print(ex); }
			Au.Compiler.Compiler.OnFileDeleted(this, k);
			_idMap[k.Id] = null;
			k.IsDeleted = true;
		}

		OnNodeRemoved(f);
		f.Remove();
		//FUTURE: call event to update other controls.

		Save.WorkspaceLater();
		return true;
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
			nodes[0].TreeControl.Invalidate(); //draw "cut" icon
			return true;
		}

		public void Clear()
		{
			if(nodes == null) return;
			if(nodes.Length > 0) nodes[0].TreeControl.Invalidate(); //was "cut" icon
			nodes = null;
		}

		public bool IsEmpty => nodes == null;

		public bool Contains(FileNode f) { return !IsEmpty && nodes.Contains(f); }
	}
	_MyClipboard _myClipboard;

	public bool IsInPrivateClipboard(FileNode f, out bool cut)
	{
		if(_myClipboard.Contains(f)) { cut = _myClipboard.cut; return true; }
		return cut = false;
	}

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
				else SetCurrentFile(f);
				break;
			//case 2:
			//	if(f.IsFolder) continue;
			//	//FUTURE
			//	break;
			case 3:
				AExec.Run(f.FilePath);
				break;
			case 4:
				AExec.SelectInExplorer(f.FilePath);
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
			var a = SelectedItems;
			if(a.Length > 0) CloseFiles(a);
			else CloseFile(_currentFile, true);
			break;
		case 2:
			CloseFiles(OpenFiles);
			_control.CollapseAll();
			break;
		case 3:
			_control.CollapseAll();
			break;
		}
	}

	public void Properties()
	{
		FileNode f = null;
		if(_inContextMenu) {
			var a = SelectedItems;
			if(a.Length == 1) f = a[0];
		} else {
			EnsureCurrentSelected();
			f = _currentFile;
		}
		if(f == null) return;
		if(f.IsCodeFile) new FProperties(f).Show(Program.MainForm);
		//else if(f.IsFolder) new EdFolderProperties(f).Show(MainForm);
		//else new EdOtherFileProperties(f).Show(MainForm);
	}

	#endregion

	#region import, move, copy

	void _OnDragDrop(TreeNodeAdv[] nodes, string[] files, bool copy, DropPosition dropPos)
	{
		var pos = dropPos.Position;
		var target = dropPos.Node.Tag as FileNode;
		if(nodes != null) {
			var a = nodes.Select(tn => tn.Tag as FileNode).ToArray();
			_MultiCopyMove(copy, a, target, pos);
		} else {
			if(files.Length == 1 && IsWorkspaceDirectory(files[0])) {
				switch(ADialog.ShowEx("Workspace", files[0], "1 Open|2 Import|0 Cancel", footerText: GetSecurityInfo("v|"))) {
				case 1: ATimer.After(1, _ => Panels.Files.ZLoadWorkspace(files[0])); break;
				case 2: ImportWorkspace(files[0], target, pos); break;
				}
				return;
			}
			_ImportFiles(copy, files, target, pos);
		}
	}

	/// <summary>
	/// Imports one or more files into the workspace.
	/// </summary>
	/// <param name="a">Files. If null, shows dialog to select files.</param>
	public void ImportFiles(string[] a = null)
	{
		if(a == null) {
			var d = new System.Windows.Forms.OpenFileDialog();
			d.Multiselect = true;
			d.Title = "Import files to the workspace";
			if(d.ShowDialog(Program.MainForm) != DialogResult.OK) return;
			a = d.FileNames;
		}

		var target = _GetInsertPos(out var pos);
		_ImportFiles(false, a, target, pos);
	}

	/// <summary>
	/// Imports another workspace into this workspace.
	/// </summary>
	/// <param name="wsDir">Workspace directory. If null, shows dialog to select.</param>
	/// <param name="target">If null, calls _GetInsertPos.</param>
	/// <param name="pos">Used when target is not null.</param>
	public void ImportWorkspace(string wsDir = null, FileNode target = null, NodePosition pos = 0)
	{
		string xmlFile;
		if(wsDir != null) xmlFile = wsDir + @"\files.xml";
		else {
			var d = new System.Windows.Forms.OpenFileDialog() { Title = "Import workspace", Filter = "files.xml|files.xml" };
			if(d.ShowDialog(Program.MainForm) != DialogResult.OK) return;
			wsDir = APath.GetDirectoryPath(xmlFile = d.FileName);
		}

		try {
			//create new folder for workspace's items
			if(target == null) target = _GetInsertPos(out pos);
			target = FileNode.NewItem(this, target, pos, "Folder", APath.GetFileName(wsDir));
			if(target == null) return;

			var m = new FilesModel(null, xmlFile);
			var a = m.Root.Children().ToArray();
			_MultiCopyMove(true, a, target, NodePosition.Inside, true);
			m.Dispose(); //currently does nothing

			target.SelectSingle();

			Print($"Info: Imported workspace '{wsDir}' to folder '{target.Name}'.\r\n\t{GetSecurityInfo()}");
		}
		catch(Exception ex) { Print(ex.Message); }
	}

	void _MultiCopyMove(bool copy, FileNode[] a, FileNode target, NodePosition pos, bool importingWorkspace = false)
	{
		_control.ClearSelection();
		_control.BeginUpdate();
		try {
			bool movedCurrentFile = false;
			var a2 = new List<FileNode>(a.Length);
			foreach(var f in (pos == NodePosition.After) ? a.Reverse() : a) {
				if(!importingWorkspace && !this.IsMyFileNode(f)) continue; //deleted?
				if(copy) {
					var fCopied = f.FileCopy(target, pos, this);
					if(fCopied != null) a2.Add(fCopied);
				} else {
					if(!f.FileMove(target, pos)) continue;
					a2.Add(f);
					if(!movedCurrentFile && _currentFile != null) {
						if(f == _currentFile || (f.IsFolder && _currentFile.IsDescendantOf(f))) movedCurrentFile = true;
					}
				}
			}
			if(movedCurrentFile) _control.EnsureVisible(_currentFile.TreeNodeAdv);
			if(pos != NodePosition.Inside || target.TreeNodeAdv.IsExpanded) {
				foreach(var f in a2) f.IsSelected = true;
			}
		}
		catch(Exception ex) { Print(ex.Message); }
		finally { _control.EndUpdate(); }

		//info: don't need to schedule saving here. FileCopy and FileMove did it.
	}

	void _ImportFiles(bool copy, string[] a, FileNode target, NodePosition pos)
	{
		bool fromWorkspaceDir = false, dirsDropped = false;
		for(int i = 0; i < a.Length; i++) {
			var s = a[i] = APath.Normalize(a[i]);
			if(s.Find(@"\$RECYCLE.BIN\", true) > 0) {
				ADialog.ShowEx("Files from Recycle Bin", $"At first restore the file to the <a href=\"{FilesDirectory}\">workspace folder</a> or other normal folder.",
					icon: DIcon.Info, owner: _control, onLinkClick: e => AExec.TryRun(e.LinkHref));
				return;
			}
			var fd = FilesDirectory;
			if(!fromWorkspaceDir) {
				if(s.Starts(fd, true) && (s.Length == fd.Length || s[fd.Length] == '\\')) fromWorkspaceDir = true;
				else if(!dirsDropped) dirsDropped = AFile.ExistsAsDirectory(s);
			}
		}
		int r;
		if(copy) {
			if(fromWorkspaceDir) {
				ADialog.ShowInfo("Files from workspace folder", "Ctrl not supported."); //not implemented
				return;
			}
			r = 2; //copy
		} else if(fromWorkspaceDir) {
			r = 3; //move
		} else {
			string buttons = (dirsDropped ? null : "1 Add as a link to the external file|") + "2 Copy to the workspace folder|3 Move to the workspace folder|0 Cancel";
			r = ADialog.ShowEx("Import files", string.Join("\n", a), buttons, DFlags.CommandLinks, owner: _control, footerText: GetSecurityInfo("v|"));
			if(r == 0) return;
		}

		var newParent = (pos == NodePosition.Inside) ? target : target.Parent;
		bool select = pos != NodePosition.Inside || target.TreeNodeAdv.IsExpanded;
		if(select) _control.ClearSelection();
		_control.BeginUpdate();
		try {
			var newParentPath = newParent.FilePath;
			var (nf1, nd1, nc1) = _CountFilesFolders();

			foreach(var path in a) {
				bool isDir;
				var itIs = AFile.ExistsAs2(path, true);
				if(itIs == FileDir2.File) isDir = false;
				else if(itIs == FileDir2.Directory && r != 1) isDir = true;
				else continue; //skip symlinks or if does not exist

				FileNode k;
				var name = APath.GetFileName(path);
				if(r == 1) { //add as link
					k = new FileNode(this, name, path, false, path); //CONSIDER: unexpand
				} else {
					//var newPath = newParentPath + "\\" + name;
					if(fromWorkspaceDir) { //already exists?
						var relPath = path.Substring(FilesDirectory.Length);
						var fExists = this.Find(relPath, null);
						if(fExists != null) {
							fExists.FileMove(target, pos);
							continue;
						}
					}
					k = new FileNode(this, name, path, isDir);
					if(isDir) _AddDir(path, k);
					if(!TryFileOperation(() => {
						if(r == 2) AFile.CopyTo(path, newParentPath, IfExists.Fail);
						else AFile.MoveTo(path, newParentPath, IfExists.Fail);
					})) continue;
				}
				target.AddChildOrSibling(k, pos, false);
				if(select) k.IsSelected = true;
			}

			var (nf2, nd2, nc2) = _CountFilesFolders();
			int nf = nf2 - nf1, nd = nd2 - nd1, nc = nc2 - nc1;
			if(nf + nd > 0) Print($"Info: Imported {nf} files and {nd} folders.{(nc > 0 ? GetSecurityInfo("\r\n\t") : null)}");
		}
		catch(Exception ex) { Print(ex.Message); }
		finally { _control.EndUpdate(); }
		Save.WorkspaceLater();

		void _AddDir(string path, FileNode parent)
		{
			foreach(var u in AFile.EnumDirectory(path, FEFlags.UseRawPath | FEFlags.SkipHiddenSystem)) {
				bool isDir = u.IsDirectory;
				var k = new FileNode(this, u.Name, u.FullPath, isDir);
				parent.AddChild(k);
				if(isDir) _AddDir(u.FullPath, k);
			}
		}

		(int nf, int nd, int nc) _CountFilesFolders()
		{
			int nf = 0, nd = 0, nc = 0;
			foreach(var v in Root.Descendants()) if(v.IsFolder) nd++; else { nf++; if(v.IsCodeFile) nc++; }
			return (nf, nd, nc);
		}
	}

	#endregion

	#region export

	/// <summary>
	/// Shows dialog to get path for new or exporting workspace.
	/// Returns workspace's directory path.
	/// Does not create any files/directories.
	/// </summary>
	/// <param name="name">Default name of the workspace.</param>
	/// <param name="location">Default parent directory of the main directory of the workspace.</param>
	public static string GetDirectoryPathForNewWorkspace(string name = null, string location = null)
	{
		var f = new FNewWorkspace();
		f.textName.Text = name;
		f.textLocation.Text = location ?? AFolders.ThisAppDocuments;
		if(f.ShowDialog() != DialogResult.OK) return null;
		return f.textPath.Text;
	}

	public bool ExportSelected(string location = null)
	{
		var a = SelectedItems; if(a.Length < 1) return false;

		string name = a[0].Name; if(!a[0].IsFolder) name = APath.GetFileName(name, true);

		if(a.Length == 1 && a[0].IsFolder && a[0].HasChildren) a = a[0].Children().ToArray();

		var wsDir = GetDirectoryPathForNewWorkspace(name, location); if(wsDir == null) return false;
		string filesDir = wsDir + @"\files";
		try {
			AFile.CreateDirectory(filesDir);
			foreach(var f in a) {
				if(!f.IsLink) AFile.CopyTo(f.FilePath, filesDir);
			}
			FileNode.Export(a, wsDir + @"\files.xml");
		}
		catch(Exception ex) {
			Print(ex);
			return false;
		}

		AExec.SelectInExplorer(wsDir);
		return true;
	}

	#endregion

	#region watch folder

	FileSystemWatcher _watcher;
	Action<FileSystemEventArgs> __watcherAction;

	public bool IsWatchingFileChanges => _watcher != null;

	void _InitWatcher()
	{
		//return;
		try {
			_watcher = new FileSystemWatcher(FilesDirectory) {
				IncludeSubdirectories = true,
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
				//,SynchronizingObject = _control //no, we call BeginInvoke eplicitly, for better performance etc
			};
			__watcherAction = _watcher_Event2;
			_watcher.Changed += _watcher_Event;
			_watcher.Created += _watcher_Event;
			//_watcher.Deleted += _watcher_Event;
			_watcher.Renamed += _watcher_Event;
			_watcher.EnableRaisingEvents = true;
		}
		catch(Exception ex) {
			_UninitWatcher();
			ADebug.Print(ex);
		}
	}

	void _UninitWatcher()
	{
		_watcher?.Dispose(); //disables raising events and sets all events = null
		_watcher = null;
	}

	private void _watcher_Event(object sender, FileSystemEventArgs e) //in thread pool
	{
		//if(e.Name.Ends("~temp") || e.Name.Ends("~backup")) return; //no such events, because we use other directory for temp files
		if(e.ChangeType == WatcherChangeTypes.Changed && AFile.ExistsAs(e.FullPath, true) != FileDir.File) return; //we receive 'directory changed' after every 'file changed' etc

		try { _control?.BeginInvoke(__watcherAction, new object[] { e }); }
		catch(Exception ex) { ADebug.Print(ex); }
	}

	private void _watcher_Event2(FileSystemEventArgs e) //in main thread
	{
		var f = Find("\\" + e.Name, null);
		//if(e is RenamedEventArgs r) Print(e.ChangeType, r.OldName, e.Name, r.OldFullPath, e.FullPath, f); else Print(e.ChangeType, e.Name, e.FullPath, f);
		if(f == null || f.IsLink) return;
		//ADebug.Print($"<><c blue>File {e.ChangeType.ToString().Lower()} externally: {f}  ({e.FullPath})<>");
		if(f.IsFolder) {
			//if(e.ChangeType == WatcherChangeTypes.Changed) return;
			foreach(var v in f.Descendants()) v.UnCacheText(fromWatcher: true);
		} else {
			f.UnCacheText(fromWatcher: true);
		}
	}

	/// <summary>
	/// Calls Action a in try/catch. On exception prints message and returns false.
	/// Temporarily disables the file system watcher if need.
	/// </summary>
	public bool TryFileOperation(Action a, bool deletion = false)
	{
		bool pause = _watcher != null && !deletion;
		try {
			if(pause) _watcher.EnableRaisingEvents = false;
			a();
		}
		catch(Exception ex) { Print(ex.ToStringWithoutStack()); return false; }
		finally { if(pause) _watcher.EnableRaisingEvents = true; } //fast
		return true;
	}

	#endregion

	#region other

	/// <summary>
	/// Saves <see cref="SettingsXml"/> to <see cref="SettingsFile"/>.
	/// If fails, prints error and returns false.
	/// </summary>
	public bool SaveWorkspaceSettings()
	{
		if(SettingsXml != null) {
			try { SettingsXml.SaveElem(SettingsFile); return true; }
			catch(Exception ex) { Print("Failed to save workspace settings. " + ex.ToStringWithoutStack()); }
		}
		return false;
	}

	public string StartupScriptsCsv {
		get => SettingsXml?.Element("users")?.Elem("user", "guid", Program.UserGuid, true)?.Element("startupScripts")?.Value;
		set {
			if(SettingsXml == null) return;
			var x = SettingsXml.ElemOrAdd("users").ElemOrAdd("user", "guid", Program.UserGuid, true).ElemOrAdd("startupScripts");
			x.Value = value;
			SaveWorkspaceSettings();
		}
	}

	public void RunStartupScripts()
	{
		var csv = StartupScriptsCsv; if(csv == null) return;
		try {
			var x = ACsv.Parse(csv);
			foreach(var row in x.Data) {
				var f = FindScript(row[0]);
				if(f == null) { Print("Startup script not found: " + row[0] + ". Please edit name in Options."); continue; }
				int delay = 10;
				if(x.ColumnCount > 1) {
					var sd = row[1];
					delay = sd.ToInt(0, out int end);
					if(end > 0 && !sd.Ends("ms", true)) delay = (int)Math.Min(delay * 1000L, int.MaxValue);
					if(delay < 10) delay = 10;
				}
				ATimer.After(delay, t => {
					_ = Au.Triggers.HooksServer.Instance.MsgWnd; //waits until started
					Run.CompileAndRun(true, f);
				});
			}
		}
		catch(FormatException) { }
	}

	#endregion

	#region util

	/// <summary>
	/// Returns true if FileNode f is not null and belongs to this FilesModel and is not deleted.
	/// </summary>
	public bool IsMyFileNode(FileNode f) { return Root.IsAncestorOf(f); }

	/// <summary>
	/// Returns true if s is path of a workspace directory.
	/// </summary>
	public static bool IsWorkspaceDirectory(string s)
	{
		string xmlFile = s + @"\files.xml";
		if(AFile.ExistsAsFile(xmlFile) && AFile.ExistsAsDirectory(s + @"\files")) {
			try { return AExtXml.LoadElem(xmlFile).Name == "files"; } catch { }
		}
		return false;
	}

	/// <summary>
	/// Security info string.
	/// </summary>
	public static string GetSecurityInfo(string prefix = null)
	{
		return prefix + "Security info: Unknown C# script files can contain malicious code - virus, spyware, etc. It is safe to import, open and edit C# files if you don't run them. Triggers don't work until run.";
	}

	#endregion
}

class EdNewFileText
{
	public bool replaceTemplate;
	public string text, meta;

	public EdNewFileText() { }

	public EdNewFileText(bool replaceTemplate, string text, string meta = null)
	{
		this.replaceTemplate = replaceTemplate;
		this.text = text;
		this.meta = meta;
	}
}
