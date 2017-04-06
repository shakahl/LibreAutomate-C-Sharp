\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_register_hot_key_Ctrl_Q" &dlg_register_hot_key_Ctrl_Q 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030301 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__RegisterHotKey-- r.Register(hDlg 5 MOD_CONTROL 'Q')
	case WM_HOTKEY out wParam
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
