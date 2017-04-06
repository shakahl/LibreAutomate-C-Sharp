int hwnd=child("" "ToolbarWindow32" win("" "Shell_TrayWnd"))
out _s.getwintext(hwnd)

Acc a=acc("" "CLIENT" win("" "Shell_TrayWnd") "Shell_TrayWnd" "" 0x1000 0 0 "child3 child4 child3")
hwnd=child(a)
out _s.getwintext(hwnd)
