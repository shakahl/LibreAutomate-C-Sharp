 /
function! hlv ARRAY(int)&a

 Populates a with indices of selected items of listview control.
 Returns 1 if there are selected items, 0 if not.


a=0
int i=-1
rep
	i=SendMessage(hlv LVM_GETNEXTITEM i LVNI_SELECTED)
	if(i<0) break
	a[]=i

ret a.len!0
