\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
if(!ShowDialog("Dialog83" &Dialog83)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 8 14 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030108 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	SIZE si
	
	int dc=GetDC(hDlg)
	if(GetDcSize(dc si)) out "%i %i" si.cx si.cy
	ReleaseDC(hDlg dc)
	
	__MemBmp mb.Create(100 200)
	if(GetDcSize(mb.dc si)) out "%i %i" si.cx si.cy
	
	case IDOK
	case IDCANCEL
ret 1
