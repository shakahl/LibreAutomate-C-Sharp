 /Macro166
 /exe 1
function!

out 100
if(AllowSetForegroundWindow(GetCurrentProcessId)) ret 1
out 101

 keybd_event 0 0 0 0
 keybd_event 0 0 2 0
ASF_Key
if(AllowSetForegroundWindow(GetCurrentProcessId)) ret 1


 int hFore=GetForegroundWindow
 if hFore
	 MSG m; PeekMessage(&m 0 0 0 1)
	 int tidMe tidFore
	 tidFore=GetWindowThreadProcessId(hFore 0)
	 if tidFore and AttachThreadInput(GetCurrentThreadId tidFore 1)
		 SetForegroundWindow win("Calculator" "CalcFrame")
		  SetForegroundWindow GetDesktopWindow
		 outw win
		 int ok=AllowSetForegroundWindow(GetCurrentProcessId)
		 AttachThreadInput(GetCurrentThreadId tidFore 0)
		 out "ok=%i %i" ok AllowSetForegroundWindow(GetCurrentProcessId)
		 if(ok) ret 1
out 102
 ret

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
ASF_Key
AllowSetForegroundWindow(GetCurrentProcessId)
SetForegroundWindow GetDesktopWindow
DestroyWindow(hh)
Q &qqqq; outq
 outw win
ret AllowSetForegroundWindow(GetCurrentProcessId)
