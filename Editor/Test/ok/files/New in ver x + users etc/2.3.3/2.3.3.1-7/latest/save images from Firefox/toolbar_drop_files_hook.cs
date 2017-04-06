 /toolbar drop files
function# hWnd message wParam lParam

sel message
	case WM_INITDIALOG
	int w=CreateControl(0 "ActiveX" "SHDocVw.WebBrowser" 0 0 22 200 200 hWnd 3)
	_s="$desktop$\test"; _s.setwintext(w) ;;open your folder for images
	
	case WM_DESTROY
	
