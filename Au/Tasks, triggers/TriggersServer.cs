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
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Microsoft.Win32.SafeHandles;

//All non-exe task processes use single low-level keyboard/mouse hook in editor process. They don't use own LL hooks because:
//	1. UAC. Editor normally is admin. Task processes can be any. Hooks of non-admin processes don't work when an admin window is active.
//	2. LL hooks are slow (read more below). For IPC we use pipe, it is fast.
//	3. In LL hook procedure some COM functions fail, eg Acc.Find in some windows.
//		Error "An outgoing call cannot be made since the application is dispatching an input-synchronous call". Like when using SendMessage for IPC.
//		That is why, even if hook is in task process, it must be in separate thread that communicates with action's thread through pipe.
//But we evaluate trigger scope (window etc) in task process, not in editor process.
//	It is much simpler, more flexible, and does not make considerably slower.
//Speed test: SendInput 10 events - 5 key down/up. Testing with cold CPU always.
//	Without LL hooks: 500 mcs (50/event, 100/key).
//	Each LL hook adds: 5000 mcs (500/event, 1000/key). Adds less when > 5 hooks, maybe because it warms up CPU.
//	To notify a task process about a trigger event, editor process uses a pipe transaction (WriteFile/ReadFile).
//	A pipe transaction then is in average 80 mcs when SendInput sends 5 keys and ~150 mcs when user presses a single key.
//	And we'll have transactions only on key-down, and only when a trigger key/mod matches.
//	So it makes slower max by 8% or 15%. Assuming: there is single LL hook in whole system; every key-down event is a trigger.
//	Almost can't notice any difference in SendInput speed.
//For window triggers we use acc hooks in task processes, not single hook in editor process.
//	Unlike LL hooks, acc hooks are fast.
//	Tested: renaming a captionless toolwindow every 1-2 ms in loop:
//		CPU usage with no acc hooks is 4%. With 1 hook - 7%. With 10 hooks in different processes - 7%.

namespace Au.Triggers
{
	/// <summary>
	/// Manages trigger engines in editor process.
	/// Communicates with multiple task processes.
	/// </summary>
	/// <remarks>
	/// Works in a separate STA thread. Trigger engines can use it for hooks.
	/// To receive data from task processes, uses a message-only window and WM_COPYDATA/WM_USER.
	/// To send data to task processes, uses pipes created by task processes.
	/// </remarks>
	class TriggersServer
	{
		unsafe class _ThreadPipe
		{
			public SafeFileHandle pipe;
			public int threadId, processId, mask;
			public int bCapacity;
			public TriggerPipeData* b; //buffer for SendAdd/Send
			public IntPtr threadHandle;

			public _ThreadPipe(SafeFileHandle pipe, int threadId, int processId, int mask)
			{
				this.pipe = pipe; this.threadId = threadId; this.processId = processId; this.mask = mask;
				threadHandle = new Util.LibKernelHandle(Api.OpenThread(Api.SYNCHRONIZE, false, threadId));
				b = (TriggerPipeData*)Util.NativeHeap.Alloc(bCapacity = 100);
			}

			public void Dispose()
			{
				pipe.Dispose();
				Api.CloseHandle(threadHandle);
				Util.NativeHeap.Free(b);
			}
		}

		Wnd.Misc.MyWindow _msgWnd;
		Thread _thread;
		List<_ThreadPipe> _pipes;
		ITriggerEngine[] _te;
		IntPtr _ev;
		bool _inTaskProcess;
		bool _addedToSend;

		public ITriggerEngine this[ETriggerType e] { get => _te[(int)e]; private set => _te[(int)e] = value; }

		public static TriggersServer Instance => s_instance;
		static TriggersServer s_instance;

		public static void Start(bool inTaskProcess) => s_instance = new TriggersServer(inTaskProcess);

		public static void Stop() => s_instance?.MsgWnd.Send(Api.WM_CLOSE);

		public TriggersServer(bool inTaskProcess)
		{
			_inTaskProcess = inTaskProcess;
			_pipes = new List<_ThreadPipe>();
			_te = new ITriggerEngine[(int)ETriggerType.ServerCount];
			this[ETriggerType.Hotkey] = new HotkeyTriggersEngine();
			this[ETriggerType.Autotext] = new AutotextTriggersEngine();
			//TODO: create mouse engines

			_thread = Thread_.Start(_Thread);
		}

		void _Thread()
		{
			string className = _inTaskProcess ? "Au.Triggers.Exe" : "Au.Triggers.Server";
			Wnd.Misc.MyWindow.RegisterClass(className);
			_msgWnd = new Wnd.Misc.MyWindow(_WndProc);
			_msgWnd.CreateMessageWindow(className);
			_ev = Api.CreateEvent(true);

			while(Api.GetMessage(out var m) > 0) Api.DispatchMessage(m);
		}

		public Thread Thread => _thread;

		public Wnd MsgWnd {
			get {
				g1:
				var w = _msgWnd?.Handle ?? default;
				if(w.Is0) { 1.ms(); goto g1; }
				return w;
			}
		}

		LPARAM _WndProc(Wnd w, int message, LPARAM wParam, LPARAM lParam)
		{
			try {
				switch(message) {
				case Api.WM_DESTROY:
					foreach(var v in _te) v.Dispose();
					foreach(var v in _pipes) v?.Dispose();
					Api.CloseHandle(_ev);
					s_instance = null; //TODO: not thread-safe
					Api.PostQuitMessage(0);
					break;
				case Api.WM_COPYDATA:
					return _WmCopyData(wParam, lParam);
				case Api.WM_USER:
					int i = (int)wParam;
					switch(i) {
					case 1:
						return _RemoveThreadTriggers((int)lParam);
					case 2:
						_RemoveTaskTriggers((int)lParam);
						break;
					}
					return 0;
				}
			}
			catch(Exception ex) { Debug_.Print(ex.Message); return default; }

			return _msgWnd.DefWndProc(w, message, wParam, lParam);
		}

		unsafe LPARAM _WmCopyData(LPARAM wParam, LPARAM lParam)
		{
			var c = new Wnd.Misc.CopyDataStruct(lParam);
			byte[] b = c.GetBytes();
			switch(c.DataId) {
			case 1:
				return _AddThreadTriggers((int)wParam, b);
			default:
				Debug.Assert(false);
				return 0;
			}
			//return 1;
		}

		unsafe int _AddThreadTriggers(int threadId, byte[] data)
		{
			var pipeName = Triggers.PipeName(threadId);
			var pipe = Api.CreateFile(pipeName, Api.GENERIC_READ | Api.GENERIC_WRITE, 0, default, Api.OPEN_EXISTING, Api.FILE_FLAG_OVERLAPPED);
			if(pipe.IsInvalid) { Debug_.LibPrintNativeError(); return 0; }

			var ms = new MemoryStream(data);
			var r = new BinaryReader(ms, Encoding.Unicode);
			int mask = r.ReadInt32();
			int processId = r.ReadInt32();
			//Print((uint)mask, data.Length);

			var tp = new _ThreadPipe(pipe, threadId, processId, mask);
			int pipeIndex = _pipes.IndexOf(null); //Print(_pipes.Count, pipeIndex);
			lock(_pipes) {
				if(pipeIndex >= 0) _pipes[pipeIndex] = tp; else { pipeIndex = _pipes.Count; _pipes.Add(tp); }
			}
			for(int i = 0; i < _te.Length; i++) if(0 != ((mask >> i) & 1)) _te[i].AddTriggers(pipeIndex, r, data);

			return 1;
		}

		int _RemoveThreadTriggers(int threadId)
		{
			int pipeIndex = _pipes.FindIndex(o => o != null && o.threadId == threadId); if(pipeIndex < 0) return 0;
			_RemovePipeTriggers(pipeIndex);
			return 1;
		}

		int _RemovePipeTriggers(int pipeIndex)
		{
			//Print(pipeIndex);
			var x = _pipes[pipeIndex];
			for(int i = 0; i < _te.Length; i++) if(0 != ((x.mask >> i) & 1)) _te[i].RemoveTriggers(pipeIndex);
			x.Dispose();
			lock(_pipes) _pipes[pipeIndex] = null;
			if(_inTaskProcess) {
				bool used = false; foreach(var v in _pipes) { if(v != null) { used = true; break; } }
				if(!used) _msgWnd.Destroy();
			}
			return 1;
		}

		void _RemoveTaskTriggers(int processId)
		{
			for(int i = _pipes.Count - 1; i >= 0; i--) {
				var k = _pipes[i]; if(k == null || k.processId != processId) continue;
				_RemovePipeTriggers(i);
			}
		}

		/// <summary>
		/// Called by RunningTasks.TaskEnded2.
		/// </summary>
		/// <param name="processId"></param>
		public void RemoveTaskTriggers(int processId)
		{
			bool hasTriggers = false;
			lock(_pipes) {
				foreach(var v in _pipes) if((v?.processId ?? 0) == processId) { hasTriggers = true; break; }
			}
			if(hasTriggers) _msgWnd.Handle.Post(Api.WM_USER, 2, processId);

			//This function is called in the main thread. All other functions that use _pipes are called in our thread.
			//	We lock _pipes only here and in functions that modify it.
		}

		public unsafe void SendBegin()
		{
			_addedToSend = false;
			foreach(var x in _pipes) {
				if(x != null) x.b->nActions = 0;
			}
		}

		public unsafe void SendAdd(int pipeIndex, int action)
		{
			var x = _pipes[pipeIndex];
			int size = sizeof(TriggerPipeData) + (x.b->nActions + 1) * 4;
			if(size > x.bCapacity) x.b = (TriggerPipeData*)Util.NativeHeap.ReAlloc(x.b, x.bCapacity *= 2);
			var a = (int*)(x.b + 1);
			a[x.b->nActions++] = action;
			_addedToSend = true;
		}

		public unsafe bool Send(ETriggerType triggerType, int data1 = 0, string data2 = null)
		{
			if(!_addedToSend) return false;
			//Perf.First('W');
			var wnd = (triggerType == ETriggerType.MouseClick || triggerType == ETriggerType.MouseWheel) ? Wnd.FromMouse(WXYFlags.NeedWindow) : Wnd.Active;
			//Perf.Next();
			for(int i = 0; i < _pipes.Count; i++) {
				Perf.First();
				var x = _pipes[i]; if(x == null) continue;
				var b = x.b; if(b->nActions == 0) continue;
				b->type = triggerType;
				b->hwnd = (int)wnd;
				b->intData = data1;
				int size = sizeof(TriggerPipeData) + b->nActions * 4;
				if(data2 != null) {
					int size2 = size + data2.Length * 2;
					if(size2 > x.bCapacity) x.b = b = (TriggerPipeData*)Util.NativeHeap.ReAlloc(b, size2 + 100);
					fixed (char* p = data2) Api.memcpy((byte*)b + size, p, data2.Length * 2);
					size = size2;
				}
				var o = new Api.OVERLAPPED { hEvent = _ev };
				if(!Api.WriteFile(x.pipe, b, size, null, &o) && !_Wait()) return false;
				Perf.Next();
				int r = 0;
				o = new Api.OVERLAPPED { hEvent = _ev };
				if(!Api.ReadFile(x.pipe, &r, 4, out _, &o) && !_Wait()) return false;
				Perf.NW();
				if(r != 0) return true;

				bool _Wait()
				{
					if(Native.GetError() != Api.ERROR_IO_PENDING) { Debug_.LibPrintNativeError(); return false; }
					var ha = stackalloc IntPtr[2] { _ev, x.threadHandle };
					for(; ; ) {
						var r1 = Api.WaitForMultipleObjectsEx(2, ha, false, Timeout.Infinite, false);
						if(r1 == 0) break;
						Api.CancelIo(x.pipe);
						if(r1 == 1) _RemovePipeTriggers(i); //probably TerminateThread
						else Debug_.LibPrintNativeError(); //WAIT_FAILED
						return false;
						//note: not _ev.WaitOne. In STA thread it gets some messages, and then hook can reenter.
					}
					return true;
				}
			}
			return false;
		}
	}
}
