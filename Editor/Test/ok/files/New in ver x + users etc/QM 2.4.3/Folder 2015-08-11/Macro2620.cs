type HWVAR hwnd hwndedit shrinking ;;add more members if needed
HWVAR v
 MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 30 30 0x90000000|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST
MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 600 500 50 50 0x90000000|WS_DLGFRAME 0 0 WS_EX_TOOLWINDOW|WS_EX_TOPMOST

 Read more in MainWindow help.


#sub WndProc v
function# hWnd message wParam lParam
 if(v.shrinking) OutWinMsg message wParam lParam
sel message
	case WM_CREATE: sub.WM_CREATE hWnd
	case WM_DESTROY: PostQuitMessage 0
	case WM_TIMER sub.Test hWnd
	
	 case WM_ERASEBKGND if(v.shrinking) ret 1
	 case WM_NCPAINT if(v.shrinking) ret
	

ret DefWindowProc(hWnd message wParam lParam)


#sub Test v
function hwnd
RECT r; GetWindowRect hwnd &r
int expand
if(r.right-r.left<100) expand=1; r.left-=100; r.top-=100; else r.left+=100; r.top+=100
PF
 SendMessage hwnd WM_SETREDRAW 0 0
 SetWinStyle hwnd WS_DLGFRAME 2|8 ;;works, but slow
SetWinStyle hwnd GetWinStyle(hwnd)~WS_DLGFRAME|WS_THICKFRAME 8 ;;works, but slow
 hid hwnd
 ShowWindow hwnd 0
 Transparent hwnd 0
 mov+ r.left r.top r.right-r.left r.bottom-r.top hwnd
 if(expand) out; v.shrinking=1
PN
SetWindowPos hwnd, 0, r.left r.top r.right-r.left r.bottom-r.top, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING
 SetWindowPos hwnd, 0, r.left r.top r.right-r.left r.bottom-r.top, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING|SWP_HIDEWINDOW
 SetWindowPos hwnd, 0, r.left r.top r.right-r.left r.bottom-r.top, SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER|SWP_NOSENDCHANGING|SWP_SHOWWINDOW
PN
 v.shrinking=0
 Sleep 50
 Transparent hwnd 256
 ShowWindow hwnd SW_SHOWNOACTIVATE
 hid- hwnd
 SetWinStyle hwnd WS_DLGFRAME 1|8
SetWinStyle hwnd GetWinStyle(hwnd)|WS_DLGFRAME~WS_THICKFRAME 8 ;;works, but slow
 SendMessage hwnd WM_SETREDRAW 1 0
PN; if(expand) PO

 AnimateWindow(hwnd 0 AW_HOR_NEGATIVE|AW_VER_NEGATIVE)


#sub WM_CREATE v
function hwnd

v.hwnd=hwnd

SetTimer hwnd 1 1000 0

 BEGIN PROJECT
 main_function  Macro2619
 exe_file  $my qm$\Macro2619.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 end_hotkey  120
 guid  {8D9A623B-8207-470E-8A1E-F86B1BB26F33}
 END PROJECT
