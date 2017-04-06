 /
function monitor int&x int&y [flags] ;;flags: 1 work area, 32 monitor is hmonitor.  monitor: 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle

 Converts monitor coordinates to normal coordinates.

 monitor - see above.
 x, y - variables that contain coordinates in the monitor. The function converts them to normal coordinates, ie relative to the primary monitor.
 flags:
   1 - x y are relative to the work area of the monitor.


RECT r
MonitorFromIndex(monitor flags&33 &r)
x+r.left
y+r.top
