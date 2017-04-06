\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 8 8 110 54 ""
 4 Button 0x54012003 0x0 8 68 48 10 "Multiline"
 5 Edit 0x54030180 0x200 124 8 96 12 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3 4 5"
str e3 c4Mul e5
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
	case 4
	 SetWinStyle id(3 hDlg) ES_MULTILINE|ES_WANTRETURN}WS_VSCROLL iif(but(lParam) 1 2) ;;TODO: wa no bug ( } )
	SetWinStyle id(3 hDlg) ES_MULTILINE|ES_WANTRETURN|WS_VSCROLL iif(but(lParam) 1 2)|8|16
	case IDCANCEL
ret 1
