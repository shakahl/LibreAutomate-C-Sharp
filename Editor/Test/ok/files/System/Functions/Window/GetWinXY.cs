 /
function# hwnd [int&x] [int&y] [int&width] [int&height] [hwndparent]

 Gets window position and size.
 Returns: 1 success, 0 failed.

 hwnd - window handle. Can be top-level or child window.
 x, y, width, height - variables for position and size. Can be 0.
 hwndparent - parent window handle. If used, x y will be in its client area, else in screen.

 See also: <DpiGetWindowRect>, <GetWindowRect>, <GetClientRect>.

 EXAMPLE
 int x y cx cy
 GetWinXY(win("Quick Macros") x y cx cy)
 out "%i %i %i %i" x y cx cy


RECT r
if(!GetWindowRect(hwnd &r)) ret
if(hwndparent) MapWindowPoints(0 hwndparent +&r 2)
if(_winver<0x603 && DpiIsWindowScaled(hwnd)) DpiScale +&r 2
if(&x) x=r.left
if(&y) y=r.top
if(&width) width=r.right-r.left
if(&height) height=r.bottom-r.top
ret 1
