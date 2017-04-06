 /
function# nCode wParam lParam
if(nCode!=HC_ACTION) goto g1

 --------------------------

 Your code here.
 Be very careful, or QM may stop working.

CWPSTRUCT& r=+lParam
OutWinMsg r.message r.wParam r.lParam

 --------------------------

 g1
ret CallNextHookEx(g_hook_qm.m_hs nCode wParam +lParam)
