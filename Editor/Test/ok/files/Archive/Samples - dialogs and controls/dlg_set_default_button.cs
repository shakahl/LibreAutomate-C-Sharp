\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "5"
str lb5
lb5="3[]4"
if(!ShowDialog("" &dlg_set_default_button &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 74 48 14 "Button 3"
 4 Button 0x54032000 0x0 58 74 48 14 "Button 4"
 5 ListBox 0x54230101 0x200 4 16 102 48 ""
 6 Static 0x54000000 0x0 4 2 100 12 "Set default button:"
 END DIALOG
 DIALOG EDITOR: "" 0x2030001 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case LBN_SELCHANGE<<16|5
	_i=LB_SelectedItem(lParam)
	sel _i
		case 0
		SendMessage hDlg DM_SETDEFID 3 0
		case 1
		SendMessage hDlg DM_SETDEFID 4 0
	
	case 3
	out 3
	
	case 4
	out 4
	
	case IDOK
	case IDCANCEL
ret 1
