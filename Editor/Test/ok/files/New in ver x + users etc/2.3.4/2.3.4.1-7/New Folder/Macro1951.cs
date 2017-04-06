 find selected item and save its name
int w1=win("Firefox" "Mozilla*WindowClass" "" 0x4)
Acc a1.Find(w1 "LISTITEM" "" "state=2 2" 0x3000 3)
str name=a1.Name
rset name "selected item" "\test"
