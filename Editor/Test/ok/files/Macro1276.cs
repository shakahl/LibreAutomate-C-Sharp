int w=win("MainWindow" "HwndWrapper[Wpf.exe;*")
RECT r
GetClientRect w &r; MapWindowPoints w 0 +&r 2
OnScreenRect 0 r
2
