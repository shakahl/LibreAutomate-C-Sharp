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
using static Au.AStatic;

namespace Au
{
	/// <summary>
	/// Time functions. Get time, sleep/wait, doevents.
	/// </summary>
	/// <seealso cref="APerf"/>
	/// <seealso cref="AWaitFor"/>
	[DebuggerStepThrough]
	public static class ATime
	{
		//info: we don't use Stopwatch because it loads System.dll, which is slow and can make speed measurement incorrect and confusing in some cases.

#if false //makes some simplest functions much slower, eg MillisecondsFast.
		static ATime()
		{
			if(Api.QueryPerformanceFrequency(out long f)) {
				s_freqMCS = 1_000_000d / f;
				s_freqMS = 1000d / f;
			}
		}
		internal static double s_freqMCS, s_freqMS; //s_freqMCS used by APerf too
#elif true
		internal static readonly double s_freqMCS = _Freq(1_000_000d); //used by APerf too
		internal static readonly double s_freqMS = _Freq(1000d);

		static double _Freq(double d) => Api.QueryPerformanceFrequency(out long f) ? d / f : 0d;
#else //fast, same as above
		internal static double s_freqMCS {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				var r = s_freqMCS2;
				if(r == 0d) _InitFreq();
				return r;
			}
		}

		internal static double s_freqMS {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get {
				var r = s_freqMS2;
				if(r == 0d) _InitFreq();
				return r;
			}
		}
		static double s_freqMCS2, s_freqMS2;

		static void _InitFreq()
		{
			if(Api.QueryPerformanceFrequency(out long f)) {
				s_freqMCS2 = 1_000_000d / f;
				s_freqMS2 = 1000d / f;
			}
		}
#endif

		/// <summary>
		/// Gets the number of microseconds elapsed since Windows startup. Uses the high-resolution system timer.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryPerformanceCounter</msdn>.
		/// This function is used to measure time differences with 1 microsecond precision, like <c>var t1=ATime.PerfMicroseconds; ... var t2=ATime.PerfMicroseconds; var diff=t2-t1;</c>.
		/// Independent of computer clock time changes.
		/// MSDN article: <msdn>Acquiring high-resolution time stamps</msdn>.
		/// </remarks>
		/// <seealso cref="APerf"/>
		public static long PerfMicroseconds { get { Api.QueryPerformanceCounter(out var t); return (long)(t * s_freqMCS); } }

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup. Uses the high-resolution system timer.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryPerformanceCounter</msdn>.
		/// This function is used to measure time differences with 1 ms precision, like <c>var t1=ATime.PerfMilliseconds; ... var t2=ATime.PerfMilliseconds; var diff=t2-t1;</c>.
		/// The return value equals <see cref="PerfMicroseconds"/>/1000 but is slightly different than of <see cref="WinMilliseconds"/> and other similar functions of this library, Windows API and .NET. Never compare times returned by different functions.
		/// Independent of computer clock time changes.
		/// </remarks>
		public static long PerfMilliseconds { get { Api.QueryPerformanceCounter(out var t); return (long)(t * s_freqMS); } }
		//On current OS and hardware QueryPerformanceCounter is reliable and fast enough.
		//Speed of 1000 calls when cold CPU: PerfMilliseconds 97, WinMillisecondsWithoutSleep 76, WinMilliseconds64/GetTickCount64 12.
		//rejected: make corrections based on GetTickCount64. It makes slower and is not necessary.

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>GetTickCount64</msdn>.
		/// Like <see cref="Environment.TickCount"/>, but returns a 64-bit value that does not overflow.
		/// Uses the low-resolution system timer. Its period usually is 15.25 ms. When need to measure time differences with better precision, use <see cref="PerfMilliseconds"/> or <see cref="PerfMicroseconds"/>.
		/// Independent of computer clock time changes.
		/// </remarks>
		public static long WinMilliseconds => Api.GetTickCount64();

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>GetTickCount</msdn>.
		/// The same as <see cref="Environment.TickCount"/>. Returns a 32-bit value that overflows after 24.9 days.
		/// Uses the low-resolution system timer. Its period usually is 15.25 ms.
		/// Independent of computer clock time changes.
		/// </remarks>
		public static int WinMilliseconds32 => Api.GetTickCount();

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup, not including the time when the computer sleeps or hibernates.
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>QueryUnbiasedInterruptTime</msdn>.
		/// Uses the low-resolution system timer. Its period usually is 15.25 ms.
		/// Independent of computer clock time changes.
		/// </remarks>
		public static long WinMillisecondsWithoutSleep {
			get {
				if(!Api.QueryUnbiasedInterruptTime(out long t)) return Api.GetTickCount64();
				return t / 10000;
			}
		}

		/// <summary>
		/// Waits <i>timeMilliseconds</i> milliseconds.
		/// </summary>
		/// <param name="timeMilliseconds">Time to wait, milliseconds. Or <see cref="Timeout.Infinite"/>.</param>
		/// <remarks>
		/// Calls <see cref="Thread.Sleep(int)"/>.
		/// Does not process Windows messages and other events, therefore should not be used in threads with windows, timers, hooks, events or COM, unless <i>timeMilliseconds</i> is small. Supports APC.
		/// If the computer goes to sleep or hibernate during that time, the real time is the specified time + the sleep/hibernate time.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><i>timeMilliseconds</i> is negative and not Timeout.Infinite (-1).</exception>
		/// <example>
		/// <code><![CDATA[
		/// ATime.Sleep(50);
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
				long t = WinMillisecondsWithoutSleep;
				Thread.Sleep(timeMilliseconds);
				t = timeMilliseconds - (WinMillisecondsWithoutSleep - t);
				if(t >= 500) { timeMilliseconds = (int)t; goto g1; }
			}
		}

		/// <summary>
		/// Waits <i>timeMilliseconds</i> milliseconds. The same as <see cref="Sleep"/>.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException"><i>timeMilliseconds</i> is negative and not Timeout.Infinite (-1).</exception>
		/// <example>
		/// <code><![CDATA[
		/// 50.ms();
		/// ]]></code>
		/// </example>
		public static void ms(this int timeMilliseconds)
		{
			Sleep(timeMilliseconds);
		}

		/// <summary>
		/// Waits <i>timeSeconds</i> seconds.
		/// The same as <see cref="Sleep"/> and <see cref="ms"/>, but the time is specified in seconds, not milliseconds.
		/// </summary>
		/// <param name="timeSeconds">Time to wait, seconds.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>timeSeconds</i> is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		/// <example>
		/// <code><![CDATA[
		/// ATime.Sleep(5000);
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
		/// Waits <i>timeSeconds</i> seconds.
		/// The same as <see cref="Sleep"/> and <see cref="ms"/>, but the time is specified in seconds, not milliseconds.
		/// </summary>
		/// <param name="timeSeconds">Time to wait, seconds. The smallest value is 0.001 (1 ms).</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>timeSeconds</i> is less than 0 or greater than 2147483 (int.MaxValue/1000, 24.8 days).</exception>
		/// <example>
		/// <code><![CDATA[
		/// ATime.Sleep(2500);
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
		/// Waits <i>timeMS</i> milliseconds. While waiting, retrieves and dispatches Windows messages and other events.
		/// </summary>
		/// <param name="timeMS">Time to wait, milliseconds. Or <see cref="Timeout.Infinite"/>.</param>
		/// <remarks>
		/// Unlike <see cref="Sleep"/>, this function retrieves and dispatches Windows messages, calls .NET event handlers, hook procedures, timer functions, COM/RPC, etc. Supports APC.
		/// This function can be used in threads with windows. However usually there are better ways, for example timer, other thread, async/await/Task. In some places this function does not work as expected, for example in Form/Control mouse event handlers .NET blocks other mouse events.
		/// Be careful, this function is as dangerous as <see cref="System.Windows.Forms.Application.DoEvents"/>.
		/// Calls API <msdn>MsgWaitForMultipleObjectsEx</msdn> and <see cref="DoEvents"/>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><i>timeMS</i> is negative and not Timeout.Infinite.</exception>
		/// <seealso cref="AWaitFor.MessagesAndCondition"/>
		/// <seealso cref="AWaitFor.PostedMessage"/>
		/// <seealso cref="Util.AMessageLoop"/>
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

			AWaitFor.LibWait(timeMS, WHFlags.DoEvents, null, null);
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
				//AWnd.More.PrintMsg(m);
				if(m.message == Api.WM_QUIT) { Api.PostQuitMessage((int)m.wParam); break; }
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
		/// Don't need this for ATime.SleepX and functions that use them (AMouse.Click etc). They call <see cref="TempSet1"/> when the sleep time is 1-99 ms.
		/// This does not change the minimal period of <see cref="ATimer"/> and System.Windows.Forms.Timer.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// _Test("before");
		/// using(new ATime.LibSleepPrecision(2)) {
		/// 	_Test("in");
		/// }
		/// _Test("after");
		/// 
		/// void _Test(string name)
		/// {
		/// 	Print(name);
		/// 	APerf.First();
		/// 	for(int i = 0; i < 8; i++) { Thread.Sleep(1); APerf.Next(); }
		/// 	APerf.Write();
		/// }
		/// ]]></code>
		/// </example>
		internal sealed class LibSleepPrecision : IDisposable
		{
			//info: this class could be public, but probably not useful. ATime.Sleep automatically sets 1 ms period if need.

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
				//	It means that if this app eg has set 10 ms resolution, then ATime.Sleep(1) will sleep 10 ms and not the normal 1-2 ms.
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
			public static float Current {
				get {
					if(0 != Api.NtQueryTimerResolution(out _, out _, out var t)) return 0f;
					return (float)t / 10000;
				}
			}

			/// <summary>
			/// Temporarily sets the system wait precision to 1 ms. It will be revoked after the specified time or when this process ends.
			/// If already set, just updates the revoking time.
			/// </summary>
			/// <param name="endAfterMS">Revoke after this time, milliseconds.</param>
			/// <example>
			/// <code><![CDATA[
			/// Print(ATime.LibSleepPrecision.Current); //probably 15.625
			/// ATime.LibSleepPrecision.TempSet1(500);
			/// Print(ATime.LibSleepPrecision.Current); //1
			/// Thread.Sleep(600);
			/// Print(ATime.LibSleepPrecision.Current); //probably 15.625 again
			/// ]]></code>
			/// </example>
			public static void TempSet1(int endAfterMS = 1111)
			{
				lock("2KgpjPxRck+ouUuRC4uBYg") {
					s_TS1_EndTime = WinMillisecondsWithoutSleep + endAfterMS;
					if(s_TS1_Obj == null) {
						s_TS1_Obj = new LibSleepPrecision(1); //info: instead could call the API directly, but may need to auto-revoke using the finalizer
						ThreadPool.QueueUserWorkItem(endAfterMS2 => {
							Thread.Sleep((int)endAfterMS2); //note: don't use captured variables. It creates new garbage all the time.
							for(; ; ) {
								int t;
								lock("2KgpjPxRck+ouUuRC4uBYg") {
									t = (int)(s_TS1_EndTime - WinMillisecondsWithoutSleep);
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
			//never mind: finalizer is not called on process exit. Not a problem, because OS clears our set value (tested). Or we could use AProcess.Exit event.

			/// <summary>
			/// Calls TempSet1 if sleepTimeMS is 1-99.
			/// </summary>
			/// <param name="sleepTimeMS">milliseconds of the caller 'sleep' function.</param>
			internal static void LibTempSet1(int sleepTimeMS)
			{
				if(sleepTimeMS < 100 && sleepTimeMS > 0) TempSet1(1111);
			}
		}
	}
}
