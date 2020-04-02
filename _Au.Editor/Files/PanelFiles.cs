using Au;
using Au.Controls;
using Au.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
//using System.Linq;

partial class PanelFiles : AuUserControlBase
{
	//idea: when file clicked, open it and show AMenu of its functions (if > 1).

	FilesModel.TreeViewFiles _c;
	FilesModel _model;

	public PanelFiles()
	{
		this.AccessibleName = this.Name = "Files";
		_c = new FilesModel.TreeViewFiles();
		_c.AccessibleName = _c.Name = "Files_list";
		this.Controls.Add(_c);
	}

	public FilesModel.TreeViewFiles ZControl => _c;

	public FilesModel ZModel => _model;

	//protected override void OnGotFocus(EventArgs e) { _c.Focus(); }

	/// <summary>
	/// Loads existing or new workspace.
	/// If fails, shows a task dialog with several choices - retry, load another, create new, cancel. If then fails, ends process.
	/// Sets Model and Text properties of the main form. Updates recent files.
	/// By default runs startup script.
	/// </summary>
	/// <param name="wsDir">
	/// Workspace's directory. The directory should contain file "files.xml" and subdirectory "files".
	/// If null, loads the last used workspace (its path is in settings).
	/// If the setting does not exist, uses AFolders.ThisAppDocuments + @"Main".
	/// If the file does not exist, copies from AFolders.ThisApp + @"Default\Workspace".
	/// </param>
	public FilesModel ZLoadWorkspace(string wsDir = null)
	{
		wsDir ??= Program.Settings.workspace;
		if(wsDir.NE()) wsDir = AFolders.ThisAppDocuments + "Main";
		var xmlFile = wsDir + @"\files.xml";
		var oldModel = _model;
		FilesModel m = null;
		_isNewWorkspace = false;
		g1:
		try {
			//SHOULDDO: if editor runs as admin, the workspace directory should be write-protected from non-admin processes.

			if(_isNewWorkspace = !AFile.ExistsAsFile(xmlFile)) {
				AFile.Copy(AFolders.ThisAppBS + @"Default\Workspace", wsDir);
			}

			_model?.UnloadingWorkspace(); //saves all, closes documents, sets current file = null

			m = new FilesModel(_c, xmlFile);
			_c.Model = m;
		}
		catch(Exception ex) {
			m?.Dispose();
			m = null;
			//AOutput.Write($"Failed to load '{wsDir}'. {ex.Message}");
			switch(ADialog.ShowError("Failed to load workspace", wsDir,
				"1 Retry|2 Load another|3 Create new|0 Cancel",
				owner: this, expandedText: ex.ToString())) {
			case 1: goto g1;
			case 2: m = ZLoadAnotherWorkspace(); break;
			case 3: m = ZLoadNewWorkspace(); break;
			}
			if(m != null) return m;
			if(_model != null) return _model;
			Environment.Exit(1);
		}

		oldModel?.Dispose();
		Program.Model = _model = m;

		//CONSIDER: unexpand path
		if(wsDir != Program.Settings.workspace) {
			if(Program.Settings.workspace != null) {
				var ar = Program.Settings.recentWS ?? Array.Empty<string>();
				int i = Array.IndexOf(ar, wsDir); if(i >= 0) ar = ar.RemoveAt(i);
				Program.Settings.recentWS = ar.InsertAt(0, Program.Settings.workspace);
			}
			Program.Settings.workspace = wsDir;
		}

		Program.MainForm.ZSetTitle();
		if(Program.Loaded >= EProgramState.LoadedWorkspace) {
			ZOpenDocuments();
			ZModel.RunStartupScripts();
		}

		return _model;
	}

	public void ZOpenDocuments()
	{
		var m = _model;
		if(_isNewWorkspace) {
			_isNewWorkspace = false;
			m.SetCurrentFile(m.Root.FirstChild, newFile: true);
		} else m.LoadState();

		ZWorkspaceLoadedAndDocumentsOpened?.Invoke();
	}
	bool _isNewWorkspace;

	public event Action ZWorkspaceLoadedAndDocumentsOpened;

	public void ZUnloadOnFormClosed()
	{
		if(_model == null) return;
		_model.Save.AllNowIfNeed();
		var oldModel = _model;
		Program.Model = _model = null;
		oldModel.Dispose();
	}

	/// <summary>
	/// Shows "Open" dialog to select an existing workspace.
	/// On OK loads the selected workspace and returns FilesModel. On Cancel return null.
	/// </summary>
	public FilesModel ZLoadAnotherWorkspace()
	{
		var d = new OpenFileDialog { Title = "Open workspace", Filter = "files.xml|files.xml" };
		if(d.ShowDialog(this) != DialogResult.OK) return null;
		var filesXml = d.FileName;
		if(!APath.GetFileName(filesXml).Eqi("files.xml")) {
			ADialog.ShowError("Must be files.xml");
			return null;
		}
		return ZLoadWorkspace(APath.GetDirectoryPath(filesXml));
	}

	/// <summary>
	/// Shows dialog to create new workspace.
	/// On OK creates new workspace and returns FilesModel. On Cancel return null.
	/// </summary>
	public FilesModel ZLoadNewWorkspace()
	{
		var path = FilesModel.GetDirectoryPathForNewWorkspace();
		if(path == null) return null;
		return ZLoadWorkspace(path);
	}

	/// <summary>
	/// Fills submenu File -> Workspace -> Recent.
	/// </summary>
	public void ZFillMenuRecentWorkspaces(ToolStripDropDownMenu dd)
	{
		void _Add(string path, bool bold)
		{
			var mi = dd.Items.Add(path, null, (o, u) => ZLoadWorkspace(o.ToString()));
			if(bold) mi.Font = Au.Util.AFontsCached_.Bold;
		}

		dd.SuspendLayout();
		dd.Items.Clear();
		_Add(Program.Settings.workspace, true);
		var ar = Program.Settings.recentWS;
		int nRemoved = 0;
		for(int i = 0, n = ar?.Length ?? 0; i < n; i++) {
			var path = ar[i];
			if(dd.Items.Count >= 20 || !AFile.ExistsAsDirectory(path)) {
				ar[i] = null;
				nRemoved++;
			} else _Add(path, false);
		}
		if(nRemoved > 0) {
			var an = new string[ar.Length - nRemoved];
			for(int i = 0, j = 0; i < ar.Length; i++) if(ar[i] != null) an[j++] = ar[i];
			Program.Settings.recentWS = an;
		}
		dd.ResumeLayout();
	}

	/// <summary>
	/// Adds templates to File -> New.
	/// </summary>
	public void ZFillMenuNew(ToolStripDropDownMenu ddm)
	{
		if(_newMenuDone) return; _newMenuDone = true;

		var templDir = FileNode.Templates.DefaultDirBS;
		var xroot = AExtXml.LoadElem(FileNode.Templates.DefaultFilesXml);

		_CreateMenu(ddm, xroot, null, 0);

		void _CreateMenu(ToolStripDropDownMenu ddParent, XElement xParent, string dir, int level)
		{
			ddParent.SuspendLayout();
			int i = level == 0 ? 4 : 0;
			foreach(var x in xParent.Elements()) {
				string tag = x.Name.LocalName, name = x.Attr("n");
				int isFolder = tag == "d" ? 1 : 0;
				if(isFolder == 1) {
					isFolder = name[0] switch { '@' => 2, '!' => 3, _ => 1 }; //@ project, ! simple folder
				} else {
					if(level == 0 && FileNode.Templates.IsStandardTemplateName(name, out _)) continue;
				}
				string relPath = dir + name;
				if(isFolder == 3) name = name[1..];
				var item = new ToolStripMenuItem(name, null, (unu, sed) => ZModel.NewItem(relPath, beginRenaming: true));
				if(isFolder == 1) {
					var ddSub = new ToolStripDropDownMenu();
					item.DropDown = ddSub;
					_CreateMenu(ddSub, x, dir + name + "\\", level + 1);
				} else {
					string si = null;
					if(isFolder != 0) si = nameof(Au.Editor.Resources.Resources.folder);
					else if(tag == "s") si = nameof(Au.Editor.Resources.Resources.fileScript);
					else if(tag == "c") si = nameof(Au.Editor.Resources.Resources.fileClass);
					Bitmap im = si != null
						? EdResources.GetImageUseCache(si)
						: FileNode.IconCache.GetImage(templDir + relPath, useExt: true);
					if(im != null) item.Image = im;
				}
				ddParent.Items.Insert(i++, item);
			}
			ddParent.ResumeLayout();
		}
	}
	bool _newMenuDone;
}
