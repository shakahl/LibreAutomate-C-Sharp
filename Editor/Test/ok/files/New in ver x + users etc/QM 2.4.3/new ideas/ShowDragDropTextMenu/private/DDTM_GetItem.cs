 /
function# hMenu miIndex ICsv&c

int r=GetMenuItemID(hMenu miIndex)-1
if(r<0 or r>=c.RowCount) ret -1 ;;>submenu or a spec item
ret r
