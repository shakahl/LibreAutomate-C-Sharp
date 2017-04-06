 Would work only if in-process. To test, log acc events. Not tested with LibreOffice.

out
 int hh=SetWinEventHook(EVENT_SYSTEM_FOREGROUND EVENT_SYSTEM_FOREGROUND 0 &sub.Hook_SetWinEventHook 0 0 WINEVENT_OUTOFCONTEXT)
int hh=SetWinEventHook(EVENT_OBJECT_FOCUS EVENT_OBJECT_FOCUS 0 &sub.Hook_SetWinEventHook 0 0 WINEVENT_OUTOFCONTEXT)
if(!hh) end F"{ERR_FAILED}. {_s.dllerror}"
opt waitmsg 1
wait -1
UnhookWinEvent hh


#sub Hook_SetWinEventHook
function hHook event hwnd idObject idChild dwEventThread dwmsEventTime

outw hwnd ;;debug

Acc a.FromEvent(hwnd idObject idChild)
a.Role(_s); out _s
out a.Name

typelib IA2 "$qm$\IA2.tlb"
def IID_IAccessible2 uuidof(IA2.IAccessible2)

_hresult=0
IServiceProvider pService
int hr=a.a.QueryInterface(IID_IServiceProvider &pService)
out hr
outx _hresult
IAccessible2 pIA2
pService.QueryService(IID_IAccessible2, uuidof(IA2.IAccessible2), &pIA2);
 pService.QueryService(IID_IAccessible2, uuidof(IA2.IAccessibleApplication), &pIA2);
outx _hresult
out "%i %i" a.a pIA2

err+ out _error.description
