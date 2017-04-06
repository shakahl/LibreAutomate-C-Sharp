 /Toolbar26
function# hWnd message wParam lParam

sel message
	case WM_CREATE
	__Font* f._new
	f.Create("Comic Sans MS" 20)
	SetProp hWnd "font" f
	
	case WM_DESTROY
	f=+GetProp(hWnd "font")
	f._delete
	
	case WM_NOTIFY
	NMHDR* nh=+lParam
	if(nh.code=NM_CUSTOMDRAW)
		NMTBCUSTOMDRAW* cd=nh
		f=+GetProp(hWnd "font")
		__Font& fr=*f
		out cd.nmcd.dwDrawStage
		sel cd.nmcd.dwDrawStage
			case CDDS_PREPAINT
			ret CDRF_NOTIFYITEMDRAW
			
			case CDDS_ITEMPREPAINT
			SelectObject cd.nmcd.hdc fr
			ret CDRF_NEWFONT
			
			
		
	 does not work
