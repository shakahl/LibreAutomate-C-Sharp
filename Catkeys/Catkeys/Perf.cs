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
using System.Windows.Forms;
using System.Drawing;
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

			static Inst()
			{
				//Prevent JIT delay when calling Next etc if not ngened.
				if(!Util.Misc.IsCatkeysInNgened) {
					Stopwatch.GetTimestamp(); //maybe the .NET assembly not ngened
#if false
					//RuntimeHelpers.PrepareMethod(typeof(Perf).GetMethod("First", new Type[0]).MethodHandle);
					//RuntimeHelpers.PrepareMethod(typeof(Perf.Inst).GetMethod("First", new Type[0]).MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf).GetMethod("Next").MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf.Inst).GetMethod("Next").MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf).GetMethod("NW").MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf.Inst).GetMethod("NW").MethodHandle);
					//This works, but the first time is still 2, or 1 if compiling First too.
#else
					Perf.Next(); Perf.NW(); Perf.First(); //JIT-compiles everything we need. _canWrite prevents calling Output.Write.
#endif
					//speed: 5 ms. The slowest part is creating shared memory. Also JITing and calling IsCatkeysInNgened takes time.
					//With PrepareMethod also 5 ms. It also creates shared memory.
				}
				_canWrite = true;
			}

			static bool _canWrite;

			public Inst(bool callFirst)
			{
				_counter = 0;
				if(callFirst) First();
			}

			uint _counter;
			fixed long _a[11];

			/// <summary>
			/// Stores current time in the first element of an internal array to use with Next() and Write().
			/// </summary>
			public void First()
			{
				if(!_canWrite) return; //called by ctor. This prevents overwriting Inst in shared memory if it was used in another domain or process.
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
			/// Stores current time in next element of an internal array to use with next Next() and Write().
			/// Don't call Next() more than 10 times after First(), because the array has fixed size.
			/// </summary>
			public void Next()
			{
				if(!_canWrite) return; //called by ctor. This prevents overwriting Inst in shared memory if it was used in another domain or process.
				if(_counter < 10) fixed (long* p = _a) { p[++_counter] = Stopwatch.GetTimestamp(); }
				//if(_counter < 10) fixed (long* p = _a) { QueryPerformanceCounter(out p[++_counter]); }
			}

			/// <summary>
			/// Formats a string from time values collected by calling First() and Next(), and shows it in the output.
			/// The string contains the number of microseconds of each code execution between calling First() and each Next().
			/// </summary>
			public void Write()
			{
				if(!_canWrite) return;
				var s = Times;
				if(s != null) Output.Write(s);
			}

			/// <summary>
			/// Calls Next() and Write().
			/// </summary>
			public void NW() { Next(); Write(); }

			/// <summary>
			/// Formats a string from time values collected by calling First() and Next().
			/// The string contains the number of microseconds of each code execution between calling First() and each Next().
			/// </summary>
			public string Times
			{
				get
				{
					uint i, n = _counter;
					if(n == 0) return null;
					double freq = 1000000.0 / Stopwatch.Frequency;
					//long _f; QueryPerformanceFrequency(out _f); double freq = 1000000.0 / _f;
					StringBuilder s = new StringBuilder("speed:");

					fixed (long* p = _a)
					{
						for(i = 0; i < n; i++) {
							s.Append("  ");
							s.Append((long)(freq * (p[i + 1] - p[i]) - 0.45));
						}

						if(n > 1) {
							s.Append("  (");
							s.Append((long)(freq * (p[n] - p[0]) - 0.45));
							s.Append(")");
						}
					}
					return s.ToString();
				}
			}

			/// <summary>
			/// Gets the number of microseconds from First() to the last Next().
			/// </summary>
			public long TimeTotal
			{
				get
				{
					if(_counter == 0) return 0;
					double freq = 1000000.0 / Stopwatch.Frequency;
					fixed (long* p = _a) { return (long)(freq * (p[_counter] - p[0]) - 0.45); }
				}
			}

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
		/// Stores current time in next element of an internal static array to use with next Next() and Write().
		/// Don't call Next() more than 10 times after First(), because the array has fixed size.
		/// </summary>
		public static void Next() { _SM->Next(); }

		/// <summary>
		/// Formats a string from time values collected by calling First() and Next(), and shows it in the output.
		/// The string contains the number of microseconds of each code execution between calling First() and each Next().
		/// Example: <c>Perf.First(100); CODE1; Perf.Next(); CODE2; Perf.Next(); Perf.Write(); //speed: timeOfCODE1 timeOfCODE2</c>
		/// </summary>
		public static void Write() { _SM->Write(); }

		/// <summary>
		/// Calls Next() and Write().
		/// You can use <c>Perf.NW();</c> instead of <c>Perf.Next(); Perf.Write();</c>
		/// </summary>
		public static void NW() { _SM->NW(); }

		/// <summary>
		/// Formats a string from time values collected by calling First() and Next().
		/// The string contains the number of microseconds of each code execution between calling First() and each Next().
		/// </summary>
		public static string Times { get { return _SM->Times; } }

		/// <summary>
		/// Gets the number of microseconds from First() to the last Next().
		/// </summary>
		public static long TimeTotal { get { return _SM->TimeTotal; } }

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
