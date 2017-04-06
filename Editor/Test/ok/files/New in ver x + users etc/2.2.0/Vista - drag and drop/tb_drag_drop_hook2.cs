 /tb_drag_drop2
function% hWnd message wParam lParam

sel message
	case WM_INITDIALOG
	siz 0 20 id(9999 hWnd) 1; err
	QmRegisterDropTarget(wParam hWnd 16)
	 QmRegisterDropTarget(hWnd 0 16)
	
	case WM_SIZE
	ret 1
	
	case WM_DESTROY
	case WM_QM_DRAGDROP
	QMDRAGDROPINFO& di=+lParam
	str s
	foreach(s di.files) out s
