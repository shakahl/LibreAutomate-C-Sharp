ARRAY(QMITEMIDLEVEL) a; int i
if(!GetQmItemsInFolder("" &a)) end "failed"
for i 0 a.len
	out _s.getmacro(a[i].id 1)
	