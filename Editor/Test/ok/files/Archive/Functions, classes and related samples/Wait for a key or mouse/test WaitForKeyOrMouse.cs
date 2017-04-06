 2
 out WaitForKeyOrMouse
 out WaitForKeyOrMouse(3)

KBDLLHOOKSTRUCT ks
MSLLHOOKSTRUCT ms
int k=WaitForKeyOrMouse(0 ks ms)
 out k
str what action
if(k<256) ;;key
	qm.FormatKeyString k 0 &what
	action=iif(ks.flags&LLKHF_UP "released" "pressed")
	
	out "key %s %s" what action
else ;;mouse
	sel k
		case [WM_LBUTTONDOWN,WM_LBUTTONUP] what="left"
		case [WM_RBUTTONDOWN,WM_RBUTTONUP] what="right"
		case [WM_MBUTTONDOWN,WM_MBUTTONUP] what="middle"
		case [WM_XBUTTONDOWN,WM_XBUTTONUP] sel(ms.mouseData>>16) case XBUTTON1 what="X1"; case XBUTTON2 what="X2"
		case WM_MOUSEMOVE what="move"
		case WM_MOUSEWHEEL what="wheel"; action=iif(ms.mouseData>>16&0x8000 "backward" "forward")
	sel k
		case [WM_LBUTTONDOWN,WM_RBUTTONDOWN,WM_MBUTTONDOWN,WM_XBUTTONDOWN] action="pressed"
		case [WM_LBUTTONUP,WM_RBUTTONUP,WM_MBUTTONUP,WM_XBUTTONUP] action="released"
	
	out "mouse %s %s at %i %i" what action ms.pt.x ms.pt.y
