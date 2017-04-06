 int w=win("Notepad")
int w=win("Test TSM")
 int w=win("Registry")
 SendMessage w WM_CUT 0 0
 ret
Q &q
int r=SyncProcess(w)
 int r=SyncProcessSM(w)
Q &qq
outq
out r
