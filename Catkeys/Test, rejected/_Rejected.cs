//Classes and functions that were (almost) finished, but rejected for some reason. Maybe still can be useful in the future.
//For example, when tried to make faster/better than existing .NET classes/functions, but the result was not fast/good enough.

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

	public unsafe class TaskSTA
	{
		public delegate object WorkCallback(object state);
		public delegate void CompletionCallback(object state, object workResult);

		WorkCallback _workCallback;
		CompletionCallback _completionCallback;
		Control _control;
		object _state;
		PTP_SIMPLE_CALLBACK _callback1; //to protect from GC through _gch
		PTP_WORK_CALLBACK _callback2; //to protect from GC through _gch
		GCHandle _gch;

		/// <summary>
		/// Initializes a new instance of TaskSTA.
		/// </summary>
		/// <param name="workCallback">Callback function that will run in a thread pool thread.</param>
		/// <param name="state">Something to pass to callback functions.</param>
		/// <param name="completionCallback">Callback function that will run in thread of control when workCallback function returns.</param>
		/// <param name="control">A control or form etc. Used to Invoke completionCallback in its thread. If its handle is still not created when workCallback returns, the worker thread before invoking completionCallback waits max 5 s until handle created; on timeout completionCallback is not called, but no exception.</param>
		/// <exception cref="ArgumentNullException">completionCallback and control must be both null or not null.</exception>
		public TaskSTA(WorkCallback workCallback, object state = null, CompletionCallback completionCallback = null, Control control = null)
		{
			if((completionCallback == null) != (control == null)) throw new ArgumentNullException();

			_workCallback = workCallback;
			_completionCallback = completionCallback;
			_control = control;
			_state = state;
			_callback1 = _Callback1;
			_callback2 = _Callback2;
			_gch = GCHandle.Alloc(this);
		}

		/// <summary>
		/// Requests that a thread pool worker thread call the callback function specified in constructor.
		/// </summary>
		public void Run()
		{
			bool ok = TrySubmitThreadpoolCallback(_callback1, Zero, _Env);
			//Debug.Assert(ok);
			if(!ok) throw new Win32Exception();
		}

		IntPtr _work;

		/// <summary>
		/// unfinished.
		/// </summary>
		public void CreateWork()
		{
			_work = CreateThreadpoolWork(_callback2, Zero, _Env);
			//Debug.Assert(ok);
			if(_work == Zero) throw new Win32Exception();
		}

		/// <summary>
		/// unfinished.
		/// </summary>
		public void RunWork()
		{
			SubmitThreadpoolWork(_work);
		}

		void _Callback2(IntPtr Instance, IntPtr Context, IntPtr work)
		{
			_Callback1(Instance, Context);
		}

		void _Callback1(IntPtr Instance, IntPtr Context)
		{
			APTTYPE apt; int aptq;
			int hr = CoGetApartmentType(out apt, out aptq);
			if(hr == 0 && apt != APTTYPE.APTTYPE_STA) hr = CoInitializeEx(Zero, COINIT.COINIT_APARTMENTTHREADED | COINIT.COINIT_DISABLE_OLE1DDE);
			Debug.Assert(hr == 0);

			//OutFunc();
			Control c = null;
			try {
				object workResult = _workCallback(_state);

				c = _control; if(c == null) return;
				for(int i = 1; i < 100; i++) { //5 s
					if(c.IsDisposed) return;
					if(c.IsHandleCreated) break;
					WaitMS(i);
				}
				c.Invoke(_completionCallback, _state, workResult);
			}
			catch(InvalidOperationException e) {
				if(c == null || !c.IsDisposed) {
					OutDebug(e.Message);
				}
			}
			catch(Exception e) { OutDebug(e); }
			finally {
				if(_work == null) _gch.Free(); //TODO
			}
		}

		//~_Work()
		//{
		//	Out("dtor");
		//}

		static TaskSTA()
		{
			var p = _Env;
			if(p->Pool != Zero) return;
			lock ("d3+gzRQ2mkiiOHFKsRGCXw") {
				if(p->Pool != Zero) return;

				p->Size = Api.SizeOf(*p);
				p->Version = 3;
				p->CallbackPriority = (int)TP_CALLBACK_PRIORITY.TP_CALLBACK_PRIORITY_NORMAL;

				var pool = CreateThreadpool(Zero);
				SetThreadpoolThreadMinimum(pool, 2);
				SetThreadpoolThreadMaximum(pool, 4);

				p->Pool = pool;
			}
		}

		static TP_CALLBACK_ENVIRON_V3* _Env { get { return &_LibProcessMemory.Ptr->threadPool; } }

		#region api

		internal struct TP_CALLBACK_ENVIRON_V3
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
		static extern void CloseThreadpool(IntPtr ptpp);

		[DllImport("kernel32.dll")]
		static extern void SetThreadpoolThreadMaximum(IntPtr ptpp, uint cthrdMost);

		[DllImport("kernel32.dll")]
		static extern bool SetThreadpoolThreadMinimum(IntPtr ptpp, uint cthrdMic);

		delegate void PTP_WORK_CALLBACK(IntPtr Instance, IntPtr Context, IntPtr Work);

		[DllImport("kernel32.dll")]
		static extern IntPtr CreateThreadpoolWork(PTP_WORK_CALLBACK pfnwk, IntPtr pv, TP_CALLBACK_ENVIRON_V3* pcbe);

		[DllImport("kernel32.dll")]
		static extern void SubmitThreadpoolWork(IntPtr pwk);

		delegate void PTP_SIMPLE_CALLBACK(IntPtr Instance, IntPtr Context);

		[DllImport("kernel32.dll")]
		static extern bool TrySubmitThreadpoolCallback(PTP_SIMPLE_CALLBACK pfns, IntPtr pv, TP_CALLBACK_ENVIRON_V3* pcbe);

		[DllImport("kernel32.dll")]
		static extern void CloseThreadpoolWork(IntPtr pwk);

		[DllImport("kernel32.dll")]
		static extern void WaitForThreadpoolWorkCallbacks(IntPtr pwk, bool fCancelPendingCallbacks);

		public enum APTTYPE
		{
			APTTYPE_CURRENT = -1,
			APTTYPE_STA,
			APTTYPE_MTA,
			APTTYPE_NA,
			APTTYPE_MAINSTA
		}

		[DllImport("ole32.dll", PreserveSig = true)]
		public static extern int CoGetApartmentType(out APTTYPE pAptType, out int pAptQualifier);

		[Flags]
		public enum COINIT :uint
		{
			COINIT_APARTMENTTHREADED = 0x2,
			COINIT_MULTITHREADED = 0x0,
			COINIT_DISABLE_OLE1DDE = 0x4,
			COINIT_SPEED_OVER_MEMORY = 0x8
		}

		[DllImport("ole32.dll", PreserveSig = true)]
		public static extern int CoInitializeEx(IntPtr pvReserved, COINIT dwCoInit);

		#endregion
	}

	public partial class Files
	{

		/// <summary>
		/// Gets shell icon of a file or protocol etc where SHGetFileInfo would fail.
		/// Also can get icons of sizes other than 16 or 32.
		/// Cannot get file extension icons.
		/// If pidl is not Zero, uses it and ignores file, else uses file.
		/// Returns Zero if failed.
		/// </summary>
		[HandleProcessCorruptedStateExceptions]
		static unsafe IntPtr _Icon_GetSpec(string file, IntPtr pidl, int size)
		{
			IntPtr R = Zero;
			bool freePidl = false;
			Api.IShellFolder folder = null;
			Api.IExtractIcon eic = null;
			try { //possible exceptions in shell32.dll or in shell extensions
				if(pidl == Zero) {
					pidl = Misc.PidlFromString(file);
					if(pidl == Zero) return Zero;
					freePidl = true;
				}

				IntPtr pidlItem;
				int hr = Api.SHBindToParent(pidl, ref Api.IID_IShellFolder, out folder, out pidlItem);
				if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }

				object o;
				hr = folder.GetUIObjectOf(Wnd0, 1, &pidlItem, Api.IID_IExtractIcon, Zero, out o);
				//if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
				if(0 != hr) {
					if(hr == Api.REGDB_E_CLASSNOTREG) return Zero;
					OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}");
					return Zero;
				}
				eic = o as Api.IExtractIcon;

				var sb = new StringBuilder(300); int ii; uint fl;
				hr = eic.GetIconLocation(0, sb, 300, out ii, out fl);
				if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
				string loc = sb.ToString();

				if((fl & (Api.GIL_NOTFILENAME | Api.GIL_SIMULATEDOC)) != 0 || 1 != Api.PrivateExtractIcons(loc, ii, size, size, out R, Zero, 1, 0)) {
					IntPtr* hiSmall = null, hiBig = null;
					if(size < 24) { hiSmall = &R; size = 32; } else hiBig = &R;
					hr = eic.Extract(loc, (uint)ii, hiBig, hiSmall, Calc.MakeUint(size, 16));
					if(0 != hr) { OutDebug($"{file}, {Marshal.GetExceptionForHR(hr).Message}"); return Zero; }
				}
			}
			catch(Exception e) { OutDebug($"Exception in _Icon_GetSpec: {file}, {e.Message}, {e.TargetSite}"); }
			finally {
				if(eic != null) Marshal.ReleaseComObject(eic);
				if(folder != null) Marshal.ReleaseComObject(folder);
				if(freePidl) Marshal.FreeCoTaskMem(pidl);
			}
			return R;
		}

	}
}
