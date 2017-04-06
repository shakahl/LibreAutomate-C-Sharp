out
int w=id(15 win("Notepad" "Notepad")) ;;get handle of Notepad edit control
act w

WindowText x.Init(w)
x.SetCallback(&QMTC_Callback 8)

x.Find("me" 0x1000)
