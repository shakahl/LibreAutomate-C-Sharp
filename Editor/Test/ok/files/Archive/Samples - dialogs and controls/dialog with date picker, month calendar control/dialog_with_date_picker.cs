 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysDateTimePick32 0x54000000 0x204 8 10 110 14 ""
 4 SysDateTimePick32 0x54000001 0x204 8 32 110 14 ""
 5 SysDateTimePick32 0x54000009 0x204 8 54 110 14 ""
 6 SysDateTimePick32 0x54000004 0x204 8 78 110 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010601 "" ""

ret
 messages
SYSTEMTIME- t_st1 t_st2 t_st3
sel message
	case WM_INITDIALOG
	 here can be initialized using DTM_SETSYSTEMTIME
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	SendMessage(id(3 hDlg) DTM_GETSYSTEMTIME 0 &t_st1)
	SendMessage(id(4 hDlg) DTM_GETSYSTEMTIME 0 &t_st2)
	SendMessage(id(5 hDlg) DTM_GETSYSTEMTIME 0 &t_st3)
	
	case IDCANCEL
ret 1
