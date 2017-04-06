 /
function# x y width height gridSize [lineThickness] [lineColor] [transparency] [hwnd]

 Shows grid on screen.
 Returns grid window handle. It can be used with OnScreenDrawEnd, or with another OnScreenGrid call (to change grid properties).
 The thread must then wait.

 x, y, width, height - where to show grid.
 gridSize - width and height of grid cells.
 lineThickness, lineColor - properties of grid lines.
 transparency - transparency of grid lines. Can be 1 (almost transparent) to 255 (opaque). If 0, does not cange.
 hwnd - handle of grid window, returned by previous OnScreenGrid call. Use if you want to dynamically change grid properties.


type OSDGRID gridSize lineThickness lineColor

if(transparency and hwnd) Transparent hwnd transparency GetSysColor(COLOR_BTNFACE) ;;OnScreenDraw doesn't change it

ret OnScreenDraw(x y width height &OSD_ProcGrid &gridSize transparency 1 sizeof(OSDGRID) hwnd)
