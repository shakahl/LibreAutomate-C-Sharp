 /
function hwnd [hwndInsertAfter] [flags] ;;hwndInsertAfter: HWND_TOP (def.), HWND_BOTTOM, HWND_TOPMOST, HWND_NOTOPMOST, or window handle;  flags: SWP_NOACTIVATE, other SWP_ flags.

 Places the window in the specified position in the Z order (top, bottom, etc).

 hwnd - window handle.
 hwndInsertAfter - one of the above constants, or a window handle (then hwnd will be placed behind that window).

 REMARKS
 Use HWND_TOPMOST and HWND_NOTOPMOST to make the window always-on-top or normal. Child windows cannot be always-on-top.
 HWND_TOP moves the window to the top of windows of its style (normal, always-on-top, or child windows of same parent).
 HWND_BOTTOM moves to the bottom of all windows. If child - of child windows of same parent. If the window is always-on-top, removes this style. Moving behind a normal window also removes this style.
 Instead can be used this code: <code>SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE)</code> . However with top-level windows it works only if the active window belongs to current thread.
 To check whether a window is always-on-top: <code>if(GetWinStyle(hwnd 1)&WS_EX_TOPMOST)</code>

 Some Windows 8.1 and 10 updates have a bug: some always-on-top windows cannot be made normal (HWND_NOTOPMOST, HWND_BOTTOM etc).
   Particularly, if the process of the window has uiAccess rigts.
   Then you also cannot make QM window normal after setting it always-on-top, unless QM runs as User. Need to restart QM.


flags|=SWP_NOSIZE|SWP_NOMOVE
if flags&SWP_NOACTIVATE or (hwndInsertAfter and hwndInsertAfter!=HWND_TOPMOST) or RealGetParent(hwnd)
	
	 Windows 8 bug: if admin or uiAccess, need to call the API 2 times to remove hwnd from topmost windows.
	 But with some Windows 8/10 updates this does not work if uiAccess. Another secret workaround - SWP_NOOWNERZORDER. And call 2 times.
	int n=1; if(_winver>=0x602 and hwndInsertAfter and hwndInsertAfter!=HWND_TOPMOST and GetWinStyle(hwnd 1)&WS_EX_TOPMOST) n=2; flags|SWP_NOOWNERZORDER
	
	rep(n) SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags|SWP_NOACTIVATE)
else
	int h=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
	act h; err
	SetWindowPos(hwnd hwndInsertAfter 0 0 0 0 flags)
	DestroyWindow(h)
