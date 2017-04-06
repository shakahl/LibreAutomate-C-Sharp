\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

#compile __CRegisterHotKey

if(!ShowDialog("dlg_registerhotkey" &dlg_registerhotkey 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 6 8 94 12 "Press Ctrl+K or Shift+F7"
 END DIALOG
 DIALOG EDITOR: "" 0x2030008 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	CRegisterHotKey-- hk
	hk.Register("Ck" hDlg 1); err out _error.description
	hk.Register("SF7" hDlg 2); err out _error.description
	
	case WM_HOTKEY
	sel wParam
		case 1 _s="Ctrl+K pressed"; _s.setwintext(id(3 hDlg))
		case 2 _s="Shift+F7 pressed"; _s.setwintext(id(3 hDlg))
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
