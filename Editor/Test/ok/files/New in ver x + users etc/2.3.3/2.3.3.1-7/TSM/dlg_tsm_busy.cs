\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("" &dlg_tsm_busy &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54231044 0x200 0 0 224 134 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1200 0
	case WM_TIMER
	1
	 15; clo hDlg
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
