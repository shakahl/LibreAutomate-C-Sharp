using Au.Types;
using Au.More;
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

namespace Au
{
	partial class keys
	{
		/// <summary>
		/// Miscellaneous rarely used keyboard-related functions.
		/// </summary>
		public static partial class more
		{
			/// <summary>
			/// Converts key name to <see cref="KKey"/>.
			/// Returns 0 if unknown key name.
			/// </summary>
			/// <param name="keyName">Key name, like with <see cref="keys.send"/>.</param>
			public static KKey parseKeyName(string keyName) {
				keyName ??= "";
				return _KeynameToKey(keyName, 0, keyName.Length);
			}

			/// <summary>
			/// Calls <see cref="parseKeyName"/> and throws ArgumentException if invalid key string.
			/// </summary>
			/// <param name="keyName"></param>
			internal static KKey ParseKeyNameThrow_(string keyName) {
				var k = parseKeyName(keyName);
				if (k == 0) throw new ArgumentException("Unknown key name or error in key string.");
				return k;
			}

			/// <summary>
			/// Converts key name to <see cref="KKey"/>.
			/// Returns 0 if unknown key name.
			/// </summary>
			/// <param name="s">String containing key name, like with <see cref="keys.send"/>.</param>
			/// <param name="startIndex">Key name start index in <i>s</i>.</param>
			/// <param name="length">Key name length.</param>
			/// <exception cref="ArgumentOutOfRangeException">Invalid start index or length.</exception>
			public static KKey parseKeyName(string s, int startIndex, int length) {
				s ??= "";
				if ((uint)startIndex > s.Length || (uint)length > s.Length - startIndex) throw new ArgumentOutOfRangeException();
				return _KeynameToKey(s, startIndex, length);
			}

			/// <summary>
			/// Converts keys string to <see cref="KKey"/> array.
			/// </summary>
			/// <param name="keys_">String containing one or more key names, like with <see cref="keys.send"/>. Operators are not supported.</param>
			/// <exception cref="ArgumentException">Error in <i>keys_</i> string.</exception>
			public static KKey[] parseKeysString(string keys_) {
				var a = new List<KKey>();
				foreach (var g in _SplitKeysString(keys_ ?? "")) {
					KKey k = _KeynameToKey(keys_, g.Start, g.Length);
					if (k == 0) throw _ArgumentException_ErrorInKeysString(keys_, g.Start, g.Length);
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
			/// Key names are like with <see cref="keys.send"/>.
			/// Must be single non-modifier key, preceded by zero or more of modifier keys Ctrl, Shift, Alt, Win, all joined with +.
			/// Valid hotkey examples: <c>"A"</c>, <c>"a"</c>, <c>"7"</c>, <c>"F12"</c>, <c>"."</c>, <c>"End"</c>, <c>"Ctrl+D"</c>, <c>"Ctrl+Alt+Shift+Win+Left"</c>, <c>" Ctrl + U "</c>.
			/// Invalid hotkey examples: null, "", <c>"A+B"</c>, <c>"Ctrl+A+K"</c>, <c>"A+Ctrl"</c>, <c>"Ctrl+Shift"</c>, <c>"Ctrl+"</c>, <c>"NoSuchKey"</c>, <c>"tab"</c>.
			/// </remarks>
			public static bool parseHotkeyString(string s, out KMod mod, out KKey key) {
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
			public static bool parseHotkeyString(string s, out System.Windows.Forms.Keys hotkey) {
				if (!parseHotkeyString(s, out var m, out var k)) { hotkey = 0; return false; }
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
			public static bool parseHotkeyString(string s, out System.Windows.Input.ModifierKeys mod, out System.Windows.Input.Key key, out System.Windows.Input.MouseAction mouse) {
				mod = 0; key = 0; mouse = 0;
				if (s.Ends("lick") || s.Ends("heel")) {
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
				if (!parseHotkeyString(s, out var m, out var k)) return false;
				mod = KModToWpf(m);
				return mouse != default || (key = KKeyToWpf(k)) != default;
				//tested: enum Key has all KKey values except mouse buttons and packet.
			}

			/// <summary>
			/// Used for parsing of hotkey triggers and mouse trigger modifiers.
			/// Like <see cref="parseHotkeyString"/>, but supports 'any mod' (like "Shift?+K" or "?+K") and <i>noKey</i>.
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

			//FUTURE: RemapKeyboardKeys. See QM2.
		}
	}
}