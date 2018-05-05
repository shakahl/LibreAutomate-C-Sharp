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
	public partial class Keyb
	{
		#region get key state

		/// <summary>
		/// Returns true if Alt key is pressed.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsAlt => Api.GetKeyState(Keys.Menu) < 0;

		/// <summary>
		/// Returns true if Ctrl key is pressed.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsCtrl => Api.GetKeyState(Keys.ControlKey) < 0;

		/// <summary>
		/// Returns true if Shift key is pressed.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsShift => Api.GetKeyState(Keys.ShiftKey) < 0;

		/// <summary>
		/// Returns true if Win key is pressed.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsWin => Api.GetKeyState(Keys.LWin) < 0 || Api.GetKeyState(Keys.RWin) < 0;

		/// <summary>
		/// Returns true if some modifier keys are pressed: Ctrl, Shift, Alt, Win.
		/// </summary>
		/// <param name="modifierKeys">Check only these keys. Default - all.</param>
		/// <seealso cref="WaitFor.NoModifierKeys"/>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsMod(KMod modifierKeys = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win)
		{
			if(0 != (modifierKeys & KMod.Ctrl) && IsCtrl) return true;
			if(0 != (modifierKeys & KMod.Shift) && IsShift) return true;
			if(0 != (modifierKeys & KMod.Alt) && IsAlt) return true;
			if(0 != (modifierKeys & KMod.Win) && IsWin) return true;
			return false;
			//speed: slightly faster than GetKeyboardState with stackalloc.
		}

		/// <summary>
		/// Gets flags indicating which modifier keys are pressed: Ctrl, Shift, Alt, Win.
		/// </summary>
		/// <param name="modifierKeys">Check only these keys. Default - all.</param>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static KMod GetMod(KMod modifierKeys = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win)
		{
			KMod R = 0;
			if(0 != (modifierKeys & KMod.Ctrl) && IsCtrl) R |= KMod.Ctrl;
			if(0 != (modifierKeys & KMod.Shift) && IsShift) R |= KMod.Shift;
			if(0 != (modifierKeys & KMod.Alt) && IsAlt) R |= KMod.Alt;
			if(0 != (modifierKeys & KMod.Win) && IsWin) R |= KMod.Win;
			return R;
		}

		/// <summary>
		/// Returns true if the Caps Lock key is toggled.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsCapsLock => (Api.GetKeyState(Keys.CapsLock) & 1) != 0;

		/// <summary>
		/// Returns true if the Num Lock key is toggled.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsNumLock => (Api.GetKeyState(Keys.NumLock) & 1) != 0;

		/// <summary>
		/// Returns true if the Scroll Lock key is toggled.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsScrollLock => (Api.GetKeyState(Keys.Scroll) & 1) != 0;

		/// <summary>
		/// Calls API <msdn>GetKeyState</msdn> and returns its return value.
		/// </summary>
		/// <remarks>
		/// If returns &lt; 0, the key is pressed. If the low-order bit is 1, the key is toggled; not all keys can be toggled.
		/// This key state changes when the active app receives the key down or up notification.
		/// This and other 'get key state' functions of this library except <see cref="GetAsyncKeyState"/> use API <msdn>GetKeyState</msdn>.
		/// If a key is blocked by a hook, its state does not change. If it's a low-level hook, then it's impossible to detect whether the key is physically pressed.
		/// </remarks>
		public static short GetKeyState(Keys key) => Api.GetKeyState(key);

		/// <summary>
		/// Calls API <msdn>GetAsyncKeyState</msdn> and returns its return value.
		/// </summary>
		/// <remarks>
		/// If returns &lt; 0, the key is pressed.
		/// This key state changes when the OS receives the key down or up notification and before the active app receives the notification.
		/// If a key is blocked by a low-level hook, its state does not change. Then it's impossible to detect whether the key is physically pressed.
		/// </remarks>
		public static short GetAsyncKeyState(Keys key) => Api.GetAsyncKeyState(key);

		//rejected: rarely used; can use GetKeyState, GetAsyncKeyState, IsCtrl, etc.
		///// <summary>
		///// Returns true if the specified key or mouse button is pressed.
		///// </summary>
		///// <remarks>
		///// Calls API <msdn>GetKeyState</msdn>.
		///// For mouse buttons use Keys.LButton etc.
		///// </remarks>
		//public static bool IsKeyPressed(Keys key) => Api.GetKeyState(key) < 0;

		//rejected: rarely used; does not work with most keys; can use GetKeyState, IsCapsLock, etc.
		///// <summary>
		///// Returns true if the specified key or mouse button is toggled.
		///// </summary>
		///// <remarks>
		///// Calls API <msdn>GetKeyState</msdn>.
		///// For mouse buttons use Keys.LButton etc.
		///// Can get the toggled state of the lock keys, modifier keys and mouse buttons. Cannot get of most other keys.
		///// </remarks>
		//public static bool IsKeyToggled(Keys key) => (Api.GetKeyState(key) & 1) != 0;

		#endregion

		/// <summary>
		/// Miscellaneous keyboard-related functions.
		/// </summary>
		public static class Misc
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
			public static bool GetTextCursorRect(out RECT r, out Wnd w, bool orMouse = false)
			{
				if(Wnd.Misc.GetGUIThreadInfo(out var g) && !g.hwndCaret.Is0) {
					if(g.rcCaret.bottom <= g.rcCaret.top) g.rcCaret.bottom = g.rcCaret.top + 16;
					r = g.rcCaret;
					g.hwndCaret.MapClientToScreen(ref r);
					w = g.hwndCaret;
					return true;
				}
				if(orMouse) {
					Api.GetCursorPos(out var p);
					r = new RECT(p.X, p.Y, 0, 16, true);
				} else r = default;
				w = default;
				return false;

				//note: in Word, after changing caret pos, gets pos 0 0. After 0.5 s gets correct. After typing always correct.
				//tested: accessibleobjectfromwindow(objid_caret) is the same, but much slower.
			}

			/// <summary>
			/// Converts hotkey string to <see cref="Keys"/> and <see cref="KMod"/>.
			/// For example, if s is "Ctrl+Left", sets mod=KMod.Ctrl, key=Keys.Left.
			/// </summary>
			/// <remarks>
			/// Returns false if the string is invalid.
			/// Must be single non-modifier key, preceded by zero or more of modifier keys Ctrl, Shift, Alt, Win, all joined with +.
			/// The first character of named keys must be upper-case, eg "Left" and not "left". Keys A-Z don't have to match case.
			/// Valid hotkey examples: "A", "a", "7", "F12", ".", "End", "Ctrl+D", "Ctrl+Alt+Shift+Win+Left", " Ctrl + U ".
			/// Invalid hotkey examples: null, "", "A+B", "Ctrl+A+K", "A+Ctrl", "Ctrl+Shift", "Ctrl+", "NoSuchKey", "tab".
			/// </remarks>
			public static bool ParseHotkeyString(string s, out KMod mod, out Keys key)
			{
				return _ParseHotkeyString(s, out mod, out key, out _);
			}

			/// <summary>
			/// Converts hotkey string to <see cref="Keys"/>.
			/// For example, if s is "Ctrl+Left", sets hotkey=Keys.Control|Keys.Left.
			/// </summary>
			/// <inheritdoc cref="ParseHotkeyString(string, out KMod, out Keys)"/>
			public static bool ParseHotkeyString(string s, out Keys hotkey)
			{
				return _ParseHotkeyString(s, out _, out _, out hotkey);
			}

			static bool _ParseHotkeyString(string s, out KMod mod, out Keys key, out Keys modAndKey)
			{
				key = 0; mod = 0; modAndKey = 0;
				if(s == null) return false;
				s = s.Trim();
				for(int i = 0, eos = s.Length; i < eos; i++) {
					if(s[i] <= ' ') continue;
					int start = i;
					while(i < eos && s[i] != '+') i++;
					int len = i - start;
					while(len > 0 && s[start + len - 1] <= ' ') len--; //trim spaces before +
					if(len == 0) break;

					Keys k = _KeynameToKey(s, start, len); if(k == 0) break;
					var m = Lib.KeyToMod(k);
					bool lastKey = (i == eos);
					if(lastKey != (m == 0)) break;
					if(lastKey) {
						key = k;
						modAndKey = KModToKeys(mod) | key;
						return true;
					}
					mod |= m;
				}
				return false;
			}

			/// <summary>
			/// Converts modifier key flags from <b>KMod</b> to <b>Keys</b>.
			/// </summary>
			public static Keys KModToKeys(KMod mod) => (Keys)((int)mod << 16);

			/// <summary>
			/// Converts modifier key flags from <b>Keys</b> to <b>KMod</b>.
			/// </summary>
			/// <remarks>
			/// For Win returns flag (Keys)0x80000.
			/// </remarks>
			public static KMod KModFromKeys(Keys mod) => (KMod)((int)mod >> 16);
		}

		/// <summary>
		/// Sends virtual keystrokes to the active window. Also can send text, wait, etc.
		/// </summary>
		/// <param name="keysEtc">
		/// Any number of arguments of these types:
		/// <list type="bullet">
		/// <item>
		/// <term>string</term>
		/// <description>one or more key names separated by spaces or operators. More info in Remarks.
		/// Example: <c>Key("Enter A Ctrl+A");</c>
		/// See <see cref="AddKeys"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term>string after 'keys' string</term>
		/// <description>literal text. When there are several strings in sequence, they are interpreted as keys, text, keys, text...
		/// Example: <c>Key("keys", "text", "keys", "text", 500, "keys", "text", "keys", Keys.Back, "keys", "text");</c>
		/// Function <see cref="Text"/> is the same as <b>Key</b>, but the first parameter is text.
		/// To send text can be used keys or clipboard, depending on <see cref="Opt.Key"/> and text.
		/// See <see cref="AddText"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="Keys"/></term>
		/// <description>a single key.
		/// Example: <c>Key("Shift+", Keys.Left, "*3");</c> is the same as <c>Key("Shift+Left*3");</c>.
		/// See <see cref="AddKey(Keys, bool?)"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term>int</term>
		/// <description>milliseconds to sleep. Max 10000.
		/// Example: <c>Key("Left", 500, "Right");</c>
		/// See <see cref="AddSleep"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term><see cref="Action"/></term>
		/// <description>callback function.
		/// Example: <c>Action click = () => Mouse.Click(); Key("Shift+", click);</c>
		/// See <see cref="AddCallback"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term>null or ""</term>
		/// <description>nothing.
		/// Example: <c>Key("keys", 500, "", "text");</c>
		/// </description>
		/// </item>
		/// <item>
		/// <term>(int, bool)</term>
		/// <description>a single key, specified using scan code and extended-key flag.
		/// Example: <c>Key("", "key F1:", (0x3B, false));</c>
		/// See <see cref="AddKey(Keys, int, bool, bool?)"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <term>(Keys, int, bool)</term>
		/// <description>a single key, specified using <see cref="Keys"/> and/or scan code and extended-key flag.
		/// Example: <c>Key("", "numpad Enter:", (Keys.Enter, 0, true));</c>
		/// See <see cref="AddKey(Keys, int, bool, bool?)"/>.
		/// </description>
		/// </item>
		/// </list>
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
		/// <list type="table">
		/// <listheader>
		/// <term>Group</term>
		/// <term>Keys</term>
		/// <term>Info</term>
		/// </listheader>
		/// <item>
		/// <description>Named keys</description>
		/// <description>
		/// <b>Modifier:</b> Alt, Ctrl, Shift, Win
		/// <b>Right side:</b> RAlt, RCtrl, RShift, RWin
		/// <b>Lock:</b> CapsLock, NumLock, ScrollLock
		/// <b>Function:</b> F1-F24
		/// <b>Arrow:</b> Down, Left, Right, Up
		/// <b>Other:</b> Back, Del, End, Enter, Esc, Home, Ins, Menu, PgDown, PgUp, PrtSc, Space, Tab
		/// </description>
		/// <description>Start with an uppercase character. Only the first 3 characters are significant. For example, can be <c>"Back"</c>, <c>"Bac"</c>, <c>"Backspace"</c> or <c>"BACK"</c>, but not <c>"back"</c> or <c>"Ba"</c>.
		/// 
		/// Alternative key names: AltGr (RAlt), App (Menu), PageDown, PageUp, PrintScreen (PrtSc).
		/// </description>
		/// </item>
		/// <item>
		/// <description>Text keys</description>
		/// <description>
		/// <b>Alphabetic:</b> A-Z (or a-z)
		/// <b>Number:</b> 0-9
		/// <b>Numeric keypad:</b> #/ #* #- #+ #. #0-#9
		/// <b>Other:</b> =, ` - [ ] \ ; ' , . /
		/// </description>
		/// <description>Spaces between keys are optional, except for uppercase A-Z. For example, can be <c>"A B"</c>, <c>"a b"</c>, <c>"A b"</c> or <c>"ab"</c>, but not <c>"AB"</c> or <c>"Ab"</c>.
		/// 
		/// For ` - [ ] \ ; ' , . / also can be used ~ _ { } | : " &lt; &gt; ?.
		/// </description>
		/// </item>
		/// <item>
		/// <description>Other keys</description>
		/// <description><see cref="Keys"/> enum member names</description>
		/// <description>Start with an uppercase character.
		/// Example: <c>Key("BrowserBack", Keys.BrowserBack);</c>
		/// </description>
		/// </item>
		/// <item>
		/// <description>Forbidden</description>
		/// <description>Fn, Ctrl+Alt+Del, Win+L, some other</description>
		/// <description>Programs cannot press these keys.</description>
		/// </item>
		/// <item>
		/// <description>Special characters</description>
		/// <description><b>Operator:</b> + * ( ) $
		/// <b>Numpad key prefix:</b> #
		/// <b>Reserved:</b> ! @ % ^ &amp;
		/// </description>
		/// <description>These characters cannot be used as keys. Use = 8 9 0 4 3 1 2 5 6 7.</description>
		/// </item>
		/// </list>
		/// 
		/// Operators:
		/// <list type="table">
		/// <listheader>
		/// <term>Operator</term>
		/// <term>Examples</term>
		/// <term>Description</term>
		/// </listheader>
		/// <item>
		/// <description>+</description>
		/// <description><c>"Ctrl+Shift+A"</c><br/><c>"Alt+E+P"</c></description>
		/// <description>The same as <c>"Ctrl*down Shift*down A Shift*up Ctrl*up"</c> and <c>"Alt*down E*down P E*up Alt*up"</c>.</description>
		/// </item>
		/// <item>
		/// <description>+()</description>
		/// <description><c>"Alt+(E P)"</c></description>
		/// <description>The same as <c>"Alt*down E P Alt*up"</c>.
		/// Inside () cannot be used + or another +().
		/// </description>
		/// </item>
		/// <item>
		/// <description>*down</description>
		/// <description><c>"Ctrl*down"</c></description>
		/// <description>Press key and don't release.</description>
		/// </item>
		/// <item>
		/// <description>*up</description>
		/// <description><c>"Ctrl*up"</c></description>
		/// <description>Release key.</description>
		/// </item>
		/// <item>
		/// <description>*number</description>
		/// <description><c>"Left*3"</c></description>
		/// <description>Press key repeatedly, like <c>"Left Left Left"</c>.
		/// See <see cref="AddRepeat"/>.
		/// </description>
		/// </item>
		/// <item>
		/// <description>$</description>
		/// <description><c>"$text"</c></description>
		/// <description>$ is the same as Shift+.</description>
		/// </item>
		/// </list>
		/// Operators and related keys can be in separate arguments. Examples: <c>Key("Shift+", Keys.A); Key(Keys.A, "*3");</c>.
		/// 
		/// Uses <see cref="Opt.Key"/>:
		/// <list type="table">
		/// <listheader>
		/// <term>Option</term>
		/// <term>Default</term>
		/// <term>Changed</term>
		/// </listheader>
		/// <item>
		/// <description><see cref="OptKey.NoBlockInput" r=""/></description>
		/// <description>false.
		/// Blocks user-pressed keys. Sends them afterwards.
		/// If the last argument is 'sleep', stops blocking before executing it; else stops blocking after executing all arguments.</description>
		/// <description>true.
		/// Does not block user-pressed keys. It can be dangerous.</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.NoCapsOff" r=""/></description>
		/// <description>false.
		/// If the CapsLock key is toggled, untoggles it temporarily (presses it before and after).</description>
		/// <description>true.
		/// Does not touch the CapsLock key.
		/// Alphabetic keys of 'keys' arguments can depend on CapsLock. Text of 'text' arguments doesn't depend on CapsLock, unless <see cref="OptKey.TextOption" r=""/> is <see cref="KTextOption.Keys"/>.</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.NoModOff" r=""/></description>
		/// <description>false.
		/// Releases modifier keys (Alt, Ctrl, Shift, Win) if pressed.
		/// Does it only at the start; later they cannot interfere, unless <see cref="OptKey.NoBlockInput" r=""/> is true.</description>
		/// <description>true.
		/// Does not touch modifier keys. It can be dangerous.</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.KeySpeed" r=""/></description>
		/// <description>1 ms.</description>
		/// <description>0 - 1000.
		/// Changes the speed for 'keys' arguments (makes slower if &gt;1).</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.TextSpeed" r=""/></description>
		/// <description>0 ms.</description>
		/// <description>0 - 1000.
		/// Changes the speed for 'text' arguments (makes slower).</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.SleepFinally" r=""/></description>
		/// <description>10 ms.</description>
		/// <description>0 - 10000.
		/// Tip: to sleep finally, also can be used code like this: <c>Key("keys", 1000);</c>.</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.TextOption" r=""/></description>
		/// <description><see cref="KTextOption.Characters"/></description>
		/// <description><b>Keys</b> (send keys and Shift) or <b>Paste</b> (use clipboard).</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.PasteLength" r=""/></description>
		/// <description>300.
		/// This option is used for 'text' arguments. If text length &gt;= this value, uses the clipboard.</description>
		/// <description>&gt;=0.</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.PasteEnter" r=""/></description>
		/// <description>false.</description>
		/// <description>true.
		/// This option is used for 'text' arguments when using the clipboard.</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.RestoreClipboard" r=""/></description>
		/// <description>true.
		/// Restore clipboard data (by default only text).
		/// This option is used for 'text' arguments when using the clipboard.</description>
		/// <description>false.
		/// Don't restore clipboard data.</description>
		/// </item>
		/// <item>
		/// <description><see cref="OptKey.Hook" r=""/></description>
		/// <description>null.</description>
		/// <description>Callback function that can modify options depending on active window etc.</description>
		/// </item>
		/// </list>
		/// When you don't want to use or modify <see cref="Opt.Key"/>, use a <see cref="Keyb"/> variable instead of this function. Example: <c>new Keyb(null).Add("keys", "text").Send();</c>. More examples in <see cref="Keyb(OptKey)"/> topic.
		/// 
		/// This function does not wait until the target app receives and processes sent keystrokes and text; there is no reliable way to know it. It just adds small delays depending on options (<see cref="OptKey.SleepFinally" r=""/> etc). If need, change options or add 'sleep' arguments or wait after calling this function. Sending text through the clipboard normally does not have these problems.
		/// 
		/// This function should not be used to automate windows of own thread. In most cases it works, but strange problems are possible, because while waiting it gets/dispatches all messages/events/etc. It's better to call it from another thread. See the last example.
		/// 
		/// Administrator and uiAccess processes don't receive keystrokes sent by standard user processes. See <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink>.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// //Press key Enter.
		/// Keyb.Key("Enter");
		/// 
		/// //The same as above. The "Keyb." prefix is optional.
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
		/// Key("F2 Ctrl+K Left*3 Space a , 5 #5 $abc", Keys.BrowserBack);
		/// 
		/// //Shift down, A 3 times, Shift up.
		/// Key("Shift+A*3");
		/// 
		/// //Shift down, A 3 times, Shift up.
		/// Key("Shift+", Keys.A, "*3");
		/// 
		/// //Shift down, A, wait 500 ms, B, Shift up.
		/// Key("Shift+(", Keys.A, 500, Keys.B, ")");
		/// 
		/// //Send keys and text slowly.
		/// Opt.Key.KeySpeed = Opt.Key.TextSpeed = 50;
		/// Key("keys$:Space 123456789 Space 123456789 ,Space", "text: 123456789 123456789\n");
		/// 
		/// //Ctrl+click
		/// Action click = () => Mouse.Click();
		/// Key("Ctrl+", click);
		/// 
		/// //Ctrl+drag
		/// Action drag = () => { using(Mouse.LeftDown()) Mouse.MoveRelative(0, 50); };
		/// Key("Ctrl+", drag);
		/// 
		/// //Ctrl+drag, poor man's version
		/// Key("Ctrl*down");
		/// using(Mouse.LeftDown()) Mouse.MoveRelative(0, 50);
		/// Key("Ctrl*up");
		/// ]]></code>
		/// Show form and send keys/text to it when button clicked.
		/// <code><![CDATA[
		/// static void TestKeyOwnThread()
		/// {
		/// 	var f = new Form();
		/// 	var b = new Button() { Text = "Key" };
		/// 	var t = new TextBox() { Top = 100 };
		/// 	var c = new Button() { Text = "Close", Left = 100 };
		/// 	f.Controls.Add(b);
		/// 	f.Controls.Add(t);
		/// 	f.Controls.Add(c); f.CancelButton = c;
		/// 
		/// 	b.Click += async (unu, sed) =>
		/// 	{
		/// 		//Key("Tab", "text", 2000, "Esc"); //possible problems
		/// 		await Task.Run(() => { Key("Tab", "text", 2000, "Esc"); }); //use other thread
		/// 	};
		/// 
		/// 	f.ShowDialog();
		/// }
		/// ]]></code>
		/// </example>
		public static void Key(params object[] keysEtc)
		{
			new Keyb(Opt.Key).Add(keysEtc).Send();
		}

		/// <summary>
		/// Sends text to the active window, using virtual keystrokes or the clipboard. Also can send non-text keystrokes.
		/// </summary>
		/// <param name="text">Text to send.</param>
		/// <param name="keysEtc">Optional more parameters. The same as with <see cref="Key"/>. Can be used for example to press non-text keys, wait, send more text.</param>
		/// <exception cref="ArgumentException">An argument in <paramref name="keysEtc"/> is of an unsupported type or has an invalid value, for example an unknown key name.</exception>
		/// <remarks>
		/// This function is identical to <see cref="Key"/>, except: the first parameter is literal text (not keys). This example shows the difference: <c>Key("keys", "text", "keys", "text"); Text("text", "keys", "text", "keys");</c>.
		/// To send text can be used keys or clipboard, depending on <see cref="Opt.Key"/> and text.
		/// More info in <see cref="Key"/> topic.
		/// </remarks>
		/// <seealso cref="Clipb.PasteText"/>
		/// <example>
		/// <code><![CDATA[
		/// Keyb.Text("Text where key names like Enter are interpreted as text.\r\n");
		/// Keyb.Text("Send this text, press key", "Enter", "and wait", 500, "milliseconds. Enter");
		/// Text("Can be used without the \"Keyb.\" prefix.\n");
		/// ]]></code>
		/// </example>
		public static void Text(string text, params object[] keysEtc)
		{
			new Keyb(Opt.Key).AddText(text).Add(keysEtc).Send();
		}
	}

	public static partial class NoClass
	{
		/// <summary>
		/// Sends virtual keystrokes to the active window. Also can send text, wait, etc.
		/// Calls <see cref="Keyb.Key"/>.
		/// </summary>
		/// <inheritdoc cref="Keyb.Key"/>
		public static void Key(params object[] keysEtc) => Keyb.Key(keysEtc);

		/// <summary>
		/// Sends text to the active window, using virtual keystrokes or the clipboard. Then also can send non-text keystrokes.
		/// Calls <see cref="Keyb.Text"/>.
		/// </summary>
		/// <inheritdoc cref="Keyb.Text"/>
		public static void Text(string text, params object[] keysEtc) => Keyb.Text(text, keysEtc);
	}
}

namespace Au.Types
{
#pragma warning disable 1591 //missing doc
	/// <summary>
	/// Modifier keys as flags.
	/// </summary>
	/// <remarks>
	/// The values don't match those in the .NET enum <see cref="Keys"/>. This library does not use the .NET enum for modifier keys, mostly because it: does not have Win as modifier flag; confusing names, for example Alt and Menu.
	/// </remarks>
	/// <seealso cref="Keyb.Misc.KModToKeys"/>
	/// <seealso cref="Keyb.Misc.KModFromKeys"/>
	[Flags]
	public enum KMod
	{
		Shift = 1,
		Ctrl = 2,
		Alt = 4,
		Win = 8,
	}
#pragma warning restore 1591
}

//FUTURE: instead of QM2 AutoPassword: FocusPasswordField(); Text(password, "Tab", user, "Enter");
//public static void FocusPasswordField()
