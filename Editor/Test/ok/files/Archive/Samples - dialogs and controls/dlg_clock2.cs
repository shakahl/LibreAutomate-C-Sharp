\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_clock2" &dlg_clock2)) ret

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 8 8 48 12 "clock"
 END DIALOG
 DIALOG EDITOR: "" 0x2010601 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1000 0
	goto g1
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_TIMER
	 g1
	_s.time("%X")
	_s.setwintext(id(3 hDlg))
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
