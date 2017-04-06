 /Macro1402
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030205 "" "" ""

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
	case IDCANCEL
	int h=win("Quick")
	dis "Dialog58"
	 act h
	 clo h
	 PostMessage h WM_CLOSE 0 0; 0.5
	 SendMessage h WM_CLOSE 0 0
	 RECT r; GetWindowRect h &r; InflateRect &r -1 0; siz r.right-r.left r.bottom-r.top h
	 RECT r; GetWindowRect h &r; InflateRect &r -10 0; SetWindowPos h 0 0 0 r.right-r.left r.bottom-r.top SWP_NOMOVE|SWP_NOZORDER|SWP_NOACTIVATE
	 PostMessage h WM_USER 0 0; 0.5
	 Sleep 2000
ret 1
