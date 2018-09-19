using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
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

using Au;
using Au.Types;
using static Au.NoClass;
using static Program;
using Au.Controls;

partial class EForm :Form
{
	public EForm()
	{
		//#if DEBUG
		//		SetHookToMonitorCreatedWindowsOfThisThread();
		//#endif

		Program.MainForm = this;

		this.SuspendLayout();
		this.Font = Stock.FontNormal;
		this.AutoScaleMode = AutoScaleMode.None;
		this.StartPosition = FormStartPosition.Manual;
		this.Location = new Point(100, 100);
		this.ClientSize = new Size(900, 600);

		//Perf.Next();
		Strips.Init();
		MainMenuStrip = Strips.Menubar;
		//Perf.Next();
		Panels.Init();

		this.Controls.Add(Panels.PanelManager);
		this.Controls.Add(Panels.Status);

		_DisableTabOrderOfControls(this);
		this.ResumeLayout(false);

		Application.AddMessageFilter(new _AppMessageFilter());

		//Perf.Next();

		//#if DEBUG
		//		Debug_.Print("Ending form ctor. Please make sure there are no parked controls created before now. Use SetHookToMonitorCreatedWindowsOfThisThread.");
		//#endif
	}

	protected override void OnLoad(EventArgs e)
	{
		//Perf.Next();
		Tasks = new RunningTasks((Wnd)this);
		Panels.Files.LoadCollection(CommandLine.CollectionDirectory);

		//Perf.Next();
		Timer_.After(1, () =>
		{
			Perf.Next();
			Perf.Write();
			//AuDialog.Show(Perf.ToString(), IsWinEventHookInstalled(EVENT_OBJECT_CREATE).ToString()); //IsWinEventHookInstalled always true (false positive, as documented)
			//GC.Collect();

			CommandLine.OnAfterCreatedFormAndOpenedCollection();

			//Close();
		});

		base.OnLoad(e);
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		IsClosed = true;
		CloseReason = e.CloseReason;
		base.OnFormClosed(e);
		Panels.Files.UnloadOnFormClosed();
	}

	/// <summary>
	/// The OnFormClosed override sets this property before unloading collection etc.
	/// </summary>
	public bool IsClosed { get; private set; }

	/// <summary>
	/// The OnFormClosed override sets this property before unloading collection etc.
	/// </summary>
	public CloseReason CloseReason { get; private set; }

	protected override void WndProc(ref Message m)
	{
		Wnd w = (Wnd)this; LPARAM wParam = m.WParam, lParam = m.LParam;
		//var s = m.ToString();

		switch(m.Msg) {
		case Au.LibRun.AuTask.WM_TASK_ENDED: //WM_USER+900
			Tasks.TaskEnded(m.WParam);
			return;
		}

		base.WndProc(ref m);

		switch(m.Msg) {
		case Api.WM_CREATE:
			//Print(w.Get.Children().Length); //0
			if(Settings.Get("wndPos", out string wndPos))
				try { w.RestorePositionSizeState(wndPos, true); } catch { }
			break;
		case Api.WM_DESTROY:
			Settings.Set("wndPos", w.SavePositionSizeState());
			break;
		case Api.WM_ENABLE:
			//.NET ignores this. Eg if an owned form etc disables this window, the Enabled property is not changed and no EnabledChanged event.
			//Print(wParam, Enabled);
			//Enabled = wParam != 0; //not good
			Panels.PanelManager.EnableDisableAllFloatingWindows(wParam != 0);
			break;
		}
	}

	/// <summary>
	/// WM_USER+n messages.
	/// </summary>
	//internal enum EMsg
	//{
	//	xxx = Api.WM_USER + 100,
	//}

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

	/// <summary>
	/// Modifies message loop of this thread, for all forms.
	/// </summary>
	class _AppMessageFilter :IMessageFilter
	{
		public bool PreFilterMessage(ref Message m)
		{
			//switch(m.Msg) {
			//case Api.WM_PAINT:
			//case Api.WM_TIMER:
			//case Api.WM_MOUSEMOVE:
			//case Api.WM_NCMOUSEMOVE:
			//case 0xc281:
			//case 0x60:
			//	return false;
			//}
			//Print(m);

			switch(m.Msg) {
			case Api.WM_MOUSEWHEEL: //let's scroll the mouse control, not the focused control
				var w1 = Wnd.FromMouse();
				if(w1.IsOfThisThread) m.HWnd = w1.Handle;
				break;
			}
			return false;
		}
	}

}

public static class Panels
{
	internal static AuDockPanel PanelManager;
	internal static PanelEdit Editor;
	internal static PanelFiles Files;
	internal static PanelOpen Open;
	internal static PanelRunning Running;
	internal static PanelRecent Recent;
	internal static PanelOutput Output;
	internal static PanelFind Find;
	internal static PanelStatus Status;

	internal static void Init()
	{
		Editor = new PanelEdit();
		Files = new PanelFiles();
		Open = new PanelOpen();
		Running = new PanelRunning();
		Recent = new PanelRecent();
		Output = new PanelOutput();
		Find = new PanelFind();
		Status = new PanelStatus();
#if TEST
		var c = new RichTextBox();
		c.Name = "Results";
#endif

		var m = PanelManager = new AuDockPanel();
		m.Create(Folders.ThisAppBS + @"Default\Panels.xml", Folders.ThisAppDocuments + @"!Settings\Panels.xml",
			Editor, Files, Output, Find, Open, Running, Recent,
#if TEST
			c,
#endif
			Strips.Menubar, Strips.tbFile, Strips.tbEdit, Strips.tbRun, Strips.tbTools, Strips.tbHelp, Strips.tbCustom1, Strips.tbCustom2
			);
		//info: would be easier to specify these in the default XML, but then cannot change in new app versions.
		m.GetPanel(Open).Init("Currently open files", EResources.GetImageUseCache("open"));
		m.GetPanel(Output).Init("Errors and other information", EResources.GetImageUseCache("output"));
		m.GetPanel(Find).Init("Find files, text, triggers", EResources.GetImageUseCache("find"));
		m.GetPanel(Files).Init("All scripts and other files of current collection");
		m.GetPanel(Running).Init("Running scripts");
		m.GetPanel(Recent).Init("Recent running scripts");
#if TEST
		m.GetPanel(c).Init("New panel", EResources.GetImageUseCache("paste"));
#endif
	}
}
