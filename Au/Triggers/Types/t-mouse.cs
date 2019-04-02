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

namespace Au.Triggers
{
	/// <summary>
	/// Flags of mouse triggers.
	/// </summary>
	[Flags]
	public enum TMFlags : byte
	{
		/// <summary>
		/// Allow other apps to receive the mouse button and wheel messages too.
		/// Used only with the click and wheel triggers.
		/// </summary>
		PassMessage = 1,

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

		//rejected. Releasing mod keys makes no sense because we cannot disable the auto-repeat. For hotkeys OS (or hardware) disables it.
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
		internal readonly TMFlags flags;
		internal readonly int screenIndex;
		string _paramsString;

		internal MouseTrigger(ActionTriggers triggers, Action<MouseTriggerArgs> action, TMFlags flags, int screenIndex, string paramsString) : base(triggers, action, true)
		{
			this.flags = flags;
			this.screenIndex = screenIndex;
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
	/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
	public class MouseTriggers : ITriggers
	{
		enum ESubtype : byte { Click, Wheel, Edge, Move }

		ActionTriggers _triggers;
		Dictionary<int, ActionTrigger> _d = new Dictionary<int, ActionTrigger>();

		internal MouseTriggers(ActionTriggers triggers)
		{
			_triggers = triggers;
			MoveSensitivity = 5;
		}

		//static int _DictKey(ESubtype subtype, KMod mod, byte data1, byte data2 = 0) => (data2 << 24) | (data1 << 16) | ((byte)mod << 8) | (byte)subtype;
		static int _DictKey(ESubtype subtype, KMod mod, byte data) => (data << 16) | ((byte)mod << 8) | (byte)subtype;

		/// <summary>
		/// Adds a mouse click trigger.
		/// </summary>
		/// <param name="button"></param>
		/// <param name="modKeys">
		/// Modifier keys, like with the <see cref="Key" r=""/> function.
		/// Examples: "Ctrl", "Ctrl+Shift+Alt+Win".
		/// To ignore modifiers: "?". Then the trigger works with any combination of modifiers.
		/// To ignore a modifier: "Ctrl?". Then the trigger works with or without the modifier. More examples: "Ctrl?+Shift?", "Ctrl+Shift?".
		/// </param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <b>Triggers.Run</b> was called, until it returns.</exception>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public Action<MouseTriggerArgs> this[TMClick button, string modKeys = null, TMFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Click, modKeys, flags, (byte)button, 0, button.ToString());
			}
		}

		/// <summary>
		/// Adds a mouse wheel trigger.
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="modKeys"><inheritdoc cref="this[TMClick, string, TMFlags]"/></param>
		/// <param name="flags"></param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <b>Triggers.Run</b> was called, until it returns.</exception>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public Action<MouseTriggerArgs> this[TMWheel direction, string modKeys = null, TMFlags flags = 0] {
			set {
				var t = _Add(value, ESubtype.Wheel, modKeys, flags, (byte)direction, 0, direction.ToString());
			}
		}

		/// <summary>
		/// Adds a mouse screen edge trigger.
		/// </summary>
		/// <param name="edge"></param>
		/// <param name="modKeys"><inheritdoc cref="this[TMClick, string, TMFlags]"/></param>
		/// <param name="flags"></param>
		/// <param name="screenIndex">
		/// Let the trigger work only in this screen (display monitor).
		/// 
		/// 0 (default) - only in the primary screen.
		/// &gt;0 - a non-primary screen index.
		/// -1 - all screens.
		/// -2 - screen of the active window at that time.
		/// 
		/// Uses <see cref="Screen.AllScreens"/> to get screen indices. They may not match the indices that you can see in Control Panel.
		/// </param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <b>Triggers.Run</b> was called, until it returns.</exception>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public Action<MouseTriggerArgs> this[TMEdge edge, string modKeys = null, TMFlags flags = 0, int screenIndex = 0] {
			set {
				var t = _Add(value, ESubtype.Edge, modKeys, flags, (byte)edge, screenIndex, edge.ToString());
			}
		}

		/// <summary>
		/// Adds a mouse move trigger.
		/// </summary>
		/// <param name="move"></param>
		/// <param name="modKeys"><inheritdoc cref="this[TMClick, string, TMFlags]"/></param>
		/// <param name="flags"></param>
		/// <param name="screenIndex"><inheritdoc cref="this[TMEdge, string, TMFlags, int]"/></param>
		/// <exception cref="ArgumentException">Invalid modKeys string or flags.</exception>
		/// <exception cref="InvalidOperationException">Cannot add triggers after <b>Triggers.Run</b> was called, until it returns.</exception>
		/// <remarks>Example: <see cref="ActionTriggers"/>.</remarks>
		public Action<MouseTriggerArgs> this[TMMove move, string modKeys = null, TMFlags flags = 0, int screenIndex = 0] {
			set {
				var t = _Add(value, ESubtype.Move, modKeys, flags, (byte)move, screenIndex, move.ToString());
			}
		}

		MouseTrigger _Add(Action<MouseTriggerArgs> f, ESubtype subtype, string modKeys, TMFlags flags, byte data, int screenIndex, string sData)
		{
			_triggers.LibThrowIfRunning();
			bool noMod = Empty(modKeys);

			string ps;
			using(new Util.LibStringBuilder(out var b)) {
				b.Append(subtype.ToString()).Append(' ').Append(sData);
				b.Append(" + ").Append(noMod ? "no modifiers" : (modKeys == "?" ? "any modifiers" : modKeys));
				if(flags != 0) b.Append(" (").Append(flags.ToString()).Append(')');
				if(subtype == ESubtype.Edge || subtype == ESubtype.Move) {
					if(screenIndex == 0) b.Append(", primary screen");
					else if(screenIndex > 0) b.Append(", non-primary screen ").Append(screenIndex);
					else if(screenIndex == -1) b.Append(", any screen");
					else if(screenIndex == -2) b.Append(", screen of the active window");
					else throw new ArgumentException();
				}
				ps = b.ToString(); //Print(ps);
			}

			var t = new MouseTrigger(_triggers, f, flags, screenIndex, ps);
			if(noMod) {
				if(flags.HasAny_(subtype == ESubtype.Click ? TMFlags.LeftMod | TMFlags.RightMod : TMFlags.LeftMod | TMFlags.RightMod | TMFlags.ButtonModUp)) throw new ArgumentException("Invalid flags.");
				t.DictAdd(_d, _DictKey(subtype, 0, data));
			} else {
				if(!Keyb.Misc.LibParseHotkeyTriggerString(modKeys, out var mod, out var modAny, out _, true)) throw new ArgumentException("Invalid modKeys string.");
				var b = TrigUtil.ModBitArray(mod, modAny);
				for(int i = 0; i < 16; i++) if(0 != (b & (1 << i))) t.DictAdd(_d, _DictKey(subtype, (KMod)i, data));
			}
			_lastAdded = t;
			return t;
		}

		/// <summary>
		/// The last added trigger.
		/// </summary>
		public MouseTrigger Last => _lastAdded;
		MouseTrigger _lastAdded;

		bool ITriggers.HasTriggers => _lastAdded != null;

		void ITriggers.StartStop(bool start)
		{
			if(start) {
				_mmState = new _MMState();
			} else {
				_mmState = null;
				_CancelUp();
			}
		}

		internal bool HookProc(HookData.Mouse k, TriggerHookContext thc)
		{
			//Print(k.Event, k.pt);
			Debug.Assert(!k.IsInjectedByAu); //server must ignore

			ESubtype subtype;
			byte data = 0, dataAnyPart = 0;
			bool isMoveEdge = false;
			POINT mmPoint = default;

			if(k.IsButton) {
				if(k.IsButtonUp) {
					if(_upTrigger != null && k.Event == _upEvent) {
						_upEvent = 0;
						if(Keyb.IsMod()) {
							_SetTimerToWaitForModUp();
						} else {
							thc.args = _upArgs;
							thc.trigger = _upTrigger;
							_upTrigger = null;
							_upArgs = null;
						}
					}
					return false;
				}
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
			} else if(k.IsWheel) {
				subtype = ESubtype.Wheel;
				TMWheel b;
				switch(k.Event) {
				case HookData.MouseEvent.WheelForward: b = TMWheel.Forward; break;
				case HookData.MouseEvent.WheelBackward: b = TMWheel.Backward; break;
				case HookData.MouseEvent.WheelLeft: b = TMWheel.Left; break;
				default: b = TMWheel.Right; break;
				}
				data = (byte)b;
			} else {
				var mm = new _MMDetector(this, k, out mmPoint);
				if(!mm.Detect()) return false;
				subtype = mm.subtype;
				if(subtype == ESubtype.Move) {
					data = (byte)mm.moveEvent; dataAnyPart = (byte)mm.moveEventAnyPart;
				} else {
					data = (byte)mm.edgeEvent; dataAnyPart = (byte)mm.edgeEventAnyPart;
				}
				isMoveEdge = true;
			}

			_CancelUp();

			var mod = TrigUtil.GetModLR(out var modL, out var modR);
			MouseTriggerArgs args = null;
			g1:
			if(_d.TryGetValue(_DictKey(subtype, mod, data), out var v)) {
				if(!isMoveEdge) thc.UseWndFromPoint(k.pt);
				for(; v != null; v = v.next) {
					if(v.DisabledThisOrAll) continue;
					var x = v as MouseTrigger;

					switch(x.flags & (TMFlags.LeftMod | TMFlags.RightMod)) {
					case TMFlags.LeftMod: if(modL != mod) continue; break;
					case TMFlags.RightMod: if(modR != mod) continue; break;
					}

					if(isMoveEdge && x.screenIndex != -1) {
						var screen = Screen_.ScreenFromIndex(x.screenIndex);
						if(!screen.Bounds.Contains(mmPoint)) continue;
					}

					if(args == null) thc.args = args = new MouseTriggerArgs(x, thc.Window, mod); //may need for scope callbacks too
					else args.Trigger = x;
					if(!x.MatchScopeWindowAndFunc(thc)) continue;

					if(0 != (x.flags & TMFlags.ButtonModUp) && (mod != 0 || subtype == ESubtype.Click)) {
						_upTrigger = x;
						_upArgs = args;
						_upEvent = 0;
						if(subtype == ESubtype.Click) _upEvent = k.Event;
						else _SetTimerToWaitForModUp();
					} else {
						thc.trigger = x;
					}

					//Print(k.Event, k.pt, mod);
					if(isMoveEdge) return false;
					return 0 == (x.flags & TMFlags.PassMessage);
				}
			}
			if(dataAnyPart != 0) {
				data = dataAnyPart;
				dataAnyPart = 0;
				goto g1;
			}
			return false;
		}

		MouseTrigger _upTrigger;
		MouseTriggerArgs _upArgs;
		HookData.MouseEvent _upEvent;
		Timer_ _upTimer;
		bool _upIsTimer;

		void _SetTimerToWaitForModUp()
		{
			if(_upTimer == null) _upTimer = new Timer_(timer => {
				if(_upTrigger != null && Keyb.IsMod()) return;
				timer.Stop();
				_upIsTimer = false;
				if(_upTrigger != null) {
					var t = _upTrigger;
					_upTrigger = null;
					_triggers.LibRunAction(t, _upArgs);
					_upArgs = null;
					_upEvent = 0;
				}
			});
			_upTimer.Start(15, false);
			_upIsTimer = true;
		}

		void _CancelUp()
		{
			if(_upTrigger != null) {
				_upTrigger = null;
				_upArgs = null;
				_upEvent = 0;
			}
			if(_upIsTimer) {
				_upTimer.Stop();
				_upIsTimer = false;
			}
		}

		/// <summary>
		/// Detects trigger events of types Edge and Move.
		/// </summary>
		struct _MMDetector
		{
			int _x, _y; //mouse position. Relative to the primary screen.
			int _xmin, _ymin, _xmax, _ymax; //min and max possible mouse position in current screen. Relative to the primary screen.
			_MMState _prev;
			int _sens;
			public ESubtype subtype;
			public TMEdge edgeEvent, edgeEventAnyPart;
			public TMMove moveEvent, moveEventAnyPart;

			public _MMDetector(MouseTriggers mTriggers, HookData.Mouse m, out POINT realPoint) : this()
			{
				_prev = mTriggers._mmState;
				_sens = mTriggers._mmSens;

				//get normal x y. In m can be outside screen when cursor moved fast and was stopped by a screen edge. Tested: never for click/wheel events.
				//var r = Screen.GetBounds(m.pt); //creates much garbage. Calls API MonitorFromPoint and creates new Screen object.
				var hmon = Api.MonitorFromPoint(m.pt, Api.MONITOR_DEFAULTTONEAREST);
				Api.MONITORINFO mi = default; mi.cbSize = Api.SizeOf<Api.MONITORINFO>();
				Api.GetMonitorInfo(hmon, ref mi);
				var r = mi.rcMonitor;
				_xmin = r.left; _ymin = r.top; _xmax = r.right - 1; _ymax = r.bottom - 1;
				_x = Math_.MinMax(m.pt.x, _xmin, _xmax);
				_y = Math_.MinMax(m.pt.y, _ymin, _ymax);
				//Print(m.pt, _x, _y);
				realPoint = (_x, _y);
			}

			public bool Detect()
			{
				_Detect();
				_prev.xx = _x; _prev.yy = _y;

				if(edgeEvent != 0) subtype = ESubtype.Edge;
				else if(moveEvent != 0) subtype = ESubtype.Move;
				else return false;

#if DEBUG
				//Print(++_prev._debug, subtype, edgeEvent, moveEvent, (_x, _y));
#endif
				return true;
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
						if(Screen_.IsInAnyScreen((x, y - 1))) return;
						edgeEvent = (TMEdge)((int)(edgeEventAnyPart = TMEdge.Top) + _PartX(x));
					} else if(y == _ymax) { //bottom
						if(_prev.yy >= _ymax) return;
						if(Screen_.IsInAnyScreen((x, y + 1))) return;
						edgeEvent = (TMEdge)((int)(edgeEventAnyPart = TMEdge.Bottom) + _PartX(x));
					} else if(x == _xmin) { //left
						if(_prev.xx <= _xmin) return;
						if(Screen_.IsInAnyScreen((x - 1, y))) return;
						edgeEvent = (TMEdge)((int)(edgeEventAnyPart = TMEdge.Left) + _PartY(y));
					} else /*if(x == _xmax)*/ { //right
						if(_prev.xx >= _xmax) return;
						if(Screen_.IsInAnyScreen((x + 1, y))) return;
						edgeEvent = (TMEdge)((int)(edgeEventAnyPart = TMEdge.Right) + _PartY(y));
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
							moveEventAnyPart = e;
							moveEvent = (TMMove)((int)e + part);
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
		}

		/// <summary>
		/// State data set by previous _MMDetector instances.
		/// </summary>
		class _MMState
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
			//public int _debug;
#endif
		}
		_MMState _mmState;

		/// <summary>
		/// Sensitivity of <see cref="TMMove"/> tiggers.
		/// Default: 5. Can be 0 (less sensitive) to 10 (more sensitive).
		/// </summary>
		public int MoveSensitivity {
			get => _mmSensPublic;
			set {
				if((uint)value > 10) throw new ArgumentOutOfRangeException(null, "0-10");
				_mmSensPublic = value;
				_mmSens = (int)(Math.Pow(1.414, 14 - value) * 1.3);
				//Print(_sens); //29 when _sensPublic 5 (default)
			}
		}
		int _mmSens, _mmSensPublic;
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
