 /
function# hwnd monitor [flags] [x] [y] ;;flags: 1 same offset, 2 center, 32 monitor is hmonitor.  monitor: 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle

 Moves window to specified monitor.
 Returns 1 if moved, 0 if the window already is in the monitor.
 Does not throw errors.

 hwnd - window handle.
 monitor - see above.
 flags:
   1 - the window will have the same coordinates in the monitor as in the current monitor, plus x y.
   2 - use x y like with mes, ShowDialog, etc. If 0, center, if >0, offset from left/top, if <0, offset from right/bottom. 
 x, y - move to these coordinates in the monitor, if flags 1 or 2 not used.

 REMARKS
 Correctly moves even if the window is minimized or maximized.
 Does not activate the window. Does not restore if minimized.


RECT r; int m m2 fl=flags&32|1|2|4

m=MonitorFromWindow(hwnd 0)
m2=MonitorFromIndex(monitor flags&32)
if(m=m2) ret

if(flags&1 and m)
	WINDOWPLACEMENT p.Length=sizeof(p)
	GetWindowPlacement hwnd &p
	r=p.rcNormalPosition
	XyNormalToMonitor m x y 33
	OffsetRect &r x y
else
	if(flags&2) fl~4
	r.left=x; r.top=y

AdjustWindowPos hwnd &r fl monitor

EnsureWindowInScreen hwnd 1|4|32 m2
ret 1
