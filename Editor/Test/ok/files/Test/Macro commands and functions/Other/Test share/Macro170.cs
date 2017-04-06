 Get toolbar button text
int hwnd=child("Notification Area" "ToolbarWindow32" "+Shell_TrayWnd" 0x1)
int buttonID=0
SendMessage(hwnd WINAPI.TB_GETBUTTONTEXTA buttonID share(hwnd))
str s.get(+share 0)
out s
