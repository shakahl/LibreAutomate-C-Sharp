 /
function nCode wParam MSLLHOOKSTRUCT*h

WFMEDATA- __wfme
if(nCode!=HC_ACTION) goto gr
if(h.flags&LLMHF_INJECTED and __wfme.flags&4=0) goto gr

int r eatAndContinue

if __wfme.flags&1=0 ;;need down
	 gd
	sel(wParam) case WM_LBUTTONDOWN r=1; case WM_RBUTTONDOWN r=2; case WM_MBUTTONDOWN r=4; case WM_XBUTTONDOWN r=8
else ;;need up
	sel(wParam) case WM_LBUTTONUP r=1; case WM_RBUTTONUP r=2; case WM_MBUTTONUP r=4; case WM_XBUTTONUP r=8
	if(!r and __wfme.flags&2) eatAndContinue=1; goto gd ;;if need up, and eat, also eat down

sel r
	case 8 if(h.mouseData>>16!XBUTTON1) r=16
	case 0 sel(wParam) case WM_MOUSEMOVE r=32; case WM_MOUSEWHEEL r=iif(h.mouseData&0x8000000 128 64); case 0x020E r=iif(h.mouseData&0x8000000 512 256) ;;WM_MOUSEHWHEEL

if r&__wfme.events
	if(__wfme.flags&8=0 and GetMod!__wfme.mod) goto gr
	if(eatAndContinue and r<=16) __wfme.flags|8; ret 1
	__wfme.r=r
	__wfme.w=1
	if(__wfme.flags&2) ret 1

 gr
ret CallNextHookEx(0 nCode wParam h)
