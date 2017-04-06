int w=win("Dialog Transparent Children" "#32770")
RECT r; GetClientRect w &r
 int w2=CreateWindowEx(0 "SysTabControl32" 0 0x54000040 0 0 r.right r.bottom w 0 _hinst 0)
int w2=CreateWindowEx(0 "#32770" 0 0x54000040 0 0 r.right r.bottom w 0 _hinst 0)
BringWindowToTop w2
opt waitmsg 1
wait 0 -WC w
