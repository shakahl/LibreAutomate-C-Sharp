out
ARRAY(int) a.create(10)
int i
for(i 0 a.len) a[i]=i
for(i 0 a.len) out a[i]
out "---"
a.shuffle
for(i 0 a.len) out a[i]
out "---"
a.sort(0 array_sort1)
for(i 0 a.len) out a[i]
