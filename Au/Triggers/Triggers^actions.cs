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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Triggers
{
	class TOptions
	{
		public Action<TOBeforeArgs> before;
		public Action<TOAfterArgs> after;
		public int thread;
		public int ifRunning;

		public TOptions Clone() => this.MemberwiseClone() as TOptions;

		//CONSIDER: before calling 'before' or action, reset all Opt options. Or use main thread's options set before adding trigger.
	}

	/// <summary>
	/// Allows to set some options for multiple triggers and their actions.
	/// </summary>
	/// <remarks>
	/// You set options through a thread-static property <see cref="Triggers.Options"/>.
	/// Changed options are applied to all triggers/actions added afterwards in this thread.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Triggers.Options.RunActionInThreadPool(singleInstance: false);
	/// Triggers.Options.BeforeAction = o => { Opt.Key.KeySpeed = 10; };
	/// Triggers.Hotkey["Ctrl+K"] = o => Print(Opt.Key.KeySpeed); //10
	/// Triggers.Hotkey["Ctrl+Shift+K"] = o => Print(Opt.Key.KeySpeed); //10
	/// Triggers.Options.BeforeAction = o => { Opt.Key.KeySpeed = 20; };
	/// Triggers.Hotkey["Ctrl+L"] = o => Print(Opt.Key.KeySpeed); //20
	/// Triggers.Hotkey["Ctrl+Shift+L"] = o => Print(Opt.Key.KeySpeed); //20
	/// ]]></code>
	/// </example>
	public class TriggerOptions
	{
		TOptions _new, _prev;

		TOptions _New() => _new ?? (_new = _prev?.Clone() ?? new TOptions());

		/// <summary>
		/// Run actions in a dedicated thread that does not end when actions end.
		/// </summary>
		/// <param name="thread">Any non-negative number that you want to use to identify the thread. Default 0.</param>
		/// <param name="ifRunningWaitMS">Defines when to start an action if an action (other or same) is currently running in this thread. If 0 (default), don't run. If -1 (<b>Timeout.Infinite</b>), run when that action ends (and possibly other queed actions). If &gt; 0, run when that action ends, if it ends within this time from now; the time is in milliseconds.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="thread"/> &lt; 0 or <paramref name="ifRunningWaitMS"/> &lt; -1.</exception>
		/// <remarks>
		/// Multiple actions in same thread cannot run simultaneously. Actions in different threads can run simultaneously.
		/// There is no "end old running action" feature. If need it, use other script. Example: <c>Triggers.Hotkey["Ctrl+M"] = o => AuTask.RunWait("Other Script");</c>.
		/// There is no "temporarily pause old running action to run new action" feature. As well as for scripts.
		/// The thread has <see cref="ApartmentState.STA"/>.
		/// The <b>RunActionInX</b> functions are mutually exclusive: only the last called function is active. If none called, it is the same as called this function without arguments.
		/// </remarks>
		public void RunActionInThread(int thread = 0, int ifRunningWaitMS = 0)
		{
			_New();
			_new.thread = thread >= 0 ? thread : throw new ArgumentOutOfRangeException(null, "thread must be >= 0");
			_new.ifRunning = ifRunningWaitMS >= -1 ? ifRunningWaitMS : throw new ArgumentOutOfRangeException(null, "ifRunningWaitMS must be >= -1");
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
			_new.thread = -1;
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
			_new.thread = -2;
			_new.ifRunning = singleInstance ? 0 : 1;
		}

		/// <summary>
		/// null or an action to run before the actual action.
		/// For example can set Opt options.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Options.BeforeAction = o => { Opt.Key.KeySpeed = 20; Opt.Key.TextSpeed = 5; };
		/// ]]></code>
		/// </example>
		public Action<TOBeforeArgs> BeforeAction { set => _New().before = value; }

		/// <summary>
		/// null or an action to run after the actual action.
		/// For example can log exception.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Options.AfterAction = o => { if(o.Exception!=null) Print(o.Exception.Message); else Print("completed successfully"); };
		/// ]]></code>
		/// </example>
		public Action<TOAfterArgs> AfterAction { set => _New().after = value; }

		internal TOptions Current {
			get {
				if(_new != null) { _prev = _new; _new = null; }
				return _prev ?? s_empty ?? (s_empty = new TOptions());
			}
		}
		static TOptions s_empty;
	}

	/// <summary>
	/// Arguments for <see cref="TriggerOptions.BeforeAction"/>.
	/// Currently does not contain any properties.
	/// </summary>
	public class TOBeforeArgs
	{

	}

	/// <summary>
	/// Arguments for <see cref="TriggerOptions.AfterAction"/>.
	/// </summary>
	public class TOAfterArgs
	{
		///
		internal TOAfterArgs(Exception e) { Exception = e; }

		/// <summary>
		/// If action ended with an exception, contains the exception. Else null.
		/// </summary>
		public Exception Exception { get; private set; }
	}

	class TriggerActionThreads
	{
		//public TriggerActionThreads(Triggers triggers)
		//{

		//}

		public void Run(Trigger ta, TriggerArgs args)
		{
			Action actionWrapper = () => {
				var opt = ta.options;
				try {
#if true
					opt.before?.Invoke(new TOBeforeArgs());
#else
					if(opt.before != null) {
						bool called = false;
						if(t_beforeCalled == null) t_beforeCalled = new List<Action<bool>> { opt.before };
						else if(!t_beforeCalled.Contains(opt.before)) t_beforeCalled.Add(opt.before);
						else called = true;
						opt.before(!called);
					}
#endif
					Exception ex = null;
					try { ta.Run(args); }
					catch(Exception e1) when(!(e1 is ThreadAbortException)) { Print(ex = e1); }
					opt.after?.Invoke(new TOAfterArgs(ex));
				}
				catch(Exception e2) {
					if(e2 is ThreadAbortException) Thread.ResetAbort(); //FUTURE: don't reset if eg thrown to end task process softly
					else Print(e2);
				}
				finally {
					if(opt.thread < 0 && opt.ifRunning == 0) _d.TryRemove(ta, out _);
				}
			};
			//never mind: we should not create actionWrapper if cannot run. But such cases are rare. Fast and small, about 64 bytes.

			int threadId = ta.options.thread;
			if(threadId >= 0) { //dedicated thread
				_Thread h = null; foreach(var v in _a) if(v.id == threadId) { h = v; break; }
				if(h == null) _a.Add(h = new _Thread(threadId));
				h.RunAction(actionWrapper, ta.options.ifRunning);
			} else {
				bool singleInstance = ta.options.ifRunning == 0;
				if(singleInstance) {
					if(_d == null) _d = new ConcurrentDictionary<Trigger, object>();
					if(_d.TryGetValue(ta, out var tt)) {
						//return;
						switch(tt) {
						case Thread thread:
							if(thread.IsAlive) return;
							break;
						case Task task:
							//Print(task.Status);
							switch(task.Status) { case TaskStatus.RanToCompletion: case TaskStatus.Faulted: case TaskStatus.Canceled: break; default: return; }
							break;
						}
					}
				}

				switch(threadId) {
				case -1: //new thread
					var thread = new Thread(actionWrapper.Invoke) { IsBackground = true };
					thread.SetApartmentState(ApartmentState.STA);
					if(singleInstance) _d[ta] = thread;
					thread.Start();
					break;
				case -2: //thread pool
					var task = new Task(actionWrapper);
					if(singleInstance) _d[ta] = task;
					task.Start();
					break;
				}
			}
		}
		//[ThreadStatic] List<Action<bool>> t_beforeCalled;

		public void Dispose()
		{
			foreach(var v in _a) v.Dispose();
		}

		List<_Thread> _a = new List<_Thread>();
		ConcurrentDictionary<Trigger, object> _d;

		class _Thread
		{
			struct _Action { public Action actionWrapper; public long time; }

			Queue<_Action> _q;
			IntPtr _event;
			bool _running;
			bool _disposed;
			public readonly int id;

			public _Thread(int id) { this.id = id; }

			public void RunAction(Action actionWrapper, int ifRunningWaitMS)
			{
				if(_disposed) return;
				if(_q == null) {
					_q = new Queue<_Action>();
					_event = Api.CreateEvent(false);
					Thread_.Start(() => {
						try {
							while(!_disposed && 0 == Api.WaitForSingleObject(_event, -1)) {
								while(!_disposed) {
									_Action x;
									lock(_q) {
										g1:
										if(_q.Count == 0) { _running = false; break; }
										x = _q.Dequeue();
										if(x.time != 0 && Time.PerfMilliseconds > x.time) goto g1;
										_running = true;
									}
									x.actionWrapper();
								}
							}
						}
						finally {
							Api.CloseHandle(_event);
							_q = null; _running = false; //restart if aborted
							//Print("thread ended");
						}
					});
				}

				lock(_q) {
					if(_running) {
						if(ifRunningWaitMS == 0) return;
					} else {
						_running = true;
						//if(ifRunningWaitMS > 0 && ifRunningWaitMS < 1000000000) ifRunningWaitMS += 1000;
					}
					_q.Enqueue(new _Action { actionWrapper = actionWrapper, time = ifRunningWaitMS <= 0 ? 0 : Time.PerfMilliseconds + ifRunningWaitMS });
				}
				Api.SetEvent(_event);
			}

			public void Dispose()
			{
				if(_disposed) return; _disposed = true;
				Api.SetEvent(_event);
			}
		}
	}
}
