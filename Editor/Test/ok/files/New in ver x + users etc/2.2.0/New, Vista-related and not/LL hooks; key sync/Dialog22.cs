\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog22" &Dialog22)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 83 36 "Form"
 END DIALOG
 DIALOG EDITOR: "" 0x2020000 "" ""

ret
 messages
sel message
	case WM_KEYDOWN
	0.2
	 case WM_KEYUP
	 0.1
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
