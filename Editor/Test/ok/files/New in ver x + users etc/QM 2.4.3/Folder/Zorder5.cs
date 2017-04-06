 /
function hwnd [hwndInsertAfter] [flags] ;;hwndInsertAfter: HWND_TOP (def.), HWND_BOTTOM, HWND_TOPMOST, HWND_NOTOPMOST, or window handle;  flags: SWP_NOACTIVATE, other SWP_ flags.

 Places the window in the specified position in the Z order (top, bottom, etc).

 hwnd - window handle.
 hwndInsertAfter - one of the above constants, or a window handle (then hwnd will be placed behind that window).

 REMARKS
 Use HWND_TOPMOST and HWND_NOTOPMOST to make the window always-on-top or normal. Child windows cannot be always-on-top.
 HWND_TOP moves the window to the top of windows of its kind (normal, always-on-top, or child windows of same parent).
 HWND_BOTTOM moves the window to the bottom of all windows. Also removes always-on-top style. Moving behind a normal window also removes this style.
 Instead can be used this code: <code>SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE)</code> . However with top-level windows it works only if the active window belongs to current thread.
 To check if a window is always-on-top: <code>if(GetWinStyle(hwnd 1)&WS_EX_TOPMOST)</code>


flags|=SWP_NOSIZE|SWP_NOMOVE
if flags&SWP_NOACTIVATE or (hwndInsertAfter and hwndInsertAfter!=HWND_TOPMOST) or RealGetParent(hwnd)
	
	int retry
	if(hwndInsertAfter and hwndInsertAfter!=HWND_TOPMOST and GetWinStyle(hwnd 1)&WS_EX_TOPMOST) retry=9
	 if(retry and flags&SWP_NOOWNERZORDER=0) int p=GetWindow(hwnd GW_OWNER); if(p and p!=GetDesktopWindow and GetWinStyle(p 1)&WS_EX_TOPMOST) Zorder p hwndInsertAfter
	 if(retry) flags|SWP_NOOWNERZORDER
	out retry
	
	int n
	rep 1+retry
		SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags|SWP_NOACTIVATE)
		n+1
		if(retry and GetWinStyle(hwnd 1)&WS_EX_TOPMOST=0) out n; break
		flags|SWP_NOOWNERZORDER
else
	int h=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
	act h; err
	rep 1
		SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags)
	if(hwndInsertAfter=HWND_TOPMOST) outx GetWinStyle(hwnd 1)&WS_EX_TOPMOST
	DestroyWindow(h)

 Windows 8/10 bug: if admin or uiAccess, need to call SWP 2 times to remove hwnd from topmost windows.
 With some Windows updates also need SWP_NOOWNERZORDER.
 	Note: after HWND_TOPMOST/SWP_NOOWNERZORDER, for non-uiAccess windows does not work HWND_TOPMOST. Therefore we call SWP first time without this flag.
 	Nevermind: then owner remains topmost (if exists and is topmost).
 The bug and both workarounds are undocumented.
