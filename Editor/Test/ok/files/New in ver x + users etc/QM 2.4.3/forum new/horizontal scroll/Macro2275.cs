 MouseWheelH 1

 int w=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x4)
 MouseWheelH 1 w

 MouseWheelH2 1 0 1|0

 int w=child(mouse)
 MouseWheelH2 1 w

int w=win("Downloads" "CabinetWClass")
Acc a.Find(w "PUSHBUTTON" "Page right" "class=ScrollBar" 0x25)
if(a.a) a.DoDefaultAction
