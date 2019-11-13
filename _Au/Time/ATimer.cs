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
	/// This example sets 3 timers.
	/// <code><![CDATA[
	/// ATimer.After(500, _ => Print("after 500 ms"));
	/// ATimer.Every(1000, _ => Print("every 1000 ms"));
	/// var t3 = new ATimer(_ => Print("after 3000 ms")); t3.After(3000); //the same as ATimer.After
	/// MessageBox.Show("");
	/// ]]></code>
	/// </example>
	public class ATimer
	{
		Action<ATimer> _action;
		LPARAM _id;
		int _threadId;
		bool _singlePeriod;

		//To control object lifetime we use a thread-static Dictionary.
		//Tried GCHandle, but could not find a way to delete object when thread ends.
		//Calling KillTimer when thread ends is optional. Need just to re-enable garbage collection for this object.
		[ThreadStatic] static Dictionary<LPARAM, ATimer> t_timers;

		///
		public ATimer(Action<ATimer> timerAction)
		{
			_action = timerAction;
		}

		/// <summary>
		/// Something to attach to this ATimer variable.
		/// </summary>
		public object Tag { get; set; }

		/// <summary>
		/// true if the timer is started and not stopped.
		/// Note: single-period timer is automatically stopped before calling the callback function.
		/// </summary>
		public bool IsRunning => _id != default;

		/// <summary>
		/// Starts one-time timer. If already started, resets and changes its period.
		/// </summary>
		/// <param name="milliseconds">Time interval after which will be called the callback function, milliseconds. The actual minimal interval is 10-20 ms.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative.</exception>
		/// <exception cref="InvalidOperationException">Called not in the same thread as previous <b>Start</b>.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The timer will be stopped before calling the callback function. The callback function can start it again.
		/// If already started, this function must be called in the same thread as when started.
		/// </remarks>
		public void After(int milliseconds, object tag = null) => _Start(true, milliseconds, tag);

		/// <summary>
		/// Starts periodic timer. If already started, resets and changes its period.
		/// </summary>
		/// <param name="milliseconds">Time interval (period) of calling the callback function, milliseconds. The actual minimal period is 10-20 ms.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative.</exception>
		/// <exception cref="InvalidOperationException">Called not in the same thread as previous <b>Start</b>.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The callback function can stop the timer or restart with different period.
		/// If already started, this function must be called in the same thread as when started.
		/// </remarks>
		public void Every(int milliseconds, object tag = null) => _Start(false, milliseconds, tag);

		void _Start(bool singlePeriod, int milliseconds, object tag)
		{
			if(milliseconds < 0) throw new ArgumentOutOfRangeException();
			bool isNew = _id == 0;
			if(!isNew) _CheckThread();
			LPARAM r = Api.SetTimer(default, _id, milliseconds, _timerProc);
			if(r == 0) throw new Win32Exception();
			Debug.Assert(isNew || r == _id);
			_id = r;
			_singlePeriod = singlePeriod;
			Tag = tag;
			if(isNew) {
				_threadId = Thread.CurrentThread.ManagedThreadId;
				(t_timers ??= new Dictionary<LPARAM, ATimer>()).Add(_id, this);
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

			try { t._action(t); }
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
		/// Later you can start the timer again (call <see cref="After(int, object)"/> or <see cref="Every(int, object)"/>).
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
			if(!isSameThread) throw new InvalidOperationException(nameof(ATimer) + " used in multiple threads.");
			//FUTURE: somehow allow other thread. It is often useful.
		}

		//~ATimer() { Print("dtor"); } //don't call Stop() here, we are in other thread

		static ATimer _StartNew(bool singlePeriod, int milliseconds, Action<ATimer> timerAction, object tag = null)
		{
			var t = new ATimer(timerAction);
			t._Start(singlePeriod, milliseconds, tag);
			return t;
		}

		/// <summary>
		/// Creates and starts new one-time timer.
		/// Returns new <see cref="ATimer"/> object. Usually you don't need it.
		/// </summary>
		/// <param name="milliseconds">Time interval after which will be called the callback function, milliseconds. The actual minimal interval is 10-20 ms.</param>
		/// <param name="timerAction">Callback function.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The timer will be stopped before calling the callback function. The callback function can start it again.
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or ADialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static ATimer After(int milliseconds, Action<ATimer> timerAction, object tag = null)
			=> _StartNew(true, milliseconds, timerAction, tag);

		/// <summary>
		/// Creates and starts new periodic timer.
		/// Returns new <see cref="ATimer"/> object that can be used to modify timer properties if you want to do it not in the callback function; usually don't need it.
		/// </summary>
		/// <param name="milliseconds">Time interval (period) of calling the callback function, milliseconds. The actual minimal period is 10-20 ms.</param>
		/// <param name="timerAction">Callback function.</param>
		/// <param name="tag">Something to pass to the callback function as <see cref="Tag"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative.</exception>
		/// <exception cref="Win32Exception">API <msdn>SetTimer</msdn> returned 0. Unlikely.</exception>
		/// <remarks>
		/// The callback function can stop the timer or restart with different period.
		/// The callback function will be called in this thread.
		/// This thread must must get/dispatch posted messages, eg call Application.Run() or Form.ShowModal() or ADialog.Show(). The callback function is not called while this thread does not do it.
		/// </remarks>
		public static ATimer Every(int milliseconds, Action<ATimer> timerAction, object tag = null)
			=> _StartNew(false, milliseconds, timerAction, tag);
	}
}
