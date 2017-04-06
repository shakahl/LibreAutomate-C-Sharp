int w=win("* - Sandcastle Help File Builder" "*.Window.*" "" 0x401)
 int c=child("" "WindowsForms10.Window.*" w 0x0 "wfName=editor")
int c=child("" "WindowsForms10.SCROLLBAR.*" w) ;;scroll bar
ret GetWindow(c GW_HWNDPREV)
