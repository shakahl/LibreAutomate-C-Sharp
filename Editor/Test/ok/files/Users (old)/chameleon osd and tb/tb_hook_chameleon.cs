 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG
	__MemBmp* mb._new
	SetProp hWnd "tb_hook_chameleon_mb" mb
	RECT r
	GetClientRect hWnd &r
	POINT p; ClientToScreen hWnd &p
	mb.Create(r.right r.bottom 1 p.x p.y)
	
	case WM_DESTROY
	mb=+GetProp(hWnd "tb_hook_chameleon_mb")
	mb._delete
	RemoveProp hWnd "tb_hook_chameleon_mb"
	
	case WM_ERASEBKGND
	mb=+GetProp(hWnd "tb_hook_chameleon_mb")
	GetClientRect hWnd &r
	BitBlt wParam 0 0 r.right r.bottom mb.dc 0 0 SRCCOPY
	ret 1
