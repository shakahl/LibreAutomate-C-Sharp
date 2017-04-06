 /dlg_apihook
function# hdc @*lpchText cchText RECT*lprc format

 goto gf
if(format&DT_CALCRECT)
	 out "<DT_CALCRECT>"
	 g1
	ret call(fnDrawTextW hdc lpchText cchText lprc format)

int- t_inAPI; if(!t_inAPI) t_inAPI=1; else goto g1

CommonTextFunc 40 hdc lpchText cchText lprc

 gf
 ret call(fnDrawTextW hdc lpchText cchText lprc format)
int g=call(fnDrawTextW hdc lpchText cchText lprc format)
 out "after DrawTextW"
t_inAPI=0
ret g
