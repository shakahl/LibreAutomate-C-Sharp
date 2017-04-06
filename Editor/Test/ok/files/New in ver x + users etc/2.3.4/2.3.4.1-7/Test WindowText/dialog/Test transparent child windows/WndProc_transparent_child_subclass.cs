 /
function# hWnd message wParam lParam

 get old window procedure address, assuming that this was used to sublass: SetProp hWnd "some unique string" SetWindowLongW(hWnd GWL_WNDPROC &My_WndProc_Subclass)
int wp
if(message=WM_NCDESTROY) wp=RemoveProp(hWnd "some unique string"); else wp=GetProp(hWnd "some unique string")
if(!wp) ret

 OutWinMsg message wParam lParam ;;uncomment to see received messages

sel message
	case WM_SIZE ;;(for example)
	

ret CallWindowProcW(wp hWnd message wParam lParam)
