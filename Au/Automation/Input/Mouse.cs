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
using System.Windows.Forms;
using System.Drawing;
//using System.Linq;

using Au.Types;
using static Au.NoClass;

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
		public static Point XY { get { Api.GetCursorPos(out var p); return p; } }

		/// <summary>
		/// Gets cursor (mouse pointer) X coordinate (Mouse.XY.X).
		/// </summary>
		public static int X => XY.X;

		/// <summary>
		/// Gets cursor (mouse pointer) Y coordinate (Mouse.XY.Y).
		/// </summary>
		public static int Y => XY.Y;

		static void _Move(Point p, bool fast)
		{
			bool relaxed = Opt.Mouse.Relaxed, willFail = false;

			if(!Screen_.IsInAnyScreen(p)) {
				if(!relaxed) throw new ArgumentOutOfRangeException(null, "Cannot mouse-move. This x y is not in screen. " + p.ToString());
				willFail = true;
			}

			if(!fast) _MoveSlowTo(p);

			Point p0 = XY;
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
				Wnd.WndActive.LibUacCheckAndThrow(es + ". The active"); //it's a mystery for users. API SendInput fails even if the point is not in the window.
																		//rejected: Wnd.Misc.WndRoot.ActivateLL()
				throw new AuException(es);
				//known reasons:
				//	Active window of higher UAC IL.
				//	BlockInput, hook, some script etc that blocks mouse movement or restores mouse position.
				//	ClipCursor.
			}
			t_prevMousePos.last = p;

			_Sleep(Opt.Mouse.MoveSleepFinally);
		}

		static void _MoveSlowTo(Point p)
		{
			int speed = Opt.Mouse.MoveSpeed;
			if(speed == 0 && IsPressed()) speed = 1; //make one intermediate movement, because some programs (older MSDEV) don't select text otherwise
			if(speed > 0) {
				var p0 = XY;
				int dxall = p.X - p0.X, dyall = p.Y - p0.Y;
				double dist = Math.Sqrt(dxall * dxall + dyall * dyall);
				if(dist > 1.5) {
					double dtall = Math.Sqrt(dist) * speed + 9;
					long t0 = Time.Milliseconds - 7, dt = 7;
					int pdx = 0, pdy = 0;
					for(; ; dt = Time.Milliseconds - t0) {
						double dtfr = dt / dtall;
						if(dtfr >= 1) break;

						int dx = (int)(dtfr * dxall), dy = (int)(dtfr * dyall);
						//Print(dx, dy, dtfr);

						var pt = new Point(p0.X + dx, p0.Y + dy);
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
		/// Moves the cursor (mouse pointer) to the specified position in screen.
		/// </summary>
		/// <param name="p">Coordinates in screen.</param>
		/// <exception cref="ArgumentOutOfRangeException">The specified x y is not in screen. No exception if the <b>Relaxed</b> option is true (then moves to a screen edge).</exception>
		/// <exception cref="AuException">Failed to move the cursor to the specified x y. Some reasons: 1. Another thread blocks or modifies mouse input (API BlockInput, mouse hooks, frequent API SendInput etc); 2. The active window belongs to a process of higher <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level; 3. Some application called API ClipCursor. No exception if the <b>Relaxed</b> option is true (then final cursor position is undefined).</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed" r=""/>, <see cref="OptMouse.MoveSleepFinally" r=""/>, <see cref="OptMouse.Relaxed" r=""/>.
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
		public static void Move(Point p)
		{
			LibWaitWhileButtonsPressed();
			_Move(p, fast: false);
		}

		/// <summary>
		/// Moves the cursor (mouse pointer) to the position x y in screen.
		/// Returns the final cursor position in primary screen coordinates.
		/// </summary>
		/// <param name="x">X coordinate relative to screen.</param>
		/// <param name="y">Y coordinate relative to screen.</param>
		/// <param name="co">Can be used to specify screen (see <see cref="Screen_.FromObject"/>) and/or whether x y are relative to the work area.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 1. The specified x y is not in screen (any screen). No exception if the <b>Relaxed</b> option is true (then moves to a screen edge).
		/// 2. Invalid screen index.
		/// </exception>
		/// <exception cref="AuException"><inheritdoc cref="Move(Point)"/></exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed" r=""/>, <see cref="OptMouse.MoveSleepFinally" r=""/>, <see cref="OptMouse.Relaxed" r=""/>.
		/// </remarks>
		public static Point Move(Coord x, Coord y, CoordOptions co = null)
		{
			LibWaitWhileButtonsPressed();
			var p = Coord.Normalize(x, y, co);
			_Move(p, fast: false);
			return p;
		}

		/// <summary>
		/// Moves the cursor (mouse pointer) to the position x y relative to window w.
		/// Returns the final cursor position in primary screen coordinates.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="WndException">
		/// Invalid window.
		/// The top-level window is hidden. No exception if just cloaked, for example in another desktop; then on click will activate, which usually uncloaks.
		/// Other window-related failures.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">The specified x y is not in screen. No exception if the <b>Relaxed</b> option is true (then moves to a screen edge).</exception>
		/// <exception cref="AuException"><inheritdoc cref="Move(Point)"/></exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed" r=""/>, <see cref="OptMouse.MoveSleepFinally" r=""/>, <see cref="OptMouse.Relaxed" r=""/>.
		/// </remarks>
		public static Point Move(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			LibWaitWhileButtonsPressed();
			w.ThrowIfInvalid();
			var wTL = w.WndWindow;
			if(!wTL.IsVisibleEx) throw new WndException(wTL, "Cannot mouse-move. The window is invisible"); //should make visible? Probably not.
			if(wTL.IsMinimized) { wTL.ShowNotMinimized(true); _Sleep(500); } //never mind: if w is a control...
			var p = Coord.NormalizeInWindow(x, y, w, nonClient, centerIfEmpty: true);
			if(!w.MapClientToScreen(ref p)) w.ThrowUseNative();
			_Move(p, fast: false);
			return p;
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
		/// Moves the mouse cursor where it was at the time of the last <see cref="Save"/> call in this thread. If it was not called - of the first 'mouse move' or 'mouse move and click' function call in this thread. Does nothing if these functions were not called.
		/// </summary>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSleepFinally" r=""/>, <see cref="OptMouse.Relaxed" r=""/>.
		/// </remarks>
		public static void Restore()
		{
			if(t_prevMousePos == null) return;
			LibWaitWhileButtonsPressed();
			_Move(t_prevMousePos.first, fast: true);
		}

		class _PrevMousePos
		{
			public Point first, last;
			public _PrevMousePos() { first = last = XY; }
		}
		[ThreadStatic] static _PrevMousePos t_prevMousePos;

		/// <summary>
		/// Mouse cursor position of the most recent successful 'mouse move' or 'mouse move and click' function call in this thread.
		/// If such functions are still not called in this thread, returns <see cref="XY"/>.
		/// </summary>
		public static Point LastMoveXY { get { var v = t_prevMousePos; return (v != null) ? v.last : XY; } }

		//rejected. MoveRelative usually is better. If need, can use code: Move(Mouse.X+dx, Mouse.Y+dy).
		//public static void MoveFromCurrent(int dx, int dy)
		//{
		//	var p = XY;
		//	p.Offset(dx, dy);
		//	Move(p);
		//}

		/// <summary>
		/// Moves the cursor (mouse pointer) relative to <see cref="LastMoveXY"/>.
		/// Returns the final cursor position in primary screen coordinates.
		/// </summary>
		/// <param name="dx">X offset from LastMoveXY.X.</param>
		/// <param name="dy">Y offset from LastMoveXY.Y.</param>
		/// <exception cref="ArgumentOutOfRangeException">The calculated x y is not in screen. No exception if the <b>Relaxed</b> option is true (then moves to a screen edge).</exception>
		/// <exception cref="AuException">Failed to move the cursor to the calculated x y. Some reasons: 1. Another thread blocks or modifies mouse input (API BlockInput, mouse hooks, frequent API SendInput etc); 2. The active window belongs to a process of higher <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level; 3. Some application called API ClipCursor. No exception if the <b>Relaxed</b> option is true (then final cursor position is undefined).</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed" r=""/>, <see cref="OptMouse.MoveSleepFinally" r=""/>, <see cref="OptMouse.Relaxed" r=""/>.
		/// </remarks>
		public static Point MoveRelative(int dx, int dy)
		{
			LibWaitWhileButtonsPressed();
			var p = LastMoveXY;
			p.Offset(dx, dy);
			_Move(p, fast: false);
			return p;
		}

		/// <summary>
		/// Plays recorded mouse movements, relative to <see cref="LastMoveXY"/>.
		/// Returns the final cursor position in primary screen coordinates.
		/// </summary>
		/// <param name="recordedString">String containing mouse movement data recorded by a recorder tool that uses <see cref="Util.Recording.MouseToString"/>.</param>
		/// <param name="speedFactor">Speed factor. For example, 0.5 makes 2 times faster.</param>
		/// <exception cref="ArgumentException">The string is not compatible with this library version (recorded with a newer version and has additional options).</exception>
		/// <exception cref="ArgumentOutOfRangeException">The last x y is not in screen. No exception if the <b>Relaxed</b> option is true (then moves to a screen edge).</exception>
		/// <exception cref="AuException">Failed to move to the last x y. Some reasons: 1. Another thread blocks or modifies mouse input (API BlockInput, mouse hooks, frequent API SendInput etc); 2. The active window belongs to a process of higher <conceptualLink target="e2645f42-9c3a-4d8c-8bef-eabba00c92e9">UAC</conceptualLink> integrity level; 3. Some application called API ClipCursor. No exception if the <b>Relaxed</b> option is true (then final cursor position is undefined).</exception>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.Relaxed" r=""/> (only for the last movement; always relaxed in intermediate movements).
		/// </remarks>
		public static Point MoveRecorded(string recordedString, double speedFactor = 1.0)
		{
			LibWaitWhileButtonsPressed();

			var a = Convert_.Base64Decode(recordedString);

			byte flags = a[0];
			const int knownFlags = 1; if((flags & knownFlags) != flags) throw new ArgumentException("Unknown string version");
			bool withSleepTimes = 0 != (flags & 1);
			bool isSleep = withSleepTimes;

			var p = LastMoveXY;
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

					p.Offset(dx, dy);
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
		public static class Recording
		{
			/// <summary>
			/// Converts multiple recorded mouse movements to string for <see cref="Mouse.MoveRecorded(string, double)"/>.
			/// </summary>
			/// <param name="recorded">
			/// List of x y distances from previous.
			/// The first distance is from the mouse position before the first movement; at run time it will be distance from <see cref="Mouse.LastMoveXY"/>.
			/// To create uint value from distance dx dy use this code: <c>Math_.MakeUint(dx, dy)</c>.
			/// </param>
			/// <param name="withSleepTimes">
			/// <paramref name="recorded"/> also contains sleep times (milliseconds) alternating with distances.
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
						int dx = Math_.LoShort(u), x = dx - pdx; pdx = dx;
						int dy = Math_.HiShort(u), y = dy - pdy; pdy = dy;

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
				//Print(a.Count, Convert_.Compress(a.ToArray()).Length);

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
		static void _SendMove(Point p)
		{
			if(t_prevMousePos == null) t_prevMousePos = new _PrevMousePos(); //sets .first=XY
			_SendRaw(Api.IMFlags.Move, p.X, p.Y);
		}

		/// <summary>
		/// Sends single mouse button down or up event.
		/// Does not use the action flags of button.
		/// Applies SM_SWAPBUTTON.
		/// If pMoved!=null, also moves to pMoved in the same API SendInput call.
		/// </summary>
		static void _SendButton(MButton button, bool down, Point? pMoved)
		{
			Api.IMFlags f; MouseButtons mb = 0;
			switch(button & (MButton.Left | MButton.Right | MButton.Middle | MButton.X1 | MButton.X2)) {
			case 0: //allow 0 for left. Example: Wnd.Find(...).MouseClick(x, y, MButton.DoubleClick)
			case MButton.Left: f = down ? Api.IMFlags.LeftDown : Api.IMFlags.LeftUp; mb = MouseButtons.Left; break;
			case MButton.Right: f = down ? Api.IMFlags.RightDown : Api.IMFlags.RightUp; mb = MouseButtons.Right; break;
			case MButton.Middle: f = down ? Api.IMFlags.MiddleDown : Api.IMFlags.MiddleUp; mb = MouseButtons.Middle; break;
			case MButton.X1: f = down ? Api.IMFlags.XDown | Api.IMFlags.X1 : Api.IMFlags.XUp | Api.IMFlags.X1; mb = MouseButtons.XButton1; break;
			case MButton.X2: f = down ? Api.IMFlags.XDown | Api.IMFlags.X2 : Api.IMFlags.XUp | Api.IMFlags.X2; mb = MouseButtons.XButton2; break;
			default: throw new ArgumentException("Several buttons specified", nameof(button)); //rejected: InvalidEnumArgumentException. It's in System.ComponentModel namespace.
			}

			//maybe mouse left/right buttons are swapped
			if(0 != (button & (MButton.Left | MButton.Right)) && 0 != Api.GetSystemMetrics(Api.SM_SWAPBUTTON))
				f ^= down ? Api.IMFlags.LeftDown | Api.IMFlags.RightDown : Api.IMFlags.LeftUp | Api.IMFlags.RightUp;

			//If this is a Click(x y), the sequence of sent events is like: move, sleep, down, sleep, up. Even Click() sleeps between down and up.
			//During the sleep the user can move the mouse. Correct it now if need.
			//tested: if don't need to move, mouse messages are not sent. Hooks not tested. In some cases are sent one or more mouse messages but it depends on other things.
			//Alternatively could temporarily block user input, but it is not good. Need a hook (UAC disables Api.BlockInput), etc. Better let scripts do it explicitly. If script contains several mouse/keys statements, it's better to block input once for all.
			if(pMoved != null) f |= Api.IMFlags.Move;

			var p = pMoved.GetValueOrDefault();
			_SendRaw(f, p.X, p.Y);

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
				x <<= 16; x += (x >= 0) ? 0x8000 : -0x8000; x /= Screen_.Width;
				y <<= 16; y += (y >= 0) ? 0x8000 : -0x8000; y /= Screen_.Height;
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

		[ThreadStatic] static MouseButtons t_pressedButtons;

		static void _Click(MButton button, Point? pMoved = null, Wnd w = default)
		{
			//Sending a click to a window of own thread often does not work.
			//Reason 1: often the window on down event enters a message loop that waits for up event. But then this func cannot send the up event because it is in the loop (if it does doevents).
			//	Known workarounds:
			//	1 (applied). Don't sleepdoevents between sending down and up events.
			//	2. Let this func send the click from another thread, and sleepdoevents until that thread finishes the click.
			//Reason 2: if this func called from a click handler, OS does not send more mouse events.
			//	Known workarounds:
			//	1. (applied): show warning. Let the user modify the script: either don't click own windows or click from another thread.

			if(w.Is0) w = Api.WindowFromPoint((pMoved != null) ? pMoved.GetValueOrDefault() : XY);
			bool windowOfThisThread = w.IsOfThisThread;
			if(windowOfThisThread) PrintWarning("Click(window of own thread) may not work. Use another thread.");

			int sleep = Opt.Mouse.ClickSpeed;

			switch(button & (MButton.Down | MButton.Up | MButton.DoubleClick)) {
			case MButton.DoubleClick:
				sleep = Math.Min(sleep, Api.GetDoubleClickTime() / 4);
				//info: default double-click time is 500. Control Panel can set 200-900. API can set 1.
				//info: to detect double-click, some apps use time between down and down (that is why /4), others between up and down.

				_SendButton(button, true, pMoved);
				if(!windowOfThisThread) _Sleep(sleep);
				_SendButton(button, false, pMoved);
				if(!windowOfThisThread) _Sleep(sleep);
				goto case 0;
			case 0: //click
				_SendButton(button, true, pMoved);
				if(!windowOfThisThread) _Sleep(sleep);
				_SendButton(button, false, pMoved);
				break;
			case MButton.Down:
				_SendButton(button, true, pMoved);
				break;
			case MButton.Up:
				_SendButton(button, false, pMoved);
				break;
			default: throw new ArgumentException("Incompatible flags: Down, Up, DoubleClick", nameof(button));
			}
			_Sleep(sleep + Opt.Mouse.ClickSleepFinally);

			//rejected: detect click failures (UAC, BlockInput, hooks).
			//	Difficult. Cannot detect reliably. SendInput returns true.
			//	Eg when blocked by UAC, GetKeyState shows changed toggle state. Then probably hooks also called, did not test.
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button.
		/// By default does not move the mouse cursor.
		/// </summary>
		/// <param name="button">Button and action. Default: left click.</param>
		/// <param name="useLastMoveXY">
		/// Use <see cref="LastMoveXY"/>. It is the mouse cursor position set by the most recent 'mouse move' or 'mouse move and click' function called in this thread. Use this option for reliability.
		/// Example: <c>Mouse.Move(100, 100); Mouse.ClickEx(..., true);</c>. The click is always at 100 100, even if somebody changes cursor position between <c>Mouse.Move</c> sets it and <c>Mouse.ClickEx</c> uses it. In such case this option atomically moves the cursor to <b>LastMoveXY</b>. This movement is instant and does not use <see cref="Opt"/>.
		/// If false (default), clicks at the current cursor position (does not move it).
		/// </param>
		/// <exception cref="ArgumentException">Invalid button flags (multiple buttons or actions specified).</exception>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.ClickSpeed" r=""/>, <see cref="OptMouse.ClickSleepFinally" r=""/>.
		/// </remarks>
		public static MRelease ClickEx(MButton button = MButton.Left, bool useLastMoveXY = false)
		{
			Point? p = null; if(useLastMoveXY) p = LastMoveXY;
			_Click(button, p);
			return button;
			//CONSIDER: OptMouse.ClickAtLastMoveXY
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button at position x y in screen.
		/// To move the mouse cursor, calls <see cref="Move(Coord, Coord, CoordOptions)"/>.
		/// </summary>
		/// <param name="button">Button and action.</param>
		/// <param name="x">X coordinate relative to screen.</param>
		/// <param name="y">Y coordinate relative to screen.</param>
		/// <param name="co">Can be used to specify screen (see <see cref="Screen_.FromObject"/>) and/or whether x y are relative to the work area.</param>
		/// <exception cref="ArgumentException">Invalid button flags (multiple buttons or actions specified).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed" r=""/>, <see cref="OptMouse.MoveSleepFinally" r=""/> (between moving and clicking), <see cref="OptMouse.ClickSpeed" r=""/>, <see cref="OptMouse.ClickSleepFinally" r=""/>, <see cref="OptMouse.Relaxed" r=""/>.
		/// </remarks>
		public static MRelease ClickEx(MButton button, Coord x, Coord y, CoordOptions co = null)
		{
			var p = Move(x, y, co);
			_Click(button, p);
			return button;
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button at position x y relative to window w.
		/// To move the mouse cursor, calls <see cref="Move(Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="button">Button and action.</param>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="ArgumentException">Invalid button flags (multiple buttons or actions specified).</exception>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="WndException">x y is not in the window (read more in Remarks).</exception>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.MoveSpeed" r=""/>, <see cref="OptMouse.MoveSleepFinally" r=""/> (between moving and clicking), <see cref="OptMouse.ClickSpeed" r=""/>, <see cref="OptMouse.ClickSleepFinally" r=""/>, <see cref="OptMouse.Relaxed" r=""/>.
		/// If after moving the cursor it is not in window w or a window of its thread, activates w (or its top-level parent window). Throws exception if then x y is still not in w. Skips all this when just releasing button or if <b>Opt.Mouse.Relaxed</b> is true. Also, if w is a control, x y can be somewhere else in its top-level parent window.
		/// </remarks>
		public static MRelease ClickEx(MButton button, Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			var p = Move(w, x, y, nonClient);

			//Make sure will click w, not another window.
			var action = button & (MButton.Down | MButton.Up | MButton.DoubleClick);
			if(action != MButton.Up && !Opt.Mouse.Relaxed) { //allow to release anywhere, eg it could be a drag-drop
				var wTL = w.WndWindow;
				bool bad = !wTL.Rect.Contains(p);
				if(!bad && !_CheckWindowFromPoint()) {
					Debug_.Print("need to activate");
					//info: activating brings to the Z top and also uncloaks
					if(!wTL.IsEnabled) bad = true; //probably an owned modal dialog disabled the window
					else if(wTL.ThreadId == Wnd.Misc.WndShell.ThreadId) bad = true; //desktop
					else if(wTL.IsActive) wTL.ZorderTop(); //can be below another window in the same topmost/normal Z order, although it is rare
					else bad = !wTL.LibActivate(Wnd.Lib.ActivateFlags.NoThrowIfInvalid | Wnd.Lib.ActivateFlags.IgnoreIfNoActivateStyleEtc | Wnd.Lib.ActivateFlags.NoGetWndWindow);

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
					if(wTL.IsEnabled && wfp.ThreadId == wTL.ThreadId && !wfp.HasStyle(Native.WS_CAPTION)) return true;
					return false;
				}
			}

			_Click(button, p, w);
			return button;
		}

		/// <summary>
		/// Left click.
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastMoveXY">Use <see cref="LastMoveXY"/>, not current cursor position.</param>
		public static void Click(bool useLastMoveXY = false)
		{
			ClickEx(MButton.Left, useLastMoveXY);
		}

		/// <summary>
		/// Left click at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord, CoordOptions)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		public static void Click(Coord x, Coord y)
		{
			//note: most Click functions don't have a workArea and screen parameter. It is rarely used. For reliability better use the overloads that use window coordinates.

			ClickEx(MButton.Left, x, y);
		}

		/// <summary>
		/// Left click at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AuException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static void Click(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Left, w, x, y, nonClient);
		}

		/// <summary>
		/// Right click.
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastMoveXY">Use <see cref="LastMoveXY"/>, not current cursor position.</param>
		public static void RightClick(bool useLastMoveXY = false)
		{
			ClickEx(MButton.Right, useLastMoveXY);
		}

		/// <summary>
		/// Right click at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord, CoordOptions)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		public static void RightClick(Coord x, Coord y)
		{
			ClickEx(MButton.Right, x, y);
		}

		/// <summary>
		/// Right click at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AuException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static void RightClick(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Right, w, x, y, nonClient);
		}

		/// <summary>
		/// Double left click.
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastMoveXY">Use <see cref="LastMoveXY"/>, not current cursor position.</param>
		public static void DoubleClick(bool useLastMoveXY = false)
		{
			ClickEx(MButton.Left | MButton.DoubleClick, useLastMoveXY);
		}

		/// <summary>
		/// Double left click at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord, CoordOptions)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		public static void DoubleClick(Coord x, Coord y)
		{
			ClickEx(MButton.Left | MButton.DoubleClick, x, y);
		}

		/// <summary>
		/// Double left click at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AuException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		public static void DoubleClick(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Left | MButton.DoubleClick, w, x, y, nonClient);
		}

		/// <summary>
		/// Left down (press and don't release).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastMoveXY">Use <see cref="LastMoveXY"/>, not current cursor position.</param>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// </remarks>
		public static MRelease LeftDown(bool useLastMoveXY = false)
		{
			return ClickEx(MButton.Left | MButton.Down, useLastMoveXY);
		}

		/// <summary>
		/// Left down (press and don't release) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord, CoordOptions)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// </remarks>
		public static MRelease LeftDown(Coord x, Coord y)
		{
			return ClickEx(MButton.Left | MButton.Down, x, y);
		}

		/// <summary>
		/// Left down (press and don't release) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AuException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example.
		/// </remarks>
		/// <example>
		/// <code><![CDATA[
		/// //drag and drop: start at x=8 y=8, move 20 pixels down, drop
		/// using(Mouse.LeftDown(w, 8, 8)) Mouse.MoveRelative(0, 20); //the button is auto-released when the 'using' code block ends
		/// ]]></code>
		/// </example>
		public static MRelease LeftDown(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			return ClickEx(MButton.Left | MButton.Down, w, x, y, nonClient);
		}

		/// <summary>
		/// Left up (release pressed button).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastMoveXY">Use <see cref="LastMoveXY"/>, not current cursor position.</param>
		public static void LeftUp(bool useLastMoveXY = false)
		{
			ClickEx(MButton.Left | MButton.Up, useLastMoveXY);
		}

		/// <summary>
		/// Left up (release pressed button) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord, CoordOptions)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		public static void LeftUp(Coord x, Coord y)
		{
			ClickEx(MButton.Left | MButton.Up, x, y);
		}

		/// <summary>
		/// Left up (release pressed button) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		public static void LeftUp(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Left | MButton.Up, w, x, y, nonClient);
		}

		/// <summary>
		/// Right down (press and don't release).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastMoveXY">Use <see cref="LastMoveXY"/>, not current cursor position.</param>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// </remarks>
		public static MRelease RightDown(bool useLastMoveXY = false)
		{
			return ClickEx(MButton.Right | MButton.Down, useLastMoveXY);
		}

		/// <summary>
		/// Right down (press and don't release) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord, CoordOptions)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// </remarks>
		public static MRelease RightDown(Coord x, Coord y)
		{
			return ClickEx(MButton.Right | MButton.Down, x, y);
		}

		/// <summary>
		/// Right down (press and don't release) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		/// <exception cref="AuException">x y is not in the window. More info: <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.</exception>
		/// <remarks>
		/// The return value can be used to auto-release pressed button. See example with <see cref="LeftDown(Wnd, Coord, Coord, bool)"/>.
		/// </remarks>
		public static MRelease RightDown(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			return ClickEx(MButton.Right | MButton.Down, w, x, y, nonClient);
		}

		/// <summary>
		/// Right up (release pressed button).
		/// Calls <see cref="ClickEx(MButton, bool)"/>. More info there.
		/// </summary>
		/// <param name="useLastMoveXY">Use <see cref="LastMoveXY"/>, not current cursor position.</param>
		public static void RightUp(bool useLastMoveXY = false)
		{
			ClickEx(MButton.Right | MButton.Up, useLastMoveXY);
		}

		/// <summary>
		/// Right up (release pressed button) at position x y.
		/// Calls <see cref="ClickEx(MButton, Coord, Coord, CoordOptions)"/>. More info there.
		/// </summary>
		/// <param name="x">X coordinate in the screen.</param>
		/// <param name="y">Y coordinate in the screen.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Coord, Coord, CoordOptions)"/>.</exception>
		public static void RightUp(Coord x, Coord y)
		{
			ClickEx(MButton.Right | MButton.Up, x, y);
		}

		/// <summary>
		/// Right up (release pressed button) at position x y relative to window w.
		/// Calls <see cref="ClickEx(MButton, Wnd, Coord, Coord, bool)"/>. More info there.
		/// </summary>
		/// <param name="w">Window or control.</param>
		/// <param name="x">X coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="y">Y coordinate relative to (but not limited to) the client area of w. Default - center.</param>
		/// <param name="nonClient">x y are relative to the top-left of the window rectangle. If false (default), they are relative to the client area.</param>
		/// <exception cref="Exception">Exceptions of <see cref="Move(Wnd, Coord, Coord, bool)"/>.</exception>
		public static void RightUp(Wnd w, Coord x = default, Coord y = default, bool nonClient = false)
		{
			ClickEx(MButton.Right | MButton.Up, w, x, y, nonClient);
		}

		/// <summary>
		/// Mouse wheel forward or backward.
		/// </summary>
		/// <param name="ticks">Number of wheel ticks forward (positive) or backward (negative).</param>
		/// <param name="horizontal">Horizontal wheel.</param>
		/// <remarks>
		/// Uses <see cref="Opt.Mouse"/>: <see cref="OptMouse.ClickSleepFinally" r=""/>.
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
		///// <param name="useLastMoveXY">If true (default), ensures that the cursor is at <see cref="LastMoveXY"/>, not at <see cref="XY"/> which may be different if eg the user or she's cat is moving the mouse.</param>
		///// <example>
		///// <code><![CDATA[
		///// var w = Wnd.Find("* Notepad");
		///// Mouse.LeftDown(w, 8, 8);
		///// Mouse.MoveRelative(20, 0);
		///// Mouse.Drop();
		///// ]]></code>
		///// </example>
		//public static void Drop(bool useLastMoveXY=true)
		//{
		//	if(useLastMoveXY) _ReleaseButtons(LastMoveXY);
		//	else _ReleaseButtons();
		//}

		//not used
		///// <summary>
		///// Releases mouse buttons pressed by this thread (t_pressedButtons).
		///// </summary>
		///// <param name="p">If not null, and XY is different, moves to this point. Used for reliability.</param>
		//static void _ReleaseButtons(Point? p = null)
		//{
		//	var b = t_pressedButtons;
		//	if(0 != (b & MouseButtons.Left)) _Click(MButton.Left | MButton.Up, p);
		//	if(0 != (b & MouseButtons.Right)) _Click(MButton.Right | MButton.Up, p);
		//	if(0 != (b & MouseButtons.Middle)) _Click(MButton.Middle | MButton.Up, p);
		//	if(0 != (b & MouseButtons.XButton1)) _Click(MButton.X1 | MButton.Up, p);
		//	if(0 != (b & MouseButtons.XButton2)) _Click(MButton.X2 | MButton.Up, p);
		//}
		//rejected: finally release script-pressed buttons, especially on exception. Instead let use code: using(Mouse.LeftDown(...)), it auto-releases pressed button.

		/// <summary>
		/// Returns true if some mouse buttons are pressed.
		/// See also: <see cref="Control.MouseButtons"/>.
		/// </summary>
		/// <param name="buttons">Check only these buttons. Default - all.</param>
		/// <seealso cref="WaitFor.NoMouseButtons"/>
		public static bool IsPressed(MouseButtons buttons = MouseButtons.Left | MouseButtons.Right | MouseButtons.Middle | MouseButtons.XButton1 | MouseButtons.XButton2)
		{
			if(0 != (buttons & MouseButtons.Left) && Api.GetKeyState(Keys.LButton) < 0) return true;
			if(0 != (buttons & MouseButtons.Right) && Api.GetKeyState(Keys.RButton) < 0) return true;
			if(0 != (buttons & MouseButtons.Middle) && Api.GetKeyState(Keys.MButton) < 0) return true;
			if(0 != (buttons & MouseButtons.XButton1) && Api.GetKeyState(Keys.XButton1) < 0) return true;
			if(0 != (buttons & MouseButtons.XButton2) && Api.GetKeyState(Keys.XButton2) < 0) return true;
			return false;
		}

		/// <summary>
		/// Waits while some buttons are in pressed state, except those pressed by a <see cref="Mouse"/> class function in this thread.
		/// Does nothing if the <b>Relaxed</b> option is true.
		/// </summary>
		internal static void LibWaitWhileButtonsPressed()
		{
			//not public, because we have WaitFor.MouseButtonsReleased, which is unaware about script-pressed buttons, and don't need this awareness because the script author knows what is pressed by that script

			if(Opt.Mouse.Relaxed) return;
			var mb = (MouseButtons.Left | MouseButtons.Right | MouseButtons.Middle | MouseButtons.XButton1 | MouseButtons.XButton2)
				& ~t_pressedButtons;
			if(WaitFor.NoMouseButtons(-5, mb)) return;
			PrintWarning("Info: Waiting for releasing mouse buttons. See Opt.Mouse.Relaxed.");
			WaitFor.NoMouseButtons(0, mb);
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
			/// <exception cref="AuException">Failed.</exception>
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
				if(!w.Post(message, wParam, lParam)) throw new AuException(0);
				_SleepMax(-1, w.IsOfThisThread);
			}

			//rejected:
			///// <param name="waitMS">
			///// Maximal time to wait, milliseconds. Also which API to use.
			///// If 0 (default), calls API <msdn>PostMessage</msdn> (it does not wait) and waits Opt.Mouse.<see cref="OptMouse.ClickSpeed" r=""/> ms.
			///// If less than 0 (eg Timeout.Infinite), calls API <msdn>SendMessage</msdn> which usually waits until the window finishes to process the message.
			///// Else calls API <msdn>SendMessageTimeout</msdn> which waits max waitMS milliseconds, then throws AuException.
			///// The SendX functions are not natural and less likely to work.
			///// If the window shows a dialog, the SendX functions usually wait until the dialog is closed.
			///// </param>
			///// <exception cref="AuException">Failed, or timeout.</exception>
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
			//	if(!ok) throw new AuException(0);
			//}
		}
	}
#endif
}

namespace Au.Types
{
	/// <summary>
	/// button parameter type for <see cref="Mouse.ClickEx(MButton, bool)"/> and similar functions.
	/// There are two groups of values:
	/// 1. Button (Left, Right, Middle, X1, X2). Default or 0: Left.
	/// 2. Action (Down, Up, DoubleClick). Default: click.
	/// Multiple values from the same group cannot be combined. For example Left|Right is invalid.
	/// Values from different groups can be combined. For example Right|Down.
	/// </summary>
	[Flags]
	public enum MButton
	{
		/// <summary>
		/// The left button.
		/// </summary>
		Left = 1,

		/// <summary>
		/// The right button.
		/// </summary>
		Right = 2,

		/// <summary>
		/// The middle button.
		/// </summary>
		Middle = 4,

		/// <summary>
		/// The 4-th button.
		/// </summary>
		X1 = 8,

		/// <summary>
		/// The 5-th button.
		/// </summary>
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
	/// The Dispose function releases mouse buttons pressed by the function that returned this variable.
	/// </summary>
	/// <tocexclude />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct MRelease :IDisposable
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

	public static partial class ExtensionMethods
	{
		#region Wnd

		/// <summary>
		/// Moves the cursor (mouse pointer) to the position x y relative to this window.
		/// Calls <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.
		/// By default x y coordinates are relative to the client area; default - center.
		/// </summary>
		/// <exception cref="NotFoundException">Window object not found (this variable is 0).</exception>
		/// <exception cref="Exception">Exceptions of Mouse.Move.</exception>
		public static void MouseMove(this Wnd t, Coord x = default, Coord y = default, bool nonClient = false)
		{
			Mouse.Move(+t, x, y, nonClient);
		}

		/// <summary>
		/// Clicks, double-clicks, presses or releases a mouse button at position x y relative to this window.
		/// Calls <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.
		/// By default x y coordinates are relative to the client area; default - center. Default action - left click.
		/// </summary>
		/// <exception cref="NotFoundException">Window object not found (this variable is 0).</exception>
		/// <exception cref="Exception">Exceptions of Mouse.ClickEx.</exception>
		public static void MouseClick(this Wnd t, Coord x = default, Coord y = default, MButton button = MButton.Left, bool nonClient = false)
		{
			Mouse.ClickEx(button, +t, x, y, nonClient);
		}

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
		/// <exception cref="AuException">Failed to get object rectangle (<see cref="Acc.GetRect(out RECT, Wnd)"/>) or container window (<see cref="Acc.WndContainer"/>).</exception>
		/// <exception cref="Exception">Exceptions of Mouse.Move.</exception>
		public static void MouseMove(this Acc t, Coord x = default, Coord y = default)
		{
			_AccMouseAction(+t, false, x, y, default);
		}

		/// <summary>
		/// Clicks this accessible object.
		/// Calls <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in the bounding rectangle of this object. Default - center.</param>
		/// <param name="y">Y coordinate in the bounding rectangle of this object. Default - center.</param>
		/// <param name="button">Which button and how to use it.</param>
		/// <exception cref="NotFoundException">Accessible object not found (this variable is null).</exception>
		/// <exception cref="AuException">Failed to get object rectangle (<see cref="Acc.GetRect(out RECT, Wnd)"/>) or container window (<see cref="Acc.WndContainer"/>).</exception>
		/// <exception cref="Exception">Exceptions of Mouse.ClickEx.</exception>
		public static void MouseClick(this Acc t, Coord x = default, Coord y = default, MButton button = MButton.Left)
		{
			_AccMouseAction(+t, true, x, y, button);
		}

		static void _AccMouseAction(Acc t, bool click, Coord x, Coord y, MButton button)
		{
			var w = t.WndContainer; //info: not necessary, but with window the mouse functions are more reliable, eg will not click another window if it is over our window

			//never mind: if w.Is0 and the action is 'click', get AO from point and fail if it is not t. But need to compare AO properties. Rare.

			if(!(w.Is0 ? t.GetRect(out RECT r) : t.GetRect(out r, w))) throw new AuException(0, "*get rectangle");
			var p = Coord.NormalizeInRect(x, y, r, centerIfEmpty: true);
			if(w.Is0) {
				if(button == 0) Mouse.Move(p.X, p.Y);
				else Mouse.ClickEx(button, p.X, p.Y);
			} else {
				if(button == 0) Mouse.Move(w, p.X, p.Y);
				else Mouse.ClickEx(button, w, p.X, p.Y);
			}
		}

		#endregion

		#region WinImage

		/// <summary>
		/// Moves the mouse to the found image.
		/// Calls <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/> or <see cref="Mouse.Move(Coord, Coord, CoordOptions)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in the found image. Default - center.</param>
		/// <param name="y">Y coordinate in the found image. Default - center.</param>
		/// <exception cref="NotFoundException">Image not found (this variable is null).</exception>
		/// <exception cref="InvalidOperationException">area is Bitmap.</exception>
		/// <exception cref="Exception">Exceptions of Mouse.Move.</exception>
		public static void MouseMove(this WinImage t, Coord x = default, Coord y = default)
		{
			(+t).LibMouseAction(0, x, y);
		}

		/// <summary>
		/// Clicks the found image.
		/// Calls <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/> or <see cref="Mouse.ClickEx(MButton, Coord, Coord, CoordOptions)"/>.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="x">X coordinate in the found image. Default - center.</param>
		/// <param name="y">Y coordinate in the found image. Default - center.</param>
		/// <param name="button">Which button and how to use it.</param>
		/// <exception cref="NotFoundException">Image not found (this variable is null).</exception>
		/// <exception cref="InvalidOperationException">area is Bitmap.</exception>
		/// <exception cref="Exception">Exceptions of Mouse.ClickEx.</exception>
		public static void MouseClick(this WinImage t, Coord x = default, Coord y = default, MButton button = MButton.Left)
		{
			(+t).LibMouseAction(button == 0 ? MButton.Left : button, x, y);
		}

		#endregion

	}
}
