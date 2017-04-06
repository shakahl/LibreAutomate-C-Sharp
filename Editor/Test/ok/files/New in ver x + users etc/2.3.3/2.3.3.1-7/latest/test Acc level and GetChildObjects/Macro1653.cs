 get all links in web page in Firefox
int w=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
Acc a.Find(w "DOCUMENT" "" "" 0x3010 2)
ARRAY(Acc) c; int i
a.GetChildObjects(c -1 "LINK")
for i 0 c.len
	out c[i].Value
