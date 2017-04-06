function# hwnd message wParam lParam

 OutWinMsg message wParam lParam

sel message
	case WM_CREATE
	WmCreate(hwnd +lParam)
	
	case WM_COMMAND
	ret WmCommand(wParam lParam)
	
	case [WM_SETTEXT,WM_GETTEXT,WM_GETTEXTLENGTH]
	sel m_type
		case 1 ret _WinSetGetText(message wParam +lParam)
	ret
	
	case WM_CTLCOLOREDIT
	if(sub._WmCtlColorEdit(wParam lParam _i)) ret _i
	
	case WM_LBUTTONDOWN
	sel m_type
		case 1
		if GetDlgCtrlID(child(mouse))=520
			if(mw_what<0) _WinSelect(1)
			_WinCapture
	
	case WM_RBUTTONUP
	sel m_type
		case 1 if(GetDlgCtrlID(child(mouse))=520) sub._WinCaptureMenu
	
	case __TWM_DRAGDROP
	sel m_type
		case 1 ret _WinCapture(wParam +lParam)
	ret
	
	case __TWM_SETFLAGS
	sel(lParam&3) case 1 m_flags|wParam; case 2 m_flags~wParam; case else m_flags=wParam
	ret
	
	case __TWM_SETLOCK
	ret _WinLock(wParam)
	
	case __TWM_SELECT
	ret _WinSelect(wParam)
	
	case __TWM_GETSELECTED
	ret mw_what
	
	case __TWM_GETCAPTUREDHWND
	 Returns captured/selected window or control handle.
	 Returns 0 if not captured/selected, or if Screen, or if Control but no control captured.
	sel(mw_what) case 1 ret mw_captW; case 2 ret mw_captC
	ret

ret DefWindowProcW(hwnd message wParam lParam)


#sub _WmCtlColorEdit c
function! wParam lParam &R

if(m_type!1) ret
if (lParam=mw_heW and mw_what<1) or (lParam=mw_heC and mw_what<2) ;;set gray
	SetTextColor wParam GetSysColor(COLOR_GRAYTEXT); SetBkMode wParam TRANSPARENT
	R=GetSysColorBrush(COLOR_BTNFACE)
	ret 1
 note: COLOR_GRAYTEXT does not match cue banner color, which is unknown and depends on theme etc. But it is safer.


#sub _WinCaptureMenu c
int hp=GetAncestor(m_hparent 2)
sel sub_to.DragTool_Menu(hp "{+}" 3)
	case 102 _WinCapture(2)
