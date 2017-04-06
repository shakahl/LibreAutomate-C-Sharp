 lef
lef 0.5 0.5 0 1
lef 9 5.9
lef (h + 8) 0.5 "+Shell_TrayWnd" 1
lef 389 327 "Program Manager"
lef+ 328 -8 "Live365 - Player Window - Maxthon" 1
lef-
dou 953 70
rig h t
rig+
rig-
rig 251 479 child("" "Internet Explorer_Server" "Live365 - Player Window - Maxthon" 0x1)
mid 453 25 "Live365 - Player Window - Maxthon"
mou 8 11 child("Notification Area" "ToolbarWindow32" "+Shell_TrayWnd" 0x1)
mou+ 0 6
mou- 0 0
POINT _m; xm _m
mou _m.x _m.y
mou
int v9=xm()
POINT p; xm(p child("Notification Area" "ToolbarWindow32" "+Shell_TrayWnd" 0x1) 1)
MouseWheel(2)
