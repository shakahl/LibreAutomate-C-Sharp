using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Runtime.Loader;

using Au.Types;

//PROBLEM: slow startup.
//A minimal script starts in 70-100 ms cold, 40 hot.
//Workaround for role miniProgram:
//	Preload task process. Let it wait for next task. While waiting, it also can JIT etc.
//	Then starts in 12/4 ms (cold/hot). With ATask.Setup 15/5.
//	Except first time. Also not faster if several scripts are started without a delay. Never mind.
//	This is implemented in this class and in Au.AppHost (just ~10 code lines added in 1 place).

/*
//To test task startup speed, use script "task startup speed.cs":

300.ms(); //give time to preload new task process
for (int i = 0; i < 5; i++) {
//	APerf.Cpu();
//	APerf.Shared.First(); //slower
	var t=ATime.PerfMilliseconds.ToStringInvariant(); t=ATime.PerfMilliseconds.ToStringInvariant();
	ATask.Run(@"miniProgram.cs", t); //cold 10, hot 3. Without Setup: 6/2. Slower on vmware Win7+Avast.
//	ATask.Run(@"exeProgram.cs", t); //cold 80, hot 43. Slightly slower on vmware Win7+Avast.
	600.ms(); //give time for the process to exit
}

//miniProgram.cs and exeProgram.cs:

AOutput.Write(ATime.PerfMilliseconds-Int64.Parse(args[0]));
*/

//Smaller problem: .NET creates many threads. No workaround.

//PROBLEM: preloaded task's windows start inactive, behind one or more windows. Unless they activate self, like ADialog.
//	It does not depend on the foreground lock setting/API. The setting/API just enable SetForegroundWindow, but most windows don't call it.
//	Workaround: use CBT hook. It receives HCBT_ACTIVATE even when the window does not become the foreground window.
//		On HCBT_ACTIVATE, async-call SetForegroundWindow. Also, editor calls AllowSetForegroundWindow before starting task.

//PROBLEM: although Script.Main starts fast, but the process ends slowly, because of .NET.
//	Eg if starting an empty "runSingle" script every <50 ms, sometimes cannot start.

//FUTURE: option to start without preloading.

namespace Au.Util
{
	/// <summary>
	/// Prepares to quickly start and execute a script with role miniProgram in this preloaded task process.
	/// </summary>
	static unsafe class MiniProgram_
	{
		//static long s_started;
		internal static string s_scriptId;

		struct _TaskInit
		{
			public IntPtr asmFile;
			public IntPtr* args;
			public int nArgs;
		}

		static void Init(nint pn, out _TaskInit r) {
			r = default;
			string pipeName = new((char*)pn);

			ATask.s_role = ATRole.MiniProgram;

			AThread.SetComApartment_(ApartmentState.STA); //1.7 ms

			ATask.AppModuleInit_(); //2.7 ms (1.8 if with AProcess.Exit below)

			//rejected. Now this is implemented in editor. To detect when failed uses process exit code. Never mind exception text, it is not very useful.
			//AProcess.Exit += e => { //0.9 ms
			//	if (s_started != 0) AOutput.TaskEvent_(e == null ? "TE" : "TF " + e.ToStringWithoutStack(), s_started);
			//};

			for (int i = 0; ; i++) {
				if (Api.WaitNamedPipe(pipeName, i == 1 ? -1 : 25)) break;
				if (Marshal.GetLastWin32Error() != Api.ERROR_SEM_TIMEOUT) return;
				if (i == 1) break;

				//rejected: ProfileOptimization. Now everything is JIT-ed and is as fast as can be.

				AThread.Start(() => {
					//using var p2 = APerf.Create();

					//JIT
					AJit.Compile(typeof(Serializer_), "Deserialize");
					AJit.Compile(typeof(Api), nameof(Api.ReadFile), nameof(Api.CloseHandle), nameof(Api.SetEnvironmentVariable));
					//p2.Next();
					var h1 = Api.CreateFile(null, Api.GENERIC_READ, 0, default, Api.OPEN_EXISTING, 0);
					//p2.Next();
					Marshal.StringToCoTaskMemUTF8("-");
					AFolders.Workspace = new FolderPath("");
					AJit.Compile(typeof(ATask), nameof(ATask.Setup), nameof(ATask.TrayIcon_));
					//p2.Next();

					//AOutput.TaskEvent_(null, 0); //8-20 ms
					Thread.Sleep(20);
					"Au".ToLowerInvariant(); //15-40 ms

					//if need to preload some assemblies, use code like this. But now .NET loads assemblies fast, not like in old framework.
					//_ = typeof(TypeFromAssembly).Assembly;
				}, sta: false);

				_Hook();
			}

			//ADebug.PrintLoadedAssemblies(true, true);

			EFlags flags;

			//using var p1 = APerf.Create();
			using (var pipe = Api.CreateFile(pipeName, Api.GENERIC_READ, 0, default, Api.OPEN_EXISTING, 0)) {
				if (pipe.Is0) { ADebug.PrintNativeError_(); return; }
				//p1.Next();
				int size; if (!Api.ReadFile(pipe, &size, 4, out int nr, default) || nr != 4) return;
				//p1.Next();
				if (!Api.ReadFileArr(pipe, out var b, size, out nr) || nr != size) return;
				//p1.Next();
				var a = Serializer_.Deserialize(b);
				//p1.Next('d');
				ATask.s_name = a[0]; //would not need, because AppDomain.CurrentDomain.FriendlyName returns the same, but I don't trust it, it used to return a different string in the past
				flags = (EFlags)(int)a[2];

				r.asmFile = Marshal.StringToCoTaskMemUTF8(a[1]);
				//p1.Next();
				string[] args = a[3];
				if (!args.NE_()) {
					r.nArgs = args.Length;
					r.args = (IntPtr*)Marshal.AllocHGlobal(args.Length * sizeof(IntPtr));
					for (int i = 0; i < args.Length; i++) r.args[i] = Marshal.StringToCoTaskMemUTF8(args[i]);
				}
				//p1.Next();

				string wrp = a[4]; if (wrp != null) Api.SetEnvironmentVariable("ATask.WriteResult.pipe", wrp);
				AFolders.Workspace = new FolderPath(a[5]);
				s_scriptId = a[6];
				//p1.Next();
			}
			//p1.Next();

			if (0 != (flags & EFlags.RefPaths)) AssemblyLoadContext.Default.Resolving += _ResolvingAssembly;

			if (0 != (flags & EFlags.MTA)) AThread.SetComApartment_(ApartmentState.MTA);

			if (0 != (flags & EFlags.Console)) Api.AllocConsole();

			//if(0 != (flags & EFlags.Config)) { //this was with .NET 4
			//	var config = asmFile + ".config";
			//	if(AFile.ExistsAsFile(config, true)) AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", config);
			//}

			if (s_hook == null) _Hook();

			//Api.QueryPerformanceCounter(out s_started);
			//AOutput.TaskEvent_("TS", s_started);
		}

		static Assembly _ResolvingAssembly(AssemblyLoadContext alc, AssemblyName an) {
			var name = an.Name;
			foreach (var v in s_refPaths ??= Assembly.GetEntryAssembly().GetCustomAttribute<RefPathsAttribute>().Paths.Split('|')) {
				int iName = v.Length - name.Length - 4;
				if (iName <= 0 || v[iName - 1] != '\\' || !v.Eq(iName, name, true)) continue;
				if (!AFile.ExistsAsFile(v)) continue;
				//try {
				return alc.LoadFromAssemblyPath(v);
				//} catch(Exception ex) { ADebug.Print(ex.ToStringWithoutStack()); break; }
			}
			return null;
		}
		static string[] s_refPaths;

		[Flags]
		public enum EFlags
		{
			/// <summary>Has [RefPaths] attribute. It is when some meta r paths cannot be automatically resolved.</summary>
			RefPaths = 1,

			/// <summary>Main() with [MTAThread].</summary>
			MTA = 2,

			/// <summary>Has meta console true.</summary>
			Console = 4,

			//Config = 256, //meta hasConfig
		}

		static void _Hook() {
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
				if (m.code == HookData.CbtEvent.ACTIVATE) {
					var w = (AWnd)m.wParam;
					if (!w.HasExStyle(WSE.NOACTIVATE)) {
						//AOutput.Write(w);
						//AOutput.Write(w.ExStyle);
						//Api.SetForegroundWindow(w); //does not work
						ATimer.After(1, _ => {
							if (s_hook == null) return;
							//AOutput.Write(AWnd.Active);
							//AOutput.Write(AWnd.ThisThread.Active);
							bool isActive = w == AWnd.Active, activate = !isActive && w == AWnd.ThisThread.Active;
							if (isActive || activate) { s_hook.Dispose(); s_hook = null; }
							if (activate) {
								Api.SetForegroundWindow(w);
								//w.ActivateL(); //no, it's against Windows rules, and works differently with meta outputPath
								//Before starting task, editor calls AllowSetForegroundWindow. But if clicked etc a window after that:
								//	SetForegroundWindow fails always or randomly;
								//	Activate[L] fails if that window is of higher UAC IL, unless the foreground lock timeout is 0.
							}
						});
					}
				}
				return false;
			});
		}
		static AHookWin s_hook;

	}
}