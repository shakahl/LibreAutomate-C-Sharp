 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
if(!ShowDialog("dialog_detect_fullscreen" &dialog_detect_fullscreen)) ret

 BEGIN DIALOG
 0 "" 0x90C808C8 0x88 0 0 72 23 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1000 0
	case WM_TIMER
	sel wParam
		case 1 IsOnScreen(hDlg)
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
