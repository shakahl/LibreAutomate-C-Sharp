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
		/// Includes the computer sleep/hibernate time (see also <see cref="MillisecondsWithoutSleepTime"/>). Independent of computer clock time changes.
		/// MSDN article: <msdn>Acquiring high-resolution time stamps</msdn>.
		/// </remarks>
		public static long Microseconds { get => (long)(Stopwatch.GetTimestamp() * _freqMCS); }

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup.
		/// The precision is 1 millisecond.
		/// See also <see cref="Microseconds"/>.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryPerformanceCounter</msdn>.
		/// Includes the computer sleep/hibernate time (see also <see cref="MillisecondsWithoutSleepTime"/>). Independent of computer clock time changes.
		/// Unlike Environment.TickCount, this function is more precise and returns a 64-bit value that will not roll over in 100 years from the most recent system boot.
		/// </remarks>
		public static long Milliseconds { get => (long)(Stopwatch.GetTimestamp() * _freqMS); }
		//public static long Milliseconds { get => Api.GetTickCount64(); } //15 ms precision. On current OS and hardware, QueryPerformanceCounter is reliable and almost as fast.

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup, not including time the system spends in sleep or hibernation.
		/// The precision is 1-16 milliseconds, depending on the system wait precision (see <see cref="SystemWaitPrecision"/>).
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryUnbiasedInterruptTime</msdn>.
		/// Independent of computer clock time changes.
		/// This function can be used to implement a timeout in 'wait for something' functions, when repeatedly checking a condition.
		/// </remarks>
		public static long MillisecondsWithoutSleepTime
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
		/// The same as <see cref="WaitMS"/>, but uses seconds, not milliseconds.
		/// </summary>
		/// <param name="timeS">
		/// The number of seconds to wait.
		/// The smallest value is 0.001 (1 ms).
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">timeS is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process events and messages, therefore should not be used in threads with windows, timers or COM events. Supports asynchronous procedure calls.
		/// Usually waits longer by 1-15 milliseconds, but not shorter. It depends on the system wait precision (see <see cref="SystemWaitPrecision"/>).
		/// If the computer goes to sleep or hibernate during that time, the real time is timeS + the sleep/hibernate time.
		/// </remarks>
		public static void Wait(double timeS)
		{
			timeS *= 1000.0;
			if(timeS > int.MaxValue || timeS < 0) throw new ArgumentOutOfRangeException();
			WaitMS((int)timeS);
		}

		/// <summary>
		/// Suspends this thread for the specified amount of time.
		/// The same as <see cref="Wait"/>, but uses milliseconds, not seconds.
		/// </summary>
		/// <param name="timeMS">
		/// The number of milliseconds to wait.
		/// The smallest value is 1.
		/// Also can be <see cref="Timeout.Infinite"/>.
		/// </param>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process events and messages, therefore should not be used with big timeMS in threads with windows, timers or COM events. Supports asynchronous procedure calls.
		/// Usually waits longer by 1-15 milliseconds, but not shorter. It depends on the system wait precision (see <see cref="SystemWaitPrecision"/>).
		/// If the computer goes to sleep or hibernate during that time, the real time is timeS + the sleep/hibernate time.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">timeMS is negative and not Timeout.Infinite.</exception>
		public static void WaitMS(int timeMS)
		{
			if(timeMS < 2000) {
				Thread.Sleep(timeMS);
			} else { //fix Thread.Sleep bug: if there are APC, returns too soon after sleep/hibernate.
				g1:
				long t = MillisecondsWithoutSleepTime;
				Thread.Sleep(timeMS);
				t = timeMS - (MillisecondsWithoutSleepTime - t);
				if(t >= 500) { timeMS = (int)t; goto g1; }
			}
		}

		/// <summary>
		/// Partially suspends this thread for the specified amount of time, during which processes events and messages.
		/// </summary>
		/// <param name="timeMS">
		/// The number of milliseconds to wait.
		/// The smallest value is 1.
		/// Also can be <see cref="Timeout.Infinite"/>.
		/// </param>
		/// <remarks>
		/// Unlike <see cref="WaitMS"/>, this function retrieves and dispatches all Windows messages, including posted (key/mouse input, window paint, timer and other). Also calls event handlers, hook procedures, etc. Supports asynchronous procedure calls.
		/// This function can be used in threads with windows. However usually there are better ways, for example timer, other thread, async/await/Task. Be careful, this function is as dangerous as <see cref="Application.DoEvents"/>. In some places does not work as expected, for example in Form/Control mouse event handlers .NET blocks other mouse events.
		/// In threads without windows and timers usually don't need to process posted messages, but in some cases need to process sent messages, some events, hooks etc. Then you can instead use <see cref="Thread.Join(int)"/>, like <c>Thread.CurrentThread.Join(1000);</c>.
		/// Calls API <msdn>MsgWaitForMultipleObjectsEx</msdn> and <see cref="DoEvents"/>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">timeMS is negative and not Timeout.Infinite.</exception>
		/// <exception cref="Exception">Any exceptions thrown by functions that are executed while waiting (event handlers etc).</exception>
		/// <seealso cref="Util.MessageLoop"/>
		public static void WaitDoEventsMS(int timeMS)
		{
			if(timeMS == 0) { DoEvents(); return; }
			if(timeMS < 0 && timeMS != Timeout.Infinite) throw new ArgumentOutOfRangeException();
			for(;;) {
				long t = 0;
				int timeSlice = 100; //we call API in loop with small timeout to make it respond to Thread.Abort
				if(timeMS > 0) {
					if(timeMS < timeSlice) timeSlice = timeMS;
					t = MillisecondsWithoutSleepTime;
				}

				uint k = Api.MsgWaitForMultipleObjectsEx(0, null, (uint)timeSlice, Api.QS_ALLINPUT, Api.MWMO_ALERTABLE);
				//info: k can be 0 (message etc), WAIT_TIMEOUT, WAIT_IO_COMPLETION, WAIT_FAILED.
				if(k == Api.WAIT_FAILED) throw new InvalidOperationException(); //unlikely, because not using handles
				if(k == 0) DoEvents();

				if(timeMS > 0) {
					timeMS -= (int)(MillisecondsWithoutSleepTime - t);
					if(timeMS <= 0) break;
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
			if(Application.MessageLoop) {
				Application.DoEvents();
			} else {
				while(Api.PeekMessage(out var m, Wnd0, 0, 0, Api.PM_REMOVE)) {
					//Wnd.Misc.PrintMsg(ref m);
					if(m.message == Api.WM_QUIT) { Api.PostQuitMessage(m.wParam); return; }
					Api.TranslateMessage(ref m);
					Api.DispatchMessage(ref m);
				}
			}

			//info: Could be bool, return false on WM_QUIT. But probably not useful. It seems that WM_QUIT is not used when closing Form.
		}

#if true
		/// <summary>
		/// Sets new timer for this thread.
		/// Returns new Timer_ object that can be used to change or stop the timer if you want to do it not in the callback function; usually don't need it.
		/// </summary>
		/// <param name="intervalMS">Time interval (period) of calling the callback function, milliseconds. Min interval is 10; if this parameter is 0-9, uses 10. Max interval is int.MaxValue.</param>
		/// <param name="singlePeriod">Call the callback function once, not repeatedly. The timer will be stopped before calling the callback function.</param>
		/// <param name="callback">A callback function (delegate), for example lambda.</param>
		/// <param name="tag">Something to pass to the callback function as Timer_.Tag.</param>
		/// <remarks>
		/// Uses class Time.Timer_, which calls API <msdn>SetTimer</msdn>.
		/// Similar to System.Windows.Forms.Timer, but more lightweight, for example does not create a hidden window.
		/// The callback function will be called in this thread.
		/// This thread must have a message loop, eg call Application.Run() or Form.ShowModal() or TaskDialog.Show(). The callback function is not called while this thread is not retrieving Windows messages from the thread's message queue.
		/// The timer interval precision is 10-16 ms.
		/// </remarks>
		public static Timer_ SetTimer(int intervalMS, bool singlePeriod, Action<Timer_> callback, object tag = null)
		{
			var t = new Timer_(callback, tag);
			t.Start(intervalMS, singlePeriod);
			return t;
		}

		/// <summary>
		/// Manages a timer for this thread.
		/// Calls API <msdn>SetTimer</msdn> and API <msdn>KillTimer</msdn>.
		/// Similar to System.Windows.Forms.Timer, but more lightweight, for example does not create a hidden window.
		/// Used by <see cref="Time.SetTimer">Time.SetTimer</see>, more info there.
		/// </summary>
		public class Timer_
		{
			Action<Timer_> _callback;
			int _id;
			int _threadId;
			bool _singlePeriod;

			//Api.TIMERPROC _timerProc;
			//GCHandle _gch;

			//To control object lifetime we use a thread-static array (actually Hashtable).
			//Tried GCHandle, but could not find a way to delete object when thread ends.
			//Calling KillTimer when thread ends is optional. Need just to re-enable garbage collection for this object.
			[ThreadStatic]
			static System.Collections.Hashtable _timers;

			/// <summary>
			/// Some object or value attached to this Timer_ variable.
			/// Can be set with this property or with <see cref="Time.SetTimer">Time.SetTimer</see>.
			/// </summary>
			public object Tag { get; set; }

			/// <summary>
			/// Initializes a new <see cref="Timer_"/> instance.
			/// Used by <see cref="Time.SetTimer"/>.
			/// </summary>
			public Timer_(Action<Timer_> callback, object tag = null) : base()
			{
				_callback = callback;
				Tag = tag;

				//_timerProc = _TimerProc; //creates new delegate and keeps from GC until GC deletes this object. The delegate is non-static, ie has a reference to this object.
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
				int r = Api.SetTimer(Wnd0, _id, (uint)intervalMS, _timerProc);
				if(r == 0) throw new Win32Exception();
				//_gch = GCHandle.Alloc(this);
				_id = r;
				_singlePeriod = singlePeriod;
				if(isNew) {
					_threadId = Thread.CurrentThread.ManagedThreadId;
					if(_timers == null) _timers = new System.Collections.Hashtable();
					_timers.Add(_id, this);
				}
			}

			static Api.TIMERPROC _timerProc = _TimerProc;
			static void _TimerProc(Wnd w, uint msg, LPARAM idEvent, uint time)
			{
				var t = _timers[(int)idEvent] as Timer_;
				if(t == null) {
					Debug.Assert(false);
					return;
				}
				if(t._singlePeriod) t.Stop();
				t._callback(t);
			}
			//void _TimerProc(Wnd w, uint msg, LPARAM idEvent, uint time)
			//{
			//	if(_id == 0) {
			//		Debug.Assert(false);
			//		return;
			//	}
			//	if(_singlePeriod) Stop();
			//	_callback(this);
			//}

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
					Api.KillTimer(Wnd0, _id);
					//tested: despite what MSDN says, KillTimer removes pending WM_TIMER messages from queue. Tested on Win 10 and 7.
					_timers.Remove(_id);
					_id = 0;
					//_gch.Free();
				}
			}

			bool _IsSameThread()
			{
				bool isSameThread = _threadId == Thread.CurrentThread.ManagedThreadId;
				Debug.Assert(isSameThread);
				return isSameThread;
			}

			//~Timer_() { Print("dtor"); } //don't call Stop() here, we are in other thread
		}
#else //tried with System.Windows.Forms.Timer. Works, but creates hidden windows, I don't like it.
		public static void SetTimer(int intervalMS, TimerHandler onTick, bool singleTick = false, object tag = null)
		{
			var t = new _Timer(intervalMS, onTick, singleTick, tag);
			t.Start();
		}

		public delegate void TimerHandler(System.Windows.Forms.Timer t);

		class _Timer :System.Windows.Forms.Timer
		{
			TimerHandler _onTick;
			bool _singleTick;

			internal _Timer(int intervalMS, TimerHandler onTick, bool singleTick = false, object tag = null) : base()
			{
				_onTick = onTick;
				_singleTick = singleTick;
				Interval = intervalMS;
				Tag = tag;
				Tick += _Tick;
			}

			void _Tick(object sender, EventArgs e)
			{
				if(_singleTick) Stop();
				_onTick(this);
			}
		}
#endif

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
				WaitMS(i);
				if(!w.Is0 && !w.SendTimeout(1000, 0)) w = Wnd0;
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
		public static void AutoDelay(int minMS, int maxMS = 0) { AutoDelay(Wnd0, minMS, maxMS); }

		/// <summary>
		/// Waits Script.Speed milliseconds.
		/// </summary>
		public static void AutoDelay() { AutoDelay(Wnd0, 0, 0); }
#endif

		/// <summary>
		/// Changes the precision of wait functions.
		/// Calls API <msdn>timeBeginPeriod</msdn>, which requests a minimum resolution (period) for various system timers and wait functions.
		/// Normal period on Windows 7-10 is 15.625 ms, minimal 1 ms. This class can temporarily make it smaller. It is applied to all threads and processes.
		/// The new period is revoked when disposing the variable or when this process ends. See example.
		/// Other applications can change it too. For example, Firefox and other web browsers temporarily set period 1 ms when opening a web page.
		/// The system uses the smallest period (highest precision) that currently is set by any application. You cannot make it bigger than current value.
		/// <note>It is not recommended to keep small period for a long time. It can be bad for power saving.</note>
		/// <note>This does not change the minimal period of <see cref="SetTimer">SetTimer</see>.</note>
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// _Test("before");
		/// using(new Time.SystemWaitPrecision(2)) {
		/// 	_Test("in");
		/// }
		/// _Test("after");
		/// 
		/// void _Test(string name)
		/// {
		/// 	Print(name);
		/// 	Perf.First();
		/// 	for(int i = 0; i < 10; i++) { Thread.Sleep(1); Perf.Next(); }
		/// 	Perf.Write();
		/// }
		/// ]]></code>
		/// </example>
		public sealed class SystemWaitPrecision :IDisposable
		{
			int _period;

			//Also tested NtSetTimerResolution, it is undocumented. It can set min 0.5 ms resolution. Tested on Win10.
			//Also tested NtQueryTimerResolution. NtSetTimerResolution also can be used for this, partially.

			/// <summary>
			/// Calls API <msdn>timeBeginPeriod</msdn>.
			/// </summary>
			/// <param name="periodMS">Requested period. Should be 2 - 16.</param>
			/// <exception cref="InvalidOperationException">timeBeginPeriod failed.</exception>
			public SystemWaitPrecision(int periodMS)
			{
				if(timeBeginPeriod((uint)periodMS) != 0) throw new ArgumentException(); //eg when periodMS is 0
				_period = periodMS;
			}

			/// <summary>
			/// Calls API <msdn>timeEndPeriod</msdn>.
			/// </summary>
			public void Dispose()
			{
				if(_period != 0) { timeEndPeriod((uint)_period); _period = 0; }
			}

			///
			~SystemWaitPrecision()
			{
				Dispose();
			}

			[DllImport("winmm.dll")]
			static extern uint timeBeginPeriod(uint uPeriod);
			[DllImport("winmm.dll")]
			static extern uint timeEndPeriod(uint uPeriod);
		}
	}
}
