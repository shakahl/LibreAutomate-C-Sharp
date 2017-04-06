out
int w=win("DebugView" "dbgviewClass")
RECT r
GetWindowRect w &r
 GetClientRect w &r
zRECT &r
DpiScale +&r 2
zRECT r
DpiScale +&r -2
zRECT r

out DpiGetWindowRect(w &r)
zRECT r

 out DpiScreenToClient(w +&r 0)
out DpiScreenToClient(w +&r 0|0x100)
zRECT r
 out DpiClientToScreen(w +&r 0)
out DpiClientToScreen(w +&r 0|0x100)
zRECT r
