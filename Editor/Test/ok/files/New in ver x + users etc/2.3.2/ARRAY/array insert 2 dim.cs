out
ARRAY(int) a.create(2 3)
int i
for(i 0 a.len) a[0 i]=i; a[1 i]=i+10
a[1 a.insert(2)]=100
for(i 0 a.len) out "%i %i" a[0 i] a[1 i]
