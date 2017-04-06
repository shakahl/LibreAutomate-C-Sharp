out
class C1 -a b --priv
class C2 :-C1'_ c d
class C3 :C1'_ m n
class C4 :C2'_ o p
class C5 C1'f m n

C1 x
C2 y
C3 z
C4 q
C5 g

 out x.a
out x.b
 out y.a
 out y.b
out y.c
out y.d
 out g.f.a
out g.f.b

x.Test1
y.Test2
z.Test3
q.Test4

