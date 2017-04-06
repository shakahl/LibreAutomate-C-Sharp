 /
 Allows starting macro when mouse pointer is in certain rectangle area in the screen.
 For mouse-click and mouse-wheel triggers.

function# iid FILTER&f

RECT r; SetRect &r 0 0 100 100 ;;change these values (left, top, right, bottom)

if(PtInRect(&r f.x f.y)) ret iid
ret -2
