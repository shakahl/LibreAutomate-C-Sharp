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

	public class Triggers
	{
		ITriggers[] _t;
		ITriggers this[ETriggerType e] => _t[(int)e];

		Triggers() => _Init();

		void _Init()
		{
			_t = new ITriggers[(int)ETriggerType.Count];
			_scopes = new TriggerScopes();
			_options = new TriggerOptions();
			_threads = null; //will create on demand
		}

		/// <summary>
		/// Gets the <see cref="Triggers"/> object of this thread.
		/// </summary>
		/// <remarks>
		/// Rarely used.
		/// </remarks>
		public static Triggers Instance => t_instance ?? (t_instance = new Triggers());
		[ThreadStatic] static Triggers t_instance;

		//public static TriggerScopes Of {
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

		//		return Instance._scopes;
		//	}
		//}
		//static bool _test;
		//static string _s1;

		public static TriggerScopes Of => Instance._scopes;
		TriggerScopes _scopes;
		internal TriggerScopes LibScopes => _scopes;

		/// <summary>
		/// Allows to set some options for multiple triggers and their actions.
		/// </summary>
		/// <remarks>
		/// More info and examples: <see cref="TriggerOptions"/>.
		/// </remarks>
		public static TriggerOptions Options => Instance._options;
		TriggerOptions _options;
		internal TriggerOptions LibOptions => _options;

		TriggerActionThreads _threads;

		ITriggers _Get(ETriggerType e)
		{
			int i = (int)e;
			var t = _t[i];
			if(t == null) {
				switch(e) {
				case ETriggerType.Hotkey: t = new HotkeyTriggers(this); break;
				case ETriggerType.Autotext: t = new AutotextTriggers(this); break;
				case ETriggerType.Mouse: t = new MouseTriggers(this); break;
				case ETriggerType.WindowActive: case ETriggerType.WindowVisible: t = new WindowTriggers(this, e); break;
				default: Debug.Assert(false); break;
				}
				_t[i] = t;
			}
			return t;
		}

		public static HotkeyTriggers Hotkey => Instance._Get(ETriggerType.Hotkey) as HotkeyTriggers;

		public static AutotextTriggers Autotext => Instance._Get(ETriggerType.Autotext) as AutotextTriggers;

		public static MouseTriggers Mouse => Instance._Get(ETriggerType.Mouse) as MouseTriggers;

		public static WindowTriggers WindowActive => Instance._Get(ETriggerType.WindowActive) as WindowTriggers;

		public static WindowTriggers WindowVisible => Instance._Get(ETriggerType.WindowVisible) as WindowTriggers;

		public static void Run() => Instance._Run();
		void _Run()
		{
			//AppDomain.CurrentDomain.AssemblyLoad += (object sender, AssemblyLoadEventArgs args) => {
			//	var a = args.LoadedAssembly;
			//	Print(a);
			//	//if(a.FullName.StartsWith_("System.Drawing")) {
			//	//	//Print(new StackTrace());
			//	//	Debugger.Launch();
			//	//}
			//};

			bool haveTriggers = false; int llHooks = 0;
			for(int i = 0; i < _t.Length; i++) {
				var t = _t[i];
				if(t == null || !t.HasTriggers) continue;
				haveTriggers = true;
				switch((ETriggerType)i) {
				case ETriggerType.Hotkey: case ETriggerType.Autotext: llHooks |= 1; break;
				case ETriggerType.Mouse: llHooks |= 2; break;
				}
			}
			//Print(haveTriggers, (uint)llHooks);
			if(!haveTriggers) return;

			try {
				if(llHooks != 0) {
					_RunWithHooksServer(llHooks);
				} else {
					_RunSimple();
				}
			}
			finally {
				_threads?.Dispose(); _threads = null;
			}
		}

		void _RunSimple()
		{
			try {
				_StartStop(true);
				WaitFor.LibWait(Timeout.Infinite, WHFlags.DoEvents, _evStop);
			}
			finally {
				_StartStop(false);
			}
		}

		unsafe void _RunWithHooksServer(int llHooks)
		{
			//Perf.Next();

			//prevent big delay later on first LL hook event while hook proc waits
			if(_scopes.HasScopes) {
				ThreadPool.QueueUserWorkItem(_ => { //never mind: should do it once. Several Triggers.Run in task is rare. Fast next time.
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
						Util.Jit.Compile(typeof(TriggerBase), "MatchScope");
						Util.Jit.Compile(typeof(Api), "WriteFile", "GetOverlappedResult");
					}
				});
			}
			//Perf.Next();

			Wnd wMsg = default;
			bool hooksInEditor = AuTask.Role != ATRole.ExeProgram;
			if(hooksInEditor) {
				//SHOULDDO: pass wMsg when starting task.
				wMsg = Wnd.Misc.FindMessageWindow(null, "Au.Hooks.Server");
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
			var pipe = Api.CreateNamedPipe(PipeName(threadId),
				Api.PIPE_ACCESS_DUPLEX | Api.FILE_FLAG_OVERLAPPED,
				Api.PIPE_TYPE_MESSAGE | Api.PIPE_READMODE_MESSAGE | Api.PIPE_REJECT_REMOTE_CLIENTS,
				1, 0, 0, 0, Api.SECURITY_ATTRIBUTES.ForPipes);
			if(pipe.IsInvalid) throw new AuException(0, "*CreateNamedPipe");

			var aCDS = new byte[8];
			aCDS.WriteInt_(llHooks, 0);
			aCDS.WriteInt_(Api.GetCurrentProcessId(), 4);
			if(1 != Wnd.Misc.CopyDataStruct.SendBytes(wMsg, 1, aCDS, threadId)) { //install hooks and start sending events to this thread
				pipe.Dispose();
				throw new AuException("*SendBytes");
			}

			var evHooks = Api.CreateEvent(true);
			const int bLen = 100; byte* b = stackalloc byte[bLen]; //buffer for ReadFile
			try {
				_StartStop(true);
				var thc = new TriggerHookContext();
				var ha = new IntPtr[2] { evHooks, _evStop };

				//GC.Collect(); //if adding triggers and scopes creates much garbage, eg if is used StackTrace or StckFrame
				//Perf.NW('T');

				while(true) {
					var o = new Api.OVERLAPPED { hEvent = evHooks };
					if(!Api.ReadFile(pipe, b, bLen, out int size, &o)) {
						int ec = Native.GetError();
						if(ec == Api.ERROR_IO_PENDING) {
							//note: while waiting here, can be called acc hook proc, timer etc (any posted and sent messages).
							if(0 != WaitFor.LibWait(Timeout.Infinite, WHFlags.DoEvents, ha)) { //with WaitHandle.WaitAny cannot use MTA thread, timers, etc
								Api.CancelIo(pipe);
								break;
							}
							ec = Api.GetOverlappedResult(pipe, ref o, out size, false) ? 0 : Native.GetError();
						}
						if(ec != 0) { Debug_.LibPrintNativeError(ec); break; }
					}

					bool suppressInputEvent = false;
					thc.InitContext();
					if(size == sizeof(Api.KBDLLHOOKSTRUCT)) {
						var k = new HookData.Keyboard(null, b);
						if(this[ETriggerType.Hotkey] is HotkeyTriggers t1)
							suppressInputEvent = t1.HookProc(k, thc);
						if(!suppressInputEvent /*&& thc.trigger == null*/ && this[ETriggerType.Autotext] is AutotextTriggers t2)
							suppressInputEvent = t2.HookProc(k, thc);
					} else {
						var m = (Api.MSLLHOOKSTRUCT2*)b;
						var k = new HookData.Mouse(null, m->message, b);
						var t1 = this[ETriggerType.Mouse] as MouseTriggers;
						suppressInputEvent = t1.HookProc(k, thc);
					}
					Perf.Next();
					thc.PerfWarn();
					Perf.NW();//TODO
					Api.WriteFile(pipe, &suppressInputEvent, 1, out _);
					if(thc.trigger?.action != null) {
						if(_threads == null) _threads = new TriggerActionThreads();
						_threads.Run(thc.trigger, thc.args);
					}
				}
			}
			finally {
				pipe.Dispose();
				Api.CloseHandle(evHooks);
				_StartStop(false);
				wMsg.Send(Api.WM_USER, 1, threadId); //stop sending hook events to this thread
			}
		}

		void _StartStop(bool start)
		{
			if(start) _evStop = Api.CreateEvent(false); else { Api.CloseHandle(_evStop); _evStop = default; }
			foreach(var t in _t) {
				if(t == null || !t.HasTriggers) continue;
				t.StartStop(start);
			}
		}

		/// <summary>
		/// Stops watching for trigger events and causes <see cref="Run"/> to return.
		/// </summary>
		/// <remarks>
		/// Rarely used.
		/// Does not abort unfinished threads of trigger actions.
		/// <note type="note">Don't call from a trigger action like this: <c>Triggers.Instance.Stop();</c>. The <b>Instance</b> then is of wrong thread. Instead call this in the main thread: <c>var ti=Triggers.Instance;</c> and then call this in a trigger action: <c>ti.Stop();</c>.</note>
		/// </remarks>
		public void Stop()
		{
			if(_evStop != default) Api.SetEvent(_evStop);
		}
		IntPtr _evStop;

		/// <summary>
		/// Clears all added triggers, scopes (<b>Triggers.Of</b>), options, etc.
		/// </summary>
		/// <remarks>
		/// Rarely used.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// Called from another thread. For example from a trigger action.
		/// Or called while in <see cref="Run"/>. Call <see cref="Stop"/> at first; then call this function after <b>Run</b> returns.
		/// </exception>
		public void Clear()
		{
			if(this != t_instance || _evStop != default) throw new InvalidOperationException();
			_Init();
		}

		internal static string PipeName(int threadId) => @"\\.\pipe\Au.Triggers-" + threadId.ToString();
	}

	enum ETriggerType
	{
		Hotkey,
		Autotext,
		Mouse,
		WindowActive,
		WindowVisible,

		Count,

		//future. Move above ServerCount.

		TimerAfter,
		TimerEvery,
		TimerAt,
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

	/// <summary>
	/// Base of trigger action argument classes of all trigger types.
	/// </summary>
	public class TriggerArgs
	{

	}


	class TriggerHookContext : WFCache
	{
		Wnd _w;
		bool _haveWnd, _mouseWnd; POINT _p;

		public TriggerHookContext()
		{
			_perfList = new _ScopeTime[32];
			_perfHookTimeout = Util.WinHook.LowLevelHooksTimeout;
		}

		public Wnd w {
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
		public TriggerBase trigger;

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
			Clear(onlyName: true);
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

		public void PerfEnd(TriggerScopeBase scope)
		{
			long tLong = Time.PerfMicroseconds - _perfTime;
			int time = (int)Math.Min(tLong, 1_000_000_000);

			//calc average time of this scope. Assume the first time is 0.
			if(scope.perfTime != 0) scope.perfTime = Math.Max(1, (int)(((long)scope.perfTime * 7 + time) / 8));
			//Print($"time={time}, avg={scope.perfTime}");

			if(scope is TriggerScopeFunc) time |= unchecked((int)0x80000000);
			if(_perfLen == _perfList.Length) Array.Resize(ref _perfList, _perfList.Length * 2); //TODO: test
			_perfList[_perfLen++] = new _ScopeTime { time = time, avgTime = scope.perfTime };
			if(scope.perfTime == 0) scope.perfTime = 1;
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
			b.AppendFormat("<>Warning: Too slow trigger scope detection (Triggers.Of). Time: {0} ms. Task name: {1}. <fold>", ttTrue, AuTask.Name);
			for(int i = 0; i < _perfLen; i++) {
				var v = _perfList[i];
				int t = v.time & 0x7fffffff;
				b.AppendFormat("\t{0} {1,8}.{2:D3} ms", v.time < 0 ? "F" : "W", t / 1000, t % 1000);
				if(v.avgTime > 0) b.AppendFormat(", average {0}.{1:D3} ms", v.avgTime / 1000, v.avgTime % 1000);
				b.AppendLine();
			}
			b.Append("* W - Triggers.Of.Window or similar; F - Triggers.Of.Func or similar.</fold>");
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
