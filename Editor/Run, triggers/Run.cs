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
using static Au.NoClass;
using static Program;
using Au.Compiler;
using Au.Controls;
using Au.LibRun;

static class Run
{
	/// <summary>
	/// Compiles and/or executes C# file or its project.
	/// If <paramref name="run"/> is false, returns 1 if compiled, 0 if failed to compile.
	/// Else returns: process id if started now, 0 if failed, (int)AuTask.ERunResult.deferred if scheduled to run later, (int)AuTask.ERunResult.editorThread if runs in editor thread.
	/// </summary>
	/// <param name="run">If true, compiles if need and executes. If false, always compiles and does not execute.</param>
	/// <param name="f">C# file (script or .cs). Does nothing if null or not C# file.</param>
	/// <param name="args">To pass to Main.</param>
	/// <param name="noDefer">Don't schedule to run later.</param>
	/// <param name="pipeName">Pipe name for AuTask.WriteResult.</param>
	/// <remarks>
	/// Saves editor text if need.
	/// Calls <see cref="Compiler.Compile"/>.
	/// Must be always called in the main UI thread (Thread.CurrentThread.ManagedThreadId == 1), because calls its file model functions.
	/// </remarks>
	public static int CompileAndRun(bool run, FileNode f, string[] args = null, bool noDefer = false, string pipeName = null)
	{
#if TEST_STARTUP_SPEED
		args = new string[] { Time.Microseconds.ToString() }; //and in script use this code: Print(Time.Microseconds-Convert.ToInt64(args[0]));
#endif

		Model.Save.TextNowIfNeed(onlyText: true);

		if(f == null) return 0;
		g1:
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		//can be set to run other script/app instead.
		//	Useful for library projects. Single files have other alternatives - move to a script project or move code to a script file.
		if(run) {
			var f2 = f.RunOther;
			if(f2 != null) { f = f2; goto g1; }
		}

		if(!f.IsCodeFile) return 0;

		bool ok = Compiler.Compile(run, out var r, f, projFolder);

		if(run && r.outputType == EOutputType.dll) { //info: if run dll, compiler sets r.outputType and returns false
			_OnRunClassFile(f, projFolder);
			return 0;
		}

		if(!ok) return 0;
		if(!run) return 1;

		if(r.runMode == ERunMode.editorThread) {
			RunAsm.Run(r.file, args, r.pdbOffset, RAFlags.InEditorThread);
			return (int)AuTask.ERunResult.editorThread;
		}

		return Tasks.RunCompiled(f, r, args, noDefer, pipeName);
	}

	static void _OnRunClassFile(FileNode f, FileNode projFolder)
	{
		if(!s_isRegisteredLinkRCF) { s_isRegisteredLinkRCF = true; SciTags.AddCommonLinkTag("+runClass", _LinkRunClassFile); }
		var ids = f.IdStringWithWorkspace;
		var s1 = projFolder != null ? "library" : "file";
		var s2 = projFolder != null ? "" : $", project (<+runClass \"2|{ids}\">create<>) or <c green>outputType app<> (<+runClass \"1|{ids}\">add<>)";
		Print($"<>Cannot run '{f.Name}'. It is a class {s1} without a test script (<+runClass \"3|{ids}\">create<>){s2}.");
	}

	static void _LinkRunClassFile(string s)
	{
		int action = s.ToInt_(); //1 add meta outputType app, 2 create Script project, 3 create new test script and set "run" attribute
		var f = Model.Find(s.Substring(2), null); if(f == null) return;
		FileNode f2 = null;
		string text = null;
		if(action == 1) {
			if(!Model.SetCurrentFile(f)) return;
			var doc = Panels.Editor.ActiveDoc;
			var t = doc.ST;
			t.GoToPos(0);
			t.SetString(Sci.SCI_INSERTTEXT, 0,
@"/* meta
outputType app
*/

");
		} else if(action == 2) {
			if(!_NewItem(out f2, out _, @"New Project\@Script")) return;
			f.FileMove(f2, Aga.Controls.Tree.NodePosition.After);
			text = "Class1.Function1();\r\n";
		} else {
			s = f.Name; s = "test " + s.Remove(s.Length - 3);
			if(!_NewItem(out f2, out bool isProject, "Script", s)) return;
			f.RunOther = f2;
			text =
$@"/* meta
{(isProject ? "library" : "c")} {f.ItemPath}
*/

{(isProject ? "Library." : "")}Class1.Function1();
";
		}
		if(text != null && f2 == Model.CurrentFile) Panels.Editor.ActiveDoc.ST.SetText(text);

		//Creates new item above f or f's project folder.
		bool _NewItem(out FileNode ni, out bool isProject, string template, string name = null)
		{
			isProject = f.FindProject(out var target, out _);
			if(!isProject) target = f;
			ni = Model.NewItem(target, Aga.Controls.Tree.NodePosition.Before, template, name);
			return ni != null;
		}
	}
	static bool s_isRegisteredLinkRCF;
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
	public readonly bool isUnattended;

	static int s_taskId;

	public RunningTask(FileNode f, bool isUnattended)
	{
		this.f = f;
		taskId = ++s_taskId;
		this.isUnattended = isUnattended;
	}

	public enum EStartProcess { normal, admin, userFromAdmin, uiAccess }

	/// <summary>
	/// Starts task in new process.
	/// Returns process id, or 0 if fails. However the process can be already ended (unlikely); then IsRunning is false.
	/// </summary>
	public int Run(EStartProcess sp, string exeFile, string args, string pipeName)
	{
		//#if TEST_STARTUP_SPEED
		//			args = args as string + " " + Time.Microseconds.ToString();
		//			//Debug_.LibMemoryPrint(false);
		//#endif

		int pid;
		try {
			Process_.StartResult psr = null;
			if(pipeName != null) pipeName = "AuTask.WriteResult.pipe=" + pipeName;
			switch(sp) {
			case EStartProcess.admin:
				if(args.Length_() > 500000) { Print($"<>Error: {f.SciLink} command line arguments string too long."); return 0; }
				pid = IpcWithHI.Call(1, (int)MainForm.Handle, exeFile, args, pipeName);
				if(pid == 0) return pid;
				_process = Au.Util.LibKernelWaitHandle.FromProcessId(pid, Api.SYNCHRONIZE | Api.PROCESS_TERMINATE);
				Debug.Assert(_process != null);
				goto g1;
			case EStartProcess.userFromAdmin:
				psr = Process_.LibStartUserIL(exeFile, args, pipeName, Process_.StartResult.Need.WaitHandle);
				break;
			default:
				psr = Process_.LibStart(exeFile, args, sp == EStartProcess.uiAccess, pipeName, Process_.StartResult.Need.WaitHandle);
				break;
			}
			pid = psr.pid;
			_process = psr.waitHandle;
			g1:
			RegisteredWaitHandle rwh = null;
			rwh = ThreadPool.RegisterWaitForSingleObject(_process, (context, wasSignaled) => {
				rwh.Unregister(_process);
				var p = _process; _process = null;
				p.Dispose();
				Tasks.TaskEnded1(taskId);
			}, null, -1, true);
		}
		catch(Exception ex) { Print(ex); return 0; }

		return pid;
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
				if(0 != Api.WaitForSingleObject(h, 2000)) { Debug_.Print("process not terminated"); return false; }
			} else {
				var s = Native.GetErrorMessage();
				if(0 != Api.WaitForSingleObject(h, 0)) { Debug_.Print(s); return false; }
			}
			//note: TerminateProcess kills process not immediately. Need at least several ms.
		}
		return true;
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
	Wnd _wMain;

	public IEnumerable<RunningTask> Items => _a;

	public RunningTasks()
	{
		_a = new List<RunningTask>();
		_q = new List<_WaitingTask>();
		_recent = new List<RecentTask>();
		_wMain = (Wnd)MainForm;
		Timer1s += _Timer1s; //updates UI
	}

	public void OnWorkspaceClosed()
	{
		bool onExit = MainForm.IsClosed;

		if(onExit) {
			_disposed = true;
			Timer1s -= _Timer1s;
		}

		for(int i = _a.Count - 1; i >= 0; i--) {
			_EndTask(_a[i], onExit: onExit);
		}

		if(onExit) _a.Clear();
		_q.Clear();
		_recent.Clear();

		if(!onExit) _UpdatePanels();
	}

	/// <summary>
	/// Adds a started task (thread or process) to the 'running' and 'recent' lists.
	/// Must be called in the main thread.
	/// </summary>
	/// <param name="rt"></param>
	void _Add(RunningTask rt)
	{
		Debug.Assert(!_disposed);
		_a.Insert(0, rt);
		_RecentStarted(rt.f);
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
	/// Removes an ended task from the 'running' and 'recent' lists. If a task is queued and can run, starts it.
	/// When task ended, TaskEnded1 posts to MainForm message WM_TASK_ENDED with task id in wParam. MainForm calls this function.
	/// </summary>
	internal void TaskEnded2(IntPtr wParam)
	{
		if(_disposed) return;

		int taskId = (int)wParam;
		int i = _Find(taskId);
		if(i < 0) { Debug_.Print("not found. It's OK, but should be very rare, mostly with 1-core CPU."); return; }

		var rt = _a[i];
		_a.RemoveAt(i);
		_RecentEnded(rt.f);

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

	void _Timer1s()
	{
		if(!_updateUI || !MainForm.Visible) return;
		_UpdatePanels();
	}

	void _UpdatePanels()
	{
		_updateUI = false;
		Panels.Running.UpdateList(); //~1 ms when list small, not including wmpaint
		Panels.Recent.UpdateList();
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
	/// Gets the supervised running task (meta runMode supervised or unspecified). Returns null if no such task.
	/// </summary>
	public RunningTask GetRunningSupervised()
	{
		for(int i = 0; i < _a.Count; i++) {
			var r = _a[i];
			if(!r.isUnattended && r.IsRunning) return r;
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
	/// </summary>
	public void EndTask(RunningTask rt)
	{
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
		case ERunMode.supervised: running = GetRunningSupervised(); break;
		case ERunMode.unattendedSingle: running = _GetRunning(f); break;
		default: return true;
		}
		return running == null;
	}

	/// <summary>
	/// Executes the compiled assembly in new process.
	/// Returns: process id if started now, 0 if failed, (int)AuTask.ERunResult.deferred if scheduled to run later.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="r"></param>
	/// <param name="args"></param>
	/// <param name="noDefer">Don't schedule to run later. If cannot run now, just return 0.</param>
	/// <param name="pipeName">Pipe name for AuTask.WriteResult.</param>
	/// <param name="ignoreLimits">Don't check whether the task can run now.</param>
	public int RunCompiled(FileNode f, Compiler.CompResults r, string[] args, bool noDefer = false, string pipeName = null, bool ignoreLimits = false)
	{
		g1:
		if(!ignoreLimits && !_CanRunNow(f, r, out var running)) {
			switch(r.ifRunning) {
			case EIfRunning.restart:
			case EIfRunning.restartOrWait:
				if(running.f == f && _EndTask(running)) goto g1;
				if(r.ifRunning == EIfRunning.restartOrWait) goto case EIfRunning.wait;
				goto case EIfRunning.unspecified;
			case EIfRunning.wait:
				if(noDefer) goto case EIfRunning.unspecified;
				_q.Insert(0, new _WaitingTask(f, r, args));
				return (int)AuTask.ERunResult.deferred;
			case EIfRunning.unspecified:
				string s1 = (running.f == f) ? "it" : $"{running.f.SciLink}";
				Print($"<>Cannot start {f.SciLink} because {s1} is running. Consider meta options <c green>runMode<>, <c green>ifRunning<>.");
				break;
			}
			return 0;
		}

		var rt = new RunningTask(f, r.runMode != ERunMode.supervised);

		string exeFile = r.file, argsString = null;
		if(!r.notInCache) {
			int iFlags = r.hasConfig ? 1 : 0; if(r.mtaThread) iFlags |= 2;
			string spo = r.pdbOffset.ToString(), sFlags = iFlags.ToString();
			if(args == null) args = new string[] { r.name, exeFile, spo, sFlags }; else args = args.Insert_(0, r.name, exeFile, spo, sFlags);
			exeFile = Folders.ThisAppBS + (r.prefer32bit ? "Au.Task32.exe" : "Au.Task.exe");
		}
		if(args != null) argsString = Au.Util.StringMisc.CommandLineFromArray(args);

		RunningTask.EStartProcess sp = RunningTask.EStartProcess.normal;
		if(!Process_.UacInfo.IsUacDisabled) {
			//info: to completely disable UAC on Win7: gpedit.msc/Computer configuration/Windows settings/Security settings/Local policies/Security options/User Account Control:Run all administrators in Admin Approval Mode/Disabled. Reboot.
			//note: when UAC disabled, if our uac is System, IsUacDisabled returns false (we probably run as SYSTEM user). It's OK.
			var IL = Process_.UacInfo.ThisProcess.IntegrityLevel;
			if(r.uac == EUac.same) {
				if(IL == UacIL.UIAccess) sp = RunningTask.EStartProcess.uiAccess;
			} else {
				switch(IL) {
				case UacIL.Medium:
				case UacIL.UIAccess:
					if(r.uac == EUac.admin) sp = RunningTask.EStartProcess.admin;
					break;
				case UacIL.High:
					if(r.uac == EUac.user) sp = RunningTask.EStartProcess.userFromAdmin;
					break;
				case UacIL.Low:
				case UacIL.Untrusted:
				case UacIL.Unknown:
				//uac = EUac.same;
				//break;
				case UacIL.System:
				case UacIL.Protected:
					Print($"<>Cannot run {f.SciLink}. Meta option <c green>uac {r.uac}<> cannot be used when the UAC integrity level of this process is {IL}. Supported levels are Medium, High and uiAccess.");
					return 0;
					//info: cannot start Medium IL process from System process. Would need another function. Never mind.
				}
			}
		}

		//Perf.First();
		int pid;
#if true
		pid = rt.Run(sp, exeFile, argsString, pipeName); if(pid == 0) return 0;
#else
		var p1 = Perf.StartNew();
		for(int i = 0; i < 8; i++) pid=rt.Run(sp, exeFile, argsString, pipeName);
		//for(int i = 0; i < 8; i++) { pid=rt.Run(sp, exeFile, argsString, pipeName); Thread.Sleep(20); }
		p1.NW('R');
#endif
		_Add(rt);
		return pid;
	}

	int _Find(int taskId)
	{
		for(int i = 0; i < _a.Count; i++) {
			if(_a[i].taskId == taskId) return i;
		}
		return -1;
	}

	#region recent tasks

	public class RecentTask
	{
		public readonly FileNode f;
		public bool running;
		//FUTURE: startTime, endTime

		public RecentTask(FileNode f)
		{
			this.f = f;
			running = true;
		}
	}

	List<RecentTask> _recent;

	public IEnumerable<RecentTask> Recent => _recent;

	int _RecentFind(FileNode f)
	{
		for(int i = 0; i < _recent.Count; i++) if(_recent[i].f == f) return i;
		return -1;
	}

	void _RecentStarted(FileNode f)
	{
		int i = _RecentFind(f);
		if(i >= 0) {
			var x = _recent[i];
			x.running = true;
			if(i > 0) {
				for(int j = i; j > 0; j--) _recent[j] = _recent[j - 1];
				_recent[0] = x;
			}
		} else {
			if(_recent.Count > 100) _recent.RemoveRange(100, _recent.Count - 100);

			_recent.Insert(0, new RecentTask(f));
		}
	}

	void _RecentEnded(FileNode f)
	{
		if(IsRunning(f)) return;
		int i = _RecentFind(f);
		Debug.Assert(i >= 0 || f.Model != Model); if(i < 0) return;
		_recent[i].running = false;
	}

	#endregion
}
