\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
if(!ShowDialog("Dialog65" &Dialog65)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 12 18 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030100 "" "" ""

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
	SetCursor LoadCursor(0 +IDC_WAIT)
	10
	case IDOK
	case IDCANCEL
ret 1
