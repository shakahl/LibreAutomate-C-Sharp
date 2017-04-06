SetProp(_hwndqm "qmtc_debug_output" 0)
out
int w=win("" "QM_Editor")
int c=id(2214 w) ;;Running Items
 int c=id(2050 w) ;;menubar
WindowText x.Init(c)
 x.SetOptions(WT_REDRAW)
 x.Mouse(1 x.Wait(100 "wait in QM"))
x.Mouse(0 x.Wait(100 "wait in QM" 0x100))
 x.Mouse(0 x.Wait(100 "Help" 0x100))
