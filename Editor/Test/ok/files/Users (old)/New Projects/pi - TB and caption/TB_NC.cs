int h=val(_command)
int htb=win("TBT_*" "QM_Toolbar" "" 1|64 h 0); if(!htb) ret
RECT r; int w=TB_GWTW(h htb r)
mov r.left+w r.top htb
