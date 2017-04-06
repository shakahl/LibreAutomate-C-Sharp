\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dialog_and_thread" &dialog_and_thread)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040000 "" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 500 0
	int- t_iid=qmitem("and_thread")
	
	case WM_TIMER
	sel wParam
		case 1
		 mac "and_thread" ;;bad
		
		 good
		__Handle+ g_andThreadEvent; if(!g_andThreadEvent) g_andThreadEvent=CreateEvent(0 0 0 0)
		SetEvent g_andThreadEvent
		if(!IsThreadRunning(+t_iid)) mac("and_thread")
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
