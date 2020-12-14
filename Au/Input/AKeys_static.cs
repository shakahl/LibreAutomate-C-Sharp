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
//using System.Linq;

using Au.Types;

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
		/// 1. When the key is pressed or released by software, such as the <see cref="Key"/> function of this library.
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
			/// Returns true if the specified key or mouse button is toggled.
			/// </summary>
			/// <remarks>
			/// Works only with CapsLock, NumLock, ScrollLock and several other keys, as well as mouse buttons.
			/// </remarks>
			public static bool IsToggled(KKey key) => 0 != (GetKeyState(key) & 1);

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
			public static bool IsMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt) {
				if (0 != (mod & KMod.Ctrl) && IsCtrl) return true;
				if (0 != (mod & KMod.Shift) && IsShift) return true;
				if (0 != (mod & KMod.Alt) && IsAlt) return true;
				if (0 != (mod & KMod.Win) && IsWin) return true;
				return false;
			}

			/// <summary>
			/// Gets flags indicating which modifier keys are down (pressed).
			/// </summary>
			/// <param name="mod">Check only these keys. Default: Ctrl, Shift, Alt.</param>
			/// <remarks>
			/// By default does not check the Win key, as it is not used in UI, but you can include it in <i>mod</i> if need.
			/// </remarks>
			public static KMod GetMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt) {
				KMod R = 0;
				if (0 != (mod & KMod.Ctrl) && IsCtrl) R |= KMod.Ctrl;
				if (0 != (mod & KMod.Shift) && IsShift) R |= KMod.Shift;
				if (0 != (mod & KMod.Alt) && IsAlt) R |= KMod.Alt;
				if (0 != (mod & KMod.Win) && IsWin) R |= KMod.Win;
				return R;
			}

			/// <summary>
			/// Returns true if the Caps Lock key is toggled.
			/// </summary>
			/// <remarks>
			/// The same as <see cref="AKeys.IsCapsLock"/>.
			/// </remarks>
			public static bool IsCapsLock => IsToggled(KKey.CapsLock);

			/// <summary>
			/// Returns true if the Num Lock key is toggled.
			/// </summary>
			/// <remarks>
			/// The same as <see cref="AKeys.IsNumLock"/>.
			/// </remarks>
			public static bool IsNumLock => IsToggled(KKey.NumLock);

			/// <summary>
			/// Returns true if the Scroll Lock key is toggled.
			/// </summary>
			/// <remarks>
			/// The same as <see cref="AKeys.IsScrollLock"/>.
			/// </remarks>
			public static bool IsScrollLock => IsToggled(KKey.ScrollLock);
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
		public static bool IsPressed(KKey key) {
			if ((key == KKey.MouseLeft || key == KKey.MouseRight) && 0 != Api.GetSystemMetrics(Api.SM_SWAPBUTTON)) key = (KKey)((int)key ^ 3); //makes this func 3 times slower, eg 2 -> 6 mcs when cold CPU. But much faster when called next time without a delay; for example AMouse.IsPressed(Left|Right) is not slower than AMouse.IsPressed(Left) in reality, although calls this func 2 times.
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
		public static bool IsMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win) {
			if (0 != (mod & KMod.Ctrl) && IsCtrl) return true;
			if (0 != (mod & KMod.Shift) && IsShift) return true;
			if (0 != (mod & KMod.Alt) && IsAlt) return true;
			if (0 != (mod & KMod.Win) && IsWin) return true;
			return false;
		}

		/// <summary>
		/// Gets flags indicating which modifier keys are down (pressed): Ctrl, Shift, Alt, Win. Calls <see cref="IsPressed"/>.
		/// Not for UI code (forms, WPF).
		/// </summary>
		/// <param name="mod">Check only these keys. Default - all four.</param>
		public static KMod GetMod(KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win) {
			KMod R = 0;
			if (0 != (mod & KMod.Ctrl) && IsCtrl) R |= KMod.Ctrl;
			if (0 != (mod & KMod.Shift) && IsShift) R |= KMod.Shift;
			if (0 != (mod & KMod.Alt) && IsAlt) R |= KMod.Alt;
			if (0 != (mod & KMod.Win) && IsWin) R |= KMod.Win;
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
		public static bool WaitForNoModifierKeys(double secondsTimeout = 0.0, KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win) {
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
		public static bool WaitForNoModifierKeysAndMouseButtons(double secondsTimeout = 0.0, KMod mod = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win, MButtons buttons = MButtons.Left | MButtons.Right | MButtons.Middle | MButtons.X1 | MButtons.X2) {
			var to = new AWaitFor.Loop(secondsTimeout, new AOptWaitFor(period: 2));
			for (; ; ) {
				if (!IsMod(mod) && !AMouse.IsPressed(buttons)) return true;
				if (!to.Sleep()) return false;
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
		public static bool WaitForReleased(double secondsTimeout, params KKey[] keys) {
			return AWaitFor.Condition(secondsTimeout, () => {
				foreach (var k in keys) if (IsPressed(k)) return false;
				return true;
			}, new AOptWaitFor(period: 2));
		}

		/// <summary>
		/// Waits while the specified keys are down (pressed).
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="keys">One or more keys. Waits until all are released. String like with <see cref="Key"/>, without operators.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="ArgumentException">Error in keys string.</exception>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		public static bool WaitForReleased(double secondsTimeout, string keys) {
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
		/// if(!AKeys.WaitForHotkey(-5, "Left")) AOutput.Write("timeout"); //returns false after 5 s
		/// ]]></code>
		/// </example>
		public static bool WaitForHotkey(double secondsTimeout, KHotkey hotkey, bool waitModReleased = false) {
			if (s_atomWFH == 0) s_atomWFH = Api.GlobalAddAtom("WaitForHotkey");
			using (ARegisteredHotkey rhk = default) {
				if (!rhk.Register(s_atomWFH, hotkey)) throw new AuException(0, "*register hotkey");
				if (!AWaitFor.PostedMessage(secondsTimeout, (ref Native.MSG m) => m.message == Api.WM_HOTKEY && m.wParam == s_atomWFH)) return false;
			}
			if (waitModReleased) return WaitForNoModifierKeys(secondsTimeout, hotkey.Mod);
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
		/// AOutput.Write("Ctrl");
		/// ]]></code>
		/// </example>
		public static bool WaitForKey(double secondsTimeout, KKey key, bool up = false, bool block = false) {
			if (key == 0) throw new ArgumentException();
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
		/// AOutput.Write("Ctrl");
		/// ]]></code>
		/// </example>
		public static bool WaitForKey(double secondsTimeout, string key, bool up = false, bool block = false) {
			return 0 != _WaitForKey(secondsTimeout, More.ParseKeyNameThrow_(key), up, block);
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
		/// AOutput.Write(key);
		/// ]]></code>
		/// </example>
		public static KKey WaitForKey(double secondsTimeout, bool up = false, bool block = false) {
			return _WaitForKey(secondsTimeout, 0, up, block);
		}

		static KKey _WaitForKey(double secondsTimeout, KKey key, bool up, bool block) {
			//SHOULDDO: if up and block: don't block if was down when starting to wait. Also in the Mouse func.

			KKey R = 0;
			using (AHookWin.Keyboard(x => {
				if (key != 0 && !x.IsKey(key)) return;
				if (x.IsUp != up) {
					if (up && block) { //key down when we are waiting for up. If block, now block down too.
						if (key == 0) key = x.vkCode;
						x.BlockEvent();
					}
					return;
				}
				R = x.vkCode; //info: for mod keys returns left/right
				if (block) x.BlockEvent();
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
			public static bool GetTextCursorRect(out RECT r, out AWnd w, bool orMouse = false) {
				if (AWnd.More.GetGUIThreadInfo(out var g) && !g.hwndCaret.Is0) {
					if (g.rcCaret.bottom <= g.rcCaret.top) g.rcCaret.bottom = g.rcCaret.top + 16;
					r = g.rcCaret;
					g.hwndCaret.MapClientToScreen(ref r);
					w = g.hwndCaret;
					return true;
				}
				if (orMouse) {
					Api.GetCursorPos(out var p);
					r = new RECT(p.x, p.y, 0, 16);
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
			public static KKey ParseKeyName(string keyName) {
				keyName ??= "";
				return _KeynameToKey(keyName, 0, keyName.Length);
			}

			/// <summary>
			/// Calls <see cref="ParseKeyName"/> and throws ArgumentException if invalid key string.
			/// </summary>
			/// <param name="keyName"></param>
			internal static KKey ParseKeyNameThrow_(string keyName) {
				var k = ParseKeyName(keyName);
				if (k == 0) throw new ArgumentException("Unknown key name or error in key string.");
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
			public static KKey ParseKeyName(string s, int startIndex, int length) {
				s ??= "";
				if ((uint)startIndex > s.Length || (uint)length > s.Length - startIndex) throw new ArgumentOutOfRangeException();
				return _KeynameToKey(s, startIndex, length);
			}

			/// <summary>
			/// Converts keys string to <see cref="KKey"/> array.
			/// </summary>
			/// <param name="keys">String containing one or more key names, like with <see cref="AKeys.Key"/>. Operators are not supported.</param>
			/// <exception cref="ArgumentException">Error in keys string.</exception>
			public static KKey[] ParseKeysString(string keys) {
				var a = new List<KKey>();
				foreach (var g in _SplitKeysString(keys ?? "")) {
					KKey k = _KeynameToKey(keys, g.Start, g.Length);
					if (k == 0) throw _ArgumentException_ErrorInKeysString(keys, g.Start, g.Length);
					a.Add(k);
				}
				return a.ToArray();
			}

			/// <summary>
			/// Converts string to <see cref="KKey"/> and <see cref="KMod"/>.
			/// For example, if s is "Ctrl+Left", sets mod=KMod.Ctrl, key=KKey.Left.
			/// Returns false if the string is invalid.
			/// </summary>
			/// <remarks>
			/// Key names are like with <see cref="AKeys.Key"/>.
			/// Must be single non-modifier key, preceded by zero or more of modifier keys Ctrl, Shift, Alt, Win, all joined with +.
			/// Valid hotkey examples: <c>"A"</c>, <c>"a"</c>, <c>"7"</c>, <c>"F12"</c>, <c>"."</c>, <c>"End"</c>, <c>"Ctrl+D"</c>, <c>"Ctrl+Alt+Shift+Win+Left"</c>, <c>" Ctrl + U "</c>.
			/// Invalid hotkey examples: null, "", <c>"A+B"</c>, <c>"Ctrl+A+K"</c>, <c>"A+Ctrl"</c>, <c>"Ctrl+Shift"</c>, <c>"Ctrl+"</c>, <c>"NoSuchKey"</c>, <c>"tab"</c>.
			/// </remarks>
			public static bool ParseHotkeyString(string s, out KMod mod, out KKey key) {
				key = 0; mod = 0;
				if (s == null) return false;
				int i = 0;
				foreach (var g in _SplitKeysString(s)) {
					if (key != 0) return false;
					if ((i++ & 1) == 0) {
						KKey k = _KeynameToKey(s, g.Start, g.Length);
						if (k == 0) return false;
						var m = Internal_.KeyToMod(k);
						if (m != 0) {
							if ((m & mod) != 0) return false;
							mod |= m;
						} else key = k;
					} else if (g.Length != 1 || s[g.Start] != '+') return false;
				}
				return key != 0 && key != KKey.Packet;
			}

			/// <summary>
			/// Converts string to winforms <see cref="System.Windows.Forms.Keys"/>.
			/// For example, if s is <c>"Ctrl+Left"</c>, sets hotkey=Keys.Control|Keys.Left.
			/// Returns false if the string is invalid or contains "Win".
			/// </summary>
			public static bool ParseHotkeyString(string s, out System.Windows.Forms.Keys hotkey) {
				if (!ParseHotkeyString(s, out var m, out var k)) { hotkey = 0; return false; }
				hotkey = KModToWinforms(m) | (System.Windows.Forms.Keys)k;
				if (m.Has(KMod.Win)) return false;
				return true;
				//return Enum.IsDefined(typeof(System.Windows.Forms.Keys), (System.Windows.Forms.Keys)k); //not too slow
				//tested: enum Keys has all KKey values + some extinct.
			}

			/// <summary>
			/// Converts string to WPF <see cref="System.Windows.Input.ModifierKeys"/> and <see cref="System.Windows.Input.Key"/> or <see cref="System.Windows.Input.MouseAction"/>.
			/// For example, if s is <c>"Ctrl+Left"</c>, sets mod=ModifierKeys.Control and key=Key.Left.
			/// Returns false if the string is invalid or contains incorrectly specified mouse buttons.
			/// Supported mouse button strings: "Click", "D-click", "R-click", "M-click", "Wheel". Example: "Ctrl+R-click". The first character of a mouse word is case-insensitive.
			/// </summary>
			public static bool ParseHotkeyString(string s, out System.Windows.Input.ModifierKeys mod, out System.Windows.Input.Key key, out System.Windows.Input.MouseAction mouse) {
				mod = 0; key = 0; mouse = 0;
				if(s.Ends("lick") || s.Ends("heel")) {
					int i = s.LastIndexOf('+') + 1;
					var v = s.AsSpan(i); var co = StringComparison.OrdinalIgnoreCase;
					if (v.Equals("Click", co)) mouse = System.Windows.Input.MouseAction.LeftClick;
					else if (v.Equals("D-click", co)) mouse = System.Windows.Input.MouseAction.LeftDoubleClick;
					else if (v.Equals("R-click", co)) mouse = System.Windows.Input.MouseAction.RightClick;
					else if (v.Equals("M-click", co)) mouse = System.Windows.Input.MouseAction.MiddleClick;
					else if (v.Equals("Wheel", co)) mouse = System.Windows.Input.MouseAction.WheelClick;
					if (mouse != default) {
						if (i == 0) return true;
						s = s.ReplaceAt(i, s.Length - i, "A");
					}
				}
				if (!ParseHotkeyString(s, out var m, out var k)) return false;
				mod = KModToWpf(m);
				return mouse != default || (key = KKeyToWpf(k)) != default;
				//tested: enum Key has all KKey values except mouse buttons and packet.
			}

			/// <summary>
			/// Used for parsing of hotkey triggers and mouse trigger modifiers.
			/// Like <see cref="ParseHotkeyString"/>, but supports 'any mod' (like "Shift?+K" or "?+K") and <i>noKey</i>.
			/// <i>noKey</i> - s can contain only modifiers, not key. If false, s must be "key" or "mod+key", else returns false. Else s must be "mod" or null/"", else returns false.
			/// </summary>
			internal static bool ParseHotkeyTriggerString_(string s, out KMod mod, out KMod modAny, out KKey key, bool noKey) {
				key = 0; mod = 0; modAny = 0;
				if (s.NE()) return noKey;
				int i = 0; bool ignore = false;
				foreach (var g in _SplitKeysString(s)) {
					if (ignore) { ignore = false; continue; }
					if (key != 0) return false;
					if ((i++ & 1) == 0) {
						KKey k = _KeynameToKey(s, g.Start, g.Length);
						if (k == 0) return false;
						var m = Internal_.KeyToMod(k);
						if (m != 0) {
							if ((m & (mod | modAny)) != 0) return false;
							if (ignore = g.End < s.Length && s[g.End] == '?') modAny |= m; //eg "Shift?+K"
							else mod |= m;
						} else {
							if (i == 1 && g.Length == 1 && s[g.Start] == '?') modAny = (KMod)15; //eg "?+K"
							else key = k;
						}
					} else if (g.Length != 1 || s[g.Start] != '+') return false;
				}
				if (noKey) return (mod | modAny) != 0 && key == 0;
				return key != 0;
			}

			/// <summary>
			/// Converts modifier key flags from <b>KMod</b> to winforms <b>Keys</b>.
			/// </summary>
			/// <remarks>
			/// For Win returns flag (Keys)0x80000.
			/// </remarks>
			public static System.Windows.Forms.Keys KModToWinforms(KMod mod) => (System.Windows.Forms.Keys)((int)mod << 16);

			/// <summary>
			/// Converts modifier key flags from winforms <b>Keys</b> to <b>KMod</b>.
			/// </summary>
			/// <remarks>
			/// For Win can be used flag (Keys)0x80000.
			/// </remarks>
			public static KMod KModFromWinforms(System.Windows.Forms.Keys mod) => (KMod)((int)mod >> 16);

			/// <summary>
			/// Converts modifier key flags from <b>KMod</b> to WPF <b>ModifierKeys</b>.
			/// </summary>
			public static System.Windows.Input.ModifierKeys KModToWpf(KMod mod) => (System.Windows.Input.ModifierKeys)_SwapMod((int)mod);

			/// <summary>
			/// Converts modifier key flags from WPF <b>ModifierKeys</b> to <b>KMod</b>.
			/// </summary>
			public static KMod KModFromWpf(System.Windows.Input.ModifierKeys mod) => (KMod)_SwapMod((int)mod);

			static int _SwapMod(int m) => (m & 0b1010) | (m << 2 & 4) | (m >> 2 & 1);

			/// <summary>
			/// Converts key from <b>KKey</b> to WPF <b>Key</b>.
			/// </summary>
			public static System.Windows.Input.Key KKeyToWpf(KKey k) => System.Windows.Input.KeyInterop.KeyFromVirtualKey((int)k);

			/// <summary>
			/// Converts key from WPF <b>Key</b> to <b>KKey</b>.
			/// </summary>
			public static KKey KKeyFromWpf(System.Windows.Input.Key k) => (KKey)System.Windows.Input.KeyInterop.VirtualKeyFromKey(k);
		}

		/// <summary>
		/// Generates virtual keystrokes (keys, text).
		/// </summary>
		/// <param name="keysEtc">
		/// Arguments of these types:
		/// <list type="bullet">
		/// <item><description>string - keys. Key names separated by spaces or operators, like <c>"Enter A Ctrl+A"</c>.
		/// Tool: in <c>""</c> string press Ctrl+Space.
		/// </description></item>
		/// <item><description>string with prefix "!" - literal text.
		/// Example: <c>var p = "pass"; AKeys.Key("!user", "Tab", "!" + p, "Enter");</c>
		/// </description></item>
		/// <item><description>string with prefix "%" - HTML to paste. Full or fragment.
		/// </description></item>
		/// <item><description><see cref="AClipboardData"/> - clipboard data to paste.
		/// </description></item>
		/// <item><description><see cref="KKey"/> - a single key.
		/// Example: <c>AKeys.Key("Shift+", KKey.Left, "*3");</c> is the same as <c>AKeys.Key("Shift+Left*3");</c>.
		/// </description></item>
		/// <item><description>int - sleep milliseconds. Max 10000.
		/// Example: <c>AKeys.Key("Left", 500, "Right");</c>
		/// </description></item>
		/// <item><description><see cref="Action"/> - callback function.
		/// Example: <c>Action click = () => AMouse.Click(); AKeys.Key("Shift+", click);</c>
		/// </description></item>
		/// <item><description><see cref="KKeyScan"/> - a single key, specified using scan code and/or virtual-key code and extended-key flag.
		/// Example: <c>AKeys.Key(new KKeyScan(0x3B, false)); //key F1</c>
		/// Example: <c>AKeys.Key(new KKeyScan(KKey.Enter, true)); //numpad Enter</c>
		/// </description></item>
		/// </list>
		/// </param>
		/// <exception cref="ArgumentException">An invalid value, for example an unknown key name.</exception>
		/// <remarks>
		/// Usually keys are specified in string, like in this example:
		/// <code><![CDATA[AKeys.Key("A F2 Ctrl+Shift+A Enter*2"); //keys A, F2, Ctrl+Shift+A, Enter Enter
		/// ]]></code>
		/// 
		/// Key names:
		/// <table>
		/// <tr>
		/// <th>Group</th>
		/// <th style="width:40%">Keys</th>
		/// <th>Info</th>
		/// </tr>
		/// <tr>
		/// <td>Named keys</td>
		/// <td>
		/// <b>Modifier:</b> <c>Alt</c>, <c>Ctrl</c>, <c>Shift</c>, <c>Win</c>, <c>RAlt</c>, <c>RCtrl</c>, <c>RShift</c>, <c>RWin</c>
		/// <br/><b>Navigate:</b> <c>Esc</c>, <c>End</c>, <c>Home</c>, <c>PgDn</c>, <c>PgUp</c>, <c>Down</c>, <c>Left</c>, <c>Right</c>, <c>Up</c>
		/// <br/><b>Other:</b> <c>Back</c>, <c>Del</c>, <c>Enter</c>, <c>Menu</c>, <c>Pause</c>, <c>PrtSc</c>, <c>Space</c>, <c>Tab</c>
		/// <br/><b>Function:</b> <c>F1</c>-<c>F24</c>
		/// <br/><b>Lock:</b> <c>CapsLock</c>, <c>NumLock</c>, <c>ScrollLock</c>, <c>Ins</c>
		/// </td>
		/// <td>Start with an uppercase character. Only the first 3 characters are significant; others can be any ASCII letters. For example, can be <c>"Back"</c>, <c>"Bac"</c>, <c>"Backspace"</c> or <c>"BACK"</c>, but not <c>"back"</c> or <c>"Ba"</c> or <c>"Back5"</c>.
		/// <br/>
		/// <br/>Alias: <c>AltGr</c> (RAlt), <c>App</c> (Menu), <c>PageDown</c> or <c>PD</c> (PgDn), <c>PageUp</c> or <c>PU</c> (PgUp), <c>PrintScreen</c> or <c>PS</c> (PrtSc), <c>BS</c> (Back), <c>PB</c> (Pause/Break), <c>CL</c> (CapsLock), <c>NL</c> (NumLock), <c>SL</c> (ScrollLock), <c>HM</c> (Home).
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Text keys</td>
		/// <td>
		/// <b>Alphabetic:</b> <c>A</c>-<c>Z</c> (or <c>a</c>-<c>z</c>)
		/// <br/><b>Number:</b> <c>0</c>-<c>9</c>
		/// <br/><b>Numeric keypad:</b> <c>#/</c> <c>#*</c> <c>#-</c> <c>#+</c> <c>#.</c> <c>#0</c>-<c>#9</c>
		/// <br/><b>Other:</b> <c>`</c> <c>-</c> <c>=</c> <c>[</c> <c>]</c> <c>\</c> <c>;</c> <c>'</c> <c>,</c> <c>.</c> <c>/</c>
		/// </td>
		/// <td>Spaces between keys are optional, except for uppercase A-Z. For example, can be <c>"A B"</c>, <c>"a b"</c>, <c>"A b"</c> or <c>"ab"</c>, but not <c>"AB"</c> or <c>"Ab"</c>.
		/// <br/>
		/// <br/>For <c>`</c> <c>-</c> <c>[</c> <c>]</c> <c>\</c> <c>;</c> <c>'</c> <c>,</c> <c>.</c> <c>/</c> also can be used <c>~</c> <c>_</c> <c>{</c> <c>}</c> <c>|</c> <c>:</c> <c>"</c> <c>&lt;</c> <c>&gt;</c> <c>?</c>.
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Other keys</td>
		/// <td>Names of enum <see cref="KKey"/> members.</td>
		/// <td>Example: <c>AKeys.Key("BrowserBack");</c>
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Other keys</td>
		/// <td>Virtual-key codes.</td>
		/// <td>Start with VK or Vk.
		/// Example: <c>AKeys.Key("VK65 VK0x42");</c>
		/// </td>
		/// </tr>
		/// <tr>
		/// <td>Forbidden</td>
		/// <td>Fn, Ctrl+Alt+Del, Win+L, some other.</td>
		/// <td>Programs cannot press these keys.</td>
		/// </tr>
		/// <tr>
		/// <td>Special characters</td>
		/// <td>
		/// <b>Operator:</b> + * ( )
		/// <br/><b>Numpad key prefix:</b> #
		/// <br/><b>Text/HTML argument prefix:</b> ! %
		/// <br/><b>Reserved:</b> @ $ ^ &amp;
		/// </td>
		/// <td>These characters cannot be used as keys. Instead use = 8 9 0 4 3 1 2 5 6 7.</td>
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
		/// <td><c>*n</c></td>
		/// <td><c>"Left*3"</c></td>
		/// <td>Press key n times, like <c>"Left Left Left"</c>.
		/// <br/>See <see cref="AddRepeat"/>.
		/// </td>
		/// <tr>
		/// <td><c>*down</c></td>
		/// <td><c>"Ctrl*down"</c></td>
		/// <td>Press key and don't release.</td>
		/// </tr>
		/// <tr>
		/// <td><c>*up</c></td>
		/// <td><c>"Ctrl*up"</c></td>
		/// <td>Release key.</td>
		/// </tr>
		/// </tr>
		/// <tr>
		/// <td><c>+</c></td>
		/// <td><c>"Ctrl+Shift+A"</c><br/><c>"Alt+E+P"</c></td>
		/// <td>The same as <c>"Ctrl*down Shift*down A Shift*up Ctrl*up"</c> and <c>"Alt*down E*down P E*up Alt*up"</c>.</td>
		/// </tr>
		/// <tr>
		/// <td><c>+()</c></td>
		/// <td><c>"Alt+(E P)"</c></td>
		/// <td>The same as <c>"Alt*down E P Alt*up"</c>.
		/// <br/>Inside () cannot be used + and +().
		/// </td>
		/// </tr>
		/// </table>
		/// 
		/// Operators and related keys can be in separate arguments. Examples: <c>AKeys.Key("Shift+", KKey.A); AKeys.Key(KKey.A, "*3");</c>.
		/// 
		/// Uses <see cref="AOpt.Key"/>:
		/// <table>
		/// <tr>
		/// <th>Option</th>
		/// <th>Default</th>
		/// <th>Changed</th>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.NoBlockInput"/></td>
		/// <td>false.
		/// Blocks user-pressed keys. Sends them afterwards.
		/// <br/>If the last argument is 'sleep', stops blocking before executing it; else stops blocking after executing all arguments.</td>
		/// <td>true.
		/// Does not block user-pressed keys.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.NoCapsOff"/></td>
		/// <td>false.
		/// If the CapsLock key is toggled, untoggles it temporarily (presses it before and after).</td>
		/// <td>true.
		/// Does not touch the CapsLock key.
		/// <br/>Alphabetic keys of "keys" arguments can depend on CapsLock. Text of "text" arguments doesn't depend on CapsLock, unless <see cref="AOptKey.TextHow"/> is <b>KeysX</b>.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.NoModOff"/></td>
		/// <td>false.
		/// Releases modifier keys (Alt, Ctrl, Shift, Win).
		/// <br/>Does it only at the start; later they cannot interfere, unless <see cref="AOptKey.NoBlockInput"/> is true.</td>
		/// <td>true.
		/// Does not touch modifier keys.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.TextSpeed"/></td>
		/// <td>0 ms.</td>
		/// <td>0 - 1000.
		/// Changes the speed for "text" arguments (makes slower).</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.KeySpeed"/></td>
		/// <td>1 ms.</td>
		/// <td>0 - 1000.
		/// Changes the speed for "keys" arguments (makes slower if &gt;1).</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.KeySpeedClipboard"/></td>
		/// <td>5 ms.</td>
		/// <td>0 - 1000.
		/// Changes the speed of Ctrl+V keys when pasting text or HTML using clipboard.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.SleepFinally"/></td>
		/// <td>10 ms.</td>
		/// <td>0 - 10000.
		/// <br/>Tip: to sleep finally, also can be used code like this: <c>AKeys.Key("keys", 1000);</c>.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.TextHow"/></td>
		/// <td><see cref="KTextHow.Characters"/></td>
		/// <td><b>KeysOrChar</b>, <b>KeysOrPaste</b> or <b>Paste</b>.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.PasteLength"/></td>
		/// <td>200.
		/// <br/>This option is used for "text" arguments. If text length &gt;= this value, uses clipboard.</td>
		/// <td>&gt;=0.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.PasteWorkaround"/></td>
		/// <td>false.
		/// <br/>This option is used for "text" arguments when using clipboard.
		/// </td>
		/// <td>true.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.RestoreClipboard"/></td>
		/// <td>true.
		/// Restore clipboard data (by default only text).
		/// <br/>This option is used for "text" and "HTML" arguments when using clipboard.</td>
		/// <td>false.
		/// Don't restore clipboard data.</td>
		/// </tr>
		/// <tr>
		/// <td><see cref="AOptKey.Hook"/></td>
		/// <td>null.</td>
		/// <td>Callback function that can modify options depending on active window etc.</td>
		/// </tr>
		/// </table>
		/// 
		/// This function does not wait until the target app receives and processes sent keystrokes and text; there is no reliable way to know it. It just adds small delays depending on options (<see cref="AOptKey.SleepFinally"/> etc). If need, change options or add 'sleep' arguments or wait after calling this function. Sending text through the clipboard normally does not have these problems.
		/// 
		/// Don't use this function to automate windows of own thread. Call it from another thread. See the last example.
		/// 
		/// Administrator and uiAccess processes don't receive keystrokes sent by standard user processes. See [](xref:uac).
		/// 
		/// Mouse button codes/names (eg <see cref="KKey.MouseLeft"/>) cannot be used to click. Instead use callback, like in the "Ctrl+click" example.
		/// 
		/// You can use an <see cref="AKeys"/> variable instead of this function. Example: <c>new AKeys(null).Add("keys", "!text").Send();</c>. More examples in <see cref="AKeys(AOptKey)"/> topic.
		/// 
		/// This function calls <see cref="Add(KKeysEtc[])"/>, which calls these functions depending on argument type: <see cref="AddKeys"/>, <see cref="AddText"/>, <see cref="AddClipboardData"/>, <see cref="AddKey(KKey, bool?)"/>, <see cref="AddKey(KKey, ushort, bool, bool?)"/>, <see cref="AddSleep"/>, <see cref="AddAction"/>. Then calls <see cref="Send"/>.
		/// 
		/// Uses API <msdn>SendInput</msdn>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// //Press key Enter.
		/// AKeys.Key("Enter");
		/// 
		/// //The same as above. The "AKeys." prefix is optional.
		/// AKeys.Key("Enter");
		/// 
		/// //Press keys Ctrl+A.
		/// AKeys.Key("Ctrl+A");
		/// 
		/// //Ctrl+Alt+Shift+Win+A.
		/// AKeys.Key("Ctrl+Alt+Shift+Win+A");
		/// 
		/// //Alt down, E, P, Alt up.
		/// AKeys.Key("Alt+(E P)");
		/// 
		/// //Alt down, E, P, Alt up.
		/// AKeys.Key("Alt*down E P Alt*up");
		/// 
		/// //Press key End, key Backspace 3 times, send text "Text".
		/// AKeys.Key("End Back*3", "!Text");
		/// 
		/// //Press Tab n times, send text "user", press Tab, send text "password", press Enter.
		/// int n = 5; string pw = "password";
		/// AKeys.Key($"Tab*{n}", "!user", "Tab", "!" + pw, "Enter");
		/// 
		/// //Send text "Example".
		/// AKeys.Text("Example");
		/// 
		/// //Press Ctrl+V, wait 500 ms, press Enter.
		/// AKeys.Key("Ctrl+V", 500, "Enter");
		/// 
		/// //F2, Ctrl+K, Left 3 times, Space, A, comma, 5, numpad 5, Shift+A, B, C, BrowserBack.
		/// AKeys.Key("F2 Ctrl+K Left*3 Space a , 5 #5 $abc", KKey.BrowserBack);
		/// 
		/// //Shift down, A 3 times, Shift up.
		/// AKeys.Key("Shift+A*3");
		/// 
		/// //Shift down, A 3 times, Shift up.
		/// AKeys.Key("Shift+", KKey.A, "*3");
		/// 
		/// //Shift down, A, wait 500 ms, B, Shift up.
		/// AKeys.Key("Shift+(", KKey.A, 500, KKey.B, ")");
		/// 
		/// //Send keys and text slowly.
		/// AOpt.Key.KeySpeed = AOpt.Key.TextSpeed = 50;
		/// AKeys.Key("keys Shift+: Space 123456789 Space 123456789 ,Space", "!text: 123456789 123456789\n");
		/// 
		/// //Ctrl+click
		/// Action click = () => AMouse.Click();
		/// AKeys.Key("Ctrl+", click);
		/// 
		/// //Ctrl+drag
		/// Action drag = () => { using(AMouse.LeftDown()) AMouse.MoveRelative(0, 50); };
		/// AKeys.Key("Ctrl+", drag);
		/// 
		/// //Ctrl+drag, poor man's version
		/// AKeys.Key("Ctrl*down");
		/// using(AMouse.LeftDown()) AMouse.MoveRelative(0, 50);
		/// AKeys.Key("Ctrl*up");
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
		/// b.Click += async (_, _) =>
		/// {
		/// 	//AKeys.Key("Tab", "!text", 2000, "Esc"); //no
		/// 	await Task.Run(() => { AKeys.Key("Tab", "!text", 2000, "Esc"); }); //use other thread
		/// };
		/// 
		/// f.ShowDialog();
		/// ]]></code>
		/// </example>
		public static void Key([ParamString(PSFormat.AKeys)] params KKeysEtc[] keysEtc) {
			new AKeys(AOpt.Key).Add(keysEtc).Send();
		}

		/// <summary>
		/// Sends text to the active window, using virtual keystrokes or clipboard. Calls <see cref="AddText"/>.
		/// </summary>
		/// <param name="text">Text. Can be null.</param>
		/// <param name="html">
		/// HTML. Can be full HTML or fragment. See <see cref="AClipboardData.AddHtml"/>.
		/// Can be specified only <i>text</i> or only <i>html</i> or both. If both, will paste <i>html</i> in apps that support it, elsewhere <i>text</i>. If only <i>html</i>, in apps that don't support HTML will paste <i>html</i> as text.
		/// </param>
		/// <remarks>
		/// To send text can use keys, characters or clipboard, depending on <see cref="AOpt.Key"/> and text. If <i>html</i> not null, uses clipboard.
		/// </remarks>
		/// <seealso cref="AClipboard.Paste"/>
		/// <example>
		/// <code><![CDATA[
		/// AKeys.Text("Text.\r\n");
		/// ]]></code>
		/// Or use function <see cref="Key"/> and prefix "!". For HTML use prefix "%".
		/// <code><![CDATA[
		/// AKeys.Key("!Send this text and press key", "Enter");
		/// AKeys.Key("%<b>bold</b> <i>italic</i>", "Enter");
		/// ]]></code>
		/// </example>
		public static void Text(string text, string html = null) {
			new AKeys(AOpt.Key).AddText(text, html).Send();
		}
	}
}

//FUTURE: instead of QM2 AutoPassword: FocusPasswordField(); AKeys.Key("!password", "Shift+Tab", "user", "Enter");
//public static void FocusPasswordField()
