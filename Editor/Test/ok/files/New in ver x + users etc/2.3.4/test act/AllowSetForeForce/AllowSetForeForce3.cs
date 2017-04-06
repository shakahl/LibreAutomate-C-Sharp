 /exe 1
function!

 ret AllowSetForegroundWindow(GetCurrentProcessId)

 min win

 keybd_event 0 0 0 0
Q &q
int hh=CreateWindowEx(WS_EX_TOOLWINDOW "#32770" 0 WS_POPUP|WS_MINIMIZE|WS_VISIBLE 0 0 0 0 0 0 _hinst 0)
 int hh=CreateWindowEx(0 "#32770" 0 WS_OVERLAPPEDWINDOW|WS_MINIMIZE 0 0 100 100 0 0 _hinst 0)
 outw win
Q &qq
 SetWindowState hh 6 1
SetWindowState hh 9 1
 min hh
 res hh
 outw win
Q &qqq
 SetForegroundWindow FindWindow("Shell_TrayWnd", 0)
AllowSetForegroundWindow(GetCurrentProcessId)
SetForegroundWindow GetDesktopWindow
DestroyWindow(hh)
Q &qqqq; outq
 outw win
ret AllowSetForegroundWindow(GetCurrentProcessId)
