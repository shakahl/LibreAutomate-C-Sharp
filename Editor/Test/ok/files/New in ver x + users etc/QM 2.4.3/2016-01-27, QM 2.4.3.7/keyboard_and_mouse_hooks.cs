int hkey=SetWindowsHookEx(WH_KEYBOARD_LL &sub.KeyProc _hinst 0)
int hmouse=SetWindowsHookEx(WH_MOUSE_LL &sub.MouseProc _hinst 0)
opt waitmsg 1
wait -1
UnhookWindowsHookEx hkey
UnhookWindowsHookEx hmouse


#sub KeyProc
function nCode wParam KBDLLHOOKSTRUCT*x

if(nCode!=HC_ACTION) goto gr

int k=x.vkCode&255
str what action

sel k
	case [VK_LCONTROL,VK_RCONTROL] k=VK_CONTROL
	case [VK_LSHIFT,VK_RSHIFT] k=VK_SHIFT
	case [VK_LMENU,VK_RMENU] k=VK_MENU
	case VK_RWIN k=VK_LWIN

qm.FormatKeyString k 0 &what
action=iif(x.flags&LLKHF_UP "released" "pressed")

out "key %s %s" what action

 gr
ret CallNextHookEx(0 nCode wParam x)


#sub MouseProc
function nCode wParam MSLLHOOKSTRUCT*x

if(nCode!=HC_ACTION) goto gr

int k=wParam
str what action

sel k
	case [WM_LBUTTONDOWN,WM_LBUTTONUP] what="left"
	case [WM_RBUTTONDOWN,WM_RBUTTONUP] what="right"
	case [WM_MBUTTONDOWN,WM_MBUTTONUP] what="middle"
	case [WM_XBUTTONDOWN,WM_XBUTTONUP] sel(x.mouseData>>16) case XBUTTON1 what="X1"; case XBUTTON2 what="X2"
	case WM_MOUSEMOVE what="move"
	case WM_MOUSEWHEEL what="wheel"; action=iif(x.mouseData>>16&0x8000 "backward" "forward")
sel k
	case [WM_LBUTTONDOWN,WM_RBUTTONDOWN,WM_MBUTTONDOWN,WM_XBUTTONDOWN] action="pressed"
	case [WM_LBUTTONUP,WM_RBUTTONUP,WM_MBUTTONUP,WM_XBUTTONUP] action="released"

out "mouse %s %s at %i %i" what action x.pt.x x.pt.y

 gr
ret CallNextHookEx(0 nCode wParam x)
