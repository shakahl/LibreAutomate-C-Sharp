 /
function# hWnd message wParam lParam

 This is window procedure of windows of "xclass" class. It is similar to dialog procedure. Read about window procedures in MSDN library.
 WM_CREATE, WM_DESTROY and other messages are documented in MSDN library.

sel message
	case WM_CREATE
	ShowWindow hWnd SW_SHOWNOACTIVATE
	 here you can create controls (CreateControl), load graphics, etc
	
	case WM_DESTROY
	
	 add more messages here

ret DefWindowProc(hWnd message wParam lParam)
