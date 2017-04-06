type T !a b !c
type TD !a ^b !c

type T1 !a b !c [pack1]
type TD1 !a ^b !c [pack1]

type T2 !a b !c [pack2]
type TD2 !a ^b !c [pack2]

type T4 !a b !c [pack4]
type TD4 !a ^b !c [pack4]

TD4 x

out "%i %i %i" sizeof(x) &x.b-&x &x.c-&x
