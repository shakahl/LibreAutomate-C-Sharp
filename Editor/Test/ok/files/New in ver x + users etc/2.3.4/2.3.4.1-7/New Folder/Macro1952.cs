 get saved item name and find the item
str name
rget name "selected item" "\test"
int w1=win("Firefox" "Mozilla*WindowClass" "" 0x4)
Acc a1.Find(w1 "LISTITEM" name "" 0x3000 3)
a1.Mouse(1)
