int hh=SetWindowsHookEx(WH_MOUSE_LL &sub.Hook _hinst 0)
mes "Testing mouse hook. Click somewhere..."
UnhookWindowsHookEx hh



#sub Hook
function# nCode message MSLLHOOKSTRUCT&m
if(nCode<0) goto gNext

OutWinMsg message 0 0 _s; out "%s at %i %i" _s m.pt.x m.pt.y

 your code here

 gNext
ret CallNextHookEx(0 nCode message &m)
