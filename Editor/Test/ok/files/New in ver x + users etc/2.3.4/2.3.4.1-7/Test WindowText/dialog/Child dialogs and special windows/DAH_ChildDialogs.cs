 /dlg_apihook
function hDlg
 ret

int h

h=DAH_ChildDialog(hDlg 100 580 150 100 "D1")
 ret
DAH_ChildDialog(h 0 60 100 50 "D2")
DAH_ChildDialog(hDlg 0 60 105 55 "D3")

h=DAH_ChildDialog(hDlg 50 200 60 40 "D4")
RECT rw; GetWindowRect h &rw; OffsetRect &rw -rw.left -rw.top
InflateRect &rw -30 -10
SetWindowRgn h CreateRectRgnIndirect(&rw) 0

 DAH_ChildWS_EX_LAYOUTRTL hDlg
