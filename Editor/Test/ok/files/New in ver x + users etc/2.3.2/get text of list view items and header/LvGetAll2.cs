 /
function! hlv ARRAY(str)&a

int hd=SendMessage(hlv LVM_GETHEADER 0 0)
int nc=SendMessage(hd HDM_GETITEMCOUNT 0 0)
if(!nc) ret
int nr=SendMessage(hlv LVM_GETITEMCOUNT 0 0)
int r c

a.create(nc nr+1)
for r 0 nr
	for c 0 nc
		GetListViewItemText hlv r a[c r+1] c

Acc ah=acc("" "LIST" hd)
for(ah.elem 1 nc+1) a[ah.elem-1 0]=ah.Name

ret 1
