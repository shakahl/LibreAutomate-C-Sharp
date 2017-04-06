out
type T1 byte'x word'y str's
type T2 int'i str's T1't double'd long'k byte'b[2]

T2 t tt
t.i=-5
t.s="''string''[]line2"
t.t.x=200
t.t.y=50000
t.t.s="string2"
t.d=1.55
t.k=0x100000000
t.b[0]=3
t.b[1]=4

str s
s.getstruct(t 1)
out s
out "---"
s.setstruct(tt 1)

out tt.i
out tt.s
out tt.t.x
out tt.t.y
out tt.t.s
out tt.d
out tt.k
out tt.b[0]
out tt.b[1]
