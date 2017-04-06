 /
function# x y x2 y2 [color] [linewidth] [hwnd]

 Draws line on screen.
 Returns draw window handle.

 x y x2 y2 - starting and ending points.
 color - color. See ColorFromRGB. Default: black.
 linewidth - line width. Default: 1.


type OSDLINE x y x2 y2 color linewidth
OSDLINE a
a.color=color
a.linewidth=iif(linewidth<1 1 linewidth)
a.x=x; a.y=y; a.x2=x2; a.y2=y2

int cx(x2-x) cy(y2-y)

 adjust rect coords if cx or cy negative
if(cx<0) cx=-cx; x-cx
if(cy<0) cy=-cy; y-cy
 inflate rect by half of line width
int c=a.linewidth/2
x-c; y-c; cx+a.linewidth; cy+a.linewidth

ret OnScreenDraw(x y cx cy &OSD_LineProc &a 0 1 sizeof(OSDLINE) hwnd)
