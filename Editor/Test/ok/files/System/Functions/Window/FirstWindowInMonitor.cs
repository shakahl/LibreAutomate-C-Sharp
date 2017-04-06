 /
function# monitor [flags] [hwndAfter] ;;flags: 1 retry from top, 2 skip minimized, 32 monitor is hmonitor.  monitor: 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle

 Returns handle of first window in specified monitor.
 Returns 0 if there are no windows.
 Does not throw errors.

 monitor - 1-based monitor index or -1 (monitor of mouse) or -2 (monitor of active window) or -3 (primary monitor).
 hwndAfter - handle of a window (in any monitor) behind which in Z order to search. Default - first window in Z order.

 REMARKS
 Skips invisible, owned and other windows that would be skipped by Alt+Tab.
 To get first (or next after certain window) window in any monitor, use RealGetNextWindow instead.


monitor=MonitorFromIndex(monitor flags&32)

int h=hwndAfter
rep
	h=RealGetNextWindow(h flags&3); if(!h) ret
	if(MonitorFromWindow(h 0)=monitor) ret h
