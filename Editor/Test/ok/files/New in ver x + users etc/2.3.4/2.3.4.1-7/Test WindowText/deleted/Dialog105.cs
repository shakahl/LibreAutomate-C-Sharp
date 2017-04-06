\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("Dialog105" &Dialog105 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90FC0AC8 0x0 0 0 81 36 "Dialog"
 4 Edit 0x50231044 0x200 0 6 46 12 ""
 3 Edit 0x50231044 0x200 0 12 46 22 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	out
	 SetTimer hDlg 1 1000 0
	 case WM_TIMER
	case WM_LBUTTONDOWN
	goto g1
	case WM_COMMAND goto messages2
ret
 messages2
ret 1

 g1
int i w dc

w=hDlg
w=id(3 hDlg)

 RedrawWindow w 0 0 RDW_INVALIDATE|RDW_FRAME|RDW_ERASE
 RECT ru; GetClientRect w &ru; ru.right/2
 RedrawWindow w &ru 0 RDW_INVALIDATE|RDW_FRAME|RDW_ERASE
 outx GetClassLong(w GCL_STYLE)

for i 0 3
	sel i
		case 0 dc=GetDC(w)
		case 1 dc=GetWindowDC(w)
		case 2 PAINTSTRUCT ps; dc=BeginPaint(w &ps)
		case 3 dc=GetDCEx(w 0 DCX_CACHE|DCX_CLIPSIBLINGS)
	
	 IsWindowDC(w dc) close
	 _________________________
	
	RECT r
	 GetUpdateRect w &r 0
	 GetClipBox dc &r
	
	__GdiHandle hr=CreateRectRgn(0 0 0 0)
	out GetRandomRgn(dc hr 4)
	GetRgnBox(hr &r)
	
	zRECT r
	
	 SetRect &r 0 0 10 1
	 out RectVisible(dc &r)
	
	 _________________________
	if(i=2) EndPaint w &ps
	else ReleaseDC w dc
