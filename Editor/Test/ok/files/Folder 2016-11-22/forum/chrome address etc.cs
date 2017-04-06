if(getopt(nthreads)>1) ret ;;allow single instance, assuming this is a function or a 'run simultaneously' macro
int hwnd=TriggerWindow ;;if trigger
if(!hwnd) hwnd=win("* - Google Chrome" "Chrome_WidgetWin_1" "" 1) ;;if Run button
if(!hwnd) ret ;;no Chrome
int pid; if(!GetWindowThreadProcessId(hwnd &pid)) end ERR_FAILED 16 ;;hook only this Chrome process
int hh=SetWinEventHook(EVENT_OBJECT_NAMECHANGE EVENT_OBJECT_NAMECHANGE 0 &sub.Hook pid 0 WINEVENT_OUTOFCONTEXT)
if(!hh) end ERR_FAILED 16
opt waitmsg 1 ;;don't block Windows messages, else sub.Hook cannot be called
wait 0 WP hwnd ;;wait until Chrome process exits
UnhookWinEvent hh


#sub Hook
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

if(!hwnd) ret
 outw hwnd ;;debug
Acc a.FromEvent(hwnd idObject idChild)
 out a.Name
sel a.Name
	case ["Address and search bar","Barre d'adresse et de recherche"]
	case else ret
str url=a.Value
err+ ;;Acc functions throw error when closing Chrome

out url
