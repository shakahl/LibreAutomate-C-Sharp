type T !a b
type TD !a ^b

type T1 !a b [pack1]
type TD1 !a ^b [pack1]

type T2 !a b [pack2]
type TD2 !a ^b [pack2]

type T4 !a b [pack4]
type TD4 !a ^b [pack4]

TD4 x

out "%i %i" sizeof(x) &x.b-&x
