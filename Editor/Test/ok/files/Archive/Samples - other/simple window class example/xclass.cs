 register class
int+ __xclass
if(!__xclass) __xclass=RegWinClass("xclass" &WndProc_xclass)

 create window
int style=WS_POPUP|WS_BORDER
int exstyle=WS_EX_TOOLWINDOW
int hwnd=CreateWindowEx(exstyle "xclass" 0 style 10 10 50 50 0 0 _hinst 0)

OnScreenDisplay "do you see your window in the top-left corner?"

 wait, or show a dialog
opt waitmsg 1
wait 10

 destroy
DestroyWindow hwnd
