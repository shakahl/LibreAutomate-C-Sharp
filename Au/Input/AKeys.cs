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
	/// <summary>
	/// Keyboard functions: send virtual keystrokes and text to the active window, get key states.
	/// </summary>
	/// <remarks>
	/// The main function is <see cref="Key"/>. Most documentation is there. See also <see cref="Text"/>. These functions use <see cref="AOpt.Key"/>. Alternatively can be used <b>AKeys</b> variables, see <see cref="AKeys(OKey)"/>.
	/// </remarks>
	/// <example>
	/// <code><![CDATA[
	/// AKeys.Key("Ctrl+Shift+Left"); //press Ctrl+Shift+Left
	/// 
	/// AOpt.Key.KeySpeed = 300; //set options for static functions
	/// AKeys.Key("Ctrl+A Del Tab*3", "!text", "Enter", 500); //press Ctrl+A, Del, Tab 3 times, send text, Enter, wait 500 ms
	/// 
	/// AKeys.Text("text\r\n"); //send text that ends with newline
	/// ]]></code>
	/// </example>
	public partial class AKeys
	{
		/// <param name="cloneOptions">Options to be copied to <see cref="Options"/> of this variable. If null, uses default options.</param>
		/// <example>
		/// <code><![CDATA[
		/// var k = new AKeys(AOpt.Static.Key);
		/// k.Options.KeySpeed = 50;
		/// k.AddKeys("Tab // Space").AddRepeat(3).AddText("text").AddKey(KKey.Enter).AddSleep(500);
		/// k.Send(); //sends and clears the variable
		/// k.Add("Tab // Space*3", "!text", KKey.Enter, 500); //the same as the above k.AddKeys... line
		/// for(int i = 0; i < 5; i++) k.Send(true); //does not clear the variable
		/// ]]></code>
		/// </example>
		public AKeys(OKey cloneOptions) { Options = new OKey(cloneOptions); }

		/// <summary>
		/// Options used by this variable.
		/// </summary>
		public OKey Options { get; }

		//KEYEVENTF_ flags for API SendInput.
		[Flags]
		enum _KFlags : byte
		{
			Extended = 1,
			Up = 2,
			Unicode = 4,
			Scancode = 8,
		};

		//_KEvent type - key, text, sleep, etc.
		enum _KType : byte
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
			[FieldOffset(0)] internal KKey vk; //byte
			[FieldOffset(1)] byte _flags; //_KFlags in 0x0F and _KType in 0xF0
			[FieldOffset(2)] internal ushort scan; //scan code if IsKey
			[FieldOffset(2)] internal ushort data; //_data or _data index if IsText or IsCallback
			[FieldOffset(2)] internal ushort repeat; //repeat count if IsRepeat
			[FieldOffset(2)] internal ushort sleep; //milliseconds if IsSleep

			//Event type KeyEvent or KeyPair.
			internal _KEvent(bool pair, KKey vk, _KFlags siFlags, ushort scan = 0) : this() {
				this.vk = vk;
				var f = (byte)siFlags; if (pair) f |= 16; _flags = f;
				this.scan = scan;
			}

			//Event of any type except KeyEvent and KeyPair.
			internal _KEvent(_KType type, ushort data) : this() {
				Debug.Assert(type > _KType.KeyPair);
				_flags = (byte)((byte)type << 4);
				this.data = data;
			}

			internal _KType Type => (_KType)(_flags >> 4);
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
				return $"{vk,-12} scan={scan,-4} flags={_flags}";
			}
#endif
		}

		//This struct is used to separate parsing-only fields from other fields.
		struct _KParsingState
		{
			public Stack<_KEvent> mod; //pushed on "+" or "+(". Then popped on key not preceded by +, and also in Send().
			public bool paren; //we are between "+(" and ")"
			public bool plus; //we are between "+" and key or text
		}

		//This struct is used to separate sending-only fields from other fields.
		struct _KSendingState
		{
			public AWnd wFocus;
			public OKey options;

			public void Clear() {
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
		/// Returns this.
		/// </summary>
		/// <param name="keys">
		/// Key names and operators, like with <see cref="Key"/>. Can be null or "".
		/// Example: <c>"Tab Ctrl+V Alt+(E P) Left*3 Space a , 5 #5 $abc"</c>.
		/// If has prefix "!" or "%", calls <see cref="AddText"/>; "!" for text, "%" for HTML.
		/// </param>
		/// <exception cref="ArgumentException">Error in <i>keys</i> string, for example an unknown key name.</exception>
		public AKeys AddKeys([ParamString(PSFormat.AKeys)] string keys) {
			_ThrowIfSending();
			if (keys.Starts('!')) return AddText(keys[1..]);
			if (keys.Starts('%')) return AddText(null, keys[1..]);
			if (keys.NE()) return this;
			int i = 0, len = 0;
			foreach (var g in _SplitKeysString(keys)) {
				//AOutput.Write($"<><c 0xC000>{g.Value}</c>"); //continue;
				i = g.Start; len = g.Length;
				char c = keys[i]; _KEvent e;
				switch (c) {
				case '*':
					if (len == 1 || _a.Count == 0) goto ge;
					e = _a[^1];
					char cLast = keys[i + len - 1];
					switch (cLast) {
					case 'n': //down
					case 'p': //up
						if (e.IsPair) {
							//make the last key down-only or up-only
							if (cLast == 'p') e.MakeUp(); else e.MakeDown();
							_a[^1] = e;
						} else if (cLast == 'p' && _FindLastKey(out e, canBeText: false)) {
							//allow eg Key("A*down*3*up") or Key("A*down", 500, "*up")
							e.MakeUp();
							_a.Add(e);
						} else goto ge;
						break;
					default: //repeat
						if (!e.IsKey) goto ge;
						AddRepeat(keys.ToInt(i + 1));
						break;
					}
					break;
				//rejected. Rarely used and not easy to read.
				//case '$': //Shift+ //note: don't add the same for other modifiers. It just makes not easy to remember and read.
				//	AddKey(KKey.Shift);
				//	goto case '+';
				case '+':
					if (_pstate.paren || _a.Count == 0) goto ge;
					e = _a[^1];
					if (!e.IsPair) goto ge;
					e.MakeDown();
					_a[^1] = e;
					e.MakeUp();
					_pstate.mod ??= new Stack<_KEvent>();
					_pstate.mod.Push(e);
					if (len > 1) _pstate.paren = true; //"*("
					else _pstate.plus = true;
					break;
				case ')':
					if (!_pstate.paren) goto ge;
					_pstate.paren = false;
					_AddModUp();
					break;
				default:
					var k = _KeynameToKey(keys, i, len);
					if (k == 0) goto ge;
					AddKey(k);
					//AOutput.Write(k);
					break;
				}
			}
			return this;
			ge: throw _ArgumentException_ErrorInKeysString(keys, i, len);

			bool _FindLastKey(out _KEvent e, bool canBeText) {
				for (int j = _a.Count - 1; j >= 0; j--) {
					var t = _a[j];
					if (canBeText ? t.IsKeyOrText : t.IsKey) { e = t; return true; }
				}
				e = default; return false;
			}
		}

		//Adds mod up events if need: if _parsing.mod is not empty and + is not active and ( is not active.
		void _AddModUp() {
			if (!_pstate.plus && !_pstate.paren && _pstate.mod != null) {
				while (_pstate.mod.Count != 0) _a.Add(_pstate.mod.Pop());
			}
		}

		//Adds key or other event. Calls _ModUp(). Not used fo sleep and repeat.
		AKeys _AddKey(_KEvent e) {
			_AddModUp();
			_pstate.plus = false;
			_a.Add(e);
			return this;
		}

		/// <summary>
		/// Adds single key, specified as <see cref="KKey"/>, to the internal collection. It will be sent by <see cref="Send"/>.
		/// Returns this.
		/// </summary>
		/// <param name="key">Virtual-key code, as <see cref="KKey"/> or int like <c>(KKey)200</c>. Valid values are 1-255.</param>
		/// <param name="down">true - key down; false - key up; null (default) - key down-up.</param>
		/// <exception cref="ArgumentException">Invalid <i>key</i> (0).</exception>
		public AKeys AddKey(KKey key, bool? down = null) {
			_ThrowIfSending();
			if (key == 0) throw new ArgumentException("Invalid value.", nameof(key));

			bool isPair; _KFlags f = 0;
			if (!(isPair = (down == null)) && !down.GetValueOrDefault()) f |= _KFlags.Up;
			if (KeyTypes_.IsExtended(key)) f |= _KFlags.Extended;

			return _AddKey(new _KEvent(isPair, key, f));
		}

		/// <summary>
		/// Adds single key to the internal collection. Allows to specify scan code and whether it is an extended key. It will be sent by <see cref="Send"/>.
		/// Returns this.
		/// </summary>
		/// <param name="key">Virtual-key code, as <see cref="KKey"/> or int like <c>(KKey)200</c>. Valid values are 1-255. Can be 0.</param>
		/// <param name="scanCode">Scan code of the physical key. Scan code values are 1-127, but this function allows 1-0xffff. Can be 0.</param>
		/// <param name="extendedKey">true if the key is an extended key.</param>
		/// <param name="down">true - key down; false - key up; null (default) - key down-up.</param>
		/// <exception cref="ArgumentException">Invalid scan code.</exception>
		public AKeys AddKey(KKey key, ushort scanCode, bool extendedKey, bool? down = null) {
			_ThrowIfSending();
			bool isPair; _KFlags f = 0;
			if (key == 0) f = _KFlags.Scancode;
			else {
				//don't: if extendedKey false, set true if need. Don't do it because this func is 'raw'.
			}

			if (!(isPair = (down == null)) && !down.GetValueOrDefault()) f |= _KFlags.Up;
			if (extendedKey) f |= _KFlags.Extended;

			return _AddKey(new _KEvent(isPair, key, f, scanCode));
		}

		/// <summary>
		/// Adds key down or up event.
		/// </summary>
		/// <param name="vk"></param>
		/// <param name="scan"></param>
		/// <param name="siFlags">SendInput flags.</param>
		internal AKeys AddRaw_(KKey vk, ushort scan, byte siFlags) {
			_ThrowIfSending();
			return _AddKey(new _KEvent(false, vk, (_KFlags)(siFlags & 0xf), scan));
		}

		/// <summary>
		/// Sends key events added by AInputBlocker -> AddRaw_.
		/// Simply calls Api.SendInput. No options, no sleep, etc.
		/// If new events added while sending, sends them too, until there are no new events added.
		/// </summary>
		/// <param name="onlyUp">Send only 'up' events.</param>
		internal unsafe void SendBlocked_(bool onlyUp) {
			g1:
			int n = 0;
			var a = new Api.INPUTK[_a.Count];
			for (int i = 0; i < _a.Count; i++) {
				var k = _a[i];
				if (onlyUp && !k.IsUp) continue;
				a[n++].Set(k.vk, k.scan, (uint)k.SIFlags);
			}
			_a.Clear();
			if (n == 0) return;
			fixed (Api.INPUTK* p = a) Api.SendInput(p, n);
			//ATime.DoEvents(); //sometimes catches one more event, but not necessary
			if (_a.Count > 0) goto g1; //the hook proc is called while in SendInput. If we don't retry, new blocked keys are lost.
		}

		/// <summary>
		/// Adds text or HTML. It will be sent by <see cref="Send"/>.
		/// Returns this.
		/// </summary>
		/// <param name="text">Text. Can be null.</param>
		/// <param name="html">
		/// HTML. Can be full HTML or fragment. See <see cref="AClipboardData.AddHtml"/>.
		/// Can be specified only <i>text</i> or only <i>html</i> or both. If both, will paste <i>html</i> in apps that support it, elsewhere <i>text</i>. If only <i>html</i>, in apps that don't support HTML will paste <i>html</i> as text.
		/// </param>
		/// <remarks>
		/// To send text can use keys, characters or clipboard, depending on <see cref="AOpt.Key"/> and text. If <i>html</i> not null, uses clipboard.
		/// </remarks>
		public AKeys AddText(string text, string html = null) {
			_ThrowIfSending();
			if (!html.NE()) {
				var data = new AClipboardData().AddHtml(html).AddText(text ?? html);
				var ke = new _KEvent(_KType.Text, _SetData(data));
				_AddKey(ke);
			} else if (!text.NE()) {
				var ke = new _KEvent(_KType.Text, _SetData(text));
				_AddKey(ke);
			}
			return this;
		}

		/// <summary>
		/// Adds text with explicitly specified sending method (keys, characters or paste).
		/// Returns this.
		/// </summary>
		/// <param name="text">Text. Can be null.</param>
		/// <param name="how">Overrides <see cref="OKey.TextHow"/>.</param>
		public AKeys AddText(string text, OKeyText how) {
			_ThrowIfSending();
			if (!text.NE()) {
				var ke = new _KEvent(_KType.Text, _SetData(text)) { vk = (KKey)((byte)how | 0x80) };
				_AddKey(ke);
			}
			return this;
		}

		/// <summary>
		/// Adds clipboard data, for example several formats. It will be pasted by <see cref="Send"/>.
		/// Returns this.
		/// </summary>
		/// <param name="cd">Clipboard data.</param>
		public AKeys AddClipboardData(AClipboardData cd) {
			_ThrowIfSending();
			if (cd == null) throw new ArgumentNullException();
			_AddKey(new _KEvent(_KType.Text, _SetData(cd)));
			return this;
		}

		//Adds text (string) or AClipboardData or callback (Action) to _data.
		ushort _SetData(object x) {
			int i;
			if (_data == null) {
				i = 0;
				_data = x;
			} else if (_data is List<object> a) {
				i = a.Count;
				a.Add(x);
			} else {
				i = 1;
				_data = new List<object>() { _data, x };
			}
			return checked((ushort)i);
		}

		//Gets text (string) or AClipboardData or callback (Action) from _data.
		object _GetData(ushort i) {
			if (_data is List<object> a) return a[i];
			return _data;
		}

		/// <summary>
		/// Adds a callback function.
		/// Returns this.
		/// </summary>
		/// <param name="a"></param>
		/// <remarks>
		/// The callback function will be called by <see cref="Send"/> and can do anything except sending keys and copy/paste.
		/// </remarks>
		public AKeys AddAction(Action a) {
			_ThrowIfSending();
			if (a == null) throw new ArgumentNullException();
			return _AddKey(new _KEvent(_KType.Callback, _SetData(a)));
		}

		/// <summary>
		/// Adds the repeat-key operator. Then <see cref="Send"/> will send the last added key the specified number of times.
		/// Returns this.
		/// </summary>
		/// <param name="count">Repeat count.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>count</i> &gt;10000 or &lt;0.</exception>
		/// <exception cref="ArgumentException">The last added item is not key. Can repeat only single key; cannot repeat text etc.</exception>
		public AKeys AddRepeat(int count) {
			_ThrowIfSending();
			if ((uint)count > 10000) throw new ArgumentOutOfRangeException(nameof(count), "Max repeat count is 10000.");
			int i = _a.Count; if (i == 0 || !_a[i - 1].IsKey) throw new ArgumentException("No key to repeat.");
			_a.Add(new _KEvent(_KType.Repeat, (ushort)count));
			return this;
		}

		/// <summary>
		/// Adds a short pause. Then <see cref="Send"/> will sleep (wait).
		/// Returns this.
		/// </summary>
		/// <param name="timeMS">Time to sleep, milliseconds.</param>
		/// <exception cref="ArgumentOutOfRangeException"><i>timeMS</i> &gt;10000 (1 minute) or &lt;0.</exception>
		public AKeys AddSleep(int timeMS) {
			_ThrowIfSending();
			if ((uint)timeMS > 10000) throw new ArgumentOutOfRangeException(nameof(timeMS), "Max sleep time is 10000.");
			_a.Add(new _KEvent(_KType.Sleep, (ushort)timeMS));
			return this;
		}

		/// <summary>
		/// Adds keystrokes, text, sleep and other events to the internal collection. They will be sent/executed by <see cref="Send"/>.
		/// Returns this.
		/// </summary>
		/// <param name="keysEtc">The same as with <see cref="Key"/>.</param>
		public AKeys Add([ParamString(PSFormat.AKeys)] params KKeysEtc[] keysEtc) {
			_ThrowIfSending();
			if (keysEtc != null) {
				for (int i = 0; i < keysEtc.Length; i++) {
					var o = keysEtc[i].Value ?? "";
					switch (o) {
					case string s:
						AddKeys(s);
						break;
					case AClipboardData cd:
						AddClipboardData(cd);
						break;
					case KKey k:
						AddKey(k);
						break;
					case int ms:
						AddSleep(ms);
						break;
					case Action g:
						AddAction(g);
						break;
					case KKeyScan t:
						AddKey(t.vk, t.scanCode, t.extendedKey);
						break;
					}
				}
			}
			return this;
		}

		/// <summary>
		/// Sends keys, text and executes other events added with the <b>AddX</b> functions.
		/// </summary>
		/// <param name="canSendAgain">Don't clear the internal collection. If true, this function then can be called again (eg in loop) to send/execute the same keys etc. If false (default), clears the added keys etc; then you can call <b>AddX</b> functions and <b>Send</b> again.</param>
		/// <exception cref="ArgumentException"><i>canSendAgain</i> is true and keys end with + or (.</exception>
		/// <exception cref="AuException">Failed. For example other desktop is active (PC locked, screen saver, UAC consent, Ctrl+Alt+Delete, etc). When sending text, fails if there is no focused window.</exception>
		public void Send(bool canSendAgain = false) {
			_ThrowIfSending();
			if (_a.Count == 0) return;
			if (canSendAgain) {
				if (_pstate.paren || _pstate.plus) throw new ArgumentException("canSendAgain cannot be true if keys ends with + or (");
			}

			//AOutput.Write("-- _parsing.mod --");
			//AOutput.Write(_parsing.mod);

			_AddModUp(); //add mod-up events if need, eg Ctrl-up after "Ctrl+A"

			//AOutput.Write("-- _a --");
			//AOutput.Write(_a);

			//APerf.First();
			int sleepFinally = 0;
			bool restoreCapsLock = false;
			var bi = new AInputBlocker() { ResendBlockedKeys = true };
			try {
				_sending = true;
				//AOutput.Write("{");
				if (!Options.NoBlockInput) bi.Start(BIEvents.Keys);
				restoreCapsLock = Internal_.ReleaseModAndCapsLock(Options);
				//APerf.Next();
				for (int i = 0; i < _a.Count; i++) {
					var k = _a[i];
					switch (k.Type) {
					case _KType.Sleep:
						if (i == _a.Count - 1) sleepFinally = k.sleep;
						else Internal_.Sleep(k.sleep);
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
				//APerf.Next();
				sleepFinally += GetOptionsAndWndFocused_(out _, false).SleepFinally;
			}
			finally {
				if (restoreCapsLock) Internal_.SendKey(KKey.CapsLock);
				_sending = false;
				bi.Dispose();
				//APerf.NW();
				//AOutput.Write("}");

				//if canSendAgain, can be used like: AddX(); for(...) Send();
				//else can be used like: AddX(); Send(); AddX(); Send();
				if (!canSendAgain) {
					_a.Clear();
					_data = null;
					_sstate.Clear();
					//and don't clear _pstate
				}
			}

			if (sleepFinally > 0) Internal_.Sleep(sleepFinally);

			//_SyncWait();
			//CONSIDER: instead of SleepFinally use TimeSyncFinally, default 100 ms. Eg send a sync key and wait max TimeSyncFinally ms.
			//	Don't sync after each (or some) sent key. Usually it does not make sense. The final sync/sleep is useful if next statement is not an input function.
			//Sync problems:
			//	Tried many ways, nothing is good enough. The test code now is in the "Unused" project.
			//	The best would be non-LL keyboard hook that sets event when receives our sent special key-up. Especially when combined with 'get thread CPU usage' while waiting for the event. However these hooks don't work eg in Store apps.
			//Better add a Sync function (AKeys.Sync) or/and special key name, let users do it explicitly where need.
		}

		unsafe void _SendKey(_KEvent k, int i) {
			bool needScanCode = k.scan == 0 && !k.SIFlags.HasAny(_KFlags.Scancode | _KFlags.Unicode);
			var opt = GetOptionsAndWndFocused_(out var wFocus, needScanCode);
			if (needScanCode) {
				var hkl = Api.GetKeyboardLayout(wFocus.ThreadId); //most layouts have the same standard scancodes, but eg dvorak different
				k.scan = Internal_.VkToSc(k.vk, hkl);
			}

			bool isLast = i == _a.Count - 1;
			_SendKey2(k, isLast ? default : _a[i + 1], isLast, opt);
		}

		//Caller should set k.scan; this func doesn't.
		unsafe static void _SendKey2(_KEvent k, _KEvent kNext, bool isLast, OKey opt) {
			var ki = new Api.INPUTK(k.vk, k.scan, (uint)k.SIFlags);

			int count = 1, sleep = opt.KeySpeed;
			if (isLast) {
				if (!k.IsPair) sleep = Internal_.LimitSleepTime(sleep) - opt.SleepFinally;
			} else {
				if (kNext.IsRepeat) count = kNext.repeat;
				else if (!k.IsPair) {
					//If this is pair, sleep between down and up, and don't sleep after up.
					//Else if repeat, sleep always.
					//Else in most cases don't need to sleep. In some cases need, but can limit the time.
					//	For example, in Ctrl+C normally would not need to sleep after Ctrl down and Ctrl up.
					//	However some apps/controls then may not work. Maybe they process mod and nonmod keys somehow async.
					//	For example, Ctrl+C in IE address bar often does not work if there is no sleep after Ctrl down. Always works if 1 ms.

					sleep = Internal_.LimitSleepTime(sleep);
					if (kNext.IsKey) {
						bool thisMod = KeyTypes_.IsMod(k.vk), nextMod = KeyTypes_.IsMod(kNext.vk);
						if (!k.IsUp) {
							if (kNext.IsUp) sleep = opt.KeySpeed;
							else if (thisMod == nextMod) sleep = 0;
						} else {
							if (!thisMod || nextMod) sleep = 0;
						}
					} else if (kNext.IsSleep) sleep -= kNext.sleep;
				}
			}
			if (sleep < 0) sleep = 0;

			//var s = (k.vk).ToString();
			//if(k.IsPair) AOutput.Write($"{s}<{sleep}>");
			//else { var ud = k.IsUp ? '-' : '+'; if(sleep > 0) AOutput.Write($"{s}{ud} {sleep}"); else AOutput.Write($"{s}{ud}"); }

			for (int r = 0; r < count; r++) {
				//APerf.First();
				Api.SendInput(&ki);
				//APerf.Next();
				if (sleep > 0) {
					Internal_.Sleep(sleep);
				}
				if (k.IsPair) {
					ki.dwFlags |= Api.KEYEVENTF_KEYUP;
					Api.SendInput(&ki);
					ki.dwFlags &= ~Api.KEYEVENTF_KEYUP;

				}
				//APerf.NW();
				//speed: min 400 mcs for each event. Often > 1000. Does not depend on whether all events sent by single SendInput call.
			}
		}

		unsafe void _SendText(_KEvent ke) {
			var opt = GetOptionsAndWndFocused_(out var wFocus, true, requireFocus: true);
			object data = _GetData(ke.data); //string or AClipboardData
			string s = data as string;

			OKeyText textHow;
			if (0 != ((byte)ke.vk & 0x80)) textHow = (OKeyText)((byte)ke.vk & 0xf);
			else if (s != null && s.Length < opt.PasteLength) textHow = opt.TextHow;
			else textHow = OKeyText.Paste;

			if (textHow != OKeyText.Paste) {
				//use paste if there are Unicode surrogate pairs, because some apps/controls/frameworks don't support surrogates with WM_PACKET.
				//known apps that support: standard Edit and RichEdit controls, Chrome, Firefox, IE, WPF, WinForms, new Scintilla, Dreamweaver, LibreOffice.
				//known apps that don't: Office 2003, OpenOffice, old Scintilla.
				//known apps that don't if 0 sleep: QT edit controls in VirtualBox.
				//known apps that don't support these chars even when pasting: Java (tested the old and new frameworks).
				//tested: the same if SendInput(arrayOfAllChars).
				for (int i = 0; i < s.Length; i++) if ((s[i] & 0xf800) == 0xd800) { textHow = OKeyText.Paste; break; }
			}

			LPARAM hkl = default;
			if (textHow == OKeyText.KeysOrChar || textHow == OKeyText.KeysOrPaste) {
				hkl = Api.GetKeyboardLayout(wFocus.ThreadId);
				if (textHow == OKeyText.KeysOrPaste) {
					foreach (char c in s) {
						if (c == '\r' || c == '\n') continue;
						if (_CharToKey(c, hkl).vk == default) { textHow = OKeyText.Paste; break; }
					}
				}
			}
			//AOutput.Write(opt.TextHow, textHow);

			if (textHow == OKeyText.Paste) {
				Pasting?.Invoke(this, new PastingEventArgs { Text = s, Options = opt, WndFocus = wFocus });
				AClipboard.Paste_(data, opt, wFocus);
				return;
			}

			static (KKey vk, KMod mod) _CharToKey(char c, LPARAM hkl) {
				short km = Api.VkKeyScanEx(c, hkl); //note: call for non-ASCII char too; depending on keyboard layout it can succeed
				if (0 != (km & 0xf800)) return default; //-1 if failed, mod flag 8 Hankaku key, 16/32 reserved for driver
				return ((KKey)(km & 0xff), (KMod)(km >> 8));
			}

			KMod prevMod = 0;
			int sleep = opt.TextSpeed;

			try {
				for (int i = 0; i < s.Length; i++) {
					char c = s[i];
					bool lastChar = i == s.Length - 1;

					//replace \r, \n and \r\n with key Enter.
					//	Cannot use WM_PACKET. Eg Word ignores \n, WordPad both. Edit control adds newline for both.
					if (c == '\r') {
						if (!lastChar && s[i + 1] == '\n') continue;
						c = '\n';
					}

					KKey vk = 0; KMod mod = 0;
					if (c == '\n') {
						vk = KKey.Enter;
					} else if (c == ' ' || c == '\t') {
						vk = (KKey)c; //eg AbiWord ignores VK_PACKET. Tested: all other chars OK.
					} else if (textHow == OKeyText.KeysOrChar || textHow == OKeyText.KeysOrPaste) {
						(vk, mod) = _CharToKey(c, hkl);
						//AOutput.Write(c, vk, mod, (ushort)km);
					}

					if (vk == 0) { //use vk_packet
						if (prevMod != 0) { Internal_.ModPressRelease(false, prevMod); prevMod = 0; }

						//note: need key-up event for VK_PACKET too.
						//	Known controls that need it: Qt edit controls; Office 2003 'type question' field.
					} else if (mod != prevMod) {
						var pm = prevMod; prevMod |= mod; //to release in case of exception between here and 'prevMod = mod'
						if (0 != (mod ^ pm & KMod.Ctrl)) Internal_.SendCtrl(0 != (mod & KMod.Ctrl));
						if (0 != (mod ^ pm & KMod.Alt)) Internal_.SendAlt(0 != (mod & KMod.Alt));
						if (0 != (mod ^ pm & KMod.Shift)) Internal_.SendShift(0 != (mod & KMod.Shift));
						prevMod = mod;
						if (sleep > 0) Internal_.Sleep(Internal_.LimitSleepTime(sleep)); //need for apps that process mod-nonmod keys async
					}

					var ki = new Internal_.INPUTKEY2(vk, vk == 0 ? c : Internal_.VkToSc(vk, hkl), vk == 0 ? Api.KEYEVENTF_UNICODE : 0);
					Api.SendInput(&ki.k0, sleep > 0 ? 1 : 2);
					if (sleep > 0) {
						Internal_.Sleep(sleep);
						Api.SendInput(&ki.k1, 1);
					}
					//rejected: try to synchronize somehow. To work better with slow and badly synchronized apps.
					//1. SendTimeout(WM_NULL). Although makes slower, usually does not make sync.
					//2. Sleep if the process uses CPU eg >50% of time. Tooo slow, even with Notepad. Tried GetProcessTimes and QueryProcessCycleTime (precise).
					//Eg UWP input processing is so slow and chaotic, impossible to sync.
					//rejected: option to sleep 1 ms every n-th char (eg use float 0...1 or negative value). Nothing good.

					//using var ph = Handle_.OpenProcess(wFocus.ProcessId);
					//static int _CpuPercent(IntPtr ph) {
					//	Api.QueryProcessCycleTime(ph, out long ctime1);
					//	long t1 = ATime.PerfMicroseconds;
					//	1.ms();
					//	long time = ATime.PerfMicroseconds - t1;
					//	Api.QueryProcessCycleTime(ph, out long ctime2);
					//	long speedCyclesMS = 2600; //to get this can be used QueryThreadCycleTime(thisThread)->_Spin(1000)->QueryThreadCycleTime(thisThread)->/1000
					//	long cycles = ctime2 - ctime1;
					//	return (int)(cycles * 100 / time / speedCyclesMS);
					//}

					//static void _Spin(long mcs) {
					//	for (long t = ATime.PerfMicroseconds; ATime.PerfMicroseconds - t < mcs;) { }
					//}
				}
			}
			finally {
				Internal_.ModPressRelease(false, prevMod);
			}

			//rejected: throw if changed the focused window.
			//	Possible false positives, because everything is async.
		}

		/// <summary>
		/// Before pasting text through clipboard.
		/// </summary>
		public event EventHandler<PastingEventArgs> Pasting;
	}
}

namespace Au.Types
{
	/// <summary>
	/// <see cref="AKeys.Pasting"/> event data.
	/// </summary>
	public class PastingEventArgs : EventArgs
	{
		///
		public string Text { get; init; }
		///
		public OKey Options { get; init; }
		///
		public AWnd WndFocus { get; init; }
	}
}