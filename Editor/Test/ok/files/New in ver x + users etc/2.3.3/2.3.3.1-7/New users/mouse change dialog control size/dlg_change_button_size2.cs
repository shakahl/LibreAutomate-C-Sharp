\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_change_button_size2" &dlg_change_button_size2)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 16 30 16 14 "A"
 4 Button 0x54032000 0x0 32 30 16 14 "B"
 5 Button 0x54032000 0x0 48 30 16 14 "C"
 6 Static 0x54000000 0x0 16 14 98 12 "move mouse on a button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	DT_ChangeMouseControlSize id(3 hDlg) 5
	DT_ChangeMouseControlSize id(4 hDlg) 5
	DT_ChangeMouseControlSize id(5 hDlg) 15
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
