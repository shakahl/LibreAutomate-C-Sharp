 /
function hlb ARRAY(str)&a

 Gets text of selected items in a multiple-selection listbox.


int i n=SendMessage(hlb LB_GETSELCOUNT 0 0)
if(n=-1) ;;single-sel control
	a.redim(1)
	if(LB_SelectedItem(hlb a[0])<0) a.redim
else
	a.redim(n)
	if(n)
		ARRAY(int) selitems.create(n)
		SendMessage(hlb LB_GETSELITEMS n &selitems[0])
		for(i 0 n) LB_GetItemText hlb selitems[i] a[i]
