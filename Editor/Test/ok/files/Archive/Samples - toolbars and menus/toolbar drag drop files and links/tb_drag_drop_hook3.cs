 /tb_drag_drop3
function# hWnd message wParam lParam

 this also inserts button

sel message
	case WM_INITDIALOG
	int htb=wParam
	QmRegisterDropTarget(htb hWnd 16)
	
	case WM_DESTROY
	case WM_QM_DRAGDROP
	TB_DropInsertButton hWnd lParam
