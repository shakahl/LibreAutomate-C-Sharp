 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG
	hid id(9999 hWnd)
	ShowDialog("dlg_tb_child" &dlg_tb_child 0 hWnd 1 WS_CHILD WS_POPUP|WS_CAPTION|WS_SYSMENU 0 0 8)
	 the 8 adds 8 pixels at the top where you can right click or drag the toolbar
	case WM_DESTROY
	
