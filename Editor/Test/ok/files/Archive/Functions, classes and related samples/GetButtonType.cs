 /
function# hwnd ;;Returns: 0 push button, 1 check, 2 radio, 3 group

 Returns Button control type.


sel GetWinStyle(hwnd)&15
	case [2 3 5 6] ret 1
	case [4 9] ret 2
	case 7 ret 3
