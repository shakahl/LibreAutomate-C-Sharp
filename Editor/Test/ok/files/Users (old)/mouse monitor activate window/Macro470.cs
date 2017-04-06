 When you move mouse into a monitor, activates first window in that monitor.

int mon pmon
rep
	1
	POINT p; xm p
	mon=MonitorFromPoint(p MONITOR_DEFAULTTONULL)
	if(mon=pmon or !mon) continue
	if(pmon)
		 mouse moved into another monitor
		 out mon
		int h=win
		if(MonitorFromWindow(h MONITOR_DEFAULTTONULL)!=mon and h!=win(mouse))
			 out 1
			ifk((1)) continue ;;maybe moving a window
			h=GetFirstWindowInMonitor(h mon)
			if(h) act h
	pmon=mon
	