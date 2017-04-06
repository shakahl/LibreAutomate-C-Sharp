\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
if(!ShowDialog("Dialog35" &Dialog35)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x203000B "" "" ""

ret
 messages
OutWinMsg message wParam lParam
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
