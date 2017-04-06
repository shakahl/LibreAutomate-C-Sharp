int w=win("" "Shell_TrayWnd")
Acc a.Find(w "PUSHBUTTON" "Quick Macros" "class=ToolbarWindow32" 0x1005)
int x y
a.Location(x y)
ShowTooltip "toltip" 3 x+10 y 0 1