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
	/// <summary>
	/// Flags of window triggers.
	/// </summary>
	[Flags]
	public enum TWFlags : byte
	{
		/// <summary>
		/// Run at startup (of <see cref="AuTriggers.Run">Triggers.Run</see>), if the window then is active (for ActiveOnce etc triggers) or visible (for VisibleOnce etc triggers).
		/// </summary>
		RunAtStartup = 1,

		/// <summary>
		/// When using the <i>later</i> parameter, call the currently active <b>Triggers.FuncOf</b> functions on "later" events too.
		/// If the function returns false, the action will not run.
		/// The function runs synchronously in the same thread that called <b>Triggers.Run</b>. The action runs asynchronously in another thread, which is slower to start.
		/// As always, <b>Triggers.FuncOf</b> functions must not execute slow code; should take less than 10 ms.
		/// </summary>
		LaterCallFunc = 2,

		//rejected. If need, better let the script do it.
		//WaitVisible = ,
		//WaitIdle = ,
		//WaitFocusedControl = ,
	}

	/// <summary>
	/// Window events for the <i>later</i> parameter of window triggers.
	/// </summary>
	[Flags]
	public enum TWEvents : ushort
	{
		/// <summary>
		/// Name changed.
		/// This event occurs only when the window is active. If name changed while inactive - when activated.
		/// </summary>
		Name = 1,

		/// <summary>
		/// Destroyed (closed).
		/// </summary>
		Destroyed = 2,

		/// <summary>
		/// Activated (became the foreground window).
		/// </summary>
		Active = 4,

		/// <summary>
		/// Deactivated (lose the foreground window status).
		/// This event also occurs when closing the window, if it was active; then the window possibly is already destroyed, and the handle is invalid.
		/// </summary>
		Inactive = 8,

		/// <summary>
		/// Became visible.
		/// The window can be new or was temporarily hidden.
		/// This event occurs when changed the <see cref="Wnd.IsVisible"/> property, and not when changed the <see cref="Wnd.IsCloaked"/> property, therefore the window is not actually visible if cloaked.
		/// </summary>
		Visible = 16,

		/// <summary>
		/// Became invisible.
		/// This event also occurs when closing the window, if it was visible; then the window possibly is already destroyed, and the handle is invalid.
		/// This event occurs when changed the <see cref="Wnd.IsVisible"/> property, and not when changed the <see cref="Wnd.IsCloaked"/> property.
		/// </summary>
		Invisible = 32,

		/// <summary>
		/// The window has been cloaked. See <see cref="Wnd.IsCloaked"/>.
		/// </summary>
		Cloaked = 64,

		/// <summary>
		/// The window has been uncloaked. See <see cref="Wnd.IsCloaked"/>.
		/// </summary>
		Uncloaked = 128,

		/// <summary>
		/// The window has been minimized.
		/// </summary>
		Minimized = 0x100,

		/// <summary>
		/// The window has been restored from the minimized state.
		/// </summary>
		Unminimized = 0x200,

		/// <summary>
		/// The user started to move or resize the window.
		/// This event does not occur when the window is moved/resized not by the user.
		/// </summary>
		MoveSizeStart = 0x400,

		/// <summary>
		/// The user finished (or canceled) to move or resize the window.
		/// This event does not occur when the window is moved/resized not by the user.
		/// </summary>
		MoveSizeEnd = 0x800,

		//CONSIDER: Location. Use timer or thread-specific AccEVENT.OBJECT_LOCATIONCHANGE.
		//CONSIDER: Focus (when eg a child control focused). Use timer or thread-specific AccEVENT.OBJECT_FOCUSED.
		//CONSIDER: Timer.
		//CONSIDER: StoppingTriggers. Or Finally.
	}

	enum _Once : byte { Always, Once, New } //Active, ActiveOnce, ActiveNew, and the same for VisibleX.

	/// <summary>
	/// Represents a window trigger.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// Triggers.Window.ActiveNew["Window name"] = o => Print(o.Window);
	/// var v = Triggers.Last.Window; //v is the new WindowTrigger
	/// ]]></code>
	/// </example>
	public class WindowTrigger : Trigger
	{
		internal readonly Wnd.Finder finder;
		internal readonly TWEvents later;
		internal readonly TWFlags flags;
		internal readonly _Once once;

		internal WindowTrigger(AuTriggers triggers, Action<WindowTriggerArgs> action, Wnd.Finder finder, TWFlags flags, TWEvents later, _Once once) : base(triggers, action, false)
		{
			this.finder = finder;
			this.flags = flags;
			this.later = later;
			this.once = once;
		}

		internal override void Run(TriggerArgs args) => RunT(args as WindowTriggerArgs);

		/// <summary>
		/// Returns "Window".
		/// </summary>
		public override string TypeString() => "Window";

		/// <summary>
		/// Returns a short trigger string.
		/// </summary>
		public override string ShortString() => finder.ToString(); //TODO
	}

	/// <summary>
	/// Window triggers.
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// var wt = Triggers.Window; //wt is the WindowTriggers instance of Triggers
	/// wt.ActiveNew["Window name"] = o => Print(o.Window);
	/// wt.Visible["Window2 name"] = o => Print(o.Window);
	/// Triggers.Run();
	/// ]]></code>
	/// </example>
	public class WindowTriggers : ITriggers
	{
		AuTriggers _triggers;
		bool _win10, _win8;

		internal WindowTriggers(AuTriggers triggers)
		{
			_triggers = triggers;
			_win10 = Ver.MinWin10;
			_win8 = Ver.MinWin8;
			Active = new Subtype(this, 0, _Once.Always);
			ActiveOnce = new Subtype(this, 0, _Once.Once);
			ActiveNew = new Subtype(this, 0, _Once.New);
			Visible = new Subtype(this, 1, _Once.Always);
			VisibleOnce = new Subtype(this, 1, _Once.Once);
			VisibleNew = new Subtype(this, 1, _Once.New);
		}

		#region subtypes

		/// <summary>
		/// Triggers that execute the action when the specified window becomes active (each time).
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.Active["Window name"] = o => Print(o.Window);
		/// ]]></code>
		/// </example>
		public Subtype Active { get; }

		/// <summary>
		/// Triggers that execute the action when the specified window becomes active the first time in the trigger's life.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.ActiveOnce["Window name"] = o => Print(o.Window);
		/// ]]></code>
		/// </example>
		public Subtype ActiveOnce { get; }

		/// <summary>
		/// Triggers that execute the action when the specified window is created and then becomes active.
		/// </summary>
		/// <remarks>
		/// The same as <see cref="ActiveOnce"/>, but windows created before calling <see cref="AuTriggers.Run">Triggers.Run</see>) are ignored.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.ActiveNew["Window name"] = o => Print(o.Window);
		/// ]]></code>
		/// </example>
		public Subtype ActiveNew { get; }

		/// <summary>
		/// Triggers that execute the action when the specified window becomes visible (each time).
		/// </summary>
		/// <remarks>
		/// Cloaked windows are considered invisible. See <see cref="Wnd.IsCloaked"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.Visible["Window name"] = o => Print(o.Window);
		/// ]]></code>
		/// </example>
		public Subtype Visible { get; }

		/// <summary>
		/// Triggers that execute the action when the specified window becomes visible the first time in the trigger's life.
		/// </summary>
		/// <remarks>
		/// Cloaked windows are considered invisible. See <see cref="Wnd.IsCloaked"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.VisibleOnce["Window name"] = o => Print(o.Window);
		/// ]]></code>
		/// </example>
		public Subtype VisibleOnce { get; }

		/// <summary>
		/// Triggers that execute the action when the specified window is created and then becomes visible.
		/// </summary>
		/// <remarks>
		/// The same as <see cref="VisibleOnce"/>, but windows created before calling <see cref="AuTriggers.Run">Triggers.Run</see>) are ignored.
		/// Cloaked windows are considered invisible. See <see cref="Wnd.IsCloaked"/>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.VisibleNew["Window name"] = o => Print(o.Window);
		/// ]]></code>
		/// </example>
		public Subtype VisibleNew { get; }

		/// <tocexclude />
		/// <remarks>Infrastructure.</remarks>
		public class Subtype
		{
			internal readonly WindowTriggers winTriggers;
			internal readonly int subtype;
			internal readonly _Once once;

			internal Subtype(WindowTriggers winTriggers, int subtype, _Once once)
			{
				this.winTriggers = winTriggers;
				this.subtype = subtype;
				this.once = once;
			}

			/// <summary>
			/// Adds (registers) a window trigger and its action.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="cn"></param>
			/// <param name="program"></param>
			/// <param name="also"></param>
			/// <param name="contains"></param>
			/// <param name="flags"></param>
			/// <param name="later">
			/// Can optionally specify one or more additional events to watch.
			/// The watching starts from the moment the primary trigger is activated, and only for that window.
			/// For example, to be notified when the window is closed or renamed, specify <c>later: TWEvents.Destroyed | TWEvents.Name</c>.
			/// When a "later" event occurs, the trigger action is executed. The <see cref="WindowTriggerArgs.Later"/> property then is that event; it is 0 when it is the primary trigger.
			/// The "later" trigers are not disabled when primary triggers are disabled.
			/// </param>
			/// <returns>The return value is not used. See also: <see cref="AuTriggers.Last"/>.</returns>
			/// <remarks>
			/// The first 5 parameters are the same as with <see cref="Wnd.Find"/>.
			/// </remarks>
			public Action<WindowTriggerArgs> this[
				string name = null, string cn = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null,
				TWFlags flags = 0, TWEvents later = 0
				] {
				set {
					var f = new Wnd.Finder(name, cn, program, 0, also, contains);
					winTriggers._Add(this, value, f, flags, later);
				}
			}

			/// <inheritdoc cref="this[string, string, WF3, Func{Wnd, bool}, object, TWFlags, TWEvents]"/>
			/// <remarks>
			/// This overload uses a <see cref="Wnd.Finder"/> to specify window properties.
			/// </remarks>
			public Action<WindowTriggerArgs> this[Wnd.Finder f, TWFlags flags = 0, TWEvents later = 0] {
				set {
					winTriggers._Add(this, value, f, flags, later);
				}
			}
		}

		#endregion

		void _Add(Subtype x, Action<WindowTriggerArgs> action, Wnd.Finder f, TWFlags flags, TWEvents later)
		{
			_triggers.LibThrowIfRunning();
			var t = new WindowTrigger(_triggers, action, f, flags, later, x.once);
			ref var last = ref _tActive; if(x.subtype == 1) last = ref _tVisible;
			if(last == null) {
				last = t;
				last.next = last;
			} else {
				t.next = last.next; //first
				last.next = t;
				last = t;
			}
			_hasTriggers = true;
			_laterEvents |= later;
		}

		bool ITriggers.HasTriggers => _hasTriggers;
		bool _hasTriggers;

		WindowTrigger _tActive, _tVisible; //null or last trigger in linked list
		TWEvents _laterEvents, _allEvents;
		bool _needVisibleArray; //0 != (_allEvents & (TWEvents.Visible | TWEvents.Invisible))

		unsafe void ITriggers.StartStop(bool start)
		{
			if(start) {
				if(_enumWinProc == null) {
					_enumWinProc = _EnumWinProc;
					_aTriggered.a = new Wnd[100];
					_aTriggeredData = new _TriggeredData[100];
					_aVisible.a = new Wnd[100];
					_aVisibleOld.a = new Wnd[100];
					_hsOld = new HashSet<Wnd>(1000);
					_hsSeenActivating = new HashSet<Wnd>(50);
					_winPropCache = new WFCache { CacheName = true, NoTimeout = true, IgnoreVisibility = true };
				} else {
					Array.Clear(_aTriggeredData, 0, _aTriggered.len);
					_aTriggered.len = _aVisible.len = 0;
					_hsOld.Clear();
					_hsSeenActivating.Clear();
					_winPropCache.Clear();
				}

				_allEvents = _laterEvents | TWEvents.Active | TWEvents.Name;
				if(_tVisible != null) _allEvents |= TWEvents.Visible | TWEvents.Uncloaked;
				_needVisibleArray = 0 != (_allEvents & (TWEvents.Visible | TWEvents.Invisible));

				var ah = new AccEVENT[7];
				ah[0] = AccEVENT.SYSTEM_FOREGROUND;
				if(_win8) {
					if(0 != (_allEvents & TWEvents.Uncloaked)) ah[1] = AccEVENT.OBJECT_UNCLOAKED;
					if(0 != (_allEvents & TWEvents.Cloaked)) ah[2] = AccEVENT.OBJECT_CLOAKED;
				}
				if(0 != (_allEvents & TWEvents.Minimized)) ah[3] = AccEVENT.SYSTEM_MINIMIZESTART;
				if(0 != (_allEvents & TWEvents.Unminimized)) ah[4] = AccEVENT.SYSTEM_MINIMIZEEND;
				if(0 != (_allEvents & TWEvents.MoveSizeStart)) ah[5] = AccEVENT.SYSTEM_MOVESIZESTART;
				if(0 != (_allEvents & TWEvents.MoveSizeEnd)) ah[6] = AccEVENT.SYSTEM_MOVESIZEEND;
				_hooks = new Util.AccHook(ah, _HookProc);
				_hookEventQueue = new Queue<(AccEVENT, int)>();

				_triggers.LibWinTimerPeriod = 250;
				_threadId = Thread_.NativeId;

				Api.IVirtualDesktopManager dm = null;
				Api.EnumWindows((w, param) => {
					if(w.IsVisible) {
						if(_needVisibleArray) _aVisible.Add(w);
						//skip empty winstore hosts that later probably will be used as new windows. Speed: ~100 mcs, first time ~10 ms.
						if(_win10 && w.HasExStyle(WS_EX.NOREDIRECTIONBITMAP) && w.IsCloaked && w.ClassNameIs("ApplicationFrameWindow")) {
							//is it a window in an inactive virtual desktop? In both cases it does not have a child Windows.UI.Core.CoreWindow.
							if(dm == null) dm = new Api.VirtualDesktopManager() as Api.IVirtualDesktopManager;
							if(0 == dm.GetWindowDesktopId(w.Get.RootOwnerOrThis(), out var guid) && guid == default) {
								//Print(w);
								return 1;
							}
						}
					}
					_hsOld.Add(w);
					return 1;
				});
				Api.ReleaseComObject(dm);

				_wActive = default; _nameActive = null;

				//run trigers that have flag TWFlags.RunAtStartup
				for(int i = 0; i < _aVisible.len; i++) _Proc(TWEvents.Visible, _aVisible.a[i], _ProcCaller.Startup);
				var wa = Wnd.Active;
				if(!wa.Is0) _Proc(TWEvents.Active, wa, _ProcCaller.Startup);
			} else {
				_hooks?.Dispose();
				_hooks = null;
				_hookEventQueue = null;
				_threadId = 0;
			}
		}

		Util.AccHook _hooks;
		Queue<(AccEVENT, int)> _hookEventQueue;
		WFCache _winPropCache;
		int _threadId;

		struct _TriggeredData
		{
			public object triggered; //WindowTrigger or List<WindowTrigger>
			public string name; //for Name triggers
		}

		_WndArray _aTriggered, _aVisible, _aVisibleOld;
		_TriggeredData[] _aTriggeredData;
		HashSet<Wnd> _hs1, _hs2; //to find added and removed visible windows
		HashSet<Wnd> _hsOld, _hsSeenActivating;
		Wnd _wActive;
		string _nameActive;
		int _timerCounter10;

		/// <summary>
		/// Called from the message loop every 255 or less ms.
		/// </summary>
		internal unsafe void LibTimer()
		{
			bool print = !Keyb.IsNumLock;
			//if(print) Print(Time.PerfMilliseconds % 10000);

			int period = _triggers.LibWinTimerPeriod;
			if(period < 250) _triggers.LibWinTimerPeriod = Math.Min(period += period / 10 + 1, 250);

			//bool verbose = !Keyb.IsNumLock;
			//if(Keyb.IsNumLock) return;
			Perf.First();
			//var a = Wnd.GetWnd.AllWindows(true);

			//Wnd.GetWnd.AllWindows(ref _listVisible, true);

			//Array.Sort(a);

			//using(var a=Wnd.Lib.EnumWindows2(Wnd.Lib.EnumAPI.EnumWindows, true)) {

			//}

			//if(_aVisibleOld != null) {
			//	//var added = a.Except(_aVisibleOld);
			//	//var removed = _aVisibleOld.Except(a);
			//	//int n1 = added.Count(), n2 = removed.Count();



			//	//if(n1 + n2 > 0) Print(n1, n2);
			//}
			//_aVisibleOld = a;
			//Debug_.LibMemoryPrint();

			//bool needEW = false;
			//if(Visible.HasTriggers) needEW = true;
			//else if(Active.HasTriggers) { if(++_timerCounterActiveEW >= 20) { _timerCounterActiveEW = 0; needEW = true; } }

			if(_needVisibleArray) {

				//Print(Time.PerfMilliseconds % 10000);

				var t1 = _aVisibleOld; _aVisibleOld = _aVisible; _aVisible = t1;
				_aVisible.len = 0;
				Api.EnumWindows(_enumWinProc);
				Perf.Next();
				_VisibleAddedRemoved();

				//Perf.NW();
				//speed with  3 main windows: 200, +IsVisible 270, +IsCloaked 450, +Name-IsCloaked 444
				//speed with 20 main windows (79 visible, 541 total): 320, +IsVisible 430 (CPU 0.05). With 480 CPU 0.06, sometimes 0.05.
				//Print(n);
				//Print(_listVisible.Count);
			}

			var a = _aTriggered.a;
			for(int i = 0; i < _aTriggered.len; i++) {
				if(!Api.IsWindow(a[i])) {
					_ProcLater(TWEvents.Destroyed, a[i], i);
					int last = --_aTriggered.len;
					a[i] = a[last];
					_aTriggeredData[i] = _aTriggeredData[last];
					_aTriggeredData[last] = default;
				}
			}

			if(++_timerCounter10 >= 10) { //execute this code every 2.5 s
				_timerCounter10 = 0;
				_hsOld.RemoveWhere(o => !Api.IsWindow(o));
				_hsSeenActivating.RemoveWhere(o => !Api.IsWindow(o));
				_winPropCache.Clear();
			}

			var w = Wnd.Active;
			if(w.Is0) {
				if(!_wActive.Is0) _ProcLater(TWEvents.Inactive, _wActive);
				_wActive = default; _nameActive = null;
			} else if(w != _wActive) {
				_Proc(TWEvents.Active, w);
			} else {
				var name = w.LibNameTL;
				if(name != null && name != _nameActive) {
					_nameActive = name;
					_Proc(TWEvents.Name, w, name: name);
				}
			}
		}

		/// <summary>
		/// Callback of EnumWindows used by LibTimer to get visible windows.
		/// </summary>
		Api.WNDENUMPROC _enumWinProc;
		unsafe int _EnumWinProc(Wnd w, void* _)
		{
			if(w.IsVisible) _aVisible.Add(w);
			return 1;
		}

		/// <summary>
		/// Called by LibTimer when it swaps _aVisible with _aVisibleOld and calls EnumWindows to populate _aVisible with visible windows.
		/// Finds what windows became visible or invisible and runs Visible/Invisible triggers for them.
		/// </summary>
		void _VisibleAddedRemoved()
		{
			//Perf.First();
			Wnd[] aNew = _aVisible.a, aOld = _aVisibleOld.a;
			int nNew = _aVisible.len, nOld = _aVisibleOld.len;
			int to = Math.Min(nOld, nNew), diffFrom;
			for(diffFrom = 0; diffFrom < to; diffFrom++) if(aNew[diffFrom] != aOld[diffFrom]) break;
			if(nNew == nOld && diffFrom == nNew) return;
			int diffTo1 = nOld, diffTo2 = nNew;
			while(diffTo1 > 0 && diffTo2 > 0) if(aNew[--diffTo2] != aOld[--diffTo1]) { diffTo1++; diffTo2++; break; }
			//Print(diffFrom, diffTo1, diffTo2, Math.Max(diffTo1 - diffFrom, diffTo2 - diffFrom));
			int n1 = diffTo1 - diffFrom, n2 = diffTo2 - diffFrom;
			//Print($"from={diffFrom} to1={diffTo1} to2={diffTo2}    n1={n1} n2={n2}  n1*n2={n1*n2}");
			if(n1 == 0) { //only added
				if(0 != (_allEvents & TWEvents.Visible)) {
					for(int i = diffFrom; i < diffTo2; i++) _Added(i);
				}
			} else if(n2 == 0) { //only removed
				if(0 != (_allEvents & TWEvents.Invisible)) {
					for(int i = diffFrom; i < diffTo1; i++) _Removed(i);
				}
			} else { //reordered or/and added/removed
					 //Print($"n1={n1} n2={n2}  n1*n2={n1 * n2}");
					 //Perf.Next();
					 //SHOULDDO: optimize. Now slow when large array reordered. Eg divide the changed range into two.
					 //n1 = n2 = 0;
				if(0 != (_allEvents & TWEvents.Invisible)) {
					if(_hs2 == null) _hs2 = new HashSet<Wnd>(500); else _hs2.Clear();
					for(int i = diffFrom; i < diffTo2; i++) _hs2.Add(aNew[i]);
					for(int i = diffFrom; i < diffTo1; i++) if(!_hs2.Remove(aOld[i])) _Removed(i);
				}
				if(0 != (_allEvents & TWEvents.Visible)) {
					if(_hs1 == null) _hs1 = new HashSet<Wnd>(500); else _hs1.Clear();
					for(int i = diffFrom; i < diffTo1; i++) _hs1.Add(aOld[i]);
					for(int i = diffFrom; i < diffTo2; i++) if(!_hs1.Remove(aNew[i])) _Added(i);
				}
				//Perf.NW();
			}
			//if(n1 + n2 > 0) Print($"<><z yellow>added {n2}, removed {n1}<>");

			void _Added(int i)
			{
				//n2++; //Print("added", aNew[i]);
				_Proc(TWEvents.Visible, aNew[i]);
			}

			void _Removed(int i)
			{
				//n1++; //Print("removed", aOld[i]);
				_ProcLater(TWEvents.Invisible, aOld[i]);
			}
		}

		/// <summary>
		/// AccHook hook procedure.
		/// </summary>
		/// <param name="k"></param>
		void _HookProc(HookData.AccHookData k)
		{
			//if(!Keyb.IsNumLock) Print(k.ev, k.idObject, k.idChild, k.wnd);

			if(k.idObject != AccOBJID.WINDOW) return;
			if(k.idChild != 0 || k.wnd.Is0) return;

			var accEvent = k.ev;
			var w = k.wnd;

			if(w.IsChild) {
				//if(accEvent == AccEVENT.OBJECT_UNCLOAKED) { } //rejected: wait for winstore app host child window
				return;
			}

			if(_inProc) {
				_hookEventQueue.Enqueue((accEvent, (int)w));
				//Print(_hookEventQueue.Count);
				Debug_.PrintIf(_hookEventQueue.Count > 4, "_hookEventQueue.Count=" + _hookEventQueue.Count);
				return;
			}
			_inProc = true;
			try {
				//Time.SleepDoEvents(300); //test queue
				//Perf.Cpu();
				for(bool queed = false; ; queed = true) {
					TWEvents e = 0;
					switch(accEvent) {
					case AccEVENT.SYSTEM_FOREGROUND:
						//Debug_.PrintIf(!w.IsActive, $"{Time.PerfMilliseconds % 10000}, SYSTEM_FOREGROUND but not active: {w}"); //it is normal. The window either will be active soon (and timer will catch it), or will not be activated (eg prevented by Windows, hooks, etc), or another window became active.
						if(w != _wActive && w.IsActive) e = TWEvents.Active;
						break;
					case AccEVENT.OBJECT_UNCLOAKED: e = TWEvents.Uncloaked; break;
					case AccEVENT.OBJECT_CLOAKED: e = TWEvents.Cloaked; break;
					case AccEVENT.SYSTEM_MINIMIZESTART: e = TWEvents.Minimized; break;
					case AccEVENT.SYSTEM_MINIMIZEEND: e = TWEvents.Unminimized; break;
					case AccEVENT.SYSTEM_MOVESIZESTART: e = TWEvents.MoveSizeStart; break;
					case AccEVENT.SYSTEM_MOVESIZEEND: e = TWEvents.MoveSizeEnd; break;
					}

					if(e != 0) _Proc(e, w, _ProcCaller.Hook, queed: queed);

					//Normal timer period is 255. It is optimized for quite small CPU usage and quite good response time.
					//To improve response time, we temporarily set smaller period on SYSTEM_FOREGROUND. Need it when the window is not active too.
					//If activating a new window first time, set period = 15, to quickly catch Visible and Name events.
					//Else set period = 105, for Name events after closing an "Open File" dialog (then usually its owner becomes active and then changes name).
					if(accEvent == AccEVENT.SYSTEM_FOREGROUND) {
						bool fast = _hsSeenActivating.Add(w) && !_hsOld.Contains(w);
						//Print("fast timer", fast);
						_triggers.LibWinTimerPeriod = fast ? 1 : 100;
					}

					if(_hookEventQueue.Count == 0) break;
					var q = _hookEventQueue.Dequeue();
					accEvent = q.Item1;
					w = (Wnd)q.Item2;
				}
				_inProc = false;
			}
			finally {
				if(_inProc) { _inProc = false; _hookEventQueue.Clear(); } //exception. Unlikely, because we handle script exceptions, except thread abort.
			}
		}
		bool _inProc;

		enum _ProcCaller { Timer, Hook, Run, Startup }

		/// <summary>
		/// Processes events for main triggers (active, visible) and most "later" triggers.
		/// Called from hook (_HookProc), timer (LibTimer), at startup (StartStop) and SimulateActiveNew/SimulateVisibleNew.
		/// </summary>
		void _Proc(TWEvents e, Wnd w, _ProcCaller caller = _ProcCaller.Timer, string name = null, bool queed = false)
		{
			//e can be:
			//Used for main triggers (Active, Visible) and other triggers:
			//	Active - from SYSTEM_FOREGROUND hook or from timer. Now the window is active and not 0.
			//	Name - from timer, and only when the window is active.
			//	Visible - from timer.
			//	Uncloaked - from OBJECT_UNCLOAKED hook.
			//Used only for other triggers:
			//	Cloaked - from OBJECT_CLOAKED hook.
			//	Minimized from SYSTEM_MINIMIZESTART hook.
			//	Unminimized from SYSTEM_MINIMIZEEND hook.
			//	MoveSizeStart from SYSTEM_MOVESIZESTART hook.
			//	MoveSizeEnd from SYSTEM_MOVESIZEEND hook.
			Debug.Assert(e == TWEvents.Active || e == TWEvents.Name || e == TWEvents.Visible || e == TWEvents.Uncloaked || e == TWEvents.Cloaked || e == TWEvents.Minimized || e == TWEvents.Unminimized || e == TWEvents.MoveSizeStart || e == TWEvents.MoveSizeEnd);

			bool callerHT = caller == _ProcCaller.Hook || caller == _ProcCaller.Timer;
			int iTriggered = _aTriggered.Find(w);

			bool runActive = false,
				isVisible = w.IsVisible,
				detectedVisibleNow = e != TWEvents.Visible && _needVisibleArray && isVisible && callerHT && _aVisible.Find(w) < 0;

			if(detectedVisibleNow) {
				_aVisible.Add(w);
				_ProcLater(TWEvents.Visible, w, iTriggered);
			}

			if(e == TWEvents.Active) {
				name = w.LibNameTL;
				if(callerHT) {
					_ProcLater(TWEvents.Name, w, iTriggered, name); //maybe name changed while inactive
					if(!_wActive.Is0) _ProcLater(TWEvents.Inactive, _wActive);
				}
				_wActive = w; _nameActive = name;
				if(callerHT) _ProcLater(e, w, iTriggered);
				runActive = _tActive != null;
			} else {
				if(callerHT) _ProcLater(e, w, iTriggered, name);
				switch(e) {
				case TWEvents.Name: //from timer, when active window name changed
					runActive = _tActive != null;
					break;
				case TWEvents.Visible:
				case TWEvents.Uncloaked:
					name = w.LibNameTL;
					break;
				default: return;
				}
			}

			bool runVisible = _tVisible != null && isVisible && !w.IsCloaked;
			if(!(runVisible | runActive)) return;

			bool oldWindow = callerHT && _hsOld.Contains(w);

			if(_log && callerHT && (_logSkip == null || !_logSkip(w))) {
				if(detectedVisibleNow) _LogEvent(TWEvents.Visible, w, caller, queed, oldWindow);
				_LogEvent(e, w, caller, queed, oldWindow);
			}

			object triggered = iTriggered >= 0 ? _aTriggeredData[iTriggered].triggered : null, triggeredOld = triggered;

			_winPropCache.Begin(w); //info: don't need to call Clear now. Timer calls it every 2.5 s.
			_winPropCache.Name = name;

			//Perf.First();
			WindowTriggerArgs args = null;

			//Print(runVisible,runActive,w.IsActive, oldWindow);

			if(runVisible) _Do(true, runActive && !detectedVisibleNow);
			if(runActive && w.IsActive) _Do(false, e != TWEvents.Active);

			void _Do(bool visible, bool secondaryEvent)
			{
				Trigger last = visible ? _tVisible : _tActive, v = last;
				if(last == null) return;

				do {
					v = v.next;
					if(v.DisabledThisOrAll) continue;
					var t = v as WindowTrigger;

					if(caller == _ProcCaller.Startup && 0 == (t.flags & TWFlags.RunAtStartup)) continue;
					if(oldWindow && (t.once == _Once.New || secondaryEvent)) continue;
					if(triggered != null && (t.once != _Once.Always || secondaryEvent)) {
						switch(triggered) {
						case WindowTrigger wt1: if(wt1 == t) continue; break;
						case List<WindowTrigger> awt1: if(awt1.Contains(t)) continue; break;
						}
					}

					try {
						if(!t.finder.IsMatch(w, _winPropCache)) continue;
					}
					catch(Exception ex) when(!(ex is ThreadAbortException)) {
						Print(ex);
						continue;
					}
					if(args == null) args = new WindowTriggerArgs(t, w, 0);
					if(!t.CallFunc(args)) continue; //info: handles exceptions

					switch(triggered) {
					case null: triggered = t; break;
					case WindowTrigger wt1: if(wt1 != t) triggered = new List<WindowTrigger> { wt1, t }; break;
					case List<WindowTrigger> awt1: if(!awt1.Contains(t)) awt1.Add(t); break;
					}

					//if(!visible && !w.IsActive) break; //no

					if(_log) Print("Window trigger " + (visible ? "Visible" : "Active") + (t.once == _Once.New ? "New" : (t.once == _Once.Once ? "Once" : "")));

					_triggers.LibRunAction(t, args);
				} while(v != last);
				//Perf.NW(); //speed ms/triggers when cold CPU: ~1/1000, 10/10000, 50/100000, 130/1000000
			}

			if(triggered != triggeredOld) {
				if(iTriggered < 0) {
					iTriggered = _aTriggered.len;
					_aTriggered.Add(w);
					if(iTriggered == _aTriggeredData.Length) Array.Resize(ref _aTriggeredData, iTriggered * 2);
					_aTriggeredData[iTriggered].name = name; //later managed by _Proc2
				}
				_aTriggeredData[iTriggered].triggered = triggered;
			}
		}

		/// <summary>
		/// Called to process "later" events from _Proc and timer.
		/// iTriggered is w index in _aTriggered, or -1 if not found, or -2 (default) to let this func find.
		/// </summary>
		void _ProcLater(TWEvents e, Wnd w, int iTriggered = -2, string name = null)
		{
			if(0 == (_laterEvents & e)) return;
			if(iTriggered < -1) iTriggered = _aTriggered.Find(w);
			if(iTriggered < 0) return;

			if(e == TWEvents.Name) { //called in 2 cases: 1. From timer, when name changed. 2. From _Proc on Active event (hook or timer).
				if(name == null || _aTriggeredData[iTriggered].name == name) return;
				_aTriggeredData[iTriggered].name = name;
			}

			if(_log) Print($"\t{Time.PerfMilliseconds % 10000,4}, {e,-12}  {w}");

			WindowTriggerArgs args = null;
			var triggered = _aTriggeredData[iTriggered].triggered;
			var a = triggered as List<WindowTrigger>;
			int n = a?.Count ?? 1;
			for(int i = 0; i < n; i++) {
				var t = a?[i] ?? (triggered as WindowTrigger);
				if(0 == (t.later & e)) continue;
				//if(t.DisabledThisOrAll) continue; //no
				if(args == null) args = new WindowTriggerArgs(t, w, e);
				if(0 != (t.flags & TWFlags.LaterCallFunc)) {
					if(!t.CallFunc(args)) continue;
				}
				_triggers.LibRunAction(t, args);
			}
		}

		/// <summary>
		/// Simulates event "activated new window" as if the the specified window is that window.
		/// </summary>
		/// <exception cref="InvalidOperationException">Cannot be before or after <b>Triggers.Run</b>.</exception>
		/// <remarks>
		/// This function usually is used to run <b>ActiveNew</b> triggers for a window created before <see cref="AuTriggers.Run">Triggers.Run</see>. Here "run triggers" means "compare window properties etc with those specified in triggers and run actions of triggers that match". Normally such triggers don't run because the window is considered old. This function runs triggers as it was a new window. Triggers like ActiveNew and ActiveOnce will run once, as usually.
		/// This function must be called while the main triggers thread is in <b>Triggers.Run</b>, for example from another trigger action. It is asynchronous (does not wait).
		/// If you call this function from another trigger action (hotkey etc), make sure the window trigger action runs in another thread or can be queed. Else both actions cannot run simultaneously. See example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Options.RunActionInNewThread(true);
		/// Triggers.Window.ActiveNew["* Notepad"] = o => o.Window.Resize(500, 200);
		/// Triggers.Hotkey["Ctrl+T"] = o => Triggers.Window.SimulateActiveNew(Wnd.Active);
		/// Triggers.Hotkey["Ctrl+Alt+T"] = o => Triggers.Stop();
		/// Triggers.Run();
		/// ]]></code>
		/// </example>
		public void SimulateActiveNew(Wnd w) => _SimulateNew(TWEvents.Active, w);

		/// <summary>
		/// Simulates event "visible new window" as if the specified window is that window.
		/// Similar to <see cref="SimulateActiveNew"/>.
		/// </summary>
		/// <param name="w"></param>
		/// <exception cref="InvalidOperationException">Cannot be before or after <b>Triggers.Run</b>.</exception>
		public void SimulateVisibleNew(Wnd w) => _SimulateNew(TWEvents.Visible, w);

		void _SimulateNew(TWEvents e, Wnd w)
		{
			_triggers.LibThrowIfNotRunning();
			Api.PostThreadMessage(_threadId, Api.WM_USER + 20, (int)e, w.Handle); //calls LibSimulateNew in correct thread
		}

		//Called in correct thread.
		internal void LibSimulateNew(LPARAM wParam, LPARAM lParam)
		{
			var w = (Wnd)lParam;
			if(w.IsAlive) _Proc((TWEvents)(int)wParam, w, _ProcCaller.Run);
		}

		bool _log;
		Func<Wnd, bool> _logSkip;

		/// <summary>
		/// Starts or stops to print window events that can help to create or debug window triggers.
		/// </summary>
		/// <param name="on">Start (true) or stop.</param>
		/// <param name="skip">An optional callback function that can be used to reduce noise, eg skip tooltip windows. Return true to skip that window.</param>
		/// <remarks>
		/// For primary trigger events is logged this info:
		/// <list type="number">
		/// <item>Time milliseconds. Shows only the remainder of dividing by 10 seconds, therefore it starts from 0 again when reached 9999 (9 seconds and 999 milliseconds).</item>
		/// <item>Event (see <see cref="TWEvents"/>).</item>
		/// <item>Letters for window state etc:
		/// <list type="bullet">
		/// <item>A - the window is active.</item>
		/// <item>H - the window is invisible (!<see cref="Wnd.IsVisible"/>).</item>
		/// <item>C - the window is cloaked (<see cref="Wnd.IsCloaked"/>).</item>
		/// <item>O - the window is considered old, ie created before calling <b>Triggers.Run</b>.</item>
		/// <item>T - the even has been detected using a timer, which means slower response time. Else detected using a hook.</item>
		/// <item>Q - the event was queued (now retrieved from the queue) because the hook procedure was called while processing previous events. It means the event may be rather old.</item>
		/// </list>
		/// </item>
		/// <item>Window (handle, class, name, program, rectangle, style, exstyle).</item>
		/// </list>
		/// Colors are used for window event types used for primary triggers: blue if activated; green if became visible; yellow if name changed.
		/// When a primary trigger is activated, prints the primary event type, like ActiveNew.
		/// For "later" events is logged time, event and window. Black, tab-indented. Prints only events that are specified in triggers.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.LogEvents(true, o => 0 != o.ClassNameIs("*tooltip*", "SysShadow", "#32774", "TaskList*"));
		/// ]]></code>
		/// </example>
		public void LogEvents(bool on, Func<Wnd, bool> skip = null)
		{
			_log = on;
			_logSkip = skip;
		}

		/// <summary>
		/// Called by _Proc.
		/// </summary>
		void _LogEvent(TWEvents e, Wnd w, _ProcCaller caller, bool queed, bool oldWindow)
		{
			string col = "0";
			switch(e) {
			case TWEvents.Active: col = "0x0000ff"; break;
			case TWEvents.Visible: case TWEvents.Uncloaked: col = "0x00c000"; break;
			case TWEvents.Name: col = "0xC0C000"; break;
			}
			var T = caller == _ProcCaller.Timer ? "T" : " ";
			var A = w.IsActive ? "A" : " ";
			var H = w.IsVisible ? " " : "H";
			var C = w.IsCloaked ? "C" : " ";
			var O = oldWindow ? "O" : " ";
			var Q = queed ? "Q" : " ";
			Print($"<><c {col}>{Time.PerfMilliseconds % 10000,4}, {e,-12} {T}{A}{H}{C}{O}{Q}, {w}, {w.Style}, {w.ExStyle}</c>");
		}

		/// <summary>
		/// For _aTriggered, _aVisible and _aVisibleOld we use resizable array, not List.
		/// We access elements by index in time-critical code. With List it is much slower.
		/// </summary>
		struct _WndArray
		{
			public Wnd[] a;
			public int len;

			public void Add(Wnd w)
			{
				if(len == a.Length) Array.Resize(ref a, len * 2);
				a[len++] = w;
			}

			public unsafe int Find(Wnd w)
			{
				fixed (Wnd* p = a) {
					for(int i = 0, n = len; i < n; i++) if(p[i] == w) return i;
				}
				return -1;
			}
		}
	}

	/// <summary>
	/// Arguments for trigger actions and <b>Triggers.FuncOf</b> functions of window triggers.
	/// </summary>
	/// <remarks>
	/// In <b>FuncOf</b> functions received as <b>TriggerArgs</b> (base class). Use code like <c>o => { var k = o as WindowTriggerArgs; ... }</c>.
	/// </remarks>
	public class WindowTriggerArgs : TriggerArgs
	{
		/// <summary>
		/// The trigger.
		/// </summary>
		public WindowTrigger Trigger { get; }

		/// <summary>
		/// The window.
		/// </summary>
		public Wnd Window { get; }

		/// <summary>
		/// The "later" event, or 0 if it is the primary trigger (ActiveNew etc). See example.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Triggers.Window.ActiveOnce["*- Notepad", later: TWEvents.Active | TWEvents.Inactive] = o => Print(o.Later, o.Window);
		/// Triggers.Run();
		/// ]]></code>
		/// </example>
		public TWEvents Later { get; }

		internal WindowTriggerArgs(WindowTrigger trigger, Wnd w, TWEvents later)
		{
			Trigger = trigger;
			Window = w;
			Later = later;
		}
	}
}
