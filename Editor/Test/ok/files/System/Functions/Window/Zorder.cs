 /
function hwnd [hwndInsertAfter] [flags] ;;hwndInsertAfter: HWND_TOP (def.), HWND_BOTTOM, HWND_TOPMOST, HWND_NOTOPMOST, or window handle;  flags: SWP_NOACTIVATE, other SWP_ flags.

 Places the window in the specified position in the Z order (top, bottom, etc).

 hwnd - window handle.
 hwndInsertAfter - one of:
   HWND_TOPMOST - make the window (hwnd) topmost (a.k.a. "always-on-top"). Topmost windows are always above other (normal) windows. Child windows cannot be topmost.
   HWND_NOTOPMOST - make the window (hwnd) normal.
   HWND_TOP - move the window (hwnd) to the top of windows of its Z order (normal, topmost, or child windows of same parent).
   HWND_BOTTOM - move the window (hwnd) to the bottom of all windows. Also removes topmost style. Moving behind a normal window also removes this style.
   A window handle. Then hwnd will be placed behind hwndInsertAfter.
 flags - <help>SetWindowPos</help> uFlags.

 REMARKS
 This function calls a Windows API function <help>SetWindowPos</help>.
 With child windows it is better tu use SetWindowPos (with flags SWP_NOSIZE|SWP_NOMOVE) or BringWindowToTop.
 To check if a window is topmost: <code>if(GetWinStyle(hwnd 1)&WS_EX_TOPMOST)</code>

 SetWindowPos bugs:
   On Windows 8/10 something may not work well with topmost windows of uiAccess processes. QM process normally is uiAccess; most other processes aren't.
   On Windows 10 cannot move a topmost window to the very bottom (HWND_BOTTOM) or after a non-topmost window. Workaround: at first call Zorder with HWND_NOTOPMOST.


flags|=SWP_NOSIZE|SWP_NOMOVE
if flags&SWP_NOACTIVATE or (hwndInsertAfter and hwndInsertAfter!=HWND_TOPMOST) or RealGetParent(hwnd)
	flags|SWP_NOACTIVATE
	SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags)
	if hwndInsertAfter and hwndInsertAfter!=HWND_TOPMOST and GetWinStyle(hwnd 1)&WS_EX_TOPMOST ;;probably a uiAccess window on Win8+
		SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags|SWP_NOOWNERZORDER)
		rep(2) SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags)
else
	int h=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
	act h; err
	SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags)
	DestroyWindow(h)

 Windows 8/10 bug: cannot make a topmost uiAccess window non-topmost (with HWND_NOTOPMOST, HWND_BOTTOM or non-topmost hwndInsertAfter).
   Workaround: call SWP 2 times. With some Windows updates also need SWP_NOOWNERZORDER.
   Problems with SWP_NOOWNERZORDER:
      1. If used with non-uiAccess windows, then later HWND_TOPMOST does not work. Solution: call SWP first time without this flag.
      2. Does not make owned windows non-topmost. Solution: finally call SWP without this flag.
      3. Does not make owner window non-topmost. Never mind, it is rare, and a solution is dirty.
   The bug and workarounds are undocumented.
 More problems with topmost uiAccess windows:
   Sometimes inserting a uiAccess hwnd after a window does not work, sometimes works...
   Problems with HWND_BOTTOM and owned windows.
   And so on.
 On Windows XP/7/8 HWND_BOTTOM moves a topmost window to the bottom of ALL windows, as documented.
   But on Windows 10 - to the top of non-topmost windows; 2-nd SWP moves to the right place, but 3-th SWP moves uiAccess windows back :), 4-th makes correct (owned windows too).
      It seems it is fixed in current Win10 version.
