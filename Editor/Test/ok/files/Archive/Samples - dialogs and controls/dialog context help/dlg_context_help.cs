\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_context_help" &dlg_context_help 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x400 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030108 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_SYSCOMMAND
	sel wParam&0xfff0
		case SC_CONTEXTHELP
		mes "help"
		ret 1
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
