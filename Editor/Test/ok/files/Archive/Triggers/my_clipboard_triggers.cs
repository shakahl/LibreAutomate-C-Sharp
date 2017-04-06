\Dialog_Editor

 A lightweight version of clipboard triggers. Can be used in exe. Not integrated into the Properties dialog.
 To define strings and macros, edit the sel code at the end.


AddTrayIcon "$qm$\copy.ico" "Clipboard triggers.[]Ctrl+click to exit." ;;remove this if don't need tray icon

str dd=
 BEGIN DIALOG
 0 "" 0x80C800C8 0x0 0 0 224 136 "My Clipboard Triggers"
 END DIALOG
 DIALOG EDITOR: "" 0x2040201 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 HWND_MESSAGE 128)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	int-- t_hwndnext=SetClipboardViewer(hDlg)
	SetTimer hDlg 23 1000 0
	
	case WM_DESTROY
	ChangeClipboardChain hDlg t_hwndnext
	
	case WM_TIMER
	sel wParam
		case 23 ;;auto restore if another viewer did not remove itself correctly
		if(!GetClipboardViewer) t_hwndnext=SetClipboardViewer(hDlg)
	
	case WM_COMMAND ret 1
	
	case WM_CHANGECBCHAIN
	if(wParam=t_hwndnext) t_hwndnext=lParam
	else if(t_hwndnext) SendMessage t_hwndnext message wParam lParam
	
	case WM_DRAWCLIPBOARD ;;clipboard contents changed
	SendMessage t_hwndnext message wParam lParam
	goto onEvent
ret

 onEvent
str s.getclip; err ret
 out s

 edit this code: add/remove case statements with your strings and functions
sel s 2 ;;flag 2 - wildcard. In recent QM versions you also can use flag 4 to support regular expressions.
	case "*Quick*"
	out "Quick"
	 mac "Function301" ;;runs Function301 in new thread. Or you can call it directly, if its execution time is less than 0.1 s.
	
	case "*Macros*"
	out "Macros"
	 mac "Function302"
	
	 ...

err+
