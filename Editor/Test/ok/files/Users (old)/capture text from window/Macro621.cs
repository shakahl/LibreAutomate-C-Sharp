out

 captures text of mouse point

typelib TCaptureXLib {92657C70-D31B-4930-9014-379E3F6FB91A} 1.1
TCaptureXLib.TextCaptureX t._create
int hwnd=child(mouse); if(!hwnd) hwnd=win(mouse)
POINT p; xm(p)
str b=t.GetTextFromPoint(hwnd p.x p.y)
out b