out
PF
opt hidden 1
ARRAY(int) a
win "" "" "" 0 "" a
PN
int i
for i 0 a.len
	int w(a[i])
	 IsWindowVisible(w) ;;22
	 GetWinStyle(w) ;;43
	 GetWinStyle(w 1) ;;55
	 GetWindowLong(w GWL_EXSTYLE) ;;33
	 IsWindowEnabled(w) ;;21
	 GetLastActivePopup(w) ;;39
	 GetWindow(w GW_OWNER) ;;24
	 w=GetShellWindow ;;15
	GetWindowThreadProcessId(w &_i) ;;~600
	 int x; if(DwmGetWindowAttribute(w 14 &x 4)) end "error" ;;~700
PN
PO

