 /
function hlv nColumns

 Proportionally adjusts widths of first nColumns columns so that
 their sum width will be equal to hlv client area width.
 Use to avoid adding horizontal scrollbar.


int i w ww
double fa
ARRAY(int) a.create(nColumns)
RECT r r2
 g1
for i 0 nColumns
	w=SendMessage(hlv LVM_GETCOLUMNWIDTH i 0)
	a[i]=w
	ww+=w

GetClientRect(hlv &r)
if(!ww or ww=r.right) ret
fa=1.0*r.right/ww
for(i 0 nColumns) SendMessage(hlv LVM_SETCOLUMNWIDTH i a[i]*fa)

 after adjusting possibly removed scrollbar...
int retry
if !retry
	retry=1
	GetClientRect(hlv &r2)
	if(r2.right!=r.right) ww=0; goto g1