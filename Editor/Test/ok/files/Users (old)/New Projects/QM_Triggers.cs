 /Dialog_Editor

 BEGIN DIALOG
 0 "" 0x10CF0A44 0x100 0 0 319 213 "QM Triggers"
 3 Button 0x54032009 0x0 0 0 52 12 "All"
 4 Button 0x54002009 0x0 54 0 52 12 "Works in"
 5 Button 0x54002009 0x0 108 0 52 12 "Specific to"
 8 Button 0x54012003 0x0 268 0 48 12 "Enabled"
 7 Static 0x54020000 0x0 0 24 318 10 "Name		Trigger		Program		Filter		Folder"
 6 Static 0x54020000 0x0 166 2 96 12 "PROGRAM"
 END DIALOG

function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DT_Init(hDlg lParam)
	ret 1
	 ----
	case WM_COMMAND
	int msg(wParam >> 16) ctrlid(wParam & 0xFFFF)
	sel msg
		case BN_CLICKED
		sel ctrlid
			case IDOK
			DT_Ok hDlg
			 ----
			case IDCANCEL
			EndDialog(hDlg 0)
	ret 1
	 ----
	case WM_DESTROY DT_DeleteData(hDlg)
