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
	FileNode _f;
	EdMetaCommentsParser _meta;

	public EdCodeFileProperties(FileNode f)
	{
		InitializeComponent();

		_f = f;
		this.Text = _f.Name + " Properties";

		//note: this.StartPosition = FormStartPosition.CenterParent; does not work with Form.Show.

		var owner = MainForm;
		var p = owner.PointToScreen(owner.ClientRectangle.Location);
		this.Location = new Point(p.X + 100, p.Y + 100);
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		//Model.Save.TextNowIfNeed();
		_meta = new EdMetaCommentsParser(_f);
		_FillGrid();
		//_FillGrid2();
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		_ddList?.Dispose();
		base.OnFormClosed(e);
	}

	void _FillGrid()
	{
		var g = _grid;
		g.ZAddHeaderRow("Run");
		_AddCombo("runMode", "green|blueSingle|blueMulti|editorThread", _meta.runMode);
		_AddCombo("ifRunning", "cancel|wait|restart|restartOrWait", _meta.ifRunning);
		_AddCombo("uac", "same|user|admin", _meta.uac);
		_AddCombo("prefer32bit", "false|true", _meta.prefer32bit);
		_AddEdit("config", _meta.config);

		g.ZAddHeaderRow("Compile");
		_AddCombo("debug", "true|false", _meta.debug);
		_AddCombo("warningLevel", "4|3|2|1|0", _meta.warningLevel);
		_AddEdit("disableWarnings", _meta.disableWarnings);
		_AddEdit("define", _meta.define);
		_AddEdit("preBuild", _meta.preBuild);
		_AddEdit("postBuild", _meta.postBuild);

		g.ZAddHeaderRow("Exe");
		//_AddEdit("outputPath", _meta.outputPath);
		_AddOutputPath("outputPath", _meta.outputPath);
		_AddCombo("outputType", "app|console|dll", _meta.outputType);
		_AddEdit("icon", _meta.icon);
		_AddEdit("manifest", _meta.manifest);
		_AddEdit("resFile", _meta.resFile);
		_AddEdit("sign", _meta.sign);
		_AddEdit("xmlDoc", _meta.xmlDoc);

		g.ZAutoSize();

		g.ZValueChanged += _G_ZValueChanged;

		void _AddCombo(string name, string values, string select, string tt = null, string info = null)
		{
			var a = values.Split_("|");
			bool isSpecified = select != null;
			int iSelect;
			if(isSpecified) {
				iSelect = -1;
				for(int i = 0; i < a.Length; i++) if(a[i] == select) { iSelect = i; break; }
			} else iSelect = 0;
			g.ZAdd(null, name, values, isSpecified, tt, info, -1, ParamGrid.EditType.ComboList, comboIndex: iSelect);
		}

		void _AddEdit(string name, string text, string tt = null, string info = null)
		{
			bool isSpecified = text != null;
			g.ZAdd(null, name, text, isSpecified, tt, info);
		}

		void _AddOutputPath(string tt = null, string info = null)
		{
			bool isSpecified = _meta.outputPath != null;
			g.ZAdd(null, "outputPath", _meta.outputPath, isSpecified, tt, info, etype: ParamGrid.EditType.TextButton, buttonAction: (sender, sed) => {
				var m = new AuMenu();
				m[@"%Folders.Workspace%\bin"] = o => _SetEditCellText(o.ToString());
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

	private void _G_ZValueChanged(SourceGrid.CellContext cc)
	{
		//uncheck if selected default value. The control checks when changed.
		var p = cc.Position;
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
			if(uncheck) _grid.ZCheck(p.Row, false);
		}
	}

	void _GetGrid()
	{
		var g = _grid;
		_meta.runMode = _Get("runMode");
		_meta.ifRunning = _Get("ifRunning");
		_meta.uac = _Get("uac");
		_meta.prefer32bit = _Get("prefer32bit");
		_meta.config = _Get("config");
		_meta.debug = _Get("debug");
		_meta.warningLevel = _Get("warningLevel");
		_meta.disableWarnings = _Get("disableWarnings");
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

		string _Get(string name)
		{
			if(g.ZGetValue(name, out var s1, false)) return s1;
			return null;
		}
	}

	//void _FillGrid2()
	//{
	//	var g = _grid2;
	//	g.ZAddHeaderRow("Files");
	//	_AddList("r", _meta.r);
	//	_AddList("library", _meta.library);
	//	_AddList("c", _meta.c);
	//	_AddList("resource", _meta.resource);

	//	g.ZAutoSize();

	//	void _AddList(string name, List<string> a)
	//	{
	//		g.ZAdd(null, name, a != null ? string.Join("\r\n", a) + "\r\n" : "\r\n", null);
	//	}
	//}

	//void _GetGrid2()
	//{
	//	var g = _grid2;
	//	_meta.r = _Get("r");
	//	_meta.library = _Get("library");
	//	_meta.c = _Get("c");
	//	_meta.resource = _Get("resource");

	//	List<string> _Get(string name)
	//	{
	//		if(g.ZGetValue(name, out var s1, false)) {
	//			var a = s1.SplitLines_(true);
	//			if(a.Length != 0) return new List<string>(a);
	//		}
	//		return null;
	//	}
	//}

	private void _bOK_Click(object sender, EventArgs e)
	{
		if(Model.CurrentFile != _f && !Model.SetCurrentFile(_f)) return;
		var t = Panels.Editor.ActiveDoc.ST;
		var code = t.GetText();
		MetaComments.FindMetaComments(code, out int endOf);
		_GetGrid();
		//_GetGrid2();
		var s = _meta.Format(endOf == 0);
		if(s.Length == 0) {
			if(endOf == 0) return;
			else if(code.EqualsAt_(endOf, "\r\n\r\n")) endOf += 4;
			else if(code.EqualsAt_(endOf, "\r\n")) endOf += 2;
		}
		t.ReplaceRange(0, endOf, s, true, true);
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
		=> _AddLibClassResource(f => (f.IsProjectFolder(out var fm) && fm.IsLibrary) ? fm : null, _meta.library, "class library projects", sender);

	private void _bAddClass_Click(object sender, EventArgs e)
		=> _AddLibClassResource(f => (f.IsCS && !f.FindProject(out _, out _)) ? f : null, _meta.c, "class files", sender);

	private void _bAddResource_Click(object sender, EventArgs e)
		=> _AddLibClassResource(f => !(f.IsFolder || f.IsCodeFile) ? f : null, _meta.resource, "resource files", sender);

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
			if(_ddList == null) _ddList = new DropDownList { OnSelected = dd => metaList.Add(dd.ResultString) };
			_ddList.Items = a.ToArray();
			_ddList.Show(button as Control);
		} else AuDialog.Show($"No {ifNone} found in this workspace");
	}
	DropDownList _ddList;

	//void _AddLibClassResource(Func<FileNode, FileNode> filter, List<string> metaList, string ifNone)
	//{
	//	var a = new List<(FileNode, string)>();
	//	foreach(var f in Model.Root.Descendants()) {
	//		var f2 = filter(f);
	//		if(f2 != null) a.Add((f2, f2.ItemPath));
	//	}
	//	var m = new AuMenu();
	//	if(a.Count > 0) {
	//		a.Sort((v1, v2) => string.Compare(v1.Item2, v2.Item2, StringComparison.OrdinalIgnoreCase));
	//		foreach(var v in a) {
	//			var f = v.Item1;
	//			var path = v.Item2;
	//			m[path, f.GetIcon()] = o => metaList.Add(path);
	//			if(metaList.Contains(path, StringComparer.OrdinalIgnoreCase)) m.LastMenuItem.Enabled = false;
	//		}
	//	} else m.Add($"No {ifNone} found in this workspace", null).Enabled = false;
	//	m.Show(this);
	//}
}
