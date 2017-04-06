\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
if(!ShowDialog("Dialog115" &Dialog115 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 SetTimer hDlg 1 5000 0
	 case WM_TIMER
	 KillTimer hDlg wParam
	 Function217
	
	case WM_LBUTTONDOWN
	Function217
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
