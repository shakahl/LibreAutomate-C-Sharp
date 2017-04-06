int hwnd=val(_command)
 outw hwnd
out "-------"

ARRAY(MOUSEMOVEPOINT) a.create(64)
POINT p; xm p
MOUSEMOVEPOINT m.x=p.x; m.y=p.y
int i=GetMouseMovePointsEx(16 &m &a[0] 64 GMMP_USE_DISPLAY_POINTS)
out i
