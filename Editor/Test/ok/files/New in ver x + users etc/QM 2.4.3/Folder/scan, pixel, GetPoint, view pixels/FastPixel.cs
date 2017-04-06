 /
function x y hwnd [flags] ;;flags: 1 client area

 Gets pixel color from window.
 Almost same as pixel(x y hwnd flags), but faster when Aero theme is enabled.
 When Aero is enabled, works in background windows.
 Does not activate the window.
 If hwnd is 0, gets from screen, slow.


__Hdc dc
if(!hwnd) dc=GetDC(0); ret GetPixel(dc x y)

if(!IsWindow(hwnd)) end ERR_WINDOW
dc=iif((flags&1) GetDC(hwnd) GetWindowDC(hwnd))
__MemBmp m.Create(1 1 dc x y) ;;GetPixel fails if not in clipping region
ret GetPixel(m.dc 0 0)
