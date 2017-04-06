 /
function nCode wParam MSLLHOOKSTRUCT*h

WFKMDATA- __wfkm
if(nCode!=HC_ACTION) goto gr

__wfkm.r=wParam
if(__wfkm.ms) *__wfkm.ms=*h
__wfkm.w=1
 gr
ret CallNextHookEx(0 nCode wParam h)
