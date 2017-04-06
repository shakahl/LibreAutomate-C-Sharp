SetProp(_hwndqm "qmtc_debug_output" 0)
out
int w=win("Task Scheduler" "MMCMainFrame")
int c=child("Last" "WindowsForms10.COMBOBOX.app.0.21d1674" w)
 c=w ;;note: with flag 0x100 does not work because then does not capture child windows of other threads ("Last 24" belongs to other thread than w). It's OK.
WindowText x.Init(c)
 x.Mouse(1 x.Wait(100 "Last 24"))
x.Mouse(1 x.Wait(100 "Last 24" 0x100))
