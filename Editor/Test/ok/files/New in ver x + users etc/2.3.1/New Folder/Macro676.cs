Acc a=acc("" "TOOLBAR" win("TOOLBAR15" "QM_toolbar") "ToolbarWindow32" "" 0x1000)
for a.elem 1 1000000000
	str s=a.Name; err break
	out s
	
