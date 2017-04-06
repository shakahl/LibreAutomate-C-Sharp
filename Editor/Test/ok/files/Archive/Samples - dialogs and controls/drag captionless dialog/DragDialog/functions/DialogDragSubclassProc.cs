 /
function# hWnd message wParam lParam

 Used internally in case a control is subclassed. Don't call explicitly.


sel message
	case [WM_LBUTTONDOWN,WM_LBUTTONUP,WM_MOUSEMOVE,WM_CANCELMODE]
	if(DragDialog(hWnd message GetProp(hWnd "qm_modifiers"))) ret

int wndproc=GetProp(hWnd "qm_wndproc"); if(!wndproc) ret
int r=CallWindowProc(wndproc hWnd message wParam lParam)

if(message=WM_NCDESTROY)
	RemoveProp(hWnd "qm_wndproc")
	RemoveProp(hWnd "qm_modifiers")

ret r
