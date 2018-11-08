//#define STANDARD_SCRIPT
//#define TEST_STARTUP_SPEED
//#define PRELOAD_PROCESS //not impl

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au.LibRun
{
	/// <summary>
	/// Used by other Au projects to execute script assembly in new appdomain etc.
	/// The code must be in Au, to avoid loading the program assembly into the new appdomain. Now Au is loaded instead.
	/// </summary>
	internal unsafe class RunAsm
	{
		Thread _thread;
		AutoResetEvent _eventStart;
		AuTask _task;
		string _name, _asmFile;
		int _pdbOffset;
		string[] _args;
		bool _hasConfig;
		volatile bool _ready, _hurry;

		static RunAsm s_ra;
		static Timer_ s_timerPrepareNextAD;
#if TEST_STARTUP_SPEED
		Perf.Inst _perfStartup, _perfPrepare;
#endif

		RunAsm(bool hurry, bool mtaThread)
		{
#if TEST_STARTUP_SPEED
			_perfPrepare.First();
#endif
			_hurry = hurry;
			_eventStart = new AutoResetEvent(false);
			_thread = new Thread(() => {
				var ty = typeof(_DomainManager);
				var auAsmName = ty.Assembly.FullName;
				AppDomainSetup se = new AppDomainSetup { AppDomainManagerAssembly = auAsmName, AppDomainManagerType = ty.FullName };

				var ad = AppDomain.CreateDomain("Au.Script", null, se);
				//ad.UnhandledException += (_, e) => Print(e.ExceptionObject); //no. We instead use AuAppBase class, then it also works when script runs as normal exe, ie not in a host process.
#if TEST_STARTUP_SPEED
				_perfPrepare.Next();
#endif
				bool taskStarted = false, aborted = false;
				try {
					var v = ad.CreateInstanceAndUnwrap(auAsmName, typeof(_ADRun).FullName) as _ADRun;
					if(!_hurry) v.PrepareOptional();
					//if(!_hurry) v.PrepareOptional2();
					_ready = true;
#if TEST_STARTUP_SPEED
					_perfPrepare.NW('P');
#endif
					_eventStart.WaitOne();
					_eventStart.Dispose();

					_task.LibThreadStarted(_name);
					taskStarted = true;

					v.RunInAD(_name, _asmFile, _pdbOffset, _args, _hasConfig);
				}
				catch(ThreadAbortException) {
					//Print("ThreadAbortException");
					aborted = true;
				}
				catch(Exception e) /*when(!(e is ThreadAbortException))*/ {
					Print(e.ToStringWithoutStack_());
				}
				finally {
					//SHOULDDO: need something better. Should wait for foreground threads, etc. But it seems .NET does not support it.
					for(int i = 0; ; i = 1) {
						long t0 = Time.Milliseconds;
						try { AppDomain.Unload(ad); }
						catch(CannotUnloadAppDomainException) { //normally thrown after 10 s
							if(i == 0 && !aborted) Print($"<>Cannot unload AppDomain '{_name}'. Probably some of its threads cannot be aborted now because is executing unmanaged code. Tip: add meta option <c green>isolation process<>.");
							if(Time.Milliseconds - t0 > 2500) continue;
						}
						catch(Exception e2) { Print(e2.ToStringWithoutStack_()); }
						break;
					}
					if(taskStarted) _task.LibTaskEnded();
				}
#if TEST_STARTUP_SPEED
				_perfStartup.NW('S');
				//GC.Collect();
#endif
			});
			AuTask.LibThreadStart(_thread, mtaThread);
		}

		void _Start(AuTask task, string name, string asmFile, int pdbOffset, string[] args, bool hasConfig)
		{
#if TEST_STARTUP_SPEED
			_perfStartup.First();
#endif
			_task = task;
			_name = name; _asmFile = asmFile; _pdbOffset = pdbOffset; _args = args; _hasConfig = hasConfig;
			_eventStart.Set();
		}

		/// <summary>
		/// Does not allow to start tasks more frequently than we can prepare appdomains.
		/// </summary>
		void _WaitReady()
		{
			_hurry = true;
			while(!_ready && _thread.IsAlive) Thread.Sleep(15); //note: don't use an event object. This code is perfect for what we need.
		}

		class _ADRun :MarshalByRefObject
		{
			//Prevents destroying the object after 5 minutes. Then we would get RemotingException: "Object has been disconnected...".
			public override object InitializeLifetimeService() => null;

			public void PrepareOptional()
			{
				RuntimeHelpers.PrepareMethod(typeof(_ADRun).GetMethod(nameof(RunInAD)).MethodHandle);
				RuntimeHelpers.PrepareMethod(typeof(RunAsm).GetMethod(nameof(LibLoadAssembly), BindingFlags.Static | BindingFlags.NonPublic).MethodHandle);
				RuntimeHelpers.PrepareMethod(typeof(RunAsm).GetMethod(nameof(_Run), BindingFlags.Static | BindingFlags.NonPublic).MethodHandle);
#if TEST_STARTUP_SPEED
				_ = Time.Microseconds; //JIT
#endif

				//load some .NET assemblies into this appdomain, it is very slow.
				//	Initially we have only mscorlib and Au (this assembly).
				//	note: actually loads these assemblies when JIT-compiling this method.
				//	note: .NET serializes loading of assemblies. If one thread is loading, other thread waits.
				_ = typeof(Stopwatch).Assembly; //System, +17 ms
				_ = typeof(System.Linq.Enumerable).Assembly; //System.Core, +18 ms

				//_ = typeof(System.Windows.Forms.Control).Assembly; //+55 ms
				//ThreadPool.QueueUserWorkItem(o => { _ = typeof(System.Windows.Forms.Control).Assembly; });

				//foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) Print(v);
			}

			//rejected. Too slow, uses much memory. Most scripts don't need it.
			//public void PrepareOptional2()
			//{
			//	//_ = typeof(System.Windows.Forms.Control).Assembly; //+55 ms
			//	ThreadPool.QueueUserWorkItem(o => { _ = typeof(System.Windows.Forms.Control).Assembly; });
			//}

			public void RunInAD(string name, string asmFile, int pdbOffset, string[] args, bool hasConfig)
			{
				Script.Name = name; //info: I could not find a way to set AppDomain.FriendlyName after creating appdomain. In UI we use Script.Name instead.
				if(hasConfig) AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", asmFile + ".config");

				var asm = LibLoadAssembly(asmFile, pdbOffset, false);

				var ad = AppDomain.CurrentDomain;
				if(ad.DomainManager is _DomainManager dm) dm.LibEntryAssembly = asm;

				_Run(RIsolation.appDomain, asm, args, 0);
			}
		}

		//Sets EntryAssembly property for the new appdomain.
		class _DomainManager :AppDomainManager
		{
			public override Assembly EntryAssembly => LibEntryAssembly;

			internal Assembly LibEntryAssembly;
		}

		[HandleProcessCorruptedStateExceptions]
		static void _Run(RIsolation caller, Assembly asm, string[] args, RHFlags flags)
		{
			try {
				var entryPoint = asm.EntryPoint ?? throw new InvalidOperationException("assembly without entry point (function Main)");

				bool useArgs = entryPoint.GetParameters().Length != 0;
				if(useArgs) {
					if(args == null) args = Array.Empty<string>();
#if STANDARD_SCRIPT
				} else if(args != null) {
					//if standard script, set __script__.args. Our compiler adds class __script__ with static string[] args, and adds __script__ to usings.
					a.GetType("__script__")?.GetField("args", BindingFlags.SetField | BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, args);

					//if standard script, will need to run triggers in this func. Depends on how we'll implement them.
#endif
				}

				if(!useArgs) {
					entryPoint.Invoke(null, null);
				} else {
					entryPoint.Invoke(null, new object[] { args });
				}
			}
			catch(TargetInvocationException te) {
				var e = te.InnerException;
				if(0 != (flags & RHFlags.DontHandleExceptions)) throw e;
				//Print(e);
				AuAppBase.OnHostHandledException(new UnhandledExceptionEventArgs(e, false));
			}

			//see also: TaskScheduler.UnobservedTaskException event.
			//	tested: the event works.
			//	tested: somehow does not terminate process even with <ThrowUnobservedTaskExceptions enabled="true"/>.
			//		Only when AuDialog.Show called, the GC.Collect makes it to disappear but the process remains running.
			//	note: the terminating behavior also can be set in registry or env var. It overrides <ThrowUnobservedTaskExceptions enabled="false"/>.
		}

		/// <summary>
		/// Executes assembly in new appdomain in new thread.
		/// Handles exceptions.
		/// </summary>
		internal static Thread LibRunInAppDomain(AuTask task, RParams x)
		{
			Debug.Assert(Thread.CurrentThread.ManagedThreadId == 1);

			Thread R;
			if(x.Has(RFlags.mtaThread)) { //don't prepare. MTA threads ar rare. All scripts are STA. Apps are STA by default.
				var ra = new RunAsm(true, true);
				ra._WaitReady();
				ra._Start(task, x.name, x.asmFile, x.pdbOffset, x.args as string[], x.Has(RFlags.hasConfig));
				R = ra._thread;
			} else {
				if(s_ra == null) s_ra = new RunAsm(true, false);
				s_ra._WaitReady();
				s_ra._Start(task, x.name, x.asmFile, x.pdbOffset, x.args as string[], x.Has(RFlags.hasConfig));
				R = s_ra._thread;

				//prepare next AD after some time.
				//	Don't do it now, because may interfere with script. Eg if script loads assemblies simultaneously, it must wait and therefore starts slower.
				s_ra = null;
				if(x.Has(RFlags.remote)) {
					//in Au.Tasks process.
					//	Cannot use timer, and don't need. This thread waits for pipe connection and doesn't dispatch messages.
					//Thread.Sleep(50); //no, better don't sleep. Then the task often ends before editor knows that it is started. It's OK but I don't like it.
					s_ra = new RunAsm(false, false);
				} else {
					//in editor process.
					//	Could use the same code as above, but better use timer. For example if script uses Print, would display the text with a delay.
					if(s_timerPrepareNextAD == null) s_timerPrepareNextAD = new Timer_(() => {
						//Print("TIMER");
						s_ra = new RunAsm(false, false);
					});
					s_timerPrepareNextAD.Start(50, true);
				}
			}
			return R;

			//By default, appdomain startup is very slow, 30 ms or more, depending on used .NET assemblies. Loading each assembly takes > 15 ms.
			//Workaround: prepare appdomain and let it wait. When need to execute an assembly, just wake up it, load assembly and execute. It takes 2 ms.
		}

		/// <summary>
		/// Executes assembly in this appdomain in this thread. Must be main appdomain.
		/// Handles exceptions.
		/// </summary>
		/// <param name="caller">Must be hostThread or process.</param>
		/// <param name="asmFile">Full path of assembly file.</param>
		/// <param name="pdbOffset">0 or offset of portable PDB in file.</param>
		/// <param name="args">To pass to Main.</param>
		/// <param name="flags"></param>
		public static void RunHere(RIsolation caller, string asmFile, int pdbOffset, string[] args = null, RHFlags flags = 0)
		{
			Debug.Assert(caller == RIsolation.hostThread || caller == RIsolation.process);
			var asm = LibLoadAssembly(asmFile, pdbOffset, caller == RIsolation.hostThread);
			_Run(caller, asm, args, flags);
		}

		/// <summary>
		/// Internal. To run in main appdomain in nonmain thread.
		/// </summary>
		internal static void LibRunHere(Assembly asm, string[] args)
		{
			_Run(RIsolation.thread, asm, args, 0);
		}

		/// <summary>
		/// Loads script assembly or finds loaded.
		/// </summary>
		/// <param name="asmFile"></param>
		/// <param name="pdbOffset"></param>
		/// <param name="orFindLoaded">Don't load the same unchanged assembly multiple times into the same appdomain. Must be called in main thread.</param>
		/// <exception cref="Exception">Exceptions of FileStream and Assembly.Load.</exception>
		internal static Assembly LibLoadAssembly(string asmFile, int pdbOffset, bool orFindLoaded)
		{
			Debug.Assert(!orFindLoaded || (Thread.CurrentThread.ManagedThreadId == 1));

			Assembly asm = null;
			_LoadedScriptAssembly lsa = default;
			if(orFindLoaded) asm = lsa.Find(asmFile);

			if(asm == null) {
				//return Assembly.LoadFrom(asmFile); //test

				byte[] bAsm, bPdb = null;
				using(var stream = File_.WaitIfLocked(() => File.OpenRead(asmFile))) {
					bAsm = new byte[pdbOffset > 0 ? pdbOffset : stream.Length];
					stream.Read(bAsm, 0, bAsm.Length);
					try {
						if(pdbOffset > 0) {
							bPdb = new byte[stream.Length - pdbOffset];
							stream.Read(bPdb, 0, bPdb.Length);
						} else {
							var s1 = Path.ChangeExtension(asmFile, "pdb");
							if(File_.ExistsAsFile(s1)) bPdb = File.ReadAllBytes(s1);
						}
					}
					catch(Exception ex) { bPdb = null; Debug_.Print(ex); } //not very important
				}
				asm = Assembly.Load(bAsm, bPdb);
				if(orFindLoaded) lsa.Add(asmFile, asm);
			}
			return asm;

			//never mind: it's possible that we load newer compiled assembly version of script than intended.
		}

		/// <summary>
		/// Remembers and finds script assemblies loaded in this appdomain, to avoid loading the same unchanged assembly multiple times.
		/// Used when isolation is thread or hostThread.
		/// </summary>
		struct _LoadedScriptAssembly
		{
			class _Asm
			{
				public DateTime time;
				public Assembly asm;
			}

			static Dictionary<string, _Asm> _d;
			DateTime _fileTime;

			public Assembly Find(string asmFile)
			{
				if(_d == null) _d = new Dictionary<string, _Asm>(StringComparer.OrdinalIgnoreCase);
				if(!File_.GetProperties(asmFile, out var p, FAFlags.UseRawPath)) return null;
				_fileTime = p.LastWriteTimeUtc;
				if(_d.TryGetValue(asmFile, out var x)) {
					if(x.time == _fileTime) return x.asm;
					_d.Remove(asmFile);
				}
				return null;
			}

			public void Add(string asmFile, Assembly asm)
			{
				if(_fileTime == default) return; //File_.GetProperties failed
				_d.Add(asmFile, new _Asm { time = _fileTime, asm = asm });

				//foreach(var v in _d.Values) Print(v.asm.FullName);
				//foreach(var v in AppDomain.CurrentDomain.GetAssemblies()) Print(v.FullName);
			}
		}
	}

	/// <summary>
	/// Caller of RunAsm.RunHere etc.
	/// </summary>
	internal enum RIsolation
	{
		appDomain, process, thread, hostThread
	}

	/// <summary>
	/// Flags for RunAsm.RunHere.
	/// </summary>
	[Flags]
	internal enum RHFlags
	{
		DontHandleExceptions = 1,
	}

	/// <summary>
	/// RParams.flags.
	/// </summary>
	[Flags]
	internal enum RFlags
	{
		isProcess = 1, //isolation process
		isThread = 2, //isolation thread
		mtaThread = 4, //no [STAThread]
		hasConfig = 8, //has config file
		noUiAccess = 16, //start process simply, without inheriting uiAccess
		userProcess = 32, //start medium IL process from high IL process
		remote = 64, //runs in Au.Tasks.exe process, not in editor process
	}

	/// <summary>
	/// Parameters for AuTask.Run.
	/// </summary>
	internal class RParams
	{
		public string name, asmFile, exeFile;
		public object args; //string[] if appdomain|thread, string if process
		public int pdbOffset;
		public RFlags flags;

		public RParams() { }

		public RParams(string name, string asmFile, string exeFile, object args, int pdbOffset, RFlags flags)
		{
			this.name = name; this.asmFile = asmFile; this.exeFile = exeFile; this.args = args; this.pdbOffset = pdbOffset; this.flags = flags;
		}

		public bool Has(RFlags flag) => 0 != (flags & flag);
	}

	/// <summary>
	/// Contains properties of a running automation task that are used in editor and Au.Tasks.
	/// Starts task, notifies when it is ended, ends it when asked.
	/// </summary>
	internal class AuTask
	{
		public readonly int taskId;
		volatile int _nativeThreadId;
		volatile object _threadOrProcess;
		readonly IAuTaskManager _manager;

		/// <param name="taskId"></param>
		/// <param name="manager">We'll notify it when task ended.</param>
		public AuTask(int taskId, IAuTaskManager manager)
		{
			this.taskId = taskId;
			_manager = manager;
		}

		/// <summary>
		/// Starts task in new process, appdomain or thread.
		/// If returns true, the task is successfully started. However it can be already ended (unlikely); then IsRunning is false.
		/// </summary>
		/// <param name="x">Parameters to start the task.</param>
		public bool Run(RParams x)
		{
#if TEST_STARTUP_SPEED
			var t1 = Time.Microseconds.ToString();
			if(x.Has(RFlags.isProcess)) x.args = x.args as string + " " + t1;
			else x.args = new string[] { t1 };
			//Debug_.LibMemoryPrint(false);
#endif

			try {
				if(x.Has(RFlags.isProcess)) {
					Process p;
					var args = x.args as string;
					if(x.Has(RFlags.userProcess)) p = Process_.LibStartUserIL(x.exeFile, args, ret: Process_.EStartReturn.Process) as Process;
					else p = Process_.LibStart(x.exeFile, args, inheritUiaccess: !x.Has(RFlags.noUiAccess), ret: Process_.EStartReturn.Process) as Process;
					//TODO: actually Process is not necessary. Need just handle, and LibStartX can return WaitHandle.
					p.EnableRaisingEvents = true; //tested: does not throw when process already ended. But can throw for other reasons.

					p.Exited += (unu, sed) => LibTaskEnded(); //info: the event is in a threadpool thread and does not give the Process object
					if(0 == Api.WaitForSingleObject(p.Handle, 0)) { /*Debug_.Print("process ended");*/ return true; } //info: we don't know whether will be the Exited event, it's OK

					_threadOrProcess = p;
				} else if(x.Has(RFlags.isThread)) {
					var asm = RunAsm.LibLoadAssembly(x.asmFile, x.pdbOffset, true);

					var t = new Thread(() => {
						try {
							LibThreadStarted(x.name);
							RunAsm.LibRunHere(asm, x.args as string[]);
						}
						finally { LibTaskEnded(); }
					});
					LibThreadStart(t, x.Has(RFlags.mtaThread));

					_threadOrProcess = t;
				} else {
					_threadOrProcess = RunAsm.LibRunInAppDomain(this, x);
				}
			}
			catch(Exception ex) { Print(ex); return false; }

			return true;
		}

		/// <summary>
		/// If task started in other process, call this function instead of Run.
		/// </summary>
		/// <param name="admin"></param>
		public void SetTaskStartedInOtherProcess(bool admin)
		{
			_threadOrProcess = admin ? "a" : "u";
		}

		/// <summary>
		/// If task started in other process, returns true and gets whether it is admin process.
		/// </summary>
		/// <param name="admin"></param>
		public bool GetTaskStartedInOtherProcess(out bool admin)
		{
			if(_threadOrProcess is string s) { admin = s[0] == 'a'; return true; }
			admin = false; return false;
		}

		//Runs in main thread.
		internal static void LibThreadStart(Thread t, bool mtaThread)
		{
			t.IsBackground = true; //or on exit woud need to abort the waiting thread of prepared appdomain
			if(!mtaThread) t.SetApartmentState(ApartmentState.STA);
			t.Start();
		}

		//Runs in other thread.
		internal void LibThreadStarted(string taskName)
		{
			_nativeThreadId = Thread_.NativeId;
			Thread.CurrentThread.Name = "[script] " + taskName;
		}

		//Runs in other thread.
		internal void LibTaskEnded()
		{
			if(_threadOrProcess is Process p) p.Dispose();
			if(_threadOrProcess == null) {
				Thread.Sleep(10);
				if(_threadOrProcess == null) {
					Debug_.Print("LibTaskEnded called before _threadOrProcess is set. It's OK, but should be rare, mostly with 1-core CPU.");
					for(int i = 0; i < 10 && _threadOrProcess == null; i++) Thread.Sleep(15);
					//Not error if even now null. In editor process it is not null when the message arrives. In Au.Tasks process would not remove one dictionary item.
					Debug_.PrintIf(_threadOrProcess == null, "_threadOrProcess is null");
				}
			}
			_threadOrProcess = null; _nativeThreadId = 0;
			_manager.TaskEnded(taskId);
			var w = _manager.Window;
			if(!w.Is0) w.Post(WM_TASK_ENDED, taskId);
		}

		/// <summary>
		/// When task ended, this message is posted to the manager window, with wParam=taskId.
		/// </summary>
		public const int WM_TASK_ENDED = Api.WM_USER + 900;

		/// <summary>
		/// False if task is already ended or still not started.
		/// </summary>
		public bool IsRunning => _threadOrProcess != null;

		/// <summary>
		/// Ends this task (thread or process), if running.
		/// Returns true if ended now or was not running.
		/// </summary>
		/// <param name="onProgramExit">Terminate threads that cannot be ended normally.</param>
		/// <remarks>
		/// If it is process, kills it instantly.
		/// Else closes thread windows, calls Thread.Abort, etc; it may end thread now or later or never. Waits briefly, but does not throw exception etc if not ended.
		/// Don't call this func to end tasks started through other process. It can only end locally started task.
		/// </remarks>
		public bool End(bool onProgramExit)
		{
			switch(_threadOrProcess) {
			case Thread t:
				//Perf.First();
				int waitMS = onProgramExit ? 150 : 500;
				//try to close thread windows
				bool hasWindows = false;
				int tid = _nativeThreadId;
				if(tid != 0) {
					bool hasVisible = false;
					foreach(var w in Wnd.GetWnd.ThreadWindows(tid, sortFirstVisible: true)) {
						//Print(w);
						bool visible = w.IsVisible;
						if(visible) hasVisible = true; else if(hasVisible) break;
						hasWindows = true;
						w.Close(noWait: true);
						//never mind: cannot close dialogs etc that have no X button or it is disabled. Could post Enter key, but it is dangerous.
					}
					if(hasWindows) {
						if(t.Join(waitMS)) break;
						if(!onProgramExit) {
							hasWindows = false;
							var a = new List<Wnd>();
							foreach(var w in Wnd.GetWnd.ThreadWindows(tid)) {
								hasWindows = true;
								if(w.IsVisible) a.Add(w);
							}
							if(hasWindows) {
								_PrintFailed(t, "Its windows cannot be closed safely");
								if(a.Count != 0) {
									Print("\tVisible windows:");
									foreach(var w in a) Print("\t\t" + w.ToString());
								}
							}
						}
					}
				}
				//abort thread
				if(!hasWindows) { //too dangerous to abort thread with windows. Can kill process. Much lower possibility if no windows.
					Task.Run(() => t.Abort()); //Abort can wait
					if(t.Join(waitMS)) break;
					if(!onProgramExit) _PrintFailed(t, "Probably it is executing unmanaged code and will end upon returning to managed code");
				}
				//terminate thread
				if(onProgramExit && _nativeThreadId != 0) {
					try { t.IsBackground = true; } catch(Exception ex) { Debug_.Print(ex); }
					Thread_.LibTerminate(_nativeThreadId);
				}
				//Perf.NW();
				break;
			case Process p:
				try { p.Kill(); }
				catch(Exception ex) { Debug_.Print(ex); break; }
				//if(!onProgramExit) p.WaitForExit(1000); //bad
				//note: _threadOrProcess now probably still not null. If need, caller can wait for WM_TASK_ENDED.
				return true;
			case string _:
				Debug.Assert(false);
				break;
			}
			return _threadOrProcess == null;

			void _PrintFailed(Thread t, string s)
			{
				var name = t.Name; if(name != null && name.StartsWith_("[script] ")) name = name.Substring(9);
				Print($"<>Failed to end task '{name}'. {s}. Tips: to end task easily, run it in separate process: add meta option <c green>isolation process<>; to end all tasks now, exit this program.");
			}
		}
	}

	internal interface IAuTaskManager
	{
		/// <summary>
		/// Manager window. If not Is0, will post WM_TASK_ENDED to it.
		/// </summary>
		Wnd Window { get; }

		/// <summary>
		/// Called when ending thread (in that thread) and when ended process (in threadpool thread).
		/// </summary>
		/// <param name="taskId"></param>
		void TaskEnded(int taskId);
	}
}

namespace Au.Types
{
	/// <summary>
	/// This class is used in automation script/app files as base of their main class. Adds some features.
	/// </summary>
	/// <remarks>
	/// This class adds these features:
	/// 1. Static constructor subscribes to <see cref="AppDomain.UnhandledException"/> event. On unhandled exception prints exception info. Without this class not all unhandled exceptions would be printed.
	/// 2. Provides function OnUnhandledException. The script/app can override it.
	/// 
	/// More features may be added in the future.
	/// </remarks>
	public abstract class AuAppBase
	{
		static AuAppBase()
		{
			AppDomain.CurrentDomain.UnhandledException += (ad, e) => {
				if((ad as AppDomain).Id != AppDomain.CurrentDomain.Id) return; //avoid printing twice if subscribed in main and other appdomain
				OnHostHandledException(e);

				//Does not see exceptions:
				//1. thrown in the primary thread of a non-primary appdomain. Workaround: our host process uses try/catch.
				//2. thrown in Task.Run etc threads. It's OK. .NET handles exceptions silently, unless something waits for the task.
				//3. corrupted state exceptions. Tried [HandleProcessCorruptedStateExceptions] etc, unsuccessfully. Never mind.

				//This is used for:
				//1. Exceptions thrown in non-primary threads.
				//2. Exceptions thrown in non-hosted exe process.
				//Other exceptions are handled by the host program with try/catch.
				//Works well with all meta isolation/outputPath combinations.
			};
		}

		static AuAppBase s_instance;
		[ThreadStatic] static AuAppBase t_instance; //for isolation thread

		///
		protected AuAppBase()
		{
			t_instance = this;
			s_instance = this;

			//Au.Triggers.Trigger.Run(this);
		}

		/// <summary>
		/// Waits for trigger events specified in function attributes like <c>[Trigger.Hotkey("Ctrl+K")]</c>. On events calls that functions and continues to wait.
		/// </summary>
		public void RunTriggers() => Au.Triggers.Trigger.Run(this);
		//TODO: remove if not useful

		/// <summary>
		/// Prints exception info.
		/// Override this function to intercept unhandled exceptions. Call the base function if want to print exception info as usually.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnUnhandledException(UnhandledExceptionEventArgs e) => Print(e.ExceptionObject);

		internal static void OnHostHandledException(UnhandledExceptionEventArgs e)
		{
			if(e.ExceptionObject is ThreadAbortException) return;
			var k = t_instance ?? s_instance;
			if(k != null) k.OnUnhandledException(e);
			else Print(e.ExceptionObject);
		}
	}
}
