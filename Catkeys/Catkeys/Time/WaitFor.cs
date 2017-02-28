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
	/// Provides functions to wait for a condition, such as 'window exists'.
	/// </summary>
	/// <remarks>
	/// All functions have a timeoutS parameter. It is the maximal time to wait, in seconds. If it is 0, waits indefinitely. If &gt;0, after timeoutS time throws <see cref="TimeoutException"/>. If &lt;0, after -timeoutS time just stops waiting and returns default value (false, Wnd0, etc).
	/// 
	/// While waiting, messages and events are not processed. For example, if used in a Form/Control event handler, the form would stop responding. Then need to use another thread, for example async/await/Task, like in the example.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Wnd w = WaitFor.WindowExists(10, "* Notepad");
	/// Print(w);
	/// ]]></code>
	/// Using in a Form/Control event handler.
	/// <code><![CDATA[
	/// var f = new Form();
	/// f.Click += async (unu, sed) =>
	///   {
	/// 	  Print("waiting for Notepad...");
	/// 	  Wnd w = await Task.Run(() => WaitFor.WindowExists(-10, "* Notepad"));
	/// 	  if(w.Is0) Print("timeout"); else Print(w);
	///   };
	/// f.ShowDialog();
	/// ]]></code>
	/// </example>
	//[DebuggerStepThrough]
	static class WaitFor
	{
		/// <summary>
		/// Can be used to easily implement timeout in wait-for functions.
		/// See how it is used eg in WindowExists.
		/// </summary>
		struct _Timeout
		{
			long _timeRemaining, _timePrev;
			bool _isTimeout, _throw;

			/// <summary>
			/// Current period. Initially it is constructor->initPeriod (default 10). The Sleep method increments it until reached MaxPeriod.
			/// </summary>
			internal int Period;

			/// <summary>
			/// Maximal period. It is constructor->maxPeriod (default 200).
			/// </summary>
			internal int MaxPeriod;

			internal _Timeout(double timeoutS, int initPeriod = 10, int maxPeriod = 200)
			{
				if(timeoutS == 0) {
					_timeRemaining = _timePrev = 0;
					_isTimeout = _throw = false;
				} else {
					_timePrev = Time.MillisecondsWithoutSleepTime;
					_timeRemaining = (long)(timeoutS * 1000.0);
					if(_timeRemaining > 0) _throw = true; else { _throw = false; _timeRemaining = -_timeRemaining; }
					_isTimeout = true;
				}
				Period = initPeriod; MaxPeriod = maxPeriod;
			}

			/// <summary>
			/// If the timeout is not expired, returns false.
			/// Else if constructor->timeoutS was negative, returns true.
			/// Else throws TimeoutException.
			/// Also gets current time and updates private fields, if need.
			/// </summary>
			internal bool IsTimeout()
			{
				if(!_isTimeout) return false;
				var t = Time.MillisecondsWithoutSleepTime;
				_timeRemaining -= t - _timePrev;
				_timePrev = t;
				if(_timeRemaining > 0) return false;
				if(_throw) throw new TimeoutException("Wait timeout.");
				return true;
			}

			/// <summary>
			/// If IsTimeout(), returns false.
			/// Else sleeps for Period milliseconds, increments Period if it is less than MaxPeriod, and returns true.
			/// </summary>
			/// <returns></returns>
			internal bool Sleep()
			{
				if(IsTimeout()) return false;
				Thread.Sleep(Period);
				if(Period < MaxPeriod) Period++;
				return true;
			}
		}

		/// <summary>
		/// Waits until exists a window that matches the specified parameters.
		/// Returns window handle.
		/// Uses <see cref="Wnd.Find"/>. All parameters etc are the same.
		/// </summary>
		/// <param name="timeoutS">
		/// The maximal time to wait, in seconds. If 0, waits indefinitely. If &gt;0, after timeoutS time throws <b>TimeoutException</b>. If &lt;0, after -timeoutS time returns Wnd0.
		/// </param>
		/// <param name="name"></param>
		/// <exception cref="TimeoutException">timeoutS time has expired.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Wnd.Find"/>.</exception>
		/// <example>
		/// <code><![CDATA[
		/// Wnd w = WaitFor.WindowExists(10, "* Notepad");
		/// Print(w);
		/// ]]></code>
		/// Using in a Form/Control event handler.
		/// <code><![CDATA[
		/// var f = new Form();
		/// f.Click += async (unu, sed) =>
		///   {
		/// 	  Print("waiting for Notepad...");
		/// 	  Wnd w = await Task.Run(() => WaitFor.WindowExists(-10, "* Notepad"));
		/// 	  if(w.Is0) Print("timeout"); else Print(w);
		///   };
		/// f.ShowDialog();
		/// ]]></code>
		/// </example>
		public static Wnd WindowExists(double timeoutS, string name)
		{
			var to = new _Timeout(timeoutS);
			var f = new Wnd.FindParams(name);
			for(;;) {
				if(f.Find()) return f.Result;
				if(!to.Sleep()) return Wnd0;
			}
		}

		//Not often used. It's easy with await Task.Run. Anyway, need to provide an example of similar size.
		//public static async Task<Wnd> WindowExistsAsync(double timeoutS, string name)
		//{
		//	return await Task.Run(() => WindowExists(timeoutS, name));
		//}

		/// <summary>
		/// Waits until does not exist a window that matches the specified parameters.
		/// Returns true.
		/// Uses <see cref="Wnd.Find"/>. All parameters etc are the same.
		/// Examples: see <see cref="WindowExists"/>.
		/// </summary>
		/// <param name="timeoutS">
		/// The maximal time to wait, in seconds. If 0, waits indefinitely. If &gt;0, after timeoutS time throws <b>TimeoutException</b>. If &lt;0, after -timeoutS time returns false.
		/// </param>
		/// <param name="name"></param>
		/// <exception cref="TimeoutException">timeoutS time has expired.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Wnd.Find"/>.</exception>
		public static bool WindowNotExists(double timeoutS, string name)
		{
			var to = new _Timeout(timeoutS);
			var f = new Wnd.FindParams(name);
			Wnd w = Wnd0;
			for(;;) {
				if(!w.IsAlive) {
					if(!f.Find()) return true;
					w = f.Result;
				}
				if(!to.Sleep()) return false;
			}
		}

		/// <summary>
		/// Waits until the window handle is invalid.
		/// Returns true.
		/// Uses <see cref="Wnd.IsAlive"/>.
		/// More info: <see cref="WindowExists"/>
		/// </summary>
		/// <param name="timeoutS">
		/// The maximal time to wait, in seconds. If 0, waits indefinitely. If &gt;0, after timeoutS time throws <b>TimeoutException</b>. If &lt;0, after -timeoutS time returns false.
		/// </param>
		/// <param name="w"></param>
		/// <exception cref="TimeoutException">timeoutS time has expired.</exception>
		public static bool WindowClosed(double timeoutS, Wnd w)
		{
			var to = new _Timeout(timeoutS);
			for(;;) {
				if(!w.IsAlive) return true;
				if(!to.Sleep()) return false;
			}
		}

		/// <summary>
		/// Waits for an user-defined condition.
		/// Returns true.
		/// </summary>
		/// <param name="timeoutS">
		/// The maximal time to wait, in seconds. If 0, waits indefinitely. If &gt;0, after timeoutS time throws <b>TimeoutException</b>. If &lt;0, after -timeoutS time returns false.
		/// </param>
		/// <param name="condition">Callback function (eg lambda). It is called repeatedly, until returns true.</param>
		/// <param name="param">Something to pass to the callback function.</param>
		/// <param name="minPeriod">The initial period of calling the callback function, in milliseconds.</param>
		/// <param name="maxPeriod">The maximal period of calling the callback function, in milliseconds. The period is incremented by 1 millisecond in each loop until it reaches maxPeriod. It gives a good response time initially, and small CPU usage after some time.</param>
		/// <exception cref="TimeoutException">timeoutS time has expired.</exception>
		/// <exception cref="ArgumentException">minPeriod &lt; 1 or maxPeriod &lt; minPeriod.</exception>
		/// <exception cref="Exception">Exceptions thrown by the condition callback function.</exception>
		/// <seealso cref="Time.SystemWaitPrecision"/>
		public static bool Condition(double timeoutS, Func<object, bool> condition, object param = null, int minPeriod = 10, int maxPeriod = 200)
		{
			if(minPeriod < 1 || maxPeriod < minPeriod) throw new ArgumentException();
			var to = new _Timeout(timeoutS, minPeriod, maxPeriod);
			for(;;) {
				if(condition(param)) return true;
				if(!to.Sleep()) return false;
			}
		}

	}
}
