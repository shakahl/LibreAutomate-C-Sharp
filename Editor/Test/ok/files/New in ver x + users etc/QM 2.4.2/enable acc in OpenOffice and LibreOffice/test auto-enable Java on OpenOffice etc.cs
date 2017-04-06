 int w=win("LibreOffice" "SAL*FRAME")
 int w=win("OpenOffice" "SAL*FRAME")
int w=win("Stylepad" "SunAwtFrame")
act w
0.2
 Acc a.Find(w "BUTTONDROPDOWN" "Paste" "" 0x1001)
Acc a.Find(w "push button" "Paste clipboard to selection" "" 0x1001)

 2
 out "getting"
 Acc a.FromMouse

 Acc a.FromFocus ;;does not work

a.Role(_s); out _s
out a.Name
a.showRECT
 a.DoDefaultAction
