 finds blue color in system notification area (tray) and moves mouse

out
Acc a=acc("Notification Area" "TOOLBAR" win("" "Shell_TrayWnd") "ToolbarWindow32" "" 0x1001)
if(!a.FindColor(ColorFromRGB(0 0 255) 0 0x3)) ret
out "found"

 RECT m.left=14
 out a.FindColor(0xff 0 3 m)
