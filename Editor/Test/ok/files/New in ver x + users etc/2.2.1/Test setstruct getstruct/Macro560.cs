out
type T3 !x @y [0]z

T3 t tt
t.x=200
t.y=50000

str s
s.getstruct(t 1)
 s.getstruct(t)
out s

out "---"

s.setstruct(tt 1)
 s.setstruct(tt)
 out "---"

out tt.x
out tt.y
out tt.z
