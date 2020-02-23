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

using Au.Types;
using static Au.AStatic;

//Triggers of all non-exe task processes/threads use single low-level keyboard/mouse hook in editor process. Not own LL hooks, because:
//	1. UAC. Editor normally is admin. Task processes can be any. Hooks of non-admin processes don't work when an admin window is active.
//	2. In LL hook procedure some COM functions fail, eg AAcc.Find in some windows. Scripts often use acc in scope context functions etc.
//		Error "An outgoing call cannot be made since the application is dispatching an input-synchronous call". Like when using SendMessage for IPC.
//		That is why, even if hook is in task process, it must be in separate thread that communicates with action's thread through pipe.
//	3. Scripts may use raw input or directX, and Windows has this bug: then low-level keyboard hook does not work in that process.
//	4. Although now slightly slower when there is 1 task process/thread using hooks, but becomes faster when there are multiple.
//We detect triggers in task process, not in editor process. It is much simpler, more flexible, and does not make considerably slower.
//	For IPC we use pipes. Unlike SendMessage, it solves the [2] problem. It is faster than SendMessage and hook IPC calls.
//	To make even faster, could do more work in hook process. More difficult. Not worth.
//For window triggers we use acc hooks in task processes, not single hook in editor process.
//	Acc hooks use less CPU for IPC than LL hooks.
//	Tested: renaming a captionless toolwindow every 1-2 ms in loop:
//		CPU usage with no acc hooks is 4%. With 1 hook - 7%. With 10 hooks in different processes - 7%.

namespace Au.Triggers
{
	/// <summary>
	/// Low-level keyboard and mouse hooks in editor process or task process.
	/// Communicates with multiple task processes/threads.
	/// </summary>
	/// <remarks>
	/// To receive data from task processes, uses a message-only window and WM_COPYDATA/WM_USER.
	/// To send data to task processes, uses pipes created by task processes/threads.
	/// </remarks>
	class HooksServer
	{
		class _ThreadPipe
		{
			public LibHandle pipe;
			public int threadId, processId;
			public UsedEvents usedEvents;
			public LibHandle threadHandle;

			public _ThreadPipe(LibHandle pipe, int threadId, int processId, UsedEvents usedEvents)
			{
				this.pipe = pipe; this.threadId = threadId; this.processId = processId; this.usedEvents = usedEvents;
				threadHandle = Api.OpenThread(Api.SYNCHRONIZE, false, threadId);
			}

			public void Dispose()
			{
				pipe.Dispose();
				threadHandle.Dispose();
			}
		}

		[Flags]
		public enum UsedEvents
		{
			Keyboard = 1, //Hotkey and Autotext triggers
			Mouse = 2, //Mouse and Autotext triggers. Just sets the hook and resets autotext; to receive events, also add other MouseX flags.
			MouseClick = 0x10, //Mouse click triggers
			MouseWheel = 0x20, //Mouse wheel triggers
			MouseEdgeMove = 0x40, //Mouse edge and move triggers
		}

		AWnd _msgWnd;
		readonly Thread _thread;
		readonly List<_ThreadPipe> _pipes;
		AHookWin _hookK, _hookM;
		LibHandle _event;
		bool _inTaskProcess;
		MouseTriggers.LibEdgeMoveDetector _emDetector;

		public static HooksServer Instance => s_instance;
		static HooksServer s_instance;

		public static void Start(bool inTaskProcess) => s_instance = new HooksServer(inTaskProcess);

		public static void Stop() => s_instance?.MsgWnd.Send(Api.WM_CLOSE);

		public HooksServer(bool inTaskProcess)
		{
			_inTaskProcess = inTaskProcess;
			_pipes = new List<_ThreadPipe>();
			_event = Api.CreateEvent(true);
			_thread = AThread.Start(_Thread);
		}

		void _Thread()
		{
			string cn = _inTaskProcess ? "Au.Hooks.Exe" : "Au.Hooks.Server";
			AWnd.More.RegisterWindowClass(cn, _WndProc);
			_msgWnd = AWnd.More.CreateMessageOnlyWindow(cn);
			Api.SetEvent(_event);
			if(_inTaskProcess) AProcess.Exit += (unu, sed) => _msgWnd.SendTimeout(1000, Api.WM_CLOSE); //unhook etc

			//if(!Util.LibAssembly.LibIsAuNgened) {
				Util.AJit.Compile(typeof(HooksServer), "_Send", "_KeyboardHookProc", "_MouseHookProc");
				Util.AJit.Compile(typeof(Api), "WriteFile", "ReadFile", "WaitForMultipleObjectsEx");
				_ = ATime.PerfMilliseconds;
			//}

			while(Api.GetMessage(out var m) > 0) Api.DispatchMessage(m);
		}

		public Thread Thread => _thread;

		public AWnd MsgWnd {
			get {
				if(_msgWnd.Is0) Api.WaitForSingleObject(_event, -1);
				return _msgWnd;
			}
		}

		LPARAM _WndProc(AWnd w, int message, LPARAM wParam, LPARAM lParam)
		{
			try {
				switch(message) {
				case Api.WM_DESTROY:
					_hookK?.Dispose();
					_hookM?.Dispose();
					_emDetector = null;
					foreach(var v in _pipes) v?.Dispose();
					_event.Dispose();
					s_instance = null;
					Api.PostQuitMessage(0);
					break;
				case Api.WM_COPYDATA:
					return _WmCopyData(wParam, lParam);
				case Api.WM_USER:
					int i = (int)wParam;
					switch(i) {
					case 1:
						return _RemoveThread((int)lParam);
					case 2:
						_RemoveTask((int)lParam);
						break;
					}
					return 0;
				}
			}
			catch(Exception ex) { ADebug.Print(ex.Message); return default; }

			return Api.DefWindowProc(w, message, wParam, lParam);
		}

		unsafe LPARAM _WmCopyData(LPARAM wParam, LPARAM lParam)
		{
			var c = new AWnd.More.CopyDataStruct(lParam);
			byte[] b = c.GetBytes();
			switch(c.DataId) {
			case 1:
				return _AddThread((int)wParam, b);
			default:
				Debug.Assert(false);
				return 0;
			}
			//return 1;
		}

		unsafe int _AddThread(int threadId, byte[] data)
		{
			var pipeName = ActionTriggers.LibPipeName(threadId);
			LibHandle pipe = Api.CreateFile(pipeName, Api.GENERIC_READ | Api.GENERIC_WRITE, 0, default, Api.OPEN_EXISTING, Api.FILE_FLAG_OVERLAPPED);
			if(pipe.Is0) { ADebug.LibPrintNativeError(); return 0; }

			var usedEvents = (UsedEvents)data.ReadInt(0);
			int processId = data.ReadInt(4);

			var tp = new _ThreadPipe(pipe, threadId, processId, usedEvents);
			lock(_pipes) {
				int pi = _pipes.IndexOf(null); //Print(_pipes.Count, pi);
				if(pi >= 0) _pipes[pi] = tp; else _pipes.Add(tp);
			}

			if(0 != (usedEvents & UsedEvents.Keyboard) && _hookK == null) {
				_hookK = AHookWin.Keyboard(_KeyboardHookProc); //note: don't use lambda, because then very slow JIT on first hook event
			}
			if(0 != (usedEvents & UsedEvents.Mouse) && _hookM == null) {
				_hookM = AHookWin.LibMouseRaw(_MouseHookProc);
			}
			if(0 != (usedEvents & UsedEvents.MouseEdgeMove) && _emDetector == null) {
				_emDetector = new MouseTriggers.LibEdgeMoveDetector();
			}

			return 1;
		}

		unsafe void _KeyboardHookProc(HookData.Keyboard k)
		{
			//var p = APerf.Create();
			var d = new Api.KBDLLHOOKSTRUCT2(k.LibNativeStructPtr);
			if(_Send(UsedEvents.Keyboard, &d, sizeof(Api.KBDLLHOOKSTRUCT2))) k.BlockEvent();
			//p.NW();
		}

		unsafe bool _MouseHookProc(LPARAM wParam, LPARAM lParam)
		{
			int msg = (int)wParam;
			if(msg == Api.WM_MOUSEMOVE) {
				var mll = (Api.MSLLHOOKSTRUCT*)lParam;
				if(_emDetector?.Detect(mll->pt) ?? false) {
					MouseTriggers.LibEdgeMoveDetector.Result d = _emDetector.result;
					_Send(UsedEvents.MouseEdgeMove, &d, sizeof(MouseTriggers.LibEdgeMoveDetector.Result));
				}
				return false;
			}

			var m = new Api.MSLLHOOKSTRUCT2(wParam, lParam);
			var eventType = m.IsWheel ? UsedEvents.MouseWheel : UsedEvents.MouseClick;
			return _Send(eventType, &m, sizeof(Api.MSLLHOOKSTRUCT2));
		}

		int _RemoveThread(int threadId)
		{
			int pipeIndex = _pipes.FindIndex(o => o != null && o.threadId == threadId); if(pipeIndex < 0) return 0;
			_RemovePipe(pipeIndex);
			return 1;
		}

		int _RemovePipe(int pipeIndex)
		{
			//Print(pipeIndex);
			_pipes[pipeIndex].Dispose();
			lock(_pipes) _pipes[pipeIndex] = null;
			return 1;
		}

		void _RemoveTask(int processId)
		{
			for(int i = _pipes.Count - 1; i >= 0; i--) {
				var k = _pipes[i]; if(k == null || k.processId != processId) continue;
				_RemovePipe(i);
			}
		}

		/// <summary>
		/// Called by RunningTasks.TaskEnded2.
		/// </summary>
		/// <param name="processId"></param>
		public void RemoveTask(int processId)
		{
			bool has = false;
			lock(_pipes) {
				foreach(var v in _pipes) if((v?.processId ?? 0) == processId) { has = true; break; }
			}
			if(has) _msgWnd.Post(Api.WM_USER, 2, processId);

			//This function is called in the main thread. All other functions that use _pipes are called in our thread.
			//	We lock _pipes only here and in functions that modify it.
		}

		unsafe bool _Send(UsedEvents eventType, void* data, int size)
		{
			for(int i = 0; i < _pipes.Count; i++) {
				var x = _pipes[i]; if(x == null) continue;
				if(0 == (x.usedEvents & eventType)) continue;

				var ha = stackalloc IntPtr[2] { _event, x.threadHandle };
				int r = 0;
				bool read = false;
				g1: //we use this weird goto code instead of a nested _Wait function to make faster JIT
				var o = new Api.OVERLAPPED { hEvent = _event };
				if(!(read ? Api.ReadFile(x.pipe, &r, 1, out _, &o) : Api.WriteFile(x.pipe, data, size, out _, &o))) {
					if(ALastError.Code != Api.ERROR_IO_PENDING) { ADebug.LibPrintNativeError(); return false; }
					for(; ; ) {
						var r1 = Api.WaitForMultipleObjectsEx(2, ha, false, Timeout.Infinite, false);
						if(r1 == 0) break;
						Api.CancelIo(x.pipe);
						if(r1 == 1) _RemovePipe(i); //probably TerminateThread
						else ADebug.LibPrintNativeError(); //WAIT_FAILED
						return false;
						//note: not _ev.WaitOne. In STA thread it gets some messages, and then hook can reenter.
					}
				}
				if(!read) { read = true; goto g1; }
				if(r != 0) return true;
			}
			return false;
		}
	}
}
