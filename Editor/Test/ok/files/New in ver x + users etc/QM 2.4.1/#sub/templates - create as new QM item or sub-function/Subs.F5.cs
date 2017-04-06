out 3
 Sub1


#sub Subs.F3 c
out 3
 Sub1


#sub Hook_WH_MOUSE_LL
function# nCode message MSLLHOOKSTRUCT&m
if(nCode<0) goto gNext



 gNext
ret CallNextHookEx(0 nCode message &m)


#sub F3 c
out 3
 Sub1
