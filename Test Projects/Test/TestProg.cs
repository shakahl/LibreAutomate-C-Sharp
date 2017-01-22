using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Linq;
//using System.Configuration;
using System.Xml;

using Catkeys;
using static Catkeys.NoClass;

public class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		TaskDialog.Show("text", icon: TDIcon.App);

		//PrintList(Application.ExecutablePath, Folders.ThisApp, Folders.ThisProcess);


		//Console.WriteLine(1);
		////for(int i=0; i<3; i++) _Compile();
		//_Compile();
		////_CompileOld();
		//      Console.WriteLine(2);

		//don't close console immediately
		//Console.ReadKey();
	}

	static MethodInfo _compilerMethod;

	static int _Compile()
	{
		Perf.First(100);

		if(_compilerMethod == null) {
			Assembly a = Assembly.LoadFile(@"Q:\app\Catkeys\Test Projects\Test\csc.exe");

			_compilerMethod = a.EntryPoint;

			Perf.Next(); //8 ms
		}

		var dom = AppDomain.CurrentDomain;
		string csFile = @"Q:\Test\test.cs", dllFile = @"C:\Users\G\AppData\Local\Catkeys\ScriptDll\form.dll";

		string[] g = new string[] {
				"/nologo", "/noconfig",
				"/out:" + dllFile, "/target:winexe",
				//"/r:System.dll", "/r:System.Core.dll", "/r:System.Windows.Forms.dll",
				csFile
			};
		object[] p = new object[1] { g };

		//g[0]="/?";

		int r = (int)_compilerMethod.Invoke(0, p); //33 ms, first time ~330 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
		if(r != 0) Console.WriteLine(r);

		for(int i=0; i<4; i++) {
			Perf.Next();
			_compilerMethod.Invoke(0, p);
		}

		Perf.Next();
		Perf.Write();

		//GC.Collect(); //releases a lot
		return r;
	}

	static void _CompileOld()
	{
		//Out("test");
		//TODO: auto-ngen compiler. Need admin.

		Perf.First();
		//System.Runtime.ProfileOptimization.SetProfileRoot(@"C:\Test");
		//System.Runtime.ProfileOptimization.StartProfile("Startup.Profile"); //does not make jitting the C# compiler assemblies faster
		//Perf.Next();

		Assembly a = Assembly.LoadFile(@"Q:\app\Catkeys\Test Projects\Test\csc.exe");
		MethodInfo m = a.EntryPoint;
		string[] g = new string[] { null, "/nologo", "/noconfig", @"Q:\Test\test.cs" };
		object[] p = new object[1] { g };

		//g[0]="/?";
		Perf.Next(); //16 ms
		for(int i = 1; i <= 4; i++) {
			g[0] = $@"/out:C:\Test\test{i}.exe";
			m.Invoke(0, p); //works, 22 ms, first time ~300 ms on Win10/.NET4.6 and ~600 on older Win/.NET.
			Perf.Next();
			//GC.Collect(); //4 ms, makes working set smaller 48 -> 33 MB
			//Perf.Next();
		}
		Perf.Write();

		GC.Collect(); //releases a lot
	}
}

[DebuggerStepThrough]
public static class Perf
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
	/// Example: <c>Perf.First(100); CODE1; Perf.Next(); CODE2; Perf.Next(); Perf.Write(); //speed: timeOfCODE1 timeOfCODE2</c>
	/// </summary>
	public static void Write()
	{
		uint i, n = _counter;

		StringBuilder s = new StringBuilder("speed:");
		for(i = 0; i < n; i++) {
			s.Append("  ");
			s.Append((int)(_freq * (_a[i + 1] - _a[i]) - 0.5));
		}

		Console.WriteLine(s.ToString());
	}

	/// <summary>
	/// Calls Next() and Write().
	/// You can use <c>Perf.NextWrite();</c> instead of <c>Perf.Next(); Perf.Write();</c>
	/// </summary>
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
		for(long t0 = Environment.TickCount; Environment.TickCount - t0 < timeMS; n++) {
			First(); Next(); //JIT-compile
			for(int i = 0; i < codes.Length; i++) codes[i]();
		}
		//Out(n);
		First();
	}
}
