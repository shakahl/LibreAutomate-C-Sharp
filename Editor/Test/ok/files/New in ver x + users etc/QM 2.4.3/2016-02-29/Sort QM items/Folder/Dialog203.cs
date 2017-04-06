
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 4 Edit 0x54030080 0x200 8 8 96 12 ""
 5 Button 0x54012003 0x0 8 28 48 10 "Check"
 3 Button 0x54032000 0x0 8 56 48 14 "Reload GUI"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040307 "*" "" "" ""

str controls = "4 5"
str e4 c5Che
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
	case 3 ;;Reload GUI
	clo hDlg
	0
	mac "Dialog199"
	 Dialog199 ;;works too, but after many reloads possible stack overflow
	
	case IDOK
	case IDCANCEL
ret 1
