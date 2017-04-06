 \Dialog_Editor
 /PLA_Main
function# hDlg message wParam lParam
if(hDlg) goto messages

PLA_DATA- d

ret
 messages
sel message
	case WM_INITDIALOG
	PLA_Init hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
str s
sel wParam
	case 5 ;;Browse
	if(!BrowseForFolder(_s)) ret
	qm.EditReplaceSel hDlg 12 _s 3
	PLA_AddFolder
	
	case 11 ;;Refresh
	PLA_AddFolder
	
	case 6 PLA_Control 2 ;;<<
	case 7 PLA_Control 1 ;;Play/Pause
	case 9 PLA_Control 0 ;;Stop
	case 10 PLA_Control 3 ;;>>
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3 ;;listview
	sel nh.code
		case LVN_ITEMACTIVATE ;;double click listview item
		PLA_Control 0
		PLA_Control 1
