out
type HWVAR hwnd hwndedit shrinking ;;add more members if needed
HWVAR v
 MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 30 30 0x90000000|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST
MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 50 50 0x90000000|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST
 MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 50 50 0x90000000|WS_THICKFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST
 MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 50 50 0x90000000|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST 0 0 0 CS_SAVEBITS

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
KillTimer hwnd 1

RECT r e t; GetWindowRect hwnd &r
int dx(100) dy(100)
int x(r.left-dx) y(r.top-dy) cx(r.right-x) cy(r.bottom-y)

OffsetRect &r -r.left -r.top
e=r; e.right+dx; e.bottom+dy
OffsetRect &r dx dy
 outRECT r; outRECT e

opt waitmsg 1
 SetThreadPriority GetCurrentThread THREAD_PRIORITY_HIGHEST
PF
int useRgn=1 ;;TODO: don't use region if WS_THICKFRAME or not WS_POPUP
if useRgn
	int cursor=SetCursor(0) ;;less flickering; when cursor is over the window, flickers always
	t=r
	SetWindowRgn(hwnd, CreateRectRgnIndirect(&t), 0)
PN

 if(expand) out; v.shrinking=1
sub.Size hwnd x y cx cy
PN
UpdateWindow hwnd
 v.shrinking=0
PN

int i
 useRgn=0
if useRgn
	 0.05
	 0.5
	int n=8
	for(i 1 n+1)
		t.left=r.left-(r.left*i/n)
		t.top=r.top-(r.top*i/n)
		 outRECT t
		SetWindowRgn(hwnd, CreateRectRgnIndirect(&t), 1)
		 0.3
		 SetWindowPos hwnd 0 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_FRAMECHANGED
		
		 wait(0.01);
		Sleep 10
		if(i=1) SetCursor(cursor)
		 RECT rr; GetWindowRect hwnd &rr; outRECT rr

PN;PO

0.5
DestroyWindow hwnd


#sub Size
function hwnd x y cx cy

SetWindowPos hwnd, 0, x y cx cy, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING
 SetWindowPos hwnd, 0, x y cx cy, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING|SWP_NOREDRAW
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

SetTimer hwnd 1 500 0

 SetWinStyle hwnd GetWinStyle(hwnd)~WS_DLGFRAME|WS_THICKFRAME 8 ;;works, but slow
 Transparent hwnd 200

