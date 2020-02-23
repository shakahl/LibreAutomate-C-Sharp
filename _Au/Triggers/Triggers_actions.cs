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

using Au.Types;
using static Au.AStatic;

namespace Au.Triggers
{
	class TOptions
	{
		public Action<TOBAArgs> before;
		public Action<TOBAArgs> after;
		public short thread; //>=0 dedicated, -1 main, -2 new, -3 pool
		public bool noWarning;
		public int ifRunning; //for dedicated thread it is the timeout (>= -1); for new/pool threads it is 0 if single instance, 1 if multi.

		public TOptions Clone() => this.MemberwiseClone() as TOptions;
	}

	/// <summary>
	/// Allows to set some options for multiple triggers and their actions.
	/// </summary>
	/// <remarks>
	/// You set options through a thread-static property <see cref="ActionTriggers.Options"/>.
	/// Changed options are applied to all triggers/actions added afterwards in this thread.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Triggers.Options.RunActionInThreadPool(singleInstance: false);
	/// Triggers.Options.BeforeAction = o => { AOpt.Key.KeySpeed = 10; };
	/// Triggers.Hotkey["Ctrl+K"] = o => Print(AOpt.Key.KeySpeed); //10
	/// Triggers.Hotkey["Ctrl+Shift+K"] = o => Print(AOpt.Key.KeySpeed); //10
	/// Triggers.Options.BeforeAction = o => { AOpt.Key.KeySpeed = 20; };
	/// Triggers.Hotkey["Ctrl+L"] = o => Print(AOpt.Key.KeySpeed); //20
	/// Triggers.Hotkey["Ctrl+Shift+L"] = o => Print(AOpt.Key.KeySpeed); //20
	/// ]]></code>
	/// </example>
	public class TriggerOptions
	{
		TOptions _new, _prev;

		TOptions _New() => _new ??= (_prev?.Clone() ?? new TOptions());

		/// <summary>
		/// Run actions in a dedicated thread that does not end when actions end.
		/// </summary>
		/// <param name="thread">A number that you want to use to identify the thread. Can be 0-32767 (short.MaxValue). Default 0.</param>
		/// <param name="ifRunningWaitMS">Defines when to start an action if an action (other or same) is currently running in this thread. If 0 (default), don't run. If -1 (<b>Timeout.Infinite</b>), run when that action ends (and possibly other queed actions). If &gt; 0, run when that action ends, if it ends within this time from now; the time is in milliseconds.</param>
		/// <param name="noWarning">No warning when cannot start an action because an action is running and ifRunningWaitMS==0.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Multiple actions in same thread cannot run simultaneously. Actions in different threads can run simultaneously.
		/// There is no "end old running action" feature. If need it, use other script. Example: <c>Triggers.Hotkey["Ctrl+M"] = o => ATask.RunWait("Other Script");</c>.
		/// There is no "temporarily pause old running action to run new action" feature. As well as for scripts.
		/// The thread has <see cref="ApartmentState.STA"/>.
		/// The <b>RunActionInX</b> functions are mutually exclusive: only the last called function is active. If none called, it is the same as called this function without arguments.
		/// </remarks>
		public void RunActionInThread(int thread = 0, int ifRunningWaitMS = 0, bool noWarning = false)
		{
			_New();
			if((uint)thread > short.MaxValue) throw new ArgumentOutOfRangeException();
			_new.thread = (short)thread;
			_new.ifRunning = ifRunningWaitMS >= -1 ? ifRunningWaitMS : throw new ArgumentOutOfRangeException();
			_new.noWarning = noWarning;
		}
		//CONSIDER: make default ifRunningWaitMS = 1000 if it is another action.

		/// <summary>
		/// Run actions in same thread as <see cref="ActionTriggers.Run"/>.
		/// </summary>
		/// <remarks>
		/// Can be used only for actions that return as soon as possible, in less than 10 ms. Use to create and show toolbars (<see cref="AToolbar"/>).
		/// The <b>RunActionInX</b> functions are mutually exclusive: only the last called function is active.
		/// </remarks>
		public void RunActionInMainThread()
		{
			_New();
			_new.thread = -1;
			_new.ifRunning = 0;
		}

		/// <summary>
		/// Run actions in new threads.
		/// </summary>
		/// <remarks>
		/// Use if need to run actions simultaneously with other actions or other instances of self, especially if the action is long-running (maybe 5 s and more).
		/// The thread has <see cref="ApartmentState.STA"/>.
		/// The <b>RunActionInX</b> functions are mutually exclusive: only the last called function is active.
		/// </remarks>
		/// <param name="singleInstance">Don't run if this action is already running. If false, multiple action instances can run paralelly in multiple threads.</param>
		public void RunActionInNewThread(bool singleInstance)
		{
			_New();
			_new.thread = -2;
			_new.ifRunning = singleInstance ? 0 : 1;
		}

		/// <summary>
		/// Run actions in thread pool threads.
		/// </summary>
		/// <remarks>
		/// Use if need to run actions simultaneously with other actions or other instances of self, and the action is short-running (maybe less than 5 s) and don't need <see cref="ApartmentState.STA"/>.
		/// Thread pool threads have <see cref="ApartmentState.MTA"/>.
		/// The <b>RunActionInX</b> functions are mutually exclusive: only the last called function is active.
		/// </remarks>
		/// <param name="singleInstance">Don't run if this action is already running. If false, multiple action instances can run paralelly in multiple threads.</param>
		public void RunActionInThreadPool(bool singleInstance)
		{
			_New();
			_new.thread = -3;
			_new.ifRunning = singleInstance ? 0 : 1;
		}

		/// <summary>
		/// A function to run before the trigger action.
		/// For example, it can set <see cref="AOpt"/> options.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Options.BeforeAction = o => { AOpt.Key.KeySpeed = 20; AOpt.Key.TextSpeed = 5; };
		/// ]]></code>
		/// </example>
		public Action<TOBAArgs> BeforeAction { set => _New().before = value; }

		/// <summary>
		/// A function to run after the trigger action.
		/// For example, it can log exceptions.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Options.AfterAction = o => { if(o.Exception!=null) Print(o.Exception.Message); else Print("completed successfully"); };
		/// ]]></code>
		/// </example>
		public Action<TOBAArgs> AfterAction { set => _New().after = value; }

		internal TOptions Current {
			get {
				if(_new != null) { _prev = _new; _new = null; }
				return _prev ?? (s_empty ??= new TOptions());
			}
		}
		static TOptions s_empty;

		/// <summary>
		/// If true, triggers added afterwards don't depend on <see cref="ActionTriggers.Disabled"/> and <see cref="ActionTriggers.DisabledEverywhere"/>.
		/// This property sets the <see cref="ActionTrigger.EnabledAlways"/> property of triggers added afterwards.
		/// </summary>
		public bool EnabledAlways { get; set; }
	}

	/// <summary>
	/// Arguments for <see cref="TriggerOptions.BeforeAction"/> and <see cref="TriggerOptions.AfterAction"/>.
	/// </summary>
	public struct TOBAArgs
	{
		internal TOBAArgs(TriggerArgs args)
		{
			ActionArgs = args;
			Exception = null;
		}

		/// <summary>
		/// Trigger event info. The same variable as passed to the trigger action.
		/// To access the info, cast to <b>HotkeyTriggerArgs</b> etc, depending on trigger type.
		/// </summary>
		public TriggerArgs ActionArgs { get; }

		/// <summary>
		/// If action ended with an exception, the exception. Else null.
		/// </summary>
		public Exception Exception { get; internal set; }
	}

	class TriggerActionThreads
	{
		public void Run(ActionTrigger trigger, TriggerArgs args, int muteMod)
		{
			Action actionWrapper = () => {
				var opt = trigger.options;
				try {
					_MuteMod(ref muteMod);

					string sTrigger = null;
					if(ATask.Role == ATRole.MiniProgram) Util.Log_.Run.Write($"Trigger action started. Trigger: {sTrigger = trigger.ToString()}");

					AOpt.Reset();

					var baArgs = new TOBAArgs(args); //struct
#if true
					opt.before?.Invoke(baArgs);
#else
					if(opt.before != null) {
						bool called = false;
						if(t_beforeCalled == null) t_beforeCalled = new List<Action<bool>> { opt.before };
						else if(!t_beforeCalled.Contains(opt.before)) t_beforeCalled.Add(opt.before);
						else called = true;
						opt.before(!called);
					}
#endif
					try {
						trigger.Run(args);

						if(sTrigger != null) Util.Log_.Run.Write($"Trigger action ended. Trigger: {sTrigger}");
					}
					catch(Exception e1) {
						if(sTrigger != null) Util.Log_.Run.Write($"Unhandled exception in trigger action. Trigger: {sTrigger}. Exception: {e1.ToStringWithoutStack()}");

						baArgs.Exception = e1;
						Print(e1);
					}
					opt.after?.Invoke(baArgs);
				}
				catch(Exception e2) {
					Print(e2);
				}
				finally {
					if(opt.thread < -1 && opt.ifRunning == 0) _d.TryRemove(trigger, out _);
				}
			};
			//never mind: we should not create actionWrapper if cannot run. But such cases are rare. Fast and small, about 64 bytes.

			int threadId = trigger.options.thread;
			if(threadId >= 0) { //dedicated thread
				_Thread h = null; foreach(var v in _a) if(v.id == threadId) { h = v; break; }
				if(h == null) _a.Add(h = new _Thread(threadId));
				if(h.RunAction(actionWrapper, trigger)) return;
			} else if(threadId == -1) { //main thread
				actionWrapper();
				return;
				//note: can reenter. Probably it is better than to cancel if already running.
			} else {
				bool canRun = true;
				bool singleInstance = trigger.options.ifRunning == 0;
				if(singleInstance) {
					_d ??= new ConcurrentDictionary<ActionTrigger, object>();
					if(_d.TryGetValue(trigger, out var tt)) {
						switch(tt) {
						case Thread thread:
							if(thread.IsAlive) canRun = false;
							break;
						case Task task:
							//Print(task.Status);
							switch(task.Status) {
							case TaskStatus.RanToCompletion: case TaskStatus.Faulted: case TaskStatus.Canceled: break;
							default: canRun = false; break;
							}
							break;
						}
					}
				}

				if(canRun) {
					if(threadId == -2) { //new thread
						var thread = new Thread(actionWrapper.Invoke) { IsBackground = true };
						thread.SetApartmentState(ApartmentState.STA);
						if(singleInstance) _d[trigger] = thread;
						try { thread.Start(); }
						catch(OutOfMemoryException) { //too many threads, probably 32-bit process
							if(singleInstance) _d.TryRemove(trigger, out _);
							_OutOfMemory();
							//SHOULDDO: before starting thread, warn if there are too many action threads.
							//	In 32-bit process normally fails at ~3000 threads.
							//	Unlikely to fail in 64-bit process, but at ~15000 threads starts to hang temporarily, which causes hook timeout, slow mouse, other anomalies.
						}
					} else { //thread pool
						var task = new Task(actionWrapper);
						if(singleInstance) _d[trigger] = task;
						task.Start();
					}
					return;
				}
			}

			if(muteMod != 0) ThreadPool.QueueUserWorkItem(_ => _MuteMod(ref muteMod));
		}
		//[ThreadStatic] List<Action<bool>> t_beforeCalled;

		public void Dispose()
		{
			foreach(var v in _a) v.Dispose();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void _OutOfMemory()
		{
			PrintWarning("There is not enough memory available to start the trigger action thread.", -1); //info: -1 because would need much memory for stack trace
		}

		List<_Thread> _a = new List<_Thread>();
		ConcurrentDictionary<ActionTrigger, object> _d;

		class _Thread
		{
			struct _Action { public Action actionWrapper; public long time; }

			Handle_ _event;
			Queue<_Action> _q;
			bool _running;
			bool _disposed;
			public readonly int id;

			public _Thread(int id) { this.id = id; }

			/// <summary>
			/// Adds the action to the queue and notifies the thread to execute it.
			/// If the thread is busy, returns false; if ifRunning!=0, the action possibly will run later.
			/// </summary>
			public bool RunAction(Action actionWrapper, ActionTrigger trigger)
			{
				if(_disposed) return false;
				if(_q == null) {
					_q = new Queue<_Action>();
					_event = Api.CreateEvent(false);
					try {
						AThread.Start(() => {
							try {
								while(!_disposed && 0 == Api.WaitForSingleObject(_event, -1)) {
									while(!_disposed) {
										_Action x;
										lock(_q) {
											g1:
											if(_q.Count == 0) { _running = false; break; }
											x = _q.Dequeue();
											if(x.time != 0 && ATime.PerfMilliseconds > x.time) goto g1;
											_running = true;
										}
										x.actionWrapper();
									}
								}
							}
							finally {
								_event.Dispose();
								_q = null; _running = false; //restart if aborted
															 //Print("thread ended");
							}
						});
					}
					catch(OutOfMemoryException) { //too many threads, probably 32-bit process
						_event.Dispose();
						_OutOfMemory();
					}
				}

				bool R = true;
				lock(_q) {
					int ifRunningWaitMS = trigger.options.ifRunning;
					if(_running) {
						if(ifRunningWaitMS == 0) {
							if(!trigger.options.noWarning) Print("Warning: can't run the trigger action because an action is running in this thread. To run simultaneously or wait, use one of Triggers.Options.RunActionInX functions. To disable this warning: Triggers.Options.RunActionInThread(0, 0, noWarning: true);. Trigger: " + trigger);
							return false;
						}
						R = false;
					} else {
						_running = true;
						//if(ifRunningWaitMS > 0 && ifRunningWaitMS < 1000000000) ifRunningWaitMS += 1000;
					}
					_q.Enqueue(new _Action { actionWrapper = actionWrapper, time = ifRunningWaitMS <= 0 ? 0 : ATime.PerfMilliseconds + ifRunningWaitMS });
				}
				Api.SetEvent(_event);
				return R;
			}

			public void Dispose()
			{
				if(_disposed) return; _disposed = true;
				Api.SetEvent(_event);
			}
		}

		void _MuteMod(ref int muteMod)
		{
			switch(Interlocked.Exchange(ref muteMod, 0)) {
			case c_modRelease:
				AKeys.Internal_.ReleaseModAndDisableModMenu();
				break;
			case c_modCtrl:
				AKeys.Internal_.SendKey(KKey.Ctrl); //disable Alt/Win menu
				break;
			}
		}

		public const int c_modRelease = 1, c_modCtrl = 2;
	}
}
