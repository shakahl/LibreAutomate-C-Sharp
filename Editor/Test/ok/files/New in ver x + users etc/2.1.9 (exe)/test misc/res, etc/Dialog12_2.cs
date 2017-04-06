\Dialog_Editor
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK EndDialog hDlg 1
	case IDCANCEL EndDialog hDlg 0
ret 1
