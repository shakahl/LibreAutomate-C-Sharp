\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

dll user32 #ChangeWindowMessageFilter message dwFlag
out ChangeWindowMessageFilter(WM_USER+5 1)

if(!ShowDialog("Dialog24" &Dialog24)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 222 134 "Form5"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2020001 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_USER+5 out "ok"
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
