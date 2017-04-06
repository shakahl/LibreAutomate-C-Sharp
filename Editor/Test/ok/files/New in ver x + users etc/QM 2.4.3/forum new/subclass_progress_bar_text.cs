 /
function# hWnd message wParam lParam uIdSubclass dwRefData

 This function can be used with SetWindowSubclass as window procedure.
 <help>SetWindowSubclass</help> is the recommended way to subclass windows. Easier and safer than SetWindowLong.

 OutWinMsg message wParam lParam ;;uncomment to see received messages

int R=DefSubclassProc(hWnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hWnd &subclass_progress_bar_text uIdSubclass)
	
	case WM_PAINT
	int dc=GetDC(hWnd)
	int k=SaveDC(dc)
	__Font& f=+dwRefData
	SelectObject(dc f)
	SetBkMode dc TRANSPARENT
	SetTextColor dc 0xFF0000
	TextOutW dc 0 0 L"TEST" 4
	RestoreDC dc k
	ReleaseDC hWnd dc

ret R
