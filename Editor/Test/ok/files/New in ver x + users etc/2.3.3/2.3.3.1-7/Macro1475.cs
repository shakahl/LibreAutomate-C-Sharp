int w1=win("„Mozilla Firefox“ pradžios tinklalapis - Mozilla Firefox" "Mozilla*WindowClass" "" 0x800)
 act w1
Q &q
Acc a1=acc("„Mozilla Firefox“ pradžios tinklalapis" "DOCUMENT" w1 "" "" 0x1001)
Q &qq
Acc a=acc("Išplėstinė paieška" "LINK" a1 "" "" 0x1001)
Q &qqq
outq
out a.Name
