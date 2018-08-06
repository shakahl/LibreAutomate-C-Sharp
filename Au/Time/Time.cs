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

namespace Au
{
	/// <summary>
	/// Time functions. Get time, sleep/wait, doevents.
	/// </summary>
	/// <seealso cref="WaitFor"/>
	[DebuggerStepThrough]
	public static class Time
	{
		static double _freqMCS = 1000000.0 / Stopwatch.Frequency;
		static double _freqMS = 1000.0 / Stopwatch.Frequency;

		/// <summary>
		/// Gets the number of microseconds elapsed since Windows startup.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryPerformanceCounter</msdn>.
		/// Independent of computer clock time changes.
		/// MSDN article: <msdn>Acquiring high-resolution time stamps</msdn>.
		/// </remarks>
		public static long Microseconds => (long)(Stopwatch.GetTimestamp() * _freqMCS);

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryPerformanceCounter</msdn>.
		/// Similar to <see cref="Environment.TickCount"/>, but more precise (1 ms) and returns a 64-bit value.
		/// Independent of computer clock time changes.
		/// </remarks>
		public static long Milliseconds => (long)(Stopwatch.GetTimestamp() * _freqMS);
		//public static long Milliseconds => Api.GetTickCount64(); //15 ms precision. On current OS and hardware, QueryPerformanceCounter is reliable and almost as fast.

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup, not including the sleep/hibernate time.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryUnbiasedInterruptTime</msdn>.
		/// The precision is 1-16 milliseconds.
		/// Independent of computer clock time changes.
		/// </remarks>
		public static long MillisecondsWithoutComputerSleepTime
		{
			get
			{
				if(!Api.QueryUnbiasedInterruptTime(out long t)) return Milliseconds;
				return t / 10000;
			}
		}
		//speed on Win10: GetTickCount64 100%, QueryPerformanceCounter 180%, QueryUnbiasedInterruptTime 130%.

		/// <summary>
		/// Waits <paramref name="timeMilliseconds"/> milliseconds.
		/// </summary>
		/// <param name="timeMilliseconds">Time to wait, milliseconds. Or <see cref="Timeout.Infinite"/>.</param>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process Windows messages and other events, therefore should not be used in threads with windows, timers, hooks, events or COM, unless <paramref name="timeMilliseconds"/> is small. Supports APC.
		/// If the computer goes to sleep or hibernate during that time, the real time is the specified time + the sleep/hibernate time.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeMilliseconds"/> is negative and not Timeout.Infinite (-1).</exception>
		/// <example>
		/// <code><![CDATA[
		/// Time.Sleep(50);
		/// 50.ms(); //the same
		/// 0.05.s(); //the same
		/// ]]></code>
		/// </example>
		public static void Sleep(int timeMilliseconds)
		{
			LibSleepPrecision.LibTempSet1(timeMilliseconds);
			if(timeMilliseconds < 2000) {
				Thread.Sleep(timeMilliseconds);
			} else { //workaround for Thread.Sleep bug: if there are APC, returns too soon after sleep/hibernate.
				g1:
				long t = MillisecondsWithoutComputerSleepTime;
				Thread.Sleep(timeMilliseconds);
				t = timeMilliseconds - (MillisecondsWithoutComputerSleepTime - t);
				if(t >= 500) { timeMilliseconds = (int)t; goto g1; }
			}
		}

		/// <summary>
		/// Waits <paramref name="timeMilliseconds"/> milliseconds. The same as <see cref="Sleep"/>.
		/// </summary>
		/// <inheritdoc cref="Sleep"/>
		public static void ms(this int timeMilliseconds)
		{
			Sleep(timeMilliseconds);
		}

		/// <summary>
		/// Waits <paramref name="timeSeconds"/> seconds.
		/// The same as <see cref="Sleep"/> and <see cref="ms"/>, but the time is specified in seconds, not milliseconds.
		/// </summary>
		/// <param name="timeSeconds">Time to wait, seconds.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeSeconds"/> is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		/// <remarks><inheritdoc cref="Sleep"/></remarks>
		/// <example>
		/// <code><![CDATA[
		/// Time.Sleep(5000);
		/// 5000.ms(); //the same
		/// 5.s(); //the same
		/// ]]></code>
		/// </example>
		public static void s(this int timeSeconds)
		{
			if((uint)timeSeconds > int.MaxValue / 1000) throw new ArgumentOutOfRangeException();
			Sleep(timeSeconds * 1000);
		}

		/// <summary>
		/// Waits <paramref name="timeSeconds"/> seconds.
		/// The same as <see cref="Sleep"/> and <see cref="ms"/>, but the time is specified in seconds, not milliseconds.
		/// </summary>
		/// <param name="timeSeconds">Time to wait, seconds. The smallest value is 0.001 (1 ms).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeSeconds"/> is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		/// <remarks><inheritdoc cref="Sleep"/></remarks>
		/// <example>
		/// <code><![CDATA[
		/// Time.Sleep(2500);
		/// 2500.ms(); //the same
		/// 2.5.s(); //the same
		/// ]]></code>
		/// </example>
		public static void s(this double timeSeconds)
		{
			double t = timeSeconds * 1000d;
			if(t > int.MaxValue || t < 0) throw new ArgumentOutOfRangeException();
			Sleep((int)t);
		}

		/// <summary>
		/// Waits <paramref name="timeMS"/> milliseconds. While waiting, retrieves and dispatches Windows messages and other events.
		/// </summary>
		/// <param name="timeMS">Time to wait, milliseconds. Or <see cref="Timeout.Infinite"/>.</param>
		/// <remarks>
		/// Unlike <see cref="Sleep"/>, this function retrieves and dispatches Windows messages, calls .NET event handlers, hook procedures, timer functions, COM/RPC, etc. Supports APC.
		/// This function can be used in threads with windows. However usually there are better ways, for example timer, other thread, async/await/Task. In some places this function does not work as expected, for example in Form/Control mouse event handlers .NET blocks other mouse events.
		/// Be careful, this function is as dangerous as <see cref="System.Windows.Forms.Application.DoEvents"/>.
		/// Calls API <msdn>MsgWaitForMultipleObjectsEx</msdn> and <see cref="DoEvents"/>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeMS"/> is negative and not Timeout.Infinite.</exception>
		/// <seealso cref="WaitFor.MessagesAndCondition"/>
		/// <seealso cref="WaitFor.PostedMessage"/>
		/// <seealso cref="Util.MessageLoop"/>
		public static void SleepDoEvents(int timeMS)
		{
			LibSleepDoEvents(timeMS);
		}

		/// <summary>SleepDoEvents + noSetPrecision.</summary>
		internal static void LibSleepDoEvents(int timeMS, bool noSetPrecision = false)
		{
			if(timeMS < 0 && timeMS != -1) throw new ArgumentOutOfRangeException();

			if(timeMS == 0) {
				DoEvents();
				return;
			}

			if(!noSetPrecision) LibSleepPrecision.LibTempSet1(timeMS);

			WaitFor.LibWait(timeMS, WHFlags.DoEvents, null, null);
		}

		/// <summary>
		/// Retrieves and dispatches events and Windows messages from the message queue of this thread.
		/// </summary>
		/// <remarks>
		/// Similar to <see cref="System.Windows.Forms.Application.DoEvents"/>, but more lightweight. Uses API functions <msdn>PeekMessage</msdn>, <msdn>TranslateMessage</msdn> and <msdn>DispatchMessage</msdn>.
		/// Be careful, this function is as dangerous as <b>Application.DoEvents</b>.
		/// </remarks>
		public static void DoEvents()
		{
			while(Api.PeekMessage(out var m, default, 0, 0, Api.PM_REMOVE)) {
				//Wnd.Misc.PrintMsg(m);
				//if(m.message == Api.WM_QUIT) { Api.PostQuitMessage((int)m.wParam); return; }
				if(m.message == Api.WM_QUIT) Thread.CurrentThread.Abort();
				Api.TranslateMessage(m);
				Api.DispatchMessage(m);
			}
		}

		/// <summary>
		/// Temporarily changes the time resolution/precision of Thread.Sleep and some other functions.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>timeBeginPeriod</msdn>, which requests a time resolution for various system timers and wait functions. Actually it is the system thread scheduling timer period.
		/// Normal resolution on Windows 7-10 is 15.625 ms. It means that, for example, <c>Thread.Sleep(1);</c> sleeps not 1 but 1-15 ms. If you set resolution 1, it sleeps 1-2 ms.
		/// The new resolution is revoked (<msdn>timeEndPeriod</msdn>) when disposing the LibSleepPrecision variable or when this process ends. See example. See also <see cref="TempSet1"/>.
		/// The resolution is applied to all threads and processes. Other applications can change it too. For example, often web browsers temporarily set resolution 1 ms when opening a web page.
		/// The system uses the smallest period (best resolution) that currently is set by any application. You cannot make it bigger than current value.
		/// <note>It is not recommended to keep small period (high resolution) for a long time. It can be bad for power saving.</note>
		/// Don't need this for Time.SleepX and functions that use them (Mouse.Click etc). They call <see cref="TempSet1"/> when the sleep time is 1-99 ms.
		/// This does not change the minimal period of <see cref="Timer_"/> and System.Windows.Forms.Timer.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// _Test("before");
		/// using(new Time.LibSleepPrecision(2)) {
		/// 	_Test("in");
		/// }
		/// _Test("after");
		/// 
		/// void _Test(string name)
		/// {
		/// 	Print(name);
		/// 	Perf.First();
		/// 	for(int i = 0; i < 8; i++) { Thread.Sleep(1); Perf.Next(); }
		/// 	Perf.Write();
		/// }
		/// ]]></code>
		/// </example>
		internal sealed class LibSleepPrecision :IDisposable
		{
			//info: this class could be public, but probably not useful. Time.Sleep automatically sets 1 ms period if need.

			int _period;

			/// <summary>
			/// Calls API <msdn>timeBeginPeriod</msdn>.
			/// </summary>
			/// <param name="periodMS">
			/// New system timer period, milliseconds.
			/// Should be 1. Other values may stuck and later cannot be made smaller due to bugs in OS or some applications; this bug would impact many functions of this library.
			/// </param>
			/// <exception cref="ArgumentOutOfRangeException">periodMS &lt;= 0.</exception>
			public LibSleepPrecision(int periodMS)
			{
				if(periodMS <= 0) throw new ArgumentOutOfRangeException();
				if(Api.timeBeginPeriod((uint)periodMS) != 0) return;
				//Print("set");
				_period = periodMS;

				//Bug in OS or drivers or some apps:
				//	On my main PC often something briefly sets 0.5 ms resolution.
				//	If at that time this process already has set a resolution of more than 1 ms, then after that time this process cannot change resolution.
				//	It means that if this app eg has set 10 ms resolution, then Time.Sleep(1) will sleep 10 ms and not the normal 1-2 ms.
				//	Known workaround (but don't use, sometimes does not work, eg cannot end period that was set by another process):
				//		timeBeginPeriod(periodMS);
				//		var r=(int)Current; if(r>periodMS) { timeEndPeriod(periodMS); timeEndPeriod(r); timeBeginPeriod(r); timeBeginPeriod(periodMS); }
			}

			/// <summary>
			/// Calls API <msdn>timeEndPeriod</msdn>.
			/// </summary>
			public void Dispose()
			{
				_Dispose();
				GC.SuppressFinalize(this);
			}

			void _Dispose()
			{
				if(_period == 0) return;
				//Print("revoke");
				Api.timeEndPeriod((uint)_period); _period = 0;
			}

			///
			~LibSleepPrecision() { _Dispose(); }

			/// <summary>
			/// Gets current actual system time resolution (period).
			/// The return value usually is between 0.5 and 15.625 milliseconds. Returns 0 if fails.
			/// </summary>
			public static float Current
			{
				get
				{
					if(0 != Api.NtQueryTimerResolution(out _, out _, out var t)) return 0f;
					return (float)t / 10000;
				}
			}

			/// <summary>
			/// Temporarily sets the system wait precision to 1 ms. It will be revoked after the specified time or when this appdomain ends.
			/// If already set, just updates the revoking time.
			/// </summary>
			/// <param name="endAfterMS">Revoke after this time, milliseconds.</param>
			/// <example>
			/// <code><![CDATA[
			/// Print(Time.LibSleepPrecision.Current); //probably 15.625
			/// Time.LibSleepPrecision.TempSet1(500);
			/// Print(Time.LibSleepPrecision.Current); //1
			/// Thread.Sleep(600);
			/// Print(Time.LibSleepPrecision.Current); //probably 15.625 again
			/// ]]></code>
			/// </example>
			public static void TempSet1(int endAfterMS = 1111)
			{
				lock("2KgpjPxRck+ouUuRC4uBYg") {
					s_TS1_EndTime = MillisecondsWithoutComputerSleepTime + endAfterMS;
					if(s_TS1_Obj == null) {
						s_TS1_Obj = new LibSleepPrecision(1); //info: instead could call the API directly, but may need to auto-revoke using the finalizer
						ThreadPool.QueueUserWorkItem(endAfterMS2 =>
						{
							Thread.Sleep((int)endAfterMS2); //note: don't use captured variables. It somehow creates new garbage all the time.
							for(; ; ) {
								int t;
								lock("2KgpjPxRck+ouUuRC4uBYg") {
									t = (int)(s_TS1_EndTime - MillisecondsWithoutComputerSleepTime);
									if(t <= 0) {
										s_TS1_Obj.Dispose();
										s_TS1_Obj = null;
										break;
									}
								}
								Thread.Sleep(t);
							}
						}, endAfterMS);
						//perf: single QueueUserWorkItem adds 3 threads, >=2 adds 5. But Thread.Start is too slow etc.
						//QueueUserWorkItem speed first time is similar to Thread.Start, then ~8.
						//Task.Run and Task.Delay are much much slower first time. Single Delay adds 5 threads.
					}
				}
				//tested: Task Manager shows 0% CPU. If we set/revoke period for each Sleep(1) in loop, shows ~0.5% CPU.
			}
			static LibSleepPrecision s_TS1_Obj;
			static long s_TS1_EndTime;

			/// <summary>
			/// Calls TempSetMax if sleepTimeMS is 1-99.
			/// </summary>
			/// <param name="sleepTimeMS">milliseconds of the caller 'sleep' function.</param>
			internal static void LibTempSet1(int sleepTimeMS)
			{
				if(sleepTimeMS < 100 && sleepTimeMS > 0) TempSet1(1111);
			}
		}
	}

	/// <summary>
	/// Timer that uses API <msdn>SetTimer</msdn> and API <msdn>KillTimer</msdn>.
	/// </summary>
	/// <remarks>
	/// Similar to System.Windows.Forms.Timer, but more lightweight, for example does not create a hidden window.
	/// Use in UI threads. Does not work if this thread does not retrieve/dispatch posted messages (<msdn>WM_TIMER</msdn>).
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// //this example sets 3 timers
	/// Timer_.After(500, () => Print("after 500 ms"));
	/// Timer_.Every(1000, () => Print("every 1000 ms"));
	/// var t3 = new Timer_(() => Print("after 3000 ms")); t3.Start(3000, true); //the same as Timer_.After
	/// MessageBox.Show("");
	/// ]]></code>
	/// </example>
	public class Timer_
	{
		object _action; //Action<Timer_> or Action
		LPARAM _id;
		int _threadId;
		bool _singlePeriod;

		//To control object lifetime we use a thread-static Dictionary.
		//Tried GCHandle, but could not find a way to delete object when thread ends.
		//Calling KillTimer when thread ends is optional. Need just to re-enable garbage collection for this object.
		[ThreadStatic] static Dictionary<LPARAM, Timer_> t_timers;

		/// <summary>
		/// Some object or value attached to this Timer_ variable.
		/// </summary>
		public object Tag { get; set; }

		///
		public Timer_(Action<Timer_> timerAction, object tag = null) : this((object)timerAction, tag) { }

		///
		public Timer_(Action timerAction, object tag = null) : this((object)timerAction, tag) { }

		Timer_(object timerAction, object tag)
		{
			_action = timerAction;
			Tag = tag;
		}

		/// <summary>
		/// Starts timer. If already started, resets and changes its period.
		/// </summary>
		/// <param name="periodMilliseconds">Time interval (period) of calling the callback function (constructor's parameter <i>timerAction</i>), milliseconds. The minimal time is 10-20, even if this parameter is less than that.</param>
		/// <param name="singlePeriod">Call the callback function once (stop the timer before calling the callback function).</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative periodMilliseconds.</exception>
		/// <exception cref="InvalidOperationException">Called not in the same thread as previous <b>Start</b>.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// If already started, this function must be called in the same thread as when started.
		/// </remarks>
		public void Start(int periodMilliseconds, bool singlePeriod)
		{
			if(periodMilliseconds < 0) throw new ArgumentOutOfRangeException();
			bool isNew = _id == 0;
			if(!isNew) {
				_CheckThread();
			}
			LPARAM r = Api.SetTimer(default, _id, periodMilliseconds, _timerProc);
			if(r == 0) throw new Win32Exception();
			Debug.Assert(isNew || r == _id);
			_id = r;
			_singlePeriod = singlePeriod;
			if(isNew) {
				_threadId = Thread.CurrentThread.ManagedThreadId;
				if(t_timers == null) t_timers = new Dictionary<LPARAM, Timer_>();
				t_timers.Add(_id, this);
			}
			//Print($"Start: {_id}  isNew={isNew}  singlePeriod={singlePeriod}  _threadId={_threadId}");
		}

		static Api.TIMERPROC _timerProc = _TimerProc;
		static void _TimerProc(Wnd w, int msg, LPARAM idEvent, uint time)
		{
			//Print(t_timers.Count, idEvent);
			if(!t_timers.TryGetValue(idEvent, out var t)) {
				//Debug_.Print($"timer id {idEvent} not in t_timers");
				return;
				//It is possible after killing timer.
				//	Normally API KillTimer removes WM_TIMER message from queue (tested), but in some conditions our callback can still be called several times.
				//	For example if multiple messages are retrieved from the OS queue without dispatching each, and then all are dispatched.
				//	Usually we can safely ignore it. But not good if the same timer id is reused for another timer. Tested on Win10: OS does not reuse ids soon.
			}
			if(t._singlePeriod) t.Stop();
			
			switch(t._action) {
			case Action<Timer_> f: f(t); break;
			case Action f: f(); break;
			}
		}

		/// <summary>
		/// Stops the timer.
		/// </summary>
		/// <exception cref="InvalidOperationException">Called not in the same thread as <b>Start</b>.</exception>
		/// <remarks>
		/// The callback function will not be called after this.
		/// Later you can start the timer again (call <see cref="Start"/>).
		/// Don't need to call this function for single-period timers. For periodic timers it is optional; the timer stops when the thread ends.
		/// This function must be called in the same thread as <b>Start</b>.
		/// </remarks>
		public void Stop()
		{
			if(_id != 0) {
				//Print($"Stop: {_id}          _threadId={_threadId}");
				_CheckThread();
				Api.KillTimer(default, _id);
				//tested: KillTimer removes pending WM_TIMER messages from queue. MSDN lies. Tested on Win 10 and 7.
				t_timers.Remove(_id);
				_id = 0;
			}
		}

		void _CheckThread()
		{
			bool isSameThread = _threadId == Thread.CurrentThread.ManagedThreadId;
			Debug.Assert(isSameThread);
			if(!isSameThread) throw new InvalidOperationException(nameof(Timer_) + " used by multiple threads.");
			//FUTURE: somehow allow other thread. It is often useful.
		}

		//~Timer_() { Print("dtor"); } //don't call Stop() here, we are in other thread

		static Timer_ _Set(int time, bool singlePeriod, object timerAction, object tag = null)
		{
			var t = new Timer_(timerAction, tag);
			t.Start(time, singlePeriod);
			return t;
		}

		/// <summary>
		/// Sets new one-time timer.
		/// Returns new <see cref="Timer_"/> object. Usually you don't need it.
		/// </summary>
		/// <param name="timeMilliseconds">Time after which will be called the callback function, milliseconds. The minimal time is 10-20, even if this parameter is less than that.</param>
		/// <param name="timerAction">Callback function.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative periodMilliseconds.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or AuDialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static Timer_ After(int timeMilliseconds, Action timerAction, object tag = null)
		{
			return _Set(timeMilliseconds, true, timerAction, tag);
		}

		/// <inheritdoc cref="After(int, Action, object)"/>
		public static Timer_ After(int timeMilliseconds, Action<Timer_> timerAction, object tag = null)
		{
			return _Set(timeMilliseconds, true, timerAction, tag);
		}

		/// <summary>
		/// Sets new periodic timer.
		/// Returns new <see cref="Timer_"/> object that can be used to modify timer properties if you want to do it not in the callback function; usually don't need it.
		/// </summary>
		/// <param name="periodMilliseconds">Time interval (period) of calling the callback function, milliseconds. The minimal period is 10-20, even if this parameter is less than that.</param>
		/// <param name="timerAction">Callback function.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative periodMilliseconds.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or AuDialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static Timer_ Every(int periodMilliseconds, Action timerAction, object tag = null)
		{
			return _Set(periodMilliseconds, false, timerAction, tag);
		}

		/// <inheritdoc cref="Every(int, Action, object)"/>
		public static Timer_ Every(int periodMilliseconds, Action<Timer_> timerAction, object tag = null)
		{
			return _Set(periodMilliseconds, false, timerAction, tag);
		}
	}
}
