function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

if getopt(nargs)!=7
	int hh=SetWinEventHook(EVENT_SYSTEM_FOREGROUND EVENT_SYSTEM_FOREGROUND 0 &exe_window_trigger 0 0 WINEVENT_OUTOFCONTEXT)
	if(!hh) end F"{ERR_FAILED}. {_s.dllerror}"
	opt waitmsg 1
	wait -1
	UnhookWinEvent hh
	ret

 This code runs whenever a window activated.

outw hwnd

 if wintest(hwnd ...)
 	clo hwnd
