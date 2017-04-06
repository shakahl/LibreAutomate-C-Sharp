 /dlg_apihook
function hdc [y] [mx] [my]
if(!mx) mx=1
if(!my) my=1

int LH=30*my
RECT r; SetRect &r 50*mx 0 300*mx LH
OffsetRect &r 0 y*my

#compile "__Gdip"
_i=&GDIP.GdipDrawString; err ret

GdipGraphics g.FromHDC(hdc)
GdipFont f.Create("Verdana" 10*abs(my))
GdipBrush b.Create(0xc000c0e0)
GDIP.RectF rr

OffsetRect &r 0 LH+(10*my)
rr.X=r.left; rr.Y=r.top;; rr.Width=50*abs(mx); rr.Height=10*abs(my)

GDIP.GdipDrawString(g L"CALL GdipDrawString" -1 f &rr 0 b)
OffsetRect &r 0 LH+(10*my)

GDIP.PointF k1 k2; k1.X=r.left; k1.Y=r.top; k2.X=r.left+(20*mx); k2.Y=r.top+(5*my)
GDIP.GdipDrawDriverString(g L"CALL GdipDrawDriverString" 2 f b &k1 GDIP.DriverStringOptionsCmapLookup 0)
OffsetRect &r 0 LH+(10*my)

 draws flipped
