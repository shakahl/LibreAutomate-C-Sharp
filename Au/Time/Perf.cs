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
//using System.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Code speed measurement. Easier to use than <see cref="Stopwatch"/>.
	/// </summary>
	/// <remarks>
	/// Stores data in shared memory, therefore the same measurement can be used in multiple appdomains and even processes. See also <see cref="Inst"/>.
	/// </remarks>
	[DebuggerStepThrough]
	public static unsafe class Perf
	{
		/// <summary>
		/// The same as <see cref="Perf"/> class, but allows to have multiple independent speed measurements.
		/// </summary>
		/// <remarks>
		/// Stores data in the variable, not in shared memory.
		/// </remarks>
		public unsafe struct Inst
		{
			//[DllImport("kernel32.dll")]
			//static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

			//[DllImport("kernel32.dll")]
			//static extern bool QueryPerformanceFrequency(out long lpFrequency);

			static Inst()
			{
				//Prevent JIT delay when calling Next etc if not ngened.
				if(!Util.Assembly_.LibIsAuNgened) {
					Stopwatch.GetTimestamp(); //maybe the .NET assembly not ngened
#if false
					//RuntimeHelpers.PrepareMethod(typeof(Perf).GetMethod("First", Array.Empty<Type>()).MethodHandle);
					//RuntimeHelpers.PrepareMethod(typeof(Perf.Inst).GetMethod("First", Array.Empty<Type>()).MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf).GetMethod("Next").MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf.Inst).GetMethod("Next").MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf).GetMethod("NW").MethodHandle);
					RuntimeHelpers.PrepareMethod(typeof(Perf.Inst).GetMethod("NW").MethodHandle);
					//This works, but the first time is still 2, or 1 if compiling First too.
#else
					Perf.Next(); Perf.NW(); Perf.First(); //JIT-compiles everything we need. _canWrite prevents calling Output.Write.
#endif
					//speed: 5 ms. The slowest part is creating shared memory. Also JITing and calling IsAuNgened takes time.
					//With PrepareMethod also 5 ms. It also creates shared memory.
				}
				_canWrite = true;
			}

			static bool _canWrite;

			//BE CAREFUL: this struct is in shared memory. Max allowed size is 256-32. Currently used 184.

			volatile int _counter;
			bool _incremental;
			int _nMeasurements; //used with incremental to display n measurements and average times
			long _time0;
			const int _nElem = 16;
			fixed long _a[_nElem];
			fixed char _aMark[_nElem];

			/// <summary><inheritdoc cref="Perf.Incremental"/></summary>
			/// <example>
			/// <code><![CDATA[
			/// var perf = new Perf.Inst();
			/// perf.Incremental = true;
			/// for(int i = 0; i < 5; i++) {
			/// 	Thread.Sleep(100); //not included in the measurement
			/// 	perf.First();
			/// 	Thread.Sleep(30); //will make sum ~150000
			/// 	perf.Next();
			/// 	Thread.Sleep(10); //will make sum ~50000
			/// 	perf.Next();
			/// 	Thread.Sleep(100); //not included in the measurement
			/// }
			/// perf.Write(); //speed:  154317  51060  (205377)
			/// perf.Incremental = false;
			/// ]]></code>
			/// </example>
			public bool Incremental
			{
				get => _incremental;
				set
				{
					if(_incremental = value) {
						fixed (long* p = _a) { for(int i = 0; i < _nElem; i++) p[i] = 0; }
						_nMeasurements = 0;
					}
				}
			}

			/// <inheritdoc cref="Perf.First()"/>
			public void First()
			{
				if(!_canWrite) return; //called by ctor. This prevents overwriting Inst in shared memory if it was used in another domain or process.
				_time0 = Stopwatch.GetTimestamp();
				//QueryPerformanceCounter(out _time0);
				_counter = 0;
				_nMeasurements++;
			}

			/// <summary>
			/// Calls <see cref="WakeCPU"/> and <see cref="First()"/>.
			/// </summary>
			public void First(int timeSpeedUpCPU)
			{
				WakeCPU(timeSpeedUpCPU);
				First();
			}

			/// <inheritdoc cref="Perf.Next"/>
			public void Next(char cMark = '\0')
			{
				if(!_canWrite) return; //called by ctor. This prevents overwriting Inst in shared memory if it was used in another domain or process.
				int n = _counter; if(n >= _nElem) return;
				_counter++;
				fixed (long* p = _a) {
					var t = Stopwatch.GetTimestamp() - _time0;
					if(_incremental) p[n] += t; else p[n] = t;
					fixed (char* c = _aMark) c[n] = cMark;
				}
			}

			/// <summary>
			/// Formats a string from time values collected by calling <see cref="First"/> and <see cref="Next"/>, and shows it in the output.
			/// The string contains the number of microseconds of each code execution between calling <b>First</b> and each <b>Next</b>.
			/// </summary>
			public void Write()
			{
				if(!_canWrite) return;
				var s = ToString();
				if(s != null) Output.Write(s);
			}

			/// <summary>
			/// Calls <see cref="Next"/> and <see cref="Write"/>.
			/// </summary>
			/// <param name="cMark">A character to mark that time in the results string, like "A=150".</param>
			public void NW(char cMark = '\0') { Next(cMark); Write(); }

			/// <summary>
			/// Formats a string from time values collected by calling <see cref="First"/> and <see cref="Next"/>.
			/// The string contains the number of microseconds of each code execution between calling <b>First</b> and each <b>Next</b>.
			/// </summary>
			public override string ToString()
			{
				using(new Util.LibStringBuilder(out var b)) {
					b.Append("speed:");
					_Results(Math.Min(_counter, _nElem), b, null);
					return b.ToString();
				}
			}

			/// <summary>
			/// Return array of time values collected by calling <see cref="First"/> and <see cref="Next"/>.
			/// Each element is the number of microseconds of each code execution between calling <b>First</b> and each <b>Next</b>.
			/// </summary>
			public long[] ToArray()
			{
				int n = Math.Min(_counter, _nElem);
				var a = new long[n];
				_Results(n, null, a);
				return a;
			}

			void _Results(int n, StringBuilder b, long[] a)
			{
				if(n == 0) return;
				double freq = 1000000.0 / Stopwatch.Frequency;
				//long _f; QueryPerformanceFrequency(out _f); double freq = 1000000.0 / _f;
				bool average = false; int nMeasurements = 1;

				fixed (long* p = _a) fixed (char* c = _aMark) {
					g1:
					double t = 0d, tPrev = 0d;
					for(int i = 0; i < n; i++) {
						t = freq * p[i];
						double d = t - tPrev; tPrev = t; //could add 0.5 to round up, but assume that Stopwatch.GetTimestamp() call time is 0 - 0.5.
						if(average) d /= nMeasurements;
						long dLong = (long)d;
						if(b != null) {
							b.Append("  ");
							if(c[i] != '\0') b.Append(c[i]).Append('=');
							b.Append(dLong.ToString());
						} else {
							a[i] = dLong;
						}
					}
					if(b == null) return;

					if(n > 1) {
						if(average) t /= nMeasurements;
						b.Append("  (").Append((long)t).Append(")");
					}

					if(!average && _incremental && (nMeasurements = _nMeasurements) > 1) {
						average = true;
						b.Append(";  measured ").Append(nMeasurements).Append(" times, average");
						goto g1;
					}
				}
			}

			/// <summary>
			/// Gets the number of microseconds between <see cref="First"/> and the last <see cref="Next"/>.
			/// </summary>
			public long TimeTotal
			{
				get
				{
					int n = _counter;
					if(n == 0) return 0;
					if(n > _nElem) n = _nElem;
					double freq = 1000000.0 / Stopwatch.Frequency;
					fixed (long* p = _a) { return (long)(freq * p[n - 1]); }
				}
			}

			/// <summary>
			/// Executes <paramref name="code"/> (lambda) <paramref name="count"/> times, and then calls <see cref="Next"/>.
			/// </summary>
			public void Execute(int count, Action code)
			{
				while(count-- > 0) code();
				Next();
			}

			/// <summary>
			/// <paramref name="countAll"/> times executes this code: <c>First(); foreach(Action a in codes) Execute(countEach, a); Write();</c>.
			/// </summary>
			public void ExecuteMulti(int countAll, int countEach, params Action[] codes)
			{
				while(countAll-- > 0) {
					First();
					foreach(Action a in codes) Execute(countEach, a);
					Write();
				}
			}
		}

		static Inst* _SM => &Util.LibSharedMemory.Ptr->perf;

		/// <summary>
		/// Creates and returns new <see cref="Inst"/> variable and calls its <see cref="Inst.First"/>.
		/// </summary>
		public static Inst StartNew()
		{
			var R = new Inst();
			R.First();
			return R;
		}

		/// <summary>
		/// If true, times of each new First/Next/Next... measurement are added to previous measurement times.
		/// Finally you can call <see cref="Write"/> or <see cref="ToString"/> to get the sums.
		/// Usually used to measure code in loops. See example.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Perf.Incremental = true;
		/// for(int i = 0; i < 5; i++) {
		/// 	Thread.Sleep(100); //not included in the measurement
		/// 	Perf.First();
		/// 	Thread.Sleep(30); //will make sum ~150000
		/// 	Perf.Next();
		/// 	Thread.Sleep(10); //will make sum ~50000
		/// 	Perf.Next();
		/// 	Thread.Sleep(100); //not included in the measurement
		/// }
		/// Perf.Write(); //speed:  154317  51060  (205377)
		/// Perf.Incremental = false;
		/// ]]></code>
		/// </example>
		public static bool Incremental
		{
			get => _SM->Incremental;
			set { _SM->Incremental = value; }
		}
		//CONSIDER: this is confusing: this process starts with Incremental = true if previous process used it and the shared memory survived

		/// <summary>
		/// Stores current time in the first element of an internal array.
		/// </summary>
		public static void First() { _SM->First(); }

		/// <summary>
		/// Calls <see cref="WakeCPU"/> and <see cref="First()"/>.
		/// </summary>
		public static void First(int timeSpeedUpCPU) { _SM->First(timeSpeedUpCPU); }

		/// <summary>
		/// Stores current time in next element of an internal array.
		/// </summary>
		/// <remarks>
		/// Don't call <b>Next</b> more than 16 times after <b>First</b>, because the array has fixed size.
		/// </remarks>
		/// <param name="cMark">A character to mark this time in the results string, like "A=150".</param>
		public static void Next(char cMark = '\0') { _SM->Next(cMark); }

		/// <summary>
		/// Formats a string from time values collected by calling <see cref="First"/> and <see cref="Next"/>, and shows it in the output.
		/// The string contains the number of microseconds of each code execution between calling <b>First</b> and each <b>Next</b>.
		/// </summary>
		/// <example>
		/// <code><![CDATA[
		/// Perf.First(100);
		/// CODE1;
		/// Perf.Next();
		/// CODE2;
		/// Perf.Next();
		/// Perf.Write(); //speed:  timeOfCODE1  timeOfCODE2  (totalTime)
		/// ]]></code>
		/// </example>
		public static void Write() { _SM->Write(); }

		/// <summary>
		/// Calls <see cref="Next"/> and <see cref="Write"/>.
		/// </summary>
		/// <param name="cMark">A character to mark that time in the results string, like "A=150".</param>
		public static void NW(char cMark = '\0') { _SM->NW(cMark); }

		/// <summary>
		/// Formats a string from time values collected by calling <see cref="First"/> and <see cref="Next"/>.
		/// The string contains the number of microseconds of each code execution between calling <b>First</b> and each <b>Next</b>.
		/// </summary>
		public static new string ToString() => _SM->ToString();

		/// <summary>
		/// Return array of time values collected by calling <see cref="First"/> and <see cref="Next"/>.
		/// Each element is the number of microseconds of each code execution between calling <b>First</b> and each <b>Next</b>.
		/// </summary>
		public static long[] ToArray() => _SM->ToArray();

		/// <summary>
		/// Gets the number of microseconds between <see cref="First"/> and the last <see cref="Next"/>.
		/// </summary>
		public static long TimeTotal => _SM->TimeTotal;

		/// <summary>
		/// Executes <paramref name="code"/> (lambda) <paramref name="count"/> times, and then calls <see cref="Next"/>.
		/// </summary>
		public static void Execute(int count, Action code) { _SM->Execute(count, code); }

		/// <summary>
		/// <paramref name="countAll"/> times executes this code: <c>First(); foreach(Action a in codes) Execute(countEach, a); Write();</c>.
		/// </summary>
		public static void ExecuteMulti(int countAll, int countEach, params Action[] codes) { _SM->ExecuteMulti(countAll, countEach, codes); }

		/// <summary>
		/// Executes some code in loop for the specified amount of time. It should make CPU to run at full speed.
		/// </summary>
		/// <param name="timeMilliseconds">How long to speed up CPU, milliseconds. The minimal required time probably is about 100 ms, but depends on CPU.</param>
		/// <remarks>
		/// Code speed measurements often are misleading because of variable CPU speed. Most CPU don't run at full speed when not actively used.
		/// 
		/// You can make CPU speed constant in Control Panel -> Power Options -> ... Advanced -> Processor power management -> Minimum or maximum power state.
		/// There are programs that show current CPU speed. For example HWMonitor.
		/// </remarks>
		public static void WakeCPU(int timeMilliseconds = 200)
		{
			int n = 0;
			for(long t0 = Time.Microseconds; Time.Microseconds - t0 < timeMilliseconds * 1000L; n++) { }
			//Print(n);
		}
	}
}
