 /
function nCode wParam KBDLLHOOKSTRUCT*h

if(nCode!=HC_ACTION or h.flags&LLKHF_UP) goto gr

byte vk=h.vkCode
sel vk
	case [VK_LCONTROL,VK_RCONTROL] vk=VK_CONTROL
	case [VK_LSHIFT,VK_RSHIFT] vk=VK_SHIFT
	case [VK_LMENU,VK_RMENU] vk=VK_MENU
	case VK_RWIN vk=VK_LWIN

__WFKDATA- __wfk
if !__wfk.vk or __wfk.vk=vk
	__wfk.r=vk
	if(__wfk.flags&1) ret 1

 gr
ret CallNextHookEx(0 nCode wParam h)
