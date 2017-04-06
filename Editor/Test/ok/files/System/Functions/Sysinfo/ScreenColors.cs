 /
function#

 Gets screen color depth, bits/pixel.

 Added in: QM 2.3.0.


int dc=GetDC(0)
_i=GetDeviceCaps(dc BITSPIXEL)
ReleaseDC(0 dc)
ret _i
