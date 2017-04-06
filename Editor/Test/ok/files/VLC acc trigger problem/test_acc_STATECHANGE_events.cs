int w=win("VLC media player" "QWidget")
Acc a.Find(w "PUSHBUTTON" "" "class=QWidget[]descr=^(Play|Pause)" 0x1088)

int hh=SetWinEventHook(WINAPI.EVENT_OBJECT_STATECHANGE WINAPI.EVENT_OBJECT_STATECHANGE 0 &sub.Hook_SetWinEventHook 0 0 WINEVENT_OUTOFCONTEXT)
if(!hh) end F"{ERR_FAILED}. {_s.dllerror}"
opt waitmsg 1
wait -1
UnhookWinEvent hh


#sub Hook_SetWinEventHook
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

outw hwnd ;;debug
