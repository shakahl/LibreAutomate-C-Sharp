SetProp(_hwndqm "qmtc_debug_output" 0)
out
int w=win("My QM" "CabinetWClass")
int c=child("" "DirectUIHWND" w 0 0 0 3)
act w; 1
 c=w
WindowText x.Init(c)
 x.Mouse(0 x.Wait(100 "shell" 8))
 x.Mouse(0 x.Wait(100 "shell" 0x108))

RECT r.right=100; r.bottom=100
WindowText x.Init(c r)
 x.Mouse(0 x.Wait(100 "shell"))
x.Mouse(0 x.Wait(100 "shell" 0x100))
