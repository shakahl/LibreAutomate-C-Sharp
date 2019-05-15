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
using System.Windows.Forms;
//using System.Drawing;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;
using System.Collections;

namespace Au.Triggers
{
	/// <summary>
	/// Flags of mouse triggers.
	/// </summary>
	[Flags]
	public enum TMFlags : byte
	{
		/// <summary>
		/// Allow other apps to receive the mouse button or wheel message too.
		/// Used only with the click and wheel triggers.
		/// To receive and block mouse messages is used a low-level hook. Other hooks may receive blocked messages or not, depending on when they were set. 
		/// </summary>
		ShareEvent = 1,

		/// <summary>
		/// Run the action when the mouse button and modifier keys are released.
		/// </summary>
		ButtonModUp = 2,

		/// <summary>
		/// The trigger works only with left-side modifier keys.
		/// </summary>
		LeftMod = 4,

		/// <summary>
		/// The trigger works only with right-side modifier keys.
		/// </summary>
		RightMod = 8,

		//rejected. We always mod-off and eat auto-repeated and up events with a temp hook. Because OS does not disable auto-repeating like for hotkeys.
		///// <summary>
		///// Don't release modifier keys.
		///// More info: <see cref="TKFlags.NoModOff"/>.
		///// </summary>
		//NoModOff = 16,
	}

	/// <summary>
	/// Represents a mouse trigger.
	/// </summary>
	public class MouseTrigger : ActionTrigger
	{
		internal readonly KMod modMasked, modMask;
		internal readonly TMFlags flags;
		internal readonly TMScreen screenIndex;
		string _paramsString;

		internal MouseTrigger(ActionTriggers triggers, Action<MouseTriggerArgs> action, KMod mod, KMod modAny, TMFlags flags, TMScreen screen, string paramsString) : base(triggers, action, true)
		{
			const KMod csaw = KMod.Ctrl | KMod.Shift | KMod.Alt | KMod.Win;
			modMask = ~modAny & csaw;
			modMasked = mod & modMask;
			this.flags = flags;
			this.screenIndex = screen;
			_paramsString = paramsString;
		}

		internal override void Run(TriggerArgs args) => RunT(args as MouseTriggerArgs);

		/// <inheritdoc/>
		public override string TypeString => "Mouse";

		/// <inheritdoc/>
		public override string ParamsString => _paramsString;

		///
		public TMFlags Flags => flags;
	}

	/// <summary>
	/// Mouse triggers.
	/// </summary>
	/// <example> See <see cref="ActionTriggers"/>.</example>
	public class MouseTriggers : ITriggers, IEnumerable<MouseTrigger>
	{
		enum ESubtype : byte { Click, Wheel, Edge, Move }

		ActionTriggers _triggers;
		Dictionary<int, ActionTrigger> _d = new Dictionary<int, ActionTrigger>();

		internal MouseTriggers(ActionTriggers triggers)
		{
			_triggers = triggers;
		}

		/// <summary>
		/// Adds a mouse click trigger.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="modKeys">
		/// Modifier keys, like with the <see cref="Keyb.Key"/> function.
		/// Examples: "Ctrl", "Ctrl+Shift+Alt+Win".
		/// To ignore modifiers: "?". Then the trigger works with any combination of modifiers.
		/// To ignore a modifier: "Ctrl?". Then the trigger works with or without the modifier. More examples: "Ctrl?+Shift?", "Ctrl+Shift?".
		/// </param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <c>Triggers.Run</c> was called, until it returns.</exception>
		/// <example> See <see cref="ActionTriggers"/>.</example>
		public Action<MouseTriggerArgs> this[TMClick button, string modKeys = null, TMFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Click, modKeys, flags, (byte)button, 0, button.ToString());
			}
		}

		/// <summary>
		/// Adds a mouse wheel trigger.
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="modKeys">See <see cref="this[TMClick, string, TMFlags]"/>.</param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <c>Triggers.Run</c> was called, until it returns.</exception>
		/// <example> See <see cref="ActionTriggers"/>.</example>
		public Action<MouseTriggerArgs> this[TMWheel direction, string modKeys = null, TMFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Wheel, modKeys, flags, (byte)direction, 0, direction.ToString());
			}
		}

		/// <summary>
		/// Adds a mouse screen edge trigger.
		/// </summary>
		/// <param name="edge"></param>
		/// <param name="modKeys">See <see cref="this[TMClick, string, TMFlags]"/>.</param>
		/// <param name="flags"></param>
		/// <param name="screen">
		/// Let the trigger work only in this screen (display monitor). Also you can specify <b>All</b>.
		/// Default: <b>Primary</b>.
		/// Uses <see cref="Screen.AllScreens"/> to get screen indices. They may not match the indices that you can see in Windows Settings.
		/// </param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <c>Triggers.Run</c> was called, until it returns.</exception>
		/// <example> See <see cref="ActionTriggers"/>.</example>
		public Action<MouseTriggerArgs> this[TMEdge edge, string modKeys = null, TMFlags flags = 0, TMScreen screen = 0] {
			set {
				var t = _Add(value, ESubtype.Edge, modKeys, flags, (byte)edge, screen, edge.ToString());
			}
		}

		/// <summary>
		/// Adds a mouse move trigger.
		/// </summary>
		/// <param name="move"></param>
		/// <param name="modKeys">See <see cref="this[TMClick, string, TMFlags]"/>.</param>
		/// <param name="flags"></param>
		/// <param name="screen">See <see cref="this[TMEdge, string, TMFlags, TMScreen]"/>.</param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <c>Triggers.Run</c> was called, until it returns.</exception>
		/// <example> See <see cref="ActionTriggers"/>.</example>
		public Action<MouseTriggerArgs> this[TMMove move, string modKeys = null, TMFlags flags = 0, TMScreen screen = 0] {
			set {
				var t = _Add(value, ESubtype.Move, modKeys, flags, (byte)move, screen, move.ToString());
			}
		}

		MouseTrigger _Add(Action<MouseTriggerArgs> f, ESubtype subtype, string modKeys, TMFlags flags, byte data, TMScreen screen, string sData)
		{
			_triggers.LibThrowIfRunning();
			bool noMod = Empty(modKeys);

			string ps;
			using(new Util.LibStringBuilder(out var b)) {
				b.Append(subtype.ToString()).Append(' ').Append(sData);
				b.Append(" + ").Append(noMod ? "none" : (modKeys == "?" ? "any" : modKeys));
				if(flags != 0) b.Append(" (").Append(flags.ToString()).Append(')');
				if(subtype == ESubtype.Edge || subtype == ESubtype.Move) {
					if(screen == 0) b.Append(", primary screen");
					else if(screen > 0) b.Append(", non-primary screen ").Append((int)screen);
					else if(screen == TMScreen.Any) b.Append(", any screen");
					else if(screen == TMScreen.OfActiveWindow) b.Append(", screen of the active window");
					else throw new ArgumentException();
				}
				ps = b.ToString(); //Print(ps);
			}

			KMod mod = 0, modAny = 0;
			if(noMod) {
				if(flags.HasAny(subtype == ESubtype.Click ? TMFlags.LeftMod | TMFlags.RightMod : TMFlags.LeftMod | TMFlags.RightMod | TMFlags.ButtonModUp)) throw new ArgumentException("Invalid flags.");
			} else {
				if(!Keyb.More.LibParseHotkeyTriggerString(modKeys, out mod, out modAny, out _, true)) throw new ArgumentException("Invalid modKeys string.");
			}
			var t = new MouseTrigger(_triggers, f, mod, modAny, flags, screen, ps);
			t.DictAdd(_d, _DictKey(subtype, data));
			_lastAdded = t;
			LibUsedHookEvents |= HooksServer.UsedEvents.Mouse; //just sets the hook
			switch(subtype) {
			case ESubtype.Click: LibUsedHookEvents |= HooksServer.UsedEvents.MouseClick; break;
			case ESubtype.Wheel: LibUsedHookEvents |= HooksServer.UsedEvents.MouseWheel; break;
			default: LibUsedHookEvents |= HooksServer.UsedEvents.MouseEdgeMove; break;
			}
			return t;
		}

		static int _DictKey(ESubtype subtype, byte data) => (data << 8) | (byte)subtype;

		/// <summary>
		/// The last added trigger.
		/// </summary>
		public MouseTrigger Last => _lastAdded;
		MouseTrigger _lastAdded;

		bool ITriggers.HasTriggers => _lastAdded != null;

		internal HooksServer.UsedEvents LibUsedHookEvents { get; private set; }

		void ITriggers.StartStop(bool start)
		{
			if(start) {
			} else {
				_ResetUpAndUnhookTempKeybHook();
				_eatUp = 0;
			}
		}

		internal bool HookProcClickWheel(HookData.Mouse k, TriggerHookContext thc)
		{
			//Print(k.Event, k.pt);
			Debug.Assert(!k.IsInjectedByAu); //server must ignore

			ESubtype subtype;
			byte data;

			if(k.IsButton) {
				if(k.IsButtonUp) {
					if(k.Event == _upEvent) {
						_upEvent = 0;
						if(_upMod == 0 && _upTrigger != null) {
							thc.args = _upArgs;
							thc.trigger = _upTrigger;
							_ResetUp();
						}
					}
					if(k.Event == _eatUp) {
						_eatUp = 0;
						return true;
						//To be safer, could return false if Mouse.IsPressed(k.Button), but then can interfere with the trigger action.
					}
					return false;
					//CONSIDER: _upTimeout.
				}
				if(k.Event == _eatUp) _eatUp = 0;

				subtype = ESubtype.Click;
				TMClick b;
				switch(k.Event) {
				case HookData.MouseEvent.LeftButton: b = TMClick.Left; break;
				case HookData.MouseEvent.RightButton: b = TMClick.Right; break;
				case HookData.MouseEvent.MiddleButton: b = TMClick.Middle; break;
				case HookData.MouseEvent.X1Button: b = TMClick.X1; break;
				default: b = TMClick.X2; break;
				}
				data = (byte)b;
			} else { //wheel
				subtype = ESubtype.Wheel;
				TMWheel b;
				switch(k.Event) {
				case HookData.MouseEvent.WheelForward: b = TMWheel.Forward; break;
				case HookData.MouseEvent.WheelBackward: b = TMWheel.Backward; break;
				case HookData.MouseEvent.WheelLeft: b = TMWheel.Left; break;
				default: b = TMWheel.Right; break;
				}
				data = (byte)b;
			}

			return _HookProc2(thc, false, subtype, k.Event, k.pt, data, 0);
		}

		internal void HookProcEdgeMove(in LibEdgeMoveDetector.Result d, TriggerHookContext thc)
		{
			ESubtype subtype;
			byte data = 0, dataAnyPart = 0;

			if(d.edgeEvent != 0) {
				subtype = ESubtype.Edge;
				data = (byte)d.edgeEvent; dataAnyPart = (byte)d.edgeEventAnyPart;
			} else {
				subtype = ESubtype.Move;
				data = (byte)d.moveEvent; dataAnyPart = (byte)d.moveEventAnyPart;
			}

			_HookProc2(thc, true, subtype, HookData.MouseEvent.Move, d.pt, data, dataAnyPart);
		}

		bool _HookProc2(TriggerHookContext thc, bool isEdgeMove, ESubtype subtype, HookData.MouseEvent mEvent, POINT pt, byte data, byte dataAnyPart)
		{
			var mod = TrigUtil.GetModLR(out var modL, out var modR) | _eatMod;
			MouseTriggerArgs args = null;
			g1:
			if(_d.TryGetValue(_DictKey(subtype, data), out var v)) {
				if(!isEdgeMove) thc.UseWndFromPoint(pt);
				for(; v != null; v = v.next) {
					var x = v as MouseTrigger;
					if((mod & x.modMask) != x.modMasked) continue;

					switch(x.flags & (TMFlags.LeftMod | TMFlags.RightMod)) {
					case TMFlags.LeftMod: if(modL != mod) continue; break;
					case TMFlags.RightMod: if(modR != mod) continue; break;
					}

					if(isEdgeMove && x.screenIndex != TMScreen.Any) {
						var screen = AScreen.ScreenFromIndex((int)x.screenIndex);
						if(!screen.Bounds.Contains(pt)) continue;
					}

					if(v.DisabledThisOrAll) continue;

					if(args == null) thc.args = args = new MouseTriggerArgs(x, thc.Window, mod); //may need for scope callbacks too
					else args.Trigger = x;

					if(!x.MatchScopeWindowAndFunc(thc)) continue;

					if(x.action == null) {
						_ResetUp();
					} else if(0 != (x.flags & TMFlags.ButtonModUp) && (mod != 0 || subtype == ESubtype.Click)) {
						_upTrigger = x;
						_upArgs = args;
						_upEvent = subtype == ESubtype.Click ? mEvent : 0;
						_upMod = mod;
					} else {
						thc.trigger = x;
						_ResetUp();
					}

					_eatMod = mod;
					if(mod != 0) {
						_SetTempKeybHook();
						if(thc.trigger != null) thc.muteMod = TriggerActionThreads.c_modRelease;
						else ThreadPool.QueueUserWorkItem(_ => Keyb.Lib.ReleaseModAndDisableModMenu());
					}

					//Print(mEvent, pt, mod);
					if(isEdgeMove || 0 != (x.flags & TMFlags.ShareEvent)) return false;
					if(subtype == ESubtype.Click) _eatUp = mEvent;
					return true;
				}
			}
			if(dataAnyPart != 0) {
				data = dataAnyPart;
				dataAnyPart = 0;
				goto g1;
			}
			return false;
		}

		//these are used to activate trigger when button and modifiers released
		MouseTrigger _upTrigger;
		MouseTriggerArgs _upArgs;
		HookData.MouseEvent _upEvent;
		KMod _upMod;
		//this is used to eat modifier keys, regardless when the trigger is activated
		KMod _eatMod;
		//these are used to eat modifier keys and to activate trigger when modifiers released
		WinHook _keyHook;
		long _keyHookTimeout;
		//this is used to eat button-up event, regardless when the trigger is activated
		HookData.MouseEvent _eatUp;

		void _ResetUp()
		{
			_upTrigger = null;
			_upArgs = null;
			_upEvent = 0;
			_upMod = 0;
		}

		void _UnhookTempKeybHook()
		{
			if(_keyHook != null) {
				//Print(". unhook");
				_keyHook.Unhook();
				_keyHookTimeout = _keyHook.LibIgnoreModInOtherHooks(0);
			}
		}

		void _ResetUpAndUnhookTempKeybHook()
		{
			_ResetUp();
			_eatMod = 0;
			_UnhookTempKeybHook();
		}

		void _SetTempKeybHook()
		{
			//Print(". hook");
			if(_keyHook == null) {
				_keyHook = WinHook.Keyboard(k => {
					if(Time.WinMilliseconds >= _keyHookTimeout) {
						_ResetUpAndUnhookTempKeybHook();
						ADebug.Print("hook timeout");
					} else {
						var mod = k.Mod;
						if(0 != (mod & _upMod) && k.IsUp) {
							_upMod &= ~mod;
							if(_upMod == 0 && _upEvent == 0 && _upTrigger != null) {
								_triggers.LibRunAction(_upTrigger, _upArgs);
								_ResetUp();
							}
						}
						if(0 != (mod & _eatMod)) {
							//Print(k);
							k.BlockEvent();
							if(k.IsUp) _eatMod &= ~mod;
						}
						if(0 == (_upMod | _eatMod)) _UnhookTempKeybHook();
					}
				}, setNow: false);
			}
			if(!_keyHook.IsSet) _keyHook.Hook();
			_keyHookTimeout = _keyHook.LibIgnoreModInOtherHooks(5000);
		}

		internal static void JitCompile()
		{
			Util.Jit.Compile(typeof(MouseTriggers), nameof(HookProcClickWheel), nameof(HookProcEdgeMove), nameof(_HookProc2));
			Wnd.FromXY(default, WXYFlags.NeedWindow);
		}

		/// <summary>
		/// Detects trigger events of types Edge and Move.
		/// </summary>
		/// <remarks>
		/// Used in the hook server, to avoid sending all mouse move events to clients, which would use 2 or more times more CPU, eg 0.9% instead of 0.45%. Tested: raw input uses slightly less CPU.
		/// </remarks>
		internal class LibEdgeMoveDetector
		{
			int _x, _y; //mouse position. Relative to the primary screen.
			int _xmin, _ymin, _xmax, _ymax; //min and max possible mouse position in current screen. Relative to the primary screen.
			_State _prev;
			int _sens;
			public Result result;

			internal struct Result
			{
				public POINT pt;
				public TMEdge edgeEvent, edgeEventAnyPart;
				public TMMove moveEvent, moveEventAnyPart;
			}

			/// <summary>
			/// State data set by previous events.
			/// </summary>
			struct _State
			{
				public int xx, yy; //previous coords
				public long time; //previous time

				//these used for Move events
				public int mx1, my1, mx2, my2;
				public TMMove mDirection;
				public long mTimeout;

				//these used for Edge events
				public long eTimeout;

#if DEBUG
				//public int debug;
#endif
			}

			public LibEdgeMoveDetector()
			{
				_sens = Util.Dpi.BaseDPI / 4; //FUTURE: different for each screen
			}

			public bool Detect(POINT pt)
			{
				//get normal x y. In pt can be outside screen when cursor moved fast and was stopped by a screen edge. Tested: never for click/wheel events.
				//var r = Screen.GetBounds(pt); //creates much garbage. Calls API MonitorFromPoint and creates new Screen object.
				//Print(pt, Mouse.XY);
				//var hmon = Api.MonitorFromPoint(pt, Api.MONITOR_DEFAULTTONEAREST); //problem with empty corners between 2 unaligned screens: when mouse tries to quickly diagonally cut such a corner, may activate a wrong trigger
				var hmon = Api.MonitorFromPoint(Mouse.XY, Api.MONITOR_DEFAULTTONEAREST); //smaller problem: Mouse.XY gets previous coordinates
				Api.MONITORINFO mi = default; mi.cbSize = Api.SizeOf<Api.MONITORINFO>();
				Api.GetMonitorInfo(hmon, ref mi);
				var r = mi.rcMonitor;
				_xmin = r.left; _ymin = r.top; _xmax = r.right - 1; _ymax = r.bottom - 1;
				_x = AMath.MinMax(pt.x, _xmin, _xmax);
				_y = AMath.MinMax(pt.y, _ymin, _ymax);
				//Print(pt, _x, _y, r);

				result = default;
				result.pt = (_x, _y);

				_Detect();
				_prev.xx = _x; _prev.yy = _y;

#if DEBUG
				//Print(++_prev.debug, edgeEvent, moveEvent, (_x, _y));
#endif
				return result.edgeEvent != 0 || result.moveEvent != 0;
			}

			void _Detect()
			{
				if(Mouse.IsPressed(MButtons.Left | MButtons.Right | MButtons.Middle)) {
					_prev.mDirection = 0;
					return;
				}

				int x = _x, y = _y;
				if(x == _prev.xx && y == _prev.yy) { /*Print("same x y");*/ return; }

				long time = Time.PerfMilliseconds;
				int dt = (int)(time - _prev.time);
				_prev.time = time;
				if(dt <= 0) return; //never noticed

				//Print((x, y), Mouse.XY, time%10000);

				if(y == _ymin || y == _ymax || x == _xmin || x == _xmax) {
					_prev.mDirection = 0;
					if(time < _prev.eTimeout) return; //prevent double trigger when OS sometimes gives strange coords if some hook blocks the event
					if(y == _ymin) { //top
						if(_prev.yy <= _ymin) return;
						if(AScreen.IsInAnyScreen((x, y - 1))) return;
						result.edgeEvent = (TMEdge)((int)(result.edgeEventAnyPart = TMEdge.Top) + _PartX(x));
					} else if(y == _ymax) { //bottom
						if(_prev.yy >= _ymax) return;
						if(AScreen.IsInAnyScreen((x, y + 1))) return;
						result.edgeEvent = (TMEdge)((int)(result.edgeEventAnyPart = TMEdge.Bottom) + _PartX(x));
					} else if(x == _xmin) { //left
						if(_prev.xx <= _xmin) return;
						if(AScreen.IsInAnyScreen((x - 1, y))) return;
						result.edgeEvent = (TMEdge)((int)(result.edgeEventAnyPart = TMEdge.Left) + _PartY(y));
					} else /*if(x == _xmax)*/ { //right
						if(_prev.xx >= _xmax) return;
						if(AScreen.IsInAnyScreen((x + 1, y))) return;
						result.edgeEvent = (TMEdge)((int)(result.edgeEventAnyPart = TMEdge.Right) + _PartY(y));
					}
					_prev.eTimeout = time + 100;
				} else {
					if(_prev.mDirection == 0) {
						if(time < _prev.mTimeout) return;

						int dx = (x - _prev.xx) * 16 / dt, dy = (y - _prev.yy) * 21 / dt;
						TMMove e = 0;
						if(dx > 0) {
							if(dy > 0) {
								if(dx > dy) {
									if(dx > _sens) e = TMMove.RightLeft;
								} else {
									if(dy > _sens) e = TMMove.DownUp;
								}
							} else {
								if(dx > -dy) {
									if(dx > _sens) e = TMMove.RightLeft;
								} else {
									if(-dy > _sens) e = TMMove.UpDown;
								}
							}
						} else {
							if(dy > 0) {
								if(-dx > dy) {
									if(-dx > _sens) e = TMMove.LeftRight;
								} else {
									if(dy > _sens) e = TMMove.DownUp;
								}
							} else {
								if(-dx > -dy) {
									if(-dx > _sens) e = TMMove.LeftRight;
								} else {
									if(-dy > _sens) e = TMMove.UpDown;
								}
							}
						}
						if(e != 0) {
							//Print(e);
							_prev.mDirection = e;
							_prev.mTimeout = time + 250;
							_prev.mx1 = _prev.xx; _prev.my1 = _prev.yy; _prev.mx2 = x; _prev.my2 = y;
						}
					} else {
						if(time > _prev.mTimeout) { _prev.mDirection = 0; return; }
						int part = 0;
						var e = _prev.mDirection;
						switch(e) {
						case TMMove.RightLeft:
							if(x < (_prev.mx1 + _prev.mx2) >> 1) {
								_prev.mDirection = 0;
								int dy = (_prev.my1 + _prev.my2) >> 1, dx = (_prev.mx2 - _prev.mx1) >> 2;
								if(Math.Abs(y - dy) > dx || Math.Abs(_prev.my1 - _prev.my2) > dx << 1) break;
								part = _PartY(y);
							} else if(x > _prev.mx2) { _prev.mx2 = x; _prev.my2 = y; }
							break;
						case TMMove.LeftRight:
							if(x > (_prev.mx1 + _prev.mx2) >> 1) {
								_prev.mDirection = 0;
								int dy = (_prev.my1 + _prev.my2) >> 1, dx = (_prev.mx1 - _prev.mx2) >> 2;
								if(Math.Abs(y - dy) > dx || Math.Abs(_prev.my1 - _prev.my2) > dx << 1) break;
								part = _PartY(y);
							} else if(x < _prev.mx2) { _prev.mx2 = x; _prev.my2 = y; }
							break;
						case TMMove.DownUp:
							if(y < (_prev.my1 + _prev.my2) >> 1) {
								_prev.mDirection = 0;
								int dx = (_prev.mx1 + _prev.mx2) >> 1, dy = (_prev.my2 - _prev.my1) >> 2;
								if(Math.Abs(x - dx) > dy || Math.Abs(_prev.mx1 - _prev.mx2) > dy << 1) break;
								part = _PartX(x);
							} else if(y > _prev.my2) { _prev.mx2 = x; _prev.my2 = y; }
							break;
						case TMMove.UpDown:
							if(y > (_prev.my1 + _prev.my2) >> 1) {
								_prev.mDirection = 0;
								int dx = (_prev.mx1 + _prev.mx2) >> 1, dy = (_prev.my1 - _prev.my2) >> 2;
								if(Math.Abs(x - dx) > dy && Math.Abs(_prev.mx1 - _prev.mx2) > dy << 1) break;
								part = _PartX(x);
							} else if(y < _prev.my2) { _prev.mx2 = x; _prev.my2 = y; }
							break;
						}
						if(part != 0) {
							result.moveEventAnyPart = e;
							result.moveEvent = (TMMove)((int)e + part);
						}
					}
				}
			}

			int _PartX(int x)
			{
				int q = (_xmax - _xmin) / 4;
				if(x < _xmin + q) return 2;
				if(x >= _xmax - q) return 3;
				return 1;
			}

			int _PartY(int y)
			{
				int q = (_ymax - _ymin) / 4;
				if(y < _ymin + q) return 2;
				if(y >= _ymax - q) return 3;
				return 1;
			}

			///// <summary>
			///// Sensitivity of <see cref="TMMove"/> tiggers.
			///// Default: 5. Can be 0 (less sensitive) to 10 (more sensitive).
			///// </summary>
			//public int MoveSensitivity {
			//	get => _sensPublic;
			//	set {
			//		if((uint)value > 10) throw new ArgumentOutOfRangeException(null, "0-10");
			//		_sensPublic = value;
			//		_sens = (int)(Math.Pow(1.414, 14 - value) * 1.3);
			//		Print(_sens); //29 when _sensPublic 5 (default)
			//	}
			//}
			//int _sensPublic;
		}

		/// <summary>
		/// Used by foreach to enumerate added triggers.
		/// </summary>
		public IEnumerator<MouseTrigger> GetEnumerator()
		{
			foreach(var kv in _d) {
				for(var v = kv.Value; v != null; v = v.next) {
					var x = v as MouseTrigger;
					yield return x;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Arguments for actions of mouse triggers.
	/// </summary>
	public class MouseTriggerArgs : TriggerArgs
	{
		///
		public MouseTrigger Trigger { get; internal set; }

		/// <summary>
		/// The active window (Edge and Move triggers) or the mouse window (Click and Wheel triggers).
		/// </summary>
		public Wnd Window { get; }

		/// <summary>
		/// The pressed modifier keys.
		/// </summary>
		/// <remarks>
		/// Can be useful when the trigger ignores modifiers. For example <i>modKeys</i> is "?" or "Shift?".
		/// </remarks>
		public KMod Mod { get; }

		///
		public MouseTriggerArgs(MouseTrigger trigger, Wnd w, KMod mod)
		{
			Trigger = trigger;
			Window = w; Mod = mod;
		}
	}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	/// <summary>
	/// Button for mouse click triggers.
	/// </summary>
	public enum TMClick : byte { Left, Right, Middle, X1, X2 }

	/// <summary>
	/// Mouse wheel direction for mouse wheel triggers.
	/// </summary>
	public enum TMWheel : byte { Forward, Backward, Left, Right }

	/// <summary>
	/// Screen edge for mouse edge triggers.
	/// </summary>
	/// <remarks>
	/// To activate a screen edge trigger, the user touches a screen edge with the mouse pointer.
	/// Each screen edge is divided into 3 parts: 1 - center 50%; 2 - left or top 25%; 3 - right or bottom 25%. Constants like <b>TopInCenter50</b> specify an edge and part; the trigger works only in that part of that edge. Constants like <b>Top</b> specify just an edge; the trigger works in all parts of that edge.
	/// </remarks>
	public enum TMEdge : byte
	{
		Top = 1, TopInCenter50, TopInLeft25, TopInRight25,
		Bottom, BottomInCenter50, BottomInLeft25, BottomInRight25,
		Left, LeftInCenter50, LeftInTop25, LeftInBottom25,
		Right, RightInCenter50, RightInTop25, RightInBottom25,
	}

	/// <summary>
	/// Mouse movement directions for mouse move triggers.
	/// </summary>
	/// <remarks>
	/// To activate a mouse move trigger, the user quickly moves the mouse pointer to the specified direction and back.
	/// The screen is divided into 3 parts: 1 - center 50%; 2 - left or top 25%; 3 - right or bottom 25%. Constants like <b>UpDownInCenter50</b> specify a direction and screen part; the trigger works only in that screen part. Constants like <b>UpDown</b> specify just a direction; the trigger works in whole screen.
	/// </remarks>
	public enum TMMove : byte
	{
		RightLeft = 1, RightLeftInCenter50, RightLeftInTop25, RightLeftInBottom25,
		LeftRight, LeftRightInCenter50, LeftRightInTop25, LeftRightInBottom25,
		UpDown, UpDownInCenter50, UpDownInLeft25, UpDownInRight25,
		DownUp, DownUpInCenter50, DownUpInLeft25, DownUpInRight25,
	}

	/// <summary>
	/// Screen index for mouse triggers.
	/// </summary>
	public enum TMScreen
	{
		Primary,
		NonPrimary1, NonPrimary2, NonPrimary3, NonPrimary4, NonPrimary5,
		Any = -1,
		OfActiveWindow = -2,
	}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
