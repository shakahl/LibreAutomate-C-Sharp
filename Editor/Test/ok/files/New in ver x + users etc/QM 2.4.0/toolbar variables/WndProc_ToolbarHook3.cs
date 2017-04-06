 /
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

 to avoid many global variables, use a user-defined type
type TB47VARIABLES
	x y
	str'text
TB47VARIABLES+ g_tb47

sel message
	case WM_INITDIALOG
	 you can initialize global variables here, or in other macro
	g_tb47.x=5
	g_tb47.text="toolbar"
	
	case WM_DESTROY
	 clear global variables here
	g_tb47.text.all
