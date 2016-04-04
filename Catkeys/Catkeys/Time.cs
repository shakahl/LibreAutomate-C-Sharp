using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using System.Linq;
using System.Threading;
//using System.Threading.Tasks;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.CompilerServices;
//using System.IO;
//using System.Windows.Forms;

using static Catkeys.NoClass;
using Catkeys.Util; using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	/// <summary>
	/// Code speed measurement, and other time functions.
	/// Easier to use than System.Stopwatch class.
	/// </summary>
	[DebuggerStepThrough]
	public static class Time
	{
		#region FirstNextWrite

		static long[] _a = new long[11];
		static uint _counter = 0;
		static double _freq = 1000000.0/Stopwatch.Frequency;

		/// <summary>
		/// Stores current time in the first element of an internal static array to use with Next() and Write().
		/// </summary>
		public static void First() { _counter=0; _a[0]=Stopwatch.GetTimestamp(); }
		/// <summary>
		/// Calls SpinCPU(100) and First().
		/// </summary>
		public static void First(bool wakeCpu) { SpinCPU(); First(); }
		/// <summary>
		/// Calls SpinCPU(wakeCpuMS) and First().
		/// </summary>
		public static void First(int wakeCpuMS) { SpinCPU(wakeCpuMS); First(); }

		/// <summary>
		/// Stores current time in the next element of an internal static array to use with next Next() and Write().
		/// Don't call Next() more than 10 times after First(), because the array has 11 elements.
		/// </summary>
		public static void Next() { if(_counter<10) _a[++_counter]=Stopwatch.GetTimestamp(); }

		/// <summary>
		/// Formats a string from time values collected by calling First() and Next(), and shows it in the output or console.
		/// The string contains the number of microseconds of each code execution between calling First() and each Next().
		/// Example: <c>Time.First(true); CODE1; Time.Next(); CODE2; Time.Next(); Time.Write(); //speed: timeOfCODE1 timeOfCODE2</c>
		/// </summary>
		public static void Write()
		{
			uint i, n = _counter;

			StringBuilder s = new StringBuilder("speed:");
			for(i=0; i<n; i++) {
				s.Append("  ");
				s.Append((int)(_freq * (_a[i+1]-_a[i]) - 0.5));
			}

			Output.Write(s.ToString());
		}

		/// <summary>
		/// Calls Next() and Write().
		/// You can use <c>Time.NextWrite();</c> instead of <c>Time.Next(); Time.Write();</c>
		/// </summary>
		public static void NextWrite() { Next(); Write(); }

		/// <summary>
		/// Gets the number of microseconds elapsed since Windows startup.
		/// </summary>
		public static long Microseconds { get { return (long)(Stopwatch.GetTimestamp()*_freq); } }

		/// <summary>
		/// Gets the number of milliseconds elapsed since Windows startup.
		/// Unlike Environment.TickCount, this function returns a 64-bit value. Calls Windows API GetTickCount64, not GetTickCount.
		/// The resolution is 10 to 16 milliseconds.
		/// </summary>
		public static long Milliseconds { get { return Api.GetTickCount64(); } }

		/// <summary>
		/// Spins CPU timeMS milliseconds, default 100.
		/// Call before measuring code speed, because after some idle time CPU needs to work some time to gain full speed.
		/// </summary>
		public static void SpinCPU(int timeMS = 100)
		{
			int n = 0;
			for(long t0 = Microseconds; Microseconds-t0 < timeMS*1000; n++) {
				First(); Next(); //JIT-compile
			}
			//Out(n);
		}

//#if UNUSED
		/// <summary>
		/// Code speed measurement.
		/// Similar to Time.First() etc, but the array is in the instance object, so you can use multiple independent measurements.
		/// </summary>
		public unsafe struct PerformanceCounter
		{
			fixed long _a[11];
			uint _counter;

			public void First() { _counter=0; unsafe { fixed (long* p = _a) { p[0]=Stopwatch.GetTimestamp(); } } }

			public void Next() { if(_counter<10) unsafe { fixed (long* p = _a) { p[++_counter]=Stopwatch.GetTimestamp(); } } }

			public void Write()
			{
				uint i, n = _counter;
				double _freq = 1000000.0/Stopwatch.Frequency;
				StringBuilder s = new StringBuilder("speed:");

				unsafe
				{
					fixed (long* p = _a)
					{
						for(i=0; i<n; i++) {
							s.Append("  ");
							s.Append((int)(_freq * (p[i+1]-p[i]) - 0.5));
						}
					}
				}

				Output.Write(s.ToString());
			}

			public void NextWrite() { Next(); Write(); }

			public void AddTicksFirst(long ticks) { _counter=0; unsafe { fixed (long* p = _a) { p[0]=ticks; } } }

			public void AddTicksNext(long ticks) { if(_counter<10) unsafe { fixed (long* p = _a) { p[++_counter]=ticks; } } }
		}
		//#endif
		#endregion

		public static void Wait(double timeS)
		{
			int ms;
			if(timeS<0) ms=Timeout.Infinite;
			else {
				timeS*=1000.0;
				if(timeS<=int.MaxValue) ms=(int)timeS;
				else throw new ArgumentOutOfRangeException();
			}
			WaitMS(ms);
		}

		public static void WaitMS(int timeMS)
		{
			if(timeMS<0) timeMS=Timeout.Infinite;
			Thread.Sleep(timeMS);
			//TODO
		}
	}
}
