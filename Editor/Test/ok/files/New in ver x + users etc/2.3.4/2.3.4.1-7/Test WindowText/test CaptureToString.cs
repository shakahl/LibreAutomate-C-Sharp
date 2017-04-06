out
SetProp(_hwndqm "qmtc_debug_output" 0)

#if 0

int w=child(mouse); if(!w) w=win(mouse)
 int w=win("Untitled - Notepad" "Notepad")
 w=id(15 w)
 int w=win("qmtc test.doc - Microsoft Word" "OpusApp")
 w=child("Microsoft Word Document" "_WwG" w)

WindowText x.Init(w)
str s=x.CaptureToString
out s

#else

WindowText x.InitInteractive
str s=x.CaptureToString
out s

#endif
