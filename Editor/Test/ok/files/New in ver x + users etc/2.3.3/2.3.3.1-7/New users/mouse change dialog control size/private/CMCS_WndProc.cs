 /
function# hWnd message wParam lParam

CMCS_DATA* p=+GetProp(hWnd "CMCS_DATA")
if(!p) ret
int wndProc=p.wndProc

sel message
	case WM_NCDESTROY
	p._delete
	RemoveProp(hWnd "CMCS_DATA")
	
	case WM_MOUSEMOVE
	if !p.big
		p.big=1
		GetWindowRect hWnd &p.r; MapWindowPoints 0 GetParent(hWnd) +&p.r 2 ;;save old rect
		RECT r=p.r; InflateRect &r p.sizePlus p.sizePlus; MoveWindow hWnd r.left r.top r.right-r.left r.bottom-r.top 1 ;;make bigger
		TRACKMOUSEEVENT t.cbSize=16; t.dwFlags=TME_LEAVE; t.hwndTrack=hWnd; TrackMouseEvent &t ;;set to notify when mouse leaves
	
	case WM_MOUSELEAVE
	if p.big
		p.big=0
		MoveWindow hWnd p.r.left p.r.top p.r.right-p.r.left p.r.bottom-p.r.top 1

ret CallWindowProc(wndProc hWnd message wParam lParam)
