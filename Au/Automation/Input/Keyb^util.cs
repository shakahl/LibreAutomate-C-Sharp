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
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Keyboard and clipboard functions.
	/// </summary>
	//[DebuggerStepThrough]
	public partial class Keyb
	{
		/// <summary>
		/// Converts part of string to Keys.
		/// The substring should contain single key name, eg "Esc", "A", "=".
		/// Returns 0 if invalid key name.
		/// </summary>
		static unsafe Keys _KeynameToKey(string s, int i, int len)
		{
			if(len < 1) return 0;

			char c = s[i];

			//character keys, like K, 9, -
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
				//{ "+", Keys.Oemplus }, //eg Ctrl+A
				//{ "*", Keys.D8 }, //*repeatCount, *down, *up
				//{ "(", Keys.D9 }, //eg Alt+(A F)
				//{ ")", Keys.D0 },
				//{ "#", Keys.D3 }, //numpad keys, eg #5, #*
				//{ "$", Keys.D4 }, //alias for Shift+

				//reserved
				//{ "!", Keys.D1 },
				//{ "@", Keys.D2 },
				//{ "%", Keys.D5 },
				//{ "^", Keys.D6 },
				//{ "&", Keys.D7 },
			}

			//numpad keys
			if(c == '#') {
				Keys k = 0;
				if(len == 2) {
					switch(s[i + 1]) {
					case '.': return Keys.Decimal;
					case '+': return Keys.Add;
					case '/': return Keys.Divide;
					case '*': return Keys.Multiply;
					case '-': return Keys.Subtract;
					case '0': return Keys.NumPad0;
					case '1': return Keys.NumPad1;
					case '2': return Keys.NumPad2;
					case '3': return Keys.NumPad3;
					case '4': return Keys.NumPad4;
					case '5': return Keys.NumPad5;
					case '6': return Keys.NumPad6;
					case '7': return Keys.NumPad7;
					case '8': return Keys.NumPad8;
					case '9': return Keys.NumPad9;
					}
				}
				return k;
			}

			//F keys
			if(c == 'F') {
				int n = s.ToInt32_(i + 1, out int e);
				if(n > 0 && n <= 24 && e == i + len) return Keys.F1 - 1 + n;
			}

			//named keys
			//names start with an uppercase letter and must have at least 2 other anycase letters, except: Up, AltG (RAl), PageU (PgU), PageD (PgD).
			var t = Util.LibTables.LowerCase;
			char c1 = t[s[i + 1]], c2 = len > 2 ? t[s[i + 2]] : ' ', c3 = len > 3 ? t[s[i + 3]] : ' ', c4 = len > 4 ? t[s[i + 4]] : ' ';
			uint u = (uint)c1 << 16 | c2;
			switch(c) {
			case 'A':
				if(_U('l', 't')) return c3 == 'g' ? Keys.RMenu : Keys.Menu;
				if(_U('p', 'p')) return Keys.Apps;
				if(_U('d', 'd')) return Keys.Add;
				break;
			case 'B':
				if(_U('a', 'c')) return Keys.Back;
				break;
			case 'C':
				if(_U('t', 'r')) return Keys.ControlKey;
				if(_U('o', 'n')) return Keys.ControlKey;
				if(_U('a', 'p')) return Keys.CapsLock;
				break;
			case 'D':
				if(_U('e', 'l')) return Keys.Delete;
				if(_U('e', 'c')) return Keys.Decimal;
				if(_U('i', 'v')) return Keys.Divide;
				if(_U('o', 'w')) return Keys.Down;
				break;
			case 'E':
				if(_U('n', 't')) return Keys.Enter;
				if(_U('n', 'd')) return Keys.End;
				if(_U('s', 'c')) return Keys.Escape;
				break;
			case 'H':
				if(_U('o', 'm')) return Keys.Home;
				break;
			case 'I':
				if(_U('n', 's')) return Keys.Insert;
				break;
			case 'L':
				if(_U('e', 'f')) return Keys.Left;
				if(_U('s', 'h')) return Keys.LShiftKey;
				if(_U('c', 't') || _U('c', 'o')) return Keys.LControlKey;
				if(_U('a', 'l')) return Keys.LMenu;
				if(_U('w', 'i')) return Keys.LWin;
				break;
			case 'M':
				if(_U('e', 'n')) return Keys.Apps;
				if(_U('u', 'l')) return Keys.Multiply;
				break;
			case 'N':
				if(_U('u', 'm')) return Keys.NumLock;
				//case "NumEnter": return Keys.Return|ExtendedKeyFlag; //Key((Keys.Return, 0, true))
				break;
			case 'P':
				if(_U('a', 'g') && c3 == 'e') return c4 == 'u' ? Keys.PageUp : (c4 == 'd' ? Keys.PageDown : 0);
				if(_U('g', 'u')) return Keys.PageUp;
				if(_U('g', 'd')) return Keys.PageDown;
				if(_U('a', 'u')) return Keys.Pause;
				if(_U('r', 'i') || _U('r', 't')) return Keys.PrintScreen;
				break;
			case 'R':
				if(_U('i', 'g')) return Keys.Right;
				if(_U('s', 'h')) return Keys.RShiftKey;
				if(_U('c', 't') || _U('c', 'o')) return Keys.RControlKey;
				if(_U('a', 'l')) return Keys.RMenu;
				if(_U('w', 'i')) return Keys.RWin;
				if(_U('e', 't')) return Keys.Enter;
				break;
			case 'S':
				if(_U('h', 'i')) return Keys.ShiftKey;
				if(_U('p', 'a')) return Keys.Space;
				if(_U('c', 'r')) return Keys.Scroll;
				if(_U('u', 'b')) return Keys.Subtract;
				//case "SysRq": return Keys.PrintScreen; //SysRq not used on Windows
				break;
			case 'T':
				if(_U('a', 'b')) return Keys.Tab;
				break;
			case 'U':
				if(c1 == 'p') return Keys.Up;
				break;
			case 'W':
				if(_U('i', 'n')) return Keys.LWin;
				break;
			}

			if(c >= 'A' && c <= 'Z' && Enum.TryParse(s.Substring(i, len), true, out Keys ev)) return ev;
			return 0;

			bool _U(char cc1, char cc2) { return u == ((uint)cc1 << 16 | cc2); }
		}

		/// <summary>
		/// Internal static functions.
		/// </summary>
		internal static class Lib
		{
			/// <summary>
			/// If k is ShiftKey, ControlKey, Menu, LWin or RWin, returns it as modifier flag, eg KMod.Shift from Keys.ShiftKey.
			/// Else returns 0.
			/// Also supports RShiftKey etc.
			/// </summary>
			internal static KMod KeyToMod(Keys k)
			{
				switch(k) {
				case Keys.ShiftKey: case Keys.LShiftKey: case Keys.RShiftKey: return KMod.Shift;
				case Keys.ControlKey: case Keys.LControlKey: case Keys.RControlKey: return KMod.Ctrl;
				case Keys.Menu: case Keys.LMenu: case Keys.RMenu: return KMod.Alt;
				case Keys.LWin: case Keys.RWin: return KMod.Win;
				}
				return 0;
			}

			/// <summary>
			/// Gets scan code from virtual-key code.
			/// </summary>
			/// <param name="vk">Virtual-key code, eg (byte)Keys.</param>
			/// <param name="hkl">Keyboard layout. If default(LPARAM), uses of current thread.</param>
			internal static ushort VkToSc(byte vk, LPARAM hkl = default)
			{
				if(hkl == default) hkl = Api.GetKeyboardLayout(0);
				uint sc = Api.MapVirtualKeyEx(vk, 0, hkl); //MAPVK_VK_TO_VSC

				//fix Windows bugs
				if(vk == 0x2C && sc == 0x54) sc = 0x37; //VK_SNAPSHOT
				if(vk == 0x13 && sc == 0) sc = 0x45; //VK_PAUSE

				return (ushort)sc;

				//tested: LCtrl, RCtrl etc are correctly sent, although MSDN does not mention that SendInput supports it.
			}

			/// <summary>
			/// Sends one key event.
			/// Just calls API SendInput with raw parameters.
			/// </summary>
			internal static unsafe void SendKeyEventRaw(ushort vk, ushort scan, uint flags)
			{
				var ki = new Api.INPUTK(vk, scan, flags);
				Api.SendInput(&ki);
			}

			/// <summary>
			/// Sends key.
			/// Not used for keys whose scancode can depend on keyboard layout. To get scancode, uses keyboard layout of current thread.
			/// </summary>
			/// <param name="k"></param>
			/// <param name="downUp">1 down, 2 up, 0 down-up.</param>
			internal static void SendKey(Keys k, int downUp = 0)
			{
				uint f = 0;
				if(_KeyTypes.IsExtended((byte)k)) f |= Api.KEYEVENTF_EXTENDEDKEY;
				ushort scan = VkToSc((byte)k);

				if(0 == (downUp & 2)) SendKeyEventRaw((ushort)k, scan, f);
				if(0 == (downUp & 1)) SendKeyEventRaw((ushort)k, scan, f | Api.KEYEVENTF_KEYUP);
			}

			internal static void SendCtrl(bool press) => SendKeyEventRaw((byte)Keys.ControlKey, 0x1D, press ? 0 : Api.KEYEVENTF_KEYUP);
			internal static void SendAlt(bool press) => SendKeyEventRaw((byte)Keys.Menu, 0x38, press ? 0 : Api.KEYEVENTF_KEYUP);
			internal static void SendShift(bool press) => SendKeyEventRaw((byte)Keys.ShiftKey, 0x2A, press ? 0 : Api.KEYEVENTF_KEYUP);
			internal static void SendRCtrlUp() => SendKeyEventRaw((byte)Keys.ControlKey, 0x1D, Api.KEYEVENTF_KEYUP | Api.KEYEVENTF_EXTENDEDKEY);
			internal static void SendRAltUp() => SendKeyEventRaw((byte)Keys.Menu, 0x38, Api.KEYEVENTF_KEYUP | Api.KEYEVENTF_EXTENDEDKEY);
			internal static void SendRShiftUp() => SendKeyEventRaw((byte)Keys.ShiftKey, 0x36, Api.KEYEVENTF_KEYUP);

			/// <summary>
			/// Presses or releases one or more modifier keys.
			/// Sends in this order: Ctrl, Alt, Shift, Win.
			/// </summary>
			/// <param name="press"></param>
			/// <param name="mod">Modifier keys. Does nothing if 0.</param>
			internal static unsafe void ModPressRelease(bool press, KMod mod)
			{
				if(mod == 0) return;
				var a = stackalloc Api.INPUTK[4];
				int n = 0; uint f = press ? 0 : Api.KEYEVENTF_KEYUP;
				if(0 != (mod & KMod.Ctrl)) a[n++].Set((byte)Keys.ControlKey, 0x1D, f);
				if(0 != (mod & KMod.Alt)) a[n++].Set((byte)Keys.Menu, 0x38, f);
				if(0 != (mod & KMod.Shift)) a[n++].Set((byte)Keys.ShiftKey, 0x2A, f);
				if(0 != (mod & KMod.Win)) a[n++].Set((byte)Keys.LWin, 0x5B, f);
				Api.SendInput(a, n);
			}

			/// <summary>
			/// Releases modifier keys if pressed. Optionally turns off CapsLock if toggled.
			/// Returns true if was CapsLock.
			/// Uses options NoModOff and NoCapsOff. Does not call Hook.
			/// </summary>
			/// <param name="opt"></param>
			/// <param name="forClipb">Used for Clipb Ctrl+V/C/X. Ignore CapsLock and always release modifiers, regardless of opt.</param>
			internal static bool ReleaseModAndCapsLock(KOptions opt, bool forClipb = false)
			{
				//note: don't call Hook here, it does not make sense.

				bool R = !forClipb && !opt.NoCapsOff && IsCapsLock;
				if(R) SendKey(Keys.CapsLock);

				if(forClipb || !opt.NoModOff) {
					bool isLAlt = IsKeyPressedSyncOrAsync(Keys.LMenu);
					bool isRAlt = IsKeyPressedSyncOrAsync(Keys.RMenu);
					bool isLWin = IsKeyPressedSyncOrAsync(Keys.LWin);
					bool isRWin = IsKeyPressedSyncOrAsync(Keys.RWin);
					bool isLCtrl = IsKeyPressedSyncOrAsync(Keys.LControlKey);
					bool isRCtrl = IsKeyPressedSyncOrAsync(Keys.RControlKey);
					bool menu = (isLAlt || isRAlt || isLWin || isRWin) && !(isLCtrl || isRCtrl);
					if(menu) SendKey(Keys.ControlKey); //if Alt or Win pressed, send Ctrl to avoid menu mode or Start menu. For Alt works Ctrl-up, but maybe not everywhere. For Win need Ctrl-down-up.
					if(isLCtrl) SendCtrl(false);
					if(isRCtrl) SendRCtrlUp();
					if(IsKeyPressedSyncOrAsync(Keys.LShiftKey)) SendShift(false);
					if(IsKeyPressedSyncOrAsync(Keys.RShiftKey)) SendRShiftUp();
					if(isLAlt) SendAlt(false);
					if(isRAlt) SendRAltUp();
					if(isLWin) SendKey(Keys.LWin, 2);
					if(isRWin) SendKey(Keys.RWin, 2);
				}

				return R;
			}

			internal static bool IsKeyPressedSyncOrAsync(Keys k)
			{
				return Api.GetKeyState(k) < 0 || Api.GetAsyncKeyState(k) < 0;
			}

			/// <summary>
			/// Sends Ctrl+V or Ctrl+C or Ctrl+X, and/or optionally Enter.
			/// Caller gets opt and wFocus with _GetOptionsAndWndFocused (it may want to know some options too).
			/// Caller calls Press, waits until the target app gets clipboard data, then calls Release.
			/// </summary>
			internal unsafe struct SendCopyPaste
			{
				ushort _scan;
				byte _vk;
				bool _enter;
				KOptions _opt;

				/// <summary>
				/// Presses Ctrl+key. Does not release.
				/// If enter is true, Release will press Enter.
				/// </summary>
				public void Press(Keys key, KOptions opt, Wnd wFocus, bool enter = false)
				{
					_scan = VkToSc(_vk = (byte)key, Api.GetKeyboardLayout(wFocus.ThreadId));
					_enter = enter;
					_opt = opt;

					SendCtrl(true);
					Time.Sleep(_LimitSleepTime(Math.Max(opt.TimeKeyPressed, 3))); //to avoid problems with apps like IE address bar
					SendKeyEventRaw(_vk, _scan, 0);
				}

				/// <summary>
				/// Releases keys.
				/// Does nothing if already released.
				/// </summary>
				public void Release()
				{
					if(_vk == 0) return;

					SendKeyEventRaw(_vk, _scan, Api.KEYEVENTF_KEYUP);
					SendCtrl(false);
					_vk = 0;
					if(_enter) Enter(_opt);
				}

				/// <summary>
				/// Sends Enter.
				/// </summary>
				public static void Enter(KOptions opt)
				{
					var e = new _KEvent(true, Keys.Enter, 0, 0x1C);
					_SendKey2(e, default, true, opt);
				}
			}

			internal struct INPUTKEY2
			{
				public Api.INPUTK k0, k1;

				public INPUTKEY2(ushort vk, ushort sc, uint flags = 0)
				{
					k0 = new Api.INPUTK(vk, sc, flags);
					k1 = new Api.INPUTK(vk, sc, flags | Api.KEYEVENTF_KEYUP);
				}
			}

			internal static Wnd GetWndFocusedOrActive()
			{
				for(int i = 0; i < 20; i++) {
					Wnd.Misc.GetGUIThreadInfo(out var g);
					//Print(i, g.hwndFocus, g.hwndActive);
					if(!g.hwndFocus.Is0) return g.hwndFocus;
					if(!g.hwndActive.Is0) return g.hwndActive;
					Time.Sleep(1);
				}
				return default;

				//note: the purpose of this wait is not synchronization. It just makes getting the focused/active window more reliable.
				//	Cannot wait for a focused window. Users must program it explicitly.
				//	When creating or activating a window, often there is no focus for 200 ms or more. Eg when opening the Save As dialog.
				//	Also, focus is optional.
				//	Anyway, waiting for focus would not make more reliable, because keys are processed asynchronously.
				//	Usually the active window is OK. We use it to get keyboard layout and/or to avoid calling Hook too frequently.
				//	We wait for active window max 20-40 ms. When switching apps, usually there is no active window for 1-5 ms.
			}
		}

		/// <summary>
		/// Returns KOptions of this variable or KOptions cloned from this variable and possibly modified by Hook.
		/// </summary>
		/// <param name="wFocus">receives the focused or active window. Also the function uses it to avoid frequent calling of Hook.</param>
		/// <param name="getWndAlways">if false, the caller does not need wFocus. Then wFocus will be default(Wnd) if Hook is null.</param>
		KOptions _GetOptionsAndWndFocused(out Wnd wFocus, bool getWndAlways)
		{
			if(base.Hook == null && !getWndAlways) {
				wFocus = default;
				return this;
			}
			return _GetOptions(wFocus = Lib.GetWndFocusedOrActive());
		}

		/// <summary>
		/// Returns KOptions of this variable or KOptions cloned from this variable and possibly modified by Hook.
		/// </summary>
		/// <param name="wFocus">the focused or active window. The function uses it to avoid frequent calling of Hook. If you don't have it, use _GetOptionsAndWndFocused instead.</param>
		KOptions _GetOptions(Wnd wFocus)
		{
			var call = base.Hook;
			if(call == null || wFocus.Is0) return this;
			if(wFocus != _sstate.wFocus) {
				_sstate.wFocus = wFocus;
				if(_sstate.options == null) _sstate.options = new KOptions(this); else _sstate.options.ResetOptions();
				call(new KOHookData(_sstate.options, wFocus));
			}
			return _sstate.options;
		}

		void _ThrowIfSending()
		{
			if(_sending) throw new InvalidOperationException();
		}

		static class _KeyTypes
		{
			[Flags]
			enum _T :byte
			{
				Mod = 1,
				Extended = 2,
				Mouse = 4,
				GetKeyStateWorks = 8, //API GetKeyState can get the 'toggled' bit
			}

			public static bool IsMod(byte vk) => _b[vk].Has_(_T.Mod);
			public static bool IsExtended(byte vk) => _b[vk].Has_(_T.Extended);
			public static bool IsMouse(byte vk) => _b[vk].Has_(_T.Mouse);
			public static bool IsGetKeyStateWorks(byte vk) => _b[vk].Has_(_T.GetKeyStateWorks);

			static _T[] _b;

			static _KeyTypes()
			{
				_b = new _T[256];

				//info: there is no API to get extended keys. MapVirtualKeyEx can get only of 50% keys. And there is no full reference.

				_b[1] = _b[2] = _b[4] = _b[5] = _b[6] = _T.Mouse | _T.GetKeyStateWorks;
				_b[16] = _b[17] = _b[18] = _T.Mod | _T.GetKeyStateWorks;

				_b[(int)Keys.PageUp] = _b[(int)Keys.PageDown] = _b[(int)Keys.End] = _b[(int)Keys.Home]
					= _b[(int)Keys.Left] = _b[(int)Keys.Up] = _b[(int)Keys.Right] = _b[(int)Keys.Down]
					= _b[(int)Keys.PrintScreen] = _b[(int)Keys.Insert] = _b[(int)Keys.Delete]
					= _b[(int)Keys.Sleep] = _b[(int)Keys.Apps] = _b[(int)Keys.Divide] = _b[(int)Keys.Cancel]
					= _T.Extended;
				//and more, eg probably IMEConvert, IMENonconvert, VK_DBE_KATAKANA, VK_DBE_SBCSCHAR,
				//	but there is no reference, and I cannot test, and MapVirtualKeyEx does know it. 

				_b[(int)Keys.CapsLock] = _b[(int)Keys.Scroll] = _b[(int)Keys.Back] = _b[(int)Keys.Tab] = _b[(int)Keys.Enter] = _b[(int)Keys.Escape]
					= _b[(int)Keys.RShiftKey] = _b[(int)Keys.LControlKey] = _b[(int)Keys.LMenu] = _b[(int)Keys.LShiftKey] = _T.GetKeyStateWorks;
				//also Home, 182, 183 and maybe more

				_b[(int)Keys.LWin] = _b[(int)Keys.RWin] = _T.Mod | _T.Extended | _T.GetKeyStateWorks;
				_b[(int)Keys.RControlKey] = _b[(int)Keys.RMenu] = _b[(int)Keys.NumLock] = _T.Extended | _T.GetKeyStateWorks;

				for(int i = (int)Keys.BrowserBack; i <= (int)Keys.LaunchApplication2; i++) _b[i] = _T.Extended; //media/browser/launchapp keys
			}
		}
	}
}
