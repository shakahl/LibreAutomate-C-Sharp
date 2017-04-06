\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog85" &Dialog85 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030200 "*" "" ""

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
	case IDCANCEL
ret 1
