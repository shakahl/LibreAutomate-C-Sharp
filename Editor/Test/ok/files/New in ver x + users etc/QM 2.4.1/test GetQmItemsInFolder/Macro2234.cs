out
ARRAY(QMITEMIDLEVEL) a; int i
if(!GetQmItemsInFolder("\System\Functions" &a)) end "failed"
for i 0 a.len
	out "%.*m%s" a[i].level 9 _s.getmacro(a[i].id 1)


 out a.len
 PF
 PN;PO ;;470 or 1360
 GetQmItemsInFolder2("\System" &a 0)
