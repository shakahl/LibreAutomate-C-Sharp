type TY :POINT a
TY t tt

t.x=4
t.y=5
t.a=6

str s
out s.getstruct(t 1)

s.setstruct(tt 1)
out tt.x
out tt.y
out tt.a
out tt.POINT.x
