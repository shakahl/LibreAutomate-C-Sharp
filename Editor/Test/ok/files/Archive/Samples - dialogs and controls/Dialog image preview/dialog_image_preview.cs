\Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 8 10 48 14 "Open..."
 END DIALOG
 DIALOG EDITOR: "" 0x2030000 "" "" ""

if(!ShowDialog(dd &sub.DlgProc)) ret


#sub DlgProc
function# hDlg message wParam lParam
__MemBmp-- mb
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_PAINT
	PAINTSTRUCT p; int dc=BeginPaint(hDlg &p)
	RECT r; GetClientRect hDlg &r; FillRect dc &r GetSysColorBrush(COLOR_BTNFACE)
	BITMAP b; GetObjectW mb.bm sizeof(BITMAP) &b
	BitBlt dc 100 10 b.bmWidth b.bmHeight mb.dc 0 0 SRCCOPY
	EndPaint hDlg &p
	
ret
 messages2
sel wParam
	case 3
	if(!OpenSaveDialog(0 _s "bmp, png, jpg, gif[]*.bmp;*.png;*.jpg;*.gif[]")) ret
	mb.Attach(LoadPictureFile(_s))
	RedrawWindow hDlg 0 0 RDW_INVALIDATE
	
	case IDOK
	case IDCANCEL
ret 1
