 /
function hwnd

outw hwnd
RedrawWindow(hwnd, 0, 0, RDW_ERASE|RDW_FRAME|RDW_INVALIDATE|RDW_ALLCHILDREN);
act hwnd
key F5
1
ret
 SetWindowPos hwnd 0 0 0 0 0 SWP_ASYNCWINDOWPOS|SWP_FRAMECHANGED|SWP_NOSIZE|SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE

lef 58 11 child("" "SysTreeView32" hwnd 0x1)
 lef 58 11 child("" "Edit" hwnd 0x1)
0.1
mou

 hid hwnd
 hid- hwnd
 min hwnd
 res hwnd

0.5
 RedrawWindow(hwnd, 0, 0, RDW_ERASE|RDW_FRAME|RDW_INVALIDATE|RDW_UPDATENOW|RDW_ALLCHILDREN);
