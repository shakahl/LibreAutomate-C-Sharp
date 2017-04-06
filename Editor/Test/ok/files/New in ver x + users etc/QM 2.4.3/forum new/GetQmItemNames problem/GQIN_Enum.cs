 /
function# iid QMITEM&q level __GQIN_DATA&d 

int i r f=q.itype=5

 skip some folders
if(f)
	sel(q.name) case ["private","System"] r=1; goto g1
	if(!SendMessage(d.htv TVM_GETNEXTITEM TVGN_CHILD q.htvi)) r=1; goto g1

d.st.all(level 2 '.') ;;indentation

if(f) d.sp.formata("%s[%s][]" d.st q.name)
else d.sp.formata("%s%s[]" d.st q.name)

 g1
d.level=level
ret r