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

using Au.Types;
using static Au.NoClass;
using Au.Util;

namespace Au
{
	/// <summary>
	/// Mouse functions.
	/// </summary>
	/// <remarks>
	/// Should not be used to click windows of own thread. It may work or not. If need, use another thread. Example in <see cref="Keyb.Key"/>.
	/// </remarks>
	public static partial class Mouse
	{
		/// <summary>
		/// Gets cursor (mouse pointer) position.
		/// </summary>
		public static POINT XY { get { Api.GetCursorPos(out var p); return p; } }

		/// <summary>
		/// Gets cursor (mouse pointer) X coordinate (Mouse.XY.x).
		/// </summary>
		public static int X => XY.x;

		/// <summary>
		/// Gets cursor (mouse pointer) Y coordinate (Mouse.XY.y).
		/// </summary>
		public static int Y => XY.y;

		static void _Move(POINT p, bool fast)
		{
			bool relaxed = Opt.Mouse.Relaxed, willFail = false;

			if(!AScreen.IsInAnyScreen(p)) {
				if(!relaxed) throw new ArgumentOutOfRangeException(null, "Cannot mouse-move. This x y is not in screen. " + p.ToString());
				willFail = true;
			}

			if(!fast) _MoveSlowTo(p);

			POINT p0 = XY;
			//bool retry = false; g1:
			bool ok = false;
			for(int i = 0, n = relaxed ? 3 : 10; i < n; i++) {
				//Perf.First();
				_SendMove(p);
				//info: now XY is still not updated in ~10% cases.
				//	In my tests was always updated after sleeping 0 or 1 ms.
				//	But the user etc also can move the mouse at the same time. Then the i loop always helps.
				//Perf.Next();
				int j = 0;
				for(; ; j++) {
					var pNow = XY;
					ok = (pNow == p);
					if(ok || pNow != p0 || j > 3) break;
					Time.Sleep(j); //0+1+2+3
				}
				//Perf.NW();
				//Print(j, i);
				if(ok || willFail) break;
				//note: don't put the _Sleep(7) here
			}
			if(!ok && !relaxed) {
				var es = $"*mouse-move to this x y in screen. " + p.ToString();
				Wnd.Active.LibUacCheckAndThrow(es + ". The active"); //it's a mystery for users. API SendInput fails even if the point is not in the window.
																	 //rejected: Wnd.GetWnd.Root.ActivateLL()
				throw new AException(es);
				//known reasons:
				//	Active window of higher UAC IL.
				//	BlockInput, hook, some script etc that blocks mouse movement or restores mouse position.
				//	ClipCursor.
			}
			t_prevMousePos.last = p;

			_Sleep(Opt.Mouse.MoveSleepFinally);
		}

		static void _MoveSlowTo(POINT p)
		{
			int speed = Opt.Mouse.MoveSpeed;
			if(speed == 0 && IsPressed()) speed = 1; //make one intermediate movement, else some programs don't select text
			if(speed > 0) {
				var p0 = XY;
				int dxall = p.x - p0.x, dyall = p.y - p0.y;
				double dist = Math.Sqrt(dxall * dxall + dyall * dyall);
				if(dist > 1.5) {
					double dtall = Math.Sqrt(dist) * speed + 9;
					long t0 = Time.PerfMilliseconds - 7, dt = 7;
					int pdx = 0, pdy = 0;
					for(; ; dt = Time.PerfMilliseconds - t0) {
						double dtfr = dt / dtall;
						if(dtfr >= 1) break;

						int dx = (int)(dtfr * dxall), dy = (int)(dtfr * dyall);
						//Print(dx, dy, dtfr);

						POINT pt = (p0.x + dx, p0.y + dy);
						if(dx != pdx || dy != pdy) {
							_SendMove(pt);
							pdx = dx; pdy = dy;
						}

						_Sleep(7); //7-8 is WM_MOUSEMOVE period when user moves the mouse quite fast, even when the system timer period is 15.625 (default).
					}
				}
			}
		}

		/// <summary>
		/// Moves the cursor (mouse pointer) to the position x y relative to window w.
		/// </summary>
		/// <returns>Cursor position in primary screen coordinates.</returns>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="WndException">
		/// - Invalid window.
		/// - The top-level window is hidden. No exception if just cloaked, for example in another desktop; then on click will activate, which usually uncloaks.
		/// - Other window-related failures.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">The specified x y is not in screen. No exception if option <b>Relaxed</b> is true (then moves to a screen edge).</exception>
		/// <exception cref="AException">Failed to move the cursor to the specified x y.</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed"/>, <see cref="OptMouse.MoveSleepFinally"/>, <see cref="OptMouse.Relaxed"/>.
		/// </remarks>
		public static POINT Move(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			LibWaitForNoButtonsPressed();
			w.ThrowIfInvalid();
			var wTL = w.Window;
			if(!wTL.IsVisible) throw new WndException(wTL, "Cannot mouse-move. The window is invisible"); //should make visible? Probably not. If cloaked because in an inactive virtual desktop etc, Click activates and it usually uncloaks.
			if(wTL.IsMinimized) { wTL.ShowNotMinimized(true); _Sleep(500); } //never mind: if w is a control...
			var p = Coord.NormalizeInWindow(x, y, w, nonClient, centerIfEmpty: true);
			if(!w.MapClientToScreen(ref p)) w.ThrowUseNative();
			_Move(p, fast: false);
			return p;
		}

		/// <summary>
		/// Moves the cursor (mouse pointer) to the specified position in screen.
		/// </summary>
		/// <returns>Normalized cursor position.</returns>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <exception cref="ArgumentOutOfRangeException">The specified x y is not in screen. No exception if option <b>Relaxed</b> is true (then moves to a screen edge).</exception>
		/// <exception cref="AException">Failed to move the cursor to the specified x y.</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed"/>, <see cref="OptMouse.MoveSleepFinally"/>, <see cref="OptMouse.Relaxed"/>.
		/// 
		/// May fail to move the cursor to the specified x y. Some reasons:
		/// - Another thread blocks or modifies mouse input (API BlockInput, mouse hooks, frequent API SendInput etc).
		/// - The active window belongs to a process of higher [](xref:uac) integrity level.
		/// - Some application called API ClipCursor. No exception if option <b>Relaxed</b> is true (then final cursor position is undefined).
		/// </remarks>
		public static POINT Move(Coord x, Coord y)
		{
			LibWaitForNoButtonsPressed();
			var p = Coord.Normalize(x, y);
			_Move(p, fast: false);
			return p;
		}
		//rejected: parameters bool workArea = false, AScreen screen = default. Rarely used. Can use the POINT overload and Coord.Normalize.

		/// <summary>
		/// Moves the cursor (mouse pointer) to the specified position in screen.
		/// </summary>
		/// <param name="p">Coordinates.
		/// <note type="tip">When need coordinates relative to a non-primary screen or/and the work area, use <see cref="Coord.Normalize"/> or tuple (x, y, workArea) etc. Example: <c>Mouse.Move((x, y, true));</c>.</note>
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">The specified x y is not in screen. No exception if option <b>Relaxed</b> is true (then moves to a screen edge).</exception>
		/// <exception cref="AException">Failed to move the cursor to the specified x y.</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed"/>, <see cref="OptMouse.MoveSleepFinally"/>, <see cref="OptMouse.Relaxed"/>.
		/// </remarks>
		/// <example>
		/// Save-restore mouse position.
		/// <code><![CDATA[
		/// var p = Mouse.XY;
		/// //...;
		/// Mouse.Move(p);
		/// ]]></code>
		/// Use coodinates in the first non-primary screen.
		/// <code><![CDATA[
		/// Mouse.Move(Coord.Normalize(10, 10, screen: 1));
		/// ]]></code>
		/// </example>
		public static void Move(POINT p)
		{
			LibWaitForNoButtonsPressed();
			_Move(p, fast: false);
		}

		/// <summary>
		/// Remembers current mouse cursor position to be later restored with <see cref="Restore"/>.
		/// </summary>
		public static void Save()
		{
			if(t_prevMousePos == null) t_prevMousePos = new _PrevMousePos();
			else t_prevMousePos.first = XY;
		}

		/// <summary>
		/// Moves the mouse cursor where it was at the time of the last <see cref="Save"/> call in this thread. If it was not called - of the first 'mouse move' or 'mouse click' function call in this thread. Does nothing if these functions were not called.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSleepFinally"/>, <see cref="OptMouse.Relaxed"/>.
		/// </remarks>
		public static void Restore()
		{
			if(t_prevMousePos == null) return;
			LibWaitForNoButtonsPressed();
			_Move(t_prevMousePos.first, fast: true);
		}

		class _PrevMousePos
		{
			public POINT first, last;
			public _PrevMousePos() { first = last = XY; }
			public _PrevMousePos(POINT p) { first = last = p; }
		}
		[ThreadStatic] static _PrevMousePos t_prevMousePos;

		/// <summary>
		/// Mouse cursor position of the most recent successful 'mouse move' or 'mouse click' function call in this thread.
		/// If such functions are still not called in this thread, returns <see cref="XY"/>.
		/// </summary>
		public static POINT LastXY => t_prevMousePos?.last ?? XY;

		//rejected. MoveRelative usually is better. If need, can use code: Move(Mouse.X+dx, Mouse.Y+dy).
		//public static void MoveFromCurrent(int dx, int dy)
		//{
		//	var p = XY;
		//	p.Offset(dx, dy);
		//	Move(p);
		//}

		/// <summary>
		/// Moves the cursor (mouse pointer) relative to <see cref="LastXY"/>.
		/// Returns the final cursor position in primary screen coordinates.
		/// </summary>
		/// <param name="dx">X offset from <b>LastXY.x</b>.</param>
		/// <param name="dy">Y offset from <b>LastXY.y</b>.</param>
		/// <exception cref="ArgumentOutOfRangeException">The calculated x y is not in screen. No exception if option <b>Relaxed</b> is true (then moves to a screen edge).</exception>
		/// <exception cref="AException">Failed to move the cursor to the calculated x y. Some reasons: 1. Another thread blocks or modifies mouse input (API BlockInput, mouse hooks, frequent API SendInput etc); 2. The active window belongs to a process of higher [](xref:uac) integrity level; 3. Some application called API ClipCursor. No exception option <b>Relaxed</b> is true (then final cursor position is undefined).</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed"/>, <see cref="OptMouse.MoveSleepFinally"/>, <see cref="OptMouse.Relaxed"/>.
		/// </remarks>
		public static POINT MoveRelative(int dx, int dy)
		{
			LibWaitForNoButtonsPressed();
			var p = LastXY;
			p.x += dx; p.y += dy;
			_Move(p, fast: false);
			return p;
		}

		/// <summary>
		/// Plays recorded mouse movements, relative to <see cref="LastXY"/>.
		/// Returns the final cursor position in primary screen coordinates.
		/// </summary>
		/// <param name="recordedString">String containing mouse movement data recorded by a recorder tool that uses <see cref="Recording.MouseToString"/>.</param>
		/// <param name="speedFactor">Speed factor. For example, 0.5 makes 2 times faster.</param>
		/// <exception cref="ArgumentException">The string is not compatible with this library version (recorded with a newer version and has additional options).</exception>
		/// <exception cref="ArgumentOutOfRangeException">The last x y is not in screen. No exception option <b>Relaxed</b> is true (then moves to a screen edge).</exception>
		/// <exception cref="AException">Failed to move to the last x y. Some reasons: 1. Another thread blocks or modifies mouse input (API BlockInput, mouse hooks, frequent API SendInput etc); 2. The active window belongs to a process of higher [](xref:uac) integrity level; 3. Some application called API ClipCursor. No exception option <b>Relaxed</b> is true (then final cursor position is undefined).</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.Relaxed"/> (only for the last movement; always relaxed in intermediate movements).
		/// </remarks>
		public static POINT MoveRecorded(string recordedString, double speedFactor = 1.0)
		{
			LibWaitForNoButtonsPressed();

			var a = AConvert.Base64Decode(recordedString);

			byte flags = a[0];
			const int knownFlags = 1; if((flags & knownFlags) != flags) throw new ArgumentException("Unknown string version");
			bool withSleepTimes = 0 != (flags & 1);
			bool isSleep = withSleepTimes;

			var p = LastXY;
			int pdx = 0, pdy = 0;

			for(int i = 1; i < a.Length;) {
				if(i > 1 && (isSleep || !withSleepTimes)) {
					_SendMove(p);
					if(!withSleepTimes) _Sleep((int)Math.Round(7 * speedFactor));
				}

				int v = a[i++], nbytes = (v & 3) + 1;
				for(int j = 1; j < nbytes; j++) v |= a[i++] << j * 8;
				v = (int)((uint)v >> 2);
				if(isSleep) {
					//Print($"nbytes={nbytes}    sleep={v}");

					_Sleep((int)Math.Round(v * speedFactor));
				} else {
					int shift = nbytes * 4 - 1, mask = (1 << shift) - 1;
					int x = v & mask, y = (v >> shift) & mask;
					shift = 32 - shift; x <<= shift; x >>= shift; y <<= shift; y >>= shift; //sign-extend
					int dx = pdx + x; pdx = dx;
					int dy = pdy + y; pdy = dy;

					//Print($"dx={dx} dy={dy}    x={x} y={y}    nbytes={nbytes}    v=0x{v:X}");

					p.x += dx; p.y += dy;
				}
				isSleep ^= withSleepTimes;
			}
			_Move(p, fast: true);
			return p;
		}
	}

	namespace Util
	{
		/// <summary>
		/// Functions for keyboard/mouse/etc recorder tools.
		/// </summary>
		public static partial class Recording
		{
			/// <summary>
			/// Converts multiple recorded mouse movements to string for <see cref="Mouse.MoveRecorded(string, double)"/>.
			/// </summary>
			/// <param name="recorded">
			/// List of x y distances from previous.
			/// The first distance is from the mouse position before the first movement; at run time it will be distance from <see cref="Mouse.LastXY"/>.
			/// To create uint value from distance dx dy use this code: <c>AMath.MakeUint(dx, dy)</c>.
			/// </param>
			/// <param name="withSleepTimes">
			/// <i>recorded</i> also contains sleep times (milliseconds) alternating with distances.
			/// It must start with a sleep time. Example: {time1, dist1, time2, dist2}. Another example: {time1, dist1, time2, dist2, time3}. This is invalid: {dist1, time1, dist2, time2}.
			/// </param>
			public static string MouseToString(IEnumerable<uint> recorded, bool withSleepTimes)
			{
				var a = new List<byte>();

				byte flags = 0;
				if(withSleepTimes) flags |= 1;
				a.Add(flags);

				int pdx = 0, pdy = 0;
				bool isSleep = withSleepTimes;
				foreach(var u in recorded) {
					int v, nbytes = 4;
					if(isSleep) {
						v = (int)Math.Min(u, 0x3fffffff);
						if(v > 3) v--; //_SendMove usually takes 0.5-1.5 ms
						if(v <= 1 << 6) nbytes = 1;
						else if(v <= 1 << 14) nbytes = 2;
						else if(v <= 1 << 22) nbytes = 3;

						//Print($"nbytes={nbytes}    sleep={v}");
						//never mind: ~90% is 7. Removing it would make almost 2 times smaller string. But need much more code. Or compress (see comment below).
					} else {
						//info: to make more compact, we write not distances (dx dy) but distance changes (x y).
						int dx = AMath.LoShort(u), x = dx - pdx; pdx = dx;
						int dy = AMath.HiShort(u), y = dy - pdy; pdy = dy;

						if(x >= -4 && x < 4 && y >= -4 && y < 4) nbytes = 1; //3+3+2=8 bits, 90%
						else if(x >= -64 && x < 64 && y >= -64 && y < 64) nbytes = 2; //7+7+2=16 bits, ~10%
						else if(x >= -1024 && x < 1024 && y >= -1024 && y < 1024) nbytes = 3; //11+11+2=24 bits, ~0%

						int shift = nbytes * 4 - 1, mask = (1 << shift) - 1;
						v = (x & mask) | ((y & mask) << shift);

						//Print($"dx={dx} dy={dy}    x={x} y={y}    nbytes={nbytes}    v=0x{v:X}");
					}
					v <<= 2; v |= (nbytes - 1);
					for(; nbytes != 0; nbytes--, v >>= 8) a.Add((byte)v);
					isSleep ^= withSleepTimes;
				}

				//rejected: by default compresses to ~80% (20% smaller). When withSleepTimes, to ~50%, but never mind, rarely used.
				//Print(a.Count, AConvert.Compress(a.ToArray()).Length);

				return Convert.ToBase64String(a.ToArray());
			}
		}
	}

	public static partial class Mouse
	{
		/// <summary>
		/// Sends single mouse movement event.
		/// x y are normal absolute coordinates.
		/// </summary>
		static void _SendMove(POINT p)
		{
			if(t_prevMousePos == null) t_prevMousePos = new _PrevMousePos(); //sets .first=.last=XY
			_SendRaw(Api.IMFlags.Move, p.x, p.y);
		}

		/// <summary>
		/// Sends single mouse button down or up event.
		/// Does not use the action flags of button.
		/// Applies SM_SWAPBUTTON.
		/// Also moves to p in the same API SendInput call.
		/// </summary>
		static void _SendButton(MButton button, bool down, POINT p)
		{
			//CONSIDER: release user-pressed modifier keys, like Keyb does.
			//CONSIDER: block user input, like Keyb does.

			Api.IMFlags f; MButtons mb = 0;
			switch(button & (MButton.Left | MButton.Right | MButton.Middle | MButton.X1 | MButton.X2)) {
			case 0: //allow 0 for left. Example: Wnd.Find(...).MouseClick(x, y, MButton.DoubleClick)
			case MButton.Left: f = down ? Api.IMFlags.LeftDown : Api.IMFlags.LeftUp; mb = MButtons.Left; break;
			case MButton.Right: f = down ? Api.IMFlags.RightDown : Api.IMFlags.RightUp; mb = MButtons.Right; break;
			case MButton.Middle: f = down ? Api.IMFlags.MiddleDown : Api.IMFlags.MiddleUp; mb = MButtons.Middle; break;
			case MButton.X1: f = down ? Api.IMFlags.XDown | Api.IMFlags.X1 : Api.IMFlags.XUp | Api.IMFlags.X1; mb = MButtons.X1; break;
			case MButton.X2: f = down ? Api.IMFlags.XDown | Api.IMFlags.X2 : Api.IMFlags.XUp | Api.IMFlags.X2; mb = MButtons.X2; break;
			default: throw new ArgumentException("Several buttons specified", nameof(button)); //rejected: InvalidEnumArgumentException. It's in System.ComponentModel namespace.
			}

			//maybe mouse left/right buttons are swapped
			if(0 != (button & (MButton.Left | MButton.Right)) && 0 != Api.GetSystemMetrics(Api.SM_SWAPBUTTON))
				f ^= down ? Api.IMFlags.LeftDown | Api.IMFlags.RightDown : Api.IMFlags.LeftUp | Api.IMFlags.RightUp;

			//If this is a Click(x y), the sequence of sent events is like: move, sleep, down, sleep, up. Even Click() sleeps between down and up.
			//During the sleep the user can move the mouse. Correct it now if need.
			//tested: if don't need to move, mouse messages are not sent. Hooks not tested. In some cases are sent one or more mouse messages but it depends on other things.
			//Alternatively could temporarily block user input, but it is not good. Need a hook (UAC disables Api.BlockInput), etc. Better let scripts do it explicitly. If script contains several mouse/keys statements, it's better to block input once for all.
			f |= Api.IMFlags.Move;

			_SendRaw(f, p.x, p.y);

			if(down) t_pressedButtons |= mb; else t_pressedButtons &= ~mb;
		}

		/// <summary>
		/// Calls Api.SendInput to send single mouse movement or/and button down or up or wheel event.
		/// Converts x, y and wheelTicks as need for MOUSEINPUT.
		/// For X buttons use Api.IMFlag.XDown|Api.IMFlag.X1 etc.
		/// If Api.IMFlag.Move, adds Api.IMFlag.Absolute.
		/// </summary>
		static unsafe void _SendRaw(Api.IMFlags flags, int x = 0, int y = 0, int wheelTicks = 0)
		{
			if(0 != (flags & Api.IMFlags.Move)) {
				flags |= Api.IMFlags.Absolute;
				x <<= 16; x += (x >= 0) ? 0x8000 : -0x8000; x /= AScreen.PrimaryWidth;
				y <<= 16; y += (y >= 0) ? 0x8000 : -0x8000; y /= AScreen.PrimaryHeight;
			}

			int mouseData = 0;
			if(0 != (flags & (Api.IMFlags.XDown | Api.IMFlags.XUp))) {
				mouseData = (int)((uint)flags >> 24);
				flags &= (Api.IMFlags)0xffffff;
			} else mouseData = wheelTicks * 120;

			var k = new Api.INPUTM(flags, x, y, mouseData);
			Api.SendInput(&k);
			//note: the API never indicates a failure if arguments are valid. Tested UAC (documented), BlockInput, ClipCursor.
		}

		static void _Sleep(int ms)
		{
			Time.SleepDoEvents(ms);

			//note: always doevents, even if window from point is not of our thread. Because:
			//	Cannot always reliably detect what window will receive the message and what then happens.
			//	There is not much sense to avoid doevents. If no message loop, it is fast and safe; else the script author should use another thread or expect anything.
			//	API SendInput dispatches sent messages anyway.
			//	_Click shows warning if window of this thread.

			//FUTURE: sync better, especially finally.
		}

		[ThreadStatic] static MButtons t_pressedButtons;

		static void _Click(MButton button, POINT p, Wnd w = default)
		{
			if(w.Is0) w = Api.WindowFromPoint(p);
			bool windowOfThisThread = w.IsOfThisThread;
			if(windowOfThisThread) PrintWarning("Click(window of own thread) may not work. Use another thread.");
			//Sending a click to a window of own thread often does not work.
			//Reason 1: often the window on down event enters a message loop that waits for up event. But then this func cannot send the up event because it is in the loop (if it does doevents).
			//	Known workarounds:
			//	1 (applied). Don't sleepdoevents between sending down and up events.
			//	2. Let this func send the click from another thread, and sleepdoevents until that thread finishes the click.
			//Reason 2: if this func called from a click handler, OS does not send more mouse events.
			//	Known workarounds:
			//	1. (applied): show warning. Let the user modify the script: either don't click own windows or click from another thread.

			int sleep = Opt.Mouse.ClickSpeed;

			switch(button & (MButton.Down | MButton.Up | MButton.DoubleClick)) {
			case MButton.DoubleClick:
				sleep = Math.Min(sleep, Api.GetDoubleClickTime() / 4);
				//info: default double-click time is 500. Control Panel can set 200-900. API can set 1.
				//info: to detect double-click, some apps use time between down and down (that is why /4), others between up and down.

				_SendButton(button, true, p);
				if(!windowOfThisThread) _Sleep(sleep);
				_SendButton(button, false, p);
				if(!windowOfThisThread) _Sleep(sleep);
				goto case 0;
			case 0: //click
				_SendButton(button, true, p);
				if(!windowOfThisThread) _Sleep(sleep);
				_SendButton(button, false, p);
				break;
			case MButton.Down:
				_SendButton(button, true, p);
				break;
			case MButton.Up:
				_SendButton(button, false, p);
				break;
			default: throw new ArgumentException("Incompatible flags: Down, Up, DoubleClick", nameof(button));
			}
			_Sleep(sleep + Opt.Mouse.ClickSleepFinally);

			//rejected: detect click failures (UAC, BlockInput, hooks).
			//	Difficult. Cannot detect reliably. SendInput returns true.
			//	Eg when blocked by UAC, GetKeyState shows changed toggle state. Then probably hooks also called, did not test.
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button at position x y relative to window w.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="button">Button and action.</param>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="ArgumentException">Invalid button flags (multiple buttons or actions specified).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="WndException">x y is not in the window (read more in Remarks).</exception>
		/// <remarks>
		/// To move the mouse cursor, calls <see cref="Move(Wnd, Coord, Coord, bool)"/>. More info there.
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed"/>, <see cref="OptMouse.MoveSleepFinally"/> (between moving and clicking), <see cref="OptMouse.ClickSpeed"/>, <see cref="OptMouse.ClickSleepFinally"/>, <see cref="OptMouse.Relaxed"/>.
		/// If after moving the cursor it is not in the window (or a window of its thread), activates the window (or its top-level parent window). Throws exception if then x y is still not in the window. Skips all this when just releasing button or if option <b>Relaxed</b> is true. Also, if it is a control, x y can be somewhere else in its top-level parent window.
		/// </remarks>
		public static MRelease ClickEx(MButton button, Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			POINT p = Move(w, x, y, nonClient);

			//Make sure will click w, not another window.
			var action = button & (MButton.Down | MButton.Up | MButton.DoubleClick);
			if(action != MButton.Up && !Opt.Mouse.Relaxed) { //allow to release anywhere, eg it could be a drag-drop
				var wTL = w.Window;
				bool bad = !wTL.Rect.Contains(p);
				if(!bad && !_CheckWindowFromPoint()) {
					ADebug.Print("need to activate");
					//info: activating brings to the Z top and also uncloaks
					if(!wTL.IsEnabled(false)) bad = true; //probably an owned modal dialog disabled the window
					else if(wTL.ThreadId == Wnd.GetWnd.Shell.ThreadId) bad = true; //desktop
					else if(wTL.IsActive) wTL.ZorderTop(); //can be below another window in the same topmost/normal Z order, although it is rare
					else bad = !wTL.LibActivate(Wnd.Lib.ActivateFlags.NoThrowIfInvalid | Wnd.Lib.ActivateFlags.IgnoreIfNoActivateStyleEtc | Wnd.Lib.ActivateFlags.NoGetWindow);

					//rejected: if wTL is desktop, minimize windows. Scripts should not have a reason to click desktop. If need, they can minimize windows explicitly.
					//CONSIDER: activate always, because some controls don't respond when clicked while the window is inactive. But there is a risk to activate a window that does not want to be activated on click, even if we don't activate windows that have noactivate style. Probably better let the script author insert Activate before Click when need.
					//CONSIDER: what if the window is hung?

					if(!bad) bad = !_CheckWindowFromPoint();
				}
				if(bad) throw new WndException(wTL, "Cannot click. The point is not in the window");

				bool _CheckWindowFromPoint()
				{
					var wfp = Wnd.FromXY(p, WXYFlags.NeedWindow);
					if(wfp == wTL) return true;
					//forgive if same thread and no caption. Eg a tooltip that disappears and relays the click to its owner window. But not if wTL is disabled.
					if(wTL.IsEnabled(false) && wfp.ThreadId == wTL.ThreadId && !wfp.HasStyle(WS.CAPTION)) return true;
					return false;
				}
			}

			_Click(button, p, w);
			return button;
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button at the specified position in screen.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="button">Button and action.</param>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <exception cref="ArgumentException">Invalid button flags (multiple buttons or actions specified).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		/// <remarks>
		/// To move the mouse cursor, calls <see cref="Move(Coord, Coord)"/>.
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed"/>, <see cref="OptMouse.MoveSleepFinally"/> (between moving and clicking), <see cref="OptMouse.ClickSpeed"/>, <see cref="OptMouse.ClickSleepFinally"/>, <see cref="OptMouse.Relaxed"/>.
		/// </remarks>
		public static MRelease ClickEx(MButton button, Coord x, Coord y)
		{
			POINT p = Move(x, y);
			_Click(button, p);
			return button;
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button at the specified position in screen.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="button">Button and action.</param>
		/// <param name="p">Coordinates.
		/// <note type="tip">When need coordinates relative to a non-primary screen or/and the work area, use <see cref="Coord.Normalize"/> or tuple (x, y, workArea) etc. Example: <c>Mouse.ClickEx(MButton.Right, (x, y, true));</c>.</note>
		/// </param>
		/// <exception cref="ArgumentException">Invalid button flags (multiple buttons or actions specified).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static MRelease ClickEx(MButton button, POINT p)
		{
			Move(p);
			_Click(button, p);
			return button;
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button.
		/// By default does not move the mouse cursor.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="button">Button and action. Default: left click.</param>
		/// <param name="useLastXY">
		/// Use <see cref="LastXY"/>. It is the mouse cursor position set by the most recent 'mouse move' or 'mouse click' function called in this thread. Use this option for reliability.
		/// Example: <c>Mouse.Move(100, 100); Mouse.ClickEx(..., true);</c>. The click is always at 100 100, even if somebody changes cursor position between <c>Mouse.Move</c> sets it and <c>Mouse.ClickEx</c> uses it. In such case this option atomically moves the cursor to <b>LastXY</b>. This movement is instant and does not use <see cref="Opt"/>.
		/// If false (default), clicks at the current cursor position (does not move it).
		/// </param>
		/// <exception cref="ArgumentException">Invalid button flags (multiple buttons or actions specified).</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.ClickSpeed"/>, <see cref="OptMouse.ClickSleepFinally"/>.
		/// </remarks>
		public static MRelease ClickEx(MButton button = MButton.Left, bool useLastXY = false)
		{
			POINT p;
			if(useLastXY) p = LastXY;
			else {
				p = XY;
				if(t_prevMousePos == null) t_prevMousePos = new _PrevMousePos(p); //sets .first=.last=p
				else t_prevMousePos.last = p;
			}
			_Click(button, p);
			return button;
			//CONSIDER: Opt.Mouse.ClickAtLastXY
		}

		/// <summary>
		/// Left button click at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static void Click(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Left, w, x, y, nonClient);
		}

		/// <summary>
		/// Left button click at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static void Click(Coord x, Coord y)
		{
			//note: most Click functions don't have a workArea and screen parameter. It is rarely used. For reliability better use the overloads that use window coordinates.

			ClickEx(MButton.Left, x, y);
		}

		/// <summary>
		/// Left button click.
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastXY">Use <see cref="LastXY"/>, not current cursor position.</param>
		public static void Click(bool useLastXY = false)
		{
			ClickEx(MButton.Left, useLastXY);
		}

		/// <summary>
		/// Right button click at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static void RightClick(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Right, w, x, y, nonClient);
		}

		/// <summary>
		/// Right button click at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static void RightClick(Coord x, Coord y)
		{
			ClickEx(MButton.Right, x, y);
		}

		/// <summary>
		/// Right button click.
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastXY">Use <see cref="LastXY"/>, not current cursor position.</param>
		public static void RightClick(bool useLastXY = false)
		{
			ClickEx(MButton.Right, useLastXY);
		}

		/// <summary>
		/// Left button double click at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static void DoubleClick(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Left | MButton.DoubleClick, w, x, y, nonClient);
		}

		/// <summary>
		/// Left button double click at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static void DoubleClick(Coord x, Coord y)
		{
			ClickEx(MButton.Left | MButton.DoubleClick, x, y);
		}

		/// <summary>
		/// Left button double click.
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastXY">Use <see cref="LastXY"/>, not current cursor position.</param>
		public static void DoubleClick(bool useLastXY = false)
		{
			ClickEx(MButton.Left | MButton.DoubleClick, useLastXY);
		}

		/// <summary>
		/// Left down (press and don't release) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static MRelease LeftDown(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			return ClickEx(MButton.Left | MButton.Down, w, x, y, nonClient);
		}

		/// <summary>
		/// Left button down (press and don't release) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord)"/>. More info there.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static MRelease LeftDown(Coord x, Coord y)
		{
			return ClickEx(MButton.Left | MButton.Down, x, y);
		}

		/// <summary>
		/// Left button down (press and don't release).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="useLastXY">Use <see cref="LastXY"/>, not current cursor position.</param>
		public static MRelease LeftDown(bool useLastXY = false)
		{
			return ClickEx(MButton.Left | MButton.Down, useLastXY);
		}

		/// <summary>
		/// Left button up (release pressed button) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		public static void LeftUp(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Left | MButton.Up, w, x, y, nonClient);
		}

		/// <summary>
		/// Left button up (release pressed button) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static void LeftUp(Coord x, Coord y)
		{
			ClickEx(MButton.Left | MButton.Up, x, y);
		}

		/// <summary>
		/// Left button up (release pressed button).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastXY">Use <see cref="LastXY"/>, not current cursor position.</param>
		public static void LeftUp(bool useLastXY = false)
		{
			ClickEx(MButton.Left | MButton.Up, useLastXY);
		}

		/// <summary>
		/// Right button down (press and don't release) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static MRelease RightDown(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			return ClickEx(MButton.Right | MButton.Down, w, x, y, nonClient);
		}

		/// <summary>
		/// Right button down (press and don't release) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord)"/>. More info there.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static MRelease RightDown(Coord x, Coord y)
		{
			return ClickEx(MButton.Right | MButton.Down, x, y);
		}

		/// <summary>
		/// Right button down (press and don't release).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <returns>The return value can be used to auto-release the pressed button. Example: <see cref="MRelease"/>.</returns>
		/// <param name="useLastXY">Use <see cref="LastXY"/>, not current cursor position.</param>
		public static MRelease RightDown(bool useLastXY = false)
		{
			return ClickEx(MButton.Right | MButton.Down, useLastXY);
		}

		/// <summary>
		/// Right button up (release pressed button) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		public static void RightUp(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Right | MButton.Up, w, x, y, nonClient);
		}

		/// <summary>
		/// Right button up (release pressed button) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord)"/>.</exception>
		public static void RightUp(Coord x, Coord y)
		{
			ClickEx(MButton.Right | MButton.Up, x, y);
		}

		/// <summary>
		/// Right button up (release pressed button).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastXY">Use <see cref="LastXY"/>, not current cursor position.</param>
		public static void RightUp(bool useLastXY = false)
		{
			ClickEx(MButton.Right | MButton.Up, useLastXY);
		}

		/// <summary>
		/// Mouse wheel forward or backward.
		/// </summary>
		/// <param name="ticks">Number of wheel ticks forward (positive) or backward (negative).</param>
		/// <param name="horizontal">Horizontal wheel.</param>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.ClickSleepFinally"/>.
		/// </remarks>
		public static void Wheel(int ticks, bool horizontal = false)
		{
			_SendRaw(horizontal ? Api.IMFlags.HWheel : Api.IMFlags.Wheel, 0, 0, ticks);
			_Sleep(Opt.Mouse.ClickSleepFinally);
		}

		//rejected: unclear etc. Instead let use Mouse.LeftUp(true) or using(Mouse.LeftDown(...).
		///// <summary>
		///// Releases mouse buttons pressed by this thread.
		///// </summary>
		///// <param name="useLastXY">If true (default), ensures that the cursor is at <see cref="LastXY"/>, not at <see cref="XY"/> which may be different if eg the user or she's cat is moving the mouse.</param>
		///// <example>
		///// <code><![CDATA[
		///// var w = Wnd.Find("* Notepad");
		///// Mouse.LeftDown(w, 8, 8);
		///// Mouse.MoveRelative(20, 0);
		///// Mouse.Drop();
		///// ]]></code>
		///// </example>
		//public static void Drop(bool useLastXY=true)
		//{
		//	if(useLastXY) _ReleaseButtons(LastXY);
		//	else _ReleaseButtons();
		//}

		//not used
		///// <summary>
		///// Releases mouse buttons pressed by this thread (t_pressedButtons).
		///// </summary>
		///// <param name="p">If not null, and XY is different, moves to this point. Used for reliability.</param>
		//static void _ReleaseButtons(POINT? p = null)
		//{
		//	var b = t_pressedButtons;
		//	if(0 != (b & MButtons.Left)) _Click(MButton.Left | MButton.Up, p);
		//	if(0 != (b & MButtons.Right)) _Click(MButton.Right | MButton.Up, p);
		//	if(0 != (b & MButtons.Middle)) _Click(MButton.Middle | MButton.Up, p);
		//	if(0 != (b & MButtons.X1)) _Click(MButton.X1 | MButton.Up, p);
		//	if(0 != (b & MButtons.X2)) _Click(MButton.X2 | MButton.Up, p);
		//}
		//rejected: finally release script-pressed buttons, especially on exception. Instead let use code: using(Mouse.LeftDown(...)), it auto-releases pressed button.

		/// <summary>
		/// Returns true if some mouse buttons are down (pressed).
		/// </summary>
		/// <param name="buttons">Return true if some of these buttons are down. Default: any (Left, Right, Middle, X1 or X2).</param>
		/// <remarks>
		/// Uses API <msdn>GetAsyncKeyState</msdn>.
		/// When processing user input in UI code (forms, WPF), instead use class <see cref="Keyb.UI"/> or .NET functions. They use API <msdn>GetKeyState</msdn>.
		/// When mouse left and right buttons are swapped, gets logical state, not physical.
		/// </remarks>
		/// <seealso cref="WaitForNoButtonsPressed"/>
		public static bool IsPressed(MButtons buttons = MButtons.Left | MButtons.Right | MButtons.Middle | MButtons.X1 | MButtons.X2)
		{
			if(0 != (buttons & MButtons.Left) && Keyb.IsPressed(KKey.MouseLeft)) return true;
			if(0 != (buttons & MButtons.Right) && Keyb.IsPressed(KKey.MouseRight)) return true;
			if(0 != (buttons & MButtons.Middle) && Keyb.IsPressed(KKey.MouseMiddle)) return true;
			if(0 != (buttons & MButtons.X1) && Keyb.IsPressed(KKey.MouseX1)) return true;
			if(0 != (buttons & MButtons.X2) && Keyb.IsPressed(KKey.MouseX2)) return true;
			return false;
		}

		//rejected: not useful.
		///// <summary>
		///// Returns a value indicating which mouse buttons are down (pressed).
		///// </summary>
		///// <param name="buttons">Check only these buttons. Default: Left, Right, Middle, X1, X2.</param>
		///// <remarks>See <see cref="IsPressed"/>.</remarks>
		//public static MButtons Buttons(MButtons buttons = MButtons.Left | MButtons.Right | MButtons.Middle | MButtons.X1 | MButtons.X2)
		//{
		//	MButtons R = 0;
		//	if(0 != (buttons & MButtons.Left) && Keyb.IsKey(KKey.MouseLeft)) R |= MButtons.Left;
		//	if(0 != (buttons & MButtons.Right) && Keyb.IsKey(KKey.MouseRight)) R |= MButtons.Right;
		//	if(0 != (buttons & MButtons.Middle) && Keyb.IsKey(KKey.MouseMiddle)) R |= MButtons.Middle;
		//	if(0 != (buttons & MButtons.X1) && Keyb.IsKey(KKey.MouseX1)) return R |= MButtons.X1;
		//	if(0 != (buttons & MButtons.X2) && Keyb.IsKey(KKey.MouseX2)) return R |= MButtons.X2;
		//	return R;
		//}

		//rejected: rarely used. Can use IsPressed.
		///// <summary>
		///// Returns true if the left mouse button is down (pressed).
		///// </summary>
		///// <remarks>See <see cref="IsPressed"/>.</remarks>
		//public static bool IsLeft => Keyb.IsPressed(KKey.MouseLeft);

		///// <summary>
		///// Returns true if the right mouse button is down (pressed).
		///// </summary>
		///// <remarks>See <see cref="IsPressed"/>.</remarks>
		//public static bool IsRight => Keyb.IsPressed(KKey.MouseRight);

		/// <summary>
		/// Waits while some mouse buttons are down (pressed). See <see cref="IsPressed"/>.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="buttons">Wait only for these buttons. Default - all.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <seealso cref="Keyb.WaitForNoModifierKeysAndMouseButtons"/>
		public static bool WaitForNoButtonsPressed(double secondsTimeout = 0.0, MButtons buttons = MButtons.Left | MButtons.Right | MButtons.Middle | MButtons.X1 | MButtons.X2)
		{
			return Keyb.WaitForNoModifierKeysAndMouseButtons(secondsTimeout, 0, buttons);
		}

		/// <summary>
		/// Waits while some buttons are down (pressed), except those pressed by a <see cref="Mouse"/> class function in this thread.
		/// Does nothing option <b>Relaxed</b> is true.
		/// </summary>
		internal static void LibWaitForNoButtonsPressed()
		{
			//not public, because we have WaitForNoButtonsPressed, which is unaware about script-pressed buttons, and don't need this awareness because the script author knows what is pressed by that script

			if(Opt.Mouse.Relaxed) return;
			var mb = (MButtons.Left | MButtons.Right | MButtons.Middle | MButtons.X1 | MButtons.X2)
				& ~t_pressedButtons;
			if(WaitForNoButtonsPressed(-5, mb)) return;
			PrintWarning("Info: Waiting for releasing mouse buttons. See Opt.Mouse.Relaxed.");
			WaitForNoButtonsPressed(0, mb);
		}

		/// <summary>
		/// Waits for button-down or button-up event of the specified mouse button or buttons.
		/// </summary>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="button">Mouse button. If several buttons specified, waits for any of them.</param>
		/// <param name="up">Wait for button-up event.</param>
		/// <param name="block">Make the event invisible for other apps. If <i>up</i> is true, makes the down event invisible too, if it comes while waiting for the up event.</param>
		/// <exception cref="ArgumentException"><i>button</i> is 0.</exception>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <remarks>
		/// Unlike <see cref="WaitForNoButtonsPressed"/>, waits for down or up event, not for button state.
		/// Uses low-level mouse hook.
		/// Ignores mouse events injected by functions of this library.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// Mouse.WaitForClick(0, MButtons.Left, up: true, block: false);
		/// Print("click");
		/// ]]></code>
		/// </example>
		public static bool WaitForClick(double secondsTimeout, MButtons button, bool up = false, bool block = false)
		{
			if(button == 0) throw new ArgumentException();
			return 0 != _WaitForClick(secondsTimeout, button, up, block);
		}

		/// <summary>
		/// Waits for button-down or button-up event of any mouse button, and gets the button code.
		/// </summary>
		/// <returns>Returns the button code. On timeout returns 0 if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <param name="secondsTimeout"></param>
		/// <param name="up"></param>
		/// <param name="block"></param>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		/// <example>
		/// <code><![CDATA[
		/// var button = Mouse.WaitForClick(0, up: true, block: true);
		/// Print(button);
		/// ]]></code>
		/// </example>
		public static MButtons WaitForClick(double secondsTimeout, bool up = false, bool block = false)
		{
			return _WaitForClick(secondsTimeout, 0, up, block);
		}

		static MButtons _WaitForClick(double secondsTimeout, MButtons button, bool up, bool block)
		{
			//info: this and related functions use similar code as Keyb._WaitForKey.

			MButtons R = 0;
			using(WinHook.Mouse(x => {
				MButtons b = 0;
				switch(x.Event) {
				case HookData.MouseEvent.LeftButton: b = MButtons.Left; break;
				case HookData.MouseEvent.RightButton: b = MButtons.Right; break;
				case HookData.MouseEvent.MiddleButton: b = MButtons.Middle; break;
				case HookData.MouseEvent.X1Button: b = MButtons.X1; break;
				case HookData.MouseEvent.X2Button: b = MButtons.X2; break;
				}
				if(b == 0) return;
				if(button != 0 && !button.Has(b)) return;
				if(x.IsButtonUp != up) {
					if(up && block) { //button down when we are waiting for up. If block, now block down too.
						if(button == 0) button = b;
						x.BlockEvent();
					}
					return;
				}
				R = b;
				if(block) x.BlockEvent();
			})) WaitFor.MessagesAndCondition(secondsTimeout, () => R != 0);

			return R;
		}
		//CONSIDER: WaitForMouseMove, WaitForMouseStop. In QM2 these functions were created because somebody asked, but I don't use.

		/// <summary>
		/// Waits for a standard mouse cursor (pointer) visible.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="cursor">Id of a standard cursor.</param>
		/// <param name="not">Wait until this cursor disappears.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		public static bool WaitForCursor(double secondsTimeout, MCursor cursor, bool not = false)
		{
			IntPtr hcur = Api.LoadCursor(default, cursor);
			if(hcur == default) throw new AException(0, "*load cursor");

			return WaitFor.Condition(secondsTimeout, () => (ACursor.GetCurrentCursor(out var h) && h == hcur) ^ not);
		}

		/// <summary>
		/// Waits for a nonstandard mouse cursor (pointer) visible.
		/// </summary>
		/// <param name="secondsTimeout">Timeout, seconds. Can be 0 (infinite), &gt;0 (exception) or &lt;0 (no exception). More info: [](xref:wait_timeout).</param>
		/// <param name="cursorHash">Cursor hash, as returned by <see cref="ACursor.HashCursor"/>.</param>
		/// <param name="not">Wait until this cursor disappears.</param>
		/// <returns>Returns true. On timeout returns false if <i>secondsTimeout</i> is negative; else exception.</returns>
		/// <exception cref="TimeoutException"><i>secondsTimeout</i> time has expired (if &gt; 0).</exception>
		public static bool WaitForCursor(double secondsTimeout, long cursorHash, bool not = false)
		{
			if(cursorHash == 0) throw new ArgumentException();
			return WaitFor.Condition(secondsTimeout, () => (ACursor.GetCurrentCursor(out var h) && ACursor.HashCursor(h) == cursorHash) ^ not);
		}
	}

#if unfinished
	public static partial class Mouse
	{
		/// <summary>
		/// Sends a mouse click or wheel message directly to window or control.
		/// It often works even when the window is inactive and/or the mouse cursor is anywhere. However it often does not work altogether.
		/// </summary>
		public static class Message
		{
			//not useful
			//public static void Move(Wnd w, Coord x=default, Coord y=default, bool nonClient = false, int waitMS=0)
			//{

			//}

			/// <summary>
			/// Sends a mouse click, double-click, down or up message(s) directly to window or control.
			/// </summary>
			/// <param name="w">Window or control.</param>
			/// <param name="x">X coordinate relative to the client area, to send with the message.</param>
			/// <param name="y">Y coordinate relative to the client area, to send with the message.</param>
			/// <param name="button"></param>
			/// <param name="nonClient">x y are relative to the window rectangle. By default they are relative to the client area.</param>
			/// <exception cref="AException">Failed.</exception>
			/// <remarks>
			/// Does not move the mouse cursor, therefore does not work if the window gets cursor position not from the message.
			/// Does not activate the window (unless the window activates itself).
			/// </remarks>
			public static void Click(Wnd w, Coord x=default, Coord y=default, MButton button = MButton.Left, bool nonClient = false, bool isCtrl=false, bool isShift=false/*, int waitMS = 0*/)
			{

			}

			/// <summary>
			/// Sends a mouse wheel message (WM_MOUSEWHEEL) directly to window or control.
			/// </summary>
			/// <param name="w">Window or control.</param>
			/// <param name="ticks">Number of wheel ticks forward (positive) or backward (negative).</param>
			public static void Wheel(Wnd w, int ticks, bool isCtrl=false, bool isShift=false/*, int waitMS = 0*/)
			{

			}

			static void _Send(Wnd w, uint message, uint wParam, uint lParam, bool isCtrl, bool isShift)
			{
				if(isCtrl || Keyb.IsCtrl) wParam |= 8; //Api.MK_CONTROL
				if(isShift || Keyb.IsShift) wParam |= 4; //Api.MK_SHIFT
				if(!w.Post(message, wParam, lParam)) throw new AException(0);
				_SleepMax(-1, w.IsOfThisThread);
			}

			//rejected:
			///// <param name="waitMS">
			///// Maximal time to wait, milliseconds. Also which API to use.
			///// If 0 (default), calls API <msdn>PostMessage</msdn> (it does not wait) and waits Opt.Mouse.<see cref="OptMouse.ClickSpeed"/> ms.
			///// If less than 0 (eg Timeout.Infinite), calls API <msdn>SendMessage</msdn> which usually waits until the window finishes to process the message.
			///// Else calls API <msdn>SendMessageTimeout</msdn> which waits max waitMS milliseconds, then throws AException.
			///// The SendX functions are not natural and less likely to work.
			///// If the window shows a dialog, the SendX functions usually wait until the dialog is closed.
			///// </param>
			///// <exception cref="AException">Failed, or timeout.</exception>
			//static void _SendOrPost(int waitMS, Wnd w, uint message, LPARAM wParam, LPARAM lParam)
			//{
			//	bool ok;
			//	if(message == 0) {
			//		ok=w.Post(message, wParam, lParam);
			//		_Sleep(Opt.Mouse.ClickSpeed);
			//	} else if(waitMS < 0) {
			//		w.Send(message, wParam, lParam);
			//		ok = true;
			//	} else {
			//		ok = w.SendTimeout(waitMS, message, wParam, lParam);
			//	}
			//	if(!ok) throw new AException(0);
			//}
		}
	}
#endif

	public static partial class ExtAu
	{
		#region Wnd

		/// <summary>
		/// Moves the cursor (mouse pointer) to the position x y relative to this window.
		/// Calls <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.
		/// </summary>
		/// <exception cref="NotFoundException">Window not found (this variable is 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.</exception>
		public static void MouseMove(this Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
			=> Mouse.Move(w.OrThrow(), x, y, nonClient);

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button at position x y relative to this window.
		/// Calls <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.
		/// </summary>
		/// <exception cref="NotFoundException">Window not found (this variable is 0).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static MRelease MouseClick(this Wnd w, Coord x = default, Coord y = default, MButton button = MButton.Left, bool nonClient = false)
			=> Mouse.ClickEx(button, w.OrThrow(), x, y, nonClient);

		#endregion

		#region Acc

		/// <summary>
		/// Moves the cursor (mouse pointer) to this accessible object.
		/// Calls <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in the bounding rectangle of this object. Default - center.</param>
		/// <param name="y">Y coordinate in the bounding rectangle of this object. Default - center.</param>
		/// <exception cref="NotFoundException">Accessible object not found (this variable is null).</exception>
		/// <exception cref="AException">Failed to get object rectangle (<see cref="Acc.GetRect(out RECT, Wnd)"/>) or container window (<see cref="Acc.WndContainer"/>).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.</exception>
		public static void MouseMove(this Acc t, Coord x = default, Coord y = default)
			=> _AccMouseAction(t.OrThrow(), false, x, y, default);

		/// <summary>
		/// Clicks this accessible object.
		/// Calls <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in the bounding rectangle of this object. Default - center.</param>
		/// <param name="y">Y coordinate in the bounding rectangle of this object. Default - center.</param>
		/// <param name="button">Which button and how to use it.</param>
		/// <exception cref="NotFoundException">Accessible object not found (this variable is null).</exception>
		/// <exception cref="AException">Failed to get object rectangle (<see cref="Acc.GetRect(out RECT, Wnd)"/>) or container window (<see cref="Acc.WndContainer"/>).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static MRelease MouseClick(this Acc t, Coord x = default, Coord y = default, MButton button = MButton.Left)
		{
			_AccMouseAction(t.OrThrow(), true, x, y, button);
			return button;
		}

		static void _AccMouseAction(Acc t, bool click, Coord x, Coord y, MButton button)
		{
			var w = t.WndContainer; //info: not necessary, but with window the mouse functions are more reliable, eg will not click another window if it is over our window

			//never mind: if w.Is0 and the action is 'click', get AO from point and fail if it is not t. But need to compare AO properties. Rare.

			if(!(w.Is0 ? t.GetRect(out RECT r) : t.GetRect(out r, w))) throw new AException(0, "*get rectangle");
			var p = Coord.NormalizeInRect(x, y, r, centerIfEmpty: true);
			if(w.Is0) {
				if(button == 0) Mouse.Move(p);
				else Mouse.ClickEx(button, p);
			} else {
				if(button == 0) Mouse.Move(w, p.x, p.y);
				else Mouse.ClickEx(button, w, p.x, p.y);
			}
		}

		#endregion

		#region WinImage

		/// <summary>
		/// Moves the mouse to the found image.
		/// Calls <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in the found image. Default - center.</param>
		/// <param name="y">Y coordinate in the found image. Default - center.</param>
		/// <exception cref="NotFoundException">Image not found (this variable is null).</exception>
		/// <exception cref="InvalidOperationException">area is Bitmap.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.</exception>
		public static void MouseMove(this WinImage t, Coord x = default, Coord y = default)
			=> t.OrThrow().LibMouseAction(0, x, y);

		/// <summary>
		/// Clicks the found image.
		/// Calls <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in the found image. Default - center.</param>
		/// <param name="y">Y coordinate in the found image. Default - center.</param>
		/// <param name="button">Which button and how to use it.</param>
		/// <exception cref="NotFoundException">Image not found (this variable is null).</exception>
		/// <exception cref="InvalidOperationException">area is Bitmap.</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static MRelease MouseClick(this WinImage t, Coord x = default, Coord y = default, MButton button = MButton.Left)
		{
			t.OrThrow().LibMouseAction(button == 0 ? MButton.Left : button, x, y);
			return button;
		}

		#endregion

	}
}

namespace Au.Types
{
	/// <summary>
	/// <i>button</i> parameter type for <see cref="Mouse.ClickEx(MButton, bool)"/> and similar functions.
	/// </summary>
	/// <remarks>
	/// There are two groups of values:
	/// 1. Button (Left, Right, Middle, X1, X2). Default or 0: Left.
	/// 2. Action (Down, Up, DoubleClick). Default: click.
	/// 
	/// Multiple values from the same group cannot be combined. For example Left|Right is invalid.
	/// Values from different groups can be combined. For example Right|Down.
	/// </remarks>
	[Flags]
	public enum MButton
	{
		/// <summary>The left button.</summary>
		Left = 1,

		/// <summary>The right button.</summary>
		Right = 2,

		/// <summary>The middle button.</summary>
		Middle = 4,

		/// <summary>The 4-th button.</summary>
		X1 = 8,

		/// <summary>The 5-th button.</summary>
		X2 = 16,

		//rejected: not necessary. Can be confusing.
		///// <summary>
		///// Click (press and release).
		///// This is default. Value 0.
		///// </summary>
		//Click = 0,

		/// <summary>(flag) Press and don't release.</summary>
		Down = 32,

		/// <summary>(flag) Don't press, only release.</summary>
		Up = 64,

		/// <summary>(flag) Double-click.</summary>
		DoubleClick = 128,
	}

	/// <summary>
	/// Flags for mouse buttons.
	/// Used with functions that check mouse button states (down or up).
	/// </summary>
	/// <remarks>
	/// The values are the same as <see cref="System.Windows.Forms.MouseButtons"/>, therefore can be cast to/from.
	/// </remarks>
	[Flags]
	public enum MButtons
	{
		/// <summary>The left button.</summary>
		Left = 0x00100000,

		/// <summary>The right button.</summary>
		Right = 0x00200000,

		/// <summary>The middle button.</summary>
		Middle = 0x00400000,

		/// <summary>The 4-th button.</summary>
		X1 = 0x00800000,

		/// <summary>The 5-th button.</summary>
		X2 = 0x01000000,
	}

	/// <summary>
	/// The <b>Dispose</b> function releases mouse buttons pressed by the function that returned this variable.
	/// </summary>
	/// <example>
	/// Drag and drop: start at x=8 y=8, move 20 pixels down, drop.
	/// <code><![CDATA[
	/// using(Mouse.LeftDown(w, 8, 8)) Mouse.MoveRelative(0, 20); //the button is auto-released when the 'using' code block ends
	/// ]]></code>
	/// </example>
	public struct MRelease : IDisposable
	{
		MButton _buttons;
		///
		public static implicit operator MRelease(MButton b) => new MRelease() { _buttons = b };

		/// <summary>
		/// Releases mouse buttons pressed by the function that returned this variable.
		/// </summary>
		public void Dispose()
		{
			if(0 == (_buttons & MButton.Down)) return;
			if(0 != (_buttons & MButton.Left)) Mouse.ClickEx(MButton.Left | MButton.Up, true);
			if(0 != (_buttons & MButton.Right)) Mouse.ClickEx(MButton.Right | MButton.Up, true);
			if(0 != (_buttons & MButton.Middle)) Mouse.ClickEx(MButton.Middle | MButton.Up, true);
			if(0 != (_buttons & MButton.X1)) Mouse.ClickEx(MButton.X1 | MButton.Up, true);
			if(0 != (_buttons & MButton.X2)) Mouse.ClickEx(MButton.X2 | MButton.Up, true);
		}
	}

	/// <summary>
	/// Standard cursor ids.
	/// Used with <see cref="Mouse.WaitForCursor(double, MCursor, bool)"/>.
	/// </summary>
	public enum MCursor
	{
		/// <summary>Standard arrow.</summary>
		Arrow = 32512,

		/// <summary>I-beam (text editing).</summary>
		IBeam = 32513,

		/// <summary>Hourglass.</summary>
		Wait = 32514,

		/// <summary>Crosshair.</summary>
		Cross = 32515,

		/// <summary>Vertical arrow.</summary>
		UpArrow = 32516,

		/// <summary>Double-pointed arrow pointing northwest and southeast.</summary>
		SizeNWSE = 32642,

		/// <summary>Double-pointed arrow pointing northeast and southwest.</summary>
		SizeNESW = 32643,

		/// <summary>Double-pointed arrow pointing west and east.</summary>
		SizeWE = 32644,

		/// <summary>Double-pointed arrow pointing north and south.</summary>
		SizeNS = 32645,

		/// <summary>Four-pointed arrow pointing north, south, east, and west.</summary>
		SizeAll = 32646,

		/// <summary>Slashed circle.</summary>
		No = 32648,

		/// <summary>Hand.</summary>
		Hand = 32649,

		/// <summary>Standard arrow and small hourglass.</summary>
		AppStarting = 32650,

		/// <summary>Arrow and question mark.</summary>
		Help = 32651,
	}
}
