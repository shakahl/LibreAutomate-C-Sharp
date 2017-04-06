 /dlg_apihook
function# hdc x y options RECT*lprect @*lpString c *lpDx

 goto gf
int- t_inAPI; if(!t_inAPI) t_inAPI=1; else ret call(fnExtTextOutW hdc x y options lprect lpString c lpDx)

 out "%i %i %i 0x%X %i %i %i %i" hdc x y options lprect lpString c lpDx

RECT r
 r.left=x; r.top=y; r.right=x; r.bottom=y
 if(lprect) r.right+lprect.right; r.bottom+lprect.bottom

CommonTextFunc 2 hdc lpString c &r

 gf
 ret call(fnExtTextOutW hdc x y options lprect lpString c lpDx)
int g=call(fnExtTextOutW hdc x y options lprect lpString c lpDx)
 out "after ExtTextOutW"
t_inAPI=0
ret g
