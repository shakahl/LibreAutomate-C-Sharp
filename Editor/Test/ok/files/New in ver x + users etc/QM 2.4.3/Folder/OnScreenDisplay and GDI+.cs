str text=
 This
 is multiline
 text
 that's
 centered

RECT r; r.right=500; r.bottom=400
__MemBmp m.Create(r.right r.bottom)

sub.Draw m.dc r text

 save in a temp bmp file
__TempFile tf.Init(".bmp")
SaveBitmap(m.bm tf)
m.Delete

 OSD with the bmp file
OnScreenDisplay "" 5 0 0 "" 0 0 128 "" 0 0 tf


#sub Draw
function hdc RECT&r $text

#compile "__Gdip"
GdipGraphics g; GdipBrush b; GdipPen p; GdipFont f; GdipBitmap bm

 let the Graphics object draw in this DC
if(!g.FromHDC(hdc)) ret ;;if fails, probably gdiplus.dll is missing

 fill with a color
g.Clear(GetSysColor(COLOR_BTNFACE)|0xff000000) ;;this color makes empty parts transparent

 fill rectangle with linear gradient brush
int gx(r.right*0.7) gy(r.bottom*0.6)
 b.CreateGradient(0 0 gx 1 ColorARGB(160 200 0 0) ColorARGB(160 200 0 160)) ;;horizontal
 b.CreateGradient(0 0 1 gy ColorARGB(160 200 0 0) ColorARGB(160 200 0 160)) ;;vertical
b.CreateGradient(0 0 gx gy ColorARGB(160 200 0 0) ColorARGB(160 200 0 160)) ;;diagonal
GDIP.GdipFillRectangleI g b 0 0 gx gy

 ellipse
p.Create(ColorARGB(0 0 255 128) 20)
GDIP.GdipDrawEllipseI g p 50 50 150 75

 line and rectangle
GDIP.GdipSetPenColor p 0x80ff00ff ;;uses hex ARGB color format, where the 80 is 128 alpha, and the ff is 255 red and blue
GDIP.GdipDrawLineI g p 10 60 250 120
GDIP.GdipDrawRectangleI g p.Create(0x40ff0000) 140 200 100 50

 text
GDIP.RectF rt
rt.X=10; rt.Y=10
f.Create("Verdana[]Helvetica" 20 3)
GDIP.GdipDrawString g @text -1 f &rt 0 b.Create(0xc000c0e0)

 fill rectangle with solid brush
GDIP.GdipFillRectangleI g b.Create(0xff808080) 300 10 100 50

 fill rectangle with hatch brush
b.CreateHatch(GDIP.HatchStyleDottedGrid 0xff000000 0xffffff00)
GDIP.GdipFillRectangleI g b 304 72 96 48

 fill rectangle with texture brush
b.CreateTextureFromFile("$qm$\function.ico")
GDIP.GdipFillRectangleI g b 302 142 100 100

 load and draw bitmap
bm.FromFile("$qm$\il_qm.bmp")
GDIP.GdipDrawImageI g bm 0 300

 craete new bitmap from part of other bitmap, and draw
GdipBitmap bm2.FromBitmapArea(bm 32 0 16 16)
GDIP.GdipDrawImageI g bm2 0 320

 copy bitmap
GdipBitmap bm3.FromBitmapArea(bm2 0 0 bm2.width bm2.height)
GDIP.GdipDrawImageI g bm3 30 320

 create empy bitmap, attach to new graphics, draw in it, create brush from it, and draw filled ellipse with the brush
bm2.CreateEmpty(16 16)
GdipGraphics g2.FromImage(bm2)
GDIP.GdipFillEllipseI g2 b.Create(0xffc0a080) 3 3 10 10
b.CreateTextureFromImage(bm2)
GDIP.GdipFillEllipseI g b 3 195 90 90
