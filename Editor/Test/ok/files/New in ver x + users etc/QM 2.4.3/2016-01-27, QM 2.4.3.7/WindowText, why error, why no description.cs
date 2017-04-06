int wMain=win("" "CabinetWClass")
int w=child("" "DirectUIHWND" wMain 0x0 "accName=Items View") ;;list 'Items View'
WindowText wt.Init(w)
wt.Mouse(0 wt.Wait(0 "Vid*" 0x2))
