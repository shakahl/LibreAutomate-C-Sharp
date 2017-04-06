 /
function nCode wParam KBDLLHOOKSTRUCT&h

OSKVAR- v

if(h.flags&LLKHF_INJECTED) goto g1
int i=OSK_FindVK(h.vkCode); if(i<0) goto g1

if h.flags&LLKHF_UP
	SetTimer v.hwnd h.vkCode v.tr &OSK_TimerProc
else
	InvalidateRect v.hwnd 0 0

 g1
ret CallNextHookEx(0 nCode wParam &h)
