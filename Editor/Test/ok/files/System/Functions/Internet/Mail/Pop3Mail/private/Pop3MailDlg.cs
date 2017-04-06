 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 210 65 "QM - receiving mail"
 2 Button 0x54030001 0x4 86 48 48 14 "Cancel"
 4 Static 0x54020000 0x4 4 4 36 13 "Account:"
 3 Edit 0x54030880 0x4 44 4 164 13 ""
 7 Static 0x54020000 0x4 4 22 152 10 "Connecting..."
 8 Static 0x54020000 0x4 164 22 44 10 ""
 5 msctls_progress32 0x54000000 0x20004 4 36 202 10 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010500 "" ""

ret
 messages
sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDCANCEL
	MailBee.POP3* p=+DT_GetParam(hDlg)
	if(p and p.Busy) p.Abort
ret 1
