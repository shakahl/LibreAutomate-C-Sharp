 /
function# hWnd message wParam lParam

 Disables only several menu items.

sel message
	case WM_COMMAND
	if(!lParam)
		 out wParam ;;see what is clicked
		sel wParam
			case [32833,32827,32828,33174] ret 1
			case else ret
