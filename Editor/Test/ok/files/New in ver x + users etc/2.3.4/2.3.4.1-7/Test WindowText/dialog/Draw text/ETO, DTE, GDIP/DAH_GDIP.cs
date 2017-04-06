 /dlg_apihook
function hdc CLogicalCoord&c

_i=&GDIP.GdipDrawString; err ret ;;XP
POINT _p; GetViewportOrgEx hdc &_p ;;GdipDrawString resets it

int LH=30*c.ey

#compile "__Gdip"

GdipGraphics g.FromHDC(hdc)
GdipBrush b.Create(0xc000c0e0)
GdipFont f.Create("Verdana" 10*c.ex)
GDIP.RectF r; r.X=c.x; r.Y=c.y ;;r.Width=c.r-c.x; r.Height=c.b-c.y
 r.Width=50
 r.Height=30
BSTR s="CALL[]GdipDrawString"
 BSTR s="[]CALL[]&GdipDrawString[]"
GDIP.GdipDrawString(g s -1 f &r 0 b)

c.Offset(200 10)
GDIP.PointF k1 k2; k1.X=c.x; k1.Y=c.y; k2.X=c.x+(20*c.ex); k2.Y=c.y+(5*c.ey)
GDIP.GdipDrawDriverString(g L"CALL GdipDrawDriverString" 2 f b &k1 GDIP.DriverStringOptionsCmapLookup 0)
 GDIP.GdipDrawDriverString(g L"[46][26]" 2 f b &k1 0 0) ;;glyphs for "K7"
 why draws above, ~10 px? Does not depend on place and on whether there is menu bar.

SetViewportOrgEx hdc _p.x _p.y 0
c.Offset(-200 30)

 see also: GdipAddPathString[I]/GdipDrawPath
