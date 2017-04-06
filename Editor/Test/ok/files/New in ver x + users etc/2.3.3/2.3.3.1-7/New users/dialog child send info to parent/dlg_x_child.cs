 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 106 22 "Child dialog"
 4 Button 0x54032000 0x0 18 4 66 14 "Send to parent"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case 4 ;;Send to parent
	str info="test"
	SendMessage GetParent(hDlg) WM_APP+10 hDlg &info ;;use message >=WM_APP
ret 1
