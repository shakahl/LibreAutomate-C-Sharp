out
int w=win("QM TOOLBAR" "QM_toolbar")
 Acc a.Find(w "PUSHBUTTON" "Mouse" "class=ToolbarWindow32[]id=9999" 0x1005)
Acc a.Find(w "PUSHBUTTON" "Mouse" "level=1 3" 0x1005)
out a.Name
