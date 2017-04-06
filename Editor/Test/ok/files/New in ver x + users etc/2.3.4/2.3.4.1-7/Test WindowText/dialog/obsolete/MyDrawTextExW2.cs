 /dlg_apihook
function# hdc @*lpchText cchText RECT*lprc format DRAWTEXTPARAMS*lpdtp

 goto gf
if(format&DT_CALCRECT)
	 out "<DT_CALCRECT>"
	 g1
	ret call(fnDrawTextExW hdc lpchText cchText lprc format lpdtp)

int- t_inAPI; if(!t_inAPI) t_inAPI=1; else goto g1

CommonTextFunc 1 hdc lpchText cchText lprc

 gf
 ret call(fnDrawTextExW hdc lpchText cchText lprc format lpdtp)
int g=call(fnDrawTextExW hdc lpchText cchText lprc format lpdtp)
 out "after DrawTextExW"
t_inAPI=0
ret g
