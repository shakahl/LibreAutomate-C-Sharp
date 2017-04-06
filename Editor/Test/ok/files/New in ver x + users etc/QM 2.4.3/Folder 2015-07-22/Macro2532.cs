function# hwnd message wParam lParam
sel message
	case WM_NCCREATE
	int- t_WM_QM_DIALOGCONTROLDATA=RegisterWindowMessage("WM_QM_DIALOGCONTROLDATA")
	 ...
	case else
	if message=t_WM_QM_DIALOGCONTROLDATA
		str& dialogVar=+lParam
		sel wParam
			case 0 ret sub.MyGetControlData(dialogVar)
			case 1 ret sub.MySetControlData(dialogVar)
ret DefWindowProcW(hwnd message wParam lParam)
