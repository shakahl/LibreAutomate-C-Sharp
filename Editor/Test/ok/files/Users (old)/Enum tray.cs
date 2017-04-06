Acc a=acc("Notification Area" "TOOLBAR" "+Shell_TrayWnd" "ToolbarWindow32" "" 0x1001)
a.Navigate("f")
rep
	out a.Name
	a.Navigate("n"); err break
