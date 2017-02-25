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
	/// Functions that wait for some condition.
	/// </summary>
	//[DebuggerStepThrough]
	static class WaitFor
	{
		/// <summary>
		/// Waits until the specified window can be found.
		/// Returns window handle.
		/// Uses <see cref="Wnd.Find"/>. All parameters etc are the same.
		/// </summary>
		/// <param name="timeoutS">
		/// The maximal time, in seconds, to wait for the window.
		/// If 0, waits indefinitely.
		/// If &gt;0, after timeoutS time throws TimeoutException.
		/// If &lt;0, after -timeoutS time returns Wnd0.
		/// </param>
		/// <param name="name"></param>
		/// <exception cref="TimeoutException">timeoutS time has expired.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Wnd.Find"/>.</exception>
		/// <remarks>
		/// Does not process messages and therefore cannot be called directly in a thread that has windows. But you can use async/await/Task, like in the example.
		/// </remarks>
		/// <example>
		/// Using in a thread that does not have windows.
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
			for(int i = 10; ;) {
				var w = Wnd.Find(name);
				if(!w.Is0) return w;
				if(to.IsTimeout()) return Wnd0;
				Thread.Sleep(i);
				if(i < 200) i++;
			}
		}

		//Not often used. It's easy with await Task.Run. Anyway, need to provide an example of similar size.
		//public static async Task<Wnd> WindowExistsAsync(double timeoutS, string name)
		//{
		//	return await Task.Run(() => WindowExists(timeoutS, name));
		//}

		struct _Timeout
		{
			long _timeRemaining, _timePrev;
			bool _isTimeout, _throw;

			internal _Timeout(double timeoutS)
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
			}

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
		}
	}
}
