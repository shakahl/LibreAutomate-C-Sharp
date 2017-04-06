 shows text of items in a folder window
 does not work with folder windows on Windows 7

int hwnd=child("" "SysListView32" win("" "CabinetWClass") 0x1)
str s; int r c
for r 0 SendMessage(hwnd LVM_GETITEMCOUNT 0 0)
	out "--- row %i ---" r
	for c 0 5
		if(!GetListViewItemText(hwnd r s c)) s=""
		out s
