 mac "Toolbar55" _hwndqm
 0.5

int w=win("TOOLBAR55" "QM_toolbar")
 SetWinStyle w WS_EX_APPWINDOW 5|8
 SetWinStyle w WS_EX_NOACTIVATE 6|8
 SetWinStyle w WS_EX_TOOLWINDOW 6|8
 SetWinStyle w WS_CAPTION|WS_SYSMENU|WS_THICKFRAME|WS_MINIMIZEBOX 1|8
 SetWinStyle w WS_MINIMIZEBOX 1|8
 SetWinStyle w WS_POPUP 2|8
 ont- w
 min w
 2
 res w

 _i=1
 if(DwmSetWindowAttribute(w 13 &_i 4)) end "error"
 mes 1
 _i=0
 if(DwmSetWindowAttribute(w 13 &_i 4)) end "error"
