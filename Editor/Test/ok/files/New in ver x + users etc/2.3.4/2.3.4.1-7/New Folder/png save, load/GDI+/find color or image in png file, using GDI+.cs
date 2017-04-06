 file, pixel
str sFile="$desktop$\qm test png.png"
 int color=0xFFE0C0
int color=pixel(xm ym) ;;color from mouse

 ________________________

 open file
#compile "__Gdip"
GdipBitmap im
if(!im.FromFile(sFile)) end "error"

 get bitmap handle
__GdiHandle hb=im.GetHBITMAP

 find color in bitmap (in memory)
RECT r
if scan(F"color:{color}" hb r 0x280)
	out "color 0x%06X found at x=%i y=%i" color r.left r.top
else out "not found"

 to find image: if scan("file.bmp" hb r 0x280)
