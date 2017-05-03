using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Xml.Linq;

using G.Controls;
using ScintillaNET;

using Catkeys;
using static Catkeys.NoClass;
using static Program;

partial class EForm :Form
{
	GDockPanel _dock;

	public class EPanels //not struct because of warning CS1690
	{
		public PanelFiles Files;
		public PanelOpen Open;
		public PanelRunning Running;
		public PanelRecent Recent;
		public PanelOutput Output;
		public PanelFind Find;
		public PanelStatus Status;
	}
	public EPanels Panels;

	public FilesModel Model;
	public Edit Code;

	public EForm()
	{
		//#if DEBUG
		//		SetHookToMonitorCreatedWindowsOfThisThread();
		//#endif
		MainForm = this;
		Panels = new EPanels();

		this.SuspendLayout();
		this.Font = Stock.FontNormal;
		this.AutoScaleMode = AutoScaleMode.None;
		this.Text = "Catkeys";
		//this.StartPosition = FormStartPosition.CenterScreen;
		this.StartPosition = FormStartPosition.Manual;
		this.Location = new Point(200, 100);
		this.ClientSize = new Size(900, 600);

		Panels.Files = new PanelFiles();
		Panels.Open = new PanelOpen();
		Panels.Running = new PanelRunning();
		Panels.Recent = new PanelRecent();
		Panels.Output = new PanelOutput();
		Panels.Find = new PanelFind();
		Panels.Status = new PanelStatus();
		Perf.Next();
		_Strips_Init();
		//Strips.Help.Padding = new Padding(); //removes 1-pixel right margin that causes a visual artifact because of gradient, but then not good when no margin when the edit is at the very right edge of the form

#if TEST
		Strips.tbCustom2.Items.Add("Test", null, (unu, sed) =>
		{
			//TestEditor();
			//Panels.Files.Test();
			Code.Test();
		});
#endif

		Code = new Edit(); //must be after creating strips
		Perf.Next();

		var c = new RichTextBox();
		//var c = new ScrollableControl_();
		c.Name = "Results";
		//c.SetScrollInfo(true, 100, 30, false);
		//c.SetScrollInfo(false, 100, 30, false);
		//c.SetScrollPos(false, 10, false);

		_dock = new GDockPanel();
		_dock.Create(Folders.ThisApp + @"Default\Panels.xml", Folders.ThisAppDocuments + @"!Settings\Panels.xml",
			Code.Sci, Panels.Files, Panels.Output, Panels.Find, Panels.Open, Panels.Running, Panels.Recent, c,
			Strips.Menubar, Strips.tbFile, Strips.tbEdit, Strips.tbRun, Strips.tbTools, Strips.tbHelp, Strips.tbCustom1, Strips.tbCustom2
			);
		//info: would be easier to specify these in the default XML, but then cannot change in new app versions.
		_dock.GetPanel(Panels.Open).Init("Currently open files", EResources.GetImageUseCache("open"));
		_dock.GetPanel(Panels.Output).Init("Errors and other information", EResources.GetImageUseCache("output"));
		_dock.GetPanel(Panels.Find).Init("Find files, text, triggers", EResources.GetImageUseCache("find"));
		_dock.GetPanel(Panels.Files).Init("All scripts and other files of current collection");
		_dock.GetPanel(Panels.Running).Init("Running scripts");
		_dock.GetPanel(Panels.Recent).Init("Recent running scripts");
		_dock.GetPanel(c).Init("New panel", EResources.GetImageUseCache("paste"));

		this.Controls.Add(_dock);
		this.Controls.Add(Panels.Status);

		_DisableTabOrderOfControls(this);

		this.ResumeLayout(false);
		Perf.Next();
		Panels.Files.LoadCollection(CommandLine.CollectionFile);
		Perf.Next();

		//#if DEBUG
		//		DebugPrint("Ending form ctor. Please make sure there are no parked controls created before now. Use the CBT hook for it.");
		//#endif

		//Perf.Next();
		Time.SetTimer(1, true, t =>
		{
			//Perf.NW();
			Perf.Next();
			//TaskDialog.Show("", Perf.Times);
			if(_dock != null) Panels.Output.Write(Perf.Times); else Print(Perf.Times);
			//TaskDialog.Show(Perf.Times, IsWinEventHookInstalled(EVENT_OBJECT_CREATE).ToString()); //IsWinEventHookInstalled always true (false positive, as documented)
			//GC.Collect();

			CommandLine.OnAfterCreatedFormAndOpenedCollection();

			//Close();
		});
	}

	protected override void WndProc(ref Message m)
	{
		Wnd w = (Wnd)this; uint msg = (uint)m.Msg; LPARAM wParam = m.WParam, lParam = m.LParam;
		//var s = m.ToString();

		//switch(msg) {
		//case Api.WM_CREATE:
		//	break;
		//}

		base.WndProc(ref m);

		switch(msg) {
		case Api.WM_CREATE:
			//Print(w.AllChildren().Length); //0
			if(Settings.Get("wndPos", out string wndPos))
				try { w.RestorePositionSizeState(wndPos, true); } catch { }
			break;
		case Api.WM_DESTROY:
			Settings.Set("wndPos", w.SavePositionSizeState());
			break;
		case Api.WM_ENABLE:
			//.NET ignores this. Eg if an owned form etc disables this window, the Enabled property is not changed and no EnabledChanged event.
			//PrintList(wParam, Enabled);
			//Enabled = wParam != 0; //not good
			_dock.EnableDisableAllFloatingWindows(wParam != 0);
			break;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys k)
	{
		//Print(k);

		return base.ProcessCmdKey(ref msg, k);
	}

	static void _DisableTabOrderOfControls(Control c)
	{
		foreach(Control cc in c.Controls) {
			cc.TabStop = false;
			_DisableTabOrderOfControls(cc);
		}
	}

}
