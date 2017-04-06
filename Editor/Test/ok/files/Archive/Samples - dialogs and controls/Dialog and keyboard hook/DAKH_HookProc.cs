 /dialog_and_keyboard_hook
function nCode wParam KBDLLHOOKSTRUCT&h

FormatKeyString h.vkCode 0 &_s; out "%s %s" _s iif(h.flags&LLKHF_UP "up" "down") ;;disable this

 Example of triggers for keys F7 and F8.
 With Ctrl etc need more work, eg reliably remember key state.

if(h.flags&(LLKHF_INJECTED|LLKHF_UP)) goto gr

int- t_hdlg
sel h.vkCode
	case [VK_F7,VK_F8] SendMessage t_hdlg WM_APP+10 0 h.vkCode

 gr
ret CallNextHookEx(0 nCode wParam &h)
