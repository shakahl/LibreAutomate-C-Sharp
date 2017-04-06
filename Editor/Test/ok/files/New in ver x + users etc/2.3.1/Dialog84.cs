\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog84" &Dialog84)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030106 "" "" ""

ret
 messages
__MemBmp- mb ;;memory device context
int- x
sel message
	case WM_INITDIALOG
	mb.Create(200 100)
	SetTimer hDlg 1 250 0
	
	case WM_TIMER
	sel wParam
		case 1
		 draw line to random y in memory DC
		int y=RandomInt(0 99)
		RECT r.right=200; r.bottom=100
		if(x=0) ;;erase
			FillRect mb.dc &r GetStockObject(WHITE_BRUSH)
			MoveToEx mb.dc 0 y 0
		int pen0=SelectObject(mb.dc CreatePen(PS_SOLID 1 0xc000)) ;;select green pen
		LineTo mb.dc x y
		DeleteObject SelectObject(mb.dc pen0)
		x+8; if(x>=200) x=0
		InvalidateRect hDlg &r 0 ;;send WM_PAINT now
	
	case WM_PAINT
	 copy from memory DC to window DC
	PAINTSTRUCT ps
	BeginPaint hDlg &ps
	BitBlt ps.hDC 0 0 200 100 mb.dc 0 0 SRCCOPY
	EndPaint hDlg &ps
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
