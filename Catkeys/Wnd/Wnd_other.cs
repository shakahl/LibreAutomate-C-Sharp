using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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

using static Catkeys.NoClass;

namespace Catkeys
{
	public partial struct Wnd
	{
		/// <summary>
		/// Sets transparency.
		/// On Windows 7 works only with top-level windows, on newer OS also with controls.
		/// </summary>
		/// <param name="allowTransparency">Set or remove WS_EX_LAYERED style that is required for transparency. If false, other parameters are not used.</param>
		/// <param name="opacity">Opacity from 0.0 (completely transparent) to 1.0 (opaque). If null, sets default value (opaque).</param>
		/// <param name="colorRGB">Make pixels painted with this color completely transparent. If null, sets default value (no transparent color). The format is 0xRRGGBB, like Color.ToArgb returns. The alpha byte is not used.</param>
		/// <exception cref="ArgumentOutOfRangeException">opacity is less than 0.0 or greater than 1.0.</exception>
		/// <exception cref="WndException"/>
		public void SetTransparency(bool allowTransparency, double? opacity = null, int? colorRGB = null)
		{
			uint est = ExStyle;
			bool layered = (est & Native.WS_EX_LAYERED) != 0;

			if(allowTransparency) {
				if(!layered) SetExStyle(est | Native.WS_EX_LAYERED);

				uint col = 0, op = 0, f = 0;
				if(colorRGB != null) {
					f |= 1;
					col = Color_.SwapRedBlue((uint)colorRGB.Value) & 0xffffff;
				}
				if(opacity != null) {
					f |= 2;
					var d = opacity.GetValueOrDefault();
					if(d < 0.0 || d > 1.0) throw new ArgumentOutOfRangeException(nameof(opacity));
					op = (uint)(d * 255);
				}

				if(!Api.SetLayeredWindowAttributes(this, col, (byte)op, f)) ThrowUseNative();
			} else if(layered) {
				//if(!Api.SetLayeredWindowAttributes(this, 0, 0, 0)) ThrowUseNative();
				SetExStyle(est & ~Native.WS_EX_LAYERED);
			}
		}

		/// <summary>
		/// Returns true if this is a full-screen window and not desktop.
		/// </summary>
		public bool IsFullScreen
		{
			get
			{
				if(Is0) return false;

				//is client rect equal to window rect (no border)?
				RECT r, rc, rm;
				r = Rect; //fast
				int cx = r.right - r.left, cy = r.bottom - r.top;
				if(cx < 400 || cy < 300) return false; //too small
				rc = ClientRect; //fast
				if(rc.right != cx || rc.bottom != cy) {
					if(cx - rc.right > 2 || cy - rc.bottom > 2) return false; //some windows have 1-pixel border
				}

				//covers whole monitor rect?
				rm = Screen.FromHandle(Handle).Bounds; //fast except first time, because uses caching
				if(r.left > rm.left || r.top > rm.top || r.right < rm.right || r.bottom < rm.bottom - 1) return false; //info: -1 for inactive Chrome

				//is it desktop?
				if(LibIsOfShellThread) return false;
				if(this == Misc.WndRoot) return false;

				return true;

				//This is the best way to test for fullscreen (FS) window. Fast.
				//Window and client rect was equal of almost all my tested FS windows. Except Winamp visualization.
				//Most FS windows are same size as screen, but some slightly bigger.
				//Don't look at window styles. For some FS windows they are not as should be.
				//Returns false if the active window is owned by a fullscreen window. This is different than appbar API interprets it. It's OK for our purposes.
			}
		}

		/// <summary>
		/// Returns true if this belongs to GetShellWindow's thread (usually it is the desktop window).
		/// </summary>
		internal bool LibIsOfShellThread
		{
			get => 1 == __isShellWindow.IsShellWindow(this);
		}

		/// <summary>
		/// Returns true if this belongs to GetShellWindow's process (eg a folder window, desktop, taskbar).
		/// </summary>
		internal bool LibIsOfShellProcess
		{
			get => 0 != __isShellWindow.IsShellWindow(this);
		}

		struct __ISSHELLWINDOW
		{
			int _tidW, _tidD, _pidW, _pidD;
			IntPtr _w, _wDesk; //not Wnd because then TypeLoadException

			public int IsShellWindow(Wnd w)
			{
				Wnd wDesk = Misc.WndShell; //fast
				if(w == wDesk) return 1; //Progman. Usually other window (WorkerW) is active when desktop active.

				//cache because GetWindowThreadProcessId quite slow
				if(w.Handle != _w) { _w = w.Handle; _tidW = Api.GetWindowThreadProcessId(w, out _pidW); }
				if(wDesk.Handle != _wDesk) { _wDesk = wDesk.Handle; _tidD = Api.GetWindowThreadProcessId(wDesk, out _pidD); }

				if(_tidW == _tidD) return 1;
				if(_pidW == _pidD) return 2;
				return 0;
			}
		}
		static __ISSHELLWINDOW __isShellWindow;

		/// <summary>
		/// Returns true if this window has Metro style, ie is not a classic desktop window.
		/// On Windows 8/8.1 most Windows Store app windows and many shell windows have Metro style.
		/// On Windows 10 few windows have Metro style.
		/// On Windows 7 there are no Metro style windows.
		/// </summary>
		/// <seealso cref="Misc.GetWindowsStoreAppId"/>
		public bool IsWindows8MetroStyle
		{
			get
			{
				if(!Ver.MinWin8) return false;
				if(!HasExStyle(Native.WS_EX_TOPMOST | Native.WS_EX_NOREDIRECTIONBITMAP) || (Style & Native.WS_CAPTION) != 0) return false;
				if(ClassNameIs("Windows.UI.Core.CoreWindow")) return true;
				if(!Ver.MinWin10 && LibIsOfShellProcess) return true;
				return false;
				//could use IsImmersiveProcess, but this is better
			}
		}

		/// <summary>
		/// Returns non-zero if this window is a Windows 10 Store app window: 1 if class name is "ApplicationFrameWindow", 2 if "Windows.UI.Core.CoreWindow".
		/// </summary>
		/// <seealso cref="Misc.GetWindowsStoreAppId"/>
		public int IsWindows10StoreApp
		{
			get
			{
				if(!Ver.MinWin10) return 0;
				if(!HasExStyle(Native.WS_EX_NOREDIRECTIONBITMAP)) return 0;
				return ClassNameIs("ApplicationFrameWindow", "Windows.UI.Core.CoreWindow");
				//could use IsImmersiveProcess, but this is better
			}
		}

		/// <summary>
		/// Gets window position, size and state stored in a string that can be used with <see cref="RestorePositionSizeState"/>.
		/// Returns null if failed. Supports <see cref="Native.GetError"/>.
		/// </summary>
		/// <param name="canBeMinimized">If now the window is minimized, let RestorePositionSizeState make it minimized. If false, RestorePlacement will restore it to the most recent non-minimized state.</param>
		public unsafe string SavePositionSizeState(bool canBeMinimized = false)
		{
			if(!LibGetWindowPlacement(out var p)) return null;
			//PrintList(p.showCmd, p.flags, p.ptMaxPosition, p.rcNormalPosition);
			if(!canBeMinimized && p.showCmd == Api.SW_SHOWMINIMIZED) p.showCmd = (p.flags & Api.WPF_RESTORETOMAXIMIZED) != 0 ? Api.SW_SHOWMAXIMIZED : Api.SW_SHOWNORMAL;
			return Convert_.HexEncode(&p, sizeof(Api.WINDOWPLACEMENT), true);
		}

		/// <summary>
		/// Restores window position, size and state that is stored in a string created by <see cref="SavePositionSizeState"/>.
		/// </summary>
		/// <param name="s">The string. Can be null/"".</param>
		/// <param name="ensureInScreen">Call <see cref="EnsureInScreen"/>. Even when s is null/"".</param>
		/// <param name="showActivate">Call <see cref="Show">Show</see>(true) and <see cref="ActivateLL"/>. Even when s is null/"".</param>
		/// <exception cref="WndException"/>
		public unsafe void RestorePositionSizeState(string s, bool ensureInScreen = false, bool showActivate = false)
		{
			Api.WINDOWPLACEMENT p; int siz = sizeof(Api.WINDOWPLACEMENT);
			if(siz == Convert_.HexDecode(s, &p, siz)) {
				//PrintList(p.showCmd, p.flags, p.ptMaxPosition, p.rcNormalPosition);
				if(!showActivate && !this.IsVisible) {
					uint style = this.Style;
					switch(p.showCmd) {
					case Api.SW_SHOWMAXIMIZED:
						if((style & Native.WS_MAXIMIZE) == 0) {
							this.MoveLL(p.rcNormalPosition.left, p.rcNormalPosition.top, p.rcNormalPosition.Width, p.rcNormalPosition.Height); //without this would be always in primary monitor
							this.SetStyle(style | Native.WS_MAXIMIZE);
						}
						break;
					case Api.SW_SHOWMINIMIZED:
						if((style & Native.WS_MINIMIZE) == 0) this.SetStyle(style | Native.WS_MINIMIZE);
						break;
					case Api.SW_SHOWNORMAL:
						if((style & (Native.WS_MAXIMIZE | Native.WS_MINIMIZE)) != 0) this.SetStyle(style & ~(Native.WS_MAXIMIZE | Native.WS_MINIMIZE));
						//never mind: if currently minimized, will not be restored. Usually currently is normal, because this func called after creating window, especially if invisible. But will restore if currently maximized.
						break;
					}
					p.showCmd = 0;
				}
				this.LibSetWindowPlacement(ref p, "*restore*");
			}

			if(ensureInScreen) this.EnsureInScreen();
			if(showActivate) {
				this.Show(true);
				this.ActivateLL();
			}
		}

		/// <summary>
		/// Calls <see cref="Mouse.Move(Wnd, Coord, Coord, bool)"/>.
		/// By default x y coordinates are relative to the client area.
		/// </summary>
		/// <exception cref="Exception">Exceptions of Mouse.Move.</exception>
		public void MouseMove(Coord x, Coord y, bool nonClient = false)
		{
			Mouse.Move(this, x, y, nonClient);
		}

		/// <summary>
		/// Calls <see cref="Mouse.ClickEx(MButton, Wnd, Coord, Coord, bool)"/>.
		/// By default x y coordinates are relative to the client area.
		/// </summary>
		/// <exception cref="Exception">Exceptions of Mouse.Click.</exception>
		public void MouseClick(Coord x, Coord y, MButton button = MButton.Left, bool nonClient = false)
		{
			Mouse.ClickEx(button, this, x, y, nonClient);
		}
	}
}
