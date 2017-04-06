 3
int w=win("" "TaskSwitcherWnd")
 int w=win("" "#32771")
 int w=CreateWindowEx(0 "#32771" 0 WS_VISIBLE|WS_POPUP 0 0 200 100 0 0 _hinst 0)

outw w
ALTTABINFO at.cbSize=sizeof(at)
 out GetAltTabInfo(w -1 &at 0 0)
out GetAltTabInfo(w 0 &at _s.all(1000) 1000)

 DestroyWindow w

 Does not work.
