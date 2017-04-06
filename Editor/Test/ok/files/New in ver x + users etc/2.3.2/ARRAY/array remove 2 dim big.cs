out
ARRAY(ArrTest2) a.create(2 3)
a[1 0].b=1
a[0 1].b=2
a[1 1].b=3
a[0 2].b=4
a[1 2].b=5
a.remove(1)
outb a.psa.pvData a.len(1)*a.len(2)*sizeof(ArrTest2)
