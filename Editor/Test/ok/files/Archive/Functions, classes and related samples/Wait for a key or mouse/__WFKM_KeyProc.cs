 /
function nCode wParam KBDLLHOOKSTRUCT*h

WFKMDATA- __wfkm
if(nCode!=HC_ACTION) goto gr

__wfkm.r=h.vkCode&255

sel __wfkm.r
	case [VK_LCONTROL,VK_RCONTROL] __wfkm.r=VK_CONTROL
	case [VK_LSHIFT,VK_RSHIFT] __wfkm.r=VK_SHIFT
	case [VK_LMENU,VK_RMENU] __wfkm.r=VK_MENU
	case VK_RWIN __wfkm.r=VK_LWIN

if(__wfkm.ks) *__wfkm.ks=*h
__wfkm.w=1
 gr
ret CallNextHookEx(0 nCode wParam h)
