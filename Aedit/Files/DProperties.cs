using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Au;
using Au.Types;
using Au.Controls;
using Au.Compiler;
using System.Globalization;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;

class DProperties : KDialogWindow
{
	readonly FileNode _f;
	readonly MetaCommentsParser _meta;
	readonly bool _isClass;
	ERole _role;

	//controls
	readonly KSciInfoBox info;
	readonly ComboBox role, ifRunning, ifRunning2, uac, warningLevel;
	readonly TextBox testScript, outputPath, icon, manifest, sign, define, noWarnings, testInternal, preBuild, postBuild, findInLists;
	readonly KCheckBox runSingle, prefer32bit, xmlDoc, console, optimize;
	readonly Expander gRun, gAssembly, gCompile;
	readonly Button addAssembly, addComBrowse, addComRegistry, addProject, addClassFile, addResource, outputPathB;

	public DProperties(FileNode f) {
		_f = f;
		_isClass = f.IsClass;

		Owner = App.Wmain;
		Title = "Properties of " + _f.Name;

		var b = new AWpfBuilder(this).Columns(440.., 0);
		b.WinProperties(WindowStartupLocation.CenterOwner, showInTaskbar: false);
		b.R.Add(out info).Height(80).Margin("B8").Span(-1);
		b.R.StartStack(vertical: true); //left column
		b.StartGrid().Columns(0, -1, 20, 0, -1.15)
			.R.Add("role", out role).Skip()
			.Add("testScript", out testScript).Validation(o => _ValidateFile(o, "testScript"));
		b.End();
		b.StartStack(out gRun, "Run", vertical: true);
		b.StartGrid().Columns(0, 120, 20, 0, 0, -1)
			.Add("ifRunning", out ifRunning).Skip()
			.Add(out runSingle, "runSingle").Align(y: "C").Margin("R20")
			.Add("ifRunning2", out ifRunning2);
		b.R.Add("uac", out uac).Skip()
			.Add(out prefer32bit, "prefer32bit").Align(y: "C").Span(1);
		b.End();
		b.End().Brush(Brushes.OldLace);
		b.StartStack(out gAssembly, "Assembly", vertical: true);
		b.StartGrid().Columns(0, -1, 30)
			.Add("outputPath", out outputPath)
			.AddButton(out outputPathB, "...", _ButtonClick_outputPath)
			.End();
		b.StartGrid().Columns(0, -1, 20, 0, -1);
		b.R.Add("icon", out icon).Skip().Validation(o => _ValidateFile(o, "icon"))
			.Add("manifest", out manifest).Validation(o => _ValidateFile(o, "manifest"));
		b.R.Add("sign", out sign).Skip().Validation(o => _ValidateFile(o, "sign"))
			.Add(out xmlDoc, "xmlDoc").Align(y: "C")
			.Add(out console, "console").Align("R", "C");
		b.End();
		b.End().Brush(Brushes.OldLace);
		b.StartGrid(out gCompile, "Compile").Columns(0, 50, 20, 0, -1);
		b.R.Add(out optimize, "optimize").Align(y: "C").Skip(2)
			.Add("define", out define);
		b.R.Add("warningLevel", out warningLevel).Editable().Skip()
			.Add("noWarnings", out noWarnings);
		b.R.Add("testInternal", out testInternal);
		b.R.StartGrid().Columns(0, -1, 20, 0, -1)
			.Add("preBuild", out preBuild).Skip().Validation(o => _ValidateFile(o, "preBuild"))
			.Add("postBuild", out postBuild).Validation(o => _ValidateFile(o, "postBuild"));
		b.End();
		b.End().Brush(Brushes.OldLace);
		b.End();
		b.StartStack(vertical: true).Margin("L20"); //right column
		b.StartGrid<GroupBox>("Add reference");
		b.R.AddButton(out addAssembly, "Assembly...", _ButtonClick_addNet);
		b.R.AddButton(out addComBrowse, "COM ▾", _bAddComBrowse_Click).AddButton(out addComRegistry, "...", _bAddComRegistry_Click).Width(30);
		b.AddButton(out addProject, "Project ▾", _ButtonClick_addProject);
		b.End();
		b.StartStack<GroupBox>("Add file", vertical: true);
		b.AddButton(out addClassFile, "Class file ▾", _ButtonClick_addClass);
		b.AddButton(out addResource, "Resource ▾", _ButtonClick_addResource);
		b.End();
		b.StartStack(vertical: true).Add("Find in lists", out findInLists).Tooltip("In button drop-down lists show only items containing this text").End();
		b.End();
		b.R.AddOkCancel();
		b.End();

		//b.Loaded += () => {
		//};

		_meta = new MetaCommentsParser(_f);

		_role = _meta.role switch {
			"miniProgram" => ERole.miniProgram,
			"exeProgram" => ERole.exeProgram,
			"editorExtension" => ERole.editorExtension,
			"classLibrary" when _isClass => ERole.classLibrary,
			"classFile" when _isClass => ERole.classFile,
			_ => _isClass ? ERole.classFile : ERole.miniProgram,
		};
		_InitCombo(role, _isClass ? "miniProgram|exeProgram|editorExtension|classLibrary|classFile" : "miniProgram|exeProgram|editorExtension", null, (int)_role);
		testScript.Text = _f.TestScript?.ItemPath;
		//Run
		_InitCombo(ifRunning, "warn|warn_restart|cancel|cancel_restart|wait|wait_restart|run|run_restart|restart", _meta.ifRunning);
		if (_meta.runSingle == "true") runSingle.IsChecked = true;
		_InitCombo(ifRunning2, "same|warn|cancel|wait", _meta.ifRunning2);
		_InitCombo(uac, "inherit|user|admin", _meta.uac);
		if (_meta.prefer32bit == "true") prefer32bit.IsChecked = true;
		//Assembly
		outputPath.Text = _meta.outputPath;
		void _ButtonClick_outputPath(WBButtonClickArgs e) {
			var m = new AWpfMenu();
			m[_GetOutputPath(getDefault: true)] = o => outputPath.Text = o.ToString();
			m["Browse..."] = o => {
				using var fd = new System.Windows.Forms.FolderBrowserDialog {
					SelectedPath = _GetOutputPath(getDefault: false, expandEnvVar: true),
					ShowNewFolderButton = true,
				};
				if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) outputPath.Text = fd.SelectedPath;
			};
			m.Show(e.Button);
		}
		icon.Text = _meta.icon;
		manifest.Text = _meta.manifest;
		sign.Text = _meta.sign;
		if (_meta.xmlDoc == "true") xmlDoc.IsChecked = true;
		if (_meta.console == "true") console.IsChecked = true;
		//Compile
		if (_meta.optimize == "true") optimize.IsChecked = true;
		define.Text = _meta.define;
		_InitCombo(warningLevel, "5|4|3|2|1|0", _meta.warningLevel);
		noWarnings.Text = _meta.noWarnings;
		testInternal.Text = _meta.testInternal;
		preBuild.Text = _meta.preBuild;
		postBuild.Text = _meta.postBuild;

		static void _InitCombo(ComboBox c, string items, string meta, int index = -1) {
			var a = items.Split('|');
			if (index < 0 && meta != null) index = Array.IndexOf(a, meta);
			foreach (var v in a) c.Items.Add(v);
			c.SelectedIndex = Math.Max(0, index);
		}

		gRun.IsExpanded = true;
#if DEBUG
		gAssembly.IsExpanded = true;
		gCompile.IsExpanded = true;
#else
		gAssembly.IsExpanded = _role == ERole.exeProgram || _role == ERole.classLibrary;
#endif

		_ChangedRole();
		role.SelectionChanged += (_, _) => {
			_role = (ERole)role.SelectedIndex;
			_ChangedRole();
		};
		void _ChangedRole() {
			_ShowHide(testScript, _role == ERole.classLibrary || _role == ERole.classFile);
			_ShowCollapse(_role == ERole.miniProgram || _role == ERole.exeProgram, gRun, console);
			_ChangedRunSingle();
			_ShowCollapse(_role == ERole.exeProgram || _role == ERole.classLibrary, outputPath, outputPathB, xmlDoc);
			_ShowCollapse(_role == ERole.exeProgram, icon, manifest);
			_ShowCollapse(_role != ERole.classFile, gAssembly, gCompile);
			addProject.IsEnabled = _role != ERole.classFile;
		}
		void _ChangedRunSingle() {
			_ShowHide(ifRunning2, _IsChecked(runSingle));
		}
		runSingle.CheckChanged += (_, _) => {
			_ChangedRunSingle();
			if (_IsIfRunningRunAndRunSingle()) ifRunning.SelectedItem = "warn_restart";
		};
		ifRunning.SelectionChanged += (_, _) => {
			if (_IsIfRunningRunAndRunSingle()) runSingle.IsChecked = false;
		};
		bool _IsIfRunningRunAndRunSingle() => _IsChecked(runSingle) && (_Get(ifRunning)?.Starts("run") ?? false);

		string _ValidateFile(FrameworkElement e, string name) {
			return (_Get(e as TextBox) is string s && null == _f.FindRelative(s, false)) ? name + " file not found" : null;
		}

		b.OkApply += _OkApply;
		_InitInfo();
	}

	void _GetMeta() {
		//info: _Get returns null if hidden

		_f.TestScript = _Get(testScript) is string sts ? _f.FindRelative(sts, false) : null; //validated

		_meta.ifRunning = _Get(ifRunning, nullIfDefault: true);
		_meta.runSingle = _Get(runSingle);
		_meta.ifRunning2 = _Get(ifRunning2, nullIfDefault: true);
		_meta.uac = _Get(uac, nullIfDefault: true);
		_meta.prefer32bit = _Get(prefer32bit);

		_meta.console = _Get(console);
		_meta.icon = _Get(icon);
		_meta.manifest = _Get(manifest);
		//_meta.resFile = _Get(resFile);
		_meta.sign = _Get(sign);
		_meta.xmlDoc = _Get(xmlDoc);

		_meta.optimize = _Get(optimize);
		_meta.define = _Get(define);
		_meta.warningLevel = _Get(warningLevel, nullIfDefault: true);
		_meta.noWarnings = _Get(noWarnings);
		_meta.testInternal = _Get(testInternal);
		_meta.preBuild = _Get(preBuild);
		_meta.postBuild = _Get(postBuild);

		_meta.role = null;
		_meta.outputPath = null;
		if (_role != ERole.classFile) {
			if (_isClass || _role != ERole.miniProgram) _meta.role = _role.ToString();
			switch (_role) {
			case ERole.exeProgram:
			case ERole.classLibrary:
				_meta.outputPath = _GetOutputPath(getDefault: false);
				break;
			}
		}
	}

	private void _OkApply(WBButtonClickArgs e) {
		if (App.Model.CurrentFile != _f && !App.Model.SetCurrentFile(_f)) return;

		_GetMeta();

		var doc = Panels.Editor.ZActiveDoc;
		var t = doc.Z;
		var code = doc.Text;
		int endOf = MetaComments.FindMetaComments(code);
		string append = null; if (endOf == 0) append = (_f.IsScript && code.Starts("//.\r")) ? " " : "\r\n";
		var s = _meta.Format(append);

		if (s.Length == 0) {
			if (endOf == 0) return;
			while (endOf < code.Length && code[endOf] <= ' ') endOf++;
		} else if (s.Length == endOf) {
			if (s == t.RangeText(true, 0, endOf)) return; //did not change
		}

		t.ReplaceRange(true, 0, endOf, s);
	}

	private void _ButtonClick_addNet(WBButtonClickArgs e) {
		var dir = AFolders.ThisApp + "Libraries"; if (!AFile.ExistsAsDirectory(dir)) dir = AFolders.ThisApp;
		var d = new OpenFileDialog { InitialDirectory = dir, Filter = "Dll|*.dll|All files|*.*", Multiselect = true };
		if (d.ShowDialog(this) != true) return;

		var a = d.FileNames;

		foreach (var v in a) {
			if (MetaReferences.IsDotnetAssembly(v)) continue;
			ADialog.ShowError("Not a .NET assembly.", v, owner: this);
			return;
		}

		//remove path and ext if need
		var thisApp = AFolders.ThisAppBS;
		if (a[0].Starts(thisApp, true)) {
			for (int i = 0; i < a.Length; i++) a[i] = a[i][thisApp.Length..];
		}

		_meta.r.AddRange(a);
		_ShowInfo_Added(e.Button, _meta.r);
	}

	private void _ButtonClick_addProject(WBButtonClickArgs e)
		=> _AddFromWorkspace(
			f => (f != _f && f.GetClassFileRole() == FileNode.EClassFileRole.Library) ? f : null,
			_meta.pr, e.Button);

	private void _ButtonClick_addClass(WBButtonClickArgs e) {
		FileNode prFolder1 = null;
		if (_f.IsScript && _f.FindProject(out prFolder1, out var prMain1, ofAnyScript: true) && _f == prMain1) prFolder1 = null;

		bool _Include(FileNode f) {
			if (!f.IsClass || f == _f) return false;
			if (f.FindProject(out var prFolder, out var prMain) && !prFolder.Name.Starts("@@")) { //exclude class files that are in projects, except if project name starts with @@
				if (prFolder != prFolder1) return false; //but if _f is a non-project script in a project folder, include local classes
			}
			return f.GetClassFileRole() == FileNode.EClassFileRole.Class;
		}

		_AddFromWorkspace(f => _Include(f) ? f : null, _meta.c, e.Button);
	}

	private void _ButtonClick_addResource(WBButtonClickArgs e) {
		_AddFromWorkspace(
			f => {
				if (f.IsCodeFile) return null;
				if (f.IsFolder) { //add if contains non-code files and does not contain code files
					bool hasRes = false;
					foreach (var v in f.Descendants()) { if (v.IsCodeFile) return null; hasRes |= !v.IsFolder; }
					if (!hasRes) return null;
				}
				return f;
			},
			_meta.resource, e.Button);
	}

	void _AddFromWorkspace(Func<FileNode, FileNode> filter, List<string> metaList, Button button) {
		var sFind = findInLists.Text;
		var a = new List<string>();
		foreach (var f in App.Model.Root.Descendants()) {
			var f2 = filter(f);
			if (f2 == null) continue;
			var path = f2.ItemPath;
			if (sFind.Length > 0 && path.Find(sFind, true) < 0) continue;
			if (!metaList.Contains(path, StringComparer.OrdinalIgnoreCase)) a.Add(path);
		}
		if (a.Count == 0) { _ShowInfo_ListEmpty(button, sFind); return; }
		a.Sort();
		var p = new KPopupListBox { PlacementTarget = button };
		p.Control.ItemsSource = a;
		p.OK += o => {
			metaList.Add(o as string);
			_ShowInfo_Added(button, metaList);
		};
		p.IsOpen = true;
	}

	#region COM

	private void _bAddComBrowse_Click(WBButtonClickArgs e) {
		var ofd = new OpenFileDialog { Filter = "Type library|*.dll;*.tlb;*.olb;*.ocx;*.exe|All files|*.*" };
		if (ofd.ShowDialog(this) == true) _ConvertTypeLibrary(ofd.FileName, e.Button);
	}

	private void _bAddComRegistry_Click(WBButtonClickArgs e) {
		//HKCU\TypeLib\typelibGuid\version\
		var sFind = findInLists.Text;
		var rx = new ARegex(@"(?i) (?:Type |Object )?Library[ \d\.]*$");
		var a = new List<_RegTypelib>(1000);
		using (var tlKey = Registry.ClassesRoot.OpenSubKey("TypeLib")) { //guids
			foreach (var sGuid in tlKey.GetSubKeyNames()) {
				if (sGuid.Length != 38) continue;
				//AOutput.Write(sGuid);
				using var guidKey = tlKey.OpenSubKey(sGuid);
				foreach (var sVer in guidKey.GetSubKeyNames()) {
					using var verKey = guidKey.OpenSubKey(sVer);
					if (verKey.GetValue("") is string description) {
						if (rx.MatchG(description, out var g)) description = description.Remove(g.Start);
						if (sFind.Length > 0 && description.Find(sFind, true) < 0) continue;
						a.Add(new _RegTypelib { guid = sGuid, text = description + ", " + sVer, version = sVer });
					} //else AOutput.Write(sGuid); //some Microsoft typelibs. VS does not show these too.
				}
			}
		}
		if (a.Count == 0) { _ShowInfo_ListEmpty(e.Button, sFind); return; }
		a.Sort((x, y) => string.Compare(x.text, y.text, true));

		var p = new KPopupListBox { PlacementTarget = e.Button };
		p.Control.ItemsSource = a;
		p.OK += o => {
			_ConvertTypeLibrary(o as _RegTypelib, e.Button);
		};
		p.IsOpen = true;
	}

	//To convert a COM type library we use TypeLibConverter class. However .NET Core does not have it (not tested .NET 5).
	//Workaround: the code is in Au.Net45.exe. It uses .NET Framework 4.5. We call it through RunConsole.
	//We don't use tlbimp.exe:
	//	1. If some used interop assemblies are in GAC (eg MS Office PIA), does not create files for them. But we cannot use GAC in Core/5 app.
	//	2. Does not tell what files created.
	//	3. My PC somehow has MS Office PIA installed and there is no uninstaller. After deleting the GAC files tlbimp.exe created all files, but it took several minutes.
	//Tested: impossible to convert .NET Framework TypeLibConverter code. Part of it is in extern methods.
	//Tested: cannot use .NET Framework dll for it. Fails at run time because uses Core/5 assemblies, and they don't have the class. Need exe.

	class _RegTypelib
	{
		public string text, guid, version;

		public override string ToString() => text;

		public string GetPath(string locale) {
			var k0 = $@"TypeLib\{guid}\{version}\{locale}\win";
			for (int i = 0; i < 2; i++) {
				var bits = AVersion.Is32BitProcess == (i == 1) ? "32" : "64";
				using var hk = Registry.ClassesRoot.OpenSubKey(k0 + bits);
				if (hk?.GetValue("") is string path) return path.Trim('\"');
			}
			return null;
		}
	}

	static string s_comConvertedDir;

	async void _ConvertTypeLibrary(object tlDef, Button button) {
		string comDll = null;
		switch (tlDef) {
		case string path:
			comDll = path;
			break;
		case _RegTypelib r:
			//can be several locales
			var aloc = new List<string>(); //registry keys like "0" or "409"
			var aloc2 = new List<string>(); //locale names for display in the list dialog
			using (var verKey = Registry.ClassesRoot.OpenSubKey($@"TypeLib\{r.guid}\{r.version}")) {
				foreach (var s1 in verKey.GetSubKeyNames()) {
					int lcid = s1.ToInt(0, out int iEnd, STIFlags.IsHexWithout0x);
					if (iEnd != s1.Length) continue; //"FLAGS" etc; must be hex number without 0x
					aloc.Add(s1);
					var s2 = "Neutral";
					if (lcid > 0) {
						try { s2 = new CultureInfo(lcid).DisplayName; } catch { s2 = s1; }
					}
					aloc2.Add(s2);
				}
			}
			string locale;
			if (aloc.Count == 1) locale = aloc[0];
			else {
				int i = ADialog.ShowList(aloc2, "Locale", owner: this);
				if (i == 0) return;
				locale = aloc[i - 1];
			}
			comDll = r.GetPath(locale);
			if (comDll == null || !AFile.ExistsAsFile(comDll)) {
				ADialog.ShowError(comDll == null ? "Failed to get file path." : "File does not exist.", owner: this);
				return;
			}
			break;
		}

		AOutput.Write($"Converting COM type library to .NET assembly.");
		List<string> converted = new();
		int rr = -1;
		this.IsEnabled = false;
		try {
			await Task.Run(() => {
				if (s_comConvertedDir == null) {
					s_comConvertedDir = AFolders.Workspace + @".interop\";
					AFile.CreateDirectory(s_comConvertedDir);
				}
				void _Callback(string s) {
					AOutput.Write(s);
					if (s.Starts("Converted: ")) {
						s.RegexMatch(@"""(.+?)"".$", 1, out s);
						converted.Add(s);
					}
				}
				rr = AFile.RunConsole(_Callback, AFolders.ThisAppBS + "Au.Net45.exe", $"/typelib \"{s_comConvertedDir}|{comDll}\"", encoding: Encoding.UTF8);
			});
		}
		catch (Exception ex) { ADialog.ShowError("Failed to convert type library", ex.ToStringWithoutStack(), owner: this); }
		this.IsEnabled = true;
		if (rr == 0) {
			foreach (var v in converted) if (!_meta.com.Contains(v)) _meta.com.Add(v);
			AOutput.Write(@"<>Converted and saved in <link>%AFolders.Workspace%\.interop<>.");
			_ShowInfo_Added(button, _meta.com);
		}
	}

	#endregion

	#region util

	static void _Show(FrameworkElement e, Visibility vis) {
		e.Visibility = vis;
		if (System.Windows.Automation.AutomationProperties.GetLabeledBy(e) is UIElement label) label.Visibility = vis;
	}

	static void _ShowHide(FrameworkElement e, bool show) => _Show(e, show ? Visibility.Visible : Visibility.Hidden);

	static void _ShowCollapse(FrameworkElement e, bool show) => _Show(e, show ? Visibility.Visible : Visibility.Collapsed);

	static void _ShowHide(bool show, params FrameworkElement[] a) {
		foreach (var v in a) _ShowHide(v, show);
	}

	static void _ShowCollapse(bool show, params FrameworkElement[] a) {
		foreach (var v in a) _ShowCollapse(v, show);
	}

	static bool _IsHidden(FrameworkElement t)
		=> t.Visibility != Visibility.Visible || (t.Parent is Expander e && e.Visibility != Visibility.Visible);

	static string _Get(TextBox t, bool nullIfHidden = true) {
		if (nullIfHidden && _IsHidden(t)) return null;
		var r = t.Text.Trim();
		return r == "" ? null : r;
	}

	static string _Get(ComboBox t, bool nullIfHidden = true, bool nullIfDefault = false) {
		if (nullIfDefault && t.SelectedIndex == 0) return null;
		if (nullIfHidden && _IsHidden(t)) return null;
		return t.IsEditable ? t.Text : t.SelectedItem as string; //note: t.Text changes after t.SelectionChanged event
	}

	static string _Get(KCheckBox t, bool nullIfHidden = true) {
		if (nullIfHidden && _IsHidden(t)) return null;
		return t.IsChecked ? "true" : null;
	}

	static bool _IsChecked(KCheckBox t, bool falseIfHidden = true) {
		if (falseIfHidden && _IsHidden(t)) return false;
		return t.IsChecked;
	}

	string _GetOutputPath(bool getDefault, bool expandEnvVar = false) {
		string r;
		if (!getDefault) getDefault = _meta.outputPath.NE();
		if (getDefault) {
			r = MetaComments.GetDefaultOutputPath(_f, _role, withEnvVar: !expandEnvVar);
		} else {
			r = _meta.outputPath;
			if (expandEnvVar) r = APath.ExpandEnvVar(r);
		}
		return r;
	}

	void _ShowInfo_ListEmpty(Button button, string sFind) {
		var s = "The list is empty";
		if (sFind.Length > 0) s = "The list contains 0 items containing " + sFind;
		_ShowInfoTooltip(button, s);
	}

	void _ShowInfo_Added(Button button, List<string> metaList) {
		_ShowInfoTooltip(button, string.Join("\r\n", metaList) + "\r\n\r\nFinally click OK to save.");
	}

	void _ShowInfoTooltip(Button button, string s) {
		Au.Tools.TUtil.InfoTooltip(ref _tt, button, s, Dock.Right);
	}
	KPopup _tt;

	#endregion

	#region info

	void _InitInfo() {
		info.Text = "This file is a C# <help editor/" + (_isClass ? @"Class files, projects>class file" : "Scripts>script") + @"<>.

C# file properties here are similar to C# project properties in Visual Studio.
Saved in <c green>/*/ meta comments /*/<> at the very start of code, and can be edited there too.
";

		info.AddElem(role,
@"<b>role</b> - purpose of this C# code file. What type of assembly to create and how to execute.
 • <i>miniProgram</i> - execute in a separate host process started from editor.
 • <i>exeProgram</i> - create/execute .exe file. It can run on any computer, without editor installed.
 • <i>editorExtension</i> - execute in the editor's UI thread. Dangerous, unstoppable. Rarely used.
 • <i>classLibrary</i> - create .dll file. It can be used as a reference assembly anywhere.
 • <i>classFile</i> - don't create/execute. Compile together with other C# code files in the project or using meta comment c. Inherits meta comments of the main file of the compilation.

Default role for scripts is miniProgram; cannot be the last two. Default for class files is classFile.
");
		info.AddElem(testScript,
@"<b>testScript</b> - a script to run when you click the Run button.
Can be path relative to this file (examples: Script5.cs, Folder\Script5.cs, ..\Folder\Script5.cs) or path in the workspace (examples: \Script5.cs, \Folder\Script5.cs).

Usually it is used to test this class file or class library. It can contain meta comment <c green>c this file<> that adds this file to the compilation, or <c green>pr this file<> that adds the output dll file as a reference assembly. The recommended way to add this option correctly and easily is to try to run this file and click a link that is then printed in the output.

This option is saved in current workspace, not in meta comments.
");
		info.AddElem(ifRunning,
@"<b>ifRunning</b> - when trying to start this script, what to do if it is already running.
 • <i>warn</i> (default) - write warning in output and don't run.
 • <i>cancel</i> - don't run.
 • <i>wait</i> - run later, when it ends.
 • <i>run</i> - run simultaneously.
 • <i>restart</i> - end it and run.

Suffix _restart means restart if starting the script with the Run button/menu.

This option is ignored when the task runs as .exe program started not from editor.
");
		info.AddElem(runSingle,
@"<b>runSingle</b> - whether tasks can run simultaneously, etc.
 • <i>false</i> (default) - multiple tasks can run simultaneously (see ifRunning).
 • <i>true</i> (checked in Properties) - multiple such tasks cannot run simultaneously. When a task is running, the tray icon is green; also green in the ""Tasks"" pane. Can be ended with the ""End task"" hotkey.

This option is ignored when the task runs as .exe program started not from editor.
");
		info.AddElem(ifRunning2,
@"<b>ifRunning2</b> - when trying to start this ""runSingle"" script, what to do if another ""runSingle"" script is running.
 • <i>same</i> (default) - same as of ifRunning: cancel if cancel[_restart], wait if wait[_restart], else warn.
 • <i>warn</i> - write warning in output and don't run.
 • <i>cancel</i> - don't run.
 • <i>wait</i> - run later, when it ends.

This option is ignored when the task runs as .exe program started not from editor.
");
		info.AddElem(uac,
@"<b>uac</b> - <help articles/UAC>UAC<> integrity level (IL) of the task process.
 • <i>inherit</i> (default) - the same as of the editor process. Normally High IL if installed on admin account, else Medium IL.
 • <i>user</i> - Medium IL, like most applications. The task cannot automate high IL process windows, write some files, change some settings, etc.
 • <i>admin</i> - High IL, aka ""administrator"", ""elevated"". The task has many rights, but cannot automate some apps through COM, etc.

This option is ignored when the task runs as .exe program started not from editor.
");
		info.AddElem(prefer32bit,
@"<b>prefer32bit</b> - whether the task process must be 32-bit everywhere.
 • <i>false</i> (default) - the process is 64-bit or 32-bit, the same as Windows on that computer.
 • <i>true</i> (checked in Properties) - the process is 32-bit on all computers.
");
		info.AddElem(outputPath,
@"<b>outputPath</b> - directory for the output assembly file and related files (used dlls, etc).
Full path. Can start with %environmentVariable% or %AFolders.SomeFolder%. Can be path relative to this file or workspace, like with other options. Default if role exeProgram: <link>%AFolders.Workspace%\bin\filename<>. Default if role classLibrary: <link>%AFolders.ThisApp%\Libraries<>. The compiler creates the folder if does not exist.

If role exeProgram, the exe file is named like the script. The 32-bit version has suffix ""-32"". If optimize true (checked in Properties), creates both 64-bit and 32-bit versions. Else creates only 32-bit if prefer32bit true (checked in Properties) or 32-bit OS, else only 64-bit.
If role classLibrary, the dll file is named like the class file. It can be used by 64-bit and 32-bit processes.
");
		info.AddElem(icon,
@"<b>icon</b> - icon of the output exe file.
The .ico file must be in this workspace. Can be path relative to this file (examples: App.ico, Folder\App.ico, ..\Folder\App.ico) or path in the workspace (examples: \App.ico, \Folder\App.ico).

The icon will be added as a native resource and displayed in File Explorer etc.
");
		info.AddElem(manifest,
@"<b>manifest</b> - <google manifest file site:microsoft.com>manifest<> of the output exe or dll file.
The .manifest file must be in this workspace. Can be path relative to this file (examples: App.manifest, Folder\App.manifest, ..\Folder\App.manifest) or path in the workspace (examples: \App.manifest, \Folder\App.manifest).

The manifest will be added as a native resource.
");
		//		info.AddElem(resFile,
		//@"<b>resFile</b> - .res file containing resources to add to the output exe or dll file as native resources.
		//The .res file must be in this workspace. Can be path relative to this file or path in the workspace.

		//.res files contain compiled native resources of any type, including icons and manifest.
		//This option is rarely used. Instead you use managed resources (button ""Resource..."").
		//");
		info.AddElem(sign,
@"<b>sign</b> - strong-name signing key file, to sign the output assembly.
The file must be in this workspace. Can be path relative to this file (examples: App.snk, Folder\App.snk, ..\Folder\App.snk) or path in the workspace (examples: \App.snk, \Folder\App.snk).
");
		info.AddElem(xmlDoc,
@"<b>xmlDoc</b> - create XML documentation file from /// XML comments of classes, functions, etc.
Creates in the 'outputPath' folder.

XML documentation files are used by code editors to display class/function/parameter info. Also can be used to create online documentation or help file, for example with <google>Sandcastle Help File Builder<> or <google>DocFX<>.
");
		info.AddElem(console,
@"<b>console</b> - let the program run with console.
");
		info.AddElem(optimize,
@"<b>optimize</b> - whether to make the compiled code as fast as possible.
 • <i>false</i> (default) - don't optimize. Define DEBUG and TRACE. Aka ""Debug configuration"".
 • <i>true</i> (checked in Properties) - optimize. Aka ""Release configuration"".

Default is false, because optimization makes difficult to debug. It makes noticeably faster only some types of code, for example processing of text and byte arrays. Before deploying class libraries and exe programs always compile with optimize true.

This option is also applied to class files compiled together, eg as part of project. Use true (checked in Properties) if they contain code that must be as fast as possible.
");
		info.AddElem(define,
@"<b>define</b> - symbols that can be used with #if.
Example: ONE,TWO,d:THREE,r:FOUR
Can be used prefix r: or d: to define the symbol only if optimize true (checked in Properties) or false (unchecked).
If no optimize true, DEBUG and TRACE are added implicitly.
These symbols also are visible in class files compiled together, eg as part of project.
See also <google C# #define>#define<>.
");
		info.AddElem(warningLevel,
@"<b>warningLevel</b> - how many warnings to show.
0 - no warnings.
1 - only severe warnings.
2 - level 1 warnings plus certain, less-severe warnings.
3 - most warnings.
4 - all warnings except warnings added in C# 9+.
5 (default) - all warnings.
6 to 9999 - warnings added in future C# versions.

This option is also applied to class files compiled together, eg as part of project.
");
		info.AddElem(noWarnings,
@"<b>noWarnings</b> - don't show these warnings.
Example: 151,3001,120

This option is also applied to class files compiled together, eg as part of project.
See also <google C# #pragma warning>#pragma warning<>.
");
		info.AddElem(testInternal,
@"<b>testInternal</b> - access internal symbols of these assemblies, like with InternalsVisibleToAttribute.
Example: Assembly1,Assembly2

This option is also applied to class files compiled together, eg as part of project.
");
		info.AddElem(preBuild,
@"<b>preBuild</b> - a script to run before compiling this code file.
Can be path relative to this file (examples: Script5.cs, Folder\Script5.cs, ..\Folder\Script5.cs) or path in the workspace (examples: \Script5.cs, \Folder\Script5.cs).

The script must have role editorExtension. It runs synchronously in compiler's thread. To stop compilation, let it throw an exception.
By default it receives full path of the output exe or dll file in args[0]. If need more info, specify command line arguments, like in this example: Script5.cs /$(outputPath) $(optimize). The script will receive real values in args[0], args[1] and so on. Variables:
 • $(outputFile) - full path of the output exe or dll file.
 • $(outputPath) - meta comment 'outputPath'.
 • $(source) - path of this C# code file in the workspace.
 • $(role) - meta comment 'role'.
 • $(optimize) - meta comment 'optimize'.
 • $(prefer32bit) - meta comment 'prefer32bit'.
");
		info.AddElem(postBuild,
@"<b>postBuild</b> - a script to run after compiling this code file successfully.
Everything else is like with preBuild.
");
		info.AddElem(addAssembly,
@"<b>Assembly<> - add one or more .NET assemblies (.dll files) as references.
Adds meta comment <c green>r FileName<>.

Don't need to add Au.dll and .NET runtime dlls.
To use 'extern alias', edit in the code editor like this: <c green>r Alias=Assembly<>
To remove this meta comment, edit the code.

If the file is in <link>%AFolders.ThisApp%<> or its subfolders, use file name or relative path, else need full path. If role of the script is not miniProgram, at run time the file must be directly in AFolders.ThisApp or AFolders.ThisApp\Libraries. If role is editorExtension, may need to restart editor.
");
		const string c_com = @" COM component's type library to an <i>interop assembly<>.
Adds meta comment <c green>com FileName.dll<>. Saves the assembly file in <link>%AFolders.Workspace%\.interop<>.

An interop assembly is a .NET assembly without real code. Not used at run time. At run time is used the COM component (registered native dll or exe file). Check prefer32bit if 64-bit dll unavailable.

To remove this meta comment, edit the code. Optionally delete unused interop assemblies.
";
		info.AddElem(addComBrowse, @"<b>COM<> - convert a registered" + c_com);
		info.AddElem(addComRegistry, @"<b>...<> - convert a" + c_com);
		info.AddElem(addProject,
@"<b>Project<> - add a reference to a class library created in this workspace.
Adds meta comment <c green>pr File.cs<>. The compiler will compile it if need and use the created dll file as a reference.

The recommended outputPath of the library project is <link>%AFolders.ThisApp%\Libraries<>. Else may not find the dll at run time.

To remove this meta comment, edit the code. Optionally delete unused dll files.
");
		info.AddElem(addClassFile,
@"<b>Class file<> - add a C# code file that contains some classes/functions used by this file.
Adds meta comment <c green>c File.cs<>. The compiler will compile all code files and create single assembly.

If this file is in a project, don't add class files that are in the project folder.
Can be added only files that are in this workspace. Import files if need, for example drag-drop.
Can be path relative to this file (examples: Class5.cs, Folder\Class5.cs, ..\Folder\Class5.cs) or path in the workspace (examples: \Class5.cs, \Folder\Class5.cs).
If folder, adds all its descendant class files.
To remove this meta comment, edit the code.
");
		info.AddElem(addResource,
@"<b>Resource<> - add image etc file(s) as managed resources.
Adds meta comment <c green>resource File<>. If folder, the compiler will add all files in it and subfolders.

Default resource type is stream. In editor you can append space and <c green>/byte[]<>, <c green>/string<>, <c green>/strings<> or <c green>/embedded<>. Example: <c green>resource file.txt /string<>.
/strings - CSV text file containing multiple strings as 2-column CSV (name, value).
/embedded - separate stream. All others are in stream AssemblyName.g.resources.

Can be added only files that are in this workspace. Import files if need, for example drag/drop.
Can be path relative to this file (examples: File.png, Folder\File.png, ..\Folder\File.png) or path in the workspace (examples: \File.png, \Folder\File.png).
To remove this meta comment, edit the code.

To load resources directly, use <help>Au.Util.AResources<>, like <code>var s = Au.Util.AResources.GetString(""file.txt"");</code>. Or <google>ResourceManager<>. To load WPF resources can be used ""pack:..."" URI.
To load embedded resources, use <google>Assembly.GetManifestResourceStream<>.
Compiled names of non-embedded resource files are lowercase, like ""file.png"" or ""subfolder/file.png"".
To browse .NET assembly resources, types, etc can be used for example <google>ILSpy<>.
");

		//		info.AddElem(,
		//");
	}

	#endregion
}
