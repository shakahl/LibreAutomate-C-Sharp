\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_tray_icon" &dialog_tray_icon 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 2 Button 0x54030000 0x4 120 116 48 14 "Hide"
 3 Button 0x54032000 0x0 172 116 48 14 "Exit"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	Tray-- t_ti
	t_ti.AddIcon("$qm$\copy.ico" "tooltip" 5 hDlg)
	 clo hDlg ;;hide
	
	case WM_DESTROY
	t_ti.Delete
	
	case WM_USER+101
	sel lParam
		case WM_LBUTTONUP ;;on left click tray icon
		 gShow
		act hDlg; err
		
		case WM_RBUTTONUP ;;on right click tray icon
		sel ShowMenu("1 Show[]2 Exit" hDlg)
			case 1 goto gShow
			case 2 goto gExit
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDCANCEL ;;Hide, X
	min hDlg; hid hDlg; ret
	
	case 3 ;;Exit
	 gExit
	DT_Cancel hDlg
ret 1
