 /dlg_apihook
function# hdc POLYTEXTW*ppt nstrings

if(!ppt or nstrings<1)
	 g1
	ret call(fnPolyTextOutW hdc ppt nstrings)

 goto gf
int- t_inAPI; if(!t_inAPI) t_inAPI=1; else goto g1

 out "%i %i %i 0x%X %i %i %i %i" hdc x y options lprect lpString c lpDx

RECT r
int i
for i 0 nstrings
	POLYTEXTW& k=ppt[i]
	 r.left=x; r.top=y; r.right=x; r.bottom=y
	 if(lprect) r.right+lprect.right; r.bottom+lprect.bottom
	
	CommonTextFunc 4 hdc k.lpstr k.n &r

 gf
 ret call(fnPolyTextOutW hdc ppt nstrings)
int g=call(fnPolyTextOutW hdc ppt nstrings)
 out "after PolyTextOutW"
t_inAPI=0
ret g
