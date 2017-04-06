 file, pixel
str sFile="$desktop$\qm test png.png"
int x(8) y(100)
 int x(xm) y(ym) ;;from mouse

 ________________________

 open file
typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.LoadBitmap(_s.expandpath(sFile))

 get pixel color
int color=g.GetColorAt(x y)
out "0x%06X" color
