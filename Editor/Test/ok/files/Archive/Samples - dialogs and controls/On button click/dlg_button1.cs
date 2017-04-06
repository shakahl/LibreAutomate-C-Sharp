\Dialog_Editor

 Shows how to run code on button click, without closing the dialog.

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_button1" &dlg_button1)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 18 16 48 14 "Button3"
 4 Button 0x54032000 0x0 18 36 48 14 "Button4"
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
	case 3
	out "Button3"
	
	case 4
	out "Button4"
	
	case IDOK
	out "OK"
	
	case IDCANCEL
ret 1

 3 and 4 are button id, as specified in text in BEGIN DIALOG ... END DIALOG
