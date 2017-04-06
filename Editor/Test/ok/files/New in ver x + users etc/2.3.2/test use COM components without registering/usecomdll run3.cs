#compile "____UseComUnregistered"
__UseComUnregistered ucd2.Activate("TCaptureX.X.manifest")

typelib Project1 "$qm$\_com\VbDll\VbDll.dll"
Project1.Class1 c._create

typelib TCaptureXLib "_com\SS\TCaptureX.dll"
TCaptureXLib.TextCaptureX t._create


c.DoubleArg(7)


int hwnd x y cx cy
if(t.CaptureInteractive(hwnd x y cx cy)) ret
str s=t.GetTextFromRect(hwnd x y cx cy)
out s
