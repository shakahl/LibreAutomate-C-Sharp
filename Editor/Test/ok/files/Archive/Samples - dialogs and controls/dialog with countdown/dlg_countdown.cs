 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54000800 0x0 10 112 34 10 "tim"
 4 Static 0x54000000 0x0 10 122 94 10 "Click to stop the countdown."
 END DIALOG
 DIALOG EDITOR: "" 0x2030000 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 1000 0
	
	case WM_TIMER
	int t=GetDlgItemInt(hDlg 3 0 0)
	t-1; if(t<=0) clo hDlg; ret
	SetDlgItemInt(hDlg 3 t 0)
	
	case WM_SETCURSOR
	if(lParam>>16=WM_LBUTTONDOWN) KillTimer hDlg 1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

