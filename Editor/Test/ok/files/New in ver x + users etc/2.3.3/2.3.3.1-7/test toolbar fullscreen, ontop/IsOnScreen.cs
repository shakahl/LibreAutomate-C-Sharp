 /dialog_detect_fullscreen
function! hTB

ifk-(C) ret
 ifk-(J 1) ret

int w=GetForegroundWindow
int hmFore hmTB
RECT r rc
 GetWindowRect(w &r)
Q &q
 hmFore=MonitorFromRect(&r 0); if(!hmFore) ret ;;slower
hmFore=MonitorFromWindow(w 0); if(!hmFore) ret
hmTB=MonitorFromWindow(hTB 0); if(hmTB!hmFore) ret
Q &qq
 out "%i %i" hmFore hmTB

MONITORINFO mi.cbSize=sizeof(mi)
if(!GetMonitorInfo(hmFore &mi)) ret
outw w
Q &qqq
GetWindowRect(w &r)
zRECT r
GetClientRect w &rc
zRECT rc
Q &qqqq
int wDesk=GetShellWindow
Q &qqqqq
 int isDesktop=(w=wDesk or GetWindowThreadProcessId(w 0)=GetWindowThreadProcessId(wDesk 0))
int isDesktop=(GetWindowThreadProcessId(w 0)=GetWindowThreadProcessId(wDesk 0))
Q &qqqqqq

 zRECT mi.rcMonitor
 outq
if(isDesktop) ret

 if(

 if(!w or r.left>0 or r.top>0) continue
 outw w
 zRECT r
 out "max=%i  ont=%i  caption=%i  border=%i" max(w) ont(w) GetWinStyle(w)&WS_CAPTION=WS_CAPTION memcmp(&r &rc 16)!0

 out "
TO_WinStyleString _s GetWinStyle(w) GetWinStyle(w 1)
out _s
