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
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Au.LibRun;

class RunningTasks2 :IAuTaskManager
{
	ConcurrentDictionary<int, AuTask> _d;
	Wnd _wManager;

	#region ITaskManager
	//Called in other thread.
	public Wnd Window => _wManager;

	//Called in other thread.
	public void TaskEnded(int taskId) => _d.TryRemove(taskId, out _);
	#endregion

	public RunningTasks2(Wnd wManager)
	{
		_wManager = wManager;
		_d = new ConcurrentDictionary<int, AuTask>();
	}

	//Can be called in other thread.
	public void Dispose(bool onExit)
	{
		if(!_wManager.IsAlive) _wManager = default;
		foreach(var rt in _d.Values) rt.End(onExit);
		//Output.LibWriteQM2($"dispose, onExit={onExit}, count={_d.Count}");
		if(!onExit && _d.Count != 0) s_hung.Add(this);
	}

	static List<RunningTasks2> s_hung = new List<RunningTasks2>();

	/// <summary>
	/// Terminates threads of all old disposed RunningTasks2 instances that could not end some tasks when disposing.
	/// Called on program exit. Such threads would prevent process exit.
	/// </summary>
	public static void FinishOffHungTasks()
	{
		foreach(var v in s_hung) v.Dispose(true);
	}

	public byte RunTask(int taskId, CsvTable c)
	{
		var rt = new AuTask(taskId, this);
		var p = new RParams();

		for(int i=2; i<c.RowCount; i++) {
			var v = c[i, 1];
			switch(c[i, 0]) {
			case "flags": p.flags = (RFlags)v.ToInt_(); break;
			case "name": p.name = v; break;
			case "file": p.asmFile = v; break;
			case "pdb": p.pdbOffset = v.ToInt_(); break;
			case "exe": p.exeFile = v; break;
			case "args":
				if(p.Has(RFlags.isProcess)) p.args = v;
				else p.args = Au.Util.StringMisc.CommandLineToArray(v);
				break;
			}
			//TODO: test all options
		}

		if(!rt.Run(p)) return 0;
		if(!rt.IsRunning) return 2;

		_d[taskId] = rt;
		return 1;
	}

	public byte EndTask(int taskId)
	{
		if(_d.TryGetValue(taskId, out var rt)) rt.End(false);
		return 1;
	}
}
