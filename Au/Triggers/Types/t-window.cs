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

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	[Flags]
	public enum TWFlags : byte
	{

	}

	public class WindowTrigger : Trigger
	{
		internal readonly Wnd.Finder finder;

		internal WindowTrigger(AuTriggers triggers, Action<WindowTriggerArgs> action, Wnd.Finder finder) : base(triggers, action, false)
		{
			this.finder = finder;
		}

		internal override void Run(TriggerArgs args) => RunT(args as WindowTriggerArgs);

		public override string TypeString() => "Window";

		public override string ShortString() => finder.ToString(); //TODO
	}


	public class WindowTriggers : ITriggers
	{
		struct _Queued
		{
			public int w;
			public AccEVENT e;
		}

		AuTriggers _triggers;
		Util.AccHook _hooks;
		Queue<_Queued> _queue;
		bool _win10;

		internal WindowTriggers(AuTriggers triggers)
		{
			_triggers = triggers;
			_win10 = Ver.MinWin10;
			Active = new WinEvent(this);
			Visible = new WinEvent(this);
			NameChanged = new WinEvent(this);
		}

		public WinEvent Active { get; }

		public WinEvent Visible { get; }

		public WinEvent NameChanged { get; }

		/// <tocexclude />
		/// <remarks>Infrastructure.</remarks>
		public class WinEvent
		{
			readonly WindowTriggers _triggers;
			internal WindowTrigger last;

			internal WinEvent(WindowTriggers triggers) { _triggers = triggers; }

			public Action<WindowTriggerArgs> this[string name = null, string cn = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null] {
				set {
					this[new Wnd.Finder(name, cn, program, WFFlags.HiddenToo, also, contains)] = value;
				}
			}

			public Action<WindowTriggerArgs> this[Wnd.Finder f] {
				set {
					var allTriggers = _triggers._triggers;
					allTriggers.LibThrowIfRunning();
					var t = new WindowTrigger(allTriggers, value, f);
					if(last == null) {
						last = t;
						last.next = last;
					} else {
						t.next = last.next; //first
						last.next = t;
						last = t;
					}
					_triggers._hasTriggers = true;
				}
			}
		}

		bool ITriggers.HasTriggers => _hasTriggers;
		bool _hasTriggers;

		void ITriggers.StartStop(bool start)
		{
			if(start) {
				var ah = new AccEVENT[_win10 ? 4 : 3];
				ah[0] = AccEVENT.OBJECT_SHOW;
				ah[1] = AccEVENT.SYSTEM_FOREGROUND;
				ah[2] = AccEVENT.OBJECT_NAMECHANGE;
				if(_win10) ah[3] = AccEVENT.OBJECT_UNCLOAKED;
				_hooks = new Util.AccHook(ah, _HookProc);
				_queue = new Queue<_Queued>();
			} else if(_hooks != null) {
				_hooks.Dispose();
				_hooks = null;
				_queue = null;
			}
		}

		void _HookProc(HookData.AccHookData k)
		{
			if(k.idObject != AccOBJID.WINDOW || k.idChild != 0 || k.wnd.Is0) return;

			var w = k.wnd; var e = k.aEvent;
			var style = w.Style;

			if(style.Has_(WS.CHILD)) {
				if(e== AccEVENT.OBJECT_UNCLOAKED) {

				}
				Debug_.PrintIf(e == AccEVENT.SYSTEM_FOREGROUND, "SYSTEM_FOREGROUND for child: " + w);
				return;
			}

			if(e == AccEVENT.SYSTEM_FOREGROUND) {
				Debug_.PrintIf(!w.IsActive, "SYSTEM_FOREGROUND but not active: " + w);
			} else {
				//	if(e == AccEVENT.OBJECT_UNCLOAKED && !w.IsVisible) return; //eg IME
				//	if(e == AccEVENT.OBJECT_SHOW && w.IsCloaked) { /*Print("cloaked", e, w);*/ return; }
				Debug_.PrintIf(e == AccEVENT.OBJECT_SHOW && !w.IsVisible && w.IsAlive, "OBJECT_SHOW but not visible: " + w);
			}

			if(_inProc) {
				_queue.Enqueue(new _Queued { e = e, w = (int)w }); //TODO: need a timeout
				Debug_.PrintIf(_queue.Count > 4, "_queue.Count=" + _queue.Count);
				return;
			}
			_inProc = true;
			try {
				//Time.SleepDoEvents(300); //test queue
				//Perf.Cpu();
				_Proc(e, w);
				while(_queue.Count > 0) { var q = _queue.Dequeue(); _Proc(q.e, (Wnd)q.w); }
			}
			finally { _inProc = false; }
		}
		bool _inProc;

		void _Proc(AccEVENT e, Wnd w)
		{
			_triggers.winCache.Clear(onlyName: true);

#if DEBUG
			_triggers.winCache.Begin(w);
			var cn = _triggers.winCache.Class ?? (_triggers.winCache.Class = w.ClassName);
			if(cn == null) return;
			if(0 != cn.Equals_(true, "tooltips_class32", "SysShadow", "#32774", "QM_toolbar", "TaskListThumbnailWnd", "TaskListOverlayWnd")) return;
			if(0 != w.ProgramName.Equals_(true, "devenv.exe", "Au.Tests.exe")) return;
			var es = w.ExStyle;
			bool ret = es.Has_(WS_EX.TRANSPARENT | WS_EX.LAYERED) && es.HasAny_(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE); //tooltips etc
			if(!ret) ret = es.Has_(WS_EX.TOOLWINDOW | WS_EX.NOACTIVATE) && es.Has_(WS_EX.LAYERED); //eg VS code tips
			if(ret) { Print($"<><c 0x80e0e0>    {e}, {w}, {es}</c>"); return; }
			//var re = w.Rect; if(re.Height < 100 && re.Width < 1000) { Print(w.Handle, cn, re, w.Style, w.ExStyle); }
			string tcol = w.IsVisible ? "0" : "0x808080";
			string bcol = Wnd.GetWnd.IsMainWindow(w, true) ? "0xc0ffc0" : "0xf8f8f8";
			Print($"<><z {bcol}><c {tcol}>{Time.PerfMilliseconds % 1000}, {e}, {w}</c></z>");
#endif

			//Perf.First();
			//w.SendTimeout(1000, 0); //nothing good
			//Perf.Next();
			//Print(_ttype, w);
			//Perf.Write();

			if(e == AccEVENT.OBJECT_NAMECHANGE) {
				//if(_timName == null) _timName = new Timer_(() => {

				//	Print($"{Time.PerfMilliseconds%1000} timer");
				//});
				//_timName.Start(200, true);
			} else if(e == AccEVENT.OBJECT_SHOW) {
				if(_tim.th != default) Api.CloseHandle(_tim.th);
				//_tim.th = Api.OpenThread(Api.THREAD_QUERY_LIMITED_INFORMATION, false, w.ThreadId);
				_tim.th = Api.OpenProcess(Api.PROCESS_QUERY_LIMITED_INFORMATION, false, w.ProcessId);
				//Api.GetThreadTimes(_tim.th, out _, out _, out var kt1, out var ut1);
				GetProcessTimes(_tim.th, out _, out _, out var kt1, out var ut1);
				_tim.tt1 = kt1 + ut1;
				_tim.t1 = Time.WinMilliseconds64;
				_tim.n = 0;
				if(_timShow == null) _timShow = new Timer_(() => {
					//Api.GetThreadTimes(_tim.th, out _, out _, out var kt2, out var ut2);
					GetProcessTimes(_tim.th, out _, out _, out var kt2, out var ut2);
					var tt2 = kt2 + ut2;
					var t2 = Time.WinMilliseconds64;
					var td = t2 - _tim.t1;
					var ttd = (tt2 - _tim.tt1) / 10000;
					//if(ttd > td) ttd = td;
					_tim.t1 = t2; _tim.tt1 = tt2;
					Print($"{Time.PerfMilliseconds % 1000} timer, {ttd}/{td}");
					if(++_tim.n > 100) {
						_timShow.Stop();
						Api.CloseHandle(_tim.th); _tim.th = default;
					}
				});
				//_timShow.Start(25, false);
			}

			//Perf.First();
			WindowTriggerArgs args = null;
			WinEvent x = e == AccEVENT.SYSTEM_FOREGROUND ? Active : (e == AccEVENT.OBJECT_SHOW ? Visible : NameChanged);
			Trigger last = x.last, v = last;
			if(last == null) return;
			do {
				v = v.next;
				if(v.DisabledThisOrAll) continue;
				var t = v as WindowTrigger;
				if(!t.finder.IsMatch(w, _triggers.winCache)) continue;
				if(args == null) args = new WindowTriggerArgs(t, w);
				if(!t.MatchScope2(args)) continue; //callback functions
				_triggers.LibRunAction(t, args);
			} while(v != last);
			//Perf.NW(); //speed ms/triggers when cold CPU: ~1/1000, 10/10000, 50/100000, 130/1000000
		}

		Timer_ _timName, _timShow;
		class _Timer
		{
			public int n;
			public long t1, tt1;
			public IntPtr th;
		}
		_Timer _tim = new _Timer();

		[DllImport("kernel32.dll")]
		internal static extern bool GetProcessTimes(IntPtr hProcess, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

		//Wnd _wActive;

		[Flags]
		enum _NWFlags
		{
			Active = 1,
			Visible = 2,
			NameChanged = 4,
			ActiveTriggered = 0x10,
			VisibleTriggered = 0x20,
		}

		struct _NewWnd
		{
			public int w;
			public _NWFlags flags;
			public long time;
		}
		_NewWnd[] _aNew = new _NewWnd[64];

		//void _AddNewWnd(Wnd w)
		//{
		//	long time = Time.WinMilliseconds64;
		//	//find empty or old
		//	int found = -1;
		//	for(int i = 0; i < _anw.Length; i++) if(time - _anw[i].time >= 30000) { found = i; break; }
		//	if(found < 0) { //recently created many windows; find oldest
		//		long age = 0;
		//		for(int i = 0; i < _anw.Length; i++) { var t = time - _anw[i].time; if(t > age) { age = t; found = i; } }
		//	}
		//	_anw[found].w = (int)w; _anw[found].time = time; _anw[found].flags = 0;
		//}

		void _AddNewWnd(Wnd w)
		{
			//find a too old or the oldest
			int found = -1;
			long time = Time.WinMilliseconds64, age = -1;
			for(int i = 0; i < _aNew.Length; i++) {
				var t = time - _aNew[i].time;
				if(t >= 30000) { found = i; break; } //too old
				if(t > age) { age = t; found = i; } //find oldest
			}
			_aNew[found].w = (int)w; _aNew[found].time = time; _aNew[found].flags = 0;
		}

		int _FindNewWnd(Wnd w, _NWFlags flags)
		{
			long time = Time.WinMilliseconds64;
			int wi = (int)w;
			for(int i = 0; i < _aNew.Length; i++) {
				if(_aNew[i].w != wi) continue;
				if(0 != (_aNew[i].flags & flags)) break; //this event not first time
				if(flags != _NWFlags.NameChanged) _aNew[i].flags |= flags; //mark that this event is not first time; except name change, it can be multiple times
				if(time - _aNew[i].time >= (flags == _NWFlags.NameChanged ? 5000 : 30000)) break; //too old
				_aNew[i].time = time;
				return i;
			}
			return -1;
		}

		int _AddWnd(int[] a, Wnd w)
		{
			//find a too old or the oldest
			int found = -1;
			long time = Time.WinMilliseconds64, age = -1;
			for(int i = 0; i < _aNew.Length; i++) {
				var t = time - _aNew[i].time;
				if(t >= 30000) return i; //too old
				if(t > age) { age = t; found = i; } //find the oldest
			}
			return found;
		}

		int _FindWnd(int[] a, Wnd w)
		{

			return -1;
		}

		int[] _aCloaked = new int[16];
	}

	public class WindowTriggerArgs : TriggerArgs
	{
		public WindowTrigger Trigger { get; }
		public Wnd Window { get; }

		internal WindowTriggerArgs(WindowTrigger trigger, Wnd w)
		{
			Trigger = trigger;
			Window = w;
		}
	}
}
