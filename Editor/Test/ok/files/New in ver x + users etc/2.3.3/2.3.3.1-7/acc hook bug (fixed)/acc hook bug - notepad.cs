 Reproduces the bug.
 Activates Notepad, waits shortly, restarts QM, launching this macro again.
 Repeats until the bug is reproduced (window trigger does not work).
 Then restart QM manually to make it work again.
 Other threads (spam filter, rss notifier) should not be running, to avoid interference.

 2 ;;the same
int+ g_accbug=0
act "Notepad"
 int w1=win("" "Shell_TrayWnd")
 Acc a.Find(w1 "PUSHBUTTON" "Untitled - Notepad" "class=MSTaskListWClass" 0x1005)
  Acc a.Find(w1 "PUSHBUTTON" "Untitled - Notepad" "class=ToolbarWindow32" 0x1005)
 a.Mouse(1); mou
 int w=wait(2 WV win("TOOLBAR38" "QM_toolbar"))
 Q &q
wait(2 V g_accbug)
err ret
 Q &qq; outq
 0.2 ;;try to change to make reproducing easier
0.4 ;;this works better with wait V (without toolbar)
 EndThread "acc_bug_intgetfile"
 1
 lock lock_igf
shutdown -2 0 "v M ''acc hook bug - notepad''"
 shutdown -2 0 "v"
 1
 lock- lock_igf
