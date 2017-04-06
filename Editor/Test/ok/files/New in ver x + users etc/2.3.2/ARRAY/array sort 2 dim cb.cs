out
ARRAY(double) a.create(3 10)
int i
for(i 0 a.len) a[0 i]=i; a[2 i]=-i
for(i 0 a.len) out "%g %g" a[0 i] a[2 i]
out "---"
a.shuffle
for(i 0 a.len) out "%g %g" a[0 i] a[2 i]
out "---"
a.sort(0 array_sort2)
for(i 0 a.len) out "%g %g" a[0 i] a[2 i]
