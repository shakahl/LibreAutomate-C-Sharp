using Au.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;

partial class MainWindow : Window
{
	public MainWindow() {
		//_StartProfileOptimization();

		App.Wmain = this;
		Title = App.AppName; //don't append document name etc

		WndSavedRect.Restore(this, App.Settings.wndPos, o => App.Settings.wndPos = o);
		//SHOULDDO: now on Win8 first time very small if high DPI and small screen. Don't use default window size. Or test with small screen on all OS.

		Panels.LoadAndCreateToolbars();

		App.Commands = new KMenuCommands(typeof(Menus), Panels.Menu);

		App.Commands[nameof(Menus.File.New)].SubmenuOpened = (o, _) => FilesModel.FillMenuNew(o as MenuItem);
		App.Commands[nameof(Menus.File.Workspace.Recent_workspaces)].SubmenuOpened = (o, _) => FilesModel.FillMenuRecentWorkspaces(o as MenuItem);

		var atb = new ToolBar[7] { Panels.THelp, Panels.TTools, Panels.TFile, Panels.TRun, Panels.TEdit, Panels.TCustom1, Panels.TCustom2 };
		App.Commands.InitToolbarsAndCustomize(folders.ThisAppBS + @"Default\Commands.xml", AppSettings.DirBS + "Commands.xml", atb);

		var bRun = App.Commands[nameof(Menus.Run.Run_script)].FindButtonInToolbar(Panels.TRun);
		if (bRun != null) { bRun.Width = 40; bRun.Margin = new(10, 0, 10, 0); } //make Run button bigger //SHOULDDO: bad if vertical toolbar

		Panels.CreatePanels();

		App.Commands.BindKeysTarget(this, "");

		Panels.PanelManager.Container = g => { this.Content = g; };


		//timerm.after(100, _ => DOptions.ZShow());
		//timerm.after(100, _ => App.Model.Properties());
		//timerm.after(100, _ => Menus.File.Workspace.New_workspace());
		//timerm.after(100, _ => DIcons.ZShow());
		//timerm.after(600, _ => Au.Tools.Dwnd.Dialog(wnd.find(null, "Shell_TrayWnd")));
		//timerm.after(500, _ => Au.Tools.Delm.Dialog(new POINT(806, 1580)));
		//timerm.after(400, _ => Au.Tools.Duiimage.Dialog());

#if DEBUG
		App.Timer1s += () => {
			var e = Keyboard.FocusedElement as FrameworkElement;
			Debug_.PrintIf(e != null && !e.IsVisible, "focused invisible");
			//print.it(e, FocusManager.GetFocusedElement(App.Wmain));
		};
#endif
	}

	protected override void OnClosing(CancelEventArgs e) {
		if (!e.Cancel) {
			App.Model.Save.AllNowIfNeed();
			Panels.PanelManager.Save();

			if (App.Settings.runHidden && IsVisible) {
				e.Cancel = true;
				Hide();
				process.ThisProcessMinimizePhysicalMemory_(1000);
			}
		}
		base.OnClosing(e); //note: must be at the end, after we set Cancel
	}

	protected override void OnClosed(EventArgs e) {
		App.Loaded = EProgramState.Unloading;
		base.OnClosed(e);
		UacDragDrop.AdminProcess.Enable(false);
		CodeInfo.Stop();
		FilesModel.UnloadOnWindowClosed();
		App.Loaded = EProgramState.Unloaded;
	}

	protected override void OnSourceInitialized(EventArgs e) {
		base.OnSourceInitialized(e);
		var hs = PresentationSource.FromVisual(this) as HwndSource;
		App.Hwnd = (wnd)hs.Handle;

		//workaround for: sometimes OS does not set foreground window. Then we have a fake active/focused state (blinking caret, called OnActivated, etc).
		//	1. When started hidden, and now clicked tray icon first time. Is it because of the "lock foreground window"? Or WPF shows window somehow incorrectly, as usual?
		//	2. When starting visible, if VMWare Player is active. Same with some other programs too (WPF, appstore, some other).
		//this.Activate(); //does not work with VMWare, also if user clicks a window after starting this process
		App.Hwnd.ActivateL(); //works always, possibly with workarounds

		Panels.PanelManager["Output"].Visible = true;

		App.Model.WorkspaceLoadedWithUI(onUiLoaded: true);

		App.Loaded = EProgramState.LoadedUI;
		//Load?.Invoke(this, EventArgs.Empty);

		CodeInfo.UiLoaded();
		UacDragDrop.AdminProcess.Enable(true); //rejected: disable when hiding main window. Some other window may be visible.

		hs.AddHook(_WndProc);

		Au.Tools.QuickCapture.RegisterHotkeys();

		CommandLine.UILoaded();
	}

	///// <summary>
	///// When window handle created.
	///// Documents are open, etc.
	///// </summary>
	//public event EventHandler Load;

	unsafe nint _WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
		var w = (wnd)hwnd;

		switch (msg) {
		case Api.WM_DPICHANGED:
			this.DpiChangedWorkaround();
			break;
		case Api.WM_HOTKEY:
			handled = true;
			switch ((ERegisteredHotkeyId)(int)wParam) {
			case ERegisteredHotkeyId.QuickCaptureMenu: Au.Tools.QuickCapture.Menu(); break;
			case ERegisteredHotkeyId.QuickCaptureDwnd: Au.Tools.QuickCapture.ToolDwnd(); break;
			case ERegisteredHotkeyId.QuickCaptureDelm: Au.Tools.QuickCapture.ToolDelm(); break;
			}
			break;
		}

		return default;
	}

	//this could be a workaround for the inactive window at startup, but probably don't need when we call Activete() in OnSourceInitialized
	//protected override void OnActivated(EventArgs e) {
	//	var w = this.Hwnd();
	//	if (wnd.active != w && _activationWorkaroundTime < Environment.TickCount64 - 5000) {
	//		//print.it(new StackTrace());
	//		_activationWorkaroundTime = Environment.TickCount64;
	//		timerm.after(10, _ => {
	//			Debug_.Print("OnActivated workaround, " + wnd.active);
	//			//w.ActivateL(); //in some cases does not work, or need key etc
	//			if (!w.IsMinimized) {
	//				w.ShowMinimized(noAnimation: true);
	//				w.ShowNotMinimized(noAnimation: true);
	//			}
	//		});
	//	}

	//	base.OnActivated(e);
	//}
	//long _activationWorkaroundTime;

	//this was for testing document tabs. Now we don't use document tabs. All documents now are in single panel.
	//void _OpenDocuments() {
	//	var docLeaf = _AddDoc("Document 1");
	//	_AddDoc("Document 2");
	//	_AddDoc("Document 3");
	//	_AddDoc("Document 4");
	//	docLeaf.Visible = true;
	//	//Panels.DocPlaceholder_.Visible = false;
	//	docLeaf.Content.Focus();

	//	KPanels.ILeaf _AddDoc(string name) {
	//		//var docPlaceholder = App.Panels["Open"]; //in stack
	//		var docPlaceholder = Panels.DocPlaceholder_; //in tab
	//		var v = docPlaceholder.AddSibling(false, KPanels.LeafType.Document, name, true);
	//		v.Closing += (_, e) => { e.Cancel = !dialog.showOkCancel("Close?"); };
	//		v.ContextMenuOpening += (o, m) => {
	//			var k = o as KPanels.ILeaf;
	//			m.Separator();
	//			m["Close 2"] = o => k.Delete();
	//		};
	//		v.TabSelected += (_, _) => _OpenDoc(v);

	//		return v;
	//	}

	//	static void _OpenDoc(KPanels.ILeaf leaf) {
	//		if (leaf.Content != null) return;
	//		leaf.Content = new KScintilla();
	//	}
	//}

	static void _StartProfileOptimization() {
#if !DEBUG
		var fProfile = folders.ThisAppDataLocal + "ProfileOptimization";
		filesystem.createDirectory(fProfile);
		System.Runtime.ProfileOptimization.SetProfileRoot(fProfile);
		System.Runtime.ProfileOptimization.StartProfile("Au.Editor.startup");
#endif
	}

	public void ZShowAndActivate() {
		Show();
		var w = this.Hwnd();
		w.ShowNotMinimized(true);
		w.ActivateL();
	}
}
