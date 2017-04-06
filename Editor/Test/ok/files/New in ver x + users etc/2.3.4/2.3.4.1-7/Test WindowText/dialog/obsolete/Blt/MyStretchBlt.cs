 /dlg_apihook
function# hDestDC x y nWidth nHeight hSrcDC xSrc ySrc dwRop

int R=call(fnBitBlt hDestDC x y nWidth nHeight hSrcDC xSrc ySrc dwRop)

int hwnd=WindowFromDC(hDestDC)
if(!hwnd) ret R
int- t_inAPI t_all; if(t_inAPI) out "<BitBlt in API>"

int- t_out
if(t_out) out "BitBlt   %i -> %i   hwndDest=%s" hSrcDC hDestDC _s.outw(hwnd)

 RECT r.left=x; r.top=y; r.right=r.left+nWidth; r.bottom=r.top+nHeight
 CommonTextFunc 100 hDestDC L" " 1 &r

ret R
