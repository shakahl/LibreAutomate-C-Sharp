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
using System.Linq;
using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.AStatic;
using static Program;
using Au.Controls;

partial class FMain : Form
{
	public static void RunApplication()
	{
		Application.Run(new FMain());
	}

	static bool _StartsVisible => CommandLine.StartVisible || Settings.GetBool("_alwaysVisible");

	public FMain()
	{

		//#if DEBUG
		//		AOutput.QM2.UseQM2 = true; AOutput.Clear();
		//		SetHookToMonitorCreatedWindowsOfThisThread();
		//#endif

		Program.MainForm = this;

		this.SuspendLayout();
		this.Font = EdStock.FontRegular;
		this.AutoScaleMode = AutoScaleMode.None;
		this.StartPosition = FormStartPosition.Manual;
		this.Location = new Point(100, 50);
		this.Size = new Size((AScreen.PrimaryWidth - 100) * 3 / 4, (AScreen.PrimaryHeight - 50) * 4 / 5);
		this.Icon = EdStock.IconAppNormal;

		//APerf.Next();
		Strips.Init();
		MainMenuStrip = Strips.Menubar;
		//APerf.Next();
		Panels.Init();

		this.Controls.Add(Panels.PanelManager);
		this.Controls.Add(Panels.Status);

		this.ResumeLayout(false);

		Application.AddMessageFilter(new _AppMessageFilter());

		//Print(IsHandleCreated); foreach(Control v in Controls) if(v.IsHandleCreated) Print(v);

		//APerf.Next();

		//#if DEBUG
		//		ADebug.Print("Ending form ctor. Must be no parked controls created; use SetHookToMonitorCreatedWindowsOfThisThread.");
		//#endif
	}

	const int c_menuid_Exit = 101;

	/// <summary>
	/// Called after creating handles of form and controls, when form still invisible.
	/// </summary>
	void _OnLoad()
	{
		var hm = Api.GetSystemMenu(_Hwnd, false);
		//Api.AppendMenu(hm);
		Api.AppendMenu(hm, 0, c_menuid_Exit, "&Exit");

		Tasks = new RunningTasks();
		Panels.Files.LoadWorkspace(CommandLine.WorkspaceDirectory, runStartupScript: false);
		EdTrayIcon.Add();
		ADebug.PrintIf(_Hwnd.IsVisible, "BAD: form became visible while loading workspace");
		Au.Triggers.HooksServer.Start(false);
		CommandLine.OnMainFormLoaded();
		IsLoaded = true;
		Model.RunStartupScripts();
		//APerf.Next();

#if TEST
		ATimer.After(1, () => {
			var s = CommandLine.TestArg;
			if(s != null) {
				Print(ATime.PerfMicroseconds - Convert.ToInt64(s));
			}
			APerf.Next('P');
			//APerf.Write();

			//EdDebug.PrintTabOrder(this);
		});
#endif
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		IsClosed = true;
		CloseReason = e.CloseReason;
		base.OnFormClosed(e);
		Au.Triggers.HooksServer.Stop();
		UacDragDrop.AdminProcess.Enable(false);
		Panels.Files.UnloadOnFormClosed();
		EdTrayIcon.Dispose();
	}

	/// <summary>
	/// Becomes true after loading workspace etc.
	/// </summary>
	public bool IsLoaded { get; private set; }

	/// <summary>
	/// The OnFormClosed override sets this property before unloading workspace etc.
	/// </summary>
	public bool IsClosed { get; private set; }

	/// <summary>
	/// The OnFormClosed override sets this property before unloading workspace etc.
	/// </summary>
	public CloseReason CloseReason { get; private set; }

	protected override void OnVisibleChanged(EventArgs e)
	{
		bool visible = Visible;
		if(!_visibleOnce && visible) {
			_visibleOnce = true;
			VisibleFirstTime?.Invoke();
			VisibleFirstTime = null;
		}
		UacDragDrop.AdminProcess.Enable(visible);
		base.OnVisibleChanged(e);
	}
	bool _visibleOnce;

	public event Action VisibleFirstTime;

	AWnd _Hwnd => (AWnd)Handle;

	protected override unsafe void WndProc(ref Message m)
	{
		AWnd w = (AWnd)m.HWnd; LPARAM wParam = m.WParam, lParam = m.LParam;
		//Print(m);

		switch(m.Msg) {
		case RunningTasks.WM_TASK_ENDED: //WM_USER+900
			Tasks.TaskEnded2(m.WParam);
			return;
		case Api.WM_ACTIVATE:
			int isActive = AMath.LoUshort(wParam); //0 inactive, 1 active, 2 click-active
			if(isActive == 1 && !w.IsActive && !Api.SetForegroundWindow(w)) {
				//Normally at startup always inactive, because started as admin from task scheduler. SetForegroundWindow sometimes works, sometimes not.
				//workaround for: If clicked a window after our app started but before w activated, w is at Z bottom and in some cases without taskbar button.
				ADebug.Print("window inactive");
				AWnd.More.TaskbarButton.Add(w);
				if(!w.ActivateLL()) AWnd.More.TaskbarButton.Flash(w, 5);
			}
			//restore focused control correctly
			if(isActive == 0) _wFocus = AWnd.ThisThread.Focused;
			else if(_wFocus.IsAlive) AWnd.ThisThread.Focus(_wFocus);
			return;
		case Api.WM_SYSCOMMAND:
			int sc = (int)wParam;
			if(sc >= 0xf000) { //system
				sc &= 0xfff0;
				if(sc == Api.SC_CLOSE && Visible && !Settings.GetBool("_alwaysVisible")) {
					this.WindowState = FormWindowState.Minimized;
					this.Visible = false;
					ThreadPool.QueueUserWorkItem(_ => {
						500.ms();
						GC.Collect();
						GC.WaitForPendingFinalizers();
						Api.SetProcessWorkingSetSize(Api.GetCurrentProcess(), -1, -1);
					});
					return;
					//initially this code was in OnFormClosing, but sometimes hides instead of closing, because .NET gives incorrect CloseReason. Cannot reproduce and debug.
				}
			} else { //our
				switch(sc) {
				case c_menuid_Exit: Strips.Cmd.File_Exit(); return;
				}
			}
			break;
		case Api.WM_POWERBROADCAST:
			if(wParam == 4) Tasks.EndTask(); //PBT_APMSUSPEND
			break;
		}

		base.WndProc(ref m);

		switch(m.Msg) {
		case Api.WM_CREATE:
			if(Settings.GetString("wndPos", out string wndPos))
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

	AWnd _wFocus;

#if true
	protected override void SetVisibleCore(bool value)
	{
		//Workaround for .NET inability to create hidden form normally.
		//	note: while form invisible, .NET will not create handles of controls added after CreateControl_.
		if(value && !IsHandleCreated) {
			this.CreateControlNow();
			_OnLoad();
			if(!_StartsVisible) return;
		}
		base.SetVisibleCore(value);
	}
#else
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		_OnLoad();
	}
#endif

	///// <summary>
	///// WM_USER+n messages.
	///// </summary>
	//internal static class EMsg
	//{
	//}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if(base.ProcessCmdKey(ref msg, keyData)) return true;
		//let Esc focus the code editor. If editor focused - previously focused control or the Files treeview. Because the code editor is excluded from tabstopping.
		if(keyData == Keys.Escape) {
			var doc = Panels.Editor.ActiveDoc;
			if(doc != null) {
				if(doc.Focused) {
					var c = _escFocus;
					for(int i = 0; ; i++) {
						if(c != null && !c.IsDisposed && c.Visible && c.FindForm() == this && c.Focus()) break;
						if(i == 0) c = Panels.Files.Control;
						else if(i == 1) c = this.GetNextControl(doc, true);
						else break;
					}
				} else {
					_escFocus = Control.FromHandle(msg.HWnd);
					doc.Focus();
				}
			}
		}
		return false;
	}
	Control _escFocus;

	/// <summary>
	/// Modifies message loop of this thread, for all forms.
	/// </summary>
	class _AppMessageFilter : IMessageFilter
	{
		public bool PreFilterMessage(ref Message m)
		{
			switch(m.Msg) {
			case Api.WM_MOUSEWHEEL: //let's scroll the mouse control, not the focused control
				var w1 = AWnd.FromMouse();
				if(w1.IsOfThisThread) m.HWnd = w1.Handle;
				break;
			}
			return false;
		}
	}

	public void SetTitle()
	{
		string title, app = Program.AppName;
#if true
		var f = Model?.CurrentFile;
		if(f == null) title = app;
		//else if(f.IsLink) title = $"{f.Name} ({f.FilePath}) - " + app;
		else title = f.Name + " - " + app;
#else
		if(Model == null) title = app;
		else if(Model.CurrentFile == null) title = app + " - " + Model.WorkspaceName;
		else title = app + " - " + Model.WorkspaceName + " - " + Model.CurrentFile.ItemPath;
#endif
		//Text = title; //no, makes form visible
		_Hwnd.SendS(Api.WM_SETTEXT, 0, title);
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
	internal static PanelFound Found;
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
		Found = new PanelFound();
		Status = new PanelStatus();
		//#if TEST
		//		var c = new RichTextBox();
		//		c.Name = "Results";
		//#endif

		var m = PanelManager = new AuDockPanel();
		m.Name = "Panels";
		m.Create(AFolders.ThisAppBS + @"Default\Panels.xml", AFolders.ThisAppDocuments + @"!Settings\Panels.xml",
			Editor, Files, Find, Found, Output, Open, Running, Recent,
			//#if TEST
			//			c,
			//#endif
			Strips.Menubar, Strips.tbFile, Strips.tbEdit, Strips.tbRun, Strips.tbTools, Strips.tbHelp, Strips.tbCustom1, Strips.tbCustom2
			);
		//info: would be easier to specify these in the default XML, but then cannot change in new app versions.
		m.GetPanel(Open).Init("Currently open files", EdResources.GetImageUseCache("open"));
		m.GetPanel(Output).Init("Errors and other information", EdResources.GetImageUseCache("output"));
		m.GetPanel(Find).Init("Find files, text, triggers", EdResources.GetImageUseCache("find"), focusable: true);
		m.GetPanel(Found).Init("Results of find");
		m.GetPanel(Files).Init("All files of this workspace", focusable: true);
		m.GetPanel(Running).Init("Running tasks");
		m.GetPanel(Recent).Init("Recent tasks");
		m.FocusControlOnUndockEtc = Editor;
		//#if TEST
		//		m.GetPanel(c).Init("New panel", EdResources.GetImageUseCache("paste"));
		//#endif
	}
}
