\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog510" &Dialog510 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 184 "Dialog"
 3 SysTreeView32 0x54030000 0x0 2 2 164 178 ""
 4 Button 0x54032000 0x0 172 6 48 14 "Refresh"
 5 Button 0x54032000 0x0 172 30 48 16 "Save"
 END DIALOG
 DIALOG EDITOR: "" 0x2030208 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	TvAddItems id(3 hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2

ret
 messages2
sel wParam
	case 4 TvAddItems id(3 hDlg)
	case 5 TvGetCheckedItems id(3 hDlg)

	case IDOK
	case IDCANCEL
ret 1

