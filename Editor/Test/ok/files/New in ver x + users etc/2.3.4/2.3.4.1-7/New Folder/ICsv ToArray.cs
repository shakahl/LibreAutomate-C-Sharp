out
ICsv x._create
x.FromString("a,1[]b,2[]c,3[]d,4")

ARRAY(str) a
x.ToArray(a)
out a.len(1)
out a.len(2)
int i
for i 0 a.len
	out "%s %s" a[0 i] a[1 i]
	