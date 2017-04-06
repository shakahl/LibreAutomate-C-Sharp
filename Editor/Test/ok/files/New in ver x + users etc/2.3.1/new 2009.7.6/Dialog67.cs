\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

hDlg=ShowDialog("Dialog67" &Dialog67 0 0 1)
 wait 1
 hid- hDlg
 SetWindowPos hDlg 0 0 0 0 0 SWP_SHOWWINDOW|SWP_NOZORDER|SWP_NOSIZE|SWP_NOMOVE
opt waitmsg 1
wait 0 -WC hDlg


 BEGIN DIALOG
 0 "" 0xC80840 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030100 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 hid- hDlg
	SetWindowPos hDlg 0 0 0 0 0 SWP_SHOWWINDOW|SWP_NOZORDER|SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
