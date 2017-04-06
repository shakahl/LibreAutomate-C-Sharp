out
SetProp(_hwndqm "qmtc_debug_output" 0)

 int w=win("Quick Macros" "QM_Editor")
 int c=id(2050 w)
 WindowText x.Capture(c)
 x.Mouse(1 x.Find("Help" 2))

 int w=win("Task Scheduler" "MMCMainFrame")
 act w
 WindowText x.Capture(w)
 x.Mouse(0 x.Find("Basic"))
  x.Mouse(1 x.Find("Status of *" 2) 150 5)

int w=win("TextCapture - Microsoft Visual Studio (Administrator)" "wndclass_desked_gsk")
 act w; 0.1
WindowText x.Capture(w)
 x.Mouse(0 x.Find("CreateTextCapture"))

 Q &q
  WTI* t=x.Find("CreateTextCapture" 2)
 WTI* t=x.Find("CreateTextCaptureObjec." 4)
 Q &qq
 outq
 out t
