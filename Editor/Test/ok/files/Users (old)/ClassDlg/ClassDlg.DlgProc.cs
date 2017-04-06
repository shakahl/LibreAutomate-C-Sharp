function# hDlg message wParam lParam

 messages
sel message
	case WM_INITDIALOG ;;don't call DT_InitDialog!
	ret 1
	case WM_DESTROY DT_DeleteData(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 mes "Button"
	case IDOK DT_Ok hDlg
	case IDCANCEL DT_Cancel hDlg
ret 1
