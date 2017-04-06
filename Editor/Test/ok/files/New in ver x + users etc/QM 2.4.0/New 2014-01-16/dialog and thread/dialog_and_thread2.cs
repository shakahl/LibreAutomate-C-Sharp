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
	SetTimer hDlg 1 50 0
	
	case WM_TIMER
	sel wParam
		case 1
		 mac "and_thread" ;;bad
		
		 good
		__Handle+ g_andThreadEvent g_andEndThreadHandle
		if(!g_andThreadEvent) g_andThreadEvent=CreateEvent(0 0 0 0)
		if(!g_andEndThreadHandle or WaitForSingleObject(g_andEndThreadHandle 0)!=WAIT_TIMEOUT)
			if(g_andEndThreadHandle) CloseHandle g_andEndThreadHandle; g_andEndThreadHandle=0
			int ht=mac("and_thread")
			DuplicateHandle GetCurrentProcess ht GetCurrentProcess &g_andEndThreadHandle 0 0 DUPLICATE_SAME_ACCESS
		
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
