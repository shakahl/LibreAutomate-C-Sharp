 /
function# iid QMITEM&q level __GQIF_DATA&d 

 skip some folders
if q.itype=5
	sel(q.name) case ["private","System"] ret 1
	 if(!SendMessage(d.htv TVM_GETNEXTITEM TVGN_CHILD q.htvi)) ret 1 ;;skip empty folders

ARRAY(int)& a=d.a
a[]=iid
if d.aLevel
	&a=d.aLevel
	a[]=level
