using Au;
using Au.Controls;
using Au.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
//using System.Linq;
using System.Xml.Linq;

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
	/// If the file does not exist, copies from AFolders.ThisApp + @"Default".
	/// </param>
	public FilesModel ZLoadWorkspace(string wsDir = null)
	{
		wsDir ??= Program.Settings.workspace;
		if(wsDir.IsNE()) wsDir = AFolders.ThisAppDocuments + "Main";
		var xmlFile = wsDir + @"\files.xml";
		var oldModel = _model;
		FilesModel m = null;
		_isNewWorkspace = false;
		g1:
		try {
			//SHOULDDO: if editor runs as admin, the workspace directory should be write-protected from non-admin user processes.

			//CONSIDER: use different logic. Now silently creates empty files, it's not always good.
			//	Add parameter createNew. If false, show error if file not found.
			if(_isNewWorkspace = !AFile.ExistsAsFile(xmlFile)) {
				AFile.CopyTo(AFolders.ThisAppBS + @"Default\files", wsDir);
				AFile.Copy(AFolders.ThisAppBS + @"Default\files.xml", xmlFile);
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
			if(bold) mi.Font = Au.Util.AFonts.Bold;
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

		var templDir = AFolders.ThisAppBS + @"Default\Templates";
		_CreateMenu(templDir, ddm, 0);

		void _CreateMenu(string dir, ToolStripDropDownMenu ddParent, int level)
		{
			ddParent.SuspendLayout();
			int i = level == 0 ? 3 : 0;
			foreach(var v in AFile.EnumDirectory(dir, FEFlags.UseRawPath | FEFlags.SkipHiddenSystem)) {
				bool isProject = false;
				string name = v.Name;
				if(v.IsDirectory) {
					if(level == 0 && name.Eqi("include")) continue;
					if(isProject = (name[0] == '@')) name = name.Substring(1);
				} else {
					if(level == 0 && 0 != name.Eq(true, "Script.cs", "Class.cs")) continue;
				}

				bool isFolder = v.IsDirectory && !isProject;
				var item = new ToolStripMenuItem(name, null, (unu, sed) => ZModel.NewItem(v.FullPath.Substring(templDir.Length + 1), beginRenaming: true));
				if(isFolder) {
					var ddSub = new ToolStripDropDownMenu();
					item.DropDown = ddSub;
					_CreateMenu(dir + "\\" + name, ddSub, level + 1);
				} else {
					string si = null;
					//if(isProject) si = nameof(Au.Editor.Resources.Resources.project);
					if(isProject) si = nameof(Au.Editor.Resources.Resources.folder);
					else {
						switch(FileNode.DetectFileType(v.FullPath)) {
						case EFileType.Script: si = nameof(Au.Editor.Resources.Resources.fileScript); break;
						case EFileType.Class: si = nameof(Au.Editor.Resources.Resources.fileClass); break;
						}
					}
					Bitmap im = si != null ? EdResources.GetImageUseCache(si) : FileNode.IconCache.GetImage(v.FullPath, true);
					if(im != null) item.Image = im;
				}
				ddParent.Items.Insert(i++, item);
			}
			ddParent.ResumeLayout();
		}
	}
	bool _newMenuDone;
}
