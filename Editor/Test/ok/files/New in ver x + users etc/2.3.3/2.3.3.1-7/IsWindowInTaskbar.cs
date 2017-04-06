 /
function! hwnd

 Returns 1 if the window has taskbar button.


int exstyle=GetWinStyle(hwnd 1)
 is appwindow?
if(exstyle&WS_EX_APPWINDOW) ret 1
 is toolwindow?
if(exstyle&(WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE)) ret
 is owned?
int ho=GetWindow(hwnd GW_OWNER)
if(ho and ho!=GetDesktopWindow) ret

ret 1
