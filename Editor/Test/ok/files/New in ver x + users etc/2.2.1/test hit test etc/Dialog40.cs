\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

#compile dwmapi

if(!ShowDialog("Dialog40" &Dialog40)) ret

 BEGIN DIALOG
 0 "" 0x10CB0A44 0x8 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 6 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetWinStyle hDlg WS_CHILD 1
	 SetWindowTheme hDlg L"" L""
	 
	 _i=DWMNCRP_DISABLED
	 out DwmSetWindowAttribute(hDlg DWMWA_NCRENDERING_POLICY &_i 4)
	 _i=1
	 out DwmSetWindowAttribute(hDlg DWMWA_TRANSITIONS_FORCEDISABLED &_i 4)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	 case WM_PAINT
	 ret 1
ret
 messages2
sel wParam
	case 3
	RECT r; GetWindowRect hDlg &r
	r.right+1000
	r.bottom+1000
	out "%i %i" r.right r.bottom
	SetWindowPos hDlg 0 r.left r.top r.right-r.left r.bottom-r.top 0
	case IDOK
	case IDCANCEL
ret 1
