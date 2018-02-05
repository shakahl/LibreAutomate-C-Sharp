using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;

static class Strips
{
	public static MenuStrip Menubar;
	public static AuToolStrip tbFile, tbEdit, tbRun, tbTools, tbHelp, tbCustom1, tbCustom2; //toolbars
	public static ToolStripDropDownMenu ddFile, ddFileNew, ddEdit, ddOutput, ddStatusBar; //drop-down menus
	public static ToolStripSpringTextBox cHelpFind; //controls on toolbars

	static CmdHandlers _cmd; //all menu/toolbar commands. Contains command handlers, their names and delegates.
	static GStripManager _strips;

	public static void Init()
	{
		//var p = Perf.StartNew();

		//map command handler names/delegates etc
		_cmd = new CmdHandlers();
		//p.Next();

		_strips = new GStripManager(MainForm, _cmd);
		_strips.BuildAll(Folders.ThisApp + @"Default\Strips.xml", Folders.ThisAppDocuments + @"!Settings\Strips.xml", new GDockPanel.DockedToolStripRenderer());
		//p.Next();

		//get top-level toolstrips (menu bar and toolbars)
		Menubar = _strips.MenuBar;
		tbFile = _strips.Toolbars["File"];
		tbEdit = _strips.Toolbars["Edit"];
		tbRun = _strips.Toolbars["Run"];
		tbTools = _strips.Toolbars["Tools"];
		tbHelp = _strips.Toolbars["Help"];
		tbCustom1 = _strips.Toolbars["Custom1"];
		tbCustom2 = _strips.Toolbars["Custom2"];

		//get submenus that will be filled later or used separately etc
		_strips.Submenus["File_Templates"].Opening += _MenuOpening_Templates;
		_strips.Submenus["File_RecentCollections"].Opening += (o, e) => Panels.Files.FillMenuRecentCollections(o as ToolStripDropDownMenu);
		(ddFileNew = _strips.Submenus["File_New"]).Opening += _MenuOpening_New;
		_strips.Submenus["Tools_Panels"].Opening += (se, da) => Panels.PanelManager.AddShowPanelsToMenu(se as ToolStripDropDown, false, true);
		_strips.Submenus["Tools_Toolbars"].Opening += (se, da) => Panels.PanelManager.AddShowPanelsToMenu(se as ToolStripDropDown, true, true);
		ddFile = _strips.Submenus["Menu_File"];
		ddEdit = _strips.Submenus["Menu_Edit"];
		ddOutput = _strips.Submenus["Tools_Output"];
		ddStatusBar = _strips.Submenus["Tools_StatusBar"];

		//get controls
		cHelpFind = tbHelp.Items["Help_Find"] as ToolStripSpringTextBox;

		//p.NW();

		//tbHelp.Padding = new Padding(); //removes 1-pixel right margin that causes a visual artifact because of gradient, but then not good when no margin when the edit is at the very right edge of the form

#if DEBUG
		//all commands have menu items?
		//var p = Perf.StartNew();
		foreach(var k in _cmd.Dict.Keys) {
			//Print(k);
			if(_strips.Xml.Descendant_(k) == null) Output.Warning("no menu item for command " + k);
		}
		//p.NW(); //450
		//for vice versa, GStripManager takes care
#endif

#if TEST
		tbCustom2.Items.Add("Test", null, (unu, sed) =>
		{
			//TestEditor();
			//Panels.Files.Test();
			//Panels.Code.Test();
			//Model.LoadState();
		});
#endif
	}

	/// <summary>
	/// Currently not used.
	/// </summary>
	static void _MenuOpening_New(object sender, CancelEventArgs e)
	{
		var dd = sender as ToolStripDropDownMenu;
		dd.SuspendLayout();

		dd.ResumeLayout();
	}

	/// <summary>
	/// Fills submenu File -> New -> Templates.
	/// </summary>
	static void _MenuOpening_Templates(object sender, CancelEventArgs e)
	{
		var dd = sender as ToolStripDropDownMenu;
		dd.SuspendLayout();
		//dd.Items.Clear();
		//TODO
		dd.ResumeLayout();
	}

	/// <summary>
	/// Checks or unchecks command's menu item and toolbar buttons.
	/// </summary>
	/// <param name="cmd">Command name. See Strips.xml.</param>
	/// <param name="check"></param>
	public static void CheckCmd(string cmd, bool check)
	{
		var a = _strips.Find(cmd);
		int i, n = a.Count;
		if(n == 0) { Debug_.Print("item not found: " + cmd); return; }
		for(i = 0; i < n; i++) {
			switch(a[i]) {
			case ToolStripMenuItem m:
				m.Checked = check;
				break;
			case ToolStripButton b:
				b.Checked = check;
				break;
			}
		}
	}

	/// <summary>
	/// Enables or disables command's menu item and toolbar buttons.
	/// </summary>
	/// <param name="cmd">Command name. See Strips.xml.</param>
	/// <param name="enable"></param>
	public static void EnableCmd(string cmd, bool enable)
	{
		var a = _strips.Find(cmd);
		int i, n = a.Count;
		if(n == 0) { Debug_.Print("item not found: " + cmd); return; }
		for(i = 0; i < n; i++) {
			a[i].Enabled = enable;
		}
	}
}
