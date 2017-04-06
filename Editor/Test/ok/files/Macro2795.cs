spe
PF
run "iexplore.exe"
PN
int w=wait(0 WV "Internet Explorer")
PN;;PO
 outw w
 out PostMessage(w WM_SYSCOMMAND SC_CLOSE 0)
 out PostMessage(w WM_CLOSE 0 0)
 out SendNotifyMessage(w WM_CLOSE 0 0)
out SendNotifyMessage(w WM_SYSCOMMAND SC_CLOSE 0)
