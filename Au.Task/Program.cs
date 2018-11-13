//#define PRELOAD_PROCESS //not impl

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

using Au;
using Au.Types;
using static Au.NoClass;
using Au.LibRun;

//PROBLEM: slow startup.
//When not ngened, a minimal script starts in 98 ms. Else in 150 ms.
//One of reasons is: when Au.dll ngened, together with it is always loaded System.dll and System.Core.dll, even if not used.
//	Don't know why. Didn't find a way to avoid it. Loading Au.dll with Assembly.LoadFrom does not help (loads ngened anyway).
//	Also then no AppDomain.AssemblyLoad event for these two .NET assemblies.
//	Luckily other assemblies used by Au.dll are not loaded when not used.
//	The same is for our-compiled .exe files, ie when used meta outputPath.

namespace Au.Task
{
	static class Program
	{
#if PRELOAD_PROCESS
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool ReadFile(IntPtr hFile, char[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr lpOverlapped);
#endif

		//[STAThread] //we use TrySetApartmentState instead
		static void Main(string[] args)
		{
#if PRELOAD_PROCESS
			if(args.Length < 1) return;
			IntPtr hPipe = (IntPtr)args[0].ToLong_();
			var a = new char[1000];
			if(!ReadFile(hPipe, a, a.Length, out int nr, default)) return;

			Print(a.ToString());
#else
			if(args.Length < 4) return;
			string name = args[0], asm = args[1];
			int pdbOffset = args[2].ToInt_();
			int flags = args[3].ToInt_();
			if(args.Length == 4) args = null; else args = args.RemoveAt_(0, 4);
#endif

			AuTask.Name = name;

			if(0 == (flags & 2)) { //!mtaThread (script has [STAThread])
				Thread.CurrentThread.TrySetApartmentState(ApartmentState.Unknown);
				Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA);
				//this is undocumented, but works if we set ApartmentState.Unknown at first.
				//With [STAThread] the process initially has 6 threads.
				//Without [STAThread] the process has 4 threads (5 after ~25 s), even if we set STA now. Also now slightly faster.
			}

			if(0 != (flags & 1)) { //hasConfig
				var config = asm + ".config";
				if(File_.ExistsAsFile(config, true)) AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", config);
			}

			try { RunAsm.Run(asm, args, pdbOffset); }
			catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }
		}
	}
}
