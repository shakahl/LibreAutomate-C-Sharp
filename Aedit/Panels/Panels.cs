using Au;
using Au.Types;
using Au.Util;
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
using System.Windows.Controls;
using System.Windows;
//using System.Linq;

using System.Windows.Forms.Integration;//TODO

static class Panels
{
	public static AuPanels PanelManager;
	internal static AuPanels.ILeaf DocPlaceholder_;
	//panels
	public static PanelEdit Editor;
	public static PanelFiles Files;
	public static PanelOpen Open;
	public static PanelRunning Running;
	public static PanelOutput Output;
	public static PanelFind Find;
	public static PanelFound Found;
	public static PanelInfo Info;
	//menu and toolbars
	public static Menu Menu;
	//public static ToolBar[] Toolbars;
	public static ToolBar TFile, TEdit, TRun, TTools, THelp, TCustom1, TCustom2;

	public static void LoadAndCreateToolbars() {
		var pm = PanelManager = new AuPanels();

		pm.BorderBrush = SystemColors.ActiveBorderBrush;
		//pm.Load(AFolders.ThisAppBS + @"Default\Layout.xml", null);
		pm.Load(AFolders.ThisAppBS + @"Default\Layout.xml", AppSettings.DirBS + "Layout.xml");

		pm["Menu"].Content = Menu = new Menu();
		TFile = _TB("File");
		TEdit = _TB("Edit");
		TRun = _TB("Run");
		TTools = _TB("Tools");
		THelp = _TB("Help", true);
		TCustom1 = _TB("Custom1");
		TCustom2 = _TB("Custom2");

		ToolBar _TB(string name, bool isHelp = false) {
			var c = new ToolBar { Name = name };
			var tt = new ToolBarTray { IsLocked = true }; //because ToolBar looks bad if parent is not ToolBarTray
			tt.ToolBars.Add(c);
#if true
			if (isHelp) {
				var p = new DockPanel { Background = tt.Background };
				DockPanel.SetDock(tt, Dock.Right);
				p.Children.Add(tt);
				var box = new TextBox { Height = 20, Margin = new Thickness(3, 1, 3, 2), Padding = new Thickness(1, 1, 1, 0) };
				p.Children.Add(box);
				pm[name].Content = p;
			} else {
				pm[name].Content = tt;
			}
#else
			if (name == "Help") c.Items.Add(new TextBox { Width = 150, Padding = new Thickness(1, 0, 1, 0) });
			pm[name].Content = tt;
#endif
			return c;
		}
	}

	public static void CreatePanels() {
		var pm = PanelManager;

		pm["Files"].Content = Files = new PanelFiles();
		pm["Running"].Content = Running = new PanelRunning();

		pm["Output"].Content = Output = new PanelOutput();
		pm["Info"].Content = new WindowsFormsHost { Child = Info = new PanelInfo() };
		pm["Found"].Content = Found = new PanelFound();
		pm["Find"].Content = Find = new PanelFind();

		pm["Open"].Content = new WindowsFormsHost { Child = Open = new PanelOpen() };
		Open = new PanelOpen();

		pm["documents"].Content = Editor = new PanelEdit();
		DocPlaceholder_ = pm["documents"];
	}
}
