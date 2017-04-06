\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_timers" &dialog_timers)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030506 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1000 0
	SetTimer hDlg 2 2000 0
	
	case WM_TIMER
	sel wParam
		case 1
		out 1
		
		case 2
		KillTimer hDlg 2
		out 2
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
