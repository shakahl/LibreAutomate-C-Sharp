 /
function $btnName [menu] [$dlgName]

key CE

Acc a=acc(btnName "PUSHBUTTON" win("QM TOOLBAR" "QM_toolbar") "ToolbarWindow32" "" 0x1001)
a.DoDefaultAction

if(menu)
	0.3
	rep(menu) key D
	key Y

test_tool_dialog dlgName
1
