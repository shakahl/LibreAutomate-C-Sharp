out
int w=win("" "CabinetWClass")
Acc a.Find(w "LIST" "Items View" "class=DirectUIHWND" 0x1005)
ARRAY(Acc) c; int i
a.GetChildObjects(c)
for i 0 c.len
	str name=c[i].Name; err name=""
	str value=c[i].Value; err value=""
	out "name='%s', value='%s'" name value
