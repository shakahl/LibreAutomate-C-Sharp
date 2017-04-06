 /tb_drag_drop
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG
	int htb=wParam
	QmRegisterDropTarget(htb hWnd 16)
	
	case WM_DESTROY
	case WM_QM_DRAGDROP
	 get button
	str buttonText
	if(!TB_GetDragDropButton(hWnd buttonText)) ret
	out "dropped on '%s':" buttonText
	
	 get files
	QMDRAGDROPINFO& di=+lParam
	str s
	foreach(s di.files) out s
