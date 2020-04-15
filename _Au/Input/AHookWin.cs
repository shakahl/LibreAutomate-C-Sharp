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

using Au.Types;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Wraps API <msdn>SetWindowsHookEx</msdn>.
	/// </summary>
	/// <remarks>
	/// Hooks are used to receive notifications about various system events. Keyboard and mouse input, window messages, various window events.
	/// 
	/// Threads that use hooks must process Windows messages. For example have a window/dialog/messagebox, or use a 'wait-for' function that dispatches messages or has such option (see <see cref="AOpt.WaitFor"/>).
	/// 
	/// <note type="important">The variable must be disposed, either explicitly (call <b>Dispose</b> or <b>Unhook</b> in the same thread) or with the 'using' pattern. Else this process may crash.</note>
	/// 
	/// <note type="warning">Avoid many hooks. Each low-level keyboard or mouse hook makes the computer slower, even if the hook procedure is fast. On each input event (key down, key up, mouse move, click, wheel) Windows sends a message to your thread.</note>
	/// 
	/// To handle hook events is used a callback functions, aka hook procedure. Hook procedures of some hook types can block some events. Blocked events are not sent to apps and older hooks.
	/// 
	/// Accessible object functions may fail in hook procedures of low-level keyboard and mouse hooks. Workarounds exist.
	/// 
	/// Exists an alternative way to monitor keyboard or mouse events - raw input API. Good: less overhead; can detect from which device the input event came. Bad: cannot block events; incompatible with low-level keyboard hooks. This library does not have functions to make the API easier to use.
	/// </remarks>
	public class AHookWin : IDisposable
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
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
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
		/// var stop = false;
		/// using var hook = AHookWin.Keyboard(x => {
		/// 	AOutput.Write(x);
		/// 	if(x.vkCode == KKey.Escape) { stop = true; x.BlockEvent(); }
		/// });
		/// ADialog.Show("hook");
		/// //or
		/// //AWaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for Esc key
		/// //AOutput.Write("the end");
		/// ]]></code>
		/// </example>
		public static AHookWin Keyboard(Action<HookData.Keyboard> hookProc, bool ignoreAuInjected = true, bool setNow = true)
			=> new AHookWin(Api.WH_KEYBOARD_LL, hookProc, setNow, 0, ignoreAuInjected);

		/// <summary>
		/// Sets a low-level mouse hook (WH_MOUSE_LL).
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
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
		/// var stop = false;
		/// using var hook = AHookWin.Mouse(x => {
		/// 	AOutput.Write(x);
		/// 	if(x.Event == HookData.MouseEvent.RightButton) { stop = x.IsButtonUp; x.BlockEvent(); }
		/// });
		/// ADialog.Show("hook");
		/// //or
		/// //AWaitFor.MessagesAndCondition(-10, () => stop); //wait max 10 s for right-click
		/// //AOutput.Write("the end");
		/// ]]></code>
		/// </example>
		public static AHookWin Mouse(Action<HookData.Mouse> hookProc, bool ignoreAuInjected = true, bool setNow = true)
			=> new AHookWin(Api.WH_MOUSE_LL, hookProc, setNow, 0, ignoreAuInjected);

		internal static AHookWin MouseRaw_(Func<LPARAM, LPARAM, bool> hookProc, bool ignoreAuInjected = true, bool setNow = true)
			=> new AHookWin(Api.WH_MOUSE_LL, hookProc, setNow, 0, ignoreAuInjected, "Mouse");

		/// <summary>
		/// Sets a WH_CBT hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
		/// <param name="hookProc">
		/// Hook procedure (function that handles hook events).
		/// Must return as soon as possible.
		/// If returns true, the event is cancelled. For some events you can modify some fields of event data.
		/// <note>When the hook procedure returns, the parameter variable becomes invalid and unsafe to use. If you need the data for later use, copy its properties and not the variable.</note>
		/// </param>
		/// <param name="threadId">Native thread id, or 0 for this thread. The thread must belong to this process.</param>
		/// <param name="setNow">Set hook now. Default true.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <example>
		/// <code><![CDATA[
		/// using var hook = AHookWin.ThreadCbt(x => {
		/// 	AOutput.Write(x.code);
		/// 	switch(x.code) {
		/// 	case HookData.CbtEvent.ACTIVATE:
		/// 		AOutput.Write(x.ActivationInfo(out _, out _));
		/// 		break;
		/// 	case HookData.CbtEvent.CREATEWND:
		/// 		AOutput.Write(x.CreationInfo(out var c, out _), c->x, c->lpszName);
		/// 		break;
		/// 	case HookData.CbtEvent.CLICKSKIPPED:
		/// 		AOutput.Write(x.MouseInfo(out var m), m->pt, m->hwnd);
		/// 		break;
		/// 	case HookData.CbtEvent.KEYSKIPPED:
		/// 		AOutput.Write(x.KeyInfo(out _));
		/// 		break;
		/// 	case HookData.CbtEvent.SETFOCUS:
		/// 		AOutput.Write(x.FocusInfo(out AWnd wPrev), wPrev);
		/// 		break;
		/// 	case HookData.CbtEvent.MOVESIZE:
		/// 		AOutput.Write(x.MoveSizeInfo(out var r), r->ToString());
		/// 		break;
		/// 	case HookData.CbtEvent.MINMAX:
		/// 		AOutput.Write(x.MinMaxInfo(out var state), state);
		/// 		break;
		/// 	case HookData.CbtEvent.DESTROYWND:
		/// 		AOutput.Write((AWnd)x.wParam);
		/// 		break;
		/// 	}
		/// 	return false;
		/// });
		/// ADialog.ShowOkCancel("hook");
		/// //new Form().ShowDialog(); //to test MINMAX
		/// ]]></code>
		/// </example>
		public static AHookWin ThreadCbt(Func<HookData.ThreadCbt, bool> hookProc, int threadId = 0, bool setNow = true)
			=> new AHookWin(Api.WH_CBT, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_GETMESSAGE hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
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
		/// using var hook = AHookWin.ThreadGetMessage(x => {
		/// 	AOutput.Write(x.msg->ToString(), x.PM_NOREMOVE);
		/// });
		/// ADialog.Show("hook");
		/// ]]></code>
		/// </example>
		public static AHookWin ThreadGetMessage(Action<HookData.ThreadGetMessage> hookProc, int threadId = 0, bool setNow = true)
			=> new AHookWin(Api.WH_GETMESSAGE, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_GETMESSAGE hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
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
		/// using var hook = AHookWin.ThreadKeyboard(x => {
		/// 	AOutput.Write(x.key, 0 != (x.lParam & 0x80000000) ? "up" : "", x.lParam, x.PM_NOREMOVE);
		/// 	return false;
		/// });
		/// ADialog.Show("hook");
		/// ]]></code>
		/// </example>
		public static AHookWin ThreadKeyboard(Func<HookData.ThreadKeyboard, bool> hookProc, int threadId = 0, bool setNow = true)
			=> new AHookWin(Api.WH_KEYBOARD, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_MOUSE hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
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
		/// using var hook = AHookWin.ThreadMouse(x => {
		/// 	AOutput.Write(x.message, x.m->pt, x.m->hwnd, x.PM_NOREMOVE);
		/// 	return false;
		/// });
		/// ADialog.Show("hook");
		/// ]]></code>
		/// </example>
		public static AHookWin ThreadMouse(Func<HookData.ThreadMouse, bool> hookProc, int threadId = 0, bool setNow = true)
			=> new AHookWin(Api.WH_MOUSE, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_CALLWNDPROC hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
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
		/// using var hook = AHookWin.ThreadCallWndProc(x => {
		/// 	ref var m = ref *x.msg;
		/// 	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam);
		/// 	AOutput.Write(mm, x.sentByOtherThread);
		/// });
		/// ADialog.Show("hook");
		/// ]]></code>
		/// </example>
		public static AHookWin ThreadCallWndProc(Action<HookData.ThreadCallWndProc> hookProc, int threadId = 0, bool setNow = true)
			=> new AHookWin(Api.WH_CALLWNDPROC, hookProc, setNow, threadId);

		/// <summary>
		/// Sets a WH_CALLWNDPROCRET hook for a thread of this process.
		/// See API <msdn>SetWindowsHookEx</msdn>.
		/// </summary>
		/// <returns>Returns a new <see cref="AHookWin"/> object that manages the hook.</returns>
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
		/// using var hook = AHookWin.ThreadCallWndProcRet(x => {
		/// 	ref var m = ref *x.msg;
		/// 	var mm = Message.Create(m.hwnd.Handle, (int)m.message, m.wParam, m.lParam); mm.Result = m.lResult;
		/// 	AOutput.Write(mm, x.sentByOtherThread);
		/// });
		/// ADialog.Show("hook");
		/// ]]></code>
		/// </example>
		public static AHookWin ThreadCallWndProcRet(Action<HookData.ThreadCallWndProcRet> hookProc, int threadId = 0, bool setNow = true)
			=> new AHookWin(Api.WH_CALLWNDPROCRET, hookProc, setNow, threadId);

		AHookWin(int hookType, Delegate hookProc, bool setNow, int tid, bool ignoreAuInjected = false, [CallerMemberName] string hookTypeString = null)
		{
			_proc2 = hookProc;
			_hookType = hookType;
			_hookTypeString = hookTypeString;
			_ignoreAuInjected = ignoreAuInjected;
			if(hookType == Api.WH_KEYBOARD_LL || hookType == Api.WH_MOUSE_LL) {
				_proc1 = _HookProcLL;
				//JIT-compile our hook proc and some functions it may call.
				//	Premature optimization? But OS gives us only 300 ms by default.
				if(!s_jit1) {
					s_jit1 = true;
					AJit.Compile(typeof(AHookWin), nameof(_HookProcLL));
					_ = ATime.PerfMilliseconds;
					_ = AKeys.KeyTypes_.IsMod(KKey.Shift) && _IgnoreMod;
				}
			} else {
				_proc1 = _HookProc;
			}
			if(setNow) Hook(tid);
		}
		static bool s_jit1;

		/// <summary>
		/// Sets the hook.
		/// </summary>
		/// <param name="threadId">If the hook type is a thread hook - thread id, or 0 for current thread. Else not used and must be 0.</param>
		/// <exception cref="AuException">Failed.</exception>
		/// <exception cref="InvalidOperationException">The hook is already set.</exception>
		/// <exception cref="ArgumentException">threadId not 0 and the hook type is not a thread hook.</exception>
		/// <remarks>
		/// Usually don't need to call this function, because the <b>AHookWin</b> static methods that return a new <b>AHookWin</b> object by default call it.
		/// </remarks>
		public void Hook(int threadId = 0)
		{
			if(_proc2 == null) throw new ObjectDisposedException(nameof(AHookWin));
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
				if(!ok) AWarning.Write($"AHookWin.Unhook failed ({_hookTypeString}). {ALastError.Message}");
				_hh = default;
			}
		}

		/// <summary>
		/// Returns true if the hook is set.
		/// </summary>
		public bool IsSet => _hh != default;

		///// <summary>
		///// Disable warning "Non-disposed AHookWin variable".
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

		/// <summary>
		/// Writes warning if the variable is not disposed. Cannot dispose in finalizer.
		/// </summary>
		~AHookWin()
		{
			//unhooking in finalizer thread makes no sense. Must unhook in same thread, else fails.
			if(_hh != default) AWarning.Write($"Non-disposed AHookWin ({_hookTypeString}) variable.");
		}

		unsafe LPARAM _HookProc(int code, LPARAM wParam, LPARAM lParam)
		{
			if(code >= 0) {
				try {
					bool eat = false;

					switch(_proc2) {
					case Func<HookData.ThreadCbt, bool> p:
						eat = p(new HookData.ThreadCbt(this, code, wParam, lParam));
						break;
					case Action<HookData.ThreadGetMessage> p:
						p(new HookData.ThreadGetMessage(this, wParam, lParam));
						break;
					case Func<HookData.ThreadKeyboard, bool> p:
						eat = p(new HookData.ThreadKeyboard(this, code, wParam, lParam));
						break;
					case Func<HookData.ThreadMouse, bool> p:
						eat = p(new HookData.ThreadMouse(this, code, wParam, lParam));
						break;
					case Action<HookData.ThreadCallWndProc> p:
						p(new HookData.ThreadCallWndProc(this, wParam, lParam));
						break;
					case Action<HookData.ThreadCallWndProcRet> p:
						p(new HookData.ThreadCallWndProcRet(this, wParam, lParam));
						break;
					}

					if(eat) return 1;
				}
				catch(Exception ex) { OnException_(ex); }
			}

			return Api.CallNextHookEx(default, code, wParam, lParam);
		}

		unsafe LPARAM _HookProcLL(int code, LPARAM wParam, LPARAM lParam)
		{
			if(code >= 0) {
				try {
					//using var p1 = APerf.Create();
					bool eat = false;
					long t1 = 0;
					Action<HookData.Mouse> pm1;
					Func<LPARAM, LPARAM, bool> pm2;

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
							//When AKeys.Lib.ReleaseModAndCapsLock sends Shift to turn off CapsLock,
							//	hooks receive a non-injected LShift down, CapsLock down/up and injected LShift up.
							//	Our triggers would recover, but cannot auto-repeat. Better don't call the hookproc.
							if((vk == KKey.CapsLock || vk == KKey.LShift) && _ignoreAuInjected && _IgnoreLShiftCaps) goto gr;

							//Test how our triggers recover when a modifier down or up event is lost. Or when triggers started while a modifier is down.
							//if(AKeys.IsScrollLock) {
							//	//if(vk == KKey.LCtrl && !kll->IsUp) { AOutput.Write("lost Ctrl down"); goto gr; }
							//	if(vk == KKey.LCtrl && kll->IsUp) { AOutput.Write("lost Ctrl up"); goto gr; }
							//}
						}
						if(AKeys.KeyTypes_.IsMod(vk) && _IgnoreMod) goto gr;
						t1 = ATime.PerfMilliseconds;
						//p1.Next();
						p(new HookData.Keyboard(this, lParam)); //info: wParam is message, but it is not useful, everything is in lParam
						if(eat = kll->BlockEvent) kll->BlockEvent = false;
						break;
					case Action<HookData.Mouse> p:
						pm1 = p; pm2 = null;
						gm1:
						var mll = (Api.MSLLHOOKSTRUCT*)lParam;
						switch((int)wParam) {
						case Api.WM_LBUTTONDOWN: case Api.WM_RBUTTONDOWN: Triggers.AutotextTriggers.ResetEverywhere = true; break;
						}
						if(_ignoreAuInjected && mll->IsInjectedByAu) goto gr;
						t1 = ATime.PerfMilliseconds;
						if(pm2 != null) {
							eat = pm2(wParam, lParam);
						} else {
							pm1(new HookData.Mouse(this, wParam, lParam));
							if(eat = mll->BlockEvent) mll->BlockEvent = false;
						}
						break;
					case Func<LPARAM, LPARAM, bool> p: //raw mouse
						pm2 = p; pm1 = null;
						goto gm1;
					}

					//Prevent Windows disabling the low-level key/mouse hook.
					//	Hook proc must return in HKEY_CURRENT_USER\Control Panel\Desktop:LowLevelHooksTimeout ms.
					//		Default 300. On Win10 max 1000 (bigger registry value is ignored and used 1000).
					//	On timeout Windows:
					//		1. Does not wait more. Passes the message to the next hook etc, and we cannot return 1 to block it.
					//		2. Kills the hook after several such cases. Usually 6 keys or 11 mouse events.
					//		3. Makes the hook useless: next times does not wait for it, and we cannot return 1 to block the event.
					//	Somehow does not apply 2 and 3 to some apps, eg C# apps (Core too) created by Visual Studio, although applies to those created not by VS. I did not find why.
					if(t1 != 0 && (t1 = ATime.PerfMilliseconds - t1) > 200 /*&& t1 < 5000*/ && !Debugger.IsAttached) {
						if(t1 > LowLevelHooksTimeout - 50) {
							var s1 = _hookType == Api.WH_KEYBOARD_LL ? "key" : "mouse";
							var s2 = eat ? $" On timeout the {s1} message is passed to the active window, other hooks, etc." : null;
							//AWarning.Write($"Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}"); //too slow first time
							//AOutput.Write($"Warning: Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}\r\n{new StackTrace(0, false)}"); //first Write() JIT 30 ms
							ThreadPool.QueueUserWorkItem(s3 => AOutput.Write(s3), $"Warning: Possible hook timeout. Hook procedure time: {t1} ms. LowLevelHooksTimeout: {LowLevelHooksTimeout} ms.{s2}\r\n{new StackTrace(0, false)}"); //fast if with false. But async print can be confusing.
						}
						//FUTURE: print warning if t1 is >25 frequently. Unhook and don't rehook if >LowLevelHooksTimeout frequently.

						Api.UnhookWindowsHookEx(_hh);
						_hh = Api.SetWindowsHookEx(_hookType, _proc1, default, 0);
					}

					if(eat) return 1;
				}
				catch(Exception ex) { OnException_(ex); }
			}
			gr:
			return Api.CallNextHookEx(default, code, wParam, lParam);
		}

		/// <summary>
		/// Gets the max time in milliseconds allowed by Windows for low-level keyboard and mouse hook procedures.
		/// </summary>
		/// <remarks>
		/// Gets registry value HKEY_CURRENT_USER\Control Panel\Desktop:LowLevelHooksTimeout. If it is missing, returns 300; it is the default value used by Windows. If greater than 1000, returns 1000, because Windows 10 ignores bigger values.
		/// 
		/// If a hook procedure takes more time, Windows does not wait. Then its return value is ignored, and the event is passed to other apps, hooks, etc. After several such cases Windows may fully or partially disable the hook. This class detects such cases; then restores the hook and writes a warning to the output. If the warning is rare, you can ignore it. If frequent, it means your hook procedure is too slow.
		/// 
		/// Callback functions of keyboard and mouse triggers are called in a hook procedure, therefore must be as fast as possible. More info: <see cref="Triggers.TriggerFuncs"/>.
		/// 
		/// More info: <msdn>registry LowLevelHooksTimeout</msdn>.
		/// 
		/// Note: After changing the timeout in registry, it is not applied immediately. Need to log off/on.
		/// </remarks>
		public static int LowLevelHooksTimeout {
			get {
				if(s_lowLevelHooksTimeout == 0) {
					if(!ARegistry.GetInt(out int i, "LowLevelHooksTimeout", @"Control Panel\Desktop")) i = 300; //default 300, tested on Win10 and 7
					else if((uint)i > 1000) i = 1000; //Win10. On Win7 the limit is bigger. Not tested on Win8. On Win7/8 may be changed by a Windows update.
					s_lowLevelHooksTimeout = i;
				}
				return s_lowLevelHooksTimeout;
			}
		}
		static int s_lowLevelHooksTimeout;

		internal static void OnException_(Exception e)
		{
			AWarning.Write("Unhandled exception in hook procedure. " + e.ToString());
		}

		[StructLayout(LayoutKind.Sequential, Size = 32)] //note: this struct is in shared memory. Size must be same in all library versions.
		internal struct SharedMemoryData_
		{
			public long dontBlockModUntil, dontBlocLShiftCapsUntil;
			//16 bytes reserved
		}

		/// <summary>
		/// Let other hooks (in all processes) ignore modifier keys for timeMS milliseconds. If 0 - restore.
		/// Used by mouse triggers.
		/// Returns the timeout time (ATime.WinMilliseconds + timeMS) or 0.
		/// </summary>
		internal unsafe long IgnoreModInOtherHooks_(long timeMS)
		{
			_ignoreModExceptThisHook = timeMS > 0;
			var r = _ignoreModExceptThisHook ? ATime.WinMilliseconds + timeMS : 0;
			SharedMemory_.Ptr->winHook.dontBlockModUntil = r;
			return r;
		}

		unsafe bool _IgnoreMod => SharedMemory_.Ptr->winHook.dontBlockModUntil > ATime.WinMilliseconds && !_ignoreModExceptThisHook;
		bool _ignoreModExceptThisHook;

		/// <summary>
		/// Let all hooks (in all processes) ignore LShift and CapsLock for timeMS milliseconds. If 0 - restore.
		/// Returns the timeout time (ATime.WinMilliseconds + timeMS) or 0.
		/// Used when turning off CapsLock with Shift.
		/// </summary>
		internal static unsafe long IgnoreLShiftCaps_(long timeMS)
		{
			var r = timeMS > 0 ? ATime.WinMilliseconds + timeMS : 0;
			SharedMemory_.Ptr->winHook.dontBlocLShiftCapsUntil = r;
			return r;
		}

		static unsafe bool _IgnoreLShiftCaps => SharedMemory_.Ptr->winHook.dontBlocLShiftCapsUntil > ATime.WinMilliseconds;
	}
}

namespace Au.Types
{
	/// <summary>
	/// Contains types of hook data for hook procedures set by <see cref="AHookWin"/> and <see cref="AHookAcc"/>.
	/// </summary>
	public static partial class HookData
	{
		/// <summary>
		/// Event data for the hook procedure set by <see cref="AHookWin.Keyboard"/>.
		/// More info: API <msdn>LowLevelKeyboardProc</msdn>.
		/// </summary>
		public unsafe struct Keyboard
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

			readonly Api.KBDLLHOOKSTRUCT* _x;

			internal Keyboard(AHookWin hook, LPARAM lParam)
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
			public KMod Mod => AKeys.Internal_.KeyToMod((KKey)_x->vkCode);

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
			/// Returns true if <i>key</i> == <b>vkCode</b> or <i>key</i> is Shift, Ctrl, Alt or Win and <b>vkCode</b> is LShift/RShift, LCtrl/RCtrl, LAlt/RAlt or RWin.
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
			internal byte SendInputFlags_ {
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

			internal Api.KBDLLHOOKSTRUCT* NativeStructPtr_ => _x;
		}

		/// <summary>
		/// Extra info value used by functions of this library that generate keyboard events. Low-level hooks receive it in <b>dwExtraInfo</b>.
		/// </summary>
		public const int AuExtraInfo = Api.AuExtraInfo;

		/// <summary>
		/// Hook data for the hook procedure set by <see cref="AHookWin.Mouse"/>.
		/// More info: API <msdn>LowLevelMouseProc</msdn>.
		/// </summary>
		public unsafe struct Mouse
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

			readonly Api.MSLLHOOKSTRUCT* _x;
			readonly MouseEvent _event;

			internal Mouse(AHookWin hook, LPARAM wParam, LPARAM lParam)
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

			internal Api.MSLLHOOKSTRUCT* NativeStructPtr_ => _x;
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
		/// Hook data for the hook procedure set by <see cref="AHookWin.ThreadCbt"/>.
		/// More info: API <msdn>CBTProc</msdn>.
		/// </summary>
		public struct ThreadCbt
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

			/// <summary>API <msdn>CBTProc</msdn></summary>
			public readonly CbtEvent code;

			/// <summary>API <msdn>CBTProc</msdn></summary>
			public readonly LPARAM wParam;

			/// <summary>API <msdn>CBTProc</msdn></summary>
			public readonly LPARAM lParam;

			internal ThreadCbt(AHookWin hook, int code, LPARAM wParam, LPARAM lParam)
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
			/// <param name="wPrevActive">The previously active window, or default(AWnd).</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.ACTIVATE.</exception>
			public unsafe AWnd ActivationInfo(out bool fMouse, out AWnd wPrevActive)
			{
				if(code != CbtEvent.ACTIVATE) throw new InvalidOperationException();
				var t = (Api.CBTACTIVATESTRUCT*)lParam;
				fMouse = t->fMouse;
				wPrevActive = t->hWndActive;
				return (AWnd)wParam;
			}

			/// <summary>
			/// Returns the window handle and gets more info about the created window.
			/// </summary>
			/// <param name="c">
			/// API <msdn>CREATESTRUCT</msdn>.
			/// You can modify x y cx cy.
			/// </param>
			/// <param name="wInsertAfter">Window whose position in the Z order precedes that of the window being created, or default(AWnd).</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.CREATEWND.</exception>
			public unsafe AWnd CreationInfo(out Native.CREATESTRUCT* c, out AWnd wInsertAfter)
			{
				if(code != CbtEvent.CREATEWND) throw new InvalidOperationException();
				var t = (Api.CBT_CREATEWND*)lParam;
				c = t->lpcs;
				wInsertAfter = t->hwndInsertAfter;
				return (AWnd)wParam;
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
			/// <param name="wLostFocus">The previously focused window, or default(AWnd).</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.SETFOCUS.</exception>
			public AWnd FocusInfo(out AWnd wLostFocus)
			{
				if(code != CbtEvent.SETFOCUS) throw new InvalidOperationException();
				wLostFocus = (AWnd)lParam;
				return (AWnd)wParam;
			}

			/// <summary>
			/// Returns the window handle and gets some more info about the move-size event.
			/// </summary>
			/// <param name="r">The new rectangle of the window.</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.MOVESIZE.</exception>
			public unsafe AWnd MoveSizeInfo(out RECT* r)
			{
				if(code != CbtEvent.MOVESIZE) throw new InvalidOperationException();
				r = (RECT*)lParam;
				return (AWnd)wParam;
			}

			/// <summary>
			/// Returns the window handle and gets some more info about the minimize-maximize-restore event.
			/// </summary>
			/// <param name="showState">The new show state. See API <msdn>ShowWindow</msdn>. Minimized 6, maximized 3, restored 9.</param>
			/// <exception cref="InvalidOperationException"><b>code</b> is not CbtEvent.MINMAX.</exception>
			public AWnd MinMaxInfo(out int showState)
			{
				if(code != CbtEvent.MINMAX) throw new InvalidOperationException();
				showState = (int)lParam & 0xffff;
				return (AWnd)wParam;
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
		/// Hook data for the hook procedure set by <see cref="AHookWin.ThreadGetMessage"/>.
		/// More info: API <msdn>GetMsgProc</msdn>.
		/// </summary>
		public unsafe struct ThreadGetMessage
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

			/// <summary>
			/// The message has not been removed from the queue, because called API <msdn>PeekMessage</msdn> with flag PM_NOREMOVE.
			/// </summary>
			public readonly bool PM_NOREMOVE;

			/// <summary>
			/// Message parameters.
			/// API <msdn>MSG</msdn>.
			/// </summary>
			public readonly Native.MSG* msg;

			internal ThreadGetMessage(AHookWin hook, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				PM_NOREMOVE = (uint)wParam == Api.PM_NOREMOVE;
				msg = (Native.MSG*)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure set by <see cref="AHookWin.ThreadKeyboard"/>.
		/// More info: API <msdn>KeyboardProc</msdn>.
		/// </summary>
		public struct ThreadKeyboard
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

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

			internal ThreadKeyboard(AHookWin hook, int code, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				PM_NOREMOVE = code == Api.HC_NOREMOVE;
				key = (KKey)(uint)wParam;
				this.lParam = (uint)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure set by <see cref="AHookWin.ThreadMouse"/>.
		/// More info: API <msdn>MouseProc</msdn>.
		/// </summary>
		public unsafe struct ThreadMouse
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

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

			internal ThreadMouse(AHookWin hook, int code, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				PM_NOREMOVE = code == Api.HC_NOREMOVE;
				message = (uint)wParam;
				m = (Native.MOUSEHOOKSTRUCT*)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure set by <see cref="AHookWin.ThreadCallWndProc"/>.
		/// More info: API <msdn>CallWndProc</msdn>.
		/// </summary>
		public unsafe struct ThreadCallWndProc
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

			/// <summary>
			/// True if the message was sent by another thread.
			/// </summary>
			public readonly bool sentByOtherThread; //note: incorrect info in MSDN

			/// <summary>
			/// Message parameters.
			/// API <msdn>CWPSTRUCT</msdn>.
			/// </summary>
			public readonly Native.CWPSTRUCT* msg;

			internal ThreadCallWndProc(AHookWin hook, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				sentByOtherThread = wParam;
				msg = (Native.CWPSTRUCT*)lParam;
			}
		}

		/// <summary>
		/// Hook data for the hook procedure set by <see cref="AHookWin.ThreadCallWndProcRet"/>.
		/// More info: API <msdn>CallWndRetProc</msdn>.
		/// </summary>
		public unsafe struct ThreadCallWndProcRet
		{
			/// <summary>The caller object of your hook procedure. For example can be used to unhook.</summary>
			public readonly AHookWin hook;

			/// <summary>
			/// True if the message was sent by another thread.
			/// </summary>
			public readonly bool sentByOtherThread; //note: incorrect info in MSDN

			/// <summary>
			/// Message parameters and the return value.
			/// API <msdn>CWPRETSTRUCT</msdn>.
			/// </summary>
			public readonly Native.CWPRETSTRUCT* msg;

			internal ThreadCallWndProcRet(AHookWin hook, LPARAM wParam, LPARAM lParam)
			{
				this.hook = hook;
				sentByOtherThread = wParam;
				msg = (Native.CWPRETSTRUCT*)lParam;
			}
		}

		/// <summary>
		/// Calls API API <msdn>ReplyMessage</msdn>, which allows to use <see cref="AAcc"/> and COM in the hook procedure.
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
