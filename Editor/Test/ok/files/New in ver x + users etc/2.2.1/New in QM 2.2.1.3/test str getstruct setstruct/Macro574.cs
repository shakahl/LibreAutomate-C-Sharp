out
type STRUCT2 a b
STRUCT z x

z.a=10
z.d=20
z.b[0]=100
z.b[2]=102
strcpy &z.c "Test"
z.s="str"
z.sa[0]="s0"
z.sa[1]="s1[]line 2"

out _s.getstruct(z)
out "---"
out _s.getstruct(z 1)
out "-----------------"

_s.setstruct(x)
out "----"
out x.a
out "%i %i %i %i" x.b[0] x.b[1] x.b[2] x.b[3]
lpstr s=&x.c; out s
out x.s
out x.sa[0]
out x.sa[1]
out x.d
