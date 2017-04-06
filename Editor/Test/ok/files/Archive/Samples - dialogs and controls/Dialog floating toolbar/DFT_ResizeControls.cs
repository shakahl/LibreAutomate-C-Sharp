 /DFT_Main
function hDlg

int htb=id(3 hDlg)
RECT r; GetClientRect hDlg &r
if htb
	RECT rt; GetWindowRect htb &rt
	r.top=rt.bottom-rt.top

MoveWindow id(5 hDlg) 0 r.top r.right r.bottom-r.top 1
