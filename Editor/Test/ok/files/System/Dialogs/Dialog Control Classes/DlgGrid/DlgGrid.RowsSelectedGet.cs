function# [ARRAY(int)&a]

 Gets indices of selected rows.
 Returns number of selected rows.

 a - variable that receives indices of selected rows. Can be 0 if need only number of selected rows.


if(!&a) ret Send(LVM_GETSELECTEDCOUNT)

a=0
int i=-1
rep
	i=Send(LVM_GETNEXTITEM i LVNI_SELECTED); if(i<0) break
	a[]=i

ret a.len

 info: virtual lv does not support LVNI_CUT
