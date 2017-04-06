 /tb QM menu icons
function# hWnd message wParam lParam

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_INITDIALOG
	out SetThreadMenuIcons("32995=0 2002=1 32833=2 32834=4" "$qm$\il_qm.bmp" 1)
	
	case WM_DESTROY
	
