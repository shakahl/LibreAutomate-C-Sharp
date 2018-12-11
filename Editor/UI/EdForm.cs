//Defines what workaround to use for .NET inability to create hidden form.
//If RUNHIDDEN_WINDOWPOSCHANGING, modifies setwindowpos flags on WM_WINDOWPOSCHANGING and overrides ShowWithoutActivation.
//Else uses ApplicationContext and CreateControl_.
//Both work. Similar speed, maybe RUNHIDDEN_WINDOWPOSCHANGING faster.
//RUNHIDDEN_WINDOWPOSCHANGING has some advantages. Eg .NET creates handles of other controls that we create after CreateControl_.
#define RUNHIDDEN_WINDOWPOSCHANGING

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

partial class EdForm : Form
{
#if RUNHIDDEN_WINDOWPOSCHANGING
	bool _canShow;
#endif

	public static void RunApplication()
	{
#if RUNHIDDEN_WINDOWPOSCHANGING
		Application.Run(new EdForm());
#else
		Application.Run(new _AppContext());
#endif
	}

#if !RUNHIDDEN_WINDOWPOSCHANGING
	class _AppContext :ApplicationContext
	{
		public _AppContext()
		{
			var f = new EdForm();
			f.CreateControl_();
			f._LoadContent();
			if(_StartsVisible) this.MainForm = f;
			else f.FormClosed += (unu, sed) => ExitThread();
		}
	}
#endif

	static bool _StartsVisible => true; //TODO: if commandline /v

	public EdForm()
	{
		//Output.LibUseQM2 = true; Output.Clear();

		//#if DEBUG
		//		SetHookToMonitorCreatedWindowsOfThisThread();
		//#endif

		Program.MainForm = this;

		this.SuspendLayout();
		this.Font = EdStock.FontRegular;
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

#if RUNHIDDEN_WINDOWPOSCHANGING
		if(_StartsVisible) _canShow = true;
#endif

		//Print(IsHandleCreated); foreach(Control v in Controls) if(v.IsHandleCreated) Print(v);

		//Perf.Next();

		//#if DEBUG
		//		Debug_.Print("Ending form ctor. Must be no parked controls created; use SetHookToMonitorCreatedWindowsOfThisThread.");
		//#endif
	}

	/// <summary>
	/// Called after creating handles of form and controls.
	/// </summary>
	void _LoadContent()
	{
		Tasks = new RunningTasks();
		Panels.Files.LoadWorkspace(CommandLine.WorkspaceDirectory);
		Debug.Assert(!((Wnd)this).IsVisible);
		IsLoaded = true;
		//Perf.Next();
	}

	/// <summary>
	/// Called when form shown first time, on WM_SHOWWINDOW.
	/// </summary>
	protected override void OnLoad(EventArgs e)
	{
		//Print("OnLoad");
#if RUNHIDDEN_WINDOWPOSCHANGING
		_LoadContent();
		if(!_canShow) ((Wnd)this).Post(EMsg.WM_EDITOR_CANSHOW);
#endif

		Timer_.After(1, () => {
			var s = CommandLine.TestArg;
			if(s != null) {
				Print(Time.Microseconds - Convert.ToInt64(s));
			}
			Perf.Next('P');
			Perf.Write();

			CommandLine.OnAfterCreatedFormAndOpenedWorkspace();
		});

		base.OnLoad(e);
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		IsClosed = true;
		CloseReason = e.CloseReason;
		base.OnFormClosed(e);
		UacDragDrop.AdminProcess.Enable(false);
		Panels.Files.UnloadOnFormClosed();
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
		//Output.LibWriteQM2($"_canShow={_canShow}, _visibleOnce={_visibleOnce}, Visible={Visible}");
#if RUNHIDDEN_WINDOWPOSCHANGING
		if(!_canShow) return;
#endif
		bool visible = Visible;
		if(!_visibleOnce && visible) {
			_visibleOnce = true;
			Api.SetForegroundWindow((Wnd)this);
			//Output.LibWriteQM2("VISIBLE");
			VisibleFirstTime?.Invoke();
			VisibleFirstTime = null;
		}
		UacDragDrop.AdminProcess.Enable(visible);
		base.OnVisibleChanged(e);
	}
	bool _visibleOnce;

	public event Action VisibleFirstTime;

	protected override unsafe void WndProc(ref Message m)
	{
		Wnd w = (Wnd)this; LPARAM wParam = m.WParam, lParam = m.LParam;
		//var s = m.ToString();
		//Print(m);

		switch(m.Msg) {
		case RunningTasks.WM_TASK_ENDED: //WM_USER+900
			Tasks.TaskEnded2(m.WParam);
			return;
		case Api.WM_ACTIVATE:
			int isActive = Math_.LoUshort(wParam); //0 inactive, 1 active, 2 click-active
			if(isActive == 1 && !w.IsActive && !Api.SetForegroundWindow(w)) {
				//workaround for Windows bug:
				//	If clicked a window after our app started but before w activated, w is at Z bottom and in some cases there is no taskbar button.
				Debug_.Print("window inactive");
				Wnd.Misc.TaskbarButton.Add(w);
				if(!w.ActivateLL()) Wnd.Misc.TaskbarButton.Flash(w, 5);
			}
			//restore focused control correctly
			if(isActive == 0) _wFocus = Wnd.ThisThread.Focused;
			else if(_wFocus.IsAlive) Wnd.ThisThread.Focus(_wFocus);
			return;
#if RUNHIDDEN_WINDOWPOSCHANGING
		case Api.WM_WINDOWPOSCHANGING:
			if(!_canShow) {
				var wp = (Api.WINDOWPOS*)lParam;
				wp->flags &= ~Native.SWP.SHOWWINDOW; wp->flags |= Native.SWP.NOACTIVATE | Native.SWP.NOZORDER;
				base.DefWndProc(ref m);
				return;
			}
			break;
		case EMsg.WM_EDITOR_CANSHOW:
			Visible = false;
			_canShow = true;
			return;
#endif
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

	Wnd _wFocus;

	/// <summary>
	/// WM_USER+n messages.
	/// </summary>
	internal static class EMsg
	{
		public const int WM_EDITOR_CANSHOW = Api.WM_USER + 100;
	}

#if RUNHIDDEN_WINDOWPOSCHANGING
	protected override bool ShowWithoutActivation => !_canShow;
#endif

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
	class _AppMessageFilter : IMessageFilter
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

	public void SetTitle()
	{
		const string app = "QM#";
		string title;
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
		((Wnd)Handle).SendS(Api.WM_SETTEXT, 0, title);
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
		m.GetPanel(Open).Init("Currently open files", EdResources.GetImageUseCache("open"));
		m.GetPanel(Output).Init("Errors and other information", EdResources.GetImageUseCache("output"));
		m.GetPanel(Find).Init("Find files, text, triggers", EdResources.GetImageUseCache("find"));
		m.GetPanel(Files).Init("All scripts and other files of current workspace");
		m.GetPanel(Running).Init("Running scripts");
		m.GetPanel(Recent).Init("Recent running scripts");
#if TEST
		m.GetPanel(c).Init("New panel", EdResources.GetImageUseCache("paste"));
#endif
	}
}
