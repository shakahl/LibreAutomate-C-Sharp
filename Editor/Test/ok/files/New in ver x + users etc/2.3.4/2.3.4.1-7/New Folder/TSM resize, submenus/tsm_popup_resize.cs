int hwnd=val(_command)
int hlv=child("" "SysListView32" hwnd)
if(GetWinStyle(hlv)&WS_VSCROLL=0) ret

RECT r.left=LVIR_BOUNDS; SendMessage(hlv LVM_GETITEMRECT 0 &r)
int ni=SendMessage(hlv LVM_GETITEMCOUNT 0 0)
if(ni>50) ni=50 ;;limit height to 50 items. Change the value if want.
r.bottom*ni
int y; GetWinXY hwnd 0 y; y=ScreenHeight-(y+r.bottom)-10; if(y<0) r.bottom+y
siz 0 r.bottom+4 hwnd 1
siz 0 r.bottom hlv 1











#ret



v visibility:hidden;


















#ret
