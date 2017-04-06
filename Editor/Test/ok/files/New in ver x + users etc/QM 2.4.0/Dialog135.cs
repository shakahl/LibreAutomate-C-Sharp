\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("Dialog135" &Dialog135 &controls)) ret
out e3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 84 10 96 14 ""
 4 Static 0x54000000 0x0 2 12 80 10 "Minimum 4 characters"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030605 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	str s
	s.getwintext(id(3 hDlg))
	if s.len<4
		mes "Minimum 4 characters"
		ret 0 ;;return 0 to not close dialog on OK
	case IDCANCEL
ret 1
