using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Reflection;

using Au;
using Au.Types;
using Au.Util;

[module: DefaultCharSet(CharSet.Unicode)]

//PROBLEM: slow startup.
//A minimal script starts in 90-100 ms.
//Workaround:
//	Preload this process. Let it wait for next task. While waiting, it also can JIT etc.
//	Then a script starts in 7 ms.
//	Except first time or if exe. Also not faster if several scripts are started without a delay. Never mind.

//Smaller problem: many threads.
//Initially 4 or 7 treads. After 20-30 s becomes 5 (+1 or -2). With [STAThread] would be +2.

//PROBLEM: preloaded task's windows start inactive, behind one or more windows. Unless they activate self, like ADialog.
//	It does not depend on the foreground lock setting/API. The setting/API just enable SetForegroundWindow, but most windows don't call it.
//Workaround: use CBT hook. It receives HCBT_ACTIVATE even when the window does not become the foreground window.
//	On HCBT_ACTIVATE, async-call SetForegroundWindow. Also, editor calls AllowSetForegroundWindow before starting task.

static unsafe class Program
{
	//[STAThread] //we use TrySetApartmentState instead
	static void Main(string[] args)
	{
		//To test task startup speed, use this script:
		//300.ms(); //give time to preload new task process
		//for (int i = 0; i < 4; i++) {
		//	APerf.Shared.First();
		//	ATask.Run(@"\blue script that contains APerf.Shared.NW();.cs");
		//	600.ms(); //give time for the process to exit
		//}
		//Results should be:
		//speed:  1=14453  4396  (18850)
		//speed:  1=1758  4533  (6292)
		//speed:  1=769  4659  (5428)
		//speed:  1=715  4253  (4968)
		//The first 14 ms is to JIT ATask.Run.

		if(args.Length != 1) return;

#if !DEBUG
		AProcess.CultureIsInvariant = true;
#endif

		string asmFile, fullPathRefs; int flags;

#if false //use shared memory instead of pipe. Works, but unfinished, used only to compare speed. Same speed.
		var eventName = args[0];
		var handles = stackalloc IntPtr[2] {
			OpenEvent(Api.SYNCHRONIZE, false, eventName),
			default
		};

		for(int i = 0; i < 2; i++) {
			int er = Api.WaitForSingleObject(handles[0], 50);
			//AOutput.Write(er);
			if(er == 0) goto g1;
			if(er != Api.WAIT_TIMEOUT) return;
			//APerf.First();
			switch(i) {
			case 0: _Prepare0(); break;
			case 1: _Prepare1(); break;
			}
			//APerf.NW();
		}
		handles[1] = Handle_.OpenProcess(eventName.ToInt(eventName.IndexOf('-') + 1), Api.SYNCHRONIZE);
		if(0 != Api.WaitForMultipleObjectsEx(2, handles, false, -1, false)) return;
		g1:

		//ADebug.PrintLoadedAssemblies(true, true);
		APerf.Shared.Next('1');

		//todo: mutex
		var m = SharedMemory_.Ptr;
		var b = new ReadOnlySpan<byte>(m->tasks.data, m->tasks.size).ToArray();
		//AOutput.Write(m->tasks.size, b);
		var a = Serializer_.Deserialize(b);
		ATask.Init_(ATRole.MiniProgram, a[0]);
		asmFile = a[1]; flags = a[2]; args = a[3]; fullPathRefs = a[4];
		string wrp = a[5]; if(wrp != null) Environment.SetEnvironmentVariable("ATask.WriteResult.pipe", wrp);
		AFolders.Workspace = new FolderPath(a[6]);
#else
		string pipeName = args[0]; //if(!pipeName.Starts(@"\\.\pipe\Au.Task-")) return;

		for(int i = 0; i < 3; i++) {
			if(Api.WaitNamedPipe(pipeName, i == 2 ? -1 : 50)) break;
			if(Marshal.GetLastWin32Error() != Api.ERROR_SEM_TIMEOUT) return;
			//APerf.First();
			switch(i) {
			case 0: _Prepare0(); break;
			case 1: _Prepare1(); break;
			}
			//APerf.NW();
		}

		//ADebug.PrintLoadedAssemblies(true, true);
		//APerf.Shared.Next('1');

		using(var pipe = Api.CreateFile(pipeName, Api.GENERIC_READ, 0, default, Api.OPEN_EXISTING, 0)) {
			if(pipe.Is0) { ADebug.PrintNativeError_(); return; }
			//APerf.Shared.Next('2');
			int size; if(!Api.ReadFile(pipe, &size, 4, out int nr, default) || nr != 4) return;
			//APerf.Shared.Next('3');
			if(!Api.ReadFileArr(pipe, out var b, size, out nr) || nr != size) return;
			//APerf.Shared.Next('4');

			var a = Serializer_.Deserialize(b);
			ATask.Init_(ATRole.MiniProgram, a[0]);
			asmFile = a[1]; flags = a[2]; args = a[3]; fullPathRefs = a[4];
			string wrp = a[5]; if(wrp != null) Environment.SetEnvironmentVariable("ATask.WriteResult.pipe", wrp);
			AFolders.Workspace = new FolderPath(a[6]);
		}
#endif

		//APerf.Shared.Next('5');

		bool mtaThread = 0 != (flags & 2); //app without [STAThread]
		if(mtaThread == s_isSTA) _SetComApartment(mtaThread ? ApartmentState.MTA : ApartmentState.STA);

		if(0 != (flags & 4)) Api.AllocConsole(); //meta console true

		//if(0 != (flags & 1)) { //hasConfig
		//	var config = asmFile + ".config";
		//	if(AFile.ExistsAsFile(config, true)) AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", config);
		//}

		if(s_hook == null) _Hook();

		//APerf.Shared.Next('6');

		try { RunAssembly.Run(asmFile, args, fullPathRefs: fullPathRefs); }
		catch(Exception ex) { AOutput.Write(ex); }
		finally { s_hook?.Dispose(); }
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _Prepare0()
	{
#if !DEBUG
		//makes faster by several ms. Without AJit.Compile makes faster 2 times. This code 5 ms.
		var fProfile = AFolders.LocalAppData.ToString() + @"\Au\ProfileOptimization"; //3.8 ms first time. Environment.GetFolderPath 6 ms. SHGetFolderPath 2.7 ms.
		if(AFile.ExistsAsDirectory(fProfile, true)) { //created by editor
			var alc = System.Runtime.Loader.AssemblyLoadContext.Default;
			alc.SetProfileOptimizationRoot(fProfile);
			alc.StartProfileOptimization("Tasks.speed");
			//same: ProfileOptimization.SetProfileRoot(fProfile); ProfileOptimization.StartProfile("Tasks.speed");
		}
#endif

		//JIT slowest-to-JIT methods. Makes faster even with profile optimization.
		AJit.Compile(typeof(RunAssembly), nameof(RunAssembly.Run));
		AJit.Compile(typeof(Serializer_), "Deserialize");

		//_ = Assembly.GetExecutingAssembly().EntryPoint.GetParameters().Length;
		var ep = Assembly.GetExecutingAssembly().EntryPoint;
		if(ep.GetParameters().Length == 1) ep.Invoke(null, new object[] { Array.Empty<string>() }); //call our Main, it just returns because args.Length==0
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _Prepare1()
	{
		_SetComApartment(ApartmentState.STA);

		//if need to preload some assemblies, use code like this
		//_ = typeof(Stack<string>).Assembly; //System.Collections

		ATask.Init_(ATRole.MiniProgram);
		Log_.Run.Write(null);

		_Hook();

		"Au".ToLowerInvariant(); //in .NET 5 preview would be first time 15-40 ms

		APerf.Shared.Next();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static void _SetComApartment(ApartmentState state)
	{
		var t = Thread.CurrentThread;
		t.TrySetApartmentState(ApartmentState.Unknown);
		t.TrySetApartmentState(state);
		s_isSTA = state == ApartmentState.STA;

		//This is undocumented, but works if we set ApartmentState.Unknown at first.
		//With [STAThread] the process initially has +2 threads. Also now slightly faster.
	}
	static bool s_isSTA;

	static void _Hook()
	{
		s_hook = AHookWin.ThreadCbt(m => {
			//AOutput.Write(m.code, m.wParam, m.lParam);
			//switch(m.code) {
			//case HookData.CbtEvent.ACTIVATE:
			//case HookData.CbtEvent.SETFOCUS:
			//	AOutput.Write((AWnd)m.wParam);
			//	AOutput.Write(AWnd.Active);
			//	AOutput.Write(AWnd.ThisThread.Active);
			//	AOutput.Write(AWnd.Focused);
			//	AOutput.Write(AWnd.ThisThread.Focused);
			//	break;
			//}
			if(m.code == HookData.CbtEvent.ACTIVATE) {
				var w = (AWnd)m.wParam;
				if(!w.HasExStyle(WS2.NOACTIVATE)) {
					//AOutput.Write(w);
					//AOutput.Write(w.ExStyle);
					//Api.SetForegroundWindow(w); //does not work
					ATimer.After(1, _ => {
						if(s_hook == null) return;
						//AOutput.Write(AWnd.Active);
						//AOutput.Write(AWnd.ThisThread.Active);
						bool isActive = w == AWnd.Active, activate = !isActive && w == AWnd.ThisThread.Active;
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
	static AHookWin s_hook;

}
