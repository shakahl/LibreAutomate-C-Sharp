 /Dialog127
function# hWnd message wParam lParam uIdSubclass dwRefData

 This function can be used with SetWindowSubclass as window procedure.
 SetWindowSubclass is the recommended way to subclass windows. Easier and safer than SetWindowLong or SubclassWindow. Documented in MSDN.
 Note: Before QM 2.3.5, SetWindowSubclass and related functions were unavailable on Windows 2000, unless explicitly declared like QM now declares them.


 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_NCCALCSIZE
	out wParam
	if !wParam
		RECT& rr=+lParam
		if(rr.right-rr.left>=30) rr.right-15
	

int R=DefSubclassProc(hWnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hWnd &WndProc_Subclass_for_SetWindowSubclass2 1)
	 Tip: If you allocated an object etc and passed it to SetWindowSubclass through dwRefData, here is a good place to free it.
	
	 case ...

ret R
