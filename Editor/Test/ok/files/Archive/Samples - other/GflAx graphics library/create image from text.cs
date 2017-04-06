str s="Text"
str sFile="$desktop$\text.gif"

typelib GflAx {059321F1-207A-47A7-93A1-29CDF876FDD3} 1.0
GflAx.GflAx g._create
g.FontSize=20
int width=g.GetTextWidth(s)+8 ;;image width will depend on text
g.NewBitmap(width 24 0xffffff)
g.TextOut(s 4 2 0xff) ;;red
g.SaveFormat=2 ;;gif
g.EnableLZW=TRUE ;;needed for gif
g.SaveBitmap(_s.expandpath(sFile))

run sFile
