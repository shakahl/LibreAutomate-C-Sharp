
int w=wait(3 WV win("" "Mozilla*WindowClass" "" 0x4))
Acc aList.Find(w "LIST" "" "" 0x3010 3)
ARRAY(Acc) a; int i iSel(-1)
aList.GetChildObjects(a -1 "LISTITEM")
for i 0 a.len
	if(a[i].State&STATE_SYSTEM_SELECTED) iSel=i; break
if(iSel<0) end "none selected"
out iSel
