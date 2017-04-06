men 33 "Notepad"
men "yy\pp" win("" "Shell_TrayWnd")
but child("")
but id(15 "Untitled - Notepad")
but+ child("" "" "Untitled - Notepad")
but- child("Notification Area" "ToolbarWindow32" "+Shell_TrayWnd" 0x1)
but* child("")
but% child("")
if(but(child("")))
	if(!but(id(15 "Untitled - Notepad")))
CB_SelectItem child("") 9
CB_SelectString id(15 "Untitled - Notepad") "jkjkj[91]9]j"
LB_SelectItem child("") 0
LB_SelectString child("Notification Area" "ToolbarWindow32" "+Shell_TrayWnd" 0x1) "oo"
SelectTab(child("Running Applications" "ToolbarWindow32" "+Shell_TrayWnd" 0x11) 5)
