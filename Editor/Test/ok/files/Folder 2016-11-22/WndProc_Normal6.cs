out
1

__RegisterWindowClass+ __MyClass1.Register("MyClass1" &sub.WndProc_Normal 4)
int w=CreateWindowEx(0 "MyClass1" 0 WS_POPUP|WS_VISIBLE|WS_CAPTION|WS_SYSMENU 300 300 300 300 0 0 0 0)
MessageLoop


#sub WndProc_Normal
function# hwnd message wParam lParam

int-- t_waitCursor=LoadCursor(0 +IDC_WAIT)

OutWinMsg message wParam lParam _s
 if(IsMouseCursor(IDC_WAIT)) out F"<><c 0xff>{_s}</c>"
if(GetCursor=t_waitCursor) out F"<><c 0xff>{_s}</c>"
 else out _s

 Remove this code if don't need.
type __WINDOW_DATA_MyClass1 hwnd ;;add more member variables
__WINDOW_DATA_MyClass1* d
if(message=WM_NCCREATE) SetWindowLong hwnd __MyClass1.baseClassCbWndExtra d._new; d.hwnd=hwnd
else d=+GetWindowLong(hwnd __MyClass1.baseClassCbWndExtra); if(!d) ret DefWindowProcW(hwnd message wParam lParam)

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_NCCREATE
	SetCursor LoadCursor(0 +IDC_ARROW)
	
	case WM_DESTROY
	PostQuitMessage 0 ;;enable this line if the thread must end when this window closed
	
	 ...


int R=DefWindowProcW(hwnd message wParam lParam)
 If the class is not Unicode, use DefWindowProc (without W) instead. __RegisterWindowClass registers only Unicode classes.

 sel message
	 case ...

if(message=WM_NCDESTROY) d._delete
ret R
