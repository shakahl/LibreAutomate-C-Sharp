﻿using System;
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

using Au.Types;
using static Au.NoClass;

namespace Au
{
	/// <summary>
	/// Keyboard functions: send virtual keystrokes and text to the active window, get key states.
	/// </summary>
	/// <remarks>
	/// The main function is <see cref="Key"/>. Most documentation is there. See also <see cref="Text"/>. These functions use <see cref="Opt.Key"/>. Alternatively can be used <b>Keyb</b> variables, see <see cref="Keyb(OptKey)"/>.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// Keyb.Key("Ctrl+Shift+Left"); //press Ctrl+Shift+Left
	/// 
	/// Opt.Key.KeySpeed = 300; //set options for static functions
	/// Keyb.Key("Ctrl+A Del Tab*3", "text", "Enter", 500); //press Ctrl+A, press Del, press Tab 3 times, send text, press Enter, wait 100 ms
	/// 
	/// Keyb.Text("text\r\n"); //send text that ends with newline
	/// Keyb.Text("text", "Enter", 300); //send text, press Enter, wait 300 ms
	/// 
	/// Text("Key and Text can be used without the \"Keyb.\" prefix.");
	/// Key("Enter");
	/// ]]></code>
	/// </example>
	public partial class Keyb
	{
		/// <param name="cloneOptions">Options to be copied to <see cref="Options"/> of this variable. If null, uses default options.</param>
		/// <example>
		/// <code><![CDATA[
		/// var k = new Keyb(Opt.Static.Key);
		/// k.Options.KeySpeed = 50;
		/// k.AddKeys("Tab // Space").AddRepeat(3).AddText("text").AddKey(Keys.Enter).AddSleep(500);
		/// k.Send(); //sends and clears the variable
		/// k.Add("Tab // Space*3", "text", Keys.Enter, 500); //does the same as the above k.Add... line
		/// for(int i = 0; i < 5; i++) k.Send(true); //does not clear the variable
		/// ]]></code>
		/// </example>
		public Keyb(OptKey cloneOptions) { Options = new OptKey(cloneOptions); }

		/// <summary>
		/// Options used by this variable.
		/// </summary>
		public OptKey Options { get; }

		//KEYEVENTF_ flags for API SendInput.
		[Flags]
		enum _KFlags :byte
		{
			Extended = 1,
			Up = 2,
			Unicode = 4,
			Scancode = 8,
		};

		//_KEvent type - key, text, sleep, etc.
		enum _KType :byte
		{
			KeyEvent, //send key down or up event, depending on _KFlags.Up. In _KEvent used vk and scan.
			KeyPair, //send key down and up events. In _KEvent used vk and scan.
			Text, //send text. In _KEvent used data, it is _data or _data element index.
			Callback, //call callback function. In _KEvent used data, it is _data or _data element index.
			Repeat, //repeat previous key. In _KEvent used repeat.
			Sleep, //sleep. In _KEvent used sleep.
		}

		[StructLayout(LayoutKind.Explicit)]
		struct _KEvent
		{
			[FieldOffset(0)] internal byte vk; //(byte)Keys
			[FieldOffset(1)] byte _flags; //_KFlags in 0x0F and _KType in 0xF0
			[FieldOffset(2)] internal ushort scan; //scan code if IsKey
			[FieldOffset(2)] internal ushort data; //_data or _data index if IsText or IsCallback
			[FieldOffset(2)] internal ushort repeat; //repeat count if IsRepeat
			[FieldOffset(2)] internal ushort sleep; //milliseconds if IsSleep

			//Event type KeyEvent or KeyPair.
			internal _KEvent(bool pair, Keys vk, _KFlags siFlags, ushort scan = 0) : this()
			{
				this.vk = (byte)vk;
				var f = (byte)siFlags; if(pair) f |= 16; _flags = f;
				this.scan = scan;
			}

			//Event of any type except KeyEvent and KeyPair.
			internal _KEvent(_KType type, ushort data) : this()
			{
				Debug.Assert(type > _KType.KeyPair);
				_flags = (byte)((byte)type << 4);
				this.data = data;
			}

			internal _KType Type => (_KType)((byte)_flags >> 4);
			internal bool IsPair => Type == _KType.KeyPair;
			internal bool IsKey => Type <= _KType.KeyPair;
			internal bool IsText => Type == _KType.Text;
			internal bool IsKeyOrText => Type <= _KType.Text;
			internal bool IsCallback => Type == _KType.Callback;
			internal bool IsRepeat => Type == _KType.Repeat;
			internal bool IsSleep => Type == _KType.Sleep;
			internal bool IsUp => 0 != (_flags & 2);
			internal _KFlags SIFlags => (_KFlags)(_flags & 15);
			internal void MakeDown() => _flags &= 9;
			internal void MakeUp() => _flags = (byte)((_flags & 9) | 2);

#if DEBUG
			public override string ToString()
			{
				if(IsText) { Debug.Assert(SIFlags == 0); return $"text " + data; }
				if(IsCallback) { Debug.Assert(SIFlags == 0); return $"callback " + data; }
				if(IsSleep) { Debug.Assert(SIFlags == 0); return "sleep " + sleep; }
				if(IsRepeat) { Debug.Assert(SIFlags == 0); return "repeat " + repeat; }
				return $"{(Keys)vk,-12} scan={scan,-4} flags={_flags}";
			}
#endif
		}

		//This struct is used to separate parsing-only fields from other fields.
		struct _KParsingState
		{
			public Stack<_KEvent> mod; //pushed on "+" or "+(". Then popped on key not preceded by +, and also in Send().
			public bool paren; //we are between "+(" and ")"
			public bool plus; //we are between "+" and key or text

			//KeyName | #n | *r | *down | *up | +( | $( | nonspace char
			public static readonly Regex_ s_rxKeys = new Regex_(@"[A-Z]\w*|#\S|\* *(?:\d+|down|up)\b|[+$]\s*\(|\S");
		}

		//This struct is used to separate sending-only fields from other fields.
		struct _KSendingState
		{
			public Wnd wFocus;
			public OptKey options;

			public void Clear()
			{
				wFocus = default;
			}
		}

		List<_KEvent> _a = new List<_KEvent>(); //all key events and elements for each text/callback/repeat/sleep
		object _data; //text and callback parts. If there is 1 such part, it is string or Action; else it is List<object>.
		_KParsingState _pstate; //parsing state
		_KSendingState _sstate; //sending state
		bool _sending; //while sending, don't allow to add or send

		/// <summary>
		/// Adds keystrokes to the internal collection. They will be sent by <see cref="Send"/>.
		/// Returns self.
		/// </summary>
		/// <param name="keys">Key names and operators. Example: <c>"Tab Ctrl+V Alt+(E P) Left*3 Space a , 5 #5 $abc"</c>. More info: <see cref="Key"/>. Can be null or "".</param>
		/// <exception cref="ArgumentException">Error in <paramref name="keys"/> string, for example an unknown key name.</exception>
		public Keyb AddKeys(string keys)
		{
			_ThrowIfSending();
			if(Empty(keys)) return this;
			int i = 0, len = 0;
			foreach(var g in _KParsingState.s_rxKeys.FindAllG(keys)) {
				//Print($"<><c 0xC000>{g.Value}</c>"); //continue;
				i = g.Index; len = g.Length;
				char c = keys[i]; _KEvent e;
				switch(c) {
				case '*':
					if(len == 1 || _a.Count == 0) goto ge;
					e = _a[_a.Count - 1];
					char cLast = keys[i + len - 1];
					switch(cLast) {
					case 'n': //down
					case 'p': //up
						if(e.IsPair) {
							//make the last key down-only or up-only
							if(cLast == 'p') e.MakeUp(); else e.MakeDown();
							_a[_a.Count - 1] = e;
						} else if(cLast == 'p' && _FindLastKey(out e, canBeText: false)) {
							//allow eg Key("A*down*3*up") or Key("A*down", 500, "*up")
							e.MakeUp();
							_a.Add(e);
						} else goto ge;
						break;
					default: //repeat
						if(!e.IsKey) goto ge;
						AddRepeat(keys.ToInt32_(i + 1));
						break;
					}
					break;
				case '$': //Shift+ //note: don't add the same for other modifiers. It just makes difficult to remember and read.
					AddKey(Keys.ShiftKey);
					goto case '+';
				case '+':
					if(_pstate.paren || _a.Count == 0) goto ge;
					e = _a[_a.Count - 1];
					if(!e.IsPair) goto ge;
					e.MakeDown();
					_a[_a.Count - 1] = e;
					e.MakeUp();
					if(_pstate.mod == null) _pstate.mod = new Stack<_KEvent>();
					_pstate.mod.Push(e);
					if(len > 1) _pstate.paren = true; //"*("
					else _pstate.plus = true;
					break;
				case ')':
					if(!_pstate.paren) goto ge;
					_pstate.paren = false;
					_AddModUp();
					break;
				default:
					var k = _KeynameToKey(keys, i, len);
					if(k == 0) goto ge;
					AddKey(k);
					//Print(k);
					break;
				}
			}
			return this;
			ge:
			throw new ArgumentException($"Error in keys string: {keys.Remove(i)}<<<{keys.Substring(i, len)}>>>{keys.Substring(i + len)}");

			bool _FindLastKey(out _KEvent e, bool canBeText)
			{
				for(int j = _a.Count - 1; j >= 0; j--) {
					var t = _a[j];
					if(canBeText ? t.IsKeyOrText : t.IsKey) { e = t; return true; }
				}
				e = default; return false;
			}
		}

		//Adds mod up events if need: if _parsing.mod is not empty and + is not active and ( is not active.
		void _AddModUp()
		{
			if(!_pstate.plus && !_pstate.paren && _pstate.mod != null) {
				while(_pstate.mod.Count != 0) _a.Add(_pstate.mod.Pop());
			}
		}

		//Adds key or other event. Calls _ModUp(). Not used fo sleep and repeat.
		Keyb _AddKey(_KEvent e)
		{
			_AddModUp();
			_pstate.plus = false;
			_a.Add(e);
			return this;
		}

		/// <summary>
		/// Adds single key, specified as <see cref="Keys"/>, to the internal collection. It will be sent by <see cref="Send"/>.
		/// Returns self.
		/// </summary>
		/// <param name="key">Virtual-key code, as <see cref="Keys"/> or int like <c>(Keys)200</c>. Valid values are 1-255.</param>
		/// <param name="down">true - key down; false - key up; null (default) - key down-up.</param>
		/// <exception cref="ArgumentException">Invalid <paramref name="key"/>, for example contains modifier flags <b>Control</b>, <b>Shift</b> or <b>Alt</b> (instead use <b>ControlKey</b>, <b>ShiftKey</b> or <b>Menu</b>).</exception>
		public Keyb AddKey(Keys key, bool? down = null)
		{
			_ThrowIfSending();
			if(key == 0 || (uint)key > 255) throw new ArgumentException("Invalid value.", nameof(key));

			bool isPair; _KFlags f = 0;
			if(!(isPair = (down == null)) && !down.GetValueOrDefault()) f |= _KFlags.Up;
			if(_KeyTypes.IsExtended((byte)key)) f |= _KFlags.Extended;

			return _AddKey(new _KEvent(isPair, key, f));
		}

		/// <summary>
		/// Adds single key to the internal collection. Allows to specify scan code and whether it is an extended key. It will be sent by <see cref="Send"/>.
		/// Returns self.
		/// </summary>
		/// <param name="key">Virtual-key code, as <see cref="Keys"/> or int like <c>(Keys)200</c>. Valid values are 1-255. Can be 0.</param>
		/// <param name="scanCode">Scan code of the physical key. Scan code values are 1-127, but this function allows 1-0xffff. Can be 0.</param>
		/// <param name="extendedKey">true if the key is an extended key.</param>
		/// <param name="down">true - key down; false - key up; null (default) - key down-up.</param>
		/// <exception cref="ArgumentException">Invalid <paramref name="key"/>, for example contains modifier flags <b>Control</b>, <b>Shift</b> or <b>Alt</b> (instead use <b>ControlKey</b>, <b>ShiftKey</b> or <b>Menu</b>). Or invalid scan code.</exception>
		public Keyb AddKey(Keys key, int scanCode, bool extendedKey, bool? down = null)
		{
			_ThrowIfSending();
			if((uint)scanCode > 0xffff) throw new ArgumentException("Invalid value.", nameof(scanCode));
			bool isPair; _KFlags f = 0;
			if(key == 0) f = _KFlags.Scancode;
			else {
				if((uint)key > 255) throw new ArgumentException("Invalid value.", nameof(key));
				//don't: if extendedKey false, set true if need. Don't do it because this func is named 'raw'.
			}

			if(!(isPair = (down == null)) && !down.GetValueOrDefault()) f |= _KFlags.Up;
			if(extendedKey) f |= _KFlags.Extended;

			return _AddKey(new _KEvent(isPair, key, f, (ushort)scanCode));
		}

		/// <summary>
		/// Adds key down or up event.
		/// </summary>
		/// <param name="vk"></param>
		/// <param name="scan"></param>
		/// <param name="siFlags">SendInput flags.</param>
		internal Keyb LibAddRaw(byte vk, ushort scan, byte siFlags)
		{
			_ThrowIfSending();
			return _AddKey(new _KEvent(false, (Keys)vk, (_KFlags)(siFlags & 0xf), scan));
		}

		/// <summary>
		/// Adds text. It will be sent by <see cref="Send"/>.
		/// Returns self.
		/// </summary>
		/// <param name="text">Text. Can be null.</param>
		/// <remarks>
		/// To send text can be used keys or clipboard, depending on <see cref="Opt.Key"/> and text.
		/// </remarks>
		public Keyb AddText(string text)
		{
			_ThrowIfSending();
			if(!Empty(text)) _AddKey(new _KEvent(_KType.Text, _SetData(text)));
			return this;
		}

		//Adds text (string) or callback (Action) to _data.
		ushort _SetData(object x)
		{
			int i;
			if(_data == null) {
				i = 0;
				_data = x;
			} else if(_data is List<object> a) {
				i = a.Count;
				a.Add(x);
			} else {
				i = 1;
				_data = new List<object>() { _data, x };
			}
			return checked((ushort)i);
		}

		//Gets text (string) or callback (Action) from _data.
		object _GetData(ushort i)
		{
			if(_data is List<object> a) return a[i];
			return _data;
		}

		/// <summary>
		/// Adds a callback function.
		/// Returns self.
		/// </summary>
		/// <param name="callback"></param>
		/// <remarks>
		/// The callback function will be called by <see cref="Send"/> and can do anything except sending keys and copy/paste.
		/// </remarks>
		public Keyb AddCallback(Action callback)
		{
			_ThrowIfSending();
			if(callback == null) throw new ArgumentNullException();
			return _AddKey(new _KEvent(_KType.Callback, _SetData(callback)));
		}

		/// <summary>
		/// Adds the repeat-key operator. Then <see cref="Send"/> will send the last added key the specified number of times.
		/// Returns self.
		/// </summary>
		/// <param name="count">Repeat count.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> &gt;10000 or &lt;0.</exception>
		/// <exception cref="ArgumentException">The last added item is not key. Can repeat only single key; cannot repeat text etc.</exception>
		public Keyb AddRepeat(int count)
		{
			_ThrowIfSending();
			if((uint)count > 10000) throw new ArgumentOutOfRangeException(nameof(count), "Max repeat count is 10000.");
			int i = _a.Count; if(i == 0 || !_a[i - 1].IsKey) throw new ArgumentException("No key to repeat.");
			_a.Add(new _KEvent(_KType.Repeat, (ushort)count));
			return this;
		}

		/// <summary>
		/// Adds a short pause. Then <see cref="Send"/> will sleep (wait).
		/// Returns self.
		/// </summary>
		/// <param name="timeMS">Time to sleep, milliseconds.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeMS"/> &gt;10000 (1 minute) or &lt;0.</exception>
		public Keyb AddSleep(int timeMS)
		{
			_ThrowIfSending();
			if((uint)timeMS > 10000) throw new ArgumentOutOfRangeException(nameof(timeMS), "Max sleep time is 10000.");
			_a.Add(new _KEvent(_KType.Sleep, (ushort)timeMS));
			return this;
		}

		//CONSIDER: AddWaitFocusChanged(float timeS)
		//	Eg when showing Open/SaveAs dialog, the file Edit control receives focus after 200 ms. Sending text to it works anyway, but the script fails if then it clicks OK not with keys (eg with Acc).

		/// <summary>
		/// Adds any number of keystrokes, text, sleep and other events to the internal collection. They will be sent/executed by <see cref="Send"/>.
		/// Returns self.
		/// </summary>
		/// <param name="keysEtc">Arguments. The same as with <see cref="Key"/>.</param>
		/// <exception cref="ArgumentException">An argument is of an unsupported type or has an invalid value, for example unknown key name.</exception>
		public Keyb Add(params object[] keysEtc)
		{
			_ThrowIfSending();
			if(keysEtc != null) {
				bool wasKeysString = false;
				for(int i = 0; i < keysEtc.Length; i++) {
					var o = keysEtc[i] ?? "";
					switch(o) {
					case string s:
						if(!wasKeysString) {
							wasKeysString = true;
							AddKeys(s);
							continue;
						}
						AddText(s);
						break;
					case Keys k:
						AddKey(k);
						break;
					case int ms:
						AddSleep(ms);
						break;
					//case double sec: //rejected
					//	AddSleep(checked((int)(sec * 1000d)));
					//	break;
					case Action g:
						AddCallback(g);
						break;
					case ValueTuple<Keys, int, bool> t:
						AddKey(t.Item1, t.Item2, t.Item3);
						break;
					case ValueTuple<int, bool> t:
						AddKey(0, t.Item1, t.Item2);
						break;
					default: throw new ArgumentException("Bad type. Expected string, Keys, int, double, Action, (Keys, int, bool) or (int, bool).");
					}
					wasKeysString = false;
				}
			}
			return this;
		}

		/// <summary>
		/// Sends keys, text and executes other events added with the <b>AddX</b> functions.
		/// </summary>
		/// <param name="canSendAgain">Don't clear the internal collection. If true, this function then can be called again (eg in loop) to send/execute the same keys etc. If false (default), clears the added keys etc; then you can call <b>AddX</b> functions and <b>Send</b> again.</param>
		/// <exception cref="ArgumentException"><paramref name="canSendAgain"/> is true and keys end with + or (.</exception>
		public void Send(bool canSendAgain = false)
		{
			_ThrowIfSending();
			if(_a.Count == 0) return;
			if(canSendAgain) {
				if(_pstate.paren || _pstate.plus) throw new ArgumentException("canSendAgain cannot be true if keys ends with + or (");
			}

			//Print("-- _parsing.mod --");
			//Print(_parsing.mod);

			_AddModUp(); //add mod-up events if need, eg Ctrl-up after "Ctrl+A"

			//Print("-- _a --");
			//Print(_a);

			//Perf.First();
			int sleepFinally = 0;
			bool restoreCapsLock = false;
			var bi = new BlockUserInput() { ResendBlockedKeys = true };
			try {
				_sending = true;
				if(!Options.NoBlockInput) bi.Start(BIEvents.Keys);
				restoreCapsLock = Lib.ReleaseModAndCapsLock(Options);
				//Perf.Next();
				for(int i = 0; i < _a.Count; i++) {
					var k = _a[i];
					switch(k.Type) {
					case _KType.Sleep:
						if(i == _a.Count - 1) sleepFinally = k.sleep;
						else Lib.Sleep(k.sleep);
						break;
					case _KType.Repeat:
						Debug.Assert(i > 0 && _a[i - 1].IsKey);
						break;
					case _KType.Callback:
						(_GetData(k.data) as Action)();
						break;
					case _KType.Text:
						_SendText(k);
						break;
					default:
						_SendKey(k, i);
						break;
					}
				}
				//Perf.Next();
				sleepFinally += _GetOptionsAndWndFocused(out _, false).SleepFinally;
			}
			finally {
				if(restoreCapsLock) Lib.SendKey(Keys.CapsLock);
				_sending = false;
				bi.Dispose();
				//Perf.NW();

				//if canSendAgain, can be used like: AddX(); for(...) Send();
				//else can be used like: AddX(); Send(); AddX(); Send();
				if(!canSendAgain) {
					_a.Clear();
					_data = null;
					_sstate.Clear();
					//and don't clear _pstate
				}
			}

			if(sleepFinally > 0) Lib.Sleep(sleepFinally);

			//_SyncWait();
			//CONSIDER: instead of SleepFinally use TimeSyncFinally, default 100 ms. Eg send a sync key and wait max TimeSyncFinally ms.
			//	Don't sync after each (or some) sent key. Usually it does not make sense. The final sync/sleep is useful if next statement is not an input function.
			//Sync problems:
			//	Tried many ways, nothing is good enough. The test code now is in the "Unused" project.
			//	The best would be non-LL keyboard hook that sets event when receives our sent special key-up. Especially when combined with 'get thread CPU usage' while waiting for the event. However these hooks don't work eg in Store apps.
			//Better add a Sync function (Keyb.Sync) or/and special key name, let users do it explicitly where need.
		}

		unsafe void _SendKey(_KEvent k, int i)
		{
			bool needScanCode = k.scan == 0 && !k.SIFlags.HasAny_(_KFlags.Scancode | _KFlags.Unicode);
			var opt = _GetOptionsAndWndFocused(out var wFocus, needScanCode);
			if(needScanCode) {
				var hkl = Api.GetKeyboardLayout(wFocus.ThreadId); //most layouts have the same standard scancodes, but eg dvorak different
				k.scan = Lib.VkToSc(k.vk, hkl);
			}

			bool isLast = i == _a.Count - 1;
			_SendKey2(k, isLast ? default : _a[i + 1], isLast, opt);
		}

		//Caller should set k.scan; this func doesn't.
		unsafe static void _SendKey2(_KEvent k, _KEvent kNext, bool isLast, OptKey opt)
		{
			var ki = new Api.INPUTK(k.vk, k.scan, (uint)k.SIFlags);

			int count = 1, sleep = opt.KeySpeed;
			if(isLast) {
				if(!k.IsPair) sleep = _LimitSleepTime(sleep) - opt.SleepFinally;
			} else {
				if(kNext.IsRepeat) count = kNext.repeat;
				else if(!k.IsPair) {
					//If this is pair, sleep between down and up, and don't sleep after up.
					//Else if repeat, sleep always.
					//Else in most cases don't need to sleep. In some cases need, but can limit the time.
					//	For example, in Ctrl+C normally would not need to sleep after Ctrl down and Ctrl up.
					//	However some apps/controls then may not work. Maybe they process mod and nonmod keys somehow async.
					//	For example, Ctrl+C in IE address bar often does not work if there is no sleep after Ctrl down. Always works if 1 ms.

					sleep = _LimitSleepTime(sleep);
					if(kNext.IsKey) {
						bool thisMod = _KeyTypes.IsMod(k.vk), nextMod = _KeyTypes.IsMod(kNext.vk);
						if(!k.IsUp) {
							if(kNext.IsUp) sleep = opt.KeySpeed;
							else if(thisMod == nextMod) sleep = 0;
						} else {
							if(!thisMod || nextMod) sleep = 0;
						}
					} else if(kNext.IsSleep) sleep = sleep - kNext.sleep;
				}
			}
			if(sleep < 0) sleep = 0;

			//var s = ((Keys)k.vk).ToString();
			//if(k.IsPair) Print($"{s}<{sleep}>");
			//else { var ud = k.IsUp ? '-' : '+'; if(sleep > 0) Print($"{s}{ud} {sleep}"); else Print($"{s}{ud}"); }

			for(int r = 0; r < count; r++) {
				//Perf.First();
				Api.SendInput(&ki);
				//Perf.Next();
				if(sleep > 0) {
					Lib.Sleep(sleep);
				}
				if(k.IsPair) {
					ki.dwFlags |= Api.KEYEVENTF_KEYUP;
					Api.SendInput(&ki);
					ki.dwFlags &= ~Api.KEYEVENTF_KEYUP;

				}
				//Perf.NW();
				//speed: min 400 mcs for each event. Often > 1000. Does not depend on whether all events sent by single SendInput call.
			}
		}

		unsafe void _SendText(_KEvent ke)
		{
			string s = _GetData(ke.data) as string;
			var opt = _GetOptionsAndWndFocused(out var wFocus, true);
			var textOption = opt.TextOption;

			if(s.Length >= opt.PasteLength) textOption = KTextOption.Paste;

			if(textOption != KTextOption.Paste) {
				//use paste if there are Unicode surrogate pairs, because some apps/controls/frameworks don't support surrogates with WM_PACKET.
				//known apps that support: standard Edit and RichEdit controls, Chrome, Firefox, IE, Edge, WPF, WinForms, new Scintilla, Dreamweaver, LibreOffice.
				//known apps that don't: Office 2003, OpenOffice, old Scintilla (QM2).
				//known apps that don't if 0 sleep: QT edit controls in VirtualBox.
				//known apps that don't support these chars even when pasting: Java (tested the old and new frameworks).
				//tested: the same if SendInput(arrayOfAllChars).
				for(int i = 0; i < s.Length; i++) if((s[i] & 0xf800) == 0xd800) { textOption = KTextOption.Paste; break; }
			}

			if(textOption == KTextOption.Paste) {
				Clipb.LibPaste(s, opt, wFocus);
				return;
			}

			LPARAM hkl = textOption == KTextOption.Keys ? Api.GetKeyboardLayout(wFocus.ThreadId) : default;
			KMod prevMod = 0;

			int sleep = opt.TextSpeed;
			//rejected: option to sleep 1 ms every n-th char (eg use float 0...1 or negative value). Does nothing good. Need a better way to wait while the target app swallows large text.

			try {
				for(int i = 0; i < s.Length; i++) {
					char c = s[i];
					bool lastChar = i == s.Length - 1;

					//replace \r, \n and \r\n with key Enter.
					//	Cannot use WM_PACKET. Eg Word ignores \n, WordPad both. Edit control adds newline for both.
					if(c == '\r') {
						if(!lastChar && s[i + 1] == '\n') continue;
						c = '\n';
					}

					int vk = 0; KMod mod = 0;
					if(c == '\n') {
						vk = (int)Keys.Enter;
					} else if(textOption == KTextOption.Keys) {
						short km = Api.VkKeyScanEx(c, hkl); //note: call for non-ASCII char too; depending on keyboard layout it can succeed
						if(0 == (km & 0xf800)) { //-1 if failed, mod flag 8 Hankaku key, 16/32 reserved for driver
							vk = km & 0xff; mod = (KMod)(km >> 8);
						}
						//Print(c, vk, mod, (ushort)km);
					}

					if(vk == 0) { //use vk_packet
						if(prevMod != 0) { Lib.ModPressRelease(false, prevMod); prevMod = 0; }

						//note: need key-up event for VK_PACKET too.
						//	Known controls that need it: Qt edit controls; Office 2003 'type question' field.
					} else if(mod != prevMod) {
						var pm = prevMod; prevMod |= mod; //to release in case of exception between here and 'prevMod = mod'
						if(0 != (mod ^ pm & KMod.Ctrl)) Lib.SendCtrl(0 != (mod & KMod.Ctrl));
						if(0 != (mod ^ pm & KMod.Alt)) Lib.SendAlt(0 != (mod & KMod.Alt));
						if(0 != (mod ^ pm & KMod.Shift)) Lib.SendShift(0 != (mod & KMod.Shift));
						prevMod = mod;
						if(sleep > 0) Lib.Sleep(_LimitSleepTime(sleep)); //need for apps that process mod-nonmod keys async. Now I did not found such apps, but had one in the past.
					}

					var ki = new Lib.INPUTKEY2((byte)vk, vk == 0 ? c : Lib.VkToSc((byte)vk, hkl), vk == 0 ? Api.KEYEVENTF_UNICODE : 0);
					Api.SendInput(&ki.k0, sleep > 0 ? 1 : 2);
					if(sleep > 0) {
						Lib.Sleep(sleep);
						Api.SendInput(&ki.k1, 1);
					}
				}
			}
			finally {
				Lib.ModPressRelease(false, prevMod);
			}

			//FUTURE: try this sync method if long text, eg > 500 char:
			//	In a thread pool thread measure CPU of the target thread. If near 100%, sleep eg 100 ms eg every 100 chars.

			//rejected: throw if changed the focused window.
			//	Possible false positives, because everything is async.
		}

		static int _LimitSleepTime(int t) => t <= 10 ? t : (t / 4 + 10);
	}
}
