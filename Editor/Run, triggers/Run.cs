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
	/// </summary>
	/// <param name="run">If true, compiles if need and executes. If false, always compiles and does not execute.</param>
	/// <param name="f">C# file (script or .cs). Does nothing if null or not C# file.</param>
	/// <param name="args">To pass to Main.</param>
	/// <remarks>
	/// Saves editor text if need.
	/// Calls <see cref="Compiler.Compile"/>.
	/// Must be always called in the main UI thread (Thread.CurrentThread.ManagedThreadId == 1), because calls its file model functions.
	/// </remarks>
	public static void CompileAndRun(bool run, FileNode f, string[] args = null)
	{
#if TEST_STARTUP_SPEED
		args = new string[] { Time.Microseconds.ToString() }; //and in script use this code: Print(Time.Microseconds-Convert.ToInt64(args[0]));
#endif

		Model.Save.TextNowIfNeed(onlyText: true);

		if(f == null) return;
		g1:
		if(f.FindProject(out var projFolder, out var projMain)) f = projMain;

		//can be set to run other script/app instead.
		//	Useful for library projects. Single files have other alternatives - move to a script project or move code to a script file.
		if(run) {
			var f2 = f.RunOther;
			if(f2 != null) { f = f2; goto g1; }
		}

		if(!f.IsCodeFile) return;

		bool ok = Compiler.Compile(run, out var r, f, projFolder);

		if(run && r.outputType == EOutputType.dll) { //info: if run dll, compiler sets r.outputType and returns false
			_OnRunClassFile(f, projFolder);
			return;
		}

		if(!ok) return;
		if(!run) return;

		if(r.isolation == EIsolation.hostThread) {
			RunAsm.RunHere(RIsolation.hostThread, r.file, r.pdbOffset, args);
			return;
		}

		Tasks.RunCompiled(f, r, args);
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

class RunningTask :AuTask
{
	public readonly FileNode f;
	public readonly bool isUnattended;

	public RunningTask(FileNode f, IAuTaskManager manager, bool isUnattended) : base(++s_taskId, manager)
	{
		this.f = f; this.isUnattended = isUnattended;
	}

	static int s_taskId;
}

class RunningTasks :IAuTaskManager
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
	Wnd _wManager;

	#region ITaskManager
	public Wnd Window => _disposed ? default : _wManager;
	public void TaskEnded(int taskId) { }
	#endregion

	public IEnumerable<RunningTask> Items => _a;

	public RunningTasks(Wnd wManager)
	{
		_wManager = wManager; //MainForm
		_a = new List<RunningTask>();
		_q = new List<_WaitingTask>();
		_recent = new List<RecentTask>();
		Timer1s += _Timer1s; //updates UI
		Wnd.Misc.UacEnableMessages(AuTask.WM_TASK_ENDED);
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
	/// Removes an ended task from the 'running' and 'recent' lists. If a task is queued and can run, starts it.
	/// When task ended, its thread or Process.Exited event handler posts to MainForm message EForm.EMsg.TaskEnded with task id in wParam. MainForm calls this function.
	/// When task ended in Au.Tasks, it posts message to our MainForm in the same way.
	/// </summary>
	internal void TaskEnded(IntPtr wParam)
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
			if(_a[i].f == f) return _a[i];
		}
		return null;
	}

	/// <summary>
	/// Returns all running files.
	/// For files that have multiple tasks is added 1 item in the list.
	/// Each time creates new list; caller can modify it.
	/// </summary>
	public List<FileNode> GetRunningFiles()
	{
		var a = new List<FileNode>(_a.Count);
		for(int i = 0; i < _a.Count; i++) {
			var t = _a[i];
			if(!a.Contains(t.f)) a.Add(t.f);
		}
		return a;
	}

	/// <summary>
	/// Gets the running task that does not have meta runUnattended true. Returns null if no such task.
	/// </summary>
	public RunningTask GetRunningSupervised()
	{
		for(int i = 0; i < _a.Count; i++) {
			var t = _a[i];
			if(!t.isUnattended) return t;
		}
		return null;
	}

	/// <summary>
	/// Tries to end all tasks of file f.
	/// Returns true if was running.
	/// Can fail, for example if the thread is in a waiting API or shows a dialog without x button. Does not throw exception.
	/// </summary>
	/// <param name="f">Can be null.</param>
	public bool EndTasksOf(FileNode f)
	{
		bool wasRunning = false;
		for(int i = _a.Count - 1; i >= 0; i--) {
			if(_a[i].f != f) continue;
			_EndTask(_a[i]);
			wasRunning = true;
		}
		return wasRunning;
	}

	/// <summary>
	/// Tries to end single task, if still running.
	/// Can fail, for example if the thread is in a waiting API or shows a dialog without x button. Does not throw exception. Does not wait.
	/// </summary>
	/// <param name="rt"></param>
	public void EndTask(RunningTask rt)
	{
		if(_a.Contains(rt)) _EndTask(rt);
	}

	bool _EndTask(RunningTask rt, bool onExit = false)
	{
		Debug.Assert(_a.Contains(rt));
		if(rt.GetTaskStartedInOtherProcess(out bool admin)) {
			if(onExit) return true; //Au.Tasks process watches us and ends tasks when this process exits, either normally or when killed or crashed
			byte R = IpcWithTasks.CallIfRunning(admin, 2, (int)_wManager.Handle, rt.taskId);
			return R == 1 || R == 255;
		} else {
			return rt.End(onExit);
		}
	}

	bool _CanRunNow(FileNode f, Compiler.CompResults r, out RunningTask running)
	{
		running = null;
		if(r.runUnattended) {
			if(r.ifRunning == EIfRunning.run) return true;
			running = _GetRunning(f);
		} else {
			running = GetRunningSupervised();
		}
		return running == null;
	}

	/// <summary>
	/// Executes the compiled assembly.
	/// Returns false if cannot execute now (see meta runUnattended/ifRunning). Then, if ifRunning waitX, adds to the queue to execute later. Also returns false if fails to start thread/process.
	/// </summary>
	/// <param name="f"></param>
	/// <param name="r"></param>
	/// <param name="args"></param>
	/// <param name="ignoreLimits">Don't check whether the task can run now.</param>
	public bool RunCompiled(FileNode f, Compiler.CompResults r, string[] args, bool ignoreLimits = false)
	{
		if(!ignoreLimits && !_CanRunNow(f, r, out var running)) {
			switch(r.ifRunning) {
			case EIfRunning.restart:
			case EIfRunning.restartOrWait:
				if(running.f == f) {
					if(_EndTask(running)) {
						//Our thread will receive posted WM_TASK_ENDED later. Until that, _CanRunNow returns false.
						//We can either wait/get WM_TASK_ENDED now, or schedule the task to run in WM_TASK_ENDED handler. Probably the later is better.
#if true
						goto case EIfRunning.wait;
#else
						for(int i = 0; i < 10; i++) {
							bool msgOK = false;
							while(Api.PeekMessage(out var m1, this.Window, AuTask.WM_TASK_ENDED, AuTask.WM_TASK_ENDED, Api.PM_REMOVE)) {
								Api.DispatchMessage(m1);
								if(m1.wParam == running.taskId) { msgOK = true; break; }
							}
							if(msgOK) break;
							if(i == 0 && 0 == Api.MsgWaitForMultipleObjectsEx(0, null, 50, Api.QS_ALLPOSTMESSAGE, 0)) continue; //makes slightly faster
							Thread.Sleep(10 + i);
							Debug_.PrintIf(i > 1, "need to wait");
						}
						if(_CanRunNow(f, r, out running)) goto gRun;
						Debug_.Print("_CanRunNow returned false");
#endif
					}
				}
				if(r.ifRunning == EIfRunning.restart) goto case EIfRunning.unspecified;
				goto case EIfRunning.wait;
			case EIfRunning.wait:
				_q.Insert(0, new _WaitingTask(f, r, args));
				break;
			case EIfRunning.unspecified:
				string s1 = (running.f == f) ? "it" : $"{running.f.SciLink}";
				Print($"<>Cannot start {f.SciLink} because {s1} is running. You may want to add meta option <c green>runUnattended true<> or/and <c green>ifRunning leave|wait|restart|restartOrWait|run<>.");
				break;
			}
			return false;
		}
		//gRun:
		var rt = new RunningTask(f, this, r.runUnattended);
		RFlags flags = 0;
		if(r.isolation == EIsolation.process) flags |= RFlags.isProcess;
		if(r.isolation == EIsolation.thread) flags |= RFlags.isThread;
		if(r.mtaThread) flags |= RFlags.mtaThread;
		if(r.hasConfig) flags |= RFlags.hasConfig;

		string exeFile = null, argsString = null;
		if(r.isolation == EIsolation.process) {
			exeFile = r.file;
			if(!r.notInCache) {
				int iFlags = r.hasConfig ? 1 : 0; if(r.mtaThread) iFlags |= 2;
				string spo = r.pdbOffset.ToString(), sFlags = iFlags.ToString();
				if(args == null) args = new string[] { r.name, exeFile, spo, sFlags }; else args = args.Insert_(0, r.name, exeFile, spo, sFlags);
				exeFile = Folders.ThisAppBS + (r.prefer32bit ? "Au.Task32.exe" : "Au.Task.exe");
			}
			if(args != null) { argsString = Au.Util.StringMisc.CommandLineFromArray(args); args = null; }
		}

		var uac = r.uac; //same - run the task in this process or in new process started directly; admin - in/through admin Au.Tasks; user - in user Au.Tasks.
		if(uac != EUac.same) {
			if(Process_.UacInfo.IsUacDisabled) {
				uac = EUac.same;
				//info: to completely disable UAC on Win7: gpedit.msc/Computer configuration/Windows settings/Security settings/Local policies/Security options/User Account Control:Run all administrators in Admin Approval Mode/Disabled. Reboot.
				//note: when UAC disabled, if our uac is System, IsUacDisabled returns false (we probably run as SYSTEM user). It's OK.
			} else {
				var IL = Process_.UacInfo.ThisProcess.IntegrityLevel;
				switch(IL) {
				case UacIL.High:
					if(uac == EUac.admin) uac = EUac.same;
					else if(r.isolation == EIsolation.process) { uac = EUac.same; flags |= RFlags.userProcess; }
					break;
				case UacIL.Medium:
					if(uac == EUac.user) uac = EUac.same;
					break;
				case UacIL.UIAccess:
					if(uac == EUac.user) { flags |= RFlags.noUiAccess; if(r.isolation == EIsolation.process) uac = EUac.same; }
					break;
				case UacIL.Low:
				case UacIL.Untrusted:
				case UacIL.Unknown:
				//uac = EUac.same;
				//break;
				case UacIL.System:
				case UacIL.Protected:
					Print($"<>Cannot run {f.SciLink}. Meta option <c green>uac {uac}<> cannot be used when the UAC integrity level of this process is {IL}. Supported levels are Medium, High and uiAccess.");
					return false;
					//info: cannot start Medium IL process from System process. Would need another function. Never mind.
				}
			}
		}
		//Print(uac, flags);

		if(uac == EUac.same) {
			var p = new RParams(r.name, r.file, exeFile, argsString ?? (object)args, r.pdbOffset, flags);
#if true
			if(!rt.Run(p)) return false;
#else
			var p1 = Perf.StartNew();
			for(int i = 0; i < 8; i++) if(!rt.Run(p, this)) return false;
			//for(int i = 0; i < 8; i++) { if(!rt.Run(p, this)) return false; Thread.Sleep(20); }
			p1.NW('R');
#endif
			if(rt.IsRunning) _Add(rt);
		} else {
			//Perf.First();
			flags |= RFlags.remote;
			if(args != null) argsString = Au.Util.StringMisc.CommandLineFromArray(args);
			if(argsString.Length_() > 500000) { Print($"<>Error: {f.SciLink} command line arguments string too long, max 500000."); return false; }
			var asmFile = exeFile == null ? r.file : null;

			bool admin = uac == EUac.admin;
			byte R = IpcWithTasks.Call(admin, 1, (int)_wManager.Handle, rt.taskId, r.name, asmFile, exeFile, argsString, r.pdbOffset, (int)flags);
			if(R != 1) return R != 0; //0 failed, 1 started, 2 started but already ended
			rt.SetTaskStartedInOtherProcess(admin);
			_Add(rt);
		}

		return true;
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
