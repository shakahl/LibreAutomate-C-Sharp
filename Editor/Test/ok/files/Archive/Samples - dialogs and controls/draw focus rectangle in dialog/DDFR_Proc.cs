 /
function button DFR_DATA&d

DrawFocusRect d.dc &d.r ;;erase prev rect
if(button) ret

POINT p; xm p d.hwnd 1
RECT r
r.left=d.p.x; r.top=d.p.y
r.right=p.x; r.bottom=p.y
if(r.right<r.left) _i=r.right; r.right=r.left; r.left=_i
if(r.bottom<r.top) _i=r.bottom; r.bottom=r.top; r.top=_i
DrawFocusRect d.dc &r
d.r=r
