\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030506 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam
__RegisterHotKey- t_hk1 t_hk2
sel message
	case WM_INITDIALOG
	t_hk1.Register(hDlg 1 MOD_CONTROL|MOD_SHIFT VK_F5) ;;Ctrl+Shift+F5
	t_hk2.Register(hDlg 2 MOD_ALT|MOD_WIN 'Q') ;;Alt+Win+Q
	
	case WM_HOTKEY
	sel wParam
		case 1 out "Ctrl+Shift+F5"
		case 2 out "Alt+Win+Q"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
