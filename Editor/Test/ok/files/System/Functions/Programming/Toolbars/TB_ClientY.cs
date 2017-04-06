function#

 Can be used in toolbars, to set initial position relative to client area of owner window.

 EXAMPLE (first line of toolbar text)
 /mov 200 TB_ClientY-20


int h=val(_command)
RECT r; GetWindowRect(h &r)
ScreenToClient h +&r
if(_winver<0x603 && DpiIsWindowScaled(h)) DpiScale +&r 1
ret -r.top
