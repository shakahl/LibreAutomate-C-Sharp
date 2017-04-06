 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ComboBox 0x54230242 0x0 8 8 96 213 ""
 4 ComboBox 0x54230243 0x0 8 32 96 213 ""
 5 ComboBox 0x54230641 0x0 8 56 96 48 ""
 6 Edit 0x54230080 0x200 112 8 96 12 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3 4 5 6"
str cb3 cb4 cb5 e6
cb3="&a[]b[]c"
cb4="&x[]y[]z"
if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret


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
