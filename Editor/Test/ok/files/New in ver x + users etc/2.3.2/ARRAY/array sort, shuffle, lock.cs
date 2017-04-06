out
ARRAY(int) a.createlb(10 5)
 a.lock
int i
for(i a.lbound a.ubound+1) a[i]=i
a.shuffle
for(i a.lbound a.ubound+1) out a[i]
out "---"
 a.lock
a.sort(1)
for(i a.lbound a.ubound+1) out a[i]
