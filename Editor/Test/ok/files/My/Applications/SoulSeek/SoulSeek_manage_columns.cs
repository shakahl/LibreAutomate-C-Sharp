function hwnd idObject idChild
int hss=GetAncestor(hwnd GA_ROOT)
if(hid(hss)) ret
Acc a.ObjectFromEvent(hwnd idObject idChild)
int h=child("" "SysHeader32" hwnd)
Acc a2=acc("D/L*" "COLUMNHEADER" h "SysHeader32" "" 0x1001 0 0 "" 5)
a2.DoDefaultAction ;;sort by download speed
lef+ 150 12 h; 0.01; lef- 534 12 h ;;make first column wider
lef 4 4 id(1000 hss)
mou
