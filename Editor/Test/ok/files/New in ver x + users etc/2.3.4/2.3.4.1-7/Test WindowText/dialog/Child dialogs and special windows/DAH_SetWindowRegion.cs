 /
function w x y cx cy

RECT rw; GetWindowRect w &rw; OffsetRect &rw -rw.left -rw.top
__GdiHandle hw=CreateRectRgnIndirect(&rw)
int he=CreateEllipticRgn(x y x+cx y+cy)
CombineRgn(he hw he RGN_DIFF)
 CombineRgn(he hw he RGN_AND)
SetWindowRgn(w he 0)
