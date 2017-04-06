out
 int w=win("Untitled - Notepad" "Notepad")
int w=win("DebugView" "dbgviewClass")
SetWinStyle w WS_EX_LAYOUTRTL 1|4|8|16

POINT p pp
 ClientToScreen w &p; out p.x
 DpiClientToScreen w &pp; out pp.x

 p.x=1668; pp=p
 ScreenToClient w &p; out p.x
 DpiScreenToClient w &pp; out pp.x

RECT r rr
SetRect &r 0 0 100 100
rr=r
MapWindowPoints w 0 +&r 2
 MapWindowPoints w 0 +&r.right 1
zRECT r
DpiClientToScreen w +&rr 0x100
 DpiClientToScreen w +&rr.right
zRECT rr
DpiScreenToClient w +&rr 0x100
 DpiScreenToClient w +&rr.right
zRECT rr

 L=1668 T=633 R=-1568 B=733   W=-3236 H=100
