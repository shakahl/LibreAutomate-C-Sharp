\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str c3Che lb4
if(!ShowDialog("dlg_context_help2" &dlg_context_help2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x400 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54012003 0x0 12 16 48 12 "Check"
 4 ListBox 0x54230101 0x200 10 34 96 48 ""
 5 Static 0x54000000 0x0 8 92 212 22 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030108 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_HELP
	HELPINFO& h=+lParam
	str s
	sel h.iCtrlId
		case 3 s="this is a check box"
		case 4 s="this is a list box"
	s.setwintext(id(5 hDlg))
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
