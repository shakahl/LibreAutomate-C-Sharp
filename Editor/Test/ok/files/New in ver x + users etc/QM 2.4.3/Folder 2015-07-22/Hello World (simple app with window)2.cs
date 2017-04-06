type HWVAR hwnd hcb ;;add more members if needed
HWVAR v
MainWindow "Hello World" "QM_HW_Class" &sub.WndProc 200 200 400 200

 Read more in MainWindow help.


#sub WndProc
function# hWnd message wParam lParam
sel message
	case WM_CREATE: sub.WM_CREATE hWnd
	case WM_COMMAND: sub.WM_COMMAND wParam lParam
	case WM_DESTROY: PostQuitMessage 0
	 case WM_LBUTTONDOWN
	case WM_MOUSEACTIVATE
	end "ff";
	
ret DefWindowProc(hWnd message wParam lParam)


#sub WM_CREATE v
function hwnd

v.hwnd=hwnd
 create child windows
CreateControl 0 "Button" "Test" 0 4 4 60 24 hwnd 4
v.hcb=CreateControl(0 "QM_ComboBox" "" 0x54230243 0 32 200 30 hwnd 3)

str s=
 0,,
 zero
 one
 two
 three

DT_SetControl v.hcb 0 s


#sub WM_COMMAND v
function wParam lParam

str s sf
sel wParam
	case 4
	out 1
	DT_GetControl v.hcb 0 s
	out s

