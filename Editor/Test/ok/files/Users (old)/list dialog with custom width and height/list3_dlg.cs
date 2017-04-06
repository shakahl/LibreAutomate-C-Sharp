 /
function# hDlg message wParam lParam

 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|3
	_i=LB_SelectedItem(lParam)
	DT_Ok hDlg _i+1
	case IDOK
	case IDCANCEL
ret 1
