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
using static Au.AStatic;

namespace Au
{
	public partial class AKeys
	{
		#region get key state

		/// <summary>
		/// Gets key states for using in UI code (forms, WPF).
		/// </summary>
		/// <remarks>
		/// Use functions of this class when processing user input events in user interface code (forms, WPF). In other code (automation scrits, etc) usually it's better to use functions of <see cref="AKeys"/> class. Functions of this class are similar to .NET's <b>Control.ModifierKeys</b>, <b>Keyboard.Modifiers</b> etc, but may be easier to use.
		/// 
		/// In Windows there are two API to get key state (down or up) - <msdn>GetKeyState</msdn> and <msdn>GetAsyncKeyState</msdn>. In most cases they return the same result, but not always.
		/// 
		/// API <b>GetAsyncKeyState</b> is used by class <see cref="AKeys"/> and not by this class (<b>AKeys.UI</b>). When the user (or some software) presses or releases a key, <b>GetAsyncKeyState</b> sees the change immediately. It is good in automation scripts, but not good in UI code because the state is not synchronized with the message queue.
		/// 
		/// This class (<b>AKeys.UI</b>) uses API <msdn>GetKeyState</msdn>. In the foreground thread (of the active window), it sees key state changes not immediately but after the thread reads key messages from its queue. It is good in UI threads. In background threads this API usually works like <b>GetAsyncKeyState</b>, but it depends on API <msdn>AttachThreadInput</msdn> and in some cases is less reliable, for example may be unaware of keys pressed before the thread started.
		/// 
		/// The key state returned by these API is not always the same as of the physical keyboard. There is no API to get physical state. The two most common cases when it is different:
		/// 1. When the key is pressed or released by software, such as the <b>Key</b> function of this library.
		/// 2. When the key is blocked by a low-level hook. For example, hotkey triggers of this library use hooks.
		/// 
		/// Also there is API <msdn>GetKeyboardState</msdn>. It gets states of all keys in single call. Works like <b>GetKeyState</b>.
		/// </remarks>
		public static class UI
		{
			/// <summary>
			/// Calls API <msdn>GetKeyState</msdn> and returns its return value.
			/// </summary>
			/// <remarks>
			/// If returns &lt; 0, the key is down (pressed). If the low-order bit is 1, the key is toggled; it works only with CapsLock, NumLock, ScrollLock and several other keys, as well as mouse buttons.
			/// Can be used for mouse buttons too, for example <c>AKeys.UI.GetKeyState(KKey.MouseLeft)</c>. When mouse left and right buttons are swapped, gets logical state, not physical.
			/// </remarks>
			public static short GetKeyState(KKey key) => Api.GetKeyState((int)key);

			/// <summary>
			/// Returns true if the specified key or mouse button is down (pressed).
			/// </summary>
			/// <remarks>
			/// Can be used for mouse buttons too, for example <c>AKeys.UI.IsPressed(KKey.MouseLeft)</c>. When mouse left and right buttons are swapped, gets logical state, not physical.
			/// </remarks>
			public static bool IsPressed(KKey key) => GetKeyState(key) < 0;

			/// <summary>
			/// Returns true if the Alt key is down (pressed).
			/// </summary>
			public static bool IsAlt => IsPressed(KKey.Alt);

			/// <summary>
			/// Returns true if the Ctrl key is down (pressed).
			/// </summary>
			public static bool IsCtrl => IsPressed(KKey.Ctrl);

			/// <summary>
			/// Returns true if the Shift key is down (pressed).
			/// </summary>
			public static bool IsShift => IsPressed(KKey.Shift);

			/// <summary>
			/// Returns true if the Win key is down (pressed).
			/// </summary>
			public static bool IsWin => IsPressed(KKey.Win) || IsPressed(KKey.RWin);

			/// <summary>
			/// Returns true if some modifier keys are down (pressed).
			/// </summary>
			/// <param name="mod">Return true if some of these keys are down (pressed). Default: Ctrl, Shift or Alt.</param>
			/// <remarks>
			/// By default does not check the Win key, as it is not used in UI, but you can include it in <i>mod</i> if need.
			/// </remarks>
			public static bool IsMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt)
			{
				if(0 != (mod & KMod.Ctrl) && IsCtrl) return true;
				if(0 != (mod & KMod.Shift) && IsShift) return true;
				if(0 != (mod & KMod.Alt) && IsAlt) return true;
				if(0 != (mod & KMod.Win) && IsWin) return true;
				return false;
			}

			/// <summary>
			/// Gets flags indicating which modifier keys are down (pressed).
			/// </summary>
			/// <param name="mod">Check only these keys. Default: Ctrl, Shift, Alt.</param>
			/// <remarks>
			/// By default does not check the Win key, as it is not used in UI, but you can include it in <i>mod</i> if need.
			/// </remarks>
			public static KMod GetMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt)
			{
				KMod R = 0;
				if(0 != (mod & KMod.Ctrl) && IsCtrl) R |= KMod.Ctrl;
				if(0 != (mod & KMod.Shift) && IsShift) R |= KMod.Shift;
				if(0 != (mod & KMod.Alt) && IsAlt) R |= KMod.Alt;
				if(0 != (mod & KMod.Win) && IsWin) R |= KMod.Win;
				return R;
			}

			/// <summary>
			/// Returns true if the Caps Lock key is toggled.
			/// </summary>
			/// <remarks>
			/// The same as <see cref="AKeys.IsCapsLock"/>.
			/// </remarks>
			public static bool IsCapsLock => (GetKeyState(KKey.CapsLock) & 1) != 0;

			/// <summary>
			/// Returns true if the Num Lock key is toggled.
			/// </summary>
			/// <remarks>
			/// The same as <see cref="AKeys.IsNumLock"/>.
			/// </remarks>
			public static bool IsNumLock => (GetKeyState(KKey.NumLock) & 1) != 0;

			/// <summary>
			/// Returns true if the Scroll Lock key is toggled.
			/// </summary>
			/// <remarks>
			/// The same as <see cref="AKeys.IsScrollLock"/>.
			/// </remarks>
			public static bool IsScrollLock => (GetKeyState(KKey.ScrollLock) & 1) != 0;
		}

		/// <summary>
		/// Returns true if the specified key or mouse button is down (pressed).
		/// Not for UI code (forms, WPF).
		/// </summary>
		/// <remarks>
		/// Uses API <msdn>GetAsyncKeyState</msdn>.
		/// When processing user input in UI code (forms, WPF), instead use class <see cref="AKeys.UI"/> or .NET functions. They use API <msdn>GetKeyState</msdn>.
		/// Can be used for mouse buttons too, for example <c>AKeys.IsPressed(KKey.MouseLeft)</c>. When mouse left and right buttons are swapped, gets logical state, not physical.
		/// </remarks>
		public static bool IsPressed(KKey key)
		{
			if((key == KKey.MouseLeft || key == KKey.MouseRight) && 0 != Api.GetSystemMetrics(Api.SM_SWAPBUTTON)) key = (KKey)((int)key ^ 3); //makes this func 3 times slower, eg 2 -> 6 mcs when cold CPU. But much faster when called next time without a delay; for example AMouse.IsPressed(Left|Right) is not slower than AMouse.IsPressed(Left) in reality, although calls this func 2 times.
			return Api.GetAsyncKeyState((int)key) < 0;
		}

		/// <summary>
		/// Returns true if the Alt key is down (pressed). Calls <see cref="IsPressed"/>.
		/// Not for UI code (forms, WPF).
		/// </summary>
		public static bool IsAlt => IsPressed(KKey.Alt);

		/// <summary>
		/// Returns true if the Ctrl key is down (pressed). Calls <see cref="IsPressed"/>.
		/// Not for UI code (forms, WPF).
		/// </summary>
		public static bool IsCtrl => IsPressed(KKey.Ctrl);

		/// <summary>
		/// Returns true if the Shift key is down (pressed). Calls <see cref="IsPressed"/>.
		/// Not for UI code (forms, WPF).
		/// </summary>
		public static bool IsShift => IsPressed(KKey.Shift);

		/// <summary>
		/// Returns true if the Win key is down (pressed). Calls <see cref="IsPressed"/>.
		/// Not for UI code (forms, WPF).
		/// </summary>
		public static bool IsWin => IsPressed(KKey.Win) || IsPressed(KKey.RWin);

		/// <summary>
		/// Returns true if some modifier keys are down (pressed): Ctrl, Shift, Alt, Win. Calls <see cref="IsPressed"/>.
		/// Not for UI code (forms, WPF).
		/// </summary>
		/// <param name="mod">Return true if some of these keys are pressed. Default - any (Ctrl, Shift, Alt or Win).</param>
		/// <seealso cref="WaitForNoModifierKeys"/>
		public static bool IsMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win)
		{
			if(0 != (mod & KMod.Ctrl) && IsCtrl) return true;
			if(0 != (mod & KMod.Shift) && IsShift) return true;
			if(0 != (mod & KMod.Alt) && IsAlt) return true;
			if(0 != (mod & KMod.Win) && IsWin) return true;
			return false;
		}

		/// <summary>
		/// Gets flags indicating which modifier keys are down (pressed): Ctrl, Shift, Alt, Win. Calls <see cref="IsPressed"/>.
		/// Not for UI code (forms, WPF).
		/// </summary>
		/// <param name="mod">Check only these keys. Default - all four.</param>
		public static KMod GetMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win)
		{
			KMod R = 0;
			if(0 != (mod & KMod.Ctrl) && IsCtrl) R |= KMod.Ctrl;
			if(0 != (mod & KMod.Shift) && IsShift) R |= KMod.Shift;
			if(0 != (mod & KMod.Alt) && IsAlt) R |= KMod.Alt;
			if(0 != (mod & KMod.Win) && IsWin) R |= KMod.Win;
			return R;
		}

		/// <summary>
		/// Returns true if the Caps Lock key is toggled.
		/// </summary>
		public static bool IsCapsLock => UI.IsCapsLock;

		/// <summary>
		/// Returns true if the Num Lock key is toggled.
		/// </summary>
		public static bool IsNumLock => UI.IsNumLock;

		/// <summary>
		/// Returns true if the Scroll Lock key is toggled.
		/// </summary>
		public static bool IsScrollLock => UI.IsScrollLock;

		#endregion

		#region wait

		/// <summary>
		/// Waits while some modifier keys (Ctrl, Shift, Alt, Win) are down (pressed). See <see cref="IsMod"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="mod">Check only these keys. Default: all.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		public static bool WaitForNoModifierKeys(double secondsTimeout = 0.0, KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win)
		{
			return WaitForNoModifierKeysAndMouseButtons(secondsTimeout, mod, 0);
		}

		/// <summary>
		/// Waits while some modifier keys (Ctrl, Shift, Alt, Win) or mouse buttons are down (pressed).
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="mod">Check only these keys. Default: all.</param>
		/// <param name="buttons">Check only these buttons. Default: all.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <seealso cref="IsMod"/>
		/// <seealso cref="AMouse.IsPressed"/>
		/// <seealso cref="AMouse.WaitForNoButtonsPressed"/>
		public static bool WaitForNoModifierKeysAndMouseButtons(double secondsTimeout = 0.0, KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win, MButtons buttons = MButtons.Left | MButtons.Right | MButtons.Middle | MButtons.X1 | MButtons.X2)
		{
			var to = new AWaitFor.Loop(secondsTimeout, 2);
			for(; ; ) {
				if(!IsMod(mod) && !AMouse.IsPressed(buttons)) return true;
				if(!to.Sleep()) return false;
			}
		}

		//public static bool WaitForKeyPressed(double secondsTimeout, KKey key)
		//{

		//	return false;
		//}

		/// <summary>
		/// Waits while the specified keys or/and mouse buttons are down (pressed).
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="keys">One or more keys or/and mouse buttons. Waits until all are released.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		public static bool WaitForReleased(double secondsTimeout, params KKey[] keys)
		{
			return AWaitFor.Condition(secondsTimeout, () => {
				foreach(var k in keys) if(IsPressed(k)) return false;
				return true;
			}, 2);
		}

		/// <summary>
		/// Waits while the specified keys are down (pressed).
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="keys">One or more keys. Waits until all are released. String like with <see cref="Key"/>, without operators.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="ArgumentException">Error in keys string.</exception>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		public static bool WaitForReleased(double secondsTimeout, string keys)
		{
			return WaitForReleased(secondsTimeout, More.ParseKeysString(keys));
		}

		/// <summary>
		/// Registers a temporary hotkey and waits for it.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="hotkey">See <see cref="ARegisteredHotkey.Register"/>.</param>
		/// <param name="waitModReleased">Also wait until hotkey modifier keys released.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="ArgumentException">Error in hotkey string.</exception>
		/// <exception cref="AuException">Failed to register hotkey.</exception>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <remarks>
		/// Uses <see cref="ARegisteredHotkey"/>; it uses API <msdn>RegisterHotKey</msdn>.
		/// Fails if the hotkey is currently registered by this or another application or used by Windows. Also if F12.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// AKeys.WaitForHotkey(0, "F11");
		/// AKeys.WaitForHotkey(0, KKey.F11);
		/// AKeys.WaitForHotkey(0, "Shift+A", true);
		/// AKeys.WaitForHotkey(0, (KMod.Ctrl | KMod.Shift, KKey.P)); //Ctrl+Shift+P
		/// AKeys.WaitForHotkey(0, Keys.Control | Keys.Alt | Keys.H); //Ctrl+Alt+H
		/// AKeys.WaitForHotkey(5, "Ctrl+Win+K"); //exception after 5 s
		/// if(!AKeys.WaitForHotkey(-5, "Left")) Print("timeout"); //returns false after 5 s
		/// ]]></code>
		/// </example>
		public static bool WaitForHotkey(double secondsTimeout, KHotkey hotkey, bool waitModReleased = false)
		{
			if(s_atomWFH == 0) s_atomWFH = Api.GlobalAddAtom("WaitForHotkey");
			using(ARegisteredHotkey rhk = default) {
				if(!rhk.Register(s_atomWFH, hotkey)) throw new AuException(0, "*register hotkey");
				if(!AWaitFor.PostedMessage(secondsTimeout, (ref Native.MSG m) => m.message == Api.WM_HOTKEY && m.wParam == s_atomWFH)) return false;
			}
			if(waitModReleased) return WaitForNoModifierKeys(secondsTimeout, hotkey.Mod);
			return true;
		}
		static ushort s_atomWFH;

		/// <summary>
		/// Waits for key-down or key-up event of the specified key.
		/// </summary>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="key">Wait for this key.</param>
		/// <param name="up">Wait for key-up event.</param>
		/// <param name="block">Make the event invisible for other apps. If <i>up</i> is true, makes the down event invisible too, if it comes while waiting for the up event.</param>
		/// <exception cref="ArgumentException"><i>key</i> is 0.</exception>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <remarks>
		/// Unlike <see cref="WaitForReleased"/>, waits for key event, not for key state.
		/// Uses low-level keyboard hook. Can wait for any single key. See also <see cref="WaitForHotkey"/>.
		/// Ignores key events injected by functions of this library.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// AKeys.WaitForKey(0, KKey.Ctrl, up: false, block: true);
		/// Print("Ctrl");
		/// ]]></code>
		/// </example>
		public static bool WaitForKey(double secondsTimeout, KKey key, bool up = false, bool block = false)
		{
			if(key == 0) throw new ArgumentException();
			return 0 != _WaitForKey(secondsTimeout, key, up, block);
		}

		/// <summary>
		/// Waits for key-down or key-up event of the specified key.
		/// </summary>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <param name="secondsTimeout"></param>
		/// <param name="key">Wait for this key. A single-key string like with <see cref="Key"/>.</param>
		/// <param name="up"></param>
		/// <param name="block"></param>
		/// <exception cref="ArgumentException">Invalid <i>key</i> string.</exception>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <example>
		/// <code><![CDATA[
		/// AKeys.WaitForKey(0, "Ctrl", up: false, block: true);
		/// Print("Ctrl");
		/// ]]></code>
		/// </example>
		public static bool WaitForKey(double secondsTimeout, string key, bool up = false, bool block = false)
		{
			return 0 != _WaitForKey(secondsTimeout, More.LibParseKeyNameThrow(key), up, block);
		}

		/// <summary>
		/// Waits for key-down or key-up event of any key, and gets the key code.
		/// </summary>
		/// <returns>
		/// Returns the key code. On timeout returns 0 if <i>secondsTimeout</i> is negative; else exception.
		/// For modifier keys returns the left or right key code, for example LCtrl/RCtrl, not Ctrl.
		/// </returns>
		/// <param name="secondsTimeout"></param>
		/// <param name="up"></param>
		/// <param name="block"></param>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <example>
		/// <code><![CDATA[
		/// var key = AKeys.WaitForKey(0, up: true, block: true);
		/// Print(key);
		/// ]]></code>
		/// </example>
		public static KKey WaitForKey(double secondsTimeout, bool up = false, bool block = false)
		{
			return _WaitForKey(secondsTimeout, 0, up, block);
		}

		static KKey _WaitForKey(double secondsTimeout, KKey key, bool up, bool block)
		{
			//SHOULDDO: if up and block: don't block if was down when starting to wait. Also in the Mouse func.

			KKey R = 0;
			using(AHookWin.Keyboard(x => {
				if(key != 0 && !x.IsKey(key)) return;
				if(x.IsUp != up) {
					if(up && block) { //key down when we are waiting for up. If block, now block down too.
						if(key == 0) key = x.vkCode;
						x.BlockEvent();
					}
					return;
				}
				R = x.vkCode; //info: for mod keys returns left/right
				if(block) x.BlockEvent();
			})) AWaitFor.MessagesAndCondition(secondsTimeout, () => R != 0);

			return R;
		}

		#endregion

		/// <summary>
		/// Miscellaneous rarely used keyboard-related functions.
		/// </summary>
		public static class More
		{
			/// <summary>
			/// Gets text cursor (caret) position and size.
			/// Returns false if fails.
			/// </summary>
			/// <param name="r">Receives the rectangle, in screen coordinates.</param>
			/// <param name="w">Receives the control that contains the text cursor.</param>
			/// <param name="orMouse">If fails, get mouse pointer coodinates.</param>
			/// <remarks>
			/// Can get only standard text cursor. Many apps use non-standard cursor; then fails.
			/// Also fails if the text cursor currently is not displayed.
			/// </remarks>
			public static bool GetTextCursorRect(out RECT r, out AWnd w, bool orMouse = false)
			{
				if(AWnd.More.GetGUIThreadInfo(out var g) && !g.hwndCaret.Is0) {
					if(g.rcCaret.bottom <= g.rcCaret.top) g.rcCaret.bottom = g.rcCaret.top + 16;
					r = g.rcCaret;
					g.hwndCaret.MapClientToScreen(ref r);
					w = g.hwndCaret;
					return true;
				}
				if(orMouse) {
					Api.GetCursorPos(out var p);
					r = (p.x, p.y, 0, 16);
				} else r = default;
				w = default;
				return false;

				//note: in Word, after changing caret pos, gets pos 0 0. After 0.5 s gets correct. After typing always correct.
				//tested: accessibleobjectfromwindow(objid_caret) is the same, but much slower.
			}

			/// <summary>
			/// Converts key name to <see cref="KKey"/>.
			/// Returns 0 if unknown key name.
			/// </summary>
			/// <param name="keyName">Key name, like with <see cref="AKeys.Key"/>.</param>
			public static KKey ParseKeyName(string keyName)
			{
				keyName = keyName ?? "";
				return _KeynameToKey(keyName, 0, keyName.Length);
			}

			/// <summary>
			/// Calls <see cref="ParseKeyName"/> and throws ArgumentException if invalid key string.
			/// </summary>
			/// <param name="keyName"></param>
			internal static KKey LibParseKeyNameThrow(string keyName)
			{
				var k = ParseKeyName(keyName);
				if(k == 0) throw new ArgumentException("Unknown key name or error in key string.");
				return k;
			}

			/// <summary>
			/// Converts key name to <see cref="KKey"/>.
			/// Returns 0 if unknown key name.
			/// </summary>
			/// <param name="s">String containing key name, like with <see cref="AKeys.Key"/>.</param>
			/// <param name="startIndex">Key name start index in <i>s</i>.</param>
			/// <param name="length">Key name length.</param>
			/// <exception cref="ArgumentOutOfRangeException">Invalid start index or length.</exception>
			public static KKey ParseKeyName(string s, int startIndex, int length)
			{
				s = s ?? "";
				if((uint)startIndex > s.Length || (uint)length > s.Length - startIndex) throw new ArgumentOutOfRangeException();
				return _KeynameToKey(s, startIndex, length);
			}

			/// <summary>
			/// Converts keys string to <see cref="KKey"/> array.
			/// </summary>
			/// <param name="keys">String containing one or more key names, like with <see cref="AKeys.Key"/>. Operators are not supported.</param>
			/// <exception cref="ArgumentException">Error in keys string.</exception>
			public static KKey[] ParseKeysString(string keys)
			{
				var a = new List<KKey>();
				foreach(var g in _SplitKeysString(keys ?? "")) {
					KKey k = _KeynameToKey(keys, g.Index, g.Length);
					if(k == 0) throw _ArgumentException_ErrorInKeysString(keys, g.Index, g.Length);
					a.Add(k);
				}
				return a.ToArray();
			}

			/// <summary>
			/// Converts hotkey string to <see cref="KKey"/> and <see cref="KMod"/>.
			/// For example, if s is "Ctrl+Left", sets mod=KMod.Ctrl, key=KKey.Left.
			/// Returns false if the string is invalid.
			/// </summary>
			/// <remarks>
			/// Key names are like with <see cref="AKeys.Key"/>.
			/// Must be single non-modifier key, preceded by zero or more of modifier keys Ctrl, Shift, Alt, Win, all joined with +.
			/// Valid hotkey examples: <c>"A"</c>, <c>"a"</c>, <c>"7"</c>, <c>"F12"</c>, <c>"."</c>, <c>"End"</c>, <c>"Ctrl+D"</c>, <c>"Ctrl+Alt+Shift+Win+Left"</c>, <c>" Ctrl + U "</c>.
			/// Invalid hotkey examples: null, "", <c>"A+B"</c>, <c>"Ctrl+A+K"</c>, <c>"A+Ctrl"</c>, <c>"Ctrl+Shift"</c>, <c>"Ctrl+"</c>, <c>"NoSuchKey"</c>, <c>"tab"</c>.
			/// </remarks>
			public static bool ParseHotkeyString(string s, out KMod mod, out KKey key)
			{
				key = 0; mod = 0;
				if(s == null) return false;
				int i = 0;
				foreach(var g in _SplitKeysString(s)) {
					if(key != 0) return false;
					if((i++ & 1) == 0) {
						KKey k = _KeynameToKey(s, g.Index, g.Length);
						if(k == 0) return false;
						var m = Lib.KeyToMod(k);
						if(m != 0) {
							if((m & mod) != 0) return false;
							mod |= m;
						} else key = k;
					} else if(g.Length != 1 || s[g.Index] != '+') return false;
				}
				return key != 0;
			}

			/// <summary>
			/// Converts hotkey string to <see cref="System.Windows.Forms.Keys"/>.
			/// For example, if s is <c>"Ctrl+Left"</c>, sets hotkey=Keys.Control|Keys.Left.
			/// Returns false if the string is invalid.
			/// </summary>
			public static bool ParseHotkeyString(string s, out System.Windows.Forms.Keys hotkey)
			{
				if(!ParseHotkeyString(s, out var mod, out var key)) { hotkey = 0; return false; }
				hotkey = KModToKeys(mod) | (System.Windows.Forms.Keys)key;
				return true;
			}

			/// <summary>
			/// Used for parsing of hotkey triggers and mouse trigger modifiers.
			/// Like <see cref="ParseHotkeyString"/>, but supports 'any mod' (like "Shift?+K" or "?+K") and <i>noKey</i>.
			/// <i>noKey</i> - s can contain only modifiers, not key. If false, s must be "key" or "mod+key", else returns false. Else s must be "mod" or null/"", else returns false.
			/// </summary>
			internal static bool LibParseHotkeyTriggerString(string s, out KMod mod, out KMod modAny, out KKey key, bool noKey)
			{
				key = 0; mod = 0; modAny = 0;
				if(Empty(s)) return noKey;
				int i = 0; bool ignore = false;
				foreach(var g in _SplitKeysString(s)) {
					if(ignore) { ignore = false; continue; }
					if(key != 0) return false;
					if((i++ & 1) == 0) {
						KKey k = _KeynameToKey(s, g.Index, g.Length);
						if(k == 0) return false;
						var m = Lib.KeyToMod(k);
						if(m != 0) {
							if((m & (mod | modAny)) != 0) return false;
							if(ignore = g.EndIndex < s.Length && s[g.EndIndex] == '?') modAny |= m; //eg "Shift?+K"
							else mod |= m;
						} else {
							if(i == 1 && g.Length == 1 && s[g.Index] == '?') modAny = (KMod)15; //eg "?+K"
							else key = k;
						}
					} else if(g.Length != 1 || s[g.Index] != '+') return false;
				}
				if(noKey) return (mod | modAny) != 0 && key == 0;
				return key != 0;
			}

			/// <summary>
			/// Converts modifier key flags from <b>KMod</b> to <b>Keys</b>.
			/// </summary>
			/// <remarks>
			/// For Win returns flag (Keys)0x80000.
			/// </remarks>
			public static System.Windows.Forms.Keys KModToKeys(KMod mod) => (System.Windows.Forms.Keys)((int)mod << 16);

			/// <summary>
			/// Converts modifier key flags from <b>Keys</b> to <b>KMod</b>.
			/// </summary>
			/// <remarks>
			/// For Win can be used flag (Keys)0x80000.
			/// </remarks>
			public static KMod KModFromKeys(System.Windows.Forms.Keys mod) => (KMod)((int)mod >> 16);
		}

		/// <summary>
		/// Sends virtual keystrokes to the active window. Also can send text, wait, etc.
		/// </summary>
		/// <param name="keysEtc">
		/// Any number of arguments of these types:
		/// - string - keys. One or more key names separated by spaces or operators. More info in Remarks.
		/// <br/>Example: <c>Key("Enter A Ctrl+A");</c>
		/// <br/>See <see cref="AddKeys"/>.
		/// - string after a "keys" string - literal text. When there are several strings in sequence, they are interpreted as keys, text, keys, text...
		/// <br/>Example: <c>Key("keys", "text", "keys", "text", 500, "keys", "text", "keys", KKey.Back, "keys", "text");</c>
		/// <br/>Function <see cref="Text"/> is the same as this function, but the first parameter is text.
		/// <br/>To send text can be used keys or clipboard, depending on <see cref="AOpt.Key"/> and text.
		/// <br/>See <see cref="AddText"/>.
		/// - <see cref="KKey"/> - a single key.
		/// <br/>Example: <c>Key("Shift+", KKey.Left, "*3");</c> is the same as <c>Key("Shift+Left*3");</c>.
		/// <br/>See <see cref="AddKey(KKey, bool?)"/>.
		/// - int - milliseconds to sleep. Max 10000.
		/// <br/>Example: <c>Key("Left", 500, "Right");</c>
		/// <br/>See <see cref="AddSleep"/>.
		/// - <see cref="Action"/> - callback function.
		/// <br/>Example: <c>Action click = () => AMouse.Click(); Key("Shift+", click);</c>
		/// <br/>See <see cref="AddCallback"/>.
		/// - null or "" - nothing.
		/// <br/>Example: <c>Key("keys", 500, "", "text");</c>
		/// - (int, bool) - a single key, specified using scan code and extended-key flag.
		/// <br/>Example: <c>Key("", "key F1:", (0x3B, false));</c>
		/// <br/>See <see cref="AddKey(KKey, int, bool, bool?)"/>.
		/// - (KKey, int, bool) - a single key, specified using <see cref="KKey"/> and/or scan code and extended-key flag.
		/// <br/>Example: <c>Key("", "numpad Enter:", (KKey.Enter, 0, true));</c>
		/// <br/>See <see cref="AddKey(KKey, int, bool, bool?)"/>.
		/// </param>
		/// <exception cref="ArgumentException">An argument is of an unsupported type or has an invalid value, for example an unknown key name.</exception>
		/// <remarks>
		/// Generates virtual keystrokes. Uses API <msdn>SendInput</msdn>. It works almost like real keyboard.
		/// 
		/// Usually keys are specified in string, like in this example:
		/// <code><![CDATA[Key("A F2 Ctrl+Shift+A Enter*2"); //keys A, F2, Ctrl+Shift+A, Enter Enter
		/// ]]></code>
		/// 
		/// Key names:
		/// <table>
		/// <tr>
		/// <th>Group</th>
		/// <th>Keys</th>
		/// <th>Info</th>
		/// </tr>
		/// <tr>
		/// <td>Named keys</td>
		/// <td>
		/// <b>Modifier:</b> Alt, Ctrl, Shift, Win
		/// <br/><b>Right side:</b> RAlt, RCtrl, RShift, RWin
		/// <br/><b>Lock:</b> CapsLock, NumLock, ScrollLock
		/// <br/><b>Function:</b> F1-F24
		/// <br/><b>Arrow:</b> Down, Left, Right, Up
		/// <br/><b>Other:</b> Back, Del, End, Enter, Esc, Home, Ins, Menu, PgDown, PgUp, PrtSc, Space, Tab
		/// </td>
		/// <td>Start with an uppercase character. Only the first 3 characters are significant; others can be any ASCII letters. For example, can be <c>"Back"</c>, <c>"Bac"</c>, <c>"Backspace"</c> or <c>"BACK"</c>, but not <c>"back"</c> or <c>"Ba"</c> or <c>"Back5"</c>.
		/// <br/>
		/// <br/>Alias: AltGr (RAlt), App (Menu), PageDown or PD (PgDn), PageUp or PU (PgUp), PrintScreen or PS (PrtSc), BS (Back), PB (Pause/Break), CL (CapsLock), NL (NumLock), SL (ScrollLock), HM (Home).
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Text keys</td>
		/// <td>
		/// <b>Alphabetic:</b> A-Z (or a-z)
		/// <br/><b>Number:</b> 0-9
		/// <br/><b>Numeric keypad:</b> #/ #* #- #+ #. #0-#9
		/// <br/><b>Other:</b> =, ` - [ ] \ ; ' , . /
		/// </td>
		/// <td>Spaces between keys are optional, except for uppercase A-Z. For example, can be <c>"A B"</c>, <c>"a b"</c>, <c>"A b"</c> or <c>"ab"</c>, but not <c>"AB"</c> or <c>"Ab"</c>.
		/// <br/>
		/// <br/>For ` - [ ] \ ; ' , . / can be used ~ _ { } | : " &lt; &gt; ?.
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Other keys</td>
		/// <td>Names of enum <see cref="KKey"/> members.</td>
		/// <td>Start with an uppercase character.
		/// <br/>Example: <c>Key("BrowserBack"); //KKey.BrowserBack</c>
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Other keys</td>
		/// <td>Virtual-key codes.</td>
		/// <td>Start with VK or Vk.
		/// <br/>Example: <c>Key("VK65 VK0x42");</c>
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Forbidden</td>
		/// <td>Fn, Ctrl+Alt+Del, Win+L, some other</td>
		/// <td>Programs cannot press these keys.</td>
		/// </tr>
		/// <tr>
		/// <td>Special characters</td>
		/// <td>
		/// <b>Operator:</b> + * ( ) $
		/// <br/><b>Numpad key prefix:</b> #
		/// <br/><b>Reserved:</b> ! @ % ^ &amp;
		/// </td>
		/// <td>These characters cannot be used as keys. Use = 8 9 0 4 3 1 2 5 6 7.</td>
		/// </tr>
		/// </table>
		/// 
		/// Operators:
		/// <table>
		/// <tr>
		/// <th>Operator</th>
		/// <th>Examples</th>
		/// <th>Description</th>
		/// </tr>
		/// <tr>
		/// <td>+</td>
		/// <td><c>"Ctrl+Shift+A"</c><br/><c>"Alt+E+P"</c></td>
		/// <td>The same as <c>"Ctrl*down Shift*down A Shift*up Ctrl*up"</c> and <c>"Alt*down E*down P E*up Alt*up"</c>.</td>
		/// </tr>
		/// <tr>
		/// <td>+()</td>
		/// <td><c>"Alt+(E P)"</c></td>
		/// <td>The same as <c>"Alt*down E P Alt*up"</c>.
		/// <br/>Inside () cannot be used + and +().
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>*down</td>
		/// <td><c>"Ctrl*down"</c></td>
		/// <td>Press key and don't release.</td>
		/// </tr>
		/// <tr>
		/// <td>*up</td>
		/// <td><c>"Ctrl*up"</c></td>
		/// <td>Release key.</td>
		/// </tr>
		/// <tr>
		/// <td>*number</td>
		/// <td><c>"Left*3"</c></td>
		/// <td>Press key repeatedly, like <c>"Left Left Left"</c>.
		/// <br/>See <see cref="AddRepeat"/>.
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>$</td>
		/// <td><c>"$text"</c></td>
		/// <td>$ is the same as Shift+.</td>
		/// </tr>
		/// </table>
		/// 
		/// Operators and related keys can be in separate arguments. Examples: <c>Key("Shift+", KKey.A); Key(KKey.A, "*3");</c>.
		/// 
		/// Uses <see cref="AOpt.Key"/>:
		/// <table>
		/// <tr>
		/// <th>Option</th>
		/// <th>Default</th>
		/// <th>Changed</th>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.NoBlockInput"/></td>
		/// <td>false.
		/// Blocks user-pressed keys. Sends them afterwards.
		/// <br/>If the last argument is 'sleep', stops blocking before executing it; else stops blocking after executing all arguments.</td>
		/// <td>true.
		/// Does not block user-pressed keys.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.NoCapsOff"/></td>
		/// <td>false.
		/// If the CapsLock key is toggled, untoggles it temporarily (presses it before and after).</td>
		/// <td>true.
		/// Does not touch the CapsLock key.
		/// <br/>Alphabetic keys of "keys" arguments can depend on CapsLock. Text of "text" arguments doesn't depend on CapsLock, unless <see cref="OptKey.TextOption"/> is <see cref="KTextOption.Keys"/>.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.NoModOff"/></td>
		/// <td>false.
		/// Releases modifier keys (Alt, Ctrl, Shift, Win).
		/// <br/>Does it only at the start; later they cannot interfere, unless <see cref="OptKey.NoBlockInput"/> is true.</td>
		/// <td>true.
		/// Does not touch modifier keys.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.TextSpeed"/></td>
		/// <td>0 ms.</td>
		/// <td>0 - 1000.
		/// Changes the speed for "text" arguments (makes slower).</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.KeySpeed"/></td>
		/// <td>1 ms.</td>
		/// <td>0 - 1000.
		/// Changes the speed for "keys" arguments (makes slower if &gt;1).</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.KeySpeedClipboard"/></td>
		/// <td>5 ms.</td>
		/// <td>0 - 1000.
		/// Changes the speed of Ctrl+V keys when text is pasted using the clipboard.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.SleepFinally"/></td>
		/// <td>10 ms.</td>
		/// <td>0 - 10000.
		/// <br/>Tip: to sleep finally, also can be used code like this: <c>Key("keys", 1000);</c>.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.TextOption"/></td>
		/// <td><see cref="KTextOption.Characters"/></td>
		/// <td><see cref="KTextOption.Keys"/> (send keys and Shift) or <see cref="KTextOption.Paste"/> (use clipboard).</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.PasteLength"/></td>
		/// <td>300.
		/// <br/>This option is used for "text" arguments. If text length &gt;= this value, uses the clipboard.</td>
		/// <td>&gt;=0.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.PasteEnter"/></td>
		/// <td>false.
		/// <br/>This option is used for "text" arguments when using the clipboard.
		/// </td>
		/// <td>true.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.RestoreClipboard"/></td>
		/// <td>true.
		/// Restore clipboard data (by default only text).
		/// <br/>This option is used for "text" arguments when using the clipboard.</td>
		/// <td>false.
		/// Don't restore clipboard data.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="OptKey.Hook"/></td>
		/// <td>null.</td>
		/// <td>Callback function that can modify options depending on active window etc.</td>
		/// </tr>
		/// </table>
		/// 
		/// When you don't want to use or modify <see cref="AOpt.Key"/>, you can use a <see cref="AKeys"/> variable instead of this function. Example: <c>new AKeys(null).Add("keys", "text").Send();</c>. More examples in <see cref="AKeys(OptKey)"/> topic.
		/// 
		/// This function does not wait until the target app receives and processes sent keystrokes and text; there is no reliable way to know it. It just adds small delays depending on options (<see cref="OptKey.SleepFinally"/> etc). If need, change options or add 'sleep' arguments or wait after calling this function. Sending text through the clipboard normally does not have these problems.
		/// 
		/// This function should not be used to automate windows of own thread. It may work or not. Call it from another thread. See the last example.
		/// 
		/// Administrator and uiAccess processes don't receive keystrokes sent by standard user processes. See [](xref:uac).
		/// 
		/// Mouse button codes/names (eg <see cref="KKey.MouseLeft"/>) cannot be used to click. Instead use callback, like in the "Ctrl+click" example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// //Press key Enter.
		/// AKeys.Key("Enter");
		/// 
		/// //The same as above. The "AKeys." prefix is optional.
		/// Key("Enter");
		/// 
		/// //Press keys Ctrl+A.
		/// Key("Ctrl+A");
		/// 
		/// //Ctrl+Alt+Shift+Win+A.
		/// Key("Ctrl+Alt+Shift+Win+A");
		/// 
		/// //Alt down, E, P, Alt up.
		/// Key("Alt+(E P)");
		/// 
		/// //Alt down, E, P, Alt up.
		/// Key("Alt*down E P Alt*up");
		/// 
		/// //Press key End, key Backspace 3 times, send text "Text".
		/// Key("End Back*3", "Text");
		/// 
		/// //Press Tab n times, send text "user", press Tab, send text "password", press Enter.
		/// int n = 5;
		/// Key($"Tab*{n}", "user", "Tab", "password", "Enter");
		/// 
		/// //Send text "Text".
		/// Text("Text");
		/// 
		/// //Send text "user", press Tab, send text "password", press Enter.
		/// Text("user", "Tab", "password", "Enter");
		/// 
		/// //Press Ctrl+V, wait 500 ms, press Enter.
		/// Key("Ctrl+V", 500, "Enter");
		/// 
		/// //Press Ctrl+V, wait 500 ms, send text "Text".
		/// Key("Ctrl+V", 500, "", "Text");
		/// 
		/// //F2, Ctrl+K, Left 3 times, Space, A, comma, 5, numpad 5, Shift+A, B, C, BrowserBack.
		/// Key("F2 Ctrl+K Left*3 Space a , 5 #5 $abc", KKey.BrowserBack);
		/// 
		/// //Shift down, A 3 times, Shift up.
		/// Key("Shift+A*3");
		/// 
		/// //Shift down, A 3 times, Shift up.
		/// Key("Shift+", KKey.A, "*3");
		/// 
		/// //Shift down, A, wait 500 ms, B, Shift up.
		/// Key("Shift+(", KKey.A, 500, KKey.B, ")");
		/// 
		/// //Send keys and text slowly.
		/// AOpt.Key.KeySpeed = AOpt.Key.TextSpeed = 50;
		/// Key("keys$:Space 123456789 Space 123456789 ,Space", "text: 123456789 123456789\n");
		/// 
		/// //Ctrl+click
		/// Action click = () => AMouse.Click();
		/// Key("Ctrl+", click);
		/// 
		/// //Ctrl+drag
		/// Action drag = () => { using(AMouse.LeftDown()) AMouse.MoveRelative(0, 50); };
		/// Key("Ctrl+", drag);
		/// 
		/// //Ctrl+drag, poor man's version
		/// Key("Ctrl*down");
		/// using(AMouse.LeftDown()) AMouse.MoveRelative(0, 50);
		/// Key("Ctrl*up");
		/// ]]></code>
		/// Show form and send keys/text to it when button clicked.
		/// <code><![CDATA[
		/// var f = new Form();
		/// var b = new Button { Text = "Key" };
		/// var t = new TextBox { Top = 100 };
		/// var c = new Button { Text = "Close", Left = 100 };
		/// f.Controls.Add(b);
		/// f.Controls.Add(t);
		/// f.Controls.Add(c); f.CancelButton = c;
		/// 
		/// b.Click += async (unu, sed) =>
		/// {
		/// 	//Key("Tab", "text", 2000, "Esc"); //no
		/// 	await Task.Run(() => { Key("Tab", "text", 2000, "Esc"); }); //use other thread
		/// };
		/// 
		/// f.ShowDialog();
		/// ]]></code>
		/// </example>
		public static void Key(params object[] keysEtc)
		{
			new AKeys(AOpt.Key).Add(keysEtc).Send();
		}

		/// <summary>
		/// Sends text to the active window, using virtual keystrokes or the clipboard. Also can send non-text keystrokes.
		/// </summary>
		/// <param name="text">Text to send.</param>
		/// <param name="keysEtc">Optional more parameters. The same as with <see cref="Key"/>. Can be used for example to press non-text keys, wait, send more text.</param>
		/// <exception cref="ArgumentException">An argument in <i>keysEtc</i> is of an unsupported type or has an invalid value, for example an unknown key name.</exception>
		/// <remarks>
		/// This function is identical to <see cref="Key"/>, except: the first parameter is literal text (not keys). This example shows the difference: <c>Key("keys", "text", "keys", "text"); Text("text", "keys", "text", "keys");</c>.
		/// To send text can be used keys or clipboard, depending on <see cref="AOpt.Key"/> and text.
		/// More info in <see cref="Key"/> topic.
		/// </remarks>
		/// <seealso cref="AClipboard.PasteText"/>
		/// <example>
		/// <code><![CDATA[
		/// AKeys.Text("Text where key names like Enter are interpreted as text.\r\n");
		/// AKeys.Text("Send this text, press key", "Enter", "and wait", 500, "milliseconds. Enter");
		/// Text("Can be used without the \"AKeys.\" prefix.\n");
		/// ]]></code>
		/// </example>
		public static void Text(string text, params object[] keysEtc)
		{
			new AKeys(AOpt.Key).AddText(text).Add(keysEtc).Send();
		}
	}

	public static partial class AStatic
	{
		/// <summary>
		/// Sends virtual keystrokes to the active window. Also can send text, wait, etc.
		/// Calls <see cref="AKeys.Key"/>.
		/// </summary>
		/// <exception cref="ArgumentException">An argument is of an unsupported type or has an invalid value, for example an unknown key name.</exception>
		public static void Key(params object[] keysEtc) => AKeys.Key(keysEtc);

		/// <summary>
		/// Sends text to the active window, using virtual keystrokes or the clipboard. Then also can send non-text keystrokes.
		/// Calls <see cref="AKeys.Text"/>.
		/// </summary>
		/// <exception cref="ArgumentException">An argument in <i>keysEtc</i> is of an unsupported type or has an invalid value, for example an unknown key name.</exception>
		public static void Text(string text, params object[] keysEtc) => AKeys.Text(text, keysEtc);
	}
}

//FUTURE: instead of QM2 AutoPassword: FocusPasswordField(); Text(password, "Tab", user, "Enter");
//public static void FocusPasswordField()
