POINT p p2
xm p
wait 2 ML; err goto g1
ret
 g1
xm p2
if(_hypot(p2.x-p.x p2.y-p.y)>3) ret

OnScreenDisplay "/\/\/\/\/"
