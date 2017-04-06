 int w=win("Calculator" "ApplicationFrameWindow")
 int w1=win("Calculator" "ApplicationFrameWindow")
 int c=child("Calculator" "Windows.UI.Core.CoreWindow" w1) ;; 'Calculator'

out
int w=2883998
outw w
outw GetParent(w)
outw GetWindow(w GW_OWNER)
outw GetWindowLong(w GWL_HWNDPARENT)
