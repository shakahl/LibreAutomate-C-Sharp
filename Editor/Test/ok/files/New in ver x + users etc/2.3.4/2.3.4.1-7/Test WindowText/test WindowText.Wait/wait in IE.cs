SetProp(_hwndqm "qmtc_debug_output" 0)
out
int w=win("Google - Windows Internet Explorer" "IEFrame")
int c=child("Zoom Level" "ToolbarWindow32" w)
act w; 1
 c=w ;;note: with flag 0x100 does not work because then does not capture child windows of other threads
WindowText x.Init(c)
 x.Mouse(1 x.Wait(100 "75"))
x.Mouse(1 x.Wait(100 "75" 0x100))
