//#define TV_OLV

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

partial class EForm :Form
{
	GDockPanel _dock;

	public struct EPanels
	{
#if TV_OLV
		public PanelFilesOLV Files;
#else
		public PanelFiles Files;
#endif
		public PanelOpen Open;
		public PanelRunning Running;
		public PanelRecent Recent;
		public PanelOutput Output;
		public PanelFind Find;
		public PanelStatus Status;
	}
	public EPanels Panels;

	public Scintilla Code;

	public static EForm MainForm;

	public EForm()
	{
		//#if DEBUG
		//		SetHookToMonitorCreatedWindowsOfThisThread();
		//#endif
		MainForm = this;

		this.SuspendLayout();
		this.ClientSize = new Size(900, 600);
		this.Font = SystemFonts.MessageBoxFont;
		this.AutoScaleMode = AutoScaleMode.None;
		this.Text = "Catkeys";
		//this.StartPosition = FormStartPosition.CenterScreen;
		this.StartPosition = FormStartPosition.Manual;
		this.Location = new Point(200, 100);

		//code
		Code = new Scintilla();
		Code.Name = Code.AccessibleName = "Code";
		Code.BorderStyle = BorderStyle.None;

#if TV_OLV
		Panels.Files = new PanelFilesOLV();
#else
		Panels.Files = new PanelFiles();
#endif
		Panels.Open = new PanelOpen();
		Panels.Running = new PanelRunning();
		Panels.Recent = new PanelRecent();
		Panels.Output = new PanelOutput();
		Panels.Find = new PanelFind();
		Panels.Status = new PanelStatus();
		Perf.Next();
		_Strips_Init();
		//Strips.Help.Padding = new Padding(); //removes 1-pixel right margin that causes a visual artefact because of gradient, but then not good when no margin when the edit is at the very right edge of the form

#if TEST
		Strips.Custom2.Items.Add("Test", null, (unu, sed) =>
		{
			//TestEditor();
			Panels.Files.Test();
		});
#endif

		Perf.Next();

		var c = new RichTextBox();
		//var c = new ScrollableControl_();
		c.Name = "Results";
		//c.SetScrollInfo(true, 100, 30, false);
		//c.SetScrollInfo(false, 100, 30, false);
		//c.SetScrollPos(false, 10, false);

		_dock = new GDockPanel();
		_dock.Create(Folders.ThisApp + @"Default\Panels.xml", Folders.ThisAppDocuments + @"Settings\Panels.xml",
			Code, Panels.Files, Panels.Output, Panels.Find, Panels.Open, Panels.Running, Panels.Recent, c,
			Strips.Menubar, Strips.File, Strips.Edit, Strips.Run, Strips.Tools, Strips.Help, Strips.Custom1, Strips.Custom2
			);
		//info: would be easier to specify these in the default XML, but then cannot change in new app versions.
		_dock.GetPanel(Panels.Open).Init("Currently open files", EResources.GetImage("open"));
		_dock.GetPanel(Panels.Output).Init("Errors and other information", EResources.GetImage("output"));
		_dock.GetPanel(Panels.Find).Init("Find files, text, triggers", EResources.GetImage("find"));
		_dock.GetPanel(Panels.Files).Init("All scripts and other files of current collection");
		_dock.GetPanel(Panels.Running).Init("Running scripts");
		_dock.GetPanel(Panels.Recent).Init("Recent running scripts");
		_dock.GetPanel(c).Init("New panel", EResources.GetImage("paste"));

		this.Controls.Add(_dock);
		this.Controls.Add(Panels.Status);

		_DisableTabOrderOfControls(this);

		this.ResumeLayout(false);
		Perf.Next();
		Panels.Files.LoadCollection(Folders.ThisApp + @"test\ok\Main.xml");
		//Panels.Files.LoadCollection(@"E:\test\ok\Main.xml");
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

			//Close();
		});
	}

	protected override void WndProc(ref Message m)
	{
		uint msg = (uint)m.Msg; LPARAM wParam = m.WParam, lParam = m.LParam;
		//var s = m.ToString();

		base.WndProc(ref m);

		switch(msg) {
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
