 /dialog_DlgInfo
function# hWnd message wParam lParam

int wp
if(message=WM_NCDESTROY) wp=RemoveProp(hWnd "wndproc"); else wp=GetProp(hWnd "wndproc")
if(!wp) ret

OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_NCPAINT:
	 ret DefWindowProcW(hWnd message wParam lParam)
	  int hdc=GetDCEx(hWnd wParam DCX_WINDOW|DCX_INTERSECTRGN)
	 int hdc=GetWindowDC(hWnd)
	 out hdc
	 FillRect hdc wParam GetStockObject(BLACK_BRUSH)
	  FillRgn hdc wParam GetStockObject(BLACK_BRUSH)
	 ReleaseDC(hWnd hdc)
	 ret
	
	 __GdiHandle-- br=CreateSolidBrush(0xffA0A0)
	__GdiHandle-- br=CreateSolidBrush(0x80C0C0)
	 int-- br=GetStockObject(GRAY_BRUSH)
	
	Q &q
	int hdc=GetWindowDC(hWnd)
	 int hdc=GetDCEx(hWnd 0 DCX_WINDOW)
	 out hdc
	Q &qq
	RECT r; GetClientRect hWnd &r; r.right+2; r.bottom+2
	Q &qqq
	FrameRect hdc &r br
	Q &qqqq
	ReleaseDC(hWnd hdc)
	Q &qqqqq
	outq
	ret

ret CallWindowProcW(wp hWnd message wParam lParam)
