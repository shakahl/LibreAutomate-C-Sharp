\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str qmg3x=
 </10>one,two
 </11>three,four
if(!ShowDialog("" &dlg_Grid_ownerdraw &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031441 0x0 0 87 96 48 "0x7,0,0,0,0x0[]A,,,[]B,,,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030203 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	case WM_DRAWITEM
	DRAWITEMSTRUCT* d=+lParam
	_s.getstruct(*d 1); out _s
	 out 1
	ret DT_Ret(hDlg 1)
	 info: cannot make lv to draw some items
	 info: itemdata 0
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	 OutWinMsg message wParam lParam
