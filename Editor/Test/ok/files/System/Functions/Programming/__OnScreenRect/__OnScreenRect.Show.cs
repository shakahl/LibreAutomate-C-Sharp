function action [RECT&rect] ;;action: 1,0 show (begin or move), 2 hide, 3 temporarily hide

 Draws on-screen rectangle.

 rect - variable with rectangle coordinates.
   With action 2 and 3 not used and can be 0. With other actions also can be 0, then shows previous rectangle hidden with action 3.

 REMARKS
 Calling this function with action 2 is optional. Hides when destroying the variable.


RECT r rs
action&3
if(m_hwnd and !IsWindow(m_hwnd)) m_hwnd=0

if action<=1
	action=!m_hwnd
	if !&rect
		if(m_hwnd) hid- m_hwnd
		ret
	
	 Limit rect to the virtual screen. If rect is very big (eg 10 * screen width and height), on Vista dwm.exe eats much memory and can hang PC. It happens with any window, regardless of style and dwm/theme attributes.
	GetVirtualScreen(rs.left rs.top rs.right rs.bottom); InflateRect(&rs 4 4); IntersectRect(&r &rect &rs)
	if(__style&1=0) InflateRect(&r 1 1); ;;to draw gray 1px rectangle around rect

sel action
	case 1 ;;begin
	__RegisterWindowClass+ ___osr_class; if(!___osr_class.atom) ___osr_class.Register("QM_Rect" &sub.WndProc 4 0 0 -1)
	m_hwnd=CreateWindowExW(WS_EX_TOOLWINDOW|WS_EX_TOPMOST|WS_EX_LAYERED|WS_EX_TRANSPARENT|WS_EX_NOACTIVATE +___osr_class.atom 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
	SetWindowLong m_hwnd 0 &this
	if(__style&1) SetLayeredWindowAttributes(m_hwnd 0 100 2) ;;__brush alpha
	else SetLayeredWindowAttributes(m_hwnd 0xffffff 0 1) ;;white transparent
	
	case 0 ;;move
	if(EqualRect(&r &m_pr)) hid- m_hwnd; ret
	
	case 2 ;;end
	if(m_hwnd) DestroyWindow(m_hwnd); m_hwnd=0
	
	case 3 ;;temp hide
	if(m_hwnd) hid m_hwnd
	ret
	
 g1
if m_hwnd
	if(IsRectEmpty(&r)) SetRect &r -10000 -10000 -9999 -9999 ;;on XP setwindowpos does not set zero window size
	SetWindowPos(m_hwnd HWND_TOPMOST r.left r.top r.right-r.left r.bottom-r.top SWP_NOACTIVATE)
	hid- m_hwnd
	UpdateWindow(m_hwnd)

m_pr=r


#sub WndProc
function# hWnd message wParam lParam

sel message
	case WM_PAINT
	__OnScreenRect& osr=+GetWindowLong(hWnd 0)
	
	PAINTSTRUCT p
	int dc=BeginPaint(hWnd &p)
	
	RECT r; GetClientRect hWnd &r
	int brush=iif(osr.__brush osr.__brush GetStockObject(BLACK_BRUSH))
	if osr.__style&1
		FillRect dc &r brush
	else
		 erase; white is transparent
		FillRect dc &r GetStockObject(WHITE_BRUSH)
		 draw gray 1px rectangle around rect, to be visible on black objects if main rectangle is black
		__GdiHandle hr=CreateRectRgnIndirect(&r)
		FrameRgn dc hr GetStockObject(GRAY_BRUSH) 1 1
		 draw main rectangle inside rect
		InflateRect(&r -1 -1); SetRectRgn hr r.left r.top r.right r.bottom
		FrameRgn dc hr brush 3 3
	
	EndPaint hWnd &p
	ret

ret DefWindowProcW(hWnd message wParam lParam)
