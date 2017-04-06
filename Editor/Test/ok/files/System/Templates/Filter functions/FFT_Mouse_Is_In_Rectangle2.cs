 /
 Allows starting macro when mouse pointer is in certain rectangle area in certain window, relative to the client area.
 For mouse-click and mouse-wheel triggers.

function# iid FILTER&f

RECT r; SetRect &r 0 -20 50 30 ;;change these values (left, top, right, bottom; relative to the client area of f.hwnd)

if(!wintest(f.hwnd "WindowName" "WindowClass")) ret -2 ;;change window name and class. Delete this line to make it work in any window.
RECT z; DpiGetWindowRect f.hwnd &z 8 ;;gets client rectangle in screen
OffsetRect &r z.left z.top ;;to change origin, change r.left to r.right and/or r.top to r.bottom.

if(PtInRect(&r f.x f.y)) ret iid
ret -2
