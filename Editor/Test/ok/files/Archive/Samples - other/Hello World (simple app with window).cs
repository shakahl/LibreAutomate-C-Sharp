type HWVAR hwnd hwndedit ;;add more members if needed
HWVAR v
MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 200 200 400 200

 Read more in MainWindow help.


#sub WndProc
function# hWnd message wParam lParam
sel message
	case WM_CREATE: sub.WM_CREATE hWnd
	case WM_COMMAND: sub.WM_COMMAND wParam lParam
	case WM_DESTROY: PostQuitMessage 0
ret DefWindowProc(hWnd message wParam lParam)


#sub WM_CREATE v
function hwnd

v.hwnd=hwnd
 create child windows
CreateControl 0 "Button" "Open ..." 0 4 4 60 24 hwnd 4
RECT r; GetClientRect hwnd &r
v.hwndedit=CreateControl(0x200 "Edit" "" WS_VSCROLL|ES_AUTOVSCROLL|ES_MULTILINE|ES_WANTRETURN 0 32 r.right r.bottom-32 hwnd 3)
DT_SetAutoSizeControls hwnd "3s"


#sub WM_COMMAND v
function wParam lParam

str s sf
sel wParam
	case 4
	if(OpenSaveDialog(0 &sf "Text files[]*.txt[]" "txt"))
		s.getfile(sf)
		s.setwintext(v.hwndedit)
