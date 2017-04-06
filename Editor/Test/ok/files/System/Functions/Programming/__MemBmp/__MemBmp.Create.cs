function width height [srcdc] [srcx] [srcy]

 Creates memory DC (device context), creates/selects bitmap, and optionally copies bitmap bits from other DC.

 width, height - bitmap width and height.
 srcdc - a DC from which to copy bitmap bits into this DC. Can be 0.
   If srcdc is 1 or 2, copies from screen. If 1, interprets srcx and srcy as coordinates in primary monitor, else in virtual screen (see <help>GetVirtualScreen</help>).
 srcx, srcx - offset in source DC from where to copy bits.

 QM 2.4.2: Returns 1. Returns 0 if fails to create large bitmap (not enough memory).


Delete

int dcs=GetDC(0)
dc=CreateCompatibleDC(0)
bm=CreateCompatibleBitmap(dcs width height)
if(!bm) Delete; ReleaseDC(0 dcs); ret
oldbm=SelectObject(dc bm)

if(srcdc)
	if(srcdc=2) srcdc=1; int x y; GetVirtualScreen x y; srcx+x; srcy+y
	BitBlt dc 0 0 width height iif(srcdc=1 dcs srcdc) srcx srcy SRCCOPY

ReleaseDC(0 dcs)
ret 1
