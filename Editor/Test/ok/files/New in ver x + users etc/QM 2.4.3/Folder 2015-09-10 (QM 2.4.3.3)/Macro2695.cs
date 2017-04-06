int hh=SetWindowsHookEx(WH_MOUSE_LL &sub.Hook_WH_MOUSE_LL _hinst 0)
mes "Testing mouse hook. Click somewhere..."
UnhookWindowsHookEx hh


#sub Hook_WH_MOUSE_LL
function# nCode message MSLLHOOKSTRUCT&m
if(nCode<0) goto gNext

if(message!=WM_MOUSEMOVE)
	OutWinMsg message 0 0 _s; out "%s at %i %i" _s m.pt.x m.pt.y
	if(message=WM_RBUTTONUP) 0.31

 gNext
ret CallNextHookEx(0 nCode message &m)
