type T1 !a b [pack1]
type T2 T1'a @b [pack1]

T1 x
T2 y

out "%i %i" sizeof(x) &x.b-&x
out "%i %i" sizeof(y) &y.b-&y
