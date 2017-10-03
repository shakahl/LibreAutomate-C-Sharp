using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
//using System.Xml;
//using System.Xml.Linq;

using Catkeys.Types;
using static Catkeys.NoClass;

namespace Catkeys
{
	/// <summary>
	/// Keyboard and clipboard functions.
	/// </summary>
	//[DebuggerStepThrough]
	public static partial class Input
	{
		class _KeyFunc
		{
			/// <summary>
			/// + operator.
			/// Presses the key and does not release. These keys are auto-released after next key or (keys) that is not followed by +. Also after next mouse click.
			/// Example: "Ctrl+Shift+A ...".
			/// Example: "Alt+(F O) ...".
			/// Example: "Ctrl+" ... Mouse.Click() //Ctrl auto-released here.
			/// </summary>
			public static Keys Plus { get => (Keys)0x1000000; }
			/// <summary>
			/// * operator.
			/// 1. Repeats the preceding key.
			/// Example: "Tab*10" //press Tab 10 times.
			/// 2. Just presses (without releasing) or just releases.
			/// Example: "Ctrl*down" ... "Ctrl*up".
			/// </summary>
			public static Keys Star { get => (Keys)0x2000000; }
			/// <summary>
			/// ( operator.
			/// 1. Encloses function arguments.
			/// Example: WAIT(0.5).
			/// 2. Turns off auto-releasing keys pressed with operator +, until ")".
			/// Example: "Alt+(F O) ...".
			/// Example: "Ctrl+(" ... Mouse.Click(); Mouse.Click(); ... ")" //Ctrl released here.
			/// </summary>
			public static Keys ParenStart { get => (Keys)0x3000000; }
			/// <summary>
			/// ) operator.
			/// Releases keys pressed with operator +, when auto-releasing was turned off with operator (. See <see cref="ParenStart"/>.
			/// </summary>
			public static Keys ParenEnd { get => (Keys)0x4000000; }
			/// <summary>
			/// VK(vk, sc=0, ext=0)
			/// Sends key specified as virtual-key code and/or scan code.
			/// vk - virtual-key code.
			/// sc - optional scan code.
			/// ext - optional flags: 1 extended key. </summary>
			public static Keys VK { get => (Keys)0x5000000; }
			/// <summary>
			/// CHAR(c)
			/// Sends a character using VK_PACKET.
			/// c - Unicode character code.
			/// </summary>
			public static Keys CHAR { get => (Keys)0x6000000; }
			/// <summary>
			/// SLEEP(t)
			/// Waits.
			/// t - number of milliseconds, eg 100. </summary>
			public static Keys SLEEP { get => (Keys)0x8000000; }

		}

		/// <summary>
		/// Less-often used functions related to keyboard input.
		/// </summary>
		public static class Misc
		{
			//UNFINISHED
			/// <summary>
			/// Converts keys string to List.
			/// </summary>
			/// <param name="s"></param>
			public static List<Keys> ReadKeyString(string s)
			{
				if(s == null) return null;
				var a = new List<Keys>();
				int i = 0, eos = s.Length;

				while(i < eos) {
					//skip space
					while(i < eos && s[i] <= ' ') i++;

					//read token
					int start = i;
					char c2 = default;
					for(; i < eos; i++) {
						c2 = s[i];
						if(c2 <= ' ' || c2 == '+' || c2 == '*' || c2 == '(' || c2 == ')') break;
					}
					int len = i - start; if(len == 0) return null;

					if(c2 == '(') { //function
						var ss = s.Substring(start, len).ToUpper_();
						switch(ss) {
						case "VK":
							break;
						case "CHAR":
							break;
						case "SLEEP":
							break;
						default:
							return null;
						}
					} else { //key
						Keys k = _KeynameToKey(s, start, len);
						if(k == 0) return null;
						a.Add(k);
					}
				}

				return a;
			}

			/// <summary>
			/// Converts hotkey string to Keys.
			/// For example, if s is "Ctrl+Left", hotkey will be Keys.Left with modifier flag Keys.Control.
			/// Must be single non-modifier key, preceded by zero or more of modifier keys Ctrl, Shift, Alt, Win, all joined with +.
			/// Named keys must match case. Keys A-Z don't have to match case.
			/// Valid hotkey examples: "A", "a", "7", "F12", ".", "End", "Ctrl+D", "Ctrl+Alt+Shift+Win+Left", " Ctrl + U ".
			/// Invalid hotkey examples: null, "", "A+B", "Ctrl+A+K", "A+Ctrl", "Ctrl+Shift", "Ctrl+", "NoSuchKey", "end".
			/// Returns false if the string is invalid.
			/// </summary>
			public static bool ReadHotkeyString(string s, out Keys hotkey)
			{
				hotkey = 0;
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
					var mod = _KeyToModifier(k);
					bool lastKey = (i == eos);
					if(lastKey != (mod == 0)) break;
					if(lastKey) { hotkey |= k; return true; }
					hotkey |= mod;
				}
				return false;
			}

			/// <summary>
			/// If k is Shift, Control, Alt or Win (including left and right), returns it as modifier flag, eg Keys.Shift from Keys.ShiftKey.
			/// Else returns 0.
			/// </summary>
			static Keys _KeyToModifier(Keys k)
			{
				switch(k) {
				case Keys.ShiftKey: case Keys.LShiftKey: case Keys.RShiftKey: return Keys.Shift;
				case Keys.ControlKey: case Keys.LControlKey: case Keys.RControlKey: return Keys.Control;
				case Keys.Menu: case Keys.LMenu: case Keys.RMenu: return Keys.Alt;
				case Keys.LWin: case Keys.RWin: return Keys_.Windows;
				}
				return 0;
			}

			/// <summary>
			/// Converts part of string to Keys.
			/// The string should contain single key name, eg "Esc", "A", "=".
			/// Returns 0 if invalid key name.
			/// </summary>
			//[MethodImpl(MethodImplOptions.NoOptimization)] //saves 2ms/40% of startup time. Later 50% slower, but it is less important. But assume that this library will be ngened when startup time is important.
			static Keys _KeynameToKey(string s, int i, int len)
			{
				char c = s[i];
				if(len == 1) {
					if(c >= 'a' && c <= 'z') return (Keys)(c - 32);
					if(c >= 'A' && c <= 'Z') return (Keys)c;
					if(c >= '0' && c <= '9') return (Keys)c;
					switch(c) {
					case '`': case '~': return Keys.Oemtilde;
					case '-': case '_': return Keys.OemMinus;
					case '=': return Keys.Oemplus;
					case '[': case '{': return Keys.OemOpenBrackets;
					case ']': case '}': return Keys.OemCloseBrackets;
					case '\\': case '|': return Keys.OemPipe;
					case ';': case ':': return Keys.OemSemicolon;
					case '\'': case '\"': return Keys.OemQuotes;
					case ',': case '<': return Keys.Oemcomma;
					case '.': case '>': return Keys.OemPeriod;
					case '/': case '?': return Keys.OemQuestion;
					}
					return 0;

					//special
					//{ "+", Keys.Oemplus },
					//{ "*", Keys.D8 },
					//{ "(", Keys.D9 },
					//{ ")", Keys.D0 },

					//reserved
					//{ "!", Keys.D1 },
					//{ "@", Keys.D2 },
					//{ "#", Keys.D3 },
					//{ "$", Keys.D4 },
					//{ "%", Keys.D5 },
					//{ "^", Keys.D6 },
					//{ "&", Keys.D7 },
				}

				if(c == 'F') {
					int n = s.ToInt32_(i + 1, out int e);
					if(n > 0 && n <= 24 && e == i + len) return Keys.F1 - 1 + n;
					return 0;
				}

				s = s.Substring(i, len);

				//_keyMap.TryGetValue(s, out var k); //slower etc. Would use it only if key names would be case-insensitive.
				//return k;

				//note: key names are case-sensitive, except A-Z. Using any-case key names in script is ugly and unwise.
				switch(s) {
				//Shift/Ctrl/Alt/Win, not as modifier flags
				case "Shift": return Keys.ShiftKey;
				case "Ctrl": return Keys.ControlKey;
				//case "Control": //often called "Control", and on some rare keyboards it is "Control", but it is confusing and nobody would use
				case "Alt": return Keys.Menu;
				case "Win": case "Windows": return Keys.LWin;
				//right
				case "RShift": return Keys.RShiftKey;
				case "RCtrl": return Keys.RControlKey;
				case "RAlt": case "AltGr": return Keys.RMenu;
				case "RWin": case "RWindows": return Keys.RWin;

				//main
				case "Back": case "Backspace": case "BackSpace": return Keys.Back;
				case "CapsLock": return Keys.CapsLock;
				case "Del": case "Delete": return Keys.Delete;
				case "Down": return Keys.Down;
				case "End": return Keys.End;
				case "Enter": return Keys.Enter;
				//case "Return": return Keys.Enter; //the key is often called "Return", but I never saw a keyboard with "Return"; it would be confusing, like it is some other key, not Enter
				case "Esc": case "Escape": return Keys.Escape;
				case "Home": return Keys.Home;
				case "Ins": case "Insert": return Keys.Insert;
				case "Left": return Keys.Left;
				case "Menu": return Keys.Apps;
				//case "Application": return Keys.Apps;
				case "PageDown": case "PgDn": return Keys.PageDown;
				case "PageUp": case "PgUp": return Keys.PageUp;
				case "Pause": return Keys.Pause;
				//case "Break": return Keys.Pause; //don't need, because some rare keyboards have 2 keys - Pause and Break, and I don't know how they are different
				case "PrintScreen": case "PrtSc": case "PrtScrn": return Keys.PrintScreen;
				//case "SysRq": return Keys.PrintScreen; //don't need SysRq because it is not used on Windows
				case "Right": return Keys.Right;
				case "ScrollLock": case "ScrLk": return Keys.Scroll;
				case "Space": case "Spacebar": case "SpaceBar": return Keys.Space;
				case "Tab": return Keys.Tab;

				//numpad
				case "Num0": return Keys.NumPad0;
				case "Num1": return Keys.NumPad1;
				case "Num2": return Keys.NumPad2;
				case "Num3": return Keys.NumPad3;
				case "Num4": return Keys.NumPad4;
				case "Num5": return Keys.NumPad5;
				case "Num6": return Keys.NumPad6;
				case "Num7": return Keys.NumPad7;
				case "Num8": return Keys.NumPad8;
				case "Num9": return Keys.NumPad9;
				case "NumEnter": return Keys_.NumEnter; //extended-key
				case "NumLock": case "NumLk": return Keys.NumLock;
				case "Num.": case "Decimal": return Keys.Decimal; //GetKeyNameText gives "Num Del"
				case "Add": return Keys.Add;
				//case "Num+": return Keys.Add; //+ is an operator
				case "Divide": return Keys.Divide;
				case "Multiply": return Keys.Multiply;
				case "Subtract": return Keys.Subtract;

				//browser, multimedia, launch, sleep
				case "BrowserBack": return Keys.BrowserBack;
				case "BrowserFavorites": return Keys.BrowserFavorites;
				case "BrowserForward": return Keys.BrowserForward;
				case "BrowserHome": return Keys.BrowserHome;
				case "BrowserRefresh": return Keys.BrowserRefresh;
				case "BrowserSearch": return Keys.BrowserSearch;
				case "BrowserStop": return Keys.BrowserStop;
				case "LaunchApplication1": return Keys.LaunchApplication1;
				case "LaunchApplication2": return Keys.LaunchApplication2;
				case "LaunchMail": return Keys.LaunchMail;
				case "MediaNextTrack": return Keys.MediaNextTrack;
				case "MediaPlayPause": return Keys.MediaPlayPause;
				case "MediaPreviousTrack": return Keys.MediaPreviousTrack;
				case "MediaStop": return Keys.MediaStop;
				case "MediaSelect": return Keys.SelectMedia;
				case "VolumeDown": return Keys.VolumeDown;
				case "VolumeMute": return Keys.VolumeMute;
				case "VolumeUp": return Keys.VolumeUp;
				case "Sleep": return Keys.Sleep;
				}
				return 0;
			}

#if false
			static Dictionary<string, Keys> _keyMap = _CreateKeyMap();

			static Dictionary<string, Keys> _CreateKeyMap()
			{
				//var a = new Dictionary<string, Keys>(90, StringComparer.OrdinalIgnoreCase) { //~30% slower than case-sensitive
				var a = new Dictionary<string, Keys>(90) { //let it be case-sensitive, because difficult to read key names like pagedown
					//Shift/Ctrl/Alt/Win, not as modifier flags
					{ "Shift", Keys.ShiftKey },
					{ "Ctrl", Keys.ControlKey }, //{ "Control", Keys.ControlKey }, //often called "Control", and on some rare keyboards it is "Control", but it is confusing and nobody would use
					{ "Alt", Keys.Menu },
					{ "Win", Keys.LWin }, { "Windows", Keys.LWin },
					//right
					{ "RShift", Keys.RShiftKey },
					{ "RCtrl", Keys.RControlKey },
					{ "RAlt", Keys.RMenu }, { "AltGr", Keys.RMenu },
					{ "RWin", Keys.RWin }, { "RWindows", Keys.RWin },

					{ "Back", Keys.Back }, { "Backspace", Keys.Back },
					{ "CapsLock", Keys.CapsLock },
					{ "Del", Keys.Delete }, { "Delete", Keys.Delete },
					{ "Down", Keys.Down },
					{ "End", Keys.End },
					{ "Enter", Keys.Enter }, //{ "Return", Keys.Enter }, //the key is often called "Return", but I never saw a keyboard with "Return"; it would be confusing, like it is some other key, not Enter
					{ "Esc", Keys.Escape }, { "Escape", Keys.Escape },
					{ "Home", Keys.Home },
					{ "Ins", Keys.Insert }, { "Insert", Keys.Insert },
					{ "Left", Keys.Left },
					{ "Menu", Keys.Apps }, //{ "Application", Keys.Apps },
					{ "PageDown", Keys.PageDown }, { "PgDn", Keys.PageDown },
					{ "PageUp", Keys.PageUp }, { "PgUp", Keys.PageUp },
					{ "Pause", Keys.Pause }, //{ "Break", Keys.Pause }, //don't need Break, because some rare keyboards have 2 keys - Pause and Break, and I don't know how they are different
					{ "PrintScreen", Keys.PrintScreen }, { "PrtSc", Keys.PrintScreen }, { "PrtScrn", Keys.PrintScreen }, //{ "SysRq", Keys.PrintScreen }, //don't need SysRq because it is not used on Windows
					{ "Right", Keys.Right },
					{ "ScrollLock", Keys.Scroll }, { "ScrLk", Keys.Scroll },
					{ "Space", Keys.Space }, { "Spacebar", Keys.Space },
					{ "Tab", Keys.Tab },

					//numpad
					{ "Num0", Keys.NumPad0 },
					{ "Num1", Keys.NumPad1 },
					{ "Num2", Keys.NumPad2 },
					{ "Num3", Keys.NumPad3 },
					{ "Num4", Keys.NumPad4 },
					{ "Num5", Keys.NumPad5 },
					{ "Num6", Keys.NumPad6 },
					{ "Num7", Keys.NumPad7 },
					{ "Num8", Keys.NumPad8 },
					{ "Num9", Keys.NumPad9 },
					{ "NumEnter", Keys.Enter }, //extended-key
					{ "NumLock", Keys.NumLock }, { "NumLk", Keys.NumLock },
					{ "Num.", Keys.Decimal }, { "Decimal", Keys.Decimal }, //GetKeyNameText gives "Num Del"
					{ "Add", Keys.Add }, //{ "Num+", Keys.Add }, //+ is an operator
					{ "Divide", Keys.Divide },
					{ "Multiply", Keys.Multiply },
					{ "Subtract", Keys.Subtract },

					//browser, multimedia, launch, sleep
					{ "BrowserBack", Keys.BrowserBack },
					{ "BrowserFavorites", Keys.BrowserFavorites },
					{ "BrowserForward", Keys.BrowserForward },
					{ "BrowserHome", Keys.BrowserHome },
					{ "BrowserRefresh", Keys.BrowserRefresh },
					{ "BrowserSearch", Keys.BrowserSearch },
					{ "BrowserStop", Keys.BrowserStop },
					{ "LaunchApplication1", Keys.LaunchApplication1 },
					{ "LaunchApplication2", Keys.LaunchApplication2 },
					{ "LaunchMail", Keys.LaunchMail },
					{ "MediaNextTrack", Keys.MediaNextTrack },
					{ "MediaPlayPause", Keys.MediaPlayPause },
					{ "MediaPreviousTrack", Keys.MediaPreviousTrack },
					{ "MediaStop", Keys.MediaStop },
					{ "MediaSelect", Keys.SelectMedia },
					{ "VolumeDown", Keys.VolumeDown },
					{ "VolumeMute", Keys.VolumeMute },
					{ "VolumeUp", Keys.VolumeUp },
					{ "Sleep", Keys.Sleep },

					//for 1-character keys and F keys we don't use dictionary.

					//left, probably will not need
					//{ "LShift", Keys.LShiftKey },
					//{ "LCtrl", Keys.LControlKey }, { "LControl", Keys.LControlKey },
					//{ "LAlt", Keys.LMenu },
					//{ "LWin", Keys.LWin }, { "LWindows", Keys.LWin },

					//Shift/Ctrl/Alt/Win as modifiers, probably will not need
					//{ "ModShift", Keys.Shift }, //0x10000
					//{ "ModCtrl", Keys.Control }, { "ModControl", Keys.Control }, //0x20000
					//{ "ModAlt", Keys.Alt }, //0x40000
					//{ "ModWin", Keys_.Windows }, { "ModWindows", Keys_.Windows }, //0x80000

					//mouse
					//{ "LButton", Keys.LButton },
					//{ "MButton", Keys.MButton },
					//{ "RButton", Keys.RButton },
					//{ "XButton1", Keys.XButton1 },
					//{ "XButton2", Keys.XButton2 },
					//also could be forward/backward. Maybe as function.
				};
				Print(a.Count);
				return a;
			}
#endif
		}
	}
}

namespace Catkeys.Types
{
	/// <summary>
	/// Keys and modifiers that are missing in enum <see cref="Keys"/>.
	/// </summary>
	public static class Keys_
	{
		/// <summary>
		/// Modifier key Windows (flag). Can be used like Keys.Shift etc.
		/// Can be used only with functions of this library or other libraries that support it. Don't use with .NET functions.
		/// </summary>
		public const Keys Windows = (Keys)0x80000;

		/// <summary>
		/// Numpad Enter key.
		/// Can be used only with functions of this library or other libraries that support it. Don't use with .NET functions.
		/// </summary>
		public const Keys NumEnter = Keys.Enter | (Keys)0x100000;
	}
}
