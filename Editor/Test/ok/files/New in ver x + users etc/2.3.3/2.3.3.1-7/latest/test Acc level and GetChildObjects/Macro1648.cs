out
int w=wait(2 WV win("LHMT _ Orų prognozė - Mozilla Firefox" "Mozilla*WindowClass" "" 0x804))
Acc a.FindFF(w "UL" "" "" 0x1000 2)
ARRAY(Acc) c; int i
 a.GetChildObjects(c -1)
 a.GetChildObjects(c 0)
 a.GetChildObjects(c 1)
 a.GetChildObjects(c 2)
 a.GetChildObjects(c MakeInt(1 2))
 a.GetChildObjects(c -1 "LINK")
 a.GetChildObjects(c -1 "LINK" " ")
 a.GetChildObjects(c -1 "LINK" "" "value=or")
 a.GetChildObjects(c -1 "" "" "value=or")
 a.GetChildObjects(c 1 "" "" "value=or")
a.GetChildObjects(c 1 "" "" "value=*or*" 4)

for i 0 c.len
	c[i].Role(_s); out _s
	out c[i].Name
out c.len
