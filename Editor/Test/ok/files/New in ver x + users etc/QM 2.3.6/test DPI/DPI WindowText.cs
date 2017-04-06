 int w=id(15 win("Notepad" "Notepad")) ;;get handle of Notepad edit control
int w=win("Compare" "#32770")
 int w=win("Notepad" "#32770")
WindowText x.Init(w)
WTI* t=x.Find("Cancel" 0x1000)
RECT rv=t.rv; DpiMapWindowPoints t.hwnd 0 +&rv 2 ;;from t.hwnd client to screen
OnScreenRect 1 &rv; 1; OnScreenRect 2 &rv
