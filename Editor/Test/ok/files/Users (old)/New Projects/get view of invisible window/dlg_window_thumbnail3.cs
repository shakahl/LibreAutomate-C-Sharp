\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_window_thumbnail3" &dlg_window_thumbnail3 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 217 143 "Thumbnail"
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 int-- brush=CreateSolidBrush(0x808080)
	int-- brush=GetStockObject(WHITE_BRUSH)
	int h
	 h=win("Notepad")
	h=win("Calc")
	 h=win("Orai")
	 h=_hwndqm
	
	RECT r; GetWindowRect h &r; AdjustWindowRectEx &r GetWinStyle(hDlg) 0 0
	MoveWindow hDlg r.left r.top r.right-r.left r.bottom-r.top 0
	DwmShowThumbnail2 hDlg h
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_CTLCOLORDLG ret brush
	 case WM_PAINT
	 PAINTSTRUCT ps
	 int dc=BeginPaint(hDlg &ps)
	  ShowThumbnail dc win("Calc")
	 ShowThumbnail dc win("Quick")
	  ShowThumbnail dc win("Taskbar")
	 EndPaint hDlg &ps
ret
 messages2
sel wParam
	case 3
	 _s="hhh"; _s.setwintext(id(15 win("Notepad")))
	case IDOK
	case IDCANCEL
ret 1
