\Dialog_Editor

lpstr dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 8 8 96 12 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040103 "*" "" "" ""

str controls = "3"
str e3
if(!ShowDialog(dd &sub.dlgproc &controls)) ret

#sub dlgproc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	out "init"
	case WM_DESTROY
	out "destroy"
	case WM_LBUTTONUP
	out "click 1"
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
