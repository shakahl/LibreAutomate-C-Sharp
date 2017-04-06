out
int w=id(15 win("Notepad" "Notepad")) ;;get handle of Notepad edit control
WindowText x.Init(w)
 WindowText x.Init(w 0 WT_REDRAW)
 x.Mouse(1 x.Find("findme"))
 x.Mouse(1 x.Wait(0 "findme"))
x.Mouse(1 x.Wait(0 "findme" 0x100))

 int w=win("Administrator: C:\Windows\system32\cmd.exe" "ConsoleWindowClass")
 WindowText x.Init(w)
  x.Mouse(0 x.Wait(0 "Users"))
 x.Mouse(0 x.Wait(0 "Users" 8))
