out
int w=win("Firefox" "Mozilla*WindowClass" "" 0x804)
Acc a.Find(w "DOCUMENT" "" "" 0x3010 2)
ARRAY(Acc) aa; int i
a.GetChildObjects(aa -1 "LINK")
for i 0 aa.len
	out "%-35s %s" aa[i].Name aa[i].Value
