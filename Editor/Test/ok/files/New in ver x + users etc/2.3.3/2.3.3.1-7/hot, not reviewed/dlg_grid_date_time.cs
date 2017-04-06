\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
qmg3x=
 09/28/2010,19:04:27
 01/20/2011,19:05:02
if(!ShowDialog("dlg_grid_date_time" &dlg_grid_date_time &controls)) ret
out qmg3x

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 90 "0x0,0,0,0,0x0[]date,,3,[]time,,11,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
