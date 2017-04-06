 /
function# nCode remove MSG&m
if(nCode<0) goto gNext



 gNext
ret CallNextHookEx(0 nCode remove &m)

 note: cannot hook windows of other processes.
