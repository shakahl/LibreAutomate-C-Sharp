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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au
{
	//[DebuggerStepThrough]
	public partial class Keyb
	{

		//static void _SyncWait()
		//{
		//	//PROBLEM: keyboard hook does not work with Store apps. AttachThreadInput too. GetThreadTimes unreliable.

		//	Perf.First();
		//	var w = Wnd.WndFocused;
		//	int threadId = w.ThreadId;

		//	var b = new StringBuilder("<>");
		//	using(var ev = new EventWaitHandle(false, EventResetMode.ManualReset, "ee57812345hh")) {
		//		var hh = Cpp.Cpp_InputSync(1, threadId, default);
		//		if(hh == default) { Debug_.Print("hook: " + Native.GetErrorMessage()); goto ge; }
		//		var ht = Api.OpenThread(Api.THREAD_QUERY_LIMITED_INFORMATION, false, threadId);
		//		try {
		//			if(ht == default) { Debug_.Print("OpenThread"); goto ge; }
		//			Perf.Next();
		//			_SendKeyEventRaw(0x8F, 0, Api.KEYEVENTF_KEYUP);
		//			Perf.Next();
		//			if(!Api.GetThreadTimes(ht, out _, out _, out long tk, out long tu)) { Debug_.Print("GetThreadTimes"); goto ge; }
		//			long t0 = tk + tu;
		//			bool ok = false;
		//			for(int i = 0; i < 50; i++) {
		//				if(ok = ev.WaitOne(30)) break;
		//				if(!Api.GetThreadTimes(ht, out _, out _, out tk, out tu)) { Debug_.Print("GetThreadTimes"); goto ge; }
		//				long t1 = tk + tu, td = (t1 - t0) / 10;
		//				b.Append($"{i}={td},  ");
		//				if(td < 9000) {
		//					break;
		//				}
		//				t0 = t1;
		//			}
		//			//MessageBox.Show("");
		//			//bool ok =ev.WaitOne(500);
		//			Perf.Next();
		//			b.Append(ok ? "<c 0x8000>True</c>" : "<c 0xff>False</c>");
		//		}
		//		finally {
		//			Cpp.Cpp_InputSync(2, 0, hh);
		//			Api.CloseHandle(ht);
		//		}
		//	}

		//	Perf.NW();
		//	Print(b);
		//	return;
		//	ge:
		//	Time.Sleep(30);
		//}

		//static void _SyncWait()
		//{
		//	Perf.First();
		//	var w = Wnd.WndFocused;
		//	int threadId = w.ThreadId;

		//	using(var ev = new EventWaitHandle(false, EventResetMode.ManualReset, "ee57812345hh")) {
		//		var hh = Cpp.Cpp_InputSync(1, threadId, default);
		//		if(hh == default) { Debug_.Print("hook"); goto ge; }
		//		var ht = Api.OpenThread(Api.THREAD_QUERY_LIMITED_INFORMATION, false, threadId);
		//		try {
		//			if(ht == default) { Debug_.Print("OpenThread"); goto ge; }
		//			Perf.Next();
		//			if(!Api.GetThreadTimes(ht, out _, out _, out long tk, out long tu)) { Debug_.Print("GetThreadTimes"); goto ge; }
		//			long t0 = tk + tu;
		//			bool ok = false;
		//			for(int i = 0; i < 100; i++) {
		//				if(ok = ev.WaitOne(32)) break;
		//				if(!Api.GetThreadTimes(ht, out _, out _, out tk, out tu)) { Debug_.Print("GetThreadTimes"); goto ge; }
		//				long t1 = tk + tu;
		//				Print(i, (t1 - t0) / 10);
		//				if(t1 - t0 < 9000) break;
		//				t0 = t1;
		//			}
		//			//MessageBox.Show("");
		//			//bool ok =ev.WaitOne(500);
		//			Perf.Next();
		//			Print(ok);
		//			Perf.Next();
		//		}
		//		finally {
		//			Cpp.Cpp_InputSync(2, 0, hh);
		//			Api.CloseHandle(ht);
		//		}
		//	}

		//	Perf.NW();
		//	return;
		//	ge:
		//	Time.Sleep(30);
		//}

		//static void _SyncWait()
		//{
		//	Perf.First();
		//	var w = Wnd.WndFocused;
		//	int threadId = w.ThreadId;

		//	using(var ev = new EventWaitHandle(false, EventResetMode.ManualReset, "ee57812345hh")) {
		//		var hh = Cpp.Cpp_InputSync(1, threadId, default);
		//		if(hh == default) { Debug_.Print("hook"); goto ge; }
		//		try {
		//			Perf.Next();
		//			//MessageBox.Show("");
		//			bool ok=ev.WaitOne(500);
		//			Perf.Next();
		//			Print(ok);
		//			Perf.Next();
		//		}
		//		finally {
		//			Cpp.Cpp_InputSync(2, 0, hh);
		//		}
		//	}

		//	Perf.NW();
		//	return;
		//	ge:
		//	Time.Sleep(30);
		//}

		//static void _SyncWait()
		//{
		//	Perf.First();
		//	var w = Wnd.WndFocused;
		//	int threadId = w.ThreadId;

		//	var ht = Api.OpenThread(Api.THREAD_QUERY_LIMITED_INFORMATION, false, threadId);
		//	if(ht == default) { Debug_.Print("OpenThread"); goto ge; }
		//	try {
		//		Perf.Next();
		//		//TODO: try GetProcessTimes. Now LibreOffice does not sync, unless Sleep(32).
		//		if(!Api.GetThreadTimes(ht, out _, out _, out long tk, out long tu)) { Debug_.Print("GetThreadTimes"); goto ge; }
		//		long t0 = tk + tu;
		//		for(int i = 0; i<100; i++) {
		//			Time.Sleep(16);
		//			if(!Api.GetThreadTimes(ht, out _, out _, out tk, out tu)) { Debug_.Print("GetThreadTimes"); goto ge; }
		//			long t1 = tk + tu;
		//			Print(i, (t1 - t0) / 10);
		//			if(t1 - t0 < 9000) break;
		//			t0 = t1;
		//		}
		//	}
		//	finally {
		//		Api.CloseHandle(ht);
		//	}

		//	Perf.NW();
		//	return;
		//	ge:
		//	Time.Sleep(30);
		//}

		//static void _SyncWait()
		//{
		//	Perf.First();
		//	//var w = Wnd.WndFocused;
		//	//int threadId = w.ThreadId;
		//	//Perf.Next();
		//	using(var ts = new Util.LibThreadSwitcher()) {
		//		//TODO: the max sleep time should depend on _input._timeTextCharSent. Sleep longer if it is small, shorter if >=1.
		//		for(int i = 0; i < 10; i++) {
		//			//if(Api.GetKeyState((Keys)syncKey) < 0) break;
		//			//Time.Sleep(1);
		//			//TODO: use Sleep(0) on all CPU.
		//			//w.Send(0);
		//			if(!ts.Switch()) { Debug_.Print("Switch failed"); Time.Sleep(1); }
		//		}
		//		//TODO: measure thread CPU time. On timeout, if it is near 100%, wait more.
		//		//Also, sleep until time period elapses, not while i<50.
		//		//TODO: focus can change while waiting.
		//		//TODO: _timeTextCharSent: the integer part = simple sleep. The after-point part = sync frequency. Eg 1.25 means wait after each key and sync every 4-th. Or use two int props.
		//		//TODO: let simple sleep be on press, and sync on release. Also don't sync in middle of hotkey.
		//	}
		//	Perf.NW();
		//}

		//static void _SyncWait()
		//{
		//	//TODO: does not work with console, store. Test many window types.

		//	Perf.First();
		//	var w = Wnd.WndFocused;
		//	int threadId = w.ThreadId;
		//	Perf.Next();
		//	using(new Util.LibAttachThreadInput(threadId, out bool atiOK)) {
		//		Perf.Next();
		//		Debug_.PrintIf(!(atiOK || w.Is0 || w.IsUacAccessDenied), "AttachThreadInput failed");
		//		if(!atiOK) {
		//			Time.Sleep(10);
		//			return;
		//		}

		//		const byte syncKey = 0x8F; //TODO: allow to config
		//		_SendKeyEventRaw(syncKey, 0, 0);
		//		try {
		//			using(var ts = new Util.LibThreadSwitcher()) {
		//				Perf.Next();
		//				//TODO: the max sleep time should depend on _input._timeTextCharSent. Sleep longer if it is small, shorter if >=1.
		//				for(int i = 0; i < 10; i++) {
		//					if(Api.GetKeyState((Keys)syncKey) < 0) break;
		//					//Time.Sleep(1);
		//					//TODO: use Sleep(0) on all CPU.
		//					//w.Send(0);
		//					if(!ts.Switch()) { Debug_.Print("Switch failed"); Time.Sleep(1); }
		//				}
		//				Perf.Next();
		//				//TODO: measure thread CPU time. On timeout, if it is near 100%, wait more.
		//				//Also, sleep until time period elapses, not while i<50.
		//				//TODO: focus can change while waiting.
		//				//TODO: _timeTextCharSent: the integer part = simple sleep. The after-point part = sync frequency. Eg 1.25 means wait after each key and sync every 4-th. Or use two int props.
		//				//TODO: let simple sleep be on press, and sync on release. Also don't sync in middle of hotkey.
		//			}
		//		}
		//		finally {
		//			_SendKeyEventRaw(syncKey, 0, Api.KEYEVENTF_KEYUP);
		//			//TODO: wait for key up
		//		}
		//		Perf.NW();
		//	}
		//}

	}
}
