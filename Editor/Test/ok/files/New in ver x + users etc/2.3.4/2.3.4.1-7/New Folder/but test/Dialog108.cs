\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4 5 6 7"
str c3Che o4Opt o5Opt o6Opt c7Che
if(!ShowDialog("Dialog108" &Dialog108 &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54012003 0x0 6 8 48 12 "Check"
 4 Button 0x54032009 0x0 2 42 48 12 "Option first"
 5 Button 0x54002009 0x0 2 58 48 12 "Option next"
 6 Button 0x54002009 0x0 2 74 48 12 "Option next"
 7 Button 0x54012006 0x0 66 8 48 12 "Check 3S"
 8 Button 0x54032000 0x0 66 42 48 14 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
if(wParam>>16=0) out "%i %i" wParam IsDlgButtonChecked(hDlg wParam)
sel wParam
	case 8
	if(!ShowDialog("Dialog108" &Dialog108 0 hDlg)) ret
ret 1
