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

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Time functions. Get, wait, doevents, timer, precision.
	/// </summary>
	//[DebuggerStepThrough]
	public static class Time
	{
		static double _freqMCS = 1000000.0 / Stopwatch.Frequency;
		static double _freqMS = 1000.0 / Stopwatch.Frequency;

		/// <summary>
		/// Gets the number of microseconds elapsed since Windows startup.
		/// The precision is 1 microsecond.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryPerformanceCounter</msdn>.
		/// Includes the computer sleep/hibernate time (see also <see cref="MillisecondsWithoutComputerSleepTime"/>). Independent of computer clock time changes.
		/// MSDN article: <msdn>Acquiring high-resolution time stamps</msdn>.
		/// </remarks>
		public static long Microseconds => (long)(Stopwatch.GetTimestamp() * _freqMCS);

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup.
		/// The precision is 1 millisecond.
		/// See also <see cref="Microseconds"/>.
		/// </summary>
		/// <remarks>
		/// Includes the computer sleep/hibernate time (see also <see cref="MillisecondsWithoutComputerSleepTime"/>). Independent of computer clock time changes.
		/// Unlike Environment.TickCount, this function is more precise and returns a 64-bit value that will not roll over in 100 years.
		/// </remarks>
		public static long Milliseconds => (long)(Stopwatch.GetTimestamp() * _freqMS);
		//public static long Milliseconds => Api.GetTickCount64(); //15 ms precision. On current OS and hardware, QueryPerformanceCounter is reliable and almost as fast.

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup, not including time the system spends in sleep or hibernation.
		/// The precision is 1-16 milliseconds.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryUnbiasedInterruptTime</msdn>.
		/// Independent of computer clock time changes.
		/// This function can be used to implement a timeout in 'wait for' functions, when repeatedly checking a condition.
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
		/// Suspends this thread for the specified amount of time.
		/// The same as <see cref="Sleep"/>, but uses seconds, not milliseconds.
		/// </summary>
		/// <param name="seconds">
		/// Time to wait, seconds.
		/// The smallest value is 0.001 (1 ms).
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">seconds is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process events and messages, therefore should not be used in threads with windows, timers or COM events. Supports asynchronous procedure calls.
		/// If the computer goes to sleep or hibernate during that time, the real time is seconds + the sleep/hibernate time.
		/// </remarks>
		public static void Wait(double seconds)
		{
			seconds *= 1000.0;
			if(seconds > int.MaxValue || seconds < 0) throw new ArgumentOutOfRangeException();
			Sleep((int)seconds);
		}

		/// <summary>
		/// Suspends this thread for the specified amount of time.
		/// The same as <see cref="Wait"/>, but uses milliseconds, not seconds; and supports Timeout.Infinite.
		/// </summary>
		/// <param name="milliseconds">
		/// Time to wait, milliseconds.
		/// If 0, can wait briefly if another busy thread runs on the same logical CPU, which happens not often on modern multi-core CPU.
		/// Also can be <see cref="Timeout.Infinite"/>.
		/// </param>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process events and messages, therefore should not be used with big milliseconds in threads with windows, timers or COM events. Supports asynchronous procedure calls.
		/// If the computer goes to sleep or hibernate during that time, the real time is milliseconds + the sleep/hibernate time.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">milliseconds is negative and not Timeout.Infinite.</exception>
		public static void Sleep(int milliseconds)
		{
			LibSleepPrecision.LibTempSet1(milliseconds);
			if(milliseconds < 2000) {
				Thread.Sleep(milliseconds);
			} else { //fix Thread.Sleep bug: if there are APC, returns too soon after sleep/hibernate.
				g1:
				long t = MillisecondsWithoutComputerSleepTime;
				Thread.Sleep(milliseconds);
				t = milliseconds - (MillisecondsWithoutComputerSleepTime - t);
				if(t >= 500) { milliseconds = (int)t; goto g1; }
			}
		}

		/// <summary>
		/// Partially suspends this thread for the specified amount of time, during which processes events and messages.
		/// </summary>
		/// <param name="milliseconds">
		/// The number of milliseconds to wait.
		/// The smallest value is 1.
		/// Also can be <see cref="Timeout.Infinite"/>.
		/// </param>
		/// <remarks>
		/// Unlike <see cref="Sleep"/>, this function retrieves and dispatches all Windows messages, including posted (key/mouse input, window paint, timer and other). Also calls event handlers, hook procedures, etc. Supports asynchronous procedure calls.
		/// This function can be used in threads with windows. However usually there are better ways, for example timer, other thread, async/await/Task. Be careful, this function is as dangerous as <see cref="Application.DoEvents"/>. In some places does not work as expected, for example in Form/Control mouse event handlers .NET blocks other mouse events.
		/// In threads without windows and timers usually don't need to process posted messages, but in some cases need to process sent messages, some events, hooks etc. Then you can instead use <see cref="Thread.Join(int)"/>, like <c>Thread.CurrentThread.Join(1000);</c>.
		/// Calls API <msdn>MsgWaitForMultipleObjectsEx</msdn> and <see cref="DoEvents"/>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">milliseconds is negative and not Timeout.Infinite.</exception>
		/// <exception cref="Exception">Any exceptions thrown by functions that are executed while waiting (event handlers etc).</exception>
		/// <seealso cref="Util.MessageLoop"/>
		public static unsafe void SleepDoEvents(int milliseconds)
		{
			if(milliseconds == 0) { DoEvents(); return; }
			if(milliseconds < 0 && milliseconds != Timeout.Infinite) throw new ArgumentOutOfRangeException();
			LibSleepPrecision.LibTempSet1(milliseconds);
			for(;;) {
				long t = 0;
				int timeSlice = 100; //we call API in loop with small timeout to make it respond to Thread.Abort
				if(milliseconds > 0) {
					if(milliseconds < timeSlice) timeSlice = milliseconds;
					t = MillisecondsWithoutComputerSleepTime;
				}

				uint k = Api.MsgWaitForMultipleObjectsEx(0, null, (uint)timeSlice, Api.QS_ALLINPUT, Api.MWMO_ALERTABLE);
				//info: k can be 0 (message etc), WAIT_TIMEOUT, WAIT_IO_COMPLETION, WAIT_FAILED.
				if(k == Api.WAIT_FAILED) throw new Win32Exception(); //unlikely, because not using handles
				if(k == 0) DoEvents();

				if(milliseconds > 0) {
					milliseconds -= (int)(MillisecondsWithoutComputerSleepTime - t);
					if(milliseconds <= 0) break;
				}
			}
		}

		/// <summary>
		/// Retrieves and dispatches messages from the message queue of current thread.
		/// </summary>
		/// <remarks>
		/// If this thread has a .NET message loop (<see cref="Application.MessageLoop"/>), calls <see cref="Application.DoEvents"/>.
		/// Else uses more lightweight API functions <msdn>PeekMessage</msdn>, <msdn>TranslateMessage</msdn> and <msdn>DispatchMessage</msdn>.
		/// Be careful, this function is as dangerous as <see cref="Application.DoEvents"/>.
		/// </remarks>
		/// <exception cref="Exception">Any exceptions thrown by functions that are executed while dispatching messages (event handlers etc).</exception>
		public static void DoEvents()
		{
			if(Application.MessageLoop) { //never mind: loads Forms.dll. It's fast.
				Application.DoEvents();
			} else {
				while(Api.PeekMessage(out var m, default, 0, 0, Api.PM_REMOVE)) {
					//Wnd.Misc.PrintMsg(ref m);
					if(m.message == Api.WM_QUIT) { Api.PostQuitMessage(m.wParam); return; }
					Api.TranslateMessage(ref m);
					Api.DispatchMessage(ref m);
				}
			}

			//info: Could be bool, return false on WM_QUIT. But probably not useful. It seems that WM_QUIT is not used when closing Form.
		}

		/// <summary>
		/// Calls <see cref="Wait"/>.
		/// This extension method allows to replace code like <c>Wait(5);</c> with <c>5.s();</c>.
		/// </summary>
		public static void s(this int seconds)
		{
			Wait(seconds);
		}

		/// <summary>
		/// Calls <see cref="Wait"/>.
		/// This extension method allows to replace code like <c>Wait(2.5);</c> with <c>2.5.s();</c>.
		/// </summary>
		public static void s(this double seconds)
		{
			Wait(seconds);
		}

		/// <summary>
		/// Calls <see cref="Sleep"/>.
		/// This extension method allows to replace code like <c>Time.Sleep(50);</c> with <c>50.ms();</c>.
		/// </summary>
		public static void ms(this int milliseconds)
		{
			Sleep(milliseconds);
		}

#if false
		//Don't use. Unfinished.
		/// <summary>
		/// Waits Script.Speed milliseconds, or longer if window w is busy.
		/// Does not wait if window w is of this thread.
		/// </summary>
		/// <param name="w">A window.</param>
		/// <param name="minMS">Minimal time to wait, ms.</param>
		/// <param name="maxMS">Maximal time to wait, ms. Does not limit if 0.</param>
		public static void AutoDelay(Wnd w, int minMS, int maxMS = 0)
		{
			if(w.IsOfThisThread) return;

			int ms = Script.Speed;
			if(ms < minMS) ms = minMS;
			if(maxMS > 0 && ms > maxMS) ms = maxMS;

			for(int i = 0, t = 0; t < ms;) {
				i += 2; t += i; if(t > ms) i -= t - ms;
				//Print(i);
				Sleep(i);
				if(!w.Is0 && !w.SendTimeout(1000, 0)) w = default;
			}
		}

		/// <summary>
		/// Waits Script.Speed milliseconds, or longer if window w is busy.
		/// Does not wait if window w is of this thread.
		/// </summary>
		/// <param name="w">A window.</param>
		public static void AutoDelay(Wnd w) { AutoDelay(w, 0, 0); }

		/// <summary>
		/// Waits Script.Speed milliseconds.
		/// </summary>
		/// <param name="minMS">Minimal time to wait, ms.</param>
		/// <param name="maxMS">Maximal time to wait, ms. Does not limit if 0.</param>
		public static void AutoDelay(int minMS, int maxMS = 0) { AutoDelay(default, minMS, maxMS); }

		/// <summary>
		/// Waits Script.Speed milliseconds.
		/// </summary>
		public static void AutoDelay() { AutoDelay(default, 0, 0); }
#endif

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
		/// Don't need this for Time.Sleep, Time.Wait, Time.SleepDoEvents and functions that use them (Mouse.Click etc). They call <see cref="TempSet1"/> when the sleep time is 1-99 ms.
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
				if(_period == 0) return;
				//Print("revoke");
				Api.timeEndPeriod((uint)_period); _period = 0;
				GC.SuppressFinalize(this);
			}

			///
			~LibSleepPrecision() { Dispose(); }

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
							for(;;) {
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
	/// Similar to System.Windows.Forms.Timer, but more lightweight, for example does not create a hidden window.
	/// Use in UI threads (need a message loop).
	/// </summary>
	/// <example>
	/// <code><![CDATA[
	/// //this example sets 3 timers
	/// Timer_.After(500, t => Print("simple one-time timer"));
	/// Timer_.Every(1000, t => Print("simple periodic timer"));
	/// var t3 = new Timer_(t => Print("with Timer_ object")); t3.Start(3000, true); //the same as Timer_.After
	/// MessageBox.Show("");
	/// ]]></code>
	/// </example>
	public class Timer_
	{
		Action<Timer_> _callback;
		int _id;
		int _threadId;
		bool _singlePeriod;

		//To control object lifetime we use a thread-static array (actually Hashtable).
		//Tried GCHandle, but could not find a way to delete object when thread ends.
		//Calling KillTimer when thread ends is optional. Need just to re-enable garbage collection for this object.
		[ThreadStatic] static System.Collections.Hashtable t_timers;

		/// <summary>
		/// Some object or value attached to this Timer_ variable.
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// Initializes a new <see cref="Timer_"/> instance.
		/// </summary>
		public Timer_(Action<Timer_> callback, object tag = null) : base()
		{
			_callback = callback;
			Tag = tag;
		}

		/// <summary>
		/// Starts timer. If already started, resets and changes its period.
		/// </summary>
		/// <param name="intervalMS">Time interval (period) of calling the callback function, milliseconds.</param>
		/// <param name="singlePeriod">Call the callback function once, not repeatedly. The timer will be stopped before calling the callback function.</param>
		/// <remarks>
		/// If already started, this function must be called in same thread.
		/// </remarks>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		public void Start(int intervalMS, bool singlePeriod)
		{
			bool isNew = _id == 0;
			if(!isNew) {
				if(!_IsSameThread()) return;
			}
			int r = Api.SetTimer(default, _id, (uint)intervalMS, _timerProc);
			if(r == 0) throw new Win32Exception();
			_id = r;
			_singlePeriod = singlePeriod;
			if(isNew) {
				_threadId = Thread.CurrentThread.ManagedThreadId;
				if(t_timers == null) t_timers = new System.Collections.Hashtable();
				t_timers.Add(_id, this);
			}
		}

		static Api.TIMERPROC _timerProc = _TimerProc;
		static void _TimerProc(Wnd w, uint msg, LPARAM idEvent, uint time)
		{
			var t = t_timers[(int)idEvent] as Timer_;
			if(t == null) {
				Debug.Assert(false);
				return;
			}
			if(t._singlePeriod) t.Stop();
			t._callback(t);
		}

		/// <summary>
		/// Stops the timer.
		/// </summary>
		/// <remarks>
		/// Your callback function will never be called after this, even if a timer period is elapsed.
		/// Later you can call Start() again.
		/// Calling Stop() is optional.
		/// Must be called in same thread.
		/// </remarks>
		public void Stop()
		{
			//Print("Stop: " + _id);
			if(_id != 0) {
				if(!_IsSameThread()) return;
				Api.KillTimer(default, _id);
				//tested: despite what MSDN says, KillTimer removes pending WM_TIMER messages from queue. Tested on Win 10 and 7.
				t_timers.Remove(_id);
				_id = 0;
			}
		}

		bool _IsSameThread()
		{
			bool isSameThread = _threadId == Thread.CurrentThread.ManagedThreadId;
			Debug.Assert(isSameThread);
			return isSameThread;
		}

		//~Timer_() { Print("dtor"); } //don't call Stop() here, we are in other thread

		static Timer_ _Set(int intervalMS, bool singlePeriod, Action<Timer_> callback, object tag = null)
		{
			var t = new Timer_(callback, tag);
			t.Start(intervalMS, singlePeriod);
			return t;
		}

		/// <summary>
		/// Sets new one-time timer.
		/// Returns new <see cref="Timer_"/> object that can be used to modify timer properties if you want to do it not in the callback function; usually don't need it.
		/// </summary>
		/// <param name="milliseconds">Time after which will be called the callback function, milliseconds. Can be 1 to int.MaxValue. The actual minimal time usually is 10-20.</param>
		/// <param name="callback">A callback function (delegate), for example lambda.</param>
		/// <param name="tag">Something to pass to the callback function as Timer_.Tag.</param>
		/// <remarks>
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or TaskDialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static Timer_ After(int milliseconds, Action<Timer_> callback, object tag = null)
		{
			return _Set(milliseconds, true, callback, tag);
		}

		/// <summary>
		/// Sets new periodic timer.
		/// Returns new <see cref="Timer_"/> object that can be used to modify timer properties if you want to do it not in the callback function; usually don't need it.
		/// </summary>
		/// <param name="periodMS">Time interval (period) of calling the callback function, milliseconds. Can be 1 to int.MaxValue. The actual minimal period usually is 10-20.</param>
		/// <param name="callback">A callback function (delegate), for example lambda.</param>
		/// <param name="tag">Something to pass to the callback function as Timer_.Tag.</param>
		/// <remarks>
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or TaskDialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static Timer_ Every(int periodMS, Action<Timer_> callback, object tag = null)
		{
			return _Set(periodMS, false, callback, tag);
		}
	}
}
