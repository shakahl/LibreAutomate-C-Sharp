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
		/// Can be used to easily implement 'wait for' functions with a timeout.
		/// </summary>
		/// <remarks>
		/// See examples. The code works like most 'wait for' functions of this library: on timeout throws exception, unless secondsTimeout is negative.
		/// Similar code is used by most 'wait for' functions of this library.
		/// See also <see cref="Condition"/>; usually it's easier; internally it uses similar code too.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// public static bool WaitForMouseLeftButtonDown(double secondsTimeout)
		/// {
		/// 	var x = new WaitFor.Loop(secondsTimeout);
		/// 	for(; ; ) {
		/// 		if(Mouse.IsPressed(MouseButtons.Left)) return true;
		/// 		if(!x.Sleep()) return false;
		/// 	}
		/// }
		/// ]]></code>
		/// The same with WaitFor.Condition.
		/// <code><![CDATA[
		/// static bool WaitForMouseLeftButtonDown2(double secondsTimeout)
		/// {
		/// 	return WaitFor.Condition(secondsTimeout, o => Mouse.IsPressed(MouseButtons.Left));
		/// }
		/// ]]></code>
		/// </example>
		public struct Loop
		{
			long _timeRemaining, _timePrev;
			bool _hasTimeout, _throw, _precisionIsSet;

			/// <summary>
			/// Current period (<see cref="Sleep"/> sleep time), milliseconds.
			/// Initially it is constructor's <i>initPeriod</i> argument (default 10). Then <see cref="Sleep"/> increments it until reached <see cref="MaxPeriod"/>.
			/// </summary>
			public int Period { get; set; }

			/// <summary>
			/// Maximal period (<see cref="Sleep"/> sleep time), milliseconds.
			/// It is constructor's <i>maxPeriod</i> argument (default 500).
			/// </summary>
			public int MaxPeriod { get; set; }

			/// <summary>
			/// Gets or sets the remaining time, milliseconds.
			/// </summary>
			public long TimeRemaining { get => _timeRemaining; set => _timeRemaining = value; }

			/// <param name="secondsTimeout">
			/// The maximal time to wait in the loop, seconds. If 0, waits indefinitely. If &gt;0, after that time interval <see cref="Sleep"/> throws <see cref="TimeoutException"/>. If &lt;0, after that time interval <see cref="Sleep"/> returns false.
			/// </param>
			/// <param name="initPeriod">The initial sleep time of <see cref="Sleep"/>, milliseconds. Default 10.</param>
			/// <param name="maxPeriod">The maximal sleep time of <see cref="Sleep"/>, milliseconds. Default 500. The period is incremented by 1 millisecond in each loop until it reaches <paramref name="maxPeriod"/>. It gives a good response time initially, and small CPU usage after some time.</param>
			/// <exception cref="ArgumentOutOfRangeException"><paramref name="initPeriod"/> &lt; 1 or <paramref name="maxPeriod"/> &lt; <paramref name="initPeriod"/>.</exception>
			public Loop(double secondsTimeout, int initPeriod = 10, int maxPeriod = 500)
			{
				if(initPeriod < 1 || maxPeriod < initPeriod) throw new ArgumentOutOfRangeException();
				if(secondsTimeout == 0) {
					_timeRemaining = _timePrev = 0;
					_hasTimeout = _throw = false;
				} else {
					_timePrev = Time.MillisecondsWithoutComputerSleepTime;
					_timeRemaining = checked((long)(secondsTimeout * 1000d));
					if(_timeRemaining > 0) _throw = true; else { _throw = false; _timeRemaining = -_timeRemaining; }
					_hasTimeout = true;
				}
				Period = initPeriod; MaxPeriod = maxPeriod;
				_precisionIsSet = false;
			}

			/// <summary>
			/// If the <i>secondsTimeout</i> time is not expired, returns false.
			/// Else if <i>secondsTimeout</i> is negative, returns true.
			/// Else throws <see cref="TimeoutException"/>.
			/// Also updates private fields, if need.
			/// </summary>
			/// <exception cref="TimeoutException">The <i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
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
			/// If <see cref="IsTimeout"/> returns true, returns false.
			/// Else sleeps for <see cref="Period"/> milliseconds, increments <b>Period</b> if it is less than <see cref="MaxPeriod"/>, and returns true.
			/// </summary>
			/// <exception cref="TimeoutException">The <i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
			public bool Sleep()
			{
				if(IsTimeout()) return false;
				if(Period < 10 && !_precisionIsSet) { //default Period is 10
					_precisionIsSet = true;
					Time.LibSleepPrecision.TempSet1();
				}
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
		/// <param name="initPeriod">The initial period of calling the callback function, milliseconds. Default 10.</param>
		/// <param name="maxPeriod">The maximal period of calling the callback function, milliseconds. Default 500. The period is incremented by 1 millisecond in each loop until it reaches <paramref name="maxPeriod"/>. It gives a good response time initially, and small CPU usage after some time.</param>
		/// <exception cref="TimeoutException"><paramref name="secondsTimeout"/> time has expired (if &gt; 0).</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="initPeriod"/> &lt; 1 or <paramref name="maxPeriod"/> &lt; <paramref name="initPeriod"/>.</exception>
		/// <exception cref="Exception">Exceptions thrown by the condition callback function.</exception>
		public static bool Condition(double secondsTimeout, Func<object, bool> condition, object param = null, int initPeriod = 10, int maxPeriod = 500)
		{
			var to = new Loop(secondsTimeout, initPeriod, maxPeriod);
			for(; ; ) {
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
			//return WaitFor.Condition(secondsTimeout, o => !Keyb.LibIsModifiers(modifierKeys), 10, 100); //shorter code but creates garbage
			var to = new Loop(secondsTimeout, 10, 100);
			for(; ; ) {
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
		public static bool NoMouseButtons(double secondsTimeout = 0.0, MouseButtons buttons = MouseButtons.Left | MouseButtons.Right | MouseButtons.Middle | MouseButtons.XButton1 | MouseButtons.XButton2)
		{
			var to = new Loop(secondsTimeout, 10, 100);
			for(; ; ) {
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
			var to = new Loop(secondsTimeout);
			for(; ; ) {
				if(!Keyb.IsMod(modifierKeys) && !Mouse.IsPressed(buttons)) return true;
				if(!to.Sleep()) return false;
			}
		}


	}
}
