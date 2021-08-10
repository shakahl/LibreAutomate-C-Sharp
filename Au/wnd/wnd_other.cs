
namespace Au
{
	public partial struct wnd
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
		/// Does not work with WPF windows, class name "HwndWrapper*".
		/// </remarks>
		public void SetTransparency(bool allowTransparency, int? opacity = null, ColorInt? colorKey = null) {
			var est = ExStyle;
			bool layered = (est & WSE.LAYERED) != 0;

			if (allowTransparency) {
				uint col = 0, f = 0; byte op = 0;
				if (colorKey != null) { f |= 1; col = (uint)colorKey.GetValueOrDefault().ToBGR(); }
				if (opacity != null) { f |= 2; op = (byte)Math.Clamp(opacity.GetValueOrDefault(), 0, 255); }

				if (!layered) SetExStyle(est | WSE.LAYERED);
				if (!Api.SetLayeredWindowAttributes(this, col, op, f)) ThrowUseNative();
			} else if (layered) {
				SetExStyle(est & ~WSE.LAYERED); //tested: resets attributes, ie after adding WSE.LAYERED the window will be normal
			}
		}

		/// <summary>
		/// Returns true if this is a full-screen window and not desktop.
		/// </summary>
		public bool IsFullScreen => IsFullScreen_(out _);

		internal unsafe bool IsFullScreen_(out screen scrn) {
			scrn = default;
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

			//covers whole screen rect?
			scrn = screen.of(this, SODefault.Zero); if (scrn.IsEmpty) return false;
			rm = scrn.Rect;

			if (r.left > rm.left || r.top > rm.top || r.right < rm.right || r.bottom < rm.bottom - 1) return false; //info: -1 for inactive Chrome

			//is it desktop?
			if (IsOfShellThread_) return false;
			if (this == getwnd.root) return false;

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
			get => 1 == s_isShellWindow.IsShellWindow(this);
		}

		/// <summary>
		/// Returns true if this belongs to GetShellWindow's process (eg a folder window, desktop, taskbar).
		/// </summary>
		internal bool IsOfShellProcess_ {
			get => 0 != s_isShellWindow.IsShellWindow(this);
		}

		struct _ISSHELLWINDOW
		{
			int _tidW, _tidD, _pidW, _pidD;
			IntPtr _w, _wDesk; //not wnd because then TypeLoadException

			public int IsShellWindow(wnd w) {
				if (w.Is0) return 0;
				wnd wDesk = getwnd.shellWindow; //fast
				if (w == wDesk) return 1; //Progman. Other window (WorkerW) may be active when desktop active.

				//cache because GetWindowThreadProcessId quite slow
				if (w.Handle != _w) { _w = w.Handle; _tidW = Api.GetWindowThreadProcessId(w, out _pidW); }
				if (wDesk.Handle != _wDesk) { _wDesk = wDesk.Handle; _tidD = Api.GetWindowThreadProcessId(wDesk, out _pidD); }

				if (_tidW == _tidD) return 1;
				if (_pidW == _pidD) return 2;
				return 0;
			}
		}
		static _ISSHELLWINDOW s_isShellWindow;

		/// <summary>
		/// Returns true if this window has Metro style, ie is not a classic desktop window.
		/// On Windows 8/8.1 most Windows Store app windows and many shell windows have Metro style.
		/// On Windows 10 few windows have Metro style.
		/// On Windows 7 there are no Metro style windows.
		/// </summary>
		/// <seealso cref="WndUtil.GetWindowsStoreAppId"/>
		public bool IsWindows8MetroStyle {
			get {
				if (!osVersion.minWin8) return false;
				if (!HasExStyle(WSE.TOPMOST | WSE.NOREDIRECTIONBITMAP) || (Style & WS.CAPTION) != 0) return false;
				if (ClassNameIs("Windows.UI.Core.CoreWindow")) return true;
				if (!osVersion.minWin10 && IsOfShellProcess_) return true;
				return false;
				//could use IsImmersiveProcess, but this is better
			}
		}

		/// <summary>
		/// On Windows 10 and later returns non-zero if this is a UWP app window: 1 if class name is "ApplicationFrameWindow", 2 if "Windows.UI.Core.CoreWindow".
		/// </summary>
		/// <seealso cref="WndUtil.GetWindowsStoreAppId"/>
		public int IsUwpApp {
			get {
				if (!osVersion.minWin10) return 0;
				if (!HasExStyle(WSE.NOREDIRECTIONBITMAP)) return 0;
				return ClassNameIs("ApplicationFrameWindow", "Windows.UI.Core.CoreWindow");
				//could use IsImmersiveProcess, but this is better
			}
		}
	}
}
