
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 50 6 ""
 3 RichEdit20A 0x54032082 0x200 0 0 50 6 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str re3
re3="thisismyrealpassword"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	SetTimer hDlg
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
