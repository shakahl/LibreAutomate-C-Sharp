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

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	/// <summary>
	/// Triggers.
	/// </summary>
	[CompilerGenerated()]
	class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	public class AuTriggers
	{
		readonly ITriggers[] _t;
		ITriggers this[TriggerType e] => _t[(int)e];

		public AuTriggers()
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

		public TriggerScopes Of => scopes;
		internal readonly TriggerScopes scopes;

		public TriggerFuncs FuncOf => funcs;
		internal readonly TriggerFuncs funcs;

		/// <summary>
		/// Allows to set some options for multiple triggers and their actions.
		/// </summary>
		/// <remarks>
		/// More info and examples: <see cref="TriggerOptions"/>.
		/// </remarks>
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

		public HotkeyTriggers Hotkey => _Get(TriggerType.Hotkey) as HotkeyTriggers;

		public AutotextTriggers Autotext => _Get(TriggerType.Autotext) as AutotextTriggers;

		public MouseTriggers Mouse => _Get(TriggerType.Mouse) as MouseTriggers;

		public WindowTriggers Window => _Get(TriggerType.Window) as WindowTriggers;

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

			bool haveTriggers = false; int llHooks = 0;
			_windowTriggers = null;
			for(int i = 0; i < _t.Length; i++) {
				var t = _t[i];
				if(t == null || !t.HasTriggers) continue;
				haveTriggers = true;
				switch((TriggerType)i) {
				case TriggerType.Hotkey: case TriggerType.Autotext: llHooks |= 1; break;
				case TriggerType.Mouse: llHooks |= 2; break;
				case TriggerType.Window: _windowTriggers = t as WindowTriggers; break;
				}
			}
			//Print(haveTriggers, (uint)llHooks);
			if(!haveTriggers) return;
			_winTimerPeriod = 0;
			_winTimerLastTime = 0;

			try {
				_threadId = Thread_.NativeId;
				if(llHooks != 0) {
					_RunWithHooksServer(llHooks);
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

		unsafe void _RunWithHooksServer(int llHooks)
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
							Util.Jit.Compile(typeof(TriggerHookContext), "InitContext", "PerfEnd", "PerfWarn");
							Util.Jit.Compile(typeof(HotkeyTriggers), "HookProc");
							Util.Jit.Compile(typeof(AutotextTriggers), "HookProc");
							Util.Jit.Compile(typeof(MouseTriggers), "HookProc");
							Util.Jit.Compile(typeof(Trigger), "MatchScopeWindowAndFunc");
							Util.Jit.Compile(typeof(Api), "WriteFile", "GetOverlappedResult");
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
			aCDS.WriteInt_(llHooks, 0);
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

					bool suppressInputEvent = false;
					thc.InitContext();
					if(size == sizeof(Api.KBDLLHOOKSTRUCT)) {
						var k = new HookData.Keyboard(null, b);
						if(this[TriggerType.Hotkey] is HotkeyTriggers t1)
							suppressInputEvent = t1.HookProc(k, thc);
						if(!suppressInputEvent /*&& thc.trigger == null*/ && this[TriggerType.Autotext] is AutotextTriggers t2)
							suppressInputEvent = t2.HookProc(k, thc);
					} else {
						var m = (Api.MSLLHOOKSTRUCT2*)b;
						var k = new HookData.Mouse(null, m->message, b);
						var t1 = this[TriggerType.Mouse] as MouseTriggers;
						suppressInputEvent = t1.HookProc(k, thc);
					}
					Perf.Next();
					thc.PerfWarn();
					//Perf.NW();//TODO
					Api.WriteFile(pipe, &suppressInputEvent, 1, out _);
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

		internal void LibRunAction(Trigger trigger, TriggerArgs args)
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
		/// Gets or sets whether triggers of this <see cref="AuTriggers"/> instance are disabled.
		/// </summary>
		/// <remarks>
		/// Does not depend on <see cref="DisabledEverywhere"/>.
		/// Does not end/pause threads of trigger actions.
		/// </remarks>
		/// <seealso cref="Trigger.EnabledAlways"/>
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
		/// true if triggers are disabled in all proceses that use this library in this user session.
		/// </summary>
		/// <seealso cref="Disabled"/>
		/// <seealso cref="TriggerOptions.EnabledAlways"/>
		public static unsafe bool DisabledEverywhere {
			get => Util.LibSharedMemory.Ptr->triggers.disabled;
			internal set => Util.LibSharedMemory.Ptr->triggers.disabled = value;
		}

		[StructLayout(LayoutKind.Sequential, Size = 16)] //note: this struct is in shared memory. Size must be same in all library versions.
		internal struct LibSharedMemoryData
		{
			public bool disabled;
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
		//internal readonly AuTriggers triggers;
		Wnd _w;
		bool _haveWnd, _mouseWnd; POINT _p;

		public TriggerHookContext(AuTriggers triggers)
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
		public Trigger trigger;

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
	}


	public static class TaskTrigger
	{
		[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
		public class HotkeyAttribute : Attribute
		{
			public string Hotkey { get; }

			public HotkeyAttribute(string hotkey)
			{
				Hotkey = hotkey;
			}
		}

	}
}
