 /
function# nCode wParam lParam
if(nCode!=HC_ACTION or wParam!=PM_REMOVE) goto g1

 --------------------------

 Your code here.
 Be very careful, or QM may stop working.

MSG& r=+lParam
sel(r.message) case WM_TIMER goto g1
OutWinMsg r.message r.wParam r.lParam

 --------------------------

 g1
ret CallNextHookEx(g_hook_qm.m_hs nCode wParam +lParam)
