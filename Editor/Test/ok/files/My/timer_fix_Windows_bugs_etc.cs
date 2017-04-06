int w idleTime idleTimePrev action
int tick tickPrev(GetTickCount)
rep
	1
	idleTime=GetIdleTime
	
	 fix Windows hibernation bugs
	tick=GetTickCount
	action=tick-tickPrev>=30000 ;;after sleep or hibernamtion
	tickPrev=tick
	if action
		7
		mac "sub.FixWindowsHibernationBugs"
		idleTime=0; idleTimePrev=0
		continue
	
	 prevent Windows starting scheduled tasks when idle
	 if idleTime>60*2+30
		  out "key"
		 key F13
	
	 stop flickering second monitor
	action=idleTime<10 and idleTimePrev>=100
	idleTimePrev=idleTime
	if(action) sub.FixFlickeringSecondMonitor w


#sub FixFlickeringSecondMonitor
function &w

 InvalidateRect(0 0 0) ;;flickers all monitors

RECT r; MonitorFromIndex(2 0 r)
 outRECT r
 InvalidateRect(0 &r 0) ;;flickers all monitors

if !IsWindow(w)
	w=id(1 win("" "WorkerW"))
	if(w=0) end "failed"

InvalidateRect(w &r 0)
InvalidateRect(_hwndqm 0 0)


#sub FixWindowsHibernationBugs

 Workaround Windows bug: after hibernation most maximized windows are smaller or bigger.
RECT r; GetWorkArea r.left r.top r.right r.bottom; r.right+r.left; r.bottom+r.top
ARRAY(int) a
int i
win "" "" "" 0 "" a
for(i a.len-1 -1 -1)
	int w=a[i]
	int isMax=max(w)
	err continue ;;sometimes error "invalid handle"
	if(!isMax) continue
	 outw w
	SetWindowState w 4 1
	SetWindowState w 3 1
	InvalidateRect w 0 1 ;;for Visual Studio


 Workaround Windows bug: after hibernation moves all windows to the primary monitor. This macro moves QM to the second monitor.
MoveWindowToMonitor _hwndqm 2

 Workaround Windows bug: after hibernation makes slow keyboard repeat speed. This macro fixes it.
SystemParametersInfo SPI_SETKEYBOARDSPEED 31 0 0
