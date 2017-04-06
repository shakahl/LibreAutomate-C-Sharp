\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str rea3
if(!ShowDialog("dlg_clipboard_viewer" &dlg_clipboard_viewer &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "QM Clipboard Viewer"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 RichEdit20A 0x54233044 0x200 0 0 224 114 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030008 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int-- t_hwndnext
	t_hwndnext=SetClipboardViewer(hDlg)
	SetTimer hDlg 23 1000 0
	
	case WM_DESTROY
	ChangeClipboardChain hDlg t_hwndnext
	
	case WM_CHANGECBCHAIN
	if(wParam=t_hwndnext) t_hwndnext=lParam
	else if(t_hwndnext) SendMessage t_hwndnext message wParam lParam
	
	case WM_TIMER
	sel wParam
		case 23 ;;auto restore if another viewer did not remove itself correctly
		if(!GetClipboardViewer) t_hwndnext=SetClipboardViewer(hDlg)
	
	case WM_DRAWCLIPBOARD ;;clipboard contents changed
	str s.getclip; err
	s.setwintext(id(3 hDlg))
	SendMessage t_hwndnext message wParam lParam
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

err+
	ChangeClipboardChain hDlg t_hwndnext
	end _error
