int hwnd=val(_command)

POINT pm pf
xm pm
RECT r; GetWindowRect hwnd &r; pf.x=r.left+r.right/2; pf.y=r.top+r.bottom/2

 calculate distance from flick icon (it is where flick started) to current mouse position
int dx(pm.x-pf.x) dy(pm.y-pf.y)
int dist=_hypot(dx dy)
if(dist<100) ret ;;it is not flick icon window. The window also is used for other purposes, eg to show click.

 calculate flick angle
int angle=Round(atan2(-dy dx)*57.2957795130786) ;;*(180/pi)
if(angle<0) angle=360+angle
out angle

 Using this angle, calculate which flick it was.
 For example, 'up' is around 90 degrees, 'left' ~180, 'down' ~270, 'right' >=0 or <360.
