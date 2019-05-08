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
//using System.Linq;

using Au.Types;
using static Au.NoClass;

//CONSIDER: instead use standard C# STA pool code.
//	Initially I rejected it because each appdomain that uses it creates 4 threads. But now we rarely have multiple appdomains.
//	Initially automation tasks by default were executed in appdomains. Later appdomains were completely rejected because unstable.

namespace Au.Util
{
	/// <summary>
	/// Thread pool where COM can be used. Initializes thread COM as single-thread-apartment.
	/// </summary>
	[System.Security.SuppressUnmanagedCodeSecurity]
	internal static unsafe class ThreadPoolSTA
	{
		/// <summary>
		/// Work callback function parameter type.
		/// More info: <see cref="WorkCallback"/>.
		/// </summary>
		public class WorkCallbackData
		{
			///
			public object state;
			///
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
		/// <exception cref="Win32Exception"/>
		/// <exception cref="InvalidOperationException">(only when completionCallback is used) This thread has a synchronization context other than WindowsFormsSynchronizationContext or null; or it is null and thread's GetApartmentState is not STA.</exception>
		public static void SubmitCallback(object state, WorkCallback workCallback, SendOrPostCallback completionCallback = null)
		{
			new Work(state, workCallback, completionCallback, false);
		}

		/// <summary>
		/// Creates a <see cref="Work"/> object that can be used when need more options than <see cref="SubmitCallback"/> has.
		/// </summary>
		/// <param name="state">Something to pass to the callback functions.</param>
		/// <param name="workCallback">Callback function to call in a thread pool thread.</param>
		/// <param name="completionCallback">Optional callback function to call in this thread after workCallback.</param>
		/// <remarks>
		/// Call Dispose() to avoid memory leaks. If not called, the object and related OS object remain in memory until this appdomain ends.
		/// </remarks>
		/// <exception cref="Win32Exception"/>
		/// <exception cref="InvalidOperationException">(only when completionCallback is used) This thread has a synchronization context other than WindowsFormsSynchronizationContext or null; or it is null and thread's GetApartmentState is not STA.</exception>
		public static Work CreateWork(object state, WorkCallback workCallback, SendOrPostCallback completionCallback = null)
		{
			return new Work(state, workCallback, completionCallback, true);
		}

		/// <summary>
		/// Allows to submit a callback function (one or more times) to be called in thread pool threads, then optionally wait and cancel.
		/// Can be used when need more options than <see cref="SubmitCallback"/> has.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// using(var work = Util.ThreadPoolSTA.CreateWork(null, o =&gt; { Thread.Sleep(100); })) {
		/// 	work.Submit();
		/// 	work.Wait();
		/// }
		/// ]]></code>
		/// </example>
		public class Work :IDisposable
		{
			GCHandle _gc; //to manage the lifetime of this object
			object _state; //client's objects to pass to callbacks
			WorkCallback _workCallback; //called in thread pool thread
			SendOrPostCallback _completionCallback; //called in client thread
			SynchronizationContext _context; //for '_context.Post(_completionCallback, _state)'
			IntPtr _work; //CreateThreadpoolWork. Not used with simple callbacks.

			/// <exception cref="Win32Exception"/>
			/// <exception cref="InvalidOperationException">(only when completionCallback is used) This thread has a synchronization context other than WindowsFormsSynchronizationContext or null; or it is null and thread's GetApartmentState is not STA.</exception>
			internal Work(object state, WorkCallback workCallback, SendOrPostCallback completionCallback, bool createWork)
			{
				_state = state;
				_workCallback = workCallback;
				if(completionCallback != null) {
					_completionCallback = completionCallback;
					//we need WindowsFormsSynchronizationContext to call _completionCallback in this thread
					_context = LibEnsureWindowsFormsSynchronizationContext.EnsurePermanently();
					//TODO: loads Forms dll. Try to avoid it. Also, test with WPF.
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
				if(_work == default) throw new ObjectDisposedException(nameof(Work));
				SubmitThreadpoolWork(_work);
			}

			/// <summary>
			/// Waits until all submitted instances of the callback function finish their work.
			/// While waiting, this thread cannot dispatch events and Windows messages.
			/// </summary>
			public void Wait()
			{
				if(_work == default) throw new ObjectDisposedException(nameof(Work));
				WaitForThreadpoolWorkCallbacks(_work, false);
			}

			/// <summary>
			/// Waits until all running instances of the callback function finish their work, and cancels pending (still not running) instances.
			/// While waiting, this thread cannot dispatch events and Windows messages.
			/// </summary>
			public void Cancel()
			{
				if(_work == default) throw new ObjectDisposedException(nameof(Work));
				WaitForThreadpoolWorkCallbacks(_work, true);
			}

			/// <summary>
			/// Calls <see cref="Cancel"/>, releases all resources used by this object and allows this object to be garbage-collected.
			/// Call this to avoid memory leaks. If not called, the object and related OS object remain in memory until this appdomain ends.
			/// </summary>
			public void Dispose()
			{
				if(_work == default) return; //this is a simple callback (ThreadPoolSTA.SubmitCallback()) or already disposed
				Cancel();
				CloseThreadpoolWork(_work);
				_work = default;
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
#if false //this is the documented/legal way, but then in 'domain.DoCallBack(() => { Application.Exit(); });' sometimes strange exceptions in .NET internal func ThreadContext.ExitCommon().
				var apState = System.Windows.Forms.Application.OleRequired();
				Debug.Assert(apState == ApartmentState.STA);
#elif false //works, but slower (JIT etc)
				int hr = Api.CoGetApartmentType(out var apt, out var aptq); //cannot use [ThreadStatic] because thread pool is shared by appdomains
				if(hr == 0 && apt != Api.APTTYPE.APTTYPE_STA) {
					hr = Api.OleInitialize(default);
					//Thread.CurrentThread.Priority = ThreadPriority.BelowNormal; //does not make getting/displaying icons better
					//Api.SetThreadPriority(Api.GetCurrentThread(), -15);
				}
				Debug.Assert(hr == 0);
				Debug.Assert(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA);
#else //works, fast, but undocumented
				bool ok = Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
				Debug.Assert(ok);
#endif

				try {
					var d = new WorkCallbackData() { state = _state, completionCallback = _completionCallback };
					_workCallback(d);
					if(_context != null && d.completionCallback != null && !_isAppDomainDying) _context.Post(d.completionCallback, d.state);
					//info: Post() throws InvalidOperationException when appdomain is being unloaded. Usually then _isAppDomainDying is true.
					//note: don't use Send(), it is very slow with icons, maybe paints each time because there are no other messages in the queue.
				}
				catch(Exception e) { ADebug.Print(e); }
				finally {
					if(_work == default) _gc.Free(); //free now if this is a simple callback (ThreadPoolSTA.SubmitCallback()). Else Dispose() frees.
				}
			}

			//~Work()
			//{
			//	Print("dtor");
			//}
		}

		static ThreadPoolSTA()
		{
			var p = _ProcVar;
			if(p->pool == default) {
				lock("d3+gzRQ2mkiiOHFKsRGCXw") {
					if(p->pool == default) {
						var pool = CreateThreadpool(default);
						//SetThreadpoolThreadMinimum(pool, 2); //don't need this
						SetThreadpoolThreadMaximum(pool, 4); //info: 3-4 is optimal for getting icons
						p->pool = pool;
					}
				}
			}

			_env.Size = Api.SizeOf<TP_CALLBACK_ENVIRON_V3>();
			_env.Version = 3;
			_env.CallbackPriority = (int)TP_CALLBACK_PRIORITY.TP_CALLBACK_PRIORITY_NORMAL; //tested: low etc does not change speeds
			_env.Pool = p->pool;
			_env.CleanupGroup = CreateThreadpoolCleanupGroup();

			AAppDomain.Exit += _CurrentDomain_DomainExit;
		}

		static void _CurrentDomain_DomainExit(object sender, EventArgs e)
		{
			_isAppDomainDying = true;
			//var perf = Perf.StartNew();
			CloseThreadpoolCleanupGroupMembers(_env.CleanupGroup, true, default);
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
			public IntPtr pool;
		}
		static ProcessVariables* _ProcVar => &LibProcessMemory.Ptr->threadPool;

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
			public int Size;
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

		#endregion

	}
}
