 /
function# hDlg message wParam lParam
if(hDlg) goto messages


ret
 messages
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	g.ColumnsWidthAdjust(5);; n columns=5
	
	case WM_DESTROY
	case WM_COMMAND goto messages2

ret
 messages2
sel wParam
	case IDOK
ret 1