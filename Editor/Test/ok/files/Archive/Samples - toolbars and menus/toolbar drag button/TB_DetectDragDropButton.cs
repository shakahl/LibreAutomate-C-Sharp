 /
function! *phWnd str&buttonText

 Call this function in a toolbar hook function, before sel message.
 It detects when you drag and drop a button somewhere outside the toolbar.
 If you drag and drop a button, it returns 1 and stores button text into buttonText.
 Else it returns 0.

 EXAMPLE
 function# hWnd message wParam lParam
 if(TB_DetectDragDropButton(&hWnd _s))
	 out "dropped %s" _s
 sel message
	 ...


int hWnd message wParam lParam i htb
memcpy &hWnd phWnd 16

sel message
	case WM_SETCURSOR
	if(lParam>>16=WM_LBUTTONDOWN and lParam&0xffff=HTCLIENT and !GetMod and GetWinId(wParam)=9999)
		htb=id(9999 hWnd)
		POINT p; xm p htb 1
		i=SendMessage(htb TB_HITTEST 0 &p) ;;1-based button index
		if(i>0)
			SetProp hWnd "tb_db_button" i
			SetTimer hWnd 28561 10 0
	
	case WM_TIMER
	sel wParam
		case 28561
		htb=id(9999 hWnd)
		if(htb=GetCapture)
			ifk(Z) ReleaseCapture; ret
			SetCursor iif(win(mouse)=hWnd LoadCursor(0 +IDC_ARROW) LoadCursor(GetModuleHandle("ole32") +4))
			ret
		KillTimer hWnd wParam
		if(win(mouse)=hWnd) ret
		ifk((1)) ret
		
		i=RemoveProp(hWnd "tb_db_button")
		Acc a=acc("" "TOOLBAR" htb "" "" 0x1000)
		a.elem=i+1; buttonText=a.Name; err ret
		ret 1
