
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Runtime;
using System.Windows.Interop;

partial class MainWindow : Window
{
	public MainWindow() {
		//_StartProfileOptimization();

		App.Wmain = this;
		Title = App.AppName; //will not append document name etc

		AWnd.More.SavedRect.Restore(this, App.Settings.wndPos, o => App.Settings.wndPos = o);

		System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false); //FUTURE: remove when forms not used

		Panels.LoadAndCreateToolbars();

		App.Commands = new AuMenuCommands(typeof(Menus), Panels.Menu);

		var atb = new ToolBar[7] { Panels.THelp, Panels.TTools, Panels.TFile, Panels.TRun, Panels.TEdit, Panels.TCustom1, Panels.TCustom2 };
		App.Commands.InitToolbarsAndCustomize(AFolders.ThisAppBS + @"Default\Commands.xml", AppSettings.DirBS + "Commands.xml", atb);

		Panels.CreatePanels();

		App.Commands.BindKeysTarget(this, "");

		var mi1 = App.Commands[nameof(Menus.New)].MenuItem;
		mi1.SubmenuOpened += (_, e) => FilesModel.FillMenuNew(mi1);
		var mi2 = App.Commands[nameof(Menus.File.Workspace.Recent_workspaces)].MenuItem;
		mi2.Items.Add(new Separator());
		mi2.SubmenuOpened += (_, e) => FilesModel.FillMenuRecentWorkspaces(mi2);

		Panels.PanelManager.Container = g => { this.Content = g; };
	}

	protected override void OnClosing(CancelEventArgs e) {
		base.OnClosing(e);
		if (e.Cancel) return;

		App.Model.Save.AllNowIfNeed();
		Panels.PanelManager.Save();

		if (App.Settings.runHidden) {
			e.Cancel = true;
			Hide();
			AProcess.MinimizePhysicalMemory_(1000);
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
		UacDragDrop.AdminProcess.Enable(true); //TODO: disable when hiding, like in old version. And CodeInfo too.

		hs.AddHook(_WndProc);
	}

	///// <summary>
	///// When window handle created.
	///// Documents are open, etc.
	///// </summary>
	//public event EventHandler Load;

	unsafe nint _WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled) {
		var w = (AWnd)hwnd;

		return default;
	}

	protected override void OnActivated(EventArgs e) {
		//var w = this.Hwnd(); if (AWnd.Active != w) w.ActivateLL(); //activates window, but this is a bad place for it, eg does not set focus correctly
		var w = this.Hwnd(); if (AWnd.Active != w) Dispatcher.InvokeAsync(() => w.ActivateLL());
		base.OnActivated(e);
	}

	protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi) {
		base.OnDpiChanged(oldDpi, newDpi);
		//workaround for WPF bug: on DPI change activates window
		var w = this.Hwnd();
		if (AWnd.Active != w) {
			bool wasVisible = w.IsVisible; //allow to activate when opening window in non-primary screen
			var h = AHookWin.ThreadCbt(k => k.code == HookData.CbtEvent.ACTIVATE && (AWnd)k.wParam == w && (wasVisible || !w.IsVisible));
			Dispatcher.InvokeAsync(() => h.Dispose());
		}
	}

	void _OpenDocuments() { //TODO
		var docLeaf = _AddDoc("Document 1");
		_AddDoc("Document 2");
		_AddDoc("Document 3");
		_AddDoc("Document 4");
		docLeaf.Visible = true;
		//Panels.DocPlaceholder_.Visible = false; //TODO
		docLeaf.Content.Focus();

		AuPanels.ILeaf _AddDoc(string name) {
			//var docPlaceholder = App.Panels["Open"]; //in stack
			var docPlaceholder = Panels.DocPlaceholder_; //in tab
			var v = docPlaceholder.AddSibling(false, AuPanels.LeafType.Document, name, true);
			v.Closing += (_, e) => { e.Cancel = !ADialog.ShowOkCancel("Close?"); };
			v.ContextMenuOpening += (o, m) => {
				var k = o as AuPanels.ILeaf;
				m.Separator();
				m["Close 2"] = o => k.Delete();
			};
			v.TabSelected += (_, _) => _OpenDoc(v);

			return v;
		}

		static void _OpenDoc(AuPanels.ILeaf leaf) {
			if (leaf.Content != null) return;
			leaf.Content = new SciHost();
		}
	}

	static void _StartProfileOptimization() {
#if !DEBUG
		var fProfile = AFolders.ThisAppDataLocal + "ProfileOptimization";
		AFile.CreateDirectory(fProfile);
		ProfileOptimization.SetProfileRoot(fProfile);
		ProfileOptimization.StartProfile("Aedit.startup");
#endif
	}

	public void ZShowAndActivate() {
		Show();
		var w = this.Hwnd();
		w.ShowNotMinimized(true);
		w.ActivateLL();
	}
}

public static class Ext1
{

}