\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_timer_auto_close" &dlg_timer_auto_close)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 4 116 48 14 "OK"
 2 Button 0x54030000 0x4 54 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 116 120 18 10 ""
 4 Static 0x54000000 0x0 136 120 92 10 "Click to stop countdown."
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "" "" ""

ret
 messages
DT_AutoCloseTimer hDlg 3 5 1 ;;display timer in control id 3; auto close after 5 seconds; to close, click button id 1
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
