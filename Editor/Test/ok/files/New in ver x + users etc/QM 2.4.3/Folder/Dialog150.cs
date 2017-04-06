 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Static 0x54000000 0x0 8 8 48 12 "Text"
 4 Edit 0x54030080 0x200 64 8 96 12 ""
 5 Button 0x54012003 0x0 8 24 48 10 "Check"
 6 ComboBox 0x54230243 0x0 8 40 96 213 ""
 7 QM_Grid 0x56031041 0x200 8 60 96 48 "0[]A[]B"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "4 5 6 7"
str e4 c5Che cb6 qmg7
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

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
