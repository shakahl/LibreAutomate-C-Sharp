\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 8 16 96 12 ""
 4 Button 0x54032000 0x0 8 0 48 14 "Button"
 5 ComboBox 0x54230242 0x0 8 52 96 213 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""

str controls = "3 5"
str e3 cb5
cb5="one[]two"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


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
	int h=id(5 hDlg)
	SetFocus h
	
	 SendMessage h EM_REPLACESEL 1 "text"
	 SendMessage h EM_SETSEL 0 0
	 SendMessage h WM_SETTEXT 0 "text"
	
	
	case IDCANCEL
ret 1
