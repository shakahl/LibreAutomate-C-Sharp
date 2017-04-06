 int w=win("" "QM_Editor")
int w=win("Firefox" "Mozilla*WindowClass" "" 0x4)
Acc a.Find(w "div" "" "" 0x3010 3)
a.WebPageProp(_s)
out _s
