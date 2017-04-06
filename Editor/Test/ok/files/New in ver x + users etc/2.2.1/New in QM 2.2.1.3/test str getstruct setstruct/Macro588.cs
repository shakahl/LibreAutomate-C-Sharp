out
type TTR a[2] POINT'p[2] str's[2]
type TTR2 TTR'a[2]
TTR2 z zz
int i

for i 0 2
	z.a[i].a[0]=2+i
	z.a[i].a[1]=3+i
	z.a[i].p[0].x=8+i
	z.a[i].p[1].y=9+i
	z.a[i].s[0].from("s0" i)
	z.a[i].s[1].from("s1" i)

str s
s.getstruct(z 1)
out s

out "-------"

s.setstruct(zz 1)
for i 0 2
	out zz.a[i].a[0]
	out zz.a[i].a[1]
	out zz.a[i].p[0].x
	out zz.a[i].p[0].y
	out zz.a[i].p[1].x
	out zz.a[i].p[1].y
	out zz.a[i].s[0]
	out zz.a[i].s[1]
