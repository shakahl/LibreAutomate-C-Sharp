 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_INITDIALOG
	SetTimer hWnd 1 500 0
	
	case WM_TIMER
	str s.getwintext(GetToolbarOwner(hWnd)); err
	out s ;;delete this
	sel s 2
		case ["Quick Macros *","* Stack Overflow - *"]
		if(!hid(hWnd)) hid hWnd
		case else
		if(hid(hWnd)) hid- hWnd
	
	case WM_DESTROY
	

err+
