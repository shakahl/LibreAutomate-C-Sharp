out
ARRAY(ArrTest) a.create(2 2 3)
a[0 0 0]=0
a[1 0 0]=1
a[0 1 0]=2
a[1 1 0]=3
a[0 0 1]=4
a[1 0 1]=5
a[0 1 1]=6
a[1 1 1]=7
a[0 0 2]=8
a[1 0 2]=9
a[0 1 2]=10
a[1 1 2]=11
a.remove(1)
outb a.psa.pvData a.len(1)*a.len(2)*a.len(3)*sizeof(ArrTest)
