 /
function RECT'r hwnd [double'timeS] [flags] ;;flags: 1 not client area

 Shows a rectangle relative to a window or control.
 By default, relative to the client area.

 r - rectangle.
 hwnd - window/control handle.
 timeS - time in seconds. Default 2.


if hwnd
	if(flags&1) RECT k; GetWindowRect hwnd &k; OffsetRect &r k.left k.top
	else MapWindowPoints hwnd 0 +&r 2

__OnScreenRect osr.SetStyle(0x8000 1)
osr.Show(1 r)
wait iif(timeS timeS 2.0)
osr.Show(2 r)
