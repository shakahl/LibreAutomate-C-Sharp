SetProp(_hwndqm "qmtc_debug_output" 1)
out
int w=win("Microsoft Excel - Book1" "XLMAIN")
int c=child("Book1" "EXCEL7" w)
act w; 1
 c=w
WindowText x.Init(c)
x.Mouse(1 x.Wait(100 "75"))
 x.Mouse(1 x.Wait(100 "75" 0x100)) ;;does not work
