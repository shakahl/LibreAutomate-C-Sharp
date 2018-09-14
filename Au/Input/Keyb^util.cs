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
//using System.Linq;
//using System.Xml.Linq;

using Au.Types;
using static Au.NoClass;

namespace Au
{
	public partial class Keyb
	{
		/// <summary>
		/// Converts part of string to <see cref="KKey"/>.
		/// The substring should contain single key name, eg "Esc", "A", "=".
		/// Returns 0 if invalid key name.
		/// </summary>
		static unsafe KKey _KeynameToKey(string s, int i, int len)
		{
			if(len < 1) return 0;

			char c = s[i];

			//character keys, like K, 9, -
			if(len == 1) {
				if(c >= 'a' && c <= 'z') return (KKey)(c - 32);
				if(c >= 'A' && c <= 'Z') return (KKey)c;
				if(c >= '0' && c <= '9') return (KKey)c;
				switch(c) {
				case '=': return KKey.OemPlus;
				case '`': case '~': return KKey.OemTilde;
				case '-': case '_': return KKey.OemMinus;
				case '[': case '{': return KKey.OemOpenBrackets;
				case ']': case '}': return KKey.OemCloseBrackets;
				case '\\': case '|': return KKey.OemPipe;
				case ';': case ':': return KKey.OemSemicolon;
				case '\'': case '\"': return KKey.OemQuotes;
				case ',': case '<': return KKey.OemComma;
				case '.': case '>': return KKey.OemPeriod;
				case '/': case '?': return KKey.OemQuestion;
				}
				return 0;

				//special
				//+ //eg Ctrl+A
				//* //*repeatCount, *down, *up
				//( //eg Alt+(A F)
				//)
				//# //numpad keys, eg #5, #*
				//$ //Shift+

				//reserved
				//! @ % ^ &
			}

			//numpad keys
			if(c == '#') {
				if(len == 2) {
					switch(s[i + 1]) {
					case '.': return KKey.Decimal;
					case '+': return KKey.Add;
					case '/': return KKey.Divide;
					case '*': return KKey.Multiply;
					case '-': return KKey.Subtract;
					case '0': return KKey.NumPad0;
					case '1': return KKey.NumPad1;
					case '2': return KKey.NumPad2;
					case '3': return KKey.NumPad3;
					case '4': return KKey.NumPad4;
					case '5': return KKey.NumPad5;
					case '6': return KKey.NumPad6;
					case '7': return KKey.NumPad7;
					case '8': return KKey.NumPad8;
					case '9': return KKey.NumPad9;
					}
				}
				return 0;
			}

			//FUTURE: virtual-key code, eg ^50

			//F keys
			if(c == 'F' && Char_.IsAsciiDigit(s[i + 1])) {
				int n = s.ToInt_(i + 1, out int e, STIFlags.NoHex);
				if(n > 0 && n <= 24 && e == i + len) return (KKey)(0x6F + n);
			}

			//named keys
			//names start with an uppercase letter and must have at least 2 other anycase letters, except: Up, AltG (RAl), PageU (PgU), PageD (PgD).
			KKey k = 0;
			char c1 = Char.ToLowerInvariant(s[i + 1]), //note: Util.LibTables.LowerCase would make startup slow
				c2 = len > 2 ? Char.ToLowerInvariant(s[i + 2]) : ' ',
				c3 = len > 3 ? Char.ToLowerInvariant(s[i + 3]) : ' ',
				c4 = len > 4 ? Char.ToLowerInvariant(s[i + 4]) : ' ';
			uint u = (uint)c1 << 16 | c2;
			switch(c) {
			case 'A':
				if(_U('l', 't')) k = c3 == 'g' ? KKey.RAlt : KKey.Alt;
				else if(_U('p', 'p')) k = KKey.Apps;
				break;
			case 'B':
				if(_U('a', 'c')) k = KKey.Back;
				break;
			case 'C':
				if(_U('t', 'r')) k = KKey.Ctrl;
				else if(_U('a', 'p')) k = KKey.CapsLock;
				break;
			case 'D':
				if(_U('e', 'l')) k = KKey.Delete;
				else if(_U('o', 'w')) k = KKey.Down;
				break;
			case 'E':
				if(_U('n', 't')) k = KKey.Enter;
				else if(_U('n', 'd')) k = KKey.End;
				else if(_U('s', 'c')) k = KKey.Escape;
				break;
			case 'H':
				if(_U('o', 'm')) k = KKey.Home;
				break;
			case 'I':
				if(_U('n', 's')) k = KKey.Insert;
				break;
			case 'L':
				if(_U('e', 'f')) k = KKey.Left;
				//don't need LShift etc
				break;
			case 'M':
				if(_U('e', 'n')) k = KKey.Apps;
				break;
			case 'N':
				if(_U('u', 'm')) k = KKey.NumLock;
				//for NumEnter use Key((KKey.Enter, 0, true))
				break;
			case 'P':
				if(_U('a', 'g') && c3 == 'e') k = c4 == 'u' ? KKey.PageUp : (c4 == 'd' ? KKey.PageDown : 0);
				else if(_U('g', 'u')) k = KKey.PageUp;
				else if(_U('g', 'd')) k = KKey.PageDown;
				else if(_U('a', 'u')) k = KKey.Pause;
				else if(_U('r', 'i') || _U('r', 't')) k = KKey.PrintScreen;
				break;
			case 'R':
				if(_U('i', 'g')) k = KKey.Right;
				else if(_U('a', 'l')) k = KKey.RAlt;
				else if(_U('c', 't')) k = KKey.RCtrl;
				else if(_U('s', 'h')) k = KKey.RShift;
				else if(_U('w', 'i')) k = KKey.RWin;
				break;
			case 'S':
				if(_U('h', 'i')) k = KKey.Shift;
				else if(_U('p', 'a')) k = KKey.Space;
				else if(_U('c', 'r')) k = KKey.ScrollLock;
				//SysRq not used on Windows
				break;
			case 'T':
				if(_U('a', 'b')) k = KKey.Tab;
				break;
			case 'U':
				if(c1 == 'p') k = KKey.Up;
				break;
			case 'W':
				if(_U('i', 'n')) k = KKey.Win;
				break;
			}
			if(k != 0) {
				for(int i2 = i + len; i < i2; i++) if(!Char_.IsAsciiAlpha(s[i])) return 0;
				return k;
			}

			if(c >= 'A' && c <= 'Z') {
				var s1 = s.Substring(i, len);
#if false
				if(Enum.TryParse(s1, true, out KKey r1)) return r1;
				//if(Enum.TryParse(s1, true, out System.Windows.Forms.Keys r2) && (uint)r2 <= 0xff) return (KKey)r2;
#else //20-50 times faster and less garbage. Good JIT speed.
				return _FindKeyInEnums(s1);
#endif
			}
			return 0;

			bool _U(char cc1, char cc2) { return u == ((uint)cc1 << 16 | cc2); }
		}

		static IEnumerable<RXGroup> _SplitKeysString(string keys) =>
			(s_rxKeys ?? (s_rxKeys = new Regex_(@"[A-Z]\w*|#\S|\*\s*(?:\d+|down|up)\b|[+$]\s*\(|\S")))
			.FindAllG(keys ?? "");
		//KeyName | #n | *r | *down | *up | +( | $( | nonspace char
		static Regex_ s_rxKeys;
		//SHOULDDO: don't use Regex_.

		static System.Collections.Hashtable s_htEnum; //with Dictionary much slower JIT
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		static KKey _FindKeyInEnums(string key)
		{
			if(s_htEnum == null) {
				var t = new System.Collections.Hashtable(160 /*220*/, StringComparer.OrdinalIgnoreCase);
				var a1 = typeof(KKey).GetFields();
				for(int j = 1; j < a1.Length; j++) { //note: start from 1 to skip the default value__, it gives exception
					t.Add(a1[j].Name, a1[j].GetRawConstantValue());
				}

				//rejected. Better avoid loading Forms dll. All useful keys are in KKey. For others can use virtual-key codes.
				//var a2 = typeof(System.Windows.Forms.Keys).GetFields();
				//for(int j = 4; j < a2.Length; j++) { //skip value__, KeyCode, Modifiers, None
				//	var v = a2[j];
				//	//Print(v.Name);
				//	if(t.ContainsKey(v.Name)) continue;
				//	var k = v.GetRawConstantValue();
				//	if((uint)(int)k >= 0xff) continue;
				//	Print(v.Name, j);
				//	t.Add(v.Name, k);
				//}

				//Print(a1.Length, /*a2.Length,*/ t.Count); //216 with Keys enum, 156 without
				s_htEnum = t;
			}
			var r = s_htEnum[key];
			if(r == null) return 0;
			if(r is byte) return (KKey)(byte)r;
			return (KKey)(int)r;
			//note: GetRawConstantValue gets byte for KKey, int for Keys. GetValue(null) gets of enum type.
		}

		static ArgumentException _ArgumentException_ErrorInKeysString(string keys, int i, int len)
		{
			return new ArgumentException($"Error in keys string: {keys.Remove(i)}<<<{keys.Substring(i, len)}>>>{keys.Substring(i + len)}");
		}

		/// <summary>
		/// Internal static functions.
		/// </summary>
		internal static class Lib
		{
			/// <summary>
			/// Calls Time.SleepDoEvents.
			/// </summary>
			internal static void Sleep(int ms)
			{
				if(ms > 0) Time.SleepDoEvents(ms);

				//see comments in Mouse._Sleep.
			}

			/// <summary>
			/// If t &gt; 10, returns (t / 4 + 8).
			/// </summary>
			internal static int LimitSleepTime(int t) => t <= 10 ? t : (t / 4 + 8);

			/// <summary>
			/// If k is Shift, Ctrl, Alt or Win, returns it as modifier flag, eg KMod.Shift.
			/// Else returns 0.
			/// Also supports RShift etc.
			/// </summary>
			internal static KMod KeyToMod(KKey k)
			{
				switch(k) {
				case KKey.Shift: case KKey.LShift: case KKey.RShift: return KMod.Shift;
				case KKey.Ctrl: case KKey.LCtrl: case KKey.RCtrl: return KMod.Ctrl;
				case KKey.Alt: case KKey.LAlt: case KKey.RAlt: return KMod.Alt;
				case KKey.Win: case KKey.RWin: return KMod.Win;
				}
				return 0;
			}

			/// <summary>
			/// Gets scan code from virtual-key code.
			/// </summary>
			/// <param name="vk"></param>
			/// <param name="hkl">Keyboard layout. If default(LPARAM), uses of current thread.</param>
			internal static ushort VkToSc(KKey vk, LPARAM hkl = default)
			{
				if(hkl == default) hkl = Api.GetKeyboardLayout(0);
				uint sc = Api.MapVirtualKeyEx((uint)vk, 0, hkl); //MAPVK_VK_TO_VSC

				//fix Windows bugs
				if(vk == KKey.PrintScreen && sc == 0x54) sc = 0x37;
				if(vk == KKey.Pause && sc == 0) sc = 0x45;

				return (ushort)sc;

				//tested: LCtrl, RCtrl etc are correctly sent, although MSDN does not mention that SendInput supports it.
			}

			/// <summary>
			/// Sends one key event.
			/// Just calls API SendInput with raw parameters.
			/// </summary>
			internal static unsafe void SendKeyEventRaw(KKey vk, ushort scan, uint flags)
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
			internal static void SendKey(KKey k, int downUp = 0)
			{
				uint f = 0;
				if(_KeyTypes.IsExtended(k)) f |= Api.KEYEVENTF_EXTENDEDKEY;
				ushort scan = VkToSc(k);

				if(0 == (downUp & 2)) SendKeyEventRaw(k, scan, f);
				if(0 == (downUp & 1)) SendKeyEventRaw(k, scan, f | Api.KEYEVENTF_KEYUP);
			}

			internal static void SendCtrl(bool down) => SendKeyEventRaw(KKey.Ctrl, 0x1D, down ? 0 : Api.KEYEVENTF_KEYUP);
			internal static void SendAlt(bool down) => SendKeyEventRaw(KKey.Alt, 0x38, down ? 0 : Api.KEYEVENTF_KEYUP);
			internal static void SendShift(bool down) => SendKeyEventRaw(KKey.Shift, 0x2A, down ? 0 : Api.KEYEVENTF_KEYUP);
			internal static void SendRCtrlUp() => SendKeyEventRaw(KKey.Ctrl, 0x1D, Api.KEYEVENTF_KEYUP | Api.KEYEVENTF_EXTENDEDKEY);
			internal static void SendRAltUp() => SendKeyEventRaw(KKey.Alt, 0x38, Api.KEYEVENTF_KEYUP | Api.KEYEVENTF_EXTENDEDKEY);
			internal static void SendRShiftUp() => SendKeyEventRaw(KKey.Shift, 0x36, Api.KEYEVENTF_KEYUP);

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
				if(0 != (mod & KMod.Ctrl)) a[n++].Set(KKey.Ctrl, 0x1D, f);
				if(0 != (mod & KMod.Alt)) a[n++].Set(KKey.Alt, 0x38, f);
				if(0 != (mod & KMod.Shift)) a[n++].Set(KKey.Shift, 0x2A, f);
				if(0 != (mod & KMod.Win)) a[n++].Set(KKey.Win, 0x5B, f);
				Api.SendInput(a, n);
			}

			/// <summary>
			/// Releases modifier keys if pressed. Optionally turns off CapsLock if toggled.
			/// Returns true if was CapsLock.
			/// Uses options NoModOff and NoCapsOff. Does not call Hook.
			/// </summary>
			/// <param name="opt"></param>
			/// <param name="forClipb">Used for Clipb Ctrl+V/C/X. Ignore CapsLock and always release modifiers, regardless of opt.</param>
			internal static bool ReleaseModAndCapsLock(OptKey opt, bool forClipb = false)
			{
				//note: don't call Hook here, it does not make sense.

				bool R = !forClipb && !opt.NoCapsOff && IsCapsLock;
				if(R) SendKey(KKey.CapsLock);

				if(forClipb || !opt.NoModOff) {
					bool isLAlt = IsKeyPressedSyncOrAsync(KKey.LAlt);
					bool isRAlt = IsKeyPressedSyncOrAsync(KKey.RAlt);
					bool isLWin = IsKeyPressedSyncOrAsync(KKey.Win);
					bool isRWin = IsKeyPressedSyncOrAsync(KKey.RWin);
					bool isLCtrl = IsKeyPressedSyncOrAsync(KKey.LCtrl);
					bool isRCtrl = IsKeyPressedSyncOrAsync(KKey.RCtrl);
					bool menu = (isLAlt || isRAlt || isLWin || isRWin) && !(isLCtrl || isRCtrl);
					if(menu) SendKey(KKey.Ctrl); //if Alt or Win pressed, send Ctrl to avoid menu mode or Start menu. For Alt works Ctrl-up, but maybe not everywhere. For Win need Ctrl-down-up.
					if(isLCtrl) SendCtrl(false);
					if(isRCtrl) SendRCtrlUp();
					if(IsKeyPressedSyncOrAsync(KKey.LShift)) SendShift(false);
					if(IsKeyPressedSyncOrAsync(KKey.RShift)) SendRShiftUp();
					if(isLAlt) SendAlt(false);
					if(isRAlt) SendRAltUp();
					if(isLWin) SendKey(KKey.Win, 2);
					if(isRWin) SendKey(KKey.RWin, 2);

					//CONSIDER: don't release if pressed by script.
				}

				return R;
			}

			internal static bool IsKeyPressedSyncOrAsync(KKey k)
			{
				return GetKeyState(k) < 0 || GetAsyncKeyState(k) < 0;
			}

			/// <summary>
			/// Sends Ctrl+V or Ctrl+C or Ctrl+X, and/or optionally Enter.
			/// Caller gets opt and wFocus with _GetOptionsAndWndFocused (it may want to know some options too).
			/// Caller calls Press, waits until the target app gets clipboard data, then calls Release.
			/// </summary>
			internal unsafe struct SendCopyPaste
			{
				ushort _scan;
				KKey _vk;
				bool _enter;
				OptKey _opt;

				/// <summary>
				/// Presses Ctrl+key. Does not release.
				/// If enter is true, Release will press Enter.
				/// </summary>
				public void Press(KKey key, OptKey opt, Wnd wFocus, bool enter = false)
				{
					_scan = VkToSc(_vk = key, Api.GetKeyboardLayout(wFocus.ThreadId));
					_enter = enter;
					_opt = opt;

					SendCtrl(true);
					Lib.Sleep(opt.KeySpeedClipboard); //need 1 ms for IE address bar, 100 ms for BlueStacks
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
				public static void Enter(OptKey opt)
				{
					var e = new _KEvent(true, KKey.Enter, 0, 0x1C);
					_SendKey2(e, default, true, opt);
				}
			}

			internal struct INPUTKEY2
			{
				public Api.INPUTK k0, k1;

				public INPUTKEY2(KKey vk, ushort sc, uint flags = 0)
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
		/// Returns OptKey of this variable or OptKey cloned from this variable and possibly modified by Hook.
		/// </summary>
		/// <param name="wFocus">receives the focused or active window. Also the function uses it to avoid frequent calling of Hook.</param>
		/// <param name="getWndAlways">if false, the caller does not need wFocus. Then wFocus will be default(Wnd) if Hook is null.</param>
		OptKey _GetOptionsAndWndFocused(out Wnd wFocus, bool getWndAlways)
		{
			if(Options.Hook == null && !getWndAlways) {
				wFocus = default;
				return Options;
			}
			return _GetOptions(wFocus = Lib.GetWndFocusedOrActive());
		}

		/// <summary>
		/// Returns OptKey of this variable or OptKey cloned from this variable and possibly modified by Hook.
		/// </summary>
		/// <param name="wFocus">the focused or active window. The function uses it to avoid frequent calling of Hook. If you don't have it, use _GetOptionsAndWndFocused instead.</param>
		OptKey _GetOptions(Wnd wFocus)
		{
			var call = Options.Hook;
			if(call == null || wFocus.Is0) return Options;
			if(wFocus != _sstate.wFocus) {
				_sstate.wFocus = wFocus;
				if(_sstate.options == null) _sstate.options = new OptKey(Options); else _sstate.options.LibReset(Options);
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
			enum _KT :byte
			{
				Mod = 1,
				Extended = 2,
				Mouse = 4,
				Toggleable = 8, //API GetKeyState can get the 'toggled' bit
			}

			public static bool IsMod(KKey vk) => _b[(byte)vk].Has_(_KT.Mod);
			public static bool IsExtended(KKey vk) => _b[(byte)vk].Has_(_KT.Extended);
			public static bool IsMouse(KKey vk) => _b[(byte)vk].Has_(_KT.Mouse);
			public static bool IsToggleable(KKey vk) => _b[(byte)vk].Has_(_KT.Toggleable);

			static _KT[] _b;

			static _KeyTypes()
			{
				_b = new _KT[256];

				_b[1] = _b[2] = _b[4] = _b[5] = _b[6] = _KT.Mouse | _KT.Toggleable;
				_b[16] = _b[17] = _b[18] = _KT.Mod | _KT.Toggleable;

				_b[(int)KKey.PageUp] = _b[(int)KKey.PageDown] = _b[(int)KKey.End] = _b[(int)KKey.Home]
					= _b[(int)KKey.Left] = _b[(int)KKey.Up] = _b[(int)KKey.Right] = _b[(int)KKey.Down]
					= _b[(int)KKey.PrintScreen] = _b[(int)KKey.Insert] = _b[(int)KKey.Delete]
					= _b[(int)KKey.Sleep] = _b[(int)KKey.Apps] = _b[(int)KKey.Divide] = _b[(int)KKey.Cancel]
					= _KT.Extended;
				//and more, but undocumented, and I cannot test.
				//	There is no API to get extended keys. MapVirtualKeyEx can get only of 50% keys.

				_b[(int)KKey.CapsLock] = _b[(int)KKey.ScrollLock] = _b[(int)KKey.Back] = _b[(int)KKey.Tab] = _b[(int)KKey.Enter] = _b[(int)KKey.Escape]
					= _b[(int)KKey.LShift] = _b[(int)KKey.RShift] = _b[(int)KKey.LCtrl] = _b[(int)KKey.LAlt] = _KT.Toggleable;
				//also Home and maybe more

				_b[(int)KKey.RCtrl] = _b[(int)KKey.RAlt] = _b[(int)KKey.NumLock] = _KT.Extended | _KT.Toggleable;
				_b[(int)KKey.Win] = _b[(int)KKey.RWin] = _KT.Mod | _KT.Extended | _KT.Toggleable;

				for(int i = (int)KKey.BrowserBack; i <= (int)KKey.LaunchApp2; i++) _b[i] = _KT.Extended; //media/browser/launchapp keys
			}
		}
	}
}
