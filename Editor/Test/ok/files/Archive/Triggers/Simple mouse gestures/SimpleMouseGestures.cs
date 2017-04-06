 /
function# [$macroList] [flags] [minDistance] [^timeout_s] ;;flags: 4 four gestures

 Detects 8 simple mouse gestures: right, right-up, up, ...
 Call this function from a function that has trigger:
    Mouse right or middle button.
    "Eat" checked, "When release" unchecked.
 Waits until you release mouse button, detects drag direction, draws blue line, and optionally launches a macro.
 Returns drag direction, from 0 (~0 degrees) to 7 (~360-45 degrees).
 Fails if you did not release button within timeout_s seconds, or did not drag >= minDistance.
 If fails, clicks in the initial place and returns -1.

 macroList - list of macros.
    Example: "m0[]m1[]m2[]m3[]m4[]m5[]m6[]m7"
    First macro will run on gesture rigt (~0 degrees), second - right-up (~ 45 degrees), and so on.
    Can be less than 8 lines. Some lines can be empty. Not error if a macro does not exist.
    If macroList is empty, does not run a macro.
 flags:
    4 - detect 4 gestures. Return: 0 rigt, 1 up, 2 left, 3 down.
 minDistance - fail if dragged by less pixels. Default: 10.
 timeout_s - fail after timeout_s seconds. Default: 2.


QMITEM q; qmitem(getopt(itemid 3) 0 q)
int mb=q.tkey-7
if(q.ttype!2 or mb<0 or mb>1) end "must be mouse right or middle button trigger"
if(q.itype=0) end "must be function, not macro"

if(!minDistance) minDistance=10
if(!timeout_s) timeout_s=2

POINT p0; xm p0

int e
opt waitmsg 1
SMG_Draw 0 1 0 p0
wait timeout_s M; err e=1
SMG_Draw 0 2 0 0
if e
	goto g1

POINT p; xm p
int xd(p.x-p0.x) yd(p0.y-p.y)

if(_hypot(xd yd)<minDistance) e=2; goto g1

int d=AngleDegrees(xd yd)
if(flags&4) d=Round(d/90.0)&3; else d=Round(d/45.0)&7

if(!empty(macroList))
	ARRAY(str) a=macroList
	if(d<a.len)
		_s=a[d]
		if(_s.len) mac _s; err

ret d

 g1
spe 10
if e=1
	if(mb) mid+ p0.x p0.y 0 4
	else rig+ p0.x p0.y 0 4
else
	if(mb) mid p0.x p0.y 0 4
	else rig p0.x p0.y 0 4
ret -1
