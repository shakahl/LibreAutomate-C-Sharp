 Functions to work with controls in other processes.

 EXAMPLES

out
#compile __Controls
Controls c

int w1=win("" "CabinetWClass"); if(!w1) w1=win("" "ExploreWClass") ;;folder window
int w2=id(1 w1)
int i n=SendMessage(w2 LVM_GETITEMCOUNT 0 0)
str s
ARRAY(str) a.create(n)
for i 0 n
	if(c.ListViewGetItemText(w2 i 0 s)) a[i]=s
out n
for(i 0 n) out a[i]
