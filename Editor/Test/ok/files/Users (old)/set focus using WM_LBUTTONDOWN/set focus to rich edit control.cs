int hwnd=id(3 "Form")
 act hwnd
 key TST
 SendMessage hwnd WM_SETFOCUS 0 0

SendMessage hwnd WM_LBUTTONDOWN 1 0
0.02
SendMessage hwnd WM_LBUTTONUP 0 0
