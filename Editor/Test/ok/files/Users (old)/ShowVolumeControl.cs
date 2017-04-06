function [nSeconds]

 EXAMPLES
 ShowVolumeControl 5 ;;sync
 mac "ShowVolumeControl" "" 5 ;;async


if(!nSeconds) nSeconds=5
Acc a=acc("Volume" "PUSHBUTTON" "+Shell_TrayWnd" "ToolbarWindow32" "" 0x1001)
POINT p; xm p
a.Mouse(1)
int h=wait(10 WV win("Volume Control" "Tray Volume"))
mou p.x p.y 0
wait nSeconds
clo h
