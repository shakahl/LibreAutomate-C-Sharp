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
//using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Triggers
{
	/// <summary>
	/// Flags of hotkey triggers.
	/// </summary>
	[Flags]
	public enum TKFlags : byte
	{
		/// <summary>
		/// Allow other apps to receive the key down message too.
		/// Without this flag, other apps receive only modifier keys and the key up message. Also, OS always receives Ctrl+Alt+Delete and some other hotkeys.
		/// </summary>
		PassMessage = 1,

		/// <summary>
		/// Run the action when the key and modifier keys are released.
		/// </summary>
		KeyModUp = 2,

		/// <summary>
		/// The trigger works only with left-side modifier keys.
		/// </summary>
		LeftMod = 4,

		/// <summary>
		/// The trigger works only with right-side modifier keys.
		/// </summary>
		RightMod = 8,

		/// <summary>
		/// Don't release modifier keys.
		/// Without this flag, for example if trigger is ["Ctrl+K"], when the user presses Ctrl and K down, the trigger sends Ctrl key-up event, making the key logically released, although it is still physically pressed. Then modifier keys don't interfer with the action. However functions like <see cref="Keyb.GetMod"/> (and any such functions in any app) will not know that the key is physically pressed.
		/// <note type="note">Unreleased modifier keys will interfere with mouse functions like <see cref="Mouse.Click"/>. Will not interfere with keyboard and clipboard functions of this library, because they release modifier keys, unless <b>Opt.Key.NoModOff</b> is true. Will not interfere with functions that send text, unless <b>Opt.Key.NoModOff</b> is true and <b>Opt.Key.TextOption</b> is <b>KTextOption.Keys</b>.</note>.
		/// Other flags that prevent releasing modifier keys: <b>KeyUp</b>, <b>PassMessage</b>. Then don't need this flag.
		/// </summary>
		NoModOff = 16,
	}

	/// <summary>
	/// Represents a hotkey trigger.
	/// </summary>
	public class HotkeyTrigger : ActionTrigger
	{
		internal readonly TKFlags flags;
		string _paramsString;

		internal HotkeyTrigger(ActionTriggers triggers, Action<HotkeyTriggerArgs> action, TKFlags flags, string paramsString) : base(triggers, action, true)
		{
			_paramsString = flags == 0 ? paramsString : paramsString + " (" + flags.ToString() + ")"; //Print(_paramsString);
			this.flags = flags;
		}

		internal override void Run(TriggerArgs args) => RunT(args as HotkeyTriggerArgs);

		/// <inheritdoc/>
		public override string TypeString => "Hotkey";

		/// <inheritdoc/>
		public override string ParamsString => _paramsString;

		///
		public TKFlags Flags => flags;
	}

	/// <summary>
	/// Hotkey triggers.
	/// </summary>
	/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
	public class HotkeyTriggers : ITriggers
	{
		ActionTriggers _triggers;
		Dictionary<int, ActionTrigger> _d = new Dictionary<int, ActionTrigger>();

		internal HotkeyTriggers(ActionTriggers triggers)
		{
			_triggers = triggers;
		}

		static int _DictKey(KKey key, KMod mod) => ((int)mod << 8) | (int)key;

		/// <summary>
		/// Adds a hotkey trigger.
		/// </summary>
		/// <param name="hotkey">
		/// A hotkey, like with the <see cref="Key" r=""/> function.
		/// Can contain 0 to 4 modifier keys (Ctrl, Shift, Alt, Win) and 1 non-modifier key.
		/// Examples: "F11", "Ctrl+K", "Ctrl+Shift+Alt+Win+A".
		/// To ignore modifiers: "?+K". Then the trigger works with any combination of modifiers.
		/// To ignore a modifier: "Ctrl?+K". Then the trigger works with or without the modifier. More examples: "Ctrl?+Shift?+K", "Ctrl+Shift?+K".
		/// </param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Invalid hotkey string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <b>Triggers.Run</b> was called, until it returns.</exception>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public Action<HotkeyTriggerArgs> this[string hotkey, TKFlags flags = 0] {
			set {
				if(!Keyb.Misc.LibParseHotkeyTriggerString(hotkey, out var mod, out var modAny, out var key, false)) throw new ArgumentException("Invalid hotkey string.");
				_Add(value, key, mod, modAny, flags, hotkey);
			}
		}

		/// <summary>
		/// Adds a hotkey trigger.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="modKeys">
		/// Modifier keys, like with the <see cref="Key" r=""/> function.
		/// Examples: "Ctrl", "Ctrl+Shift+Alt+Win".
		/// To ignore modifiers: "?". Then the trigger works with any combination of modifiers.
		/// To ignore a modifier: "Ctrl?". Then the trigger works with or without the modifier. More examples: "Ctrl?+Shift?", "Ctrl+Shift?".
		/// </param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <b>Triggers.Run</b> was called, until it returns.</exception>
		public Action<HotkeyTriggerArgs> this[KKey key, string modKeys, TKFlags flags = 0] {
			set {
				var ps = key.ToString(); if(Char_.IsAsciiDigit(ps[0])) ps = "vk" + ps;
				if(!Empty(modKeys)) ps = modKeys + "+" + ps;

				if(!Keyb.Misc.LibParseHotkeyTriggerString(modKeys, out var mod, out var modAny, out _, true)) throw new ArgumentException("Invalid modKeys string.");
				_Add(value, key, mod, modAny, flags, ps);
			}
		}

		void _Add(Action<HotkeyTriggerArgs> action, KKey key, KMod mod, KMod modAny, TKFlags flags, string paramsString)
		{
			if(mod == 0 && flags.HasAny_((TKFlags.LeftMod | TKFlags.RightMod))) throw new ArgumentException("Invalid flags.");
			_triggers.LibThrowIfRunning();
			//actually could safely add triggers while running.
			//	Currently would need just lock(_d) in several places. Also some triggers of this type must be added before starting, else we would not have the hook etc.
			//	But probably not so useful. Makes programming more difficult. If need, can Stop, add triggers, then Run again.

			//Print($"key={key}, mod={mod}, modAny={modAny}");
			var t = new HotkeyTrigger(_triggers, action, flags, paramsString);
			int b = TrigUtil.ModBitArray(mod, modAny);
			for(int i = 0; i < 16; i++) if(0 != (b & (1 << i))) t.DictAdd(_d, _DictKey(key, (KMod)i));
			_lastAdded = t;
		}

		/// <summary>
		/// The last added trigger.
		/// </summary>
		public HotkeyTrigger Last => _lastAdded;
		HotkeyTrigger _lastAdded;

		bool ITriggers.HasTriggers => _lastAdded != null;

		void ITriggers.StartStop(bool start)
		{
			_mod = _modL = _modR = 0;
			_lastKeyTime = 0;
			_upTrigger = null;
			_upArgs = null;
			_upKey = 0;
		}

		internal bool HookProc(HookData.Keyboard k, TriggerHookContext thc)
		{
			//Print(k.vkCode, !k.IsUp);
			Debug.Assert(!k.IsInjectedByAu); //server must ignore

			long time = Time.WinMilliseconds;

			bool up = k.IsUp;
			if(!up) _upTrigger = null;

			KMod modL = 0, modR = 0;
			switch(k.vkCode) {
			case KKey.LCtrl: modL = KMod.Ctrl; break;
			case KKey.LShift: modL = KMod.Shift; break;
			case KKey.LAlt: modL = KMod.Alt; break;
			case KKey.Win: modL = KMod.Win; break;
			case KKey.RCtrl: modR = KMod.Ctrl; break;
			case KKey.RShift: modR = KMod.Shift; break;
			case KKey.RAlt: modR = KMod.Alt; break;
			case KKey.RWin: modR = KMod.Win; break;
			}

			if((modL | modR) != 0) {
				if(up) {
					_modL &= ~modL; _modR &= ~modR;
				} else {
					_modL |= modL; _modR |= modR;
				}
				_mod = _modL | _modR;

				if(_upTrigger != null && _mod == 0 && _upKey == 0) _UpTriggered(thc);
			} else if(up) {
				if(_upTrigger != null && k.vkCode == _upKey) {
					if(Keyb.IsMod()) _upKey = 0;
					else _UpTriggered(thc);
				}
			} else {
				//We cannot trust _mod, because hooks are unreliable. We may not receive some events because of hook timeout, other hooks, OS quirks, etc. Also triggers may start while a modifier key is pressed.
				//And we cannot use Keyb.IsPressed, because our triggers release modifiers. Also Key() etc. Then triggers could not be auto-repeated.
				//We use both. If IsPressed(mod), add mod to _mod. Else remove from _mod after >5 s since the last seen key event. The max auto-repeat delay that you can set in CP is ~1 s.
				TrigUtil.GetModLR(out modL, out modR);
				//Debug_.PrintIf(modL != _modL || modR != _modR, $"KEY={k.vkCode}    modL={modL}  _modL={_modL}    modR={modR}  _modR={_modR}"); //normally should be only when auto-repeating a trigger
				_modL |= modL; _modR |= modR;
				if(time - _lastKeyTime > 5000) {
					_modL &= modL; _modR &= modR;
				}
				_mod = _modL | _modR;
				//Print(_mod, k.vkCode);
				_lastKeyTime = time;

				KKey key = k.vkCode;
				if(_d.TryGetValue(_DictKey(key, _mod), out var v)) {
					HotkeyTriggerArgs args = null;
					for(; v != null; v = v.next) {
						if(v.DisabledThisOrAll) continue;
						var x = v as HotkeyTrigger;

						switch(x.flags & (TKFlags.LeftMod | TKFlags.RightMod)) {
						case TKFlags.LeftMod: if(_modL != _mod) continue; break;
						case TKFlags.RightMod: if(_modR != _mod) continue; break;
						}

						if(args == null) thc.args = args = new HotkeyTriggerArgs(x, thc.Window, key, _mod); //may need for scope callbacks too
						else args.Trigger = x;
						if(!x.MatchScopeWindowAndFunc(thc)) continue;

						if(0 != (x.flags & TKFlags.KeyModUp)) {
							_upTrigger = x;
							_upArgs = args;
							_upKey = key;
						} else {
							thc.trigger = x;
						}

						//Print(key, _mod);
						return 0 == (x.flags & TKFlags.PassMessage);
					}
				}
			}
			return false;
		}

		KMod _mod, _modL, _modR;
		long _lastKeyTime;
		HotkeyTrigger _upTrigger;
		HotkeyTriggerArgs _upArgs;
		KKey _upKey;

		void _UpTriggered(TriggerHookContext thc)
		{
			thc.args = _upArgs;
			thc.trigger = _upTrigger;
			_upTrigger = null;
			_upArgs = null;
			_upKey = 0;
		}
	}

	/// <summary>
	/// Arguments for actions of hotkey triggers.
	/// </summary>
	public class HotkeyTriggerArgs : TriggerArgs
	{
		///
		public HotkeyTrigger Trigger { get; internal set; }

		/// <summary>
		/// The active window.
		/// </summary>
		public Wnd Window { get; }

		/// <summary>
		/// The pressed key.
		/// </summary>
		public KKey Key { get; }

		/// <summary>
		/// The pressed modifier keys.
		/// </summary>
		/// <remarks>
		/// Can be useful when the trigger ignores modifiers. For example "?+F11" or "Shift?+A".
		/// </remarks>
		public KMod Mod { get; }

		///
		public HotkeyTriggerArgs(HotkeyTrigger trigger, Wnd w, KKey key, KMod mod)
		{
			Trigger = trigger;
			Window = w; Key = key; Mod = mod;
		}
	}
}
