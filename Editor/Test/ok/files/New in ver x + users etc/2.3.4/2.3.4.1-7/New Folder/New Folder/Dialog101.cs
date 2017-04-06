\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog101" &Dialog101 0)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 217 129 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	RegWinPos hDlg "my web browser" "\MyMacros" 0 1
	EnsureWindowInScreen hDlg 1
	
	case WM_DESTROY
	RegWinPos hDlg "my web browser" "\MyMacros" 1
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
