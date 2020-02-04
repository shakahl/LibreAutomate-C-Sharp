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
using System.Linq;

using Au.Types;
using static Au.AStatic;

namespace Au
{
	public partial class AToolbar
	{
		class _OwnerWindow
		{
			public readonly List<AToolbar> a;
			public readonly AWnd w;
			public bool visible;
			bool _updatedOnce;
			RECT _rect, _clientRect;
			SIZE _prevSize, _prevClientSize;
			//public readonly int thread;

			public _OwnerWindow(AWnd window)
			{
				w = window;
				//thread = w.ThreadId;
				a = new List<AToolbar>();
			}

			public (bool visible, bool dead) IsVisible()
			{
				ALastError.Clear();
				if(!w.IsVisible) return (false, ALastError.Code != 0);
				return (!w.IsMinimized && !w.IsCloaked, false);
				//speed: IsCloaked now on Win10 quite fast, faster than GetRect
			}

			public bool UpdateRect(out bool changed)
			{
				changed = false;
				int have = 0;
				foreach(var tb in a) {
					if(tb._oc != null || tb._os != null) continue;
					if(tb._followClientArea) {
						if(0 != (have & 2)) continue;
						if(!w.GetClientRect(out var r, inScreen: true)) return false;
						if(r != _clientRect) {
							_prevClientSize = (_clientRect.Width, _clientRect.Height);
							_clientRect = r;
							changed = true;
						}
						have |= 2;
					} else {
						if(0 != (have & 1)) continue;
						if(!w.GetRect(out var r)) return false;
						if(r != _rect) {
							_prevSize = (_rect.Width, _rect.Height);
							_rect = r;
							changed = true;
						}
						have |= 1;
					}
					if(have == 3) break;
				}
				if(!_updatedOnce) _updatedOnce = changed = true;
				return true;
			}

			public RECT GetCachedRect(AToolbar tb) => tb._followClientArea ? _clientRect : _rect;

			public SIZE GetPrevSize(AToolbar tb) => tb._followClientArea ? _prevClientSize : _prevSize;
		}

		class _OwnerControl
		{
			public readonly AWnd c;
			public readonly ITBOwnerObject oo;
			public bool visible, visible2;
			bool _updatedOnce;
			public RECT cachedRect;
			public SIZE prevSize;

			public _OwnerControl(AWnd control, ITBOwnerObject ioo)
			{
				c = control;
				oo = ioo;
			}

			public (bool visible, bool dead) IsVisible(bool parentVisible = true)
			{
				if(!c.Is0) {
					ALastError.Clear();
					if(!c.IsVisible) return (false, ALastError.Code != 0);
					if(!parentVisible || c.IsMinimized) return default; //never mind: ancestors controls may be minimized
				}
				if(oo != null) {
					if(!oo.IsAlive) return (false, true);
					if(!oo.IsVisible) return default;
				}
				return (true, false);
			}

			public bool UpdateRect(out bool changed)
			{
				bool ok = oo != null ? oo.GetRect(out RECT r) : c.GetRect(out r);
				if(changed = ok && r != cachedRect) {
					prevSize = (cachedRect.Width, cachedRect.Height);
					cachedRect = r;
				}
				if(!_updatedOnce) _updatedOnce = changed = true;
				return ok;
			}
		}

		class _OwnerScreen
		{
			public _OwnerScreen(AToolbar tb, AScreen screen)
			{
				_tb = tb;
				_screen = (_isAuto = screen.IsNull) ? _tb._sett.screen : screen;
				UpdateRect(out _);
			}

			AToolbar _tb;
			AScreen _screen;
			bool _isAuto;
			public RECT cachedRect;
			public SIZE prevSize;

			public bool UpdateRect(out bool changed)
			{
				RECT r = _screen.ToDevice().Bounds;
				if(changed = r != cachedRect) {
					prevSize = (cachedRect.Width, cachedRect.Height);
					cachedRect = r;
				}
				return true;
			}

			public void UpdateIfAutoScreen()
			{
				if(!_isAuto) return;
				var k = AScreen.Of(_tb.Control);
				int i = k.Index;
				//Print(_tb._sett.screen, i);
				if(i != _tb._sett.screen) {
					_screen = i;
					_tb._sett.screen = i;
					UpdateRect(out _);
				}
			}
		}

		_OwnerWindow _ow; //not null if owned
		_OwnerControl _oc; //not null if owned by a control or other object (ITBOwnerObject)
		_OwnerScreen _os; //not null if not owned or if anchor None
		bool _followClientArea;

		[ThreadStatic] static _TBManager t_man;
		_TBManager _Manager => t_man ??= new _TBManager();
		//static readonly _TBManager _Manager = new _TBManager();

		class _TBManager
		{
			List<AToolbar> _atb = new List<AToolbar>();
			List<_OwnerWindow> _aow = new List<_OwnerWindow>();
			ATimer _timer;
			int _timerPeriod;
			AHookAcc _hook;
			int _tempHook;
			bool _inHook;

			public void Add(AToolbar tb, AWnd w, AWnd c, ITBOwnerObject ioo)
			{
				bool isOwned = !w.Is0;
				if(isOwned) {
					if(!_FindOW(w, out var ow)) _aow.Add(ow = new _OwnerWindow(w));
					ow.a.Add(tb);
					tb._ow = ow;
					if(!c.Is0 || ioo != null) tb._oc = new _OwnerControl(c, ioo);
				}

				_atb.Add(tb);

				if(_hook == null) {
					_hook = new AHookAcc(
						new AccEVENT[] {
						0, AccEVENT.OBJECT_REORDER,
						AccEVENT.OBJECT_CLOAKED, AccEVENT.OBJECT_UNCLOAKED,
						AccEVENT.SYSTEM_MOVESIZESTART, AccEVENT.SYSTEM_MOVESIZEEND,
						AccEVENT.SYSTEM_MINIMIZESTART, AccEVENT.SYSTEM_MINIMIZEEND,
						},
						_Hook,
						flags: AccHookFlags.SKIPOWNTHREAD);
					_timer = new ATimer(_Timer);
				}

				if(isOwned) {
					_SetTimer(150);
				} else {
					if(!_timer.IsRunning) _SetTimer(250);
					tb._FollowRect();
					//tb.Control.Show(); //no, tries to activate
					tb.Control.Hwnd().ShowLL(true);
					//tb._Zorder();
				}
			}

			void _SetTimer(int period)
			{
				_timer.Every(_timerPeriod = period);
			}

			void _Timer(ATimer t)
			{
				if(_timerPeriod != 250) _SetTimer(250);

				//remove closed toolbars and their owners if need
				for(int i = _atb.Count - 1; i >= 0; i--) {
					var tb = _atb[i];
					if(tb.Control.IsDisposed) {
						_atb.RemoveAt(i);
						var ow = tb._ow;
						if(ow != null) {
							ow.a.Remove(tb);
							if(ow.a.Count == 0) _aow.Remove(ow);
						}
					}
				}

				//move/close/hide/show owned toolbars together with their owners
				for(int i = _aow.Count - 1; i >= 0; i--) {
					var ow = _aow[i];
					if(!_FollowOwner(ow)) {
						foreach(var tb in ow.a) {
							tb.Close();
							_atb.Remove(tb);
						}
						_aow.RemoveAt(i);
					}
				}

				_ManageFullScreen();
			}

			void _Hook(HookData.AccHookData d)
			{
				//Print(d.ev, d.idObject, d.idChild, d.idThread, d.wnd);
				if(d.wnd.Is0 || d.idObject != (d.ev == AccEVENT.OBJECT_REORDER ? AccOBJID.CLIENT : AccOBJID.WINDOW) || d.idChild != 0) return;
				_OwnerWindow ow;
				switch(d.ev) {
				case AccEVENT.OBJECT_REORDER when d.wnd == AWnd.GetWnd.Root: //the hook does not give the window, only its thread id
#if true
					foreach(var tb in _atb) tb._Zorder();
#else
					//This version is faster but unreliable.
					//	1. For console windows getwindowthreadprocessid gives wrong thread id. Hook receives the correct id.
					//	2. When clicked client area of a Store app, hook receives thread id of the child control. It is different than that of the main host window.
					//	3. All unknown and future things like those.
					foreach(var v in _ao) {
						if(v.thread != d.idThread) continue;
						foreach(var tb in v.a) tb._Zorder();
					}
#endif
					break;
				case AccEVENT.SYSTEM_MOVESIZESTART when _tempHook == 0:
					if(_FindOW(d.wnd, out _)) _tempHook = _hook.Add(AccEVENT.OBJECT_LOCATIONCHANGE, flags: AccHookFlags.SKIPOWNTHREAD);
					break;
				case AccEVENT.SYSTEM_MOVESIZEEND when _tempHook != 0:
					_hook.Remove(_tempHook);
					_tempHook = 0;
					break;
				case AccEVENT.OBJECT_LOCATIONCHANGE:
				case AccEVENT.OBJECT_CLOAKED:
				case AccEVENT.OBJECT_UNCLOAKED:
				case AccEVENT.SYSTEM_MINIMIZESTART:
					//ADebug.PrintIf(_inHook, "_inHook"); //it's ok
					if(!_inHook && _FindOW(d.wnd, out ow)) {
						//prevent reenter.
						//	The ITBOwnerObject may retrieve sent messages, eg when getting acc rect.
						//	It's ok if hook missed. We'll call it on timer or next OBJECT_LOCATIONCHANGE.
						_inHook = true;
						try { _FollowOwner(ow); }
						finally { _inHook = false; }
					}
					break;
				case AccEVENT.SYSTEM_MINIMIZEEND:
					if(_FindOW(d.wnd, out _)) _SetTimer(150);
					break;
				}

				//SYSTEM_MOVESIZESTART and SYSTEM_MOVESIZEEND temporarily add/remove OBJECT_LOCATIONCHANGE to move toolbars with the owner window.
				//	Cannot make OBJECT_LOCATIONCHANGE always active, because it is called frequently, on each cursor position change etc.
				//	There are no other not-in-process hooks to detect moved windows. For CBT hook need 2 processes - 64bit and 32bit.

				//OBJECT_REORDER keeps toolbars above their owner windows in the Z order.
				//	Easier would be to make the owner natively owner. But then problems:
				//	1. If this process is admin, the owner's process cannot receive drag&drop from other non-admin processes. Don't know why, probably it is a Windows bug.
				//	2. Fails if owner's process is a Store app. Also probaby if higher UAC IL.
				//	3. In some cases possible various anomalies, for example wrong Z order of windows after closing the owner window.
				//	4. All unknown and future things like those.
				//	In QM2 some of these problems were solved by adding a child window to the owner window and making in the native owner of the toolbar. But then other problems, eg DPI-scaling.

				//PROBLEM: OBJECT_REORDER makes creating windows slower.
				//	For example, combobox controls send OBJECT_REORDER when adding items. Two for each item that would be visible in the drop-down list.
				//	Tested: standard dialog box with 12 comboboxes, each with 30 such items. We receive ~720 OBJECT_REORDER.
				//		If there are 4 processes with 1 OBJECT_REORDER hook, dialog startup time increases 50%, from 360 to 540 ms.
				//Other used hooks aren't called frequently. Except OBJECT_LOCATIONCHANGE, but it is temporary.
			}

			bool _FindOW(AWnd owner, out _OwnerWindow ow)
			{
				foreach(var v in _aow) if(v.w == owner) { ow = v; return true; }
				ow = null; return false;
			}

			bool _FollowOwner(_OwnerWindow ow)
			{
				var (visible, dead) = ow.IsVisible();
				if(dead) return false;

				int nControls = 0;
				for(int i = ow.a.Count - 1; i >= 0; i--) {
					var oc = ow.a[i]._oc; if(oc == null) continue;
					(oc.visible2, dead) = oc.IsVisible(visible);
					if(dead) {
						ow.a[i].Close();
						ow.a.RemoveAt(i);
						continue;
					}
					nControls++;
				}
				if(ow.a.Count == 0) return false;

				if(visible) {
					if(!ow.UpdateRect(out bool changed)) visible = false;
					else if(changed) {
						foreach(var tb in ow.a) {
							if(tb._oc == null) tb._FollowRect(true);
						}
					}
					if(nControls > 0) {
						foreach(var tb in ow.a) {
							var oc = tb._oc; if(oc == null) continue;
							if(!visible) oc.visible2 = false;
							if(!oc.visible2) continue;
							if(!oc.UpdateRect(out changed)) oc.visible2 = false;
							else if(changed) tb._FollowRect(true);
						}
					}
				}

				if(visible != ow.visible) {
					ow.visible = visible;
					foreach(var tb in ow.a) {
						if(tb._oc != null) continue;
						tb.Control.Hwnd().ShowLL(visible);
						if(visible) tb._Zorder();
					}
				}
				if(nControls > 0) {
					foreach(var tb in ow.a) {
						var oc = tb._oc; if(oc == null) continue;
						if(oc.visible2 != oc.visible) {
							oc.visible = oc.visible2;
							tb.Control.Hwnd().ShowLL(oc.visible);
							if(oc.visible) tb._Zorder();
						}
					}
				}

				return true;
			}

			void _ManageFullScreen(AToolbar tb = null)
			{
				APerf.First();
				if(tb?.MiscFlags.Has(TBFlags.HideIfFullScreen) ?? _atb.Any(o => o.MiscFlags.Has(TBFlags.HideIfFullScreen))) {
					var w = AWnd.Active;
					w.IsFullScreen_(out var screen);
					APerf.Next();
					if(tb != null) tb._ManageFullScreen(w, screen);
					else foreach(var v in _atb) if(v.MiscFlags.Has(TBFlags.HideIfFullScreen)) v._ManageFullScreen(w, screen);
				}
				APerf.NW();
			}
		}

		void _ManageFullScreen(AWnd wFore, AScreen.Device screen)
		{
			bool hide;
			if(screen.Is0) hide = false;
			else if(IsOwned) hide = OwnerWindow == wFore;
			else hide = AScreen.Of(_c, SDefault.Zero) == screen;
			//Print(hide, screen);

			if(hide == _hide.Has(_EHide.FullScreen)) return;
			_hide ^= _EHide.FullScreen;
			_c.Hwnd().ShowLL(!hide);
			//TODO: finish. Eg don't show if is hidden for other reasons.
		}

		[Flags]
		enum _EHide : byte
		{
			FullScreen = 4,
		}
		_EHide _hide;

		void _Zorder()
		{
			if(!IsOwned) {

			} else if(_ow.visible) {
				var wt = _c.Hwnd();
				if(!_zorderedOnce || !wt.ZorderIsAbove(_ow.w)) {
					//Print("ZorderAbove", _ow.w);
					wt.ZorderAbove(_ow.w);
					_zorderedOnce = true;
					//never mind: when clicked owner's caption, we receive 2 hook events and need to ZorderAbove 2 times. Speed is OK, but flickers more often.
					//	When we ZorderAbove on mouse down, Windows also zorders the window on mouse up, and then we receive second event.
					//	Possible workarounds:
					//	1. Temporarily make wt nativaly owned by _ow.w. Restore after 500 ms. But fails with higher UAC IL windows and appstore windows.
					//	2. Temporarily make wt topmost. Restore after 500 ms. But Windows makes it difficult and possibly unreliable.
				}
			}
		}
		bool _zorderedOnce;

		void _FollowRect(bool onFollowOwner = false)
		{
			if(_inMoveSize) return;
			if(_anchor == TBAnchor.None && onFollowOwner && _followedOnce) return;

			var (r, prevSize) = _GetCachedOwnerRect();
			//Print(r, _anchor, _xy, Size);

			var swp = Native.SWP.NOZORDER | Native.SWP.NOOWNERZORDER | Native.SWP.NOACTIVATE;
			var bounds = _c.Bounds;
			int x, y, cx = bounds.Width, cy = bounds.Height;

			if(_anchor.HasLeft()) {
				x = r.left + _xy.Left;
				if(_anchor.HasRight() && (!_followedOnce || r.Width != prevSize.width)) {
					if(_preferSize) _xy.Right = r.right - x - cx;
					else cx = Math.Max(r.right - _xy.Right - x, 2); //_OnWindowPosChanging will limit min max if need
				}
			} else if(_anchor.HasRight()) {
				x = r.right - _xy.Right - cx;
			} else { //none
				x = r.left + _xy.Left;
			}
			if(_anchor.HasTop()) {
				y = r.top + _xy.Top;
				if(_anchor.HasBottom() && (!_followedOnce || r.Height != prevSize.height)) {
					if(_preferSize) _xy.Bottom = r.bottom - y - cy;
					else cy = Math.Max(r.bottom - _xy.Bottom - y, 2);
				}
			} else if(_anchor.HasBottom()) {
				y = r.bottom - _xy.Bottom - cy;
			} else { //none
				y = r.top + _xy.Top;
			}

			if(_preferSize) {
				_preferSize = false;
				_sett.location = _xy;
			}

			if(x == bounds.X && y == bounds.Y) swp |= Native.SWP.NOMOVE;
			if(cx == bounds.Width && cy == bounds.Height) swp |= Native.SWP.NOSIZE;
			if(!swp.Has(Native.SWP.NOMOVE | Native.SWP.NOSIZE)) {
				var w = _c.Hwnd();
				_ignorePosChanged = true;
				w.SetWindowPos(swp, x, y, cx, cy);
				_ignorePosChanged = false;
			}
			_followedOnce = true;
		}
		bool _followedOnce;
		bool _ignorePosChanged;
		bool _preferSize;

		void _OnWindowPosChanging(ref Api.WINDOWPOS wp)
		{
			if(!_loaded) return;
			if(!wp.flags.Has(Native.SWP.NOSIZE)) {
				SIZE min = _GetMinSize();
				if(wp.cx < min.width) wp.cx = min.width;
				if(wp.cy < min.height) wp.cy = min.height;
				SIZE max = _c.MaximumSize;
				if(max.width > 0) wp.cx = Math.Min(wp.cx, Math.Max(max.width, min.width));
				if(max.height > 0) wp.cy = Math.Min(wp.cy, Math.Max(max.height, min.height));
			}

			SIZE _GetMinSize()
			{
				int k = _border < TBBorder.ThreeD ? 1 : AWnd.More.BorderWidth(_c.Hwnd());
				k = k * 2;
				var ms = _c.MinimumSize;
				return (Math.Max(k, ms.Width), Math.Max(k, ms.Height));
			}
			//SIZE _GetMinSize()
			//{
			//	int k = _border < TBBorder.ThreeD ? (int)_border : AWnd.More.BorderWidth(_c.Hwnd());
			//	k = k * 2 + 1; //two borders and 1 pixel of client area
			//	var ms = _c.MinimumSize;
			//	return (Math.Max(k, ms.Width), Math.Max(k, ms.Height));
			//}
		}

		void _OnWindowPosChanged(in Api.WINDOWPOS wp)
		{
			if(!_loaded) return;
			if(!wp.flags.Has(Native.SWP.NOMOVE | Native.SWP.NOSIZE)) {
				if(!_ignorePosChanged) {
					_os?.UpdateIfAutoScreen();
					_UpdateXY(wp.x, wp.y, wp.cx, wp.cy);
				} else {
					if(!wp.flags.Has(Native.SWP.NOSIZE) && !_inMoveSize) _sett.size = (wp.cx, wp.cy);
				}
				_SatFollow();
			}
			if(wp.flags.Has(Native.SWP.HIDEWINDOW)) {
				_SatHide();
			}
		}

		void _UpdateXY(int x, int y, int cx, int cy)
		{
			var (r, _) = _GetCachedOwnerRect();
			if(_anchor.HasLeft() || _anchor == TBAnchor.None) _xy.Left = x - r.left;
			if(_anchor.HasTop() || _anchor == TBAnchor.None) _xy.Top = y - r.top;
			if(_anchor.HasRight()) _xy.Right = r.right - x - cx;
			if(_anchor.HasBottom()) _xy.Bottom = r.bottom - y - cy;
			if(!_inMoveSize) {
				_sett.location = _xy;
				_sett.size = (cx, cy);
			}
		}

		void _InMoveSize(bool start)
		{
			_inMoveSize = start;
			if(!start) {
				_sett.location = _xy;
				SIZE z = _c.Size;
				if(z != _sett.size) {
					_sett.size = z;
					if(_sett.autoSize) {
						var cz = _c.ClientSize;
						_sett.wrapWidth = _IsVerticalFlow ? cz.Height : cz.Width;
						_AutoSize();
					}
				}
			}
		}
		bool _inMoveSize;

		void _OnDisplayChanged()
		{
			if(_os == null) return;
			ATimer.After(200, _ => {
				_os.UpdateRect(out bool changed);
				if(changed) _FollowRect();
			});
		}

		/// <summary>
		/// Gets the cached rectangle of the owner window, screen, control, etc.
		/// If is owned and anchor is None, the rectangle is of toolbar's screen.
		/// Also gets previous size.
		/// The values are cached by <b>UpdateRect</b> of <b>_OwnerWindow</b> etc.
		/// </summary>
		(RECT r, SIZE prevSize) _GetCachedOwnerRect()
		{
			if(_os != null) return (_os.cachedRect, _os.prevSize);
			if(_oc != null) return (_oc.cachedRect, _oc.prevSize);
			return (_ow.GetCachedRect(this), _ow.GetPrevSize(this));
		}
	}
}
