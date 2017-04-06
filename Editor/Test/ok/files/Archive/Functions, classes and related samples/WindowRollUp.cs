 /
function hwnd !up ;;up: 1 roll up, 0 restore

 Resizes window so tht only title bar is visible.

 hwnd - window handle.

 EXAMPLES
 WindowRollUp win("" "notepad") 1


if up
	if(GetProp(hwnd "qm_roll_cy")) ret
	int cy
	GetWinXY hwnd 0 0 0 cy
	SetProp hwnd "qm_roll_cy" cy
	TITLEBARINFO ti.cbSize=sizeof(TITLEBARINFO)
	GetTitleBarInfo hwnd &ti
	siz 0 ti.rcTitleBar.bottom-ti.rcTitleBar.top hwnd 1
else
	cy=GetProp(hwnd "qm_roll_cy")
	if(!cy) ret
	RemoveProp hwnd "qm_roll_cy"
	siz 0 cy hwnd 1
