 /Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 193 115 "QM power management triggers"
 3 Button 0x54000001 0x4 138 98 50 15 "Exit"
 1 Button 0x44000000 0x4 3 98 48 15 "OK"
 2 Button 0x44000000 0x4 55 98 50 15 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2010800 "" ""

function# hDlg message wParam lParam

sel message
	case WM_CLOSE if(wParam) DestroyWindow(hDlg)
	case WM_DESTROY
	PostQuitMessage(0)
	 
	case WM_COMMAND
	sel wParam
		case IDCANCEL min hDlg; hid hDlg; ret
		case 3 DestroyWindow(hDlg)
	ret 1
	
	case WM_POWERBROADCAST PM_PowerManagement(wParam lParam)
	
	 here you can add more cases for other broadcasted messages, like
	 case WM_SETTINGCHANGE PM_OnWmSettingChange(wParam lParam)
	