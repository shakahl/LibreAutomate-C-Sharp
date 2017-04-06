 /dlg_apihook
function# hdc x y @*lpString c

 goto gf
int- t_inAPI; if(!t_inAPI) t_inAPI=1; else ret call(fnTextOutW hdc x y lpString c)

 out "%i %i %i 0x%X %i %i %i %i" hdc x y options lprect lpString c lpDx

RECT r
 r.left=x; r.top=y; r.right=x; r.bottom=y
 if(lprect) r.right+lprect.right; r.bottom+lprect.bottom

CommonTextFunc 3 hdc lpString c &r

 gf
 ret call(fnTextOutW hdc x y lpString c)
int g=call(fnTextOutW hdc x y lpString c)
 out "after TextOutW"
t_inAPI=0
ret g
