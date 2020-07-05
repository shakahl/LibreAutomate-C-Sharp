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
//using System.Linq;

using Au.Types;
using Au.Util;

namespace Au
{
	public partial struct AWnd
	{
		/// <summary>
		/// Sets opacity and/or transparent color.
		/// </summary>
		/// <param name="allowTransparency">Set or remove WS_EX_LAYERED style that is required for transparency. If false, other parameters are not used.</param>
		/// <param name="opacity">Opacity from 0 (completely transparent) to 255 (opaque). Does not change if null. If less than 0 or greater than 255, makes 0 or 255.</param>
		/// <param name="colorKey">Make pixels of this color completely transparent. Does not change if null. The alpha byte is not used.</param>
		/// <exception cref="AuWndException"/>
		/// <remarks>
		/// Uses API <msdn>SetLayeredWindowAttributes</msdn>.
		/// On Windows 7 works only with top-level windows, on newer OS also with controls.
		/// </remarks>
		public void SetTransparency(bool allowTransparency, int? opacity = null, ColorInt? colorKey = null) {
			var est = ExStyle;
			bool layered = (est & WS2.LAYERED) != 0;

			if (allowTransparency) {
				uint col = 0, f = 0; byte op = 0;
				if (colorKey != null) { f |= 1; col = (uint)colorKey.GetValueOrDefault().ToBGR(); }
				if (opacity != null) { f |= 2; op = (byte)Math.Clamp(opacity.GetValueOrDefault(), 0, 255); }

				if (!layered) SetExStyle(est | WS2.LAYERED);
				if (!Api.SetLayeredWindowAttributes(this, col, op, f)) ThrowUseNative();
			} else if (layered) {
				SetExStyle(est & ~WS2.LAYERED); //tested: resets attributes, ie after adding WS2.LAYERED the window will be normal
			}
		}

		/// <summary>
		/// Returns true if this is a full-screen window and not desktop.
		/// </summary>
		public bool IsFullScreen => IsFullScreen_(out _);

		internal unsafe bool IsFullScreen_(out AScreen.ScreenHandle screen) {
			screen = default;
			if (Is0) return false;

			//is client rect equal to window rect (no border)?
			RECT r, rc, rm;
			r = Rect; //fast
			int cx = r.right - r.left, cy = r.bottom - r.top;
			if (cx < 400 || cy < 300) return false; //too small
			rc = ClientRect; //fast
			if (rc.right != cx || rc.bottom != cy) {
				if (cx - rc.right > 2 || cy - rc.bottom > 2) return false; //some windows have 1-pixel border
			}

			//covers whole monitor rect?
			screen = AScreen.Of(this, SODefault.Zero); if (screen.Is0) return false;
			rm = screen.Bounds;

			if (r.left > rm.left || r.top > rm.top || r.right < rm.right || r.bottom < rm.bottom - 1) return false; //info: -1 for inactive Chrome

			//is it desktop?
			if (IsOfShellThread_) return false;
			if (this == GetWnd.Root) return false;

			return true;

			//This is the best way to test for fullscreen (FS) window. Fast.
			//Window and client rect was equal of almost all my tested FS windows. Except Winamp visualization.
			//Most FS windows are same size as screen, but some slightly bigger.
			//Don't look at window styles. For some FS windows they are not as should be.
			//Returns false if the active window is owned by a fullscreen window. This is different than appbar API interprets it. It's OK for our purposes.
		}

		/// <summary>
		/// Returns true if this belongs to GetShellWindow's thread (usually it is the desktop window).
		/// </summary>
		internal bool IsOfShellThread_ {
			get => 1 == __isShellWindow.IsShellWindow(this);
		}

		/// <summary>
		/// Returns true if this belongs to GetShellWindow's process (eg a folder window, desktop, taskbar).
		/// </summary>
		internal bool IsOfShellProcess_ {
			get => 0 != __isShellWindow.IsShellWindow(this);
		}

		struct __ISSHELLWINDOW
		{
			int _tidW, _tidD, _pidW, _pidD;
			IntPtr _w, _wDesk; //not AWnd because then TypeLoadException

			public int IsShellWindow(AWnd w) {
				if (w.Is0) return 0;
				AWnd wDesk = GetWnd.ShellWindow; //fast
				if (w == wDesk) return 1; //Progman. Other window (WorkerW) may be active when desktop active.

				//cache because GetWindowThreadProcessId quite slow
				if (w.Handle != _w) { _w = w.Handle; _tidW = Api.GetWindowThreadProcessId(w, out _pidW); }
				if (wDesk.Handle != _wDesk) { _wDesk = wDesk.Handle; _tidD = Api.GetWindowThreadProcessId(wDesk, out _pidD); }

				if (_tidW == _tidD) return 1;
				if (_pidW == _pidD) return 2;
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
		/// <seealso cref="More.GetWindowsStoreAppId"/>
		public bool IsWindows8MetroStyle {
			get {
				if (!AVersion.MinWin8) return false;
				if (!HasExStyle(WS2.TOPMOST | WS2.NOREDIRECTIONBITMAP) || (Style & WS.CAPTION) != 0) return false;
				if (ClassNameIs("Windows.UI.Core.CoreWindow")) return true;
				if (!AVersion.MinWin10 && IsOfShellProcess_) return true;
				return false;
				//could use IsImmersiveProcess, but this is better
			}
		}

		/// <summary>
		/// Returns non-zero if this window is a Windows 10 Store app window: 1 if class name is "ApplicationFrameWindow", 2 if "Windows.UI.Core.CoreWindow".
		/// </summary>
		/// <seealso cref="More.GetWindowsStoreAppId"/>
		public int IsWindows10StoreApp {
			get {
				if (!AVersion.MinWin10) return 0;
				if (!HasExStyle(WS2.NOREDIRECTIONBITMAP)) return 0;
				return ClassNameIs("ApplicationFrameWindow", "Windows.UI.Core.CoreWindow");
				//could use IsImmersiveProcess, but this is better
			}
		}

		/// <summary>
		/// Gets window position, size and state stored in a string that can be used with <see cref="RestoreSavedRect"/>.
		/// Returns null if failed. Supports <see cref="ALastError"/>.
		/// </summary>
		/// <param name="canBeMinimized">If now the window is minimized, let <see cref="RestoreSavedRect"/> make it minimized. If false, RestorePlacement will restore it to the most recent non-minimized state.</param>
		public string SaveRect(bool canBeMinimized = false) {
			if (!GetWindowPlacement_(out var p, false)) return null;
			//AOutput.Write(p.showCmd, p.flags, p.ptMaxPosition, p.rcNormalPosition);
			if (!canBeMinimized && p.showCmd == Api.SW_SHOWMINIMIZED) p.showCmd = (p.flags & Api.WPF_RESTORETOMAXIMIZED) != 0 ? Api.SW_SHOWMAXIMIZED : Api.SW_SHOWNORMAL;
			return AConvert.Base64UrlEncode(p);
		}

		/// <summary>
		/// Restores window position, size and state that is stored in a string created by <see cref="SaveRect"/>.
		/// </summary>
		/// <param name="s">The string. Can be null/"".</param>
		/// <param name="ensureInScreen">Call <see cref="EnsureInScreen"/>. Even when s is null/"". Default true.</param>
		/// <param name="showActivate">Call <see cref="Show"/>(true) and <see cref="ActivateLL"/>. Even when s is null/"".</param>
		/// <exception cref="AuWndException"/>
		public unsafe void RestoreSavedRect(string s, bool ensureInScreen = true, bool showActivate = false) {
			if (AConvert.Base64UrlDecode(s, out Api.WINDOWPLACEMENT p)) {
				//AOutput.Write(p.showCmd, p.flags, p.ptMaxPosition, p.rcNormalPosition);
				if (!showActivate && !this.IsVisible) {
					var style = this.Style;
					switch (p.showCmd) {
						case Api.SW_SHOWMAXIMIZED:
							if ((style & WS.MAXIMIZE) == 0) { //TODO: does it really work? In QM2 doesn't.
								this.MoveLL(p.rcNormalPosition); //without this would be always in primary monitor
								this.SetStyle(style | WS.MAXIMIZE);
							}
							break;
						case Api.SW_SHOWMINIMIZED:
							if ((style & WS.MINIMIZE) == 0) this.SetStyle(style | WS.MINIMIZE);
							break;
						case Api.SW_SHOWNORMAL:
							if ((style & (WS.MAXIMIZE | WS.MINIMIZE)) != 0) this.SetStyle(style & ~(WS.MAXIMIZE | WS.MINIMIZE));
							//never mind: if currently minimized, will not be restored. Usually currently is normal, because this func called after creating window, especially if invisible. But will restore if currently maximized.
							break;
					}
					p.showCmd = 0;
				}
				this.SetWindowPlacement_(ref p, false, "*restore*");
			}

			if (ensureInScreen) this.EnsureInScreen();
			if (showActivate) {
				this.Show(true);
				this.ActivateLL();
			}
		}

		public static partial class More
		{
			/// <summary>
			/// Helps to save and restore window rectangle and state in correct way (ensure in screen, per-monitor-DPI-aware, etc).
			/// </summary>
			/// <example>
			/// <code><![CDATA[
			/// class Form9 : Form {
			/// 	const string c_rkey = @"HKEY_CURRENT_USER\Software\Au\Test";
			/// 	const string c_rvalue = @"Form9.Rect";
			/// 	
			/// 	public Form9() {
			/// 		this.StartPosition = FormStartPosition.Manual;
			/// 		if (AWnd.More.SavedRect.FromString(Registry.GetValue(c_rkey, c_rvalue, null) as string, out var v)) {
			/// 			this.Bounds = v.NormalizeRect();
			/// 			if(v.Maximize) this.WindowState = FormWindowState.Maximized;
			/// 		}
			/// 	}
			/// 
			/// 	protected override void OnFormClosing(FormClosingEventArgs e) {
			/// 		Registry.SetValue(c_rkey, c_rvalue, new AWnd.More.SavedRect(this.Hwnd()).ToString());
			/// 		base.OnFormClosing(e);
			/// 	}
			/// }
			/// ]]></code>
			/// </example>
			public struct SavedRect
			{
				/// <summary>
				/// Window rectangle in normal state (not maximized/minimized), as retrieved by API <msdn>GetWindowPlacement</msdn>.
				/// </summary>
				public RECT RawRect { get; set; }

				/// <summary>
				/// <see cref="ADpi.OfWindow"/>.
				/// </summary>
				public int Dpi { get; set; }

				/// <summary>
				/// The window should be maximized.
				/// </summary>
				public bool Maximize { get; set; }

				/// <summary>
				/// <see cref="AWnd.IsToolWindow"/>. If false, <see cref="RawRect"/> may have an offset that depends on work area.
				/// </summary>
				public bool IsToolWindow { get; set; }

				/// <summary>
				/// Converts this object to string for saving.
				/// The string is very simple, like "1 2 3 4 5 6".
				/// </summary>
				public override string ToString() {
					var r = RawRect;
					return $"{r.left} {r.top} {r.Width} {r.Height} {Dpi} {(Maximize ? 1 : 0) | (IsToolWindow ? 2 : 0)}";
				}

				/// <summary>
				/// Creates <b>SavedRect</b> from string created by <see cref="ToString"/>.
				/// Returns false if the string is null or invalid.
				/// </summary>
				public static bool FromString(string saved, out SavedRect x) {
					x = default;
					if (saved == null) return false;
					var t = saved.Split(' '); if (t.Length < 6) return false;
					var a = new int[t.Length]; for (int i = 0; i < t.Length; i++) if(!t[i].ToInt(out a[i])) return false;
					x.RawRect = (a[0], a[1], a[2], a[3]);
					x.Dpi = a[4];
					var flags = a[5];
					x.Maximize = 0 != (flags & 1);
					x.IsToolWindow = 0 != (flags & 2);
					return true;
				}

				/// <summary>
				/// Gets window rectangle and state for saving. Usually called when closing the window.
				/// See also <see cref="ToString"/>.
				/// </summary>
				/// <exception cref="AuWndException">Failed to get rectangle, probably invalid window handle.</exception>
				public SavedRect(AWnd w) {
					if (!w.GetWindowPlacement_(out var p, false)) w.ThrowUseNative();
					RawRect = p.rcNormalPosition;
					Dpi = ADpi.OfWindow(w);
					Maximize = p.showCmd == Api.SW_SHOWMAXIMIZED || (p.showCmd == Api.SW_SHOWMINIMIZED && 0 != (p.flags & Api.WPF_RESTORETOMAXIMIZED));
					IsToolWindow = w.IsToolWindow;
				}

				/// <summary>
				/// Gets real rectangle for restoring saved window rectangle.
				/// </summary>
				/// <remarks>
				/// It is recommended to call this before creating window, and create window with the returned rectangle. Also set maximized state if <see cref="Maximize"/>.
				/// If it is not possible, call this when window is already created but still invisible. However then may need to set window rectangle two times, because the window may be for example DPI-scaled when moving to another screen etc.
				/// 
				/// This function ensures the window is in screen, ensures correct size when screen DPI changed, etc.
				/// </remarks>
				public RECT NormalizeRect() { //TODO: bool forWpf = false; test and example with AWindowBuilder
					var r = RawRect;
					var scr = AScreen.Of(r);
					int dpi = ADpi.OfScreen(scr.Handle);
					if (dpi != this.Dpi) {
						r.Width = AMath.MulDiv(r.Width, dpi, this.Dpi);
						r.Height = AMath.MulDiv(r.Height, dpi, this.Dpi);
					}
					if (!IsToolWindow) {
						var v = scr.GetInfo();
						r.Offset(v.workArea.left - v.bounds.left, v.workArea.top - v.bounds.top);
					}
					r.EnsureInScreen(new AScreen(scr), !IsToolWindow);
					return r;
				}
			}
		}
	}
}
