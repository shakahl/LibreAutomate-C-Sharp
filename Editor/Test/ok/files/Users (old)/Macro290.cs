int desktop=win("Program Manager" "Progman")
if(desktop!=win) ret ;;not active
Acc a=acc("Desktop" "LIST" desktop "SysListView32" "" 0x1001)
a.Focus(1) ;;finds focused icon. To find selected icons, use Selection instead.
 out a.Name
int x y w h
a.Location(x y w h)
x+w/2; y+h/2
mou x y
