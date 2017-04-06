 /
function# $tooltip hide

 Hides or shows tray icon whose tooltip matches tooltip (can contain *?).
 Returns 1 if successful.

 tooltip - tooltip. Can contain wildcard characters (* and ?).
 hide - 1 to hide, 0 to show.

 EXAMPLE
 HideTrayIcon "Quick*" 1
 5
 HideTrayIcon "Quick*" 0


int hwnd=child("" "ToolbarWindow32" "+Shell_TrayWnd" 0x1)
if(!hwnd) ret
int i=FindTrayIcon(hwnd tooltip); if(i<0) ret
if(!SendMessage(hwnd TB_HIDEBUTTON i hide)) ret
Tray t.AddIcon; t.Delete ;;autosize
ret 1
