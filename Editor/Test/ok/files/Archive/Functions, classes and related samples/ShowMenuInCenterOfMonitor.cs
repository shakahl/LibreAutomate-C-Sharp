 /
function# $menu monitor [yOffset] [flags] ;;monitor: 0 primary, 1-30 index, -1 mouse, -2 active window, -3 primary, or window handle.  flags: 1 sync

 Shows popup menu in center of specified monitor.
 Returns 0. If flag 1 used and an item selected, returns 1-based line index.
 Error if menu does not exist.

 menu - menu name.
 yOffset - offset in pixels from screen center vertically. Typically it should be minus half of menu height.
 flags:
   1 - wait until the menu is closed.

 EXAMPLE
 ShowMenuInCenterOfMonitor "Menu16" -1 -100


RECT r
MonitorFromIndex monitor 1 &r
int x(r.left+r.right/2) y(r.top+r.bottom/2+yOffset)
if(flags&1) ret mac(menu "" x y)
mac menu "" x y
err+ end _error
