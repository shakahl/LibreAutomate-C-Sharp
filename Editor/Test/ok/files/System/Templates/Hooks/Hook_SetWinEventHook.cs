 /
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

outw hwnd ;;debug

 your code here


  SetWinEventHook example
 int hh=SetWinEventHook(EVENT_SYSTEM_FOREGROUND EVENT_SYSTEM_FOREGROUND 0 &sub.Hook_SetWinEventHook 0 0 WINEVENT_OUTOFCONTEXT)
 if(!hh) end F"{ERR_FAILED}. {_s.dllerror}"
 opt waitmsg 1
 wait -1
 UnhookWinEvent hh
