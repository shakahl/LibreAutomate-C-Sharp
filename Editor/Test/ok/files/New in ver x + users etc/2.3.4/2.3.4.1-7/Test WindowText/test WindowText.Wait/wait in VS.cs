SetProp(_hwndqm "qmtc_debug_output" 0)
out
int w=win("Visual Studio" "wndclass_desked_gsk")
spe
act w;; 1
int c=child("" "VsTextEditPane" w 0 0.4 0.5)
WindowText x.Init(c)
 x.Mouse(1 x.Wait(0 "return")) ;;caret flickers
  x.Mouse(1 x.Wait(0 "return" 0x100))
 x.Mouse(1 x.Wait(0 "clickme")) ;;caret flickers
x.Mouse(1 x.Wait(0 "clickme" 0x100))
