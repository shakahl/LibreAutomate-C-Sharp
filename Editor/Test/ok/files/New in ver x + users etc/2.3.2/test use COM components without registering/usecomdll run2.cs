#compile "____UseComUnregistered"
__UseComUnregistered ucd.Activate("TCaptureX.X.manifest")

typelib Project1 "$qm$\_com\VbDll\VbDll.dll"
Project1.Class1 c._create

c.DoubleArg(7)
