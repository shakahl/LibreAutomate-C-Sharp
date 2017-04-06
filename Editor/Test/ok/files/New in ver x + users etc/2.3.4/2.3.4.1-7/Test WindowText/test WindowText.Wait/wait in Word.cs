SetProp(_hwndqm "qmtc_debug_output" 0)
out
int w=win("Microsoft Word" "OpusApp")
int c=child("Microsoft Word Document" "_WwG" w)
act w; 1
 c=w
WindowText x.Init(c 0 WT_JOIN)
 x.Mouse(1 x.Wait(100 "clickme"))
x.Mouse(1 x.Wait(100 "clickme" 0x100))
