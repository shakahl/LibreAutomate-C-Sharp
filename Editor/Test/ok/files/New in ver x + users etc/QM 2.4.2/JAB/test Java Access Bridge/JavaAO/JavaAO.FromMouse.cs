function!

 Gets Java accessible object from mouse.
 Returns: 1 success, 0 failed. Also may throw error, but not error if there is no accessible Java window there.


opt noerrorshere 1
POINT p; xm p
ret FromXY(p.x p.y)
