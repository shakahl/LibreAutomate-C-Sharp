str s
if(!inp(s "partial text")) ret

int w=win("Add Command" "HwndWrapper[DefaultDomain;*")
Acc aList.Find(w "LIST" "" "" 0x1000 0 2)
ARRAY(Acc) a; int i
aList.GetChildObjects(a 0 "LISTITEM" s "" 0x10)
if(!a.len) err OnScreenDisplay "not found"; ret

s=""
for i 0 a.len
	s.addline(a[i].Name)
i=ShowMenu(s 0 0 2)-1; if(i<0) ret
 
act w
a[i].Select(3)
