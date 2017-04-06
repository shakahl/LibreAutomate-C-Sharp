\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 6"
str qmg3x qmg4x qmg5x qmg6x
if(!ShowDialog("" &dialog_test_grid &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 QM_Grid 0x56035041 0x200 18 6 96 32 "0x0,0,0,0,0x0[]A,80%,,"
 4 QM_Grid 0x56031041 0x200 18 52 96 48 "0x0,0,0,0,0x0[]A,80%,8,"
 5 QM_Grid 0x56031041 0x200 118 6 96 48 "0x0,0,0,0,0x0[]A,200,,"
 6 QM_Grid 0x56031041 0x200 118 58 96 48 "0x0,0,0,0,0x0[]A,120%,,[]B,100%,,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030507 "*" "" "" ""

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
