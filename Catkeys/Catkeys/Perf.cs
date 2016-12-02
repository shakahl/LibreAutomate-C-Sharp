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
using Util = Catkeys.Util;
using Catkeys.Winapi;

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
				if(!Util.Misc.IsCatkeysNgened) {
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
					//speed: 5 ms. The slowest part is creating shared memory. Also JITing and calling IsCatkeysNgened takes time.
					//With PrepareMethod also 5 ms. It also creates shared memory.
				}
				_canWrite = true;
			}

			static bool _canWrite;

			public Inst(bool callFirst) : this()
			{
				if(callFirst) First();
			}

			volatile int _counter;
			bool _incremental;
			int _nMeasurements; //used with incremental to display n measurements and average times
			long _time0;
			const int nElem = 10;
			fixed long _a[nElem];

			/// <summary>
			/// If true, times of each new First/Next/Next... measurement are added to previous measurement times.
			/// Finally you can call Write() or Times to get the sums.
			/// Usually used to measure code in loops. See example.
			/// </summary>
			/// <example>
			/// var perf = new Perf.Inst();
			/// perf.Incremental = true;
			/// for(int i = 0; i ˂ 5; i++) {
			/// 	WaitMS(100); //not included in the measurement
			/// 	perf.First();
			/// 	WaitMS(30); //will make sum ~150000
			/// 	perf.Next();
			/// 	WaitMS(10); //will make sum ~50000
			/// 	perf.Next();
			/// 	WaitMS(100); //not included in the measurement
			/// }
			/// perf.Write(); //speed:  154317  51060  (205377)
			/// perf.Incremental = false;
			/// </example>
			public bool Incremental
			{
				get { return _incremental; }
				set
				{
					if(_incremental = value) {
						fixed (long* p = _a) { for(int i = 0; i < nElem; i++) p[i] = 0; }
						_nMeasurements = 0;
					}
				}
			}

			/// <summary>
			/// Stores current time in the first element of an internal array to use with Next() and Write().
			/// </summary>
			public void First()
			{
				if(!_canWrite) return; //called by ctor. This prevents overwriting Inst in shared memory if it was used in another domain or process.
				_time0 = Stopwatch.GetTimestamp();
				//QueryPerformanceCounter(out _time0);
				_counter = 0;
				_nMeasurements++;
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
				int n = _counter; if(n >= nElem) return;
				_counter++;
				fixed (long* p = _a) { var t = Stopwatch.GetTimestamp() - _time0; if(_incremental) p[n] += t; else p[n] = t; }
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
					int i, n = _counter;
					if(n == 0) return null;
					if(n > nElem) n = nElem;
					double freq = 1000000.0 / Stopwatch.Frequency;
					//long _f; QueryPerformanceFrequency(out _f); double freq = 1000000.0 / _f;
					StringBuilder s = new StringBuilder("speed:");
					bool average = false; int nMeasurements = 1;

					fixed (long* p = _a)
					{
						g1:
						double t = 0.0, tPrev = 0.0;
						for(i = 0; i < n; i++) {
							s.Append("  ");
							t = freq * p[i];
							double d = t - tPrev; //could add 0.5 to round up, but assume that Stopwatch.GetTimestamp() call time is up to 0.5.
							if(average) d /= nMeasurements;
							s.Append((long)d);
							tPrev = t;
						}

						if(n > 1) {
							s.Append("  (");
							if(average) t /= nMeasurements;
							s.Append((long)t);
							s.Append(")");
						}

						if(!average && _incremental && (nMeasurements = _nMeasurements) > 1) {
							average = true;
                            s.Append(";  measured "); s.Append(nMeasurements); s.Append(" times, average");
							goto g1;
						}
					}
					return s.ToString();
				}
			}

			static void _FormatTimes(StringBuilder s, long* p, int n, int nMeasurements, double freq)
			{
				double t = 0.0, tPrev = 0.0;
				for(int i = 0; i < n; i++) {
					s.Append("  ");
					t = freq * p[i];
					s.Append((long)(t - tPrev)); //could add 0.5 to round up, but assume that Stopwatch.GetTimestamp() call time is up to 0.5.
					tPrev = t;
				}

				if(n > 1) {
					s.Append("  (");
					s.Append((long)t);
					s.Append(")");
				}
			}

			/// <summary>
			/// Gets the number of microseconds from First() to the last Next().
			/// </summary>
			public long TimeTotal
			{
				get
				{
					int n = _counter;
					if(n == 0) return 0;
					if(n > nElem) n = nElem;
					double freq = 1000000.0 / Stopwatch.Frequency;
					fixed (long* p = _a) { return (long)(freq * p[n - 1]); }
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
		/// If true, times of each new First/Next/Next... measurement are added to previous measurement times.
		/// Finally you can call Write() or Times to get the sums.
		/// Usually used to measure code in loops. See example.
		/// </summary>
		/// <example>
		/// Perf.Incremental = true;
		/// for(int i = 0; i ˂ 5; i++) {
		/// 	WaitMS(100); //not included in the measurement
		/// 	Perf.First();
		/// 	WaitMS(30); //will make sum ~150000
		/// 	Perf.Next();
		/// 	WaitMS(10); //will make sum ~50000
		/// 	Perf.Next();
		/// 	WaitMS(100); //not included in the measurement
		/// }
		/// Perf.Write(); //speed:  154317  51060  (205377)
		/// Perf.Incremental = false;
		/// </example>
		public static bool Incremental
		{
			get { return _SM->Incremental; }
			set { _SM->Incremental = value; }
		}

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
