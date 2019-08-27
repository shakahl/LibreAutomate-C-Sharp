using Au;
using Au.Controls;
using Au.Types;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static Au.AStatic;

partial class FMain : Form
{
	AWnd _wFocus;
	const int c_menuid_Exit = 101;

	public static void RunApplication()
	{
		var f = new FMain();
		ATimer.After(1, () => APerf.NW()); //235 ms, ngen 115 ms //TODO
		if(CommandLine.StartVisible || Program.Settings.GetBool("_alwaysVisible")) Application.Run(f);
		else Application.Run();
	}

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
		this.Icon = EdStock.IconAppNormal;
		this.StartPosition = FormStartPosition.Manual;
		unsafe {
			Api.WINDOWPLACEMENT p; int siz = sizeof(Api.WINDOWPLACEMENT);
			if(Program.Settings.GetString("wndPos", out string s) && siz == Au.Util.AConvert.HexDecode(s, &p, siz)) {
				p.rcNormalPosition.EnsureInScreen();
				this.Bounds = p.rcNormalPosition;
				if(p.showCmd == Api.SW_SHOWMAXIMIZED) this.WindowState = FormWindowState.Maximized;
			} else {
				this.Bounds = new Rectangle(100, 50, (AScreen.PrimaryWidth - 100) * 3 / 4, (AScreen.PrimaryHeight - 50) * 4 / 5);
			}
		}

		//APerf.Next();
		Strips.Init();
		MainMenuStrip = Strips.Menubar;
		//APerf.Next();
		Panels.Init();

		this.Controls.Add(Panels.PanelManager);
		this.Controls.Add(Panels.Status);

		this.ResumeLayout(false);

		this.CreateHandleNow(); //this does not create child control handles. We need only of the main form.

		Program.Tasks = new RunningTasks();
		Panels.Files.LoadWorkspace(CommandLine.WorkspaceDirectory);
		EdTrayIcon.Add();
		Au.Triggers.HooksServer.Start(false);
		CommandLine.OnProgramLoaded();
		Program.Loaded = EProgramState.LoadedWorkspace;
		Program.Model.RunStartupScripts();

		//APerf.Next();

		//#if DEBUG
		//		ADebug.Print("Ending form ctor. Must be no parked controls created; use SetHookToMonitorCreatedWindowsOfThisThread.");
		//#endif

		_MonitorGC();//TODO
	}

	AWnd _Hwnd => (AWnd)Handle;

	protected override void OnVisibleChanged(EventArgs e)
	{
		//Print("OnVisibleChanged", Visible, ((AWnd)this).IsVisible); //true, false
		bool visible = Visible;

		//note: we don't use OnLoad. It's unreliable, sometimes not called, eg when made visible from outside.
		if(visible && Program.Loaded == EProgramState.LoadedWorkspace) {

			Panels.PanelManager.GetPanel(Panels.Output).Visible = true; //else Print etc would not auto set visible until the user makes it visible, because handle not created if invisible

			var hm = Api.GetSystemMenu(_Hwnd, false);
			Api.AppendMenu(hm, 0, c_menuid_Exit, "&Exit");

			Application.AddMessageFilter(new _AppMessageFilter());

			Panels.Files.OpenDocuments();

			Program.Loaded = EProgramState.LoadedUI;
			Load?.Invoke(this, EventArgs.Empty);

			CodeInfo.UiLoaded();

#if TEST
			ATimer.After(1, () => {
				var s = CommandLine.TestArg;
				if(s != null) {
					Print(ATime.PerfMicroseconds - Convert.ToInt64(s));
				}
				//APerf.NW('V');

				//EdDebug.PrintTabOrder(this);
			});
#endif
		}

		if(!visible) CodeInfo.Stop();
		UacDragDrop.AdminProcess.Enable(visible);

		base.OnVisibleChanged(e);
	}

	/// <summary>
	/// When first time showing this form.
	/// Documents are open, etc.
	/// </summary>
	public new event EventHandler Load;

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		if(Program.Loaded >= EProgramState.LoadedUI) {
			Program.Settings.Set("wndPos", _Hwnd.SavePositionSizeState());
			UacDragDrop.AdminProcess.Enable(false);
		}
		Program.Loaded = EProgramState.Unloading;
		CloseReason = e.CloseReason;

		base.OnFormClosed(e);

		CodeInfo.Stop();
		Au.Triggers.HooksServer.Stop();
		Panels.Files.UnloadOnFormClosed();
		EdTrayIcon.Dispose();
		Program.Loaded = EProgramState.Unloaded;
		Application.Exit();
	}

	/// <summary>
	/// The OnFormClosed override sets this property before unloading workspace etc.
	/// </summary>
	public CloseReason CloseReason { get; private set; }

	protected override unsafe void WndProc(ref Message m)
	{
		AWnd w = (AWnd)m.HWnd; LPARAM wParam = m.WParam, lParam = m.LParam;
		//AWnd.More.PrintMsg(m, Api.WM_ENTERIDLE, Api.WM_SETCURSOR, Api.WM_GETTEXT, Api.WM_GETTEXTLENGTH, Api.WM_GETICON, Api.WM_NCMOUSEMOVE);

		switch(m.Msg) {
		case RunningTasks.WM_TASK_ENDED: //WM_USER+900
			Program.Tasks.TaskEnded2(m.WParam);
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
		case Api.WM_ACTIVATEAPP:
			if(wParam == default) CodeInfo.Stop();
			break;
		case Api.WM_SYSCOMMAND:
			int sc = (int)wParam;
			if(sc >= 0xf000) { //system
				sc &= 0xfff0;
				if(sc == Api.SC_CLOSE && Visible && !Program.Settings.GetBool("_alwaysVisible")) {
					this.WindowState = FormWindowState.Minimized;
					this.Visible = false;
					EdUtil.MinimizeProcessPhysicalMemory(500);
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
			if(wParam == 4) Program.Tasks.EndTask(); //PBT_APMSUSPEND
			break;
		case Api.WM_WINDOWPOSCHANGING:
			var p = (Api.WINDOWPOS*)lParam;
			//Print(p->flags & Native.LibSwpPublicMask);
			//workaround: if started maximized, does not receive WM_SHOWWINDOW. Then .NET at first makes visible, then creates controls and calls OnLoad.
			if(p->flags.Has(Native.SWP.SHOWWINDOW) && Program.Loaded == EProgramState.LoadedWorkspace) {
				//p->flags &= ~Native.SWP.SHOWWINDOW; //no, adds 5 duplicate messages
				var m2 = Message.Create(m.HWnd, Api.WM_SHOWWINDOW, (IntPtr)1, default);
				base.WndProc(ref m2); //creates controls and calls OnLoad and OnVisibleChanged
				return;
			}
			break;
		}

		base.WndProc(ref m);

		switch(m.Msg) {
		case Api.WM_ENABLE:
			//.NET ignores this. Eg if an owned form etc disables this window, the Enabled property is not changed and no EnabledChanged event.
			//Print(wParam, Enabled);
			//Enabled = wParam != 0; //not good
			Panels.PanelManager.EnableDisableAllFloatingWindows(wParam != 0);
			break;
		}
	}

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
		var f = Program.Model?.CurrentFile;
		if(f == null) title = app;
		//else if(f.IsLink) title = $"{f.Name} ({f.FilePath}) - " + app;
		else title = f.Name + " - " + app;
#else
		if(Model == null) title = app;
		else if(Model.CurrentFile == null) title = app + " - " + Model.WorkspaceName;
		else title = app + " - " + Model.WorkspaceName + " - " + Model.CurrentFile.ItemPath;
#endif
		Text = title;
	}
}

static class Panels
{
	public static AuDockPanel PanelManager;
	public static PanelEdit Editor;
	public static PanelFiles Files;
	public static PanelOpen Open;
	public static PanelRunning Running;
	public static PanelOutput Output;
	public static PanelFind Find;
	public static PanelFound Found;
	public static PanelCodein Codein;
	public static PanelStatus Status;

	internal static void Init()
	{
		Editor = new PanelEdit();
		Files = new PanelFiles();
		Open = new PanelOpen();
		Running = new PanelRunning();
		Output = new PanelOutput();
		Find = new PanelFind();
		Found = new PanelFound();
		Codein = new PanelCodein();
		Status = new PanelStatus();
		//#if TEST
		//		var c = new RichTextBox();
		//		c.Name = "Results";
		//#endif

		var m = PanelManager = new AuDockPanel();
		m.Name = "Panels";
		m.Create(AFolders.ThisAppBS + @"Default\Panels.xml", AFolders.ThisAppDocuments + @"!Settings\Panels.xml",
			Editor, Files, Find, Found, Output, Open, Running, Codein,
			//#if TEST
			//			c,
			//#endif
			Strips.Menubar, Strips.tbFile, Strips.tbEdit, Strips.tbRun, Strips.tbTools, Strips.tbHelp, Strips.tbCustom1, Strips.tbCustom2
			);
		//info: would be easier to specify these in the default XML, but then cannot change in new app versions.
		m.GetPanel(Open).Init("Currently open files"/*, EdResources.GetImageUseCache("open")*/);
		m.GetPanel(Output).Init("Errors and other information"/*, EdResources.GetImageUseCache("output")*/);
		m.GetPanel(Find).Init("Find files, text, triggers"/*, EdResources.GetImageUseCache("find")*/, focusable: true);
		m.GetPanel(Found).Init("Results of find");
		m.GetPanel(Files).Init("All files of this workspace", focusable: true);
		m.GetPanel(Running).Init("Running tasks");
		m.GetPanel(Codein).Init("Code info");
		m.FocusControlOnUndockEtc = Editor;
		//#if TEST
		//		m.GetPanel(c).Init("New panel", EdResources.GetImageUseCache("paste"));
		//#endif
	}
}
