 /
function# hwnd message wParam lParam

 <help #IDH_EXTOOLBAR>Toolbar hook help</help>

 OutWinMsg message wParam lParam ;;uncomment to see received messages

#compile "macro containing type MyToolbarInteractData..."

sel message
	case WM_MYMESSAGE
	MyToolbarInteractData& x=+lParam
	 out x.h.len
	
	case WM_INITDIALOG
	SetWindowSubclass(id(9999 hwnd) &sub.WndProc_Subclass 1 0)
	
	case WM_DESTROY
	


#sub WndProc_Subclass
function# hwnd message wParam lParam uIdSubclass dwRefData

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_MBUTTONUP
	int i=ShowMenu("1 one[]2 two" hwnd)
	out i

int R=DefSubclassProc(hwnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hwnd &sub.WndProc_Subclass uIdSubclass)
	
	 case ...

ret R
