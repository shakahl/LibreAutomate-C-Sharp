\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "6 7 8"
str e6 e7 e8
e8="request data"
if(!ShowDialog("DDE_server_with_dialog" &DDE_server_with_dialog &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 6 6 48 12 "Execute"
 4 Static 0x54000000 0x0 6 30 48 12 "Poke"
 5 Static 0x54000000 0x0 6 54 48 12 "Request"
 6 Edit 0x54230844 0x20000 56 4 96 18 ""
 7 Edit 0x54230844 0x20000 56 28 96 18 ""
 8 Edit 0x54231044 0x200 56 52 96 18 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	out
	DdeServerStart("QM_dlg" &dde_server_callback2 hDlg)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
