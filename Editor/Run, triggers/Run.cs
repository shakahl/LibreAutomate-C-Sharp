//#define TEST_STARTUP_SPEED

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
//using System.Windows.Forms;
//using System.Drawing;
using System.Linq;
using System.Xml;
//using System.Xml.Linq;
using System.IO.Pipes;

using Au;
using Au.Types;
using static Au.AStatic;
using Au.Compiler;
using Au.Controls;
using Au.Triggers;

static class Run
{
	/// <summary>
	/// Compiles and/or executes C# file or its project.
	/// If <paramref name="run"/> is false, returns 1 if compiled, 0 if failed to compile.
	/// Else returns: process id if started now, 0 if failed, (int)ATask.ERunResult.deferred if scheduled to run later, (int)ATask.ERunResult.editorThread if runs in editor thread.
	/// </summary>
	/// <param name="run">If true, compiles if need and executes. If false, always compiles and does not execute.</param>
	/// <param name="f">C# file. Does nothing if null or not C# file.</param>
	/// <param name="args">To pass to Main.</param>
	/// <param name="noDefer">Don't schedule to run later.</param>
	/// <param name="wrPipeName">Pipe name for ATask.WriteResult.</param>
	/// <remarks>
	/// Saves editor text if need.
	/// Calls <see cref="Compiler.Compile"/>.
	/// Must be always called in the main UI thread (Thread.CurrentThread.ManagedThreadId == 1), because calls its file model functions.
	/// </remarks>
	public static int CompileAndRun(bool run, FileNode f, string[] args = null, bool noDefer = false, string wrPipeName = null)
	{
#if TEST_STARTUP_SPEED
		args = new string[] { ATime.PerfMicroseconds.ToString() }; //and in script use this code: Print(ATime.Microseconds-Convert.ToInt64(args[0]));
#endif

		Program.Model.Save.TextNowIfNeed(onlyText: true);
		Program.Model.Save.WorkspaceNowIfNeed(); //because the script may kill editor, eg if runs in editor thread

		if(f == null) return 0;
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		//can be set to run other script instead.
		//	Useful for library projects. Single files have other alternatives - move to a script project or move code to a script file.
		if(run) {
			var f2 = f.TestScript;
			if(f2 != null) {
				if(!f2.FindProject(out projFolder, out projMain)) f = f2;
				else if(projMain != f) f = projMain;
				else { Print($"<>The test script {f2.SciLink} cannot be in the project folder {projFolder.SciLink}"); return 0; }
			}
		}

		if(!f.IsCodeFile) return 0;

		bool ok = Compiler.Compile(run ? ECompReason.Run : ECompReason.CompileAlways, out var r, f, projFolder);

		if(run && (r.role == ERole.classFile || r.role == ERole.classLibrary)) { //info: if classFile, compiler sets r.role and returns false (does not compile)
			_OnRunClassFile(f, projFolder);
			return 0;
		}

		if(!ok) return 0;
		if(!run) return 1;

		if(r.role == ERole.editorExtension) {
			RunAssembly.Run(r.file, args, r.pdbOffset, RAFlags.InEditorThread);
			return (int)ATask.ERunResult.editorThread;
		}

		return Program.Tasks.RunCompiled(f, r, args, noDefer, wrPipeName);
	}

	static void _OnRunClassFile(FileNode f, FileNode projFolder)
	{
		if(!s_isRegisteredLinkRCF) { s_isRegisteredLinkRCF = true; SciTags.AddCommonLinkTag("+runClass", _SciLink_RunClassFile); }
		var ids = f.IdStringWithWorkspace;
		var s2 = projFolder != null ? "" : $", project (<+runClass \"2|{ids}\">create<>) or role exeProgram (<+runClass \"1|{ids}\">add<>)";
		Print($"<>Cannot run '{f.Name}'. It is a class file without a test script (<+runClass \"3|{ids}\">create<>){s2}.");
	}

	static void _SciLink_RunClassFile(string s)
	{
		int action = s.ToInt(); //1 add meta role miniProgram, 2 create Script project, 3 create new test script and set "run" attribute
		var f = Program.Model.Find(s.Substring(2), null); if(f == null) return;
		FileNode f2 = null;
		if(action == 1) { //add meta role exeProgram
			if(!Program.Model.SetCurrentFile(f)) return;
			Program.Model.Properties();
		} else {
			if(action == 2) { //create project
				if(!_NewItem(out f2, @"New Project\@Script")) return;
				f.FileMove(f2, Aga.Controls.Tree.NodePosition.After);
			} else { //create test script
				s = "test " + APath.GetFileName(f.Name, true);
				if(!_NewItem(out f2, "Script.cs", s)) return;
				f.TestScript = f2;
			}

			//Creates new item above f or f's project folder.
			bool _NewItem(out FileNode ni, string template, string name = null)
			{
				bool isProject = f.FindProject(out var target, out _);
				if(!isProject) target = f;

				var text = new EdNewFileText();
				if(action == 2) {
					text.text = "Class1.Function1();\r\n";
				} else {
					text.meta = $"/*/ {(isProject ? "pr" : "c")} {f.ItemPath} /*/ ";
					text.text = $"{(isProject ? "Library." : "")}Class1.Function1();\r\n";
				}

				ni = Program.Model.NewItem(target, Aga.Controls.Tree.NodePosition.Before, template, name, text: text);
				return ni != null;
			}
		}
	}
	static bool s_isRegisteredLinkRCF;

	/// <summary>
	/// Disables, enables or toggles triggers in all processes. See <see cref="ActionTriggers.DisabledEverywhere"/>.
	/// Also updates UI: changes tray icon and checks/unchecks the menu item.
	/// </summary>
	/// <param name="disable">If null, toggles.</param>
	public static void DisableTriggers(bool? disable)
	{
		bool dis = disable switch { true => true, false => false, _ => !ActionTriggers.DisabledEverywhere };
		if(dis == ActionTriggers.DisabledEverywhere) return;
		ActionTriggers.DisabledEverywhere = dis;
		EdTrayIcon.Disabled = dis;
		Strips.CheckCmd(nameof(CmdHandlers.Run_DisableTriggers), dis);
	}
}

/// <summary>
/// A running automation task.
/// Starts/ends task, watches/notifies when ended.
/// </summary>
class RunningTask
{
	volatile WaitHandle _process;
	public readonly FileNode f;
	public readonly int taskId;
	public readonly int processId;
	public readonly bool isBlue;

	static int s_taskId;

	public RunningTask(FileNode f, WaitHandle hProcess, bool isBlue)
	{
		taskId = ++s_taskId;
		this.f = f;
		_process = hProcess;
		processId = Api.GetProcessId(hProcess.SafeWaitHandle.DangerousGetHandle());
		this.isBlue = isBlue;

		RegisteredWaitHandle rwh = null;
		rwh = ThreadPool.RegisterWaitForSingleObject(_process, (context, wasSignaled) => {
			rwh.Unregister(_process);
			var p = _process; _process = null;
			p.Dispose();
			Program.Tasks.TaskEnded1(taskId);
		}, null, -1, true);
	}

	/// <summary>
	/// False if task is already ended or still not started.
	/// </summary>
	//public bool IsRunning => _process != null;
	public bool IsRunning {
		get {
			var p = _process;
			if(p == null) return false;
			return 0 != Api.WaitForSingleObject(p.SafeWaitHandle.DangerousGetHandle(), 0);
		}
	}

	/// <summary>
	/// Ends this task (kills process), if running.
	/// Returns false if fails, unlikely.
	/// </summary>
	/// <param name="onProgramExit">Called on program exit. Returns true even if fails. Does not wait.</param>
	public bool End(bool onProgramExit)
	{
		var p = _process;
		if(p != null) {
			var h = p.SafeWaitHandle.DangerousGetHandle();
			bool ok = Api.TerminateProcess(h, -1);
			if(onProgramExit) return true;
			if(ok) {
				if(0 != Api.WaitForSingleObject(h, 2000)) { ADebug.Print("process not terminated"); return false; }
			} else {
				var s = ALastError.Message;
				if(0 != Api.WaitForSingleObject(h, 0)) { ADebug.Print(s); return false; }
			}
			//note: TerminateProcess kills process not immediately. Need at least several ms.
		}
		return true;
		//SHOULDDO: release pressed keys.
	}
}

/// <summary>
/// Manages running automation tasks.
/// </summary>
class RunningTasks
{
	class _WaitingTask
	{
		public readonly FileNode f;
		public readonly Compiler.CompResults r;
		public readonly string[] args;

		public _WaitingTask(FileNode f, Compiler.CompResults r, string[] args)
		{
			this.f = f; this.r = r; this.args = args;
		}
	}

	readonly List<RunningTask> _a;
	readonly List<_WaitingTask> _q; //not Queue because may need to remove item at any index
	bool _updateUI;
	volatile bool _disposed;
	AWnd _wMain;

	public IEnumerable<RunningTask> Items => _a;

	public RunningTasks()
	{
		_a = new List<RunningTask>();
		_q = new List<_WaitingTask>();
		_wMain = (AWnd)Program.MainForm;
		Program.Timer1sOr025s += _TimerUpdateUI;
		ATask.LibInit(ATRole.EditorExtension);
		AFile.Delete(LibLog.Run.FilePath);
	}

	public void OnWorkspaceClosed()
	{
		bool onExit = Program.Loaded >= EProgramState.Unloading;

		if(onExit) {
			_disposed = true;
			Program.Timer1sOr025s -= _TimerUpdateUI;
		}

		for(int i = _a.Count - 1; i >= 0; i--) {
			_EndTask(_a[i], onExit: onExit);
		}

		if(onExit) _a.Clear();
		_q.Clear();

		if(!onExit) _UpdatePanels();
	}

	/// <summary>
	/// Adds a started task to the 'running' list.
	/// Must be called in the main thread.
	/// </summary>
	/// <param name="rt"></param>
	void _Add(RunningTask rt)
	{
		Debug.Assert(!_disposed);
		_a.Insert(0, rt);
		_updateUI = true;
	}

	/// <summary>
	/// Called in a threadpool thread when a task process exited.
	/// </summary>
	/// <param name="taskId"></param>
	internal void TaskEnded1(int taskId)
	{
		if(_disposed) return;
		_wMain.Post(WM_TASK_ENDED, taskId);
	}

	/// <summary>
	/// When task ended, this message is posted to MainForm, with wParam=taskId.
	/// </summary>
	public const int WM_TASK_ENDED = Api.WM_USER + 900;

	/// <summary>
	/// Removes an ended task from the 'running' list. If a task is queued and can run, starts it.
	/// When task ended, TaskEnded1 posts to MainForm message WM_TASK_ENDED with task id in wParam. MainForm calls this function.
	/// </summary>
	internal void TaskEnded2(IntPtr wParam)
	{
		if(_disposed) return;

		int taskId = (int)wParam;
		int i = _Find(taskId);
		if(i < 0) { ADebug.Print("not found. It's OK, but should be very rare, mostly with 1-core CPU."); return; }

		var rt = _a[i];
		_a.RemoveAt(i);
		HooksServer.Instance?.RemoveTask(rt.processId);

		for(int j = _q.Count - 1; j >= 0; j--) {
			var t = _q[j];
			if(_CanRunNow(t.f, t.r, out _)) {
				_q.RemoveAt(j);
				RunCompiled(t.f, t.r, t.args, ignoreLimits: true);
				break;
			}
		}

		_updateUI = true;
	}

	void _TimerUpdateUI()
	{
		if(!_updateUI) return;
		EdTrayIcon.Running = GetGreenTask() != null;
		if(!Program.MainForm.Visible) return;
		_UpdatePanels();
	}

	void _UpdatePanels()
	{
		_updateUI = false;
		Panels.Running.UpdateList(); //~1 ms when list small, not including wmpaint
	}

	/// <summary>
	/// Returns true if one or more tasks of file f are running.
	/// </summary>
	/// <param name="f">Can be null.</param>
	public bool IsRunning(FileNode f) => null != _GetRunning(f);

	RunningTask _GetRunning(FileNode f)
	{
		for(int i = 0; i < _a.Count; i++) {
			var r = _a[i];
			if(r.f == f && r.IsRunning) return r;
		}
		return null;
	}

	/// <summary>
	/// Gets the "green" running task (meta runMode green or unspecified). Returns null if no such task.
	/// </summary>
	public RunningTask GetGreenTask()
	{
		for(int i = 0; i < _a.Count; i++) {
			var r = _a[i];
			if(!r.isBlue && r.IsRunning) return r;
		}
		return null;
	}

	//currently not used
	///// <summary>
	///// Returns all running files.
	///// For files that have multiple tasks is added 1 item in the list.
	///// Each time creates new list; caller can modify it.
	///// </summary>
	//public List<FileNode> GetRunningFiles()
	//{
	//	var a = new List<FileNode>(_a.Count);
	//	for(int i = 0; i < _a.Count; i++) {
	//		var t = _a[i];
	//		if(!a.Contains(t.f)) a.Add(t.f);
	//	}
	//	return a;
	//}

	/// <summary>
	/// Ends all tasks of file f.
	/// Returns true if was running.
	/// </summary>
	/// <param name="f">Can be null.</param>
	public bool EndTasksOf(FileNode f)
	{
		bool wasRunning = false;
		for(int i = _a.Count - 1; i >= 0; i--) {
			var r = _a[i];
			if(r.f != f || !r.IsRunning) continue;
			_EndTask(r);
			wasRunning = true;
		}
		return wasRunning;
	}

	/// <summary>
	/// Ends single task, if still running.
	/// If rt==null, ends the green task, if running.
	/// </summary>
	public void EndTask(RunningTask rt = null)
	{
		if(rt == null) { rt = Program.Tasks.GetGreenTask(); if(rt == null) return; }
		if(_a.Contains(rt)) _EndTask(rt);
	}

	bool _EndTask(RunningTask rt, bool onExit = false)
	{
		Debug.Assert(_a.Contains(rt));
		return rt.End(onExit);
	}

	bool _CanRunNow(FileNode f, Compiler.CompResults r, out RunningTask running)
	{
		running = null;
		switch(r.runMode) {
		case ERunMode.green: running = GetGreenTask(); break;
		case ERunMode.blue when r.ifRunning != EIfRunning.runIfBlue: running = _GetRunning(f); break;
		default: return true;
		}
		return running == null;
	}

	/// <summary>
	/// Executes the compiled assembly in new process.
	/// Returns: process id if started now, 0 if failed, (int)ATask.ERunResult.deferred if scheduled to run later.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="r"></param>
	/// <param name="args"></param>
	/// <param name="noDefer">Don't schedule to run later. If cannot run now, just return 0.</param>
	/// <param name="wrPipeName">Pipe name for ATask.WriteResult.</param>
	/// <param name="ignoreLimits">Don't check whether the task can run now.</param>
	public unsafe int RunCompiled(FileNode f, Compiler.CompResults r, string[] args, bool noDefer = false, string wrPipeName = null, bool ignoreLimits = false)
	{
		g1:
		if(!ignoreLimits && !_CanRunNow(f, r, out var running)) {
			switch(r.ifRunning) {
			case EIfRunning.restart:
			case EIfRunning.restartOrWait:
				if(running.f == f && _EndTask(running)) goto g1;
				if(r.ifRunning == EIfRunning.restartOrWait) goto case EIfRunning.wait;
				goto case EIfRunning.runIfBlue;
			case EIfRunning.wait:
				if(noDefer) goto case EIfRunning.runIfBlue;
				_q.Insert(0, new _WaitingTask(f, r, args));
				return (int)ATask.ERunResult.deferred;
			case EIfRunning.runIfBlue:
				string s1 = (running.f == f) ? "it" : $"{running.f.SciLink}";
				Print($"<>Cannot start {f.SciLink} because {s1} is running. You may want to <+properties \"{f.IdStringWithWorkspace}\">change<> <c green>runMode<> or/and <c green>ifRunning<>.");
				break;
			}
			return 0;
		}

		_SpUac uac = _SpUac.normal; int preIndex = 0;
		if(!AUac.IsUacDisabled) {
			//info: to completely disable UAC on Win7: gpedit.msc/Computer configuration/Windows settings/Security settings/Local policies/Security options/User Account Control:Run all administrators in Admin Approval Mode/Disabled. Reboot.
			//note: when UAC disabled, if our uac is System, IsUacDisabled returns false (we probably run as SYSTEM user). It's OK.
			var IL = AUac.OfThisProcess.IntegrityLevel;
			if(r.uac == EUac.inherit) {
				switch(IL) {
				case UacIL.High: preIndex = 1; break;
				case UacIL.UIAccess: uac = _SpUac.uiAccess; preIndex = 2; break;
				}
			} else {
				switch(IL) {
				case UacIL.Medium:
				case UacIL.UIAccess:
					if(r.uac == EUac.admin) uac = _SpUac.admin;
					break;
				case UacIL.High:
					if(r.uac == EUac.user) uac = _SpUac.userFromAdmin;
					break;
				case UacIL.Low:
				case UacIL.Untrusted:
				case UacIL.Unknown:
				//break;
				case UacIL.System:
				case UacIL.Protected:
					Print($"<>Cannot run {f.SciLink}. Meta option <c green>uac {r.uac}<> cannot be used when the UAC integrity level of this process is {IL}. Supported levels are Medium, High and uiAccess.");
					return 0;
					//info: cannot start Medium IL process from System process. Would need another function. Never mind.
				}
				if(r.uac == EUac.admin) preIndex = 1;
			}
		}

		string exeFile, argsString;
		_Preloaded pre = null; byte[] taskParams = null;
		if(r.notInCache) { //meta role exeProgram
			exeFile = r.file;
			argsString = args == null ? null : Au.Util.AStringUtil.CommandLineFromArray(args);
		} else {
			exeFile = AFolders.ThisAppBS + (r.prefer32bit ? "Au.Task32.exe" : "Au.Task.exe");

			int iFlags = r.hasConfig ? 1 : 0;
			if(r.mtaThread) iFlags |= 2;
			if(r.console) iFlags |= 4;
			taskParams = Au.Util.LibSerializer.SerializeWithSize(r.name, r.file, r.pdbOffset, iFlags, args, wrPipeName);
			wrPipeName = null;

			if(r.prefer32bit && AVersion.Is64BitOS) preIndex += 3;
			pre = s_preloaded[preIndex] ?? (s_preloaded[preIndex] = new _Preloaded(preIndex));
			argsString = pre.pipeName;
		}

		int pid; WaitHandle hProcess = null; bool disconnectPipe = false;
		try {
			//APerf.First();
			var pp = pre?.hProcess;
			if(pp != null && 0 != Api.WaitForSingleObject(pp.SafeWaitHandle.DangerousGetHandle(), 0)) { //preloaded process exists
				hProcess = pp; pid = pre.pid;
				pre.hProcess = null; pre.pid = 0;
			} else {
				if(pp != null) { pp.Dispose(); pre.hProcess = null; pre.pid = 0; } //preloaded process existed but somehow ended
				(pid, hProcess) = _StartProcess(uac, exeFile, argsString, wrPipeName);
			}
			Api.AllowSetForegroundWindow(pid);

			if(pre != null) {
				//APerf.First();
				var o = new Api.OVERLAPPED { hEvent = pre.overlappedEvent };
				if(!Api.ConnectNamedPipe(pre.hPipe, &o)) {
					int e = ALastError.Code; if(e != Api.ERROR_IO_PENDING) throw new AuException(e);
					var ha = stackalloc IntPtr[2] { pre.overlappedEvent, hProcess.SafeWaitHandle.DangerousGetHandle() };
					int wr = Api.WaitForMultipleObjectsEx(2, ha, false, -1, false);
					if(wr != 0) { Api.CancelIo(pre.hPipe); throw new AuException("*start task. Preloaded task process ended"); } //note: if fails when 32-bit process, rebuild solution with platform x86
					disconnectPipe = true;
					if(!Api.GetOverlappedResult(pre.hPipe, ref o, out _, false)) throw new AuException(0);
				}
				//APerf.Next();
				if(!Api.WriteFileArr(pre.hPipe, taskParams, out _)) throw new AuException(0);
				//APerf.Next();
				Api.DisconnectNamedPipe(pre.hPipe); disconnectPipe = false;
				//APerf.NW('e');

				//start preloaded process for next task. Let it wait for pipe connection.
				if(uac != _SpUac.admin) { //we don't want second UAC consent
					try { (pre.pid, pre.hProcess) = _StartProcess(uac, exeFile, argsString, null); }
					catch(Exception ex) { ADebug.Print(ex); }
				}
			}
		}
		catch(Exception ex) {
			Print(ex);
			if(disconnectPipe) Api.DisconnectNamedPipe(pre.hPipe);
			hProcess?.Dispose();
			return 0;
		}

		var rt = new RunningTask(f, hProcess, r.runMode != ERunMode.green);
		_Add(rt);
		return pid;
	}

	class _Preloaded
	{
		public readonly string pipeName;
		public readonly LibHandle hPipe;
		public readonly LibHandle overlappedEvent;
		public WaitHandle hProcess;
		public int pid;

		public _Preloaded(int index)
		{
			pipeName = $@"\\.\pipe\Au.Task-{Api.GetCurrentProcessId()}-{index}";
			hPipe = Api.CreateNamedPipe(pipeName,
				Api.PIPE_ACCESS_OUTBOUND | Api.FILE_FLAG_OVERLAPPED, //use async pipe because editor would hang if task process exited without connecting. Same speed.
				Api.PIPE_TYPE_MESSAGE | Api.PIPE_REJECT_REMOTE_CLIENTS,
				1, 0, 0, 0, null);
			overlappedEvent = Api.CreateEvent(false);
		}

		~_Preloaded()
		{
			hPipe.Dispose();
			overlappedEvent.Dispose();
		}
	}
	_Preloaded[] s_preloaded = new _Preloaded[6]; //user, admin, uiAccess, user32, admin32, uiAccess32

	/// <summary>
	/// How _StartProcess must start process.
	/// Note: it is not UAC IL of the process.
	/// </summary>
	enum _SpUac
	{
		normal, //start process of same IL as this process, but without uiAccess. It is how CreateProcess API works.
		admin, //start admin process from this user or uiAccess process
		userFromAdmin, //start user process from this admin process
		uiAccess, //start uiAccess process from this uiAccess process
	}

	/// <summary>
	/// Starts task process.
	/// Returns (processId, processHandle). Throws if failed.
	/// </summary>
	static (int pid, WaitHandle hProcess) _StartProcess(_SpUac uac, string exeFile, string args, string wrPipeName)
	{
		if(wrPipeName != null) wrPipeName = "ATask.WriteResult.pipe=" + wrPipeName;
		if(uac == _SpUac.admin) {
			if(wrPipeName != null) throw new AuException($"*start process '{exeFile}' as admin and enable ATask.WriteResult"); //cannot pass environment variables. //rare //FUTURE
			var k = AExec.Run(exeFile, args, RFlags.Admin | RFlags.NeedProcessHandle, "");
			return (k.ProcessId, k.ProcessHandle);
			//note: don't try to start task without UAC consent. It is not secure.
			//	Normally Au editor runs as admin in admin user account, and don't need to go through this.
		} else {
			var ps = new Au.Util.LibProcessStarter(exeFile, args, "", envVar: wrPipeName, rawExe: true);
			var need = Au.Util.LibProcessStarter.Result.Need.WaitHandle;
			var psr = uac == _SpUac.userFromAdmin ? ps.StartUserIL(need) : ps.Start(need, inheritUiaccess: uac == _SpUac.uiAccess);
			return (psr.pid, psr.waitHandle);
		}
	}

	int _Find(int taskId)
	{
		for(int i = 0; i < _a.Count; i++) {
			if(_a[i].taskId == taskId) return i;
		}
		return -1;
	}
}
