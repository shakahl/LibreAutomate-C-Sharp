 /dlg_apihook
function# hdc @*lpchText cchText RECT*lprc format DRAWTEXTPARAMS*lpdtp

int- t_inAPI t_all; int inAPI=t_inAPI; t_inAPI+1
int R=call(fnDrawTextExW hdc lpchText cchText lprc format lpdtp)
t_inAPI-1; if(inAPI and !t_all) ret R

 ret R
if(!R or format&DT_CALCRECT) ret R
 if(!R) ret R

RECT r=*lprc
if lpdtp
	r.left+lpdtp.iLeftMargin

if(format&DT_NOCLIP)
	r.right=r.left; r.bottom=r.top ;;calc from text

CommonTextFunc 1 hdc lpchText cchText r

ret R
