 class Ti1 a --b
 class Ti2 :--Ti1 c --d
 class Ti3 :--Ti2 e --f
class Ti1 a -b
class Ti2 :Ti1'_ c -d
class Ti3 :Ti2'base e -f

Ti1 x
Ti2 y
Ti3 z

out x.a
 out x.b
out x.F1

 out y.a
 out y.b
out y.c
 out y.d
 out y.F1
out y.F2

 out z.a
 out z.b
 out z.c
 out z.d
out z.e
 out z.f
 out z.F1
 out z.F2
out z.F3

