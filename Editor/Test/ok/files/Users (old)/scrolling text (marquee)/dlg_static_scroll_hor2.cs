\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_static_scroll_hor" &dlg_static_scroll_hor)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x20000 4 4 114 13 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030004 "" "" ""

ret
 messages
int-- x
str-- s
sel message
	case WM_INITDIALOG
	s="DrawTextW mb.dc @s -1 &r DT_EXPANDTABS|DT_NOPREFIX"
	SetTimer hDlg 1 10 0
	
	case WM_TIMER
	sel wParam
		case 1
		int h=id(3 hDlg)
		RECT r; GetClientRect(h &r)
		__MemBmp mb.Create(r.right r.bottom)
		FillRect mb.dc &r COLOR_BTNFACE+1
		SetBkMode mb.dc TRANSPARENT
		int f0=SelectObject(mb.dc GetStockObject(DEFAULT_GUI_FONT))
		
		r.left=x; x-1
		if(x<-300) x=r.right
		DrawTextW mb.dc @s -1 &r DT_EXPANDTABS|DT_NOPREFIX
		
		SelectObject(mb.dc f0)
		
		int dc=GetDC(h)
		BitBlt dc 0 0 r.right r.bottom mb.dc 0 0 SRCCOPY
		ReleaseDC(h dc)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
