 run "$windows$\SysWOW64\mspaint.exe" "" "" "" 0x800 win("Untitled - Paint" "MSPaintApp")

 int w=win("Untitled - Paint" "MSPaintApp")
 scan "image:h2979CDD4" w 0 1|2|16

int w=win("Untitled - Paint" "MSPaintApp")
scan "image:h2979CDD4" w 0 1|2|16|0x1100

 int w=win("Untitled - Paint" "MSPaintApp")
 Acc a.Find(w "LISTITEM" "Four-point star" "class=NetUIHWND" 0x1005)
 a.Mouse(1)
