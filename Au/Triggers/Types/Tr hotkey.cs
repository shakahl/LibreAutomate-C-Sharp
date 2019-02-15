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
using Microsoft.Win32.SafeHandles;

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	public class HotkeyTriggers : ITriggers //in task process
	{
		[Flags]
		public enum TFlags : byte
		{
			/// <summary>
			/// Allow other apps to receive the key too, as if it is not a hotkey.
			/// Without this flag, other apps receive only modifier keys.
			/// </summary>
			Shared = 1,

			/// <summary>
			/// Run the action when all hotkey keys (key and modifiers) are released.
			/// Without this flag, the action runs when the non-modifier key pressed down.
			/// </summary>
			Up = 2,

			/// <summary>
			/// Run the action only when using left-side modifier keys.
			/// </summary>
			ModLeft = 4,

			/// <summary>
			/// Run the action only when using right-side modifier keys.
			/// </summary>
			ModRight = 8,

			/// <summary>
			/// Don't release modifier keys.
			/// Without this flag, for example if trigger is ["Ctrl+K"], when the user presses Ctrl and K down, the program sends Ctrl key-up event, making the key logically released, although it is still physically pressed. It prevents the modifier keys interfering with the action. However functions like <see cref="Keyb.GetMod"/> (and any such functions in any app) will not know that the key is physically pressed.
			/// </summary>
			DontReleaseMod = 16,

			/// <summary>
			/// The trigger is temporarily disabled.
			/// </summary>
			Disabled = 128,
		}

		class _TriggerEtc : TriggerBase
		{
			public readonly TFlags flags;
			string _shortString;

			public _TriggerEtc(Triggers triggers, Action<HotkeyTriggerArgs> action, string hotkey, TFlags flags) : base(triggers, action, true)
			{
				_shortString = hotkey;
				this.flags = flags;
			}

			public override void Run(TriggerArgs args) => RunT(args as HotkeyTriggerArgs);

			public override string TypeString() => "Hotkey";

			public override string ShortString() => _shortString;

			//public override string ToString()
			//{
			//	return "Hotkey " + _shortString;
			//	//using(new Util.LibStringBuilder(out var b)) {
			//	//	b.Append("Hotkey ").Append(_hotkey);

			//	//	return b.ToString();
			//	//}
			//}
		}

		Triggers _triggers;
		Dictionary<int, TriggerBase> _d = new Dictionary<int, TriggerBase>();

		internal HotkeyTriggers(Triggers triggers)
		{
			_triggers = triggers;
		}

		static int _DictKey(KKey key, KMod mod) => ((int)mod << 8) | (int)key;

		public Action<HotkeyTriggerArgs> this[string hotkey, TFlags flags = 0] {
			set {
				if(!Keyb.Misc.LibParseHotkeyTriggerString(hotkey, out var mod, out var modAny, out var key, false)) throw new ArgumentException("Invalid hotkey string.");
				if(key == KKey.Delete && mod == (KMod.Ctrl | KMod.Alt) && !flags.Has_(TFlags.Shared)) throw new ArgumentException("With Ctrl+Alt+Delete need flag Shared.");
				//Print($"key={key}, mod={mod}, modAny={modAny}");
				var t = new _TriggerEtc(_triggers, value, hotkey, flags);
				int b = LibModBitArray(mod, modAny);
				for(int i = 0; i < 16; i++) if(0 != (b & (1 << i))) t.DictAdd(_d, _DictKey(key, (KMod)i));
			}
		}

		//To specify 'any modifier(s)' in hotkey and mouse triggers, can be used ?, like "Shift?+K" (any Shift) or "Ctrl+Shift?+K" (Ctrl+any Shift) or "?+K" (any mods).
		//To implement it simply, we add all modifier combinations to the dictionary. For example, for "Shift?+K" we add "K" and "Shift+K".
		//This function returns a 16 bit array containing 1 bits for modifier combinations to add: 0, Shift, Ctrl, Ctrl+Shift, Alt, ....
		internal static int LibModBitArray(KMod mod, KMod modAny)
		{
			int b = 0b1111111111111111;
			if(0 != (mod & KMod.Shift)) b &= 0b1010101010101010; else if(0 == (modAny & KMod.Shift)) b &= 0b0101010101010101; //if must be Shift, erase all without Shift; else if cannot be Shift, erase all with Shift
			if(0 != (mod & KMod.Ctrl)) b &= 0b1100110011001100; else if(0 == (modAny & KMod.Ctrl)) b &= 0b0011001100110011;
			if(0 != (mod & KMod.Alt)) b &= 0b1111000011110000; else if(0 == (modAny & KMod.Alt)) b &= 0b0000111100001111;
			if(0 != (mod & KMod.Win)) b &= 0b1111111100000000; else if(0 == (modAny & KMod.Win)) b &= 0b0000000011111111;
			//for(int i = 0; i < 16; i++) if(0 != (b & (1 << i))) Print((KMod)i);
			return b;
		}

		bool ITriggers.HasTriggers => _d.Count > 0;

		void ITriggers.StartStop(bool start)
		{

		}

		internal bool HookProc(HookData.Keyboard k, TriggerHookContext thc)
		{
			//Print(k.Key);
			if(!k.IsInjectedByAu && !k.IsUp && 0 == k.Mod) {
				Perf.Next();
				KMod mod = Keyb.GetMod(); //usually ~10 mcs, but sometimes 200-500 mcs
				Perf.Next();
				//KMod mod = 0;//TODO
				if(_d.TryGetValue(_DictKey(k.Key, mod), out var v)) {
					HotkeyTriggerArgs args = null;
					for(; v != null; v = v.next) {
						var x = v as _TriggerEtc;
						if(0!=(x.flags&TFlags.Disabled)) continue;
						if(args == null) thc.args = args = new HotkeyTriggerArgs(thc.w, k.Key, mod); //may need for scope callbacks too
						args.Flags = x.flags;
						if(!x.MatchScope(thc)) continue;
						thc.trigger = v;
						//Print(k.Key, mod);
						return 0 == (x.flags & TFlags.Shared);
					}
				}
			}
			return false;
		}
	}

	public class HotkeyTriggerArgs : TriggerArgs
	{
		public Wnd Window { get; }
		public KKey Key { get; }
		public KMod Mod { get; }
		public HotkeyTriggers.TFlags Flags { get; internal set; }
		internal HotkeyTriggerArgs(Wnd w, KKey key, KMod mod)
		{
			Window = w; Key = key; Mod = mod;
		}
	}
}
