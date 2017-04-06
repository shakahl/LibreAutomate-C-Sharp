Acc a=acc("" "LIST" "Find" "ListBox" "" 0x1000)
for a.elem 1 1000000000
	str s=a.Name; err break
	out s

Acc a=acc("" "LIST" "My Macros" "SysListView32" "" 0x1000)
for a.elem 1 1000000000
	str s=a.Name; err break
	out s

int hwnd=child(1003 "" "ComboBox" "Options" 0x5)
int i n=control.CB_GetCount(hwnd)
for i 0 n
	str s
	control.CB_GetItemText(hwnd i s)
	out s
