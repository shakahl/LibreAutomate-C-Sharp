 finds image "Macro517.bmp" in system notification area (tray) and moves mouse

out
Acc a=acc("Notification Area" "TOOLBAR" win("" "Shell_TrayWnd") "ToolbarWindow32" "" 0x1001)
if(!a.FindImage("Macro517.bmp" 0 0x3)) ret
out "found"
