function# [time_s]

 Waits for the registered hotkeys.
 Returns hotkey id.
 Error on timeout or if an argument is invalid.
 Must be registered with hWnd=0.
 While waiting, processes messages, even if opt waitmsg is not used.

 time_s - timeout in seconds. If omitted or 0, waits indefinitely.


if(!m_a.len) end ES_INIT
if(m_hwnd) end "must be registered with hWnd=0"

if(time_s)
	long t=time_s*1000; if(t>USER_TIMER_MAXIMUM) end ES_BADARG
	int tid=SetTimer(0 m_a t 0)

MSG m
int r i
rep
	if(GetMessage(&m 0 0 0)<1) break
	sel m.message
		case WM_HOTKEY
		for(i 0 m_a.len) if(m.wParam=m_a[i]) r=1; goto g1
		case WM_TIMER
		if(m.wParam=tid) r=2; break
	TranslateMessage &m
	DispatchMessage &m

 g1
if(time_s) KillTimer 0 tid

sel r
	case 0 end
	case 2 end "timeout"
ret m.wParam
