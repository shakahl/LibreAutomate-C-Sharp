\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_static_scroll" &dlg_static_scroll)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 4 4 118 32 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030004 "" "" ""
 3 Edit 0x54231044 0x200 6 6 96 48 ""
 3 RichEdit20A 0x54233044 0x200 4 4 96 48 ""

ret
 messages
int-- y
sel message
	case WM_INITDIALOG
	str s
	 s.getmacro("dlg_static_scroll")
	 s.setwintext(id(3 hDlg))
	SetTimer hDlg 1 50 0
	
	case WM_TIMER
	sel wParam
		case 1
		 this works but only with edit controls and scrolls whole line
		 SendMessage id(3 hDlg) WM_VSCROLL SB_LINEDOWN 0
		
		 this does not work
		 ScrollWindowEx id(3 hDlg) 0 -1 0 0 0 0 SW_ERASE|SW_INVALIDATE
		 UpdateWindow id(3 hDlg)
		  RedrawWindow id(3 hDlg) 0 0 RDW_INVALIDATE|RDW_ERASE
		
		int h=id(3 hDlg)
		int dc=GetDC(h)
		RECT r; GetClientRect(h &r); r.top=y; y-1
		FillRect dc &r COLOR_BTNFACE+1
		SetBkMode dc TRANSPARENT
		
		int f0=SelectObject(dc GetStockObject(DEFAULT_GUI_FONT))
		
		s.getmacro("dlg_static_scroll")
		DrawText dc s -1 &r 0
		
		SelectObject(dc f0)
		ReleaseDC(h dc)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
