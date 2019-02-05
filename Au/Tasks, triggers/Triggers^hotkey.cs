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
			/// Run the action even if other (unspecified in trigger) modifier keys are pressed.
			/// For example, if trigger is ["Ins"], the action will run when the Ins key is pressed alone or with any combination of modifiers. If trigger is ["Ctrl+Ins"], the action will run on Ctrl+Ins alone or with any combination of Shift, Alt, Win.
			/// </summary>
			IgnoreOtherMod = 16,

			/// <summary>
			/// Don't release modifier keys.
			/// Without this flag, for example if trigger is ["Ctrl+K"], when the user presses Ctrl and K down, the program sends Ctrl key-up event, making the key logically released, although it is still physically pressed. It prevents the modifier keys interfering with the action. However functions like <see cref="Keyb.GetMod"/> (and any such functions in any app) will not know that the key is physically pressed.
			/// </summary>
			DontReleaseMod = 32,

			/// <summary>
			/// The trigger is temporarily disabled.
			/// </summary>
			Disabled = 128,
		}

		internal struct Trigger
		{
			public KKey key;
			public KMod mod;
			public TFlags flags;
			//byte _unused; //The above 3 are 1-byte enums.
		}

		class _TriggerEtc : TriggerBase
		{
			public readonly Trigger trigger;
			public readonly int scope; //0 or Triggers.Of.Current

			public _TriggerEtc(Trigger trigger, Action<HotkeyTriggerArgs> action) : base(action)
			{
				this.trigger = trigger;
				this.scope = Triggers.Of.Current;
			}

			public override void Run(int data1, string data2, Wnd w)
			{
				var a = action as Action<HotkeyTriggerArgs>;
				a(new HotkeyTriggerArgs(w, trigger.key, (KMod)data1, trigger.flags));
			}
		}

		List<_TriggerEtc> _a = new List<_TriggerEtc>();

		internal HotkeyTriggers() { }

		public Action<HotkeyTriggerArgs> this[string hotkey, TFlags flags = 0] {
			set {
				if(!Keyb.Misc.ParseHotkeyString(hotkey, out var mod, out var key)) throw new ArgumentException("Invalid hotkey string.");
				if(key == KKey.Delete && mod == (KMod.Ctrl | KMod.Alt) && !flags.Has_(TFlags.Shared)) throw new ArgumentException("With Ctrl+Alt+Delete need flag Shared.");
				var t = new Trigger { key = key, mod = mod, flags = flags };
				_a.Add(new _TriggerEtc(t, value));
			}
		}

		bool ITriggers.HasTriggers => _a.Count > 0;
		bool ITriggers.UsesServer => true;

		unsafe void ITriggers.Write(BinaryWriter w)
		{
			w.Write(_a.Count);
			foreach(var v in _a) { var t = v.trigger; w.Write(*(int*)&t); } //key, mod, flags; not window, action
		}

		bool ITriggers.CanRun(int action, int data1, string data2, Wnd w, WFCache cache)
		{
			//Print(action);
			var v = _a[action];
			if(v.trigger.flags.Has_(TFlags.Disabled)) return false;
			if(!Triggers.Of.Match(v.scope, w, cache)) return false;
			return true;
		}

		TriggerBase ITriggers.GetAction(int action) => _a[action];
	}

	public class HotkeyTriggerArgs : TriggerArgs
	{
		public Wnd Window { get; set; }
		public KKey Key { get; set; }
		public KMod Mod { get; set; }
		public HotkeyTriggers.TFlags Flags { get; set; }
		public HotkeyTriggerArgs(Wnd w, KKey key, KMod mod, HotkeyTriggers.TFlags flags)
		{
			Window = w; Key = key; Mod = mod; Flags = flags;
		}
	}

	class HotkeyTriggersServer : ITriggersServer
	{
		class _TriggerValue
		{
			public _TriggerValue next;
			public int pipeIndex, action;
			public HotkeyTriggers.TFlags flags;

			public _TriggerValue(int pipeIndex, int action, HotkeyTriggers.TFlags flags)
			{
				this.pipeIndex = pipeIndex; this.action = action; this.flags = flags;
			}
		}

		Dictionary<int, _TriggerValue> _d;

		public void Dispose()
		{
			KeyboardHook.Instance?.Dispose();
			_d?.Clear();
		}

		unsafe void ITriggersServer.AddTriggers(int pipeIndex, BinaryReader r, byte[] raw)
		{
			if(_d == null) _d = new Dictionary<int, _TriggerValue>();
			int n = r.ReadInt32(); //how many triggers
			for(int i = 0; i < n; i++) {
				HotkeyTriggers.Trigger t = default;
				*(int*)&t = r.ReadInt32();
				//Print(t.key, t.mod);
				var k = Math_.MakeUshort((int)t.key, (int)t.mod);
				var v = new _TriggerValue(pipeIndex, i, t.flags);
				if(!_d.TryGetValue(k, out var o)) _d.Add(k, v);
				else { //append to the linked list
					while(o.next != null) o = o.next;
					o.next = v;
				}
			}

			if(KeyboardHook.Instance == null) KeyboardHook.Instance = new KeyboardHook();

			//TODO
			if(MouseHook.Instance == null) {
				MouseHook.Instance = new MouseHook();
			}
		}

		void ITriggersServer.RemoveTriggers(int pipeIndex)
		{
			//_Print("BEFORE");
			var ar = new List<KeyValuePair<int, _TriggerValue>>();
			foreach(var kv in _d) {
				_TriggerValue v = kv.Value, vFirstOther = null, vLastOther = null;
				for(; v != null; v = v.next) {
					if(v.pipeIndex == pipeIndex) {
						if(vLastOther != null) vLastOther.next = v.next;
					} else {
						vLastOther = v;
						if(vFirstOther == null) vFirstOther = v;
					}
				}
				if(vFirstOther != kv.Value) ar.Add(new KeyValuePair<int, _TriggerValue>(kv.Key, vFirstOther));
			}
			foreach(var kv in ar) if(kv.Value == null) _d.Remove(kv.Key); else _d[kv.Key] = kv.Value;
			//_Print("AFTER");

			//void _Print(string name)
			//{
			//	Print(name);
			//	foreach(var kv in _d) {
			//		Print($"<><c green>{(KKey)(byte)kv.Key}, {(KMod)(byte)(kv.Key >> 8)}<>");
			//		for(var u = kv.Value; u != null; u = u.next) Print($"pipe={u.pipeIndex}, action={u.action}");
			//	}
			//}
		}

		internal bool HookProc(HookData.Keyboard k)
		{
			//TODO
			if(Keyb.IsScrollLock) {
				var ti = TriggersServer.Instance;
				ti.SendHook(-100);
			}

			//if(!k.IsUp && k.Mod == 0) {
			//	var mod = Keyb.GetMod();
			//	var km = Math_.MakeUshort((int)k.vkCode, (int)mod);
			//	//Print(k.vkCode, mod, km);
			//	if(_d.TryGetValue(km, out var o)) {
			//		//Print("trigger");
			//		var ti = TriggersServer.Instance;
			//		ti.SendBegin();
			//		//Perf.First();
			//		for(; o != null; o = o.next) {
			//			//TODO: apply flags

			//			ti.SendAdd(o.pipeIndex, o.action);
			//		}
			//		//Perf.NW();
			//		return ti.Send(ETriggerType.Hotkey, (int)mod);
			//	}
			//}
			return false;
		}
	}

	//Used by HotkeyTriggersEngine and AutotextTriggersEngine.
	class KeyboardHook
	{
		Util.WinHook _hook;
		public static KeyboardHook Instance;

		public KeyboardHook()
		{
			//Output.LibWriteQM2("ctor");
			_hook = Util.WinHook.Keyboard(_Hook);
		}

		public void Dispose()
		{
			//Output.LibWriteQM2("disp");
			_hook.Dispose(); _hook = null;
			Instance = null;
		}

		bool _Hook(HookData.Keyboard k)
		{
			var ti = TriggersServer.Instance;
			var hotkey = ti[ETriggerType.Hotkey] as HotkeyTriggersServer;
			if(hotkey?.HookProc(k) ?? false) return true;
			var autotext = ti[ETriggerType.Autotext] as AutotextTriggersServer;
			if(autotext?.HookProc(k) ?? false) return true;
			return false;
		}
	}
}
