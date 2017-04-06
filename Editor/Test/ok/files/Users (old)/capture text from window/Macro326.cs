out

 captures text of user selected rectangle

typelib TCaptureXLib {92657C70-D31B-4930-9014-379E3F6FB91A} 1.1
TCaptureXLib.TextCaptureX t._create
int hwnd x y cx cy
if(t.CaptureInteractive(hwnd x y cx cy)) ret
str s=t.GetTextFromRect(hwnd x y cx cy)
out s
