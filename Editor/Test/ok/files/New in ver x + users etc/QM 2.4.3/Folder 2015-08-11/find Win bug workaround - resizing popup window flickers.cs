type HWVAR hwnd hwndedit shrinking ;;add more members if needed
HWVAR v
 MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 30 30 0x90000000|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST
MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 50 50 0x90000000|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST;;|WS_EX_WINDOWEDGE
 MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 50 50 0x90000000|WS_THICKFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST

 Does not flicker if WS_THICKFRAME or no WS_POPUP.

 Read more in MainWindow help.


#sub WndProc v
function# hWnd message wParam lParam
 if(v.shrinking) OutWinMsg message wParam lParam
sel message
	case WM_CREATE: sub.WM_CREATE hWnd
	case WM_DESTROY: PostQuitMessage 0
	case WM_TIMER sub.Test hWnd
	
	 case WM_NCCALCSIZE ret
	
	 case WM_ERASEBKGND if(v.shrinking) ret 1
	 case WM_NCPAINT if(v.shrinking) ret
	
	 case WM_MOVE if(v.shrinking) ret

ret DefWindowProc(hWnd message wParam lParam)


#sub Test v
function hwnd
RECT r; GetWindowRect hwnd &r
int expand
int x y cx cy
if(r.right-r.left<100) expand=1; x=r.left-100; y=r.top-100; else x=r.left+100; y=r.top+100
cx=r.right-x
cy=r.bottom-y

 TRY: SetWindowRgn
PF
 SetWinStyle hwnd GetWinStyle(hwnd)|WS_DLGFRAME~WS_THICKFRAME 8
 SetWinStyle hwnd GetWinStyle(hwnd)~WS_DLGFRAME|WS_THICKFRAME 0 ;;works, but slow
SetWinStyle hwnd GetWinStyle(hwnd)|WS_THICKFRAME 0 ;;works, but slow
PN
 int i cxOld(r.right-r.left) cyOld(r.bottom-r.top) dx(cx-cxOld) dy(cy-cyOld) xOld(r.left) yOld(r.top)
 if(abs(dx)>=8 || abs(dy)>=8)
	 dx/=8; dy/=8;
	 for(i 0 8)
		 cxOld+=dx; cyOld+=dy;
		 xOld-dx; yOld-dy
		 sub.Size hwnd xOld yOld cxOld, cyOld
		 opt waitmsg 1
		 wait(0.01);
		  RECT rr; GetWindowRect hwnd &rr; outRECT rr

if(expand) out; v.shrinking=1
sub.Size hwnd x y cx cy
v.shrinking=0
PN
SetWinStyle hwnd GetWinStyle(hwnd)~WS_THICKFRAME 0
PN;PO

#sub Size
function hwnd x y cx cy

SetWindowPos hwnd, 0, x y cx cy, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING
 SetWindowPos hwnd, 0, x y cx cy, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING|SWP_HIDEWINDOW
 ShowWindow hwnd SW_SHOWNOACTIVATE

 WINDOWPLACEMENT p.Length=sizeof(p)
 GetWindowPlacement(hwnd &p)
 SetRect &p.rcNormalPosition x y x+cx y+cy
 SetWindowPlacement(hwnd &p)

 RECT r; GetWindowRect hwnd &r
 if(x<r.left or y<r.top)
	 SetWindowPos hwnd, 0, r.left r.top cx cy, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING
	 opt waitmsg 1
	 0.15
 SetWindowPos hwnd, 0, x y cx cy, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING



#sub WM_CREATE v
function hwnd

v.hwnd=hwnd

SetTimer hwnd 1 1000 0

 SetWinStyle hwnd GetWinStyle(hwnd)~WS_DLGFRAME|WS_THICKFRAME 8 ;;works, but slow
 Transparent hwnd 200

