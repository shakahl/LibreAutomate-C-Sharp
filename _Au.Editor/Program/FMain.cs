using Au;
using Au.Controls;
using Au.Types;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime;
using System.Threading;
using System.Windows.Forms;
using static Au.AStatic;

partial class FMain : Form
{
	AWnd _wFocus;
	const int c_menuid_Exit = 101;

	public static void ZRunApplication()
	{
		var f = new FMain();
#if TRACE
		ATimer.After(1, _ => APerf.NW('P'));
#endif
		if(!Program.Settings.runHidden || CommandLine.StartVisible) Application.Run(f);
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
		this.AutoScaleMode = AutoScaleMode.None;
		this.Icon = EdStock.IconAppNormal;
		this.StartPosition = FormStartPosition.Manual;
		unsafe {
			var s = Program.Settings.wndPos;
			if(s!=null && Au.Util.AConvert.Base64UrlDecode(s, out Api.WINDOWPLACEMENT p)) {
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

		this.ResumeLayout(false);

		this.Hwnd(create: true); //this does not create child control handles. We need only of the main form.

		Program.Tasks = new RunningTasks();
		Panels.Files.ZLoadWorkspace(CommandLine.WorkspaceDirectory);
		EdTrayIcon.Add();
		Au.Triggers.HooksServer.Start(false);
		CommandLine.OnProgramLoaded();
		Program.Loaded = EProgramState.LoadedWorkspace;
		Program.Model.RunStartupScripts();

		//APerf.Next();

		//#if DEBUG
		//		ADebug.Print("Ending form ctor. Must be no parked controls created; use SetHookToMonitorCreatedWindowsOfThisThread.");
		//#endif

#if TRACE
		_MonitorGC();
#endif
	}

	AWnd _Hwnd => (AWnd)Handle;

	protected override void OnVisibleChanged(EventArgs e)
	{
		//Print("OnVisibleChanged", Visible, ((AWnd)this).IsVisible); //true, false
		bool visible = Visible;

		//note: we don't use OnLoad. It's unreliable, sometimes not called, eg when made visible from outside.
		if(visible && Program.Loaded == EProgramState.LoadedWorkspace) {

			//APerf.Next();
			_StartProfileOptimization(); //fast
			APerf.Next('v');

			Panels.PanelManager.ZGetPanel(Panels.Output).Visible = true; //else Print etc would not auto set visible until the user makes it visible, because handle not created if invisible

			var hm = Api.GetSystemMenu(_Hwnd, false);
			Api.AppendMenu(hm, 0, c_menuid_Exit, "&Exit");

			Application.AddMessageFilter(new _AppMessageFilter());

			Panels.Files.ZOpenDocuments();

			Program.Loaded = EProgramState.LoadedUI;
			Load?.Invoke(this, EventArgs.Empty);

			APerf.Next('o');
			CodeInfo.UiLoaded();

#if TRACE
			ATimer.After(1, _ => {
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
			Program.Settings.wndPos = _Hwnd.SavePositionSizeState();
			UacDragDrop.AdminProcess.Enable(false);
		}
		Program.Loaded = EProgramState.Unloading;
		ZCloseReason = e.CloseReason;

		base.OnFormClosed(e);

		CodeInfo.Stop();
		Au.Triggers.HooksServer.Stop();
		Panels.Files.ZUnloadOnFormClosed();
		EdTrayIcon.Dispose();
		Program.Loaded = EProgramState.Unloaded;
		Application.Exit();
	}

	/// <summary>
	/// The OnFormClosed override sets this property before unloading workspace etc.
	/// </summary>
	public CloseReason ZCloseReason { get; private set; }

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
		case Api.WM_SYSCOMMAND:
			int sc = (int)wParam;
			if(sc >= 0xf000) { //system
				sc &= 0xfff0;
				if(sc == Api.SC_CLOSE && Visible && Program.Settings.runHidden) {
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
			//Print(p->flags);
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
			Panels.PanelManager.ZEnableDisableAllFloatingWindows(wParam != 0);
			break;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if(base.ProcessCmdKey(ref msg, keyData)) return true;
		//let Esc focus the code editor. If editor focused - previously focused control or the Files treeview. Because the code editor is excluded from tabstopping.
		if(keyData == Keys.Escape || keyData == (Keys.Escape| Keys.Shift)) {
			var doc = Panels.Editor.ZActiveDoc;
			if(doc != null) {
				if(doc.Focused) {
					if(keyData == Keys.Escape) {
						CodeInfo.Cancel();
					} else {
						var c = _escFocus;
						for(int i = 0; ; i++) {
							if(c != null && !c.IsDisposed && c.Visible && c.FindForm() == this && c.Focus()) break;
							if(i == 0) c = Panels.Files.ZControl;
							else if(i == 1) c = this.GetNextControl(doc, true);
							else break;
						}
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

	static void _StartProfileOptimization()
	{
#if !DEBUG
		var fProfile = AFolders.ThisAppDataLocal + "ProfileOptimization";
		AFile.CreateDirectory(fProfile);
		ProfileOptimization.SetProfileRoot(fProfile);
		ProfileOptimization.StartProfile("Editor.speed"); //makes startup faster eg 680 -> 560 ms. Makes compiler startup faster 4000 -> 2500 (ngen 670).
#endif
	}

	public void ZShowAndActivate()
	{
		Show();
		var w = (AWnd)Program.MainForm;
		w.ShowNotMinimized(true);
		w.ActivateLL();
	}

	public void ZSetTitle()
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
	public static PanelInfo Info;

	internal static void Init()
	{
		//Print("----");
		//var p1 = APerf.Create();
		Editor = new PanelEdit();
		//p1.Next('e');
		Files = new PanelFiles();
		//p1.Next('f');
		Open = new PanelOpen();
		Running = new PanelRunning();
		//p1.Next('r');
		Output = new PanelOutput();
		//p1.Next('o');
		Find = new PanelFind();
		//p1.Next('1');
		Found = new PanelFound();
		//p1.Next('2');
		Info = new PanelInfo();
		//p1.Next('i');
		//p1.Write();
		//#if TRACE
		//		var c = new RichTextBox();
		//		c.Name = "Test";
		//#endif

		var m = PanelManager = new AuDockPanel();
		m.Name = "Panels";
		m.ZCreate(AFolders.ThisAppBS + @"Default\Panels.xml", AFolders.ThisAppDocuments + @"!Settings\Panels.xml",
			Editor, Files, Find, Found, Output, Open, Running, Info,
			//#if TRACE
			//			c,
			//#endif
			Strips.Menubar, Strips.tbFile, Strips.tbEdit, Strips.tbRun, Strips.tbTools, Strips.tbHelp, Strips.tbCustom1, Strips.tbCustom2
			);
		//info: would be easier to specify these in default XML, but then cannot change in new app versions.
		m.ZGetPanel(Files).Init("Files - all files of this workspace", focusable: true);
		m.ZGetPanel(Open).Init("Open - currently open files"/*, EdResources.GetImageUseCache("open")*/);
		m.ZGetPanel(Running).Init("Running - running script tasks");
		m.ZGetPanel(Find).Init("Find - find text, files"/*, EdResources.GetImageUseCache("find")*/, focusable: true);
		m.ZGetPanel(Output).Init("Output - errors and other information", EdResources.GetImageUseCache("output"));
		m.ZGetPanel(Info).Init("Info - quick info about object from mouse", EdResources.GetImageUseCache("info"));
		m.ZGetPanel(Found).Init("Found - results of 'Find in files'", EdResources.GetImageUseCache("found"));
		m.ZFocusControlOnUndockEtc = Editor;
		//#if TRACE
		//		m.GetPanel(c).Init("New panel", EdResources.GetImageUseCache("paste"));
		//#endif
	}
}
