 /tb_drag_button
function# hWnd message wParam lParam

if(TB_DetectDragDropButton(&hWnd _s))
	out "dropped %s" _s

sel message
	case WM_INITDIALOG
	
	case WM_DESTROY
	
