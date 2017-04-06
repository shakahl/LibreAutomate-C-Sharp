out
 int w=win("Options" "#32770")
 WindowText x.Init(w)
 Acc a=x.GetAcc(x.Find("Run t"))
 out a.Name
 a.Role(_s); out _s

int w=id(15 win("Notepad" "Notepad")) ;;get handle of Notepad edit control
WindowText x.Init(w)
x.Mouse(1 x.Find("findme"))
