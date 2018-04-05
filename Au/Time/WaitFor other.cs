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
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	//[DebuggerStepThrough]
	public static partial class WaitFor
	{
		/// <summary>
		/// Can be used to easily implement timeout in wait-for functions.
		/// See how it is used eg in <see cref="Condition"/>.
		/// </summary>
		internal struct LibTimeout
		{
			long _timeRemaining, _timePrev;
			bool _hasTimeout, _throw;

			/// <summary>
			/// Current period, milliseconds.
			/// Initially it is constructor->initPeriod (default 10). The Sleep method increments it until reached MaxPeriod.
			/// </summary>
			public int Period { get; set; }

			/// <summary>
			/// Maximal period, milliseconds.
			/// It is constructor->maxPeriod (default 500).
			/// </summary>
			public int MaxPeriod { get; set; }

			/// <summary>
			/// Remaining time, milliseconds.
			/// </summary>
			public long TimeRemaining { get => _timeRemaining; set => _timeRemaining = value; }

			public LibTimeout(double secondsTimeout, int initPeriod = 10, int maxPeriod = 500)
			{
				if(secondsTimeout == 0) {
					_timeRemaining = _timePrev = 0;
					_hasTimeout = _throw = false;
				} else {
					_timePrev = Time.MillisecondsWithoutComputerSleepTime;
					_timeRemaining = (long)(secondsTimeout * 1000.0);
					if(_timeRemaining > 0) _throw = true; else { _throw = false; _timeRemaining = -_timeRemaining; }
					_hasTimeout = true;
				}
				Period = initPeriod; MaxPeriod = maxPeriod;
			}

			/// <summary>
			/// If the timeout is not expired, returns false.
			/// Else if constructor->secondsTimeout was negative, returns true.
			/// Else throws TimeoutException.
			/// Also gets current time and updates private fields, if need.
			/// </summary>
			public bool IsTimeout()
			{
				if(!_hasTimeout) return false;
				var t = Time.MillisecondsWithoutComputerSleepTime;
				_timeRemaining -= t - _timePrev;
				_timePrev = t;
				if(_timeRemaining > 0) return false;
				if(_throw) throw new TimeoutException();
				return true;
			}

			/// <summary>
			/// If IsTimeout(), returns false.
			/// Else sleeps for Period milliseconds, increments Period if it is less than MaxPeriod, and returns true.
			/// </summary>
			public bool Sleep()
			{
				if(IsTimeout()) return false;
				Thread.Sleep(Period);
				if(Period < MaxPeriod) Period++;
				return true;
			}
		}

		/// <summary>
		/// Waits for an user-defined condition.
		/// Returns true. On timeout returns false if <paramref name="secondsTimeout"/> is negative; else exception.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns false.
		/// </param>
		/// <param name="condition">Callback function (eg lambda). It is called repeatedly, until returns true.</param>
		/// <param name="param">Something to pass to the callback function.</param>
		/// <param name="minPeriod">The initial period of calling the callback function, in milliseconds.</param>
		/// <param name="maxPeriod">The maximal period of calling the callback function, in milliseconds. The period is incremented by 1 millisecond in each loop until it reaches maxPeriod. It gives a good response time initially, and small CPU usage after some time.</param>
		/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
		/// <exception cref="ArgumentException">minPeriod &lt; 1 or maxPeriod &lt; minPeriod.</exception>
		/// <exception cref="Exception">Exceptions thrown by the condition callback function.</exception>
		public static bool Condition(double secondsTimeout, Func<object, bool> condition, object param = null, int minPeriod = 10, int maxPeriod = 200)
		{
			if(minPeriod < 1 || maxPeriod < minPeriod) throw new ArgumentException();
			var to = new LibTimeout(secondsTimeout, minPeriod, maxPeriod);
			for(;;) {
				if(condition(param)) return true;
				if(!to.Sleep()) return false;
			}
		}

		/// <summary>
		/// Waits while some modifier keys (Ctrl, Shift, Alt, Win) are in pressed state.
		/// Returns true. On timeout returns false if <paramref name="secondsTimeout"/> is negative; else exception.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns false.
		/// </param>
		/// <param name="modifierKeys">Check only these keys. Default - all.</param>
		/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
		/// <seealso cref="Keyb.IsMod"/>
		public static bool NoModifierKeys(double secondsTimeout = 0.0, KMod modifierKeys = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win)
		{
			//return WaitFor.Condition(secondsTimeout, o => !Keyb.LibIsModifiers(modifierKeys)); //shorter but creates garbage
			var to = new LibTimeout(secondsTimeout);
			for(;;) {
				if(!Keyb.IsMod(modifierKeys)) return true;
				if(!to.Sleep()) return false;
			}
		}

		/// <summary>
		/// Waits while some mouse buttons are in pressed state.
		/// Returns true. On timeout returns false if <paramref name="secondsTimeout"/> is negative; else exception.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns false.
		/// </param>
		/// <param name="buttons">Wait only for these buttons. Default - all.</param>
		/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
		/// <seealso cref="Mouse.IsPressed"/>
		public static bool NoMouseButtons(double secondsTimeout = 0.0, MouseButtons buttons= MouseButtons.Left | MouseButtons.Right | MouseButtons.Middle | MouseButtons.XButton1 | MouseButtons.XButton2)
		{
			var to = new LibTimeout(secondsTimeout);
			for(;;) {
				if(!Mouse.IsPressed(buttons)) return true;
				if(!to.Sleep()) return false;
			}
		}

		/// <summary>
		/// Waits while some modifier keys (Ctrl, Shift, Alt, Win) or mouse buttons are in pressed state.
		/// Returns true. On timeout returns false if <paramref name="secondsTimeout"/> is negative; else exception.
		/// </summary>
		/// <param name="secondsTimeout">
		/// The maximal time to wait, seconds. If 0, waits indefinitely. If &gt;0, after that time interval throws <see cref="TimeoutException"/>. If &lt;0, after that time interval returns false.
		/// </param>
		/// <param name="modifierKeys">Check only these keys. Default - all.</param>
		/// <param name="buttons">Check only these buttons. Default - all.</param>
		/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
		/// <seealso cref="Keyb.IsMod"/>
		/// <seealso cref="Mouse.IsPressed"/>
		public static bool NoModifierKeysAndMouseButtons(double secondsTimeout = 0.0, KMod modifierKeys = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win, MouseButtons buttons = MouseButtons.Left | MouseButtons.Right | MouseButtons.Middle | MouseButtons.XButton1 | MouseButtons.XButton2)
		{
			var to = new LibTimeout(secondsTimeout);
			for(;;) {
				if(!Keyb.IsMod(modifierKeys) && !Mouse.IsPressed(buttons)) return true;
				if(!to.Sleep()) return false;
			}
		}


	}
}
