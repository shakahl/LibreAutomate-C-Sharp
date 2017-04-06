 /
function# hWnd message wParam lParam

 note: this is not necessary in QM 2.3.3 and later, but makes faster.

sel message
	case WM_INITDIALOG
	SetTimer hWnd -1 200 0
	
	case WM_TIMER
	sel wParam
		case -1
		int w=GetToolbarOwner(hWnd)
		int wa=GetAncestor(w 2)
		if(w=wa) ret
		if !IsWindow(w)
			clo hWnd
		else if min(wa) or hid(wa)
			if(!hid(hWnd)) hid w; hid hWnd
		else
			if(hid(hWnd)) hid- w; hid- hWnd

err+
