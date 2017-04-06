 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 6 6 48 14 "Help"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	mes "To close or move, right-click in 8-pixel height area at the top."
ret 1
