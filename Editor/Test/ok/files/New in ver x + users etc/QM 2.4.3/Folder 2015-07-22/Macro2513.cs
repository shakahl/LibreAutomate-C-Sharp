int w=win("Dialog" "#32770")
int c=id(3 w) ;;combo box
RECT r; GetClientRect c &r; MapWindowPoints c 0 +&r 2
outRECT r
OnScreenRect 1 r
1
GetWindowRect c &r
outRECT r
OnScreenRect 1 r
1
