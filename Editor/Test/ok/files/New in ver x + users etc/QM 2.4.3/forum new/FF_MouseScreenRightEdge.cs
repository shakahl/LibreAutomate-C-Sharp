 /
 Allows starting macro when mouse pointer is by the right edge of the screen, primary monitor.
 For mouse-click and mouse-wheel triggers.

function# iid FILTER&f

int x=ScreenWidth-1
if(f.x=x) ret iid
 if(f.x=x or f.x=x-1) ret iid
ret -2
