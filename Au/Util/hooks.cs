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
using Au.Util;
using static Au.NoClass;

namespace Au.Util
{
	/// <summary>
	/// Helps with windows hooks. See <msdn>SetWindowsHookEx</msdn>.
	/// </summary>
	/// <remarks>
	/// The thread that uses hooks must process Windows messages. For example have a window/dialog/messagebox, or use a 'wait-for' function that dispatches messages or has such option (see <see cref="Opt.WaitFor"/>).
	/// The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Uninstall</b> in the same thread) or with the 'using' pattern. Else this process may crash.
	/// </remarks>
	public class WinHook : IDisposable
	{
		IntPtr _hh; //HHOOK
		Api.HOOKPROC _proc1; //our intermediate dispatcher hook proc that calls _proc2
		Delegate _proc2; //caller's hook proc

		/// <summary>
		/// Installs a low-level keyboard hook (WH_KEYBOARD_LL).
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible, or the system hangs and removes the hook.
		/// If returns true or calls <see cref="HookData.ReplyMessage"/>, the event is cancelled (not visible to apps and other hooks). Event data cannot be modified.
		/// </param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// var stop = false;
		/// using(WinHook.Keyboard(x =>
		/// {
		/// 	Print(x);
		/// 	if(x.vkCode == KKey.Escape) { stop = true; return true; } //return true to suppress the event
		/// 	return false;
		/// })) {
		/// 	MessageBox.Show("Low-level keyboard hook.", "Test");
		/// 	//or
		/// 	//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for Esc key
		/// 	//Print("the end");
		/// }
		/// ]]></code>
		/// </example>
		public static WinHook Keyboard(Func<HookData.Keyboard, bool> hookProc)
			=> new WinHook(Api.WH_KEYBOARD_LL, hookProc, -1);

		/// <summary>
		/// Installs a low-level mouse hook (WH_MOUSE_LL).
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible, or the system hangs and removes the hook.
		/// If returns true or calls <see cref="HookData.ReplyMessage"/>, the event is cancelled (not visible to apps and other hooks). Event data cannot be modified.
		/// </param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// var stop = false;
		/// using(WinHook.Mouse(x =>
		/// {
		/// 	Print(x);
		/// 	if(x.Event == HookData.MouseEvent.RightButton) { stop = x.IsButtonUp; return true; } //return true to suppress the event
		/// 	return false;
		/// })) {
		/// 	MessageBox.Show("Low-level mouse hook.", "Test");
		/// 	//or
		/// 	//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for right-click
		/// 	//Print("the end");
		/// }
		/// ]]></code>
		/// </example>
		public static WinHook Mouse(Func<HookData.Mouse, bool> hookProc)
			=> new WinHook(Api.WH_MOUSE_LL, hookProc, -1);

		/// <summary>
		/// Installs a WH_CBT hook for a thread of this process.
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible.
		/// If returns true, the event is cancelled. For some events you can modify some fields of event data.
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadCbt(x =>
		/// {
		/// 	Print(x.code);
		/// 	switch(x.code) {
		/// 	case HookData.CbtEvent.ACTIVATE:
		/// 		Print(x.ActivationInfo(out _, out _));
		/// 		break;
		/// 	case HookData.CbtEvent.CREATEWND:
		/// 		Print(x.CreationInfo(out var c, out _), c->x, c->lpszName);
		/// 		break;
		/// 	case HookData.CbtEvent.CLICKSKIPPED:
		/// 		Print(x.MouseInfo(out var m), m->pt, m->hwnd);
		/// 		break;
		/// 	case HookData.CbtEvent.KEYSKIPPED:
		/// 		Print(x.KeyInfo(out _));
		/// 		break;
		/// 	case HookData.CbtEvent.SETFOCUS:
		/// 		Print(x.FocusInfo(out Wnd wPrev), wPrev);
		/// 		break;
		/// 	case HookData.CbtEvent.MOVESIZE:
		/// 		Print(x.MoveSizeInfo(out var r), r->ToString());
		/// 		break;
		/// 	case HookData.CbtEvent.MINMAX:
		/// 		Print(x.MinMaxInfo(out var state), state);
		/// 		break;
		/// 	case HookData.CbtEvent.DESTROYWND:
		/// 		Print((Wnd)x.wParam);
		/// 		break;
		/// 	}
		/// 	return false;
		/// })) {
		/// 	MessageBox.Show("CBT hook.", "Test", MessageBoxButtons.OKCancel);
		/// 	//new Form().ShowDialog(); //to test MINMAX
		/// }
		/// ]]></code>
		/// </example>
		public static WinHook ThreadCbt(Func<HookData.ThreadCbt, bool> hookProc, int threadId = 0)
			=> new WinHook(Api.WH_CBT, hookProc, threadId);

		/// <summary>
		/// Installs a WH_GETMESSAGE hook for a thread of this process.
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible.
		/// The event cannot be cancelled. As a workaround, you can set msg->message=0. Also can modify other fields.
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadGetMessage(x =>
		/// {
		/// 	Print(x.msg->ToString(), x.PM_NOREMOVE);
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadGetMessage(Action<HookData.ThreadGetMessage> hookProc, int threadId = 0)
			=> new WinHook(Api.WH_GETMESSAGE, hookProc, threadId);

		/// <summary>
		/// Installs a WH_GETMESSAGE hook for a thread of this process.
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible.
		/// If returns true, the event is cancelled.
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadKeyboard(x =>
		/// {
		/// 	Print(x.key, 0 != (x.lParam & 0x80000000) ? "up" : "", x.lParam, x.PM_NOREMOVE);
		/// 	return false;
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadKeyboard(Func<HookData.ThreadKeyboard, bool> hookProc, int threadId = 0)
			=> new WinHook(Api.WH_KEYBOARD, hookProc, threadId);

		/// <summary>
		/// Installs a WH_MOUSE hook for a thread of this process.
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible.
		/// If returns true, the event is cancelled.
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadMouse(x =>
		/// {
		/// 	Print(x.message, x.m->pt, x.m->hwnd, x.PM_NOREMOVE);
		/// 	return false;
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadMouse(Func<HookData.ThreadMouse, bool> hookProc, int threadId = 0)
			=> new WinHook(Api.WH_MOUSE, hookProc, threadId);

		/// <summary>
		/// Installs a WH_CALLWNDPROC hook for a thread of this process.
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible.
		/// The event cannot be cancelled or modified.
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadCallWndProc(x =>
		/// {
		/// 	ref var m = ref *x.msg;
		/// 	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam);
		/// 	Print(mm, x.sentByOtherThread);
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadCallWndProc(Action<HookData.ThreadCallWndProc> hookProc, int threadId = 0)
			=> new WinHook(Api.WH_CALLWNDPROC, hookProc, threadId);

		/// <summary>
		/// Installs a WH_CALLWNDPROCRET hook for a thread of this process.
		/// See <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <param name="hookProc">
		/// Hook procedure.
		/// Must return as soon as possible.
		/// The event cannot be cancelled or modified.
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadCallWndProcRet(x =>
		/// {
		/// 	ref var m = ref *x.msg;
		/// 	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam); mm.Result = m.lResult;
		/// 	Print(mm, x.sentByOtherThread);
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadCallWndProcRet(Action<HookData.ThreadCallWndProcRet> hookProc, int threadId = 0)
			=> new WinHook(Api.WH_CALLWNDPROCRET, hookProc, threadId);

		/// <summary>
		/// Installs hook of the specified type.
		/// This ctor is private, because our dispatcher hook procedure does not know how to call hookProc.
		/// </summary>
		/// <param name="hookType">One of WH_ constants that are used with API <msdn>SetWindowsHookEx</msdn>.</param>
		/// <param name="hookProc">Delegate of the hook procedure of correct type.</param>
		/// <param name="tid">Thread id or: 0 (default) this thread, -1 global hook.</param>
		/// <exception cref="AuException">Failed.</exception>
		WinHook(int hookType, Delegate hookProc, int tid = 0)
		{
			if(tid == 0) tid = Api.GetCurrentThreadId(); else if(tid == -1) tid = 0;
			_hh = Api.SetWindowsHookEx(hookType, _proc1 = _HookProc, default, tid);
			if(_hh == default) throw new AuException(0, "*set hook");
			_proc2 = hookProc;
		}

		/// <summary>
		/// Uninstalls the hook if installed.
		/// </summary>
		public void Unhook()
		{
			if(_hh != default) {
				bool ok = Api.UnhookWindowsHookEx(_hh);
				if(!ok) PrintWarning($"Failed to unhook WinHook ({_HookTypeStr}). {Native.GetErrorMessage()}");
				_hh = default;
				_proc2 = null;
			}
		}

		///// <summary>
		///// Don't print warning "Non-disposed WinHook variable".
		///// </summary>
		//public bool NoWarningNondisposed { get; set; }

		/// <summary>
		/// Calls <see cref="Unhook"/>.
		/// </summary>
		public void Dispose() { Unhook(); GC.SuppressFinalize(this); }

		///
		~WinHook()
		{
			//unhooking in finalizer thread makes no sense. Must unhook in same thread, else fails.
			//if(_hh != default && !NoWarningNondisposed) PrintWarning($"Non-disposed WinHook ({_HookTypeStr}) variable."); //rejected. Eg when called Environment.Exit, finalizers are executed but finally code blocks not.
			Debug_.PrintIf(_hh != default, $"Non-disposed WinHook ({_HookTypeStr}) variable.");
		}

		string _HookTypeStr => _proc2.GetType().GenericTypeArguments[0].Name;

		LPARAM _HookProc(int code, LPARAM wParam, LPARAM lParam)
		{
			try {
				if(code >= 0) {
					bool R = false;
					long t1 = 0; int hookType = 0;

					switch(_proc2) {
					case Func<HookData.Keyboard, bool> p:
						t1 = Time.PerfMilliseconds; hookType = Api.WH_KEYBOARD_LL;
						R = p(new HookData.Keyboard(this, lParam)); //info: wParam is message, but it is not useful, everything is in lParam
						break;
					case Func<HookData.Mouse, bool> p:
						t1 = Time.PerfMilliseconds; hookType = Api.WH_MOUSE_LL;
						R = p(new HookData.Mouse(this, wParam, lParam));
						break;
					case Func<HookData.ThreadCbt, bool> p:
						R = p(new HookData.ThreadCbt(this, code, wParam, lParam));
						break;
					case Action<HookData.ThreadGetMessage> p:
						p(new HookData.ThreadGetMessage(this, wParam, lParam));
						break;
					case Func<HookData.ThreadKeyboard, bool> p:
						R = p(new HookData.ThreadKeyboard(this, code, wParam, lParam));
						break;
					case Func<HookData.ThreadMouse, bool> p:
						R = p(new HookData.ThreadMouse(this, code, wParam, lParam));
						break;
					case Action<HookData.ThreadCallWndProc> p:
						p(new HookData.ThreadCallWndProc(this, wParam, lParam));
						break;
					case Action<HookData.ThreadCallWndProcRet> p:
						p(new HookData.ThreadCallWndProcRet(this, wParam, lParam));
						break;
					}

					//Prevent Windows disabling the low-level key/mouse hook.
					//	Hook proc must return in HKEY_CURRENT_USER\Control Panel\Desktop:LowLevelHooksTimeout ms.
					//		Default 300. On Win10 max 1000 (bigger registry value is ignored and used 1000).
					//	On timeout Windows:
					//		1. Does not wait more. Passes the event to the next hook etc, and we cannot return 1 to block the event.
					//		2. Kills the hook after several such cases. Usually 6 keys or 11 mouse events.
					//		3. Makes the hook useless: next times does not wait for it, and we cannot return 1 to block the event.
					//	Somehow does not apply 2 and 3 to some apps, eg C# apps created by Visual Studio, although applies to those created not by VS. I did not find why.
					if(hookType != 0 && (t1 = Time.PerfMilliseconds - t1) > 200 /*&& t1 < 5000*/ && !Debugger.IsAttached) {
						if(t1 > LowLevelHooksTimeout - 50) {
							var s1 = hookType == Api.WH_KEYBOARD_LL ? "keyboard" : "mouse";
							var s2 = R ? $" On timeout the {s1} event is passed to the active window, other hooks, etc." : null;
							//PrintWarning($"Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}"); //too slow first time
							//Print($"Warning: Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}\r\n{new StackTrace(0, false)}"); //first Print JIT 30 ms
							ThreadPool.QueueUserWorkItem(s3 => Print(s3), $"Warning: Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}\r\n{new StackTrace(0, false)}"); //fast if with false. But async print can be confusing.
						}
						//FUTURE: print warning if t1 is >25 frequently. Unhook and don't rehook if >LowLevelHooksTimeout-50 frequently.

						Api.UnhookWindowsHookEx(_hh);
						_hh = Api.SetWindowsHookEx(hookType, _proc1, default, 0);
					}

					if(R) return 1;
				}
			}
			catch(Exception ex) { if(LibOnException(ex, this)) return 0; }
			//info: on any exception .NET would terminate process, even on ThreadAbortException.
			//	This prevents it when using eg AuDialog. But not when eg MessageBox.Show; I don't know how to prevent it.

			return Api.CallNextHookEx(default, code, wParam, lParam);
		}

		/// <summary>
		/// Gets the max time in milliseconds allowed by Windows for low-level keyboard and mouse hook procedures.
		/// </summary>
		/// <remarks>
		/// Gets registry value HKEY_CURRENT_USER\Control Panel\Desktop:LowLevelHooksTimeout.
		/// If the registry value is missing, returns 300, because it is the default value used by Windows. If the registry value is more than 1000, returns 1000, because Windows 10 ignores bigger values.
		/// If a hook procedure takes more time, Windows does not wait. Then the return value is ignored, and the event is passed to the active window, other hooks, etc. Also Windows may fully or partially disable the hook.
		/// More info: <google>registry LowLevelHooksTimeout</google>.
		/// </remarks>
		static int LowLevelHooksTimeout {
			get {
				if(s_lowLevelHooksTimeout == 0) {
					if(!Registry_.GetInt(out int i, "LowLevelHooksTimeout", @"Control Panel\Desktop")) i = 300; //default 300, tested on Win10 and 7
					else if((uint)i > 1000) i = 1000; //Win10. On Win7 the limit is bigger. Not tested on Win8. On Win7/8 may be changed by a Windows update.
					s_lowLevelHooksTimeout = i;
				}
				return s_lowLevelHooksTimeout;
				//info: After changing the timeout in registry, it is not applied immediately. Need to log off/on.
			}
		}
		static int s_lowLevelHooksTimeout;

		/// <summary>
		/// Call on any catched exception in a hook procedure.
		/// Returns true if it is ThreadAbortException.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="hook">On ThreadAbortException calls hook.Dispose.</param>
		internal static bool LibOnException(Exception e, IDisposable hook)
		{
			if(e is ThreadAbortException eta) {
				Thread.ResetAbort();
				hook.Dispose();
				Debug_.Print("ThreadAbortException");
				var t = Thread.CurrentThread;
				Task.Run(() => { Thread.Sleep(50); t.Abort(eta.ExceptionState); });
				return true;
			}

			PrintWarning("Unhandled exception in hook procedure. " + e.ToString());
			return false;
		}
	}
}

namespace Au.Types
{
	/// <summary>
	/// Contains types of hook data for hook procedures installed by <see cref="WinHook"/> and <see cref="AccHook"/>.
	/// </summary>
	public static partial class HookData
	{
		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.Keyboard"/>.
		/// More info: <msdn>LowLevelKeyboardProc</msdn>.
		/// </summary>
		public unsafe struct Keyboard
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			Api.KBDLLHOOKSTRUCT* _x;

			internal Keyboard(WinHook hook, LPARAM lParam)
			{
				this.hook = hook;
				_x = (Api.KBDLLHOOKSTRUCT*)lParam;
			}

			/// <summary>
			/// Is extended key.
			/// </summary>
			public bool IsExtended => 0 != (flags & Api.LLKHF_EXTENDED);

			/// <summary>
			/// true if the event was generated by API such as <msdn>SendInput</msdn>.
			/// false if the event was generated by the keyboard.
			/// </summary>
			public bool IsInjected => 0 != (flags & Api.LLKHF_INJECTED);

			/// <summary>
			/// Key Alt is pressed.
			/// </summary>
			public bool IsAlt => 0 != (flags & Api.LLKHF_ALTDOWN);

			/// <summary>
			/// Is key-up event.
			/// </summary>
			public bool IsUp => 0 != (flags & Api.LLKHF_UP);

			/// <summary>
			/// If the key is a modifier key (Shift, Ctrl, Alt, Win), returns the modifier flag. Else returns 0.
			/// </summary>
			public KMod Mod {
				get {
					switch(vkCode) {
					case KKey.Shift: case KKey.LShift: case KKey.RShift: return KMod.Shift;
					case KKey.Ctrl: case KKey.LCtrl: case KKey.RCtrl: return KMod.Ctrl;
					case KKey.Alt: case KKey.LAlt: case KKey.RAlt: return KMod.Alt;
					case KKey.Win: case KKey.RWin: return KMod.Win;
					}
					return 0;
				}
			}

			/// <summary>
			/// If <b>vkCode</b> is a left or right modifier key code (LShift, LCtrl, LAlt, RShift, RCtrl, RAlt, RWin), returns the common modifier key code (Shift, Ctrl, Alt, Win). Else returns <b>vkCode</b>.
			/// </summary>
			public KKey Key {
				get {
					switch(vkCode) {
					case KKey.LShift: case KKey.RShift: return KKey.Shift;
					case KKey.LCtrl: case KKey.RCtrl: return KKey.Ctrl;
					case KKey.LAlt: case KKey.RAlt: return KKey.Alt;
					case KKey.RWin: return KKey.Win;
					}
					return vkCode;
				}
			}

			/// <summary>
			/// Returns true if <paramref name="key"/> == <b>vkCode</b> or <paramref name="key"/> is Shift, Ctrl, Alt or Win and <b>vkCode</b> is LShift/RShift, LCtrl/RCtrl, LAlt/RAlt or RWin.
			/// </summary>
			public bool IsKey(KKey key)
			{
				if(key == vkCode) return true;
				switch(key) {
				case KKey.Shift: return vkCode == KKey.LShift || vkCode == KKey.RShift;
				case KKey.Ctrl: return vkCode == KKey.LCtrl || vkCode == KKey.RCtrl;
				case KKey.Alt: return vkCode == KKey.LAlt || vkCode == KKey.RAlt;
				case KKey.Win: return vkCode == KKey.RWin;
				}
				return false;
			}

			/// <summary>
			/// Converts flags to API SendInput flags KEYEVENTF_KEYUP and KEYEVENTF_EXTENDEDKEY.
			/// </summary>
			internal byte LibSendInputFlags {
				get {
					uint f = 0;
					if(IsUp) f |= Api.KEYEVENTF_KEYUP;
					if(IsExtended) f |= Api.KEYEVENTF_EXTENDEDKEY;
					return (byte)f;
				}
			}

			///
			public override string ToString()
			{
				return $"{vkCode.ToString()} {(IsUp ? "up" : "")}{(IsInjected ? " (injected)" : "")}";
			}

			/// <summary><msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public KKey vkCode => (KKey)_x->vkCode;
			/// <summary><msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public uint scanCode => _x->scanCode;
			/// <summary><msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public uint flags => _x->flags;
			/// <summary><msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public int time => _x->time;
			/// <summary><msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public LPARAM dwExtraInfo => _x->dwExtraInfo;
		}

		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.Mouse"/>.
		/// More info: <msdn>LowLevelMouseProc</msdn>.
		/// </summary>
		public unsafe struct Mouse
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			Api.MSLLHOOKSTRUCT* _x;
			uint _event;

			internal Mouse(WinHook hook, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				_x = (Api.MSLLHOOKSTRUCT*)lParam;
				_event = (uint)wParam;
				IsButtonDown = IsButtonUp = IsWheel = false;
				short wheelOrXButton = (short)(mouseData >> 16);
				switch(_event) {
				case Api.WM_LBUTTONDOWN: case Api.WM_RBUTTONDOWN: case Api.WM_MBUTTONDOWN: IsButtonDown = true; break;
				case Api.WM_LBUTTONUP: case Api.WM_RBUTTONUP: case Api.WM_MBUTTONUP: _event--; IsButtonUp = true; break;
				case Api.WM_XBUTTONUP: _event--; IsButtonUp = true; goto g1;
				case Api.WM_XBUTTONDOWN:
					IsButtonDown = true;
					g1:
					switch(wheelOrXButton) { case 1: _event |= 0x1000; break; case 2: _event |= 0x2000; break; }
					break;
				case Api.WM_MOUSEWHEEL:
				case Api.WM_MOUSEHWHEEL:
					IsWheel = true;
					if(wheelOrXButton > 0) _event |= 0x1000; else if(wheelOrXButton < 0) _event |= 0x2000;
					break;
				}
			}

			/// <summary>
			/// What event it is (button, move, wheel).
			/// </summary>
			public MouseEvent Event => (MouseEvent)_event;

			/// <summary>
			/// Is button-down event.
			/// </summary>
			public bool IsButtonDown { get; }

			/// <summary>
			/// Is button-up event.
			/// </summary>
			public bool IsButtonUp { get; }

			/// <summary>
			/// Is button event (down or up).
			/// </summary>
			public bool IsButton => IsButtonDown | IsButtonUp;

			/// <summary>
			/// Is wheel event.
			/// </summary>
			public bool IsWheel { get; }

			/// <summary>
			/// true if the event was generated by API such as <msdn>SendInput</msdn>.
			/// false if the event was generated by the mouse.
			/// </summary>
			public bool IsInjected => 0 != (flags & Api.LLMHF_INJECTED);

			///
			public override string ToString()
			{
				var ud = ""; if(IsButtonDown) ud = "down"; else if(IsButtonUp) ud = "up";
				return $"{Event.ToString()} {ud} {pt.ToString()}{(IsInjected ? " (injected)" : "")}";
			}

			/// <summary><msdn>MSLLHOOKSTRUCT</msdn></summary>
			public POINT pt => _x->pt;
			/// <summary><msdn>MSLLHOOKSTRUCT</msdn></summary>
			public uint mouseData => _x->mouseData;
			/// <summary><msdn>MSLLHOOKSTRUCT</msdn></summary>
			public uint flags => _x->flags;
			/// <summary><msdn>MSLLHOOKSTRUCT</msdn></summary>
			public int time => _x->time;
			/// <summary><msdn>MSLLHOOKSTRUCT</msdn></summary>
			public LPARAM dwExtraInfo => _x->dwExtraInfo;
		}

		/// <summary>
		/// Mouse hook event types. See <see cref="Mouse.Event"/>.
		/// </summary>
		public enum MouseEvent
		{
#pragma warning disable 1591 //no XML doc
			Move = 0x0200, //WM_MOUSEMOVE
			LeftButton = 0x0201, //WM_LBUTTONDOWN
			RightButton = 0x0204, //WM_RBUTTONDOWN
			MiddleButton = 0x0207, //WM_MBUTTONDOWN
			X1Button = 0x120B, //WM_XBUTTONDOWN | 0x1000
			X2Button = 0x220B, //WM_XBUTTONDOWN | 0x2000
			WheelForward = 0x120A, //WM_WHEEL | 0x1000
			WheelBackward = 0x220A, //WM_WHEEL | 0x2000
			WheelRight = 0x120E, //WM_HWHEEL | 0x1000
			WheelLeft = 0x220E, //WM_HWHEEL | 0x2000
#pragma warning restore 1591
		}

		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.ThreadCbt"/>.
		/// More info: <msdn>CBTProc</msdn>.
		/// </summary>
		public struct ThreadCbt
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			/// <summary><msdn>CBTProc</msdn></summary>
			public readonly CbtEvent code;

			/// <summary><msdn>CBTProc</msdn></summary>
			public readonly LPARAM wParam;

			/// <summary><msdn>CBTProc</msdn></summary>
			public readonly LPARAM lParam;

			internal ThreadCbt(WinHook hook, int code, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				this.code = (CbtEvent)code;
				this.wParam = wParam;
				this.lParam = lParam;
			}

			/// <summary>
			/// Returns the window handle of the window being activated and gets some more info.
			/// </summary>
			/// <param name="fMouse">true if the reason is the mouse.</param>
			/// <param name="wPrevActive">The previously active window, or default(Wnd).</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.ACTIVATE.</exception>
			public unsafe Wnd ActivationInfo(out bool fMouse, out Wnd wPrevActive)
			{
				if(code != CbtEvent.ACTIVATE) throw new InvalidOperationException();
				var t = (Api.CBTACTIVATESTRUCT*)lParam;
				fMouse = t->fMouse;
				wPrevActive = t->hWndActive;
				return (Wnd)wParam;
			}

			/// <summary>
			/// Returns the window handle and gets more info about the created window.
			/// </summary>
			/// <param name="c"><msdn>CREATESTRUCT</msdn>. You can modify x y cx cy.</param>
			/// <param name="wInsertAfter">Window whose position in the Z order precedes that of the window being created, or default(Wnd).</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.CREATEWND.</exception>
			public unsafe Wnd CreationInfo(out Native.CREATESTRUCT* c, out Wnd wInsertAfter)
			{
				if(code != CbtEvent.CREATEWND) throw new InvalidOperationException();
				var t = (Api.CBT_CREATEWND*)lParam;
				c = t->lpcs;
				wInsertAfter = t->hwndInsertAfter;
				return (Wnd)wParam;
			}

			/// <summary>
			/// Returns the mouse message and gets some more info about the mouse event.
			/// </summary>
			/// <param name="m"><msdn>MOUSEHOOKSTRUCT</msdn>.</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.CLICKSKIPPED.</exception>
			public unsafe uint MouseInfo(out Native.MOUSEHOOKSTRUCT* m)
			{
				if(code != CbtEvent.CLICKSKIPPED) throw new InvalidOperationException();
				m = (Native.MOUSEHOOKSTRUCT*)lParam;
				return (uint)wParam;
			}

			/// <summary>
			/// Returns the key code and gets some more info about the keyboard event.
			/// </summary>
			/// <param name="lParam"><i>lParam</i> of the key message. Specifies the repeat count, scan code, etc. See <msdn>WM_KEYDOWN</msdn>.</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.KEYSKIPPED.</exception>
			public KKey KeyInfo(out uint lParam)
			{
				if(code != CbtEvent.KEYSKIPPED) throw new InvalidOperationException();
				lParam = (uint)this.lParam;
				return (KKey)(uint)wParam;
			}

			/// <summary>
			/// Returns the window handle and gets some more info about the focus event.
			/// </summary>
			/// <param name="wLostFocus">The previously focused window, or default(Wnd).</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.SETFOCUS.</exception>
			public Wnd FocusInfo(out Wnd wLostFocus)
			{
				if(code != CbtEvent.SETFOCUS) throw new InvalidOperationException();
				wLostFocus = (Wnd)lParam;
				return (Wnd)wParam;
			}

			/// <summary>
			/// Returns the window handle and gets some more info about the move-size event.
			/// </summary>
			/// <param name="r">The new rectangle of the window.</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.MOVESIZE.</exception>
			public unsafe Wnd MoveSizeInfo(out RECT* r)
			{
				if(code != CbtEvent.MOVESIZE) throw new InvalidOperationException();
				r = (RECT*)lParam;
				return (Wnd)wParam;
			}

			/// <summary>
			/// Returns the window handle and gets some more info about the minimize-maximize-restore event.
			/// </summary>
			/// <param name="showState">The new show state. See <msdn>ShowWindow</msdn>. Minimized 6, maximized 3, restored 9.</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.MINMAX.</exception>
			public Wnd MinMaxInfo(out int showState)
			{
				if(code != CbtEvent.MINMAX) throw new InvalidOperationException();
				showState = (int)lParam & 0xffff;
				return (Wnd)wParam;
			}
		}

		/// <summary>
		/// CBT hook event types. Used with <see cref="ThreadCbt"/>.
		/// More info: <msdn>CBTProc</msdn>.
		/// </summary>
		public enum CbtEvent
		{
#pragma warning disable 1591 //no XML doc
			MOVESIZE = 0,
			MINMAX = 1,
			//QS = 2,
			CREATEWND = 3,
			DESTROYWND = 4,
			ACTIVATE = 5,
			CLICKSKIPPED = 6,
			KEYSKIPPED = 7,
			SYSCOMMAND = 8,
			SETFOCUS = 9,
#pragma warning restore 1591
		}

		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.ThreadGetMessage"/>.
		/// More info: <msdn>GetMsgProc</msdn>.
		/// </summary>
		public unsafe struct ThreadGetMessage
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			/// <summary>
			/// The message has not been removed from the queue, because called API <msdn>PeekMessage</msdn> with flag PM_NOREMOVE.
			/// </summary>
			public readonly bool PM_NOREMOVE;

			/// <summary>
			/// Message parameters.
			/// </summary>
			public readonly Native.MSG* msg;

			internal ThreadGetMessage(WinHook hook, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				PM_NOREMOVE = (uint)wParam == Api.PM_NOREMOVE;
				msg = (Native.MSG*)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.ThreadKeyboard"/>.
		/// More info: <msdn>KeyboardProc</msdn>.
		/// </summary>
		public struct ThreadKeyboard
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			/// <summary>
			/// The message has not been removed from the queue, because called API <msdn>PeekMessage</msdn> with flag PM_NOREMOVE.
			/// </summary>
			public readonly bool PM_NOREMOVE;

			/// <summary>
			/// The key code.
			/// </summary>
			public readonly KKey key;

			/// <summary>
			/// <i>lParam</i> of the key message. Specifies the key state, scan code, etc. See <msdn>KeyboardProc</msdn>.
			/// </summary>
			public readonly uint lParam;

			internal ThreadKeyboard(WinHook hook, int code, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				PM_NOREMOVE = code == Api.HC_NOREMOVE;
				key = (KKey)(uint)wParam;
				this.lParam = (uint)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.ThreadMouse"/>.
		/// More info: <msdn>MouseProc</msdn>.
		/// </summary>
		public unsafe struct ThreadMouse
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			/// <summary>
			/// The message has not been removed from the queue, because called API <msdn>PeekMessage</msdn> with flag PM_NOREMOVE.
			/// </summary>
			public readonly bool PM_NOREMOVE;

			/// <summary>
			/// The mouse message, for example WM_MOUSEMOVE.
			/// </summary>
			public readonly uint message;

			/// <summary>
			/// More info about the mouse message.
			/// </summary>
			public readonly Native.MOUSEHOOKSTRUCT* m;

			internal ThreadMouse(WinHook hook, int code, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				PM_NOREMOVE = code == Api.HC_NOREMOVE;
				message = (uint)wParam;
				m = (Native.MOUSEHOOKSTRUCT*)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.ThreadCallWndProc"/>.
		/// More info: <msdn>CallWndProc</msdn>.
		/// </summary>
		public unsafe struct ThreadCallWndProc
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			/// <summary>
			/// True if the message was sent by another thread.
			/// </summary>
			public readonly bool sentByOtherThread; //note: incorrect info in MSDN

			/// <summary>
			/// Message parameters.
			/// </summary>
			public readonly Native.CWPSTRUCT* msg;

			internal ThreadCallWndProc(WinHook hook, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				sentByOtherThread = wParam;
				msg = (Native.CWPSTRUCT*)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="WinHook.ThreadCallWndProcRet"/>.
		/// More info: <msdn>CallWndRetProc</msdn>.
		/// </summary>
		public unsafe struct ThreadCallWndProcRet
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			/// <summary>
			/// True if the message was sent by another thread.
			/// </summary>
			public readonly bool sentByOtherThread; //note: incorrect info in MSDN

			/// <summary>
			/// Message parameters and the return value.
			/// </summary>
			public readonly Native.CWPRETSTRUCT* msg;

			internal ThreadCallWndProcRet(WinHook hook, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				sentByOtherThread = wParam;
				msg = (Native.CWPRETSTRUCT*)lParam;
			}
		}

		/// <summary>
		/// Calls API <msdn>ReplyMessage</msdn>, which allows to use <see cref="Acc"/> and COM in the hook procedure.
		/// </summary>
		/// <param name="cancelEvent">
		/// Don't notify the target window about the event, and don't call other hook procedures.
		/// This value is used instead of the return value of the hook procedure, which is ignored.
		/// </param>
		/// <remarks>
		/// It can be used as a workaround for this problem: in low-level hook procedure some functions don't work with some windows. For example cannot get an accessible object or use a COM object. Error/exception "An outgoing call cannot be made since the application is dispatching an input-synchronous call (0x8001010D)".
		/// </remarks>
		public static void ReplyMessage(bool cancelEvent) => Api.ReplyMessage(cancelEvent);
	}
}

namespace Au.Util
{
	/// <summary>
	/// Helps with accessible object event hooks. See <msdn>SetWinEventHook</msdn>.
	/// </summary>
	/// <remarks>
	/// The thread that uses hooks must process Windows messages. For example have a window/dialog/messagebox, or use a 'wait-for' function that dispatches messages or has such option (see <see cref="Opt.WaitFor"/>).
	/// The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Uninstall</b>) or with the 'using' pattern.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// //using Au.Util;
	/// bool stop = false;
	/// using(new Au.Util.AccHook(AccEVENT.SYSTEM_FOREGROUND, 0, x =>
	/// {
	/// 	Print(x.wnd);
	/// 	var a = x.GetAcc();
	/// 	Print(a);
	/// 	if(x.wnd.ClassNameIs("Shell_TrayWnd")) stop = true;
	/// })) {
	/// 	MessageBox.Show("hook");
	/// 	//or
	/// 	//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for activated taskbar
	/// 	//Print("the end");
	/// }
	/// ]]></code>
	/// </example>
	public class AccHook : IDisposable
	{
		IntPtr _hh; //HHOOK
		Api.WINEVENTPROC _proc1; //our intermediate hook proc that calls _proc2
		Action<HookData.AccHookData> _proc2; //caller's hook proc

		/// <summary>
		/// Installs hook.
		/// Calls API <msdn>SetWinEventHook</msdn>.
		/// </summary>
		/// <param name="eventMin">Specifies the event constant for the lowest event value in the range of events that are handled by the hook function. This parameter can be set to AccEVENT.MIN to indicate the lowest possible event value. Events reference: <msdn>SetWinEventHook</msdn>.</param>
		/// <param name="eventMax">Specifies the event constant for the highest event value in the range of events that are handled by the hook function. This parameter can be set to AccEVENT.MAX to indicate the highest possible event value. If 0, uses <paramref name="eventMin"/>.</param>
		/// <param name="hookProc">Delegate of the hook procedure.</param>
		/// <param name="idProcess">Specifies the id of the process from which the hook function receives events. If 0 - all processes on the current desktop.</param>
		/// <param name="idThread">Specifies the native id of the thread from which the hook function receives events. If 0 - all threads.</param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example><inheritdoc cref="AccHook"/></example>
		public AccHook(AccEVENT eventMin, AccEVENT eventMax, Action<HookData.AccHookData> hookProc, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			if(eventMax == 0) eventMax = eventMin;
			_hh = Api.SetWinEventHook(eventMin, eventMax, default, _proc1 = _HookProc, idProcess, idThread, flags);
			if(_hh == default) throw new AuException("*set hook");
			_proc2 = hookProc;
		}

		/// <summary>
		/// Uninstalls the hook if installed.
		/// </summary>
		/// <remarks>
		/// Must be called from the same thread that installed the hook.
		/// </remarks>
		public void Unhook()
		{
			if(_hh != default) {
				bool ok = Api.UnhookWinEvent(_hh);
				if(!ok) PrintWarning("Failed to unhook AccHook.");
				_hh = default;
			}
		}

		/// <summary>
		/// Calls <see cref="Unhook"/>.
		/// </summary>
		public void Dispose() { Unhook(); GC.SuppressFinalize(this); }

		//MSDN: UnhookWinEvent fails if called from a thread different from the call that corresponds to SetWinEventHook.
		///
		~AccHook() { PrintWarning("Non-disposed AccHook variable."); } //unhooking makes no sense

		void _HookProc(IntPtr hHook, AccEVENT aEvent, Wnd wnd, int idObject, int idChild, int idThread, int eventTime)
		{
			try {
				_proc2(new HookData.AccHookData(this, aEvent, wnd, idObject, idChild, idThread, eventTime));
			}
			catch(Exception ex) { WinHook.LibOnException(ex, this); }
		}
	}
}

namespace Au.Types
{
	public static partial class HookData
	{
		/// <summary>
		/// Hook data for the hook procedure installed by <see cref="AccHook"/>.
		/// More info: <msdn>WinEventProc</msdn>.
		/// </summary>
		public unsafe struct AccHookData
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AccHook hook;

			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly AccEVENT aEvent;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly Wnd wnd;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int idObject;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int idChild;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int idThread;
			/// <summary><msdn>WinEventProc</msdn></summary>
			public readonly int eventTime;

			internal AccHookData(AccHook hook, AccEVENT aEvent, Wnd wnd, int idObject, int idChild, int idThread, int eventTime)
			{
				this.hook = hook;
				this.aEvent = aEvent;
				this.wnd = wnd;
				this.idObject = idObject;
				this.idChild = idChild;
				this.idThread = idThread;
				this.eventTime = eventTime;
			}

			///<inheritdoc cref="Acc.FromEvent"/>
			public Acc GetAcc()
			{
				return Acc.FromEvent(wnd, idObject, idChild);
			}
		}
	}
}
