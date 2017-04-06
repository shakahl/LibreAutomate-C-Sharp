out
JabInit

 find a Java window
 int w=win("Stylepad" "SunAwtFrame")
 int w=win("SwingSet2" "SunAwtFrame")
int w=win(" - OpenOffice.org" "SALFRAME") ;;no mouse events
outw w
if(!w) end ERR_WINDOW

 get object from window
if(!JAB.IsJavaWindow(w)) end "IsJavaWindow failed. The window does not implement the Java Accessibility API, or Java Access Bridge is disabled (see <open>enable Java Access Bridge</open>), or something does not work."
int vmID; JAB.AccessibleContext ac
if(!JAB.GetAccessibleContextFromHWND(w &vmID &ac)) end "GetAccessibleContextFromHWND failed"
out "%i %i" vmID ac
outw JAB.GetHWNDFromAccessibleContext(vmID ac)
JAB.ReleaseJavaObject(vmID ac)
ret

 receive some events
 while this macro runs (10 s), move mouse over the Java window, type some text
JAB.SetMouseEntered &JabEvent_MouseEntered
JAB.SetPropertyTextChange &JabEvent_PropertyTextChange
opt waitmsg 1
10
