 /ToolbarWithChildDialog
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	int hDlg=ShowDialog("TWCD_Dialog" &TWCD_Dialog 0 hWnd 1 WS_CHILD)
	mov 0 22 hDlg
	RECT r; GetClientRect(hDlg &r); siz r.right r.bottom+22 hWnd ;;autosize, because dialog dimensions depend on system font settings
	
	case WM_DESTROY
	
