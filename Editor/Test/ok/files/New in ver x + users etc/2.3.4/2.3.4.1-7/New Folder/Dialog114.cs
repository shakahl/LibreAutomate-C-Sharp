\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog114" &Dialog114)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_LBUTTONDOWN
	 Q &q
	 int dc=GetDC(0)
	  int dc=CreateCompatibleDC(0)
	 Q &qq
	 int w=WindowFromDC(dc)
	 Q &qqq
	 int a=ReleaseDC(WindowFromDC(dc) dc)
	  int b=DeleteDC(dc)
	 Q &qqqq
	 outq
	 outw w
	  out "%i %i" a b
	 out a
	
	__Hdc dc.Init(hDlg)
	 dc.Attach(GetWindowDC(hDlg))
	 dc.Attach(CreateCompatibleDC(0))
	TextOut dc 0 20 "TEST" 4
	dc.Release
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
