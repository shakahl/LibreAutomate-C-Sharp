 /
function# hwnd message wParam lParam uIdSubclass dwRefData

 This function can be used with SetWindowSubclass as window procedure.
 <help>SetWindowSubclass</help> is the recommended way to subclass windows. Easier and safer than SetWindowLong. Example: SetWindowSubclass(hwnd &sub.WndProc_Subclass 1 0)


 OutWinMsg message wParam lParam ;;uncomment to see received messages

 sel message
	 case ...

int R=DefSubclassProc(hwnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hwnd &ThisFunction uIdSubclass) ;;replace ThisFunction with the name of this function or subfunction (eg sub.WndProc_Subclass)
	
	 case ...

ret R
