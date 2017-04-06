__Handle ev=CreateEvent(0 0 0 0)
Acc a
int hh=SetWinEventHook(EVENT_OBJECT_FOCUS EVENT_OBJECT_FOCUS 0 &sub.Hook_SetWinEventHook 0 0 WINEVENT_OUTOFCONTEXT)
opt waitmsg 1
wait 0 H ev
UnhookWinEvent hh

_s=a.Name
mes _s "Clicked"


#sub Hook_SetWinEventHook v
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

if(idObject!=OBJID_CLIENT) ret
 outw hwnd
if(!WinTest(hwnd "Chrome_RenderWidgetHostHWND")) ret
a.FromEvent(hwnd idObject idChild)
if(a.Role!=ROLE_SYSTEM_PUSHBUTTON) ret
SetEvent ev

err+
