int ho=CreateWindowEx(WS_EX_TOOLWINDOW "#32770" 0 WS_VISIBLE|WS_POPUP 0 0 -10000 -10000 0 0 _hinst 0)
int htb=mac("Toolbar7" ho)
opt waitmsg 1
wait 0 WD htb
