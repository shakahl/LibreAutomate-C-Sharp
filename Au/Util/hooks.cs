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

#region winhook

namespace Au.Util
{
	/// <summary>
	/// Helps with windows hooks. See API <msdn>SetWindowsHookEx</msdn>.
	/// </summary>
	/// <remarks>
	/// The thread that uses hooks must process Windows messages. For example have a window/dialog/messagebox, or use a 'wait-for' function that dispatches messages or has such option (see <see cref="Opt.WaitFor"/>).
	/// The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Unhook</b> in the same thread) or with the 'using' pattern. Else this process may crash.
	/// </remarks>
	public class WinHook : IDisposable
	{
		IntPtr _hh; //HHOOK
		readonly Api.HOOKPROC _proc1; //our intermediate dispatcher hook proc that calls _proc2
		Delegate _proc2; //caller's hook proc
		readonly string _hookTypeString; //"Keyboard" etc
		readonly int _hookType; //Api.WH_
		readonly bool _ignoreAuInjected;

		/// <summary>
		/// Sets a low-level keyboard hook (WH_KEYBOARD_LL).
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// The hook procedure (function that handles hook events).
		/// Must return as soon as possible. More info: <see cref="LowLevelHooksTimeout"/>.
		/// If calls <see cref="HookData.Keyboard.BlockEvent"/> or <see cref="HookData.ReplyMessage"/>(true), the event is not sent to apps and other hooks.
		/// Event data cannot be modified.
		/// <note>When the hook procedure returns, the parameter variable becomes invalid and unsafe to use. If you need the data for later use, copy its properties and not whole variable.</note>
		/// </param>
		/// <param name="ignoreAuInjected">Don't call the hook procedure for events sent by functions of this library. Default true.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// var stop = false;
		/// using(WinHook.Keyboard(x => {
		/// 	Print(x);
		/// 	if(x.vkCode == KKey.Escape) { stop = true; x.BlockEvent(); }
		/// })) {
		/// 	MessageBox.Show("Low-level keyboard hook.", "Test");
		/// 	//or
		/// 	//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for Esc key
		/// 	//Print("the end");
		/// }
		/// ]]></code>
		/// </example>
		public static WinHook Keyboard(Action<HookData.Keyboard> hookProc, bool ignoreAuInjected = true, bool setNow = true)
			=> new WinHook(Api.WH_KEYBOARD_LL, hookProc, setNow, 0, ignoreAuInjected);

		/// <summary>
		/// Sets a low-level mouse hook (WH_MOUSE_LL).
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// The hook procedure (function that handles hook events).
		/// Must return as soon as possible. More info: <see cref="LowLevelHooksTimeout"/>.
		/// If calls <see cref="HookData.Mouse.BlockEvent"/> or <see cref="HookData.ReplyMessage"/>(true), the event is not sent to apps and other hooks.
		/// Event data cannot be modified.
		/// <note>When the hook procedure returns, the parameter variable becomes invalid and unsafe to use. If you need the data for later use, copy its properties and not whole variable.</note>
		/// </param>
		/// <param name="ignoreAuInjected">Don't call the hook procedure for events sent by functions of this library. Default true.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// var stop = false;
		/// using(WinHook.Mouse(x => {
		/// 	Print(x);
		/// 	if(x.Event == HookData.MouseEvent.RightButton) { stop = x.IsButtonUp; x.BlockEvent(); }
		/// })) {
		/// 	MessageBox.Show("Low-level mouse hook.", "Test");
		/// 	//or
		/// 	//WaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for right-click
		/// 	//Print("the end");
		/// }
		/// ]]></code>
		/// </example>
		public static WinHook Mouse(Action<HookData.Mouse> hookProc, bool ignoreAuInjected = true, bool setNow = true)
			=> new WinHook(Api.WH_MOUSE_LL, hookProc, setNow, 0, ignoreAuInjected);

		internal static WinHook LibMouseRaw(Func<LPARAM, LPARAM, bool> hookProc, bool ignoreAuInjected = true, bool setNow = true)
			=> new WinHook(Api.WH_MOUSE_LL, hookProc, setNow, 0, ignoreAuInjected, "Mouse");

		/// <summary>
		/// Sets a WH_CBT hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// Hook procedure (function that handles hook events).
		/// Must return as soon as possible.
		/// If returns true, the event is cancelled. For some events you can modify some fields of event data.
		/// <note>When the hook procedure returns, the parameter variable becomes invalid and unsafe to use. If you need the data for later use, copy its properties and not whole variable.</note>
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadCbt(x => {
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
		public static WinHook ThreadCbt(Func<HookData.ThreadCbt, bool> hookProc, int threadId = 0, bool setNow = true)
			=> new WinHook(Api.WH_CBT, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_GETMESSAGE hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// The hook procedure (function that handles hook events).
		/// Must return as soon as possible.
		/// The event cannot be cancelled. As a workaround, you can set msg->message=0. Also can modify other fields.
		/// <note>When the hook procedure returns, the pointer field of the parameter variable becomes invalid and unsafe to use.</note>
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadGetMessage(x => {
		/// 	Print(x.msg->ToString(), x.PM_NOREMOVE);
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadGetMessage(Action<HookData.ThreadGetMessage> hookProc, int threadId = 0, bool setNow = true)
			=> new WinHook(Api.WH_GETMESSAGE, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_GETMESSAGE hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// The hook procedure (function that handles hook events).
		/// Must return as soon as possible.
		/// If returns true, the event is cancelled.
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadKeyboard(x => {
		/// 	Print(x.key, 0 != (x.lParam & 0x80000000) ? "up" : "", x.lParam, x.PM_NOREMOVE);
		/// 	return false;
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadKeyboard(Func<HookData.ThreadKeyboard, bool> hookProc, int threadId = 0, bool setNow = true)
			=> new WinHook(Api.WH_KEYBOARD, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_MOUSE hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// The hook procedure (function that handles hook events).
		/// Must return as soon as possible.
		/// If returns true, the event is cancelled.
		/// <note>When the hook procedure returns, the pointer field of the parameter variable becomes invalid and unsafe to use.</note>
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadMouse(x => {
		/// 	Print(x.message, x.m->pt, x.m->hwnd, x.PM_NOREMOVE);
		/// 	return false;
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadMouse(Func<HookData.ThreadMouse, bool> hookProc, int threadId = 0, bool setNow = true)
			=> new WinHook(Api.WH_MOUSE, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_CALLWNDPROC hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// The hook procedure (function that handles hook events).
		/// Must return as soon as possible.
		/// The event cannot be cancelled or modified.
		/// <note>When the hook procedure returns, the pointer field of the parameter variable becomes invalid and unsafe to use.</note>
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadCallWndProc(x => {
		/// 	ref var m = ref *x.msg;
		/// 	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam);
		/// 	Print(mm, x.sentByOtherThread);
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadCallWndProc(Action<HookData.ThreadCallWndProc> hookProc, int threadId = 0, bool setNow = true)
			=> new WinHook(Api.WH_CALLWNDPROC, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_CALLWNDPROCRET hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="WinHook"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// The hook procedure (function that handles hook events).
		/// Must return as soon as possible.
		/// The event cannot be cancelled or modified.
		/// <note>When the hook procedure returns, the pointer field of the parameter variable becomes invalid and unsafe to use.</note>
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// //using Au.Util;
		/// using(WinHook.ThreadCallWndProcRet(x => {
		/// 	ref var m = ref *x.msg;
		/// 	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam); mm.Result = m.lResult;
		/// 	Print(mm, x.sentByOtherThread);
		/// })) MessageBox.Show("hook");
		/// ]]></code>
		/// </example>
		public static WinHook ThreadCallWndProcRet(Action<HookData.ThreadCallWndProcRet> hookProc, int threadId = 0, bool setNow = true)
			=> new WinHook(Api.WH_CALLWNDPROCRET, hookProc, setNow, threadId);

		WinHook(int hookType, Delegate hookProc, bool setNow, int tid, bool ignoreAuInjected = false, [CallerMemberName] string hookTypeString = null)
		{
			_proc1 = _HookProc;
			_proc2 = hookProc;
			_hookType = hookType;
			_hookTypeString = hookTypeString;
			_ignoreAuInjected = ignoreAuInjected;
			if(setNow) Hook(tid);

			//info: don't need to JIT _HookProc for LL hooks of triggers, because we use CBT hook before it to create the message-only window.
		}

		/// <summary>
		/// Sets the hook.
		/// </summary>
		/// <param name="threadId">If the hook type is a thread hook - thread id, or 0 for current thread. Else not used and must be 0.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <exception cref="InvalidOperationException">The hook is already set.</exception>
		/// <exception cref="ArgumentException">threadId not 0 and the hook type is not a thread hook.</exception>
		/// <remarks>
		/// Usually don't need to call this function, because the <b>WinHook</b> static methods that return a new <b>WinHook</b> object by default call it.
		/// </remarks>
		public void Hook(int threadId = 0)
		{
			if(_proc2 == null) throw new ObjectDisposedException("WinHook");
			if(_hh != default) throw new InvalidOperationException("The hook is already set.");
			if(_hookType == Api.WH_KEYBOARD_LL || _hookType == Api.WH_MOUSE_LL) {
				if(threadId != 0) throw new ArgumentException("threadId must be 0");
			} else if(threadId == 0) {
				threadId = Api.GetCurrentThreadId();
			}
			_hh = Api.SetWindowsHookEx(_hookType, _proc1, default, threadId);
			if(_hh == default) throw new AuException(0, "*set hook");
		}

		/// <summary>
		/// Removes the hook.
		/// </summary>
		/// <remarks>
		/// Does nothing if already removed or wasn't set.
		/// Later you can call <see cref="Hook"/> to set hook again.
		/// </remarks>
		public void Unhook()
		{
			if(_hh != default) {
				bool ok = Api.UnhookWindowsHookEx(_hh);
				if(!ok) PrintWarning($"Failed to unhook WinHook ({_hookTypeString}). {WinError.Message}");
				_hh = default;
			}
		}

		/// <summary>
		/// Returns true if the hook is set.
		/// </summary>
		public bool IsSet => _hh != default;

		///// <summary>
		///// Don't print warning "Non-disposed WinHook variable".
		///// </summary>
		//public bool NoWarningNondisposed { get; set; }

		/// <summary>
		/// Calls <see cref="Unhook"/> and disposes this object.
		/// </summary>
		public void Dispose()
		{
			Unhook();
			_proc2 = null;
			GC.SuppressFinalize(this);
		}

		///
		~WinHook()
		{
			//unhooking in finalizer thread makes no sense. Must unhook in same thread, else fails.
			//if(_hh != default && !NoWarningNondisposed) PrintWarning($"Non-disposed WinHook ({_hookTypeString}) variable."); //rejected. Eg when called Environment.Exit, finalizers are executed but finally code blocks not.
			Debug_.PrintIf(_hh != default, $"Non-disposed WinHook ({_hookTypeString}) variable.");
		}

		unsafe LPARAM _HookProc(int code, LPARAM wParam, LPARAM lParam)
		{
			try {
				if(code >= 0) {
					bool R = false;
					long t1 = 0;
					Action<HookData.Mouse> pm1; Func<LPARAM, LPARAM, bool> pm2;

					switch(_proc2) {
					case Action<HookData.Keyboard> p:
						var kll = (Api.KBDLLHOOKSTRUCT*)lParam;
						var vk = (KKey)kll->vkCode;
						if(kll->IsInjected) {
							if(kll->IsInjectedByAu) {
								if(kll->vkCode == 0) goto gr; //used to enable activating windows
								if(!kll->IsUp) Triggers.AutotextTriggers.ResetEverywhere = true;
								if(_ignoreAuInjected) goto gr;
							}
							if(vk == KKey.MouseX2 && kll->dwExtraInfo == 1354291109) goto gr; //QM2 sync code
						} else {
							//When Keyb.Lib.ReleaseModAndCapsLock sends Shift to turn off CapsLock,
							//	hooks receive a non-injected LShift down, CapsLock down/up and injected LShift up.
							//	Our triggers would recover, but cannot auto-repeat. Better don't call the hookproc.
							if((vk == KKey.CapsLock || vk == KKey.LShift) && _ignoreAuInjected && _IgnoreLShiftCaps) goto gr;

							//Test how our triggers recover when a modifier down or up event is lost. Or when triggers started while a modifier is down.
							//if(Keyb.IsScrollLock) {
							//	//if(vk == KKey.LCtrl && !kll->IsUp) { Print("lost Ctrl down"); goto gr; }
							//	if(vk == KKey.LCtrl && kll->IsUp) { Print("lost Ctrl up"); goto gr; }
							//}
						}
						if(Keyb.LibKeyTypes.IsMod(vk) && _IgnoreMod) goto gr;
						t1 = Time.PerfMilliseconds;
						p(new HookData.Keyboard(this, lParam)); //info: wParam is message, but it is not useful, everything is in lParam
						if(R = kll->BlockEvent) kll->BlockEvent = false;
						break;
					case Action<HookData.Mouse> p:
						pm1 = p; pm2 = null;
						gm1:
						var mll = (Api.MSLLHOOKSTRUCT*)lParam;
						switch((int)wParam) {
						case Api.WM_LBUTTONDOWN: case Api.WM_RBUTTONDOWN: Triggers.AutotextTriggers.ResetEverywhere = true; break;
						}
						if(_ignoreAuInjected && mll->IsInjectedByAu) goto gr;
						t1 = Time.PerfMilliseconds;
						if(pm2 != null) {
							R = pm2(wParam, lParam);
						} else {
							pm1(new HookData.Mouse(this, wParam, lParam));
							if(R = mll->BlockEvent) mll->BlockEvent = false;
						}
						break;
					case Func<LPARAM, LPARAM, bool> p: //raw mouse
						pm2 = p; pm1 = null;
						goto gm1;
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
					//		1. Does not wait more. Passes the message to the next hook etc, and we cannot return 1 to block it.
					//		2. Kills the hook after several such cases. Usually 6 keys or 11 mouse events.
					//		3. Makes the hook useless: next times does not wait for it, and we cannot return 1 to block the event.
					//	Somehow does not apply 2 and 3 to some apps, eg C# apps created by Visual Studio, although applies to those created not by VS. I did not find why.
					if(t1 != 0 && (t1 = Time.PerfMilliseconds - t1) > 200 /*&& t1 < 5000*/ && !Debugger.IsAttached) {
						if(t1 > LowLevelHooksTimeout - 50) {
							var s1 = _hookType == Api.WH_KEYBOARD_LL ? "key" : "mouse";
							var s2 = R ? $" On timeout the {s1} message is passed to the active window, other hooks, etc." : null;
							//PrintWarning($"Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}"); //too slow first time
							//Print($"Warning: Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}\r\n{new StackTrace(0, false)}"); //first Print JIT 30 ms
							ThreadPool.QueueUserWorkItem(s3 => Print(s3), $"Warning: Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}\r\n{new StackTrace(0, false)}"); //fast if with false. But async print can be confusing.
						}
						//FUTURE: print warning if t1 is >25 frequently. Unhook and don't rehook if >LowLevelHooksTimeout-50 frequently.

						Api.UnhookWindowsHookEx(_hh);
						_hh = Api.SetWindowsHookEx(_hookType, _proc1, default, 0);
					}

					if(R) return 1;
				}
			}
			catch(Exception ex) { if(LibOnException(ex, this)) return 0; }
			//info: on any exception .NET would terminate process, even on ThreadAbortException.
			//	This prevents it when using eg AuDialog. But not when eg MessageBox.Show; I don't know how to prevent it.
			gr:
			return Api.CallNextHookEx(default, code, wParam, lParam);
		}

		/// <summary>
		/// Gets the max time in milliseconds allowed by Windows for low-level keyboard and mouse hook procedures.
		/// </summary>
		/// <remarks>
		/// Gets registry value HKEY_CURRENT_USER\Control Panel\Desktop:LowLevelHooksTimeout. If it is missing, returns 300; it is the default value used by Windows. If greater than 1000, returns 1000, because Windows 10 ignores bigger values.
		/// 
		/// If a hook procedure takes more time, Windows does not wait. Then the return value is ignored, and the event is passed to other apps, hooks, etc. After several such cases Windows may fully or partially disable the hook. This class detects such cases; then it restores the hook and displays a warning. If the warning is rare, you can ignore it. If frequent, it means your hook procedure is too slow and need to edit it.
		/// 
		/// Callback functions of keyboard and mouse triggers are called in a hook procedure, therefore must be as fast as possible. More info: <see cref="Triggers.TriggerFuncs"/>.
		/// 
		/// More info: <msdn>registry LowLevelHooksTimeout</msdn>.
		/// </remarks>
		public static int LowLevelHooksTimeout {
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
		/// Called on any catched exception in a hook procedure.
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

		[StructLayout(LayoutKind.Sequential, Size = 32)] //note: this struct is in shared memory. Size must be same in all library versions.
		internal struct LibSharedMemoryData
		{
			public long dontBlockModUntil, dontBlocLShiftCapsUntil;
			//16 bytes reserved
		}

		/// <summary>
		/// Let other hooks (in all processes) ignore modifier keys for timeMS milliseconds. If 0 - restore.
		/// Used by mouse triggers.
		/// Returns the timeout time (Time.WinMilliseconds + timeMS) or 0.
		/// </summary>
		internal unsafe long LibIgnoreModInOtherHooks(long timeMS)
		{
			_ignoreModExceptThisHook = timeMS > 0;
			var r = _ignoreModExceptThisHook ? Time.WinMilliseconds + timeMS : 0;
			LibSharedMemory.Ptr->winHook.dontBlockModUntil = r;
			return r;
		}

		unsafe bool _IgnoreMod => LibSharedMemory.Ptr->winHook.dontBlockModUntil > Time.WinMilliseconds && !_ignoreModExceptThisHook;
		bool _ignoreModExceptThisHook;

		/// <summary>
		/// Let all hooks (in all processes) ignore LShift and CapsLock for timeMS milliseconds. If 0 - restore.
		/// Returns the timeout time (Time.WinMilliseconds + timeMS) or 0.
		/// Used when turning off CapsLock with Shift.
		/// </summary>
		internal static unsafe long LibIgnoreLShiftCaps(long timeMS)
		{
			var r = timeMS > 0 ? Time.WinMilliseconds + timeMS : 0;
			LibSharedMemory.Ptr->winHook.dontBlocLShiftCapsUntil = r;
			return r;
		}

		static unsafe bool _IgnoreLShiftCaps => LibSharedMemory.Ptr->winHook.dontBlocLShiftCapsUntil > Time.WinMilliseconds;
	}
}

namespace Au.Types
{
	/// <summary>
	/// Contains types of hook data for hook procedures set by <see cref="WinHook"/> and <see cref="AccHook"/>.
	/// </summary>
	public static partial class HookData
	{
		/// <summary>
		/// Event data for the hook procedure set by <see cref="WinHook.Keyboard"/>.
		/// More info: API <msdn>LowLevelKeyboardProc</msdn>.
		/// </summary>
		public unsafe struct Keyboard
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			readonly Api.KBDLLHOOKSTRUCT* _x;

			internal Keyboard(WinHook hook, LPARAM lParam)
			{
				this.hook = hook;
				_x = (Api.KBDLLHOOKSTRUCT*)lParam;
			}

			/// <summary>
			/// Call this function to steal this event from other hooks and apps.
			/// </summary>
			public void BlockEvent() => _x->BlockEvent = true;

			/// <summary>
			/// Is extended key.
			/// </summary>
			public bool IsExtended => 0 != (_x->flags & Api.LLKHF_EXTENDED);

			/// <summary>
			/// true if the event was generated by API such as <msdn>SendInput</msdn>.
			/// false if the event was generated by the keyboard.
			/// </summary>
			public bool IsInjected => 0 != (_x->flags & Api.LLKHF_INJECTED);

			/// <summary>
			/// true if the event was generated by functions of this library.
			/// </summary>
			public bool IsInjectedByAu => 0 != (_x->flags & Api.LLKHF_INJECTED) && _x->dwExtraInfo == Api.AuExtraInfo;

			/// <summary>
			/// Key Alt is pressed.
			/// </summary>
			public bool IsAlt => 0 != (_x->flags & Api.LLKHF_ALTDOWN);

			/// <summary>
			/// Is key-up event.
			/// </summary>
			public bool IsUp => 0 != (_x->flags & Api.LLKHF_UP);

			/// <summary>
			/// If the key is a modifier key (Shift, Ctrl, Alt, Win), returns the modifier flag. Else returns 0.
			/// </summary>
			public KMod Mod => Keyb.Lib.KeyToMod((KKey)_x->vkCode);

			/// <summary>
			/// If <b>vkCode</b> is a left or right modifier key code (LShift, LCtrl, LAlt, RShift, RCtrl, RAlt, RWin), returns the common modifier key code (Shift, Ctrl, Alt, Win). Else returns <b>vkCode</b>.
			/// </summary>
			public KKey Key {
				get {
					var vk = (KKey)_x->vkCode;
					switch(vk) {
					case KKey.LShift: case KKey.RShift: return KKey.Shift;
					case KKey.LCtrl: case KKey.RCtrl: return KKey.Ctrl;
					case KKey.LAlt: case KKey.RAlt: return KKey.Alt;
					case KKey.RWin: return KKey.Win;
					}
					return vk;
				}
			}

			/// <summary>
			/// Returns true if *key* == <b>vkCode</b> or *key* is Shift, Ctrl, Alt or Win and <b>vkCode</b> is LShift/RShift, LCtrl/RCtrl, LAlt/RAlt or RWin.
			/// </summary>
			public bool IsKey(KKey key)
			{
				var vk = (KKey)_x->vkCode;
				if(key == vk) return true;
				switch(key) {
				case KKey.Shift: return vk == KKey.LShift || vk == KKey.RShift;
				case KKey.Ctrl: return vk == KKey.LCtrl || vk == KKey.RCtrl;
				case KKey.Alt: return vk == KKey.LAlt || vk == KKey.RAlt;
				case KKey.Win: return vk == KKey.RWin;
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

			/// <summary>API <msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public KKey vkCode => (KKey)_x->vkCode;
			/// <summary>API <msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public uint scanCode => _x->scanCode;
			/// <summary>API <msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public uint flags => _x->flags;
			/// <summary>API <msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public int time => _x->time;
			/// <summary>API <msdn>KBDLLHOOKSTRUCT</msdn></summary>
			public LPARAM dwExtraInfo => _x->dwExtraInfo;

			internal Api.KBDLLHOOKSTRUCT* LibNativeStructPtr => _x;
		}

		/// <summary>
		/// Extra info value used by functions of this library that generate keyboard events. Low-level hooks receive it in <b>dwExtraInfo</b>.
		/// </summary>
		public const int AuExtraInfo = Api.AuExtraInfo;

		/// <summary>
		/// Hook data for the hook procedure set by <see cref="WinHook.Mouse"/>.
		/// More info: API <msdn>LowLevelMouseProc</msdn>.
		/// </summary>
		public unsafe struct Mouse
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			readonly Api.MSLLHOOKSTRUCT* _x;
			readonly MouseEvent _event;

			internal Mouse(WinHook hook, LPARAM wParam, LPARAM lParam)
			{
				IsButtonDown = IsButtonUp = IsWheel = false;
				this.hook = hook;
				var p = (Api.MSLLHOOKSTRUCT*)lParam;
				_x = p;
				int e = (int)wParam;
				switch(e) {
				case Api.WM_LBUTTONDOWN: case Api.WM_RBUTTONDOWN: case Api.WM_MBUTTONDOWN: IsButtonDown = true; break;
				case Api.WM_LBUTTONUP: case Api.WM_RBUTTONUP: case Api.WM_MBUTTONUP: e--; IsButtonUp = true; break;
				case Api.WM_XBUTTONUP: e--; IsButtonUp = true; goto g1;
				case Api.WM_XBUTTONDOWN:
					IsButtonDown = true;
					g1:
					switch(p->mouseData >> 16) { case 1: e |= 0x1000; break; case 2: e |= 0x2000; break; }
					break;
				case Api.WM_MOUSEWHEEL:
				case Api.WM_MOUSEHWHEEL:
					IsWheel = true;
					short wheel = (short)(p->mouseData >> 16);
					if(wheel > 0) e |= 0x1000; else if(wheel < 0) e |= 0x2000;
					break;
				}
				_event = (MouseEvent)e;
			}

			/// <summary>
			/// Call this function to steal this event from other hooks and apps.
			/// </summary>
			public void BlockEvent() => _x->BlockEvent = true;

			/// <summary>
			/// What event it is (button, move, wheel).
			/// </summary>
			public MouseEvent Event => _event;

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
			/// Converts <see cref="Event"/> to <see cref="MButtons"/>.
			/// </summary>
			public MButtons Button {
				get {
					switch(_event) {
					case MouseEvent.LeftButton: return MButtons.Left;
					case MouseEvent.RightButton: return MButtons.Right;
					case MouseEvent.MiddleButton: return MButtons.Middle;
					case MouseEvent.X1Button: return MButtons.X1;
					case MouseEvent.X2Button: return MButtons.X2;
					}
					return 0;
				}
			}

			/// <summary>
			/// Is wheel event.
			/// </summary>
			public bool IsWheel { get; }

			/// <summary>
			/// true if the event was generated by API such as <msdn>SendInput</msdn>.
			/// false if the event was generated by the mouse.
			/// </summary>
			public bool IsInjected => 0 != (flags & Api.LLMHF_INJECTED);

			/// <summary>
			/// true if the event was generated by functions of this library.
			/// </summary>
			public bool IsInjectedByAu => IsInjected && dwExtraInfo == Api.AuExtraInfo;

			///
			public override string ToString()
			{
				var ud = ""; if(IsButtonDown) ud = "down"; else if(IsButtonUp) ud = "up";
				return $"{Event.ToString()} {ud} {pt.ToString()}{(IsInjected ? " (injected)" : "")}";
			}

			/// <summary>API <msdn>MSLLHOOKSTRUCT</msdn></summary>
			public POINT pt => _x->pt;
			/// <summary>API <msdn>MSLLHOOKSTRUCT</msdn></summary>
			public uint mouseData => _x->mouseData;
			/// <summary>API <msdn>MSLLHOOKSTRUCT</msdn></summary>
			public uint flags => _x->flags;
			/// <summary>API <msdn>MSLLHOOKSTRUCT</msdn></summary>
			public int time => _x->time;
			/// <summary>API <msdn>MSLLHOOKSTRUCT</msdn></summary>
			public LPARAM dwExtraInfo => _x->dwExtraInfo;

			internal Api.MSLLHOOKSTRUCT* LibNativeStructPtr => _x;
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
		/// Hook data for the hook procedure set by <see cref="WinHook.ThreadCbt"/>.
		/// More info: API <msdn>CBTProc</msdn>.
		/// </summary>
		public struct ThreadCbt
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly WinHook hook;

			/// <summary>API <msdn>CBTProc</msdn></summary>
			public readonly CbtEvent code;

			/// <summary>API <msdn>CBTProc</msdn></summary>
			public readonly LPARAM wParam;

			/// <summary>API <msdn>CBTProc</msdn></summary>
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
			/// <param name="c">
			/// API <msdn>CREATESTRUCT</msdn>.
			/// You can modify x y cx cy.
			/// </param>
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
			/// <param name="m">API <msdn>MOUSEHOOKSTRUCT</msdn>.</param>
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
			/// <param name="lParam"><i>lParam</i> of the key message. Specifies the repeat count, scan code, etc. See API <msdn>WM_KEYDOWN</msdn>.</param>
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
			/// <param name="showState">The new show state. See API <msdn>ShowWindow</msdn>. Minimized 6, maximized 3, restored 9.</param>
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
		/// More info: API <msdn>CBTProc</msdn>.
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
		/// Hook data for the hook procedure set by <see cref="WinHook.ThreadGetMessage"/>.
		/// More info: API <msdn>GetMsgProc</msdn>.
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
			/// API <msdn>MSG</msdn>.
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
		/// Hook data for the hook procedure set by <see cref="WinHook.ThreadKeyboard"/>.
		/// More info: API <msdn>KeyboardProc</msdn>.
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
			/// <i>lParam</i> of the key message. Specifies the key state, scan code, etc. See API <msdn>KeyboardProc</msdn>.
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
		/// Hook data for the hook procedure set by <see cref="WinHook.ThreadMouse"/>.
		/// More info: API <msdn>MouseProc</msdn>.
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
			/// API <msdn>MOUSEHOOKSTRUCT</msdn>.
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
		/// Hook data for the hook procedure set by <see cref="WinHook.ThreadCallWndProc"/>.
		/// More info: API <msdn>CallWndProc</msdn>.
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
			/// API <msdn>CWPSTRUCT</msdn>.
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
		/// Hook data for the hook procedure set by <see cref="WinHook.ThreadCallWndProcRet"/>.
		/// More info: API <msdn>CallWndRetProc</msdn>.
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
			/// API <msdn>CWPRETSTRUCT</msdn>.
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
		/// Calls API API <msdn>ReplyMessage</msdn>, which allows to use <see cref="Acc"/> and COM in the hook procedure.
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

#endregion

#region acchook

namespace Au.Util
{
	/// <summary>
	/// Helps with accessible object event hooks. See API <msdn>SetWinEventHook</msdn>.
	/// </summary>
	/// <remarks>
	/// The thread that uses hooks must process Windows messages. For example have a window/dialog/messagebox, or use a 'wait-for' function that dispatches messages or has such option (see <see cref="Opt.WaitFor"/>).
	/// The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Unhook</b>) or with the 'using' pattern.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
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
		IntPtr[] _ahh; //multiple HHOOK
		Api.WINEVENTPROC _proc1; //our intermediate hook proc that calls _proc2
		Action<HookData.AccHookData> _proc2; //caller's hook proc

		/// <summary>
		/// Sets a hook for an event or a range of events.
		/// Calls API <msdn>SetWinEventHook</msdn>.
		/// </summary>
		/// <param name="eventMin">The lowest event constant value in the range of events. Can be AccEVENT.MIN to indicate the lowest possible event value. Events reference: <msdn>SetWinEventHook</msdn>.</param>
		/// <param name="eventMax">The highest event constant value in the range of events. Can be AccEVENT.MAX to indicate the highest possible event value. If 0, uses *eventMin*.</param>
		/// <param name="hookProc">The hook procedure (function that handles hook events).</param>
		/// <param name="idProcess">The id of the process from which the hook function receives events. If 0 - all processes on the current desktop.</param>
		/// <param name="idThread">The native id of the thread from which the hook function receives events. If 0 - all threads.</param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>See <see cref="AccHook"/>.</example>
		public AccHook(AccEVENT eventMin, AccEVENT eventMax, Action<HookData.AccHookData> hookProc, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			if(eventMax == 0) eventMax = eventMin;
			_proc1 = _HookProc;
			Hook(eventMin, eventMax, idProcess, idThread, flags);
			_proc2 = hookProc;
		}

		/// <summary>
		/// Sets a hook for multiple events.
		/// Calls API <msdn>SetWinEventHook</msdn>.
		/// </summary>
		/// <param name="events">Events. Reference: API <msdn>SetWinEventHook</msdn>. Elements with value 0 are ignored.</param>
		/// <param name="hookProc">The hook procedure (function that handles hook events).</param>
		/// <param name="idProcess">The id of the process from which the hook function receives events. If 0 - all processes on the current desktop.</param>
		/// <param name="idThread">The native id of the thread from which the hook function receives events. If 0 - all threads.</param>
		/// <param name="flags"></param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>See <see cref="AccHook"/>.</example>
		public AccHook(AccEVENT[] events, Action<HookData.AccHookData> hookProc, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_proc1 = _HookProc;
			Hook(events, idProcess, idThread, flags);
			_proc2 = hookProc;
		}

		/// <summary>
		/// Sets hooks again after <see cref="Unhook"/>.
		/// </summary>
		/// <remarks>
		/// Parameters are the same as of the constructor, but values can be different.
		/// </remarks>
		public void Hook(AccEVENT eventMin, AccEVENT eventMax, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_Throw1();
			_hh = Api.SetWinEventHook(eventMin, eventMax, default, _proc1, idProcess, idThread, flags);
			if(_hh == default) throw new AuException(0, "*set hook");
		}

		/// <summary>
		/// Sets hooks again after <see cref="Unhook"/>.
		/// </summary>
		/// <remarks>
		/// Parameters are the same as of the constructor, but values can be different.
		/// </remarks>
		public void Hook(AccEVENT[] events, int idProcess = 0, int idThread = 0, AccHookFlags flags = 0)
		{
			_Throw1();
			_ahh = new IntPtr[events.Length];
			for(int i = 0; i < events.Length; i++) {
				var e = events[i]; if(e == 0) continue;
				var hh = Api.SetWinEventHook(e, e, default, _proc1, idProcess, idThread, flags);
				if(hh == default) { var ec = WinError.Code; Unhook(); throw new AuException(ec, "*set hook for " + e.ToString()); }
				_ahh[i] = hh;
			}
		}

		void _Throw1()
		{
			if(_hh != default || _ahh != null) throw new InvalidOperationException();
			if(_proc1 == null) throw new ObjectDisposedException(nameof(AccHook));
		}

		/// <summary>
		/// Removes the hook.
		/// </summary>
		/// <remarks>
		/// Does nothing if already removed or wasn't set.
		/// Must be called from the same thread that sets the hook.
		/// </remarks>
		public void Unhook()
		{
			if(_hh != default) {
				if(!Api.UnhookWinEvent(_hh)) PrintWarning("Failed to unhook AccHook.");
				_hh = default;
			} else if(_ahh != null) {
				foreach(var hh in _ahh) {
					if(hh == default) continue;
					if(!Api.UnhookWinEvent(hh)) PrintWarning("Failed to unhook AccHook.");
				}
				_ahh = null;
			}
		}

		/// <summary>
		/// Calls <see cref="Unhook"/>.
		/// </summary>
		public void Dispose()
		{
			Unhook();
			_proc1 = null;
			GC.SuppressFinalize(this);
		}

		//MSDN: UnhookWinEvent fails if called from a thread different from the call that corresponds to SetWinEventHook.
		///
		~AccHook() { PrintWarning("Non-disposed AccHook variable."); } //unhooking makes no sense

		void _HookProc(IntPtr hHook, AccEVENT ev, Wnd wnd, AccOBJID idObject, int idChild, int idThread, int eventTime)
		{
			try {
				_proc2(new HookData.AccHookData(this, ev, wnd, idObject, idChild, idThread, eventTime));
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
		/// Hook data for the hook procedure set by <see cref="AccHook"/>.
		/// More info: API <msdn>WinEventProc</msdn>.
		/// </summary>
		public unsafe struct AccHookData
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AccHook hook;

			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly Wnd wnd;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly AccEVENT ev;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly AccOBJID idObject;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly int idChild;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly int idThread;
			/// <summary>API <msdn>WinEventProc</msdn></summary>
			public readonly int eventTime;

			internal AccHookData(AccHook hook, AccEVENT ev, Wnd wnd, AccOBJID idObject, int idChild, int idThread, int eventTime)
			{
				this.hook = hook;
				this.ev = ev;
				this.wnd = wnd;
				this.idObject = idObject;
				this.idChild = idChild;
				this.idThread = idThread;
				this.eventTime = eventTime;
			}

			/// <summary>
			/// Calls <see cref="Acc.FromEvent"/>.
			/// </summary>
			public Acc GetAcc()
			{
				return Acc.FromEvent(wnd, idObject, idChild);
			}
		}
	}
}

#endregion
