out
ARRAY(ArrTest) a.create(2 3)
a[0 0]=0
a[1 0]=1
a[0 1]=2
a[1 1]=3
a[0 2]=4
a[1 2]=5
a.remove(1)
outb a.psa.pvData a.len(1)*a.len(2)*sizeof(ArrTest)
