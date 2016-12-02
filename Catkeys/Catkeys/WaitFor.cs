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
using Catkeys.Winapi;

namespace Catkeys
{
	//[DebuggerStepThrough]
	public static class Time
	{
		static double _freq = 1000000.0 / Stopwatch.Frequency;

		/// <summary>
		/// Gets the number of microseconds elapsed since Windows startup.
		/// </summary>
		public static long Microseconds { get { return (long)(Stopwatch.GetTimestamp() * _freq); } }

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup.
		/// Unlike Environment.TickCount, this function returns a 64-bit value. Calls Windows API GetTickCount64, not GetTickCount.
		/// The resolution is 10 to 16 milliseconds; use Microseconds if need better resolution.
		/// </summary>
		public static long Milliseconds { get { return Api.GetTickCount64(); } }

		public static void Wait(double timeS)
		{
			int ms;
			if(timeS < 0) ms = Timeout.Infinite;
			else {
				timeS *= 1000.0;
				if(timeS <= int.MaxValue) ms = (int)timeS;
				else throw new ArgumentOutOfRangeException();
			}
			WaitMS(ms);
		}

		public static void WaitMS(int timeMS)
		{
			//if(timeMS < 0) timeMS = Timeout.Infinite;
			Thread.Sleep(timeMS);
			//TODO
		}

#if true
		/// <summary>
		/// Sets new timer for this thread.
		/// Returns new Timer_ object that can be used to change or stop the timer if you want to do it not in the callback function; usually don't need it.
		/// </summary>
		/// <param name="intervalMS">Time interval (period) of calling the callback function, milliseconds. Min interval is 10; if this parameter is 0-9, uses 10. Max interval is int.MaxValue.</param>
		/// <param name="singlePeriod">Call the callback function once, not repeatedly. The timer will be stopped before calling the callback function.</param>
		/// <param name="callback">A callback function (delegate), for example lambda.</param>
		/// <param name="tag">Any object or value to pass to the callback function as Timer_.Tag.</param>
		/// <remarks>
		/// Uses class Time.Timer_, which calls Api.SetTimer().
		/// Similar to System.Windows.Forms.Timer, but has less overhead, for example does not create a hidden window.
		/// The callback function will be called in this thread.
		/// This thread must have a message loop, eg call Application.Run() or Form.ShowModal() or show a modal dialog or menu or repeatedly call Application.DoEvents().
		/// The timer interval precision is 10 ms. For example, if you specify intervalMS=15, actual interval time will be between 10 and 20 ms, not exactly 15.
		/// Some intervals may be longer, because the callback function is not called while this thread is processing something or waiting and not retrieving Windows messages from the thread's message queue.
		/// </remarks>
		public static Timer_ SetTimer(int intervalMS, bool singlePeriod, Action<Timer_> callback, object tag = null)
		{
			var t = new Timer_(callback, tag);
			t.Start(intervalMS, singlePeriod);
			return t;
		}

		/// <summary>
		/// Manages a timer for this thread.
		/// Uses Api.SetTimer() and Api.KillTimer().
		/// Similar to System.Windows.Forms.Timer, but has less overhead, for example does not create a hidden window.
		/// Used by Time.SetTimer(), more info there.
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
			/// The tag parameter of Start() or Time.SetTimer().
			/// The 'set' function can change it.
			/// </summary>
			public object Tag { get; set; }

			/// <summary>
			/// Creates new Timer_ object.
			/// Used by Time.SetTimer(), more info there.
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
			public void Start(int intervalMS, bool singlePeriod)
			{
				if(intervalMS < 0) throw new ArgumentOutOfRangeException("intervalMS");
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
				//Out("Stop: " + _id);
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

			//~Timer_() { Out("dtor"); } //don't call Stop() here, we are in other thread
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
				//Out(i);
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
	}

	//[DebuggerStepThrough]
	public static class WaitFor
	{
		//public static bool WindowIdle(int timeoutMS)
		//{

		//}
	}
}
