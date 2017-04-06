out
type TTR a POINT'p[2] str's[2]
TTR z zz

z.a=7
z.p[0].x=8
z.p[1].y=9
z.s[0]="s0"
z.s[1]="s1"

str s
s.getstruct(z 1)
out s

out "-------"

s.setstruct(zz 1)
out zz.a
out zz.p[0].x
out zz.p[1].y
out zz.s[0]
out zz.s[1]
