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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.Triggers
{
	/// <summary>
	/// This namespace contains classes of triggers, for example hotkeys.
	/// </summary>
	[CompilerGenerated()]
	class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	/// <summary>
	/// The main class of action triggers.
	/// </summary>
	/// <remarks>
	/// There are two categories of triggers:
	/// 1. Script triggers launch automation scripts, usually as new processes (running programs).
	/// 2. Action triggers launch functions (aka <i>trigger actions</i>) in running automation scripts. Most trigger types are action triggers.
	/// 
	/// This class manages action triggers. The <see cref="AuScript.Triggers" r=""/> property gets its instance. Through it you access all trigger types (hotkey, window, etc) and add triggers to them.
	/// 
	/// Code syntax to add an action trigger:
	/// <code>Triggers.TriggerType[parameters] = action;</code>
	/// Examples:
	/// <code>
	/// Triggers.Hotkey["Ctrl+K"] = o => Print(o.Trigger);
	/// Triggers.Hotkey["Ctrl+Shift+K"] = o => {
	///		Print("This is a trigger action. Usually it is a lambda function, like in these examples.");
	///		Print($"It runs when you press {o.Trigger}.");
	/// };
	/// Triggers.Run();
	/// </code>
	/// 
	/// Also you can set options (<see cref="TriggerOptions">Triggers.Options</see>), window scopes (<see cref="TriggerScopes">Triggers.Of</see>) and custom scopes (<see cref="TriggerFuncs">Triggers.FuncOf</see>) for multiple triggers added afterwards.
	/// 
	/// Finally call <see cref="Run">Triggers.Run()</see>. It runs all the time (like <b>Application.Run</b>) and launches trigger actions (functions) when need. Actions run in other thread(s).
	/// </remarks>
	/// <example>
	/// This is a single script with many action triggers.
	/// <code><![CDATA[
	/// //if you want to set options for all or some triggers, do it before adding these triggers
	/// Triggers.Options.RunActionInThread(0, 500);
	/// 
	/// //you can use variables if don't want to type "Triggers.Hotkey" etc for each trigger
	/// var hk = Triggers.Hotkey;
	/// var mouse = Triggers.Mouse;
	/// var window = Triggers.Window;
	/// var tt = Triggers.Autotext;
	/// 
	/// //hotkey triggers
	/// 
	/// hk["Ctrl+K"] = o => Print(o.Trigger);
	/// hk["Ctrl+Shift+F11"] = o => {
	/// 	Print(o.Trigger);
	/// 	var w1 = Wnd.FindOrRun("* Notepad", run: () => Shell.Run(Folders.System + "notepad.exe"));
	/// 	Text("text");
	/// 	w1.Close();
	/// };
	/// 
	/// //triggers that work only with some windows
	/// 
	/// Triggers.Of.Window("* WordPad", "WordPadClass"); //let the following triggers work only when a WordPad window is active
	/// hk["Ctrl+F5"] = o => Print(o.Trigger, o.Window);
	/// hk["Ctrl+F6"] = o => Print(o.Trigger, o.Window);
	/// 
	/// var notepad = Triggers.Of.Window("* Notepad"); //let the following triggers work only when a Notepad window is active
	/// hk["Ctrl+F5"] = o => Print(o.Trigger, o.Window);
	/// hk["Ctrl+F6"] = o => Print(o.Trigger, o.Window);
	/// 
	/// Triggers.Of.AllWindows(); //let the following triggers work with all windows
	/// 
	/// //mouse triggers
	/// 
	/// mouse[TMClick.Right, "Ctrl+Shift", TMFlags.ButtonModUp] = o => Print(o.Trigger);
	/// mouse[TMEdge.RightInCenter50] = o => { Print(o.Trigger); AuDialog.ShowEx("Bang!", x: Coord.Max); };
	/// mouse[TMMove.LeftRightInCenter50] = o => Wnd.SwitchActiveWindow();
	/// 
	/// Triggers.FuncOf.NextTrigger = o => Keyb.IsScrollLock; //example of a custom scope (aka context, condition)
	/// mouse[TMWheel.Forward] = o => Print($"{o.Trigger} while ScrollLock is on");
	/// 
	/// Triggers.Of.Again(notepad); //let the following triggers work only when a Notepad window is active
	/// mouse[TMMove.LeftRightInBottom25] = o => { Print(o.Trigger); o.Window.Close(); };
	/// Triggers.Of.AllWindows();
	/// 
	/// //window triggers. Note: window triggers don't depend on Triggers.Of window.
	/// 
	/// window.ActiveNew["* Notepad", "Notepad"] = o => Print("opened Notepad window");
	/// window.ActiveNew["Notepad", "#32770", contains: "Do you want to save *"] = o => {
	/// 	Print("opened Notepad's 'Do you want to save' dialog");
	/// 	Key("Alt+S");
	/// };
	/// 
	/// //autotext triggers
	/// 
	/// tt["los"] = o => o.Replace("Los Angeles");
	/// tt["WIndows", TAFlags.MatchCase] = o => o.Replace("Windows");
	/// tt.DefaultPostfixType = TAPostfix.None;
	/// tt["<b>"] = o => o.Replace("<b>[[|]]</b>");
	/// Triggers.Options.BeforeAction = o => { Opt.Key.TextOption = KTextOption.Paste; };
	/// tt["#file"] = o => {
	/// 	o.Replace("");
	/// 	var fd = new OpenFileDialog();
	/// 	if(fd.ShowDialog() == DialogResult.OK) Text(fd.FileName);
	/// };
	/// Triggers.Options.BeforeAction = null;
	/// tt.DefaultPostfixType = default;
	/// 
	/// var ts = Triggers.Autotext.Simple;
	/// ts["#su"] = "Sunday"; //the same as tt["#su"] = o => o.Replace("Sunday");
	/// ts["#mo"] = "Monday";
	/// 
	/// //how to stop and disable/enable triggers
	/// 
	/// hk["Ctrl+Alt+Q"] = o => Triggers.Stop(); //let Triggers.Run() end its work and return
	/// hk.Last.EnabledAlways = true;
	/// 
	/// hk["Ctrl+Alt+D"] = o => Triggers.Disabled ^= true; //disable/enable triggers here
	/// hk.Last.EnabledAlways = true;
	/// 
	/// hk["Ctrl+Alt+Win+D"] = o => ActionTriggers.DisabledEverywhere ^= true; //disable/enable triggers in all processes
	/// hk.Last.EnabledAlways = true;
	/// 
	/// hk["Ctrl+F7"] = o => Print("This trigger can be disabled/enabled with Ctrl+F8.");
	/// var t1 = hk.Last;
	/// hk["Ctrl+F8"] = o => t1.Disabled ^= true; //disable/enable a trigger
	/// 
	/// //finally call Triggers.Run(). Without it the triggers won't work.
	/// Triggers.Run();
	/// //Triggers.Run returns when is called Triggers.Stop (see the "Ctrl+Alt+Q" trigger above).
	/// Print("called Triggers.Stop");
	/// //Recommended properties for scripts containg triggers: 'runMode'='blue' and 'ifRunning'='restart'. You can set it in the Properties dialog.
	/// //The first property allows other scripts to start while this script is running.
	/// //The second property makes easy to restart the script after editing: just click the Run button.
	/// ]]></code>
	/// </example>
	public class ActionTriggers
	{
		readonly ITriggers[] _t;
		ITriggers this[TriggerType e] => _t[(int)e];

		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <remarks>
		/// In automation scripts don't need to create new instances of this class. Instead use the <see cref="AuScript.Triggers" r=""/> property to get an instance.
		/// </remarks>
		public ActionTriggers()
		{
			_t = new ITriggers[(int)TriggerType.Count];
			scopes = new TriggerScopes();
			funcs = new TriggerFuncs();
			options = new TriggerOptions();
		}

		//public TriggerScopes Of {
		//	get {
		//		if(_test == false){
		//			Debug_.LibMemorySetAnchor();
		//			Timer_.After(500, () => Debug_.LibMemoryPrint());
		//		}

		//		var k=new StackFrame(1, true);
		//		//new StackTrace(1, true);
		//		var s = k.GetFileName();

		//		//if(_test == false) _s1 = s; else Print(ReferenceEquals(s, _s1));

		//			 _test = true;

		//		return scopes;
		//	}
		//}
		//static bool _test;
		//static string _s1;

		/// <summary>
		/// Allows to set window scopes (working windows) for triggers.
		/// </summary>
		/// <remarks>Examples: <see cref="TriggerScopes"/>, <see cref="ActionTriggers"/>.</remarks>
		public TriggerScopes Of => scopes;
		internal readonly TriggerScopes scopes;

		/// <summary>
		/// Allows to set custom scopes/contexts/conditions for triggers.
		/// </summary>
		/// <remarks>More info and examples: <see cref="TriggerFuncs"/>, <see cref="ActionTriggers"/>.</remarks>
		public TriggerFuncs FuncOf => funcs;
		internal readonly TriggerFuncs funcs;

		/// <summary>
		/// Allows to set some options for multiple triggers and their actions.
		/// </summary>
		/// <remarks>More info and examples: <see cref="TriggerOptions"/>, <see cref="ActionTriggers"/>.</remarks>
		public TriggerOptions Options => options;
		internal readonly TriggerOptions options;

		TriggerActionThreads _threads;

		internal int LibThreadId => _threadId;
		int _threadId;

		ITriggers _Get(TriggerType e)
		{
			int i = (int)e;
			var t = _t[i];
			if(t == null) {
				switch(e) {
				case TriggerType.Hotkey: t = new HotkeyTriggers(this); break;
				case TriggerType.Autotext: t = new AutotextTriggers(this); break;
				case TriggerType.Mouse: t = new MouseTriggers(this); break;
				case TriggerType.Window: t = new WindowTriggers(this); break;
				default: Debug.Assert(false); break;
				}
				_t[i] = t;
			}
			return t;
		}

		/// <summary>
		/// Hotkey triggers.
		/// </summary>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public HotkeyTriggers Hotkey => _Get(TriggerType.Hotkey) as HotkeyTriggers;

		/// <summary>
		/// Autotext triggers.
		/// </summary>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public AutotextTriggers Autotext => _Get(TriggerType.Autotext) as AutotextTriggers;

		/// <summary>
		/// Mouse triggers.
		/// </summary>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public MouseTriggers Mouse => _Get(TriggerType.Mouse) as MouseTriggers;

		/// <summary>
		/// Window triggers.
		/// </summary>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public WindowTriggers Window => _Get(TriggerType.Window) as WindowTriggers;

		/// <summary>
		/// Makes triggers alive.
		/// </summary>
		/// <remarks>
		/// This function monitors hotkeys, activated windows and other events. When an event matches an added trigger, launches the thrigger's action, which runs in other thread.
		/// Does not return immediately, unless there are no triggers added. Runs until this process or thread is terminated/aborted or <see cref="Stop"/> called.
		/// Example: <see cref="ActionTriggers"/>.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Already running.</exception>
		/// <exception cref="AuException">Something failed.</exception>
		public void Run()
		{
			//AppDomain.CurrentDomain.AssemblyLoad += (object sender, AssemblyLoadEventArgs args) => {
			//	var a = args.LoadedAssembly;
			//	Print(a);
			//	//if(a.FullName.StartsWith_("System.Drawing")) {
			//	//	//Print(new StackTrace());
			//	//	Debugger.Launch();
			//	//}
			//};

			LibThrowIfRunning();

			bool haveTriggers = false; HooksServer.UsedEvents hookEvents = 0;
			_windowTriggers = null;
			for(int i = 0; i < _t.Length; i++) {
				var t = _t[i];
				if(t == null || !t.HasTriggers) continue;
				haveTriggers = true;
				switch((TriggerType)i) {
				case TriggerType.Hotkey: hookEvents |= HooksServer.UsedEvents.Keyboard; break;
				case TriggerType.Autotext: hookEvents |= HooksServer.UsedEvents.Keyboard | HooksServer.UsedEvents.Mouse; break;
				case TriggerType.Mouse: hookEvents |= (t as MouseTriggers).LibUsedHookEvents; break;
				case TriggerType.Window: _windowTriggers = t as WindowTriggers; break;
				}
			}
			//Print(haveTriggers, (uint)llHooks);
			if(!haveTriggers) return;
			_winTimerPeriod = 0;
			_winTimerLastTime = 0;

			try {
				_threadId = Thread_.NativeId;
				if(hookEvents != 0) {
					_RunWithHooksServer(hookEvents);
					//CONSIDER: run key/mouse triggers in separate thread.
					//	Because now possible hook timeout if something other (window triggers, a form, timer, etc) sometimes runs too long.
					//	But it makes programming more difficult in various places. Need to lock etc.
				} else {
					_RunSimple();
				}
			}
			finally {
				_threads?.Dispose(); _threads = null;
				_threadId = 0;
			}
		}

		unsafe void _RunSimple()
		{
			try {
				_StartStop(true);
				IntPtr h = _evStop;
				_Wait(&h, 1);
			}
			finally {
				_StartStop(false);
			}
		}

		unsafe int _Wait(IntPtr* ha, int nh)
		{
			for(; ; ) {
				int slice = -1;
				if(_winTimerPeriod > 0) {
					long t = Time.PerfMilliseconds;
					if(_winTimerLastTime == 0) _winTimerLastTime = t;
					int td = (int)(t - _winTimerLastTime);
					int period = _Period();
					if(td >= period - 5) {
						_winTimerLastTime = t;
						_windowTriggers?.LibTimer();
						slice = _Period();
					} else slice = period - td;

					int _Period() => _winTimerPeriod / 15 * 15 + 10;
					//int _Period() => Math.Max(_winTimerPeriod, 10);
				}
				var k = Api.MsgWaitForMultipleObjectsEx(nh, ha, slice, Api.QS_ALLINPUT, Api.MWMO_ALERTABLE | Api.MWMO_INPUTAVAILABLE);
				if(k == nh) { //message, COM (RPC uses postmessage), hook, etc
					while(Api.PeekMessage(out var m, default, 0, 0, Api.PM_REMOVE)) {
						if(m.hwnd.Is0 && m.message == Api.WM_USER + 20) {
							_windowTriggers.LibSimulateNew(m.wParam, m.lParam);
							continue;
						}
						Api.TranslateMessage(m);
						Api.DispatchMessage(m);
					}
				} else if(!(k == Api.WAIT_TIMEOUT || k == Api.WAIT_IO_COMPLETION)) return k; //signaled handle, abandoned mutex, WAIT_FAILED (-1)
			}
		}
		long _winTimerLastTime;
		WindowTriggers _windowTriggers;

		internal int LibWinTimerPeriod {
			get => _winTimerPeriod;
			set {
				long t = Time.PerfMilliseconds;
				int td = (int)(t - _winTimerLastTime);
				if(td > 10) _winTimerLastTime = t;
				_winTimerPeriod = value;
			}
		}
		int _winTimerPeriod;

		unsafe void _RunWithHooksServer(HooksServer.UsedEvents usedEvents)
		{
			//Perf.Next();

			//prevent big delay later on first LL hook event while hook proc waits
			if(scopes.Used || funcs.Used) {
				ThreadPool.QueueUserWorkItem(_ => { //never mind: should do it once. Several Triggers.Run in task is rare. Fast next time.
					try {
						Util.Assembly_.LibEnsureLoaded(true, true); //System.Core, System, System.Windows.Forms, System.Drawing
						if(!Util.Assembly_.LibIsAuNgened) {
							new Wnd.Finder("*a").IsMatch(Wnd.Active);
							Wnd.FromXY(default, WXYFlags.NeedWindow);
							Keyb.GetMod();
							_ = Time.PerfMicroseconds;
							Util.Jit.Compile(typeof(HotkeyTriggers), nameof(HotkeyTriggers.HookProc));
							//Util.Jit.Compile(typeof(AutotextTriggers), nameof(AutotextTriggers.HookProc)); //does not make sense
							MouseTriggers.JitCompile();
							Util.Jit.Compile(typeof(ActionTrigger), nameof(ActionTrigger.MatchScopeWindowAndFunc));
							Util.Jit.Compile(typeof(Api), "WriteFile", "GetOverlappedResult");
							Util.Jit.Compile(typeof(TriggerHookContext), "InitContext", "PerfEnd", "PerfWarn");
						}
					}
					catch(Exception ex) { Debug_.Print(ex); }
				});
			}
			//Perf.Next();

			Wnd wMsg = default;
			bool hooksInEditor = AuTask.Role != ATRole.ExeProgram;
			if(hooksInEditor) {
				//SHOULDDO: pass wMsg when starting task.
				wMsg = Wnd.Misc.FindMessageOnlyWindow(null, "Au.Hooks.Server");
				if(wMsg.Is0) {
					Debug_.Print("Au.Hooks.Server");
					hooksInEditor = false;
				} else {
					//if this process is admin, and editor isn't, useEditor=false.
					var u = Uac.OfProcess(wMsg.ProcessId);
					if(u != null && u.IntegrityLevel < UacIL.UIAccess && Uac.OfThisProcess.IntegrityLevel >= UacIL.UIAccess) hooksInEditor = false;
				}
			}
			if(!hooksInEditor) {
				lock("8xBteR5Pp0y/uM63gGQuhw") {
					if(HooksServer.Instance == null) HooksServer.Start(true);
				}
				wMsg = HooksServer.Instance.MsgWnd;
			}

			int threadId = Api.GetCurrentThreadId();
			var pipe = Api.CreateNamedPipe(LibPipeName(threadId),
				Api.PIPE_ACCESS_DUPLEX | Api.FILE_FLAG_OVERLAPPED,
				Api.PIPE_TYPE_MESSAGE | Api.PIPE_READMODE_MESSAGE | Api.PIPE_REJECT_REMOTE_CLIENTS,
				1, 0, 0, 0, Api.SECURITY_ATTRIBUTES.ForPipes);
			if(pipe.IsInvalid) throw new AuException(0, "*CreateNamedPipe");

			var aCDS = new byte[8];
			aCDS.WriteInt_((int)usedEvents, 0);
			aCDS.WriteInt_(Api.GetCurrentProcessId(), 4);
			if(1 != Wnd.Misc.CopyDataStruct.SendBytes(wMsg, 1, aCDS, threadId)) { //install hooks and start sending events to us
				pipe.Dispose();
				throw new AuException("*SendBytes");
			}

			var evHooks = Api.CreateEvent(true);
			const int bLen = 100; byte* b = stackalloc byte[bLen]; //buffer for ReadFile
			try {
				_StartStop(true);
				var thc = new TriggerHookContext(this);
				var ha = stackalloc IntPtr[2] { evHooks, _evStop };

				//GC.Collect(); //if adding triggers and scopes creates much garbage, eg if is used StackTrace or StckFrame
				//Perf.NW('T');

				while(true) {
					var o = new Api.OVERLAPPED { hEvent = evHooks };
					if(!Api.ReadFile(pipe, b, bLen, out int size, &o)) {
						int ec = WinError.Code;
						if(ec == Api.ERROR_IO_PENDING) {
							//note: while waiting here, can be called acc hook proc, timer etc (any posted and sent messages).
							if(0 != _Wait(ha, 2)) {
								Api.CancelIo(pipe);
								break;
							}
							ec = Api.GetOverlappedResult(pipe, ref o, out size, false) ? 0 : WinError.Code;
						}
						if(ec != 0) { Debug_.LibPrintNativeError(ec); break; }
					}

					bool eat = false;
					thc.InitContext();
					if(size == sizeof(Api.KBDLLHOOKSTRUCT)) {
						//Print("key");
						var k = new HookData.Keyboard(null, b);
						thc.InitMod(k);
						if(this[TriggerType.Hotkey] is HotkeyTriggers tk) { //if not null
							eat = tk.HookProc(k, thc);
						}
						if(!eat /*&& thc.trigger == null*/ && this[TriggerType.Autotext] is AutotextTriggers ta) {
							ta.HookProc(k, thc);
						}
					} else if(size == sizeof(Api.MSLLHOOKSTRUCT2)) {
						var m = (Api.MSLLHOOKSTRUCT2*)b;
						//Print((uint)m->message);
						if(this[TriggerType.Mouse] is MouseTriggers tm) {
							var k = new HookData.Mouse(null, m->message, b);
							eat = tm.HookProcClickWheel(k, thc);
						}
					} else {
						//Print("edge/move");
						if(this[TriggerType.Mouse] is MouseTriggers tm) {
							var m = (MouseTriggers.LibEdgeMoveDetector.Result*)b;
							tm.HookProcEdgeMove(*m, thc);
						}
					}
					Perf.Next();
					thc.PerfWarn();
					//Perf.NW();//TODO

					//var mem = GC.GetTotalMemory(false);
					//if(mem != _debugMem && _debugMem != 0) Print(mem - _debugMem);
					//_debugMem = mem;

					Api.WriteFile(pipe, &eat, 1, out _);
					if(thc.trigger != null) LibRunAction(thc.trigger, thc.args);
				}
			}
			finally {
				pipe.Dispose();
				Api.CloseHandle(evHooks);
				wMsg.Send(Api.WM_USER, 1, threadId); //stop sending hook events to us
				_StartStop(false);
			}
		}

		//long _debugMem;

		internal void LibRunAction(ActionTrigger trigger, TriggerArgs args)
		{
			if(trigger.action != null) {
				if(_threads == null) _threads = new TriggerActionThreads();
				_threads.Run(trigger, args);
			}
		}

		void _StartStop(bool start)
		{
			if(start) {
				_evStop = Api.CreateEvent(false);
			} else {
				Stopping?.Invoke(this, EventArgs.Empty);
				Api.CloseHandle(_evStop);
				_evStop = default;
			}
			foreach(var t in _t) {
				if(t == null || !t.HasTriggers) continue;
				t.StartStop(start);
			}
		}

		/// <summary>
		/// Stops watching for trigger events and causes <see cref="Run"/> to return.
		/// </summary>
		/// <remarks>
		/// Does not abort threads of trigger actions that are still running.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Hotkey["Ctrl+T"] = o => Print("Ctrl+T");
		/// Triggers.Hotkey["Ctrl+Q"] = o => { Print("Ctrl+Q (stop)"); Triggers.Stop(); };
		/// Triggers.Hotkey.Last.EnabledAlways = true;
		/// Triggers.Run();
		/// Print("stopped");
		/// ]]></code>
		/// </example>
		public void Stop()
		{
			Api.SetEvent(_evStop);
		}
		IntPtr _evStop;

		/// <summary>
		/// Occurs before <see cref="Run">Triggers.Run</see> stops trigger engines and returns. Runs in its thread.
		/// </summary>
		public event EventHandler Stopping;

		/// <summary>
		/// True if executing <see cref="Run"/>.
		/// </summary>
		internal bool LibRunning => _evStop != default;

		/// <summary>
		/// Throws InvalidOperationException if executing <see cref="Run"/>.
		/// </summary>
		internal void LibThrowIfRunning() { if(LibRunning) throw new InvalidOperationException("Must be before or after Triggers.Run."); }

		/// <summary>
		/// Throws InvalidOperationException if not executing <see cref="Run"/>.
		/// </summary>
		internal void LibThrowIfNotRunning() { if(!LibRunning) throw new InvalidOperationException("Cannot be before or after Triggers.Run."); }

		/// <summary>
		/// Gets or sets whether triggers of this <see cref="ActionTriggers"/> instance are disabled.
		/// </summary>
		/// <remarks>
		/// Does not depend on <see cref="DisabledEverywhere"/>.
		/// Does not end/pause threads of trigger actions.
		/// </remarks>
		/// <seealso cref="ActionTrigger.EnabledAlways"/>
		/// <seealso cref="TriggerOptions.EnabledAlways"/>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Hotkey["Ctrl+T"] = o => Print("Ctrl+T");
		/// Triggers.Hotkey["Ctrl+D"] = o => { Print("Ctrl+D (disable/enable)"); Triggers.Disabled ^= true; }; //toggle
		/// Triggers.Hotkey.Last.EnabledAlways = true;
		/// Triggers.Run();
		/// ]]></code>
		/// </example>
		public bool Disabled { get; set; }

		/// <summary>
		/// true if triggers are disabled in all processes that use this library in this user session.
		/// </summary>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		/// <seealso cref="Disabled"/>
		/// <seealso cref="TriggerOptions.EnabledAlways"/>
		public static unsafe bool DisabledEverywhere {
			get => Util.LibSharedMemory.Ptr->triggers.disabled;
			set => Util.LibSharedMemory.Ptr->triggers.disabled = value;
		}

		[StructLayout(LayoutKind.Sequential, Size = 16)] //note: this struct is in shared memory. Size must be same in all library versions.
		internal struct LibSharedMemoryData
		{
			public bool disabled;
			public bool resetAutotext;
		}

		internal static string LibPipeName(int threadId) => @"\\.\pipe\Au.Triggers-" + threadId.ToString();
	}

	enum TriggerType
	{
		Hotkey,
		Autotext,
		Mouse,
		Window,

		Count,

		//TimerAfter,
		//TimerEvery,
		//TimerAt,
	}

	interface ITriggers
	{
		/// <summary>
		/// Return true if added triggers of this type.
		/// </summary>
		bool HasTriggers { get; }

		/// <summary>
		/// Optionally start/stop trigger engine (hooks etc).
		/// </summary>
		/// <param name="start"></param>
		void StartStop(bool start);
	}


	class TriggerHookContext : WFCache
	{
		//internal readonly ActionTriggers triggers;
		Wnd _w;
		bool _haveWnd, _mouseWnd; POINT _p;

		public TriggerHookContext(ActionTriggers triggers)
		{
			//this.triggers = triggers;
			_perfList = new _ScopeTime[32];
			_perfHookTimeout = Util.WinHook.LowLevelHooksTimeout;
			base.CacheName = true; //we'll call Clear(onlyName: true) at the start of each event
		}

		public Wnd Window {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				if(!_haveWnd) {
					_haveWnd = true;
					_w = _mouseWnd ? Wnd.FromXY(_p, WXYFlags.NeedWindow) : Wnd.Active;
				}
				return _w;
			}
		}

		/// <summary>
		/// Trigger/action to run. Set by a hook proc of a trigger engine.
		/// </summary>
		public ActionTrigger trigger;

		/// <summary>
		/// Used with <see cref="trigger"/>.
		/// </summary>
		public TriggerArgs args;

		///// <summary>
		///// This event was processed (not ignored). Set by a hook proc of a trigger engine.
		///// </summary>
		//public bool processed;

		/// <summary>
		/// Called before processing each hook event. Clears most properties and fields.
		/// </summary>
		public void InitContext()
		{
			Perf.First();//TODO

			_w = default; _haveWnd = _mouseWnd = false;
			base.Clear(onlyName: true);
			trigger = null; args = null;
			_perfLen = 0;
		}

		/// <summary>
		/// Tells to get window (for scope) from the specified point. If not called, will use the active window. In any case, gets window on demand.
		/// </summary>
		public void UseWndFromPoint(POINT p)
		{
			_mouseWnd = true; _p = p;
		}

		struct _ScopeTime
		{
			public int time, avgTime;
		}

		long _perfTime;
		_ScopeTime[] _perfList; //don't use List<> because its JIT is too slow in time-critical code
		int _perfLen;
		int _perfHookTimeout;

		public void PerfStart()
		{
			_perfTime = Time.PerfMicroseconds;
		}

		public void PerfEnd(bool isFunc, ref int perfTime)
		{
			long tLong = Time.PerfMicroseconds - _perfTime;
			int time = (int)Math.Min(tLong, 1_000_000_000);

			//calc average time of this scope. Assume the first time is 0.
			if(perfTime != 0) perfTime = Math.Max(1, (int)(((long)perfTime * 7 + time) / 8));
			//Print($"time={time}, avg={perfTime}");

			if(isFunc) time |= unchecked((int)0x80000000);
			if(_perfLen == _perfList.Length) Array.Resize(ref _perfList, _perfList.Length * 2);
			_perfList[_perfLen++] = new _ScopeTime { time = time, avgTime = perfTime };
			if(perfTime == 0) perfTime = 1;
		}

		public void PerfWarn()
		{
			if(_perfLen == 0) return;
			long ttTrue = 0, ttCompare = 0;
			for(int i = 0; i < _perfLen; i++) {
				var v = _perfList[i];
				int t = v.time & 0x7fffffff, ta;
				ttTrue += t;
				if(v.avgTime == 0) ta = Math.Max(t - 150_000, 0); //first time. Can be slow JIT and assembly loading.
				else ta = Math.Min(t, v.avgTime);
				ttCompare += ta;
			}
			ttCompare /= 1000; ttTrue /= 1000;
			//Print(ttTrue, ttCompare);
			if(ttCompare <= 25 && (ttTrue < 200 || ttTrue < _perfHookTimeout - 100)) return;
			var b = new StringBuilder();
			b.AppendFormat("<>Warning: Too slow trigger scope detection (Triggers.Of or Triggers.FuncOf). Time: {0} ms. Task name: {1}. <fold>", ttTrue, AuTask.Name);
			for(int i = 0; i < _perfLen; i++) {
				var v = _perfList[i];
				int t = v.time & 0x7fffffff;
				b.AppendFormat("\t{0} {1,8}.{2:D3} ms", v.time < 0 ? "F" : "W", t / 1000, t % 1000);
				if(v.avgTime > 0) b.AppendFormat(", average {0}.{1:D3} ms", v.avgTime / 1000, v.avgTime % 1000);
				b.AppendLine();
			}
			b.Append("* W - Triggers.Of (window); F - Triggers.FuncOf.</fold>");
			ThreadPool.QueueUserWorkItem(s1 => Print(s1), b.ToString()); //4 ms first time. Async because Print JIT slow.
		}

		/// <summary>
		/// Currently pressed modifier keys. Valid only in hotkey and autotext triggers.
		/// </summary>
		public KMod Mod => _mod;

		/// <summary>
		/// Currently pressed left-side modifier keys. Valid only in hotkey and autotext triggers.
		/// </summary>
		public KMod ModL => _modL;

		/// <summary>
		/// Currently pressed right-side modifier keys. Valid only in hotkey and autotext triggers.
		/// </summary>
		public KMod ModR => _modR;

		/// <summary>
		/// Not 0 if this key event is a modifier key. Valid only in hotkey and autotext triggers.
		/// </summary>
		public KMod ModThis => _modThis;

		KMod _mod, _modL, _modR, _modThis;
		long _lastKeyTime;

		/// <summary>
		/// Called before processing each keyboard hook event.
		/// Updates Mod, ModL, ModR, IsThisKeyMod. They are used by hotkey and autotext triggers.
		/// </summary>
		public void InitMod(HookData.Keyboard k)
		{
			KMod modL = 0, modR = 0;
			switch(k.vkCode) {
			case KKey.LCtrl: modL = KMod.Ctrl; break;
			case KKey.LShift: modL = KMod.Shift; break;
			case KKey.LAlt: modL = KMod.Alt; break;
			case KKey.Win: modL = KMod.Win; break;
			case KKey.RCtrl: modR = KMod.Ctrl; break;
			case KKey.RShift: modR = KMod.Shift; break;
			case KKey.RAlt: modR = KMod.Alt; break;
			case KKey.RWin: modR = KMod.Win; break;
			}

			if((_modThis = (modL | modR)) != 0) {
				if(k.IsUp) {
					_modL &= ~modL; _modR &= ~modR;
				} else {
					_modL |= modL; _modR |= modR;
				}
				_mod = _modL | _modR;
			} else if(!k.IsUp) {
				//We cannot trust _mod, because hooks are unreliable. We may not receive some events because of hook timeout, other hooks, OS quirks, etc. Also triggers may start while a modifier key is pressed.
				//And we cannot use Keyb.IsPressed, because our triggers release modifiers. Also Key() etc. Then triggers could not be auto-repeated.
				//We use both. If IsPressed(mod), add mod to _mod. Else remove from _mod after >5 s since the last seen key event. The max auto-repeat delay that you can set in CP is ~1 s.
				TrigUtil.GetModLR(out modL, out modR);
				//Debug_.PrintIf(modL != _modL || modR != _modR, $"KEY={k.vkCode}    modL={modL}  _modL={_modL}    modR={modR}  _modR={_modR}"); //normally should be only when auto-repeating a trigger
				_modL |= modL; _modR |= modR;
				long time = Time.WinMilliseconds;
				if(time - _lastKeyTime > 5000) {
					_modL &= modL; _modR &= modR;
				}
				_mod = _modL | _modR;
				//Print(_mod, k.vkCode);
				_lastKeyTime = time;
			}
		}
	}

	//public static class TaskTrigger
	//{
	//	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	//	public class HotkeyAttribute : Attribute
	//	{
	//		public string Hotkey { get; }

	//		public HotkeyAttribute(string hotkey)
	//		{
	//			Hotkey = hotkey;
	//		}
	//	}

	//}
}
