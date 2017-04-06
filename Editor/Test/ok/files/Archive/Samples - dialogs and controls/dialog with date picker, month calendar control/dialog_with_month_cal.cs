 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysMonthCal32 0x54030000 0x0 6 6 102 98 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

ret
 messages
SYSTEMTIME- t_st1
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 SendMessage(id(3 hDlg) MCM_GETCURSEL 0 &t_st1)
	
	case IDCANCEL
ret 1
 messages3
NMHDR* n=+lParam
sel n.code
	case MCN_SELECT out 1
	case MCN_SELCHANGE out 2
	