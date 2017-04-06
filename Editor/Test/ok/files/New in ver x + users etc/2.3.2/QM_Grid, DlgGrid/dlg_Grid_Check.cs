\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
qmg3x=
 a,Yes
 b,

if(!ShowDialog("dlg_Grid_Check" &dlg_Grid_Check &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031049 0x0 0 0 224 110 "0x0,0,0,0,0x0[]A,,,[]B,,2,[]B,,18,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030203 "*" "" ""

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
