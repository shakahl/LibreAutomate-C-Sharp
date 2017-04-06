 /
function# NMHDR*nh idEdit double'step [double'rangeMin] [double'rangeMax]


NMUPDOWN* nu=+nh
 out "%i %i" nu.iPos nu.iDelta ;; :)
int hDlg=GetParent(nh.hwndFrom)
int buddy=id(idEdit hDlg)
double _d=val(_s.getwintext(buddy) 2)
_d-step*nu.iDelta
if(rangeMin or rangeMax)
	if(_d<rangeMin) _d=rangeMin
	if(_d>rangeMax) _d=rangeMax
_s=_d; _s.setwintext(buddy)
ret DT_Ret(hDlg 1)
