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
using System.IO.Pipes;

using Au;
using Au.Types;
using static Au.NoClass;

[module: DefaultCharSet(CharSet.Unicode)]
//[System.Security.SuppressUnmanagedCodeSecurity]

//PROBLEM: slow startup.
//When Au.dll not ngened, a minimal script starts in 98 ms. Else in 150 ms.
//One of reasons is: when Au.dll ngened, together with it is always loaded System.dll and System.Core.dll, even if not used.
//	Don't know why. Didn't find a way to avoid it. Loading Au.dll with Assembly.LoadFrom does not help (loads ngened anyway).
//	Also then no AppDomain.AssemblyLoad event for these two .NET assemblies.
//	Luckily other assemblies used by Au.dll are not loaded when not used.
//	The same is for our-compiled .exe files, ie when used meta outputPath.
//Workaround:
//	Preload this process. Let it wait for next task.
//	Then a script can start in 4-6 ms when Au.dll ngened, else 6-8 ms. The 2-ms diff depends on antivirus (Avast); faster when this process is admin.
//	We don't preload first time or if used meta outputPath. Also not faster if several scripts are started without a delay. Never mind.

//Smaller problem: many threads.
//Initially 4 treads, sometimes 7. After 20-30 s becomes 5 (+1 or -2). With [STAThread] would be +2.

//PROBLEM: preloaded task's windows start inactive, behind one or more windows. Unless they activate self, like ADialog.
//	It does not depend on the foreground lock setting/API. The setting/API just enable SetForegroundWindow, but most windows don't call it.
//Workaround: use CBT hook. It receives HCBT_ACTIVATE even when the window does not become the foreground window.
//	On HCBT_ACTIVATE, async-call SetForegroundWindow. Also, editor calls AllowSetForegroundWindow before starting task.

static unsafe class Program
{
	//[STAThread] //we use TrySetApartmentState instead
	static void Main(string[] args)
	{
		string asmFile; int pdbOffset, flags;

		if(args.Length != 1) return;
		string pipeName = args[0]; //if(!pipeName.Starts(@"\\.\pipe\Au.Task-")) return;

		int nr = 0;
#if false
			//With NamedPipeClientStream faster by 1 ms, because don't need to JIT. But problems:
			//1. Loads System and System.Core immediately, making slower startup.
			//2. This process does not end when editor process ended, because then Connect spin-waits for server created.
			using(var pipe = new NamedPipeClientStream(".", pipeName.Substring(9), PipeDirection.In)) {
				pipe.Connect();
				var b = new byte[10000];
				nr=pipe.Read(b, 0, b.Length);
			}
#else
		for(int i = 0; i < 3; i++) {
			if(Api.WaitNamedPipe(pipeName, i == 2 ? -1 : 100)) break;
			if(Marshal.GetLastWin32Error() != Api.ERROR_SEM_TIMEOUT) return;
			//Perf.First();
			switch(i) {
			case 0: _Prepare1(); break; //~70 ms with cold CPU, 35 with hot
			case 1: _Prepare2(); break; //~65 ms with cold CPU, 30 with hot
			}
			//Perf.NW();
		}
		//Perf.First();
		using(var pipe = Api.CreateFile(pipeName, Api.GENERIC_READ, 0, default, Api.OPEN_EXISTING, 0)) {
			if(pipe.Is0) { ADebug.LibPrintNativeError(); return; }
			//Perf.Next();
			int size; if(!Api.ReadFile(pipe, &size, 4, out nr, default) || nr != 4) return;
			//Perf.Next();
			if(!Api.ReadFileArr(pipe, out var b, size, out nr) || nr != size) return;
			//Perf.Next();
			var a = Au.Util.LibSerializer.Deserialize(b);
			ATask.Name = a[0]; asmFile = a[1]; pdbOffset = a[2]; flags = a[3]; args = a[4];
			string wrp = a[5]; if(wrp != null) Environment.SetEnvironmentVariable("ATask.WriteResult.pipe", wrp);
		}
#endif
		//Perf.Next();

		bool mtaThread = 0 != (flags & 2); //app without [STAThread]
		if(mtaThread == s_isSTA) _SetComApartment(mtaThread ? ApartmentState.MTA : ApartmentState.STA);

		if(0 != (flags & 4)) Api.AllocConsole(); //meta console true

		if(0 != (flags & 1)) { //hasConfig
			var config = asmFile + ".config";
			if(AFile.ExistsAsFile(config, true)) AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", config);
		}

		if(s_hook == null) _Hook();

		//Perf.Next();
		try { RunAssembly.Run(asmFile, args, pdbOffset); }
		catch(Exception ex) when(!(ex is ThreadAbortException)) { Print(ex); }
		finally { s_hook?.Dispose(); }
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _Prepare1()
	{
		_SetComApartment(ApartmentState.STA);

		//JIT slowest-to-JIT methods
		if(!Au.Util.AAssembly.LibIsAuNgened) {
			Au.Util.Jit.Compile(typeof(RunAssembly), nameof(RunAssembly.Run));
			Au.Util.Jit.Compile(typeof(Au.Util.LibSerializer), "Deserialize");
			AFile.WaitIfLocked(() => (FileStream)null);
		}

		//load some .NET assemblies, it is very slow.
		//	Initially we have only mscorlib and Au.
		//	note: actually loads these assemblies when JIT-compiling this method.
		//	note: .NET serializes loading of assemblies. If one thread is loading, other thread waits. Not tested with processes.
		_ = typeof(System.Linq.Enumerable).Assembly; //System.Core, System

		_Hook();
	}

	//CONSIDER: reject. The speed gain isn't noticeable.
	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _Prepare2()
	{
		_ = typeof(System.Windows.Forms.Control).Assembly; //System.Windows.Forms, System.Drawing

		//SetProcessWorkingSetSize(Api.GetCurrentProcess(), -1, -1); //makes starting slower, eg non-ngened 7.5 -> 10.5 ms
	}
	//[DllImport("kernel32.dll")]
	//internal static extern bool SetProcessWorkingSetSize(IntPtr hProcess, LPARAM dwMinimumWorkingSetSize, LPARAM dwMaximumWorkingSetSize);

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _SetComApartment(ApartmentState state)
	{
		Thread.CurrentThread.TrySetApartmentState(ApartmentState.Unknown);
		Thread.CurrentThread.TrySetApartmentState(state);
		s_isSTA = state == ApartmentState.STA;

		//This is undocumented, but works if we set ApartmentState.Unknown at first.
		//With [STAThread] the process initially has +2 threads. Also now slightly faster.
	}
	static bool s_isSTA;

	static void _Hook()
	{
		s_hook = WinHook.ThreadCbt(m => {
			//Print(m.code, m.wParam, m.lParam);
			//switch(m.code) {
			//case HookData.CbtEvent.ACTIVATE:
			//case HookData.CbtEvent.SETFOCUS:
			//	Print((Wnd)m.wParam);
			//	Print(Wnd.Active);
			//	Print(Wnd.ThisThread.Active);
			//	Print(Wnd.Focused);
			//	Print(Wnd.ThisThread.Focused);
			//	break;
			//}
			if(m.code == HookData.CbtEvent.ACTIVATE) {
				var w = (Wnd)m.wParam;
				if(!w.HasExStyle(WS_EX.NOACTIVATE)) {
					//Print(w);
					//Print(w.ExStyle);
					//Api.SetForegroundWindow(w); //does not work
					ATimer.After(1, () => {
						if(s_hook == null) return;
						//Print(Wnd.Active);
						//Print(Wnd.ThisThread.Active);
						bool isActive = w == Wnd.Active, activate = !isActive && w == Wnd.ThisThread.Active;
						if(isActive || activate) { s_hook.Dispose(); s_hook = null; }
						if(activate) {
							Api.SetForegroundWindow(w);
							//w.ActivateLL(); //no, it's against Windows rules, and works differently with meta outputPath
							//Before starting task, editor calls AllowSetForegroundWindow. But if clicked etc a window after that:
							//	SetForegroundWindow fails always or randomly;
							//	Activate[LL] fails if that window is of higher UAC IL, unless the foreground lock timeout is 0.
						}
					});
				}
			}
			return false;
		});
	}
	static WinHook s_hook;

}
