\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_connect_1" &dlg_connect_1 0)) ret

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 217 129 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
int-- hDlg2
sel message
	case WM_INITDIALOG
	hDlg2=ShowDialog("dlg_connect_2" 0 0 hDlg 1)
	goto g1
	
	case WM_DESTROY
	DestroyWindow hDlg2
	
	case WM_COMMAND goto messages2
	
	case WM_MOVE
	 g1
	RECT r r2
	GetWindowRect hDlg &r
	GetWindowRect hDlg2 &r2
	mov r.left r.top-(r2.bottom-r2.top) hDlg2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
