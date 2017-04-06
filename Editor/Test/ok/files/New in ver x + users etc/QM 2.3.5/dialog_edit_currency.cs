\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 note: this code (4 lines) can be in other macro
str controls = "7"
str e7Pri
if(!ShowDialog("dialog_edit_currency" &dialog_edit_currency &controls)) ret
out e7Pri

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 7 Edit 0x54030080 0x200 8 10 96 14 "Price"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030503 "" "" "" ""

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
	str s.getwintext(id(7 hDlg))
	CURRENCY cy=s; err
	if cy<=0
		mes "incorret currency format"
		ret ;;don't close dialog
	case IDCANCEL
ret 1
