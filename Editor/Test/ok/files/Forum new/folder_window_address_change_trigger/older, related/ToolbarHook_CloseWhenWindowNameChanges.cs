 /
function# hwnd message wParam lParam

 This function can be used with any QM toolbar as a toolbar hook function.
 Closes the toolbar when its owner window name changes.
 Just add this first line in toolbar text (1 space character at the beginning):
  /hook ToolbarHook_CloseWhenWindowNameChanges


sel message
	case WM_INITDIALOG
	str* s1._new; SetProp(hwnd "ownerText" s1)
	s1.getwintext(GetToolbarOwner(hwnd))
	SetTimer hwnd 1 500 0
	
	case WM_TIMER
	sel wParam
		case 1
		s1=+GetProp(hwnd "ownerText")
		str s2.getwintext(GetToolbarOwner(hwnd))
		if s2!=*s1
			KillTimer hwnd wParam
			clo hwnd
	
	case WM_DESTROY
	RemoveProp(hwnd "ownerText")
