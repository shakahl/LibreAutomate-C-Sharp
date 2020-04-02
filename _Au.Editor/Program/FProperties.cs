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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;

using Au;
using Au.Types;
using Au.Controls;
using Au.Compiler;
using System.Runtime.InteropServices.ComTypes;
using System.Globalization;

partial class FProperties : DialogForm, IMessageFilter
{
	FileNode _f;
	//FileNode _fProjectFolder;
	EdMetaCommentsParser _meta;
	bool _isClass;
	ERole _role;

	public FProperties(FileNode f)
	{
		InitializeComponent();

		_f = f;
		//f.FindProject(out _fProjectFolder, out _);
		_isClass = f.IsClass;

		this.Text = _f.Name + " Properties";

		var owner = Program.MainForm;
		var p = owner.PointToScreen(owner.ClientRectangle.Location);
		this.Location = new Point(p.X + 100, p.Y + 100); //note: this.StartPosition = FormStartPosition.CenterParent; does not work with Form.Show.
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		//Model.Save.TextNowIfNeed();
		_InfoInit();
		_meta = new EdMetaCommentsParser(_f);
		_FillGrid();
		Application.AddMessageFilter(this);
		//_tSearch.SetCueBanner("List must contain");
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		Application.RemoveMessageFilter(this);

		base.OnFormClosed(e);
	}

	void _FillGrid()
	{
		var g = _grid;

		_role = _meta.role switch
		{
			"miniProgram" => ERole.miniProgram,
			"exeProgram" => ERole.exeProgram,
			"editorExtension" => ERole.editorExtension,
			"classLibrary" when _isClass => ERole.classLibrary,
			"classFile" when _isClass => ERole.classFile,
			_ => _isClass ? ERole.classFile : ERole.miniProgram,
		};
		var roles = _isClass ? "miniProgram|exeProgram|editorExtension|classLibrary|classFile" : "miniProgram|exeProgram|editorExtension";
		_AddCombo("role", roles, null,
@"<b>role</b> - purpose of this C# code file. What type of assembly to create and how to execute.
 • <i>miniProgram</i> - execute in a separate host process started from editor.
 • <i>exeProgram</i> - create/execute .exe file. It can run on any computer, without editor installed.
 • <i>editorExtension</i> - execute in the editor's UI thread. Dangerous, unstoppable. Rarely used.
 • <i>classLibrary</i> - create .dll file. It can be used as a reference assembly anywhere.
 • <i>classFile</i> - don't create/execute. Compile together with other C# code files in the project or using meta c. Inherits meta comment options of the main file of the compilation.

Default role for scripts is miniProgram; cannot be the last two. Default for class files is classFile.
",
			noCheckbox: true, index: (int)_role);

		g.ZAddHeaderRow("Run");
		_AddEdit("testScript", _f.TestScript?.ItemPath,
@"<b>testScript</b> - a script to run when you click the Run button.
Can be path relative to this file (examples: Script5.cs, Folder\Script5.cs, ..\Folder\Script5.cs) or path in the workspace (examples: \Script5.cs, \Folder\Script5.cs).

Usually it is used to test this class file or class library. It can contain meta comment option <c green>c this file<> that adds this file to the compilation, or <c green>pr this file<> that adds the output dll file as a reference assembly. The recommended way to add this option correctly and easily is to try to run this file and click a link that is then printed in the output.

Unlike most other options, this option is saved not in meta comments. It is saved in file files.xml.
");
		_AddCombo("runMode", "green|blue", _meta.runMode,
@"<b>runMode</b> - whether tasks can run simultaneously, etc.
 • <i>green</i> (default) - multiple green tasks cannot run simultaneously.
 • <i>blue</i> - multiple blue tasks can run simultaneously (see ifRunning).

Green tasks change the tray icon and obey the ""End task"" hotkey; blue tasks don't.

This option is ignored when the task runs as .exe program started not from editor.
");
		_AddCombo("ifRunning", "warn|warn_restart|cancel|cancel_restart|wait|wait_restart|run|run_restart|restart", _meta.ifRunning,
@"<b>ifRunning</b> - when trying to start this script, what to do if it is already running.
 • <i>warn</i> (default) - write warning in output and don't run.
 • <i>cancel</i> - don't run.
 • <i>wait</i> - run later, when it ends.
 • <i>run</i> - run simultaneously. Requires runMode blue.
 • <i>restart</i> - end it and run.

Suffix _restart means: restart when clicked the Run button/menu; don't restart when using other ways to run the script.

This option is ignored when the task runs as .exe program started not from editor.
");
		_AddCombo("ifRunning2", "same|warn|cancel|wait", _meta.ifRunning2,
@"<b>ifRunning2</b> - when trying to start this green script, what to do if another green script is running.
 • <i>same</i> (default) - same as of ifRunning: cancel if cancel[_restart], wait if wait[_restart], else warn.
 • <i>warn</i> - write warning in output and don't run.
 • <i>cancel</i> - don't run.
 • <i>wait</i> - run later, when it ends.

This option is ignored when the task runs as .exe program started not from editor.
");
		_AddCombo("uac", "inherit|user|admin", _meta.uac,
@"<b>uac</b> - <help articles/UAC>UAC<> integrity level (IL) of the task process.
 • <i>inherit</i> (default) - the same as of the editor process. Normally High IL if installed on admin account, else Medium IL.
 • <i>user</i> - Medium IL, like most applications. The task cannot automate high IL process windows, write some files, change some settings, etc.
 • <i>admin</i> - High IL, aka ""administrator"", ""elevated"". The task has many rights, but cannot automate some apps through COM, etc.

This option is ignored when the task runs as .exe program started not from editor.
");
		_AddCombo("prefer32bit", "false|true", _meta.prefer32bit,
@"<b>prefer32bit</b> - whether the task process must be 32-bit everywhere.
 • <i>false</i> (default) - the process is 64-bit or 32-bit, the same as Windows on that computer.
 • <i>true</i> - the process is 32-bit on all computers.
");

		g.ZAddHeaderRow("Compile");
		_AddCombo("optimize", "false|true", _meta.optimize,
@"<b>optimize</b> - whether to make the compiled code as fast as possible.
 • <i>false</i> (default) - don't optimize. Define DEBUG and TRACE. This is known as ""Debug configuration"".
 • <i>true</i> - optimize. This is known as ""Release configuration"".

If true, low-level processing code is faster, but can be difficult to debug.
This option is also applied to class files compiled together. Use true if they contain code that must be as fast as possible. Not applied to used dlls.
");
		_AddEdit("define", _meta.define,
@"<b>define</b> - symbols that can be used with #if.
List separated by comma, semicolon or space. Example: TRACE,ETC
If no optimize true, DEBUG and TRACE are added implicitly.
These symbols also are visible in class files compiled together, but not in used dlls.
See also <google C# #define>#define<>.
");
		_AddCombo("warningLevel", "4|3|2|1|0", _meta.warningLevel,
@"<b>warningLevel</b> - how many warnings to show.
0 - no warnings.
1 - only severe warnings.
2 - level 1 warnings plus certain, less-severe warnings.
3 - most warnings.
4 (default) all warnings.

This option is also applied to class files compiled together.
 ");
		_AddEdit("noWarnings", _meta.noWarnings,
@"<b>noWarnings</b> - don't show these warnings.
List separated by comma, semicolon or space. Example: 151,3001,120

This option is also applied to class files compiled together.
See also <google C# #pragma warning>#pragma warning<>.
");
		_AddEdit("preBuild", _meta.preBuild,
@"<b>preBuild</b> - a script to run before compiling this code file.
Can be path relative to this file (examples: Script5.cs, Folder\Script5.cs, ..\Folder\Script5.cs) or path in the workspace (examples: \Script5.cs, \Folder\Script5.cs).

The script must have role editorExtension. It runs synchronously in compiler's thread. To stop compilation, let it throw an exception.
By default it receives full path of the output exe or dll file in args[0]. If need more info, specify command line arguments, like in this example: Script5.cs /$(outputPath) $(optimize). The script will receive real values in args[0], args[1] and so on. Variables:
 • $(outputFile) - full path of the output exe or dll file.
 • $(outputPath) - meta comment option 'outputPath'.
 • $(source) - path of this C# code file in the workspace.
 • $(role) - meta comment option 'role'.
 • $(optimize) - meta comment option 'optimize'.
 • $(prefer32bit) - meta comment option 'prefer32bit'.
");
		_AddEdit("postBuild", _meta.postBuild,
@"<b>postBuild</b> - a script to run after compiling this code file successfully.
Everything else is like with preBuild.
");

		g.ZAddHeaderRow("Assembly");
		_AddEdit("outputPath", _meta.outputPath,
@"<b>outputPath</b> - directory for the output assembly file and related files (used dlls, etc).
Full path. Can start with %environmentVariable% or %AFolders.SomeFolder%. Can be path relative to this file or workspace, like with other options. Default if role exeProgram: <link>%AFolders.Workspace%\bin<>. Default if role classLibrary: <link>%AFolders.ThisApp%\Libraries<>. The compiler creates the folder if does not exist.

If role exeProgram, the exe file is named like the script. The 32-bit version has suffix ""-32"". If optimize true, creates both 64-bit and 32-bit versions. Else creates only 32-bit if prefer32bit true or 32-bit OS, else only 64-bit.
If role classLibrary, the dll file is named like the class file. It can be used by 64-bit and 32-bit processes.
", noCheckbox: true, buttonAction: (sender, sed) => {
	var m = new AMenu();
	m[_role == ERole.classLibrary ? @"%AFolders.ThisApp%\Libraries" : @"%AFolders.Workspace%\bin"] = o => _SetEditCellText(o.ToString());
	m["Browse..."] = o => {
		var f = new FolderBrowserDialog { SelectedPath = AFolders.ThisAppDocuments, ShowNewFolderButton = true };
		if(f.ShowDialog(this) == DialogResult.OK) _SetEditCellText(f.SelectedPath);
		f.Dispose();
	};
	m.Show(sender as Control);
});
		_AddCombo("console", "false|true", _meta.console,
@"<b>console</b> - let the program run with console.
");
		_AddEdit("icon", _meta.icon,
@"<b>icon</b> - icon of the output assembly file.
The file must be in this workspace. Can be path relative to this file (examples: App.ico, Folder\App.ico, ..\Folder\App.ico) or path in the workspace (examples: \App.ico, \Folder\App.ico).

The icon will be added as a native resource and displayed in File Explorer etc.
");
		_AddEdit("manifest", _meta.manifest,
@"<b>manifest</b> - <google manifest file site:microsoft.com>manifest<> of the output assembly file.
The file must be in this workspace. Can be path relative to this file (examples: App.manifest, Folder\App.manifest, ..\Folder\App.manifest) or path in the workspace (examples: \App.manifest, \Folder\App.manifest).

The manifest will be added as a native resource.
");
		_AddEdit("resFile", _meta.resFile,
@"<b>resFile</b> - .res file containing resources to add to the output assembly file as native resources.
The file must be in this workspace. Can be path relative to this file or path in the workspace.

.res files contain compiled native resources of any type, including icons and manifest.
This option is rarely used. Instead you use managed resources (button ""Resource..."").
");
		_AddEdit("xmlDoc", _meta.xmlDoc,
@"<b>xmlDoc</b> - XML documentation file to create from XML comments of classes, functions, etc.
If not full path, the compiler creates the XML file in the 'outputPath' folder.

The XML file can be used to create a help file, for example with <google>Sandcastle Help File Builder<>.
");
		_AddEdit("sign", _meta.sign,
@"<b>sign</b> - strong-name signing key file, to sign the output assembly.
The file must be in this workspace. Can be path relative to this file (examples: App.snk, Folder\App.snk, ..\Folder\App.snk) or path in the workspace (examples: \App.snk, \Folder\App.snk).
");

		_SelectRole();
		_SelectRunMode();

		g.ZAutoSize(false, true); //and _SelectRole autosizes rows through ZShowRows

		g.ZValueChanged += _grid_ZValueChanged;

		void _AddCombo(string name, string values, string select, string info = null, bool noCheckbox = false, int index = -1)
		{
			var a = values.SegSplit("|");
			bool isSpecified = select != null;
			if(index < 0) {
				index = 0;
				if(isSpecified) {
					isSpecified = false;
					for(int i = 0; i < a.Length; i++) if(a[i] == select) { index = i; isSpecified = true; break; }
				}
			}
			g.ZAdd(null, name, values,
				noCheckbox ? default(bool?) : isSpecified,
				null, info,
				etype: ParamGrid.EditType.ComboList,
				comboIndex: index);
			if(info != null) _infoDict.Add(name, info);
		}

		void _AddEdit(string name, string text, string info = null, bool noCheckbox = false, EventHandler buttonAction = null)
		{
			g.ZAdd(null, name, text,
				noCheckbox ? default(bool?) : text != null,
				null, info,
				etype: buttonAction != null ? ParamGrid.EditType.TextButton : ParamGrid.EditType.Text,
				buttonAction: buttonAction);
			if(info != null) _infoDict.Add(name, info);
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
		var hide = _role switch
		{
			ERole.miniProgram => "testScript outputPath icon-xmlDoc",
			ERole.exeProgram => "testScript",
			ERole.editorExtension => "Run-prefer32bit outputPath-xmlDoc",
			ERole.classLibrary => "runMode-prefer32bit console manifest",
			_ => "runMode-",
		};
		_grid.ZShowRows(true, "Run-", hide);
		_bAddLibraryProject.Enabled = _role != ERole.classFile;
	}

	void _SelectRunMode()
	{
		_grid.ZShowRows(_Get("runMode") != "blue", "ifRunning2");
	}

	private void _grid_ZValueChanged(SourceGrid.CellContext cc)
	{
		var g = _grid;
		var p = cc.Position;
		int row = p.Row;

		if(row == 0) { //role
			var cb = cc.Cell.Editor as SourceGrid.Cells.Editors.ComboBox;
			_role = (ERole)cb.Control.SelectedIndex;
			_SelectRole();
			return;
		}

		//AOutput.Write(p.Column, p.Row, cc.IsEditing());

		//uncheck if selected default value. The control checks when changed.
		if(p.Column == 1 && cc.IsEditing()) {
			bool uncheck = false;
			switch(cc.Cell.Editor) {
			case SourceGrid.Cells.Editors.ComboBox cb:
				if(cb.Control.SelectedIndex <= 0) uncheck = true;
				break;
			case SourceGrid.Cells.Editors.TextBox tb:
				if((cc.Value as string).NE()) uncheck = true;
				break;
			}
			if(uncheck) g.ZCheck(row, false);
		}

		var rk = g.ZGetRowKey(row);

		//AOutput.Write(p.Column, row, g.ZIsChecked(row));

		switch(rk) {
		case "runMode":
			_SelectRunMode(); //show/hide ifRunning2
			if(_IsRunGreen()) g.ZSetCellText("ifRunning", 1, "warn");
			break;
		case "ifRunning": //if runMode green, cannot be ifRunning run[_restart]
			if(_IsRunGreen()) { g.ZSetCellText("runMode", 1, "blue"); g.ZCheck("runMode", true); }
			break;
		}
		bool _IsRunGreen() => cc.IsEditing() && (_Get("ifRunning")?.Starts("run") ?? false) && _Get("runMode") != "blue";

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
			if(fts == null) { ADialog.ShowInfo("testScript file not found", "Must be path relative to this file or path in worspace like \\file or \\folder\\file.", owner: this); return false; }
		}
		_f.TestScript = fts;

		//info: _Get returns null if hidden

		_meta.runMode = _Get("runMode");
		_meta.ifRunning = _Get("ifRunning");
		_meta.ifRunning2 = _Get("ifRunning2");
		_meta.uac = _Get("uac");
		_meta.prefer32bit = _Get("prefer32bit");

		_meta.optimize = _Get("optimize");
		_meta.warningLevel = _Get("warningLevel");
		_meta.noWarnings = _Get("noWarnings");
		_meta.define = _Get("define");
		_meta.preBuild = _Get("preBuild");
		_meta.postBuild = _Get("postBuild");

		_meta.outputPath = _Get("outputPath");
		_meta.console = _Get("console");
		_meta.icon = _Get("icon");
		_meta.manifest = _Get("manifest");
		_meta.resFile = _Get("resFile");
		_meta.sign = _Get("sign");
		_meta.xmlDoc = _Get("xmlDoc");

		_meta.role = null;
		if(_role != ERole.classFile) {
			if(_isClass || _role != ERole.miniProgram) _meta.role = _role.ToString();
			switch(_role) {
			case ERole.exeProgram:
			case ERole.classLibrary:
				if(_meta.outputPath.NE()) _meta.outputPath = _role == ERole.exeProgram ? @"%AFolders.Workspace%\bin" : @"%AFolders.ThisApp%\Libraries";
				break;
			}
			var name = APath.GetFileName(_f.Name, true);
			if(_meta.xmlDoc == "") _meta.xmlDoc = name + ".xml";
			if(_meta.manifest == "") _meta.manifest = name + ".exe.manifest";
		}

		return true;
	}

	private void _bOK_Click(object sender, EventArgs e)
	{
		if(Program.Model.CurrentFile != _f && !Program.Model.SetCurrentFile(_f)) return;
		var doc = Panels.Editor.ZActiveDoc;
		var t = doc.Z;
		var code = doc.Text;
		int endOf = MetaComments.FindMetaComments(code);

		if(!_GetGrid()) { this.DialogResult = DialogResult.None; return; };

		string append = null; if(endOf == 0) append = (_f.IsScript && code.Starts("//.\r")) ? " " : "\r\n";
		var s = _meta.Format(append);

		if(s.Length == 0) {
			if(endOf == 0) return;
			while(endOf < code.Length && code[endOf] <= ' ') endOf++;
		} else if(s.Length == endOf) {
			if(s == t.RangeText(true, 0, endOf)) return; //did not change
		}

		t.ReplaceRange(true, 0, endOf, s);
	}

	private void _bAddNet_Click(object button, EventArgs e)
	{
		using var d = new System.Windows.Forms.OpenFileDialog { InitialDirectory = AFolders.ThisApp, Filter = "Dll|*.dll|All files|*.*", Multiselect = true };
		if(d.ShowDialog(this) != DialogResult.OK) return;

		var a = d.FileNames;

		foreach(var v in a) {
			if(MetaReferences.IsDotnetAssembly(v)) continue;
			ADialog.ShowError("Not a .NET assembly.", v, owner: this);
			return;
		}

		//remove path and ext if need
		var thisApp = AFolders.ThisAppBS;
		if(a[0].Starts(thisApp, true)) {
			for(int i = 0; i < a.Length; i++) a[i] = a[i].Substring(thisApp.Length);
		}

		_meta.r.AddRange(a);
		_Added(button, _meta.r);
	}

	private void _bAddLibraryProject_Click(object sender, EventArgs e)
		=> _AddFromWorkspace(
			f => (f != _f && f.GetClassFileRole() == FileNode.EClassFileRole.Library) ? f : null,
			_meta.pr, sender);

	private void _bAddClass_Click(object sender, EventArgs e)
	{
		FileNode prFolder1 = null;
		if(_f.IsScript && _f.FindProject(out prFolder1, out var prMain1, ofAnyScript: true) && _f == prMain1) prFolder1 = null;

		bool _Include(FileNode f)
		{
			if(!f.IsClass || f == _f) return false;
			if(f.FindProject(out var prFolder, out var prMain)) { //exclude class files that are in projects
				if(prFolder != prFolder1) return false; //but if _f is a non-project script in a project folder, include local classes
			}
			return f.GetClassFileRole() == FileNode.EClassFileRole.Class;
		}

		_AddFromWorkspace(f => _Include(f) ? f : null, _meta.c, sender);
	}

	private void _bAddResource_Click(object sender, EventArgs e)
		=> _AddFromWorkspace(
			f => !(f.IsFolder || f.IsCodeFile) ? f : null,
			_meta.resource, sender);

	void _AddFromWorkspace(Func<FileNode, FileNode> filter, List<string> metaList, object button)
	{
		var sFind = _tFindInList.Text;
		var a = new List<string>();
		foreach(var f in Program.Model.Root.Descendants()) {
			var f2 = filter(f);
			if(f2 == null) continue;
			var path = f2.ItemPath;
			if(sFind.Length > 0 && path.Find(sFind, true) < 0) continue;
			if(!metaList.Contains(path, StringComparer.OrdinalIgnoreCase)) a.Add(path);
		}
		if(a.Count == 0) { _NotFound(button, sFind); return; }
		a.Sort();
		var dd = new PopupList {
			Items = a.ToArray(),
			SelectedAction = o => {
				metaList.Add(o.ResultItem as string);
				_Added(button, metaList);
			}
		};
		dd.Show(button as Control);
	}

	void _NotFound(object button, string sFind)
	{
		var s = "The list is empty";
		if(sFind.Length > 0) s = "The list contains 0 items containing " + sFind;
		AOsd.ShowText(s, 3, _OsdXY(button));
	}

	void _Added(object button, List<string> metaList)
	{
		AOsd.ShowText(string.Join("\r\n", metaList) + "\r\n\r\nFinally click OK to save.", 5, _OsdXY(button));
	}

	static PopupXY _OsdXY(object button)
	{
		var c = button as Control;
		return PopupXY.In(c.Hwnd().Rect, c.Width, 0);
	}

	#region COM

	private void _bAddComBrowse_Click(object button, EventArgs e)
	{
		using var ofd = new System.Windows.Forms.OpenFileDialog { Filter = "Type library|*.dll;*.tlb;*.olb;*.ocx;*.exe|All files|*.*" };
		if(ofd.ShowDialog(this) == DialogResult.OK) _ConvertTypeLibrary(ofd.FileName, button);
	}

	private void _bAddComRegistry_Click(object button, EventArgs e)
	{
		//HKCU\TypeLib\typelibGuid\version\
		var sFind = _tFindInList.Text;
		var rx = new ARegex(@"(?i) (?:Type |Object )?Library[ \d\.]*$");
		var a = new List<_RegTypelib>(1000);
		using(var tlKey = Registry.ClassesRoot.OpenSubKey("TypeLib")) { //guids
			foreach(var sGuid in tlKey.GetSubKeyNames()) {
				if(sGuid.Length != 38) continue;
				//AOutput.Write(sGuid);
				using var guidKey = tlKey.OpenSubKey(sGuid);
				foreach(var sVer in guidKey.GetSubKeyNames()) {
					using var verKey = guidKey.OpenSubKey(sVer);
					if(verKey.GetValue("") is string description) {
						if(rx.MatchG(description, out var g)) description = description.Remove(g.Start);
						if(sFind.Length > 0 && description.Find(sFind, true) < 0) continue;
						a.Add(new _RegTypelib { guid = sGuid, text = description + ", " + sVer, version = sVer });
					} //else AOutput.Write(sGuid); //some Microsoft typelibs. VS does not show these too.
				}
			}
		}
		if(a.Count == 0) { _NotFound(button, sFind); return; }
		a.Sort((x, y) => string.Compare(x.text, y.text, true));

		var dd = new PopupList { Items = a.ToArray(), SelectedAction = o => _ConvertTypeLibrary(o.ResultItem as _RegTypelib, button) };
		dd.Show(_bAddComRegistry);
	}

	static string _convertedDir;

	//To convert a COM type library we use TypeLibConverter class. However .NET Core does not have it.
	//Workaround: the code is in Au.Net45.exe. It uses .NET Framework 4.5. We call it through RunConsole.
	//We don't use tlbimp.exe:
	//	1. If some used interop assemblies are in GAC (eg MS Office PIA), does not create files for them. But we cannot use GAC in Core app.
	//	2. Does not tell what files created.
	//	3. My PC somehow has MS Office PIA installed and there is no uninstaller. After deleting the GAC files tlbimp.exe created all files, but it took several minutes.
	//Tested: impossible to convert .NET Framework TypeLibConverter code. Part of it is in extern methods.
	//Tested: cannot use .NET Framework dll for it. Fails at run time because uses Core assemblies, and they don't have the class. Need exe.

	class _RegTypelib
	{
		public string text, guid, version;

		public override string ToString() => text;

		public string GetPath(string locale)
		{
			var k0 = $@"TypeLib\{guid}\{version}\{locale}\win";
			for(int i = 0; i < 2; i++) {
				var bits = AVersion.Is32BitProcess == (i == 1) ? "32" : "64";
				using var hk = Registry.ClassesRoot.OpenSubKey(k0 + bits);
				if(hk?.GetValue("") is string path) return path.Trim('\"');
			}
			return null;
		}
	}

	async void _ConvertTypeLibrary(object tlDef, object button)
	{
		string comDll = null;
		switch(tlDef) {
		case string path:
			comDll = path;
			break;
		case _RegTypelib r:
			//can be several locales
			var aloc = new List<string>(); //registry keys like "0" or "409"
			var aloc2 = new List<string>(); //locale names for display in the list dialog
			using(var verKey = Registry.ClassesRoot.OpenSubKey($@"TypeLib\{r.guid}\{r.version}")) {
				foreach(var s1 in verKey.GetSubKeyNames()) {
					int lcid = s1.ToInt(0, out int iEnd, STIFlags.IsHexWithout0x);
					if(iEnd != s1.Length) continue; //"FLAGS" etc; must be hex number without 0x
					aloc.Add(s1);
					var s2 = "Neutral";
					if(lcid > 0) {
						try { s2 = new CultureInfo(lcid).DisplayName; } catch { s2 = s1; }
					}
					aloc2.Add(s2);
				}
			}
			string locale;
			if(aloc.Count == 1) locale = aloc[0];
			else {
				int i = ADialog.ShowList(aloc2, "Locale", owner: this);
				if(i == 0) return;
				locale = aloc[i - 1];
			}
			comDll = r.GetPath(locale);
			if(comDll == null || !AFile.ExistsAsFile(comDll)) {
				ADialog.ShowError(comDll == null ? "Failed to get file path." : "File does not exist.", owner: this);
				return;
			}
			break;
		}

		await Task.Run(() => {
			this.Enabled = false;
			AOutput.Write($"Converting COM type library to .NET assembly.");
			try {
				if(_convertedDir == null) {
					_convertedDir = AFolders.Workspace + @".interop\";
					AFile.CreateDirectory(_convertedDir);
				}
				List<string> converted = new List<string>();
				Action<string> callback = s => {
					AOutput.Write(s);
					if(s.Starts("Converted: ")) {
						s.RegexMatch(@"""(.+?)"".$", 1, out s);
						converted.Add(s);
					}
				};
				int rr = AExec.RunConsole(callback, AFolders.ThisAppBS + "Au.Net45.exe", $"/typelib \"{_convertedDir}|{comDll}\"", encoding: Encoding.UTF8);
				if(rr == 0) {
					foreach(var v in converted) if(!_meta.com.Contains(v)) _meta.com.Add(v);
					AOutput.Write(@"<>Converted and saved in <link>%AFolders.Workspace%\.interop<>.");
					_Added(button, _meta.com);
				}
			}
			catch(Exception ex) { ADialog.ShowError("Failed to convert type library", ex.ToStringWithoutStack(), owner: this); }
			this.Enabled = true;
		});
	}

	#endregion

	#region info

	Dictionary<string, string> _infoDict;

	void _InfoInit()
	{
		const string c_script = @"This file is a C# script. There are several ways to run a script:
1. Click the Run button or menu item.
2. Add script name in Options -> General -> Run scripts when this workspace loaded.
3. Call <help>ATask.Run<> from another script. Example: <code>ATask.Run(""Script8.cs"");</code>
4. <help editor/Command line>Command line<>. Example: <code>Au.CL.exe ""Script8.cs""</code>
5. An output link. Example: <code>AOutput.Write(""<>Click to run <script>Script8.cs<>."");</code>

In script code you can add <help Au.Triggers.ActionTriggers>triggers<> (hotkey etc) to execute parts of script code when it is running. There are no such triggers to launch scripts.
";
		const string c_class = "This file is a C# class file.";
		_info.Z.SetText(
@"C# file properties here are similar to C# project properties in Visual Studio. This program does not use project files. A C# file is like a project. Its properties are saved in <c green>/*/ meta comments /*/<> at the very start of code. Can be changed here or in the code editor.

" + (_isClass ? c_class : c_script));

		_infoDict = new Dictionary<string, string>(32);
		_Add(_bAddNet,
@"<b>Assembly<> - add one or more .NET assemblies (.dll files) as references.
Adds meta <c green>r FileName<>.

Don't need to add Au.dll and .NET Core runtime dlls.
To use 'extern alias', edit in the code editor like this: <c green>r Alias=Assembly<>
To remove, delete the line in the code editor.

If the file is in <link>%AFolders.ThisApp%<> or its subfolders, use file name or relative path, else need full path. If role of the script is not miniProgram, at run time the file must be directly in AFolders.ThisApp or AFolders.ThisApp\Libraries. If role is editorExtension, may need to restart editor.
");
		const string c_com = @" COM component's type library to an <i>interop assembly<>.
Adds meta <c green>com FileName.dll<>. Saves the assembly file in <link>%AFolders.Workspace%\.interop<>.

An interop assembly is a .NET assembly without real code. Not used at run time. At run time is used the COM component (registered native dll or exe file). Set prefer32bit true if 64-bit dll unavailable.

To remove, delete the line in the code editor. Optionally delete unused interop assemblies.
";
		_Add(_bAddComRegistry,
@"<b>COM<> - convert a registered" + c_com);
		_Add(_bAddComBrowse,
@"<b>...<> - convert a" + c_com);
		_Add(_bAddLibraryProject,
@"<b>Library project<> - add a reference to a class library created in this workspace.
Adds meta <c green>pr File.cs<>. The compiler will compile it if need and use the created dll file as a reference.

The recommended outputPath of the library project is <link>%AFolders.ThisApp%\Libraries<>. Else may not find the dll at run time.

To remove, delete the line in the code editor. Optionally delete unused dll files.
");
		_Add(_bAddClass,
@"<b>Class file<> - add a C# code file that contains some classes/functions used by this file.
Adds meta <c green>c File.cs<>. The compiler will compile all code files and create single assembly.

If this file is in a project, don't need to add class files that are in the project folder.
Can be added only files that are in this workspace. Import files if need, for example drag/drop.
Can be path relative to this file (examples: Class5.cs, Folder\Class5.cs, ..\Folder\Class5.cs) or path in the workspace (examples: \Class5.cs, \Folder\Class5.cs).
To remove, delete the line in the code editor.
");
		_Add(_bAddResource,
@"<b>Resource<> - add an image or other file as a managed resource.
Adds meta <c green>resource File<>. The compiler will add the file to the output assembly.

Resource type: .png, .bmp, .jpg, .jpeg, .gif, .tif, .tiff - Bitmap. .ico - Icon. Other - byte[].
To add a text file as string, in the code editor edit like this: <c green>resource file.txt /string<>
To add multiple strings, add a 2-column CSV file. Edit: <c green>resource file.csv /strings<>

Need to add resource files even if they are in project folder.
Can be added only files that are in this workspace. Import files if need, for example drag/drop.
Can be path relative to this file (examples: File.png, Folder\File.png, ..\Folder\File.png) or path in the workspace (examples: \File.png, \Folder\File.png).
To remove, delete the line in the code editor.

Examples of loading resources at run time:
<code>var bitmap = Au.Util.AResources.GetAppResource(""file.png"") as Bitmap;</code>
<code>var icon = new Icon(Au.Util.AResources.GetAppResource(""file.ico"") as Icon, 16, 16);</code>
<code>var cursor = Au.Util.ACursor.LoadCursorFromMemory(Au.Util.AResources.GetAppResource(""file.cur"") as byte[]);</code>
");
		_Add(_tFindInList,
@"<b>Find in lists<> - in the drop-down lists show only items containing this text.
");

		void _Add(Control c, string s) => _infoDict.Add(c.Name, s);

		_infoRow = -1;
		_infoTimer = new ATimer(_ => _SetInfoText(_infoText));
		_grid.ZShowEditInfo += (cc, rowInfo) => { _SetInfoText(rowInfo); _gridEditMode = rowInfo != null; };

		void _SetInfoText(string infoText)
		{
			if(infoText == null || _gridEditMode || infoText == _infoTextPrev) return;
			_infoTextPrev = infoText;
			_info.Z.SetText(infoText);
		}
	}

	void _InfoOnMouseMove(AWnd w, LPARAM xy)
	{
		if(_gridEditMode) return;
		AWnd wForm = (AWnd)this;
		if(w == wForm || w.Get.Window != wForm) {
			_infoWnd = default;
			_infoRow = -1;
			_infoText = null;
			return;
		}

		if(w == (AWnd)_grid) {
			var pp = _grid.PositionAtPoint(new Point(AMath.LoShort(xy), AMath.HiShort(xy)));
			if(pp.Row != _infoRow) {
				_infoText = null;
				_infoRow = pp.Row;
				if(_infoRow >= 0) _SetTimer(_grid.ZGetRowKey(_infoRow));
			}
		} else {
			_infoRow = -1;
			if(w != _infoWnd) {
				_infoText = null;
				_SetTimer(Control.FromHandle(w.Handle)?.Name);
			}
		}
		_infoWnd = w;

		void _SetTimer(string name)
		{
			//AOutput.Write(name);
			if(name.NE() || !_infoDict.TryGetValue(name, out _infoText)) return;
			_infoTimer.After(700);
		}
	}
	AWnd _infoWnd;
	int _infoRow;
	//Point _infoXY;
	ATimer _infoTimer;
	string _infoText, _infoTextPrev;
	bool _gridEditMode;

	bool IMessageFilter.PreFilterMessage(ref Message m)
	{
		switch(m.Msg) {
		case Api.WM_MOUSEMOVE:
			_InfoOnMouseMove((AWnd)m.HWnd, m.LParam);
			break;
		}
		return false;
	}

	#endregion
}
