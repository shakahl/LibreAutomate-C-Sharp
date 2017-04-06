out
type T1 byte'x word'y str's
type T2 int'i str's T1'b c double'd long'k

T2 t tt
t.s="''string''[]line2"
t.b.x=200
t.b.y=50000
t.b.s="string2"
 t.b.s=""
 t.b.s="''''"
t.c=-4
 t.d=1.55
t.d=1.5E55
 t.d=5

str s
s.getstruct(t 1)
 s.getstruct(t)
out s

out "---"
 s.ucase
tt.s="ssss"
tt.b.x=5
 s ''string''[]line2
 s=
  b.x 200
  b.y 50000
  b.s   "pp"
  c -4


s.setstruct(tt 1|0)
 s.setstruct(tt)
 out "---"

out tt.s
out tt.b.x
out tt.b.y
_i=tt.b.s.lpstr; out "%s %i" tt.b.s _i
out tt.c
out tt.d
