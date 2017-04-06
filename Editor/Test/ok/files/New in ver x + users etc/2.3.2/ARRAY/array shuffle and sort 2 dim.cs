out
ARRAY(int) a.create(2 10)
int i
for(i 0 a.len) a[0 i]=i; a[1 i]=i*10
for(i 0 a.len) out "%i %i" a[0 i] a[1 i]
out "---"
a.shuffle
for(i 0 a.len) out "%i %i" a[0 i] a[1 i]
out "---"
a.sort(1)
for(i 0 a.len) out "%i %i" a[0 i] a[1 i]
