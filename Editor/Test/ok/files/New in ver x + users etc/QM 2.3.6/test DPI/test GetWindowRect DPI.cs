out
 int w=win("Dialog" "#32770")
 int w=win("Untitled - Notepad" "Notepad")
int w=win("Microsoft Spy++ - [Threads 1]" "Afx:*" "" 0x4)

RECT r
GetWindowRect w &r; zRECT r
GetClientRect w &r; zRECT r

#compile dwmapi
if(!DwmGetWindowAttribute(w DWMWA_EXTENDED_FRAME_BOUNDS &r 16)) zRECT r
 gets bigger, ie with rounded frame that is not part of GetWindowRect

Acc a.FromWindow(w)
a.Location(r.left r.top r.right r.bottom)
r.right+r.left; r.bottom+r.top
zRECT r
