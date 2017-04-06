out
int w=win("Internet Explorer" "IEFrame")
Acc a.Find(w "PANE" "" "" 0x3000 3)
ARRAY(Acc) b
a.GetChildObjects(b -1 "LINK")
int i
type ACC_OBJECT_PROP role str'roleStr str'name str'value
ARRAY(ACC_OBJECT_PROP) c

for i 0 b.len
	ACC_OBJECT_PROP& r=c[] ;;add new array element and get reference to it
	r.name=b[i].Name
	r.value=b[i].Value
	r.role=b[i].Role(r.roleStr)




	 out F"{sName} | {sValue}"