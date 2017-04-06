 /dlg_apihook
function# hDestDC x y nWidth nHeight hSrcDC xSrc ySrc dwRop

int hwnd=WindowFromDC(hDestDC)
if(!hwnd) goto gf
int- t_inAPI; if(t_inAPI) out "<BitBlt in API>"

out "src=%i dest=%i hwndDest=%s" hSrcDC hDestDC _s.outw(hwnd)

 gf
ret call(fnBitBlt hDestDC x y nWidth nHeight hSrcDC xSrc ySrc dwRop)
