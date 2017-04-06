 file, pixel
str sFile="$desktop$\qm test png gdip.png"
 int x(8) y(100)
int x(xm) y(ym) ;;from mouse

 ________________________

 open file
#compile "__Gdip"
GdipBitmap im
if(!im.FromFile(sFile)) end "error"

 get pixel color
int color
GDIP.GdipBitmapGetPixel(+im x y &color)
color=ColorToARGB(color 0) ;;0xAARRGGBB to 0xBBGGRR
out "0x%06X" color
