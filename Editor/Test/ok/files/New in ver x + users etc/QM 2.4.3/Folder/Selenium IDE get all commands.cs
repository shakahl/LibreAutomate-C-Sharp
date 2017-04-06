 Shows commands from the combo list in Selenium IDE.

out
int w=win("Selenium IDE" "Mozilla*WindowClass" "" 0x4)
act w
Acc a.Find(w "COMBOBOX" "Command" "" 0x1001)
a.DoDefaultAction
1
ARRAY(Acc) b; int i
a.GetChildObjects(b -1 "LISTITEM" "" "" 16)
 out b.len
for i 0 b.len
	str s=b[i].Name
	if(s.end("AndWait")) continue
	out s

act _hwndqm
