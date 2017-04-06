\Dialog_Editor
out
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ComboBox 0x54230243 0x0 8 8 96 213 ""
 4 Button 0x54032000 0x0 8 36 48 14 "Set"
 5 Button 0x54032000 0x0 64 36 48 14 "Get"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3"
str cb3
cb3="&one[]two[]three"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret
out cb3


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4
	if(DT_SetControl(hDlg 3 "Sunday[]&Monday")) out 1
	case 5
	if(DT_GetControl(hDlg 3 &_s)) out _s
	 if(DT_GetControl(id(3 hDlg) 0 &_s)) out _s
ret 1
