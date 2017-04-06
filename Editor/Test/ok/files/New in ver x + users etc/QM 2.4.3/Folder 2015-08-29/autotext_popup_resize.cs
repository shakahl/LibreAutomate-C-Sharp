 Resizes QM autotext popup lists. Changes font.

int hwnd=TriggerWindow
int hlv=child("" "SysListView32" hwnd)

__Font f.Create("" 14) ;;change this
SendMessage hlv WM_SETFONT f 1
int widthPercent=150 ;;change this

if(GetWinStyle(hlv)&WS_VSCROLL=0) ret

RECT r.left=LVIR_BOUNDS; SendMessage(hlv LVM_GETITEMRECT 0 &r)
int ni=SendMessage(hlv LVM_GETITEMCOUNT 0 0)
if(ni>50) ni=50 ;;limit height to 50 items. Change the value if want.
r.bottom*ni
int y cx addWidth
GetWinXY hwnd 0 y cx; y=ScreenHeight-(y+r.bottom)-10; if(y<0) r.bottom+y
addWidth=MulDiv(cx widthPercent 100)-cx
cx+addWidth
siz cx r.bottom+4 hwnd
siz cx-4 r.bottom hlv
if addWidth
	SendMessage(hlv LVM_SETCOLUMNWIDTH 0 SendMessage(hlv LVM_GETCOLUMNWIDTH 0 0)+addWidth-(addWidth/10))
	SendMessage(hlv LVM_SETCOLUMNWIDTH 1 SendMessage(hlv LVM_GETCOLUMNWIDTH 1 0)+(addWidth/10))

wait 0 -WC hwnd






#ret

css=