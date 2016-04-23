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
using System.Runtime.CompilerServices;
//using System.IO;
//using System.Windows.Forms;

using static Catkeys.NoClass;
using Util = Catkeys.Util;
using static Catkeys.Util.NoClass;
using Catkeys.Winapi;
using Auto = Catkeys.Automation;

namespace Catkeys
{
	/// <summary>
	/// Code speed measurement.
	/// Easier to use than System.Stopwatch class.
	/// </summary>
	[DebuggerStepThrough]
	public static class Speed
	{
		static long[] _a = new long[11];
		static uint _counter = 0;
		static double _freq = 1000000.0 / Stopwatch.Frequency;

		/// <summary>
		/// Stores current time in the first element of an internal static array to use with Next() and Write().
		/// </summary>
		public static void First() { _counter = 0; _a[0] = Stopwatch.GetTimestamp(); }
		/// <summary>
		/// Calls SpinCPU(100) and First().
		/// </summary>
		public static void First(bool wakeCpu) { SpinCPU(100); First(); }
		/// <summary>
		/// Calls SpinCPU(wakeCpuMS, codes) and First().
		/// </summary>
		public static void First(int wakeCpuMS, params Action[] codes) { SpinCPU(wakeCpuMS, codes); First(); }

		/// <summary>
		/// Stores current time in the next element of an internal static array to use with next Next() and Write().
		/// Don't call Next() more than 10 times after First(), because the array has 11 elements.
		/// </summary>
		public static void Next() { if(_counter < 10) _a[++_counter] = Stopwatch.GetTimestamp(); }

		/// <summary>
		/// Formats a string from time values collected by calling First() and Next(), and shows it in the output or console.
		/// The string contains the number of microseconds of each code execution between calling First() and each Next().
		/// Example: <c>Speed.First(true); CODE1; Speed.Next(); CODE2; Speed.Next(); Speed.Write(); //speed: timeOfCODE1 timeOfCODE2</c>
		/// </summary>
		public static void Write()
		{
			uint i, n = _counter;

			StringBuilder s = new StringBuilder("speed:");
			for(i = 0; i < n; i++) {
				s.Append("  ");
				s.Append((int)(_freq * (_a[i + 1] - _a[i]) - 0.5));
			}

			Output.Write(s.ToString());
		}

		/// <summary>
		/// Calls Next() and Write().
		/// You can use <c>Speed.NextWrite();</c> instead of <c>Speed.Next(); Speed.Write();</c>
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NextWrite() { Next(); Write(); }

		/// <summary>
		/// Executes code (lambda) nTimes times, and then calls Next().
		/// </summary>
		public static void Execute(int nTimes, Action code)
		{
			while(nTimes-- > 0) code();
			Next();
		}

		/// <summary>
		/// nTimesAll times executes this code: <c>First(); foreach(Action a in codes) Execute(nTimesEach, a); Write();</c>
		/// </summary>
		public static void ExecuteMulti(int nTimesAll, int nTimesEach, params Action[] codes)
		{
            while(nTimesAll-- > 0) {
				First();
				foreach(Action a in codes) Execute(nTimesEach, a);
				Write();
			}
		}

		/// <summary>
		/// Repeatedly executes codes (zero or more lambda functions) timeMS milliseconds (recommended 100 or more).
		/// Call before measuring code speed, because after some idle time CPU needs to work some time to gain full speed.
		/// Also it JIT-compiles First(), Next() and codes.
		/// </summary>
		public static void SpinCPU(int timeMS, params Action[] codes)
		{
			int n = 0;
			for(long t0 = Time.Microseconds; Time.Microseconds - t0 < timeMS * 1000; n++) {
				First(); Next(); //JIT-compile
				for(int i = 0; i < codes.Length; i++) codes[i]();
			}
			//Out(n);
			First();
		}

		//#if UNUSED
		/// <summary>
		/// Code speed measurement.
		/// Similar to Speed.First() etc, but the array is in the instance object, so you can use multiple independent measurements.
		/// </summary>
		public unsafe struct PerformanceCounter
		{
			fixed long _a[11];
			uint _counter;

			public void First() { _counter = 0; unsafe { fixed (long* p = _a) { p[0] = Stopwatch.GetTimestamp(); } } }

			public void Next() { if(_counter < 10) unsafe { fixed (long* p = _a) { p[++_counter] = Stopwatch.GetTimestamp(); } } }

			public void Write()
			{
				uint i, n = _counter;
				double _freq = 1000000.0 / Stopwatch.Frequency;
				StringBuilder s = new StringBuilder("speed:");

				unsafe
				{
					fixed (long* p = _a)
					{
						for(i = 0; i < n; i++) {
							s.Append("  ");
							s.Append((int)(_freq * (p[i + 1] - p[i]) - 0.5));
						}
					}
				}

				Output.Write(s.ToString());
			}

			public void AddTicksFirst(long ticks) { _counter = 0; unsafe { fixed (long* p = _a) { p[0] = ticks; } } }

			public void AddTicksNext(long ticks) { if(_counter < 10) unsafe { fixed (long* p = _a) { p[++_counter] = ticks; } } }
		}
		//#endif
	}
}
