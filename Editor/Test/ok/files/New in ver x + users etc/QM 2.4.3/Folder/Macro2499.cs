 int w=196700
 outw w
 SetWinStyle w WS_EX_APPWINDOW 1|4|8

 act win("Notepad" "Notepad")

int w=win("Notepad" "Notepad")
SetWindowLong w GWL_HWNDPARENT GetDesktopWindow
mes 1
SetWindowLong w GWL_HWNDPARENT 0
