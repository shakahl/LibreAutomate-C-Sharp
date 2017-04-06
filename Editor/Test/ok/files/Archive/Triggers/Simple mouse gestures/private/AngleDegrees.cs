 /
function# x y

 Returns angle in degrees, 0-359.

int d=Round(atan2(y x)*57.2957795130786) ;;*(180/pi)
if(d<0) d=360+d
ret d
