out
ARRAY(str) a.create(2 4)
int i
for i 0 a.len
	a[0 i].format("%c" 'A'+i)
	a[1 i]=i+1

ICsv x._create
x.FromArray(a)
str s
x.ToString(s)
out s
