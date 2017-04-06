 /count
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	CreateControl 0 "Static" 0 0 0 0 150 20 hWnd 2
	SetTimer(hWnd 1 1000 0); goto timer
	case WM_TIMER
		if(wParam=1)
			 timer
			int c=GetProp(hWnd "count")
			if(c=5) clo hWnd; ret
			SetProp(hWnd "count" c+1)
			str s.getwintext(id(2 hWnd)); s=val(s)+1; s.setwintext(id(2 hWnd))
			