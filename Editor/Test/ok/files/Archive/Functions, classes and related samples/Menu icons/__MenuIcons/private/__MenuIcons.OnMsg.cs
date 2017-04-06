 /Menu icons test
function# hwnd message wParam lParam

int sub=GetProp(hwnd "__mi_sub")
if(!sub) ret DefWindowProcW(hwnd message wParam lParam)
int r=CallWindowProcW(sub hwnd message wParam lParam)

 sel(message) case [MN_GETHMENU,WM_NCHITTEST] case else OutWinMsg message wParam lParam &_s; out "%s    <%i>" _s hwnd
sel message
	case WM_NCDESTROY
	RemoveProp hwnd "__mi_sub"
	
	case WM_PRINTCLIENT ;;sent after creation of main popup menu. Sent after creation of a submenu when invoked with mouse.
	OnDraw(hwnd wParam)
	
	case WM_PAINT ;;sent after creation of a submenu when invoked with keyboard
	OnDraw(hwnd)
	
	case 0x1E5 ;;undocumented NM_SELECTITEM, sent on mouse move, with wParam = item index if on item or -1 if not
	OnDraw(hwnd)
	
	case WM_KEYDOWN ;;keyboard navigation (arrows)
	OnDraw(hwnd)

ret r
