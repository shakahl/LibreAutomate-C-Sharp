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
	/// Timer that uses API <msdn>SetTimer</msdn> and API <msdn>KillTimer</msdn>.
	/// </summary>
	/// <remarks>
	/// Similar to System.Windows.Forms.Timer, but more lightweight, for example does not create a hidden window.
	/// Use in UI threads. Does not work if this thread does not retrieve/dispatch posted messages (<msdn>WM_TIMER</msdn>).
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// //this example sets 3 timers
	/// ATimer.After(500, () => Print("after 500 ms"));
	/// ATimer.Every(1000, () => Print("every 1000 ms"));
	/// var t3 = new ATimer(() => Print("after 3000 ms")); t3.Start(3000, true); //the same as ATimer.After
	/// MessageBox.Show("");
	/// ]]></code>
	/// </example>
	public class ATimer
	{
		Delegate _action; //Action<ATimer> or Action
		LPARAM _id;
		int _threadId;
		bool _singlePeriod;

		//To control object lifetime we use a thread-static Dictionary.
		//Tried GCHandle, but could not find a way to delete object when thread ends.
		//Calling KillTimer when thread ends is optional. Need just to re-enable garbage collection for this object.
		[ThreadStatic] static Dictionary<LPARAM, ATimer> t_timers;

		/// <summary>
		/// Some object or value attached to this ATimer variable.
		/// </summary>
		public object Tag { get; set; }

		///
		public ATimer(Action<ATimer> timerAction, object tag = null) : this((Delegate)timerAction, tag) { }

		///
		public ATimer(Action timerAction, object tag = null) : this((Delegate)timerAction, tag) { }

		ATimer(Delegate timerAction, object tag)
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
				if(t_timers == null) t_timers = new Dictionary<LPARAM, ATimer>();
				t_timers.Add(_id, this);
			}
			//Print($"Start: {_id}  isNew={isNew}  singlePeriod={singlePeriod}  _threadId={_threadId}");
		}

		static Api.TIMERPROC _timerProc = _TimerProc;
		static void _TimerProc(AWnd w, int msg, LPARAM idEvent, uint time)
		{
			//Print(t_timers.Count, idEvent);
			if(!t_timers.TryGetValue(idEvent, out var t)) {
				//ADebug.Print($"timer id {idEvent} not in t_timers");
				return;
				//It is possible after killing timer.
				//	Normally API KillTimer removes WM_TIMER message from queue (tested), but in some conditions our callback can still be called several times.
				//	For example if multiple messages are retrieved from the OS queue without dispatching each, and then all are dispatched.
				//	Usually we can safely ignore it. But not good if the same timer id is reused for another timer. Tested on Win10: OS does not reuse ids soon.
			}
			if(t._singlePeriod) t.Stop();

			try {
				switch(t._action) {
				case Action<ATimer> f: f(t); break;
				case Action f: f(); break;
				}
			}
			catch(ThreadAbortException) { t.Stop(); }
			catch(Exception ex) { PrintWarning(ex.ToString(), -1); }
			//info: OS handles exceptions in timer procedure.
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
			if(!isSameThread) throw new InvalidOperationException(nameof(ATimer) + " used by multiple threads.");
			//FUTURE: somehow allow other thread. It is often useful.
		}

		//~ATimer() { Print("dtor"); } //don't call Stop() here, we are in other thread

		static ATimer _Set(int time, bool singlePeriod, Delegate timerAction, object tag = null)
		{
			var t = new ATimer(timerAction, tag);
			t.Start(time, singlePeriod);
			return t;
		}

		/// <summary>
		/// Sets new one-time timer.
		/// Returns new <see cref="ATimer"/> object. Usually you don't need it.
		/// </summary>
		/// <param name="timeMilliseconds">Time after which will be called the callback function, milliseconds. The minimal time is 10-20, even if this parameter is less than that.</param>
		/// <param name="timerAction">Callback function.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative periodMilliseconds.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or ADialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static ATimer After(int timeMilliseconds, Action timerAction, object tag = null)
		{
			return _Set(timeMilliseconds, true, timerAction, tag);
		}

		///
		public static ATimer After(int timeMilliseconds, Action<ATimer> timerAction, object tag = null)
		{
			return _Set(timeMilliseconds, true, timerAction, tag);
		}

		/// <summary>
		/// Sets new periodic timer.
		/// Returns new <see cref="ATimer"/> object that can be used to modify timer properties if you want to do it not in the callback function; usually don't need it.
		/// </summary>
		/// <param name="periodMilliseconds">Time interval (period) of calling the callback function, milliseconds. The minimal period is 10-20, even if specified smaller.</param>
		/// <param name="timerAction">Callback function.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative periodMilliseconds.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or ADialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static ATimer Every(int periodMilliseconds, Action timerAction, object tag = null)
		{
			return _Set(periodMilliseconds, false, timerAction, tag);
		}

		///
		public static ATimer Every(int periodMilliseconds, Action<ATimer> timerAction, object tag = null)
		{
			return _Set(periodMilliseconds, false, timerAction, tag);
		}
	}
}
