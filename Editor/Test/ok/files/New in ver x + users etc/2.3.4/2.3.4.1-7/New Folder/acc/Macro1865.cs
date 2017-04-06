out
 get name/URL of all Firefox tabs
int w=win("Mozilla Firefox" "Mozilla*WindowClass" "" 0x804)
Acc a.Find(w "" "" "" 0x3010 0 0 "parent3") ;;find first object in current web page and get its parent GROUPING
ARRAY(Acc) b; int i
a.GetChildObjects(b 2 "DOCUMENT" "" "" 16) ;;get all child DOCUMENT at level 2, including hidden
for i 0 b.len
	out "--------"
	out b[i].Name ;;page name
	out b[i].Value ;;page URL
	if(b[i].State&STATE_SYSTEM_INVISIBLE=0) out "<visible>"
