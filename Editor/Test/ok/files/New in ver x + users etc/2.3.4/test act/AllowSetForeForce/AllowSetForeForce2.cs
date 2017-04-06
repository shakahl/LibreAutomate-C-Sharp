 /exe 1
function!

 min win

 keybd_event 0 0 0 0
Q &q
int hh=CreateWindowEx(WS_EX_TOOLWINDOW "#32770" 0 WS_POPUP|WS_MINIMIZE 0 0 0 0 0 0 _hinst 0)
Q &qq
 SetWindowState hh 6 1
SetWindowState hh 9 1
 min hh
res hh
Q &qqq
 SetForegroundWindow FindWindow("Shell_TrayWnd", 0)
SetForegroundWindow GetDesktopWindow
DestroyWindow(hh)
Q &qqqq; outq
outw win
ret AllowSetForegroundWindow(GetCurrentProcessId)
