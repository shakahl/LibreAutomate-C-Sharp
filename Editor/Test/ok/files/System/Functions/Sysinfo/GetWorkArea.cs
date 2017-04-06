 /
function int&x int&y [int&width] [int&height] [flags] [monitor] ;;flags: 1 get screen coordinates, 32 hmonitor.  monitor: 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle

 Gets work area.
 Usually it is whole screen except taskbar.

 x, y, width, height - variables for work area. Can be 0.
 flags:
   1 - get whole screen instead.
   32 (QM 2.4.3) - monitor is monitor handle.

 See also: <MonitorFromIndex>.

 EXAMPLE
 int waL waW
 GetWorkArea waL 0 waW
 out waL; out waW


RECT r
MonitorFromIndex(monitor flags&0x21^1 &r)

if(&x) x=r.left
if(&y) y=r.top
if(&width) width=r.right-r.left
if(&height) height=r.bottom-r.top
