 /
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG ;;QM 2.2.0
	mov 0 50 id(9999 hWnd)
	
	case WM_SIZE
	ret 1 ;;prevent restoring child tb position when resized
