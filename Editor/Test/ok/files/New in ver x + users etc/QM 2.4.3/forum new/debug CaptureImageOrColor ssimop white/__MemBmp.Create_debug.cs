function width height [srcdc] [srcx] [srcy]

 Creates memory DC (device context), creates/selects bitmap, and optionally copies bitmap bits from other DC.

 width, height - bitmap width and height.
 srcdc - a DC from which to copy bitmap bits into this DC. Can be 0.
   If srcdc is 1 or 2, copies from screen.
   If it is 1, interprets srcx and srcy as coordinates in primary monitor.
   If it is 2, interprets srcx and srcy as coordinates in virtual screen (see GetVirtualScreen).
 srcx, srcx - offset in source DC from where to copy bits.


Delete

int dcs=GetDC(0)
if(!dcs) end "GetDC" 17
dc=CreateCompatibleDC(0)
if(!dc) end "CreateCompatibleDC" 17
bm=CreateCompatibleBitmap(dcs width height)
if(!bm) end "CreateCompatibleBitmap" 17
oldbm=SelectObject(dc bm)
if(!oldbm) end "CreateCompatibleBitmap" 17

if(srcdc)
	if(srcdc=2) srcdc=1; int x y; GetVirtualScreen x y; srcx+x; srcy+y
	_i=BitBlt(dc 0 0 width height iif(srcdc=1 dcs srcdc) srcx srcy SRCCOPY)
	if(!_i) end "BitBlt" 17

ReleaseDC(0 dcs)
