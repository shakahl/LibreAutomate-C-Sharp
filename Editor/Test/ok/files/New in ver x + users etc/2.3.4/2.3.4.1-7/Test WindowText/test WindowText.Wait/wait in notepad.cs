SetProp(_hwndqm "qmtc_debug_output" 1)
out
 wait for text "clickme" in Notepad, and click it
int c=id(15 win("Notepad" "Notepad"))
 int c=win("Notepad" "#32770")
spe
act c;; 1
WindowText x.Init(c)
 x.Mouse(1 x.Wait(100 "clickme"))
x.Mouse(1 x.Wait(100 "clickme" 0x100))
