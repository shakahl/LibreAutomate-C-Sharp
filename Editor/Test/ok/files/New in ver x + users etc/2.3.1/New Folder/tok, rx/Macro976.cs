out
out findrx("abc abc" "(?C)c")

FINDRX x
x.fcallout=&callout2
x.paramc=100
out findrx("abc abc" "(?C)c" x)

ARRAY(str) a
 findrx("abc abc" "(?C)c" x 4 a)
findrx("abc abc" "(?C)c" x 4 a)
int i
for i 0 a.len
	out a[0 i]
	