function [button] [offsetx] [offsety] ;;button: 0 move, 1 left, 2 right, 3 middle, 4 left double.

 Moves mouse pointer to this object. Optionally clicks.

 button - mouse button. See above. If 0, does not click.
 offsetx, offsety - x and y offsets from top-left corner of the object. Default: somewhere near top-left.


if(!a) end ERR_INIT
int x y cx cy
a.Location(&x &y &cx &cy elem)

if(getopt(nargs)>1) x+offsetx; y+offsety
else
	if(cx>60) cx=60
	if(cy>60) cy=60
	x+cx/2; y+cy/2

int w=GetAncestor(child(a) 2)
DpiScreenToClient w +&x

spe -1
opt slowmouse -1
sel button
	case 0 mou x y w 1
	case 1 lef x y w 1
	case 2 rig x y w 1
	case 3 mid x y w 1
	case 4 dou x y w 1
	case else end ERR_BADARG

err+ end _error
