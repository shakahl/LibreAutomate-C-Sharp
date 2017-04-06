out
int w=win("Internet Explorer" "IEFrame")
Acc a.Find(w "PANE" "" "" 0x3000 3)
ARRAY(Acc) b
a.GetChildObjects(b -1)
ICsv x=CreateCsv(1)
int i
for i 0 b.len
	str sRole sName sValue
	b[i].Role(sRole)
	sName=b[i].Name
	sValue=b[i].Value
	




	 out F"{sName} | {sValue}"