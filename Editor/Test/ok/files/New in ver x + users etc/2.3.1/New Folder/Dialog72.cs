\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str rea3 cb4
if(!ShowDialog("" &Dialog72 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 2 0 218 92 ""
 4 ComboBox 0x54230243 0x0 4 104 96 213 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	EnableWindow id(4 hDlg) 0
	SendMessage id(3 hDlg) EM_SETEVENTMASK 0 ENM_SELCHANGE
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.code
	case EN_SELCHANGE
	SELCHANGE* sc=+nh
	sel nh.idFrom
		case 3
		EnableWindow id(4 hDlg) sc.chrg.cpMax!=sc.chrg.cpMin
