\Dialog_Editor
out
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 222 136 "Dialog"
 3 QM_ComboBox 0x54030242 0x0 28 38 96 13 ""
 4 Button 0x54012003 0x0 112 8 48 10 "Check"
 5 Button 0x54032000 0x0 28 8 48 14 "Button"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3 4"
str qmcb3 c4Che

if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret
out qmcb3


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case CBN_DROPDOWN<<16|3
	 out
	str s=
	 ,,0x401|8
	 zero
	 one
	 two
	DT_SetControl hDlg 3 s
	
ret 1
