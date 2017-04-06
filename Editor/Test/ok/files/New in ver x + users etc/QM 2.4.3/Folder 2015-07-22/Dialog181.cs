\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Edit 0x54030080 0x200 8 8 96 12 ""
 4 Edit 0x54231044 0x200 8 28 96 48 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3 4"
str e3 e4
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	_s="kk"
	 _s.setwintext(id(3 hDlg))
	 _s.setwintext(id(4 hDlg))
	SendMessage id(3 hDlg) EM_REPLACESEL 1 _s
	SendMessage id(4 hDlg) EM_REPLACESEL 1 _s
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case EN_CHANGE<<16|3
	out 3
	case EN_CHANGE<<16|4
	out 4
ret 1
