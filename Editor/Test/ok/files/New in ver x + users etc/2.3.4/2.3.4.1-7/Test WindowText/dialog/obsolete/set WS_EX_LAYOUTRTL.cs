 int w=win("" "CabinetWClass")
 int w=win("Font" "#32770")
 int w=win("Microsoft Word" "OpusApp")
 int w=win("Document - WordPad" "WordPadClass")
 int w=win("Task Scheduler" "MMCMainFrame")
 int w=win("Character Map" "#32770")
 int w=win("Calculator" "CalcFrame")
int w=win("SyncBack" "TfrmMain")

ARRAY(int) a; int i
child "" "" w 0 0 0 a
a[a.insert(0)]=w
for i 0 a.len
	SetWinStyle a[i] WS_EX_LAYOUTRTL 1|4|8|16
