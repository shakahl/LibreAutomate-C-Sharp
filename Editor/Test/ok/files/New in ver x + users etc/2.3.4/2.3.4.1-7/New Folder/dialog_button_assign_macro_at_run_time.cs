\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dialog_button_assign_macro_at_run_time)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 8 48 14 ""
 4 Button 0x54032000 0x0 6 26 48 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	case WM_SETCURSOR
	if lParam>>16=WM_RBUTTONUP and wintest(wParam "" "Button")
		if(!inp(_s "Macro to assign to the button:" "" "" 0 "" 0 hDlg)) ret
		_s.setwintext(wParam)
		
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case [3,4]
	_s.getwintext(lParam)
	if _s.len
		mes _s
		 mac _s; err
	
	case IDOK
	case IDCANCEL
ret 1
