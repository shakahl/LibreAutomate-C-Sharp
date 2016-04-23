using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
//using System.Linq;
using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;

using Catkeys;
using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

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
			if(timeMS < 0) timeMS = Timeout.Infinite;
			Thread.Sleep(timeMS);
			//TODO
		}
	}

	//[DebuggerStepThrough]
	public static class WaitFor
	{
	}
}
