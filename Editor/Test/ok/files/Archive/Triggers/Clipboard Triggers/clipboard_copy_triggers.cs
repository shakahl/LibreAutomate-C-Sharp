\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 hidden dialog
hDlg=ShowDialog("clipboard_copy_triggers" &clipboard_copy_triggers 0 0 1)

#ifdef MessageLoop_debug
MessageLoop_debug ;;i use it to debug
#else
MessageLoop
#endif

 BEGIN DIALOG
 0 "" 0x80C80048 0x100 0 0 223 135 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int-- t_hwndnext t_started
	t_hwndnext=SetClipboardViewer(hDlg)
	t_started=1
	_s="QM Clipboard Triggers"; _s.setwintext(hDlg)
	SetTimer hDlg 23 1000 0
	
	case WM_APP
	CCT_table +lParam
	case WM_APP+1
	DestroyWindow hDlg
	
	case WM_DESTROY
	CCT_table
	ChangeClipboardChain hDlg t_hwndnext
	PostQuitMessage 0
	
	case WM_CHANGECBCHAIN
	if(wParam=t_hwndnext) t_hwndnext=lParam
	else if(t_hwndnext) SendMessage t_hwndnext message wParam lParam
	
	case WM_TIMER
	sel wParam
		case 23 ;;auto restore if another viewer did not remove itself correctly
		if(!GetClipboardViewer) t_hwndnext=SetClipboardViewer(hDlg)
	
	case WM_DRAWCLIPBOARD ;;clipboard contents changed
	if(!t_started) ret
	SendMessage t_hwndnext message wParam lParam
	CCT_event
	
	case WM_CLOSE if(wParam) DestroyWindow hDlg
	