using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

//using System.Reflection;
//using System.Linq;

using Catkeys;
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
	/// Stores data in shared memory, therefore works across appdomains.
	/// </summary>
	[DebuggerStepThrough]
	public static unsafe class Perf
	{
		/// <summary>
		/// The same as Perf class, but allows to have multiple independent speed measurements.
		/// Stores data int the variable, not in shared memory like Perf class.
		/// </summary>
		public unsafe struct Inst
		{
			//[DllImport("kernel32.dll")]
			//static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

			//[DllImport("kernel32.dll")]
			//static extern bool QueryPerformanceFrequency(out long lpFrequency);

			uint _counter;
			fixed long _a[11];

			/// <summary>
			/// Stores current time in the first element of an internal array to use with Next() and Write().
			/// </summary>
			public void First()
			{
				_counter = 0;
				fixed (long* p = _a) { p[0] = Stopwatch.GetTimestamp(); }
				//fixed (long* p = _a) { QueryPerformanceCounter(out p[0]); }
			}
			/// <summary>
			/// Calls SpinCPU(spinCpuMS) and First().
			/// </summary>
			public void First(int spinCpuMS) { SpinCPU(spinCpuMS); First(); }
			/// <summary>
			/// Calls SpinCPU(spinCpuMS, codes) and First().
			/// </summary>
			public void First(int spinCpuMS, params Action[] codes) { SpinCPU(spinCpuMS, codes); First(); }

			/// <summary>
			/// Stores current time in a next element of an internal array to use with next Next() and Write().
			/// Don't call Next() more than 10 times after First(), because the array has fixed size.
			/// </summary>
			public void Next()
			{
				if(_counter < 10) fixed (long* p = _a) { p[++_counter] = Stopwatch.GetTimestamp(); }
				//if(_counter < 10) fixed (long* p = _a) { QueryPerformanceCounter(out p[++_counter]); }
			}

			/// <summary>
			/// Formats a string from time values collected by calling First() and Next(), and shows it in the output.
			/// </summary>
			public void Write()
			{
				uint i, n = _counter;
				double freq = 1000000.0 / Stopwatch.Frequency;
				//long _f; QueryPerformanceFrequency(out _f); double freq = 1000000.0 / _f;
				StringBuilder s = new StringBuilder("speed:");

				fixed (long* p = _a)
				{
					for(i = 0; i < n; i++) {
						s.Append("  ");
						s.Append((int)(freq * (p[i + 1] - p[i]) - 0.5));
					}
				}

				Output.Write(s.ToString());
			}

			/// <summary>
			/// Calls Next() and Write().
			/// </summary>
			public void NextWrite() { Next(); Write(); }

			/// <summary>
			/// Executes code (lambda) nTimes times, and then calls Next().
			/// </summary>
			public void Execute(int nTimes, Action code)
			{
				while(nTimes-- > 0) code();
				Next();
			}

			/// <summary>
			/// nTimesAll times executes this code: <c>First(); foreach(Action a in codes) Execute(nTimesEach, a); Write();</c>
			/// </summary>
			public void ExecuteMulti(int nTimesAll, int nTimesEach, params Action[] codes)
			{
				while(nTimesAll-- > 0) {
					First();
					foreach(Action a in codes) Execute(nTimesEach, a);
					Write();
				}
			}

			/// <summary>
			/// Repeatedly executes codes (zero or more lambda functions) timeMS milliseconds.
			/// </summary>
			public void SpinCPU(int timeMS, params Action[] codes)
			{
				int n = 0;
				for(long t0 = Time.Microseconds; Time.Microseconds - t0 < timeMS * 1000; n++) {
					First(); Next(); //JIT-compile
					for(int i = 0; i < codes.Length; i++) codes[i]();
				}
				//Out(n);
				First();
			}
		}

		static Inst* _SM { get { return &Util.LibSharedMemory.Ptr->perf; } }

		/// <summary>
		/// Stores current time in the first element of an internal static array to use with Next() and Write().
		/// </summary>
		public static void First() { _SM->First(); }
		/// <summary>
		/// Calls SpinCPU(spinCpuMS) and First().
		/// </summary>
		public static void First(int spinCpuMS) { _SM->First(spinCpuMS); }
		/// <summary>
		/// Calls SpinCPU(spinCpuMS, codes) and First().
		/// </summary>
		public static void First(int spinCpuMS, params Action[] codes) { _SM->First(spinCpuMS, codes); }

		/// <summary>
		/// Stores current time in a next element of an internal static array to use with next Next() and Write().
		/// Don't call Next() more than 10 times after First(), because the array has fixed size.
		/// </summary>
		public static void Next() { _SM->Next(); }

		/// <summary>
		/// Formats a string from time values collected by calling First() and Next(), and shows it in the output or console.
		/// The string contains the number of microseconds of each code execution between calling First() and each Next().
		/// Example: <c>Perf.First(100); CODE1; Perf.Next(); CODE2; Perf.Next(); Perf.Write(); //speed: timeOfCODE1 timeOfCODE2</c>
		/// </summary>
		public static void Write() { _SM->Write(); }

		/// <summary>
		/// Calls Next() and Write().
		/// You can use <c>Perf.NextWrite();</c> instead of <c>Perf.Next(); Perf.Write();</c>
		/// </summary>
		public static void NextWrite() { _SM->NextWrite(); }

		/// <summary>
		/// Executes code (lambda) nTimes times, and then calls Next().
		/// </summary>
		public static void Execute(int nTimes, Action code) { _SM->Execute(nTimes, code); }

		/// <summary>
		/// nTimesAll times executes this code: <c>First(); foreach(Action a in codes) Execute(nTimesEach, a); Write();</c>
		/// </summary>
		public static void ExecuteMulti(int nTimesAll, int nTimesEach, params Action[] codes) { _SM->ExecuteMulti(nTimesAll, nTimesEach, codes); }

		/// <summary>
		/// Repeatedly executes codes (zero or more lambda functions) timeMS milliseconds (recommended 100 or more).
		/// Call before measuring code speed, because after some idle time CPU needs to work some time to gain full speed.
		/// Also it JIT-compiles First(), Next() and codes.
		/// </summary>
		public static void SpinCPU(int timeMS, params Action[] codes) { _SM->SpinCPU(timeMS, codes); }
	}
}
