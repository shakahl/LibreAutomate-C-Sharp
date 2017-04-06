function Tray&x message

sel message
	case WM_RBUTTONUP
	sel ShowMenu("1 Show QM[]2 Go to download[]-[]10 Exit")
		case 1 shutdown -4
		case 2 run "http://www.quickmacros.com/download.html"
		case 10 shutdown -7
