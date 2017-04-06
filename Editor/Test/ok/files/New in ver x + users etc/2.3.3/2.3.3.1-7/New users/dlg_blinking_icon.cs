\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str si3
si3="qm.exe"
if(!ShowDialog("dlg_blinking_icon" &dlg_blinking_icon &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000003 0x0 14 12 16 16 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 500 0
	
	case WM_TIMER
	sel wParam
		case 1
		_i=id(3 hDlg)
		ShowWindow _i iif(IsWindowVisible(_i) SW_HIDE SW_SHOW)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
