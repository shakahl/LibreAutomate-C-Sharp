#compile "____UseComUnregistered"
__UseComUnregistered ucd.Activate("TCaptureX.X.manifest")

typelib TCaptureXLib "_com\SS\TCaptureX.dll"
TCaptureXLib.TextCaptureX t._create

int hwnd x y cx cy
if(t.CaptureInteractive(hwnd x y cx cy)) ret
str s=t.GetTextFromRect(hwnd x y cx cy)
out s
