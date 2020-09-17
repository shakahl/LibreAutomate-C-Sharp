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

partial class MainWindow : Window
{
	AuPanels _panels = new();
	Menu _menu;

	public MainWindow() {
		Title = "Aedit";

		_panels.BorderBrush = SystemColors.ActiveBorderBrush;
		//_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", null);
		_panels.Load(AFolders.ThisAppBS + @"Default\Layout.xml", AppSettings.DirBS + "Layout.xml");

		_panels["Files"].Content = new TreeView { BorderThickness = default };
		_panels["Output"].Content = new SciHost() { Focusable = false };
		_panels["Info"].Content = new TextBlock();
		_panels["Found"].Content = new SciHost() { Focusable = false };
		_panels["Running"].Content = new ListBox { BorderThickness = default };
		_panels["Find"].Content = new PanelFind();

		_panels["Menu"].Content = _menu = new Menu();
		App.Commands = new AuMenuCommands(typeof(Menus), _menu, this);

		var atb = App.Toolbars = new ToolBar[7];
		for (int i = 0; i < atb.Length; i++) {
			string name = i switch { 0 => "Custom1", 1 => "Custom2", 2 => "Tools", 3 => "Help", 4 => "File", 5 => "Run", _ => "Edit" };
			var tt = new ToolBarTray { IsLocked = true }; //because ToolBar looks bad if parent is not ToolBarTray
			var c = new ToolBar { Name = name };
			tt.ToolBars.Add(atb[i] = c);

			if (name == "Help") c.Items.Add(new TextBox { Width = 150, Padding = new Thickness(1, 0, 1, 0) });
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
	}

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
}

public static class Ext1
{

}