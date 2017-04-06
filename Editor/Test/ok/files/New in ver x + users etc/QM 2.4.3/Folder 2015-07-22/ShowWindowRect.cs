 /
function hwnd

 Three times shows black rectangle around window or control.

 hwnd - window or control handle.

 EXAMPLE
 int w=win("Quick Macros" "QM_Editor")
 int c=id(2053 w) ;;tool bar
 ShowWindowRect c


RECT r; GetWindowRect hwnd &r
rep(3) OnScreenRect 1 r; 0.3; OnScreenRect 3; 0.3
OnScreenRect 2
