using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;
using Au.Compiler;

partial class EdCodeFileProperties : Form_
{
	FileNode _f, _fProjectFolder;
	EdMetaCommentsParser _meta;
	bool _isCS;
	_Role _role;

	enum _Role
	{
		miniProgram, exeProgram, editorExtension, classLibrary, classFile
	}

	public EdCodeFileProperties(FileNode f)
	{
		InitializeComponent();

		_f = f;
		f.FindProject(out _fProjectFolder, out _);
		_isCS = f.IsCS;

		this.Text = _f.Name + " Properties";

		var owner = MainForm;
		var p = owner.PointToScreen(owner.ClientRectangle.Location);
		this.Location = new Point(p.X + 100, p.Y + 100); //note: this.StartPosition = FormStartPosition.CenterParent; does not work with Form.Show.
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		//Model.Save.TextNowIfNeed();
		_meta = new EdMetaCommentsParser(_f);
		_FillGrid();
		//_FillGrid2();
	}

	//protected override void OnFormClosed(FormClosedEventArgs e)
	//{
	//	base.OnFormClosed(e);
	//}

	void _FillGrid()
	{
		var g = _grid;

		if(_meta.runMode == "editorThread") _role = _Role.editorExtension;
		else if(_isCS && (_meta.outputType == null || _meta.outputType == "dll")) _role = _meta.outputPath == null ? _Role.classFile : _Role.classLibrary;
		else _role = _meta.outputPath == null ? _Role.miniProgram : _Role.exeProgram;

		var roles = _isCS ? "Mini program|Exe program|Editor extension|Class library|Class file" : "Mini program|Exe program|Editor extension";
		g.ZAdd(null, "Role", roles, null, etype: ParamGrid.EditType.ComboList, comboIndex: (int)_role);

		g.ZAddHeaderRow("Run");
		_AddEdit("testScript", _f.TestScript?.ItemPath);
		_AddCombo("runMode", "green|blueSingle|blueMulti", _meta.runMode);
		_AddCombo("ifRunning", "cancel|wait|restart|restartOrWait", _meta.ifRunning);
		_AddCombo("uac", "same|user|admin", _meta.uac);
		_AddCombo("prefer32bit", "false|true", _meta.prefer32bit);
		_AddEdit("config", _meta.config);

		g.ZAddHeaderRow("Compile");
		_AddCombo("debug", "true|false", _meta.debug);
		_AddCombo("warningLevel", "4|3|2|1|0", _meta.warningLevel);
		_AddEdit("noWarnings", _meta.noWarnings);
		_AddEdit("define", _meta.define);
		_AddEdit("preBuild", _meta.preBuild);
		_AddEdit("postBuild", _meta.postBuild);

		g.ZAddHeaderRow("Assembly");
		_AddOutputPath("outputPath", _meta.outputPath);
		_AddCombo("outputType", "app|console", _meta.outputType, noCheckbox: _isCS);
		_AddEdit("icon", _meta.icon);
		_AddEdit("manifest", _meta.manifest);
		_AddEdit("resFile", _meta.resFile);
		_AddEdit("xmlDoc", _meta.xmlDoc);
		_AddEdit("sign", _meta.sign);

		_SelectRole();

		g.ZAutoSize();

		g.ZValueChanged += _grid_ZValueChanged;

		void _AddCombo(string name, string values, string select, string tt = null, string info = null, bool noCheckbox = false)
		{
			var a = values.Split_("|");
			bool isSpecified = select != null;
			int iSelect = 0;
			if(isSpecified) {
				for(int i = 0; i < a.Length; i++) if(a[i] == select) { iSelect = i; break; }
			}
			bool? check = isSpecified; if(noCheckbox) check = null;
			g.ZAdd(null, name, values, check, tt, info, -1, ParamGrid.EditType.ComboList, comboIndex: iSelect);
		}

		void _AddEdit(string name, string text, string tt = null, string info = null)
		{
			bool isSpecified = text != null;
			g.ZAdd(null, name, text, isSpecified, tt, info);
		}

		void _AddOutputPath(string tt = null, string info = null)
		{
			g.ZAdd(null, "outputPath", _meta.outputPath, null, tt, info, etype: ParamGrid.EditType.TextButton, buttonAction: (sender, sed) => {
				var m = new AuMenu();
				m[@"%Folders.Workspace%\bin"] = o => _SetEditCellText(o.ToString());
				if(_isCS) m[@"%Folders.ThisApp%\Libraries"] = o => _SetEditCellText(o.ToString());
				m["Browse..."] = o => {
					var f = new FolderBrowserDialog { SelectedPath = Folders.ThisAppDocuments, ShowNewFolderButton = true };
					if(f.ShowDialog(this) == DialogResult.OK) _SetEditCellText(f.SelectedPath);
					f.Dispose();
				};
				m.Show(sender as Control);
			});
		}

		void _SetEditCellText(string s)
		{
			if(!_grid.ZGetEditCell(out var cc)) return;
			int row = cc.Position.Row;
			_grid.ZSetCellText(row, 1, s);
			_grid.ZCheck(row, true);
		}
	}

	void _SelectRole()
	{
		string hide;
		switch(_role) {
		case _Role.miniProgram: hide = "testScript outputPath icon-xmlDoc"; break;
		case _Role.exeProgram: hide = "testScript"; break;
		case _Role.editorExtension: hide = "Run-config outputPath-xmlDoc"; break;
		case _Role.classLibrary: hide = "runMode-config outputType manifest"; break;
		default: hide = "runMode-"; break;
		}
		_grid.ZShowRows(true, "Run-", hide);
		_bAddLib.Enabled = _role != _Role.classFile;
	}

	private void _grid_ZValueChanged(SourceGrid.CellContext cc)
	{
		var g = _grid;
		var p = cc.Position;
		int row = p.Row;

		if(row == 0) { //role
			var cb = cc.Cell.Editor as SourceGrid.Cells.Editors.ComboBox;
			_role = (_Role)cb.Control.SelectedIndex;
			_SelectRole();
			return;
		}

		//Print(p.Column, p.Row, cc.IsEditing());

		//uncheck if selected default value. The control checks when changed.
		if(p.Column == 1 && cc.IsEditing()) {
			bool uncheck = false;
			switch(cc.Cell.Editor) {
			case SourceGrid.Cells.Editors.ComboBox cb:
				if(cb.Control.SelectedIndex <= 0) uncheck = true;
				break;
			case SourceGrid.Cells.Editors.TextBox tb:
				if(Empty(cc.Value as string)) uncheck = true;
				break;
			}
			if(uncheck) g.ZCheck(row, false);
		}

		var rk = g.ZGetRowKey(row);

		//Print(p.Column, row, g.ZIsChecked(row));

		//if runMode blueMulti, cannot use ifRunning
		switch(rk) {
		case "runMode":
			bool multi = _Get("runMode") == "blueMulti";
			g.ZEnableRow(row + 1, !multi, multi);
			break;
		}

		//if runMode blueSingle, cannot be ifRunning restartOrWait
		switch(rk) {
		case "runMode":
		case "ifRunning":
			if(_Get("ifRunning") == "restartOrWait" && _Get("runMode") == "blueSingle") g.ZSetCellText("ifRunning", 1, "restart");
			break;
		}

		if(p.Column == 0 && g.ZIsChecked(row)) {
			switch(rk) {
			case "icon":
			case "manifest":
				g.ZCheck("resFile", false);
				break;
			case "resFile":
				g.ZCheck("icon", false);
				g.ZCheck("manifest", false);
				break;
			}
		}

		//	_toolTip.SetToolTip(g, "Must be .cs file");
	}

	string _Get(string name)
	{
		if(_grid.ZGetValue(name, out var s1, false, true)) return s1 ?? "";
		return null;
	}

	bool _GetGrid()
	{
		var g = _grid;

		//test script
		FileNode fts = null;
		if(g.ZGetValue("testScript", out var sts, true, true)) {
			fts = _f.FindRelative(sts, false);
			if(fts == null) { AuDialog.ShowError("Test script not found", sts); return false; }
		}
		_f.TestScript = fts;

		//info: _Get returns null if hidden

		_meta.runMode = _Get("runMode");
		_meta.ifRunning = _Get("ifRunning");
		_meta.uac = _Get("uac");
		_meta.prefer32bit = _Get("prefer32bit");
		_meta.config = _Get("config");

		_meta.debug = _Get("debug");
		_meta.warningLevel = _Get("warningLevel");
		_meta.noWarnings = _Get("noWarnings");
		_meta.define = _Get("define");
		_meta.preBuild = _Get("preBuild");
		_meta.postBuild = _Get("postBuild");

		_meta.outputPath = _Get("outputPath");
		_meta.outputType = _Get("outputType");
		_meta.icon = _Get("icon");
		_meta.manifest = _Get("manifest");
		_meta.resFile = _Get("resFile");
		_meta.sign = _Get("sign");
		_meta.xmlDoc = _Get("xmlDoc");

		if(_role != _Role.classFile) {
			switch(_role) {
			case _Role.editorExtension:
				_meta.runMode = "editorThread";
				break;
			case _Role.exeProgram:
			case _Role.classLibrary:
				if(Empty(_meta.outputPath)) _meta.outputPath = _role == _Role.exeProgram ? @"%Folders.Workspace%\bin" : @"%Folders.ThisApp%\Libraries";
				break;
			}
			if(_isCS && _meta.outputType == null) _meta.outputType = _role == _Role.classLibrary ? "dll" : "app";
			if(_meta.config == "") _meta.config = "App.config";
			var name = Path_.GetFileName(_f.Name, true);
			if(_meta.xmlDoc == "") _meta.xmlDoc = name + ".xml";
			if(_meta.manifest == "") _meta.manifest = name + ".exe.manifest";
		}

		return true;
	}

	private void _bOK_Click(object sender, EventArgs e)
	{
		if(Model.CurrentFile != _f && !Model.SetCurrentFile(_f)) return;
		var t = Panels.Editor.ActiveDoc.ST;
		var code = t.GetText();
		MetaComments.FindMetaComments(code, out int endOf);
		if(!_GetGrid()) { this.DialogResult = DialogResult.None; return; };
		//_GetGrid2();
		var s = _meta.Format(endOf == 0);
		if(s.Length == 0) {
			if(endOf == 0) return;
			else if(code.EqualsAt_(endOf, "\r\n\r\n")) endOf += 4;
			else if(code.EqualsAt_(endOf, "\r\n")) endOf += 2;
		}
		if(s.Length == endOf && s == t.RangeText(0, endOf, SciFromTo.ToIsChars)) return;
		t.ReplaceRange(0, endOf, s, SciFromTo.ToIsChars);
	}

	private void _bAddRefNet_Click(object sender, EventArgs e) => _AddReference(true);
	private void _bAddRefOther_Click(object sender, EventArgs e) => _AddReference(false);

	void _AddReference(bool net)
	{
		string fNET = Folders.NetFrameworkRuntime, fApp = Folders.ThisApp;
		var d = new OpenFileDialog { InitialDirectory = net ? fNET : fApp, Filter = "Dll|*.dll|All files|*.*", Multiselect = true };
		if(d.ShowDialog(this) != DialogResult.OK) return;

		//remove path and ext if need
		bool noDir = false, noExt = false;
		var a = d.FileNames;
		var dir = Path_.GetDirectoryPath(a[0]);
		if(dir.EqualsI_(fNET) || dir.EqualsI_(fNET + @"\WPF")) noDir = noExt = true;
		else if(dir.EqualsI_(fApp) || dir.EqualsI_(Folders.ThisAppBS + "Libraries") || dir.EqualsI_(Folders.ThisAppBS + "Compiler")) noDir = true; //App.config: <probing privatePath="Compiler;Libraries"/>
		if(noDir) for(int i = 0; i < a.Length; i++) a[i] = Path_.GetFileName(a[i], noExt);

		_meta.r.AddRange(a);
	}

	private void _bAddLib_Click(object sender, EventArgs e)
		=> _AddLibClassResource(
			f => (f.IsProjectFolder(out var fm) && f != _fProjectFolder && fm.GetCsFileType() == FileNode.ECsType.Library) ? fm : null,
			_meta.library, "class library projects", sender);

	private void _bAddClass_Click(object sender, EventArgs e)
		=> _AddLibClassResource(
			f => (_isCS && !f.FindProject(out _, out _) && f != _f && f.GetCsFileType() != FileNode.ECsType.App) ? f : null,
			_meta.c, "class files", sender);

	private void _bAddResource_Click(object sender, EventArgs e)
		=> _AddLibClassResource(
			f => !(f.IsFolder || f.IsCodeFile) ? f : null,
			_meta.resource, "resource files", sender);

	void _AddLibClassResource(Func<FileNode, FileNode> filter, List<string> metaList, string ifNone, object button)
	{
		var a = new List<string>();
		foreach(var f in Model.Root.Descendants()) {
			var f2 = filter(f);
			if(f2 == null) continue;
			var path = f2.ItemPath;
			if(!metaList.Contains(path, StringComparer.OrdinalIgnoreCase)) a.Add(path);
		}
		if(a.Count > 0) {
			a.Sort();
			var dd = new PopupList { ComboBoxAnimation = true };
			dd.Selected += o => metaList.Add(o.ResultItem as string);
			dd.Items = a.ToArray();
			dd.Show(button as Control);
		} else AuDialog.Show($"No {ifNone} found in this workspace");
	}
}
