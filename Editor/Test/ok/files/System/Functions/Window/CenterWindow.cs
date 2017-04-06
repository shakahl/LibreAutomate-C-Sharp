 /
function hwnd [x] [y] [monitor] [flags] ;;monitor: 0 same or nearest, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle.  flags: 32 monitor is hmonitor.

 Moves window to the screen center or to a specified position.
 Adjusts the coordinates so that full window is visible.
 If minimized or maximized, makes normal.
 Does not activate the window.
 Does not throw errors.

 hwnd - window handle.
 x, y - coordinates where to move, in the work area of the monitor where the window will be.
   If 0 or omitted - center.
   If negative - offset from screen right and/or bottom.
 monitor - if not 0, move the window to the monitor.


if(!res(hwnd)) SetWindowState hwnd 4 1

RECT r; r.left=x; r.top=y
AdjustWindowPos hwnd &r flags&32|3 iif(monitor monitor hwnd)

err+
