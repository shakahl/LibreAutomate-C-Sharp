 /
function nCode wParam KBDLLHOOKSTRUCT&h

FormatKeyString h.vkCode 0 &_s
out "%s %s%s" _s iif(h.flags&LLKHF_UP "up" "down") iif(h.flags&LLKHF_INJECTED ", injected" "")

ret CallNextHookEx(0 nCode wParam &h)
