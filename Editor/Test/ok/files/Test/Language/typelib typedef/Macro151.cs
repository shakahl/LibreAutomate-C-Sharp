 tltest.sa a.create(2)
 a[1].i=4
 out a[1].i
 a[1].s="oop"
 out a[1].s

 tltest.sa2 a.Alias.create(2)
 a[1].i=4
 out a[1].i
 a[1].s="oop"
 out a[1].s

 tltest.INTSTR is
 tltest.sa4 a.create(2)
 a[1]=&is
 a[1].i=4
 out a[1].i
 a[1].s="oop"
 out a[1].s
