 /
function [int&x] [int&y] [int&width] [int&height]

 Gets bounding rectangle of all monitors.

 x, y, width, height - variables for the rectangle. Can be 0.

 REMARKS
 If there is only 1 monitor, x and y will be 0, width will be the same as returned by ScreenWidth, height will be the same as returned by ScreenHeight.
 x and/or y will be negative if there are monitors at left and/or top from the primary monitor.


if(&x) x=GetSystemMetrics(SM_XVIRTUALSCREEN)
if(&y) y=GetSystemMetrics(SM_YVIRTUALSCREEN)
if(&width) width=GetSystemMetrics(SM_CXVIRTUALSCREEN)
if(&height) height=GetSystemMetrics(SM_CYVIRTUALSCREEN)
