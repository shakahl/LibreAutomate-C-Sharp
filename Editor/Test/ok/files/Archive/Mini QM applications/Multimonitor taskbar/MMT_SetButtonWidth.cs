 /MMT_Main

MMTVAR- v
TBBUTTONINFOW bi.cbSize=32
if(!v.a.len) ret

int i w
GetWinXY v.htb 0 0 w
w-4 ;;leave some place to right click
w/v.a.len
if(w>160) w=160; else if(w<20) w=20
if(w=v.bwidth) ret
v.bwidth=w
for i 0 v.a.len
	bi.dwMask=TBIF_SIZE
	bi.cx=w
	SendMessage v.htb TB_SETBUTTONINFOW i &bi
