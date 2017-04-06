 /dlg_apihook
function# hdc x y options RECT*lprect @*lpString c *lpDx

 0.1
int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnExtTextOutW hdc x y options lprect lpString c lpDx)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(!R) ret R

if options&ETO_GLYPH_INDEX
	out "<GLYPH"

RECT r
r.left=x; r.top=y
if(options&ETO_CLIPPED and lprect) r.right=lprect.right; r.bottom=lprect.bottom

CommonTextFunc 2 hdc lpString c r

ret R
