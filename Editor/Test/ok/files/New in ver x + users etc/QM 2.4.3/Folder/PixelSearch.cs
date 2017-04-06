function# SX SY EX EY color [VAR] [POINT&p]

 Searches a region of the screen for a pixel of the specified color.
 Returns: 1 found, 0 not.

 SX, SY - top-left coord of the search region relative to the active window.
 EX, EY - bottom-right coord. The point is inside the search region.
 color - search for this pixel color, in 0xBBGGRR format.
 VAR - allowed color component difference, 0-255.
 p - optional POINT variable that receives found pixel coordinates in screen.

 EXAMPLE
 POINT p
 if PixelSearch(552 21 592 51 0x2f59fd 0 p)
 	mou p.x p.y


opt noerrorshere 1
RECT r; SetRect &r SX SY EX+1 EY+1
if(!scan(F"color:{color}" win r 0 VAR)) ret
if(&p) p.x=r.left; p.y=r.top
ret 1
