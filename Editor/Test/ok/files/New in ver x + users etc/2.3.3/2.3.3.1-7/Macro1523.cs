type XXX ARRAY(str)a b
type YYY :ARRAY(str)a !b
 XXX x
 out x[0].beg("")
YYY y
y.create(1)
y[0]="abc"
out y.a[0].beg("a")
out y[0].beg("a")
out y[0]

 ARRAY(str) a
 a[1]
