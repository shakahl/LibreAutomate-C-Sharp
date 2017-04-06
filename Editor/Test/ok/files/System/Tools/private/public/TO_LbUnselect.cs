 /
function# hlb ifSelectedItem ~unSelectItems [hcb] ;;unSelectItems: "1 2 +3" (+ forces to unselect current).

 Unselects ListBox hlb items unSelectItems if item ifSelectedItem of hlb or hcb (a ComboBox) is selected.

int curr=-1
if hcb
	if(CB_SelectedItem(hcb)!=ifSelectedItem) ret
else
	if(!SendMessage(hlb LB_GETSEL ifSelectedItem 0)) ret
	curr=SendMessage(hlb LB_GETCARETINDEX 0 0)

ARRAY(lpstr) a
tok unSelectItems a -1 " "
int i j
for(i 0 a.len)
	j=val(a[i])
	if(j=curr and a[i][0]!='+') continue
	SendMessage(hlb LB_SETSEL 0 j)
ret 1
