 act "Firefox"
int w1=win("" "Shell_TrayWnd")
Acc a.Find(w1 "PUSHBUTTON" "* Firefox" "class=MSTaskListWClass" 0x1005)
a.Mouse(1)
int w=wait(2 WV win("TB FIREFOX" "QM_toolbar"))
err ret
 act "Calculator"
shutdown -2 0 "v M ''acc hook bug - firefox''"
