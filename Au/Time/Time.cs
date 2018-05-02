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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
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
		/// Waits <paramref name="timeS"/> seconds.
		/// The same as <see cref="Sleep"/>, but the time is specified in seconds, not milliseconds.
		/// </summary>
		/// <param name="timeS">Time to wait, seconds. The smallest value is 0.001 (1 ms).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeS"/> is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process Windows messages and other events, therefore should not be used in threads with windows, timers, hooks, events or COM events. Supports APC.
		/// If the computer goes to sleep or hibernate during that time, the real time is <paramref name="timeS"/> + the sleep/hibernate time.
		/// Tip: code <c>5.s();</c> is the same as <c>Time.SleepS(5);</c>.
		/// </remarks>
		public static void SleepS(double timeS)
		{
			timeS *= 1000.0;
			if(timeS > int.MaxValue || timeS < 0) throw new ArgumentOutOfRangeException();
			Sleep((int)timeS);
		}

		/// <summary>
		/// Waits <paramref name="timeMS"/> milliseconds.
		/// The same as <see cref="SleepS"/>, but the time is specified in milliseconds, not seconds; and supports Timeout.Infinite.
		/// </summary>
		/// <param name="timeMS">Time to wait, milliseconds. Or <see cref="Timeout.Infinite"/>.</param>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process Windows messages and other events, therefore should not be used in threads with windows, timers, hooks, events or COM events, unless <paramref name="timeMS"/> is small. Supports APC.
		/// If the computer goes to sleep or hibernate during that time, the real time is <paramref name="timeMS"/> + the sleep/hibernate time.
		/// Tip: code <c>50.ms();</c> is the same as <c>Time.Sleep(50);</c>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeMS"/> is negative and not Timeout.Infinite.</exception>
		public static void Sleep(int timeMS)
		{
			LibSleepPrecision.LibTempSet1(timeMS);
			if(timeMS < 2000) {
				Thread.Sleep(timeMS);
			} else { //workaround for Thread.Sleep bug: if there are APC, returns too soon after sleep/hibernate.
				g1:
				long t = MillisecondsWithoutComputerSleepTime;
				Thread.Sleep(timeMS);
				t = timeMS - (MillisecondsWithoutComputerSleepTime - t);
				if(t >= 500) { timeMS = (int)t; goto g1; }
			}
		}

		/// <summary>
		/// Waits <paramref name="timeMS"/> milliseconds. While waiting, retrieves and dispatches Windows messages and other events.
		/// </summary>
		/// <param name="timeMS">Time to wait, milliseconds. Or <see cref="Timeout.Infinite"/>.</param>
		/// <remarks>
		/// Unlike <see cref="Sleep"/>, this function retrieves and dispatches Windows messages, calls .NET/COM event handlers, hook procedures, etc. Supports APC.
		/// This function can be used in threads with windows. However usually there are better ways, for example timer, other thread, async/await/Task. In some places this function does not work as expected, for example in Form/Control mouse event handlers .NET blocks other mouse events.
		/// Be careful, this function is as dangerous as <see cref="Application.DoEvents"/>.
		/// Calls API <msdn>MsgWaitForMultipleObjectsEx</msdn> and <see cref="DoEvents"/>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeMS"/> is negative and not Timeout.Infinite.</exception>
		/// <seealso cref="Util.MessageLoop"/>
		public static unsafe void SleepDoEvents(int timeMS)
		{
			bool _ = false;
			LibSleepDoEvents(timeMS, ref _);
		}

		/// <summary>
		/// The same as <see cref="SleepDoEvents(int)"/>, but also can wait until a variable is set.
		/// </summary>
		/// <param name="timeMS"></param>
		/// <param name="stop">Stop waiting when this variable is set to true. You can set it when processing events/messages/etc while waiting.</param>
		/// <inheritdoc cref="SleepDoEvents(int)"/>
		public static unsafe void SleepDoEvents(int timeMS, ref bool stop)
		{
			LibSleepDoEvents(timeMS, ref stop);
		}

		/// <summary>SleepDoEvents + noSetPrecision.</summary>
		internal static unsafe void LibSleepDoEvents(int timeMS, bool noSetPrecision)
		{
			bool _ = false;
			LibSleepDoEvents(timeMS, ref _, noSetPrecision);
		}

		/// <summary>SleepDoEvents + noSetPrecision.</summary>
		internal static unsafe void LibSleepDoEvents(int timeMS, ref bool stop, bool noSetPrecision = false)
		{
			//rejected: , bool qsSendmessage = false. Not useful.
			//	If thread has windows, hangs if we don't get posted messages.
			//	Else dispatching them usually does not harm.

			if(stop) return;

			if(timeMS == 0) {
				_DoEvents();
				return;
			}

			long tEnd = long.MaxValue;
			if(timeMS > 0) tEnd = MillisecondsWithoutComputerSleepTime + timeMS;
			else if(timeMS < 0 && timeMS != Timeout.Infinite) throw new ArgumentOutOfRangeException();

			if(!noSetPrecision) LibSleepPrecision.LibTempSet1(timeMS);

			while(!stop) {
				int timeSlice = 300; //call API in loop with small timeout to make it respond to Thread.Abort
				if(timeMS > 0) {
					long t = tEnd - MillisecondsWithoutComputerSleepTime;
					if(t <= 0) break;
					if(t < timeSlice) timeSlice = (int)t;
				}
				uint k = Api.MsgWaitForMultipleObjectsEx(0, null, (uint)timeSlice, Api.QS_ALLINPUT, Api.MWMO_ALERTABLE | Api.MWMO_INPUTAVAILABLE);
				//info: k can be 0 (message etc), WAIT_TIMEOUT, WAIT_IO_COMPLETION, WAIT_FAILED.
				if(k == Api.WAIT_FAILED) throw new Win32Exception(); //unlikely, because not using handles
				if(k == 0 && !_DoEvents()) break;
			}
		}

		static bool _DoEvents(bool onlySentMessages = false)
		{
			//note: with PeekMessage don't use |Api.PM_QS_SENDMESSAGE. Then setwineventhook hook does not work. Although eg LL key/mouse hooks work.
			while(Api.PeekMessage(out var m, default, 0, 0, Api.PM_REMOVE)) {
				//Wnd.Misc.PrintMsg(ref m);
				if(!onlySentMessages) {
					if(m.message == Api.WM_QUIT) { Api.PostQuitMessage(m.wParam); return false; }
					Api.TranslateMessage(ref m);
				}
				Api.DispatchMessage(ref m);
			}
			return true;
		}

		/// <summary>
		/// Retrieves and dispatches events and Windows messages from the message queue of this thread.
		/// </summary>
		/// <remarks>
		/// Similar to <see cref="Application.DoEvents"/>, but more lightweight. Uses API functions <msdn>PeekMessage</msdn>, <msdn>TranslateMessage</msdn> and <msdn>DispatchMessage</msdn>.
		/// Be careful, this function is as dangerous as <see cref="Application.DoEvents"/>.
		/// </remarks>
		public static void DoEvents()
		{
			_DoEvents();
		}

		/// <summary>
		/// Waits for a signaled kernel handle.
		/// Calls API <msdn>MsgWaitForMultipleObjectsEx</msdn> with QS_SENDMESSAGE and MWMO_ALERTABLE.
		/// If a handle is signaled, returns its 0-based index. If abandoned mutex, returns 0-based index + Api.WAIT_ABANDONED_0 (0x80). If failed, returns -1.
		/// </summary>
		/// <param name="nHandles">Count.</param>
		/// <param name="handle1"></param>
		/// <param name="handle2"></param>
		/// <param name="handle3"></param>
		/// <param name="handle4"></param>
		/// <remarks>
		/// Dispatches received messages, hook notifications, etc. Uses API PeekMessage/DispatchMessage in loop.
		/// Allows to abort thread.
		/// </remarks>
		internal static unsafe int LibMsgWaitFor(int nHandles, IntPtr handle1, IntPtr handle2 = default, IntPtr handle3 = default, IntPtr handle4 = default)
		{
			var ha = stackalloc IntPtr[4];
			ha[0] = handle1; ha[1] = handle2; ha[2] = handle3; ha[3] = handle4;
			const uint timeSlice = 300; //we call API in loop with small timeout to make it respond to Thread.Abort
			for(; ; ) {
				uint k = Api.MsgWaitForMultipleObjectsEx(nHandles, ha, timeSlice, Api.QS_SENDMESSAGE, Api.MWMO_ALERTABLE | Api.MWMO_INPUTAVAILABLE);
				if(k == nHandles) { //sent message, hook notification, etc. Note: COM RPC uses postmessage; this func not tested.
					_DoEvents(true);
				} else if((k >= 0 && k < nHandles) || (k >= Api.WAIT_ABANDONED_0 && k < Api.WAIT_ABANDONED_0 + nHandles)) {
					return (int)k;
				} else if(k == Api.WAIT_FAILED) return -1;
				//else WAIT_TIMEOUT or WAIT_IO_COMPLETION (APC, aborting thread, etc)
			}
		}

		/// <summary>
		/// Calls <see cref="SleepS"/>.
		/// Example: <c>5.s();</c> is the same as <c>Time.SleepS(5);</c>.
		/// </summary>
		public static void s(this int seconds)
		{
			SleepS(seconds);
		}

		/// <summary>
		/// Calls <see cref="SleepS"/>.
		/// Example: <c>2.5.s();</c> is the same as <c>Time.SleepS(2.5);</c>.
		/// </summary>
		public static void s(this double seconds)
		{
			SleepS(seconds);
		}

		/// <summary>
		/// Calls <see cref="Sleep"/>.
		/// Example: <c>50.ms();</c> is the same as <c>Time.Sleep(50);</c>.
		/// </summary>
		public static void ms(this int milliseconds)
		{
			Sleep(milliseconds);
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
		/// Don't need this for Time.Sleep, Time.SleepS, Time.SleepDoEvents and functions that use them (Mouse.Click etc). They call <see cref="TempSet1"/> when the sleep time is 1-99 ms.
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
	/// Use in UI threads (need a message loop).
	/// </remarks>
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
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or AuDialog.Show(). The callback function is not called while this thread does not do it.
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
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or AuDialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static Timer_ Every(int periodMS, Action<Timer_> callback, object tag = null)
		{
			return _Set(periodMS, false, callback, tag);
		}
	}
}
