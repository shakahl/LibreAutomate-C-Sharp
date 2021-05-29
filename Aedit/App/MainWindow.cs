
using Au;
using Au.Types;
using Au.Controls;
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
//using System.Linq;
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

		AWnd.More.SavedRect.Restore(this, App.Settings.wndPos, o => App.Settings.wndPos = o);
		//SHOULDDO: now on Win8 first time very small if high DPI and small screen. Don't use default window size. Or test with small screen on all OS.

		Panels.LoadAndCreateToolbars();

		App.Commands = new KMenuCommands(typeof(Menus), Panels.Menu);

		App.Commands[nameof(Menus.File.New)].SubmenuOpened = (o, _) => FilesModel.FillMenuNew(o as MenuItem);
		App.Commands[nameof(Menus.File.Workspace.Recent_workspaces)].SubmenuOpened = (o, _) => FilesModel.FillMenuRecentWorkspaces(o as MenuItem);

		var atb = new ToolBar[7] { Panels.THelp, Panels.TTools, Panels.TFile, Panels.TRun, Panels.TEdit, Panels.TCustom1, Panels.TCustom2 };
		App.Commands.InitToolbarsAndCustomize(AFolders.ThisAppBS + @"Default\Commands.xml", AppSettings.DirBS + "Commands.xml", atb);

		Panels.CreatePanels();

		App.Commands.BindKeysTarget(this, "");

		Panels.PanelManager.Container = g => { this.Content = g; };


		//ATimer.After(100, _ => DOptions.ZShow());
		//ATimer.After(100, _ => App.Model.Properties());
		//ATimer.After(100, _ => Menus.File.Workspace.New_workspace());

//		ATimer.After(100, _ => {
//#if !true
//					//var w = +AWnd.Find("Quick Macros -*");
//					//w = +w.ChildById(2212);
//					var w = +AWnd.Find("Character Map");
//					w = +w.ChildById(103);
//					//AOutput.Write(w);
//					//new Au.Tools.DAWnd(w).Show();
//					//new Au.Tools.DAWnd(w, uncheckControl: true).Show();
//					new Au.Tools.DAAcc(AAcc.FromWindow(w, AccOBJID.CLIENT)).Show();
//#elif true
//			new Au.Tools.DAWinImage().Show();
//#else
//					var w = +AWnd.Find("Untitled Document - Google Chrome", "Chrome_WidgetWin_1");
//					var a = +AAcc.Find(w, "web:BUTTON", "PayPal - The safer, easier way to pay online!");
//					new Au.Tools.DAAcc(a).Show();
//#endif
//		});

#if DEBUG
		App.Timer1s += () => {
			var e = Keyboard.FocusedElement as FrameworkElement;
			ADebug_.PrintIf(e != null && !e.IsVisible, "focused invisible");
			//AOutput.Write(e, FocusManager.GetFocusedElement(App.Wmain));
		};
#endif
	}

	protected override void OnClosing(CancelEventArgs e) {
		base.OnClosing(e);
		if (e.Cancel) return;

		App.Model.Save.AllNowIfNeed();
		Panels.PanelManager.Save();

		if (App.Settings.runHidden && IsVisible) {
			e.Cancel = true;
			Hide();
			AThisProcess.MinimizePhysicalMemory_(1000);
		}
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
		App.Hwnd = (AWnd)hs.Handle;

		Panels.PanelManager["Output"].Visible = true;

		App.Model.WorkspaceLoadedWithUI(onUiLoaded: true);

		App.Loaded = EProgramState.LoadedUI;
		//Load?.Invoke(this, EventArgs.Empty);

		CodeInfo.UiLoaded();
		UacDragDrop.AdminProcess.Enable(true); //rejected: disable when hiding main window. Some other window may be visible.

		hs.AddHook(_WndProc);

		Au.Tools.QuickCapture.RegisterHotkey(App.Hwnd);
	}

	///// <summary>
	///// When window handle created.
	///// Documents are open, etc.
	///// </summary>
	//public event EventHandler Load;

	unsafe nint _WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
		var w = (AWnd)hwnd;

		switch (msg) {
		case Api.WM_DPICHANGED:
			this.DpiChangedWorkaround();
			break;
		case Api.WM_HOTKEY:
			handled = true;
			switch ((ERegisteredHotkeyId)(int)wParam) {
			case ERegisteredHotkeyId.QuickCapture: Au.Tools.QuickCapture.WmHotkey(); break;
			}
			
			break;
		}

		return default;
	}

	protected override void OnActivated(EventArgs e) {
		//var w = this.Hwnd(); if (AWnd.Active != w) w.ActivateL(); //activates window, but this is a bad place for it, eg does not set focus correctly
		var w = this.Hwnd(); if (AWnd.Active != w) Dispatcher.InvokeAsync(() => w.ActivateL());
		base.OnActivated(e);
	}

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
	//		v.Closing += (_, e) => { e.Cancel = !ADialog.ShowOkCancel("Close?"); };
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
		var fProfile = AFolders.ThisAppDataLocal + "ProfileOptimization";
		AFile.CreateDirectory(fProfile);
		System.Runtime.ProfileOptimization.SetProfileRoot(fProfile);
		System.Runtime.ProfileOptimization.StartProfile("Aedit.startup");
#endif
	}

	public void ZShowAndActivate() {
		Show();
		var w = this.Hwnd();
		w.ShowNotMinimized(true);
		w.ActivateL();
	}
}

public static class Ext1
{

}