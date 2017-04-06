 \Dialog_Editor
function# hDlg message wParam lParam
 messages
 OutWinMsg message wParam lParam
sel message
	case WM_INITDIALOG
	DT_SetBackgroundColor hDlg 1 0xff8080 0xffff
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 7
	if(ColorDialog(_i _s hDlg)) _s.setwintext(id(4 hDlg))
	case IDOK
	case IDCANCEL
ret 1
