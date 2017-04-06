 tltest._long2 i=8
 out i

 tltest._long3 k
 int* p
 k=&p
 out k

 tltest._INTSTR2 is
 is.i=8
 out is.i

 tltest._INTSTR3 i3
 tltest.INTSTR is
 i3.Alias=&is
 i3.Alias.s="u"
 out i3.Alias.s

 tltest._IDispatch2 i
 i._create

 tltest._IDispatch3 i
 i.Alias._create

 tltest._sa a.create(2)
 a[1].i=9
 out a[1].i

 tltest._sa2
 tltest._sa3
 tltest._sa4
 tltest._sa5

 tltest._SUPER z
 z.a.s="a"
 z.b.s="b"
 out z.a.s
 out z.b.s

 tltest.MyEnum e
 e=8
 out e

