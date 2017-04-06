 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("NS_ProgressDialog" &NS_ProgressDialog)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 58 ""
 2 Button 0x54030000 0x4 82 40 48 14 "Cancel"
 3 msctls_progress32 0x54030000 0x0 4 22 216 14 ""
 4 Static 0x54000000 0x0 4 4 216 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	_s=_command; _s.setwintext(hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_APP SendMessage id(3 hDlg) PBM_SETPOS wParam 0
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
