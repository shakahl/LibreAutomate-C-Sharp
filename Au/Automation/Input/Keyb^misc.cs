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
		/// Returns true if Win key is pressed (left or right).
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
		/// Returns true if the Caps Lock key is locked.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsCapsLock => (Api.GetKeyState(Keys.CapsLock) & 1) != 0;

		/// <summary>
		/// Returns true if the Num Lock key is locked.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="GetKeyState"/>.
		/// </remarks>
		public static bool IsNumLock => (Api.GetKeyState(Keys.NumLock) & 1) != 0;

		/// <summary>
		/// Returns true if the Scroll Lock key is locked.
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
		/// Miscellaneous functions related to keyboard input.
		/// </summary>
		public static class Misc
		{
			/// <summary>
			/// Gets current text cursor (caret) rectangle in screen coordinates.
			/// Returns the control that contains it.
			/// If there is no text cursor or cannot get it (eg it is not a standard text cursor), gets mouse pointer coodinates and returns default(Wnd).
			/// </summary>
			public static Wnd GetTextCursorRect(out RECT r)
			{
				if(Wnd.Misc.GetGUIThreadInfo(out var g) && !g.hwndCaret.Is0) {
					if(g.rcCaret.bottom <= g.rcCaret.top) g.rcCaret.bottom = g.rcCaret.top + 16;
					r = g.rcCaret;
					g.hwndCaret.MapClientToScreen(ref r);
					return g.hwndCaret;
				}

				Api.GetCursorPos(out var p);
				r = new RECT(p.X, p.Y, 0, 16, true);
				return default;

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
					var m = _Util.KeyToMod(k);
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
		/// This static <see cref="KOptions"/> instance contains default options for all threads.
		/// </summary>
		/// <remarks>
		/// These options are copied to <see cref="Options"/> (a thread-static instance) when <b>Options</b> is used first time in a thread, explicitly or implicitly by <see cref="Key"/> and similar functions.
		/// You can change these options at the start of your script or in the static constructor of script's class. See the first example. Don't change later.
		/// Also can be used when creating <see cref="Keyb"/> instances; then they inherit these options. See the second example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// static MyScriptClass() { Keyb.StaticOptions.TimeKeyPressed = 20; } //static constructor
		/// ...
		/// Key("Tab Ctrl+V"); //uses Keyb.Options, which is implicitly copied from Keyb.StaticOptions
		/// ]]></code>
		/// Use a Keyb instance.
		/// <code><![CDATA[
		/// var k = new Keyb(Keyb.StaticOptions); //create new Keyb instance and copy options from Keyb.StaticOptions
		/// k.TimeKeyPressed = 100; //changes option of k but not of Keyb.StaticOptions
		/// k.Key("Tab Ctrl+V"); //uses options of k
		/// ]]></code>
		/// </example>
		public static KOptions StaticOptions => s_options;
		static KOptions s_options = new KOptions();

		/// <summary>
		/// This [ThreadStatic] <see cref="KOptions"/> instance is used by the static 'send keys or text' functions - <see cref="Key"/>, <see cref="Text"/>, <see cref="Paste"/> and similar.
		/// </summary>
		/// <remarks>
		/// Each thread has its own <b>Options</b> instance. It inherits options from <see cref="StaticOptions"/> when used first time in thread (explicitly or by these functions).
		/// Use it to set options for the static 'send keys or text' functions (<see cref="Key"/> etc), anywhere in your script. See the first example.
		/// Also can be used when creating <see cref="Keyb"/> instances; then they inherit these options. See the second example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Keyb.Options.TimeKeyPressed = 20;
		/// Key("Tab Ctrl+V"); //this function will use these options, unless called in another thread
		/// ]]></code>
		/// Use a Keyb instance.
		/// <code><![CDATA[
		/// var k = new Keyb(Keyb.Options); //create new Keyb instance and copy options from Keyb.Options
		/// k.TimeKeyPressed = 100; //changes option of k but not of Keyb.Options
		/// k.Key("Tab Ctrl+V"); //uses options of k
		/// ]]></code>
		/// </example>
		public static KOptions Options => t_options ?? (t_options = new KOptions(s_options));
		[ThreadStatic] static KOptions t_options;

		/// <summary>
		/// Sends keys and/or text to the active window or presses a hotkey.
		/// </summary>
		/// <param name="keys">
		/// Any number of arguments of these types:
		/// </param>
		/// <remarks>
		/// //TODO
		/// </remarks>
		public static void Key(params object[] keys)
		{
			new Keyb(Keyb.Options).Add(keys).Send();
		}
		//Example:
		//Key("keys", "text", "keys", "text", 500, "keys", "text", Keys.Back)

		//CONSIDER: Key("TimeKeyPressed=10 PasteText=true ...")

		/// <summary>
		/// Sends text and optionally keys to the active window.
		/// </summary>
		/// <param name="text">Text to send.</param>
		/// <param name="keys">Optional more parameters. The same as with <see cref="Keys"/>. Can be used for example to press keys or wait.</param>
		/// <remarks>
		/// This function is identical to <see cref="Keys"/>, except that the first parameter is interpreted as text, not as keys.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Keyb.Text("Text where key names like Enter are interpreted as text.\r\n");
		/// Keyb.Text("Send this text, press key", "Enter", "and wait", 500, "milliseconds. Enter");
		/// Text("Can be used without the \"Keyb.\" prefix.\n");
		/// ]]></code>
		/// </example>
		public static void Text(string text, params object[] keys)
		{
			new Keyb(Keyb.Options).AddText(text).Add(keys).Send();
		}
	}

	public static partial class NoClass
	{
		/// <summary>
		/// Sends keys and/or text to the active window or presses a hotkey.
		/// Alias of <see cref="Keyb.Key"/>.
		/// </summary>
		/// <inheritdoc cref="Keyb.Key"/>
		public static void Key(params object[] keys) => Keyb.Key(keys);

		/// <summary>
		/// Sends text and optionally keys to the active window.
		/// Alias of <see cref="Keyb.Text"/>.
		/// </summary>
		/// <inheritdoc cref="Keyb.Text"/>
		public static void Text(string text, params object[] keys) => Keyb.Text(text, keys);
	}
}

namespace Au.Types
{
	/// <summary>
	/// Options for class <see cref="Keyb"/>.
	/// </summary>
	/// <seealso cref="Keyb.Options"/>
	/// <seealso cref="Keyb.StaticOptions"/>
	public class KOptions
	{
		//FUTURE: let the default KOptions use a config file or something, because may need different options for different windows.
		//	Currently it is possible with Hook.

		/// <summary>
		/// Initializes this instance with default values or values copied from another instance.
		/// </summary>
		/// <param name="cloneOptions">If not null, copies its options into this variable.</param>
		public KOptions(KOptions cloneOptions = null)
		{
			_clonedFrom = cloneOptions;
			ResetOptions();
		}

		/// <summary>
		/// Resets options to the default values or to the current values of the <b>KOptions</b> instance passed to the constructor.
		/// </summary>
		public void ResetOptions() //note: named not Reset because KOptions is base of Keyb
		{
			var o = _clonedFrom;
			if(o != null) {
				_timeKeyPressed = o._timeKeyPressed;
				_timeTextChar = o._timeTextChar;
				_sleepFinally = o._sleepFinally;
				TextOption = o.TextOption;
				_pasteLength = o._pasteLength;
				PasteEnter = o.PasteEnter;
				NoModOff = o.NoModOff;
				NoCapsOff = o.NoCapsOff;
				Hook = o.Hook;
			} else {
				_timeKeyPressed = 1;
				_timeTextChar = 0f;
				_sleepFinally = 10;
				_pasteLength = 300;
				TextOption = KTextOption.Characters;
				//TODO: set all others to 0/false
			}
		}
		KOptions _clonedFrom;

		/// <summary>
		/// Wait milliseconds between each key down and up event of 'keys' parameters of <b>Key</b> and similar functions.
		/// Default: 1.
		/// </summary>
		/// <value>Time to sleep, milliseconds. Valid values: 0 - 1000 (1 second). Valid values for <see cref="Keyb.StaticOptions"/>: 0 - 10.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used for 'text' parameters; see <see cref="TimeTextChar"/>.
		/// </remarks>
		public int TimeKeyPressed
		{
			get => _timeKeyPressed;
			set => _timeKeyPressed = _SetValue(value, 1000, 10);
		}
		int _timeKeyPressed;

		/// <summary>
		/// Wait milliseconds between each key down and up event of 'text' parameters of <b>Text</b>, <b>Key</b> and similar functions.
		/// Default: 0.
		/// </summary>
		/// <value>
		/// Time to sleep, milliseconds. Valid values: 0 - 1000 (1 second). Valid values for <see cref="Keyb.StaticOptions"/>: 0 - 10.
		/// If less than 1, sleeps 1 ms every several characters. For example, if 0.2, sleeps 1 ms every 5 characters.
		/// </value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// Not used for 'keys' parameters; see <see cref="TimeKeyPressed"/>.
		/// </remarks>
		public double TimeTextChar
		{
			get => _timeTextChar;
			set => _timeTextChar = (float)_SetValue(value, 1000d, 10d);
		}
		float _timeTextChar;

		/// <summary>
		/// Wait milliseconds before a 'send keys' function returns.
		/// Default: 10.
		/// </summary>
		/// <value>Time to sleep, milliseconds. Valid values: 0 - 10000 (10 seconds). Valid values for <see cref="Keyb.StaticOptions"/>: 0 - 100.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <remarks>
		/// </remarks>
		public int SleepFinally
		{
			get => _sleepFinally;
			set => _sleepFinally = _SetValue(value, 10000, 100);
		}
		int _sleepFinally;

		bool _IsStatic => this == Keyb.StaticOptions;

		T _SetValue<T>(T value, T max, T rootMax) where T : IComparable<T>
		{
			bool isRoot = this == Keyb.StaticOptions;
			var m = isRoot ? rootMax : max;
			if(value.CompareTo(m) > 0 || value.CompareTo(default) < 0) throw new ArgumentOutOfRangeException(null, "Max " + m.ToString());
			return value;
		}

		/// <summary>
		/// How functions send text.
		/// Default: <see cref="KTextOption.Characters"/>.
		/// </summary>
		public KTextOption TextOption { get; set; }

		/// <summary>
		/// To send text use <see cref="KTextOption.Paste"/> if text length is &gt;= this value.
		/// Default: 300.
		/// </summary>
		/// <value>Valid values: 0 - int.MaxValue.</value>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public int PasteLength
		{
			get => _pasteLength;
			set => _pasteLength = _SetValue(value, int.MaxValue, int.MaxValue);
		}
		int _pasteLength;

		/// <summary>
		/// When pasting text that ends with newline characters, remove the last newline and after pasting send the Enter key.
		/// Default: false.
		/// </summary>
		/// <remarks>
		/// Some apps remove the last newline when pasting. For example Word, WordPad, OpenOffice, LibreOffice, standard rich text controls. This option is a workaround.
		/// </remarks>
		public bool PasteEnter { get; set; }

		//rejected: rarely used. Eg can be useful for Python programmers. Let call Paste() explicitly or set the Paste option eg in hook.
		///// <summary>
		///// To send text use <see cref="KTextOption.Paste"/> if text contains characters '\n' followed by '\t' (tab) or spaces.
		///// </summary>
		///// <remarks>
		///// Some apps auto-indent. This option is a workaround.
		///// </remarks>
		//public bool PasteMultilineIndented { get; set; }

		//rejected: WmPaste. Few windows support it.

		/// <summary>
		/// When starting, don't release modifier keys.
		/// Default: false.
		/// </summary>
		public bool NoModOff { get; set; }

		/// <summary>
		/// When starting, don't turn off CapsLock.
		/// Default: false.
		/// </summary>
		public bool NoCapsOff { get; set; }

		/// <summary>
		/// Callback function that can modify options of 'send keys or text' functions depending on conditions (active window etc).
		/// Default: null.
		/// </summary>
		/// <remarks>
		/// The callback function is called by <see cref="Key"/>, <see cref="Text"/>, <see cref="Keyb.Send"/> and similar functions.
		/// </remarks>
		/// <seealso cref="HookData"/>
		public Action<HookData> Hook { get; set; }

		/// <summary>
		/// Parameter type of the <see cref="Hook"/> callback function.
		/// </summary>
		public struct HookData
		{
			internal HookData(KOptions opt, Wnd w) { this.opt = opt; this.w = w; }

			/// <summary>
			/// Options used by the 'send keys or text' function. The callback function can modify them, except Hook, NoModOff, NoCapsOff.
			/// </summary>
			public readonly KOptions opt;

			/// <summary>
			/// The focused control. If there is no focused control - the active window. Use <c>w.WndWindow</c> to get top-level parent window; if <c>w.WndWindow == w</c>, <b>w</b> is the active window, else the focused control. The callback function is not called if there is no active window.
			/// </summary>
			public readonly Wnd w;
		}
	}

	/// <summary>
	/// How functions send text.
	/// See <see cref="KOptions.TextOption"/>.
	/// </summary>
	/// <remarks>
	/// There are three ways to send text to the active app using keys: 1. Characters (default) - use special key code VK_PACKET. 2. Keys - press keybord keys. 3. Paste - use the clipboard and Ctrl+V.
	/// Most but not all apps support all three ways. Most Unicode characters cannot be sent with <b>Keys</b>.
	/// Depending on text, the 'send text' functions may use other method than specified. For some characters or for whole text. More info below.
	/// Many apps don't support Unicode surrogate pairs sent as keys. If the text contains such characters, is used <b>Paste</b> instead of other options (implicitly). These characters are rarely used.
	/// </remarks>
	public enum KTextOption
	{
		/// <summary>
		/// Send text characters using special key code VK_PACKET.
		/// Few apps don't support it.
		/// This option is default.
		/// Supports most Unicode characters.
		/// For newlines is used key Enter (implicitly).
		/// </summary>
		Characters,
		//info:
		//	Tested many apps/controls/frameworks. Works almost everywhere.
		//	Does not work with Pidgin (thanks PhraseExpress for this info) and probably in all apps that use the same framework (GTK?).
		//	I guess does not work with many games.
		//	In PhraseExpress this is default. Its alternative methods are SendKeys (does not send Unicode chars) and clipboard. It uses clipboard if text is long, default 100. Allows to choose different for specified apps. Does not add any delays between chars; for some apps too fast, eg VirtualBox edit fields when text contains Unicode surrogates.

		/// <summary>
		/// Send text keys, with Shift or other modifiers where need, depending on the keyboard layout of the active window. The numpad keys are not used.
		/// All apps support it.
		/// Cannot send characters that cannot be typed using the keyboard. For example most Unicode characters. For these characters is used the <b>Characters</b> option (implicitly).
		/// </summary>
		Keys,

		/// <summary>
		/// Paste text using the clipboard and Ctrl+V.
		/// Few apps don't support it.
		/// This option is recommended for long text, because other ways then are too slow.
		/// Other options are unreliable when text length is more than 4000 or 5000 and the target app is too slow to process sent characters. Then <see cref="KOptions.TimeTextChar"/> can help.
		/// Also, other options are unreliable when the target app modifies typed text, for example has such features as auto-complete or auto-indent. However some apps modify even pasted text, for example trim the last newline.
		/// When pasting text, previous clipboard data is lost.
		/// </summary>
		Paste,
	}

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

//FUTURE: instead of AutoPassword: FocusPasswordField(); Text(password, "Tab"); Text(user, "Enter");
//public static void FocusPasswordField()
