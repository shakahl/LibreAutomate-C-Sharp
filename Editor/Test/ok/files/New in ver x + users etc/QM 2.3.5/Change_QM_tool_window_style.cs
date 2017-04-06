int hwnd=val(_command)
if(GetWinStyle(hwnd 1)&WS_EX_TOOLWINDOW=0) ret
SetWinStyle hwnd WS_EX_TOOLWINDOW 6
SetWinStyle hwnd WS_MAXIMIZEBOX|WS_MINIMIZEBOX 1
