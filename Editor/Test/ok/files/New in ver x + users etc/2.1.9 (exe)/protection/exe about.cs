 tests protection against attempts to hide the about window

 out 1
 0.5
int h=win("" "#32770")
 zw h
 out "%x %x" GetWinStyle(h) GetWinStyle(h 1)
 clo h
 hid h
 mov 0 0 h
 siz 0 0 h
 Zorder h HWND_BOTTOM
 min h
 SetWindowPos h 0 0 0 0 0 SWP_NOSENDCHANGING
 SetWindowPos h HWND_BOTTOM 0 0 0 0 SWP_NOSIZE|SWP_NOMOVE|SWP_NOSENDCHANGING
 SetParent h win("" "Shell_TrayWnd")
 SetWinStyle h WS_CHILD
 SetWinStyle h WS_MINIMIZE
 end
 clo+ "+QM_ExeManager"
 dll msvcrt exit ec
 exit 0

 0.5
mes "mac 2"
 inp _s
 list "a[]b"
