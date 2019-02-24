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

#pragma warning disable CS1591 // Missing XML comment //TODO

namespace Au.Triggers
{
	public class MouseTriggers : ITriggers
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

			//TODO: this enum is copied from HotkeyTriggers without changed. Review etc.
		}

		enum ESubtype : byte { Click, Wheel, Edge, Move }

		class _TriggerEtc : Trigger
		{
			internal readonly TFlags flags;
			string _shortString;

			internal _TriggerEtc(Triggers triggers, Action<MouseTriggerArgs> action, TFlags flags) : base(triggers, action, true)
			{
				this.flags = flags;
			}

			internal override void Run(TriggerArgs args) => RunT(args as MouseTriggerArgs);

			internal override string TypeString() => "Mouse";

			internal override string ShortString() => _shortString; //TODO
		}

		Triggers _triggers;
		Dictionary<int, Trigger> _d = new Dictionary<int, Trigger>();

		internal MouseTriggers(Triggers triggers)
		{
			_triggers = triggers;
		}

		static int _DictKey(ESubtype subtype, KMod mod, byte data1, byte data2 = 0) => (data2 << 24) | (data1 << 16) | ((byte)mod << 8) | (byte)subtype;

		public Action<MouseTriggerArgs> this[TMClick button, string modKeys = null, TFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Click, modKeys, flags, (byte)button);
			}
		}

		public Action<MouseTriggerArgs> this[TMWheel direction, string modKeys = null, TFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Wheel, modKeys, flags, (byte)direction);
			}
		}

		public Action<MouseTriggerArgs> this[TMEdge edge, TMPart part, string modKeys = null, TFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Edge, modKeys, flags, (byte)edge, (byte)part);
			}
		}

		public Action<MouseTriggerArgs> this[TMMove move, TMPart part, string modKeys = null, TFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Move, modKeys, flags, (byte)move, (byte)part);
			}
		}

		_TriggerEtc _Add(Action<MouseTriggerArgs> f, ESubtype subtype, string modKeys, TFlags flags, byte data1, byte data2 = 0)
		{
			var t = new _TriggerEtc(_triggers, f, flags);
			if(Empty(modKeys)) {
				t.DictAdd(_d, _DictKey(subtype, 0, data1, data2));
			} else {
				if(!Keyb.Misc.LibParseHotkeyTriggerString(modKeys, out var mod, out var modAny, out _, true)) throw new ArgumentException("Invalid modKeys string.");
				var b = HotkeyTriggers.LibModBitArray(mod, modAny);
				for(int i = 0; i < 16; i++) if(0 != (b & (1 << i))) t.DictAdd(_d, _DictKey(subtype, (KMod)i, data1, data2));
			}
			return t;
		}

		bool ITriggers.HasTriggers => _d.Count > 0;

		void ITriggers.StartStop(bool start)
		{

		}

		internal bool HookProc(HookData.Mouse k, TriggerHookContext thc)
		{
			//Print(k.Event, k.pt);
			if(!k.IsInjectedByAu) {
				ESubtype subtype; byte data1 = 0; TMPart part = default;
				if(k.IsButton) {
					if(k.IsButtonUp) return false;
					subtype = ESubtype.Click;
					TMClick b;
					switch(k.Event) {
					case HookData.MouseEvent.LeftButton: b = TMClick.Left; break;
					case HookData.MouseEvent.RightButton: b = TMClick.Right; break;
					case HookData.MouseEvent.MiddleButton: b = TMClick.Middle; break;
					case HookData.MouseEvent.X1Button: b = TMClick.X1; break;
					default: b = TMClick.X2; break;
					}
					data1 = (byte)b;
				} else if(k.IsWheel) {
					subtype = ESubtype.Wheel;
					TMWheel b;
					switch(k.Event) {
					case HookData.MouseEvent.WheelForward: b = TMWheel.Forward; break;
					case HookData.MouseEvent.WheelBackward: b = TMWheel.Backward; break;
					case HookData.MouseEvent.WheelLeft: b = TMWheel.Left; break;
					default: b = TMWheel.Right; break;
					}
					data1 = (byte)b;
				} else {
					subtype = ESubtype.Move;
					return false;
				}

				var mod = Keyb.GetMod();
				if(_d.TryGetValue(_DictKey(subtype, mod, data1, (byte)part), out var v)) {
					if(subtype == ESubtype.Click || subtype == ESubtype.Wheel) thc.UseWndFromPoint(k.pt);
					MouseTriggerArgs args = null;
					for(; v != null; v = v.next) {
						if(v.DisabledThisOrAll) continue;
						var x = v as _TriggerEtc;
						if(args == null) thc.args = args = new MouseTriggerArgs(v, thc.w, mod); //may need for scope callbacks too
						args.Flags = x.flags;
						if(!x.MatchScope(thc)) continue;
						thc.trigger = v;
						Print(k.Event, k.pt, mod);
						return 0 == (x.flags & TFlags.Shared);
					}
				}
			}

			return false;
		}
	}

	public class MouseTriggerArgs : TriggerArgs
	{
		public Wnd Window { get; }
		public KMod Mod { get; }
		public MouseTriggers.TFlags Flags { get; internal set; }

		internal MouseTriggerArgs(Trigger trigger, Wnd w, KMod mod) : base(trigger)
		{
			Window = w; Mod = mod;
		}
	}

	public enum TMClick : byte { Left, Right, Middle, X1, X2 }

	public enum TMWheel : byte { Forward, Backward, Left, Right }

	public enum TMEdge : byte { Top, Right, Bottom, Left }

	public enum TMMove : byte { RightLeft = 1, LeftRight, UpDown, DownUp }

	public enum TMPart : byte { Any, Center, Top, Bottom, Left = Top, Right = Bottom }
}
