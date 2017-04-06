 Run this function.
 Run again if want to end it.
 Edit the 100 if need. Don't need more editing.


if getopt(nthreads)>1
	EndThread "disable_too_fast_double_click"
	ret

int hh=SetWindowsHookEx(WH_MOUSE_LL &sub.Hook_WH_MOUSE_LL _hinst 0)
opt waitmsg 1
wait -1
UnhookWindowsHookEx hh


#sub Hook_WH_MOUSE_LL
function# nCode message MSLLHOOKSTRUCT&m
if(nCode<0) goto gNext

 if(message!=WM_MOUSEMOVE) OutWinMsg message 0 0 _s; out "%s at %i %i" _s m.pt.x m.pt.y

int-- prevTime disableNextUp
sel message
	case [WM_LBUTTONDOWN,WM_RBUTTONDOWN,WM_MBUTTONDOWN]
	if(timeGetTime-prevTime < 100) disableNextUp=1; ret 1
	disableNextUp=0
	
	case [WM_LBUTTONUP,WM_RBUTTONUP,WM_MBUTTONUP]
	prevTime=timeGetTime
	if(disableNextUp) disableNextUp=0; ret 1

 gNext
ret CallNextHookEx(0 nCode message &m)
