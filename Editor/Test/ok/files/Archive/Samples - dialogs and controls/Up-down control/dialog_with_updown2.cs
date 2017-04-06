\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
e3=0
if(!ShowDialog("dialog_with_updown2" &dialog_with_updown2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54030080 0x204 16 28 32 13 ""
 4 msctls_updown32 0x54000082 0x4 50 26 11 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010703 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
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
sel nh.idFrom
	case 4
	sel nh.code
		case UDN_DELTAPOS
		ret UdIncrement(nh 3 0.01 0 100) ;;0.01 is step. change it
		
