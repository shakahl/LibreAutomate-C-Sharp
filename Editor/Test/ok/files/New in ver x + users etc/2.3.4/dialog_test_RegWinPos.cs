\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

hDlg=ShowDialog("" &dialog_test_RegWinPos 0 0 1)
 hDlg=ShowDialog("" &dialog_test_RegWinPos 0 0 1|128)
MessageLoop

 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 221 132 "DialogRWP"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030307 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	RegWinPos(hDlg "rwp" "\test" 0)
	 RegWinPos(hDlg "rwp" "\test" 0 1)
	SetTimer hDlg 1 2000 0
	
	case WM_DESTROY
	RegWinPos hDlg "rwp" "\test" 1
	PostQuitMessage 0
	
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		ShowWindow hDlg SW_SHOW
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
