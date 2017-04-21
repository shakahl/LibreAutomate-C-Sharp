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
//using System.Xml.Linq;
//using System.Xml.XPath;

using static Catkeys.NoClass;

namespace Catkeys
{
	//[DebuggerStepThrough]
	static partial class WaitFor
	{
		/// <summary>
		/// Can be used to easily implement timeout in wait-for functions.
		/// See how it is used eg in WindowExists or Condition.
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
		/// Waits for an user-defined condition.
		/// Returns true. If timeoutS is negative, on timeout returns false (else exception).
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
