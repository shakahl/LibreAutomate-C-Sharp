 /
function# hwnd

 Returns nonzero if window is a shell process window, eg desktop or taskbar.
 Returns 1 if it is desktop or belongs to the same thread as desktop (usually it is desktop).
 Returns 2 if belongs to the same process as desktop and has no caption with 'Close' button. Eg taskbar, or Windows 8 "Start" screen.
 Returns -1 if belongs to the same process as desktop and has caption with 'Close' button. Eg a folder window, when not using option 'Launch folder windows in a separate process'.
 Else returns 0.

 hwnd - a window handle.

 EXAMPLE
  close window from mouse, if it is not desktop, taskbar or other shell window
 int w=win(mouse)
 if(IsShellWindow(w)<1) clo w; err


int tid1 tid2 pid1 pid2 sw=GetShellWindow
tid1=GetWindowThreadProcessId(hwnd &pid1)
tid2=GetWindowThreadProcessId(sw &pid2)
if pid1=pid2
	if(tid1=tid2) ret 1
	if(GetWinStyle(hwnd)&(WS_CAPTION|WS_SYSMENU)=(WS_CAPTION|WS_SYSMENU)) ret -1
	ret 2
