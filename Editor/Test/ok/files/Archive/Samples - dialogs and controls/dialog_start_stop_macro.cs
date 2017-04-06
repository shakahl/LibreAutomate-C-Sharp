\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_start_stop_macro" &dialog_start_stop_macro 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x108 0 0 111 24 "Dialog"
 3 Button 0x54032000 0x0 6 6 48 14 "Start"
 4 Button 0x54032000 0x0 58 6 48 14 "Stop"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "" "" ""

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
	mac "Function75"
	
	case 4
	shutdown -6 0 "Function75"
	
	case IDOK
	case IDCANCEL
ret 1
