
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
	AuPanels _panels = new();
	Menu _menu;

	public MainWindow() {
		//_StartProfileOptimization();
		
		Title = "Aedit";

		App.Panels = _panels;
		_panels.BorderBrush = SystemColors.ActiveBorderBrush;
		//_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", null);
		_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", AppSettings.DirBS + "Layout.xml");

		_panels["Files"].Content = new Script().Create();
		_panels["Running"].Content = new Script().Create();

		_panels["Output"].Content = new SciHost() { Focusable = false };
		_panels["Info"].Content = new TextBlock();
		_panels["Found"].Content = new SciHost() { Focusable = false };
		_panels["Find"].Content = new PanelFind();

		_panels["Menu"].Content = _menu = new Menu();
		App.Commands = new AuMenuCommands(typeof(Menus), _menu, this);

		var atb = App.Toolbars = new ToolBar[7];
		for (int i = 0; i < atb.Length; i++) {
			string name = i switch { 0 => "Custom1", 1 => "Custom2", 2 => "Help", 3 => "Tools", 4 => "File", 5 => "Run", _ => "Edit" };
			var c = new ToolBar { Name = name };
			atb[i] = c;
			var tt = new ToolBarTray { IsLocked = true }; //because ToolBar looks bad if parent is not ToolBarTray
			tt.ToolBars.Add(c);
#if true
			if (name == "Help") {
				var p = new DockPanel { Background = tt.Background };
				DockPanel.SetDock(tt, Dock.Right);
				p.Children.Add(tt);
				var box = new TextBox { Height = 20, Margin = new Thickness(3, 1, 3, 2), Padding = new Thickness(1, 1, 1, 0) };
				p.Children.Add(box);
				_panels[name].Content = p;
			} else {
				_panels[name].Content = tt;
			}
#else
			if (name == "Help") c.Items.Add(new TextBox { Width = 150, Padding = new Thickness(1, 0, 1, 0) });
			_panels[name].Content = tt;
#endif
		}

		App.Commands.InitToolbarsAndCustomize(AFolders.ThisAppBS + @"Default\Commands.xml", AppSettings.DirBS + "Commands.xml", atb);

		_panels.Container = g => {
			this.Content = g;
		};

		ATimer.After(1, _ => _OpenDocuments());

		AWnd.More.SavedRect.Restore(this, App.Settings.wndPos, o => App.Settings.wndPos = o);
	}

	protected override void OnClosing(CancelEventArgs e) {
		base.OnClosing(e);
		_panels.Save();
		if (App.Settings.runHidden) {
			e.Cancel = true;
			Hide();
			AProcess.MinimizePhysicalMemory_(1000);
		}
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

	//protected override void OnSourceInitialized(EventArgs e) {
	//	var hs = PresentationSource.FromVisual(this) as HwndSource;
	//	hs.AddHook(_WndProc);

	//	base.OnSourceInitialized(e);
	//}

	//unsafe IntPtr _WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
	//	var w = (AWnd)hwnd;

	//	return default;
	//}

	void _OpenDocuments() {
		var docLeaf = _AddDoc("Document 1");
		_AddDoc("Document 2");
		_AddDoc("Document 3");
		_AddDoc("Document 4");
		docLeaf.Visible = true;
		_panels["documents"].Visible = false;
		docLeaf.Content.Focus();

		AuPanels.ILeaf _AddDoc(string name) {
			//var docPlaceholder = _panels["Open"]; //in stack
			var docPlaceholder = _panels["documents"]; //in tab
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
}

public static class Ext1
{

}