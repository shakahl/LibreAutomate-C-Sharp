 /
function hWnd message wParam lParam

 Makes a window (eg dialog or toolbar) visible on all virtual desktops.
 Call this function from a window procedure (eg dialog procedure or toolbar
 hook procedure), on every message.

 EXAMPLE
 WindowOnAllDesktops hDlg message wParam lParam
 sel message
	  ...


type WINDOWPOS hWnd hWndInsertAfter x y cx cy flags
dll user32 #IntersectRect RECT*lpDestRect RECT*lpSrc1Rect RECT*lpSrc2Rect

sel message
	case WM_WINDOWPOSCHANGING
	if(!GetProp(hWnd "closing"))
		WINDOWPOS* wp=+lParam
		if(wp.flags&SWP_HIDEWINDOW) wp.flags~SWP_HIDEWINDOW

	case WM_MOVE
	RECT rtb rsc r; int x y
	rsc.right=ScreenWidth; rsc.bottom=ScreenHeight
	GetWindowRect hWnd &rtb
	if(IntersectRect(&r &rsc &rtb)) SetProp hWnd "pos" (rtb.left&0xffff)|(rtb.top<<16)
	else
		x=GetProp(hWnd "pos")
		y=x>>16; x&0xffff
		if(x&0x8000) x|0xffff0000
		if(y&0x8000) y|0xffff0000
		mov x y hWnd
	
	case WM_CLOSE
	 g1
	SetProp(hWnd "closing" 1)
	
	case WM_COMMAND sel(wParam) case [IDOK,IDCANCEL] goto g1
	