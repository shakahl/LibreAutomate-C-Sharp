SetProp(_hwndqm "qmtc_debug_output" 0)
out
int w=win("" "_macr_dreamweaver_frame_window_")
int c=id(59893 w)
act w; 1
 c=w
WindowText x.Init(c)
 x.Mouse(1 x.Wait(100 "clickme")) ;;caret flickers
x.Mouse(1 x.Wait(100 "clickme" 0x100))
