\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_control_centered" &dlg_control_centered 0)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 217 129 "Dialog"
 3 Button 0x54032000 0x0 82 54 50 16 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SendMessage hDlg WM_SIZE 0 0
	
	case WM_SIZE
	int c=id(3 hDlg)
	RECT rd rc
	GetClientRect hDlg &rd
	GetClientRect c &rc
	mov rd.right/2-(rc.right/2) rd.bottom/2-(rc.bottom/2) c
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
