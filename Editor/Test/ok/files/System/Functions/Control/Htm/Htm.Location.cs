function [int&x] [int&y] [int&width] [int&height] [_]

 Gets position (in screen) and size.

 x, y, width, height - variables for position and size. Can be 0.

 REMARKS
 QM 2.3.3. Does not use the 5-th parameter.


if(!el) end ERR_INIT
opt noerrorshere 1

RECT r; GetRect(r)
if(&x) x=r.left
if(&y) y=r.top
if(&width) width=r.right-r.left
if(&height) height=r.bottom-r.top
