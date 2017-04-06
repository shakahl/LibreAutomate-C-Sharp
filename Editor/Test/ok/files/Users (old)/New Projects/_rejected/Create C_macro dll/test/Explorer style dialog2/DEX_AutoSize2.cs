 /DEX_Main3
function hDlg

int htb(id(3 hDlg)) htv(id(4 hDlg)) hlv(id(5 hDlg)) hsb(id(6 hDlg))
SendMessage(htb TB_AUTOSIZE 0 0)
SendMessage(hsb WM_SIZE 0 0)

int y1 h1 w1 w2
RECT rc rtb rsb
GetClientRect(hDlg &rc)
GetWindowRect(htb &rtb)
GetWindowRect(hsb &rsb)
y1=rtb.bottom-rtb.top
h1=rc.bottom-y1-(rsb.bottom-rsb.top); if(h1<0) h1=0
w1=120
w2=rc.right-w1-4; if(w2<0) w2=0
MoveWindow htv 0 y1 w1 h1 1
MoveWindow hlv w1+4 y1 w2 h1 1
