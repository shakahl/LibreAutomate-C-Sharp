 /
function# hwnd message wParam lParam

DTSIDATA* p=+GetProp(hwnd "dtsidata")

sel message
	case WM_PAINT
	PAINTSTRUCT ps; int dc=BeginPaint(hwnd &ps)
	RECT rc; GetClientRect hwnd &rc; FillRect dc &rc GetSysColorBrush(COLOR_BTNFACE) ;;erase
	BITMAP b; GetObject p.mb.bm sizeof(BITMAP) &b ;;get bitmap dimensions
	BitBlt dc 0 0 b.bmWidth b.bmHeight p.mb.dc 0 0 SRCCOPY ;;memory bitmap -> display
	 TODO: use GdiAlphaBlend instead.
	EndPaint hwnd &ps
	ret
	
	case WM_ERASEBKGND
	ret

int r=CallWindowProcW(p.wndproc hwnd message wParam lParam)

sel message
	case WM_NCDESTROY
	SubclassWindow hwnd p.wndproc
	RemoveProp(hwnd "dtsidata")
	p._delete

ret r
