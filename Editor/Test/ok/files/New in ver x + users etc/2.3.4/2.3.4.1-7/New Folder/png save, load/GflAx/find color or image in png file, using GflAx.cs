 file, pixel
str sFile="$desktop$\qm test png.png"
 int color=0xFFE0C0
int color=pixel(xm ym) ;;color from mouse

 ________________________

 open file
typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.LoadBitmap(_s.expandpath(sFile))

 get bitmap handle
IPicture p=g.GetPicture
int hb; p.get_Handle(hb)

 find color in bitmap (in memory)
RECT r
if scan(F"color:{color}" hb r 0x280)
	out "color 0x%06X found at x=%i y=%i" color r.left r.top
else out "not found"

 to find image: if scan("file.bmp" hb r 0x280)
