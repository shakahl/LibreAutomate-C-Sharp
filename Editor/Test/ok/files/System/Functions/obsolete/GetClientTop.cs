 /
function# hwnd

 Gets y offset of window client area.

 hwnd - window handle. Can be top-level or child window.


if(!hwnd) ret
RECT r; GetWindowRect(hwnd &r)
ScreenToClient hwnd +&r
ret -r.top
