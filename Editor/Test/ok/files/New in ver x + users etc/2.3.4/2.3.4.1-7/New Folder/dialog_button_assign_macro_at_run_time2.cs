\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dialog_button_assign_macro_at_run_time)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 8 48 14 "A"
 4 Button 0x54032000 0x0 6 26 48 14 "B"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
IStringMap-- m
sel message
	case WM_INITDIALOG
	m=CreateStringMap(4)
	
	case WM_SETCURSOR
	if lParam>>16=WM_RBUTTONUP and wintest(wParam "" "Button")
		str sh(wParam) sm
		if(!inp(sm "Macro to assign to the button:" "" "" 0 "" 0 hDlg)) ret
		if(sm.len) m.Add(sh sm); else m.Remove(sh)
		
	case WM_COMMAND goto messages2
ret
 messages2
if wParam>>16=0 and m.Get2(F"{lParam}" sm)
	mes sm
	 mac sm; err
	ret 1

sel wParam
	case IDOK
	case IDCANCEL
ret 1
