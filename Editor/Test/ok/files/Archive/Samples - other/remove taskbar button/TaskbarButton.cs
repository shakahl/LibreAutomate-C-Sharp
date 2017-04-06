 /
function action hwnd ;;action: 1 add (only if removed), 2 remove

 Deletes or adds (if deleted using this function) taskbar button for window whose handle is hwnd.


int fl
sel action
	case 2
	if(!GetWindow(hwnd GW_OWNER))
		fl|1
		SetWindowLong(hwnd GWL_HWNDPARENT GetDesktopWindow)
	if(GetWinStyle(hwnd 1)&WS_EX_APPWINDOW)
		fl|2
		SetWinStyle hwnd WS_EX_APPWINDOW 6
	if(fl) SetProp(hwnd "qm_tb_flags" fl)
	
	case 1
	fl=RemoveProp(hwnd "qm_tb_flags")
	if(fl&1)
		SetWindowLong(hwnd GWL_HWNDPARENT 0)
	if(fl&2)
		SetWinStyle hwnd WS_EX_APPWINDOW 5
