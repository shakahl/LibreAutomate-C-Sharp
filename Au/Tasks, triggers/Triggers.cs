using System;
using System.Collections.Generic;
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
//using System.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using Microsoft.Win32.SafeHandles;

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	/// <summary>
	/// Triggers.
	/// </summary>
	[CompilerGenerated()]
	class NamespaceDoc
	{
		//SHFB uses this for namespace documentation.
	}

	public class Triggers
	{
		ITriggers[] _t = new ITriggers[(int)EType.Count];
		//ITriggers this[EType e] { get => _t[(int)e]; set => _t[(int)e] = value; }
		ITriggers this[EType e] => _t[(int)e];

		Triggers() { }

		static Triggers _Instance => t_instance ?? (t_instance = new Triggers());
		[ThreadStatic] static Triggers t_instance;

		public static Scopes Of => _Instance._scope ?? (_Instance._scope = new Scopes());
		Scopes _scope;

		ITriggers _Get(EType e)
		{
			int i = (int)e;
			//return (_t[i] ?? (_t[i] = new T())) as T; //error if T ctor is internal. But I cannot make it public. Another way - Activator.Create instance, but probably slow.
			var t = _t[i];
			if(t == null) {
				switch(e) {
				case EType.Hotkey: t = new HotkeyTriggers(); break;
				case EType.Autotext: t = new AutotextTriggers(); break;
				case EType.WindowCreated: t = new WindowCreatedTriggers(); break;
				default: Debug.Assert(false); break;
				}
				_t[i] = t;
			}
			return t;
		}

		public static HotkeyTriggers Hotkey => _Instance._Get(EType.Hotkey) as HotkeyTriggers;

		public static AutotextTriggers Autotext => _Instance._Get(EType.Autotext) as AutotextTriggers;

		public static WindowCreatedTriggers WindowCreated => _Instance._Get(EType.WindowCreated) as WindowCreatedTriggers;

		private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			var a = args.LoadedAssembly;
			Print(a);
			if(a.FullName.StartsWith_("System.Drawing")) {
				//Print(new StackTrace());
				Debugger.Launch();
			}
		}

		public static void Run() => _Instance._Run();
		unsafe void _Run()
		{
			AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;


			bool haveTriggers = false; int remoteMask = 0;
			MemoryStream ms = null; BinaryWriter wr = null;
			for(int i = 0; i < (int)EType.Count; i++) {
				var t = _t[i]; if(t == null) continue;
				var ep = t.EngineProcess; if(ep == EEngineProcess.None) continue;
				if(ep == EEngineProcess.Remote) {
					remoteMask |= 1 << i;
					if(ms == null) {
						ms = new MemoryStream();
						wr = new BinaryWriter(ms, Encoding.Unicode);
						wr.Write(0); //reserve for remoteMask
						wr.Write(Api.GetCurrentProcessId());
					}
					t.Write(wr);
				}
				haveTriggers = true;
			}
			//Print((uint)mask);
			if(!haveTriggers) return;
			if(remoteMask != 0) {
				ms.Position = 0; wr.Write(remoteMask);
			}

			Wnd wMsg = default;
			bool useEditor = AuTask.Role != ATRole.ExeProgram;
			if(useEditor) {
				wMsg = Api.FindWindow("Au.Triggers.Server", null);
				if(wMsg.Is0) { Debug_.Print("Editor process not running."); useEditor = false; }
				//TODO: if this process is admin, and editor isn't, useEditor=false.
				//TODO: can pass wMsg when starting task.
			}
			if(!useEditor) {
				lock(this) {
					if(TriggersServer.Instance == null) TriggersServer.Start(true);
				}
				wMsg = TriggersServer.Instance.MsgWnd;
			}

			int threadId = Api.GetCurrentThreadId();
			string pipeName = PipeName(threadId);
			var pipe = Api.CreateNamedPipe(pipeName,
				Api.PIPE_ACCESS_DUPLEX | Api.FILE_FLAG_OVERLAPPED,
				Api.PIPE_TYPE_MESSAGE | Api.PIPE_READMODE_MESSAGE | Api.PIPE_REJECT_REMOTE_CLIENTS,
				1, 0, 0, 0, Api.SECURITY_ATTRIBUTES.ForPipes);
			if(pipe.IsInvalid) throw new AuException(0, "*CreateNamedPipe");
			var ev = Api.CreateEvent(true);
			_evEnd = Api.CreateEvent(false);
			var ha = new IntPtr[2] { ev, _evEnd };
			byte* b = null;
			try {
				if(1 != Wnd.Misc.CopyDataStruct.SendBytes(wMsg, 1, ms.ToArray(), threadId)) { //_AddThreadTriggers(threadId, data)
					threadId = 0;
					throw new AuException("*SendBytes");
				}

				int bLen = 4000;
				b = (byte*)Util.NativeHeap.Alloc(bLen); //buffer for ReadFile
				var wndCache = new WFCache(true); //to make faster Wnd.Finder.IsMatch() when called multiple times for same HWND
				while(true) {
					var o = new Api.OVERLAPPED { hEvent = ev };
					if(!Api.ReadFile(pipe, b, bLen, out int size, &o)) {
						int ec = Native.GetError();
						if(ec == Api.ERROR_IO_PENDING) {
							//note: while waiting here, acc hook proc may be called. Also timer etc (any posted and sent messages).
							if(0 != WaitFor.LibWait(Timeout.Infinite, WHFlags.DoEvents, ha)) { //with WaitHandle.WaitAny cannot use MTA thread, timers, etc
								Api.CancelIo(pipe);
								break;
							}
							ec = Api.GetOverlappedResult(pipe, ref o, out size, false) ? 0 : Native.GetError(); //info: ERROR_MORE_DATA if b too small
						}
						for(; ec == Api.ERROR_MORE_DATA; ec = Native.GetError()) {
							b = (byte*)Util.NativeHeap.ReAlloc(b, bLen *= 2);
							bool r1 = Api.ReadFile(pipe, b + size, bLen - size, out int nr2);
							size += nr2;
							if(r1) { ec = 0; break; }
						}
						if(ec != 0) { Debug_.LibPrintNativeError(ec); break; }
					}
					var h = (IpcPipeData*)b;
					int nActions = h->nActions;
					var wnd = (Wnd)h->hwnd;
					int data1 = h->intData;
					int data2Offset = sizeof(IpcPipeData) + (nActions * 4);
					Debug.Assert(size >= data2Offset);
					string data2 = size == data2Offset ? null : new string((char*)(b + data2Offset), 0, (size - data2Offset) / 2);
					var t = this[h->type];
					int r = 0, action = 0;
					int* actions = (int*)(h + 1);
					for(int i = 0; i < nActions; i++) {
						action = actions[i];
						//1500.ms(); //TODO
						if(t.CanRun(action, data1, data2, wnd, wndCache)) { r = i + 1; break; }
					}
					Api.WriteFile(pipe, &r, 4);
					if(r != 0) {
						Task.Run(() => t.Run(action, data1, data2, wnd));
						//t.Run(action, data1, data2, wnd);
					}
				}
			}
			finally {
				Api.CloseHandle(_evEnd); _evEnd = default;
				Api.CloseHandle(ev);
				pipe.Dispose();
				Util.NativeHeap.Free(b);
				if(threadId != 0) wMsg.Send(Api.WM_USER, 1, threadId); //_RemoveThreadTriggers(threadId)
			}
		}

		internal static string PipeName(int threadId) => @"\\.\pipe\Au.Triggers-" + threadId.ToString();

		public static void EndRun(bool clear = false)
		{
			var ti = t_instance; if(ti == null) return;
			if(ti._evEnd != default) Api.SetEvent(ti._evEnd);
			if(clear) t_instance = null;
		}
		IntPtr _evEnd;
	}

	enum EType
	{
		Hotkey,
		Autotext,
		WindowCreated,
		Count,

		//future
		MouseEdge,
		MouseArea,
		MouseClick,
		MouseWheel,
	}

	enum EEngineProcess
	{
		None, //no triggers
		Local, //in task process
		Remote, //in editor process
	}

	interface ITriggers
	{
		EEngineProcess EngineProcess { get; }
		void Write(BinaryWriter w);
		bool CanRun(int action, int data1, string data2, Wnd w, WFCache cache);
		void Run(int action, int data1, string data2, Wnd w);
	}

	interface ITriggersEngine : IDisposable
	{
		void AddTriggers(int pipe, BinaryReader r, byte[] raw);
		void RemoveTriggers(int pipe);
	}

	/// <summary>
	/// Base of trigger action argument classes of all trigger types. For example HotkeyTriggerArgs.
	/// </summary>
	public class TriggerArgs
	{

	}

	struct IpcPipeData
	{
		public int nActions;
		public EType type;
		public int hwnd;
		public int intData;
	}

	public class Scopes
	{
		class _Scope
		{
			public object o; //Wnd.Finder, Wnd, Wnd.Finder[], Wnd[], Func<bool>
			public int andScope; //a window scope to run after (if >0) or before (if <0) Func
			public bool not;
			public byte warningCounter; //for long execution time warning
			public uint warningTime; //for long execution time warning
		}

		List<_Scope> _a;
		int _index; //0 or _aw index + 1

		internal int Index => _index;

		internal Scopes() { }

		public void Reset(int index = 0) => _index = (uint)index <= (_a?.Count ?? 0) ? index : throw new ArgumentOutOfRangeException();

		public int Window(string name = null, string className = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null)
			=> _Window(false, name, className, program, also, contains);

		public int NotWindow(string name = null, string className = null, WF3 program = default, Func<Wnd, bool> also = null, object contains = null)
			=> _Window(true, name, className, program, also, contains);

		int _Window(bool not, string name, string className, WF3 program, Func<Wnd, bool> also, object contains)
			=> _Add(not, new Wnd.Finder(name, className, program, 0, also, contains));

		public int Window(Wnd.Finder f)
			=> _Add(false, f);

		public int NotWindow(Wnd.Finder f)
			=> _Add(true, f);

		public int Windows(params Wnd.Finder[] any)
			=> _Add(false, any);

		public int NotWindows(params Wnd.Finder[] any)
			=> _Add(true, any);

		public void Window(Wnd w)
			=> _Add(false, w);

		public int NotWindow(Wnd w)
			=> _Add(true, w);

		public int Windows(params Wnd[] any)
			=> _Add(false, any);

		public int NotWindows(params Wnd[] any)
			=> _Add(true, any);

		public int Context(Func<bool> f)
			=> _Add(false, f);

		public int Context(Func<bool> f, int andWindow, bool windowFirst)
		{
			if((uint)andWindow > _a.Count) throw new ArgumentOutOfRangeException();
			int i = _Add(false, f);
			_a[i - 1].andScope = windowFirst ? -andWindow : andWindow;
			return i;
		}

		int _Add(bool not, object o)
		{
			switch(o) {
			case Wnd w:
				w.ThrowIf0();
				break;
			case Wnd[] any:
				foreach(var w in any) w.ThrowIf0();
				break;
			case Wnd.Finder[] any:
				foreach(var f in any) if(f == null) throw new ArgumentNullException();
				break;
			case null:
				throw new ArgumentNullException();
			}

			if(_a == null) _a = new List<_Scope>();
			_a.Add(new _Scope { not = not, o = o });
			return _index = _a.Count;
		}

		/// <summary>
		/// Returns true if index==0 or w matches the scope.
		/// </summary>
		/// <param name="index">0 or 1-based index of window scope.</param>
		/// <param name="w"></param>
		/// <param name="cache"></param>
		internal bool Match(int index, Wnd w, WFCache cache)
		{
			if(index == 0) return true;
			var x = _a[index - 1];
			if(w.Is0) return x.not;
			bool yes = false;
			int andScope = 0;
			long tLong = 0;
			switch(x.o) {
			case Wnd.Finder f:
				tLong = Time.Milliseconds;
				yes = f.IsMatch(w, cache);
				break;
			case Wnd.Finder[] af:
				tLong = Time.Milliseconds;
				foreach(var v in af) if(yes = v.IsMatch(w, cache)) break;
				break;
			case Wnd hwnd:
				yes = w == hwnd;
				break;
			case Wnd[] ahwnd:
				foreach(var v in ahwnd) if(yes = (w == v)) break;
				break;
			case Func<bool> ff:
				if(x.andScope < 0 && !Match(-x.andScope, w, cache)) return false;
				tLong = Time.Milliseconds;
				yes = ff();
				if(yes) andScope = x.andScope;
				break;
			}

			if(tLong != 0) {
				tLong = Time.Milliseconds - tLong;
				uint tInt = (uint)Math.Min(tLong, 100_000_000), count = x.warningCounter++, avg, max;
				if(count == 0) { //first time can be slow JIT etc
					avg = tInt; //don't add t1 to x.warningTime first time
					max = 250;
					//note: if a hook proc takes by default 300 ms, Windows ignores its return value and passes the event to other hooks and apps.
				} else {
					avg = (x.warningTime += tInt) / count;
					max = 80 / count + 20;
				}
				//Print($"{x.warningCounter}, t={tInt}, avg={avg}, max={max}"); //max 250, 100 ... 25, 30 ... 25, 30 ... 25, ...
				if(avg > max) {
					var th = index == 1 ? "st" : (index == 2 ? "nd" : "th");
					PrintWarning($"The {index}-{th} 'Triggers.Of' statement is too slow, in average {avg} ms. It slows down the trigger.");
				}
				if(x.warningCounter >= 16) { x.warningCounter = 8; x.warningTime = avg * 8; }
			}

			if(andScope > 0) yes = Match(x.andScope, w, cache);
			return yes != x.not;
		}
	}

	public static class TaskTrigger
	{
		[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
		public class HotkeyAttribute : Attribute
		{
			public string Hotkey { get; }

			public HotkeyAttribute(string hotkey)
			{
				Hotkey = hotkey;
			}
		}

	}
}
