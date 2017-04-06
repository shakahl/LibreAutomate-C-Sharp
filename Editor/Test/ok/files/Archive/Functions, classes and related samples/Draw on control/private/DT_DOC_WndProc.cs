 /
function# hWnd message wParam lParam uIdSubclass __DT_DOC*d

RECT r
sel message
	case WM_PAINT
	GetUpdateRect hWnd &r 0

int R=DefSubclassProc(hWnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hWnd &DT_DOC_WndProc 1)
	
	case WM_PAINT
	__Hdc dc.Init(hWnd)
	_i=SaveDC(dc)
	call d.cbFunc hWnd dc.dc &r d.cbParam
	RestoreDC(dc _i)

ret R
