using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Linq;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys.Util
{
	//[DebuggerStepThrough]
	public static unsafe class ThreadPoolSTA
	{
		/// <summary>
		/// Work callback function parameter type.
		/// More info: <see cref="WorkCallback"/>.
		/// </summary>
		public class WorkCallbackData
		{
			public object state;
			public SendOrPostCallback completionCallback;
		}

		/// <summary>
		/// Work callback function type.
		/// </summary>
		/// <param name="d">
		/// Contains state and completionCallback that were passed to the submit function.
		/// Your work callback function can change completionCallback and/or state. For example it can set completionCallback = null to prevent calling it (the call is expensive); or change state, if want to pass another object to the completion function.
		/// Note that the completion function is never called if initially it was null.
		/// </param>
		public delegate void WorkCallback(WorkCallbackData d);

		/// <summary>
		/// Requests that a thread pool thread call the callback function.
		/// </summary>
		/// <param name="state">Something to pass to the callback functions.</param>
		/// <param name="workCallback">Callback function to call in a thread pool thread.</param>
		/// <param name="completionCallback">Optional callback function to call in this thread after workCallback.</param>
		public static void SubmitCallback(object state, WorkCallback workCallback, SendOrPostCallback completionCallback = null)
		{
			new Work(state, workCallback, completionCallback, false);
		}

		/// <summary>
		/// Creates a Work object that can be used when need more options than SubmitCallback() has.
		/// </summary>
		/// <param name="state">Something to pass to the callback functions.</param>
		/// <param name="workCallback">Callback function to call in a thread pool thread.</param>
		/// <param name="completionCallback">Optional callback function to call in this thread after workCallback.</param>
		/// <remarks>
		/// Call Dispose() to avoid memory leaks. If not called, the object and related OS object remain in memory until this appdomain ends.
		/// </remarks>
		public static Work CreateWork(object state, WorkCallback workCallback, SendOrPostCallback completionCallback = null)
		{
			return new Work(state, workCallback, completionCallback, true);
		}

		/// <summary>
		/// Allows to submit a callback function (one or more times) to be called in thread pool threads, then optionally wait and cancel.
		/// Can be used when need more options than SubmitCallback() has.
		/// </summary>
		/// <example>
		/// using(var work = Util.ThreadPoolSTA.CreateWork(null, o =˃ { WaitMS(100); })) {
		/// 	work.Submit();
		/// 	work.Wait();
		/// }
		/// </example>
		public class Work :IDisposable
		{
			GCHandle _gc; //to manage the lifetime of this object
			object _state; //client's objects to pass to callbacks
			WorkCallback _workCallback; //called in thread pool thread
			SendOrPostCallback _completionCallback; //called in client thread
			SynchronizationContext _context; //for '_context.Post(_completionCallback, _state)'
			IntPtr _work; //CreateThreadpoolWork. Not used with simple callbacks.

			[ThreadStatic]
			static WindowsFormsSynchronizationContext _wfContext;

			internal Work(object state, WorkCallback workCallback, SendOrPostCallback completionCallback, bool createWork)
			{
				_state = state;
				_workCallback = workCallback;
				if(completionCallback != null) {
					_completionCallback = completionCallback;
					//we need WindowsFormsSynchronizationContext to call _completionCallback in this thread
					_context = SynchronizationContext.Current;
					if(!(_context is WindowsFormsSynchronizationContext)) {
						if(_wfContext == null) _wfContext = new WindowsFormsSynchronizationContext();
						if(_context == null && Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
							SynchronizationContext.SetSynchronizationContext(_wfContext);
							WindowsFormsSynchronizationContext.AutoInstall = false; //prevent Application.Run/DoEvents setting wrong context
						}
						_context = _wfContext;
					}
				}

				_gc = GCHandle.Alloc(this);
				Debug.Assert(sizeof(GCHandle) == IntPtr.Size); //we declare API IntPtr parameters as GCHandle

				if(createWork) {
					_work = CreateThreadpoolWork(_workCallbackDelegate, _gc, ref _env);
				} else {
					bool ok = TrySubmitThreadpoolCallback(_simpleCallbackDelegate, _gc, ref _env);
					//Debug.Assert(ok);
					if(!ok) throw new Win32Exception();
				}
			}

			/// <summary>
			/// Requests that a thread pool thread call the callback function.
			/// This function can be called multiple times.
			/// </summary>
			public void Submit()
			{
				if(_work == Zero) throw new ObjectDisposedException(nameof(Work));
				SubmitThreadpoolWork(_work);
			}

			/// <summary>
			/// Waits until all submitted instances of the callback function finish their work.
			/// While waiting, this thread cannot dispatch events and Windows messages.
			/// </summary>
			public void Wait()
			{
				if(_work == Zero) throw new ObjectDisposedException(nameof(Work));
				WaitForThreadpoolWorkCallbacks(_work, false);
			}

			/// <summary>
			/// Waits until all running instances of the callback function finish their work, and cancels pending (still not running) instances.
			/// While waiting, this thread cannot dispatch events and Windows messages.
			/// </summary>
			public void Cancel()
			{
				if(_work == Zero) throw new ObjectDisposedException(nameof(Work));
				WaitForThreadpoolWorkCallbacks(_work, true);
			}

			/// <summary>
			/// Calls Cancel(), releases all resources used by this object and allows this object to be garbage-collected.
			/// Call this to avoid memory leaks. If not called, the object and related OS object remain in memory until this appdomain ends.
			/// </summary>
			public void Dispose()
			{
				if(_work == Zero) return; //this is a simple callback (ThreadPoolSTA.SubmitCallback()) or already disposed
				Cancel();
				CloseThreadpoolWork(_work);
				_work = Zero;
				_gc.Free();
			}

			static void _WorkCallback(IntPtr Instance, GCHandle gcWork, IntPtr ipWork)
			{
				(gcWork.Target as Work)._Callback();
			}
			static PTP_WORK_CALLBACK _workCallbackDelegate = _WorkCallback;

			static void _SimpleCallback(IntPtr Instance, GCHandle gcWork)
			{
				(gcWork.Target as Work)._Callback();
			}
			static PTP_SIMPLE_CALLBACK _simpleCallbackDelegate = _SimpleCallback;

			void _Callback()
			{
				//set STA
#if false
				//This is faster than CoGetApartmentType/CoInitializeEx/OleInitialize.
				//But then in 'domain.DoCallBack(() => { Application.Exit(); });' sometimes strange exceptions in .NET internal func ThreadContext.ExitCommon().

				var apState = Application.OleRequired();
				Debug.Assert(apState == ApartmentState.STA);
#else
				APTTYPE apt; int aptq;
				int hr = CoGetApartmentType(out apt, out aptq); //cannot use [ThreadStatic] because thread pool is shared by appdomains
				if(hr == 0 && apt != APTTYPE.APTTYPE_STA) {
					hr = OleInitialize(Zero);
					//Thread.CurrentThread.Priority = ThreadPriority.BelowNormal; //does not make getting/displaying icons better
					//Api.SetThreadPriority(Api.GetCurrentThread(), -15);
				}
				Debug.Assert(hr == 0);
				Debug.Assert(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
#endif

				try {
					var d = new WorkCallbackData() { state = _state, completionCallback = _completionCallback };
					_workCallback(d);
					if(_context != null && d.completionCallback != null && !_isAppDomainDying) _context.Post(d.completionCallback, d.state);
					//info: Post() throws InvalidOperationException when appdomain is being unloaded. Usually then _isAppDomainDying is true.
					//note: don't use Send(), it is very slow with icons, maybe paints each time because there are no other messages in the queue.
				}
				catch(Exception e) { OutDebug(e); }
				finally {
					if(_work == Zero) _gc.Free(); //free now if this is a simple callback (ThreadPoolSTA.SubmitCallback()). Else Dispose() frees.
				}
			}

			//~Work()
			//{
			//	Out("dtor");
			//}
		}

		static ThreadPoolSTA()
		{
			var p = _ProcVar;
			if(p->_pool == Zero) {
				lock ("d3+gzRQ2mkiiOHFKsRGCXw") {
					if(p->_pool == Zero) {
						var pool = CreateThreadpool(Zero);
						//SetThreadpoolThreadMinimum(pool, 2); //don't need this
						SetThreadpoolThreadMaximum(pool, 4); //info: 3-4 is optimal for getting icons
						p->_pool = pool;
					}
				}
			}

			_env.Size = (uint)Marshal.SizeOf(typeof(TP_CALLBACK_ENVIRON_V3));
			_env.Version = 3;
			_env.CallbackPriority = (int)TP_CALLBACK_PRIORITY.TP_CALLBACK_PRIORITY_NORMAL; //tested: low etc does not change speeds
			_env.Pool = p->_pool;
			_env.CleanupGroup = CreateThreadpoolCleanupGroup();

			if(AppDomain.CurrentDomain.IsDefaultAppDomain()) AppDomain.CurrentDomain.ProcessExit += _CurrentDomain_DomainExit;
			else AppDomain.CurrentDomain.DomainUnload += _CurrentDomain_DomainExit;
		}

		static void _CurrentDomain_DomainExit(object sender, EventArgs e)
		{
			//OutFunc();
			_isAppDomainDying = true;
			//var perf = new Perf.Inst(true);
			CloseThreadpoolCleanupGroupMembers(_env.CleanupGroup, true, Zero);
			CloseThreadpoolCleanupGroup(_env.CleanupGroup);
			//perf.NW();
			//info:
			//	When unloading a non-default domain, CloseThreadpoolCleanupGroupMembers can wait until callbacks finished.
			//	When default domain is ending - can wait max 2 s, then process silently exits. It is documented.
			//info:
			//	Bad things happen if we don't call CloseThreadpoolCleanupGroupMembers (or call too late, in a static finalizer).
			//	If there are pending callbacks, we get "thread aborted" exception there, and next appdomain somehow does not run (process exits).
		}

		static bool _isAppDomainDying;

		internal struct ProcessVariables
		{
			public IntPtr _pool;
		}
		static ProcessVariables* _ProcVar { get { return &LibProcessMemory.Ptr->threadPool; } }

		//Each appdomain has its own TP_CALLBACK_ENVIRON_V3 with its own cleanup group and shared thread pool.
		static TP_CALLBACK_ENVIRON_V3 _env;

		#region api

		struct TP_CALLBACK_ENVIRON_V3
		{
			public uint Version;
			public IntPtr Pool;
			public IntPtr CleanupGroup;
			//public PTP_CLEANUP_GROUP_CANCEL_CALLBACK CleanupGroupCancelCallback;
			public IntPtr CleanupGroupCancelCallback;
			public IntPtr RaceDll;
			public IntPtr ActivationContext;
			//public PTP_SIMPLE_CALLBACK FinalizationCallback;
			public IntPtr FinalizationCallback;
			public uint Flags;
			public int CallbackPriority;
			public uint Size;
		}

		enum TP_CALLBACK_PRIORITY
		{
			TP_CALLBACK_PRIORITY_HIGH,
			TP_CALLBACK_PRIORITY_NORMAL,
			TP_CALLBACK_PRIORITY_LOW,
			TP_CALLBACK_PRIORITY_INVALID,
			TP_CALLBACK_PRIORITY_COUNT = 3
		}

		[DllImport("kernel32.dll")]
		static extern IntPtr CreateThreadpool(IntPtr reserved);

		[DllImport("kernel32.dll")]
		static extern void SetThreadpoolThreadMaximum(IntPtr ptpp, uint cthrdMost);

		delegate void PTP_SIMPLE_CALLBACK(IntPtr Instance, GCHandle Context);

		[DllImport("kernel32.dll")]
		static extern bool TrySubmitThreadpoolCallback(PTP_SIMPLE_CALLBACK pfns, GCHandle pv, ref TP_CALLBACK_ENVIRON_V3 pcbe);

		[DllImport("kernel32.dll")]
		static extern IntPtr CreateThreadpoolCleanupGroup();

		[DllImport("kernel32.dll")]
		static extern void CloseThreadpoolCleanupGroupMembers(IntPtr ptpcg, bool fCancelPendingCallbacks, IntPtr pvCleanupContext);

		[DllImport("kernel32.dll")]
		static extern void CloseThreadpoolCleanupGroup(IntPtr ptpcg);

		delegate void PTP_WORK_CALLBACK(IntPtr Instance, GCHandle Context, IntPtr ipWork);

		[DllImport("kernel32.dll")]
		static extern IntPtr CreateThreadpoolWork(PTP_WORK_CALLBACK pfnwk, GCHandle pv, ref TP_CALLBACK_ENVIRON_V3 pcbe);

		[DllImport("kernel32.dll")]
		static extern void SubmitThreadpoolWork(IntPtr pwk);

		[DllImport("kernel32.dll")]
		static extern void CloseThreadpoolWork(IntPtr pwk);

		[DllImport("kernel32.dll")]
		static extern void WaitForThreadpoolWorkCallbacks(IntPtr pwk, bool fCancelPendingCallbacks);

		//[DllImport("kernel32.dll")]
		//static extern void CloseThreadpool(IntPtr ptpp);

		//[DllImport("kernel32.dll")]
		//static extern bool SetThreadpoolThreadMinimum(IntPtr ptpp, uint cthrdMic);

		enum APTTYPE
		{
			APTTYPE_CURRENT = -1,
			APTTYPE_STA,
			APTTYPE_MTA,
			APTTYPE_NA,
			APTTYPE_MAINSTA
		}

		[DllImport("ole32.dll", PreserveSig = true)]
		static extern int CoGetApartmentType(out APTTYPE pAptType, out int pAptQualifier);

		[DllImport("ole32.dll", PreserveSig = true)]
		public static extern int OleInitialize(IntPtr pvReserved);

		#endregion

	}
}
