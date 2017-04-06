 
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 112 96 "NEALBOT"
 3 Button 0x54032000 0x0 6 74 48 14 "ON"
 4 Button 0x54032000 0x0 58 74 48 14 "OFF"
 5 ListBox 0x54230101 0x200 8 10 96 48 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

str controls = "5"
str lb5
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
	case 3
	mac "NEALBot Ver.3"
	case 4
	EndThread "NEALBot Ver.3"
	
	case IDOK
	case IDCANCEL
ret 1