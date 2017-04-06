\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str m=
 BEGIN MENU
 item &ąčę : 101
 >&žsub
 	&ūūū : 102
 END MENU

if(!ShowDialog("Dialog51" &Dialog51 0 0 0 0 0 0 0 0 "" m)) ret

 BEGIN DIALOG
 1 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030000 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
out wParam
sel wParam
	case IDOK
	case IDCANCEL
ret 1

