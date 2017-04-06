 int w1=id(1 win("My QM" "CabinetWClass"))
int w1=id(3006 win("" "QM_Editor"))

int i n=SendMessage(w1 LVM_GETITEMCOUNT 0 0)
str s
ARRAY(str) a.create(n)
Q &q
for i 0 n
	 if(GetListViewItemText4(w1 i s)) a[i]=s
	if(GetListViewItemText(w1 i s)) a[i]=s
	
Q &qq
out
outq
out n
 for(i 0 n) out a[i]
